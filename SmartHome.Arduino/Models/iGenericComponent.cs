using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static SmartHome.Arduino.Models.GenericComponent;

namespace SmartHome.Arduino.Models
{
    public interface IGenericComponent
    {
        public Guid Id { get; set; }
        public ComponentsId ComponentId { get; set; }
        public List<BoardPin> ConnectedPins { get; set; }
        public string Description { get; set; }
        [JsonIgnore] public ArduinoClient ParentClient { get; set; }
    }
}
