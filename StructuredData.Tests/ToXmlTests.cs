namespace Reductech.Sequence.Connectors.StructuredData.Tests;

public partial class ToXmlTests : StepTestBase<ToXml, StringStream>
{
    /// <inheritdoc />
    protected override IEnumerable<StepCase> StepCases
    {
        get
        {
            yield return new StepCase(
                "Single Property",
                new ToXml { Entity = Constant(Entity.Create(("Foo", 1))) },
                "<root>\n\t<Foo>1</Foo>\n</root>"
            );

            yield return new StepCase(
                "List property",
                new ToXml
                {
                    Entity = Constant(
                        Entity.Create(("Foo", 1), ("Bar", new[] { "a", "b", "c" }))
                    )
                },
                "<root>\n\t<Foo>1</Foo>\n\t<Bar>a</Bar>\n\t<Bar>b</Bar>\n\t<Bar>c</Bar>\n</root>"
            );

            yield return new StepCase(
                "Nested Entities",
                new ToXml
                {
                    Entity = Constant(
                        Entity.Create(
                            ("Foo", 1),
                            ("Bar", new[] { "a", "b", "c" }),
                            ("Baz", Entity.Create(("Foo", 2), ("Bar", new[] { "d", "e", "f" })))
                        )
                    ),
                },
                "<root>\n\t<Foo>1</Foo>\n\t<Bar>a</Bar>\n\t<Bar>b</Bar>\n\t<Bar>c</Bar>\n\t<Baz>\n\t\t<Foo>1</Foo>\n\t\t<Bar>d</Bar>\n\t\t<Bar>e</Bar>\n\t\t<Bar>f</Bar>\n\t</Baz>\n</root>"
            );
        }
    }
}
