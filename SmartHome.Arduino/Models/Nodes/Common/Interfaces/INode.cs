using SmartHome.Arduino.Models.Arduino;
using SmartHome.Arduino.Models.CommonTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SmartHome.Arduino.Models.Nodes.Common.GeneralNode;

namespace SmartHome.Arduino.Models.Nodes.Common.Interfaces
{
	public interface INode
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public FlexibleValue FlexiValue { get; set; }
		public NodeTypes Type { get; set; }
		public object? GetValue();
	}
}
