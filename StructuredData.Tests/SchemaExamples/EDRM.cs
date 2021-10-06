using System.Collections.Generic;
using System.Collections.Immutable;
using AutoTheory;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Entities;
using Reductech.EDR.Core.Enums;
using Reductech.EDR.Core.Internal.Serialization;
using Xunit;

namespace Reductech.EDR.Connectors.StructuredData.Tests.SchemaExamples
{

[UseTestOutputHelper]
public partial class EDRMSchemaExamples
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
        var formatText = EDRMSchema.ConvertToEntity().Format();

        TestOutputHelper.WriteLine(formatText);
    }

    //https://edrm.net/resources/frameworks-and-standards/edrm-model/edrm-stages-standards/edrm-production-standards-version-1/
    public static readonly Schema EDRMSchema = new()
    {
        Name                 = "EDRM Production Standards",
        DefaultErrorBehavior = ErrorBehavior.Error,
        ExtraProperties      = ExtraPropertyBehavior.Warn,
        Properties = new Dictionary<string, SchemaProperty>
        {
            { "ATTACHMENTIDS", ExactlyOneString }, //Docids of attachment(s) to email/edoc

            {
                "BATES RANGE", ExactlyOneString
            }, //Begin and end bates number of a document if it differs from DocID; this can be provided in one bates range field or 2 separate fields for the beginning and ending number

            { "BCC", AnyMultiplicityString }, //Names of persons blind copied on an email

            { "CC", AnyMultiplicityString }, //Names of persons copied on an email

            { "CUSTODIAN", ExactlyOneString }, //Name of person from whom the file was obtained

            { "DATERECEIVED", UpToOneUKDate }, //Date email was received

            { "DATESENT", UpToOneUKDate }, //Date email was sent

            { "DOCEXT", ExactlyOneString }, //Extension of native document

            { "DOCID", ExactlyOneString }, //Extension of native document
            {
                "DOCLINK", ExactlyOneString
            }, //Full relative path to the current location of the native or near-native document used to link metadata to native or near native file

            {
                "FILENAME", ExactlyOneString
            }, //Name of the original native file as it existed at the time of collection

            {
                "FOLDER", ExactlyOneString
            }, //File path/folder structure for the original native file as it existed at the time of collection

            { "FROM", UpToOneString }, //Name of person sending an email

            {
                "HASH", UpToOneString
            }, //Identifying value of an electronic record – used for deduplication and authentication; hash value is typically MD5 or SHA1

            { "PARENTID", UpToOneString }, //DocId of the parent document

            {
                "RCRDTYPE", ExactlyOneString
            }, //Indicates document type, i.e., email; attachment; edoc; scanned; etc.

            { "SUBJECT", UpToOneString }, //Subject line of an email

            { "TIMERECEIVED", UpToOneUKTime }, //Time email was received in user’s mailbox

            { "TIMESENT", UpToOneUKTime }, //Time email was sent

            { "TO", AnyMultiplicityString }, //Name(s) of person(s) receiving email

            { "AUTHORS", UpToOneString }, //Name of person creating document

            { "DATECREATED", UpToOneUKDate }, //Date document was created

            { "DATESAVED", UpToOneUKDate }, //Date document was last saved

            { "DOCTITLE", UpToOneString }, //Title given to native file
        }.ToImmutableSortedDictionary()
    };
}

}
