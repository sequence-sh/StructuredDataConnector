using Reductech.Sequence.Connectors.StructuredData.Util.IDX;
using Reductech.Sequence.Core.Internal.Errors;
using Entity = Reductech.Sequence.Core.Entity;

namespace Reductech.Sequence.Connectors.StructuredData;

/// <summary>
/// Create an entity from an IDX Stream
/// </summary>
public sealed class FromIDX : CompoundStep<Entity>
{
    /// <inheritdoc />
    protected override async Task<Result<Entity, IError>> Run(
        IStateMonad stateMonad,
        CancellationToken cancellationToken)
    {
        var text = await Stream.Run(stateMonad, cancellationToken)
            .Map(x => x.GetStringAsync());

        if (text.IsFailure)
            return text.ConvertFailure<Entity>();

        var parser = new IdxParser(IdxParserConfiguration.Default);

        var parseResult = parser.TryParseEntity(text.Value);

        if (parseResult.IsFailure)
            return parseResult.ConvertFailure<Entity>().MapError(x => x.WithLocation(this));

        return parseResult.Value;
    }

    /// <summary>
    /// Stream containing the Json data.
    /// </summary>
    [StepProperty(1)]
    [Required]
    public IStep<StringStream> Stream { get; set; } = null!;

    /// <inheritdoc />
    public override IStepFactory StepFactory { get; } = new SimpleStepFactory<FromIDX, Entity>();
}
