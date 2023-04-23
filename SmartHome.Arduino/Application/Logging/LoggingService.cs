using Newtonsoft.Json;
using SmartHome.Arduino.Models.Interfaces;
using SmartHome.Arduino.Models.Json.Converting;
using SmartHome.Arduino.Models.Json.FileStorage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Arduino.Application.Logging
{
    public static class LoggingService
    {
        private const string LogsFileName = "ProgramLogs.json";
        public static List<ILog> LogEntries { get; } = new();

        public static List<ILog>? GetAllLogs()
        {
            string? recoveredData = FileDataStorage.ReadStringFromFile(LogsFileName);
            if (string.IsNullOrEmpty(recoveredData))
                return null;

            return JsonConvert.DeserializeObject<List<ILog>>(recoveredData);
        }

        public static void InfoLog(ILog log)
        {
            LogEntries.Add(log);
            log.LogState = LogStates.Info;
            Task.Run(async () => { await SaveLogsToFile(); });
        }

        public static void ErrorLog(ILog log)
        {
            LogEntries.Add(log);
            log.LogState = LogStates.Error;
            Task.Run(async () => { await SaveLogsToFile(); });
        }

        public static void WarningLog(ILog log)
        {
            LogEntries.Add(log);
            log.LogState = LogStates.Warning;
            Task.Run(async () => { await SaveLogsToFile(); });
        }

        private static async Task SaveLogsToFile()
        {
            FileDataStorage.SaveDataToJsonFile(LogEntries, LogsFileName);
        }

        public enum LogStates
        {
            Info,
            Error,
            Warning,
        }
    }
}
