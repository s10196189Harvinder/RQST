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
                bool success = await DataDALContext.postElderly(Name, Gender, Email, Password, Address, PostalCode, auth);
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
        public async Task<IActionResult> RequestsAsync()
        {
            string auth = HttpContext.Session.GetString("auth");
            List<UserRequests> something = await DataDALContext.getuserrequests(auth);
            something = await DataDALContext.getUserRequestsAdress(auth, something);
            List<Area> arealist = new List<Area>();
            arealist.Add(new Area("Raffles Place, Cecil, Marina, People's Park"));
            arealist.Add(new Area("Anson, Tanjong Pagar"));
            arealist.Add(new Area("Telok Blangah, Harbourfront"));
            arealist.Add(new Area("Pasir Panjang, Hong Leong Garden, Clementi New Town"));
            arealist.Add(new Area("Bukit Merah, Queenstown, Tiong Bahru"));
            arealist.Add(new Area("High Street, Beach Road(part)"));
            arealist.Add(new Area("Middle Road, Golden Mile"));
            arealist.Add(new Area("Little India, Farrer Park, Jalan Besar, Lavender"));
            arealist.Add(new Area("Orchard, Cairnhill, River Valley"));
            arealist.Add(new Area("Ardmore, Bukit Timah, Holland Road, Tanglin"));
            arealist.Add(new Area("Watten Estate, Novena, Thomson"));
            arealist.Add(new Area("Balestier, Toa Payoh, Serangoon"));
            arealist.Add(new Area("Macpherson, Braddell"));
            arealist.Add(new Area("Geylang, Eunos, Aljunied"));
            arealist.Add(new Area("Katong, Joo Chiat, Amber Road"));
            arealist.Add(new Area("Bedok, Upper East Coast, Eastwood, Kew Drive"));
            arealist.Add(new Area("Loyang, Changi"));
            arealist.Add(new Area("Simei, Tampines, Pasir Ris"));
            arealist.Add(new Area("Serangoon Garden, Hougang, Punggol"));
            arealist.Add(new Area("Bishan, Ang Mo Kio"));
            arealist.Add(new Area("Upper Bukit Timah, Clementi Park, Ulu Pandan"));
            arealist.Add(new Area("Penjuru, Jurong, Pioneer, Tuas"));
            arealist.Add(new Area("Hillview, Dairy Farm, Bukit Panjang, Choa Chu Kang "));
            arealist.Add(new Area("Lim Chu Kang, Tengah"));
            arealist.Add(new Area("Kranji, Woodgrove, Woodlands"));
            arealist.Add(new Area("Upper Thomson, Springleaf"));
            arealist.Add(new Area("Yishun, Sembawang, Senoko"));
            arealist.Add(new Area("Seletar"));
            foreach (UserRequests usrqst in something)
            {
                string prefix = usrqst.PostalCode.Substring(0, 2);
                int pre = Convert.ToInt32(prefix);
                if (pre < 7)
                {
                    usrqst.AreaName = "Raffles Place, Cecil, Marina, People's Park";
                    arealist[0].AddReq(usrqst);
                    foreach(var item in usrqst.itemlist)
                    {
                        arealist[0].AddItem(item);
                    }
                    usrqst.PostalDistrict = 1;
                }
                else if (7 <= pre || pre < 9)
                {
                    usrqst.AreaName = "Anson, Tanjong Pagar";
                    arealist[1].AddReq(usrqst);
                    foreach (var item in usrqst.itemlist)
                    {
                        arealist[1].AddItem(item);
                    }
                    usrqst.PostalDistrict = 2;
                }
                else if (9 <= pre || pre < 11)
                {
                    usrqst.AreaName = "Telok Blangah, Harbourfront";
                    arealist[2].AddReq(usrqst);
                    foreach (var item in usrqst.itemlist)
                    {
                        arealist[2].AddItem(item);
                    }
                    usrqst.PostalDistrict = 4;
                }
                else if (11 <= pre || pre < 14)
                {
                    usrqst.AreaName = "Pasir Panjang, Hong Leong Garden, Clementi New Town ";
                    arealist[3].AddReq(usrqst);
                    foreach (var item in usrqst.itemlist)
                    {
                        arealist[3].AddItem(item);
                    }
                    usrqst.PostalDistrict = 5;
                }
                else if (14 <= pre || pre < 17)
                {
                    usrqst.AreaName = "Bukit Merah, Queenstown, Tiong Bahru";
                    arealist[4].AddReq(usrqst);
                    foreach (var item in usrqst.itemlist)
                    {
                        arealist[4].AddItem(item);
                    }
                    usrqst.PostalDistrict = 3;
                }
                else if (17 == pre)
                {
                    usrqst.AreaName = "High Street, Beach Road (part) ";
                    arealist[5].AddReq(usrqst);
                    foreach (var item in usrqst.itemlist)
                    {
                        arealist[5].AddItem(item);
                    }
                    usrqst.PostalDistrict = 6;
                }
                else if (18 <= pre || pre < 20)
                {
                    usrqst.AreaName = "Middle Road, Golden Mile ";
                    arealist[6].AddReq(usrqst);
                    foreach (var item in usrqst.itemlist)
                    {
                        arealist[6].AddItem(item);
                    }
                    usrqst.PostalDistrict = 7;
                }
                else if (20 <= pre || pre < 22)
                {
                    usrqst.AreaName = "Little India, Farrer Park, Jalan Besar, Lavender";
                    arealist[7].AddReq(usrqst);
                    foreach (var item in usrqst.itemlist)
                    {
                        arealist[7].AddItem(item);
                    }
                    usrqst.PostalDistrict = 8;
                }
                else if (22 <= pre || pre < 24)
                {
                    usrqst.AreaName = "Orchard, Cairnhill, River Valley ";
                    arealist[8].AddReq(usrqst);
                    foreach (var item in usrqst.itemlist)
                    {
                        arealist[8].AddItem(item);
                    }
                    usrqst.PostalDistrict = 9;
                }
                else if (24 <= pre || pre < 28)
                {
                    usrqst.AreaName = "Ardmore, Bukit Timah, Holland Road, Tanglin";
                    arealist[9].AddReq(usrqst);
                    foreach (var item in usrqst.itemlist)
                    {
                        arealist[9].AddItem(item);
                    }
                    usrqst.PostalDistrict = 10;
                }
                else if (28 <= pre || pre < 31)
                {
                    usrqst.AreaName = "Watten Estate, Novena, Thomson ";
                    arealist[10].AddReq(usrqst);
                    foreach (var item in usrqst.itemlist)
                    {
                        arealist[10].AddItem(item);
                    }
                    usrqst.PostalDistrict = 11;
                }
                else if (31 <= pre || pre < 34)
                {
                    usrqst.AreaName = "Balestier, Toa Payoh, Serangoon ";
                    arealist[11].AddReq(usrqst);
                    foreach (var item in usrqst.itemlist)
                    {
                        arealist[11].AddItem(item);
                    }
                    usrqst.PostalDistrict = 12;
                }
                else if (34 <= pre || pre < 38)
                {
                    usrqst.AreaName = "Macpherson, Braddell ";
                    arealist[12].AddReq(usrqst);
                    foreach (var item in usrqst.itemlist)
                    {
                        arealist[12].AddItem(item);
                    }
                    usrqst.PostalDistrict = 13;
                }
                else if (38 <= pre || pre < 42)
                {
                    usrqst.AreaName = "Geylang, Eunos, Aljunied ";
                    arealist[13].AddReq(usrqst);
                    foreach (var item in usrqst.itemlist)
                    {
                        arealist[13].AddItem(item);
                    }
                    usrqst.PostalDistrict = 14;
                }
                else if (42 <= pre || pre < 46)
                {
                    usrqst.AreaName = "Katong, Joo Chiat, Amber Road ";
                    arealist[14].AddReq(usrqst);
                    foreach (var item in usrqst.itemlist)
                    {
                        arealist[14].AddItem(item);
                    }
                    usrqst.PostalDistrict = 15;
                }
                else if (46 <= pre || pre < 49)
                {
                    usrqst.AreaName = "Bedok, Upper East Coast, Eastwood, Kew Drive ";
                    arealist[15].AddReq(usrqst);
                    foreach (var item in usrqst.itemlist)
                    {
                        arealist[15].AddItem(item);
                    }
                    usrqst.PostalDistrict = 16;
                }
                else if (49 <= pre || pre < 51 || pre == 81)
                {
                    usrqst.AreaName = "Loyang, Changi ";
                    arealist[16].AddReq(usrqst);
                    foreach (var item in usrqst.itemlist)
                    {
                        arealist[16].AddItem(item);
                    }
                    usrqst.PostalDistrict = 17;
                }
                else if (51 <= pre || pre < 53)
                {
                    usrqst.AreaName = "Simei, Tampines, Pasir Ris ";
                    arealist[17].AddReq(usrqst);
                    foreach (var item in usrqst.itemlist)
                    {
                        arealist[17].AddItem(item);
                    }
                    usrqst.PostalDistrict = 18;
                }
                else if (53 <= pre || pre < 55 || pre == 82)
                {
                    Area area = arealist[18];
                    area.AddReq(usrqst);
                    foreach (var item in usrqst.itemlist)
                    {
                        area.AddItem(item);
                    }
                    usrqst.AreaName = "Serangoon Garden, Hougang, Punggol ";
                    usrqst.PostalDistrict = 19;
                }
                else if (56 <= pre || pre < 58)
                {
                    usrqst.AreaName = "Bishan, Ang Mo Kio ";
                    Area area = arealist[19];
                    area.AddReq(usrqst);
                    foreach (var item in usrqst.itemlist)
                    {
                        area.AddItem(item);
                    }
                    usrqst.PostalDistrict = 20;
                }
                else if (58 <= pre || pre < 60)
                {
                    usrqst.AreaName = "Upper Bukit Timah, Clementi Park, Ulu Pandan ";
                    Area area = arealist[20];
                    area.AddReq(usrqst);
                    foreach (var item in usrqst.itemlist)
                    {
                        area.AddItem(item);
                    }
                    usrqst.PostalDistrict = 21;
                }
                else if (60 <= pre || pre < 65)
                {
                    usrqst.AreaName = "Penjuru, Jurong, Pioneer, Tuas ";
                    Area area = arealist[21];
                    area.AddReq(usrqst);
                    foreach (var item in usrqst.itemlist)
                    {
                        area.AddItem(item);
                    }
                    usrqst.PostalDistrict = 22;
                }
                else if (65 <= pre || pre < 69)
                {
                    usrqst.AreaName = "Hillview, Dairy Farm, Bukit Panjang, Choa Chu Kang ";
                    Area area = arealist[22];
                    area.AddReq(usrqst);
                    foreach (var item in usrqst.itemlist)
                    {
                        area.AddItem(item);
                    }
                    usrqst.PostalDistrict = 23;
                }
                else if (69 <= pre || pre < 72)
                {
                    usrqst.AreaName = "Lim Chu Kang, Tengah ";
                    Area area = arealist[23];
                    area.AddReq(usrqst);
                    foreach (var item in usrqst.itemlist)
                    {
                        area.AddItem(item);
                    }
                    usrqst.PostalDistrict = 24;
                }
                else if (72 <= pre || pre < 74)
                {
                    usrqst.AreaName = "Kranji, Woodgrove, Woodlands";
                    Area area = arealist[24];
                    area.AddReq(usrqst);
                    foreach (var item in usrqst.itemlist)
                    {
                        area.AddItem(item);
                    }
                    usrqst.PostalDistrict = 25;
                }
                else if (77 <= pre || pre < 79)
                {
                    usrqst.AreaName = "Upper Thomson, Springleaf";
                    Area area = arealist[25];
                    area.AddReq(usrqst);
                    foreach (var item in usrqst.itemlist)
                    {
                        area.AddItem(item);
                    }
                    usrqst.PostalDistrict = 26;
                }
                else if (75 <= pre || pre < 77)
                {
                    usrqst.AreaName = "Yishun, Sembawang, Senoko";
                    Area area = arealist[26];
                    area.AddReq(usrqst);
                    foreach (var item in usrqst.itemlist)
                    {
                        area.AddItem(item);
                    }
                    usrqst.PostalDistrict = 27;
                }
                else if (79 <= pre || pre < 81)
                {
                    usrqst.AreaName = "Seletar";
                    Area area = arealist[27];
                    area.AddReq(usrqst);
                    foreach (var item in usrqst.itemlist)
                    {
                        area.AddItem(item);
                    }
                    usrqst.PostalDistrict = 28;
                }
            }
            return View(arealist);
        }
        public async Task<IActionResult> CreateElderlyAsync()
        {
            return View();
        }
    }
}
