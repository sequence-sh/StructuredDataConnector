using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Attributes;
using Reductech.EDR.Core.Entities;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Internal.Errors;
using Reductech.EDR.Core.Util;
using Entity = Reductech.EDR.Core.Entity;

namespace Reductech.EDR.Connectors.StructuredData
{

/// <summary>
/// Writes an entity to a stream in JSON format
/// </summary>
public sealed class ToJson : CompoundStep<StringStream>
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
    public IStep<bool> FormatOutput { get; set; } = new BoolConstant(true);

    /// <inheritdoc />
    public override IStepFactory StepFactory { get; } =
        new SimpleStepFactory<ToJson, StringStream>();
}

}
