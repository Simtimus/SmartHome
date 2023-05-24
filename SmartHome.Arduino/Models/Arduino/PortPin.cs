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
		public string FormatString { get; set; } = "{0}";
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
			string stringData = DataLink.GetValue();
			
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
				return ApplyDataLinkOperation(int.Parse(stringData));
			}
			else if (ValueType == ObjectValueType.Float)
			{
				return ApplyDataLinkOperation(float.Parse(stringData, CultureInfo.InvariantCulture));
			}
			else if (ValueType == ObjectValueType.Boolean)
			{
				if (bool.TryParse(stringData, out bool boolValue))
				{ 
					return ApplyDataLinkOperation(boolValue);
				}
				else
				{
					if (stringData == "1")
					{
						return ApplyDataLinkOperation(true);
					}
					else
					{
						return ApplyDataLinkOperation(false);
					}
				}
			}
			return null;
		}

		public object ApplyDataLinkOperation(object value)
		{
			string dataLinkValue = DataLink.Value;
			object dataValue;

			if (DataLink.ValueOperation == DataLink.ValueOperations.None)
				return value;

			if (!string.IsNullOrEmpty(dataLinkValue) || ValueType == ObjectValueType.Boolean)
			{
				if (ValueType == ObjectValueType.Integer)
				{
					dataValue = int.Parse(dataLinkValue);
					if (DataLink.ValueOperation == DataLink.ValueOperations.Addition)
					{
						return (int)value + (int)dataValue;
					}
					else if (DataLink.ValueOperation == DataLink.ValueOperations.Substraction)
					{
						return (int)value - (int)dataValue;
					}
					else if (DataLink.ValueOperation == DataLink.ValueOperations.Multiplication)
					{
						return (int)value * (int)dataValue;
					}
					else if (DataLink.ValueOperation == DataLink.ValueOperations.Division)
					{
						return (int)value / (int)dataValue;
					}
				}
				else if (ValueType == ObjectValueType.Float)
				{
					dataValue = float.Parse(dataLinkValue, CultureInfo.InvariantCulture);
					if (DataLink.ValueOperation == DataLink.ValueOperations.Addition)
					{
						return (float)value + (float)dataValue;
					}
					else if (DataLink.ValueOperation == DataLink.ValueOperations.Substraction)
					{
						return (float)value - (float)dataValue;
					}
					else if (DataLink.ValueOperation == DataLink.ValueOperations.Multiplication)
					{
						return (float)value * (float)dataValue;
					}
					else if (DataLink.ValueOperation == DataLink.ValueOperations.Division)
					{
						return (float)value / (float)dataValue;
					}
				}
				else if (ValueType == ObjectValueType.Boolean)
				{
					if (DataLink.ValueOperation == DataLink.ValueOperations.Negation)
					{
						return !(bool)value;
					}
				}
			}
			return value;
		}

		public string? GetValueString()
		{
			object? value = GetValue();
			return value is null ? string.Empty : value.ToString();
		}

		public string? GetFormatedStringValue()
		{
			return string.Format(FormatString, GetValue());
		}
	}
}
