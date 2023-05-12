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
    public class PortPin
    {
        public int Id { get; set; }
        public PinMode Mode { get; set; }
        public string Value { get; set; } = string.Empty;
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
            string stringData;

            stringData = DataLink.GetValue();

            if (string.IsNullOrEmpty(stringData))
            {
                stringData = Value;
            }

			if (ValueType == PortPin.ObjectValueType.String)
			{
				return stringData;
			}
			else if (ValueType == PortPin.ObjectValueType.Integer)
			{
				return int.Parse(stringData);
			}
			else if (ValueType == PortPin.ObjectValueType.Float)
			{
				return float.Parse(stringData);
			}
			else if (ValueType == PortPin.ObjectValueType.Boolean)
			{
                if (bool.TryParse(stringData, out bool boolValue))
                { 
                    return boolValue;
                }
                else
                {
					if (stringData == "1")
					{
						return true;
					}
					else
					{
						return false;
					}
				}
			}
            return null;
		}

        public string? GetValueString()
        {
            object? value = GetValue();
            return value is null ? string.Empty : value.ToString();
        }

        public override string ToString() => $"Pin {Id} [{Enum.GetName(typeof(PinMode), Mode)}]";
    }
}
