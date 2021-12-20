using System.Linq;
using System.Text;
using Reductech.Sequence.Core.Internal.Errors;
using Entity = Reductech.Sequence.Core.Entity;

namespace Reductech.Sequence.Connectors.StructuredData.Util.IDX;

/// <summary>
/// Contains methods for serializing entities to IDX
/// </summary>
public static class IDXSerializer
{
    /// <summary>
    /// Convert this entity to an IDX Document.
    /// Will fail if the entity contains nested entities or doubly nested lists.
    /// </summary>
    public static Result<string, IErrorBuilder> TryConvertToIDXDocument(this Entity entity)
    {
        var stringBuilder = new StringBuilder();

        var r = TryAppendValues(entity, stringBuilder);

        if (r.IsFailure)
            return r.ConvertFailure<string>();

        stringBuilder.AppendLine($"#{DREEndDoc}");
        stringBuilder.AppendLine($"#{DREEndDataReference}");

        return stringBuilder.ToString().Trim();
    }

    /// <summary>
    /// Convert this entity to IDX Data.
    /// Will fail if the entity contains nested entities or doubly nested lists.
    /// </summary>
    public static Result<string, IErrorBuilder> TryConvertToIDXData(this Entity entity)
    {
        var stringBuilder = new StringBuilder();

        var r = TryAppendValues(entity, stringBuilder);

        if (r.IsFailure)
            return r.ConvertFailure<string>();

        stringBuilder.AppendLine($"#{DREEndData}");

        return stringBuilder.ToString().Trim();
    }

    private static Result<Unit, IErrorBuilder> TryAppendValues(
        Entity entity,
        StringBuilder stringBuilder)
    {
        foreach (var dreField in OrderedDREFields)
        {
            var value = entity.TryGetValue(dreField.Name);

            if (value.HasValue)
            {
                var v = TryAppendValue(
                    stringBuilder,
                    value.Value,
                    dreField
                );

                if (v.IsFailure)
                    return v;
            }
            else if (dreField.Mandatory)
                return ErrorCode.SchemaViolation.ToErrorBuilder(
                    dreField.Name,
                    entity
                );
        }

        foreach (var entityProperty in entity)
        {
            if (!DREFieldsSet.Contains(entityProperty.Name))
            {
                var fieldValue = new SpecialField(
                    $"{DREField} {entityProperty.Name}",
                    StringHandling.Quote,
                    false,
                    true,
                    true
                );

                var v = TryAppendValue(
                    stringBuilder,
                    entityProperty.Value,
                    fieldValue
                );

                if (v.IsFailure)
                    return v;
            }
        }

        return Unit.Default;
    }

    private static Result<Unit, IErrorBuilder> TryAppendValue(
        StringBuilder sb,
        ISCLObject entityValue,
        SpecialField field)
    {
        if (entityValue is SCLNull)
            return Unit.Default;

        void AppendString(string s1) //TODO escape value
        {
            if (s1.Contains('\n') || field.StringHandling == StringHandling.Paragraph)
            {
                sb.AppendLine("#" + field.Name);
                sb.AppendLine(s1);
            }
            else
            {
                var maybeQuotedString =
                    field.StringHandling == StringHandling.Quote ? $"\"{s1}\"" : s1;

                AppendField(maybeQuotedString);
            }
        }

        void AppendField(string s2)
        {
            var equalsMaybe = field.UseEquals ? "=" : "";
            var line        = $"#{field.Name}{equalsMaybe} {s2}";
            sb.AppendLine(line);
        }

        if (entityValue is StringStream s)
        {
            AppendString(s.GetString());
        }
        else if (entityValue is SCLDateTime date)
        {
            AppendField(date.Value.ToString("yyyy/MM/dd"));
            return Unit.Default;
        }

        else if (entityValue is Entity)
            return ErrorCode.CannotConvertNestedEntity.ToErrorBuilder("IDX");

        else if (entityValue is IArray array)
        {
            if (!field.AllowList)
                return ErrorCode.CannotConvertNestedList.ToErrorBuilder("IDX");

            var list = array.ListIfEvaluated().Value;

            for (var j = 0; j < list.Count; j++)
            {
                var member       = list[j];
                var newFieldName = $"{field.Name}{j + 1}";
                TryAppendValue(sb, member, field with { AllowList = false, Name = newFieldName });
            }
        }
        else
        {
            AppendField(entityValue.Serialize(SerializeOptions.Primitive));
        }

        return Unit.Default;
    }

    private enum StringHandling
    {
        Quote,
        Paragraph,
        InlineUnquoted
    }

    private record SpecialField(
        string Name,
        StringHandling StringHandling,
        bool Mandatory,
        bool AllowList,
        bool UseEquals);

    private const string DREEndData = "DREENDDATA";

    private const string DREField = "DREFIELD";
    private const string DREEndDoc = "DREENDDOC";
    private const string DREEndDataReference = "DREENDDATAREFERENCE";

    private static readonly IReadOnlyList<SpecialField> OrderedDREFields = new List<SpecialField>()
    {
        // ReSharper disable StringLiteralTypo
        new("DREREFERENCE", StringHandling.InlineUnquoted, true, false, false),
        new("DREDATE", StringHandling.InlineUnquoted, false, false, false),
        new("DRETITLE", StringHandling.Paragraph, false, false, false),
        new("DRECONTENT", StringHandling.Paragraph, false, false, false),
        new("DREDBNAME", StringHandling.InlineUnquoted, false, false, false)
        // ReSharper restore StringLiteralTypo
    };

    private static readonly IReadOnlyCollection<string> DREFieldsSet =
        OrderedDREFields.Select(x => x.Name).ToHashSet();
}
