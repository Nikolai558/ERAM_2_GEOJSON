using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Models
{
    [XmlRoot("ConsoleCommandControl_Records")]
    public class ConsoleCommandControl_Records
    {
        [XmlElement("MapBrightnessMenu")]
        public List<MapBrightnessMenu> MapBrightnessMenu { get; set; }

        [XmlElement("MapFilterMenu")]
        public List<MapFilterMenu> MapFilterMenu { get; set; }

        [XmlElement("SDKeypadKeys")]
        public SDKeypadKeys SDKeypadKeys { get; set; }

        [XmlElement("CPDLCKeys")]
        public CPDLCKeys CPDLCKeys { get; set; }

        [XmlElement("ActiveBorderKeys")]
        public ActiveBorderKeys ActiveBorderKeys { get; set; }

        [XmlElement("RPositionKeys")]
        public List<RPositionKeys> RPositionKeys { get; set; }

        [XmlElement("ATSpecialistKeys")]
        public List<ATSpecialistKeys> ATSpecialistKeys { get; set; }
    }

    public class MapBrightnessMenu
    {
        [XmlElement("BCGMenuName")]
        public string BCGMenuName { get; set; }

        [XmlElement("MapBCGButton")]
        public List<MapBCGButton> MapBCGButton { get; set; }
    }

    public class MapBCGButton
    {
        [XmlElement("MenuPosition")]
        public int MenuPosition { get; set; }

        [XmlElement("Label")]
        public string Label { get; set; }

        [XmlElement("MapBCGGroups")]
        public MapBCGGroups MapBCGGroups { get; set; }
    }

    public class MapBCGGroups
    {
        [XmlElement("MapBCGGroup")]
        public List<int> MapBCGGroup { get; set; }
    }

    public class MapFilterMenu
    {
        [XmlElement("FilterMenuName")]
        public string FilterMenuName { get; set; }

        [XmlElement("MapFilterButton")]
        public List<MapFilterButton> MapFilterButton { get; set; }
    }

    public class MapFilterButton
    {
        [XmlElement("MenuPosition")]
        public int MenuPosition { get; set; }

        [XmlElement("DefaultSetting")]
        public string DefaultSetting { get; set; }

        [XmlElement("LabelLine1")]
        public string LabelLine1 { get; set; }

        [XmlElement("LabelLine2")]
        public string LabelLine2 { get; set; }

        [XmlElement("MapFilterGroups")]
        public MapFilterGroups MapFilterGroups { get; set; }
    }

    public class MapFilterGroups
    {
        [XmlElement("MapFilterGroup")]
        public List<int> MapFilterGroup { get; set; }
    }

    public class SDKeypadKeys
    {
        [XmlElement("KeypadKey")]
        public List<string> KeypadKey { get; set; }
    }

    public class CPDLCKeys
    {
        [XmlElement("CPDLCEnterKeyID")]
        public int CPDLCEnterKeyID { get; set; }
    }

    public class ActiveBorderKeys
    {
        [XmlElement("ActiveBorderEnterKeyID")]
        public int ActiveBorderEnterKeyID { get; set; }
    }

    public class RPositionKeys
    {
        [XmlElement("FunctionKeyID")]
        public int FunctionKeyID { get; set; }

        [XmlElement("KeyType")]
        public string KeyType { get; set; }

        [XmlElement("DisplayText")]
        public string DisplayText { get; set; }
    }

    public class ATSpecialistKeys
    {
        [XmlElement("ATSpecialistID")]
        public string ATSpecialistID { get; set; }

        [XmlElement("ATSpecialistKey")]
        public List<ATSpecialistKey> ATSpecialistKey { get; set; }
    }

    public class ATSpecialistKey
    {
        [XmlElement("FunctionKeyID")]
        public int FunctionKeyID { get; set; }

        [XmlElement("KeyType")]
        public string KeyType { get; set; }

        [XmlElement("DisplayText")]
        public string DisplayText { get; set; }
    }
}
