using SmartHome.Arduino.Application;
using SmartHome.Arduino.Models.Arduino;
using SmartHome.Arduino.Models.Nodes.Common;
using SmartHome.Arduino.Models.Nodes.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using static SmartHome.Arduino.Models.Nodes.Common.GeneralNode;
using SmartHome.Arduino.Models.CommonTypes;

namespace SmartHome.Arduino.Models.Nodes
{
	public class DataLinkNode : INode
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public FlexibleValue FlexiValue { get; set; } = new();
		public NodeTypes Type { get; set; } = NodeTypes.DataLink;
		public DataReference LinkFrom { get; set; }
		public ValueOperations ValueOperation { get; set; } = ValueOperations.None;
		[JsonIgnore] public PortPin ParentPortPin { get; set; }

		public enum ValueOperations
		{
			None,
			Negation,
			Addition,
			Subtraction,
			Multiplication,
			Division
		}

		public DataLinkNode() { }

		public DataLinkNode(PortPin reference)
		{
			LinkFrom.DataId = reference.ParentComponent.ParentClient.Id;
			LinkFrom.ComponentId = reference.ParentComponent.Id;
			LinkFrom.PortPinId = reference.Id;
		}

		public object GetValue()
		{
			if (!ClientManager.GetClientIndexById(LinkFrom.DataId, out int clientIndex)) return string.Empty;
			if (!ClientManager.GetComponentIndexById(clientIndex, LinkFrom.ComponentId, out int componentIndex)) return string.Empty;
			if (!ClientManager.GetBoardPinIndexById(clientIndex, componentIndex, LinkFrom.PortPinId, out int pinIndex)) return string.Empty;
			return ClientManager.Clients[clientIndex].Components[componentIndex].ConnectedPins[pinIndex].Value;
		}

		public static bool IsNullOrEmpty([NotNullWhen(false)] DataLinkNode? dataLink)
		{
			if (dataLink is null)
				return true;
			if (dataLink.Equals(default))
				return true;
			return false;
		}
	}
}
