using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    [RoutePrefix("api/Asset")]
    public class AssetController : ApiController
    {
        [Route("Tag")]
        [HttpGet]
        [HttpPost]
        public HttpResponseMessage Tag(string tagRequest)
        {
			Trace.TraceInformation("Beginning VypinApi.Controllers.AssetControllers.TagRequest");

			HttpResponseMessage responseMessage = new HttpResponseMessage();

            try
            {
                DataAccessManager dataAccessManager = new DataAccessManager();

				TagInfoRequest tagInfoRequest = new TagInfoRequest() { TagId = tagRequest, TagName = tagRequest};
                Trace.TraceInformation($"TagRequest: tagId: {tagInfoRequest.TagId}, tagName:{tagInfoRequest.TagName}");

                List<TagEntity> tags = dataAccessManager.GetTags(tagInfoRequest);

                List<TagResult> tagResults = new List<TagResult>();
                Trace.TraceInformation($"Number of Tags Found {tags.Count}");
                foreach (TagEntity tag in tags)
                {
                    tagResults.Add(new TagResult
                    {
                        Id = tag.RawId,
                        Name = tag.Name,
                        Latitude = tag.Latitude,
                        Longitude = tag.Longitude,
                        ReaderId = tag.ReaderId,
                        AssociatedAssets = dataAccessManager.GetAssociatedAssetsByTagId(tag.Id),
                        SequenceNumber = tag.SequenceNumber
                    });
                }

				//Trace.TraceInformation($"Number of Tags Found {tagResults.Count}");

				TagResponse tagResponse = new TagResponse()
                {
                    TagResults = tagResults
                };

                string jsonResponse = JsonConvert.SerializeObject(tagResponse);
                responseMessage.Content = new StringContent(jsonResponse, Encoding.ASCII, "application/json");

            }
            catch (Exception e)
            {
                Trace.TraceError(String.Format("Tag Exception: {0} ", e.ToString()));
            }

			//Trace.TraceInformation("Ending VypinApi.Controllers.AssetControllers.TagRequest");

			return responseMessage;
        }

        [Route("TensarReportTags")]
        [HttpGet]
        [HttpPost]
        public HttpResponseMessage GetTensarReportTags()
        {
            Trace.TraceInformation("Beginning VypinApi.Controllers.AssetControllers.GetTensarReportTags");

            HttpResponseMessage responseMessage = new HttpResponseMessage();

            try
            {
                DataAccessManager dataAccessManager = new DataAccessManager();
                List<TagEntity> tags = dataAccessManager.GetTensarReportTags();

                List<TagResult> tagResults = new List<TagResult>();

                foreach (TagEntity tag in tags)
                {
                    tagResults.Add(new TagResult
                    {
                        Id = tag.RawId,
                        Name = tag.Name,
                        LastUpdatedOnServer = tag.LastUpdatedOn,
                        ReceviedByGatewayOn = tag.ReceivedOn,
                        Latitude = tag.Latitude,
                        Longitude = tag.Longitude,
                        Category = tag.Category,
                        AssignedType = tag.Type,
                        Rssi = tag.Rssi,
                        MZone1Rssi = tag.mZone1Rssi,
                        MZone1 = tag.MicroZoneCurrent,
                        MZone2 = tag.MicroZonePrevious,
                        ReaderId = tag.ReaderId,
                        Battery = tag.Battery,
                        StatusCode = tag.StatusCode,
                        SequenceNumber = tag.SequenceNumber
                    });
                }

                Trace.TraceInformation($"Number of Tags Found {tagResults.Count}");

                TagResponse tagResponse = new TagResponse()
                {
                    TagResults = tagResults
                };

                string jsonResponse = JsonConvert.SerializeObject(tagResponse);
                responseMessage.Content = new StringContent(jsonResponse, Encoding.ASCII, "application/json");
            }
            catch (Exception e)
            {
                Trace.TraceError(String.Format("Tensar Report Tag Exception: {0} ", e.ToString()));
            }
            return responseMessage;
        }

        [Route("AllTags")]
        [HttpGet]
        [HttpPost]
        public HttpResponseMessage AllTags()
        {
			Trace.TraceInformation("Beginning VypinApi.Controllers.AssetControllers.AllTags");

            HttpResponseMessage responseMessage = new HttpResponseMessage();

            try
            {
                DataAccessManager dataAccessManager = new DataAccessManager();
                List<TagEntity> tags = dataAccessManager.GetTags(null);
                List<TagResult> tagResults = new List<TagResult>();

                foreach (TagEntity tag in tags)
                {
                    tagResults.Add(new TagResult
                    {
                        Id = tag.RawId,
                        Name = tag.Name,
                        LastUpdatedOnServer = tag.LastUpdatedOn,
                        ReceviedByGatewayOn = tag.ReceivedOn,
                        Latitude = tag.Latitude,
                        Longitude = tag.Longitude,
                        Category = tag.Category,
                        AssignedType = tag.Type,
                        Rssi = tag.Rssi,
                        MZone1Rssi = tag.mZone1Rssi,
                        MZone1 = tag.MicroZoneCurrent,
                        MZone2 = tag.MicroZonePrevious,
                        ReaderId = tag.ReaderId,
                        Battery = tag.Battery,
                        StatusCode = tag.StatusCode,
                        TagType = tag.TagType,
                        SequenceNumber = tag.SequenceNumber,
                        AssociatedAssets = dataAccessManager.GetAssociatedAssetsByTagId(tag.Id)
                    });

                }

				//Trace.TraceInformation($"Number of Tags Found {tagResults.Count}");

				TagResponse tagResponse = new TagResponse()
                {
                    TagResults = tagResults
                };

                string jsonResponse = JsonConvert.SerializeObject(tagResponse);
                responseMessage.Content = new StringContent(jsonResponse, Encoding.ASCII, "application/json");
            }
            catch (Exception e)
            {
                Trace.TraceError($"AllTags Exception: {e.ToString()}");
            }

			//Trace.TraceInformation("Ending VypinApi.Controllers.AssetControllers.AllTags");

			return responseMessage;
        }

        [Route("AllVisualizerTags")]
        [HttpGet]
        [HttpPost]
        public HttpResponseMessage AllVisualizerTags()
        {
            HttpResponseMessage responseMessage = new HttpResponseMessage();

            try
            {
                DataAccessManager dataAccessManager = new DataAccessManager();
                List<TagEntity> tags = dataAccessManager.GetTags(null);
                List<TagResult> tagResults = new List<TagResult>();

                foreach (TagEntity tag in tags)
                {
                    tagResults.Add(new TagResult
                    {
                        Id = tag.RawId,
                        Name = tag.Name,
                        LastUpdatedOnServer = tag.LastUpdatedOn,
                        ReceviedByGatewayOn = tag.ReceivedOn,
                        Latitude = tag.Latitude,
                        Longitude = tag.Longitude,
                        Category = tag.Category,
                        AssignedType = tag.Type,
                        Rssi = tag.Rssi,
                        MZone1Rssi = tag.mZone1Rssi,
                        MZone1 = tag.MicroZoneCurrent,
                        MZone2 = tag.MicroZonePrevious,
                        ReaderId = tag.ReaderId,
                        Battery = tag.Battery,
                        StatusCode = tag.StatusCode,
                        TagType = tag.TagType,
                        SequenceNumber = tag.SequenceNumber,
                        AssociatedAssets = dataAccessManager.GetAssociatedAssetsByTagId(tag.Id),
                        DatabaseId = tag.Id,
                        Temperature = tag.Temperature,
                        Humidity = tag.Humidity
                    });

                }

                //Trace.TraceInformation($"Number of Tags Found {tagResults.Count}");

                TagResponse tagResponse = new TagResponse()
                {
                    TagResults = tagResults
                };

                string jsonResponse = JsonConvert.SerializeObject(tagResponse);
                responseMessage.Content = new StringContent(jsonResponse, Encoding.ASCII, "application/json");
            }
            catch (Exception e)
            {
                Trace.TraceError($"AllVisualizerTags Exception: {e.ToString()}");
            }

            //Trace.TraceInformation("Ending VypinApi.Controllers.AssetControllers.AllTags");

            return responseMessage;
        }

        [Route("TagHistory")]
        [HttpGet]
        [HttpPost]
        public HttpResponseMessage TagHistory()
        {
            //Trace.TraceInformation("Beginning VypinApi.Controllers.AssetControllers.TagHistory");

            HttpResponseMessage responseMessage = new HttpResponseMessage();

            try
            {
                DataAccessManager dataAccessManager = new DataAccessManager();
                List<TagHistory> tags = dataAccessManager.GetTagHistory();

                List<TagResult> tagResults = new List<TagResult>();

                foreach (TagHistory tag in tags)
                {
                    tagResults.Add(new TagResult
                    {
                        Id = tag.RawId,
                        Name = tag.Name,
                        LastUpdatedOnServer = tag.LastUpdatedOn,
                        ReceviedByGatewayOn = tag.ReceivedOn,
                        Latitude = tag.Latitude,
                        Longitude = tag.Longitude,
                        Category = tag.Category,
                        AssignedType = tag.Type,
                        Rssi = tag.Rssi,
                        MZone1Rssi = tag.mZone1Rssi,
                        MZone1 = tag.MicroZoneCurrent,
                        MZone2 = tag.MicroZonePrevious,
                        ReaderId = tag.ReaderId,
                        Battery = tag.Battery,
                        StatusCode = tag.StatusCode,
                        TagType = tag.TagType,
                        SequenceNumber = tag.SequenceNumber,
                        Operation = tag.Operation,
                        BeaconCount = tag.BeaconCount,
                        Temperature = tag.Temperature,
                        Humidity = tag.Humidity
                    });
                }

                //Trace.TraceInformation($"Number of Tags Found {tagResults.Count}");

                TagResponse tagResponse = new TagResponse()
                {
                    TagResults = tagResults
                };

                string jsonResponse = JsonConvert.SerializeObject(tagResponse);
                responseMessage.Content = new StringContent(jsonResponse, Encoding.ASCII, "application/json");
            }
            catch (Exception e)
            {
                Trace.TraceError($"AllTags Exception: {e.ToString()}");
            }

            //Trace.TraceInformation("Ending VypinApi.Controllers.AssetControllers.TagHistory");

            return responseMessage;
        }

        [Route("GetSelectedTagHistory")]
        [HttpGet]
        [HttpPost]
        public HttpResponseMessage SelectedTagHistory()
        {
            //Trace.TraceInformation("Beginning VypinApi.Controllers.AssetControllers.TagHistory");

            HttpResponseMessage responseMessage = new HttpResponseMessage();

            try
            {
                DataAccessManager dataAccessManager = new DataAccessManager();
                List<TagHistory> tags = dataAccessManager.GetSelectedTagHistory();

                List<TagResult> tagResults = new List<TagResult>();

                foreach (TagHistory tag in tags)
                {
                    tagResults.Add(new TagResult
                    {
                        Id = tag.RawId,
                        Name = tag.Name,
                        LastUpdatedOnServer = tag.LastUpdatedOn,
                        ReceviedByGatewayOn = tag.ReceivedOn,
                        Latitude = tag.Latitude,
                        Longitude = tag.Longitude,
                        Category = tag.Category,
                        AssignedType = tag.Type,
                        Rssi = tag.Rssi,
                        MZone1Rssi = tag.mZone1Rssi,
                        MZone1 = tag.MicroZoneCurrent,
                        MZone2 = tag.MicroZonePrevious,
                        ReaderId = tag.ReaderId,
                        Battery = tag.Battery,
                        StatusCode = tag.StatusCode,
                        TagType = tag.TagType,
                        SequenceNumber = tag.SequenceNumber,
                        Operation = tag.Operation,
                        BeaconCount = tag.BeaconCount
                    });
                }

                //Trace.TraceInformation($"Number of Tags Found {tagResults.Count}");

                TagResponse tagResponse = new TagResponse()
                {
                    TagResults = tagResults
                };

                string jsonResponse = JsonConvert.SerializeObject(tagResponse);
                responseMessage.Content = new StringContent(jsonResponse, Encoding.ASCII, "application/json");
            }
            catch (Exception e)
            {
                Trace.TraceError($"AllTags Exception: {e.ToString()}");
            }

            //Trace.TraceInformation("Ending VypinApi.Controllers.AssetControllers.TagHistory");

            return responseMessage;
        }

        [Route("AllCarts")]
        [HttpGet]
        [HttpPost]
        public HttpResponseMessage AllCarts()
        {
            Trace.TraceInformation("Beginning VypinApi.Controllers.AssetControllers.AllCarts");

            HttpResponseMessage responseMessage = new HttpResponseMessage();

            try
            {
                DataAccessManager dataAccessManager = new DataAccessManager();
                List<CartEntity> carts = dataAccessManager.GetCarts();

                List<CartResult> cartResults = new List<CartResult>();

                foreach (CartEntity cart in carts)
                {
                    cartResults.Add(new CartResult
                    {
                        TagId = cart.TagId,
                        CartNumber = cart.CartNumber
                    });
                }

                Trace.TraceInformation($"Number of Tags Found {cartResults.Count}");

                CartResponse cartResponse = new CartResponse()
                {
                    CartResults = cartResults
                };

                string jsonResponse = JsonConvert.SerializeObject(cartResponse);
                responseMessage.Content = new StringContent(jsonResponse, Encoding.ASCII, "application/json");

            }
            catch (Exception e)
            {
                Trace.TraceError($"AllTags Exception: {e.ToString()}");
            }

            Trace.TraceInformation("Ending VypinApi.Controllers.AssetControllers.AllTags");

            return responseMessage;
        }

        [Route("AllMicroZones")]
        [HttpGet]
        [HttpPost]
        public HttpResponseMessage AllMicroZones()
        {
           // Trace.TraceInformation("Beginning VypinApi.Controllers.AssetControllers.AllMicroZones");

            HttpResponseMessage responseMessage = new HttpResponseMessage();

            try
            {
                DataAccessManager dataAccessManager = new DataAccessManager();
                List<MicroZoneEntity> mZones = dataAccessManager.GetMicroZones();

                List<MicroZoneResult> mZoneResults = new List<MicroZoneResult>();

                foreach (MicroZoneEntity mZone in mZones)
                {
                    mZoneResults.Add(new MicroZoneResult
                    {
                        MicroZoneId = mZone.MicroZoneId,
                        RawId = mZone.RawId,
                        MicroZoneName = mZone.MicroZoneName,
                        MicroZoneNumber = mZone.MicroZoneNumber,
                        TagAssociationNumber = mZone.TagAssociationNumber,
                        DepartmentId = mZone.DepartmentId,
                        MicroZoneX = mZone.MicroZoneX,
                        MicroZoneY = mZone.MicroZoneY,
                        MicroZoneHeight = mZone.MicroZoneHeight,
                        MicroZoneWidth = mZone.MicroZoneWidth,
                        IsLocked = mZone.IsLocked
                    });
                }

                //Trace.TraceInformation($"Number of Tags Found {mZoneResults.Count}");

                MicroZoneResponse mZoneResponse = new MicroZoneResponse()
                {
                    MicroZoneResults = mZoneResults
                };

                string jsonResponse = JsonConvert.SerializeObject(mZoneResponse);
                responseMessage.Content = new StringContent(jsonResponse, Encoding.ASCII, "application/json");

            }
            catch (Exception e)
            {
                Trace.TraceError($"AllTags Exception: {e.ToString()}");
            }

            //TraceInformation("Ending VypinApi.Controllers.AssetControllers.AllTags");

            return responseMessage;
        }

        [Route("AllDepartments")]
        [HttpGet]
        [HttpPost]
        public HttpResponseMessage AllDepartments()
        {
            Trace.TraceInformation("Beginning VypinApi.Controllers.AssetControllers.AllDepartments");

            HttpResponseMessage responseMessage = new HttpResponseMessage();

            try
            {
                DataAccessManager dataAccessManager = new DataAccessManager();
                List<DepartmentEntity> depts = dataAccessManager.GetDepartments();

                List<DepartmentResult> deptResults = new List<DepartmentResult>();

                foreach (DepartmentEntity deptEntity in depts)
                {
                    deptResults.Add(new DepartmentResult
                    {
                        DepartmentId = deptEntity.DepartmentId,
                        Name = deptEntity.Name,
                        FilePath = deptEntity.FilePath,
                        IsLastLoaded = deptEntity.IsLastLoaded,
                        ScreenWidth = deptEntity.ScreenWidth,
                        ScreenHeight = deptEntity.ScreenHeight
                    });
                }

                Trace.TraceInformation($"Number of Departments Found {deptResults.Count}");

                DepartmentResponse deptResponse = new DepartmentResponse()
                {
                    DepartmentResults = deptResults
                };

                string jsonResponse = JsonConvert.SerializeObject(deptResponse);
                Trace.TraceInformation($"Departments JSON: {jsonResponse}");
                responseMessage.Content = new StringContent(jsonResponse, Encoding.ASCII, "application/json");

            }
            catch (Exception e)
            {
                Trace.TraceError($"AllDepartments Exception: {e.ToString()}");
            }

            //TraceInformation("Ending VypinApi.Controllers.AssetControllers.AllDepartments");

            return responseMessage;
        }

        [Route("AllAudits")]
        [HttpGet]
        [HttpPost]
        public HttpResponseMessage AllAudits()
        {
            Trace.TraceInformation("Beginning VypinApi.Controllers.AssetControllers.AllAudits");

            HttpResponseMessage responseMessage = new HttpResponseMessage();

            try
            {
                DataAccessManager dataAccessManager = new DataAccessManager();
                List<TagAudit> audits = dataAccessManager.GetAudits();

                List<TagAuditResult> auditResults = new List<TagAuditResult>();

                foreach (TagAudit audit in audits)
                {
                    auditResults.Add(new TagAuditResult
                    {
                        Id = audit.Id,
                        CartId = audit.CartId,
                        CurrentMicroZone = audit.CurrentMicroZone,
                        PreviousMicroZone = audit.PreviousMicroZone,
                        MicroZoneName = audit.MicroZoneName,
                        MicroZoneNumber = audit.MicroZoneNumber,
                        LastUpdatedOn = audit.LastUpdatedOn,
                        Rssi = audit.Rssi 
                    });
                }

                Trace.TraceInformation($"Number of Tags Found {auditResults.Count}");

                TagAuditResponse auditResponse = new TagAuditResponse()
                {
                    TagAuditResults = auditResults
                };

                string jsonResponse = JsonConvert.SerializeObject(auditResponse);
                responseMessage.Content = new StringContent(jsonResponse, Encoding.ASCII, "application/json");

            }
            catch (Exception e)
            {
                Trace.TraceError($"AllAudits Exception: {e.ToString()}");
            }

            Trace.TraceInformation("Ending VypinApi.Controllers.AssetControllers.AllAudits");

            return responseMessage;
        }

        [Route("DeleteAsset")]
        [HttpGet]
        [HttpPost]
        public async Task<HttpResponseMessage> DeleteAsset()
        {
            HttpResponseMessage responseMessage = new HttpResponseMessage();
            string assetEvent = await Request.Content.ReadAsStringAsync();

            try
            {
                System.Diagnostics.Trace.TraceInformation($"Deserializing assetEvent {assetEvent}");
                List<Asset> assetEventRequest = JsonConvert.DeserializeObject<List<Asset>>(assetEvent, new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Ignore
                });

                //string id = "";
                //string tagName = "";
                //string type = "";
                //string category = "";

                //if (tagFields != null)
                //{
                //    id = tagFields[0];
                //    tagName = tagFields[1];
                //    type = tagFields[2];
                //    category = tagFields[3];
                //}


                DataAccessManager dataAccessManager = new DataAccessManager();
                string response = dataAccessManager.DeleteAssets(assetEventRequest);

                TagEventResponse tagEventResponse = new TagEventResponse()
                {
                    status = response,
                };

                string jsonResponse = JsonConvert.SerializeObject(tagEventResponse);
                responseMessage.Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json");

            }
            catch (Exception e)
            {
                Trace.TraceError(String.Format("DeleteAsset Exception: {0}", e.ToString()));
            }

            return responseMessage;
        }

        [Route("EditAsset")]
        [HttpGet]
        [HttpPost]
        public async Task<HttpResponseMessage> EditAsset()
        {
            HttpResponseMessage responseMessage = new HttpResponseMessage();
            string assetEvent = await Request.Content.ReadAsStringAsync();

            try
            {
                System.Diagnostics.Trace.TraceInformation($"Deserializing assetEvent {assetEvent}");
                Asset assetEventRequest = JsonConvert.DeserializeObject<Asset>(assetEvent, new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Ignore
                });

                //string id = "";
                //string tagName = "";
                //string type = "";
                //string category = "";

                //if (tagFields != null)
                //{
                //    id = tagFields[0];
                //    tagName = tagFields[1];
                //    type = tagFields[2];
                //    category = tagFields[3];
                //}


                DataAccessManager dataAccessManager = new DataAccessManager();
                dataAccessManager.EditAssets(assetEventRequest);

                List<Asset> associatedAssets = dataAccessManager.GetAssociatedAssetsByTagId(assetEventRequest.TagId);
                
                List<AssetResult> assetResults = new List<AssetResult>();

                foreach (Asset asset in associatedAssets)
                {
                    assetResults.Add(new AssetResult
                    {
                        TagId= asset.TagId,
                        AssetId= asset.AssetId,
                        SlotId= asset.SlotId,
                        AssetIdentifier= asset.AssetIdentifier,
                        IsActive= asset.IsActive,
                        Name= asset.Name
                    });
                }

                Trace.TraceInformation($"Number of Associated Assets Found {assetResults.Count}");

                AssetResponse assetResponse = new AssetResponse()
                {
                    AssetResults = assetResults,
                };


                string jsonResponse = JsonConvert.SerializeObject(assetResponse);
                responseMessage.Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json");

            }
            catch (Exception e)
            {
                Trace.TraceError(String.Format("EditAsset Exception: {0}", e.ToString()));
            }

            return responseMessage;
        }

        [Route("AddOrEditMicroZone")]
        [HttpGet]
        [HttpPost]
        public async Task<HttpResponseMessage> AddOrEditMicroZone()
        {
            HttpResponseMessage responseMessage = new HttpResponseMessage();
            string microZoneEvent = await Request.Content.ReadAsStringAsync();

            try
            {
                System.Diagnostics.Trace.TraceInformation($"Deserializing microZoneEvent {microZoneEvent}");
                MicroZoneEntity mZoneEventRequest = JsonConvert.DeserializeObject<MicroZoneEntity>(microZoneEvent, new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Ignore
                });

                DataAccessManager dataAccessManager = new DataAccessManager();
                string response = dataAccessManager.AddOrEditMicroZone(mZoneEventRequest);

                TagEventResponse tagEventResponse = new TagEventResponse()
                {
                    status = response,
                };

                string jsonResponse = JsonConvert.SerializeObject(tagEventResponse);
                responseMessage.Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json");

            }
            catch (Exception e)
            {
                Trace.TraceError(String.Format("EditMicroZone Exception: {0}", e.ToString()));
            }

            return responseMessage;
        }

        [Route("DeleteMicroZone")]
        [HttpGet]
        [HttpPost]
        public async Task<HttpResponseMessage> DeleteMicroZone()
        {
            HttpResponseMessage responseMessage = new HttpResponseMessage();
            string mZoneEvent = await Request.Content.ReadAsStringAsync();

            try
            {
                System.Diagnostics.Trace.TraceInformation($"Deserializing mZoneEvent {mZoneEvent}");
                MicroZoneEntity mZoneEventRequest = JsonConvert.DeserializeObject<MicroZoneEntity>(mZoneEvent, new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Ignore
                });

                DataAccessManager dataAccessManager = new DataAccessManager();
                string response = dataAccessManager.DeleteMicroZone(mZoneEventRequest);

                TagEventResponse tagEventResponse = new TagEventResponse()
                {
                    status = response,
                };

                string jsonResponse = JsonConvert.SerializeObject(tagEventResponse);
                responseMessage.Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json");

            }
            catch (Exception e)
            {
                Trace.TraceError(String.Format("DeleteMicroZone Exception: {0}", e.ToString()));
            }

            return responseMessage;
        }

        [Route("EditDepartment")]
        [HttpGet]
        [HttpPost]
        public async Task<HttpResponseMessage> EditDepartment()
        {
            HttpResponseMessage responseMessage = new HttpResponseMessage();
            string deptEvent = await Request.Content.ReadAsStringAsync();

            try
            {
                System.Diagnostics.Trace.TraceInformation($"Deserializing deptEvent {deptEvent}");
                DepartmentEntity deptEventRequest = JsonConvert.DeserializeObject<DepartmentEntity>(deptEvent, new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Ignore
                });

                DataAccessManager dataAccessManager = new DataAccessManager();
                string response = dataAccessManager.EditDepartment(deptEventRequest);

                TagEventResponse tagEventResponse = new TagEventResponse()
                {
                    status = response,
                };

                string jsonResponse = JsonConvert.SerializeObject(tagEventResponse);
                responseMessage.Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json");

            }
            catch (Exception e)
            {
                Trace.TraceError(String.Format("EditDepartment Exception: {0}", e.ToString()));
            }

            return responseMessage;
        }

        [Route("AddAsset")]
        [HttpGet]
        [HttpPost]
        public async Task<HttpResponseMessage> AddAsset()
        {
            HttpResponseMessage responseMessage = new HttpResponseMessage();
            string assetEvent = await Request.Content.ReadAsStringAsync();

            try
            {
                System.Diagnostics.Trace.TraceInformation($"Deserializing assetEvent {assetEvent}");
                Asset assetEventRequest = JsonConvert.DeserializeObject<Asset>(assetEvent, new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Ignore
                });

                DataAccessManager dataAccessManager = new DataAccessManager();
                string response = dataAccessManager.AddAsset(assetEventRequest);

                TagEventResponse tagEventResponse = new TagEventResponse()
                {
                    status = response,
                };

                string jsonResponse = JsonConvert.SerializeObject(tagEventResponse);
                responseMessage.Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json");

            }
            catch (Exception e)
            {
                Trace.TraceError(String.Format("AddAsset Exception: {0}", e.ToString()));
            }

            return responseMessage;
        }

        [Route("AddDepartment")]
        [HttpGet]
        [HttpPost]
        public async Task<HttpResponseMessage> AddDepartment()
        {
            HttpResponseMessage responseMessage = new HttpResponseMessage();
            string departmentEvent = await Request.Content.ReadAsStringAsync();

            try
            {
                System.Diagnostics.Trace.TraceInformation($"Deserializing departmentEvent {departmentEvent}");
                DepartmentEntity departmentEventRequest = JsonConvert.DeserializeObject<DepartmentEntity>(departmentEvent, new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Ignore
                });

                DataAccessManager dataAccessManager = new DataAccessManager();
                string response = dataAccessManager.AddDepartment(departmentEventRequest);

                TagEventResponse tagEventResponse = new TagEventResponse()
                {
                    status = response,
                };

                string jsonResponse = JsonConvert.SerializeObject(tagEventResponse);
                responseMessage.Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json");

            }
            catch (Exception e)
            {
                Trace.TraceError(String.Format("AddDepartment Exception: {0}", e.ToString()));
            }

            return responseMessage;
        }

        [Route("DeleteTag")]
        [HttpGet]
        [HttpPost]
        public HttpResponseMessage DeleteTag(string[] tagToDelete)
        {
            HttpResponseMessage responseMessage = new HttpResponseMessage();

            string id = "";

            if (tagToDelete != null)
            {
                id = tagToDelete[0];
            }

            Trace.TraceError("Delete Tag" + id);
            try
            {
                DataAccessManager dataAccessManager = new DataAccessManager();
                string response = dataAccessManager.DeleteTag(id);

                TagEventResponse tagEventResponse = new TagEventResponse()
                {
                    status = response,
                };

                string jsonResponse = JsonConvert.SerializeObject(tagEventResponse);
                responseMessage.Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json");
            }
            catch (Exception e)
            {
                Trace.TraceError(String.Format("Tag Exception: {0} ", e.ToString()));
            }

            return responseMessage;
        }


        //[Route("AddTag")]
        //[HttpGet]
        //[HttpPost]
        //public HttpResponseMessage AddTag(string[] tagRequest)
        //{
            //HttpResponseMessage responseMessage = new HttpResponseMessage();

            //string id = "";
            //string tagName = "";
            //string type = "";
            //string category = "";

            //if (tagRequest != null)
            //{
            //    id = tagRequest[0];
            //    tagName = tagRequest[1];
            //    type = tagRequest[2];
            //    category = tagRequest[3];
            //}
            //try
            //{
            //    DataAccessManager dataAccessManager = new DataAccessManager();
            //    string response = dataAccessManager.EditTag(id, tagName, type, category);

            //    TagEventResponse tagEventResponse = new TagEventResponse()
            //    {
            //        status = response,
            //    };

            //    string jsonResponse = JsonConvert.SerializeObject(tagEventResponse);
            //    responseMessage.Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json");

            //}
            //catch (Exception e)
            //{
            //    Trace.TraceError(String.Format("AddTag Exception: {0}", e.ToString()));
            //}

            //return responseMessage;
       // }
    }
}
