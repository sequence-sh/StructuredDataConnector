using Json.Schema;

namespace Reductech.EDR.Connectors.StructuredData.Tests.SchemaExamples
{

public static class SchemaHelpers
{
    public static JsonSchema USDate = new JsonSchemaBuilder()
        .Type(SchemaValueType.String)
        .Format(Formats.Date); //"MM/dd/yyyy"

    public static JsonSchema USTime = new JsonSchemaBuilder()
        .Type(SchemaValueType.String)
        .Format(Formats.Time); //"hh:mm tt zzz"

    public static JsonSchema UKDate = new JsonSchemaBuilder()
        .Type(SchemaValueType.String)
        .Format(Formats.Date); //"MM/dd/yyyy"

    public static JsonSchema UKTime = new JsonSchemaBuilder()
        .Type(SchemaValueType.String)
        .Format(Formats.Time); //"HH:mm:ss zzz"

    public static JsonSchema StringArray = new JsonSchemaBuilder()
        .Type(SchemaValueType.Array)
        .Items(Core.TestHarness.SchemaHelpers.AnyString);
}

}
