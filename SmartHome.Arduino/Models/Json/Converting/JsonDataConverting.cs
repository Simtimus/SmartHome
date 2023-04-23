using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartHome.Arduino.Application.Exceptions;
using SmartHome.Arduino.Models.Interfaces;
using SmartHome.Arduino.Models.Data.Received;
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

namespace SmartHome.Arduino.Models.Json.Converting
{
    public static class JsonDataConverting
    {
        public static void ConvertReceivedData(ReceivedData receivedData)
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
            GeneralComponent.ComponentsId componentId = GeneralComponent.GetIdByString(jsonObject["ComponentId"].ToString());
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
                UpdateModelFromJson(boardPin, jsonObject, false); ;
                return boardPin;
            }
            return null;
            //return (BoardPin?)JsonConvert.DeserializeObject(serializedObject);
        }

        public static void UpdateClientFromJson(ArduinoClient client, string serializedObject)
        {
            JObject jsonObject = JObject.Parse(serializedObject);
            UpdateModelFromJson(client, jsonObject);
            JArray? componentsArray = (JArray?)jsonObject["Components"];
            if (componentsArray != null)
            {
                for (int i = 0; i < client.Components.Count; i++)
                {
                    var idPropriety = client.Components[i].Id;
                    JObject? foundObject = FindObjecInArrayByPropriety(componentsArray, idPropriety);
                    if (foundObject != null)
                    {
                        UpdateComponentFromJson(client.Components[i], foundObject);
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
                    var idPropriety = component.ConnectedPins[i].Id;
                    JObject? foundObject = FindObjecInArrayByPropriety(componentsArray, idPropriety);
                    if (foundObject != null)
                    {
                        UpdateModelFromJson(component.ConnectedPins[i], foundObject);
                    }
                }
            }
        }

        private static JObject? FindObjecInArrayByPropriety(JArray objectArray, object proprietyValue)
        {
            string proprietyName = nameof(proprietyValue);
            Type proprietyType = proprietyValue.GetType();

            foreach (var jsonComponent in objectArray)
            {
                JToken? propriety = jsonComponent[proprietyName];
                if (propriety != null)
                {
                    object? value = propriety.ToObject(proprietyType);
                    if (value != null)
                    {
                        if (value == proprietyValue)
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

        public static void UpdateModelFromJson<T>(T model, JObject jsonObject, bool setNewGuid = false)
        {
            Type clientType = model.GetType();
            PropertyInfo[] properties = clientType.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (jsonObject.TryGetValue(property.Name, out JToken? value))
                {
                    object? deserializedValue = null;

                    if (property.PropertyType == typeof(Guid))
                    {
                        if (setNewGuid)
                            deserializedValue = Guid.NewGuid();
                        else
                            deserializedValue = Guid.Parse(value.ToString());
                    }
                    else if (property.PropertyType == typeof(IPEndPoint))
                    {
                        if (value != null && !string.IsNullOrEmpty(value.ToString()))
                        {
                            deserializedValue = IPEndPoint.Parse(value.ToString());
                        }
                    }
                    else if (property.PropertyType == typeof(DateTime))
                    {
                        DateTime.TryParse(value.ToString(), out DateTime dateTimeValue);
                        if (dateTimeValue != default) { deserializedValue = dateTimeValue; }
                    }
                    else if (property.PropertyType == typeof(int))
                    {
                        int.TryParse(value.ToString(), out int intValue);
                        deserializedValue = intValue;
                    }
                    else if (property.PropertyType == typeof(double))
                    {
                        double.TryParse(value.ToString(), out double doubleValue);
                        deserializedValue = doubleValue;
                    }
                    else if (property.PropertyType == typeof(string))
                    {
                        deserializedValue = value.ToString();
                    }
                    else if (property.PropertyType.IsEnum)
                    {
                        deserializedValue = Enum.Parse(property.PropertyType, value.ToString());
                    }

                    if (deserializedValue != null)
                    {
                        property.SetValue(model, deserializedValue);
                    }
                }
            }
        }
    }
}
