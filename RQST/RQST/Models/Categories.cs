using Newtonsoft.Json;
using System.Collections.Generic;

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
        public string Namezh { get; set; }
        public string ID { get; set; }
        public Categories(string name, string namezh, string icon)
        {
            Name = name;
            Namezh = namezh;
            Icon = icon;
        }
    }
}
