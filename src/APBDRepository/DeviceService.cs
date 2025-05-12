using System.Data;
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
                    RowVersion = reader.GetSqlBinary(3).Value,
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
                RowVersion = reader.GetSqlBinary(3).Value,
            };
        }
    }

    //works
   public bool Create(DeviceDTO device) 
   {
    ValidateDevice(device);
    
    string prefix = device switch
    {
        SmartwatchDTO _        => "SW-",
        PersonalComputerDTO _  => "P-",
        EmbeddedDTO _          => "ED-",
        _ => throw new ArgumentException("Unknown device type", nameof(device))
    };
    

    int counter = 1;
    string newId;
    while (true)
    {
        newId = $"{prefix}{counter}";
        if (GetDeviceById(newId) == null)
            break;
        counter++;
    }
    device.Id = newId;
    device.Id = newId;
    
    using var connection = new SqlConnection(_connectionString);
    connection.Open();
    using var transaction = connection.BeginTransaction();
    try
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandType = CommandType.StoredProcedure;

        switch (device)
        {
            case EmbeddedDTO e:
                command.CommandText = "AddEmbedded";
                command.Parameters.AddWithValue("@DeviceId", device.Id);
                command.Parameters.AddWithValue("@Name", device.Name);
                command.Parameters.AddWithValue("@IsEnabled", device.IsEnabled);
                command.Parameters.AddWithValue("@IpAddress", e.IpAddress);
                command.Parameters.AddWithValue("@NetworkName",e.NetworkName);
                break;
            case SmartwatchDTO sw:
                command.CommandText = "AddSmartwatch";
                command.Parameters.AddWithValue("@DeviceId", device.Id);
                command.Parameters.AddWithValue("@Name", device.Name);
                command.Parameters.AddWithValue("@IsEnabled", device.IsEnabled);
                command.Parameters.AddWithValue("@BatteryPercentage",sw.BatteryLevel);
                break;
            case PersonalComputerDTO pc:
                command.CommandText = "AddPersonalComputer";
                command.Parameters.AddWithValue("@DeviceId", device.Id);
                command.Parameters.AddWithValue("@Name", device.Name);
                command.Parameters.AddWithValue("@IsEnabled", device.IsEnabled);
                command.Parameters.AddWithValue("@OperationSystem", pc.OperatingSystem);
                break;
            default:
                throw new ArgumentException("Unknown device type", nameof(device));
        }
        int rows = command.ExecuteNonQuery();
        if (rows == 0)
            throw new Exception("No rows were inserted");
        transaction.Commit();
        return true;
    }
    catch (Exception ex)
    {
        RollbackTransaction(transaction);
        throw new Exception("Error creating device: " + ex.Message);
    }
}
   
public bool Update(DeviceDTO deviceDto)
{
    if (deviceDto == null) throw new ArgumentNullException(nameof(deviceDto));
    ValidateDevice(deviceDto);
    const string sqlUpdateDevice = @"UPDATE Device SET Name = @Name, IsEnabled = @IsEnabled WHERE Id = @Id AND RowVersion = @RowVersion";
    using var connection = new SqlConnection(_connectionString);
    connection.Open();
    using var transaction = connection.BeginTransaction();
    try
    {
        using var command = new SqlCommand(sqlUpdateDevice, connection, transaction);
        command.Parameters.AddWithValue("@Id", deviceDto.Id);
        command.Parameters.AddWithValue("@Name", deviceDto.Name);
        command.Parameters.AddWithValue("@IsEnabled", deviceDto.IsEnabled);
        command.Parameters.AddWithValue("@RowVersion", deviceDto.RowVersion);

        int rowsAffected = command.ExecuteNonQuery();
        if (rowsAffected == 0)
        {
            //this is the optimistic locking moment, so the rowVer does not apply to any object, meaning its version is diff now
            throw new Exception("not found or modified by someone else");
        }
        
        bool result = deviceDto switch
        {
            SmartwatchDTO swDto => UpdateSmartwatchDetails(swDto, connection, transaction),
            PersonalComputerDTO pcDto => UpdatePersonalComputerDetails(pcDto, connection, transaction),
            EmbeddedDTO edDto => UpdateEmbeddedDetails(edDto, connection, transaction),
            _ => false
        };

        if (!result)
        {
            RollbackTransaction(transaction);
            return false;
        }

        transaction.Commit();
        return true;
    }
    catch (Exception ex)
    {
        transaction.Rollback();
        Console.WriteLine($"Error updating device: {ex.Message}");
        return false;
    }
}

private bool UpdateSmartwatchDetails(SmartwatchDTO swDto, SqlConnection connection, SqlTransaction transaction)
{
    int count;
    string query = "UPDATE Smartwatch SET BatteryPercentage = @BatteryPercentage WHERE DeviceId = @DeviceId";
    using var command = new SqlCommand(query, connection, transaction);
    command.Parameters.AddWithValue("@DeviceId", swDto.Id);
    command.Parameters.AddWithValue("@BatteryPercentage", swDto.BatteryLevel);
    count = command.ExecuteNonQuery();
    return count > 0;
}

private bool UpdatePersonalComputerDetails(PersonalComputerDTO pcDto, SqlConnection connection, SqlTransaction transaction)
{
    int count;
    string query = "UPDATE PersonalComputer SET OperationSystem = @OperationSystem WHERE DeviceId = @DeviceId";
    using var command = new SqlCommand(query, connection, transaction);
    command.Parameters.AddWithValue("@DeviceId", pcDto.Id);
    command.Parameters.AddWithValue("@OperationSystem", pcDto.OperatingSystem);
    count = command.ExecuteNonQuery();
    return count > 0;
}

private bool UpdateEmbeddedDetails(EmbeddedDTO edDto, SqlConnection connection, SqlTransaction transaction)
{
    int count;
    string query = "UPDATE Embedded SET IpAddress = @IpAddress, NetworkName = @NetworkName WHERE DeviceId = @DeviceId";
    using var command = new SqlCommand(query, connection, transaction);
    command.Parameters.AddWithValue("@DeviceId", edDto.Id);
    command.Parameters.AddWithValue("@IpAddress", edDto.IpAddress);
    command.Parameters.AddWithValue("@NetworkName", edDto.NetworkName);
    count = command.ExecuteNonQuery();
    return count > 0;
}


public bool Delete(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return false;

        const string sql = @"DELETE FROM Device WHERE Id = @Id;";

        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        using var transaction = connection.BeginTransaction();
        try
        {
            using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandType = CommandType.Text;
            command.CommandText = sql;
            command.Parameters.AddWithValue("@Id", id);
            int count = command.ExecuteNonQuery();
            transaction.Commit();
            return count > 0;
        }
        catch(Exception ex)
        {
            RollbackTransaction(transaction);
            throw;
        }
    }

    private void ValidateDevice(DeviceDTO device)
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

    private void RollbackTransaction(SqlTransaction transaction)
    {
        transaction.Rollback();
    }
}
