using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Arduino.Application.Exceptions;
using SmartHome.Arduino.Models.Components.Common.Interfaces;

namespace SmartHome.Arduino.Models.Components.Common
{
    public class GeneralComponent
    {
        public static IGeneralComponent? CreateById(ComponentTypes componentId)
        {
            Type? componentType = GetTypeById(componentId);
            if (componentType is null)
                throw new ComponentNotFoundException(componentId);
            return Activator.CreateInstance(componentType) as IGeneralComponent;
        }

        public static Type? GetTypeById(ComponentTypes componentId)
        {
            string componentName = Enum.GetName(typeof(ComponentTypes), componentId) ?? string.Empty;
            if (string.IsNullOrEmpty(componentName))
                throw new ComponentNotFoundException(componentId);
            return Type.GetType($"SmartHome.Arduino.Models.Components.{componentName}");
        }

        public static ComponentTypes GetTypeByString(string componentName)
        {
            return (ComponentTypes)Enum.Parse(typeof(ComponentTypes), componentName);
        }

        public enum ComponentTypes
		{
			Undefined,
			LightSensor,
			Relay,
			Button,
			LedDiode,
			HumiditySensor,
			GasSensor,
            InfraredSensor
		}
    }
}
