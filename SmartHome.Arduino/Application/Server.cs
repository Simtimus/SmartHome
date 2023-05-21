using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using SmartHome.Arduino.Application.Events;
using SmartHome.Arduino.Models.Arduino;
using SmartHome.Arduino.Models.Data.Received;
using SmartHome.Arduino.Application.Logging;
using SmartHome.Arduino.Models.Logs;
using SmartHome.Arduino.Models.Data.DataBoxs;
using SmartHome.Arduino.Models.Data.Transmited;
using SmartHome.Arduino.Models.Json.Converting;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using SmartHome.Arduino.Models.Data.DataLinks;
using SmartHome.Arduino.Models.Components.Common.Interfaces;

namespace SmartHome.Arduino.Application
{
    public class Server
    {
        public string IpHost { get { return ipHost; } }
        private readonly string ipHost = "0.0.0.0";

        public int PortHost { get { return portHost; } }
        public const int portHost = 8080;

        private const int secondsUntilOffline = 60;
        private const int secondsBetweenSaves = 30;

        private readonly UdpClient server = new(portHost);
        private DataTransmittingManager transmittingManager;
        private ArduinoDataProcessor dataProcessor;
        private static readonly TimeZoneInfo TimeZone = TimeZoneInfo.FindSystemTimeZoneById("GTB Standard Time");
        private static DateTime LastSave = new();

        public Server()
        {
            ipHost = GetLocalIPv4(NetworkInterfaceType.Wireless80211);
            transmittingManager = new(server);
            dataProcessor = new(transmittingManager);
            //Console.WriteLine(ipHost);
            LoggingService.RecoverAllLogs();
            //ClientManager.SaveClientTestData();
            ClientManager.RecoverClientData();
            DataLinker.RecoverDataLinks();
            Task.Run(() => ReceiveAndProcessMessages());
            Task.Run(async () => await MonitorClients());
        }

        private async Task MonitorClients()
        {
			ClientEvents.OnClientChanged += SendUpdatedValues;
			while (true)
            {
                foreach (var client in ClientManager.Clients)
                {
                    MonitorClientState(client, client.LastConnection.AddSeconds(secondsUntilOffline));
                    if (client.State != ArduinoClient.ConnectionState.Offline)
                        MonitorClientPing(client, GetDTNow());
                }
                if (GetDTNow() >= LastSave.AddSeconds(secondsBetweenSaves))
                {
                    ClientManager.SaveClientData();
                    LastSave = GetDTNow();
                }
                await Task.Delay(1000);
            }
        }

        private void SendUpdatedValues()
        {
            TransmitedData transmitedData;
			foreach (ArduinoClient client in ClientManager.Clients)
            {
                transmitedData = new()
                {
                    BoardId = Guid.Parse(client.Id.ToString()),
                };
                foreach (IGeneralComponent component in client.Components)
                {
                    foreach (PortPin portPin in component.ConnectedPins)
                    {
                        if (portPin.Mode == PortPin.PinMode.Write)
                        {
                            TransmitedCommand command = new();
                            command.Action = TransmitedCommand.CommandAction.SetValue;
                            command.ComponentId = component.Id;
                            command.PinId = portPin.Id;
                            command.Value = portPin.GetValueString();
                            transmitedData.Commands.Add(command);
                        }
                    }    
                }
                transmittingManager.TransmitData(transmitedData);
            }
        }

        private static void MonitorClientPing(ArduinoClient client, DateTime currentTime)
        {
            double timeDelta = currentTime.Subtract(client.LastConnection).TotalMilliseconds;
            client.Ping = (int)timeDelta;
            ClientEvents.TriggerClientChanged();
        }

        private static void MonitorClientState(ArduinoClient client, DateTime offlineTime)
        {
            if (offlineTime.Subtract(GetDTNow()).TotalSeconds <= 0)
            {
                client.State = ArduinoClient.ConnectionState.Offline;
                ClientEvents.TriggerClientChanged();
            }
        }

        private void ReceiveAndProcessMessages()
        {
            IPEndPoint? receivedClientIP;
            while (true)
            {
                receivedClientIP = null;
                byte[] data = server.Receive(ref receivedClientIP);
                string message = Encoding.UTF8.GetString(data);

                ArduinoDataPacket receivedData = new()
                {
                    IP = receivedClientIP,
                    LastConnection = GetDTNow(),
                    Data = message,
                };

                try
                {
                    // Process Received Data from Arduino
                    dataProcessor.ProcessArduinoData(receivedData);
                }
                catch(Exception ex)
                {
                    LoggingService.WarningLog(new MessageLog()
                    {
                        Message = ex.Message,
                    });
                }
            }
        }

        public static DateTime GetDTNow()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZone);
        }

        private static string GetLocalIPv4(NetworkInterfaceType _type)
        {
            string output = "";
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (item.NetworkInterfaceType == _type && item.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            output = ip.Address.ToString();
                        }
                    }
                }
            }
            return output;
        }
    }
}
