namespace Reductech.Sequence.Connectors.StructuredData;

/// <summary>
/// Extracts the entity from a Xml stream containing a single entity.
/// </summary>
public sealed class FromXml : CompoundStep<Entity>
{
    /// <inheritdoc />
    protected override async Task<Result<Entity, IError>> Run(
        IStateMonad stateMonad,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
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
