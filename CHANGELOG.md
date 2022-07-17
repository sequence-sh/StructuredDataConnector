# v0.16.0 (2022-07-13)

- Enabled [Source Link](https://docs.microsoft.com/en-us/dotnet/standard/library-guidance/sourcelink)
- Enabled publish to [Nuget.org](https://www.nuget.org) including symbols
- Update Core to v0.16.0

# v0.15.0 (2022-05-27)

## Summary of Changes

### Steps

- Added steps to convert XML and YAML to/from entities
  - `FromXML`
  - `ToXML`
  - `FromXMLArray`
  - `ToXMLArray`
  - `FromYaml`
  - `ToYaml`

## Issues Closed in this Release

### New Features

- Add steps to convert to/from XML #58
- Add steps to convert to/from YAML #57

# v0.14.0 (2022-03-25)

Maintenance release - dependency updates only.

# v0.13.0 (2022-01-16)

EDR is now Sequence. The following has changed:

- The GitLab group has moved to https://gitlab.com/reductech/sequence
- The root namespace is now `Reductech.Sequence`
- The documentation site has moved to https://sequence.sh

Everything else is still the same - automation, simplified.

The project has now been updated to use .NET 6.

## Issues Closed in this Release

### Maintenance

- Rename EDR to Sequence #28
- Update Core to support SCLObject types #23
- Upgrade to use .net 6 #22

# v0.12.0 (2021-11-26)

`FromSinger` step has now been moved to the Singer Connector.

## Issues Closed in this Release

### New Features

- Remove FromSinger #14
- Create a Step to read Singer Data allowing EDR to act as a Singer Target #13

### Maintenance

- Update Core to latest version #15

# v0.11.0 (2021-09-03)

Maintenace release - dependency updates and testing improvements.

## Issues Closed in this Release

### Maintenance

- Add Concordance Parsing Tests which parse a concordance file and then apply a schema to it #9

# v0.10.0 (2021-07-02)

## Issues Closed in this Release

### Maintenance

- Update Core to latest and removing SCLSettings #5

# v0.9.0 (2021-05-14)

First release. Versions numbers are aligned with Core.

## Summary of Changes

### Steps

- Moved the following Steps from Core:
  - `FromConcordance`
  - `FromCSV`
  - `FromIDX`
  - `FromJson`
  - `FromJsonArray`
  - `ToConcordance`
  - `ToCSV`
  - `ToIDX`
  - `ToJson`
  - `ToJsonArray`

## Issues Closed in this Release

### Maintenance

- Enable publish to connector registry #3
- Update Core dependecies #2
- Move in steps and tests from Core #1
