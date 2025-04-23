using Microsoft.Data.SqlClient;

namespace APBD5;

public class DeviceService: IDeviceService
{
    private string _connectionString;
    public DeviceService(string connectionString)
    {
        _connectionString = connectionString;
    }
    public IEnumerable<DeviceDTO> GetAllDevices()
    {
        List<DeviceDTO> containers = [];
        string queryString = "SELECT * FROM Devices";
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            SqlCommand command = new SqlCommand(queryString, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var container = new DeviceDTO
                        {
                            Id = reader.GetString(3),
                            Name = reader.GetString(3),
                            IsEnabled = reader.GetBoolean(2),
                        };
                        containers.Add(container);
                    }
                }
            }
            finally
            {
                reader.Close();
            }
        }
        return containers;
    }

    public DeviceDTO? GetDeviceById(string id)
    {
        string queryString = "SELECT * FROM Devices WHERE Id = @id";
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            SqlCommand command = new SqlCommand(queryString, connection);
            command.Parameters.AddWithValue("@id", id); 
            connection.Open();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read()) 
                {
                    var device = new DeviceDTO
                    {
                        Id = reader.GetString(3),
                        Name = reader.GetString(3),
                        IsEnabled = reader.GetBoolean(2),
                    };
                    return device;
                }
            }
        }
        return null; 
    }
    
    public bool Create(Device device)
{
    string queryString = "INSERT INTO Devices (Id, Name, IsEnabled) VALUES (@Id, @Name, @IsEnabled)";
    if (device is Smartwatch)
    {
        queryString = "INSERT INTO Devices (Id, Name, IsEnabled, BatteryLevel) VALUES (@Id, @Name, @IsEnabled, @BatteryLevel)";
    }
    else if (device is Embedded)
    {
        queryString = "INSERT INTO Devices (Id, Name, IsEnabled, IpAddress, NetworkName) VALUES (@Id, @Name, @IsEnabled, @IpAddress, @NetworkName)";
    }
    else if (device is PersonalComputer)
    {
        queryString = "INSERT INTO Devices (Id, Name, IsEnabled, OperatingSystem) VALUES (@Id, @Name, @IsEnabled, @OperatingSystem)";
    }
    using (SqlConnection connection = new SqlConnection(_connectionString))
    {
        SqlCommand command = new SqlCommand(queryString, connection);
        connection.Open();
        command.Parameters.AddWithValue("@Id", device.Id);
        command.Parameters.AddWithValue("@Name", device.Name);
        command.Parameters.AddWithValue("@IsEnabled", device.IsEnabled);

        if (device is Smartwatch smartwatchDevice)
        {
            command.Parameters.AddWithValue("@BatteryLevel", smartwatchDevice.BatteryLevel);
        }
        else if (device is Embedded embeddedDevice)
        {
            command.Parameters.AddWithValue("@IpAddress", embeddedDevice.IpAddress);
            command.Parameters.AddWithValue("@NetworkName", embeddedDevice.NetworkName);
        }
        else if (device is PersonalComputer pcDevice)
        {
            command.Parameters.AddWithValue("@OperatingSystem", pcDevice.OperatingSystem);
        }

        int rowsAffected = command.ExecuteNonQuery();
        
        return rowsAffected > 0;
    }
}
    
    public bool Delete(string id)
    {
        string queryString = "DELETE FROM Devices WHERE Id = @Id";
        int countRowsDeleted = -1;
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            SqlCommand command = new SqlCommand(queryString, connection);
            connection.Open();
            command.Parameters.AddWithValue("@id", id); 
            countRowsDeleted = command.ExecuteNonQuery();
        }
        return countRowsDeleted == -1;
    }

    public bool Update(Device device)
{
    string queryString = "UPDATE Devices SET Name = @Name, IsEnabled = @IsEnabled WHERE Id = @Id";
    if (device is Smartwatch)
    {
        queryString = "UPDATE Devices SET Name = @Name, IsEnabled = @IsEnabled, BatteryLevel = @BatteryLevel WHERE Id = @Id";
    }
    else if (device is Embedded)
    {
        queryString = "UPDATE Devices SET Name = @Name, IsEnabled = @IsEnabled, IpAddress = @IpAddress, NetworkName = @NetworkName WHERE Id = @Id";
    }
    else if (device is PersonalComputer)
    {
        queryString = "UPDATE Devices SET Name = @Name, IsEnabled = @IsEnabled, OperatingSystem = @OperatingSystem WHERE Id = @Id";
    }
    using (SqlConnection connection = new SqlConnection(_connectionString))
    {
        SqlCommand command = new SqlCommand(queryString, connection);
        connection.Open();
        command.Parameters.AddWithValue("@Id", device.Id);
        command.Parameters.AddWithValue("@Name", device.Name);
        command.Parameters.AddWithValue("@IsEnabled", device.IsEnabled);

        if (device is Smartwatch smartwatchDevice)
        {
            command.Parameters.AddWithValue("@BatteryLevel", smartwatchDevice.BatteryLevel);
        }
        else if (device is Embedded embeddedDevice)
        {
            command.Parameters.AddWithValue("@IpAddress", embeddedDevice.IpAddress);
            command.Parameters.AddWithValue("@NetworkName", embeddedDevice.NetworkName);
        }
        else if (device is PersonalComputer pcDevice)
        {
            command.Parameters.AddWithValue("@OperatingSystem", pcDevice.OperatingSystem);
        }

        int rowsAffected = command.ExecuteNonQuery();
        
        return rowsAffected > 0;
    }
}

    

}