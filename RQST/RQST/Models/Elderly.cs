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
        public string Name { get; set; }
        public char Gender { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }

        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }
    }
}
