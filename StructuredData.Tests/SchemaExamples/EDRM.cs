using AutoTheory;
using Json.Schema;
using Sequence.Core.Entities;
using Xunit;
using static Sequence.Core.TestHarness.SchemaHelpers;
using static Sequence.Connectors.StructuredData.Tests.SchemaExamples.SchemaHelpers;

namespace Sequence.Connectors.StructuredData.Tests.SchemaExamples;

[UseTestOutputHelper]
public partial class EDRMSchemaExamples
{
    [Fact]
    public void EDRMSchemaShouldMatchText()
    {
        var formatText = EDRMSchema.ConvertToEntity().Format();

        TestOutputHelper.WriteLine(formatText);
    }

    /// <summary>
    /// The Schema for EDRM documents
    /// //https://edrm.net/resources/frameworks-and-standards/edrm-model/edrm-stages-standards/edrm-production-standards-version-1/
    /// </summary>
    public static readonly JsonSchema EDRMSchema = new JsonSchemaBuilder()
        .Title("EDRM Production Standards")
        .AdditionalProperties(JsonSchema.False)
        .Type(SchemaValueType.Object)
        .Required(
            "ATTACHMENTIDS",
            "BATES RANGE",
            "CUSTODIAN",
            "DOCEXT",
            "DOCID",
            "DOCLINK",
            "FILENAME",
            "RCRDTYPE"
        )
        .Properties(
            new Dictionary<string, JsonSchema>
            {
                { "ATTACHMENTIDS", AnyString }, //Docids of attachment(s) to email/edoc

                {
                    "BATES RANGE", AnyString
                }, //Begin and end bates number of a document if it differs from DocID; this can be provided in one bates range field or 2 separate fields for the beginning and ending number

                { "BCC", StringArray }, //Names of persons blind copied on an email

                { "CC", StringArray }, //Names of persons copied on an email

                { "CUSTODIAN", AnyString }, //Name of person from whom the file was obtained

                { "DATERECEIVED", UKDate }, //Date email was received

                { "DATESENT", UKDate }, //Date email was sent

                { "DOCEXT", AnyString }, //Extension of native document

                { "DOCID", AnyString }, //Extension of native document
                {
                    "DOCLINK", AnyString
                }, //Full relative path to the current location of the native or near-native document used to link metadata to native or near native file

                {
                    "FILENAME", AnyString
                }, //Name of the original native file as it existed at the time of collection

                {
                    "FOLDER", AnyString
                }, //File path/folder structure for the original native file as it existed at the time of collection

                { "FROM", AnyString }, //Name of person sending an email

                {
                    "HASH", AnyString
                }, //Identifying value of an electronic record – used for deduplication and authentication; hash value is typically MD5 or SHA1

                { "PARENTID", AnyString }, //DocId of the parent document

                {
                    "RCRDTYPE", AnyString
                }, //Indicates document type, i.e., email; attachment; edoc; scanned; etc.

                { "SUBJECT", AnyString }, //Subject line of an email

                { "TIMERECEIVED", UKTime }, //Time email was received in user’s mailbox

                { "TIMESENT", UKTime }, //Time email was sent

                { "TO", StringArray }, //Name(s) of person(s) receiving email

                { "AUTHORS", AnyString }, //Name of person creating document

                { "DATECREATED", UKDate }, //Date document was created

                { "DATESAVED", UKDate }, //Date document was last saved

                { "DOCTITLE", AnyString }, //Title given to native file
            }
        );
}
