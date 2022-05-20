namespace Reductech.Sequence.Connectors.StructuredData.Tests;

public partial class FromXmlArrayTests : StepTestBase<FromXmlArray, Array<Entity>>
{
    /// <inheritdoc />
    protected override IEnumerable<StepCase> StepCases
    {
        get
        {
            static Array<Entity> CreateArray(params Entity[] entities)
            {
                return entities.ToSCLArray();
            }

            yield return new StepCase(
                "Single Property",
                new FromXmlArray { Stream = Constant("<root><Foo>1</Foo></root>") },
                CreateArray(Entity.Create(("value", 1)))
            );

            yield return new StepCase(
                "Two Entities",
                new FromXmlArray { Stream = Constant("<root><Foo>1</Foo><Foo>2</Foo></root>") },
                CreateArray(Entity.Create(("value", 1)), Entity.Create(("value", 2)))
            );

            yield return new StepCase(
                "List property",
                new FromXmlArray
                {
                    Stream = Constant(@"<root><element><Foo>1</Foo></element></root>")
                },
                CreateArray(Entity.Create(("Foo", 1)))
            );

            yield return new StepCase(
                "Nested Entities",
                new FromXmlArray
                {
                    Stream = Constant(
                        @"<root><element><Foo><Bar>1</Bar></Foo></element></root>"
                    )
                },
                CreateArray(Entity.Create(("Foo", ("Bar", 1))))
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
                new FromJsonArray { Stream = Constant("My Invalid Json") },
                ErrorCode.CouldNotParse.ToErrorBuilder("My Invalid Json", "JSON")
            );

            foreach (var ec in base.ErrorCases)
                yield return ec;
        }
    }
}
