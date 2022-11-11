using System.Threading;
using System.Threading.Tasks;
using AutoTheory;
using Divergic.Logging.Xunit;
using Json.Schema;
using Sequence.Core.Abstractions;
using Sequence.Core.Entities;
using Sequence.Core.Enums;
using Sequence.Core.Steps;
using Xunit;
using static Sequence.Core.TestHarness.SchemaHelpers;
using static Sequence.Connectors.StructuredData.Tests.SchemaExamples.SchemaHelpers;

namespace Sequence.Connectors.StructuredData.Tests.SchemaExamples;

[UseTestOutputHelper]
public partial class UssecSchemaExamples
{
    [Fact]
    public void USSecSchemaShouldMatchText()
    {
        var formatText = USSecSchema.ConvertToEntity().Format();

        TestOutputHelper.WriteLine(formatText);
    }

    [Fact]
    public async Task ValidateShouldWork()
    {
        var ussecMap = Entity.Create(
            ("BEGINBATES", "FIRSTBATES"),
            ("ENDBATES", "LASTBATES"),
            ("ATTACH_DOCID", "ATTACHRANGE"),
            ("BEGINGROUP", "BEGATTACH"),
            ("ENDGROUP", "ENDATTACH"),
            ("PARENT_DOCID", "PARENT_BATES"),
            ("Custodian", "CUSTODIAN"),
            ("From", "FROM"),
            ("To", "TO"),
            ("Cc", "CC"),
            ("Bcc", "BCC"),
            ("Subject", "SUBJECT"),
            ("Name", "FILE_NAME"),
            ("ITEMPATH", "LINK"),
            ("File Type", "MIME_TYPE"),
            ("File Extension (Original)", "FILE_EXTEN"),
            ("Author", "AUTHOR"),
            ("Last Author", "LAST_AUTHOR"),
            ("File Created", "DATE_CREATED"),
            ("File Created", "TIME_CREATED/TIME_ZONE"),
            ("File Modified", "DATE_MOD"),
            ("File Modified", "TIME_MOD/TIME_ZONE"),
            ("File Accessed", "DATE_ACCESSD"),
            ("File Accessed", "TIME_ACCESSD/TIME_ZONE"),
            ("Last Printed", "PRINTED_DATE"),
            ("File Size", "FILE_SIZE"),
            ("PAGECOUNT", "PGCOUNT"),
            ("Path Name", "PATH"),
            ("GUID", "INTMSGID"),
            ("MD5 Digest", "MD5HASH"),
            ("TEXTPATH", "OCRPATH")
        );

        var data = Examples.LoadFile;

        var step = new ForEach<Entity>
        {
            Action = new LambdaFunction<Entity, Unit>(
                null,
                new Log { Value = new GetAutomaticVariable<Entity>() }
            ),
            Array = new Validate
            {
                Schema = Constant(USSecSchema.ConvertToEntity()),
                EntityStream = new EntityMapProperties
                {
                    EntityStream = new ArrayMap<Entity, Entity>
                    {
                        Array = new FromConcordance { Stream = Constant(data) },
                        Function = new LambdaFunction<Entity, Entity>(
                            null,
                            new EntitySetValue<SCLInt>
                            {
                                Value =
                                    new If<SCLInt>
                                    {
                                        Condition = new Not
                                        {
                                            Boolean = new StringIsEmpty
                                            {
                                                String = new EntityGetValue<StringStream>
                                                {
                                                    Entity   = GetEntityVariable,
                                                    Property = Constant("File Size")
                                                }
                                            }
                                        },
                                        Then = new StringToInt
                                        {
                                            Integer = new StringInterpolate
                                            {
                                                Strings = new List<IStep>
                                                {
                                                    new DoubleProduct
                                                    {
                                                        Terms = new ArrayNew<SCLDouble>
                                                        {
                                                            Elements =
                                                                new List<IStep<SCLDouble>>
                                                                {
                                                                    new StringToDouble
                                                                    {
                                                                        Double =
                                                                            new
                                                                                StringReplace
                                                                                {
                                                                                    String =
                                                                                        new
                                                                                            StringReplace
                                                                                            {
                                                                                                String =
                                                                                                    new
                                                                                                        EntityGetValue
                                                                                                        <StringStream>
                                                                                                        {
                                                                                                            Entity =
                                                                                                                GetEntityVariable,
                                                                                                            Property =
                                                                                                                Constant(
                                                                                                                    "File Size"
                                                                                                                ),
                                                                                                        },
                                                                                                Pattern =
                                                                                                    Constant(
                                                                                                        "\\s*kb\\s*"
                                                                                                    ),
                                                                                                IgnoreCase =
                                                                                                    Constant(
                                                                                                        true
                                                                                                    ),
                                                                                                Replace =
                                                                                                    Constant(
                                                                                                        ""
                                                                                                    )
                                                                                            },
                                                                                    Pattern =
                                                                                        Constant(
                                                                                            "null"
                                                                                        ),
                                                                                    IgnoreCase =
                                                                                        Constant(
                                                                                            true
                                                                                        ),
                                                                                    Replace =
                                                                                        Constant(
                                                                                            "0"
                                                                                        )
                                                                                }
                                                                    },
                                                                    Constant(1000d)
                                                                }
                                                        }
                                                    }
                                                }
                                            }
                                        },
                                        Else = Constant(0)
                                    },
                                Entity   = GetEntityVariable,
                                Property = Constant("File Size"),
                            }
                        )
                    },
                    Mappings = Constant(ussecMap)
                },
                ErrorBehavior = Constant(ErrorBehavior.Error)
            }
        };

        var serializedStep = step.Serialize(SerializeOptions.Serialize);

        TestOutputHelper.WriteLine(serializedStep);

        var stepFactoryStore = StepFactoryStore.TryCreateFromAssemblies(
                ExternalContext.Default,
                typeof(FromConcordance).Assembly
            )
            .GetOrThrow();

        var sm = new StateMonad(
            new TestOutputLogger("Logger", TestOutputHelper),
            stepFactoryStore,
            ExternalContext.Default,
            new Dictionary<string, object>()
        );

        var result = await step.Run<Unit>(sm, CancellationToken.None);

        result.ShouldBeSuccessful();
    }

    public static readonly JsonSchema USSecSchema = new JsonSchemaBuilder()
        .Title("United States Securities and Exchange Commission Data Delivery Standards")
        .AdditionalProperties(JsonSchema.False)
        .Required(
            "FIRSTBATES",
            "LASTBATES",
            "ATTACHRANGE",
            "BEGATTACH",
            "ENDATTACH",
            "CUSTODIAN",
            "SUBJECT",
            "MIME_TYPE",
            "FILE_EXTEN",
            "FILE_SIZE",
            "PGCOUNT",
            "OCRPATH"
        )
        .Properties(
            new Dictionary<string, JsonSchema>
            {
                { "FIRSTBATES", AnyString }, //First Bates number of native file document/email
                { "LASTBATES", AnyString },  //Last Bates number of native file document/email
                {
                    "ATTACHRANGE", AnyString
                }, //Bates number of the first page of the parent document to the Bates number of the last page of the last attachment “child” document
                { "BEGATTACH", AnyString }, //First Bates number of attachment range
                { "ENDATTACH", AnyString }, //Last Bates number of attachment range
                { "PARENT_BATES", AnyString },
                { "CHILD_BATES", StringArray },
                { "CUSTODIAN", AnyString },
                { "FROM", StringArray },
                { "TO", StringArray },
                { "CC", StringArray },
                { "BCC", StringArray },
                { "SUBJECT", AnyString },
                { "FILE_NAME", AnyString },
                { "DATE_SENT", USDate },
                { "TIME_SENT/TIME_ZONE", USTime },
                { "TIME_ZONE", AnyString },
                { "LINK", AnyString },
                { "MIME_TYPE", AnyString },
                { "FILE_EXTEN", AnyString },
                { "AUTHOR", AnyString },
                { "LAST_AUTHOR", AnyString },
                { "DATE_CREATED", USDate },
                { "TIME_CREATED/TIME_ZONE", USTime },
                { "DATE_MOD", USDate },
                { "TIME_MOD/TIME_ZONE", USTime },
                { "DATE_ACCESSD", USDate },
                { "TIME_ACCESSD/TIME_ZONE", USTime },
                { "PRINTED_DATE", USDate },
                { "FILE_SIZE", AnyInt },
                { "PGCOUNT", AnyInt },
                { "PATH", AnyString },
                { "INTFILEPATH", AnyString },
                { "INTMSGID", AnyString },
                { "HEADER", AnyString },
                {
                    "MD5HASH", new JsonSchemaBuilder()
                        .Type(SchemaValueType.String)
                        .Pattern( //lang=regex
                            "[0-9a-f]+"
                        )
                },
                { "OCRPATH", AnyString },
            }
        );
}
