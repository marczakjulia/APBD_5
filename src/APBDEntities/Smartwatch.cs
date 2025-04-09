namespace APBD5
{
    /// <summary>
    /// Smartwatch class, with battery logic handled by a separate class and ID validation as well.
    /// </summary>
    public class Smartwatch : Device
    {
        public static IIdValidator IdValidator { get; set; } = new SmartWatchIdValidator();
        private int _batteryLevel;

        public int BatteryLevel
        {
            get => _batteryLevel;
            set
            {
                if (value < 0 || value > 100)
                {
                    throw new ArgumentException("Battery level must be between 0 and 100.");
                }
                _batteryLevel = value;
                if (_batteryLevel < 20)
                {
                    Notify(_batteryLevel);
                }
            }
        }

        public Smartwatch(string id, string name, bool isEnabled, int batteryLevel)
            : base(id, name, isEnabled)
        {
            IdValidator.ValidateOrThrow(id);
            BatteryLevel = batteryLevel;
        }

        public void Notify(int batteryLevel)
        {
            Console.WriteLine($"Battery level is low. Current level is: {batteryLevel}%");
        }

        public override void TurnOn()
        {
            if (BatteryLevel < 11)
            {
                throw new EmptyBatteryException();
            }

            base.TurnOn();
            BatteryLevel -= 10;

            if (BatteryLevel < 20)
            {
                Notify(BatteryLevel);
            }
        }

        public override string ToString()
        {
            string enabledStatus = IsEnabled ? "enabled" : "disabled";
            return $"Smartwatch {Name} ({Id}) is {enabledStatus} and has {BatteryLevel}% battery";
        }

        public override string ToSavingString()
        {
            return $"{Id},{Name},{IsEnabled},{BatteryLevel}%";
        }
    }
}