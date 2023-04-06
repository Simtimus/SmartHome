using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static SmartHome.Arduino.Models.GenericComponent;

namespace SmartHome.Arduino.Models.Components
{
    public class Relay : IGenericComponent
    {
        public Guid Id { get; set; }
        public ComponentsId ComponentId { get; set; } = ComponentsId.Relay;
        public List<BoardPin> ConnectedPins { get; set; } = new List<BoardPin>();
        public string Description { get; set; } = string.Empty;
        [JsonIgnore] public ArduinoClient ParentClient { get; set; }
    }
}
