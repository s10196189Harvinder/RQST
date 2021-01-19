using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RQST.Models
{
    public class Subzone
    {
        public string name { get; set; }
        public string Name { get; set; }
        public string SUBZONE_NO { get; set; }
        public string SUBZONE_N { get; set; }
        public string REGION_C { get; set; }
    }

    public class Geometry
    {
        public string type { get; set; }
        public List<List<List<object>>> coordinates { get; set; }
    }

    public class SubzoneRoot
    {
        public string type { get; set; }
        public Subzone properties { get; set; }
        public Geometry geometry { get; set; }
    }

    public class SubzoneList
    {
        public string type { get; set; }
        public List<SubzoneRoot> features { get; set; }
    }
}
