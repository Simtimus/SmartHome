using SmartHome.Arduino.Application.Exceptions;
using SmartHome.Arduino.Models.Nodes.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Arduino.Models.Nodes.Common
{
	public class GeneralNode
	{
		public enum NodeTypes
		{
			Value,
			DataLink,
			ValueOperation,
			Condition
		}

        public static INode? CreateById(NodeTypes nodeId)
		{
			Type? nodeType = GetTypeById(nodeId);
			if (nodeType is null)
				throw new ComponentNotFoundException(nodeId);
			return Activator.CreateInstance(nodeType) as INode;
		}
		
		public static Type? GetTypeById(NodeTypes nodeId)
		{
			string nodeName = Enum.GetName(typeof(NodeTypes), nodeId) ?? string.Empty;
			if (string.IsNullOrEmpty(nodeName))
				throw new ComponentNotFoundException(nodeId);
			return Type.GetType($"SmartHome.Arduino.Models.Nodes.{nodeName}Node");
		}

		public static NodeTypes GetTypeByString(string nodeName)
		{
			return (NodeTypes)Enum.Parse(typeof(NodeTypes), nodeName);
		}
	}
}
