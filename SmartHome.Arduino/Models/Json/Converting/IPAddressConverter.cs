using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Arduino.Models.Json.Converting
{
    public class IPAddressConverter : JsonConverter<IPEndPoint>
    {
        public override IPEndPoint ReadJson(JsonReader reader, Type objectType, IPEndPoint existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            // Parse the IPAddress from the JSON string
            string ipAddressString = (string)reader.Value;
            return IPEndPoint.Parse(ipAddressString);
        }

        public override void WriteJson(JsonWriter writer, IPEndPoint value, JsonSerializer serializer)
        {
            // Serialize the IPAddress to a string
            string ipAddressString = value.ToString();
            writer.WriteValue(ipAddressString);
        }
    }
}
