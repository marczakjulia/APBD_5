using APBD5;

var builder = WebApplication.CreateBuilder(args);

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

var deviceManager = DeviceManagerCreation.CreateDeviceManager("/Users/juliamarczak/RiderProjects/APBD_5/src/APBDLogic /input.txt");

app.MapGet("/api/devices", () =>
{
    try
    {
        return Results.Ok(deviceManager.GetAllDevices());
    }
    catch (Exception ex)
    {
        return Results.Problem("Error with getting all devices", statusCode: 500);
    }
});

app.MapGet("/api/devices/{id}", (string id) =>
{
    try
    {
        var device = deviceManager.GetDeviceById(id);
        if (device == null)
        {
            return Results.NotFound("Device not found");
        }
        return Results.Ok(device);
    }
    catch (Exception ex)
    {
        return Results.Problem("Error occured when finding device", statusCode: 500);
    }
});

app.MapPost("/api/devices/pc", (PersonalComputer pc) =>
{
    try
    {
        deviceManager.AddDevice(pc);
        return Results.Created($"/api/devices/{pc.Id}", pc);
    }
    catch (Exception ex)
    {
        return Results.Problem("Error when adding device", statusCode: 500);
    }
});

app.MapPost("/api/devices/smartwatch", (Smartwatch watch) =>
{
    try
    {
        deviceManager.AddDevice(watch);
        return Results.Created($"/api/devices/{watch.Id}", watch);
    }
    catch (Exception ex)
    {
        return Results.Problem("Error when adding device", statusCode: 500);
    }
});

app.MapPost("/api/devices/embedded", (Embedded ed) =>
{
    try
    {
        deviceManager.AddDevice(ed);
        return Results.Created($"/api/devices/{ed.Id}", ed);
    }
    catch (Exception ex)
    {
        return Results.Problem("Error when adding device", statusCode: 500);
    }
});

app.MapPut("/api/devices/pc", (PersonalComputer pc) =>
{
    try
    {
        deviceManager.EditDevice(pc);
        return Results.NoContent();
    }
    catch (Exception ex)
    {
        return Results.Problem("Error when updating device", statusCode: 500);
    }
});

app.MapPut("/api/devices/smartwatch", (Smartwatch sw) =>
{
    try
    {
        deviceManager.EditDevice(sw);
        return Results.NoContent();
    }
    catch (Exception ex)
    {
        return Results.Problem("Error when updating device", statusCode: 500);
    }
});

app.MapPut("/api/devices/embedded", (Embedded ed) =>
{
    try
    {
        deviceManager.EditDevice(ed);
        return Results.NoContent();
    }
    catch (Exception ex)
    {
        return Results.Problem("Error when updating device", statusCode: 500);
    }
});

app.MapDelete("/api/devices/{id}", (string id) =>
{
    try
    {
        deviceManager.RemoveDeviceById(id);
        return Results.NoContent();
    }
    catch (Exception ex)
    {
        return Results.Problem("Error when deleting device", statusCode: 500);
    }
});

app.Run();
