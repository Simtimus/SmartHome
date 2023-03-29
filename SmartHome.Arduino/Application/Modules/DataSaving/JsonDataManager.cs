using SmartHome.Arduino.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SmartHome.Arduino.Application.Modules.DataSaving
{
    public class JsonDataManager
    {
        public static void SaveObjectToFile(string filePath, object data)
        {

            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            string serializedObject = JsonConvert.SerializeObject(data);
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.Write(serializedObject);
            }
        }


        public static string? GetObjectFromFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                     return reader.ReadToEnd();
                }
            }
            return null;
        }
    }
}
