using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json;
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
                res = await ap.CreateUserWithEmailAndPasswordAsync(email, password);      //Attemps to create user
            }
            catch
            {

                return false;
            }
            Elderly elderly = new Elderly(name,gender,email,address,postalcode,specialneeds,zone);                                    //Creates a elderly 
            await firebaseClient                                    //Posts the request object to under (DATABASE)/Requests
                    .Child("elderly")
                    .Child(res.User.LocalId)
                    .PutAsync(elderly);
            await firebaseClient
                .Child("authroles")
                .PatchAsync("{\"" + res.User.LocalId + "\":\"elderly\"}");
            return true;
        }
        public async Task<bool> AddCat(string auth, string category, string icon)
        {
            FirebaseClient firebaseClient = await InitClientAsync(auth);
            var volreq = await firebaseClient
                .Child("categories")
                .PostAsync("{\"category\":\""+category+"\",\"icon\":\""+icon+"\"}");
            return true;
        }
        public async Task<bool> AddItemtoCat(string auth, string id, string categoryid)
        {
            FirebaseClient firebaseClient = await InitClientAsync(auth);
            var item = await firebaseClient
                .Child("categories")
                .Child(categoryid)
                .Child("items")
                .PostAsync("{'"+categoryid + "':'" + categoryid+ "'}");
            return true;
        }
        public async Task<bool> AddItem(string auth, items item)
        {
            FirebaseClient firebaseClient = await InitClientAsync(auth);
            var itemp = await firebaseClient
                .Child("items")
                .PostAsync(item);
            return true;
        }
        public async Task<bool> postVolunteer(string name, string contact, string attendance, string status, string auth)
        {
            FirebaseClient firebaseClient = await InitClientAsync(auth);
            Volunteer volunteer = new Volunteer();
            volunteer.Name = name;
            volunteer.Contact = contact;
            volunteer.Attendance = attendance;
            volunteer.Status = status;
            var volreq = await firebaseClient
                .Child("volunteer")
                .PostAsync(volunteer);
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
            return reqlist;                                                 //Returns the list of requests
        }

        public async Task<List<UserRequests>> getuserrequests(string auth)//Method populates UserRequests, which shows Users' IDs, their requests,
        {                                                                 //Their adddress, popstalcode,itemlist.
            FirebaseClient firebaseClient = await InitClientAsync(auth);
            var userRequestsData = await firebaseClient                   //Gets all user requests under /userRequests    
                        .Child("userRequests")
                        .OnceAsync<IDictionary<string,string>>();
            List<Subzone> zoneList = new List<Subzone>();
            List<UserRequests> usersReqList = new List<UserRequests>();
            var fbItemList = await firebaseClient
                                    .Child("items")
                                    .OnceAsync<items>();
            List<items> itemList = new List<items>();
            foreach (var item in fbItemList)
            {
                items itemActual = item.Object;
                itemActual.ID = item.Key;
                itemList.Add(itemActual);
            }
            foreach (var userID in userRequestsData)
            {
                List<items> userItemList = new List<items>();
                UserRequests requests = new UserRequests();

                Elderly user = await firebaseClient
                                .Child("elderly")
                                .Child(userID.Key)
                                .OnceSingleAsync<Elderly>();
                requests.User = user;

                List<Request> userReqList = new List<Request>();
                foreach (var reqid in userID.Object)
                {
                    Request req = await firebaseClient
                        .Child("requests")
                        .Child(reqid.Value)
                        .OnceSingleAsync<Request>();
                    foreach (var item in req.Contents)
                    {
                        items itemF = userItemList.Find(x => x.ID == item.Key);
                        if (itemF != null)
                        {
                            itemF.Requested += item.Value;
                        }
                        else
                        {
                            items itemActual = itemList.Find(x => x.ID == item.Key);
                            itemActual.Requested = item.Value;
                            userItemList.Add(itemActual);
                        }
                    }
                    userReqList.Add(req);
                }
                requests.Requests = userReqList;
                requests.itemlist = userItemList;
                usersReqList.Add(requests);
                zoneList.Add(user.Zone);
            }
            return usersReqList;
        }

        public async Task<List<Elderly>> getElderly(string auth)              
        {
            FirebaseClient firebaseClient = await InitClientAsync(auth);      
            var elderlies = await firebaseClient                              
                        .Child("elderly")
                        .OnceAsync<Elderly>();
            List<Elderly> elderlylist = new List<Elderly>();                  
            foreach (var elderly in elderlies)
            {
                elderlylist.Add(elderly.Object);
            }
            return elderlylist;                                               
        }

        public async Task<List<Categories>> getCat(string auth)
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

        public async Task<Categories> getaCat(string auth,string id)
        {
            FirebaseClient firebaseClient = await InitClientAsync(auth);
            var cat = await firebaseClient
                        .Child("categories")
                        .Child(id)
                        .OnceSingleAsync<Categories>();
            return cat;
        }

        public async Task<bool> putIinC(string auth, string cat, string id)
        {
            FirebaseClient firebaseClient = await InitClientAsync(auth);
            await firebaseClient
                        .Child("categories")
                        .Child(cat)
                        .Child("items")
                        .PatchAsync("{\"" + id + "\":\"" + id + "\"}");
            return true;
        }


        public async Task<List<Volunteer>> getVolunteer(string auth)            
        {
            FirebaseClient firebaseClient = await InitClientAsync(auth);        
            var volunteers = await firebaseClient                               
                        .Child("volunteer")
                        .OnceAsync<Volunteer>();
            List<Volunteer> volunteerlist = new List<Volunteer>();              
            foreach (var volunteer in volunteers)
            {
                volunteerlist.Add(volunteer.Object);
            }
            return volunteerlist;                                               
        }

        public async Task<IDictionary<string, items>> getitems(string auth) 
        {
            FirebaseClient firebaseClient = await InitClientAsync(auth);    
            var items = await firebaseClient                                
                        .Child("items")
                        .OnceAsync<items>();
            IDictionary<string, items> itemlist = new Dictionary<string, items>();                        //Turns all objects inside "requests" into Request objects
            foreach (var item in items)
            {
                itemlist.Add(item.Key, item.Object);
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
        public async Task<string> refreshToken(FirebaseAuthLink auth)                         //Function returns the authentication token
        {
            
            if (auth.IsExpired())                                   //Have not tested this part of the code yet as expiry takes 3600 seconds, no thanks
            {
                await auth.GetFreshAuthAsync();
            }
            return auth.FirebaseToken;
        }
    }
}
