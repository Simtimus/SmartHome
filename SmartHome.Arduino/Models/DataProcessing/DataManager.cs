using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SmartHome.Arduino.Application;
using SmartHome.Arduino.Models;

namespace SmartHome.Arduino.Models.DataProcessing
{
    public class DataManager
    {
        public static void GetDataFromReceiver(ReceivedData receivedData)
        {
            receivedData = JsonProcessing.JsonDataParser.ParseReceivedData(receivedData);
            bool processState = true;

            switch (receivedData.Mode)
            {
                case ReceivedData.RecievingMode.EntireBoard:
                    ProcessEntireBoard(receivedData);
                    break;
                case ReceivedData.RecievingMode.SingleComponent:
                    processState = ProcessSingleComponent(receivedData);
                    break;
                case ReceivedData.RecievingMode.SingleBoardPin:
                    processState = ProcessSingleBoardPin(receivedData);
                    break;
            }

            if (!processState)
            {
                //TODO: Do some log
            }
        }

        private static void ProcessEntireBoard(ReceivedData receivedData)
        {
            ArduinoClient? arduinoClient = JsonProcessing.JsonDataParser.ParseClientOnly(receivedData.Data, false);

            if (ArduinoClient.IsNullOrEmpty(arduinoClient)) { return; }

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
                    JsonProcessing.JsonDataParser.UpdateArduinoClient(ClientManager.Clients[clientIndex], receivedData.Data);
                }
                else
                {
                    ClientManager.AddNewClient(arduinoClient); ;
                }
            }
        }

        private static bool ProcessSingleComponent(ReceivedData receivedData)
        {
            if (!ClientManager.GetClientIndexById(receivedData.BoardId, out int clientIndex)) return false;
            if (!ClientManager.GetComponentIndexById(clientIndex, receivedData.ComponentId, out int componentIndex)) return false;

            JObject jsonObject = JObject.Parse(receivedData.Data);

            JsonProcessing.JsonDataParser.UpdateGeneralComponent(
                ClientManager.Clients[clientIndex].
                Components[componentIndex], jsonObject);
            return true;
        }

        private static bool ProcessSingleBoardPin(ReceivedData receivedData)
        {
            if (!ClientManager.GetClientIndexById(receivedData.BoardId, out int clientIndex)) return false;
            if (!ClientManager.GetComponentIndexById(clientIndex, receivedData.ComponentId, out int componentIndex)) return false;
            if (!ClientManager.GetBoardPinIndexById(clientIndex, componentIndex, receivedData.PinId, out int pinIndex)) return false;

            JObject jsonObject = JObject.Parse(receivedData.Data);
            JsonProcessing.JsonDataParser.UpdateModelByJsonObject(
                ClientManager.Clients[clientIndex].
                Components[componentIndex].
                ConnectedPins[pinIndex], jsonObject);

            return true;
        }
    }
}
