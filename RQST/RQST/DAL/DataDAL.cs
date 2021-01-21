using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RQST.Models;

namespace RQST.DAL
{
    public class DataDAL
    {
        public async Task<bool> postElderly(string name, char gender, string email, string password, string address, string postalcode, string specialneeds, Subzone zone, string auth)     //This method POSTS data to the firebase
        {
            FirebaseClient firebaseClient = await InitClientAsync(auth);        //Initialize firebase client for posting
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
            Elderly elderly = new Elderly(name, gender, email, address, postalcode, specialneeds, zone.Name, zone.REGION_C); //Puts elderly in firebase
            await firebaseClient                //Posts the elderly object to under (DATABASE)/Requests/UserID
                    .Child("elderly")
                    .Child(res.User.LocalId)    //Sets the location of the data to be posted to the ID of the user
                    .PutAsync(elderly);         //PUTs location at /Eldelry/UserID/...  (Not POST beacuase POST generates random ID - it would become /Eldelry/UID/ID/...)
            await firebaseClient
                .Child("authroles")             // Places UserID in the authroles section
                .PatchAsync("{\"" + res.User.LocalId + "\":\"elderly\"}");  //Patching in JSON format - "USERID:elderly"
            return true;
        }
        public async Task<bool> AddCat(string auth, string category, string icon)   //Add Category to database - NOT IN USE CURRENTLY
        {
            FirebaseClient firebaseClient = await InitClientAsync(auth);
            var volreq = await firebaseClient
                .Child("categories")                                                    //Posted to /Categories/random ID/...
                .PostAsync("{\"category\":\"" + category + "\",\"icon\":\"" + icon + "\"}");    //POST in JSON format - { "category" : "CATEGORYNAME", "icon": "ICON" }
            return true;
        }
        public async Task<bool> AddItemtoCat(string auth, string id, string categoryid)   //Adds items to categories in the DB
        {
            FirebaseClient firebaseClient = await InitClientAsync(auth);
            var item = await firebaseClient     //POSTS to /categories/CATEGORYID/items/ITEMID/...
                .Child("categories")
                .Child(categoryid)
                .Child("items")
                .PostAsync(id);                 //POST to RANDOMID:ITEMID
            return true;
        }
        public async Task<bool> AddItem(string auth, items item)    //Adds item to DB
        {
            FirebaseClient firebaseClient = await InitClientAsync(auth);
            var itemp = await firebaseClient
                .Child("items")             //Posts to /items/RANDOMID/...
                .PostAsync(item);
            return true;
        }
        public async Task<bool> postVolunteer(string name, string email, string password, string contact, string postalcode, Subzone zone, string assignedzone, string auth)
        {
            FirebaseClient firebaseClient = await InitClientAsync(auth);        //Initialize firebase client for posting
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
            return true;
        }

        public async Task<List<Request>> getrequests(string auth)               //This method obtains data from the firebase
        {
            FirebaseClient firebaseClient = await InitClientAsync(auth);        //Initialize firebase client
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

        public async Task<List<Request_NEW>> getUserRequests(string auth)
        {
            FirebaseClient firebaseClient = await InitClientAsync(auth);
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


        public async Task<List<Request_NEW>> getUserRequestsMIN(string auth)
        {
            FirebaseClient firebaseClient = await InitClientAsync(auth);
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
                        items newerItem = new items(newItem.Name, kvp.Value, newItem.Icon, newItem.stock, newItem.BgCol);
                        newerItem.ID = kvp.Key;
                        currReq.addItem(newerItem);
                    }
                    req.ReqList.Add(currReq);
                    
                }
                reqList.Add(req);
            }
            return (reqList);
        }





        public async Task<List<Elderly>> getElderly(string auth)  //Obtains the elderly data
        {
            FirebaseClient firebaseClient = await InitClientAsync(auth);
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

        public async Task<List<Categories>> getCat(string auth) //Gets all the categories
        {
            FirebaseClient firebaseClient = await InitClientAsync(auth);
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

        public async Task<Categories> getaCat(string auth, string id)    //Gets a specific category - based on ID supplied
        {
            FirebaseClient firebaseClient = await InitClientAsync(auth);
            var cat = await firebaseClient
                        .Child("categories")
                        .Child(id)
                        .OnceSingleAsync<Categories>();
            return cat;
        }

        public async Task<List<Volunteer>> getVolunteer(string auth)  //Obtains list of volunteers
        {
            FirebaseClient firebaseClient = await InitClientAsync(auth);
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

        public async Task<Volunteer> getAVolunteer(string auth, string id)
        {
            FirebaseClient firebaseClient = await InitClientAsync(auth);
            Volunteer volunteer = await firebaseClient
                        .Child("volunteer")
                        .Child(id)
                        .OnceSingleAsync<Volunteer>();
            volunteer.ID = id;
            return volunteer;
        }
        public async Task<Boolean> updateVolunteer(string auth, Volunteer vol)
        {
            FirebaseClient firebaseClient = await InitClientAsync(auth);
            await firebaseClient
                        .Child("volunteer")
                        .Child(vol.ID)
                        .PutAsync(vol);     //Need to prevent it from posting ID, but otherwise works
            return true;
        }
        public async Task<Boolean> updateVolunteerID(string auth, string vol, string zones)
        {
            FirebaseClient firebaseClient = await InitClientAsync(auth);
            await firebaseClient
                        .Child("volunteer")
                        .Child(vol)
                        .PatchAsync("{\"AssignedZones\":\"" + zones + "\"}");     //Need to prevent it from posting ID, but otherwise works
            return true;
        }

        public async Task<List<items>> getItems(string auth)
        {
            FirebaseClient firebaseClient = await InitClientAsync(auth);
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

        public async Task<FirebaseClient> InitClientAsync(string auth)
        {
            var deserializedAuth = JsonConvert.DeserializeObject<FirebaseAuthLink>(auth);       //Deserializes the JSON into the token OBJECT
            var firebaseClient = new FirebaseClient(
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
        public async Task<Boolean> asgnZone(string auth, Volunteer vol, string zones)
        {
            FirebaseClient firebaseClient = await InitClientAsync(auth);
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

        public async Task<Elderly> getAElderly(string auth, string id)
        {
            FirebaseClient firebaseClient = await InitClientAsync(auth);
            Elderly elderly = await firebaseClient
                        .Child("elderly")
                        .Child(id)
                        .OnceSingleAsync<Elderly>();
            elderly.ID = id;
            return elderly;
        }

        public async Task<bool> updateElderly(string auth, Elderly eld)     //This method POSTS data to the firebase
        {
            FirebaseClient firebaseClient = await InitClientAsync(auth);
            await firebaseClient
                .Child("elderly")
                .Child(eld.ID)
                .PutAsync(eld);
            return true;
        }
    }
}
