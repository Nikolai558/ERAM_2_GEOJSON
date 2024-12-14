# ERAM_2_GEOJSON

## [DOWNLOAD](https://github.com/KSanders7070/ERAM_2_GEOJSON/releases/latest/download/ERAM_2_GEOJSON)
Note: That link does not work yet.

## Purpose
Something here about purpose

## Output Directory & Format
The user will select an output directory and a new folder will be created there "ERAM_2_GEOJSON" where all output data will be stored. For each GeomapId in the Geomaps.xml, a new folder within ERAM_2_GEOJSON folder will be created with the name of the GeomapId and the LabelLine1 & LabelLine2.

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



User will be given the choice of output formats of:

- By Filter
  - 
- By Attribute
- Raw