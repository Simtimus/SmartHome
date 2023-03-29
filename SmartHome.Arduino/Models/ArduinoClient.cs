using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Arduino.Models
{
    public class ArduinoClient : Board
    {
        [JsonConverter(typeof(IPAddressConverter))]
        public IPEndPoint? IP { get; set; }
        public DateTime LastConnection { get; set; }
        public ConnectionState State { get; set; }
        public int Ping { get; set; } = 0;

        public enum ConnectionState
        {
            Offline,
            Online
        }

        public static bool IsNullOrEmpty(ArduinoClient? client)
        {
            if (client is null)
                return true;
            if (client.Equals(new ArduinoClient()))
                return true;
            return false;
        }

        public static ArduinoClient ParseFromJson(IPEndPoint ip, string serializedObject)
        {
            ArduinoClient? arduinoClient = JsonConvert.DeserializeObject<ArduinoClient>(serializedObject);
            if (arduinoClient is not null)
            {
                arduinoClient.IP = ip;
                return arduinoClient;
            }
            return new ArduinoClient();
        }
    }
}
