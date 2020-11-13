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
            HttpContext.Session.Clear();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> IndexAsync(string email, string password)
        {
            IDictionary<string, string> response = new Dictionary<string, string>();
            response = await loginDALContext.loginAsync(email, password);       //reponse obtained from the DAL
            string emailname = email.Split('@')[0];
            string exception = "";
            if (response.TryGetValue("Exception",out exception))                //Attempts to get any value if Dictionary has an "Exception" key. Only occurs if an exception happens while signing in.
            {
                TempData["Message"] = exception;
                return View();
            }
            string auth = "";
            response.TryGetValue("Auth", out auth);                             //Obtains the authentication token (in JSON)
            HttpContext.Session.SetString("auth", auth);                       //Stores token in the session
            if (emailname.Contains("admin"))                                    //Checking for which type of email the user is signing in with
            {
                return RedirectToAction("Admin","Admin");     //Admin home page
            }
            else if (emailname.Contains("vol"))
            {
                return RedirectToAction("Volunteer", "Volunteer");  //Volunteer home page
            }
            else
            {
                TempData["Message"] = "You cannot login with this email.";  //Neither volunteer nor admin
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
