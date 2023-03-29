using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Arduino.Models.Components;

namespace SmartHome.Arduino.Models
{
    public class Board
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = String.Empty;
        public List<iGenericComponent> Components { get; set; } = new List<iGenericComponent>();
		public string Model { get; set; } = String.Empty;
		public string Description { get; set; } = String.Empty;
	}
}
