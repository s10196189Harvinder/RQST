using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RQST.Models
{
    //Not final
    public class Request
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Deliverables { get; set; }
        [JsonProperty(PropertyName = "Special Request")]
        public string SpecialRequest { get; set; }
    }
    
}
