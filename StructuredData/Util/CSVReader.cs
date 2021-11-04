using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using CsvHelper;
using CsvHelper.Configuration;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Enums;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Internal.Errors;
using Reductech.EDR.Core.Util;
using Entity = Reductech.EDR.Core.Entity;

namespace Reductech.EDR.Connectors.StructuredData.Util
{

/// <summary>
/// Helps read blocks
/// </summary>
public static class CSVReader
{
    private record CharMap : IStepValueMap<StringStream, char?>
    {
        public string PropertyName { get; }
        public ErrorLocation ErrorLocation { get; }

        public CharMap(string propertyName, ErrorLocation errorLocation)
        {
            PropertyName  = propertyName;
            ErrorLocation = errorLocation;
        }

        /// <inheritdoc />
        public async Task<Result<char?, IError>> Map(
            StringStream t,
            CancellationToken cancellationToken)
        {
            var stringResult = await t.GetStringAsync();

            char? resultChar;

            if (stringResult.Length == 0)
                resultChar = null;
            else if (stringResult.Length == 1)
                resultChar = stringResult.Single();
            else
                return new SingleError(
                    ErrorLocation,
                    ErrorCode.SingleCharacterExpected,
                    PropertyName,
                    stringResult
                );

            return resultChar;
        }
    }

    private static IRunnableStep<char?> WrapChar(IStep<StringStream> step, string name) =>
        step.WrapStep(new CharMap(name, step.TextLocation!));

    /// <summary>
    /// Reads a CSV stream to an entity stream based on all the input steps.
    /// </summary>
    /// <returns></returns>
    public static async Task<Result<Array<Entity>, IError>> ReadCSV(
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
            WrapChar(commentCharacter,    nameof(commentCharacter)),
            WrapChar(quoteCharacter,      nameof(quoteCharacter)),
            WrapChar(multiValueDelimiter, nameof(multiValueDelimiter)),
            cancellationToken
        );

        if (stuff.IsFailure)
            return stuff.ConvertFailure<Array<Entity>>();

        var (streamResult, delimiterResult, commentResult, quoteResult, multiValueDelimiterResult) =
            stuff.Value;

        var asyncEnumerable = ReadCSV(
                streamResult,
                delimiterResult,
                quoteResult,
                commentResult,
                multiValueDelimiterResult,
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

        await foreach (var row in reader.GetRecordsAsync<dynamic>())
        {
            var dict = row as IDictionary<string, object>;

            IEnumerable<(EntityPropertyKey, object?)> values =
                dict!.Select(x => (new EntityPropertyKey(x.Key), x.Value))!;

            var entity = Entity.Create(values);
            yield return entity;
        }

        reader.Dispose();

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

}
