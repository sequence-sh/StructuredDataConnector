using Reductech.EDR.Core.Steps;

namespace Reductech.EDR.Connectors.StructuredData.Tests;

public partial class FromCSVTests : StepTestBase<FromCSV, Array<Entity>>
{
    /// <inheritdoc />
    protected override IEnumerable<StepCase> StepCases
    {
        get
        {
            yield return new StepCase(
                "Read CSV and Log all lines",
                new ForEach<Entity>
                {
                    Array = new FromCSV
                    {
                        Stream = Constant(
                            $@"Foo,Bar{Environment.NewLine}Hello,World{Environment.NewLine}Hello 2,World 2"
                        )
                    },
                    Action = new LambdaFunction<Entity, Unit>(
                        null,
                        new Log<Entity> { Value = new GetAutomaticVariable<Entity>() }
                    )
                },
                Unit.Default,
                "('Foo': \"Hello\" 'Bar': \"World\")",
                "('Foo': \"Hello 2\" 'Bar': \"World 2\")"
            );

            yield return new StepCase(
                "Read CSV and Log all lines should ignore missing columns",
                new ForEach<Entity>
                {
                    Array = new FromCSV
                    {
                        Stream = Constant(
                            $@"Foo{Environment.NewLine}Hello,World{Environment.NewLine}Hello 2,World 2"
                        )
                    },
                    Action = new LambdaFunction<Entity, Unit>(
                        null,
                        new Log<Entity> { Value = new GetAutomaticVariable<Entity>() }
                    )
                },
                Unit.Default,
                "('Foo': \"Hello\")",
                "('Foo': \"Hello 2\")"
            );

            yield return new StepCase(
                "Read CSV and Log all lines should ignore comments",
                new ForEach<Entity>
                {
                    Array = new FromCSV
                    {
                        Stream = Constant(
                            $@"#this is a comment{Environment.NewLine}Foo{Environment.NewLine}Hello,World{Environment.NewLine}Hello 2,World 2"
                        )
                    },
                    Action = new LambdaFunction<Entity, Unit>(
                        null,
                        new Log<Entity> { Value = new GetAutomaticVariable<Entity>() }
                    )
                },
                Unit.Default,
                "('Foo': \"Hello\")",
                "('Foo': \"Hello 2\")"
            );
        }
    }

    /// <inheritdoc />
    protected override IEnumerable<ErrorCase> ErrorCases
    {
        get
        {
            foreach (var errorCase in base.ErrorCases)
                yield return errorCase;

            //TODO tests for errors if we can find any :)
        }
    }
}
