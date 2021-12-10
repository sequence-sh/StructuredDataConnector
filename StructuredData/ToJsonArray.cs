using System.Text.Json;
using System.Text.Json.Serialization;
using Reductech.EDR.Core.Entities;
using Reductech.EDR.Core.Internal.Errors;
using Entity = Reductech.EDR.Core.Entity;

namespace Reductech.EDR.Connectors.StructuredData;

/// <summary>
/// Write entities to a stream in Json format.
/// </summary>
public sealed class ToJsonArray : CompoundStep<StringStream>
{
    /// <inheritdoc />
    protected override async Task<Result<StringStream, IError>> Run(
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
    public IStep<bool> FormatOutput { get; set; } = new BoolConstant(true);

    /// <inheritdoc />
    public override IStepFactory StepFactory { get; } =
        new SimpleStepFactory<ToJsonArray, StringStream>();
}
