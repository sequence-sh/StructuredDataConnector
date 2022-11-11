# Sequence® StructuredData Connector

[Sequence®](https://sequence.sh) is a collection of libraries for
automation of cross-application e-discovery and forensic workflows.

This connector contains Steps to interact with structured data formats:

- Json
- CSV
- Concordance
- IDX

There are steps to convert data to and from all of these formats.

## Examples

To read an entity from Json, add a property and then convert it back to json

```scala
- <entity> = FromJson  "{\"Foo\":1}"
- print <entity>['Foo'] #prints 1

- <entity2> = <entity> + ('Bar': 2)
- <json> = ToJson <entity2>
- print <json> #{"Foo":1, "Bar": 2}
```

# Documentation

https://sequence.sh

# Download

https://sequence.sh/download

# Try SCL and Core

https://sequence.sh/playground

# Package Releases

Can be downloaded from the [Releases page](https://gitlab.com/sequence/connectors/structureddata/-/releases).

# NuGet Packages

Release nuget packages are available from [nuget.org](https://www.nuget.org/profiles/Sequence).
