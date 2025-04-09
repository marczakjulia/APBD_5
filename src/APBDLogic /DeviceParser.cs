namespace APBD5
{
    /// <summary>
    /// Class that contains methods which specify the parsing of different devices.
    /// </summary>
    public class DeviceParser : IDeviceParser
    {
        private const int MinimumRequiredElements = 4;
        private const int IndexPosition = 0;
        private const int DeviceNamePosition = 1;
        private const int EnabledStatusPosition = 2;

        /// <summary>
        /// Parsing PC and all of its components.
        /// </summary>
        /// <param name="line">The line from the file.</param>
        /// <param name="lineNumber">Which line is being read.</param>
        /// <exception cref="ArgumentException">Thrown if the line is corrupted.</exception>
        public PersonalComputer ParsePC(string line, int lineNumber)
        {
            const int SystemPosition = 3;
            var infoSplits = line.Split(',');
            if (infoSplits.Length < MinimumRequiredElements)
            {
                throw new ArgumentException($"Corrupted line {lineNumber}", line);
            }
            if (!bool.TryParse(infoSplits[EnabledStatusPosition], out bool _))
            {
                throw new ArgumentException($"Corrupted line {lineNumber}: can't parse enabled status for computer.", line);
            }
            return new PersonalComputer(infoSplits[IndexPosition], infoSplits[DeviceNamePosition], bool.Parse(infoSplits[EnabledStatusPosition]),
                infoSplits[SystemPosition]);
        }

        /// <summary>
        /// Parsing Smartwatch and all of its components.
        /// </summary>
        /// <param name="line">The line from the file.</param>
        /// <param name="lineNumber">Which line is being read.</param>
        /// <exception cref="ArgumentException">Thrown if the line is corrupted.</exception>
        public Smartwatch ParseSmartwatch(string line, int lineNumber)
        {
            const int BatteryPosition = 3;
            var infoSplits = line.Split(',');
            if (infoSplits.Length < MinimumRequiredElements)
            {
                throw new ArgumentException($"Corrupted line {lineNumber}", line);
            }
            if (!bool.TryParse(infoSplits[EnabledStatusPosition], out bool _))
            {
                throw new ArgumentException($"Corrupted line {lineNumber}: can't parse enabled status for smartwatch.", line);
            }
            // Remove "%" and parse battery level
            if (!int.TryParse(infoSplits[BatteryPosition].Replace("%", ""), out int batteryLevel))
            {
                throw new ArgumentException($"Corrupted line {lineNumber}: can't parse battery level for smartwatch.", line);
            }
            return new Smartwatch(infoSplits[IndexPosition], infoSplits[DeviceNamePosition], bool.Parse(infoSplits[EnabledStatusPosition]),
                batteryLevel);
        }

        /// <summary>
        /// Parsing ED and all of its components.
        /// </summary>
        /// <param name="line">The line from the file.</param>
        /// <param name="lineNumber">Which line is being read.</param>
        /// <exception cref="ArgumentException">Thrown if the line is corrupted.</exception>
        public Embedded ParseEmbedded(string line, int lineNumber)
        {
            const int IpAddressPosition = 3;
            const int NetworkNamePosition = 4;
            var infoSplits = line.Split(',');
            if (infoSplits.Length < MinimumRequiredElements + 1)
            {
                throw new ArgumentException($"Corrupted line {lineNumber}", line);
            }
            if (!bool.TryParse(infoSplits[EnabledStatusPosition], out bool _))
            {
                throw new ArgumentException($"Corrupted line {lineNumber}: can't parse enabled status for embedded device.", line);
            }
            return new Embedded(infoSplits[IndexPosition],
                infoSplits[DeviceNamePosition], bool.Parse(infoSplits[EnabledStatusPosition]), infoSplits[IpAddressPosition],
                infoSplits[NetworkNamePosition]);
        }

        /// <summary>
        /// A new function, that we call an instance of and it determines which device parsing function should be used based on the ID prefix.
        /// </summary>
        /// <param name="line">The line from the file.</param>
        /// <param name="lineNumber">Which line is being read.</param>
        public Device ParseDevice(string line, int lineNumber)
        {
            if (line.StartsWith("P-"))
            {
                return ParsePC(line, lineNumber);
            }
            else if (line.StartsWith("SW-"))
            {
                return ParseSmartwatch(line, lineNumber);
            }
            else if (line.StartsWith("ED-"))
            {
                return ParseEmbedded(line, lineNumber);
            }
            else
            {
                throw new ArgumentException($"Line {lineNumber} is corrupted.");
            }
        }
    }
}
