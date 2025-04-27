using System;

namespace APBD5
{
    /// <summary>
    /// Class that is responsible for instance of device.
    /// </summary>
    public class Embedded : Device
    {
        /// <summary>
        /// Validating the ID for embedded.
        /// </summary>
        public static IIdValidator IdValidator { get; set; } =
            new EmbeddedDeviceIdValidator();

        public string NetworkName { get; set; }

        private string _ipAddress;

        public string IpAddress
        {
            get => _ipAddress;
            set
            {
                if (!IpAddressValidator.IsValid(value))
                {
                    throw new ArgumentException("Wrong IP address format.");
                }

                _ipAddress = value;
            }
        }

        private bool _isConnected = false;
        
        public Embedded() { }

        public Embedded(string id, string name, bool isEnabled, string ipAddress, string networkName)
            : base(id, name, isEnabled)
        {
            IdValidator.ValidateOrThrow(id);
            IpAddress = ipAddress;
            NetworkName = networkName;
        }

        public override void TurnOn()
        {
            Connect();
            base.TurnOn();
        }

        public override void TurnOff()
        {
            _isConnected = false;
            base.TurnOff();
        }

        public override string ToString()
        {
            string enabledStatus = IsEnabled ? "enabled" : "disabled";
            return $"Embedded device {Name} ({Id}) is {enabledStatus} and has IP address {IpAddress}";
        }

        private void Connect()
        {
            if (NetworkName.Contains("MD Ltd."))
            {
                _isConnected = true;
            }
            else
            {
                throw new ConnectionException();
            }
        }

        /// <summary>
        /// Function override from the device that later is used for saving.
        /// </summary>
        public override string ToSavingString()
        {
            return $"{Id},{Name},{IsEnabled},{IpAddress},{NetworkName}";
        }
    }
}
