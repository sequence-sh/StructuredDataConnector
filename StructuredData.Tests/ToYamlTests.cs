namespace Reductech.Sequence.Connectors.StructuredData.Tests;

public partial class ToYamlTests : StepTestBase<ToYaml, StringStream>
{
    /// <inheritdoc />
    protected override IEnumerable<StepCase> StepCases
    {
        get
        {
            yield return new StepCase(
                "Single Property",
                new ToYaml { Entity = Constant(Entity.Create(("Foo", 1))) },
                "Foo: 1"
            );

            yield return new StepCase(
                "Single Property Formatted",
                new ToYaml { Entity = Constant(Entity.Create(("Foo", 1))) },
                "Foo: 1"
            );

            yield return new StepCase(
                "List property",
                new ToYaml
                {
                    Entity = Constant(
                        Entity.Create(("Foo", 1), ("Bar", new[] { "a", "b", "c" }))
                    )
                },
                "Foo: 1\r\nBar:\r\n- a\r\n- b\r\n- c"
            );

            yield return new StepCase(
                "Nested Entities",
                new ToYaml
                {
                    Entity = Constant(
                        Entity.Create(
                            ("Foo", 1),
                            ("Bar", new[] { "a", "b", "c" }),
                            ("Baz", Entity.Create(("Foo", 2), ("Bar", new[] { "d", "e", "f" })))
                        )
                    )
                },
                "Foo: 1\r\nBar:\r\n- a\r\n- b\r\n- c\r\nBaz:\r\n  Foo: 2\r\n  Bar:\r\n  - d\r\n  - e\r\n  - f"
            );
        }
    }
}
