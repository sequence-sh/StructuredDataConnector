namespace Sequence.Connectors.StructuredData.Tests;

public partial class ToXmlArrayTests : StepTestBase<ToXmlArray, StringStream>
{
    /// <inheritdoc />
    protected override IEnumerable<StepCase> StepCases
    {
        get
        {
            yield return new StepCase(
                "Single Property",
                new ToXmlArray { Entities = Array(Entity.Create(("Foo", 1))), },
                @"<?xml version=""1.0"" encoding=""utf-8""?> <root> <entity> <Foo>1</Foo> </entity> </root>"
            );

            yield return new StepCase(
                "Single Property Formatted",
                new ToXmlArray { Entities = Array(Entity.Create(("Foo", 1))), },
                @"<?xml version=""1.0"" encoding=""utf-8""?> <root> <entity> <Foo>1</Foo> </entity> </root>"
            );

            yield return new StepCase(
                "Two Entities",
                new ToXmlArray
                {
                    Entities = Array(Entity.Create(("Foo", 1)), Entity.Create(("Foo", 2))),
                },
                @"<?xml version=""1.0"" encoding=""utf-8""?> <root> <entity> <Foo>1</Foo> </entity> <entity> <Foo>2</Foo> </entity> </root>"
            );

            yield return new StepCase(
                "List property",
                new ToXmlArray
                {
                    Entities = Array(
                        Entity.Create(("Foo", 1), ("Bar", new[] { "a", "b", "c" }))
                    ),
                },
                @"<?xml version=""1.0"" encoding=""utf-8""?> <root> <entity> <Foo>1</Foo> <Bar>a</Bar> <Bar>b</Bar> <Bar>c</Bar> </entity> </root>"
            );

            yield return new StepCase(
                "Nested Entities",
                new ToXmlArray
                {
                    Entities = Array(
                        Entity.Create(
                            ("Foo", 1),
                            ("Bar", new[] { "a", "b", "c" }),
                            ("Baz", Entity.Create(("Foo", 2), ("Bar", new[] { "d", "e", "f" })))
                        )
                    )
                },
                @"<?xml version=""1.0"" encoding=""utf-8""?> <root> <entity> <Foo>1</Foo> <Bar>a</Bar> <Bar>b</Bar> <Bar>c</Bar> <Baz> <Foo>2</Foo> <Bar>d</Bar> <Bar>e</Bar> <Bar>f</Bar> </Baz> </entity> </root>"
            );
        }
    }
}
