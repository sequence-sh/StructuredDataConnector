using SharpYaml.Serialization;

namespace Reductech.Sequence.Connectors.StructuredData;

/// <summary>
/// Writes an entity to a stream in YAML format
/// </summary>
[Alias("ToYml")]
public sealed class ToYaml : CompoundStep<StringStream>
{
    /// <inheritdoc />
    protected override async ValueTask<Result<StringStream, IError>> Run(
        IStateMonad stateMonad,
        CancellationToken cancellationToken)
    {
        var result = await stateMonad.RunStepsAsync(Entity, cancellationToken);

        if (result.IsFailure)
            return result.ConvertFailure<StringStream>();

        var entity = result.Value;
        var cso    = entity.ToCSharpObject();

        var settings = new SerializerSettings();
        settings.EmitTags = false;

        var serializer = new Serializer(settings);

        var yaml = serializer.Serialize(cso);

        return new StringStream(yaml);
    }

    /// <summary>
    /// The entity to write.
    /// </summary>
    [StepProperty(1)]
    [Required]
    public IStep<Entity> Entity { get; set; } = null!;

    /// <inheritdoc />
    public override IStepFactory StepFactory { get; } =
        new SimpleStepFactory<ToYaml, StringStream>();
}
