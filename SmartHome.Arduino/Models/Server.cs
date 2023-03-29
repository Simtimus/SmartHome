using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;

namespace SmartHome.Arduino.Models
{
    public class Server
    {
        public string IpHost { get { return ipHost; } }
        private readonly string ipHost = "0.0.0.0";

        private const int PortHost = 8080;

        private const int secondsUntilOffline = 10;
        private const int secondsUntilDelete = 60;

        private readonly UdpClient server = new UdpClient(PortHost);
        public readonly ClientManager ClientManager = new ClientManager();

        public static TimeZoneInfo TimeZone = TimeZoneInfo.FindSystemTimeZoneById("GTB Standard Time");

        public Server()
        {
            ipHost = GetLocalIPv4(NetworkInterfaceType.Wireless80211);
            Console.WriteLine(ipHost);
            ClientManager.SaveClientTestData();
            ClientManager.RecoverClientData();

            Task.Run(() => RecieveMessages());
            Task.Run(() => MonitorClientsState());
        }

        private void MonitorClientsState()
        {
            DateTime offlineTime;
            foreach (var client in ClientManager.Clients)
            {
                offlineTime = client.LastConnection;
                offlineTime.AddSeconds(secondsUntilOffline);

                if (offlineTime.Subtract(GetDTNow()).TotalSeconds <= 0)
                {
                    client.State = ArduinoClient.ConnectionState.Offline;
                }
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

                ArduinoClient arduinoClient = ArduinoClient.ParseFromJson(recivedClientIP, message);
                arduinoClient.State = ArduinoClient.ConnectionState.Online;
                arduinoClient.LastConnection = GetDTNow();

                if (ArduinoClient.IsNullOrEmpty(arduinoClient))
                    continue;

                if (arduinoClient.Id == Guid.Empty)
                {
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

        private string GetLocalIPv4(NetworkInterfaceType _type)
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
