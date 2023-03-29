using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Arduino.Application.Exceptions
{
    public class ComponentNotFoundException : Exception
    {
        public ComponentNotFoundException(object? key)
            : base($"Component with ID ({key}) not found.") { }
        public ComponentNotFoundException(string name, object? key)
            : base($"Component \"{name}\" with ID ({key}) not found.") { }
    }
}
