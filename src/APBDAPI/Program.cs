using System.Text.Json;
using System.Text.Json.Nodes;
using APBD5;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("UniversityDatabase");
builder.Services.AddTransient<IDeviceService,DeviceService >(
    _ => new DeviceService(connectionString));

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();



app.MapGet("/api/devices", (IDeviceService deviceService) =>
{
    try
    {
        return Results.Ok(deviceService.GetAllDevices());
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.MapGet("/api/devices/{id}", (string id, IDeviceService deviceService) =>
{
    try
    {
        var device = deviceService.GetDeviceById(id);
        if (device == null)
        {
            return Results.NotFound("Device not found");
        }
        return Results.Ok(device);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.MapPost("/api/devices", async (HttpRequest request, IDeviceService deviceService) =>
{
    try
    {
        string? contentType = request.ContentType?.ToLower();
        
        if (contentType == "application/json")
        {
            using var reader = new StreamReader(request.Body);
            string rawJson = await reader.ReadToEndAsync();
            var json = JsonNode.Parse(rawJson);
            var deviceData = json?["typeValue"];
            if (deviceData != null)
            {
                if (json?["deviceType"]?.ToString() == "PC")
                {
                    var pc = JsonSerializer.Deserialize<PersonalComputer>(deviceData.ToString());
                    if (pc == null) return Results.BadRequest("Invalid PC data");
                    bool created = deviceService.Create(pc);
                    if (created)
                        return Results.Created($"/api/devices/{pc.Id}", pc);
                    else
                        return Results.BadRequest("Device with this ID already exists.");
                }
                else if (json?["deviceType"]?.ToString() == "Smartwatch")
                {
                    var watch = JsonSerializer.Deserialize<Smartwatch>(deviceData.ToString());
                    if (watch == null) return Results.BadRequest("Invalid Smartwatch data");
                    bool created = deviceService.Create(watch);
                    if (created)
                        return Results.Created($"/api/devices/{watch.Id}", watch);
                    else
                        return Results.BadRequest("Device with this ID already exists.");
                }
                else if (json?["deviceType"]?.ToString() == "Embedded")
                {
                    var embedded = JsonSerializer.Deserialize<Embedded>(deviceData.ToString());
                    if (embedded == null) return Results.BadRequest("Invalid Embedded device data");
                    bool created = deviceService.Create(embedded);
                    if (created)
                        return Results.Created($"/api/devices/{embedded.Id}", embedded);
                    else
                        return Results.BadRequest("Device with this ID already exists.");
                }
                else
                {
                    return Results.BadRequest("Unknown device type");
                }
            }
            return Results.BadRequest("Device data not provided.");
        }
        else if (contentType == "text/plain")
        {
            // Handle plain text input (similar logic as for JSON)
            using var reader = new StreamReader(request.Body);
            string rawText = await reader.ReadToEndAsync();

            // Process plain text data here
            // (You can adjust how you handle plain text depending on your needs)
            var device = new PersonalComputer(rawText, rawText, true, "Windows");
            deviceService.Create(device);

            return Results.Created($"/api/devices/{device.Id}", device);
        }
        else
        {
            return Results.BadRequest("Unsupported content type");
        }
    }
    catch (Exception ex)
    {
        return Results.Problem("Error processing request: " + ex.Message);
    }
})
.Accepts<string>("application/json", "text/plain"); 

app.MapPut("/api/devices/pc", (PersonalComputer pc) =>
{
    try
    {
        return Results.NoContent();
    }
    catch (Exception ex)
    {
        return Results.Problem("Error when updating device");
    }
});

app.MapPut("/api/devices/smartwatch", (Smartwatch sw) =>
{
    try
    {
        return Results.NoContent();
    }
    catch (Exception ex)
    {
        return Results.Problem("Error when updating device");
    }
});

app.MapPut("/api/devices/embedded", (Embedded ed) =>
{
    try
    {
        return Results.NoContent();
    }
    catch (Exception ex)
    {
        return Results.Problem("Error when updating device");
    }
});

app.MapDelete("/api/devices/{id}", (string id, IDeviceService deviceService) =>
{
    try
    {
        deviceService.Delete(id);
        return Results.NoContent();
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.Run();
