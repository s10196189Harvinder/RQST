using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Http;
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
        public static DataDAL DataDALContext = new DataDAL();
        public async Task<IActionResult> MapAsync()
        {
            string auth = (HttpContext.Session.GetString("auth"));
            await DataDALContext.InitClientAsync(auth);
            List<Request_NEW> reqList = await DataDALContext.getUserRequests();
            return View(reqList);
        }
        [HttpPost]
        public async Task<IActionResult> MapAsync(string Name, string Deliverables, string SpecialRequest, string Address)
        {
            
            return RedirectToAction("RawRequests");
        }


        public async Task<IActionResult> CreateElderlyAsync()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateElderlyAsync(string Name, char Gender, string Email, string Contact, string Password, string Address, string PostalCode, string SpecialNeeds)
        {
            if (ModelState.IsValid)
            {
                

                SubzoneRoot zone = await getZoneAsync(PostalCode);
                if (zone == null)
                {
                    TempData["Message"] = "Geocoding failed - check for valid postal code";     //If geocoding fails (no identified subzone), probably because of bad (incorrect) postal code. Sends error.
                    return View();
                }
                bool success = await DataDALContext.postElderly(Name, Gender, Email, Contact, Password, Address, PostalCode, SpecialNeeds, zone.properties); //Posts the elderly to the FB
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
            
            List<Elderly> elderlylist = await DataDALContext.getElderly();     //Gets authentication token (in JSON) and passes it to the DAL function getdata
            return View(elderlylist);
        }
        public async Task<IActionResult> CreateVolunteerAsync()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreateVolunteerAsync(string Name, string Email, string Password, string Contact, string PostalCode, string AssignedZone)
        {
            if (ModelState.IsValid)
            {
                
                SubzoneRoot zone = await getZoneAsync(PostalCode);
                if (zone != null)
                {
                    TempData["Message"] = "Geocoding failed - check for valid postal code";     //If geocoding fails (no identified subzone), probably because of bad postal code. Sends error.
                    return View();
                }
                await DataDALContext.postVolunteer(Name, Email, Password, Contact, PostalCode, zone.properties, AssignedZone);
                return RedirectToAction("_ViewVolunteer");
            }
            else
            {
                return View();
            }
        }
        public async Task<IActionResult> RawRequestsAsync()
        {
            
            List<Request> reqlist = await DataDALContext.getrequests();     //Gets authentication token (in JSON) and passes it to the DAL function getdata
            return View(reqlist);                                               //Returns the list to the view
        }
        public async Task<IActionResult> RequestsAsync()
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            SubzoneList SZList = JsonConvert.DeserializeObject<SubzoneList>(System.IO.File.ReadAllText(@"wwwroot/subzones.geojson"));     //Read subzones from JSON file and store them as list of class <Subzone>
            timer.Stop();
            Debug.WriteLine(timer.Elapsed);
            timer.Start();
            List<Request_NEW> reqList = await DataDALContext.getUserRequestsMIN();
            timer.Stop();
            Debug.WriteLine(timer.Elapsed);
            AreaRoot areaList = new AreaRoot();
            List<Elderly> elderlyList = await DataDALContext.getElderly();
            foreach (Request_NEW req in reqList)
            {
                SubzoneRoot zone = SZList.features.Find(x => x.properties.Name == req.ZoneID);
                req.RegionCode = zone.properties.REGION_C;
                Area area = areaList.arealist.Find(x => x.AreaCode == req.RegionCode);
                if (area != null)
                {
                    area.ReqList.Add(req);
                }
                else
                {
                    Area nArea = new Area(req.RegionCode);
                    nArea.ReqList.Add(req);
                    areaList.arealist.Add(nArea);
                }
                foreach (Request request in req.ReqList)
                {
                    Elderly elder = elderlyList.Find(x => x.ID == request.SenderID);
                    request.Sender = elder;
                }
            }
            areaList.tItemsList = await DataDALContext.getItems();
            return View(areaList);
        }
        public async Task<IActionResult> AddItoC(string catid)      //Add item to category page
        {
            
            Categories cat = await DataDALContext.getaCat(catid);
            return View(cat);
        }
        [HttpPost]
        public async Task<IActionResult> AddItoC(string name, string catid)
        {
            
            await DataDALContext.AddItemtoCat(catid, name);
            return View();

        }


        public async Task<IActionResult> _ViewVolunteerAsync()
        {
            
            List<Volunteer> volunteerlist = await DataDALContext.getVolunteer();     //Gets authentication token (in JSON) and passes it to the DAL function getdata
            return View(volunteerlist);
        }
        public async Task<IActionResult> ViewVolAsync()
        {
            
            List<Volunteer> volunteerlist = await DataDALContext.getVolunteer();     //Gets authentication token (in JSON) and passes it to the DAL function getdata
            return View(volunteerlist);
        }

        public async Task<IActionResult> AsgnVolunteer(string? id)
        {
            string auth = (HttpContext.Session.GetString("auth"));
            Volunteer vol = await DataDALContext.getAVolunteer(id);
            ViewBag.id = vol.ID;
            return View(vol);
        }
        [HttpPost]
        public async Task<IActionResult> AsgnVolunteer(string vol, string zones)
        {
            if (ModelState.IsValid)
            {
                bool success = await DataDALContext.updateVolunteerID(vol, zones);
                return RedirectToAction("ViewVol");
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddItemAsync(items items)
        {
            
            await DataDALContext.AddItem(items);
            TempData["GMessage"] = "Created successfully !";
            return View();
        }

        public IActionResult AddCat()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddCatAsync(string category, string icon)
        {
            
            await DataDALContext.AddCat(category, icon);
            TempData["GMessage"] = "Created successfully !";
            return View();
        }

        public async Task<IActionResult> CatViewAsync(string category, string icon)
        {
            
            List<Categories> catlist = await DataDALContext.getCat();
            return View(catlist);
        }
        public IActionResult AddItem()
        {
            return View();
        }

        public async Task<IActionResult> _EditElderlyAsync(string? id)
        {
            
            Elderly eld = await DataDALContext.getAElderly(id);
            return View(eld);
        }

        [HttpPost]
        public async Task<IActionResult> _EditElderlyAsync(Elderly eld)
        {
            if (ModelState.IsValid)
            {
                
                bool success = await DataDALContext.updateElderly(eld);
                return RedirectToAction("_ViewElderly");
            }

            else
            {
                return View();
            }
        }

        public async Task<IActionResult> _EditVolunteerAsync(string? id)
        {
            
            Volunteer vol = await DataDALContext.getAVolunteer(id);
            return View(vol);
        }

        [HttpPost]
        public async Task<IActionResult> _EditVolunteerAsync(Volunteer vol)
        {
            if (ModelState.IsValid)
            {
                
                bool success = await DataDALContext.updateVolunteer(vol);
                return RedirectToAction("_ViewVolunteer");
            }

            else
            {
                return View();
            }
        }
        public async Task<IActionResult> ViewLogAsync()
        {
            
            List<LogDay> logDays = await DataDALContext.getLog();
            return View(logDays);
        }




        public static bool check(List<PointF> polygon, PointF testPoint)      //This function checks if a point is in a polygon using the Ray Casting Algorithm
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
        public static async Task<SubzoneRoot> getZoneAsync(string postalCode)
        {
            SubzoneList SZList = JsonConvert.DeserializeObject<SubzoneList>(System.IO.File.ReadAllText(@"wwwroot/subzones.geojson"));     //Read subzones from JSON file and store them as list of class <Subzone>
            string addr = "Singapore " + postalCode; //Set address as e.g. Singapore 468123
            var url = string.Format("https://maps.googleapis.com/maps/api/geocode/json?address={0}&key={1}", addr, "AIzaSyA3QoucpamS6ylPkzBSJBXmbt5ZH7Np6Jk");  //Set the URL for GeoCoding
            Geocoded res = await url        //Make a request to the Google GeoCoding API, supplying the Singapore 468123 as the "search" field
                .PostAsync()
                .ReceiveJson<Geocoded>();
            float lat;
            float lng;
            try
            {
                lat = res.results[0].geometry.location.lat;       //Get the latitude from the request result
                lng = res.results[0].geometry.location.lng;       //Get the longitude from the request result - These are the exact locations of the postal code.;
            }
            catch
            {
                return null;
            }
            PointF latlng = new PointF(lat, lng);                   //Target point - Lat/Lng where request is
            foreach (SubzoneRoot sz in SZList.features)             //Loop thru ALL subzones from the JSON file
            {
                foreach (var obj in sz.geometry.coordinates)        //Obtains Lat/Lng of subzones from the list.
                {                                                   //Create polygon of pointF
                    List<PointF> subz = new List<PointF>();
                    for (int i = 0; i < obj.Count(); i++)           //This part obtains the latitude & longitude - It's weird and complex because the GEOJSON file (from data.gov) has a lot of nesting
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
                        return sz;
                    }
                }
            }
            return null;
        }
    }
}