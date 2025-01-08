// File: Helpers/ConsoleCommandControlTxtGenerator.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Models;

namespace Helpers
{
    public class ConsoleCommandControlTxtGenerator
    {
        /// <summary>
        /// Generates ConsoleCommandControl.txt from the data contained within ConsoleCommandControl.xml.
        /// </summary>
        public static void GenerateTxtFile(
            string outputPath,
            ConsoleCommandControl_Records records,
            Dictionary<string, HashSet<string>> bcgMenuToGeomapIds,
            Dictionary<string, HashSet<string>> filterMenuToGeomapIds)
        {
            var sb = new StringBuilder();

            sb.AppendLine(":::::::::::::::::::::::::::::::::::");
            sb.AppendLine("::   Brightness Control Groups   ::");
            sb.AppendLine(":::::::::::::::::::::::::::::::::::\n");

            foreach (var menu in records.MapBrightnessMenu)
            {
                sb.AppendLine($"BCG Menu: {menu.BCGMenuName}");

                // Adds a list of all Geomaps that use this BCG Group.
                if (bcgMenuToGeomapIds.ContainsKey(menu.BCGMenuName))
                {
                    sb.AppendLine($"\n\tUsed with:\t{string.Join(", ", bcgMenuToGeomapIds[menu.BCGMenuName])}\n");
                }
                else
                {
                    sb.AppendLine($"\n\tUsed with:\tNone\n");
                }

                // List the BCG buttons and their associated name and positions.
                foreach (var button in menu.MapBCGButton)
                {
                    sb.AppendLine($"\tLabel:\t\t{button.Label}");
                    sb.AppendLine($"\tPosition:\t{button.MenuPosition}");
                    sb.AppendLine($"\tGroup:\t\t{string.Join(", ", button.MapBCGGroups?.MapBCGGroup ?? new List<int>())}\n");
                }
                sb.AppendLine();
            }

            sb.AppendLine(":::::::::::::::::::::::::::::::::::");
            sb.AppendLine("::        Filter Groups          ::");
            sb.AppendLine(":::::::::::::::::::::::::::::::::::\n");

            foreach (var filterMenu in records.MapFilterMenu)
            {
                sb.AppendLine($"FilterMenu: {filterMenu.FilterMenuName}");

                // Adds a list of all Geomaps that use this Filter Group.
                if (filterMenuToGeomapIds.ContainsKey(filterMenu.FilterMenuName))
                {
                    sb.AppendLine($"\n\tUsed with:\t{string.Join(", ", filterMenuToGeomapIds[filterMenu.FilterMenuName])}\n");
                }
                else
                {
                    sb.AppendLine($"\n\tUsed with:\tNone\n");
                }

                // List the Filters buttons and their associated name and positions.
                foreach (var button in filterMenu.MapFilterButton)
                {
                    sb.AppendLine($"\tLabel:\t\t{button.LabelLine1}");
                    if (!string.IsNullOrWhiteSpace(button.LabelLine2))
                    {
                        sb.AppendLine($"\t\t\t\t{button.LabelLine2}");
                    }
                    sb.AppendLine($"\tPosition:\t{button.MenuPosition}");
                    sb.AppendLine($"\tGroup:\t\t{string.Join(", ", button.MapFilterGroups?.MapFilterGroup ?? new List<int>())}\n");
                }
                sb.AppendLine();
            }

            // Write the formatted text to the file
            File.WriteAllText(outputPath, sb.ToString());
        }
    }
}
