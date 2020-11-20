using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RQST.DAL;
using RQST.Models;

namespace RQST.Controllers
{
    public class AdminController : Controller
    {
        private DataDAL DataDALContext = new DataDAL();
        public async Task<IActionResult> AdminAsync()
        {
            List<Area> arealist = await InitRequestsAsync();
            return View(arealist);
        }
        public IActionResult AddItem()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddItemAsync(items items)
        {
            string auth = HttpContext.Session.GetString("auth");
            await DataDALContext.AddItem(auth, items);
            TempData["GMessage"] = "Created successfully !";
            return View();
        }

        public IActionResult AddCat()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddCatAsync(string category,string icon)
        {
            string auth = HttpContext.Session.GetString("auth");
            await DataDALContext.AddCat(auth,category,icon);
            TempData["GMessage"] = "Created successfully !";
            return View();
        }

        public async Task<IActionResult> CatViewAsync(string category, string icon)
        {
            string auth = HttpContext.Session.GetString("auth");
            List<Categories> catlist = await DataDALContext.getCat(auth);
            return View(catlist);
        }

        [HttpPost]
        public async Task<IActionResult> CreateElderlyAsync(string Name, char Gender, string Email, string Password, string Address, string PostalCode, string SpecialNeeds)
        {
            if (ModelState.IsValid)
            {
                string auth = HttpContext.Session.GetString("auth");
                bool success = await DataDALContext.postElderly(Name, Gender, Email, Password, Address, PostalCode, SpecialNeeds, auth);
                if (success != true)
                {
                    TempData["Message"] = "Failed";
                    return View();
                }
                return RedirectToAction("_ViewElderly");
            }

            else
            {
                return View();
            }
        }

        public async Task<IActionResult> CreateElderlyAsync()
        {
            return View();
        }

        public async Task<IActionResult> _ViewElderlyAsync()
        {
            string auth = HttpContext.Session.GetString("auth");
            List<Elderly> elderlylist = await DataDALContext.getElderly(auth);     //Gets authentication token (in JSON) and passes it to the DAL function getdata
            return View(elderlylist);
        }

        [HttpPost]
        public async Task<ActionResult> CreateVolunteerAsync(string Name, string Contact, string Attendance, string Status)
        {
            if (ModelState.IsValid)
            {
                string auth = HttpContext.Session.GetString("auth");
                await DataDALContext.postVolunteer(Name, Contact, Attendance, Status, auth);
                return RedirectToAction("_ViewVolunteer");
            }

            else 
            {
                return View();
            }
        }

        public async Task<IActionResult> CreateVolunteerAsync()
        {
            return View();
        }

        public async Task<IActionResult> AddItoC(string catid)
        {
            string auth = HttpContext.Session.GetString("auth");
            Categories cat = await DataDALContext.getaCat(auth,catid);
            return View(cat);
        }
        [HttpPost]
        public async Task<IActionResult> AddItoC(string name, string catid)
        {
            string auth = HttpContext.Session.GetString("auth");
            await DataDALContext.putIinC(auth, catid, name);
            return View();
            
        }


        public async Task<IActionResult> _ViewVolunteerAsync()
        {
            string auth = HttpContext.Session.GetString("auth");
            List<Volunteer> volunteerlist = await DataDALContext.getVolunteer(auth);     //Gets authentication token (in JSON) and passes it to the DAL function getdata
            return View(volunteerlist);
        }

        [HttpPost]
        public async Task<IActionResult> AdminAsync(string Name, string Deliverables, string SpecialRequest, string Address)
        {
            string auth = HttpContext.Session.GetString("auth");
            return RedirectToAction("RawRequests");
        }
        public async Task<IActionResult> RawRequestsAsync()
        {
            string auth = HttpContext.Session.GetString("auth");
            List<Request> reqlist = await DataDALContext.getrequests(auth);     //Gets authentication token (in JSON) and passes it to the DAL function getdata
            return View(reqlist);                                           //Returns the list to the view
        }
        public async Task<IActionResult> RequestsAsync()
        {
            List<Area> arealist = await InitRequestsAsync();
            return View(arealist);
        }
        public async Task<List<Area>> InitRequestsAsync()
        {
            string auth = HttpContext.Session.GetString("auth");
            List<UserRequests> something = await DataDALContext.getuserrequests(auth);
            List<Area> arealist = new List<Area>();
            foreach (UserRequests usrqst in something)
            {
                string prefix = usrqst.PostalCode.Substring(0, 2);
                int pre = Convert.ToInt32(prefix);
                if (pre < 7)
                {
                    Area oldarea = arealist.Find(x => x.AreaName == "Telok Blangah, Harbourfront");
                    if (oldarea == null)
                    {
                        oldarea = new Area("Telok Blangah, Harbourfront");
                        arealist.Add(oldarea);
                    }
                    oldarea.AddReq(usrqst);
                    foreach (var item in usrqst.itemlist)
                    {
                        oldarea.AddItem(item);
                    }
                }
                else if (7 >= pre || pre < 9)
                {
                    Area oldarea = arealist.Find(x => x.AreaName == "Anson, Tanjong Pagar");
                    if (oldarea == null)
                    {
                        oldarea = new Area("Anson, Tanjong Pagar");
                        arealist.Add(oldarea);
                    }
                    oldarea.AddReq(usrqst);
                    foreach (var item in usrqst.itemlist)
                    {
                        oldarea.AddItem(item);
                    }
                }
                else if (9 >= pre || pre < 11)
                {
                    Area oldarea = arealist.Find(x => x.AreaName == "Telok Blangah, Harbourfront");
                    if (oldarea == null)
                    {
                        oldarea = new Area("Telok Blangah, Harbourfront");
                        arealist.Add(oldarea);
                    }
                    oldarea.AddReq(usrqst);
                    foreach (var item in usrqst.itemlist)
                    {
                        oldarea.AddItem(item);
                    }
                }
                else if (11 >= pre || pre < 14)
                {
                    Area oldarea = arealist.Find(x => x.AreaName == "Pasir Panjang, Hong Leong Garden, Clementi New Town");
                    if (oldarea == null)
                    {
                        oldarea = new Area("Pasir Panjang, Hong Leong Garden, Clementi New Town");
                        arealist.Add(oldarea);
                    }
                    oldarea.AddReq(usrqst);
                    foreach (var item in usrqst.itemlist)
                    {
                        oldarea.AddItem(item);
                    }
                }
                else if (14 >= pre || pre < 17)
                {
                    Area oldarea = arealist.Find(x => x.AreaName == "Bukit Merah, Queenstown, Tiong Bahru");
                    if (oldarea == null)
                    {
                        oldarea = new Area("Bukit Merah, Queenstown, Tiong Bahru");
                        arealist.Add(oldarea);
                    }
                    oldarea.AddReq(usrqst);
                    foreach (var item in usrqst.itemlist)
                    {
                        oldarea.AddItem(item);
                    }
                }
                else if (17 == pre)
                {
                    Area oldarea = arealist.Find(x => x.AreaName == "High Street, Beach Road(part)");
                    if (oldarea == null)
                    {
                        oldarea = new Area("High Street, Beach Road(part)");
                        arealist.Add(oldarea);
                    }
                    oldarea.AddReq(usrqst);
                    foreach (var item in usrqst.itemlist)
                    {
                        oldarea.AddItem(item);
                    }
                }
                else if (18 >= pre || pre < 20)
                {
                    Area oldarea = arealist.Find(x => x.AreaName == "Middle Road, Golden Mile");
                    if (oldarea == null)
                    {
                        oldarea = new Area("Middle Road, Golden Mile");
                        arealist.Add(oldarea);
                    }
                    oldarea.AddReq(usrqst);
                    foreach (var item in usrqst.itemlist)
                    {
                        oldarea.AddItem(item);
                    }
                }
                else if (20 >= pre || pre < 22)
                {
                    Area oldarea = arealist.Find(x => x.AreaName == "Little India, Farrer Park, Jalan Besar, Lavender");
                    if (oldarea == null)
                    {
                        oldarea = new Area("Little India, Farrer Park, Jalan Besar, Lavender");
                        arealist.Add(oldarea);
                    }
                    oldarea.AddReq(usrqst);
                    foreach (var item in usrqst.itemlist)
                    {
                        oldarea.AddItem(item);
                    }
                }
                else if (22 >= pre || pre < 24)
                {
                    Area oldarea = arealist.Find(x => x.AreaName == "Orchard, Cairnhill, River Valley");
                    if (oldarea == null)
                    {
                        oldarea = new Area("Orchard, Cairnhill, River Valley");
                        arealist.Add(oldarea);
                    }
                    oldarea.AddReq(usrqst);
                    foreach (var item in usrqst.itemlist)
                    {
                        oldarea.AddItem(item);
                    }
                }
                else if (24 >= pre || pre < 28)
                {
                    Area oldarea = arealist.Find(x => x.AreaName == "Ardmore, Bukit Timah, Holland Road, Tanglin");
                    if (oldarea == null)
                    {
                        oldarea = new Area("Ardmore, Bukit Timah, Holland Road, Tanglin");
                        arealist.Add(oldarea);
                    }
                    oldarea.AddReq(usrqst);
                    foreach (var item in usrqst.itemlist)
                    {
                        oldarea.AddItem(item);
                    }
                }
                else if (28 >= pre || pre < 31)
                {
                    Area oldarea = arealist.Find(x => x.AreaName == "Watten Estate, Novena, Thomson");
                    if (oldarea == null)
                    {
                        oldarea = new Area("Watten Estate, Novena, Thomson");
                        arealist.Add(oldarea);
                    }
                    oldarea.AddReq(usrqst);
                    foreach (var item in usrqst.itemlist)
                    {
                        oldarea.AddItem(item);
                    }
                }
                else if (31 >= pre || pre < 34)
                {
                    Area oldarea = arealist.Find(x => x.AreaName == "Balestier, Toa Payoh, Serangoon");
                    if (oldarea == null)
                    {
                        oldarea = new Area("Balestier, Toa Payoh, Serangoon");
                        arealist.Add(oldarea);
                        
                    }
                    oldarea.AddReq(usrqst);
                    foreach (var item in usrqst.itemlist)
                    {
                        oldarea.AddItem(item);
                    }
                }
                else if (34 >= pre || pre < 38)
                {
                    Area oldarea = arealist.Find(x => x.AreaName == "Macpherson, Braddell");
                    if (oldarea == null)
                    {
                        oldarea = new Area("Macpherson, Braddell");
                        arealist.Add(oldarea);
                    }
                    oldarea.AddReq(usrqst);
                    foreach (var item in usrqst.itemlist)
                    {
                        oldarea.AddItem(item);
                    }
                }
                else if (38 >= pre || pre < 42)
                {
                    Area oldarea = arealist.Find(x => x.AreaName == "Geylang, Eunos, Aljunied");
                    if (oldarea == null)
                    {
                        oldarea = new Area("Geylang, Eunos, Aljunied");
                        arealist.Add(oldarea);
                    }
                    else
                    {
                        oldarea.AddReq(usrqst);
                    }
                    oldarea.AddReq(usrqst);
                    foreach (var item in usrqst.itemlist)
                    {
                        oldarea.AddItem(item);
                    }
                }
                else if (42 >= pre || pre < 46)
                {
                    Area oldarea = arealist.Find(x => x.AreaName == "Katong, Joo Chiat, Amber Road");
                    if (oldarea == null)
                    {
                        oldarea = new Area("Katong, Joo Chiat, Amber Road");
                        arealist.Add(oldarea);
                    }
                    oldarea.AddReq(usrqst);
                    foreach (var item in usrqst.itemlist)
                    {
                        oldarea.AddItem(item);
                    }
                }
                else if (46 >= pre || pre < 49)
                {
                    Area oldarea = arealist.Find(x => x.AreaName == "Bedok, Upper East Coast, Eastwood, Kew Drive");
                    if (oldarea == null)
                    {
                        oldarea = new Area("Bedok, Upper East Coast, Eastwood, Kew Drive");
                        arealist.Add(oldarea);
                    }
                    oldarea.AddReq(usrqst);
                    foreach (var item in usrqst.itemlist)
                    {
                        oldarea.AddItem(item);
                    }
                }
                else if (49 >= pre || pre < 51 || pre == 81)
                {
                    Area oldarea = arealist.Find(x => x.AreaName == "Loyang, Changi");
                    if (oldarea == null)
                    {
                        oldarea = new Area("Loyang, Changi");
                        arealist.Add(oldarea);
                    }
                    oldarea.AddReq(usrqst);
                    foreach (var item in usrqst.itemlist)
                    {
                        oldarea.AddItem(item);
                    }
                }
                else if (51 >= pre || pre < 53)
                {
                    Area oldarea = arealist.Find(x => x.AreaName == "Simei, Tampines, Pasir Ris");
                    if (oldarea == null)
                    {
                        oldarea = new Area("Simei, Tampines, Pasir Ris");
                        arealist.Add(oldarea);
                    }
                    oldarea.AddReq(usrqst);
                    foreach (var item in usrqst.itemlist)
                    {
                        oldarea.AddItem(item);
                    }
                }
                else if (53 >= pre || pre < 55 || pre == 82)
                {
                    Area oldarea = arealist.Find(x => x.AreaName == "Serangoon Garden, Hougang, Punggol");
                    if (oldarea == null)
                    {
                        oldarea = new Area("Serangoon Garden, Hougang, Punggol");
                        arealist.Add(oldarea);
                    }
                    oldarea.AddReq(usrqst);
                    foreach (var item in usrqst.itemlist)
                    {
                        oldarea.AddItem(item);
                    }
                }
                else if (56 >= pre || pre < 58)
                {
                    Area oldarea = arealist.Find(x => x.AreaName == "Bishan, Ang Mo Kio");
                    if (oldarea == null)
                    {
                        oldarea = new Area("Bishan, Ang Mo Kio");
                        arealist.Add(oldarea);
                    }
                    oldarea.AddReq(usrqst);
                    foreach (var item in usrqst.itemlist)
                    {
                        oldarea.AddItem(item);
                    }
                }
                else if (58 >= pre || pre < 60)
                {
                    Area oldarea = arealist.Find(x => x.AreaName == "Upper Bukit Timah, Clementi Park, Ulu Pandan");
                    if (oldarea == null)
                    {
                        oldarea = new Area("Upper Bukit Timah, Clementi Park, Ulu Pandan");
                        arealist.Add(oldarea);
                    }
                    oldarea.AddReq(usrqst);
                    foreach (var item in usrqst.itemlist)
                    {
                        oldarea.AddItem(item);
                    }
                }
                else if (60 >= pre || pre < 65)
                {
                    Area oldarea = arealist.Find(x => x.AreaName == "Penjuru, Jurong, Pioneer, Tuas");
                    if (oldarea == null)
                    {
                        oldarea = new Area("Penjuru, Jurong, Pioneer, Tuas");
                        arealist.Add(oldarea);
                    }
                    oldarea.AddReq(usrqst);
                    foreach (var item in usrqst.itemlist)
                    {
                        oldarea.AddItem(item);
                    }
                }
                else if (65 >= pre || pre < 69)
                {
                    Area oldarea = arealist.Find(x => x.AreaName == "Hillview, Dairy Farm, Bukit Panjang, Choa Chu Kang");
                    if (oldarea == null)
                    {
                        oldarea = new Area("Hillview, Dairy Farm, Bukit Panjang, Choa Chu Kang");
                        arealist.Add(oldarea);
                    }
                    oldarea.AddReq(usrqst);
                    foreach (var item in usrqst.itemlist)
                    {
                        oldarea.AddItem(item);
                    }
                }
                else if (69 >= pre || pre < 72)
                {
                    Area oldarea = arealist.Find(x => x.AreaName == "Lim Chu Kang, Tengah");
                    if (oldarea == null)
                    {
                        oldarea = new Area("Lim Chu Kang, Tengah");
                        arealist.Add(oldarea);
                    }
                    oldarea.AddReq(usrqst);
                    foreach (var item in usrqst.itemlist)
                    {
                        oldarea.AddItem(item);
                    }
                }
                else if (72 >= pre || pre < 74)
                {
                    Area oldarea = arealist.Find(x => x.AreaName == "Kranji, Woodgrove, Woodlands");
                    if (oldarea == null)
                    {
                        oldarea = new Area("Kranji, Woodgrove, Woodlands");
                        arealist.Add(oldarea);
                    }
                    oldarea.AddReq(usrqst);
                    foreach (var item in usrqst.itemlist)
                    {
                        oldarea.AddItem(item);
                    }
                }
                else if (77 >= pre || pre < 79)
                {
                    Area oldarea = arealist.Find(x => x.AreaName == "Upper Thomson, Springleaf");
                    if (oldarea == null)
                    {
                        oldarea = new Area("Upper Thomson, Springleaf");
                        arealist.Add(oldarea);
                    }
                    oldarea.AddReq(usrqst);
                    foreach (var item in usrqst.itemlist)
                    {
                        oldarea.AddItem(item);
                    }
                }
                else if (75 >= pre || pre < 77)
                {
                    Area oldarea = arealist.Find(x => x.AreaName == "Yishun, Sembawang, Senoko");
                    if (oldarea == null)
                    {
                        oldarea = new Area("Yishun, Sembawang, Senoko");
                        arealist.Add(oldarea);
                    }
                    oldarea.AddReq(usrqst);
                    foreach (var item in usrqst.itemlist)
                    {
                        oldarea.AddItem(item);
                    }
                }
                else if (79 >= pre || pre < 81)
                {
                    Area oldarea = arealist.Find(x => x.AreaName == "Seletar");
                    if (oldarea == null)
                    {
                        oldarea = new Area("Seletar");
                        arealist.Add(oldarea);
                    }
                    oldarea.AddReq(usrqst);
                    foreach (var item in usrqst.itemlist)
                    {
                        oldarea.AddItem(item);
                    }
                }
            }
            return arealist;
        }
    }
}
