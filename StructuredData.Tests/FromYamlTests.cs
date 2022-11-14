namespace Sequence.Connectors.StructuredData.Tests;

public partial class FromYamlTests : StepTestBase<FromYaml, Entity>
{
    /// <inheritdoc />
    protected override IEnumerable<StepCase> StepCases
    {
        get
        {
            yield return new StepCase(
                "Single Property",
                new FromYaml { Stream = Constant("Foo: 1") },
                Entity.Create(("Foo", 1))
            );

            yield return new StepCase(
                "List property",
                new FromYaml { Stream = Constant("Foo: 1\r\nBar:\r\n- a\r\n- b\r\n- c") },
                Entity.Create(("Foo", 1), ("Bar", new[] { "a", "b", "c" }))
            );

            yield return new StepCase(
                "Nested Entities",
                new FromYaml
                {
                    Stream = Constant(
                        "Foo: 1\r\nBar:\r\n- a\r\n- b\r\n- c\r\nBaz:\r\n  Foo: 2\r\n  Bar:\r\n  - d\r\n  - e\r\n  - f"
                    )
                },
                Entity.Create(
                    ("Foo", 1),
                    ("Bar", new[] { "a", "b", "c" }),
                    ("Baz", Entity.Create(("Foo", 2), ("Bar", new[] { "d", "e", "f" })))
                )
            );
        }
    }

    /// <inheritdoc />
    protected override IEnumerable<ErrorCase> ErrorCases
    {
        get
        {
            yield return new ErrorCase(
                "Invalid Json",
                new FromYaml { Stream = Constant("My Invalid Yaml") },
                ErrorCode.CouldNotParse.ToErrorBuilder("My Invalid Yaml", "YAML")
            );

            foreach (var ec in base.ErrorCases)
                yield return ec;
        }
    }
}
