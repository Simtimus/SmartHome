using Newtonsoft.Json;
using SmartHome.Arduino.Models.Arduino;
using SmartHome.Arduino.Models.Components.Common.Interfaces;
using static SmartHome.Arduino.Models.Components.Common.GeneralComponent;

namespace SmartHome.Arduino.Models.Components
{
	public class HumiditySensor : IGeneralComponent
	{
		public int Id { get; set; }
		public ComponentTypes ComponentType { get; set; } = ComponentTypes.HumiditySensor;
		public List<PortPin> ConnectedPins { get; set; } = new List<PortPin>();
		public string Description { get; set; } = string.Empty;
		public bool Favorite { get; set; }
		[JsonIgnore] public ArduinoClient ParentClient { get; set; }
	}
}