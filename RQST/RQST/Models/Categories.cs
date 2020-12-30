using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RQST.Models
{
    public class Categories
    {
        [JsonProperty("items")]
        public IDictionary<string, string> Contents { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }
        [JsonProperty("category")]
        public string Name { get; set; }
        public string ID { get; set; }
    }
}
