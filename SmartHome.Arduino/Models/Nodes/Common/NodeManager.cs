using SmartHome.Arduino.Application;
using SmartHome.Arduino.Models.Arduino;
using SmartHome.Arduino.Models.Components.Common.Interfaces;
using SmartHome.Arduino.Models.Interfaces;
using SmartHome.Arduino.Models.Json.Converting;
using SmartHome.Arduino.Models.Json.FileStorage;
using SmartHome.Arduino.Models.Nodes.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Arduino.Models.Nodes.Common
{
	public class NodeManager
	{
		public static List<INode> Nodes { get; set; } = new();
		private const string NodesFileName = "Nodes.json";
		private static readonly object _lock = new();

		public static void RecoverAllData()
		{
			lock (_lock)
			{
				string? recoveredData = FileDataStorage.ReadStringFromFile(NodesFileName);
				if (string.IsNullOrEmpty(recoveredData))
					return;

				JsonDataConverting.ConvertINodes(recoveredData, out List<INode> nodes);
				Nodes = nodes;
			}
		}
		public static void SaveData()
		{
			lock (_lock)
			{
				FileDataStorage.SaveDataToJsonFile(Nodes, NodesFileName);
			}
		}

		public static INode? GetNodeById(Guid Id)
		{
			lock( _lock)
			{
				return Nodes.Find(node => node.Id == Id);
			}
		}

		public static bool RemoveNodesById(Guid Id)
		{
			lock (_lock)
			{
				int deletedItems = Nodes.RemoveAll(node => node.Id == Id);
				if (deletedItems > 0)
					return true;
			}
			return false;
		}

		public static string CreateValueNode()
		{
			ValueNode Node = new();
			Node.Id = Guid.NewGuid();
			Nodes.Add(Node);
			return Node.Id.ToString();
		}

		public static string CreateDataLinkNode()
		{
			DataLinkNode Node = new();
			Node.Id = Guid.NewGuid();
			Nodes.Add(Node);
			return Node.Id.ToString();
		}

		public static string CreateValueOperationNode()
		{
			ValueOperationNode Node = new();
			Node.Id = Guid.NewGuid();
			Nodes.Add(Node);
			return Node.Id.ToString();
		}

		public static string CreateConditionNode()
		{
			ConditionNode Node = new();
			Node.Id = Guid.NewGuid();
			Nodes.Add(Node);
			return Node.Id.ToString();
		}

		//public static void LinkData(ref PortPin from, ref PortPin to, DataLinkNode.ValueOperations valueOperation, string value)
		//{
		//	to.DataLink = new(from)
		//	{
		//		Id = Guid.NewGuid(),
		//		ParentPortPin = to,
		//		FlexiValue = value,
		//		ValueOperation = valueOperation,
		//	};
		//	lock (_lock)
		//	{
		//		DataLinks.Add(to.DataLink);
		//	}
		//}
	}
}
