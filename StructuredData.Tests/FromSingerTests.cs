using System.Collections.Generic;
using Reductech.EDR.Connectors.StructuredData.Singer;
using Reductech.EDR.Connectors.StructuredData.Tests.SchemaExamples;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Steps;
using Reductech.EDR.Core.TestHarness;
using Reductech.EDR.Core.Util;

namespace Reductech.EDR.Connectors.StructuredData.Tests
{

public partial class FromSingerTests : StepTestBase<FromSinger, Array<Entity>>
{
    /// <inheritdoc />
    protected override IEnumerable<StepCase> StepCases
    {
        get
        {
            const string testData = @"
{""type"": ""STATE"",  ""value"": {}}
{""type"": ""SCHEMA"", ""stream"": ""test"", ""schema"": {}, ""key_properties"": [""id""]}
{""type"": ""RECORD"", ""stream"": ""test"", ""record"": {""a"": 1}, ""time_extracted"": ""2021-10-04T15:13:38.301481Z""}
{""type"": ""RECORD"", ""stream"": ""test"", ""record"": {""a"": 2}, ""time_extracted"": ""2021-10-04T15:13:38.301481Z""}
";

            //var text = Examples.SingerData;

            var step = new ForEach<Entity>()
            {
                Array = new FromSinger() { Stream = new StringConstant(testData) },
                Action = new LambdaFunction<Entity, Unit>(
                    null,
                    new Log<Entity>() { Value = new GetAutomaticVariable<Entity>() }
                )
            };

            ;

            yield return new StepCase(
                "Read Singer Data",
                step,
                Unit.Default,
                "('a': 1)",
                "('a': 2)"
            );
        }
    }
}

}
