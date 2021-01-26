using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RQST.Models
{
    public class items
    {
        public items() {}

        public items(string bgCol, string icon, int limit, string name, int requested, int remaining)
        {
            Name = name;
            Requested = requested;
            Icon = icon;
            Limit = limit;
            stock = remaining;
            BgCol = bgCol;
        }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("requested")]
        public int Requested { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }
        
        [JsonProperty("stock")]
        public int stock { get; set; }
        
        [JsonProperty("bgCol")]
        public string BgCol { get; set; }

        [JsonProperty("limit")]
        public int Limit { get; set; }

        public string ID { get; set; }
    }
}
