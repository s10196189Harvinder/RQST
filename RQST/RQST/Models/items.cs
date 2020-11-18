using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RQST.Models
{
    public class items
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("requested")]
        public int Requested { get; set; }
    }
}
