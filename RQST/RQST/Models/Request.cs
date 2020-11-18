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
        [JsonProperty("content")]
        public IDictionary<string, int> Contents { get; set; }

        [JsonProperty("delTimeStart")]
        public long DelTimeStart { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }

}
