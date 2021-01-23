using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RQST.Models
{
    public class Log
    {
        public string Action { get; set; }
        public string adminID { get; set; }
        public DateTime Time { get; set; }
        public Log(string id, string action, DateTime time)
        {
            Action = action;
            adminID = id;
            Time = time;
        }
    }
    public class LogDay
    {
        public List<Log> LogList { get; set; } = new List<Log>();
        public DateTime Date { get; set; }

    }
}
