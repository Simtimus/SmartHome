using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SmartHome.Arduino.Models.Data.Received
{
    public class ReceivedData
    {
        public IPEndPoint? IP { get; set; }
        public DateTime LastConnection { get; set; }
        public string Data { get; set; } = string.Empty;
        public RecievingMode Mode { get; set; }
        public Guid BoardId { get; set; }
        public int ComponentId { get; set; } = -1;
        public int PinId { get; set; } = -1;

        public enum RecievingMode
        {
            EntireBoard,
            BoardInfo,
            SingleComponent,
            SingleBoardPin,
        }

        public override string ToString()
        {
            return $"{BoardId}.{ComponentId}.{PinId}";
        }
    }
}
