using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RQST.Models
{
    public class Area
    {
        public string AreaCode { get; set; }
        public List<Request_NEW> ReqList { get; set; } = new List<Request_NEW>();
        public Area(string areacode)
        {
            AreaCode = areacode;
        }
    }
}
