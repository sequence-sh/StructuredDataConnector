using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Reductech.EDR.Core.Entities;
using Reductech.EDR.Core.Internal.Errors;
using Entity = Reductech.EDR.Core.Entity;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Reductech.EDR.Connectors.StructuredData;

/// <summary>
/// Extracts entities from a Json stream containing an array of entities.
/// </summary>
public sealed class FromJsonArray : CompoundStep<Array<Entity>>
{
    /// <inheritdoc />
    protected override async Task<Result<Array<Entity>, IError>> Run(
        IStateMonad stateMonad,
        CancellationToken cancellationToken)
    {
        var text = await Stream.Run(stateMonad, cancellationToken)
            .Map(x => x.GetStringAsync());

        if (text.IsFailure)
            return text.ConvertFailure<Array<Entity>>();

        List<Entity>? entities;

        try
        {
            entities = JsonSerializer.Deserialize<List<Entity>>(
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
            entities = null;
        }

        if (entities is null)
            return
                Result.Failure<Array<Entity>, IError>(
                    ErrorCode.CouldNotParse.ToErrorBuilder(text.Value, "JSON")
                        .WithLocation(this)
                );

        return entities.ToSCLArray();
    }

    /// <summary>
    /// Stream containing the Json data.
    /// </summary>
    [StepProperty(1)]
    [Required]
    public IStep<StringStream> Stream { get; set; } = null!;

    /// <inheritdoc />
    public override IStepFactory StepFactory { get; } =
        new SimpleStepFactory<FromJsonArray, Array<Entity>>();
}
