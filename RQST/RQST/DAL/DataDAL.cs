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
        
        //public async Task<bool> postdata(string name, string deliverable, string specialneeds, string address, string auth)     //This method POSTS data to the firebase
        //{
        //    FirebaseClient firebaseClient = await InitClientAsync(auth);        //Initialize firebase client for posting
        //    Request request = new Request();                                    //Creates a request object (can be improved, too lazy)
        //    request.Name = name;
        //    request.Address = address;
        //    request.SpecialRequest = specialneeds;
        //    request.Deliverables = deliverable;
        //    var smth = await firebaseClient                                    //Posts the request object to under (DATABASE)/Requests
        //            .Child("Requests")
        //            .PostAsync(request);
        //    return true;
        //}
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

        public async Task<List<UserRequests>> getuserrequests(string auth)               //This method obtains data from the firebase
        {
            FirebaseClient firebaseClient = await InitClientAsync(auth);        //Initialize firebase client
            var requests = await firebaseClient                                 //Obtains all data from (DATABASE)/Requests
                        .Child("userRequests")
                        .OnceAsync<IDictionary<string,string>>();
            List<UserRequests> userRequests = new List<UserRequests>();
            foreach (var usrrequest in requests)
            {
                UserRequests request = new UserRequests();
                request.Requests = usrrequest.Object.Values.ToList();
                request.UserID = usrrequest.Key;
                userRequests.Add(request);
            }
            return userRequests;                                                 //Returns the list of requests
        }

        public async Task<List<UserRequests>> getUserRequestsAdress(string auth, List<UserRequests> userRequests)               //This method obtains data from the firebase
        {
            FirebaseClient firebaseClient = await InitClientAsync(auth);        //Initialize firebase client
            foreach (UserRequests request in userRequests)
            {
                var userdata = await firebaseClient                                 //Obtains all data from (DATABASE)/Requests
                        .Child("elderly")
                        .Child(request.UserID)
                        .OnceSingleAsync<IDictionary<string, string>>();
                string address = "";
                string postalcode = "";
                userdata.TryGetValue("address", out address);
                userdata.TryGetValue("postalCode", out postalcode);
                request.Address = address;
                request.PostalCode = postalcode;
            }
            return userRequests;                                                 //Returns the list of requests
        }
















































        public async Task<IDictionary<string, items>> getitems(string auth)               //This method obtains data from the firebase
        {
            FirebaseClient firebaseClient = await InitClientAsync(auth);        //Initialize firebase client
            var items = await firebaseClient                                 //Obtains all data from (DATABASE)/Requests
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
