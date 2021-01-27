using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RQST.Models
{
    public class AreaRoot
    {
        public List<Area> arealist { get; set; } = new List<Area>();
        public List<items> ItemList { get; set; } = new List<items>();
        public void addItem(items item)
        {
            items itemF = ItemList.Find(x => x.ID == item.ID);
            if (itemF != null)
            {
                itemF.Requested += item.Requested;
            }
            else
            {
                ItemList.Add(item);
            }
        }
    }
    public class Area
    {
        public string AreaCode { get; set; }
        public List<Request_NEW> ReqList { get; set; } = new List<Request_NEW>();
        public Area(string areacode)
        {
            AreaCode = areacode;
        }
        public Area()
        {

        }

    }
}
