using Newtonsoft.Json;
using SmartHome.Arduino.Models.Arduino;
using static SmartHome.Arduino.Models.Components.Common.GeneralComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Arduino.Models.Components.Common.Interfaces
{
    public interface IGeneralComponent
    {
        public int Id { get; set; }
        public ComponentTypes ComponentType { get; set; }
        public List<PortPin> ConnectedPins { get; set; }
        public string Description { get; set; }
		public bool Favorite { get; set; }
		[JsonIgnore] public ArduinoClient ParentClient { get; set; }
    }
}
