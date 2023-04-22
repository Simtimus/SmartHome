using SmartHome.Arduino.Application.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Arduino.Models.Interfaces
{
    public interface ILog
    {
        public DateTime Time { get; set; }
        public string Message { get; set; }
        public LoggingService.LogStates LogState { get; set; }
    }
}
