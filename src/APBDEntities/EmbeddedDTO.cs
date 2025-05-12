namespace APBD5
{
    /// <summary>
    /// DTO for Embedded Device.
    /// </summary>
    public class EmbeddedDTO : DeviceDTO
    {
        public string IpAddress { get; set; }
        public string NetworkName { get; set; }
    }
}