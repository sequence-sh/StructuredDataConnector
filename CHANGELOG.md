# v0.12.0 (2021-11-26)

## Summary of Changes

### Steps

- Removed `FromSinger` - It has been moved to the Singer Connector


- Added `FromSinger`

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

