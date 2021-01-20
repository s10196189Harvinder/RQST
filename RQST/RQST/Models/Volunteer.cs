﻿using Newtonsoft.Json;
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
        public string AssignedZones { get; set; }

        public string RegionCode { get; set; }

        [JsonPropertyName("CompletedRequests")]
        public int CompletedRequests { get; set; }

        [JsonPropertyName("Contact")]
        public int Contact { get; set; }

        [JsonPropertyName("Name")]
        public string Name { get; set; }

        public string ZoneID { get; set; }
        public string PostalCode { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public string ID { get; set; }
    }
}
