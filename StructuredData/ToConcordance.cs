namespace Reductech.Sequence.Connectors.StructuredData;

/// <summary>
/// Write entities to a stream in concordance format.
/// The same as ToCSV but with different default values.
/// </summary>
[Alias("ConvertEntityToConcordance")]
public sealed class ToConcordance : CompoundStep<StringStream>
{
    /// <inheritdoc />
    protected override async Task<Result<StringStream, IError>> Run(
        IStateMonad stateMonad,
        CancellationToken cancellationToken)
    {
        var result = await CSVWriter.WriteCSV(
            stateMonad,
            Entities,
            Delimiter,
            Encoding,
            QuoteCharacter,
            AlwaysQuote,
            MultiValueDelimiter,
            DateTimeFormat,
            cancellationToken
        );

        return result;
    }

    /// <inheritdoc />
    public override IStepFactory StepFactory { get; } =
        new SimpleStepFactory<ToConcordance, StringStream>();

    /// <summary>
    /// The entities to write.
    /// </summary>
    [StepProperty(1)]
    [Required]
    public IStep<Array<Entity>> Entities { get; set; } = null!;

    /// <summary>
    /// How the stream is encoded.
    /// </summary>
    [StepProperty(2)]
    [DefaultValueExplanation("UTF8 no BOM")]
    public IStep<SCLEnum<EncodingEnum>> Encoding { get; set; } =
        new SCLConstant<SCLEnum<EncodingEnum>>(new SCLEnum<EncodingEnum>(EncodingEnum.UTF8));

    /// <summary>
    /// The delimiter to use to separate fields.
    /// </summary>
    [StepProperty(3)]
    [DefaultValueExplanation("\u0014")]
    [Log(LogOutputLevel.Trace)]
    public IStep<StringStream> Delimiter { get; set; } =
        new SCLConstant<StringStream>("\u0014");

    /// <summary>
    /// The quote character to use.
    /// Should be a single character or an empty string.
    /// If it is empty then strings cannot be quoted.
    /// </summary>
    [StepProperty(4)]
    [DefaultValueExplanation("\u00FE")]
    [SingleCharacter]
    [Log(LogOutputLevel.Trace)]
    public IStep<StringStream> QuoteCharacter { get; set; } =
        new SCLConstant<StringStream>("\u00FE");

    /// <summary>
    /// Whether to always quote all fields and headers.
    /// </summary>
    [StepProperty(5)]
    [DefaultValueExplanation("false")]
    public IStep<SCLBool> AlwaysQuote { get; set; } = new SCLConstant<SCLBool>(SCLBool.True);

    /// <summary>
    /// The multi value delimiter character to use.
    /// Should be a single character or an empty string.
    /// If it is empty then fields cannot have multiple fields.
    /// </summary>
    [StepProperty(6)]
    [DefaultValueExplanation("")]
    [SingleCharacter]
    [Log(LogOutputLevel.Trace)]
    public IStep<StringStream> MultiValueDelimiter { get; set; } =
        new SCLConstant<StringStream>("|");

    /// <summary>
    /// The format to use for DateTime fields.
    /// </summary>
    [StepProperty(7)]
    [DefaultValueExplanation("O - ISO 8601 compliant - e.g. 2009-06-15T13:45:30.0000000-07:00")]
    [Example("yyyy/MM/dd HH:mm:ss")]
    [Log(LogOutputLevel.Trace)]
    public IStep<StringStream> DateTimeFormat { get; set; } =
        new SCLConstant<StringStream>("O");
}
