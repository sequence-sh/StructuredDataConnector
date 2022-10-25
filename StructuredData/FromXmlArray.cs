using System.Linq;
using System.Xml.Linq;

namespace Reductech.Sequence.Connectors.StructuredData;

/// <summary>
/// Extracts entities from a Xml stream containing an array of entities.
/// </summary>
public sealed class FromXmlArray : CompoundStep<Array<Entity>>
{
    /// <inheritdoc />
    protected override async ValueTask<Result<Array<Entity>, IError>> Run(
        IStateMonad stateMonad,
        CancellationToken cancellationToken)
    {
        var stream = await Stream.Run(stateMonad, cancellationToken);

        if (stream.IsFailure)
            return stream.ConvertFailure<Array<Entity>>();

        XElement element;

        try
        {
            element = await XElement.LoadAsync(
                stream.Value.GetStream().stream,
                LoadOptions.None,
                cancellationToken
            );
        }
        catch
        {
            return
                Result.Failure<Array<Entity>, IError>(
                    ErrorCode.CouldNotParse.ToErrorBuilder(stream.Value, "XML")
                        .WithLocation(this)
                );
        }

        var sclObject = XmlMethods.ToSCLObject(element);

        if (sclObject is Entity entity)
        {
            if (entity.Headers.Length == 1
             && entity.Values.Single() is IArray array)
            {
                var array2 = array.ListIfEvaluated()
                    .Value
                    .Select(x => x is Entity e ? e : Entity.CreatePrimitive(x))
                    .ToSCLArray();

                return array2;
            }

            return new EagerArray<Entity>(new[] { entity });
        }

        var result = Entity.CreatePrimitive(sclObject);
        return new EagerArray<Entity>(new[] { result });
    }

    /// <summary>
    /// Stream containing the Json data.
    /// </summary>
    [StepProperty(1)]
    [Required]
    public IStep<StringStream> Stream { get; set; } = null!;

    /// <inheritdoc />
    public override IStepFactory StepFactory { get; } =
        new SimpleStepFactory<FromXmlArray, Array<Entity>>();
}
