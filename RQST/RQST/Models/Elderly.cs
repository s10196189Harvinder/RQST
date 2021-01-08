using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace RQST.Models
{
    //Not final
    public class Elderly
    {
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Name is is too long! Enter a shorter name.")]
        public string Name { get; set; }

        public char Gender { get; set; }

        [EmailAddress]
        [RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$")]
        public string Email { get; set; }

        public string Password { get; set; }

        public string Address { get; set; }

        [RegularExpression(@"[0-9]{6}")]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

        [Display(Name = "Special Needs")]
        public string SpecialNeeds { get; set; }

        public string ID { get; set; }
    }
}
