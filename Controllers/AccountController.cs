//#define DEMO

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Newtonsoft.Json;
using VypinApi.DataAccessLayer;
using VypinApi.Models.Responses;
using VypinApi.Models;
using System.Threading.Tasks;
using VypinApi.Models.Requests;
using System.Diagnostics;

namespace VypinApi.Controllers
{
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        int saltLength = 10;
        int hashLength = 20;
        int iterations = 10;
        Helper helper = new Helper();

        [Route("Register")]
        [HttpPost]
        public async Task<HttpResponseMessage> Register()
        {
            //System.Diagnostics.Trace.TraceError("Received Register Request");

            HttpResponseMessage responseMessage = new HttpResponseMessage();
            Dictionary<string, Object> responseContent = new Dictionary<string, Object>();
			DataAccessManager dataAccessManager = new DataAccessManager();

			try
            {
#if !DEMO

				// Retrieve the posted username and password
				string originalRequest = await Request.Content.ReadAsStringAsync();
                Trace.TraceError($"Register originalRequest: {originalRequest}");
                AccountEvent accountEventRequest = JsonConvert.DeserializeObject<AccountEvent>(originalRequest, new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Ignore
                });
                String email = accountEventRequest.Username;
                String password = accountEventRequest.Password;

                Trace.TraceError($"Register email: {email}");
                Trace.TraceError($"Register password: {password}");
                //System.Diagnostics.Trace.TraceError("Starting Register() ");
                // If the account already exists then return a failure status
                if (dataAccessManager.GetAccount(email) != null) {
                    responseContent.Add("status", "Failure");
                    responseContent.Add("description", "Account already exists");
                    responseMessage.Content = new StringContent(JsonConvert.SerializeObject(responseContent), Encoding.ASCII, "application/json");
                    return responseMessage;
                }

                // Store new account and retrieve session 
                String salt = helper.generateSalt(saltLength);
                String hash = helper.generateHash(password, salt, iterations, hashLength);

                String sessionGuid = dataAccessManager.CreateAccount(email, salt, hash, iterations);
                if ((sessionGuid == null) || (sessionGuid.Length == 0))
                {
                    responseContent.Add("status", "Failure");
                    responseContent.Add("description", "Unknown error");
                    responseMessage.Content = new StringContent(JsonConvert.SerializeObject(responseContent), Encoding.ASCII, "application/json");
                    return responseMessage;
                }

                responseContent.Add("status", "Success");
                responseContent.Add("session", sessionGuid);

                // Assign new account to default group
                dataAccessManager.AssignAccountGroup(email, "Default");
#endif

#if DEMO
				string email = "Account1";
				string password = "Account1Password";

				String salt = helper.generateSalt(saltLength);
				String hash = helper.generateHash(password, salt, iterations, hashLength);

				String sessionGuid = dataAccessManager.CreateAccount(email, salt, hash, iterations);

				responseContent.Add("status", "Success");
				responseContent.Add("session", Guid.NewGuid());
#endif
				// TODO: Retrive all tags available to this account (this will be the tags assigned to the default group)
				// Find the tags that are associated with this account
				List<TagResult> tagResults = new List<TagResult>();
				List<TagEntity> tags = dataAccessManager.GetAccountTags("", dataAccessManager.GetAccount(email).AccountId);

				foreach (TagEntity tag in tags)
                {
                    tagResults.Add(new TagResult
                    {
                        Id = tag.RawId,
                        Name = tag.Name,
                        Latitude = tag.Latitude,
                        Longitude = tag.Longitude,
                        SequenceNumber = tag.SequenceNumber
                    });
                }

				responseContent.Add("tags", tagResults);
                
                string jsonResponse = JsonConvert.SerializeObject(responseContent);
                responseMessage.Content = new StringContent(jsonResponse, Encoding.ASCII, "application/json");
            }
            catch (Exception e)
            {
                //System.Diagnostics.Trace.TraceError(String.Format("Register Exception: {0} ", e.ToString()));

                responseContent.Add("status", "Failure");
                responseContent.Add("description", String.Format("Register Exception: {0} ", e.ToString()));
                responseMessage.Content = new StringContent(JsonConvert.SerializeObject(responseContent), Encoding.ASCII, "application/json");
                return responseMessage;
            }

            return responseMessage;
        }

        [Route("Signin")]
        [HttpGet]
        [HttpPost]
        public async Task<HttpResponseMessage> Signin()
        {
            System.Diagnostics.Trace.TraceError("Beginning Asset ");

            HttpResponseMessage responseMessage = new HttpResponseMessage();
            Dictionary<string, Object> responseContent = new Dictionary<string, Object>();

            try
            {
                // Get the posted username and password
                string originalRequest = await Request.Content.ReadAsStringAsync();
                AccountEvent accountEventRequest = JsonConvert.DeserializeObject<AccountEvent>(originalRequest, new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Ignore
                });
                String email = accountEventRequest.Username;
                String password = accountEventRequest.Password;

                System.Diagnostics.Trace.TraceError("Starting Signin() ");
                DataAccessManager dataAccessManager = new DataAccessManager();

                // If the account already exists then return a failure status
                Account existingAccount = dataAccessManager.GetAccount(email);
                if (existingAccount == null)
                {
                    responseContent.Add("status", "Failure");
                    responseContent.Add("description", "Account not found");
                    responseMessage.Content = new StringContent(JsonConvert.SerializeObject(responseContent), Encoding.ASCII, "application/json");
                    return responseMessage;
                }

                // Store new account and retrieve session 
                String salt = existingAccount.Salt;
                String hash = helper.generateHash(password, salt, iterations, hashLength);

                String sessionGuid = dataAccessManager.SignIn(
                    email,
                    hash, 
                    iterations
                );
                if((sessionGuid == null) || (sessionGuid.Length == 0))
                {
                    responseContent.Add("status", "Failure");
                    responseContent.Add("description", "Invalid username or password.");
                    responseMessage.Content = new StringContent(JsonConvert.SerializeObject(responseContent), Encoding.ASCII, "application/json");
                    return responseMessage;
                }

                responseContent.Add("status", "Success");
                responseContent.Add("session", sessionGuid);
                //responseContent.Add("email", email);
                //responseContent.Add("password", password);
                //responseContent.Add("salt", salt);
                //responseContent.Add("hash", hash);
                //responseContent.Add("account", existingAccount);
                //responseContent.Add("originalRequest", originalRequest);

                // Find the tags that are associated with this account
                List<TagResult> tagResults = new List<TagResult>();

                List<TagEntity> tags = dataAccessManager.GetAccountTags("", existingAccount.AccountId);
                foreach (TagEntity tag in tags)
                {
                    tagResults.Add(new TagResult
                    {
                        Id = tag.RawId,
                        Name = tag.Name,
                        Latitude = tag.Latitude,
                        Longitude = tag.Longitude,
                        SequenceNumber = tag.SequenceNumber
                    });
                }
                responseContent.Add("tags", tagResults);
                
                //responseContent.Add("tags", dataAccessManager.GetAccountTags("", existingAccount.AccountId));
                string jsonResponse = JsonConvert.SerializeObject(responseContent);
                responseMessage.Content = new StringContent(jsonResponse, Encoding.ASCII, "application/json");
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.TraceError(String.Format("Signin Exception: {0} ", e.ToString()));

                responseContent.Add("status", "Failure");
                responseContent.Add("description", String.Format("Signin Exception: {0} ", e.ToString()));
                responseMessage.Content = new StringContent(JsonConvert.SerializeObject(responseContent), Encoding.ASCII, "application/json");
                return responseMessage;
            }

            return responseMessage;
        }

        [Route("Signout")]
        [HttpGet]
        [HttpPost]
        public HttpResponseMessage Signout()
        {
            HttpResponseMessage responseMessage = new HttpResponseMessage();

            try
            {
                // TODO: clear active session for the selected account
            }
            catch (Exception e)
            {
                //System.Diagnostics.Trace.TraceError(String.Format("Tag Exception: {0} ", e.ToString()));
            }

            return responseMessage;
        }

        [Route("TestSessionId")]
        [HttpGet]
        [HttpPost]
        public async Task<HttpResponseMessage> TestSessionId()
        {
            HttpResponseMessage responseMessage = new HttpResponseMessage();
            Dictionary<string, Object> responseContent = new Dictionary<string, Object>();

            try
            {
                // Retrieve the posted username and password
                string originalRequest = await Request.Content.ReadAsStringAsync();

                //System.Diagnostics.Trace.TraceError("Starting TestSessionId() ");
                DataAccessManager dataAccessManager = new DataAccessManager();

                // TODO: Replace with getSession
                /*
                if (dataAccessManager.getAccount(email) != null)
                {
                    responseContent.Add("status", "Failure");
                    responseContent.Add("description", "Account already exists");
                    responseMessage.Content = new StringContent(JsonConvert.SerializeObject(responseContent), Encoding.ASCII, "application/json");
                    return responseMessage;
                }
                */
                responseContent.Add("status", "Success");

                string jsonResponse = JsonConvert.SerializeObject(responseContent);
                responseMessage.Content = new StringContent(jsonResponse, Encoding.ASCII, "application/json");
            }
            catch (Exception e)
            {
                //System.Diagnostics.Trace.TraceError(String.Format("Register Exception: {0} ", e.ToString()));

                responseContent.Add("status", "Failure");
                responseContent.Add("description", String.Format("Register Exception: {0} ", e.ToString()));
                responseMessage.Content = new StringContent(JsonConvert.SerializeObject(responseContent), Encoding.ASCII, "application/json");
                return responseMessage;
            }

            return responseMessage;
        }


        [Route("VerifySession")]
        [HttpGet]
        [HttpPost]
        public async Task<HttpResponseMessage> VerifySession(String sessionGuid)
        {
            HttpResponseMessage responseMessage = new HttpResponseMessage();
            Dictionary<string, Object> responseContent = new Dictionary<string, Object>();

            try
            {
                // Retrieve the posted username and password
                string originalRequest = await Request.Content.ReadAsStringAsync();

                //System.Diagnostics.Trace.TraceError("Starting TestSessionId() ");
                DataAccessManager dataAccessManager = new DataAccessManager();

                responseContent.Add("status", "Success");
                responseContent.Add("description", dataAccessManager.VerifySession(sessionGuid));

                string jsonResponse = JsonConvert.SerializeObject(responseContent);
                responseMessage.Content = new StringContent(jsonResponse, Encoding.ASCII, "application/json");
            }
            catch (Exception e)
            {
                //System.Diagnostics.Trace.TraceError(String.Format("Register Exception: {0} ", e.ToString()));

                responseContent.Add("status", "Failure");
                responseContent.Add("description", String.Format("Register Exception: {0} ", e.ToString()));
                responseMessage.Content = new StringContent(JsonConvert.SerializeObject(responseContent), Encoding.ASCII, "application/json");
                return responseMessage;
            }

            return responseMessage;
        }
    }
}
