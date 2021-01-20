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
                res = await ap.CreateUserWithEmailAndPasswordAsync(email, password);      //Attemps to create user with given email & password
            }
            catch
            {

                return false;
            }
            Elderly elderly = new Elderly(name,gender,email,address,postalcode,specialneeds,zone.Name, zone.REGION_C); //Puts elderly in firebase
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
                .PostAsync("{\"category\":\""+category+"\",\"icon\":\""+icon+"\"}");    //POST in JSON format - { "category" : "CATEGORYNAME", "icon": "ICON" }
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
        public async Task<bool> postVolunteer(string name, string contact,  string postalcode, int completedrequest, string auth)
        {
            FirebaseClient firebaseClient = await InitClientAsync(auth);
            Volunteer volunteer = new Volunteer();
            volunteer.Name = name;
            volunteer.Contact = Convert.ToInt32(contact);
            volunteer.PostalCode = postalcode;
            volunteer.CompletedRequests = Convert.ToInt32(completedrequest);
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
            return reqlist;                                                    //Returns the list of requests
        }

        public async Task<List<UserRequests>> getuserrequests(string auth)//Method populates UserRequests, which shows Users' IDs, their requests,
        {                                                                 //Their adddress, popstalcode,itemlist.
            FirebaseClient firebaseClient = await InitClientAsync(auth);
            var userRequestsData = await firebaseClient                   //Gets all user requests under /userRequests    
                        .Child("userRequests")
                        .OnceAsync<IDictionary<string,string>>();
            List<UserRequests> usersReqList = new List<UserRequests>();
            var fbItemList = await firebaseClient                       //Obtains ALL possible items orderable from the FB
                                    .Child("items")
                                    .OnceAsync<items>();
            List<items> itemList = new List<items>();                   //Populate an Itemlist
            foreach (var item in fbItemList)
            {
                items itemActual = item.Object;
                itemActual.ID = item.Key;
                itemList.Add(itemActual);
            }
            foreach (var userID in userRequestsData)                //Loop through all user requests to populate it with useful data
            {
                List<items> userItemList = new List<items>();
                UserRequests requests = new UserRequests();         //Get the details of the elderly who posted the request
                Elderly user = await firebaseClient                 //This is so that we can get useful details such as Zone_ID, etc.
                                .Child("elderly")
                                .Child(userID.Key)
                                .OnceSingleAsync<Elderly>();
                requests.User = user;                               //Sets the owner of the request to the owner object

                List<Request> userReqList = new List<Request>();    //A user may have multiple requests in one time - We create a list of requests for the user
                foreach (var reqid in userID.Object)                //Foreach request in UserRequest (If you're confused about this refer to the firebase - under /userRequests/ users can have a lot of requests)
                {
                    Request req = await firebaseClient              //Obtains the full request data based on the request ID - We do this so we can access the content of the request
                        .Child("requests")                          //NOTE - USING THIS IN A FOR LOOP MAY BE A BAD IDEA
                        .Child(reqid.Value)                         //MIGHT BE BANDWIDTH INTENSIVE - IT'S SENDING LIKE 5 GETS FROM THE FIREBASE - ALSO MIGHT BE WHY THE MAP PAGE & REQUEST PAGE TAKE LONG TO LOAD
                        .OnceSingleAsync<Request>();
                    foreach (var item in req.Contents)              //Foreach item in the content of the request
                    {
                        items itemF = userItemList.Find(x => x.ID == item.Key);     //Looks for the item in the user item list
                        if (itemF != null)
                        {
                            itemF.Requested += item.Value;                         //If it's found, it increases the "Requested" value of the item
                        }
                        else
                        {
                            items itemActual = itemList.Find(x => x.ID == item.Key);    //If it isn't, it looks for the item in the pre-populated general item list
                            itemActual.Requested = item.Value;                          //The items' requested count is set to the value
                            userItemList.Add(itemActual);                               //The item is added to the user item list
                        }
                    }
                    userReqList.Add(req);                         //Adds the request to the User Request list - This is just to have a count of the requests
                }
                requests.Requests = userReqList;                  //The specific request will have a request list because users may make more than one request at once
                requests.itemlist = userItemList;                 //The request item list is set - This will show the consolidated amount of items required
                usersReqList.Add(requests);                       //Adds the request to the overall request list
            }
            return usersReqList;                                //Return the request list
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

        public async Task<Categories> getaCat(string auth,string id)    //Gets a specific category - based on ID supplied
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

        public async Task<Volunteer> getAVolunteer(string auth,string id)
        {
            FirebaseClient firebaseClient = await InitClientAsync(auth);
            Volunteer volunteer = await firebaseClient
                        .Child("volunteer")
                        .Child(id)
                        .OnceSingleAsync<Volunteer>();
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

        public async Task<IDictionary<string, items>> getitems(string auth) 
        {
            FirebaseClient firebaseClient = await InitClientAsync(auth);    
            var items = await firebaseClient                                
                        .Child("items")
                        .OnceAsync<items>();
            IDictionary<string, items> itemlist = new Dictionary<string, items>(); //Turns all objects inside "requests" into Request objects
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
    }
}
