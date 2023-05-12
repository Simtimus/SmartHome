using Newtonsoft.Json;
using SmartHome.Arduino.Application.Exceptions;
using SmartHome.Arduino.Models.Components.Common.Interfaces;
using SmartHome.Arduino.Models.Interfaces;
using SmartHome.Arduino.Models.Json.Converting;
using SmartHome.Arduino.Models.Json.FileStorage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SmartHome.Arduino.Models.Components.Common.GeneralComponent;

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

            return JsonDataConverting.ConvertILogs(recoveredData);
        }

        public static LogTypes GetTypeByString(string logType)
        {
            return (LogTypes)Enum.Parse(typeof(LogTypes), logType);
        }
        public static ILog? CreateById(LogTypes logType)
        {
            Type? componentType = GetTypeById(logType);
            if (componentType is null)
                throw new ComponentNotFoundException(logType);
            return Activator.CreateInstance(componentType) as ILog;
        }

        public static Type? GetTypeById(LogTypes logType)
        {
            string componentName = Enum.GetName(typeof(LogTypes), logType) ?? string.Empty;
            if (string.IsNullOrEmpty(componentName))
                throw new ComponentNotFoundException(logType);
            return Type.GetType($"SmartHome.Arduino.Models.Logs.{logType}");
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

        public enum LogTypes
        {
            MessageLog,
            ParametreLog,
            ClassLog
        }

        public enum LogStates
        {
            Info,
            Error,
            Warning,
        }
    }
}
