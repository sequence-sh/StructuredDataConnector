//using SharpYaml;
//using SharpYaml.Events;
//using SharpYaml.Serialization;

//namespace Reductech.Sequence.Connectors.StructuredData;

//public class EntityObjectFactory : IObjectFactory
//{
//    public object Create(Type type)
//    {
//        throw new NotImplementedException();
//    }
//}

///// <inheritdoc />
//public class EntityYamlTypeConverter :
//    YamlTypeConverter
//{
//    private EntityYamlTypeConverter() { }

//    /// <summary>
//    /// The instance
//    /// </summary>
//    public static IYamlTypeConverter Instance { get; } = new EntityYamlTypeConverter();

//    /// <inheritdoc />
//    public bool Accepts(Type type)
//    {
//        var r = type == typeof(Entity);
//        return r;
//    }

//    /// <inheritdoc />
//    public object? ReadYaml(IParser parser, Type type) =>
//        ParseNext(parser, x => false).GetValueOrDefault();

//    private static Maybe<ISCLObject> ParseNext(IParser parser, Func<ParsingEvent?, bool> isClose)
//    {
//        if (!parser.MoveNext())
//            throw new YamlException("Unexpected End of Yaml");

//        if (isClose(parser.Current))
//        {
//            return Maybe<ISCLObject>.None;
//        }

//        switch (parser.Current)
//        {
//            case null: return SCLNull.Instance;
//            case AnchorAlias anchorAlias: throw new Exception("Anchor aliases are not supported");
//            case DocumentEnd d: throw new YamlException("Unexpected Document End");
//            case DocumentStart documentStart: throw new YamlException("Unexpected Document Start");
//            case MappingEnd mappingEnd: throw new YamlException("Unexpected Mapping End");
//            case MappingStart mappingStart:
//            {
//                var properties = new List<EntityProperty>();

//                while (parser.MoveNext())
//                {
//                    if (parser.Current is MappingEnd)
//                        return Maybe<ISCLObject>.From(new Entity(properties));

//                    if (parser.Current is NodeEvent ne)
//                    {
//                        var value = ParseNext(parser, _ => false);

//                        if (value.HasValue)
//                        {
//                            properties.Add(
//                                new EntityProperty(ne.Tag.Value, value.Value, properties.Count)
//                            );
//                        }

//                        throw new YamlException("Expected Value");
//                    }

//                    throw new YamlException("Expected NodeEvent");
//                }

//                throw new YamlException("Unexpected End of Yaml");
//            }
//            case Scalar scalar: return new StringStream(scalar.Value);
//            case SequenceStart sequenceStart:
//            {
//                var list = new List<ISCLObject>();

//                while (true)
//                {
//                    var next = ParseNext(parser, x => x is SequenceStart);

//                    if (next.HasValue)
//                        list.Add(next.Value);
//                    else
//                    {
//                        break;
//                    }
//                }

//                return new EagerArray<ISCLObject>(list);
//            }
//            case NodeEvent nodeEvent:
//            {
//                throw new YamlException("Unexpected Node Event");
//            }
//            case SequenceEnd sequenceEnd: throw new YamlException("Unexpected Sequence End");
//            case StreamEnd streamEnd:     throw new YamlException("Unexpected Stream End");
//            case StreamStart streamStart:
//            {
//                throw new NotImplementedException();
//            }
//            default: throw new ArgumentOutOfRangeException();
//        }
//    }

//    /// <inheritdoc />
//    public void WriteYaml(IEmitter emitter, object? value, Type type)
//    {
//        var sclObject = (ISCLObject)value!;
//    }
//}


