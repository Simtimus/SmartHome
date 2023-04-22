using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Arduino.Models.Arduino;

namespace SmartHome.Arduino.Models.Data.DataLinks
{
    public class DataLinker
    {
        public List<DataLink> DataLinks = new();

        public void LinkData(BoardPin from, BoardPin to)
        {
            to.DataLink = new(from)
            {
                Id = Guid.NewGuid()
            };
            DataLinks.Add(to.DataLink);
        }

        public bool RemoveLink(Guid Id)
        {
            DataLink? dataLink = DataLinks.Find(x => x.Id == Id);
            if (dataLink == null)
            {
                dataLink = new();
                DataLinks.Remove(dataLink);
                return true;
            }
            return false;
        }
    }
}
