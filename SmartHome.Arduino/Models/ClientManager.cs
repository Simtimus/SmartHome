using Newtonsoft.Json;
using SmartHome.Arduino.Application.Modules.DataSaving;
using SmartHome.Arduino.Models.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Arduino.Models
{
    public class ClientManager
    {
        private readonly string FileName = "ClientData.json";
        public List<ArduinoClient> Clients { get; set; } = new List<ArduinoClient>();
        
        public void AddNewClient(ArduinoClient client)
        {
            client.Id = Guid.NewGuid();
            Clients.Add(client);
        }

        public void UpdateClient(ArduinoClient client)
        {
            ArduinoClient? foundClient = Clients.Find(x => x.Id == client.Id);
            if (ArduinoClient.IsNullOrEmpty(foundClient))
            {
                AddNewClient(client);
            }
            else
            {
                ArduinoClient bufferClient = foundClient;
                foundClient = client;
                foundClient.Id = bufferClient.Id;
            }
        }

        public void SaveClientData()
        {
            JsonDataManager.SaveObjectToFile(FileName, Clients);
        }

        public void SaveClientTestData()
        {
            JsonDataManager.SaveObjectToFile(FileName, TestDecoy);
        }

        public void RecoverClientData()
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
            catch (Exception ex) { }
        }

        public List<ArduinoClient> TestDecoy = new List<ArduinoClient>()
        {
            new ArduinoClient()
            {
                Id = Guid.NewGuid(),
                Name = "Living Parameters",
                Components = new List<iGenericComponent>()
                {
                    new Components.LightSensor.LightSensor()
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
                    new Components.Relay.Relay()
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
                        Description = String.Empty,
                    }
                },
                Model = "Arduino UNO WiFi",
                Description = String.Empty,
                IP = new System.Net.IPEndPoint(123424, 8088),
                LastConnection = Server.GetDTNow(),
                State = ArduinoClient.ConnectionState.Online
            },
            new ArduinoClient()
            {
                Id = Guid.NewGuid(),
                Name = "Living Parameters",
                Components = new List<iGenericComponent>()
                {
                    new Components.LightSensor.LightSensor()
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
                        Description = String.Empty,
                    }
                },
                Model = "Arduino UNO WiFi",
                Description = String.Empty,
                IP = new System.Net.IPEndPoint(123424, 8088),
                LastConnection = Server.GetDTNow(),
                State = ArduinoClient.ConnectionState.Online
            },
            new ArduinoClient()
            {
                Id = Guid.NewGuid(),
                Name = "Living Parameters",
                Components = new List<iGenericComponent>()
                {
                    new Components.LightSensor.LightSensor()
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
                        Description = String.Empty,
                    }
                },
                Model = "Arduino UNO WiFi",
                Description = String.Empty,
                IP = new System.Net.IPEndPoint(123424, 8088),
                LastConnection = Server.GetDTNow(),
                State = ArduinoClient.ConnectionState.Online
            },

            new ArduinoClient()
            {
                Id = Guid.NewGuid(),
                Name = "Living Parameters",
                Components = new List<iGenericComponent>()
                {
                    new Components.LightSensor.LightSensor()
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
                        Description = String.Empty,
                    }
                },
                Model = "Arduino UNO WiFi",
                Description = String.Empty,
                IP = new System.Net.IPEndPoint(123424, 8088),
                LastConnection = Server.GetDTNow(),
                State = ArduinoClient.ConnectionState.Online
            },
            new ArduinoClient()
            {
                Id = Guid.NewGuid(),
                Name = "Living Parameters",
                Components = new List<iGenericComponent>()
                {
                    new Components.LightSensor.LightSensor()
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
                        Description = String.Empty,
                    }
                },
                Model = "Arduino UNO WiFi",
                Description = String.Empty,
                IP = new System.Net.IPEndPoint(123424, 8088),
                LastConnection = Server.GetDTNow(),
                State = ArduinoClient.ConnectionState.Online
            },
        };
    }
}
