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
using SmartHome.Arduino.Models.Data.Processing;
using SmartHome.Arduino.Models.Data.Received;
using SmartHome.Arduino.Application.Logging;
using SmartHome.Arduino.Models.Logs;

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

        private static readonly TimeZoneInfo TimeZone = TimeZoneInfo.FindSystemTimeZoneById("GTB Standard Time");

        private static DateTime LastSave = new();

        public Server()
        {
            ipHost = GetLocalIPv4(NetworkInterfaceType.Wireless80211);
            //Console.WriteLine(ipHost);
            LoggingService.GetAllLogs();
            //ClientManager.SaveClientTestData();
            ClientManager.RecoverClientData();
            Task.Run(() => ReceiveAndProcessMessages());
            Task.Run(() => MonitorClients());
        }

        private static async Task MonitorClients()
        {
            while (true)
            {
                DateTime currentTime = GetDTNow();
                foreach (var client in ClientManager.Clients)
                {
                    MonitorClientState(client, client.LastConnection.AddSeconds(secondsUntilOffline));
                    if (client.State != ArduinoClient.ConnectionState.Offline)
                        MonitorClientPing(client, currentTime);
                }
                if (currentTime >= LastSave.AddSeconds(secondsBetweenSaves))
                {
                    ClientManager.SaveClientData();
                    LastSave = currentTime;
                }
                await Task.Delay(1000);
            }
        }

        private static void MonitorClientPing(ArduinoClient client, DateTime currentTime)
        {
            double timeDelta = currentTime.Subtract(client.LastConnection).TotalMilliseconds;
            client.Ping = (int)timeDelta;
        }

        private static void MonitorClientState(ArduinoClient client, DateTime offlineTime)
        {
            offlineTime.AddSeconds(secondsUntilOffline);

            if (offlineTime.Subtract(GetDTNow()).TotalSeconds <= 0)
            {
                client.State = ArduinoClient.ConnectionState.Offline;
                ClientEvents.TriggerClientChanged();
            }
        }

        private void ReceiveAndProcessMessages()
        {
            IPEndPoint? recivedClientIP;
            while (true)
            {
                recivedClientIP = null;
                byte[] data = server.Receive(ref recivedClientIP);
                string message = Encoding.UTF8.GetString(data);

                ReceivedData receivedData = new()
                {
                    IP = recivedClientIP,
                    LastConnection = GetDTNow(),
                    Data = message,
                };

                try
                {
                    // Process Received Data from Arduino
                    DeviceDataProcessor.HandleReceivedData(receivedData);
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

        public int SendMessageToClientById(Guid id, string message)
        {
            ClientManager.GetClientIndexById(id, out int index);
            if (index == -1) return 0;

            byte[] byteMsg = Encoding.UTF8.GetBytes(message);
            return server.Send(byteMsg, byteMsg.Length, ClientManager.Clients[index].IP);
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
