using SmartHome.Arduino.Application;
using SmartHome.Arduino.Models.CommonTypes;
using SmartHome.Arduino.Models.Nodes.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Arduino.Models.Nodes.Common
{
	public class DataReference
	{
		public Guid DataId { get; set; }
		public int ComponentId { get; set; } = -1;
		public int PortPinId { get; set; } = -1;
		public ReferenceType Type { get; set; }

		public enum ReferenceType
		{
			PortPin,
			Node
		}

		public object? GetValue()
		{
			if (Type == ReferenceType.PortPin)
			{
				if (!ClientManager.GetClientIndexById(DataId, out int clientIndex)) return string.Empty;
				if (!ClientManager.GetComponentIndexById(clientIndex, ComponentId, out int componentIndex)) return string.Empty;
				if (!ClientManager.GetBoardPinIndexById(clientIndex, componentIndex, PortPinId, out int pinIndex)) return string.Empty;
				return ClientManager.Clients[clientIndex].Components[componentIndex].ConnectedPins[pinIndex].GetValue();
			}
			else
			{
				INode? Node = NodeManager.GetNodeById(DataId);
				return Node?.GetValue();
			}
		}

		public ObjectValueType GetValueType()
		{
			if (Type == ReferenceType.PortPin)
			{
				if (!ClientManager.GetClientIndexById(DataId, out int clientIndex)) return ObjectValueType.String;
				if (!ClientManager.GetComponentIndexById(clientIndex, ComponentId, out int componentIndex)) return ObjectValueType.String;
				if (!ClientManager.GetBoardPinIndexById(clientIndex, componentIndex, PortPinId, out int pinIndex)) return ObjectValueType.String;
				return ClientManager.Clients[clientIndex].Components[componentIndex].ConnectedPins[pinIndex].FlexiValue.Type;
			}
			else
			{
				INode? Node = NodeManager.GetNodeById(DataId);
				if (Node is null) return ObjectValueType.String;
				return Node.FlexiValue.Type;
			}
		}

		public static bool IsNullOrEmpty([NotNullWhen(false)] DataReference? dataReference)
		{
			if (dataReference is null)
				return true;
			if (dataReference.DataId == default)
				return true;
			return false;
		}
	}
}
