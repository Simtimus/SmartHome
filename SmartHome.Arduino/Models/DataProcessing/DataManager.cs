using System;
using System.Collections.Generic;
using System.Linq;
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

            switch (receivedData.Mode)
            {
                case ReceivedData.RecievingMode.EntireBoard:
                    ProcessEntireBoard(receivedData);
                    break;
                case ReceivedData.RecievingMode.SingleComponent:
                    ProcessSingleComponent(receivedData);
                    break;
                case ReceivedData.RecievingMode.SingleBoardPin:
                    ProcessSingleBoardPin(receivedData);
                    break;
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
                int clientIndex = ClientManager.GetClientIndexById(arduinoClient.Id);
                if (clientIndex == -1)
                {
                    ClientManager.AddNewClient(arduinoClient);
                }
                else
                {
                    JsonProcessing.JsonDataParser.UpdateArduinoClient(ClientManager.Clients[clientIndex], receivedData.Data);
                }
            }
        }

        private static void ProcessSingleComponent(ReceivedData receivedData)
        {
            int clientIndex = ClientManager.GetClientIndexById(receivedData.BoardId);
            int componentIndex = ClientManager.GetComponentIndexById(clientIndex, receivedData.ComponentId);

            JObject jsonObject = JObject.Parse(receivedData.Data);

            JsonProcessing.JsonDataParser.UpdateGeneralComponent(
                ClientManager.Clients[clientIndex].
                Components[componentIndex], jsonObject);
        }

        private static void ProcessSingleBoardPin(ReceivedData receivedData)
        {
            int clientIndex = ClientManager.GetClientIndexById(receivedData.BoardId);
            int componentIndex = ClientManager.GetComponentIndexById(clientIndex, receivedData.ComponentId);
            int boardPinIndex = ClientManager.GetBoardPinIndexById(clientIndex, componentIndex, receivedData.ComponentId);

            JObject jsonObject = JObject.Parse(receivedData.Data);

            JsonProcessing.JsonDataParser.UpdateModelByJsonObject(
                ClientManager.Clients[clientIndex].
                Components[componentIndex].
                ConnectedPins[boardPinIndex], jsonObject);
        }
    }
}
