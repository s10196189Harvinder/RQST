using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RQST.Models
{
    //Not final
    public class Volunteer
    {
        [JsonPropertyName("Assigned-Zones")]
        public string AssignedZones { get; set; }

        [JsonPropertyName("CompletedRequests")]
        public int CompletedRequests { get; set; }

        [JsonPropertyName("Contact")]
        public int Contact { get; set; }

        [JsonPropertyName("Name")]
        public string Name { get; set; }

        public Subzone Zone { get; set; }
        public string ID { get; set; }
    }
}
