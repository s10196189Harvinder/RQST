using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RQST.Models
{
    public class UserRequests
    {
        public Elderly User { get; set; }
        public List<Request> Requests { get; set; } = new List<Request>();
        public List<items> itemlist { get; set; } = new List<items>();
        public void addItem(items item)
        {
            items itemF = itemlist.Find(x => x.ID == item.ID);
            if (itemF != null)
            {
                itemF.Requested += item.Requested;
            }
            else
            {
                itemlist.Add(item);
            }
        }
    }
}
