using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RQST.Models
{
    public class Location
    {
        public float lat { get; set; }
        public float lng { get; set; }
    }

    public class Geometry2
    {
        public Location location { get; set; }
        public string location_type { get; set; }
    }

    public class Result
    {
        public Geometry2 geometry { get; set; }
    }

    public class Geocoded
    {
        public List<Result> results { get; set; }
        public string status { get; set; }
    }
}
