using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SmartHome.Arduino.Models.Arduino;
using SmartHome.Arduino.Models.Components.Common.Interfaces;
using static SmartHome.Arduino.Models.Components.Common.GeneralComponent;

namespace SmartHome.Arduino.Models.Components
{
    public class LightSensor : IGeneralComponent
    {
        public int Id { get; set; }
        public ComponentTypes ComponentType { get; set; } = ComponentTypes.LightSensor;
        public List<PortPin> ConnectedPins { get; set; } = new List<PortPin>();
        public string Description { get; set; } = string.Empty;
        [JsonIgnore] public ArduinoClient ParentClient { get; set; }
    }
}
