namespace APBD5;

public class DeviceService : IDeviceService
{
    private readonly DeviceRepository _deviceRepository;

    public DeviceService(DeviceRepository deviceRepository)
    {
        _deviceRepository = deviceRepository;
    }

    public IEnumerable<DeviceDTO> GetAllDevices()
    {
        try
        {
            return _deviceRepository.GetAllDevices();
        }
        catch (Exception ex)
        {
            throw new Exception("Error retrieving devices", ex);
        }
    }

    public DeviceDTO? GetDeviceById(string id)
    {
        try
        {
            return _deviceRepository.GetDeviceById(id);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error retrieving device with id {id}", ex);
        }
    }

    public bool Create(DeviceDTO device)
    {
        try
        {
            DeviceValidator validator = new DeviceValidator();
            validator.ValidateDevice(device);
            string prefix = device switch
            {
                SmartwatchDTO _ => "SW-",
                PersonalComputerDTO _ => "P-",
                EmbeddedDTO _ => "ED-",
                _ => throw new ArgumentException("Unknown device type", nameof(device))
            };

            int counter = 1;
            string newId;
            while (true)
            {
                newId = $"{prefix}{counter}";
                if (_deviceRepository.GetDeviceById(newId) == null)
                    break;
                counter++;
            }
            device.Id = newId;

            return _deviceRepository.Create(device);
        }
        catch (Exception ex)
        {
            throw new Exception("Error creating device", ex);
        }
    }

    public bool Update(DeviceDTO device)
    {
        try
        {
            DeviceValidator validator = new DeviceValidator();
            validator.ValidateDevice(device);
            return _deviceRepository.Update(device);
        }
        catch (Exception ex)
        {
            throw new Exception("Error updating device", ex);
        }
    }

    public bool Delete(string id)
    {
        try
        {
            return _deviceRepository.Delete(id);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error deleting device with id {id}", ex);
        }
    }
}
