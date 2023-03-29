using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static SmartHome.Arduino.Models.Components.GenericComponent;

namespace SmartHome.Arduino.Models.Components.LightSensor
{
    public class LightSensor : iGenericComponent
	{
		public Guid Id { get; set; }
		public ComponentsId ComponentId { get; set; } = ComponentsId.LightSensor;
        public List<BoardPin> ConnectedPins { get; set; } = new List<BoardPin>();
		public string Description { get; set; } = string.Empty;
	}
}
