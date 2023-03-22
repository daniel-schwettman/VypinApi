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
using VypinApi.Models;

namespace VypinApi.Controllers
{
	[RoutePrefix("api/Settings")]
	public class SettingsController : ApiController
	{
		[Route("AddTagType")]
		[HttpGet]
		[HttpPost]
		public async Task<HttpResponseMessage> AddTagType()
		{
			//Trace.TraceInformation("Beginning VypinApi.Controllers.SettingsController.AddTagType");

			HttpResponseMessage responseMessage = new HttpResponseMessage();

			try
			{
				string request = await Request.Content.ReadAsStringAsync();
				TagType tagType = JsonConvert.DeserializeObject<TagType>(request, new JsonSerializerSettings
				{
					MissingMemberHandling = MissingMemberHandling.Ignore
				});

				DataAccessManager dataAccessManager = new DataAccessManager();
				string response = dataAccessManager.AddTagType(tagType);

				string jsonResponse = JsonConvert.SerializeObject(response);
				responseMessage.Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json");

			}
			catch (Exception e)
			{
				//Trace.TraceError(String.Format("AddTagType Exception: {0} ", e.ToString()));
			}

			//Trace.TraceInformation("Ending VypinApi.Controllers.SettingsController.AddTagType");

			return responseMessage;
		}

		[Route("AddTagCategory")]
		[HttpGet]
		[HttpPost]
		public async Task<HttpResponseMessage> AddTagCategory()
		{
			//Trace.TraceInformation("Beginning VypinApi.Controllers.SettingsController.AddTagCategory");

			HttpResponseMessage responseMessage = new HttpResponseMessage();

			try
			{
				string request = await Request.Content.ReadAsStringAsync();
				TagCategory tagCategory = JsonConvert.DeserializeObject<TagCategory>(request, new JsonSerializerSettings
				{
					MissingMemberHandling = MissingMemberHandling.Ignore
				});

				DataAccessManager dataAccessManager = new DataAccessManager();
				string response = dataAccessManager.AddTagCategory(tagCategory);

				string jsonResponse = JsonConvert.SerializeObject(response);
				responseMessage.Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json");

			}
			catch (Exception e)
			{
				//Trace.TraceError(String.Format("AddTagType Exception: {0} ", e.ToString()));
			}

			//Trace.TraceInformation("Ending VypinApi.Controllers.SettingsController.AddTagCategory");

			return responseMessage;
		}

		[Route("GetTagTypes")]
		[HttpGet]
		public HttpResponseMessage GetTagTypes()
		{
			//Trace.TraceInformation("Beginning VypinApi.Controllers.SettingsController.GetTagTypes");

			HttpResponseMessage responseMessage = new HttpResponseMessage();

			try
			{
				DataAccessManager dataAccessManager = new DataAccessManager();
				List<TagType> tags = dataAccessManager.GetTagTypes();

				//Trace.TraceInformation("GetTagTypes Found" + tags.Count);

				string jsonResponse = JsonConvert.SerializeObject(tags);
				responseMessage.Content = new StringContent(jsonResponse, Encoding.ASCII, "application/json");

			}
			catch (Exception e)
			{
				//Trace.TraceError(String.Format("GetTagTypes Exception: {0} ", e.ToString()));
			}

			return responseMessage;
		}

		[Route("GetTagCategories")]
		[HttpGet]
		public HttpResponseMessage GetTagCategories()
		{
			//Trace.TraceInformation("Beginning VypinApi.Controllers.SettingsController.GetTagCategories");

			HttpResponseMessage responseMessage = new HttpResponseMessage();

			try
			{
				DataAccessManager dataAccessManager = new DataAccessManager();
				List<TagCategory> tags = dataAccessManager.GetTagCategories();

				//Trace.TraceInformation("GetTagCategories Found" + tags.Count);

				string jsonResponse = JsonConvert.SerializeObject(tags);
				responseMessage.Content = new StringContent(jsonResponse, Encoding.ASCII, "application/json");

			}
			catch (Exception e)
			{
				//Trace.TraceError(String.Format("GetTagCategories Exception: {0} ", e.ToString()));
			}

			return responseMessage;
		}

		[Route("DeleteTagType")]
		[HttpDelete]
		public async Task<HttpResponseMessage> DeleteTagType()
		{
			//Trace.TraceInformation("Beginning VypinApi.Controllers.SettingsController.DeleteTagType");

			HttpResponseMessage responseMessage = new HttpResponseMessage();

			try
			{
				string tagTypeToDelete = await Request.Content.ReadAsStringAsync();

				DataAccessManager dataAccessManager = new DataAccessManager();
				string response = dataAccessManager.DeleteTagType(tagTypeToDelete);
			}
			catch (Exception e)
			{
				//Trace.TraceError(String.Format("DeleteTagType Exception: {0} ", e.ToString()));
			}

			return responseMessage;
		}

		[Route("DeleteTagCategory")]
		[HttpDelete]
		public async Task<HttpResponseMessage> DeleteTagCategory()
		{
			//Trace.TraceInformation("Beginning VypinApi.Controllers.SettingsController.DeleteTagCategory");

			HttpResponseMessage responseMessage = new HttpResponseMessage();

			try
			{
				string tagCategoryToDelete = await Request.Content.ReadAsStringAsync();

				DataAccessManager dataAccessManager = new DataAccessManager();
				string response = dataAccessManager.DeleteTagCategory(tagCategoryToDelete);
			}
			catch (Exception e)
			{
				//Trace.TraceError(String.Format("DeleteTagCategory Exception: {0} ", e.ToString()));
			}

			return responseMessage;
		}
	}
}