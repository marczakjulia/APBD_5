namespace APBD5;

public class DeviceValidator : IDeviceValidator
{
    public void ValidateDevice(DeviceDTO device)
    {
        if (device == null)
            throw new ArgumentNullException(nameof(device));
        if (string.IsNullOrWhiteSpace(device.Name))
            throw new ArgumentException("name is required", nameof(device.Name));

        switch (device)
        {
            case SmartwatchDTO sw:
                if (sw.BatteryLevel < 0 || sw.BatteryLevel > 100)
                    throw new ArgumentException("battery level invalid", nameof(sw.BatteryLevel));
                break;

            case PersonalComputerDTO pc:
                if (pc.IsEnabled && string.IsNullOrWhiteSpace(pc.OperatingSystem))
                    throw new ArgumentException("OS is required for PC", nameof(pc.OperatingSystem));
                break;

            case EmbeddedDTO ed:
                if (!System.Net.IPAddress.TryParse(ed.IpAddress, out _))
                    throw new ArgumentException("Invalid IP address", nameof(ed.IpAddress));
                if (string.IsNullOrWhiteSpace(ed.NetworkName))
                    throw new ArgumentException("network is required", nameof(ed.NetworkName));
                break;

            default:
                throw new ArgumentException("unknown device", nameof(device));
        }
    }
}
