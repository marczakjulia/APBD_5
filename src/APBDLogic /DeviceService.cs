namespace APBD5
{
    /// <summary>
    ///Class that is responbile for the turning logic of device manager 
    /// </summary>
    public class DeviceService
    {
        private readonly IDeviceRepository _deviceRepository;

        public DeviceService(IDeviceRepository deviceRepository)
        {
            _deviceRepository = deviceRepository;
        }
    }
}
