using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RQST.Models
{
    public class Area
    {
        public string AreaName { get; set; }
        public int AreaNum { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public double Radius { get; set; }
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
        public Area(string areaname, int areanum, double lat, double lng, double radius)
        {
            AreaName = areaname;
            AreaNum = areanum;
            Lat = lat;
            Lng = lng;
            Radius = radius;
        }
    }
}
