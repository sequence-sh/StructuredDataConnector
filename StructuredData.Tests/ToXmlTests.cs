namespace Sequence.Connectors.StructuredData.Tests;

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
                "<?xml version=\"1.0\" encoding=\"utf-8\"?> <root>\n\t<Foo>1</Foo>\n</root>"
            );

            yield return new StepCase(
                "List property",
                new ToXml
                {
                    Entity = Constant(
                        Entity.Create(("Foo", 1), ("Bar", new[] { "a", "b", "c" }))
                    )
                },
                "<?xml version=\"1.0\" encoding=\"utf-8\"?> <root>\n\t<Foo>1</Foo>\n\t<Bar>a</Bar>\n\t<Bar>b</Bar>\n\t<Bar>c</Bar>\n</root>"
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
                "<?xml version=\"1.0\" encoding=\"utf-8\"?> <root>\n\t<Foo>1</Foo>\n\t<Bar>a</Bar>\n\t<Bar>b</Bar>\n\t<Bar>c</Bar>\n\t<Baz>\n\t\t<Foo>2</Foo>\n\t\t<Bar>d</Bar>\n\t<Bar>e</Bar>\n\t<Bar>f</Bar>\n\t</Baz>\n</root>"
            );
        }
    }
}
