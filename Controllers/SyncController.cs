using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Windows.Forms;
using VypinApi.DataAccessLayer;
using VypinApi.Models.Requests;
using VypinApi.Models.Responses;

namespace VypinApi.Controllers
{
    [RoutePrefix("api/Sync")]
    public class SyncController : ApiController
    {
        [Route("TagEvent")]
        [HttpPost]
        public async Task<HttpResponseMessage> TagEvent()
        {
            System.Diagnostics.Trace.TraceError($"Beginning sync/TagEvent {Environment.NewLine}");

            HttpResponseMessage responseMessage = new HttpResponseMessage();

            string tagEvent = await Request.Content.ReadAsStringAsync();
            try
            {
                //System.Diagnostics.Trace.TraceInformation($"Attempting to Add Tag Event {tagEvent}");
                TagEvent tagEventRequest = JsonConvert.DeserializeObject<TagEvent>(tagEvent, new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Ignore
                });

                System.Diagnostics.Trace.TraceError($"Successfully Parsed TagEvent {tagEvent}");
                    if (tagEventRequest.Tags == null)
                    {
                        System.Diagnostics.Trace.TraceError("List of TagEvent Tags is Null");
                    }

                    if (tagEventRequest.Tags.Count == 0)
                    {
                        System.Diagnostics.Trace.TraceError("List of TagEvent Tags is empty");
                    }

                    DataAccessManager dataAccessManager = new DataAccessManager();
                    String response = "Session not found";
                    if (dataAccessManager.VerifySession(tagEventRequest.SessionId))
                    {
                        response = dataAccessManager.AddTag(tagEventRequest);
                    }

                    TagEventResponse tagEventResponse = new TagEventResponse()
                    {
                        status = response,
                    };

                    string jsonResponse = JsonConvert.SerializeObject(tagEventResponse);
                    responseMessage.Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json");
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.TraceError($"Error Message {e.StackTrace}, trying Legacy TagEvent");
                try
                {
                    LegacyTagEvent legacyTagEventRequest = JsonConvert.DeserializeObject<LegacyTagEvent>(tagEvent, new JsonSerializerSettings
                    {
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    });

                    //System.Diagnostics.Trace.TraceError($"Successfully Parsed TagEvent {tagEvent}");

                    if (legacyTagEventRequest.Tags == null)
                    {
                        System.Diagnostics.Trace.TraceError("List of Legacy TagEvent Tags is Null");
                    }

                    if (legacyTagEventRequest.Tags.Count == 0)
                    {
                        System.Diagnostics.Trace.TraceError("List of Legacy TagEvent Tags is empty");
                    }

                    DataAccessManager dataAccessManager = new DataAccessManager();
                    String response = "Session not found";
                    System.Diagnostics.Trace.TraceInformation($"Adding tags for Legacy TagEvent");

                    if (dataAccessManager.VerifySession(legacyTagEventRequest.SessionId))
                    {
                        response = dataAccessManager.AddLegacyTag(legacyTagEventRequest);
                    }

                    System.Diagnostics.Trace.TraceInformation($"Successfully Added Legacy TagEvent to DB {Environment.NewLine}");

                    TagEventResponse tagEventResponse = new TagEventResponse()
                    {
                        status = response,
                    };

                    string jsonResponse = JsonConvert.SerializeObject(tagEventResponse);
                    responseMessage.Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json");
                    //System.Diagnostics.Trace.TraceInformation("Successfully Serialized TagEvent Response");
                }
                catch(Exception error)
                {
                    System.Diagnostics.Trace.TraceError($"Legacy TagEvent Exception:{error.ToString()}");
                    responseMessage.Content = new StringContent(error.ToString());
                }
            }

            return responseMessage;
        }

        [Route("TagLocation")]
        [HttpGet]
        [HttpPost]
        public async Task<HttpResponseMessage> TagLocation()
        {
            HttpResponseMessage responseMessage = new HttpResponseMessage();

            try
            {
                string tagLocation = await Request.Content.ReadAsStringAsync();

                LegacyTagEvent tagEventRequest = JsonConvert.DeserializeObject<LegacyTagEvent>(tagLocation, new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Ignore
                });

                System.Diagnostics.Trace.TraceInformation($"Beginning VypinApi.Controllers.SyncController.TagLocation tags: {tagLocation}");

                DataAccessManager dataAccessManager = new DataAccessManager();
                String response = "Session not found";
                if (dataAccessManager.VerifySession(tagEventRequest.SessionId))
                {
                    response = dataAccessManager.ProcessTagLocation(tagEventRequest);
                }

                TagEventResponse tagEventResponse = new TagEventResponse()
                {
                    status = response,
                };
                System.Diagnostics.Trace.TraceInformation($"Beginning VypinApi.Controllers.SyncController.ProcessTagLocation response:{response}");

                string jsonResponse = JsonConvert.SerializeObject(tagEventResponse);
                responseMessage.Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json");

            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.TraceError(String.Format("TagEvent Exception: {0} ", e.ToString()));
            }

            System.Diagnostics.Trace.TraceInformation("Ending VypinApi.Controllers.SyncController.TagLocation");

            return responseMessage;
        }

        [Route("TagName")]
        [HttpGet]
        [HttpPost]
        public async Task<HttpResponseMessage> TagName()
        {
            HttpResponseMessage responseMessage = new HttpResponseMessage();

            try
            {
                string tagLocation = await Request.Content.ReadAsStringAsync();
                Trace.TraceError($"Register tag message: {tagLocation}");
                LegacyTagEvent tagEventRequest = JsonConvert.DeserializeObject<LegacyTagEvent>(tagLocation, new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Ignore
                });

                DataAccessManager dataAccessManager = new DataAccessManager();
                String response = "Session not found";
                if (dataAccessManager.VerifySession(tagEventRequest.SessionId))
                {
                    response = dataAccessManager.ProcessTagName(tagEventRequest);
                }
                Trace.TraceError($"Register tag response: {response}");
                TagEventResponse tagEventResponse = new TagEventResponse()
                {
                    status = response,
                };

                string jsonResponse = JsonConvert.SerializeObject(tagEventResponse);
                responseMessage.Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json");

            }
            catch (Exception e)
            {
                string message = e.Message;
                System.Diagnostics.Trace.TraceError(String.Format("TagEvent Exception: {0} ", e.ToString()));
            }

            return responseMessage;
        }

        [Route("DeleteTags")]
        [HttpDelete]
        public HttpResponseMessage DeleteTags()
        {
            HttpResponseMessage responseMessage = new HttpResponseMessage();

            try
            {
                DataAccessManager dataAccessManager = new DataAccessManager();
                string response = dataAccessManager.DeleteTags();

                TagEventResponse tagEventResponse = new TagEventResponse()
                {
                    status = response,
                };

                string jsonResponse = JsonConvert.SerializeObject(tagEventResponse);
                responseMessage.Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json");

            }
            catch (Exception e)
            {
                //System.Diagnostics.Trace.TraceError(String.Format("DeleteTags Exception: {0}", e.ToString()));
            }

            return responseMessage;
        }
    }
}
