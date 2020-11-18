using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RQST.Models
{
    public class UserRequests
    {
        public string UserID { get; set; }
        public List<string> Requests { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string AreaName { get; set; }
        public int PostalDistrict { get; set; }
    }
}
