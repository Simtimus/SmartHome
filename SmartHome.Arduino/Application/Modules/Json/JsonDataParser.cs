using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        public static ArduinoClient? ParseClient(string serializedObject)
        {
            JObject jsonObject = JObject.Parse(serializedObject);

            ArduinoClient? client = GetClientFromJsonObject(jsonObject);
            if (ArduinoClient.IsNullOrEmpty(client))
                return null;

            JArray componentsArray = (JArray)jsonObject["Components"];
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

            return client;
        }

        public static IGenericComponent? ParseComponent(string serializedObject)
        {
            JObject jsonObject = JObject.Parse(serializedObject);
            GenericComponent.ComponentsId componentId = GenericComponent.GetIdByString(jsonObject["ComponentId"].ToString());
            Type? componentType = GenericComponent.GetTypeById(componentId);
            if (componentType is null)
                return null;
            object? deserializedObject = JsonConvert.DeserializeObject(serializedObject, componentType);

            return deserializedObject as IGenericComponent;
        }

        private static void UpdateComponentChildrenReferences(IGenericComponent component)
        {
            foreach (BoardPin boardPin in component.ConnectedPins)
            {
                boardPin.ParentComponent = component;
            }
        }

        private static ArduinoClient? GetClientFromJsonObject(JObject jsonObject)
        {
            if (jsonObject["Id"] is null)
                return null;

            ArduinoClient client = new();
            UpdateClientByJsonObject(client, jsonObject);
            client.State = ArduinoClient.ConnectionState.Offline;
            //client.State = (ArduinoClient.ConnectionState)Enum.Parse(typeof(ArduinoClient.ConnectionState), jsonObject["State"].ToString());

            return client;
        }

        public static void UpdateClientByJsonObject(ArduinoClient client, JObject jsonObject)
        {
            Type clientType = typeof(ArduinoClient);
            PropertyInfo[] properties = clientType.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (jsonObject.TryGetValue(property.Name, out JToken? value))
                {
                    object? deserializedValue = null;

                    if (property.PropertyType == typeof(Guid))
                    {
                        //  Checks if `client` has a GUID
                        if (value.Type == JTokenType.Null)
                        {
                            client = new();
                            break;
                        }
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
                    //else if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                    //{
                    //    Type itemType = property.PropertyType.GetGenericArguments()[0];
                    //    deserializedValue = JsonConvert.DeserializeObject(value.ToString(), typeof(List<>).MakeGenericType(itemType));
                    //}

                    if (deserializedValue != null)
                    {
                        property.SetValue(client, deserializedValue);
                    }
                }
            }
        }
    }
}
