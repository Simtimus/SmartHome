using Newtonsoft.Json.Linq;
using SmartHome.Arduino.Application.Logging;
using SmartHome.Arduino.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Arduino.Models.Logs
{
    public class MessageLog : ILog
    {
        public LoggingService.LogTypes LogType { get; set; } = LoggingService.LogTypes.MessageLog;
        public DateTime Time { get; set; } = DateTime.Now;
        public string Message { get; set; } = string.Empty;
        public LoggingService.LogStates LogState { get; set; }

        public override string ToString()
        {
            return $"{Enum.GetName(typeof(LoggingService.LogStates), LogState)}\n\t{Message}";
        }
    }
}
