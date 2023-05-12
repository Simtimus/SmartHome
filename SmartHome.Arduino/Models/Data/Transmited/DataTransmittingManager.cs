using Newtonsoft.Json;
using SmartHome.Arduino.Application;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Arduino.Models.Data.Transmited
{
    public class DataTransmittingManager
    {
        private UdpClient Server { get; set; }
        public DataTransmittingManager(UdpClient server)
        {
            Server = server;
        }

        public void TransmitEmpty(string boardId)
        {
            TransmitedData transmitedData = new()
            {
                BoardId = Guid.Parse(boardId),
                Commands = new()
                {
                    new TransmitedCommand()
                    {
                        Value = String.Empty,
                        Action = TransmitedCommand.CommandAction.Empty,
                    }
                }
            };
            string transmitedPacket = JsonConvert.SerializeObject(transmitedData);
            SendMessageToClientById(transmitedData.BoardId, transmitedPacket);
        }

        public void TransmitNewID(string newId)
        {
            TransmitedData transmitedData = new()
            {
                BoardId = Guid.Parse(newId),
                Commands = new()
                {
                    new TransmitedCommand()
                    {
                        Value = newId,
                        Action = TransmitedCommand.CommandAction.SetId,
                    }
                }
            };
            string transmitedPacket = JsonConvert.SerializeObject(transmitedData);
            SendMessageToClientById(transmitedData.BoardId, transmitedPacket);
        }

        public int SendMessageToClientById(Guid id, string message)
        {
            ClientManager.GetClientIndexById(id, out int index);
            if (index == -1) return 0;

            byte[] byteMsg = Encoding.UTF8.GetBytes(message);
            return Server.Send(byteMsg, byteMsg.Length, ClientManager.Clients[index].IP);
        }
    }
}
