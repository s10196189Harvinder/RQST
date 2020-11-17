using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RQST.Models
{
    //Not final
    public class Elderly
    {
        public string Name { get; set; }
        public string ICNo { get; set; }
        public char Gender { get; set; }
        public DateTime DOB { get; set; }
        public string Contact { get; set; }
        public string Address { get; set; }
        [JsonProperty(PropertyName = "Special Request")]
        public String SpecialRequest { get; set; }
    }
}
