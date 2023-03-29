using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Arduino.Models
{
	public class BoardPin
	{
		public int Id { get; set; }
		public PinMode Mode { get; set; }
		public object? Value { get; set; }
		public ObjectValueType ValueType { get; set; }

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

		public string? GetValueString()
		{
			return Value is null ? String.Empty : Value.ToString();
		}

		public override string ToString() => $"Pin {Id.ToString()} [{Enum.GetName(typeof(PinMode), Mode)}]";

	}
}
