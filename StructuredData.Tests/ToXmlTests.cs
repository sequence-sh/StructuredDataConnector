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
                "<?xml version=\"1.0\" encoding=\"utf-8\"?> <root>\n\t<Foo>1</Foo>\n\t<Bar>\n\t<Element>a</Element>\n\t<Element>b</Element>\n\t<Element>c</Element>\n</Bar>\n</root>"
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
                "<?xml version=\"1.0\" encoding=\"utf-8\"?> <root>\n\t<Foo>1</Foo>\n\t<Bar>\n\t<Element>a</Element>\n\t<Element>b</Element>\n\t<Element>c</Element>\n</Bar>\n\t<Baz>\n\t\t<Foo>2</Foo>\n\t\t<Bar>\n\t<Element>d</Element>\n\t<Element>e</Element>\n\t<Element>f</Element>\n</Bar>\n\t</Baz>\n</root>"
            );
        }
    }
}
