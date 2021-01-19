using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RQST.Models
{
    public class Area
    {
        public string AreaCode { get; set; }
        public List<SubArea> SubArea { get; set; } = new List<SubArea>();
        public Area(string areacode)
        {
            AreaCode = areacode;
        }
    }
    public class SubArea
    {
        public int Name { get; set; }
        public List<UserRequests> reqlist { get; set; } = new List<UserRequests>();
    }
}
