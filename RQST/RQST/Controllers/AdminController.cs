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
            string auth = HttpContext.Session.GetString("auth");
            List<UserRequests> userreqlist = await DataDALContext.getuserrequests(auth);
            List<Area> arealist = new List<Area>();
            foreach(UserRequests req in userreqlist)
            {
                string areacode = req.User.Region_Code;
                Area area = arealist.Find(x => x.AreaCode == areacode);
                if (area!=null)
                {
                    SubArea subArea = area.SubArea.Find(x => x.Name == req.User.Zone_ID);
                    if (subArea != null)
                    {
                        subArea.reqlist.Add(req);
                    }
                    else
                    {
                        SubArea nSubArea = new SubArea();
                        nSubArea.Name = req.User.Zone_ID;
                        nSubArea.reqlist.Add(req);
                        area.SubArea.Add(nSubArea);
                    }
                }
                else
                {
                    Area nArea = new Area(areacode);
                    SubArea nSubArea = new SubArea();
                    nSubArea.Name = req.User.Zone_ID;
                    nSubArea.reqlist.Add(req);
                    nArea.SubArea.Add(nSubArea);
                    arealist.Add(nArea);
                }
            }
            return View(arealist);
        }
        public async Task<IActionResult> AsgnVolunteerAsync()
        {
            string auth = (HttpContext.Session.GetString("auth"));
            List<UserRequests> vollist = await DataDALContext.getuserrequests(auth);
            return View(vollist);
        }
        //[HttpPost]
        //public async Task<IActionResult> AsgnVolunteerAsync(Volunteer vol, string zoneList)
        //{
        //    string auth = (HttpContext.Session.GetString("auth"));
        //    //List<UserRequests> vollist = await DataDALContext.assgnZone(auth, vol, zoneList);
        //    return View(vollist);
        //}
    }
}
