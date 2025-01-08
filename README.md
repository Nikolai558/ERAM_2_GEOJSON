# ERAM_2_GEOJSON

Special Thanks to Kyle Sanders for creating this app! Hosting on my account in order to make the transition into FE-Buddy easier.

## [DOWNLOAD](https://github.com/Nikolai558/ERAM_2_GEOJSON/releases/latest/download/E2G.exe)
*Change Log located at the end of this document*

---

## Purpose
ERAM_2_GEOJSON (E2G) converts Real World ERAM `Geomaps.xml` files into GeoJSON files compatible with the CRC program. Additionally, if the `ConsoleCommandControl.xml` file is present in the same directory as `Geomaps.xml`, the tool generates a comprehensive `.txt` file detailing the menu labels and positions used by each GeoMap.

---

## System Requirements
1. Windows 10 Version 1809 Build 17763 or later. (or Windows Server 2019-Build 17763 or later)
2. Processor: 64bit with at least a 1.6 GHz clock speed or equivalent.
3. Memory (RAM): At least 2GB. However, 4 GB is recommended for smoother performance.

---

## Use
1. Ensure the `Geomaps.xml` file in an unzipped folder/directory.
2. If available, include the `ConsoleCommandControl.xml` file in the same directory.
3. Download and launch `E2G.exe`, then follow the on-screen prompts.

---

## Output Directory & Naming Format
The tool creates an `E2G_OUTPUT` folder in the selected output directory. If output by Filters or Attributes, a subfolder named `<GeomapId>_<LabelLine1>_<LabelLine2>` is created for each `GeomapId` in `Geomaps.xml`. Ex: `CENTER_CENTER-MAP`

For Raw output, the `<GeomapId>_<LabelLine1>_<LabelLine2>` is included in the file name instead of a subdirectory.

### Output Formats
1. **By Filters**: Groups GeoJSON files by shared Filter Group assignments.
2. **By Attributes**: Groups GeoJSON files by shared object attributes.
3. **Raw (1-for-1)**: Produces an exact conversion with overriding properties in each feature.

---

## Example Output Trees

### By Filters
```
E2G_OUTPUT\
	CENTER_CENTER-MAP\
		Filter_01\
			Filter_01_Lines.geojson
			Filter_01_Symbols.geojson
			Filter_01_Text.geojson
		Filter_04\
			Filter_04_Lines.geojson
		 Multi-Filter_02_03_08\
			Multi-Filter_02_03_08_Lines.geojson
```

### By Attributes
```
E2G_OUTPUT\
	CENTER_CENTER-MAP\
		BCG 01_Filters 01_Type AAV_Group 64_Object ZOB3NM_Style Solid_Thick 1_Lines.geojson
		BCG 01_Filters 01_Type SupplementalLine_Group 48_Object AP_EAST_TOOTH_BOX_Style Solid_Thick 1_Lines.geojson
		BCG 02_Filters 02 03 08_Type SupplementalLine_Group 44_Object AP_PCT_BINNSLUCKE_Style ShortDashed_Thick 1_Lines.geojson
```

### Raw Conversion
```
E2G_OUTPUT\
	CENTER_CENTER-MAP.geojson
	OCP_OCP-MAPS.geojson
```

---

## Output Format Details

### By Filters
- **Lines**: Groups line objects by Filter Group assignments into the same file. "Efficient LineString Handling" merges consecutive points into a single `LineString` or `MultiLineString` feature if the `MapObjectType`, `MapGroupId`, and `LineObjectId` are identical.
- **Symbols**: Groups symbol objects by filter assignments into the same file. Each symbol is a separate feature.
- **Text**: Groups text objects by filter assignments into the same file. Features have a property `text: <TextLines>` if the `DisplaySetting` value is `True`, otherwise `E2G_text: <TextLines>` (preventing display in CRC).

### By Attributes
- **Lines**: Groups line objects by shared attributes such as `MapObjectType`, `Filters`, and `LineStyle` into the same file. Efficient `LineString` handling is applied. File name is comprised of the shared attributes and values.
- **Symbols**: Groups symbol objects by attributes like `Filters`, `SymbolStyle`, and `FontSize` into the same file. Each symbol is a separate feature. File name is comprised of the shared attributes and values.
- **Text**: Groups text objects by attributes such as `Filters`, `Font`, and `Underline` into the same file. Features have a property `text: <TextLines>` if the `DisplaySetting` value is `True`, otherwise `E2G_text: <TextLines>` (preventing display in CRC). File name is comprised of the shared attributes and values.

### Raw Conversion
- Combines all objects for each `GeomapId` into a single file.
- No grouping or Efficient LineString Handling is applied.
- Overriding properties will be found in each feature utilizing the assigned/applied characteristics found in either the default or overriding properties section of the Geomaps file.

---

## isDefaults

**What are isDefaults?**
- `isLineDefaults`
- `isSymbolDefaults`
- `isTextDefaults`

isDefaults are features included in a geojson that contain all default attributes to be applied to all other features within the geojson file if not defined in its' own property section (overriding properties).  
For more information, refer to the [vNAS Documentation](https://data-admin.vnas.vatsim.net/docs/#/video-maps?id=map-defaults).

`isDefaults` features are included in GeoJSON files when the user selects By Filters or By Attributes formats. For Raw/Direct Conversion, `isDefaults` is unnecessary because each feature contains all required attributes within each feature properties section as an overriding property.

---

## Pros & Cons of Output Formats

### By Filters
- **Pros**:
  - Fewer files.
  - Short file names.
  - Quick to view data by filter.
  - isDefaults features created for you.
- **Cons**:
  - Moderate file size.
  - Less flexible for editing.

### By Attributes
- **Pros**:
  - Highly customizable and flexible for editing.
  - Closely represents real-world control of data.
  - isDefaults features created for you.
- **Cons**:
  - Large number of files.
  - Long file names.
  - Moderate file size.

### Raw Conversion
- **Pros**:
  - Minimal setup required.
  - One file per Geomap ID.
  - Short File Names.
- **Cons**:
  - Large file sizes.
  - Difficult to manipulate or edit.
  - isDefaults features not created because all features have overriding properties.

---

## Properties

Users have the option to include additional custom properties in each GeoJSON feature. These properties help organize and review data but have no effect in CRC.

*Note: Electing into including these custom properties will increase the data size of your output files by approximately 40%.*

### Lines:
```json
{
  "E2G_MapObjectType": "<MapObjectType>",
  "E2G_MapGroupId": "<MapGroupId>",
  "E2G_LineObjectId": "<LineObjectId>"
}
```

### Symbols & Text:
```json
{
  "E2G_MapObjectType": "<MapObjectType>",
  "E2G_MapGroupId": "<MapGroupId>",
  "E2G_SymbolId": "<SymbolId>"
}
```

---

## Developer Notes

### Recommended: Use By-Attribute Output Format

Unless your facility can get updated GeoMap.xml data on a regular basis, it is recommended to use the By-Attribute output format, even though this format will require some initial effort on part of the FE team.

**Why?** By-Attribute files allow you to use, delete, or edit objects separately easier than them being all together in the same file. Let's say you use FE-Buddy or some other resource to update airways, SIDs/STARS, etc..., when constructing your Geomap with By-Attribute files, you can easily choose to leave those files out of the Geomap. You can also more easily edit objects like sector data to match your local SOPs/LOAs.

### Recommended Workflow

1. **Reference the Raw Conversion**  
   In CRC, make a GeoMap for the Raw conversion file. Use this as a guide to understand how the data should be displayed.

2. **Construct a New GeoMap**  
   - Utilize the By-Attribute files to create a new GeoMap.  
   - You can load each file into a GeoJSON viewer, such as [geojson.tools](https://geojson.tools/), for easier inspection.  
   - Rename each file to reflect its contents, incorporating any naming conventions used by your facility.

3. **Validate the New GeoMap**  
   - Compare the new GeoMap you’ve constructed with the Raw GeoMap.  
   - Ensure that the new GeoMap matches the Raw GeoMap as closely as desired.  

4. **Maintain and Manipulate the Data**  
   Once the new GeoMap is finalized, it is recommended to manipulate and maintain this data.

---

## CHANGE LOG

### v1.0.0rc1 (05JAN2025)
- Testing versioncheck

- Implemented

### v0.1.3 (05JAN2025)

- Implemented Change Log
- By Raw output no longer has GeoMapID subdirectories, instead the file name contains the ID and Label1/2 data.
- GeoMapId and Label1/2 now run through a FileNameSanitizer prior to use in directory/file names to remove illegal characters.
- Added data in UI for users.

### v0.1.2 (02JAN2025)

- .EXE created
- Shared for testing on mulitple systems