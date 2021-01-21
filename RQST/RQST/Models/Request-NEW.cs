using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RQST.Models
{
    public class Request_NEW
    {
        public string ZoneID { get; set; }
        public string RegionCode { get; set; }
        public List<Request> ReqList { get; set; } = new List<Request>();
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
}
