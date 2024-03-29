﻿namespace Sequence.Connectors.StructuredData.Tests;

public partial class FromJsonTests : StepTestBase<FromJson, Entity>
{
    /// <inheritdoc />
    protected override IEnumerable<StepCase> StepCases
    {
        get
        {
            yield return new StepCase(
                "Single Property",
                new FromJson { Stream = Constant("{\"Foo\":1}") },
                Entity.Create(("Foo", 1))
            );

            yield return new StepCase(
                "List property",
                new FromJson { Stream = Constant(@"{""Foo"":1,""Bar"":[""a"",""b"",""c""]}") },
                Entity.Create(("Foo", 1), ("Bar", new[] { "a", "b", "c" }))
            );

            yield return new StepCase(
                "Nested Entities",
                new FromJson
                {
                    Stream = Constant(
                        @"{""Foo"":1,""Bar"":[""a"",""b"",""c""],""Baz"":{""Foo"":2,""Bar"":[""d"",""e"",""f""]}}"
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
                new FromJson { Stream = Constant("My Invalid Json") },
                ErrorCode.CouldNotParse.ToErrorBuilder("My Invalid Json", "JSON")
            );

            foreach (var ec in base.ErrorCases)
                yield return ec;
        }
    }
}
