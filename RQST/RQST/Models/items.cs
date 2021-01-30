using Newtonsoft.Json;
using System;

namespace RQST.Models
{
    public class items
    {
        public items() {}

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("name_zh")]
        public string Name_CL { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

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

        public items(string bgCol, string icon, string name, string name_zh, string category, int remaining, int requested, int limit)
        {
            BgCol = bgCol;
            Category = category;
            Icon = icon;
            Name = name;
            Name_CL = name_zh;
            Requested = requested;
            Limit = limit;
            stock = remaining;
        }
    }
}
