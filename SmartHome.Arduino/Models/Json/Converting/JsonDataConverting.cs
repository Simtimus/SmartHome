using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartHome.Arduino.Application.Exceptions;
using SmartHome.Arduino.Models.Interfaces;
using SmartHome.Arduino.Models.Arduino;
using SmartHome.Arduino.Models.Components.Common;
using SmartHome.Arduino.Models.Components.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using SmartHome.Arduino.Models.Data.DataBoxs;
using SmartHome.Arduino.Models.Data.Transmited;
using SmartHome.Arduino.Application.Logging;
using System.Globalization;
using SmartHome.Arduino.Models.Nodes.Common.Interfaces;
using SmartHome.Arduino.Models.Nodes.Common;
using SmartHome.Arduino.Models.Nodes;
using SmartHome.Arduino.Models.CommonTypes;

namespace SmartHome.Arduino.Models.Json.Converting
{
	public static class JsonDataConverting
	{
		public static void ConvertReceivedData(ArduinoDataPacket receivedData)
		{
			JObject jsonObject = JObject.Parse(receivedData.Data);
			UpdateModelFromJson(receivedData, jsonObject);
		}

		public static List<ArduinoClient> ConvertClients(string serializedObject, bool setNewGuid)
		{
			List<ArduinoClient> arduinoClients = new();

			JArray jsonArray = JArray.Parse(serializedObject);
			foreach (var jsonClient in jsonArray)
			{
				ArduinoClient? client = ConvertClient(jsonClient.ToString(), setNewGuid);
				if (client != null)
				{
					arduinoClients.Add(client);
				}
			}

			return arduinoClients;
		}

		public static ArduinoClient? ConvertClientOnly(JObject jsonObject, bool setNewGuid)
		{
			ArduinoClient? client = GetClientFromJsonObject(jsonObject, setNewGuid);
			if (ArduinoClient.IsNullOrEmpty(client))
				return null;

			return client;
		}

		public static void ConvertClientComponents(ArduinoClient client, JObject jsonObject, bool setNewGuid)
		{
			JArray? componentsArray = (JArray?)jsonObject["Components"];
			if (componentsArray != null)
			{
				foreach (var jsonComponent in componentsArray)
				{
					IGeneralComponent? component = ConvertComponent(jsonComponent.ToString(), setNewGuid);
					if (component != null)
					{
						UpdateComponentReferences(component);
						component.ParentClient = client;
						client.Components.Add(component);
					}
				}
			}
		}

		public static ArduinoClient? ConvertClient(string serializedObject, bool setNewGuid)
		{
			JObject jsonObject = JObject.Parse(serializedObject);

			ArduinoClient? client = GetClientFromJsonObject(jsonObject, setNewGuid);
			if (ArduinoClient.IsNullOrEmpty(client))
				return null;

			JArray? componentsArray = (JArray?)jsonObject["Components"];
			if (componentsArray != null)
			{
				foreach (var jsonComponent in componentsArray)
				{
					IGeneralComponent? component = ConvertComponent(jsonComponent.ToString(), setNewGuid);
					if (component != null)
					{
						UpdateComponentReferences(component);
						component.ParentClient = client;
						client.Components.Add(component);
					}
				}
			}

			return client;
		}

		public static IGeneralComponent? ConvertComponent(string serializedObject, bool setNewGuid)
		{
			JObject jsonObject = JObject.Parse(serializedObject);
			GeneralComponent.ComponentTypes componentId = GeneralComponent.GetTypeByString(jsonObject["ComponentType"].ToString());
			IGeneralComponent? component = GeneralComponent.CreateById(componentId);
			if (component != null)
				UpdateModelFromJson(component, jsonObject, setNewGuid);
			else
				return null;

			JArray? componentsArray = (JArray?)jsonObject["ConnectedPins"];
			if (componentsArray != null)
			{
				foreach (var jsonComponent in componentsArray)
				{
					PortPin? boardPin = ConvertBoardPin(jsonComponent.ToString());
					if (boardPin != null)
					{
						component.ConnectedPins.Add(boardPin);

					}
				}
			}

			return component;
		}

		public static PortPin? ConvertBoardPin(string serializedObject)
		{
			JObject jsonObject = JObject.Parse(serializedObject);
			if (jsonObject != null)
			{
				PortPin? boardPin = new();
				UpdateModelFromJson(boardPin, jsonObject, false);
				if (jsonObject.ContainsKey("DataReference"))
					boardPin.DataReference = ConvertDataReference(jsonObject["DataReference"].ToString());
				if (jsonObject.ContainsKey("FlexiValue"))
					boardPin.FlexiValue = ConvertFlexibleValue(jsonObject["FlexiValue"].ToString());
				if (jsonObject.ContainsKey("ValueType"))
					boardPin.FlexiValue = ConvertFlexibleValue(jsonObject["ValueType"].ToString(), jsonObject["Value"].ToString());
				return boardPin;
			}
			return null;
		}

		public static void UpdateClientFromJson(ArduinoClient client, string serializedObject)
		{
			JObject jsonObject = JObject.Parse(serializedObject);
			UpdateModelFromJson(client, jsonObject);
			JArray? componentsArray = (JArray?)jsonObject["Components"];
			if (componentsArray != null)
			{
				if (client.Components.Count > 0)
				{
					for (int i = 0; i < client.Components.Count; i++)
					{
						string proprietyName = "Id";
						JObject? foundObject = FindObjecInArrayByPropriety(componentsArray, proprietyName, client.Components[i].Id);
						if (foundObject != null)
						{
							UpdateComponentFromJson(client.Components[i], foundObject);
						}
					}
				}
				else
				{
					foreach (var jsonComponent in componentsArray)
					{
						IGeneralComponent? component = ConvertComponent(jsonComponent.ToString(), false);
						if (component != null)
						{
							UpdateComponentReferences(component);
							component.ParentClient = client;
							client.Components.Add(component);
						}
					}
				}
			}
		}

		public static void UpdateComponentFromJson(IGeneralComponent component, JObject jsonObject)
		{
			UpdateModelFromJson(component, jsonObject);
			JArray? componentsArray = (JArray?)jsonObject["ConnectedPins"];
			if (componentsArray != null)
			{
				for (int i = 0; i < component.ConnectedPins.Count; i++)
				{
					string proprietyName = "Id";
					JObject? foundObject = FindObjecInArrayByPropriety(componentsArray, proprietyName, component.ConnectedPins[i].Id);
					if (foundObject != null)
					{
						UpdatePortPinFromJson(component.ConnectedPins[i], foundObject);
					}
				}
			}
		}

		public static void UpdatePortPinFromJson(PortPin portPin, JObject jsonObject)
		{
			UpdateModelFromJson(portPin, jsonObject);
			if (jsonObject.ContainsKey("DataReference"))
				portPin.DataReference = ConvertDataReference(jsonObject["DataReference"].ToString());
			if (jsonObject.ContainsKey("FlexiValue"))
				portPin.FlexiValue = ConvertFlexibleValue(jsonObject["FlexiValue"].ToString());
			if (jsonObject.ContainsKey("ValueType"))
				portPin.FlexiValue = ConvertFlexibleValue(jsonObject["ValueType"].ToString(), jsonObject["Value"].ToString());
		}

		private static JObject? FindObjecInArrayByPropriety(JArray objectArray, string proprietyName, object Value)
		{
			Type proprietyType = Value.GetType();

			foreach (var jsonComponent in objectArray)
			{
				JToken? propriety = jsonComponent[proprietyName];
				if (propriety != null)
				{
					object? value = propriety.ToObject(proprietyType);
					if (value != null)
					{
						if (value.ToString() == Value.ToString())
							return JObject.Parse(jsonComponent.ToString());
					}
				}
			}
			return null;
		}

		private static void UpdateComponentReferences(IGeneralComponent component)
		{
			foreach (PortPin boardPin in component.ConnectedPins)
			{
				boardPin.ParentComponent = component;
			}
		}

		private static ArduinoClient? GetClientFromJsonObject(JObject jsonObject, bool setNewGuid)
		{
			if (jsonObject["Id"] is null && !setNewGuid)
				return null;

			ArduinoClient client = new();
			UpdateModelFromJson(client, jsonObject, setNewGuid);
			client.State = ArduinoClient.ConnectionState.Offline;
			//client.State = (ArduinoClient.ConnectionState)Enum.Parse(typeof(ArduinoClient.ConnectionState), jsonObject["State"].ToString());

			return client;
		}

		public static void ConvertILogs(string serializedObject, out List<ILog> logList)
		{
			logList = new();

			JArray componentsArray = JArray.Parse(serializedObject);
			if (componentsArray != null)
			{
				foreach (var jsonComponent in componentsArray)
				{
					JObject jsonObject = JObject.Parse(jsonComponent.ToString());
					LoggingService.LogTypes componentId = LoggingService.GetTypeByString(jsonObject["LogType"].ToString());
					ILog? component = LoggingService.CreateById(componentId);
					if (component != null)
					{
						UpdateModelFromJson(component, jsonObject, false);
						logList.Add(component);
					}
				}
			}
		}

		public static void ConvertINodes(string serializedObject, out List<INode> nodesList)
		{
			nodesList = new();

			JArray nodesArray = JArray.Parse(serializedObject);
			if (nodesArray != null)
			{
				foreach (var jsonComponent in nodesArray)
				{
					INode? Node = ConvertINode(jsonComponent.ToString());
					if (Node != null)
					{
						nodesList.Add(Node);
					}
				}
			}
		}

		public static INode? ConvertINode(string serializedObject)
		{
			JObject jsonObject = JObject.Parse(serializedObject);
			GeneralNode.NodeTypes nodeId = GeneralNode.GetTypeByString(jsonObject["Type"].ToString());
			INode? Node = GeneralNode.CreateById(nodeId);
			if (Node != null)
				UpdateModelFromJson(Node, jsonObject, false);
			else
				return null;

			if (Node is ValueNode) { }
			else if (Node is DataLinkNode dataLinkComponent)
			{
				JObject linkFromObject = JObject.Parse(jsonObject["LinkFrom"].ToString());
				UpdateModelFromJson(dataLinkComponent.LinkFrom, linkFromObject, false);
			}

			return Node;
		}

		public static DataReference ConvertDataReference(string? serializedObject)
		{
			DataReference dataReference = new();

			if (string.IsNullOrEmpty(serializedObject)) return dataReference;
			JObject jsonObject = JObject.Parse(serializedObject);

			if (jsonObject != null)
			{
				UpdateModelFromJson(dataReference, jsonObject, false);
			}

			return dataReference;
		}

		public static FlexibleValue ConvertFlexibleValue(string? serializedObject)
		{
			FlexibleValue flexiValue = new();

			if (string.IsNullOrEmpty(serializedObject)) return flexiValue;
			JObject jsonObject = JObject.Parse(serializedObject);

			if (jsonObject != null)
			{
				flexiValue.Type = Enum.Parse<ObjectValueType>(jsonObject["Type"].ToString());
				flexiValue.SetOrDefault(jsonObject["Value"].ToString());
			}

			return flexiValue;
		}

		public static FlexibleValue ConvertFlexibleValue(string? type, string? value)
		{
			FlexibleValue flexiValue = new();
			if (!string.IsNullOrEmpty(type))
			{
				flexiValue.Type = Enum.Parse<ObjectValueType>(type);
			}
			flexiValue.SetOrDefault(value);

			return flexiValue;
		}

		public static void UpdateModelFromJson<T>(T model, JObject jsonObject, bool setNewGuid = false)
		{
			Type clientType = model.GetType();
			PropertyInfo[] properties = clientType.GetProperties();

			foreach (PropertyInfo property in properties)
			{
				if (jsonObject.TryGetValue(property.Name, out JToken? stringValue))
				{
					object? deserializedValue = null;

					if (property.PropertyType == typeof(Guid))
					{
						if (setNewGuid || string.IsNullOrEmpty(stringValue.ToString()))
							deserializedValue = Guid.NewGuid();
						else
							deserializedValue = Guid.Parse(stringValue.ToString());
					}
					else if (property.PropertyType == typeof(IPEndPoint))
					{
						if (stringValue != null && !string.IsNullOrEmpty(stringValue.ToString()))
						{
							deserializedValue = IPEndPoint.Parse(stringValue.ToString());
						}
					}
					else if (property.PropertyType == typeof(DateTime))
					{
						DateTime.TryParse(stringValue.ToString(), out DateTime dateTimeValue);
						if (dateTimeValue != default) { deserializedValue = dateTimeValue; }
					}
					else if (property.PropertyType == typeof(int))
					{
						int.TryParse(stringValue.ToString(), out int intValue);
						deserializedValue = intValue;
					}
					else if (property.PropertyType == typeof(double))
					{
						double.TryParse(stringValue.ToString(), NumberStyles.Float, CultureInfo.InstalledUICulture, out double doubleValue);
						deserializedValue = doubleValue;
					}
					else if (property.PropertyType == typeof(string))
					{
						deserializedValue = stringValue.ToString();
					}
					else if (property.PropertyType == typeof(bool))
					{
						bool.TryParse(stringValue.ToString(), out bool boolValue);
						deserializedValue = boolValue;
					}
					else if (property.PropertyType.IsEnum)
					{
						deserializedValue = Enum.Parse(property.PropertyType, stringValue.ToString());
					}
					else
					{
						ComplementaryChecks(model, property, stringValue, out deserializedValue);
                    }

					if (deserializedValue != null)
					{
						property.SetValue(model, deserializedValue);
					}
				}
			}
		}

		private static void ComplementaryChecks<T>(T model, PropertyInfo property, JToken? stringValue, out object deserializedValue)
		{
			deserializedValue = null;

			if (model is INode)
			{
				if (property.PropertyType == typeof(FlexibleValue))
				{
					if (stringValue != null && stringValue is JObject jObject)
					{
						FlexibleValue flexibleValue = new()
						{
							Type = Enum.Parse<ObjectValueType>(jObject["Type"].ToString())
						};
						string val = jObject["Value"].ToString();
						FlexibleValue.TryParse(val, flexibleValue.Type, out object value);
						flexibleValue.Set(value);
						deserializedValue = flexibleValue;
					}
				}
			}
		}
	}
}
