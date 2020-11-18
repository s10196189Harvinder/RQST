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

        [HttpPost]
        public async Task<IActionResult> CreateElderlyAsync(string Name, char Gender, string Email, string Password, string Address, string PostalCode)
        {
            if (ModelState.IsValid)
            {
                string auth = HttpContext.Session.GetString("auth");
                await DataDALContext.postElderly(Name, Gender, Email, Password, Address, PostalCode, auth);
                return RedirectToAction("_ViewElderly");
            }

            else
            {
                return View();
            }
        }

        public async Task<IActionResult> _ViewElderlyAsync()
        {
            string auth = HttpContext.Session.GetString("auth");
            List<Elderly> elderlylist = await DataDALContext.getElderly(auth);     //Gets authentication token (in JSON) and passes it to the DAL function getdata
            return View(elderlylist);
        }

        [HttpPost]
        public ActionResult CreateVolunteer(Volunteer volunteer)
        {
            if (ModelState.IsValid)
            {
                // auto assign serial no of volunteer
                return RedirectToAction("Admin");
            }

            else 
            {
                return View(volunteer);
            }
        }

        public IActionResult CreateVolunteer()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AdminAsync(string Name, string Deliverables, string SpecialRequest, string Address)
        {
            string auth = HttpContext.Session.GetString("auth");
            //await DataDALContext.postdata(Name, Deliverables, SpecialRequest, Address, auth);       //gets auth token, passes to DAL
            return RedirectToAction("RawRequests");
        }
        public async Task<IActionResult> RawRequestsAsync()
        {
            string auth = HttpContext.Session.GetString("auth");
            List<Request> reqlist = await DataDALContext.getrequests(auth);     //Gets authentication token (in JSON) and passes it to the DAL function getdata
            return View(reqlist);                                           //Returns the list to the view
        }
        public async Task<IActionResult> CreateElderlyAsync()
        {
            return View();
        }
    }
}
