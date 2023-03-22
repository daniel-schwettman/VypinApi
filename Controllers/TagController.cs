using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using VypinApi.DataAccessLayer;
using VypinApi.Models.Requests;
using VypinApi.Models.Responses;

namespace VypinApi.Controllers
{
    [RoutePrefix("api/Tag")]
    public class TagController : ApiController
    {
        [Route("TagRequest")]
        [HttpGet]
        [HttpPost]
        public async Task<HttpResponseMessage> TagRequest()
        {
            System.Diagnostics.Trace.TraceInformation("Beginning VypinApi.Controllers.TagController.TagRequest");

            HttpResponseMessage responseMessage = new HttpResponseMessage();

            try
            {
                string tagRequest = await Request.Content.ReadAsStringAsync();
                System.Diagnostics.Trace.TraceInformation(tagRequest.ToString());

                TagInfoRequest tagInfoRequest = new TagInfoRequest();
                if (tagRequest != null && tagRequest != "")
                {
                    System.Diagnostics.Trace.TraceInformation("Deserializing");

                    tagInfoRequest = JsonConvert.DeserializeObject<TagInfoRequest>(tagRequest, new JsonSerializerSettings
                    {
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    });
                    if(string.IsNullOrEmpty(tagInfoRequest.SearchText))
                        System.Diagnostics.Trace.TraceInformation("empty");
                }
                else
                {
                    tagInfoRequest.SearchText = "";

                    //System.Diagnostics.Trace.TraceInformation("Setting search text");
                }
                //System.Diagnostics.Trace.TraceInformation(tagInfoRequest.ToString());

                DataAccessManager dataAccessManager = new DataAccessManager();

                List<TagEntity> tags = dataAccessManager.GetTags(tagInfoRequest);
                System.Diagnostics.Trace.TraceInformation("Got tags");
                List<TagResult> tagResults = new List<TagResult>();

                List<Asset> associatedAssets = new List<Asset>();

                if (tags != null)
                {
                    foreach (TagEntity tag in tags)
                    {
                        //System.Diagnostics.Trace.TraceInformation(tags.Count.ToString());
                        if (tag == null)
                            System.Diagnostics.Trace.TraceInformation("null tag");
                        else
                        {
                            //if (tag.Id != null)
                            //System.Diagnostics.Trace.TraceInformation($"TagId: {tag.Id}, Lat:{tag.Latitude} ,Lon:{tag.Longitude}");

                            associatedAssets = dataAccessManager.GetAssociatedAssetsByTagId(tag.Id);

                            //System.Diagnostics.Trace.TraceInformation("Got tag id");
                            tagResults.Add(new TagResult
                            {
                                Id = tag.RawId,
                                Name = tag.Name,
                                Latitude = tag.Latitude,
                                Longitude = tag.Longitude,
                                LastUpdatedOnServer = tag.LastUpdatedOn,
                                ReceviedByGatewayOn = tag.ReceivedOn,
                                Rssi = tag.Rssi,
                                MZone1Rssi = tag.mZone1Rssi,
                                MZone1 = tag.MicroZoneCurrent,
                                MZone2 = tag.MicroZonePrevious,
                                Category = tag.Category,
                                AssignedType = tag.Type,
                                ReaderId = tag.ReaderId,
                                Battery = tag.Battery,
                                StatusCode = tag.StatusCode,
                                TagType = tag.TagType,
                                AssociatedAssets = associatedAssets,
                                SequenceNumber = tag.SequenceNumber,
                                Temperature = tag.Temperature,
                                Humidity = tag.Humidity
                            });
                        }
                    }
                }

                System.Diagnostics.Trace.TraceInformation($"Number of Tags Found {tagResults.Count}");

                TagResponse tagResponse = new TagResponse()
                {
                    TagResults = tagResults
                };

                string jsonResponse = JsonConvert.SerializeObject(tagResponse);
                responseMessage.Content = new StringContent(jsonResponse, Encoding.ASCII, "application/json");

                //dont clear assets list until we have written a response back with them
                associatedAssets.Clear();
                System.Diagnostics.Trace.TraceInformation($"done");

            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.TraceError(String.Format("Tag Exception: {0} ", e.ToString()));
            }
            return responseMessage;
        }
    }
}
