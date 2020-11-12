using System;
using System.Collections.Generic;
using System.Linq;
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
        
        public async Task<bool> postdata(string name, string deliverable, string specialneeds, string address, string auth)
        {
            var firebaseClient = new FirebaseClient(
                                "https://ca2-qn1.firebaseio.com/",
                                new FirebaseOptions
                                {
                                    AuthTokenAsyncFactory = () => getToken(auth)
                                });
            var firebase = new FirebaseClient("https://kasei-bb0e0.firebaseio.com/");
            Request request = new Request();
            request.Name = name;
            request.Address = address;
            request.SpecialRequest = specialneeds;
            request.Deliverables = deliverable;
            var smth = await firebase
                    .Child("Requests")
                    .PostAsync(request);
            return true;

        }
        public async Task<List<Request>> getdata(string auth)
        {
            var firebaseClient = new FirebaseClient(
                                "https://ca2-qn1.firebaseio.com/",
                                new FirebaseOptions
                                {
                                    AuthTokenAsyncFactory = () => getToken(auth)
                                });
            var firebase = new FirebaseClient("https://kasei-bb0e0.firebaseio.com/");
            var requests = await firebase
                        .Child("Requests")
                        .OnceAsync<Request>();
            List<Request> reqlist = new List<Request>();
            foreach (var request in requests)
            {
                reqlist.Add(request.Object);
            }
            return reqlist;
        }


        public async Task<string> getToken(string auth)
        {
            FirebaseAuthLink authentication = JsonConvert.DeserializeObject<FirebaseAuthLink>(auth);
            return authentication.FirebaseToken;
        }
    }
}
