using System.Text.Json;
using System.Text.Json.Nodes;
using APBD5;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("UniversityDatabase");
builder.Services.AddTransient<DeviceRepository>(x => new DeviceRepository(connectionString));
builder.Services.AddTransient<IDeviceService, DeviceService>();

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
    string? contentType = request.ContentType?.ToLowerInvariant();
    string? deviceType  = null;   
    string? deviceJson  = null;    

    switch (contentType)
    {
        case "application/json":
        {
            using var reader = new StreamReader(request.Body);
            string rawJson = await reader.ReadToEndAsync();
            var json = JsonNode.Parse(rawJson);
            deviceType = json["deviceType"]?.ToString().ToUpperInvariant();
            deviceJson = json["typeValue"]?.ToString();
            if (deviceJson is null)
                return Results.BadRequest("Device data not provided.");
            break;
        }
        case "text/plain":
        {
            using var reader = new StreamReader(request.Body);
            string text = await reader.ReadToEndAsync();
            if (string.IsNullOrWhiteSpace(text))
                return Results.BadRequest("Empty text payload.");
            int x = text.IndexOf('\n');
            if (x < 0)
                return Results.BadRequest();
            //took this from the internet because i could not find another way around it :(
            deviceType = text[..x].Trim().ToUpperInvariant();
            deviceJson = text[(x + 1)..].Trim();
            break;
        }
        default:
            return Results.BadRequest("Unable to read");
    }
    if (string.IsNullOrWhiteSpace(deviceType) || string.IsNullOrWhiteSpace(deviceJson))
        return Results.BadRequest("Not enoguh info");
    var options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    switch (deviceType)
    {
        case "PC":
        {
            var pc = JsonSerializer.Deserialize<PersonalComputerDTO>(deviceJson, options);
            if (pc is null) return Results.BadRequest("Invalid PC data");

            return deviceService.Create(pc)
                ? Results.Created($"/api/devices/{pc.Id}", pc)
                : Results.BadRequest("Device ID already exists");
        }
        case "SW":
        {
            var watch = JsonSerializer.Deserialize<SmartwatchDTO>(deviceJson, options);
            if (watch is null) return Results.BadRequest("Invalid Smartwatch data");

            return deviceService.Create(watch)
                ? Results.Created($"/api/devices/{watch.Id}", watch)
                : Results.BadRequest("Device ID already exists");
        }
        case "ED":
        {
            var em = JsonSerializer.Deserialize<EmbeddedDTO>(deviceJson, options);
            if (em is null) return Results.BadRequest("Invalid ED data");

            return deviceService.Create(em)
                ? Results.Created($"/api/devices/{em.Id}", em)
                : Results.BadRequest("Device ID already exists");
        }
        default:
            return Results.BadRequest("Unknown device");
    }
})
.Accepts<string>("application/json", ["text/plain"]);



app.MapPut("/api/devices/{id}", async (string id, HttpRequest request, IDeviceService deviceService) =>
{
    try
    {
        string? contentType = request.ContentType?.ToLower();

        if (contentType != "application/json")
        {
            return Results.BadRequest("Unsupported type");
        }

        using var reader = new StreamReader(request.Body);
        string rawJson = await reader.ReadToEndAsync();
        var json = JsonNode.Parse(rawJson);
        var deviceData = json?["typeValue"];
        var deviceType = json?["deviceType"]?.ToString();

        if (deviceData == null || string.IsNullOrEmpty(deviceType))
        {
            return Results.BadRequest("data is missing");
        }

        DeviceDTO device = null;

        if (deviceType == "PC")
        {
            device = JsonSerializer.Deserialize<PersonalComputerDTO>(deviceData.ToString());
        }
        else if (deviceType == "SW")
        {
            device = JsonSerializer.Deserialize<SmartwatchDTO>(deviceData.ToString());
        }
        else if (deviceType == "ED")
        {
            device = JsonSerializer.Deserialize<EmbeddedDTO>(deviceData.ToString());
        }
        else
        {
            return Results.BadRequest("unknown device type");
        }

        if (device == null)
        {
            return Results.BadRequest("Invalid device data");
        }

        device.Id = id;

        bool updated = deviceService.Update(device);
        if (updated)
        {
            return Results.NoContent();  
        }
        else
        {
            return Results.BadRequest("Failed to update");
        }
    }
    catch (Exception ex)
    {
        return Results.Problem("error", ex.Message);
    }
});





app.MapDelete("/api/devices/{id}", (string id, IDeviceService deviceService) =>
{
    try
    {
        bool deleted = deviceService.Delete(id);
        if (deleted)
        return Results.NoContent();
        else
        {
            return Results.BadRequest("Couldnt delete");
        }
    }
    catch (Exception ex)
    {
        return Results.Problem("error", ex.Message);
    }
});

app.Run();
