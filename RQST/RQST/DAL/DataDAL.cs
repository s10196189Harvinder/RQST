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
        
        public async Task<bool> postdata(string name, string deliverable, string specialneeds, string address, string auth)     //This method POSTS data to the firebase
        {
            FirebaseClient firebaseClient = await InitClientAsync(auth);        //Initialize firebase client for posting
            Request request = new Request();                                    //Creates a request object (can be improved, too lazy)
            request.Name = name;
            request.Address = address;
            request.SpecialRequest = specialneeds;
            request.Deliverables = deliverable;
            var smth = await firebaseClient                                    //Posts the request object to under (DATABASE)/Requests
                    .Child("Requests")
                    .PostAsync(request);
            return true;

        }
        public async Task<List<Request>> getdata(string auth)               //This method obtains data from the firebase
        {
            FirebaseClient firebaseClient = await InitClientAsync(auth);        //Initialize firebase client
            var requests = await firebaseClient                                 //Obtains all data from (DATABASE)/Requests
                        .Child("Requests")
                        .OnceAsync<Request>();
            List<Request> reqlist = new List<Request>();                        //Turns all objects inside "requests" into Request objects
            foreach (var request in requests)
            {
                reqlist.Add(request.Object);
            }
            return reqlist;                                                 //Returns the list of requests
        }

        public async Task<FirebaseClient> InitClientAsync(string auth)
        {
            var firebaseClient = new FirebaseClient(
                                "https://kasei-bb0e0.firebaseio.com/",              //Sets the firebase project to use
                                new FirebaseOptions
                                {
                                    AuthTokenAsyncFactory = () => getToken(auth)    //Sets the authentication token for the client.
                                });
            return firebaseClient;
        }
        public async Task<string> getToken(string auth)                         //Function returns the authentication token
        {
            var deserializedAuth = JsonConvert.DeserializeObject<FirebaseAuthLink>(auth);       //Deserializes the JSON into the token OBJECT
            if (deserializedAuth.IsExpired())                                   //Have not tested this part of the code yet as expiry takes 3600 seconds, no thanks
            {
                await deserializedAuth.GetFreshAuthAsync();
            }
            return deserializedAuth.FirebaseToken;
        }
    }
}
