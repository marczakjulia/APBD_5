using Microsoft.Data.SqlClient;

namespace APBD5;

public class DeviceService : IDeviceService
{
    private readonly string _connectionString;

    public DeviceService(string connectionString)
    {
        _connectionString = connectionString;
    }
//works
    public IEnumerable<DeviceDTO> GetAllDevices()
    {
        var devices = new List<DeviceDTO>();
        string querystring = "SELECT * FROM Device";
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            SqlCommand command = new SqlCommand(querystring, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                devices.Add(new DeviceDTO
                {
                    Id = reader.GetString(0),
                    Name = reader.GetString(1),
                    IsEnabled = reader.GetBoolean(2),
                });
            }
            reader.Close();
            return devices;
        }
    }

    public DeviceDTO? GetDeviceById(string id)
    {
        string querystring = "SELECT * FROM Device WHERE Id = @id";
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            SqlCommand command = new SqlCommand(querystring, connection);
            command.Parameters.AddWithValue("@Id", id);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            if (!reader.Read()) return null;
            return new DeviceDTO
            {
                Id = reader.GetString(0),
                Name = reader.GetString(1),
                IsEnabled = reader.GetBoolean(2),
            };
        }
    }

    //works
    public bool Create(Device device)
    {
        //function to validate beforehand hence no transaction now used 
        ValidateDevice(device);
        
        string prefix = device switch
        {
            Smartwatch _        => "SW-",
            PersonalComputer _  => "P-",
            Embedded _          => "ED-",
            _ => throw new ArgumentException("Unknown type", nameof(device))
        };
        int counter = 1;
        string newId;
        //creating my id in a way that i just check which next is null - keeps it neat so i have all in order
        //not like SW-1O, SW-2445 ... if i used a counter of come sorts to assign values 
        while (true)
        {
            newId = $"{prefix}{counter}";
            if (GetDeviceById(newId) == null)
                break;
            counter++;
        }
        device.Id = newId;
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            const string sql = "INSERT INTO Device (Id, Name, IsEnabled) VALUES (@Id, @Name, @IsEnabled)";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", device.Id);
            command.Parameters.AddWithValue("@Name", device.Name);
            command.Parameters.AddWithValue("@IsEnabled", device.IsEnabled);
            if (command.ExecuteNonQuery() != 1)
                throw new Exception("Failed to insert device");
                
            if (device is Smartwatch sw)
            {
                const string swSql =
                    "INSERT INTO Smartwatch (BatteryPercentage, DeviceId) VALUES (@BatteryPercentage, @Id)";
                using var swCmd = new SqlCommand(swSql, connection);
                swCmd.Parameters.AddWithValue("@BatteryPercentage", sw.BatteryLevel);
                swCmd.Parameters.AddWithValue("@Id", device.Id);
                if (swCmd.ExecuteNonQuery() != 1)
                    throw new Exception("Failed to insert smartwatch");
            }
            else if (device is PersonalComputer pc)
            {
                const string pcSql =
                    "INSERT INTO PersonalComputer (OperationSystem, DeviceId) VALUES (@OperationSystem, @Id)";
                using var pcCmd = new SqlCommand(pcSql, connection);
                pcCmd.Parameters.AddWithValue("@OperationSystem", pc.OperatingSystem);
                pcCmd.Parameters.AddWithValue("@Id", device.Id);
                if (pcCmd.ExecuteNonQuery() != 1)
                    throw new Exception("Failed to insert pc");
            }
            else if (device is Embedded e)
            {
                const string edSql = "INSERT INTO Embedded (IpAddress, NetworkName, DeviceId) VALUES (@IpAddress, @NetworkName, @Id)";
                using var edCmd = new SqlCommand(edSql, connection);
                edCmd.Parameters.AddWithValue("@IpAddress", e.IpAddress);
                edCmd.Parameters.AddWithValue("@NetworkName", e.NetworkName);
                edCmd.Parameters.AddWithValue("@Id", device.Id);
                if (edCmd.ExecuteNonQuery() != 1)
                    throw new Exception("Failed to insert embedded");
            }
        }
        return true;
    }
    
    
public bool Update(Device device)
{
    ValidateDevice(device);
    const string sqlDevice = @" UPDATE Device SET Name = @Name, IsEnabled = @IsEnabled WHERE Id = @Id";
    using (SqlConnection connection = new SqlConnection(_connectionString))
    {
        connection.Open();
        SqlCommand command = new SqlCommand(sqlDevice, connection);
        command.Parameters.AddWithValue("@Id", device.Id);
        command.Parameters.AddWithValue("@Name", device.Name);
        command.Parameters.AddWithValue("@IsEnabled", device.IsEnabled);
        int rows = command.ExecuteNonQuery();
        if (rows == 0)
            return false;
        bool goodDetails = device switch
        {
            Smartwatch sw => UpdateSmartwatchDetails(sw, connection),
            PersonalComputer pc => UpdatePersonalComputerDetails(pc, connection),
            Embedded ed => UpdateEmbeddedDetails(ed, connection),
            _ => throw new ArgumentException("Unknown device type", nameof(device))
        };

        return goodDetails;
    }
}

private bool UpdateSmartwatchDetails(Smartwatch sw, SqlConnection conn)
{
    const string sql = @"UPDATE Smartwatch SET BatteryPercentage = @BatteryLevel WHERE DeviceId = @Id";
    using var cmd = new SqlCommand(sql, conn);
    cmd.Parameters.AddWithValue("@Id",      sw.Id);
    cmd.Parameters.AddWithValue("@BatteryLevel", sw.BatteryLevel);
    return cmd.ExecuteNonQuery() == 1;
}

private bool UpdatePersonalComputerDetails(PersonalComputer pc, SqlConnection conn)
{
    const string sql = @"UPDATE PersonalComputer SET OperationSystem = @OperationSystem WHERE DeviceId = @Id";
    using var cmd = new SqlCommand(sql, conn);
    cmd.Parameters.AddWithValue("@Id", pc.Id);
    cmd.Parameters.AddWithValue("@OperationSystem", pc.OperatingSystem);
    return cmd.ExecuteNonQuery() == 1;
}

private bool UpdateEmbeddedDetails(Embedded ed, SqlConnection conn)
{
    const string sql = @"UPDATE Embedded SET IpAddress   = @IpAddress, NetworkName = @NetworkName WHERE DeviceId = @Id";
    using var cmd = new SqlCommand(sql, conn);
    cmd.Parameters.AddWithValue("@Id",  ed.Id);
    cmd.Parameters.AddWithValue("@IpAddress",  ed.IpAddress);
    cmd.Parameters.AddWithValue("@NetworkName", ed.NetworkName);
    return cmd.ExecuteNonQuery() == 1;
}


//delete on cascade in sql
    public bool Delete(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return false;

        string sql = @"DELETE FROM Device WHERE Id = @Id;";
        int count;
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", id);
            connection.Open();
            count = command.ExecuteNonQuery();

        }

        return count > 0;
    }

    private void ValidateDevice(Device device)
    {
        if (device == null)
            throw new ArgumentNullException(nameof(device));
        if (string.IsNullOrWhiteSpace(device.Name))
            throw new ArgumentException("name is required", nameof(device.Name));

        switch (device)
        {
            case Smartwatch sw:
                if (sw.BatteryLevel < 0 || sw.BatteryLevel > 100)
                    throw new ArgumentException("battery level invalid", nameof(sw.BatteryLevel));
                break;

            case PersonalComputer pc:
                if (string.IsNullOrWhiteSpace(pc.OperatingSystem))
                    throw new ArgumentException("OS is required for PC", nameof(pc.OperatingSystem));
                break;

            case Embedded ed:
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
