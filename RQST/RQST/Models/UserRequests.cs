using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RQST.Models
{
    public class UserRequests
    {
        public string UserID { get; set; }
        public List<string> Requests { get; set; } = new List<string>();
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public List<items> itemlist { get; set; } = new List<items>();
        public void addItem(items item)
        {
            itemlist.Add(item);
        }
    }
}
