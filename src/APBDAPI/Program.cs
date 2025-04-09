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

app.MapGet("/api/devices", () => deviceManager.GetAllDevices());

app.MapGet("/api/devices/{id}", (string id) => deviceManager.GetDeviceById(id));

app.MapPost("/api/devices/pc", (PersonalComputer pc) =>
{
    deviceManager.AddDevice(pc);
});

app.MapPost("/api/devices/smartwatch", (Smartwatch watch) =>
{
    deviceManager.AddDevice(watch);
});

app.MapPost("/api/devices/embedded", (Embedded ed) =>
{
    deviceManager.AddDevice(ed);
});

app.MapPut("/api/devices/pc", (PersonalComputer pc) =>
{
    deviceManager.EditDevice(pc);
});

app.MapPut("/api/devices/smartwatch", (Smartwatch sw) =>
{
    deviceManager.EditDevice(sw);
});

app.MapPut("/api/devices/embedded", (Embedded ed) =>
{
    deviceManager.EditDevice(ed);
});

app.MapDelete("/api/devices/{id}", (string id) =>
{
    deviceManager.RemoveDeviceById(id);
});

app.Run();