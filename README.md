# ERAM_2_GEOJSON

## [DOWNLOAD](https://github.com/KSanders7070/ERAM_2_GEOJSON/releases/latest/download/ERAM_2_GEOJSON)
Note: That link does not work yet.

## Purpose
Something here about purpose

## Output Directory & Format
The user will select an output directory and a new folder will be created there "ERAM_2_GEOJSON" where all output data will be stored. For each GeomapId in the Geomaps.xml, a new folder within ERAM_2_GEOJSON folder will be created with the name of the GeomapId and the LabelLine1 & LabelLine2.

By Filters example:
 - ERAM_2_GEOJSON_OUTPUT
   -  CENTER_CENTER-MAP
     - **Filter_01**
       - `Filter_01_Lines.geojson`
       - `Filter_01_Symbols.geojson`
       - `Filter_01_Text.geojson`
     - **Filter_04**
       - `Filter_04_Lines.geojson`
       - `Filter_04_Symbols.geojson`
    - **Multi-Filter_02_03_08**
       - `Multi-Filter_02_03_08_Lines.geojson`
    - **Multi-Filter_02_08**
      - `Multi-Filter_02_08_Lines.geojson`
  - OCP_OCP-MAP
    - **Filter_01**
      - `Filter_01_Lines.geojson`
    - **Filter_04**
      - `Filter_04_Lines.geojson`
      - `Filter_04_Symbols.geojson`
      - `Filter_04_Text.geojson`
  - TMU_TMU-MAP
    - **Filter_01**
      - `Filter_01_Lines.geojson`
      - `Filter_01_Symbols.geojson`
      - `Filter_01_Text.geojson`
    - **Multi-Filter_03_09_10_11_12_14**
      - `Multi-Filter_03_09_10_11_12_14_Symbols.geojson`

User will be given the choice of output formats of:

- By Filter
  - 
- By Attribute
- Raw