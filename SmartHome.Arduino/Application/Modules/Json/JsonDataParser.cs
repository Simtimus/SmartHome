using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartHome.Arduino.Application.Exceptions;
using SmartHome.Arduino.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SmartHome.Arduino.Application.Modules.DataSaving
{
    public static class JsonDataParser
    {
        public static List<ArduinoClient> ParseClients(string serializedObject)
        {
            List<ArduinoClient> arduinoClients = new();

            JArray jsonArray = JArray.Parse(serializedObject);
            foreach (var jsonClient in jsonArray)
            {
                ArduinoClient? client = ParseClient(jsonClient.ToString());
                if (client != null)
                {
                    arduinoClients.Add(client);
                }
            }

            return arduinoClients;
        }

        public static ArduinoClient? ParseClient(string serializedObject, bool setNewGuid = false)
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
                    IGenericComponent? component = ParseComponent(jsonComponent.ToString());
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

        public static IGenericComponent? ParseComponent(string serializedObject, bool setNewGuid = false)
        {
            JObject jsonObject = JObject.Parse(serializedObject);
            GenericComponent.ComponentsId componentId = GenericComponent.GetIdByString(jsonObject["ComponentId"].ToString());
            IGenericComponent? component = GenericComponent.CreateById(componentId);
            if (component != null)
                UpdateModelByJsonObject(component, jsonObject, setNewGuid);
            else
                return null;

            JArray? componentsArray = (JArray?)jsonObject["Components"];
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
            return (BoardPin?)JsonConvert.DeserializeObject(serializedObject);
        }

        private static void UpdateComponentChildrenReferences(IGenericComponent component)
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

        public static void UpdateModelByJsonObject<T>(T model, JObject jsonObject, bool setNewGuid)
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
