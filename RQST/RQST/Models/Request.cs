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

        [JsonProperty("delSlotStart")]
        public long DelSlotStart { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
        public string ID { get; set; }
        public string SenderID { get; set; }
        public Elderly Sender { get; set; }

        public float dateCreated { get; set; }
        public List<items> itemList { get; set; } = new List<items>();
    }

}
