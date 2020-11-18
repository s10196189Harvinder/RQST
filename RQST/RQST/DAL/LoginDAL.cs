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
using System.Security.Cryptography.X509Certificates;

namespace RQST.DAL
{
    public class LoginDAL
    {
        public async Task<IDictionary<string,string>> loginAsync(string email, string password)
        {
            var ap = new FirebaseAuthProvider(new Firebase.Auth.FirebaseConfig("AIzaSyBjdJIn1k3ksbbZAgY-kQIwUXbD0Zo_q8w"));     //Initialized the authentication provider for firebase. string is the API key of the firebase project.
            IDictionary<string,string> response = new Dictionary<string, string>();                                 // Dictionary to store either the authnetication code OR the exception, when an error occurs.
            try
            {
                var auth = await ap.SignInWithEmailAndPasswordAsync(email, password);           //Attempts to sign in via the Authentication provider, with the credentials
                var serialauth = JsonConvert.SerializeObject(auth);                             //Once authentication is obtained from the sign in function, it is serialized as JSON

                var firebaseClient = new FirebaseClient(
                                    "https://kasei-bb0e0.firebaseio.com/",              //Sets the firebase project to use
                                    new FirebaseOptions
                                    {
                                        AuthTokenAsyncFactory = () => Task.FromResult(auth.FirebaseToken)    //Sets the authentication token for the client.
                                    });
                var role = await firebaseClient                                 //Obtains all data from (DATABASE)/Requests
                        .Child("authroles")
                        .Child(auth.User.LocalId)
                        .OnceSingleAsync<string>();
                if (role != "admin")
                {
                    response.Add("Exception", "Not an admin");
                    return response;
                }
                response.Add("Auth", serialauth);                                               //Adds the serialized JSON into the dictioniary
                return response;
            }
            catch (FirebaseAuthException ex)                                                    //In the case of exception, exceptions are stored as a "Exception" key in the dictionary
            {
                if (ex.Reason.ToString() == "WrongPassword")
                {
                    response.Add("Exception", "Wrong Password");                                //IF statements for common errors, general error message is shown otherwise
                    return response;
                }
                else if (ex.Reason.ToString() == "UnknownEmailAddress")
                {
                    response.Add("Exception", "Unknown Email");
                    return response;
                }
                else
                    response.Add("Exception", "An error has occurred.");
                return response;
            }
        }
        public async Task<string> resetpass(string email)
        {
            var ap = new FirebaseAuthProvider(new Firebase.Auth.FirebaseConfig("AIzaSyBjdJIn1k3ksbbZAgY-kQIwUXbD0Zo_q8w"));
            string result = "";
            try
            {
                await ap.SendPasswordResetEmailAsync(email);
                result = "Success";
            }
            catch(FirebaseAuthException ex)
            {
                if (ex.Reason.ToString()== "UnknownEmailAddress")
                {
                    result = "Invalid Email";
                }
                else
                {
                    result = "An unknown error has occured. Please try again later.";
                }

            }
            
            return result;
        }
    }
}
