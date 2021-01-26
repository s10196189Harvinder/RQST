using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RQST.Models;

namespace RQST.DAL
{
    public class DataDAL
    {
        public FirebaseClient firebaseClient { get; set; }
        public string AdminID { get; set; }
        public async Task<bool> postElderly(string name, char gender, string email, string contact, string password, string address, string postalcode, string specialneeds, Subzone zone)     //This method POSTS data to the firebase
        {
            var ap = new FirebaseAuthProvider(new Firebase.Auth.FirebaseConfig("AIzaSyBjdJIn1k3ksbbZAgY-kQIwUXbD0Zo_q8w"));
            FirebaseAuthLink res;
            try
            {
                res = await ap.CreateUserWithEmailAndPasswordAsync(email, password);      //Attemps to create user with given email & password
            }
            catch
            {

                return false;
            }
            Elderly elderly = new Elderly(name, gender, email, contact, address, postalcode, specialneeds, zone.Name, zone.REGION_C); //Puts elderly in firebase
            await firebaseClient                //Posts the elderly object to under (DATABASE)/Requests/UserID
                    .Child("elderly")
                    .Child(res.User.LocalId)    //Sets the location of the data to be posted to the ID of the user
                    .PutAsync(elderly);         //PUTs location at /Eldelry/UserID/...  (Not POST beacuase POST generates random ID - it would become /Eldelry/UID/ID/...)
            await firebaseClient
                .Child("authroles")             // Places UserID in the authroles section
                .PatchAsync("{\"" + res.User.LocalId + "\":\"elderly\"}");  //Patching in JSON format - "USERID:elderly"
            await PostLog("Created Elderly");
            return true;
        }
        public async Task<bool> AddCat(string name, string namezh, string icon)   //Add Category to database
        {
            Categories categories = new Categories(name, namezh, icon);
            await firebaseClient
                .Child("Categories")
                .PostAsync(categories);
            await PostLog("Created Category");
            return true;
        }
        public async Task<bool> AddItemtoCat(string id, string categoryid)   //Adds items to categories in the DB
        {
            var item = await firebaseClient     //POSTS to /categories/CATEGORYID/items/ITEMID/...
                .Child("categories")
                .Child(categoryid)
                .Child("items")
                .PostAsync(id);                 //POST to RANDOMID:ITEMID
            return true;
        }
        public async Task<bool> AddItem(items item)    //Adds item to DB
        {
            var itemp = await firebaseClient
                .Child("items")             //Posts to /items/RANDOMID/...
                .PostAsync(item);
            await PostLog("Added Item");
            return true;
        }
        public async Task<bool> postVolunteer(string name, string email, string password, string contact, string postalcode, Subzone zone, string assignedzone)
        {
            var ap = new FirebaseAuthProvider(new Firebase.Auth.FirebaseConfig("AIzaSyBjdJIn1k3ksbbZAgY-kQIwUXbD0Zo_q8w"));
            FirebaseAuthLink res;
            try
            {
                res = await ap.CreateUserWithEmailAndPasswordAsync(email, password);      //Attemps to create user with given email & password
            }
            catch
            {

                return false;
            }
            Volunteer volunteer = new Volunteer();
            volunteer.Name = name;
            volunteer.Email = email;
            volunteer.Password = password;
            volunteer.Contact = Convert.ToInt32(contact);
            volunteer.PostalCode = postalcode;
            volunteer.ZoneID = zone.Name;
            volunteer.RegionCode = zone.REGION_C;
            volunteer.AssignedZones = assignedzone;
            await firebaseClient
                .Child("volunteer")
                .Child(res.User.LocalId)
                .PutAsync(volunteer);
            await firebaseClient
                .Child("authroles")             // Places UserID in the authroles section
                .PatchAsync("{\"" + res.User.LocalId + "\":\"volunteer\"}");  //Patching in JSON format - "USERID:elderly"
            await PostLog("Created Volunteer");
            return true;
        }

        public async Task<List<Request>> getrequests()               //This method obtains data from the firebase
        {
            var requests = await firebaseClient                                 //Obtains all data from (DATABASE)/Requests
                        .Child("requests")
                        .OnceAsync<Request>();
            List<Request> reqlist = new List<Request>();                        //Turns all objects inside "requests" into Request objects
            foreach (var request in requests)
            {
                reqlist.Add(request.Object);
            }
            return reqlist;                                                    //Returns the list of requests
        }

        public async Task<List<Request_NEW>> getUserRequests()
        {
            var reqData = await firebaseClient
                .Child("requests")
                .OnceAsync<IDictionary<string, Object>>();
            var itemData = await firebaseClient
                .Child("requestCounter")
                .OnceAsync<IDictionary<string, string>>();
            List<items> itemList = new List<items>();
            var fbItemList = await firebaseClient
                                    .Child("items")
                                    .OnceAsync<items>();
            foreach (var item in fbItemList)
            {
                items itemActual = item.Object;
                itemActual.ID = item.Key;
                itemList.Add(itemActual);
            }
            List<Request_NEW> reqList = new List<Request_NEW>();
            foreach (var area in reqData)
            {
                Request_NEW req = new Request_NEW();
                req.ZoneID = area.Key;
                foreach (var requestID in area.Object)
                {
                    Request currReq = JsonConvert.DeserializeObject<Request>(requestID.Value.ToString());
                    currReq.ID = requestID.Key;
                    req.ReqList.Add(currReq);
                }
                reqList.Add(req);
            }
            foreach (var area in itemData)
            {
                Request_NEW req = reqList.Find(x => x.ZoneID == area.Key);
                foreach (var item in area.Object)
                {
                    items itemF = itemList.Find(x => x.ID == item.Key);
                    itemF.Requested = Convert.ToInt32(item.Value);
                    req.ItemList.Add(itemF);
                }
            }
            return (reqList);
        }


        public async Task<List<Request_NEW>> getUserRequestsMIN()
        {
            var reqData = await firebaseClient
                .Child("requests")
                .OnceAsync<IDictionary<string, Object>>();
            List<items> itemList = new List<items>();
            var fbItemList = await firebaseClient
                                    .Child("items")
                                    .OnceAsync<items>();
            foreach (var item in fbItemList)
            {
                items itemActual = item.Object;
                itemActual.ID = item.Key;
                itemList.Add(itemActual);
            }
            List<Request_NEW> reqList = new List<Request_NEW>();
            foreach (var area in reqData)
            {
                Request_NEW req = new Request_NEW();
                req.ZoneID = area.Key;
                foreach (KeyValuePair<string, object> requestID in area.Object)
                {
                    Request currReq = JsonConvert.DeserializeObject<Request>(requestID.Value.ToString());
                    currReq.ID = requestID.Key;
                    foreach (KeyValuePair<string, int> kvp in currReq.Contents)
                    {
                        items newItem = itemList.Find(x => x.ID == kvp.Key);
                        items newerItem = new items(newItem.BgCol, newItem.Icon, newItem.Limit, newItem.Name, kvp.Value, newItem.stock);
                        newerItem.ID = kvp.Key;
                        currReq.addItem(newerItem);
                    }
                    currReq.dateCreatedD  = DateTimeOffset.FromUnixTimeSeconds(currReq.dateCreated).DateTime.ToLocalTime();
                    req.ReqList.Add(currReq);
                    
                }
                reqList.Add(req);
            }
            return (reqList);
        }

        public async Task<List<Elderly>> getElderly()  //Obtains the elderly data
        {
            var elderlies = await firebaseClient
                        .Child("elderly")
                        .OnceAsync<Elderly>();
            List<Elderly> elderlylist = new List<Elderly>();
            foreach (var elderly in elderlies)
            {
                elderly.Object.ID = elderly.Key;
                elderlylist.Add(elderly.Object);
            }
            return elderlylist;
        }

        public async Task<List<Categories>> getCat() //Gets all the categories
        {
            var elderlies = await firebaseClient
                        .Child("categories")
                        .OnceAsync<Categories>();
            List<Categories> catlist = new List<Categories>();
            foreach (var elderly in elderlies)
            {
                elderly.Object.ID = elderly.Key;
                catlist.Add(elderly.Object);
            }
            return catlist;
        }

        public async Task<Categories> getaCat(string id)    //Gets a specific category - based on ID supplied
        {
            var cat = await firebaseClient
                        .Child("categories")
                        .Child(id)
                        .OnceSingleAsync<Categories>();
            return cat;
        }

        public async Task<List<Volunteer>> getVolunteer()  //Obtains list of volunteers
        {
            var volunteers = await firebaseClient
                        .Child("volunteer")
                        .OnceAsync<Volunteer>();
            List<Volunteer> volunteerlist = new List<Volunteer>();
            foreach (var volunteer in volunteers)
            {
                volunteer.Object.ID = volunteer.Key;
                volunteerlist.Add(volunteer.Object);
            }
            return volunteerlist;
        }

        public async Task<Volunteer> getAVolunteer(string id)
        {
            Volunteer volunteer = await firebaseClient
                        .Child("volunteer")
                        .Child(id)
                        .OnceSingleAsync<Volunteer>();
            volunteer.ID = id;
            return volunteer;
        }
        public async Task<Boolean> updateVolunteer(Volunteer vol)
        {
            await firebaseClient
                        .Child("volunteer")
                        .Child(vol.ID)
                        .PutAsync(vol);
            await PostLog("Updated Volunteer");
            return true;
        }
        public async Task<Boolean> updateVolunteerID(string vol, string zones) //Updates volunteer's assigned zones
        {
            await firebaseClient
                        .Child("volunteer")
                        .Child(vol)
                        .PatchAsync("{\"AssignedZones\":\"" + zones + "\"}");
            await PostLog("Updated Assigned zones for volunteer");
            return true;
        }

        public async Task<List<items>> getItems()
        {
            var items = await firebaseClient
                        .Child("items")
                        .OnceAsync<items>();
            List<items> itemlist = new List<items>(); //Turns all objects inside "requests" into Request objects
            foreach (var item in items)
            {
                item.Object.ID = item.Key;
                itemlist.Add(item.Object);
            }
            return itemlist;                                         //Returns the list of requests
        }
        public async Task<Boolean> asgnZone(Volunteer vol, string zones)
        {
            var items = await firebaseClient
                        .Child("volunteer")
                        .Child(vol.ID)
                        .OnceAsync<items>();
            IDictionary<string, items> itemlist = new Dictionary<string, items>();                        //Turns all objects inside "requests" into Request objects
            foreach (var item in items)
            {
                itemlist.Add(item.Key, item.Object);
            }
            return true;
        }

        public async Task<string> refreshToken(FirebaseAuthLink auth)                         //Function returns the authentication token
        {

            if (auth.IsExpired())                                   //Have not tested this part of the code yet as expiry takes 3600 seconds, no thanks
            {
                await auth.GetFreshAuthAsync();
            }
            return auth.FirebaseToken;
        }

        public async Task<Elderly> getAElderly(string id)
        {
            Elderly elderly = await firebaseClient
                        .Child("elderly")
                        .Child(id)
                        .OnceSingleAsync<Elderly>();
            elderly.ID = id;
            return elderly;
        }

        public async Task<bool> updateElderly(Elderly eld)     //This method POSTS data to the firebase
        {
            await firebaseClient
                .Child("elderly")
                .Child(eld.ID)
                .PutAsync(eld);
            await PostLog("Updated Elderly details");
            return true;
        }

        public async Task<bool> forgetPassword(string email)
        {
            var ap = new FirebaseAuthProvider(new Firebase.Auth.FirebaseConfig("AIzaSyBjdJIn1k3ksbbZAgY-kQIwUXbD0Zo_q8w"));
            
            FirebaseAuthLink res;
            try
            {
                await ap.SendPasswordResetEmailAsync(email);      //Attemps to create user with given email & password
            }
            catch
            {
                return false;
            }

            await firebaseClient
                .Child("authroles")
                .OnceAsync<Elderly>();
            await PostLog("An email have been successfully send!");
            return true;
        }

        public async Task<Boolean> PostLog(string action)
        {
            long time = DateTimeOffset.UtcNow.ToUnixTimeSeconds();  //Get current time in epochs
            TimeSpan span = DateTime.UtcNow.Date - new DateTime(1970, 1, 1);
            int dateE = (int)span.TotalSeconds;                     //Get current date in epochs
            await firebaseClient
                .Child("logs")
                .Child(dateE.ToString())
                .Child(time.ToString())
                .PutAsync("{\"adminID\":\""+AdminID+"\",\"action\":\""+action+"\"}");
            return true;
        }

        public async Task<List<LogDay>> getLog()
        {
            var Obj = await firebaseClient
                        .Child("logs")
                        .OnceAsync<IDictionary<string,IDictionary<string,object>>>();
            List<LogDay> dayList = new List<LogDay>();
            foreach (var date in Obj)
            {
                LogDay day = new LogDay();
                int dateE = Convert.ToInt32(date.Key);
                DateTime dateD = DateTimeOffset.FromUnixTimeSeconds(dateE).Date.ToLocalTime();
                day.Date = dateD;
                foreach (var time in date.Object)
                {
                    long timeE = Convert.ToInt32(time.Key); //Epoch time
                    DateTime timeD = DateTimeOffset.FromUnixTimeSeconds(timeE).DateTime.ToLocalTime();
                    string action = (string)time.Value["action"];
                    string ID = (string)time.Value["adminID"];
                    Log log = new Log(ID, action, timeD);
                    day.LogList.Add(log);                    
                }
                dayList.Add(day);
            }
            dayList = dayList.OrderByDescending(i => i.Date).ToList();
            return dayList;
        }
        public async Task<FirebaseClient> InitClientAsync(string auth)
        {
            var deserializedAuth = JsonConvert.DeserializeObject<FirebaseAuthLink>(auth);       //Deserializes the JSON into the token OBJECT
            AdminID = deserializedAuth.User.LocalId;
            firebaseClient = new FirebaseClient(
                                "https://kasei-bb0e0.firebaseio.com/",              //Sets the firebase project to use
                                new FirebaseOptions
                                {
                                    AuthTokenAsyncFactory = () => refreshToken(deserializedAuth)    //Sets the authentication token for the client.
                                });
            var role = await firebaseClient                                 //Obtains all data from (DATABASE)/Requests
                        .Child("authroles")
                        .Child(deserializedAuth.User.LocalId)
                        .OnceSingleAsync<string>();
            if (role != "admin")
            {
                throw new Exception("Not an admin");
            }
            return firebaseClient;
        }
    }
}
