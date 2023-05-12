using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Arduino.Models.Data.Transmited
{
    public class TransmitedCommand
    {
        public string Value { get; set; } = string.Empty;
        public CommandAction Action { get; set; }
        public int ComponentId { get; set; } = -1;
        public int PinId { get; set; } = -1;
        public enum CommandAction
        {
            Empty,
            SetId,
            SetValue,
        }
    }
}
