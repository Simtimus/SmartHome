using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SmartHome.Arduino.Application;
using SmartHome.Arduino.Application.Logging;
using SmartHome.Arduino.Models.Arduino;
using SmartHome.Arduino.Models.Data.Received;
using SmartHome.Arduino.Models.Json.Converting;
using SmartHome.Arduino.Models.Logs;

namespace SmartHome.Arduino.Models.Data.Processing
{
    public class DeviceDataProcessor
    {
        public static void HandleReceivedData(ReceivedData receivedData)
        {
            bool processState = true;
            try
            {
                JsonDataConverting.ConvertReceivedData(receivedData);
            }
            catch(Exception ex)
            {
                LoggingService.WarningLog(new MessageLog()
                {
                    Message = ex.Message,
                });
                processState = false;
            }

            switch (receivedData.Mode)
            {
                case ReceivedData.RecievingMode.EntireBoard:
                    ProcessFullDevice(receivedData);
                    break;
                case ReceivedData.RecievingMode.SingleComponent:
                    processState = ProcessComponent(receivedData);
                    break;
                case ReceivedData.RecievingMode.SingleBoardPin:
                    processState = ProcessDevicePin(receivedData);
                    break;
            }

            if (!processState)
            {
                LoggingService.WarningLog(new ClassLog<ReceivedData>()
                {
                    Message = "\"HandleReceivedData\" failed to process \"receivedData\"",
                    Class = receivedData,
                    CodeSpace = "SmartHome.Arduino.Models.Data.Processing.DeviceDataProcessor"
                }); ; ;
            }
        }

        private static void ProcessFullDevice(ReceivedData receivedData)
        {
            JObject jsonObject = JObject.Parse(receivedData.Data);
            ArduinoClient? arduinoClient = JsonDataConverting.ConvertClientOnly(jsonObject, false);

            if (ArduinoClient.IsNullOrEmpty(arduinoClient)) { return; }

            JsonDataConverting.ConvertClientComponents(arduinoClient, jsonObject, false);

            arduinoClient.IP = receivedData.IP;
            arduinoClient.State = ArduinoClient.ConnectionState.Online;
            arduinoClient.LastConnection = receivedData.LastConnection;


            if (arduinoClient.Id == Guid.Empty)
            {
                arduinoClient.Id = Guid.NewGuid();
                ClientManager.AddNewClient(arduinoClient);
            }
            else
            {
                if (ClientManager.GetClientIndexById(arduinoClient.Id, out int clientIndex))
                {
                    JsonDataConverting.UpdateClientFromJson(ClientManager.Clients[clientIndex], receivedData.Data);
                }
                else
                {
                    ClientManager.AddNewClient(arduinoClient); ;
                }
            }
        }

        private static bool ProcessComponent(ReceivedData receivedData)
        {
            if (!ClientManager.GetClientIndexById(receivedData.BoardId, out int clientIndex)) return false;
            if (!ClientManager.GetComponentIndexById(clientIndex, receivedData.ComponentId, out int componentIndex)) return false;

            JObject jsonObject = JObject.Parse(receivedData.Data);

            JsonDataConverting.UpdateComponentFromJson(
                ClientManager.Clients[clientIndex].
                Components[componentIndex], jsonObject);
            return true;
        }

        private static bool ProcessDevicePin(ReceivedData receivedData)
        {
            if (!ClientManager.GetClientIndexById(receivedData.BoardId, out int clientIndex)) return false;
            if (!ClientManager.GetComponentIndexById(clientIndex, receivedData.ComponentId, out int componentIndex)) return false;
            if (!ClientManager.GetBoardPinIndexById(clientIndex, componentIndex, receivedData.PinId, out int pinIndex)) return false;

            JObject jsonObject = JObject.Parse(receivedData.Data);
            JsonDataConverting.UpdateModelFromJson(
                ClientManager.Clients[clientIndex].
                Components[componentIndex].
                ConnectedPins[pinIndex], jsonObject);

            return true;
        }
    }
}
