using SmartHome.Arduino.Models.Interfaces;
using SmartHome.Arduino.Models.Json.FileStorage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Arduino.Application.Logging
{
    public class LoggingService
    {
        private const string LogsFileName = "ProgramLogs.json";
        public List<ILog> LogEntries { get; } = new();

        public List<ILog>? GetAllLogs()
        {
            string? recoveredData = FileDataStorage.ReadStringFromFile(LogsFileName);
            if (string.IsNullOrEmpty(recoveredData))
                return null;
            return new();
        }

        public void InfoLog()
        {

        }

        public void ErrorLog()
        {

        }

        public void WarningLog()
        {

        }

        private void SaveLogsToFile()
        {

        }

        public enum LogStates
        {
            Info,
            Error,
            Warning,
        }
    }
}
