using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RQST.DAL;
using RQST.Models;

namespace RQST.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private LoginDAL loginDALContext = new LoginDAL();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> IndexAsync(string email, string password)
        {
            string auth = "";
            try
            {
                auth = await loginDALContext.loginAsync(email, password);       //Firebase authentication token
            }
            catch{
                TempData["Message"] = "Login failed, please try again.";
                return View();
            }
            string emailname = email.Split('@')[0];
            HttpContext.Session.SetString("auth", auth);                    //Stores token in the cookies
            if (emailname.Contains("admin"))
            {
                return RedirectToAction("Admin","Admin");
            }
            else if (emailname.Contains("vol"))
            {
                return RedirectToAction("Volunteer", "Volunteer");
            }
            else
            {
                TempData["Message"] = "You cannot login with this email.";
                return View();
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
