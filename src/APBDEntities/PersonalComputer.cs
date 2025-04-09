namespace APBD5
{
    /// <summary>
    /// Class that is responsible for instance of device.
    /// </summary>
    public class PersonalComputer : Device
    {
        public static IIdValidator IdValidator { get; set; } = new PersonalComputerIdValidator();

        public string OperatingSystem { get; set; }

        public PersonalComputer(string id, string name, bool isEnabled, string operatingSystem)
            : base(id, name, isEnabled)
        {
            IdValidator.ValidateOrThrow(id);
            OperatingSystem = operatingSystem;
        }

        public override void TurnOn()
        {
            if (OperatingSystem == null)
            {
                throw new EmptySystemException();
            }
            base.TurnOn();
        }

        public override string ToString()
        {
            string enabledStatus = IsEnabled ? "enabled" : "disabled";
            string osStatus = OperatingSystem == null ? "has no OS" : $"has {OperatingSystem}";
            return $"PC {Name} ({Id}) is {enabledStatus} and {osStatus}";
        }

        public override string ToSavingString()
        {
            return $"{Id},{Name},{IsEnabled},{OperatingSystem}";
        }
    }
}