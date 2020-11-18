using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RQST.Models
{
    //Not final
    public class Volunteer
    {
        public int SerialNo { get; set; }
        public string Name { get; set; }
        public string Nric { get; set; }
        public string Contact { get; set; }
        [JsonProperty(PropertyName = "Attending")]
        public string Attendance { get; set; }
        public string Status { get; set; }
    }
}
