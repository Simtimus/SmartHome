using SmartHome.Arduino.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Arduino.Models.DataProcessing
{
    public class DataLinker
    {
        public List<DataLink> DataLinks = new();

        public void LinkData(BoardPin from, BoardPin to)
        {
            to.DataLink = new(from);
            to.DataLink.Id = Guid.NewGuid();
            DataLinks.Add(to.DataLink);
        }

        public void RemoveLink(Guid Id)
        {

        }
    }
}
