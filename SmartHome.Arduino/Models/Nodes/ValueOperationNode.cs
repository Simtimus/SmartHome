using SmartHome.Arduino.Models.CommonTypes;
using SmartHome.Arduino.Models.Nodes.Common;
using SmartHome.Arduino.Models.Nodes.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using static SmartHome.Arduino.Models.Nodes.Common.GeneralNode;

namespace SmartHome.Arduino.Models.Nodes
{
    public class ValueOperationNode : INode
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public FlexibleValue FlexiValue { get; set; } = new();
        public NodeTypes Type { get; set; } = NodeTypes.ValueOperation;

		public ValueOperations Operation { get; set; }
		public DataReference? In1 { get; set; }
		public DataReference? In2 { get; set; }

		public enum ValueOperations
		{
			Negation,
			Addition,
			Substraction,
			Multiplication,
			Division
		}

		public object? GetValue()
		{
			dynamic? First;
			dynamic? Second;

			if (DataReference.IsNullOrEmpty(In1))
				First = FlexiValue.Value;
			else
				First = In1.GetValue();

			if (DataReference.IsNullOrEmpty(In2))
				Second = FlexiValue.Value;
			else
				Second = In2.GetValue();

			return Operation switch
			{
				ValueOperations.Negation => Negate(First),
				ValueOperations.Addition => Add(First, Second),
				ValueOperations.Substraction => Subtract(First, Second),
				ValueOperations.Multiplication => Multiply(First, Second),
				ValueOperations.Division => Divide(First, Second),
				_ => FlexiValue.Value,
			};
		}

        public static dynamic Negate(dynamic value)
		{
			if (value is bool)
				return !value;

			if (value is int || value is double)
				return -value;

			return value;
		}

		public static dynamic Add(dynamic firstOperand, dynamic secondOperand)
		{
			if (firstOperand is not int && firstOperand is not double)
				return firstOperand;

			if (secondOperand is not int && secondOperand is not double)
				return firstOperand;

			return firstOperand + secondOperand;
		}

		public static dynamic Subtract(dynamic firstOperand, dynamic secondOperand)
		{
			if (firstOperand is not int && firstOperand is not double)
				return firstOperand;

			if (secondOperand is not int && secondOperand is not double)
				return firstOperand;

			return firstOperand - secondOperand;
		}

		public static dynamic Multiply(dynamic firstOperand, dynamic secondOperand)
		{
			if (firstOperand is not int && firstOperand is not double)
				return firstOperand;

			if (secondOperand is not int && secondOperand is not double)
				return firstOperand;

			return firstOperand * secondOperand;
		}

		public static dynamic Divide(dynamic firstOperand, dynamic secondOperand)
		{
			if (firstOperand is not int && firstOperand is not double)
				return firstOperand;

			if (secondOperand is not int && secondOperand is not double)
				return firstOperand;

			return firstOperand / secondOperand;
		}
	}
}
