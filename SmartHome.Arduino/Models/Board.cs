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
		public string Name { get; set; } = String.Empty;
        public List<IGenericComponent> Components { get; set; } = new List<IGenericComponent>();
		public string Model { get; set; } = String.Empty;
		public string Description { get; set; } = String.Empty;
	}
}
