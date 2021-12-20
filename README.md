# Seqeuence StructuredData Connector

[Reductech Sequence](https://gitlab.com/reductech/sequence) is a collection of
libraries that automates cross-application e-discovery and forensic workflows.

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

## Documentation

Documentation is available here: https://docs.reductech.io

## E-discovery Reduct

The StructuredData Connector is part of a group of projects called
[E-discovery Reduct](https://gitlab.com/reductech/sequence)
which consists of a collection of [Connectors](https://gitlab.com/reductech/sequence/connectors)
and a command-line application for running Sequences, called
[Sequence](https://gitlab.com/reductech/sequence/console/-/releases).

# Releases

Can be downloaded from the [Releases page](https://gitlab.com/reductech/sequence/connectors/structureddata/-/releases).

# NuGet Packages

Are available in the [Reductech Nuget feed](https://gitlab.com/reductech/nuget/-/packages).
