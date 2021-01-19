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

        public IActionResult LoginPage()
        {
            HttpContext.Session.Clear();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginPageAsync(string email, string password)
        {
            IDictionary<string, string> response = await loginDALContext.loginAsync(email, password);       //reponse obtained from the DALs
            string exception = "";
            if (response.TryGetValue("Exception", out exception))                //Attempts to get any value if Dictionary has an "Exception" key. Only occurs if an exception happens while signing in.
            {
                TempData["Message"] = exception;
                return View();
            }
            string auth = "";
            response.TryGetValue("Auth", out auth);                             //Obtains the authentication token (in JSON)
            HttpContext.Session.SetString("auth", auth);                       //Stores token in the session
            return RedirectToAction("Map", "Admin");                          //Admin home page
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult ForgotPass()
        {
            return View();
        }
        public IActionResult ResetSent()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassAsync(string email)
        {
            string result = await loginDALContext.resetpass(email);
            if (result != "Success")
            {
                TempData["Message"] = result;
                return View();
            }
            return RedirectToAction("ResetSent","Home");
        }
    }
}
