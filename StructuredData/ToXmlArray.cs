using System.IO;
using System.Xml.Linq;

namespace Sequence.Connectors.StructuredData;

/// <summary>
/// Write entities to a stream in Xml format.
/// </summary>
public sealed class ToXmlArray : CompoundStep<StringStream>
{
    /// <inheritdoc />
    protected override async ValueTask<Result<StringStream, IError>> Run(
        IStateMonad stateMonad,
        CancellationToken cancellationToken)
    {
        var result = await stateMonad.RunStepsAsync(Entities, cancellationToken);

        if (result.IsFailure)
            return result.ConvertFailure<StringStream>();

        var xElements = XmlMethods.ToXmlElement("entity", result.Value).AsT1; //Will always be a T1

        var ms = new MemoryStream();

        var root = new XElement("root");

        foreach (var xElement in xElements)
        {
            root.Add(xElement);
        }

        await root.SaveAsync(ms, SaveOptions.None, cancellationToken);

        return new StringStream(ms, EncodingEnum.UTF8);
    }

    /// <summary>
    /// The entities to write.
    /// </summary>
    [StepProperty(1)]
    [Required]
    public IStep<Array<Entity>> Entities { get; set; } = null!;

    /// <inheritdoc />
    public override IStepFactory StepFactory { get; } =
        new SimpleStepFactory<ToXmlArray, StringStream>();
}
