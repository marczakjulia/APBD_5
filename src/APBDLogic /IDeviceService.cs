namespace APBD5;

public interface IDeviceService
{
    IEnumerable<DeviceDTO> GetAllDevices();
    
    DeviceDTO? GetDeviceById(string id);
    bool Create(Device device);
    bool Delete(string id);
    bool Update(Device device);
}