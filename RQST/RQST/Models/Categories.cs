using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RQST.Models
{
    public class Categories
    {
        public Categories() { }
        [JsonProperty("items")]
        public IDictionary<string, string> Contents { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }
        [JsonProperty("category")]
        public string Name { get; set; }
        [JsonProperty("category_zh")]
        public string Name_zh { get; set; }
        public string ID { get; set; }
        public Categories(string name, string namezh, string icon)
        {
            Name = name;
            Name_zh = namezh;
            Icon = icon;
        }
    }
}
