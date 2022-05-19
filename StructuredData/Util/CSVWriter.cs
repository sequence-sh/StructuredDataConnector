using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace Reductech.Sequence.Connectors.StructuredData.Util;

/// <summary>
/// Helper methods for writing CSV files
/// </summary>
public static class CSVWriter
{
    /// <summary>
    /// Writes entities from an entityStream to a stream in csv format.
    /// </summary>
    public static async Task<Result<StringStream, IError>> WriteCSV(
        IStateMonad stateMonad,
        IStep<Array<Entity>> entityStream,
        IStep<StringStream> delimiter,
        IStep<SCLEnum<EncodingEnum>> encoding,
        IStep<StringStream> quoteCharacter,
        IStep<SCLBool> alwaysQuote,
        IStep<StringStream> multiValueDelimiter,
        IStep<StringStream> dateTimeFormat,
        CancellationToken cancellationToken)
    {
        var stuff = await stateMonad.RunStepsAsync(
            entityStream,
            delimiter.WrapStringStream(),
            encoding,
            quoteCharacter.WrapChar(nameof(quoteCharacter)),
            alwaysQuote,
            multiValueDelimiter.WrapChar(nameof(multiValueDelimiter)),
            dateTimeFormat.WrapStringStream(),
            cancellationToken
        );

        if (stuff.IsFailure)
            return stuff.ConvertFailure<StringStream>();

        var (entityStreamResult, delimiterResult, encodingResult, quoteResult, alwaysQuoteResult,
            multiValueDelimiterResult, dateTimeResult) = stuff.Value;

        var result = await WriteCSV(
                entityStreamResult,
                encodingResult.Value.Convert(),
                delimiterResult,
                quoteResult,
                alwaysQuoteResult,
                multiValueDelimiterResult,
                dateTimeResult,
                cancellationToken
            )
            .Map(x => new StringStream(x, encodingResult));

        return result;
    }

    /// <summary>
    /// Writes entities from an entityStream to a stream in csv format.
    /// </summary>
    public static async Task<Result<Stream, IError>> WriteCSV(
        Array<Entity> entityStream,
        Encoding encoding,
        string delimiter,
        char? quoteCharacter,
        bool alwaysQuote,
        char? multiValueDelimiter,
        string dateTimeFormat,
        CancellationToken cancellationToken)
    {
        var results = await entityStream.GetElementsAsync(cancellationToken);

        if (results.IsFailure)
            return results.ConvertFailure<Stream>();

        var stream = new MemoryStream();

        if (!results.Value.Any())
            return stream; //empty stream

        var textWriter = new StreamWriter(stream, encoding);

        var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter                = delimiter,
            Encoding                 = encoding,
            SanitizeForInjection     = false,
            DetectColumnCountChanges = false
        };

        if (quoteCharacter.HasValue)
        {
            configuration.Quote = quoteCharacter.Value;

            if (alwaysQuote)
                configuration.ShouldQuote = _ => true;
        }

        var writer = new CsvWriter(textWriter, configuration);

        var options = new TypeConverterOptions { Formats = new[] { dateTimeFormat } };
        writer.Context.TypeConverterOptionsCache.AddOptions<DateTime>(options);
        writer.Context.TypeConverterOptionsCache.AddOptions<DateTime?>(options);

        var records =
            results.Value.Select(
                x => ConvertToObject(x, multiValueDelimiter ?? '|', dateTimeFormat)
            );

        await writer.WriteRecordsAsync(records, cancellationToken); //TODO pass an async enumerable

        await textWriter.FlushAsync();

        stream.Seek(0, SeekOrigin.Begin);

        return stream;

        static object ConvertToObject(Entity entity, char delimiter, string dateTimeFormat)
        {
            IDictionary<string, object> expandoObject = new ExpandoObject()!;

            foreach (var entityProperty in entity)
            {
                var s = Format(entityProperty.Value, delimiter, dateTimeFormat);

                expandoObject[entityProperty.Name] = s;
            }

            return expandoObject;
        }

        static string Format(ISCLObject obj, char delimiter, string dateTimeFormat)
        {
            if (obj is SCLDateTime dateTime)
                return dateTime.Value.ToString(dateTimeFormat);

            if (obj is IArray array)
                return string.Join(
                    delimiter,
                    array.ListIfEvaluated().Value.Select(x => Format(x, delimiter, dateTimeFormat))
                );

            return obj.Serialize(SerializeOptions.Primitive);
        }
    }
}
