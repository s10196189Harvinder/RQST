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

        public async Task<bool> postElderly(string name, char gender, string email, string password, string address, string postalcode, string auth)     //This method POSTS data to the firebase
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
            Elderly elderly = new Elderly();                                    //Creates a elderly 
            elderly.Name = name;
            elderly.Gender = gender;
            elderly.Email = email;
            elderly.Address = address;
            elderly.PostalCode = postalcode;
            await firebaseClient                                    //Posts the request object to under (DATABASE)/Requests
                    .Child("elderly")
                    .Child(res.User.LocalId)
                    .PutAsync(elderly);
            await firebaseClient
                .Child("authroles")
                .PatchAsync("{\"" + res.User.LocalId + "\":\"elderly\"}");
            return true;
        }

        public async Task<bool> postVolunteer(string name, string nric, string contact, string attendance, string status, string auth)
        {
            FirebaseClient firebaseClient = await InitClientAsync(auth);
            Volunteer volunteer = new Volunteer();
            volunteer.Name = name;
            volunteer.Nric = nric;
            volunteer.Contact = contact;
            volunteer.Attendance = attendance;
            volunteer.Status = status;
            var volreq = await firebaseClient
                .Child("Volunteer")
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
                foreach(var request1 in request.Requests)
                {
                    var arequest = await firebaseClient                                 //Obtains all data from (DATABASE)/Requests
                        .Child("requests")
                        .Child(request1)
                        .OnceSingleAsync<Request>();
                    foreach (var item in arequest.Contents.Keys)
                    {
                        var anItem = await firebaseClient                                 //Obtains all data from (DATABASE)/Requests
                        .Child("items")
                        .Child(item)
                        .OnceSingleAsync<items>();
                        request.itemlist.Add(anItem);
                    }
                }
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
                        .OnceSingleAsync<Elderly>();
                request.Address = userdata.Address;
                request.PostalCode = userdata.PostalCode;
            }
            return userRequests;                                                 //Returns the list of requests
        }
        public async Task<List<Elderly>> getElderly(string auth)               //This method obtains data from the firebase
        {
            FirebaseClient firebaseClient = await InitClientAsync(auth);        //Initialize firebase client
            var elderlies = await firebaseClient                                 //Obtains all data from (DATABASE)/Requests
                        .Child("Elderly")
                        .OnceAsync<Elderly>();
            List<Elderly> elderlylist = new List<Elderly>();                        //Turns all objects inside "requests" into Request objects
            foreach (var elderly in elderlies)
            {
                elderlylist.Add(elderly.Object);
            }
            return elderlylist;                                                 //Returns the list of requests
        }
<<<<<<< HEAD
=======

        public async Task<List<Volunteer>> getVolunteer(string auth)               //This method obtains data from the firebase
        {
            FirebaseClient firebaseClient = await InitClientAsync(auth);        //Initialize firebase client
            var volunteers = await firebaseClient                                 //Obtains all data from (DATABASE)/Requests
                        .Child("Volunteer")
                        .OnceAsync<Volunteer>();
            List<Volunteer> volunteerlist = new List<Volunteer>();                        //Turns all objects inside "requests" into Request objects
            foreach (var volunteer in volunteers)
            {
                volunteerlist.Add(volunteer.Object);
            }
            return volunteerlist;                                                 //Returns the list of requests
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
>>>>>>> 42ba965b906eebdf000471f0624faa4cb2646714

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
