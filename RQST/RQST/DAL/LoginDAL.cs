using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;

namespace RQST.DAL
{
    public class LoginDAL
    {
        public async Task<string> loginAsync(string email, string password)
        {
            var ap = new FirebaseAuthProvider(new Firebase.Auth.FirebaseConfig("AIzaSyBjdJIn1k3ksbbZAgY-kQIwUXbD0Zo_q8w"));
            var auth = await ap.SignInWithEmailAndPasswordAsync(email, password);
            return JsonConvert.SerializeObject(auth);
        }
        public async Task<string> getToken(string auth)
        {
            FirebaseAuthLink authentication = JsonConvert.DeserializeObject<FirebaseAuthLink>(auth);
            return authentication.FirebaseToken;
        }
    }
}
