using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace Reductech.Sequence.Connectors.StructuredData;

/// <summary>
/// Writes an entity to a stream in XML format
/// </summary>
public sealed class ToXml : CompoundStep<StringStream>
{
    /// <inheritdoc />
    protected override async Task<Result<StringStream, IError>> Run(
        IStateMonad stateMonad,
        CancellationToken cancellationToken)
    {
        var result = await stateMonad.RunStepsAsync(Entity, cancellationToken);

        if (result.IsFailure)
            return result.ConvertFailure<StringStream>();

        var root = ToXmlElement("root", result.Value);

        var ms = new MemoryStream();

        await root.SaveAsync(ms, SaveOptions.None, cancellationToken);

        return new StringStream(ms, EncodingEnum.UTF8);
    }

    /// <summary>
    /// The entity to write.
    /// </summary>
    [StepProperty(1)]
    [Required]
    public IStep<Entity> Entity { get; set; } = null!;

    /// <inheritdoc />
    public override IStepFactory StepFactory { get; } =
        new SimpleStepFactory<ToXml, StringStream>();

    private static XElement ToXmlElement(string name, ISCLObject obj)
    {
        switch (obj)
        {
            case Entity entity:
            {
                var parent = new XElement(name);

                foreach (var ep in entity)
                {
                    var child = ToXmlElement(ep.Name, ep.Value);
                    parent.Add(child);
                }

                return parent;
            }
            case ISCLOneOf oneOf: return ToXmlElement(name, oneOf.Value);
            case Unit:            return new XElement(name);
            case SCLNull:         return new XElement(name);

            case IArray array1:
            {
                var parent = new XElement(name);

                var l = array1.ListIfEvaluated();

                if (l.HasValue)
                {
                    foreach (var ep in l.Value)
                    {
                        var child = ToXmlElement("Element", ep);
                        parent.Add(child);
                    }
                }
                else
                {
                    throw new XmlException("Could not serialize Lazy Array");
                }

                return parent;
            }
            default:
            {
                var v       = obj.ToCSharpObject();
                var element = new XElement(name, v);

                return element;
            }
        }
    }
}
