using System.Text.Json;
using System.Text.Json.Serialization;

namespace Reductech.Sequence.Connectors.StructuredData;

/// <summary>
/// Write entities to a stream in Json format.
/// </summary>
public sealed class ToJsonArray : CompoundStep<StringStream>
{
    /// <inheritdoc />
    protected override async ValueTask<Result<StringStream, IError>> Run(
        IStateMonad stateMonad,
        CancellationToken cancellationToken)
    {
        var result = await stateMonad.RunStepsAsync(
            Entities.WrapArray(),
            FormatOutput,
            cancellationToken
        );

        if (result.IsFailure)
            return result.ConvertFailure<StringStream>();

        var (list, writeIndented) = result.Value;

        var jsonString = JsonSerializer.Serialize(
            list,
            new JsonSerializerOptions()
            {
                Converters    = { new JsonStringEnumConverter(), VersionJsonConverter.Instance },
                WriteIndented = writeIndented
            }
        );

        return new StringStream(jsonString);
    }

    /// <summary>
    /// The entities to write.
    /// </summary>
    [StepProperty(1)]
    [Required]
    public IStep<Array<Entity>> Entities { get; set; } = null!;

    /// <summary>
    /// Whether to format to the Json output
    /// </summary>
    [StepProperty(2)]
    [DefaultValueExplanation("true")]
    public IStep<SCLBool> FormatOutput { get; set; } = new SCLConstant<SCLBool>(SCLBool.True);

    /// <inheritdoc />
    public override IStepFactory StepFactory { get; } =
        new SimpleStepFactory<ToJsonArray, StringStream>();
}
