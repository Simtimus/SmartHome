﻿using SmartHome.Arduino.Application.Events;
using SmartHome.Arduino.Models.Arduino;
using SmartHome.Arduino.Models.Components.Common.Interfaces;
using SmartHome.Arduino.Models.Json.Converting;
using SmartHome.Arduino.Models.Json.FileStorage;

namespace SmartHome.Arduino.Application
{
    public static class ClientManager
    {
        private static readonly string FileName = "ArduinoDataPacket.json";
        public static List<ArduinoClient> Clients { get; set; } = new List<ArduinoClient>();
        private static readonly object _lock = new object();

        public static void AddNewClient(ArduinoClient client)
        {
            lock (_lock)
            {
                Clients.Add(client);
            }
            ClientEvents.TriggerClientAdded();
            ClientEvents.TriggerClientChanged();
        }

        public static void ReplaceClient(ArduinoClient client)
        {
            lock (_lock)
            {
                bool clientFound = GetClientIndexById(client.Id, out int index);
                if (!clientFound)
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
            }
            ClientEvents.TriggerClientChanged();
        }

        public static bool GetClientIndexById(Guid clientId, out int index)
        {
            lock (_lock)
            {
                index = Clients.FindIndex(c => c.Id == clientId);
            }
            return index != -1;
        }

        public static bool GetComponentIndexById(int clientIndex, int componentId, out int index)
        {
            lock (_lock)
            {
                index = Clients[clientIndex].Components.FindIndex(c => c.Id == componentId);
            }
            return index != -1;
        }

        public static bool GetBoardPinIndexById(int clientIndex, int componentIndex, int boardPinId, out int index)
        {
            lock (_lock)
            {
                index = Clients[clientIndex].Components[componentIndex].ConnectedPins.FindIndex(c => c.Id == boardPinId);
            }
            return index != -1;
        }

        public static void SaveClientData()
        {
            lock (_lock)
            {
                FileDataStorage.SaveDataToJsonFile(Clients, FileName);
            }
        }

        public static void SaveClientTestData()
        {
            lock (_lock)
            {
                FileDataStorage.SaveDataToJsonFile(TestDecoy, FileName);
            }
        }

        public static void RecoverClientData()
        {
            lock (_lock)
            {
                string? serializedObject = FileDataStorage.ReadStringFromFile(FileName);
                if (string.IsNullOrEmpty(serializedObject)) return;
                List<ArduinoClient>? arduinoClients = JsonDataConverting.ConvertClients(serializedObject, false);
                if (arduinoClients is not null)
                {
                    Clients = arduinoClients;
                }
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
                        ConnectedPins = new List<PortPin>()
                        {
                            new PortPin()
                            {
                                Id = 4,
                                Mode = PortPin.PinMode.Read,
                                Value = "454",
                                ValueType = PortPin.ObjectValueType.Integer,
                            }
                        },
                        Description = "Some dumb message",
                    },
                    new Models.Components.Relay()
                    {
                        Id = 1,
                        ConnectedPins = new List<PortPin>()
                        {
                            new PortPin()
                            {
                                Id = 6,
                                Mode = PortPin.PinMode.Read,
                                Value = "true",
                                ValueType = PortPin.ObjectValueType.Boolean,
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
                        ConnectedPins = new List<PortPin>()
                        {
                            new PortPin()
                            {
                                Id = 4,
                                Mode = PortPin.PinMode.Read,
                                Value = "454",
                                ValueType = PortPin.ObjectValueType.Integer,
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
                        ConnectedPins = new List<PortPin>()
                        {
                            new PortPin()
                            {
                                Id = 4,
                                Mode = PortPin.PinMode.Read,
                                Value = "454",
                                ValueType = PortPin.ObjectValueType.Integer,
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
                        ConnectedPins = new List<PortPin>()
                        {
                            new PortPin()
                            {
                                Id = 4,
                                Mode = PortPin.PinMode.Read,
                                Value = "454",
                                ValueType = PortPin.ObjectValueType.Integer,
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
                        ConnectedPins = new List<PortPin>()
                        {
                            new PortPin()
                            {
                                Id = 4,
                                Mode = PortPin.PinMode.Read,
                                Value = "454",
                                ValueType = PortPin.ObjectValueType.Integer,
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



