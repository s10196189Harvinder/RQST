using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RQST.Models
{
    public class Subzone
    {
        public string Name { get; set; }
        public string SubzoneCode { get; set; }
        public List<List<double>> coordinates { get; set; }
    }
}
