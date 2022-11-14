using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;

namespace Sequence.Connectors.StructuredData.Util.IDX;

/// <summary>
/// The Idx parser configuration
/// </summary>
public record IdxParserConfiguration(
    string FieldDelimiter,
    string FieldNameDelimiter,
    string StringDelimiter)
{
    /// <summary>
    /// Default Configuration for IDX parsers
    /// </summary>
    public static readonly IdxParserConfiguration Default = new("#DRE", "=", "\"");
}

/// <summary>
/// Contains methods for parsing IDX
/// </summary>
public record IdxParser(IdxParserConfiguration Config)
{
    private static readonly Regex FieldRegex = new(
        @"(?<name>\w+)(?<number>\d+)",
        RegexOptions.Compiled | RegexOptions.IgnoreCase
    );

    private static EagerArray<ISCLObject> Combine(ISCLObject ev1, ISCLObject ev2)
    {
        if (ev1 is IArray l1)
        {
            if (ev2 is IArray l2)
            {
                return new EagerArray<ISCLObject>(
                    l1.ListIfEvaluated().Value.Concat(l2.ListIfEvaluated().Value).ToList()
                );
            }

            return new EagerArray<ISCLObject>(l1.ListIfEvaluated().Value.Append(ev2).ToList());
        }

        if (ev2 is IArray list2)
        {
            return new EagerArray<ISCLObject>(list2.ListIfEvaluated().Value.Prepend(ev1).ToList());
        }

        return new EagerArray<ISCLObject>(new[] { ev1, ev2 });
    }

    /// <summary>
    /// Parse an IDX string as an entity.
    /// </summary>
    public Result<Entity, IErrorBuilder> TryParseEntity(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return ErrorCode.CouldNotParse.ToErrorBuilder("", "IDX");

        var keys   = ImmutableArray.CreateBuilder<EntityKey>();
        var values = new List<ISCLObject>();

        void AddField(string fieldName, string fieldValue)
        {
            var fieldMatch = FieldRegex.Match(fieldName);
            var newValue   = new StringStream(fieldValue);

            if (fieldMatch.Success)
                fieldName = fieldMatch.Groups["name"].Value;

            var key = new EntityKey(fieldName);

            var index = keys.IndexOf(key);

            if (index < 0)
            {
                keys.Add(new EntityKey(fieldName));
                values.Add(newValue);
            }
            else
            {
                var existingValue = values[index];
                var combinedValue = Combine(existingValue, newValue);

                values[index] = combinedValue;
            }
        }

        foreach (var fieldBlock in input.Split(Config.FieldDelimiter))
        {
            // skip any potential empty lines at the start of the document
            if (string.IsNullOrWhiteSpace(fieldBlock))
                continue;

            var fieldNameEnd = fieldBlock.IndexOfAny(new[] { ' ', '\n' });

            if (fieldNameEnd < 0)
                continue;

            var fieldName  = fieldBlock.Substring(0, fieldNameEnd).TrimEnd('\r');
            var fieldValue = fieldBlock[(fieldNameEnd + 1)..];

            fieldValue = fieldValue.Trim();

            switch (fieldName)
            {
                case "CONTENT":
                    AddField("DRECONTENT", fieldValue);
                    break;
                case "DATE":
                    AddField("DREDATE", fieldValue);
                    break;
                case "DBNAME":
                    AddField("DREDBNAME", fieldValue);
                    break;
                case "REFERENCE":
                    AddField("DREREFERENCE", fieldValue);
                    break;
                case "SECTION":
                    AddField("DRESECTION", fieldValue);
                    break;
                case "TITLE":
                    AddField("DRETITLE", fieldValue);
                    break;
                case "FIELD":
                    var fieldEnd = fieldValue.IndexOf(
                        Config.FieldNameDelimiter,
                        StringComparison.Ordinal
                    );

                    var field = fieldEnd == -1 ? fieldValue : fieldValue.Substring(0, fieldEnd);
                    var value = "";

                    if (fieldEnd != -1)
                    {
                        value = fieldValue[(fieldEnd + 1)..];
                        value = value.Trim();

                        if (value.StartsWith(Config.StringDelimiter))
                            value = value[1..];

                        if (value.EndsWith(Config.StringDelimiter))
                            value = value.Remove(value.Length - 1);
                    }

                    AddField(field, value);
                    break;
                case "ENDDOC":
                    // TODO: Maybe a check if there are any fields AFTER the end tag
                    break;
                default:
                    AddField(fieldName, fieldValue);
                    break;
            }
        }

        var entity = new Entity(keys.ToImmutable(), values.ToImmutableArray());
        return entity;
    }
}
