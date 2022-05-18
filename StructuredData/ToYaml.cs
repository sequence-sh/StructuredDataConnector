using System.Text.Json;
using System.Text.Json.Serialization;

namespace Reductech.Sequence.Connectors.StructuredData;

/// <summary>
/// Writes an entity to a stream in YAML format
/// </summary>
[Alias("ToYml")]
public sealed class ToYaml : CompoundStep<StringStream>
{
    /// <inheritdoc />
    protected override async Task<Result<StringStream, IError>> Run(
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
        new SimpleStepFactory<ToYaml, StringStream>();
}
