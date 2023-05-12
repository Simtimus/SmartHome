using SmartHome.Arduino.Application.Logging;
using SmartHome.Arduino.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Arduino.Models.Logs
{
    public class ClassLog<TClass> : ILog
    {
        public LoggingService.LogTypes LogType { get; set; } = LoggingService.LogTypes.ClassLog;
        public DateTime Time { get; set; } = DateTime.Now;
        public string Message { get; set; } = string.Empty;
        public LoggingService.LogStates LogState { get; set; }
        public TClass Class { get; set; }
        public string CodeSpace { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"{Enum.GetName(typeof(LoggingService.LogStates), LogState)} : [{CodeSpace}]\n" +
                $"\t{Message}\n" +
                $"\tWhen [{nameof(TClass)}] is \"{Class?.ToString()}\"";
        }
    }
}
