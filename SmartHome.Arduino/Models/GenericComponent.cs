using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Arduino.Application.Exceptions;

namespace SmartHome.Arduino.Models
{
    public class GenericComponent
    {
        public static IGenericComponent? CreateById(ComponentsId componentId)
        {
            Type? componentType = GetTypeById(componentId);
            if (componentType is null)
                throw new ComponentNotFoundException(componentId);
            return Activator.CreateInstance(componentType) as IGenericComponent;
        }

        public static Type? GetTypeById(ComponentsId componentId)
        {
            string componentName = Enum.GetName(typeof(ComponentsId), componentId) ?? string.Empty;
            if (string.IsNullOrEmpty(componentName))
                throw new ComponentNotFoundException(componentId);
            return Type.GetType($"SmartHome.Arduino.Models.Components.{componentName}");
        }

        public static ComponentsId GetIdByString(string componentName)
        {
            return (ComponentsId)Enum.Parse(typeof(ComponentsId), componentName);
        }

        public enum ComponentsId
        {
            Unknown,
            LightSensor,
            Relay,
        }
    }
}
