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

        public long dateCreated { get; set; }
        public DateTime dateCreatedD { get; set; }
        public List<items> itemList { get; set; } = new List<items>();
        public void addItem(items item)
        {
            items itemF = itemList.Find(x => x.ID == item.ID);
            if (itemF != null)
            {
                itemF.Requested += item.Requested;
            }
            else
            {
                itemList.Add(item);
            }
        }
    }

}
