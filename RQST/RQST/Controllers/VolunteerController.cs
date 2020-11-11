using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace RQST.Controllers
{
    public class VolunteerController : Controller
    {
        public IActionResult Volunteer()
        {
            return View();
        }
    }
}
