using System;
using System.Collections.Generic;
using System.IO;

namespace APBD5
{
    /// <summary>
    /// Responsible for handling file operations.
    /// </summary>
    public class DeviceFileManager
    {
        /// <summary>
        /// Reads all lines from the specified file.
        /// </summary>
        /// <param name="filePath">Path of the file to read.</param>
        /// <exception cref="FileNotFoundException">Thrown if the file does not exist.</exception>
        public virtual string[] ReadFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The input device file could not be found.");
            }
            return File.ReadAllLines(filePath);
        }

        /// <summary>
        /// Saves the devices to a file.
        /// </summary>
        /// <param name="outputPath">Path of the file to write to.</param>
        /// <param name="devices">List of devices to save.</param>
        public virtual void SaveDevices(string outputPath, List<Device> devices)
        {
            var linesList = new List<string>();
            foreach (Device device in devices)
            {
                linesList.Add(device.ToSavingString());
            }
            File.WriteAllLines(outputPath, linesList.ToArray());
        }
    }
}