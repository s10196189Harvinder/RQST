using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RQST.Models
{
    public class Properties
    {
        public string Name { get; set; }
        public object description { get; set; }
        public string SUBZONE_NO { get; set; }
        public string SUBZONE_N { get; set; }
        public string SUBZONE_C { get; set; }
        public string CA_IND { get; set; }
        public string PLN_AREA_N { get; set; }
        public string PLN_AREA_C { get; set; }
        public string REGION_N { get; set; }
        public string REGION_C { get; set; }
        public string INC_CRC { get; set; }
        public string FMEL_UPD_D { get; set; }
    }

    public class Geometry
    {
        public string type { get; set; }
        public List<List<List<object>>> coordinates { get; set; }
    }

    public class Subzone
    {
        public string type { get; set; }
        public Properties properties { get; set; }
        public Geometry geometry { get; set; }
    }
}
