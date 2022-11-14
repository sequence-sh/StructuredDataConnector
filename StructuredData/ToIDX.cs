using Sequence.Connectors.StructuredData.Util.IDX;

namespace Sequence.Connectors.StructuredData;

/// <summary>
/// Write an entity to a stream in IDX format.
/// </summary>
public sealed class ToIDX : CompoundStep<StringStream>
{
    /// <inheritdoc />
    protected override async ValueTask<Result<StringStream, IError>> Run(
        IStateMonad stateMonad,
        CancellationToken cancellationToken)
    {
        //TODO maybe stream the result?
        var entity = await Entity.Run(stateMonad, cancellationToken);

        if (entity.IsFailure)
            return entity.ConvertFailure<StringStream>();

        var toDocumentResult = await ConvertToDocument.Run(stateMonad, cancellationToken);

        if (toDocumentResult.IsFailure)
            return toDocumentResult.ConvertFailure<StringStream>();

        Result<string, IErrorBuilder> result;

        if (toDocumentResult.Value)
            result = entity.Value.TryConvertToIDXDocument(0);
        else
            result = entity.Value.TryConvertToIDXData(0);

        if (result.IsFailure)
            return result.MapError(x => x.WithLocation(this)).ConvertFailure<StringStream>();

        return new StringStream(result.Value);
    }

    /// <summary>
    /// The entity to write
    /// </summary>
    [StepProperty(1)]
    [Required]
    public IStep<Entity> Entity { get; set; } = null!;

    /// <summary>
    /// True to convert to a document.
    /// False to convert to data.
    /// </summary>
    [StepProperty(2)]
    [DefaultValueExplanation("Convert to a document")]
    public IStep<SCLBool> ConvertToDocument { get; set; } = new SCLConstant<SCLBool>(SCLBool.True);

    /// <inheritdoc />
    public override IStepFactory StepFactory { get; } =
        new SimpleStepFactory<ToIDX, StringStream>();
}
