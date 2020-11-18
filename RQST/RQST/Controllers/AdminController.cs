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
            foreach(UserRequests usrqst in something)
            {
                string prefix = usrqst.PostalCode.Substring(0, 2);
                int pre = Convert.ToInt32(prefix);
                if (pre < 7)
                {
                    usrqst.AreaName = "Raffles Place, Cecil, Marina, People's Park ";
                    usrqst.PostalDistrict = 1;
                }
                else if (7 <= pre || pre < 9)
                {
                    usrqst.AreaName = "Anson, Tanjong Pagar ";
                    usrqst.PostalDistrict = 2;
                }
                else if (9 <= pre || pre < 11)
                {
                    usrqst.AreaName = "Telok Blangah, Harbourfront";
                    usrqst.PostalDistrict = 4;
                }
                else if (11 <= pre || pre < 14)
                {
                    usrqst.AreaName = "Pasir Panjang, Hong Leong Garden, Clementi New Town ";
                    usrqst.PostalDistrict = 5;
                }
                else if (14 <= pre || pre < 17)
                {
                    usrqst.AreaName = "Bukit Merah, Queenstown, Tiong Bahru";
                    usrqst.PostalDistrict = 3;
                }
                else if (17==pre)
                {
                    usrqst.AreaName = "High Street, Beach Road (part) ";
                    usrqst.PostalDistrict = 6;
                }
                else if (18 <= pre || pre < 20)
                {
                    usrqst.AreaName = "Middle Road, Golden Mile ";
                    usrqst.PostalDistrict = 7;
                }
                else if (20 <= pre || pre < 22)
                {
                    usrqst.AreaName = "Little India, Farrer Park, Jalan Besar, Lavender";
                    usrqst.PostalDistrict = 8;
                }
                else if (22 <= pre || pre < 24)
                {
                    usrqst.AreaName = "Orchard, Cairnhill, River Valley ";
                    usrqst.PostalDistrict = 9;
                }
                else if (24 <= pre || pre < 28)
                {
                    usrqst.AreaName = "Ardmore, Bukit Timah, Holland Road, Tanglin";
                    usrqst.PostalDistrict = 10;
                }
                else if (28 <= pre || pre < 31)
                {
                    usrqst.AreaName = "Watten Estate, Novena, Thomson ";
                    usrqst.PostalDistrict = 11;
                }
                else if (31 <= pre || pre < 34)
                {
                    usrqst.AreaName = "Balestier, Toa Payoh, Serangoon ";
                    usrqst.PostalDistrict = 12;
                }
                else if (34 <= pre || pre < 38)
                {
                    usrqst.AreaName = "Macpherson, Braddell ";
                    usrqst.PostalDistrict = 13;
                }
                else if (38 <= pre || pre < 42)
                {
                    usrqst.AreaName = "Geylang, Eunos, Aljunied ";
                    usrqst.PostalDistrict = 14;
                }
                else if (42 <= pre || pre < 46)
                {
                    usrqst.AreaName = "Katong, Joo Chiat, Amber Road ";
                    usrqst.PostalDistrict = 15;
                }
                else if (46 <= pre || pre < 49)
                {
                    usrqst.AreaName = "Bedok, Upper East Coast, Eastwood, Kew Drive ";
                    usrqst.PostalDistrict = 16;
                }
                else if (49 <= pre || pre < 51 || pre==81)
                {
                    usrqst.AreaName = "Loyang, Changi ";
                    usrqst.PostalDistrict = 17;
                }
                else if (51 <= pre || pre < 53)
                {
                    usrqst.AreaName = "Simei, Tampines, Pasir Ris ";
                    usrqst.PostalDistrict = 18;
                }
                else if (53 <= pre || pre < 55 || pre==82)
                {
                    usrqst.AreaName = "Serangoon Garden, Hougang, Punggol ";
                    usrqst.PostalDistrict = 19;
                }
                else if (56 <= pre || pre < 58)
                {
                    usrqst.AreaName = "Bishan, Ang Mo Kio ";
                    usrqst.PostalDistrict = 20;
                }
                else if (58 <= pre || pre < 60)
                {
                    usrqst.AreaName = "Upper Bukit Timah, Clementi Park, Ulu Pandan ";
                    usrqst.PostalDistrict = 21;
                }
                else if (60 <= pre || pre < 65)
                {
                    usrqst.AreaName = "Penjuru, Jurong, Pioneer, Tuas ";
                    usrqst.PostalDistrict = 22;
                }
                else if (65 <= pre || pre < 69)
                {
                    usrqst.AreaName = "Hillview, Dairy Farm, Bukit Panjang, Choa Chu Kang ";
                    usrqst.PostalDistrict = 23;
                }
                else if (69 <= pre || pre < 72)
                {
                    usrqst.AreaName = "Lim Chu Kang, Tengah ";
                    usrqst.PostalDistrict = 24;
                }
                else if (72 <= pre || pre < 74)
                {
                    usrqst.AreaName = "Kranji, Woodgrove, Woodlands";
                    usrqst.PostalDistrict = 25;
                }
                else if (77 <= pre || pre < 79)
                {
                    usrqst.AreaName = "Upper Thomson, Springleaf";
                    usrqst.PostalDistrict = 26;
                }
                else if (75 <= pre || pre < 77)
                {
                    usrqst.AreaName = "Yishun, Sembawang, Senoko";
                    usrqst.PostalDistrict = 27;
                }
                else if (79 <= pre || pre < 81)
                {
                    usrqst.AreaName = "Seletar";
                    usrqst.PostalDistrict = 28;
                }
            }
            return View(something);
        }
    }
}
