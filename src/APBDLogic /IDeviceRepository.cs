namespace APBD5
{
    /// <summary>
    /// Repo for managing the functins in the DeviceManager
    /// </summary>
    public interface IDeviceRepository
    {
        void AddDevice(Device newDevice);
        void EditDevice(Device editDevice);
        void RemoveDeviceById(string deviceId);
        Device GetDeviceById(string id);
    }
}
