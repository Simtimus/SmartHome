using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Diagnostics;
using SmartHome.Arduino.Models;
using SmartHome.Arduino.Application.Modules.DataSaving;
using System.Runtime.CompilerServices;

namespace SmartHome.Arduino.Application
{
    public class Server
    {
        public string IpHost { get { return ipHost; } }
        private readonly string ipHost = "0.0.0.0";
        private const int ArduinoCycleTime = 1000;

        private const int PortHost = 8080;

        private const int secondsUntilOffline = 60;
        private const int secondsBetweenSaves = 30;

        private readonly UdpClient server = new(PortHost);

        private static readonly TimeZoneInfo TimeZone = TimeZoneInfo.FindSystemTimeZoneById("GTB Standard Time");

        private static DateTime LastSave = new();

        public Server()
        {
            ipHost = GetLocalIPv4(NetworkInterfaceType.Wireless80211);
            //Console.WriteLine(ipHost);
            //ClientManager.SaveClientTestData();
            //ClientManager.RecoverClientData();
            Task.Run(() => RecieveMessages());
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
                ClientManager.ClientsUpdated = true;
            }
        }

        private void RecieveMessages()
        {
            IPEndPoint? recivedClientIP;
            while (true)
            {
                recivedClientIP = null;
                byte[] data = server.Receive(ref recivedClientIP);
                string message = Encoding.UTF8.GetString(data);

                ArduinoClient arduinoClient = JsonDataParser.ParseClient(message) ?? new();
                arduinoClient.IP = recivedClientIP;
                arduinoClient.State = ArduinoClient.ConnectionState.Online;
                arduinoClient.LastConnection = GetDTNow();

                if (ArduinoClient.IsNullOrEmpty(arduinoClient))
                    continue;

                if (arduinoClient.Id == Guid.Empty)
                {
                    arduinoClient.Id = Guid.NewGuid();
                    ClientManager.AddNewClient(arduinoClient);
                }
                else
                {
                    ClientManager.UpdateClient(arduinoClient);
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
