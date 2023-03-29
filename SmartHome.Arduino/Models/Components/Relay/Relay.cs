using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SmartHome.Arduino.Models.Components.GenericComponent;

namespace SmartHome.Arduino.Models.Components.Relay
{
    public class Relay : iGenericComponent
    {
        public Guid Id { get; set; }
        public ComponentsId ComponentId { get; set; } = ComponentsId.Relay;
        public List<BoardPin> ConnectedPins { get; set; } = new List<BoardPin>();
        public string Description { get; set; } = string.Empty;
    }
}
