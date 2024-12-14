# ERAM_2_GEOJSON

## [DOWNLOAD](https://github.com/KSanders7070/ERAM_2_GEOJSON/releases/latest/download/ERAM_2_GEOJSON.exe)
Note: That link does not work yet.

## Purpose
Convert Real World ERAM Geomap.xml to usable geojson files usable by CRC. Will also read ConsoleCommandControl.xml, if found in the user-selected source directory, and create a comprehencsive .txt document from that data.

## Output Directory & Naming Format
The user will select an output directory and a new folder will be created there "ERAM_2_GEOJSON" where all output data will be stored. For each GeomapId in the Geomaps.xml, a new folder within ERAM_2_GEOJSON folder will be created with the name of the GeomapId and the LabelLine1 & LabelLine2.

User will be given the choice of output formats of "By Filters", "By Attributes", or a "Raw/Direct" conversion. For By Filters, additional sub-folders will be created to host all like-filter geojsons.

By Filters example:
 - ERAM_2_GEOJSON_OUTPUT
   - CENTER_CENTER-MAP
      - **Filter_01**
        - `Filter_01_Lines.geojson`
        - `Filter_01_Symbols.geojson`
        - `Filter_01_Text.geojson`
        - etc...
      - **Filter_04**
        - `Filter_04_Lines.geojson`
        - `Filter_04_Symbols.geojson`
        - etc...
      - **Multi-Filter_02_03_08**
        - `Multi-Filter_02_03_08_Lines.geojson`
        - etc...
      - **Multi-Filter_02_08**
        - `Multi-Filter_02_08_Lines.geojson`
        - etc...
   - OCP_OCP-MAP
      - **Filter_01**
        - `Filter_01_Lines.geojson`
        - etc...
      - **Filter_04**
        - `Filter_04_Lines.geojson`
        - `Filter_04_Symbols.geojson`
        - `Filter_04_Text.geojson`
        - etc...
   - TMU_TMU-MAP
      - **Filter_01**
        - `Filter_01_Lines.geojson`
        - `Filter_01_Symbols.geojson`
        - `Filter_01_Text.geojson`
        - etc...
      - **Multi-Filter_03_09_10_11_12_14**
        - `Multi-Filter_03_09_10_11_12_14_Symbols.geojson`
        - etc...

By Attributes example:
 - ERAM_2_GEOJSON_OUTPUT
   - CENTER_CENTER-MAP
      - Type AAV_Group 64_Object ZOB3NM_Filters 01_BCG 1_Style Solid_Thick 1_Lines.geojson
      - Type AIRPORT_Group 54_Filters 03 08_BCG 3_Style Airport_Font 4_Symbols.geojson
      - Type AIRWAY_Group 10_Object Q103_Filters 05_BCG 5_Style Solid_Thick 1_Lines.geojson
      - Type AIRWAY_Group 10_Object Q140_Filters 05_BCG 5_Style Solid_Thick 1_Lines.geojson
      - etc...
   - SOCMAP_SOC-MAP
      - Type AAV_Group 2_Object OLCAAV3_Filters 02_BCG 2_Style LongDashShortDash_Thick 1_Lines.geojson
      - Type AAV_Group 2_Object OLCAAV5_Filters 02_BCG 2_Style LongDashShortDash_Thick 1_Lines.geojson
      - Type AIRPORT_Group 1_Filters 02_BCG 2_Font 1_Underline F_X 12_Y 0_Text.geojson
      - Type AIRPORT_Group 1_Filters 02_BCG 2_Style Airport_Font 1_Symbols.geojson
      - etc...
   - TMU_TMU-MAP
      - Type AAV_Group 45_Object ZOB3NM_Filters 01_BCG 1_Style Solid_Thick 1_Lines.geojson
      - Type STAR_Group 57_Object CUUGR2_Filters 07_BCG 7_Style ShortDashed_Thick 1_Lines.geojson
      - Type STAR_Group 57_Object GRAYT2_Filters 07_BCG 7_Style ShortDashed_Thick 1_Lines.geojson
      - etc...

Raw convert example:
 - ERAM_2_GEOJSON_OUTPUT
    - CENTER_CENTER-MAP
      - CENTER_Record.geojson
   - SOCMAP_SOC-MAP
      - OCP_Record.geojson
   - TMU_TMU-MAP
      - TMU_Record.geojson

## Output Format Details
- By Filter
  - Lines:
    - All line objects that share the same filter assignments will placed into the same `_Lines.geojson` file.
    - All objects with identical MapObjectType, MapGroupId, and LineObjectId will be placed into the same Feature & efficient LineString handling will be implemented (if previous endingLat/Lon is the same as the current startLat/Lon, they will be merged into the same linestring and break into a multiLineString, otherwise.)
  - Symbols:
    - All symbol objects that share the same filter assignments will placed into the same `_Symbols.geojson` file.
    - Each symbol object will have its' own Feature.
  - Text:
    - All text objects that share the same filter assignments will placed into the same `_Text.geojson` file.
    - Each Text object will have its' own Feature with a property of "text: TextLines" if the DisplaySetting is set to True in the Geomap.xml for that text object, otherwise it will display as "E2G_text: TextLines"
- By Attribute
  - Lines:
    - All line objects that share the same MapObjectType, MapGroupId, LineObjectId, Filters, BcgGroup, LineStyle, & Thickness attribute assignments will placed into the same `_Lines.geojson` file and the named accordingly.
    - Efficient LineString handling will be implemented same as By Filter.
  - Symbols:
    - All symbol objects that share the same MapObjectType, MapGroupId, Filters, BCG, SymbolStyle, & FontSize attribute assignments will placed into the same `_Symbols.geojson` file and the named accordingly.
    - Each Symbol object will have its' own Feature.
  - Text:
    - All text objects that share the same MapObjectType, MapGroupId, Filters, BCG, Font, Underline, XPixelOffset, & YPixelOffset attribute assignments will placed into the same `_Text.geojson` file and the named accordingly.
    - Each Text object will have its' own Feature with a property of "text: TextLines" if the DisplaySetting is set to True in the Geomap.xml for that text object.
- Raw / Director Conversion
  - All objects, regardless of Line/Symbol/Text that share the same GeomapIds will placed into the same `GeomapId_Record.geojson` file.
  - Efficient LineString handling will not be implemented.
  - Each object will have its' own Feature.
  - Applied (default or overriding) attributes will be included in each feature as an overriding property readable by CRC.

## Pro / Con by Output Format
- Filter
  - Pros:
    - Small amount of files.
    - Quick to see data per filter.
  - Cons:
    - Manipulating/editing may prove to be slightly difficult.
    - Creates moderate size files.
- Attribute
  - Pros:
    - Variety to manipulate/edit as desired.
    - As close to real world control of data as possible with ease..
  - Cons:
    - Creates significant amounts of files
    - Creates moderate size files.
- Raw / Director Conversion
  - Pros:
    - Creates less files.
    - Very little setup required by FE.
    - As close to real world display as possible.
  - Cons:
    - Large files
    - Not easy to manipulate/edit.
    - isDefaults likely will not have an effect.
