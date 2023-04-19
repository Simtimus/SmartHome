using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Arduino.Models
{
    public class Board
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = string.Empty;
        public List<IGeneralComponent> Components { get; set; } = new List<IGeneralComponent>();
		public string Model { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
	}
}
