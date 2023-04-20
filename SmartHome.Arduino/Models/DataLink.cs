using Newtonsoft.Json.Linq;
using SmartHome.Arduino.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SmartHome.Arduino.Models.BoardPin;

namespace SmartHome.Arduino.Models
{
    public class DataLink
    {
        public Guid Id { get; set; }
        public Guid BoardId { get; set; }
        public int ComponentId { get; set; }
        public int PinId { get; set; }
        public object? Value { get; set; }

        public DataLink() { }

        public DataLink(BoardPin reference)
        {
            BoardId = reference.ParentComponent.ParentClient.Id;
            ComponentId = reference.ParentComponent.Id;
            PinId = reference.Id;
        }

        public object? GetValue()
        {
            if (!ClientManager.GetClientIndexById(BoardId, out int clientIndex)) return null;
            if (!ClientManager.GetComponentIndexById(clientIndex, ComponentId, out int componentIndex)) return null;
            if (!ClientManager.GetBoardPinIndexById(clientIndex, componentIndex, PinId, out int pinIndex)) return null;

            return ClientManager.Clients[clientIndex].Components[componentIndex].ConnectedPins[pinIndex].Value;
        }

        public override string ToString() => $"{BoardId}.{ComponentId}.{PinId}";

        public static bool IsNullOrEmpty(DataLink? dataLink)
        {
            if (dataLink is null)
                return true;
            if (dataLink.Equals(new DataLink()))
                return true;
            return false;
        }
    }
}
