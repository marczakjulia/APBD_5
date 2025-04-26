namespace APBD5;

/// <summary>
/// Class which kinds works like Device but for the DataBase, since the Device is abstract
/// and i cannot use it technically(or i dont know how to use it).
/// </summary>
public class DeviceDTO
{
    public string Id { get; set; }
    public string Name { get; set; }
    public bool IsEnabled { get; set; }
}