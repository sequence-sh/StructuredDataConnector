using System.IO;
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

        var root = XmlMethods.ToXmlElement("root", result.Value).AsT0; //Will always be a T0

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
}
