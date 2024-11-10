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

            // Extract the degree, minute, and second components
            string degreesPart = dms.Substring(0, dms.Length - 6);
            string minutesPart = dms.Substring(dms.Length - 6, 2);
            string secondsPart = dms.Substring(dms.Length - 4, 2);
            string decimalSecondsPart = dms.Length > 7 ? dms.Substring(dms.Length - 3, 1) : "0";

            if (!int.TryParse(degreesPart, out int degrees) ||
                !int.TryParse(minutesPart, out int minutes) ||
                !int.TryParse(secondsPart, out int seconds) ||
                !int.TryParse(decimalSecondsPart, out int decimalSeconds))
            {
                throw new FormatException("DMS components are not in the correct format.");
            }

            // Validate degrees, minutes, and seconds
            if (minutes < 0 || minutes >= 60 || seconds < 0 || seconds >= 60)
            {
                throw new ArgumentException("Invalid DMS values. Minutes and seconds must be between 0 and 59.");
            }

            // Convert DMS to decimal
            double decimalValue = degrees + (minutes / 60.0) + ((seconds + (decimalSeconds / 10.0)) / 3600.0);

            // Adjust for direction
            if (direction == 'S' || direction == 'W')
            {
                decimalValue *= -1;
            }

            return Math.Round(decimalValue, 8);
        }
    }
}
