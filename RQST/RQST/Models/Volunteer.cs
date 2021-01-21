using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RQST.Models
{
    public class Volunteer
    {
        public Volunteer() { }
        public string AssignedZones { get; set; }

        [EmailAddress]
        [RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$")]
        public string Email { get; set; }

        public string Password { get; set; }

        public string RegionCode { get; set; }

        [JsonPropertyName("CompletedRequests")]
        public int CompletedRequests { get; set; }

        [JsonPropertyName("Contact")]
        public int Contact { get; set; }

        [JsonPropertyName("Name")]
        public string Name { get; set; }

        public string ZoneID { get; set; }
        public string PostalCode { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public string ID { get; set; }

    }
}
