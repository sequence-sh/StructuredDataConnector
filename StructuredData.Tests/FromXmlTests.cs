namespace Reductech.Sequence.Connectors.StructuredData.Tests;

public partial class FromXmlTests : StepTestBase<FromXml, Entity>
{
    /// <inheritdoc />
    protected override IEnumerable<StepCase> StepCases
    {
        get
        {
            yield return new StepCase(
                "Single Property",
                new FromXml { Stream = Constant("<root>\n\t<Foo>1</Foo>\n</root>") },
                Entity.Create(("Foo", "1"))
            );

            yield return new StepCase(
                "List property",
                new FromXml
                {
                    Stream = Constant(
                        "<root>\n\t<Foo>1</Foo>\n\t<Bar>a</Bar>\n\t<Bar>b</Bar>\n\t<Bar>c</Bar>\n</root>"
                    )
                },
                Entity.Create(("Foo", "1"), ("Bar", new[] { "a", "b", "c" }))
            );

            yield return new StepCase(
                "Nested Entities",
                new FromXml
                {
                    Stream = Constant(
                        "<root>\n\t<Foo>1</Foo>\n\t<Bar>a</Bar>\n\t<Bar>b</Bar>\n\t<Bar>c</Bar>\n\t<Baz>\n\t\t<Foo>2</Foo>\n\t\t<Bar>d</Bar>\n\t\t<Bar>e</Bar>\n\t\t<Bar>f</Bar>\n\t</Baz>\n</root>"
                    )
                },
                Entity.Create(
                    ("Foo", "1"),
                    ("Bar", new[] { "a", "b", "c" }),
                    ("Baz", Entity.Create(("Foo", "2"), ("Bar", new[] { "d", "e", "f" })))
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
                "Invalid Xml",
                new FromXml { Stream = Constant("My Invalid FromXml") },
                ErrorCode.CouldNotParse.ToErrorBuilder("My Invalid FromXml", "XML")
            );

            foreach (var ec in base.ErrorCases)
                yield return ec;
        }
    }
}
