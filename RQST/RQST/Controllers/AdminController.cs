using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RQST.DAL;
using RQST.Models;

namespace RQST.Controllers
{
    public class AdminController : Controller
    {
        private DataDAL DataDALContext = new DataDAL();
        public IActionResult Admin()
        {
            return View();
        }

        public async Task<IActionResult> RawRequestsAsync()
        {
            string auth = HttpContext.Session.GetString("auth");
            List<Request> reqlist = await DataDALContext.getdata(auth);
            return View(reqlist);
        }
    }
}
