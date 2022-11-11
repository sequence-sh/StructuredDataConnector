using System.Collections.Immutable;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;

namespace Sequence.Connectors.StructuredData.Util;

/// <summary>
/// Helps read blocks
/// </summary>
public static class CSVReader
{
    /// <summary>
    /// Reads a CSV stream to an entity stream based on all the input steps.
    /// </summary>
    /// <returns></returns>
    public static async ValueTask<Result<Array<Entity>, IError>> ReadCSV(
        IStateMonad stateMonad,
        IStep<StringStream> stream,
        IStep<StringStream> delimiter,
        IStep<StringStream> commentCharacter,
        IStep<StringStream> quoteCharacter,
        IStep<StringStream> multiValueDelimiter,
        ErrorLocation errorLocation,
        CancellationToken cancellationToken)
    {
        var stuff = await stateMonad.RunStepsAsync(
            stream,
            delimiter.WrapStringStream(),
            commentCharacter.WrapChar(nameof(commentCharacter)),
            quoteCharacter.WrapChar(nameof(quoteCharacter)),
            multiValueDelimiter.WrapChar(nameof(multiValueDelimiter)),
            cancellationToken
        );

        if (stuff.IsFailure)
            return stuff.ConvertFailure<Array<Entity>>();

        var (streamResult, delimiterResult, commentResult, quoteResult, multiValueResult) =
            stuff.Value;

        var asyncEnumerable = ReadCSV(
                streamResult,
                delimiterResult,
                quoteResult,
                commentResult,
                multiValueResult,
                errorLocation
            )
            .ToSCLArray();

        return asyncEnumerable;
    }

    /// <summary>
    /// Creates a block that will produce records from the CSV file.
    /// </summary>
    public static async IAsyncEnumerable<Entity> ReadCSV(
        StringStream stringStream,
        string delimiter,
        char? quoteCharacter,
        char? commentCharacter,
        char? multiValueDelimiter,
        ErrorLocation location)
    {
        var (stream, encodingEnum) = stringStream.GetStream();

        var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter                = delimiter,
            Encoding                 = encodingEnum.Convert(),
            SanitizeForInjection     = false,
            DetectColumnCountChanges = false,
            ReadingExceptionOccurred = HandleException,
            BadDataFound             = BadDataFound
        };

        if (quoteCharacter.HasValue)
        {
            configuration.Quote = quoteCharacter.Value;
            configuration.Mode  = CsvMode.RFC4180;

            configuration.Escape =
                quoteCharacter.Value; //https://github.com/JoshClose/CsvHelper/issues/1659
        }
        else
            configuration.Mode = CsvMode.Escape;

        if (commentCharacter.HasValue)
        {
            configuration.Comment       = commentCharacter.Value;
            configuration.AllowComments = true;
        }
        else
            configuration.AllowComments = false;

        var textReader = new StreamReader(stream, encodingEnum.Convert());

        var reader = new CsvReader(textReader, configuration);

        //reader.ReadHeader();

        //var headerRecord = reader.HeaderRecord;
        var headers = new Lazy<ImmutableArray<EntityKey>>(
            () => reader.HeaderRecord.Select(x => new EntityKey(x)).ToImmutableArray()
        );

        await foreach (var row in reader.GetRecordsAsync<dynamic>())
        {
            ExpandoObject eo = row;

            var values = eo.Select(x => ISCLObject.CreateFromCSharpObject(GetValue(x.Value)))
                .ToImmutableArray();

            var entity = new Entity(headers.Value, values);
            yield return entity;
        }

        reader.Dispose();

        object? GetValue(object? dictValue)
        {
            if (!multiValueDelimiter.HasValue)
                return dictValue;

            var s = dictValue?.ToString();

            var arr = s?.Split(multiValueDelimiter.Value);

            if (arr?.Length == 1)
                return dictValue;

            return arr;
        }

        bool HandleException(ReadingExceptionOccurredArgs args)
        {
            throw new ErrorException(new SingleError(location, args.Exception, ErrorCode.CSVError));
        }

        void BadDataFound(BadDataFoundArgs args)
        {
            throw new ErrorException(
                ErrorCode.Unknown
                    .ToErrorBuilder($"BadData - Field:{args.Field} RawRecord:{args.RawRecord}")
                    .WithLocationSingle(location)
            );
        }
    }
}
