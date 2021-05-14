# EDR StructuredData Connector

[Reductech EDR](https://gitlab.com/reductech/edr) is a collection of
libraries that automates cross-application e-discovery and forensic workflows.

This connector contains Steps to interact with structureddata formats - JSON, CSV, Concordance, and IDX. 

## Steps

|       Step        | Description                                           | Result Type |
| :---------------: | :---------------------------------------------------- | :---------: |
| `FromConcordance` | Extracts entities from a Concordance stream. |   `Array<Entity>`    |
| `FromCSV` | Extracts entities from a CSV stream. |   `Array<Entity>`    |
| `FromIDX` | Extracts entities from an IDX stream. |   `Array<Entity>`    |
| `FromJson` | Extracts the entity from a Json stream containing a single entity. |   `Entity`    |
| `FromJsonArray` | Extracts entities from a Json stream containing an array of entities. |   `Array<Entity>`    |
| `ToConcordance` | Write entities to a stream in Concordance format. |   `String`    |
| `ToCSV` | Write entities to a stream in CSV format. |   `String`    |
| `ToIDX` | Write entities to a stream in IDX format. |   `String`    |
| `ToJson` | Writes an entity to a stream in Json format. |   `String`    |
| `ToJsonArray` | Write entities to a stream in JSON format. |   `String`    |

## Examples

To read an entity from Json, add a property and then convert it back to json

```scala
- <entity> = FromJson  "{\"Foo\":1}"
- print <entity>['Foo'] #prints 1

- <entity2> = entity + ('Bar': 2)
- <json> = ToJson <entity2>
- print <json> #{"Foo":1, "Bar": 2}
```

### [Try StructuredData Connector](https://gitlab.com/reductech/edr/edr/-/releases)

Using [EDR](https://gitlab.com/reductech/edr/edr),
the command line tool for running Sequences.

## Documentation

Documentation is available here: https://docs.reductech.io

## E-discovery Reduct

The PowerShell Connector is part of a group of projects called
[E-discovery Reduct](https://gitlab.com/reductech/edr)
which consists of a collection of [Connectors](https://gitlab.com/reductech/edr/connectors)
and a command-line application for running Sequences, called
[EDR](https://gitlab.com/reductech/edr/edr/-/releases).

# Releases

Can be downloaded from the [Releases page](https://gitlab.com/reductech/edr/connectors/structureddata/-/releases).

# NuGet Packages

Are available in the [Reductech Nuget feed](https://gitlab.com/reductech/nuget/-/packages).
