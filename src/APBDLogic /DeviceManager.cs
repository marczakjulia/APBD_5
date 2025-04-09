using System;
using System.Collections.Generic;

namespace APBD5
{
    /// <summary>
    /// Class that extends the IDeviceRepo and is responsible for all operations on devices, such as adding or deleting.
    /// </summary>
    public class DeviceManager : IDeviceRepository
    {
        private readonly IDeviceParser _deviceParser;
        private const int MaxCapacity = 15;
        private readonly List<Device> _devices = new(MaxCapacity);
        private readonly DeviceFileManager _deviceFileManager;

        // The constructor now accepts a filePath and reads the file internally
        public DeviceManager(string filePath, IDeviceParser deviceParser, DeviceFileManager deviceFileManager)
        {
            _deviceParser = deviceParser;
            _deviceFileManager = deviceFileManager;
            string[] lines = deviceFileManager.ReadFile(filePath);
            ParseDevices(lines);
        }

        public void AddDevice(Device newDevice)
        {
            foreach (var storedDevice in _devices)
            {
                if (storedDevice.Id.Equals(newDevice.Id))
                {
                    throw new ArgumentException($"Device with ID {storedDevice.Id} is already stored.", nameof(newDevice));
                }
            }

            if (_devices.Count >= MaxCapacity)
            {
                throw new Exception("Device storage is full.");
            }

            _devices.Add(newDevice);
            _deviceFileManager.SaveDevices("/Users/juliamarczak/RiderProjects/APBD_5/src/APBDLogic /input.txt", _devices);
        }

        public void EditDevice(Device editDevice)
        {
            int targetDeviceIndex = -1;
            for (int index = 0; index < _devices.Count; index++)
            {
                if (_devices[index].Id.Equals(editDevice.Id))
                {
                    targetDeviceIndex = index;
                    break;
                }
            }

            if (targetDeviceIndex == -1)
            {
                throw new ArgumentException($"Device with ID {editDevice.Id} is not stored.", nameof(editDevice));
            }

            if (_devices[targetDeviceIndex].GetType() != editDevice.GetType())
            {
                throw new ArgumentException(
                    $"Type mismatch between devices. Target device has type {_devices[targetDeviceIndex].GetType().Name}");
            }

            _devices[targetDeviceIndex] = editDevice;
            _deviceFileManager.SaveDevices("/Users/juliamarczak/RiderProjects/APBD_5/src/APBDLogic /input.txt", _devices);
        }

        public void RemoveDeviceById(string deviceId)
        {
            Device targetDevice = null;
            foreach (var storedDevice in _devices)
            {
                if (storedDevice.Id.Equals(deviceId))
                {
                    targetDevice = storedDevice;
                    break;
                }
            }

            if (targetDevice == null)
            {
                throw new ArgumentException($"Device with ID {deviceId} is not stored.", nameof(deviceId));
            }

            _devices.Remove(targetDevice);
        }

        public Device GetDeviceById(string id)
        {
            foreach (var storedDevice in _devices)
            {
                if (storedDevice.Id.Equals(id))
                {
                    return storedDevice;
                }
            }

            return null;
        }

        public void ShowAllDevices()
        {
            foreach (var device in _devices)
            {
                Console.WriteLine(device.ToString());
            }
        }

        private void ParseDevices(string[] lines)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                try
                {
                    Device parsedDevice = _deviceParser.ParseDevice(lines[i], i);
                    AddDevice(parsedDevice);
                }
                catch (ArgumentException argEx)
                {
                    Console.WriteLine(argEx.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        $"Something went wrong during parsing line: {lines[i]}. Exception: {ex.Message}");
                }
            }
        }

        public List<Device> GetAllDevices()
        {
            return _devices;
        }
    }
}
