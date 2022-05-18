using Reductech.Sequence.Core.Steps;

namespace Reductech.Sequence.Connectors.StructuredData.Tests;

public partial class FromConcordanceTests : StepTestBase<FromConcordance, Array<Entity>>
{
    /// <inheritdoc />
    protected override IEnumerable<StepCase> StepCases
    {
        get
        {
            yield return new StepCase(
                "Read Concordance and Log all lines",
                new ForEach<Entity>
                {
                    Array = new FromConcordance
                    {
                        Stream = Constant(
                            $@"þFooþþBarþ{Environment.NewLine}þHelloþþWorldþ{Environment.NewLine}þHello 2þþWorld 2þ"
                        )
                    },
                    Action = new LambdaFunction<Entity, Unit>(
                        null,
                        new Log { Value = new GetAutomaticVariable<Entity>() }
                    )
                },
                Unit.Default,
                "('Foo': \"Hello\" 'Bar': \"World\")",
                "('Foo': \"Hello 2\" 'Bar': \"World 2\")"
            );

            yield return new StepCase(
                "Read Concordance containing quotes and Log all lines",
                new ForEach<Entity>
                {
                    Array = new FromConcordance
                    {
                        Stream = Constant(
                            $@"þFooþþBarþ{Environment.NewLine}þHelloþþ""World""þ{Environment.NewLine}þHello 2þþ""World 2""þ"
                        )
                    },
                    Action = new LambdaFunction<Entity, Unit>(
                        null,
                        new Log { Value = new GetAutomaticVariable<Entity>() }
                    )
                },
                Unit.Default,
                "('Foo': \"Hello\" 'Bar': \"\\\"World\\\"\")",
                "('Foo': \"Hello 2\" 'Bar': \"\\\"World 2\\\"\")"
            );

            yield return new StepCase(
                "Read Concordance with multiValue and Log all lines",
                new ForEach<Entity>
                {
                    Array = new FromConcordance
                    {
                        Stream = Constant(
                            $@"þFooþþBarþ{Environment.NewLine}þHelloþþWorld|Earthþ{Environment.NewLine}þHello 2þþWorld 2|Earth 2þ"
                        )
                    },
                    Action = new LambdaFunction<Entity, Unit>(
                        null,
                        new Log { Value = new GetAutomaticVariable<Entity>() }
                    )
                },
                Unit.Default,
                "('Foo': \"Hello\" 'Bar': [\"World\", \"Earth\"])",
                "('Foo': \"Hello 2\" 'Bar': [\"World 2\", \"Earth 2\"])"
            );
        }
    }
}
