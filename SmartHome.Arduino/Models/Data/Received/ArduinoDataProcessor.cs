using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SmartHome.Arduino.Application;
using SmartHome.Arduino.Application.Events;
using SmartHome.Arduino.Application.Logging;
using SmartHome.Arduino.Models.Arduino;
using SmartHome.Arduino.Models.Components.Common.Interfaces;
using SmartHome.Arduino.Models.Data.DataBoxs;
using SmartHome.Arduino.Models.Data.Transmited;
using SmartHome.Arduino.Models.Json.Converting;
using SmartHome.Arduino.Models.Logs;

namespace SmartHome.Arduino.Models.Data.Received
{
	public class ArduinoDataProcessor
	{
		public DataTransmittingManager transmitManager { get; set; }
		public ArduinoDataProcessor(DataTransmittingManager transmittingManager)
		{
            transmitManager = transmittingManager;
		}

		public void ProcessArduinoData(ArduinoDataPacket receivedData)
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

			switch (receivedData.ContentType)
			{
				case ArduinoDataPacket.DataContentType.BoardInfo:
					ProcessBoardInfo(receivedData);
					break;
				case ArduinoDataPacket.DataContentType.EntireBoard:
					ProcessFullDevice(receivedData);
					break;
				case ArduinoDataPacket.DataContentType.SingleComponent:
					processState = ProcessComponent(receivedData);
					break;
				case ArduinoDataPacket.DataContentType.SinglePortPin:
					processState = ProcessDevicePin(receivedData);
					break;
			}

			if (!processState)
			{
				LoggingService.WarningLog(new ClassLog<ArduinoDataPacket>()
				{
					Message = "\"ProcessArduinoData\" failed to process \"receivedData\"",
					Class = receivedData,
					CodeSpace = "SmartHome.Arduino.Models.Data.Received.DeviceDataProcessor"
				});
			}
		}

		private void ProcessBoardInfo(ArduinoDataPacket receivedData)
		{
			JObject jsonObject = JObject.Parse(receivedData.Data);
			ArduinoClient? arduinoClient = JsonDataConverting.ConvertClientOnly(jsonObject, false);

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
					JsonDataConverting.UpdateClientFromJson(ClientManager.Clients[clientIndex], receivedData.Data);
                    ClientEvents.TriggerClientChanged();
                }
				else
				{
					ClientManager.AddNewClient(arduinoClient);
				}
            }
			transmitManager.TransmitEmpty(arduinoClient.Id.ToString());
        }

		private void ProcessFullDevice(ArduinoDataPacket receivedData)
		{
			JObject jsonObject = JObject.Parse(receivedData.Data);
			ArduinoClient? arduinoClient = JsonDataConverting.ConvertClientOnly(jsonObject, false);

			if (ArduinoClient.IsNullOrEmpty(arduinoClient)) { return; }

			JsonDataConverting.ConvertClientComponents(arduinoClient, jsonObject, false);

			arduinoClient.IP = receivedData.IP;
			arduinoClient.Id = Guid.Parse(receivedData.BoardId.ToString());
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
                    ClientManager.Clients[clientIndex].IP = receivedData.IP;
                    ClientManager.Clients[clientIndex].Id = Guid.Parse(receivedData.BoardId.ToString());
                    ClientManager.Clients[clientIndex].State = ArduinoClient.ConnectionState.Online;
                    ClientManager.Clients[clientIndex].LastConnection = receivedData.LastConnection;
                    ClientEvents.TriggerClientChanged();
				}
				else
				{
					ClientManager.AddNewClient(arduinoClient);
				}
            }
            transmitManager.TransmitEmpty(arduinoClient.Id.ToString());
        }

		private bool ProcessComponent(ArduinoDataPacket receivedData)
		{
			if (!ClientManager.GetClientIndexById(receivedData.BoardId, out int clientIndex)) return false;
			if (!ClientManager.GetComponentIndexById(clientIndex, receivedData.ComponentId, out int componentIndex)) return false;

			ArduinoClient client = ClientManager.GetClientById(receivedData.BoardId);
			IGeneralComponent component = ClientManager.GetComponentById(receivedData.BoardId, receivedData.ComponentId);

			JObject jsonObject = JObject.Parse(receivedData.Data);

			JsonDataConverting.UpdateComponentFromJson(component, jsonObject);

			client.IP = receivedData.IP;
			client.State = ArduinoClient.ConnectionState.Online;
			client.LastConnection = receivedData.LastConnection;

			ClientEvents.TriggerClientChanged();
            transmitManager.TransmitEmpty(ClientManager.Clients[clientIndex].Id.ToString());
            return true;
		}

		private bool ProcessDevicePin(ArduinoDataPacket receivedData)
		{
			if (!ClientManager.GetClientIndexById(receivedData.BoardId, out int clientIndex)) return false;
			if (!ClientManager.GetComponentIndexById(clientIndex, receivedData.ComponentId, out int componentIndex)) return false;
			if (!ClientManager.GetBoardPinIndexById(clientIndex, componentIndex, receivedData.PinId, out int pinIndex)) return false;

			ArduinoClient client = ClientManager.GetClientById(receivedData.BoardId);
			PortPin pin = ClientManager.GetPortPinById(receivedData.BoardId, receivedData.ComponentId, receivedData.PinId);

			JObject jsonObject = JObject.Parse(receivedData.Data);
			JsonDataConverting.UpdatePortPinFromJson(pin, jsonObject);

			client.IP = receivedData.IP;
			client.State = ArduinoClient.ConnectionState.Online;
			client.LastConnection = receivedData.LastConnection;

			ClientEvents.TriggerClientChanged();
            transmitManager.TransmitEmpty(client.Id.ToString());
            return true;
		}
	}
}
