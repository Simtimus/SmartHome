using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Arduino.Models.CommonTypes
{
    public class FlexibleValue
    {
        public object Value { get; private set; }
        public ObjectValueType Type { get; set; }


		public FlexibleValue()
        {
            Type = ObjectValueType.String;
            Value = string.Empty;
        }

        public FlexibleValue(ObjectValueType valueType, object value = null)
        {
            Type = valueType;
            Value = value ?? GetDefault(valueType);
        }

        public bool Set(string? inputValue)
        {
            if (string.IsNullOrEmpty(inputValue)) return false;
            if (TryParse(inputValue, Type, out object value))
            {
                Value = value;
                return true;
            }
            return false;
        }

        public bool Set(object? inputValue)
        {
            if (inputValue is not null)
            {
                Value = inputValue;
                return true;
            }
            return false;
		}

		public void SetOrDefault(string? inputValue)
		{
            if (string.IsNullOrEmpty(inputValue))
            {
                Value = GetDefault(Type);
            }
            else
            {
			    if (TryParse(inputValue, Type, out object value))
			    {
				    Value = value;
			    }
                else
                {
					Value = GetDefault(Type);
				}
            }
		}

		public void SetOrDefault(object? inputValue)
		{
			if (inputValue is not null)
			{
				Value = inputValue;
			}
            else {
			    Value = GetDefault(Type);
            }
		}

        public void SetDefault()
        {
			Value = GetDefault(Type);
		}

		public static object GetDefault(ObjectValueType valueType)
        {
            if (valueType == ObjectValueType.Integer)
            {
                return 0;
            }
            else if (valueType == ObjectValueType.Float)
            {
                return double.Parse("0", CultureInfo.InvariantCulture);
            }
            else if (valueType == ObjectValueType.Boolean)
            {
                return false;
            }
            else
            {
                return string.Empty;
            }
		}

		public static object GetDefault(FlexibleValue flexiValue)
		{
			if (flexiValue.Type == ObjectValueType.Integer)
			{
				return 0;
			}
			else if (flexiValue.Type == ObjectValueType.Float)
			{
				return double.Parse("0", CultureInfo.InvariantCulture);
			}
			else if (flexiValue.Type == ObjectValueType.Boolean)
			{
				return false;
			}
			else
			{
				return string.Empty;
			}
		}

		public static bool TryParse(string? inputValue, ObjectValueType valueType, out object result)
        {
            bool Parsed;

            if (string.IsNullOrEmpty(inputValue))
            {
                result = string.Empty;
                return false;
            }

            if (valueType == ObjectValueType.String)
            {
                result = inputValue;
                return true;
            }
            else if (valueType == ObjectValueType.Integer)
            {
                Parsed = int.TryParse(inputValue, out int intValue);
                result = intValue;
                return Parsed;
            }
            else if (valueType == ObjectValueType.Float)
            {
                Parsed = double.TryParse(inputValue, NumberStyles.Float, CultureInfo.InstalledUICulture, out double doubleValue);
                result = doubleValue;
                return Parsed;
            }
            else if (valueType == ObjectValueType.Boolean)
            {
                if (bool.TryParse(inputValue, out bool boolValue))
                {
                    result = boolValue;
                    return true;
                }
                else
                {
                    if (inputValue == "1")
                    {
                        result = true;
                        return true;
                    }
                    else if (inputValue == "0")
                    {
                        result = false;
                        return true;
                    }
                    else
                    {
                        result = boolValue;
                        return false;
                    }
                }
            }
            result = GetDefault(valueType);
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, Value);
        }

        public override bool Equals(object obj)
        {
            if (obj is FlexibleValue flexiValue)
            {
                return Type == flexiValue.Type && Equals(Value, flexiValue.Value);
            }
            return false;
        }
    }
}
