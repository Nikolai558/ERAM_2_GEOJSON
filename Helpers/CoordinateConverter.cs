using System;

namespace ERAM_2_GEOJSON.Helpers
{
    public static class CoordinateConverter
    {
        public static double ConvertDMSToDecimal(string dms)
        {
            if (string.IsNullOrEmpty(dms) || dms.Length < 9)
                throw new ArgumentException("Invalid DMS coordinate format.");

            // Example input: "42124729N"
            int degrees = int.Parse(dms.Substring(0, 2));
            int minutes = int.Parse(dms.Substring(2, 4)) / 100;
            double seconds = double.Parse(dms.Substring(6, 2) + "." + dms.Substring(8, 1));

            double decimalCoordinate = degrees + (minutes / 60.0) + (seconds / 3600.0);

            // Apply direction
            if (dms.EndsWith("S") || dms.EndsWith("W"))
            {
                decimalCoordinate *= -1;
            }

            return Math.Round(decimalCoordinate, 8);
        }
    }
}
