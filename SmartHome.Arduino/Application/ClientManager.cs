using Newtonsoft.Json;
using SmartHome.Arduino.Application.Events;
using SmartHome.Arduino.Models.Arduino;
using SmartHome.Arduino.Models.Components.Common.Interfaces;
using SmartHome.Arduino.Models.Json.Converting;
using SmartHome.Arduino.Models.Json.FileStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Arduino.Application
{
    public static class ClientManager
    {
        private static readonly string FileName = "ClientData.json";
        public static List<ArduinoClient> Clients { get; set; } = new List<ArduinoClient>();

        public static void AddNewClient(ArduinoClient client)
        {
            Clients.Add(client);
            ClientEvents.TriggerClientAdded();
            ClientEvents.TriggerClientChanged();
        }

        public static void ReplaceClient(ArduinoClient client)
        {
            int index = Clients.FindIndex(c => c.Id == client.Id);
            if (index == -1)
            {
                AddNewClient(client);
            }
            else
            {
                ArduinoClient bufferClient = Clients[index];
                Clients[index] = client;
                Clients[index].Id = bufferClient.Id;
                if (bufferClient.State == ArduinoClient.ConnectionState.Online)
                {
                    Clients[index].Ping = (int)Clients[index].LastConnection.Subtract(bufferClient.LastConnection).TotalMilliseconds;
                }
            }
            ClientEvents.TriggerClientChanged();
        }

        public static bool GetClientIndexById(Guid clientId, out int index)
        {
            index = Clients.FindIndex(c => c.Id == clientId);
            return index != -1;
        }

        public static bool GetComponentIndexById(int clientIndex, int componentId, out int index)
        {
            index = Clients[clientIndex].Components.FindIndex(c => c.Id == componentId);
            return index != -1;
        }

        public static bool GetBoardPinIndexById(int clientIndex, int componentIndex, int boardPinId, out int index)
        {
            index = Clients[clientIndex].Components[componentIndex].ConnectedPins.FindIndex(c => c.Id == boardPinId);
            return index != -1;
        }

        public static void SaveClientData()
        {
            FileDataStorage.SaveDataToJsonFile(Clients, FileName);
        }

        public static void SaveClientTestData()
        {
            FileDataStorage.SaveDataToJsonFile(TestDecoy, FileName);
        }

        public static void RecoverClientData()
        {
            string? serializedObject = FileDataStorage.ReadStringFromFile(FileName);
            if (string.IsNullOrEmpty(serializedObject)) return;
            List<ArduinoClient>? arduinoClients = JsonDataConverting.ConvertClients(serializedObject, false);
            if (arduinoClients is not null)
            {
                Clients = arduinoClients;
            }
        }


        //  ----------  FOR TESTING  ----------  //
        public static List<ArduinoClient> TestDecoy = new()
        {
            new ArduinoClient()
            {
                Id = Guid.NewGuid(),
                Name = "Living Parameters",
                Components = new List<IGeneralComponent>()
                {
                    new Models.Components.LightSensor()
                    {
                        Id = 0,
                        Sequence = 0,
                        ConnectedPins = new List<BoardPin>()
                        {
                            new BoardPin()
                            {
                                Id = 4,
                                Mode = BoardPin.PinMode.Read,
                                Value = 454,
                                ValueType = BoardPin.ObjectValueType.Integer,
                            }
                        },
                        Description = "Some dumb message",
                    },
                    new Models.Components.Relay()
                    {
                        Id = 1,
                        Sequence = 1,
                        ConnectedPins = new List<BoardPin>()
                        {
                            new BoardPin()
                            {
                                Id = 6,
                                Mode = BoardPin.PinMode.Read,
                                Value = true,
                                ValueType = BoardPin.ObjectValueType.Boolean,
                            }
                        },
                        Description = "Another dumb message??",
                    }
                },
                Model = "Arduino UNO WiFi",
                Description = string.Empty,
                IP = new System.Net.IPEndPoint(123424, 8088),
                LastConnection = Server.GetDTNow(),
                State = ArduinoClient.ConnectionState.Online
            },
            new ArduinoClient()
            {
                Id = Guid.NewGuid(),
                Name = "Living Parameters",
                Components = new List<IGeneralComponent>()
                {
                    new Models.Components.LightSensor()
                    {
                        Id = 0,
                        Sequence = 0,
                        ConnectedPins = new List<BoardPin>()
                        {
                            new BoardPin()
                            {
                                Id = 4,
                                Mode = BoardPin.PinMode.Read,
                                Value = 454,
                                ValueType = BoardPin.ObjectValueType.Integer,
                            }
                        },
                        Description = string.Empty,
                    }
                },
                Model = "Arduino UNO WiFi",
                Description = string.Empty,
                IP = new System.Net.IPEndPoint(123424, 8088),
                LastConnection = Server.GetDTNow(),
                State = ArduinoClient.ConnectionState.Online
            },
            new ArduinoClient()
            {
                Id = Guid.NewGuid(),
                Name = "Living Parameters",
                Components = new List<IGeneralComponent>()
                {
                    new Models.Components.Relay()
                    {
                        Id = 0,
                        Sequence = 0,
                        ConnectedPins = new List<BoardPin>()
                        {
                            new BoardPin()
                            {
                                Id = 4,
                                Mode = BoardPin.PinMode.Read,
                                Value = 454,
                                ValueType = BoardPin.ObjectValueType.Integer,
                            }
                        },
                        Description = string.Empty,
                    }
                },
                Model = "Arduino UNO WiFi",
                Description = string.Empty,
                IP = new System.Net.IPEndPoint(123424, 8088),
                LastConnection = Server.GetDTNow(),
                State = ArduinoClient.ConnectionState.Online
            },

            new ArduinoClient()
            {
                Id = Guid.NewGuid(),
                Name = "Living Parameters",
                Components = new List<IGeneralComponent>()
                {
                    new Models.Components.LightSensor()
                    {
                        Id = 0,
                        Sequence = 0,
                        ConnectedPins = new List<BoardPin>()
                        {
                            new BoardPin()
                            {
                                Id = 4,
                                Mode = BoardPin.PinMode.Read,
                                Value = 454,
                                ValueType = BoardPin.ObjectValueType.Integer,
                            }
                        },
                        Description = string.Empty,
                    }
                },
                Model = "Arduino UNO WiFi",
                Description = string.Empty,
                IP = new System.Net.IPEndPoint(123424, 8088),
                LastConnection = Server.GetDTNow(),
                State = ArduinoClient.ConnectionState.Online
            },
            new ArduinoClient()
            {
                Id = Guid.NewGuid(),
                Name = "Living Parameters",
                Components = new List<IGeneralComponent>()
                {
                    new Models.Components.Relay()
                    {
                        Id = 0,
                        Sequence = 0,
                        ConnectedPins = new List<BoardPin>()
                        {
                            new BoardPin()
                            {
                                Id = 4,
                                Mode = BoardPin.PinMode.Read,
                                Value = 454,
                                ValueType = BoardPin.ObjectValueType.Integer,
                            }
                        },
                        Description = string.Empty,
                    }
                },
                Model = "Arduino UNO WiFi",
                Description = string.Empty,
                IP = new System.Net.IPEndPoint(123424, 8088),
                LastConnection = Server.GetDTNow(),
                State = ArduinoClient.ConnectionState.Online
            },
        };
    }
}
