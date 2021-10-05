using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Reductech.EDR.Connectors.StructuredData.Logging;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Attributes;
using Reductech.EDR.Core.Enums;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Internal.Errors;
using Entity = Reductech.EDR.Core.Entity;

namespace Reductech.EDR.Connectors.StructuredData.Singer
{

/// <summary>
/// Extracts the data from a Singer Tap and converts it to entities
/// </summary>
public sealed class FromSinger : CompoundStep<Array<Entity>>
{
    /// <inheritdoc />
    protected override async Task<Result<Array<Entity>, IError>> Run(
        IStateMonad stateMonad,
        CancellationToken cancellationToken)
    {
        var ss = await Stream.Run(stateMonad, cancellationToken);

        if (ss.IsFailure)
        {
            return ss.ConvertFailure<Array<Entity>>();
        }

        var asyncEnumerable = ReadSingerStreamEntities(
            ss.Value,
            this,
            stateMonad,
            cancellationToken
        );

        var lazyArray = new LazyArray<Entity>(asyncEnumerable);

        return lazyArray;
    }

    /// <summary>
    /// Stream containing the Json data.
    /// </summary>
    [StepProperty(1)]
    [Required]
    public IStep<StringStream> Stream { get; set; } = null!;

    /// <inheritdoc />
    public override IStepFactory StepFactory { get; } =
        new SimpleStepFactory<FromSinger, Array<Entity>>();

    private static async IAsyncEnumerable<Entity> ReadSingerStreamEntities(
        StringStream stringStream,
        IStep step,
        IStateMonad stateMonad,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var result in ReadSingerStream(stringStream, cancellationToken))
        {
            if (result.IsFailure)
                throw new ErrorException(result.Error.WithLocation(step));

            if (result.Value is SingerRecord singerRecord)
            {
                yield return Entity.Create(singerRecord.Record);
            }
            else if (result.Value is SingerState singerState)
            {
                LogSituationStructuredData.SingerState.Log(stateMonad, step, singerState.Value);
            }
        }
    }

    private static async IAsyncEnumerable<Result<SingerObject, IErrorBuilder>> ReadSingerStream(
        StringStream stringStream,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var (stream, encoding) = stringStream.GetStream();

        var streamReader = new StreamReader(stream, encoding.Convert());

        while (!streamReader.EndOfStream && !cancellationToken.IsCancellationRequested)
        {
            var line = await streamReader.ReadLineAsync();

            if (!string.IsNullOrWhiteSpace(line))
            {
                var obj = JsonConvert.DeserializeObject<SingerObject>(
                    line,
                    SingerJsonConverter.Instance
                );

                if (obj is null)
                {
                    yield return ErrorCode.CouldNotParse.ToErrorBuilder(line, nameof(SingerObject));
                }
                else
                {
                    yield return obj;
                }
            }
        }
    }
}

/// <summary>
/// Json converter for singer objects
/// </summary>
public class SingerJsonConverter : JsonConverter
{
    private SingerJsonConverter() { }

    /// <summary>
    /// The instance
    /// </summary>
    public static JsonConverter Instance { get; } = new SingerJsonConverter();

    /// <inheritdoc />
    public override bool CanConvert(Type objectType)
    {
        return (objectType == typeof(SingerObject));
    }

    /// <inheritdoc />
    public override object ReadJson(
        JsonReader reader,
        Type objectType,
        object? existingValue,
        JsonSerializer serializer)
    {
        JObject jo   = JObject.Load(reader);
        var     type = jo["type"]?.Value<string>();

        object item = type switch
        {
            "SCHEMA" => new SingerSchema(),
            "RECORD" => new SingerRecord(),
            "STATE"  => new SingerState(),
            _        => throw new Exception($"Singer object has type '{type}'")
        };

        serializer.Populate(jo.CreateReader(), item);
        return item;
    }

    /// <inheritdoc />
    public override bool CanWrite => false;

    /// <inheritdoc />
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        throw new NotImplementedException(); // won't be called because CanWrite returns false
    }
}

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable 8618
/// <summary>
/// A singer Record, Schema, or State
/// </summary>
public abstract class SingerObject { }

/// <summary>
/// A singer Record
/// </summary>
public sealed class SingerRecord : SingerObject
{
    [JsonProperty("stream")] public string Stream { get; set; }

    [JsonProperty("record")] public JObject Record { get; set; }

    [JsonProperty("time_extracted")] public DateTime TimeExtracted { get; set; }
}

/// <summary>
/// A singer Schema
/// </summary>
public sealed class SingerSchema : SingerObject
{
    [JsonProperty("stream")] public string Stream { get; set; }

    [JsonProperty("key_properties")] public string[] KeyProperties { get; set; }

    [JsonProperty("bookmark_properties")] public string[] BookmarkProperties { get; set; }

    [JsonProperty("properties")] public SingerSchemaProperty[] Properties { get; set; }
}

/// <summary>
/// A schema property
/// </summary>
public sealed class SingerSchemaProperty : SingerObject
{
    [JsonProperty("type")] public string Type { get; set; }
    [JsonProperty("format")] public string Format { get; set; }
}

public sealed class SingerState : SingerObject
{
    [JsonProperty("value")] public JObject Value { get; set; }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning restore 8618

}
