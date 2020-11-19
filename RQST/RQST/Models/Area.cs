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
            itemsList.Add(item);
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
