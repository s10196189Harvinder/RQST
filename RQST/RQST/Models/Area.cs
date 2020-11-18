using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RQST.Models
{
    public class Area
    {
        public string AreaName { get; set; }
        public List<items> itemsList { get; set; } = new List<items>();
        public void AddItem(items item)
        {
            items itemf = itemsList.Find(x => x.Name == item.Name);
            if (itemf != null)
            {
                itemf.Requested += item.Requested;
            }
            else
            {
                itemsList.Add(item);
            }
        }
        public List<UserRequests> reqlist { get; set; } = new List<UserRequests>();
        public void AddReq(UserRequests req)
        {
            reqlist.Add(req);
        }
        public Area(string areaname)
        {
            AreaName = areaname;
        }
    }
}
