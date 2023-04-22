using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SmartHome.Arduino.Models.Data.DataLinks;
using SmartHome.Arduino.Models.Components.Common.Interfaces;

namespace SmartHome.Arduino.Models.Arduino
{
    public class BoardPin
    {
        public int Id { get; set; }
        public PinMode Mode { get; set; }
        public object? Value { get; set; }
        public ObjectValueType ValueType { get; set; }
        public DataLink DataLink { get; set; } = new DataLink();
        [JsonIgnore] public IGeneralComponent? ParentComponent { get; set; }

        public enum PinMode
        {
            Read,
            Write,
            Virtual,    // Conventional -> Id >= 100
        }

        public enum ObjectValueType
        {
            String,
            Integer,
            Float,
            Boolean,
        }

        public object? GetValue()
        {
            if (!DataLink.IsNullOrEmpty(DataLink))
            {
                return DataLink.GetValue();
            }
            return Value;
        }

        public string? GetValueString()
        {
            object? value = GetValue();
            return value is null ? string.Empty : value.ToString();
        }

        public override string ToString() => $"Pin {Id} [{Enum.GetName(typeof(PinMode), Mode)}]";
    }
}
