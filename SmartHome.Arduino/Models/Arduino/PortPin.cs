using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SmartHome.Arduino.Models.Components.Common.Interfaces;
using SmartHome.Arduino.Models.CommonTypes;
using System.Globalization;
using SmartHome.Arduino.Models.Nodes.Common;

namespace SmartHome.Arduino.Models.Arduino
{
	public class PortPin
	{
		public int Id { get; set; }
		public PinType Type { get; set; }
		public PinMode Mode { get; set; }
		public FlexibleValue FlexiValue { get; set; } = new();
		public DataReference? DataReference { get; set; } = new();
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

		public object? GetValue()
		{
			if (DataReference is not null && DataReference.DataId != default)
			{
				object? result = DataReference.GetValue();
				if (result is null)
				{
					return FlexibleValue.GetDefault(FlexiValue);
				}
				return result;
			}
			return FlexiValue.Value;
		}

		//public object ApplyDataLinkOperation(object value)
		//{
		//	string dataLinkValue = DataReference.Value;
		//	object dataValue;

		//	if (DataReference.ValueOperation == DataReference.ValueOperations.None)
		//		return value;

		//	if (!string.IsNullOrEmpty(dataLinkValue) || ValueType == ObjectValueType.Boolean)
		//	{
		//		if (ValueType == ObjectValueType.Integer)
		//		{
		//			dataValue = int.Parse(dataLinkValue);
		//			if (DataReference.ValueOperation == DataReference.ValueOperations.Addition)
		//			{
		//				return (int)value + (int)dataValue;
		//			}
		//			else if (DataReference.ValueOperation == DataReference.ValueOperations.Substraction)
		//			{
		//				return (int)value - (int)dataValue;
		//			}
		//			else if (DataReference.ValueOperation == DataReference.ValueOperations.Multiplication)
		//			{
		//				return (int)value * (int)dataValue;
		//			}
		//			else if (DataReference.ValueOperation == DataReference.ValueOperations.Division)
		//			{
		//				return (int)value / (int)dataValue;
		//			}
		//		}
		//		else if (ValueType == ObjectValueType.Float)
		//		{
		//			dataValue = double.Parse(dataLinkValue, CultureInfo.InvariantCulture);
		//			if (DataReference.ValueOperation == DataReference.ValueOperations.Addition)
		//			{
		//				return (double)value + (double)dataValue;
		//			}
		//			else if (DataReference.ValueOperation == DataReference.ValueOperations.Substraction)
		//			{
		//				return (double)value - (double)dataValue;
		//			}
		//			else if (DataReference.ValueOperation == DataReference.ValueOperations.Multiplication)
		//			{
		//				return (double)value * (double)dataValue;
		//			}
		//			else if (DataReference.ValueOperation == DataReference.ValueOperations.Division)
		//			{
		//				return (double)value / (double)dataValue;
		//			}
		//		}
		//		else if (ValueType == ObjectValueType.Boolean)
		//		{
		//			if (DataReference.ValueOperation == DataReference.ValueOperations.Negation)
		//			{
		//				return !(bool)value;
		//			}
		//		}
		//	}
		//	return value;
		//}

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
