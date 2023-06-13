using SmartHome.Arduino.Models.CommonTypes;
using SmartHome.Arduino.Models.Nodes.Common;
using SmartHome.Arduino.Models.Nodes.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SmartHome.Arduino.Models.Nodes.Common.GeneralNode;

namespace SmartHome.Arduino.Models.Nodes
{
	public class ConditionNode : INode
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public FlexibleValue FlexiValue { get; set; } = new();
		public NodeTypes Type { get; set; } = NodeTypes.Value;

		public Conditions Condition { get; set; }
		public DataReference In1 { get; set; }
		public DataReference In2 { get; set; }
		public DataReference Out1 { get; set; }
		public DataReference Out2 { get; set; }

		public enum Conditions
		{
			Equals,
			NotEquals,
			BiggerThan,
			SmallerThan,
		}

		public object? GetValue()
		{
			bool comparation = Compare();

			if (comparation)
				return Out1.GetValue();
			else return Out2.GetValue();
		}

		public bool Compare()
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

			return Condition switch
			{
				Conditions.Equals => IsEqual(First, Second),
				Conditions.NotEquals => IsNotEqual(First, Second),
				Conditions.BiggerThan => IsBigger(First, Second),
				Conditions.SmallerThan => IsSmaller(First, Second),
				_ => false,
			};
		}

		public static bool IsEqual(object first, object second)
		{
			return first.Equals(second);
		}

		public static bool IsNotEqual(object first, object second)
		{
			return !first.Equals(second);
		}

		public static bool IsBigger(dynamic first, dynamic second)
		{
			if (first is bool || second is bool)
			{
				return false;
			}
			return first > second;
		}

		public static bool IsSmaller(dynamic first, dynamic second)
		{
			if (first is bool || second is bool)
			{
				return false;
			}
			return first < second;
		}

	}
}
