using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sequence.Connectors.StructuredData;

/// <summary>
/// Writes an entity to a stream in JSON format
/// </summary>
public sealed class ToJson : CompoundStep<StringStream>
{
    /// <inheritdoc />
    protected override async ValueTask<Result<StringStream, IError>> Run(
        IStateMonad stateMonad,
        CancellationToken cancellationToken)
    {
        var result = await stateMonad.RunStepsAsync(Entity, FormatOutput, cancellationToken);

        if (result.IsFailure)
            return result.ConvertFailure<StringStream>();

        var (entity, writeIndented) = result.Value;

        var jsonString = JsonSerializer.Serialize(
            entity,
            new JsonSerializerOptions()
            {
                Converters    = { new JsonStringEnumConverter(), VersionJsonConverter.Instance },
                WriteIndented = writeIndented
            }
        );

        return new StringStream(jsonString);
    }

    /// <summary>
    /// The entity to write.
    /// </summary>
    [StepProperty(1)]
    [Required]
    public IStep<Entity> Entity { get; set; } = null!;

    /// <summary>
    /// Whether to indent to the Json output
    /// </summary>
    [StepProperty(2)]
    [DefaultValueExplanation("true")]
    public IStep<SCLBool> FormatOutput { get; set; } = new SCLConstant<SCLBool>(SCLBool.True);

    /// <inheritdoc />
    public override IStepFactory StepFactory { get; } =
        new SimpleStepFactory<ToJson, StringStream>();
}
