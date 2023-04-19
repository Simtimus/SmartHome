using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SmartHome.Arduino.Models
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

		public string? GetValueString()
		{
			return Value is null ? string.Empty : Value.ToString();
		}

		public override string ToString() => $"Pin {Id} [{Enum.GetName(typeof(PinMode), Mode)}]";
    }
}
