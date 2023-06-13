using SmartHome.Arduino.Models.CommonTypes;
using SmartHome.Arduino.Models.Nodes.Common;
using SmartHome.Arduino.Models.Nodes.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SmartHome.Arduino.Models.Nodes.Common.GeneralNode;

namespace SmartHome.Arduino.Models.Nodes
{
	public class ValueNode : INode
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public FlexibleValue FlexiValue { get; set; } = new();
		public NodeTypes Type { get; set; } = NodeTypes.Value;

		public object? GetValue()
		{
			return FlexiValue.Value;
		}
	}
}
