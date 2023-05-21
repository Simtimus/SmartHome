using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SmartHome.Arduino.Models.Data.DataLinks;
using SmartHome.Arduino.Models.Components.Common.Interfaces;
using System.Globalization;

namespace SmartHome.Arduino.Models.Arduino
{
    public class PortPin
    {
        public int Id { get; set; }
        public PinType Type { get; set; }
        public PinMode Mode { get; set; }
        public string Value { get; set; } = string.Empty;
        public ObjectValueType ValueType { get; set; }
        public DataLink DataLink { get; set; } = new DataLink();
        public bool Favorite { get; set; }
        [JsonIgnore] public IGeneralComponent? ParentComponent { get; set; }

        public enum PinType
        {
            Real,
            Virtual,    // Conventional -> Id >= 100
		}

        public enum PinMode
        {
            Read,
            Write,
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

			if (ValueType == ObjectValueType.String)
			{
				return stringData;
			}
			else if (ValueType == ObjectValueType.Integer)
			{
				return int.Parse(stringData);
			}
			else if (ValueType == ObjectValueType.Float)
			{
				return float.Parse(stringData, CultureInfo.InvariantCulture);
			}
			else if (ValueType == ObjectValueType.Boolean)
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
    }
}
