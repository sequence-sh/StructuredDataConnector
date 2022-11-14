using Microsoft.Extensions.Logging;
using SharpYaml.Serialization;

namespace Sequence.Connectors.StructuredData;

/// <summary>
/// Extracts the entity from a Yaml stream containing a single entity.
/// </summary>
[Alias("FromYml")]
public sealed class FromYaml : CompoundStep<Entity>
{
    /// <inheritdoc />
    protected override async ValueTask<Result<Entity, IError>> Run(
        IStateMonad stateMonad,
        CancellationToken cancellationToken)
    {
        var text = await Stream.Run(stateMonad, cancellationToken)
            .Map(x => x.GetStringAsync());

        if (text.IsFailure)
            return text.ConvertFailure<Entity>();

        Dictionary<string, object>? dictionary;

        try
        {
            var serializer = new Serializer(new SerializerSettings() { });

            dictionary = serializer.Deserialize<Dictionary<string, object>>(text.Value);
        }
        catch (Exception e)
        {
            stateMonad.Log(LogLevel.Error, e.Message, this);
            dictionary = null;
        }

        if (dictionary is null)
            return
                Result.Failure<Entity, IError>(
                    ErrorCode.CouldNotParse.ToErrorBuilder(text.Value, "YAML")
                        .WithLocation(this)
                );

        var entity = Entity.Create(dictionary);

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
        new SimpleStepFactory<FromYaml, Entity>();
}
