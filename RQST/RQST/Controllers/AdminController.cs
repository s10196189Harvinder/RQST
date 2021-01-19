using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Flurl.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RQST.DAL;
using RQST.Models;

namespace RQST.Controllers
{
    public class AdminController : Controller
    {
        private static readonly HttpClient client = new HttpClient();
        private DataDAL DataDALContext = new DataDAL();
        public async Task<IActionResult> AdminAsync()
        {
            string auth = (HttpContext.Session.GetString("auth"));
            List<UserRequests> userreqlist = await DataDALContext.getuserrequests(auth);
            return View(userreqlist);
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
        public async Task<IActionResult> CreateElderlyAsync(string Name, char Gender, string Email, string Password, string Address, string PostalCode, string SpecialNeeds)//not working as intended currently
        {
            if (ModelState.IsValid)
            {
                string auth = HttpContext.Session.GetString("auth");

                SubzoneList SZList = JsonConvert.DeserializeObject<SubzoneList>(System.IO.File.ReadAllText(@"wwwroot/subzones.geojson"));     //Read subzones from JSON file and store them as list of class <Subzone>
                string addr = "Singapore " + PostalCode;
                var url = string.Format("https://maps.googleapis.com/maps/api/geocode/json?address={0}&key={1}", addr, "AIzaSyA3QoucpamS6ylPkzBSJBXmbt5ZH7Np6Jk");
                Geocoded res = await url
                    .PostAsync()
                    .ReceiveJson<Geocoded>();
                float lat = res.results[0].geometry.location.lat;
                float lng = res.results[0].geometry.location.lng;
                SubzoneRoot zone = null;
                bool success = false;
                PointF latlng = new PointF(lat, lng);       //Target point - Lat/Lng where request is
                foreach (SubzoneRoot sz in SZList.features)
                {
                    foreach (var obj in sz.geometry.coordinates)        //Obtains Lat/Lng of subzones from the list.
                    {                                                   //Create polygon of pointF
                        List<PointF> subz = new List<PointF>();
                        for (int i = 0; i < obj.Count(); i++)
                        {
                            for (int y = 0; y < obj[i].Count(); y++)
                            {
                                PointF point = new PointF();
                                if (obj[i].Count() != 3)
                                {
                                    foreach (var coorpair in obj[i])
                                    {
                                        JArray pts = new JArray(coorpair);
                                        point = new PointF((float)Convert.ToDouble(pts[0][1]), (float)Convert.ToDouble(pts[0][0]));
                                    }
                                }
                                else
                                {
                                    point = new PointF((float)Convert.ToDouble(obj[i][1]), (float)Convert.ToDouble(obj[i][0]));
                                }
                                subz.Add(point);
                            }
                        }
                        bool isIn = check(subz, latlng);        //Checks if target point is in subzone polygon
                        if (isIn)
                        {
                            zone = sz;                          //Sets zone to subzone, and breaks from loop
                            success = true;
                            break;
                        }
                    }    
                }
                if (success != true)
                {
                    TempData["Message"] = "Geocoding failed - check for valid postal code";     //If geocoding fails (no identified subzone), probably because of bad postal code. Sends error.
                    return View();
                }
                success = await DataDALContext.postElderly(Name, Gender, Email, Password, Address, PostalCode, SpecialNeeds, zone.properties,auth);
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
        public static bool check(List<PointF> polygon, PointF testPoint)
        {
            bool result = false;
            int j = polygon.Count() - 1;
            for (int i = 0; i < polygon.Count(); i++)
            {
                if (polygon[i].Y < testPoint.Y && polygon[j].Y >= testPoint.Y || polygon[j].Y < testPoint.Y && polygon[i].Y >= testPoint.Y)
                {
                    if (polygon[i].X + (testPoint.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) * (polygon[j].X - polygon[i].X) < testPoint.X)
                    {
                        result = !result;
                    }
                }
                j = i;
            }
            return result;
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
        public async Task<ActionResult> CreateVolunteerAsync(string Name, string Contact, string Attendance, string PostalCode, string Status)
        {
            if (ModelState.IsValid)
            {
                string auth = HttpContext.Session.GetString("auth");
                await DataDALContext.postVolunteer(Name, Contact, Attendance, Status, auth);
                bool success = false;
                return RedirectToAction("_ViewVolunteer");
                success = await DataDALContext.postVolunteer(Name, Contact, Attendance, Status, auth);
                if (success != true)
                {
                    TempData["Message"] = "Failed";
                    return View();
                }
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
                string prefix = usrqst.User.PostalCode.Substring(0, 2);  //Get 1st 2 digits of postal code
                int pre = Convert.ToInt32(prefix);
                if (pre < 7)        //Checks postal code prefix
                {
                    //Tries to find area based on name
                    Area oldarea = arealist.Find(x => x.AreaName == "Raffles Place, Cecil, Marina, People's Park");
                    if (oldarea == null)    //If area does not exist create area
                    {
                        oldarea = new Area("Raffles Place, Cecil, Marina, People's Park", 1, 1.285121,103.865261,1391.31);
                        arealist.Add(oldarea);
                    }
                    oldarea.AddReq(usrqst); //If it do add request to area
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
                        oldarea = new Area("Anson, Tanjong Pagar", 2,1.276060,103.840770,1390.16);
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
                        oldarea = new Area("Telok Blangah, Harbourfront",4,1.260455,103.816247,1575.08);
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
                        oldarea = new Area("Pasir Panjang, Hong Leong Garden, Clementi New Town",5,1.289376,103.765630,3594.95);
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
                        oldarea = new Area("Bukit Merah, Queenstown, Tiong Bahru",3,1.287417,103.813870,1258.08);
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
                        oldarea = new Area("High Street, Beach Road(part)",6,1.291077,103.848087,432.97);
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
                        oldarea = new Area("Middle Road, Golden Mile",7,1.299355,103.854196,440.02);
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
                        oldarea = new Area("Little India, Farrer Park, Jalan Besar, Lavender",8,1.306101,103.848052,521.22);
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
                        oldarea = new Area("Orchard, Cairnhill, River Valley",9, 1.303492,103.831832,1050.75);
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
                        oldarea = new Area("Ardmore, Bukit Timah, Holland Road, Tanglin",10,1.316878,103.808821,1484.54);
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
                        oldarea = new Area("Watten Estate, Novena, Thomson",11,1.327624,103.832866,1383.73);
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
                        oldarea = new Area("Balestier, Toa Payoh, Serangoon",12,1.320075,103.857490,1307.81);
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
                        oldarea = new Area("Macpherson, Braddell",13, 1.328716, 103.877371,1132.26);
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
                        oldarea = new Area("Geylang, Eunos, Aljunied",14, 1.333101, 103.9002822, 1758.74);
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
                        oldarea = new Area("Katong, Joo Chiat, Amber Road",15, 1.297363, 103.897392,2200.54);
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
                        oldarea = new Area("Bedok, Upper East Coast, Eastwood, Kew Drive",16, 1.320215,103.932891,1660.25);
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
                        oldarea = new Area("Loyang, Changi",17,1.351193,103.991776,3384.61);
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
                        oldarea = new Area("Simei, Tampines, Pasir Ris",18, 1.358760,103.939414,2192.53);
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
                        oldarea = new Area("Serangoon Garden, Hougang, Punggol",19,1.373012,103.898936,3354.03);
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
                        oldarea = new Area("Bishan, Ang Mo Kio",20,1.359630,103.839218,1940.24);
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
                        oldarea = new Area("Upper Bukit Timah, Clementi Park, Ulu Pandan", 21,1.335503,103.789779,2009.37);
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
                        oldarea = new Area("Penjuru, Jurong, Pioneer, Tuas", 22,1.315477,103.661500,5509.66);
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
                        oldarea = new Area("Hillview, Dairy Farm, Bukit Panjang, Choa Chu Kang",23,1.361798,103.756060,3550.96);
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
                        oldarea = new Area("Lim Chu Kang, Tengah",24,1.407104,103.700098,3975.65);
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
                        oldarea = new Area("Kranji, Woodgrove, Woodlands",25,1.423270,103.773422,3250.99);
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
                        oldarea = new Area("Upper Thomson, Springleaf", 26,1.397369,103.819582,2367.58);
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
                        oldarea = new Area("Yishun, Sembawang, Senoko", 27,1423655,103.840142,2401.0);
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
                        oldarea = new Area("Seletar", 28,1.409982,103.877239,1940.36);
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
