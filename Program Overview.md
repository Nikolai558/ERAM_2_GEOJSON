# PROGRAM OVERVIEW

## INTENT

Create a .cs terminal program that will take data from a RW ERAM GeoMaps.xml and create geojson files in accordance with RFC-7946 format standards for use by the program CRC.
The program will create .geojsons that are Linestrings (including multilinestrings), Symbols, and Text.

## INPUT/OUTPUT

Via arguments, the user will provide the path to the geomaps.xml file and then in the second argument the directory for the output location.

A third argument will be a T/F boolean turning Custom Properties on or off (off by default). More details explained later in this document.

Each geojson file may contain multiple features but those features will all be of the same type (i.e. Lines, Symbols, or Text). Each geojson will have appended `_Lines` `_Symbols` and `_Text` at the end of the .geojson name accordingly.

## XML TAG VALUES AND USE

- `Geomaps_Records`
  - Contains all GeoMapRecords.
  - May contain multiple.
  - `GeoMapRecord`
    - All records for a GeoMap.
    - May contain multiple.
    - This value will be used to create the name of the folder where the children objects are output.
    - `GeomapId`
      - (String) ID for the GeoMap.
    - `BCGMenuName`
      - Ignore.
    - `FilterMenuName`
      - Ignore.
    - `LabelLine1`
      - (String) Save this value to a variable "GeoMapLabelLine1" under this GeomapId library.
    - `LabelLine2`
      - (String) Save this value to a variable "GeoMapLabelLine2" under this GeomapId library.
    - `MinLatitude`
      - Ignore.
    - `MaxLatitude`
      - Ignore.
    - `MinLongitude`
      - Ignore.
    - `MaxLongitude`
      - Ignore.
    - `MinXSpherical`
      - Ignore.
    - `MinYSpherical`
      - Ignore.
    - `MinZSpherical`
      - Ignore.
    - `MaxXSpherical`
      - Ignore.
    - `MaxYSpherical`
      - Ignore.
    - `MaxZSpherical`
      - Ignore.
    - `GeoMapObjectType`
      - Contains all Objects attached to this GeomapId.
      - May contain multiple.
      - `MapObjectType`
        - (String) Type of object with possible values of:
          - VOR
          - TACAN
          - WAYPOINT
          - NDB
          - DME
          - AIRPORT
          - EmergencyAirport
          - SatelliteAirport
          - MilitaryRoutes
          - AIRWAY
          - STAR
          - ADAR
          - ADR
          - APR
          - AAR
          - DP
          - SAA
          - ARTS
          - FAV
          - SECTOR
          - ApproachControl
          - ARTCC
          - SupplementalLine
          - SupplementalSymbol
          - AAV
          - TAV
          - TAVAOI
      - `MapGroupId`
        - (integer) Group Number assigned to the MapObjectType.
        - For lines, this will assist in knowing which objects to put together in the same Geojson Feature.
      - `DefaultLineProperties`
        - List of properties that are assigned to the following GeoMapLine objects that do not have overriding properties of their own.
        - FilterGroup is of importance.
        - Optional.
        - `LineStyle`
          - Ignore.
          - Optional.
        - `BCGGroup`
          - Ignore.
          - Optional.
        - `Color`
          - Ignore.
          - Optional.
        - `Thickness`
          - Ignore.
          - Optional.
        - `GeoLineFilters`
          - Single or group of filters to be assigned to the line.
          - May be multiple.
          - Optional.
          - `FilterGroup`
            - (Integer) The actual value to be assigned.
            - May be multiple.
      - `DefaultSymbolProperties`
        - List of properties that are assigned to the following GeoMapSymbol objects that do not have overriding properties of their own.
        - SymbolStyle and FilterGroup is of importance.
        - Optional.
        - `SymbolStyle`
          - (String) Details the Style to be used for this Symbol.
          - May include:
            - VOR
              - Convert to read "vor"
            - TACAN
              - Convert to read "tacan"
            - OtherWaypoints
              - Convert to read "otherWaypoints"
            - NDB
              - Convert to read "ndb"
            - DME
              - Convert to read "dme"
            - Airport
              - Convert to read "airport"
            - EmergencyAirport
              - Convert to read "emergencyAirport"
            - SatelliteAirport
              - Convert to read "satelliteAirport"
            - Obstruction1
              - Convert to read "obstruction1"
            - Obstruction2
              - Convert to read "obstruction2"
            - Heliport
              - Convert to read "heliport"
            - Nuclear
              - Convert to read "solinucleard"
            - Radar
              - Convert to read "radar"
            - IAF
              - Convert to read "iaf"
            - RNAVOnlyWaypoint
              - Convert to read "rnavOnlyWaypoint"
            - RNAV
              - Convert to read "rnav"
            - AirwayIntersections
              - Convert to read "airwayIntersections"
          - Optional.
        - `BCGGroup`
          - Ignore.
          - Optional.
        - `Color`
          - Ignore.
          - Optional.
        - `FontSize`
          - Ignore.
          - Optional.
        - `GeoSymbolFilters`
          - Single or group of filters to be assigned to the symbol.
          - May be multiple.
          - Optional.
          - `FilterGroup`
            - (Integer) The actual value to be assigned.
            - May be multiple.
      - `TextDefaultProperties`
        - List of properties that are assigned to the following GeoMapSymbol objects that do not have overriding properties of their own.
        - SymbolStyle and FilterGroup is of importance.
        - Optional.
        - `BCGGroup`
          - Ignore.
          - Optional.
        - `Color`
          - Ignore.
          - Optional.
        - `FontSize`
          - Ignore.
          - Optional.
        - `Underline`
          - Ignore.
          - Optional.
        - `DisplaySetting`
          - Ignore.
          - Optional.
        - `XPixelOffset`
          - Ignore.
          - Optional.
        - `YPixelOffset`
          - Ignore.
          - Optional.
        - `GeoTextFilters`
          - Single or group of filters to be assigned to the text.
          - May be multiple.
          - Optional.
          - `FilterGroup`
            - (Integer) The actual value to be assigned.
            - May be multiple.
      - `GeoMapLine`
        - Line data for this object.
        - `LineObjectId`
          - This value will later be used to help group other like-coordinates into the same feature and possibly the custom properties section if the user elects to have them injected into the features.
        - `StartLatitude`
          - The Start Latitude for this GeoMapLine>LineObjectId
        - `StartLongitude`
          - The Start Longitude for this GeoMapLine>LineObjectId
        - `EndLatitude`
          - The End Latitude for this GeoMapLine>LineObjectId
        - `EndLongitude`
          - The End Longitude for this GeoMapLine>LineObjectId
        - `StartXSpherical`
          - Ignore.
        - `StartYSpherical`
          - Ignore.
        - `StartZSpherical`
          - Ignore.
        - `EndXSpherical`
          - Ignore.
        - `EndYSpherical`
          - Ignore.
        - `EndZSpherical`
          - Ignore.
        - `GeoLineFilters`
          - Single or group of filters to be assigned to the line.
          - May be multiple.
          - Optional.
          - `FilterGroup`
            - (Integer) The actual value to be assigned.
            - May be multiple.
      - `GeoMapSymbol`
        - Symbol data for this object.
        - `SymbolId`
          - Ignore.
        - `FontSize`
          - Ignore.
        - `Latitude`
          - Latitude for this symbol.
        - `Longitude`
          - Longitude for this symbol.
        - `XSpherical`
          - Ignore.
        - `YSpherical`
          - Ignore.
        - `ZSpherical`
          - Ignore.
        - `GeoSymbolFilters`
          - Single or group of filters to be assigned to the symbol.
          - May be multiple.
          - Optional.
          - `FilterGroup`
            - (Integer) The actual value to be assigned.
            - May be multiple.
        - `GeoMapText`
          - Text data for this symbol object.
          - Note that the GeoMapText section would have been made its' own object outside of the GeoMapSymbol object if I had made the .xml but it is within the GeoMapSymbol object, unfortunately. The coordinates for the GeoMapText is found in the GeoMapSymbol data.
          - `GeoTextStrings`
            - The string of text associated with this GeoMapText object.
          - `GeoTextFilters`
            - Single or group of filters to be assigned to the text.
            - May be multiple.
            - Optional.
            - `FilterGroup`
              - (Integer) The actual value to be assigned.
              - May be multiple.
      - `GeoMapSaa`
        - Ignore.

## OUTPUT DIRECTORY SETUP

First the program will find each `<GeoMapRecord>`.

Inside of that GeoMapRecord, get the `<GeomapId>` value and the `<LabelLine1>` and `<LabelLine2>` values. Create folders in the output directory that follow this format: `<GeomapId>`  > `<LabelLine1>`_`<LabelLine2>`
Example: **CENTER-CTR_MAP**  
*Note: If the `<GeomapId>` folder already exists, the program will delete that directory and subdirectories and content, and remake it.*

In each folder that was made, create subdirectories "Filter_01", "Filter_02", etc...
Also create a folder labeled "Multi-Filter".  
*Note: When creating directories, Filter numbers must always be made with 2 digits but prefixing zeros should not be added to file name or other data later.*

## DETERMINING LINES, SYMBOLS, or TEXT

To determine if a coordinate is attached to a Line, Symbol, or Text, look in `<GeomapId>` > `GeoMapObjectType`

- If `GeoMapLine` it is a Linestring object.
- If `GeoMapSymbol` it is a Symbol object.
  - Exception: If there is a `GeoMapText` within `GeoMapSymbol`, change the object type from Symbol to Text.

## FILTERGROUP ASSIGNMENT

To determine what filter directory to place the .geojson in, the program will need to know the FilterGroup that is assigned to that data.

- For Lines (GeoMapLine)
  - First look for the FilterGroup in `GeoMapObjectType`  > `GeoMapLine`  > `GeoLineFilters`, if not found, look in `GeoMapObjectType`  > `DefaultLineProperties`  > `GeoLineFilters`.
    - It is important to look in this order because the data in `GeoMapObjectType`  > `GeoMapLine`  > `GeoLineFilters` overrides `GeoMapObjectType`  > `DefaultLineProperties`  > `GeoLineFilters`.
- For Symbols (GeoMapSymbol without GeoMapText)
  - First look for the FilterGroup in `GeoMapObjectType`  > `GeoMapSymbol`  > `GeoSymbolFilters`, if not found, look in `GeoMapObjectType`  > `DefaultSymbolProperties`  > `GeoSymbolFilters`.
    - It is important to look in this order because the data in `GeoMapObjectType`  > `GeoMapSymbol`  > `GeoSymbolFilters` overrides `GeoMapObjectType`  > `DefaultSymbolProperties`  > `GeoSymbolFilters`.
- For Text (GeoMapSymbol with GeoMapText)
  - First look for the FilterGroup in `GeoMapObjectType`  > `GeoMapSymbol`  > `GeoMapText`  > `GeoTextFilters`, if not found, look in `GeoMapObjectType`  > `TextDefaultProperties`  > `GeoTextFilters`.
    - It is important to look in this order because the data in `GeoMapObjectType`  > `GeoMapSymbol`  > `GeoMapText`  > `GeoTextFilters` overrides `GeoMapObjectType`  > `TextDefaultProperties`  > `GeoTextFilters`.

## COORDINATES

Coordinates retrieved from the .xml will be in DMS format such as "37000000N" and need to be converted to decimal format for the .geojson. Round to the nearest 8 decimal places.

- Example Latitude: 42124729N
  - Degrees: 42
  - Minutes: 12
  - Seconds: 47
  - Decimal seconds: .29
  - Decimal Format: 42.21313611

- Example Longitude: 083215999W
  - Degrees: 083 (or 83)
  - Minutes: 21
  - Seconds: 59
  - Decimal seconds: .99
  - Decimal Format: -83.36666639

Coordinates may be found for the objects as follows:  
For Lines:

- `GeoMapObjectType`  > `GeoMapLine`  > `StartLatitude`
- `GeoMapObjectType`  > `GeoMapLine`  > `StartLongitude`

For Symbols & Text:

- `GeoMapObjectType`  > `GeoMapSymbol`  > `Latitude`
- `GeoMapObjectType`  > `GeoMapSymbol`  > `Longitude`

## OUTPUT DIRECTORY AND FILE NAME

The completed geojson file will be saved in the Filter_## folder determined by the `FilterGroup`.

If more than one FilterGroup value is assigned, the coordinates for this feature will be saved in a file in the Multi-Filter folder with a subdirectory created with the following format organizing the assigned FilterGroups in numerical order:
`Filter_##_##_##` etc...  
Example: if there is one or more elements with a filtergroup assignment of 8,2,1,12 the following directory would be created: `...Multi-Filter\Filter_01_02_08_12`

Multiple like-elements with identical filter assignments will be output to the same .geojson.  
For example, if one line-type element has a FilterGroup of "3,5,13" and later on in the .xml another line-type element has that also has a FilterGroup assignment of "3,5,13", these two elements will be placed togheter in the same `_Lines.geojson`.

Name of the outout file will follow the following format:  
`<same as the host directory>`_`<Lines/Symbols/Text>`.geojson

Examples:

- ...\Filter_01\
  - Filter_01_Lines.geojson
  - Filter_01_Symbols.geojson
  - Filter_01_Text.geojson
- ...\Filter_13\
  - Filter_13_Lines.geojson
  - Filter_13_Symbols.geojson
  - Filter_13_Text.geojson
- ...\Multi-Filter\Filter_02_04_05_11\
  - Filter_02_04_05_11_Lines.geojson
  - Filter_02_04_05_11_Symbols.geojson
  - Filter_02_04_05_11_Text.geojson

## CUSTOM PROPERTIES

Custom Properties will be information from the .xml that is not needed to make the geojson but may be helpful with testing and organization by the Facility Engineer (person that manages the .geojson files). This feature is turned off (F) by default.

Not all tags and elements will carry over into the geojson; Below you will find information for exactly how this information is handled and when it is passed in to the .geojson.

In order to avoid future confliction with other .geojson readers, the properties keys will be the tag name from the .xml but prefixed with "E2G_".

FOR LINES:

- If the user elects to have Custom Properties turned on, the program will insert a custom key/values into the properties section of the feature detailing the .xml's MapObjectType, MapGroupId, and LineObjectId.
  - Format:
    - "E2G_MapObjectType": "`<GeoMapObjectType>`"
    - "E2G_MapGroupId": "`<MapGroupId>`"
    - "E2G_LineObjectId": "`<LineObjectId>`"
  - Example:
    - "E2G_MapObjectType": "ApproachControl"
    - "E2G_MapGroupId": "1"
    - "E2G_LineObjectId": "DTW"

FOR SYMBOLS:

- If the user elects to have Custom Properties turned on, the program will insert a custom key/values into the properties section of the feature detailing the .xml's MapObjectType.
  - Format:
    - "E2G_MapObjectType": "`<MapObjectType>`"
  - Example:
    - "E2G_MapObjectType": "WAYPOINT"

FOR TEXT:

- If the user elects to have Custom Properties turned on, the program will insert a custom key/values into the properties section of the feature detailing the .xml's MapObjectType.

- Regardless if the user elects to have custom properties turned on or not, the program will create a property key of "text" and assign it the value for the .xml's tag TextLine.
  - Format:
    - "E2G_MapObjectType": "`<MapObjectType>`"
    - "text": ["`<TextLine>`"]
  - Example:
    - "E2G_MapObjectType": "WAYPOINT"
    - "text": ["ALEEE"]

- When cdreating a Text feature in the `_Text.geojson`,the program will also create a feature for this text element as a "Symbol" in the `_Symbols.geojson` file using the same FilterGroup value. (i.e., after creating the text feature, run the symbol feature aswell but add the same `"text": ["<TextLine>"]` property in the symbols file.)

## EFFICIENT LINESTRING HANDLING

To save space and organization of the .geojson line files, we will combine MapObjectType, MapGroupId, and LineObjectId together.

For example, if `MapObjectType=ApproachControl`, `MapGroupId=1`, and the `LineObjectId=BUF`, then all LineObjectId elements that match that sequence of attributes will be grouped together in the same Feature.

If the current StartLatitude and StartLongitude is equal to the previous EndLatitude and EndLongitude, then just add the current EndLatitude and EndLongitude to the same linestring. When the current StartLatitude and StartLongitude are not equal to the previous EndLatitude and EndLongitude, create a break in the multilinestring.

In the example below, the user elected to have custom properties included.

Example

```json
{"type":"FeatureCollection","features":[{"type":"Feature","geometry":{"type":"MultiLineString","coordinates":[[[-108.79043292472375,40.8506145555433],[-102.2181888812155,37.06641395317544],[-108.9552885445442,40.80903657260835]],[[-108.3837890625,40.46711432758179],[-105.2,37.1]]]},"properties":{"E2G_MapObjectType":"ApproachControl","E2G_MapGroupId":"1","E2G_LineObjectId":"BUF"}},{"type":"Feature","geometry":{"type":"MultiLineString","coordinates":[[[-109.1,41.8],[-103.1,38.1],[-109.2,41.9]],[[-107.2,41.3],[-108.2,42.3]]]},"properties":{"E2G_MapObjectType":"ApproachControl","E2G_MapGroupId":"2","E2G_LineObjectId":"BUF"}},{"type":"Feature","geometry":{"type":"MultiLineString","coordinates":[[[-111.1,42.8],[-108.1,39.1],[-110.2,42.9]],[[-105.2,40.70000000000001],[-105.3,40.8]]]},"properties":{"E2G_MapObjectType":"ApproachControl","E2G_MapGroupId":"1","E2G_LineObjectId":"DTW"}}]}
```
