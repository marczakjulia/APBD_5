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
    if (GetDeviceById(device.Id) is not null)
        return false;

    const string sql = "INSERT INTO Device (Id, Name, IsEnabled) VALUES (@Id, @Name, @IsEnabled)";
    using (var connection = new SqlConnection(_connectionString))
    {
        connection.Open();
        //i have used transaction like on utp was shown last year, to make sure that no commits to the databse are made unless all arguments are okay
        //this allows for all the checking conditions that were earlier implemented when creating the project. hence nothing will be added to the device nor one 
        //of their types unless all arugments fit the requirements 
        // https://learn.microsoft.com/en-us/dotnet/framework/data/adonet/local-transactions
        using (var transaction = connection.BeginTransaction())
        {
            try
            {
                using (var command = new SqlCommand(sql, connection, transaction))
                {
                    command.Parameters.AddWithValue("@Id", device.Id);
                    command.Parameters.AddWithValue("@Name", device.Name);
                    command.Parameters.AddWithValue("@IsEnabled", device.IsEnabled);

                    if (command.ExecuteNonQuery() == 0)
                        throw new Exception("failed to insert device");
                }
                bool result = device switch
                {
                    Smartwatch sw => CreateSmartwatchDetails(sw, connection, transaction),
                    PersonalComputer pc => CreatePersonalComputerDetails(pc, connection, transaction),
                    Embedded ed => CreateEmbeddedDetails(ed, connection, transaction)
                };
                if (!result)
                {
                    throw new Exception("failed to insert details");
                }
                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine("Error adding device");
                return false;
            }
        }
    }
}


private bool CreateSmartwatchDetails(Smartwatch sw, SqlConnection connection, SqlTransaction transaction)
{
    int count;
    string query = "INSERT INTO Smartwatch (DeviceId, BatteryPercentage) VALUES (@DeviceId, @BatteryPercentage)";
    using var command = new SqlCommand(query, connection, transaction);
    command.Parameters.AddWithValue("@DeviceId", sw.Id);
    command.Parameters.AddWithValue("@BatteryPercentage", sw.BatteryLevel);
    count = command.ExecuteNonQuery();
    return count > 0;
}

private bool CreatePersonalComputerDetails(PersonalComputer pc, SqlConnection connection, SqlTransaction transaction)
{
    int count = -1;
    string query = "INSERT INTO PersonalComputer (OperationSystem, DeviceID) VALUES (@OperationSystem, @DeviceId)";
    using var command = new SqlCommand(query, connection, transaction);
    command.Parameters.AddWithValue("@DeviceId", pc.Id);
    command.Parameters.AddWithValue("@OperationSystem", pc.OperatingSystem);
    count = command.ExecuteNonQuery();
    return count == -1;
}

private bool CreateEmbeddedDetails(Embedded ed, SqlConnection connection, SqlTransaction transaction)
{
    int count;
    string query = "INSERT INTO Embedded (IpAddress, NetworkName, DeviceID) VALUES (@IpAddress, @NetworkName, @DeviceId)";
    using var command = new SqlCommand(query, connection, transaction);
    command.Parameters.AddWithValue("@DeviceId", ed.Id);
    command.Parameters.AddWithValue("@IpAddress", ed.IpAddress);
    command.Parameters.AddWithValue("@NetworkName", ed.NetworkName);
    count = command.ExecuteNonQuery();
    return count > 0;
}

    
public bool Update(Device device)
{
    if (device is null) throw new ArgumentNullException(nameof(device));

    const string sqlUpdateDevice = @"UPDATE Device SET Name = @Name, IsEnabled = @IsEnabled WHERE Id = @Id";

    using var connection = new SqlConnection(_connectionString);
    connection.Open();
    using var transaction = connection.BeginTransaction();
    try
    {
        using var command = new SqlCommand(sqlUpdateDevice, connection, transaction);
        command.Parameters.AddWithValue("@Id", device.Id);
        command.Parameters.AddWithValue("@Name", device.Name);
        command.Parameters.AddWithValue("@IsEnabled", device.IsEnabled);
        
        int rowsAffected = command.ExecuteNonQuery();
        if (rowsAffected == 0)
        {
            throw new Exception("Device not found");
        }
        bool result = device switch
        {
            Smartwatch sw => UpdateSmartwatchDetails(sw, connection, transaction),
            PersonalComputer pc => UpdatePersonalComputerDetails(pc, connection, transaction),
            Embedded ed => UpdateEmbeddedDetails(ed, connection, transaction),
        };

        if (!result)
        {
            transaction.Rollback();
            return false;
        }

        transaction.Commit();
        return true;
    }
    catch (Exception ex)
    {
        transaction.Rollback();
        Console.WriteLine($"Error updating device {ex.Message}");
        return false;
    }
}

private bool UpdateSmartwatchDetails(Smartwatch sw, SqlConnection connection, SqlTransaction transaction)
{
    int count;
    string query = "UPDATE Smartwatch SET BatteryPercentage = @BatteryPercentage WHERE DeviceId = @DeviceId";
    using var command = new SqlCommand(query, connection, transaction);
    command.Parameters.AddWithValue("@DeviceId", sw.Id);
    command.Parameters.AddWithValue("@BatteryPercentage", sw.BatteryLevel);
    count = command.ExecuteNonQuery();
    return count > 0;
}

private bool UpdatePersonalComputerDetails(PersonalComputer pc, SqlConnection connection, SqlTransaction transaction)
{
    int count;
    string query = "UPDATE PersonalComputer SET OperationSystem = @OperationSystem WHERE DeviceId = @DeviceId";
    using var command = new SqlCommand(query, connection, transaction);
    command.Parameters.AddWithValue("@DeviceId", pc.Id);
    command.Parameters.AddWithValue("@OperationSystem", pc.OperatingSystem);
    count = command.ExecuteNonQuery();
    return count > 0;
}

private bool UpdateEmbeddedDetails(Embedded ed, SqlConnection connection, SqlTransaction transaction)
{
    int count;
    string query = "UPDATE Embedded SET IpAddress = @IpAddress, NetworkName = @NetworkName WHERE DeviceId = @DeviceId";
    using var command = new SqlCommand(query, connection, transaction);
    command.Parameters.AddWithValue("@DeviceId", ed.Id);
    command.Parameters.AddWithValue("@IpAddress", ed.IpAddress);
    command.Parameters.AddWithValue("@NetworkName", ed.NetworkName);
    count = command.ExecuteNonQuery();
    return count > 0;
}



//works
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

   
}
