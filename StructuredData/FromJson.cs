using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace Reductech.Sequence.Connectors.StructuredData;

/// <summary>
/// Extracts the entity from a Json stream containing a single entity.
/// </summary>
public sealed class FromJson : CompoundStep<Entity>
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

        Entity? entity;

        try
        {
            entity = JsonSerializer.Deserialize<Entity>(
                text.Value,
                new JsonSerializerOptions()
                {
                    Converters =
                    {
                        new JsonStringEnumConverter(), VersionJsonConverter.Instance
                    }
                }
            );
        }
        catch (Exception e)
        {
            stateMonad.Log(LogLevel.Error, e.Message, this);
            entity = null;
        }

        if (entity is null)
            return
                Result.Failure<Entity, IError>(
                    ErrorCode.CouldNotParse.ToErrorBuilder(text.Value, "JSON")
                        .WithLocation(this)
                );

        return entity;
    }

    /// <summary>
    /// Stream containing the Json data.
    /// </summary>
    [StepProperty(1)]
    [Required]
    public IStep<StringStream> Stream { get; set; } = null!;

    /// <inheritdoc />
    public override IStepFactory StepFactory { get; } =
        new SimpleStepFactory<FromJson, Entity>();
}
