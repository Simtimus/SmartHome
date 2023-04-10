using Newtonsoft.Json;
using SmartHome.Arduino.Application.Modules.DataSaving;
using SmartHome.Arduino.Models;
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

        public static bool ClientsUpdated = false;

        public static void AddNewClient(ArduinoClient client)
        {
            Clients.Add(client);
            ClientsUpdated = true;
        }

        public static void UpdateClient(ArduinoClient client)
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
            ClientsUpdated = true;
        }

        public static void SaveClientData()
        {
            JsonDataManager.SaveObjectToFile(FileName, Clients);
        }

        public static void SaveClientTestData()
        {
            JsonDataManager.SaveObjectToFile(FileName, TestDecoy);
        }

        public static void RecoverClientData()
        {
            string? serializedObject = JsonDataManager.GetObjectFromFile(FileName);
            if (string.IsNullOrEmpty(serializedObject)) return;
            try
            {
                List<ArduinoClient>? arduinoClients = JsonDataParser.ParseClients(serializedObject);
                if (arduinoClients is not null)
                {
                    Clients = arduinoClients;
                }
            }
            catch (Exception) { }
        }

        public static List<ArduinoClient> TestDecoy = new()
        {
            new ArduinoClient()
            {
                Id = Guid.NewGuid(),
                Name = "Living Parameters",
                Components = new List<IGenericComponent>()
                {
                    new Models.Components.LightSensor()
                    {
                        Id = Guid.NewGuid(),
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
                        Id = Guid.NewGuid(),
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
                Components = new List<IGenericComponent>()
                {
                    new Models.Components.LightSensor()
                    {
                        Id = Guid.NewGuid(),
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
                Components = new List<IGenericComponent>()
                {
                    new Models.Components.LightSensor()
                    {
                        Id = Guid.NewGuid(),
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
                Components = new List<IGenericComponent>()
                {
                    new Models.Components.LightSensor()
                    {
                        Id = Guid.NewGuid(),
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
                Components = new List<IGenericComponent>()
                {
                    new Models.Components.LightSensor()
                    {
                        Id = Guid.NewGuid(),
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
