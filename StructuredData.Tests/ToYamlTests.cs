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
                "Foo: 1\r\n"
            );

            yield return new StepCase(
                "List property",
                new ToYaml
                {
                    Entity = Constant(
                        Entity.Create(("Foo", 1), ("Bar", new[] { "a", "b", "c" }))
                    )
                },
                "Bar:\r\n- a\r\n- b\r\n- c\r\nFoo: 1\r\n"
            );

            yield return new StepCase(
                "Nested Entities",
                new ToYaml
                {
                    Entity = Constant(
                        Entity.Create(
                            ("Foo1", 1),
                            ("Bar1", new[] { "a", "b", "c" }),
                            ("Baz", Entity.Create(("Foo2", 2), ("Bar2", new[] { "d", "e", "f" })))
                        )
                    )
                },
                "Bar1: - a - b - c Baz: Bar2: - d - e - f Foo2: 2 Foo1: 1\r\n"
            );
        }
    }
}
