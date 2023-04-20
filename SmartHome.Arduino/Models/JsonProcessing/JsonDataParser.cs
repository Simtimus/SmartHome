using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartHome.Arduino.Application.Exceptions;
using SmartHome.Arduino.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SmartHome.Arduino.Models.JsonProcessing
{
    public static class JsonDataParser
    {
        public static ReceivedData ParseReceivedData(ReceivedData receivedData)
        {
            JObject jsonObject = JObject.Parse(receivedData.Data);
            UpdateModelByJsonObject(receivedData.Data, jsonObject);
            return receivedData;
        }

        public static List<ArduinoClient> ParseClients(string serializedObject, bool setNewGuid)
        {
            List<ArduinoClient> arduinoClients = new();

            JArray jsonArray = JArray.Parse(serializedObject);
            foreach (var jsonClient in jsonArray)
            {
                ArduinoClient? client = ParseClient(jsonClient.ToString(), setNewGuid);
                if (client != null)
                {
                    arduinoClients.Add(client);
                }
            }

            return arduinoClients;
        }

        public static ArduinoClient? ParseClientOnly(string serializedObject, bool setNewGuid)
        {
            JObject jsonObject = JObject.Parse(serializedObject);

            ArduinoClient? client = GetClientFromJsonObject(jsonObject, setNewGuid);
            if (ArduinoClient.IsNullOrEmpty(client))
                return null;

            return client;
        }

        public static ArduinoClient? ParseClient(string serializedObject, bool setNewGuid)
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
                    IGeneralComponent? component = ParseComponent(jsonComponent.ToString(), setNewGuid);
                    if (component != null)
                    {
                        UpdateComponentChildrenReferences(component);
                        component.ParentClient = client;
                        client.Components.Add(component);
                    }
                }
            }

            return client;
        }

        public static IGeneralComponent? ParseComponent(string serializedObject, bool setNewGuid)
        {
            JObject jsonObject = JObject.Parse(serializedObject);
            GeneralComponent.ComponentsId componentId = GeneralComponent.GetIdByString(jsonObject["ComponentId"].ToString());
            IGeneralComponent? component = GeneralComponent.CreateById(componentId);
            if (component != null)
                UpdateModelByJsonObject(component, jsonObject, setNewGuid);
            else
                return null;

            JArray? componentsArray = (JArray?)jsonObject["ConnectedPins"];
            if (componentsArray != null)
            {
                foreach (var jsonComponent in componentsArray)
                {
                    BoardPin? boardPin = ParseBoardPin(jsonComponent.ToString());
                    if (boardPin != null)
                    {
                        component.ConnectedPins.Add(boardPin);
                    }
                }
            }

            return component;
        }

        public static BoardPin? ParseBoardPin(string serializedObject)
        {
            JObject jsonObject = JObject.Parse(serializedObject);
            if (jsonObject != null)
            {
                BoardPin? boardPin = new();
                UpdateModelByJsonObject(boardPin, jsonObject, false); ;
                return boardPin;
            }
            return null;
            //return (BoardPin?)JsonConvert.DeserializeObject(serializedObject);
        }

        public static void UpdateArduinoClient(ArduinoClient client, string serializedObject)
        {
            JObject jsonObject = JObject.Parse(serializedObject);
            UpdateModelByJsonObject(client, jsonObject);
            JArray? componentsArray = (JArray?)jsonObject["Components"];
            if (componentsArray != null)
            {           
                for (int i = 0; i < client.Components.Count; i++)
                {
                    var idPropriety = client.Components[i].Id;
                    JObject? foundObject = FindObjecInArrayByPropriety(componentsArray, idPropriety);
                    if (foundObject != null)
                    {
                        UpdateGeneralComponent(client.Components[i], foundObject);
                    }
                }
            }
        }

        public static void UpdateGeneralComponent(IGeneralComponent component, JObject jsonObject)
        {
            UpdateModelByJsonObject(component, jsonObject);
            JArray? componentsArray = (JArray?)jsonObject["ConnectedPins"];
            if (componentsArray != null)
            {
                for (int i = 0; i < component.ConnectedPins.Count; i++)
                {
                    var idPropriety = component.ConnectedPins[i].Id;
                    JObject? foundObject = FindObjecInArrayByPropriety(componentsArray, idPropriety);
                    if (foundObject != null)
                    {
                        UpdateModelByJsonObject(component.ConnectedPins[i], foundObject);
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

        private static void UpdateComponentChildrenReferences(IGeneralComponent component)
        {
            foreach (BoardPin boardPin in component.ConnectedPins)
            {
                boardPin.ParentComponent = component;
            }
        }

        private static ArduinoClient? GetClientFromJsonObject(JObject jsonObject, bool setNewGuid)
        {
            if (jsonObject["Id"] is null && !setNewGuid)
                return null;

            ArduinoClient client = new();
            UpdateModelByJsonObject(client, jsonObject, setNewGuid);
            client.State = ArduinoClient.ConnectionState.Offline;
            //client.State = (ArduinoClient.ConnectionState)Enum.Parse(typeof(ArduinoClient.ConnectionState), jsonObject["State"].ToString());

            return client;
        }

        public static void UpdateModelByJsonObject(object model, JObject jsonObject, bool setNewGuid = false)
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
                        if (value != null)
                        {
                            deserializedValue = IPEndPoint.Parse(value.ToString());
                        }
                    }
                    else if (property.PropertyType == typeof(DateTime))
                    {
                        deserializedValue = DateTime.Parse(value.ToString());
                    }
                    else if (property.PropertyType == typeof(int))
                    {
                        deserializedValue = int.Parse(value.ToString());
                    }
                    else if (property.PropertyType == typeof(double))
                    {
                        deserializedValue = double.Parse(value.ToString());
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
