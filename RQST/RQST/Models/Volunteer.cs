using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace RQST.Models
{
    //Not final
    public class Volunteer
    {
        public int SerialNo { get; set; }

        [StringLength(100, MinimumLength = 3, ErrorMessage = "Name is is too long! Enter a shorter name.")]
        public string Name { get; set; }

        [RegularExpression(@"[0-9]{8}")]
        public string Contact { get; set; }

        [JsonProperty(PropertyName = "Attending")]
        public string Attendance { get; set; }

        public string Status { get; set; }
    }
}
