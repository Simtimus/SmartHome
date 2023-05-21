using Newtonsoft.Json;
using SmartHome.Arduino.Models.Json.Converting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Arduino.Models.Arduino
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

        public static bool IsNullOrEmpty([NotNullWhen(false)] ArduinoClient? client)
        {
            if (client is null)
                return true;
            if (client.Equals(default))
                return true;
            return false;
        }
	}
}
