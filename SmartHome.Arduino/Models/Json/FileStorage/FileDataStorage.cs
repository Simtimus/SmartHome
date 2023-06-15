using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SmartHome.Arduino.Models.Json.FileStorage
{
    /// <summary>
    /// Provides methods for saving data to a file and reading a string from a file.
    /// </summary>
    public static class FileDataStorage
    {
        /// <summary>
        /// Writes and saves the given data to a file.
        /// </summary>
        /// <param name="data">The data string to be saved.</param>
        /// <param name="filePath">The file path where the data will be saved.</param>
        public static void WriteStringToFile(string data, string filePath)
        {
            if (string.IsNullOrEmpty(data))
            {
                throw new ArgumentNullException(nameof(data));
            }
            using StreamWriter writer = new(filePath);
            writer.Write(data);
        }

        /// <summary>
        /// Serializes and saves the given data to a Json file.
        /// </summary>
        /// <param name="data">The data object to be saved.</param>
        /// <param name="filePath">The file path where the data will be saved.</param>
        public static void SaveDataToJsonFile(object data, string filePath)
        {
            try
            {
                if (data is null)
                {
                    throw new ArgumentNullException(nameof(data));
                }
            }
            catch { }

            string serializedObject = JsonConvert.SerializeObject(data);
            using StreamWriter writer = new(filePath);
            writer.Write(serializedObject);
        }

        /// <summary>
        /// Reads a string from a file at the given file path.
        /// </summary>
        /// <param name="filePath">The file path to read the string from.</param>
        /// <returns>The contents of the file as a string or null if the file does not exist.</returns>
        public static string? ReadStringFromFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                using StreamReader reader = new(filePath);
                return reader.ReadToEnd();
            }
            return null;
        }
    }
}
