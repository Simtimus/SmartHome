using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Arduino.Application;
using SmartHome.Arduino.Models.Arduino;
using SmartHome.Arduino.Models.Components.Common.Interfaces;
using SmartHome.Arduino.Models.Interfaces;
using SmartHome.Arduino.Models.Json.Converting;
using SmartHome.Arduino.Models.Json.FileStorage;

namespace SmartHome.Arduino.Models.Data.DataLinks
{
	public static class DataLinker
	{
		public static List<DataLink> DataLinks = new();
		private static readonly object _lock = new object();

		public static void LinkData(ref PortPin from, ref PortPin to)
		{
			to.DataLink = new(from)
			{
				Id = Guid.NewGuid()
			};
			lock (_lock)
			{
				DataLinks.Add(to.DataLink);
			}
		}

		public static bool RemoveLink(Guid Id)
		{
			lock (_lock)
			{
				DataLink? dataLink = DataLinks.Find(x => x.Id == Id);
				if (dataLink == null)
				{
					dataLink = new();
					DataLinks.Remove(dataLink);
					return true;
				}
			}
			return false;
		}

		public static void RecoverDataLinks()
		{
			foreach (ArduinoClient client in ClientManager.Clients)
			{
				if (client.Components.Count > 0)
				{
					foreach (IGeneralComponent component in client.Components)
					{
						foreach (PortPin portPin in component.ConnectedPins)
						{
							if (!DataLink.IsNullOrEmpty(portPin.DataLink))
							{
								DataLinks.Add(portPin.DataLink);
							}
						}
					}
				}
			}
		}
	}
}
