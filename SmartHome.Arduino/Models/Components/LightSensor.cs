using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static SmartHome.Arduino.Models.GeneralComponent;

namespace SmartHome.Arduino.Models.Components
{
    public class LightSensor : IGeneralComponent
    {
        public Guid Id { get; set; }
        public int Sequence { get; set; }
        public ComponentsId ComponentId { get; set; } = ComponentsId.LightSensor;
        public List<BoardPin> ConnectedPins { get; set; } = new List<BoardPin>();
        public string Description { get; set; } = string.Empty;
        [JsonIgnore] public ArduinoClient ParentClient { get; set; }
    }
}
