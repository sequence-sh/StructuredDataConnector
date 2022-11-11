using System.Xml.Linq;

namespace Sequence.Connectors.StructuredData;

/// <summary>
/// Extracts the entity from a Xml stream containing a single entity.
/// </summary>
public sealed class FromXml : CompoundStep<Entity>
{
    /// <inheritdoc />
    protected override async ValueTask<Result<Entity, IError>> Run(
        IStateMonad stateMonad,
        CancellationToken cancellationToken)
    {
        var stream = await Stream.Run(stateMonad, cancellationToken);

        if (stream.IsFailure)
            return stream.ConvertFailure<Entity>();

        XElement element;

        try
        {
            element = await XElement.LoadAsync(
                stream.Value.GetStream().stream,
                LoadOptions.None,
                cancellationToken
            );
        }
        catch
        {
            return
                Result.Failure<Entity, IError>(
                    ErrorCode.CouldNotParse.ToErrorBuilder(stream.Value, "XML")
                        .WithLocation(this)
                );
        }

        var sclObject = XmlMethods.ToSCLObject(element);

        if (sclObject is Entity entity)
            return entity;

        var result = Entity.CreatePrimitive(sclObject);
        return result;
    }

    /// <summary>
    /// Stream containing the Xml data.
    /// </summary>
    [StepProperty(1)]
    [Required]
    public IStep<StringStream> Stream { get; set; } = null!;

    /// <inheritdoc />
    public override IStepFactory StepFactory { get; } =
        new SimpleStepFactory<FromXml, Entity>();
}
