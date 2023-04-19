using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Arduino.Application.Events
{
    public class ClientEvents
    {
        public delegate void ClientChangedHandler();
        public static event ClientChangedHandler? OnClientChanged;

        public static void TriggerClientChanged()
        {
            OnClientChanged?.Invoke();
        }
    }
}
