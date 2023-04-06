using SmartHome.Arduino.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Arduino.Application.Modules.DataLinking
{
    public class DataLinker
    {
        public List<DataLink> DataLinks = new ();

        public void LinkData(BoardPin from, BoardPin to)
        {
            to.DataLink = new(from);
            DataLinks.Add(to.DataLink);
        }
    }
}
