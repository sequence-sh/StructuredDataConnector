using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using AutoTheory;
using Divergic.Logging.Xunit;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Abstractions;
using Reductech.EDR.Core.Entities;
using Reductech.EDR.Core.Enums;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Internal.Serialization;
using Reductech.EDR.Core.Steps;
using Reductech.EDR.Core.TestHarness;
using Reductech.EDR.Core.Util;
using Xunit;
using static Reductech.EDR.Core.TestHarness.StaticHelpers;

namespace Reductech.EDR.Connectors.StructuredData.Tests.SchemaExamples
{

[UseTestOutputHelper]
public partial class UssecSchemaExamples
{
    public static SchemaProperty ExactlyOneString =
        new() { Multiplicity = Multiplicity.ExactlyOne, Type = SCLType.String };

    public static SchemaProperty UpToOneString =
        new() { Multiplicity = Multiplicity.UpToOne, Type = SCLType.String };

    public static SchemaProperty AnyMultiplicityString =
        new() { Multiplicity = Multiplicity.Any, Type = SCLType.String };

    public static SchemaProperty AtLeastOneString =
        new() { Multiplicity = Multiplicity.Any, Type = SCLType.String };

    public static SchemaProperty UpToOneUSDate = new()
    {
        Multiplicity     = Multiplicity.UpToOne,
        Type             = SCLType.Date,
        DateOutputFormat = "MM/dd/yyyy"
    };

    public static SchemaProperty UpToOneUSTime = new()
    {
        Multiplicity     = Multiplicity.UpToOne,
        Type             = SCLType.Date,
        DateOutputFormat = "hh:mm tt zzz"
    };

    public static SchemaProperty UpToOneUKDate = new()
    {
        Multiplicity     = Multiplicity.UpToOne,
        Type             = SCLType.Date,
        DateOutputFormat = "yyyy/MM/dd"
    };

    public static SchemaProperty UpToOneUKTime = new()
    {
        Multiplicity     = Multiplicity.UpToOne,
        Type             = SCLType.Date,
        DateOutputFormat = "HH:mm:ss zzz"
    };

    [Fact]
    public void USSecSchemaShouldMatchText()
    {
        var formatText = USSecSchema.ConvertToEntity().Format();

        TestOutputHelper.WriteLine(formatText);
    }

    [Fact]
    public async Task EnforceSchemaShouldWork()
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
                new Log<Entity> { Value = new GetAutomaticVariable<Entity>() }
            ),
            Array = new EnforceSchema
            {
                Schema = Constant(USSecSchema.ConvertToEntity()),
                EntityStream = new EntityMapProperties
                {
                    EntityStream = new EntityMap
                    {
                        EntityStream = new FromConcordance { Stream = Constant(data) },
                        Function = new LambdaFunction<Entity, Entity>(
                            null,
                            new EntitySetValue<int>
                            {
                                Value =
                                    new If<int>()
                                    {
                                        Condition = new Not()
                                        {
                                            Boolean = new StringIsEmpty()
                                            {
                                                String = new EntityGetValue<StringStream>()
                                                {
                                                    Entity   = GetEntityVariable,
                                                    Property = Constant("File Size")
                                                }
                                            }
                                        },
                                        Then = new StringToInt()
                                        {
                                            Integer = new StringInterpolate()
                                            {
                                                Strings = new List<IStep>()
                                                {
                                                    new DoubleProduct
                                                    {
                                                        Terms = new ArrayNew<double>
                                                        {
                                                            Elements =
                                                                new List<IStep<double>>
                                                                {
                                                                    new StringToDouble
                                                                    {
                                                                        Double =
                                                                            new RegexReplace()
                                                                            {
                                                                                String =
                                                                                    new RegexReplace
                                                                                    {
                                                                                        String =
                                                                                            new
                                                                                                EntityGetValue
                                                                                                <
                                                                                                    StringStream>
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
                                                                                Pattern = Constant(
                                                                                    "null"
                                                                                ),
                                                                                IgnoreCase =
                                                                                    Constant(true),
                                                                                Replace = Constant(
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

        var serializedStep = step.Serialize();

        TestOutputHelper.WriteLine(serializedStep);

        var sm = new StateMonad(
            new TestOutputLogger("Logger", TestOutputHelper),
            StepFactoryStore.CreateFromAssemblies(typeof(FromConcordance).Assembly),
            ExternalContext.Default,
            DefaultRestClientFactory.Instance,
            new Dictionary<string, object>()
        );

        var result = await step.Run<Unit>(sm, CancellationToken.None);

        result.ShouldBeSuccessful();
    }

    public static readonly Schema USSecSchema = new()
    {
        Name = "United States Securities and Exchange Commission Data Delivery Standards",
        DefaultErrorBehavior = ErrorBehavior.Error,
        ExtraProperties = ExtraPropertyBehavior.Remove,
        Properties = new Dictionary<string, SchemaProperty>
        {
            { "FIRSTBATES", ExactlyOneString }, //First Bates number of native file document/email
            { "LASTBATES", ExactlyOneString },  //Last Bates number of native file document/email
            {
                "ATTACHRANGE", ExactlyOneString
            }, //Bates number of the first page of the parent document to the Bates number of the last page of the last attachment “child” document
            { "BEGATTACH", ExactlyOneString }, //First Bates number of attachment range
            { "ENDATTACH", ExactlyOneString }, //Last Bates number of attachment range
            { "PARENT_BATES", UpToOneString },
            { "CHILD_BATES", AnyMultiplicityString },
            { "CUSTODIAN", ExactlyOneString },
            { "FROM", AtLeastOneString },
            { "TO", AnyMultiplicityString },
            { "CC", AnyMultiplicityString },
            { "BCC", AnyMultiplicityString },
            { "SUBJECT", ExactlyOneString },
            { "FILE_NAME", UpToOneString },
            { "DATE_SENT", UpToOneUSDate },
            { "TIME_SENT/TIME_ZONE", UpToOneUSTime },
            { "TIME_ZONE", UpToOneString },
            { "LINK", UpToOneString },
            { "MIME_TYPE", ExactlyOneString },
            { "FILE_EXTEN", ExactlyOneString },
            { "AUTHOR", UpToOneString },
            { "LAST_AUTHOR", UpToOneString },
            { "DATE_CREATED", UpToOneUSDate },
            { "TIME_CREATED/TIME_ZONE", UpToOneUSTime },
            { "DATE_MOD", UpToOneUSDate },
            { "TIME_MOD/TIME_ZONE", UpToOneUSTime },
            { "DATE_ACCESSD", UpToOneUSDate },
            { "TIME_ACCESSD/TIME_ZONE", UpToOneUSTime },
            { "PRINTED_DATE", UpToOneUSDate },
            {
                "FILE_SIZE",
                new SchemaProperty
                {
                    Multiplicity = Multiplicity.ExactlyOne, Type = SCLType.Integer
                }
            },
            {
                "PGCOUNT",
                new SchemaProperty
                {
                    Multiplicity = Multiplicity.ExactlyOne, Type = SCLType.Integer
                }
            },
            { "PATH", UpToOneString },
            { "INTFILEPATH", UpToOneString },
            { "INTMSGID", UpToOneString },
            { "HEADER", UpToOneString },
            {
                "MD5HASH", UpToOneString with
                {
                    Regex =
                    //lang=regex
                    "[0-9a-f]+"
                }
            },
            { "OCRPATH", ExactlyOneString },
        }.ToImmutableSortedDictionary()
    };
}

}
