using SmartHome.Arduino.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static SmartHome.Arduino.Models.Components.GenericComponent;

namespace SmartHome.Arduino.Models.Components
{
    public interface iGenericComponent
    {
        public Guid Id { get; set; }
        public ComponentsId ComponentId { get; set; }
        public List<BoardPin> ConnectedPins { get; set; }
        public string Description { get; set; }
    }
}
