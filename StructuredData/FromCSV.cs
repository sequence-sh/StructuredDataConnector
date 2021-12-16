using Reductech.EDR.Connectors.StructuredData.Util;
using Reductech.EDR.Core.Internal.Errors;
using Entity = Reductech.EDR.Core.Entity;

namespace Reductech.EDR.Connectors.StructuredData;

/// <summary>
/// Extracts entities from a CSV stream.
/// The same as FromConcordance but with different default values.
/// </summary>
[Alias("ConvertCSVToEntity")]
public sealed class FromCSV : CompoundStep<Array<Entity>>
{
    /// <inheritdoc />
    protected override async Task<Result<Array<Entity>, IError>> Run(
        IStateMonad stateMonad,
        CancellationToken cancellationToken)
    {
        var result = await CSVReader.ReadCSV(
            stateMonad,
            Stream,
            Delimiter,
            CommentCharacter,
            QuoteCharacter,
            MultiValueDelimiter,
            new ErrorLocation(this),
            cancellationToken
        );

        return result;
    }

    /// <summary>
    /// Stream containing the CSV data.
    /// </summary>
    [StepProperty(1)]
    [Required]
    public IStep<StringStream> Stream { get; set; } = null!;

    /// <summary>
    /// The delimiter to use to separate fields.
    /// </summary>
    [StepProperty(2)]
    [DefaultValueExplanation(",")]
    [Log(LogOutputLevel.Trace)]
    public IStep<StringStream> Delimiter { get; set; } = new SCLConstant<StringStream>(",");

    /// <summary>
    /// The token to use to indicate comments.
    /// Must be a single character, or an empty string.
    /// If it is empty, then comments cannot be indicated
    /// </summary>
    [StepProperty(3)]
    [DefaultValueExplanation("#")]
    [SingleCharacter]
    [Log(LogOutputLevel.Trace)]
    public IStep<StringStream> CommentCharacter { get; set; } =
        new SCLConstant<StringStream>("#");

    /// <summary>
    /// The quote character to use.
    /// Should be a single character or an empty string.
    /// If it is empty then strings cannot be quoted.
    /// </summary>
    [StepProperty(4)]
    [DefaultValueExplanation("\"")]
    [SingleCharacter]
    [Log(LogOutputLevel.Trace)]
    public IStep<StringStream> QuoteCharacter { get; set; } =
        new SCLConstant<StringStream>("\"");

    /// <summary>
    /// The multi value delimiter character to use.
    /// Should be a single character or an empty string.
    /// If it is empty then fields cannot have multiple fields.
    /// </summary>
    [StepProperty(5)]
    [DefaultValueExplanation("")]
    [SingleCharacter]
    [Log(LogOutputLevel.Trace)]
    public IStep<StringStream> MultiValueDelimiter { get; set; } =
        new SCLConstant<StringStream>("");

    /// <inheritdoc />
    public override IStepFactory StepFactory { get; } =
        new SimpleStepFactory<FromCSV, Array<Entity>>();
}
