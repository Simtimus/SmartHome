using SmartHome.Arduino.Models.Json.Converting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SmartHome.Arduino.Models.Data.Transmited
{
    public class TransmitedData
    {
        [JsonIgnore] public Guid BoardId { get; set; }
        public List<TransmitedCommand> Commands { get; set; } = new();
    }
}
