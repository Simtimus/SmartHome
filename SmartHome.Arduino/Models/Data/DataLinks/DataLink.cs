﻿using Newtonsoft.Json.Linq;
using SmartHome.Arduino.Application;
using SmartHome.Arduino.Models.Arduino;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Arduino.Models.Data.DataLinks
{
    public class DataLink
    {
        public Guid Id { get; set; }
        public Guid BoardId { get; set; }
        public int ComponentId { get; set; } = -1;
        public int PinId { get; set; } = -1;
        public string Value { get; set; } = string.Empty;

        public DataLink() { }

        public DataLink(PortPin reference)
        {
            BoardId = reference.ParentComponent.ParentClient.Id;
            ComponentId = reference.ParentComponent.Id;
            PinId = reference.Id;
        }

        public string GetValue()
        {
            if (!ClientManager.GetClientIndexById(BoardId, out int clientIndex)) return string.Empty;
            if (!ClientManager.GetComponentIndexById(clientIndex, ComponentId, out int componentIndex)) return string.Empty;
            if (!ClientManager.GetBoardPinIndexById(clientIndex, componentIndex, PinId, out int pinIndex)) return string.Empty;

            return ClientManager.Clients[clientIndex].Components[componentIndex].ConnectedPins[pinIndex].Value;
        }

        public override string ToString() => $"{BoardId}.{ComponentId}.{PinId}";

        public static bool IsNullOrEmpty([NotNullWhen(false)] DataLink? dataLink)
        {
            if (dataLink is null)
                return true;
            if (dataLink.Equals(default))
                return true;
            return false;
        }
    }
}
