namespace APBD5;

public interface IDeviceRepository
{
    IEnumerable<DeviceDTO> GetAllDevices();
    
    DeviceDTO? GetDeviceById(string id);
    bool Create(DeviceDTO devic);
    bool Delete(string id);
    bool Update(DeviceDTO device);
}