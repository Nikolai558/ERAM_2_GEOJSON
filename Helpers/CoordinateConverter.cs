using System;

namespace ERAM_2_GEOJSON.Helpers
{
    public class CoordinateConverter
    {
        public static double ConvertDMSToDecimal(string dms)
        {
            if (string.IsNullOrEmpty(dms) || dms.Length < 7)
            {
                throw new ArgumentException("Invalid DMS format. DMS should be at least 7 characters long.");
            }

            // Validate the direction, which should be at the end of the string and must be 'N', 'S', 'E', or 'W'
            char direction = dms[^1];
            if (direction != 'N' && direction != 'S' && direction != 'E' && direction != 'W')
            {
                throw new ArgumentException("Invalid DMS format. Direction must be 'N', 'S', 'E', or 'W'.");
            }

            // Determine if the input is latitude or longitude based on the length
            bool isLatitude = (dms.Length == 9); // Latitude should have 9 characters, including the direction

            // Extract the degree, minute, and second components for latitude and longitude accordingly
            string degreesPart, minutesPart, secondsPart, decimalSecondsPart;

            if (isLatitude)
            {
                degreesPart = dms.Substring(0, 2); // First 2 characters for degrees
                minutesPart = dms.Substring(2, 2); // Next 2 characters for minutes
                secondsPart = dms.Substring(4, 2); // Next 2 characters for seconds
                decimalSecondsPart = dms.Substring(6, dms.Length - 7); // Remaining characters before direction for decimalSeconds
            }
            else
            {
                degreesPart = dms.Substring(0, 3); // First 3 characters for degrees (longitude)
                minutesPart = dms.Substring(3, 2); // Next 2 characters for minutes
                secondsPart = dms.Substring(5, 2); // Next 2 characters for seconds
                decimalSecondsPart = dms.Substring(7, dms.Length - 8); // Remaining characters before direction for decimalSeconds
            }

            // Convert string components to integers or doubles
            if (!int.TryParse(degreesPart, out int degrees) ||
                !int.TryParse(minutesPart, out int minutes) ||
                !int.TryParse(secondsPart, out int seconds))
            {
                throw new ArgumentException("DMS components are not in the correct format.");
            }

            // If decimalSeconds is empty, assigns "0.0" as a double.
            // if not empty, concatenates "0." with the value of decimalSecondsPart as a double.
            double decimalSeconds = decimalSecondsPart == string.Empty ? 0.0 : Convert.ToDouble("0." + decimalSecondsPart);

            // Add decimalSeconds to seconds
            double secondsPlusDecimalSeconds = seconds + decimalSeconds;

            // Validate minutes and seconds ranges
            if (minutes < 0 || minutes >= 60 || secondsPlusDecimalSeconds < 0 || secondsPlusDecimalSeconds >= 60)
            {
                throw new ArgumentException("Invalid DMS values. Minutes and seconds must be between 0 and 59.");
            }

            // Convert DMS to decimal
            double decimalValue = degrees + (minutes / 60.0) + (secondsPlusDecimalSeconds / 3600.0);

            // Adjust for direction
            if (direction == 'S' || direction == 'W')
            {
                decimalValue *= -1;
            }

            return decimalValue;
        }
    }
}
