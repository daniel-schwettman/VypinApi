using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using VypinApi.Models;
using VypinApi.Models.Requests;
using VypinApi.Models.Responses;

namespace VypinApi.DataAccessLayer
{
    public class DataAccessManager
    {
        /**
         * Account
         */
        // Determine if the account exists
        public Account GetAccount(String email)
        {
            // Check if an account exists for the given email address
            try
            {
                using (var context = new AssetEntities())
                {
                    Account existingAccount = (from account in context.Accounts
                                               where account.Email == email
                                               select account).FirstOrDefault();

                    if (existingAccount != null)
                    {
                        return existingAccount;
                    }
                }

                return null;
            }
            catch (Exception e)
            {
                //Trace.TraceError("accountExists Exception" + e.ToString());
                return null;
            }
        }

        public String SignIn(String email, String hash, int iterations)
        {
            String sessionGuid = Guid.NewGuid().ToString();

            // Create account and return session guid
            try
            {
                using (var context = new AssetEntities())
                {
                    Account existingAccount = (from account in context.Accounts
                                               where account.Email == email
                                                    && account.Hash == hash
                                               select account).FirstOrDefault();

                    if (existingAccount != null)
                    {
                        existingAccount.Session = sessionGuid;
                        context.SaveChanges();
                        return existingAccount.Session;
                    }
                }

                return null;
            }
            catch (DbEntityValidationException dbEx)
            {
                return dbEx.ToString();
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        public String CreateAccount(String email, String salt, String hash, int iterations)
        {
            String sessionGuid = Guid.NewGuid().ToString();

            // Create account and return session guid
            //Trace.TraceInformation("VypinApi.DataAccessLayer.DataAccessManager: Beginning CreateAccount");

            try
            {
                using (var context = new AssetEntities())
                {
                    Account account = new Account
                    {
                        Email = email,
                        Salt = salt,
                        Hash = hash,
                        Iterations = iterations,
                        Session = sessionGuid,
                        StatusId = 1
                    };

                    context.Accounts.Add(account);
                    context.SaveChanges();

                    return sessionGuid;
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                Trace.TraceInformation($"VypinApi.DataAccessLayer.DataAccessManager: Database Exception {dbEx.InnerException}");
                return dbEx.ToString();
            }
            catch (Exception e)
            {
                Trace.TraceInformation($"VypinApi.DataAccessLayer.DataAccessManager: Exception {e.InnerException}");
                return e.ToString();
            }
        }

        public Boolean VerifySession(String sessionGuid)
        {
            // Create account and return session guid
            //Trace.TraceInformation("VypinApi.DataAccessLayer.DataAccessManager: Beginning VerifySession");

            return true;

            //try
            //{
            //	using (var context = new AssetEntities())
            //	{
            //		Account activeSession = (from account in context.Accounts
            //								 where account.Session == sessionGuid
            //								 select account).FirstOrDefault();

            //		if (activeSession != null)
            //		{
            //			return true;
            //		}
            //	}

            //	return false;
            //}
            //catch (DbEntityValidationException dbEx)
            //{
            //	return false;
            //	//return null;
            //}
            //catch (Exception e)
            //{
            //	return false;
            //	//return null;
            //}
        }

        public void AssignAccountGroup(String email, String group)
        {
            // TODO: Assign account to group
        }

        public List<TagEntity> GetTensarReportTags()
        {
            List<TagEntity> tags = new List<TagEntity>();

            try
            {
                using (var context = new AssetEntities())
                {
                    tags = context.TagEntities.ToList();
                }
            }
            catch (Exception e)
            {

            }
            return tags;
        }


        /**
         * Tag
         */
        public List<TagEntity> GetTags(TagInfoRequest tagInfoRequest)
        {
            // Trace.TraceInformation("VypinApi.DataAccessLayer.DataAccessManager: Beginning GetTags");

            List<TagEntity> tags = new List<TagEntity>();
            try
            {
                bool returnAllTags = false;
                string searchText = "";
                if (tagInfoRequest == null)
                {
                    //System.Diagnostics.Trace.TraceInformation("tagInfoRequest is null");
                    returnAllTags = true;
                }
                else if (tagInfoRequest.SearchText == null)
                {
                    if (tagInfoRequest.TagId != null)
                    {
                        searchText = tagInfoRequest.TagId;
                    }
                    else if (tagInfoRequest.TagName != null)
                    {
                        searchText = tagInfoRequest.TagName;
                    }
                    else
                    {
                        System.Diagnostics.Trace.TraceInformation("searchText is null");
                        returnAllTags = true;
                    }
                }
                else if (tagInfoRequest.SearchText == "" || string.IsNullOrEmpty(tagInfoRequest.SearchText))
                {
                    if (tagInfoRequest.TagId != null)
                    {
                        searchText = tagInfoRequest.TagId;
                    }
                    else if (tagInfoRequest.TagName != null)
                    {
                        searchText = tagInfoRequest.TagName;
                    }
                    else
                    {
                        System.Diagnostics.Trace.TraceInformation("searchText is null");
                        returnAllTags = true;
                    }
                }
                else
                {
                    searchText = tagInfoRequest.SearchText;
                    // System.Diagnostics.Trace.TraceInformation($"searchtext: {searchText}");
                }
                //System.Diagnostics.Trace.TraceInformation("checking return all tags or finding particular tag");
                using (var context = new AssetEntities())
                {
                    // With this search, we just return the entire tag data set
                    if (returnAllTags)
                    {
                        tags.AddRange(context.TagEntities.ToList());
                    }
                    else
                    {
                        TagEntity tag = null;
                        Asset asset = null;

                        //go get tag by Id
                        tag = GetTagById(context.TagEntities, searchText);

                        //if its null, try getting it by its RawId
                        if (tag == null)
                        {
                            tag = GetTagByRawId(context.TagEntities, searchText);
                        }

                        //if tag is still null, try getting it by name
                        if (tag == null)
                        {
                            tag = GetTagByName(context.TagEntities, searchText);
                        }

                        //if tag is still null, then search for searchTerm in asset table
                        if (tag == null)
                        {
                            asset = GetAssetByAssetId(context.Assets, searchText);
                        }

                        if (asset == null)
                        {
                            asset = GetAssetByName(context.Assets, searchText);
                        }

                        if (asset == null)
                        {
                            asset = GetAssetBySlotId(context.Assets, searchText);
                        }

                        if (asset == null)
                        {
                            asset = GetAssetByTagId(context.Assets, searchText);
                        }

                        if (asset == null)
                        {
                            asset = GetAssetByAssetIdentifier(context.Assets, searchText);
                        }

                        //if the asset isnt null after all these checks, we found an asset from their search term
                        //so now go find the associated tag and return that since it has the search asset in its associated asset list
                        if (asset != null)
                        {
                            tag = GetTagById(context.TagEntities, asset.TagId.ToString());
                        }

                        tags.Add(tag);
                    }
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("GetTags Exception" + e.ToString());
            }
            return tags;
        }

        public List<TagHistory> GetTagHistory()
        {
            List<TagHistory> tags = new List<TagHistory>();

            try
            {

                using (var context = new AssetEntities())
                {
                    tags.AddRange(context.TagHistories.ToList());
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("GetCarts Exception" + e.ToString());
            }

            //Trace.TraceInformation("VypinApi.DataAccessLayer.DataAccessManager: Ending GetTagHistory");
            return tags;
        }

        public List<TagHistory> GetSelectedTagHistory()
        {
            List<TagHistory> tags = new List<TagHistory>();

            try
            {

                using (var context = new AssetEntities())
                {
                    tags.AddRange(context.TagHistories.ToList());
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("GetCarts Exception" + e.ToString());
            }

            //Trace.TraceInformation("VypinApi.DataAccessLayer.DataAccessManager: Ending GetTagHistory");
            return tags;
        }

        public List<CartEntity> GetCarts()
        {
            List<CartEntity> carts = new List<CartEntity>();

            try
            {

                using (var context = new AssetEntities())
                {
                    carts.AddRange(context.CartEntities.ToList());
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("GetCarts Exception" + e.ToString());
            }

            Trace.TraceInformation("VypinApi.DataAccessLayer.DataAccessManager: Ending GetCarts");
            return carts;
        }

        public List<MicroZoneEntity> GetMicroZones()
        {
            List<MicroZoneEntity> mZones = new List<MicroZoneEntity>();

            try
            {

                using (var context = new AssetEntities())
                {
                    mZones.AddRange(context.MicroZoneEntities.ToList());
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("GetMicroZones Exception" + e.ToString());
            }

            //Trace.TraceInformation("VypinApi.DataAccessLayer.DataAccessManager: Ending GetMicroZones");
            return mZones;
        }

        public List<DepartmentEntity> GetDepartments()
        {
            List<DepartmentEntity> depts = new List<DepartmentEntity>();

            try
            {
                using (var context = new AssetEntities())
                {
                    depts.AddRange(context.DepartmentEntities.ToList());
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("GetDepartments Exception" + e.ToString());
            }

            Trace.TraceInformation("VypinApi.DataAccessLayer.DataAccessManager: Ending GetDepartments");
            return depts;
        }

        public List<TagAudit> GetAudits()
        {
            List<TagAudit> audits = new List<TagAudit>();

            try
            {

                using (var context = new AssetEntities())
                {
                    audits.AddRange(context.TagAudits.ToList());
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("GetAudits Exception" + e.ToString());
            }

            Trace.TraceInformation("VypinApi.DataAccessLayer.DataAccessManager: Ending GetAudits");
            return audits;
        }

        public string EditAssets(Asset asset)
        {
            string response = "";

            try
            {
                using (var context = new AssetEntities())
                {
                    Trace.TraceInformation($"VypinApi.DataAccessLayer.DataAccessManager: Looking for asset with id {asset.AssetId}");
                    Asset existingAsset = context.Assets.Where(assetItem => assetItem.AssetId == asset.AssetId).FirstOrDefault();

                    if (existingAsset != null)
                    {
                        existingAsset.Name = asset.Name;
                        existingAsset.IsActive = asset.IsActive;
                        existingAsset.AssetIdentifier = asset.AssetIdentifier;
                    }
                    else
                        response = $"Asset {asset.AssetId} does not exist";
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Edit Asset Exception" + e.Message);
                return "Failure";
            }

            return response;
        }

        public string AddOrEditMicroZone(MicroZoneEntity mZone)
        {
            string response = "";

            try
            {
                using (var context = new AssetEntities())
                {
                    MicroZoneEntity existingZone = context.MicroZoneEntities.Where(mzoneItem => mzoneItem.RawId == mZone.RawId).FirstOrDefault();

                    if (existingZone != null)
                    {
                        existingZone.MicroZoneName= mZone.MicroZoneName;
                        existingZone.MicroZoneX = mZone.MicroZoneX;
                        existingZone.MicroZoneY = mZone.MicroZoneY;
                        existingZone.MicroZoneHeight = mZone.MicroZoneHeight;
                        existingZone.MicroZoneWidth = mZone.MicroZoneWidth;
                        existingZone.IsLocked = mZone.IsLocked;
                        response = $"Mzone {existingZone.MicroZoneName} updated.";
                    }
                    else
                    {
                        response = $"Mzone {mZone.RawId} does not exist, creating one.";
                        MicroZoneEntity newEntity = new MicroZoneEntity()
                        {
                             MicroZoneName = mZone.MicroZoneName,
                             MicroZoneX = mZone.MicroZoneX,
                             MicroZoneY = mZone.MicroZoneY,
                             MicroZoneHeight = mZone.MicroZoneHeight,
                             MicroZoneWidth = mZone.MicroZoneWidth,
                             DepartmentId = mZone.DepartmentId,
                             RawId = mZone.RawId,
                             TagAssociationNumber = mZone.TagAssociationNumber,
                             IsLocked = mZone.IsLocked
                        };

                        context.MicroZoneEntities.Add(newEntity);
                    }

                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Edit Mzone Exception" + e.Message);
                return "Failure";
            }

            return response;
        }

        public string EditDepartment(DepartmentEntity dept)
        {
            string response = "";

            try
            {
                using (var context = new AssetEntities())
                {
                    DepartmentEntity existingDepartment = context.DepartmentEntities.Where(x => x.DepartmentId == dept.DepartmentId).FirstOrDefault();

                    if (existingDepartment != null)
                    {
                        List<DepartmentEntity> existingDepts = context.DepartmentEntities.ToList();

                        foreach (DepartmentEntity existingDept in existingDepts)
                        {
                            if (existingDept.DepartmentId == dept.DepartmentId)
                            {
                                existingDept.Name = dept.Name;
                                existingDept.IsLastLoaded = dept.IsLastLoaded;
                                existingDepartment.FilePath = dept.FilePath;
                                existingDepartment.ScreenHeight = dept.ScreenHeight;
                                existingDepartment.ScreenWidth = dept.ScreenWidth;
                            }
                            else
                            {
                                existingDept.IsLastLoaded = !dept.IsLastLoaded;
                                existingDepartment.Name = existingDepartment.Name;
                                existingDepartment.FilePath = existingDepartment.FilePath;
                                existingDepartment.ScreenHeight = existingDepartment.ScreenHeight;
                                existingDepartment.ScreenWidth = existingDepartment.ScreenWidth;
                            }
                        }
                    }


                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Edit Department Exception" + e.Message);
                return "Failure";
            }

            return response;
        }

        public string AddAsset(Asset asset)
        {
            string response = "";

            try
            {
                using (var context = new AssetEntities())
                {
                    context.Assets.Add(asset);
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Add Asset Exception" + e.Message);
                return "Failure";
            }

            return response;
        }

        public string AddDepartment(DepartmentEntity department)
        {
            string response = "";
            try
            {
                using (var context = new AssetEntities())
                {
                    context.DepartmentEntities.Add(department);
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Add Department Exception" + e.Message);
                return "Failure";
            }

            return response;
        }

        public string DeleteAssets(List<Asset> assets)
        {
            string response = "";

            try
            {
                using (var context = new AssetEntities())
                {
                    foreach (Asset result in assets)
                    {
                        Trace.TraceInformation($"Looking for asset with assetId {result.AssetId}");
                        Asset existingAsset = context.Assets.Where(assetItem => assetItem.AssetId == result.AssetId).FirstOrDefault();

                        if (existingAsset != null)
                        {
                            Trace.TraceInformation($"Found asset, removing.");
                            context.Assets.Remove(existingAsset);
                        }
                        else
                        {
                            Trace.TraceInformation($"Found asset, does not exist.");
                            response = $"Asset {result.AssetId} does not exist";
                        }
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Delete Asset Exception" + e.Message);
                return "Failure";
            }

            return response;
        }

        public string DeleteMicroZone(MicroZoneEntity mZone)
        {
            string response = "";

            try
            {
                using (var context = new AssetEntities())
                {
                    Trace.TraceInformation($"Looking for mZone with id {mZone.RawId}");
                    MicroZoneEntity existingmZone = context.MicroZoneEntities.Where(mZoneItem => mZoneItem.RawId == mZone.RawId).FirstOrDefault();

                        if (existingmZone != null)
                        {
                            Trace.TraceInformation($"Found microZone, removing.");
                            context.MicroZoneEntities.Remove(existingmZone);
                        }
                        else
                        {
                            Trace.TraceInformation($"MicroZone does not exist.");
                            response = $"MicroZone w/ id {mZone.RawId} does not exist";
                        }
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Delete MicroZone Exception" + e.Message);
                return "Failure";
            }

            return response;
        }

        public string AddTag(string id, string tagName, string type, string category)
        {
            //Trace.TraceInformation("VypinApi.DataAccessLayer.DataAccessManager: Beginning AddTag");

            try
            {
                using (var context = new AssetEntities())
                {
                    if (String.IsNullOrEmpty(id))
                    {
                        return $"Tag ID is empty";
                    }
                    else
                    {
                        TagEntity tagEventExisting = context.TagEntities.Where(tagItem => tagItem.RawId == id).FirstOrDefault();
                        if (tagEventExisting != null)
                        {
                            tagEventExisting.Name = tagName;
                            tagEventExisting.Category = category;
                            tagEventExisting.Type = type;

                            context.SaveChanges();
                        }
                        else
                            return $"Tag {id} does not exist";
                    }
                }
            }
            catch (Exception e)
            {
                //Trace.TraceError("AddTag Exception" + e.ToString());
                return "Failure";
            }

            //Trace.TraceInformation("VypinApi.DataAccessLayer.DataAccessManager: Ending AddTag");
            return "success";
        }

        public List<TagEntity> GetAccountTags(string tagName, int accountId)
        {
            //Trace.TraceInformation("VypinApi.DataAccessLayer.DataAccessManager: Beginning GetAccountTags");
            List<TagEntity> tags = new List<TagEntity>();

            try
            {
                using (var context = new AssetEntities())
                {
                    tags = (from tagItem in context.TagEntities
                            join tagGroupItem in context.TagGroups on tagItem.Id equals tagGroupItem.TagId
                            join accountGroupItem in context.AccountGroups on tagGroupItem.GroupId equals accountGroupItem.GroupId
                            where accountGroupItem.AccountId == accountId
                            select tagItem).ToList();
                }
            }
            catch (Exception e)
            {
                //	Trace.TraceError("GetTags Exception" + e.ToString());
            }

            //Trace.TraceInformation("VypinApi.DataAccessLayer.DataAccessManager: Ending GetAccountTags");

            return tags;
        }

        public string ProcessTagLocation(LegacyTagEvent tagLocation)
        {
            //Trace.TraceInformation("VypinApi.DataAccessLayer.DataAccessManager: Beginning ProcessTagLocation");

            if (tagLocation == null)
            {
                //Trace.TraceError("TagLocation is Null");
                return "TagLocation is Null";
            }
            var tags = tagLocation.Tags;

            if (tags == null)
            {
                //Trace.TraceError("List of TagLocation Tags is Null");
                return "List of TagLocation Tags is Null";
            }

            if (tags.Count == 0)
            {
                //Trace.TraceError("List of TagLocation Tags is Empty");
                return "List of TagLocation Tags empty";
            }

            try
            {
                using (var context = new AssetEntities())
                {
                    //Trace.TraceError("List of TagLocation Tags Found" + tags.Count);
                    foreach (LegacyTag tag in tags)
                    {
                        var tagId = tag.TagId;

                        TagEntity tagExisting = context.TagEntities.Where(tagItem => tagItem.RawId == tag.TagId).FirstOrDefault();

                        string latitude = String.Empty;
                        string longitude = String.Empty;
                        AssignLatitudeAndLongitude(tag, tagExisting, out latitude, out longitude);

                        string microZoneCurrent = tag.Mzone1;
                        string microZonePrevious = tag.Mzone2;

                        TagEntity tagNew = BuildTagEntity(tag, tagLocation.ReceivedOn, microZoneCurrent, microZonePrevious);
                        TagHistory tagHistoryNew = BuildTagHistory(tag, tagLocation.ReceivedOn, microZoneCurrent, microZonePrevious);

                        //if the readerId is null on the tag itself, try the overall payload readerId
                        if (tag.ReaderId == null || tag.ReaderId == "")
                        {
                            tagNew.ReaderId = tagLocation.ReaderId;
                            tagHistoryNew.ReaderId = tagLocation.ReaderId;
                        }

                        if (latitude == "0 " || longitude == "0")
                        {
                            return "Ignoring because lat||lon are 0.";
                        }
                        else if (latitude == String.Empty || longitude == String.Empty)
                        {
                            return "Ignoring because lat||lon are empty.";
                        }

                        if (tagExisting == null)
                        {
                            context.TagEntities.Add(tagNew);
                        }
                        else
                        {

                            //if the tag exists, get its name from there, not the json
                            tagHistoryNew.Name = tagExisting.Name;
                            tagHistoryNew.Operation = "Mark";

                            // This is an existing tag - we don't want to update the name
                            tagExisting.Rssi = tagNew.Rssi;
                            tagExisting.mZone1Rssi = tagNew.mZone1Rssi;
                            tagExisting.StatusCode = tagNew.StatusCode;
                            tagExisting.LastUpdatedOn = DateTime.Now;
                            tagExisting.ReceivedOn = tagLocation.ReceivedOn;
                            tagExisting.Battery = tagNew.Battery;
                            tagExisting.TagType = tagNew.TagType;
                            tagExisting.Raw = tagNew.Raw;
                            tagExisting.MicroZoneCurrent = microZoneCurrent;
                            tagExisting.MicroZonePrevious = microZonePrevious;

                            if (tagNew.Longitude != "0" && tagNew.Longitude != "")
                                tagExisting.Longitude = tagNew.Longitude;

                            if (tagNew.Latitude != "0" && tagNew.Latitude != "")
                                tagExisting.Latitude = tagNew.Latitude;

                            tagExisting.ReaderId = tagNew.ReaderId;
                            tagExisting.Version = tagNew.Version;
                            tagExisting.TagType = tagNew.TagType;
                            tagExisting.SequenceNumber = tagNew.SequenceNumber;
                        }

                        //either way add a line to the taghistory
                        context.TagHistories.Add(tagHistoryNew);

                        context.SaveChanges();
                    }
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        return String.Format("Property: {0} Error: {1}",
                                                validationError.PropertyName,
                                                validationError.ErrorMessage);
                    }
                }
            }
            catch (Exception e)
            {
                //Trace.TraceError("TagLocation Exception" + e.ToString());
                return String.Format("Exception: {0}", e.ToString());
            }

            //Trace.TraceInformation("VypinApi.DataAccessLayer.DataAccessManager: Ending ProcessTagLocation");

            return "success";
        }

        public string ProcessTagName(LegacyTagEvent tagLocation)
        {
            Trace.TraceInformation("VypinApi.DataAccessLayer.DataAccessManager: Beginning ProcessTagName");

            if (tagLocation == null)
            {
                //Trace.TraceError("TagLocation is Null");
                return "TagLocation is Null";
            }

            var tags = tagLocation.Tags;

            if (tags == null)
            {
                ///Trace.TraceError("List of TagLocation Tags is Null");
                return "List of TagLocation Tags is Null";
            }

            if (tags.Count == 0)
            {
                //	Trace.TraceError("List of TagLocation Tags is Empty");
                return "List of TagLocation Tags empty";
            }

            try
            {
                using (var context = new AssetEntities())
                {
                    foreach (LegacyTag recentTag in tags)
                    {
                        TagEntity tagEventExisting = context.TagEntities.Where(tagItem => tagItem.RawId == recentTag.TagId).FirstOrDefault();

                        if (recentTag.Rssi == null)
                            recentTag.Rssi = "";
                        if (tagEventExisting == null)
                        {
                            Trace.TraceError($"No existing tag found with tagId {recentTag.TagId}");
                            // Register a new tag
                            TagEntity tagEventEntity = new TagEntity
                            {
                                RawId = String.IsNullOrEmpty(recentTag.TagId) == true ? String.Empty : recentTag.TagId,
                                Name = String.IsNullOrEmpty(recentTag.TagName) == true ? String.Empty : recentTag.TagName,
                                Rssi = String.IsNullOrEmpty(recentTag.Rssi.ToString()) == true ? String.Empty : recentTag.Rssi.ToString(),
                                mZone1Rssi = String.IsNullOrEmpty(recentTag.Mzone1Rssi) == true ? String.Empty : recentTag.Mzone1Rssi,
                                StatusCode = String.IsNullOrEmpty(recentTag.StatusCode) == true ? String.Empty : recentTag.StatusCode,
                                Battery = String.IsNullOrEmpty(recentTag.Battery) == true ? String.Empty : recentTag.Battery,
                                TagType = String.IsNullOrEmpty(recentTag.TagType) == true ? String.Empty : recentTag.TagType,
                                MicroZoneCurrent = String.IsNullOrEmpty(recentTag.Mzone1) == true ? String.Empty : recentTag.Mzone1,
                                MicroZonePrevious = String.IsNullOrEmpty(recentTag.Mzone2) == true ? String.Empty : recentTag.Mzone2,
                                LastUpdatedOn = DateTime.Now,
                                Latitude = String.IsNullOrEmpty(recentTag.Latitude) == true ? String.Empty : recentTag.Latitude.ToString(),
                                Longitude = String.IsNullOrEmpty(recentTag.Longitude) == true ? String.Empty : recentTag.Longitude.ToString(),
                                ReceivedOn = DateTime.Now,
                                ReaderId = String.IsNullOrEmpty(recentTag.ReaderId) == true ? String.Empty : recentTag.ReaderId
                            };

                            //add an event to the tag history for this also
                            TagHistory tagHistory = new TagHistory
                            {
                                RawId = String.IsNullOrEmpty(recentTag.TagId) == true ? String.Empty : recentTag.TagId,
                                Name = String.IsNullOrEmpty(recentTag.TagName) == true ? String.Empty : recentTag.TagName,
                                Rssi = String.IsNullOrEmpty(recentTag.Rssi.ToString()) == true ? String.Empty : recentTag.Rssi.ToString(),
                                mZone1Rssi = String.IsNullOrEmpty(recentTag.Mzone1Rssi) == true ? String.Empty : recentTag.Mzone1Rssi,
                                StatusCode = String.IsNullOrEmpty(recentTag.StatusCode) == true ? String.Empty : recentTag.StatusCode,
                                Battery = String.IsNullOrEmpty(recentTag.Battery) == true ? String.Empty : recentTag.Battery,
                                TagType = String.IsNullOrEmpty(recentTag.TagType) == true ? String.Empty : recentTag.TagType,
                                MicroZoneCurrent = String.IsNullOrEmpty(recentTag.Mzone1) == true ? String.Empty : recentTag.Mzone1,
                                MicroZonePrevious = String.IsNullOrEmpty(recentTag.Mzone2) == true ? String.Empty : recentTag.Mzone2,
                                LastUpdatedOn = DateTime.Now,
                                Latitude = String.IsNullOrEmpty(recentTag.Latitude) == true ? String.Empty : recentTag.Latitude.ToString(),
                                Longitude = String.IsNullOrEmpty(recentTag.Longitude) == true ? String.Empty : recentTag.Longitude.ToString(),
                                ReceivedOn = DateTime.Now,
                                ReaderId = String.IsNullOrEmpty(recentTag.ReaderId) == true ? String.Empty : recentTag.ReaderId
                            };

                            tagHistory.Operation = "Register";

                            context.TagHistories.Add(tagHistory);
                            context.TagEntities.Add(tagEventEntity);
                            context.SaveChanges();

                            tagEventExisting = context.TagEntities.Where(tagItem => tagItem.RawId == recentTag.TagId).FirstOrDefault();

                            //Add any associated assets
                            if (tagEventEntity != null && recentTag.AssociatedAssets != null)
                            {
                                foreach (Asset asset in recentTag.AssociatedAssets)
                                {
                                    Asset dbAsset = context.Assets.Where(assetItem => assetItem.AssetIdentifier == asset.AssetIdentifier).FirstOrDefault();

                                    //if one exists, just update it properties
                                    if (dbAsset != null)
                                    {
                                        dbAsset.Name = asset.Name;
                                        dbAsset.SlotId = asset.SlotId;
                                        dbAsset.TagId = tagEventExisting.Id;
                                        dbAsset.IsActive = asset.IsActive;
                                        dbAsset.AssetIdentifier = asset.AssetIdentifier;

                                        context.SaveChanges();
                                    }
                                    else
                                    {
                                        //otherwise add it to the db
                                        asset.TagId = tagEventExisting.Id;
                                        context.Assets.Add(asset);
                                    }
                                }
                            }
                        }
                        else
                        {
                            // This is an existing tag - we only want to update the name (tag rename), the update time, and where it came from
                            tagEventExisting.Name = recentTag.TagName;
                            tagEventExisting.ReaderId = recentTag.ReaderId;
                            tagEventExisting.LastUpdatedOn = DateTime.Now;

                            Trace.TraceError($"Existing tag found with tagId {recentTag.TagId}");

                            if (recentTag.AssociatedAssets != null)
                            {
                                List<Asset> assetsToRemove = context.Assets.Where(assetItem => assetItem.TagId == tagEventExisting.Id).ToList();
                                Trace.TraceError($"Looking for assets associated with tagId {tagEventExisting.Id}");

                                if (assetsToRemove != null)
                                {
                                    Trace.TraceError($"Found {assetsToRemove.Count} assets to update/remove.");
                                    context.Assets.RemoveRange(assetsToRemove);

                                    context.SaveChanges();
                                }

                                foreach (Asset asset in recentTag.AssociatedAssets)
                                {
                                    Asset dbAsset = context.Assets.Where(assetItem => assetItem.AssetIdentifier == asset.AssetIdentifier).FirstOrDefault();
                                    Trace.TraceError($"Looking for assets with assetId {asset.AssetIdentifier}");

                                    //if one exists, just update it properties
                                    if (dbAsset != null)
                                    {
                                        Trace.TraceError($"Found existing asset, updating assetName to {asset.Name}, isActive to {asset.IsActive}, and Identifier to {asset.AssetIdentifier}");

                                        dbAsset.Name = asset.Name;
                                        dbAsset.SlotId = asset.SlotId;
                                        dbAsset.TagId = tagEventExisting.Id;
                                        dbAsset.IsActive = asset.IsActive;
                                        dbAsset.AssetIdentifier = asset.AssetIdentifier;

                                        context.SaveChanges();
                                    }
                                    else
                                    {
                                        Trace.TraceError($"NO asset found, adding a new one with tagID {tagEventExisting.Id}");

                                        //otherwise add it to the db
                                        asset.TagId = tagEventExisting.Id;
                                        Trace.TraceError($"Setting new assetName to {asset.Name}, isActive to {asset.IsActive}, and Identifier to {asset.AssetIdentifier}");
                                        context.Assets.Add(asset);

                                        context.SaveChanges();
                                    }
                                }
                            }

                            //add an event to the tag history for this also
                            TagHistory tagHistory = new TagHistory
                            {
                                RawId = String.IsNullOrEmpty(recentTag.TagId) == true ? String.Empty : recentTag.TagId,
                                Name = String.IsNullOrEmpty(recentTag.TagName) == true ? String.Empty : recentTag.TagName,
                                Rssi = String.IsNullOrEmpty(recentTag.Rssi.ToString()) == true ? String.Empty : recentTag.Rssi.ToString(),
                                mZone1Rssi = String.IsNullOrEmpty(recentTag.Mzone1Rssi) == true ? String.Empty : recentTag.Mzone1Rssi,
                                StatusCode = String.IsNullOrEmpty(recentTag.StatusCode) == true ? String.Empty : recentTag.StatusCode,
                                Battery = String.IsNullOrEmpty(recentTag.Battery) == true ? String.Empty : recentTag.Battery,
                                TagType = String.IsNullOrEmpty(recentTag.TagType) == true ? String.Empty : recentTag.TagType,
                                MicroZoneCurrent = String.IsNullOrEmpty(recentTag.Mzone1) == true ? String.Empty : recentTag.Mzone1,
                                MicroZonePrevious = String.IsNullOrEmpty(recentTag.Mzone2) == true ? String.Empty : recentTag.Mzone2,
                                LastUpdatedOn = DateTime.Now,
                                Latitude = String.IsNullOrEmpty(recentTag.Latitude) == true ? String.Empty : recentTag.Latitude.ToString(),
                                Longitude = String.IsNullOrEmpty(recentTag.Longitude) == true ? String.Empty : recentTag.Longitude.ToString(),
                                ReceivedOn = DateTime.Now,
                                ReaderId = String.IsNullOrEmpty(recentTag.ReaderId) == true ? String.Empty : recentTag.ReaderId
                            };

                            tagHistory.Operation = "Rename";

                            context.TagHistories.Add(tagHistory);
                        }

                        context.SaveChanges();
                    }
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        return String.Format("Property: {0} Error: {1}",
                                                validationError.PropertyName,
                                                validationError.ErrorMessage);
                    }
                }
            }
            catch (Exception e)
            {
                return String.Format("Exception: {0}", e.ToString());
                Trace.TraceInformation($"ProcessTagName Exception {e.Message}, Inner Exception {e.InnerException}");
            }

            //Trace.TraceInformation("VypinApi.DataAccessLayer.DataAccessManager: Ending ProcessTagName");
            return "success";
        }

        public string DeleteTags()
        {
            //Trace.TraceInformation("VypinApi.DataAccessLayer.DataAccessManager: Beginning DeleteTags");

            try
            {
                using (var context = new AssetEntities())
                {
                    context.Database.ExecuteSqlCommand("TRUNCATE TABLE TagEntities");

                }
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        //Trace.TraceError("DeleteTags dbException" + validationError.ErrorMessage);
                        return String.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
            }
            catch (Exception e)
            {
                //Trace.TraceError("DeleteTags Exception" + e.ToString());
                return String.Format("Exception: {0}", e.ToString());
            }

            //Trace.TraceInformation("VypinApi.DataAccessLayer.DataAccessManager: Ending DeleteTags");

            return "success";
        }

        public string DeleteTag(string tagRequest)
        {
            //Trace.TraceInformation("VypinApi.DataAccessLayer.DataAccessManager: Beginning DeleteTag");

            try
            {
                using (var context = new AssetEntities())
                {
                    TagEntity tagEventExisting = context.TagEntities.Where(tagItem => tagItem.RawId == tagRequest).FirstOrDefault();

                    if (tagEventExisting != null)
                    {
                        context.TagEntities.Remove(tagEventExisting);
                        context.SaveChanges();

                        return "success";
                    }
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        //Trace.TraceError("DeleteTag Exception" + validationError.ErrorMessage);
                        return String.Format("Property: {0} Error: {1}",
                                                validationError.PropertyName,
                                                validationError.ErrorMessage);
                    }
                }
            }
            catch (Exception e)
            {
                //Trace.TraceError("DeleteTag Exception" + e.ToString());
                return String.Format("Exception: {0}", e.ToString());
            }

            return String.Format($"Failed to find tag: {0}", tagRequest.ToString());
        }

        public string AddTag(TagEvent tagEvent)
        {
            StringBuilder logBuilder = new StringBuilder();

            logBuilder.AppendLine("Api.DataAccessLayer.DataAccessManager: Beginning AddTag");
            Trace.TraceInformation("VypinApi.DataAccessLayer.DataAccessManager: Beginning AddTag");

            if (tagEvent == null)
            {
                Trace.TraceError("TagEvent is Null");
                return "TagEvent is Null";
            }

            var tags = tagEvent.Tags;

            if (tags == null)
            {
                Trace.TraceError("List of TagEvent Tags is Null");
                return "List of TagEvent Tags is Null";
            }

            if (tags.Count == 0)
            {
                System.Diagnostics.Trace.TraceError("List of TagEvent Tags is empty now");

                Trace.TraceError("List of TagEvent Tags is empty");
                return "List of TagEvent Tags empty";
            }

            try
            {
                using (var context = new AssetEntities())
                {
                    foreach (Tag currentTag in tags)
                    {
                        Trace.TraceInformation($"Add tag for tagId {currentTag.LastEvent.TagId}");
                        Trace.TraceInformation($"Latitude: {currentTag.LastEvent.Latitude.ToString()}");
                        Trace.TraceInformation($"Longitude: {currentTag.LastEvent.Longitude.ToString()}");
                        Trace.TraceInformation($"mZone1: {currentTag.LastEvent.Mzone1.ToString()}");
                        Trace.TraceInformation($"mZone2: {currentTag.LastEvent.Mzone2.ToString()}");
                        Trace.TraceInformation($"rssi: {currentTag.LastEvent.Longitude.ToString()}");
                        Trace.TraceInformation($"Received On: {currentTag.LastEvent.ReceivedOn.ToString()}");
                        Trace.TraceInformation($"Last Updated On: {DateTime.Now.ToString()}");

                        var currentTagId = currentTag.LastEvent.TagId.Trim();
                        var currentTagName = String.IsNullOrEmpty(currentTag.LastEvent.TagName) == true ? String.Empty : currentTag.LastEvent.TagName;
                        var currentMzone1Rssi = String.IsNullOrEmpty(currentTag.Mzone1Rssi.ToString()) == true ? String.Empty : currentTag.Mzone1Rssi.ToString();
                        var currentRaw = "";
                        //Trace.TraceInformation($"mZoneRssi for {currentTag.LastEvent.TagId} is {currentMzone1Rssi}");

                        List<TagEntity> tagEvents = new List<TagEntity>();
                        tagEvents = context.TagEntities.Where(tagItem => tagItem.RawId == currentTagId).ToList();

                        if (tagEvents.Count == 0)
                        {
                            logBuilder.AppendLine($"This is a new tag for tag {currentTagId}");

                            TagEntity tagEventEntity = new TagEntity
                            {
                                RawId = currentTagId,
                                Name = currentTagName,
                                Rssi = currentTag.LastEvent.Rssi.ToString(),
                                mZone1Rssi = currentMzone1Rssi,
                                StatusCode = currentTag.LastEvent.StatusCode,
                                LastUpdatedOn = DateTime.Now,
                                ReceivedOn = DateTime.Parse(tagEvent.ReceivedOn),
                                Battery = currentTag.LastEvent.Battery,
                                TagType = currentTag.LastEvent.TagType,
                                Raw = currentRaw,
                                MicroZoneCurrent = currentTag.LastEvent.Mzone1.Trim(),
                                MicroZonePrevious = currentTag.LastEvent.Mzone2.Trim(),
                                Longitude = currentTag.LastEvent.Longitude.ToString(),
                                Latitude = currentTag.LastEvent.Latitude.ToString(),
                                ReaderId = tagEvent.ReaderId,
                                Version = tagEvent.Version,
                                SequenceNumber = currentTag.LastEvent.SequenceNumber,
                                Temperature = currentTag.LastEvent.Temperature,
                                Humidity = currentTag.LastEvent.Humidity
                            };

                            context.TagEntities.Add(tagEventEntity);

                            //add a history event also since this is the first time
                            TagHistory tagEventHistory = new TagHistory
                            {
                                RawId = currentTagId,
                                Name = currentTagName,
                                Rssi = currentTag.LastEvent.Rssi.ToString(),
                                mZone1Rssi = currentMzone1Rssi,
                                StatusCode = currentTag.LastEvent.StatusCode,
                                LastUpdatedOn = DateTime.Now,
                                ReceivedOn = DateTime.Parse(tagEvent.ReceivedOn),
                                Battery = currentTag.LastEvent.Battery,
                                TagType = currentTag.LastEvent.TagType,
                                Raw = currentRaw,
                                MicroZoneCurrent = currentTag.LastEvent.Mzone1.Trim(),
                                MicroZonePrevious = currentTag.LastEvent.Mzone2.Trim(),
                                Longitude = currentTag.LastEvent.Longitude.ToString(),
                                Latitude = currentTag.LastEvent.Latitude.ToString(),
                                ReaderId = tagEvent.ReaderId,
                                Version = tagEvent.Version,
                                SequenceNumber = currentTag.LastEvent.SequenceNumber,
                                BeaconCount = 1,
                                Temperature = currentTag.LastEvent.Temperature,
                                Humidity = currentTag.LastEvent.Humidity
                            };

                            context.TagHistories.Add(tagEventHistory);
                        }
                        else
                        {

                            Trace.TraceError("Existing tag, getting latest tag.");
                            //get the most recent tagHistory to see if we need to add another record (the last updated is a minute or older)
                            List<TagHistory> existingTags = context.TagHistories.Where(tagItem => tagItem.RawId == currentTagId).ToList();

                            //get the name for this id since this doesnt come through on an update
                            string tagName = context.TagEntities.Where(tagItem => tagItem.RawId == currentTagId).First().Name;
                            Trace.TraceError($"tagName for existing tag is {tagName}");

                            if (existingTags != null && existingTags.Count > 0)
                            {
                                Trace.TraceInformation($"Found tag history for tag {currentTag.LastEvent.TagId}");
                                DateTime latestTime = existingTags.Max(x => x.LastUpdatedOn);
                                TagHistory tagHistory = existingTags.Where(x => x.LastUpdatedOn == latestTime).FirstOrDefault();

                               // if (DateTime.Now >= tagHistory.LastUpdatedOn.AddMinutes(1))
                                //{
                                    string mostRecentRssi = tagHistory.mZone1Rssi;

                                    //Trace.TraceInformation($"Last TagEvent for {tagHistory.RawId} older than 1 minute, checking rssi {currentMzone1Rssi}");
                                    TagHistory newTagHistory = new TagHistory();
                                    newTagHistory.RawId = currentTag.LastEvent?.TagId.Trim() ?? "";
                                    newTagHistory.Name = tagName;
                                    newTagHistory.Rssi = currentTag.LastEvent.Rssi.ToString();
                                    newTagHistory.StatusCode = currentTag.LastEvent.StatusCode;
                                    newTagHistory.ReceivedOn = DateTime.Parse(tagEvent.ReceivedOn);
                                    newTagHistory.Battery = currentTag.LastEvent.Battery;
                                    newTagHistory.TagType = currentTag.LastEvent.TagType;
                                    newTagHistory.Raw = currentRaw;
                                    newTagHistory.MicroZoneCurrent = currentTag.LastEvent?.Mzone1.Trim() ?? "";
                                    newTagHistory.MicroZonePrevious = currentTag.LastEvent?.Mzone2.Trim() ?? "";
                                    newTagHistory.ReaderId = tagEvent.ReaderId;
                                    newTagHistory.Version = tagEvent.Version;
                                    newTagHistory.SequenceNumber = currentTag.LastEvent.SequenceNumber;
                                    newTagHistory.Operation = "Update";
                                    newTagHistory.Temperature = currentTag.LastEvent.Temperature;
                                    newTagHistory.Humidity = currentTag.LastEvent.Humidity;

                                    //make sure we update the name if one exists
                                    if (currentTag.LastEvent.TagName != null)
                                    {
                                        if (currentTag.LastEvent.TagName != string.Empty && currentTag.LastEvent.TagName != "")
                                        {
                                            newTagHistory.Name = currentTag.LastEvent.TagName;
                                        }
                                    }

                                    newTagHistory.mZone1Rssi = currentMzone1Rssi;
                                    newTagHistory.LastUpdatedOn = DateTime.Now;
                                    newTagHistory.Longitude = currentTag.LastEvent.Longitude.ToString();
                                    newTagHistory.Latitude = currentTag.LastEvent.Latitude.ToString();

                                    int mZoneRssi;

                                    if (Int32.TryParse(currentMzone1Rssi, out mZoneRssi))
                                    {
                                        Trace.TraceInformation($"Parsed {mZoneRssi}.");
                                    }
                                    else
                                    {
                                        Trace.TraceInformation($"Failed to parsed {currentMzone1Rssi}.");
                                    }

                                //check the beacon count to determine:
                                //a)whether or not we need to add this event
                                //b)the avg amount of the lat/lon

                                // double parsedLon = 0;
                                // double parsedLat = 0;

                                //another note
                                //UPDATE 08/03/2022
                                //The algorithm for adding new tags is as follows:

                                // a) If it's a mZone, report it no matter what. We know it's a mZXone if the mZone1Rssi is '0'
                                // b) If it isn't a mZone, check it's current mZone and the most recent tagHistory, for this tagId, mZone. If the most recent was '000000'
                                // and the current is not, then it picked up a new mZone. Add a new tag history for that and increase beacon count to one.
                                // c) If not, then check the beacon count. If it is less than 5, go ahead and add a new tag history for this tag id in the current mZone.
                                // d) If the beacon count is above 5, check the current mZone again. If it is still the same, ignore it as we don't want to add any more
                                // beacons for the current mZone above 5. If it has changed to '000000', then add a new tag history and reset the beacon count since we
                                // dropped the current mZone. Our algorithm will restart from here back at a).

                                //if this is a mZone, report it no matter what
                                if(newTagHistory.mZone1Rssi == "0")
                                {
                                    newTagHistory.BeaconCount = 0;
                                    context.TagHistories.Add(newTagHistory);
                                }
                                else if (tagHistory.MicroZoneCurrent == "000000" && newTagHistory.MicroZoneCurrent != "000000")
                                {
                                    newTagHistory.BeaconCount = 1;
                                    context.TagHistories.Add(newTagHistory);

                                    TagEntity existingTag = context.TagEntities.Where(tagItem => tagItem.RawId == currentTagId).FirstOrDefault();

                                    Trace.TraceInformation($"New mZone picked up. Adding tag event for tag {existingTag.RawId} at {DateTime.Now.ToString()}");
                                    existingTag.Rssi = newTagHistory.Rssi;
                                    existingTag.mZone1Rssi = newTagHistory.mZone1Rssi;
                                    existingTag.StatusCode = newTagHistory.StatusCode;
                                    existingTag.LastUpdatedOn = DateTime.Now;
                                    existingTag.ReceivedOn = newTagHistory.ReceivedOn;
                                    existingTag.Battery = newTagHistory.Battery;
                                    existingTag.TagType = newTagHistory.TagType;
                                    existingTag.MicroZoneCurrent = newTagHistory.MicroZoneCurrent;
                                    existingTag.MicroZonePrevious = newTagHistory.MicroZonePrevious;
                                    existingTag.ReaderId = newTagHistory.ReaderId;
                                    existingTag.Version = newTagHistory.Version;
                                    existingTag.SequenceNumber = newTagHistory.SequenceNumber;
                                    existingTag.Latitude = newTagHistory.Latitude;
                                    existingTag.Longitude = newTagHistory.Longitude;
                                    existingTag.Temperature = newTagHistory.Temperature;
                                    existingTag.Humidity = newTagHistory.Humidity;
                                    
                                    context.SaveChanges();
                                }
                                else if (tagHistory.BeaconCount < 5)
                                {
                                        Trace.TraceInformation($"Beacon Count for previous tag {tagHistory.Raw} is {tagHistory.BeaconCount} at {DateTime.Now.ToString()}");
                                        newTagHistory.BeaconCount = tagHistory.BeaconCount + 1;
                                        context.TagHistories.Add(newTagHistory);

                                        TagEntity existingTag = context.TagEntities.Where(tagItem => tagItem.RawId == currentTagId).FirstOrDefault();

                                        Trace.TraceInformation($"Adding tag event for tag {existingTag.RawId} at {DateTime.Now.ToString()}");
                                        existingTag.Rssi = newTagHistory.Rssi;
                                        existingTag.mZone1Rssi = newTagHistory.mZone1Rssi;
                                        existingTag.StatusCode = newTagHistory.StatusCode;
                                        existingTag.LastUpdatedOn = DateTime.Now;
                                        existingTag.ReceivedOn = newTagHistory.ReceivedOn;
                                        existingTag.Battery = newTagHistory.Battery;
                                        existingTag.TagType = newTagHistory.TagType;
                                        existingTag.MicroZoneCurrent = newTagHistory.MicroZoneCurrent;
                                        existingTag.MicroZonePrevious = newTagHistory.MicroZonePrevious;
                                        existingTag.ReaderId = newTagHistory.ReaderId;
                                        existingTag.Version = newTagHistory.Version;
                                        existingTag.SequenceNumber = newTagHistory.SequenceNumber;
                                        existingTag.Latitude = newTagHistory.Latitude;
                                        existingTag.Longitude = newTagHistory.Longitude;
                                        existingTag.Temperature = newTagHistory.Temperature;
                                        existingTag.Humidity = newTagHistory.Humidity;

                                        context.SaveChanges();
                                }
                                else
                                {
                                    Trace.TraceInformation($"Beacon Count for {newTagHistory.RawId} limit reached. Waiting for new mZone");

                                    if(newTagHistory.MicroZoneCurrent == "000000" && tagHistory.MicroZoneCurrent != "000000")
                                    {
                                        newTagHistory.BeaconCount = 5;
                                        context.TagHistories.Add(newTagHistory);

                                        TagEntity existingTag = context.TagEntities.Where(tagItem => tagItem.RawId == currentTagId).FirstOrDefault();

                                        Trace.TraceInformation($"Adding tag event for tag {existingTag.RawId} at {DateTime.Now.ToString()}");
                                        existingTag.Rssi = newTagHistory.Rssi;
                                        existingTag.mZone1Rssi = newTagHistory.mZone1Rssi;
                                        existingTag.StatusCode = newTagHistory.StatusCode;
                                        existingTag.LastUpdatedOn = DateTime.Now;
                                        existingTag.ReceivedOn = newTagHistory.ReceivedOn;
                                        existingTag.Battery = newTagHistory.Battery;
                                        existingTag.TagType = newTagHistory.TagType;
                                        existingTag.MicroZoneCurrent = newTagHistory.MicroZoneCurrent;
                                        existingTag.MicroZonePrevious = newTagHistory.MicroZonePrevious;
                                        existingTag.ReaderId = newTagHistory.ReaderId;
                                        existingTag.Version = newTagHistory.Version;
                                        existingTag.SequenceNumber = newTagHistory.SequenceNumber;
                                        existingTag.Latitude = newTagHistory.Latitude;
                                        existingTag.Longitude = newTagHistory.Longitude;
                                        existingTag.Temperature = newTagHistory.Temperature;
                                        existingTag.Humidity = newTagHistory.Humidity;

                                        context.SaveChanges();
                                    }
                                }
                               // else
                               // {
                               //     Trace.TraceInformation($"Less than 1 minute since last update. Not adding tag {currentTag.LastEvent.TagId}");
                               // }
                                //}
                                //else
                                //{
                                //    switch (tagHistory.BeaconCount)
                                //    {
                                //        case 0:
                                //            //our last beacon was -127, just add this beacon and set count to 1
                                //            Trace.TraceInformation($"Beacon count for tag {newTagHistory.RawId} is 0, mZoneRssi={mZoneRssi}");
                                //            if (mZoneRssi > -127)
                                //            {
                                //                if (currentTag.LastEvent.Longitude.ToString() != "0" && currentTag.LastEvent.Longitude.ToString() != "")
                                //                    newTagHistory.Longitude = currentTag.LastEvent.Longitude.ToString();

                                //                if (currentTag.LastEvent.Latitude.ToString() != "0" && currentTag.LastEvent.Latitude.ToString() != "")
                                //                    newTagHistory.Latitude = currentTag.LastEvent.Latitude.ToString();

                                //                newTagHistory.BeaconCount = 1;
                                //                context.TagHistories.Add(newTagHistory);
                                //            }
                                //            else
                                //            {
                                //                Trace.TraceInformation($"ignoring -127 for beacon count 0");
                                //            }
                                //            break;
                                //        case 1:
                                //            //this is now our second beacon, so take the existing lat/lon, add the new lat/lon / 2 to get the avg.
                                //            //set beacon count to 2
                                //            Trace.TraceInformation($"Beacon count for tag {newTagHistory.RawId} is 1, mZoneRssi={mZoneRssi}");
                                //            if (mZoneRssi > -127)
                                //            {
                                //                if (currentTag.LastEvent.Longitude.ToString() != "0" && currentTag.LastEvent.Longitude.ToString() != "")
                                //                {
                                //                    parsedLon = Double.Parse(tagHistory.Longitude);
                                //                    newTagHistory.Longitude = ((parsedLon + currentTag.LastEvent.Longitude) / 2).ToString();
                                //                    Trace.TraceInformation($"Parsed lon={parsedLon}, calculated average from two gps coords is {newTagHistory.Longitude}");

                                //                }

                                //                if (currentTag.LastEvent.Latitude.ToString() != "0" && currentTag.LastEvent.Latitude.ToString() != "")
                                //                {
                                //                    parsedLat = Double.Parse(tagHistory.Latitude);
                                //                    newTagHistory.Latitude = ((parsedLat + currentTag.LastEvent.Latitude) / 2).ToString();
                                //                    Trace.TraceInformation($"Parsed lon={parsedLat}, calculated average from two gps coords is {newTagHistory.Latitude}");
                                //                }

                                //                newTagHistory.BeaconCount = 2;
                                //                context.TagHistories.Add(newTagHistory);
                                //            }
                                //            else
                                //            {
                                //                Trace.TraceInformation($"ignoring -127 for beacon count 1");
                                //            }
                                //            break;
                                //        case 2:
                                //            //this is our third and final beacon, take the existing lat/lon / 2 to get original sum
                                //            //then add the third lat/lon and / 3 to get the final avg.
                                //            //set beacon count to 3;
                                //            Trace.TraceInformation($"Beacon count for tag {newTagHistory.RawId} is 2, mZoneRssi={mZoneRssi}");
                                //            if (mZoneRssi > -127)
                                //            {
                                //                if (currentTag.LastEvent.Longitude.ToString() != "0" && currentTag.LastEvent.Longitude.ToString() != "")
                                //                {
                                //                    parsedLon = Double.Parse(tagHistory.Longitude);
                                //                    double meanSum = parsedLon * 2;
                                //                    newTagHistory.Longitude = ((meanSum + currentTag.LastEvent.Longitude) / 3).ToString();
                                //                    Trace.TraceInformation($"Parsed lon={parsedLon}, mean sum={meanSum}, calculated average from three gps coords is {newTagHistory.Latitude}");
                                //                }

                                //                if (currentTag.LastEvent.Latitude.ToString() != "0" && currentTag.LastEvent.Latitude.ToString() != "")
                                //                {
                                //                    parsedLat = Double.Parse(tagHistory.Latitude);
                                //                    double meanSum = parsedLat * 2;
                                //                    newTagHistory.Latitude = ((meanSum + currentTag.LastEvent.Latitude) / 3).ToString();
                                //                    Trace.TraceInformation($"Parsed lon={parsedLat}, mean sum={meanSum}, calculated average from three gps coords is {newTagHistory.Latitude}");
                                //                }

                                //                newTagHistory.BeaconCount = 3;
                                //                context.TagHistories.Add(newTagHistory);

                                //                //now that we have an average of the last three beacons, we can add a new TagEntity to the list
                                //                //now get the existing tag and update everything but the Id
                                //                TagEntity existingTag = context.TagEntities.Where(tagItem => tagItem.RawId == currentTagId).FirstOrDefault();

                                //                Trace.TraceInformation($"Adding tag event for 3rd beacon.");
                                //                existingTag.Rssi = newTagHistory.Rssi;
                                //                existingTag.mZone1Rssi = newTagHistory.mZone1Rssi;
                                //                existingTag.StatusCode = newTagHistory.StatusCode;
                                //                existingTag.LastUpdatedOn = newTagHistory.LastUpdatedOn;
                                //                existingTag.ReceivedOn = newTagHistory.ReceivedOn;
                                //                existingTag.Battery = newTagHistory.Battery;
                                //                existingTag.TagType = newTagHistory.TagType;
                                //                existingTag.MicroZoneCurrent = newTagHistory.MicroZoneCurrent;
                                //                existingTag.MicroZonePrevious = newTagHistory.MicroZonePrevious;
                                //                existingTag.ReaderId = newTagHistory.ReaderId;
                                //                existingTag.Version = newTagHistory.Version;
                                //                existingTag.SequenceNumber = newTagHistory.SequenceNumber;
                                //                existingTag.Latitude = newTagHistory.Latitude;
                                //                existingTag.Longitude = newTagHistory.Longitude;
                                //                context.SaveChanges();
                                //            }
                                //            else
                                //            {
                                //                Trace.TraceInformation($"ignoring -127 for beacon count 2");
                                //            }
                                //            break;
                                //        default:
                                //            //3 or above should end up here, ignore anything at this point unless its a -127
                                //            //if so set mZone1Rssi to -127 and reset beacon count to 0
                                //            Trace.TraceInformation($"Beacon count for tag {newTagHistory.RawId} is 3 or more, mZoneRssi={mZoneRssi}");
                                //            if (mZoneRssi <= -127)
                                //            {
                                //                if (currentTag.LastEvent.Longitude.ToString() != "0" && currentTag.LastEvent.Longitude.ToString() != "")
                                //                    newTagHistory.Longitude = currentTag.LastEvent.Longitude.ToString();

                                //                if (currentTag.LastEvent.Latitude.ToString() != "0" && currentTag.LastEvent.Latitude.ToString() != "")
                                //                    newTagHistory.Latitude = currentTag.LastEvent.Latitude.ToString();

                                //                newTagHistory.BeaconCount = 0;
                                //                context.TagHistories.Add(newTagHistory);
                                //            }
                                //            else
                                //            {
                                //                Trace.TraceInformation($"GPS avg beacon met, ignoring and waiting for -127");
                                //            }
                                //            break;
                                //    }
                                //}

                                //context.SaveChanges();
                            }
                            else
                            {
                                Trace.TraceInformation($"No tag found, creating new tag history for  {currentTag.LastEvent.TagId}");
                                string log = "0";
                                string lat = "0";

                                if (currentTag.LastEvent.Longitude.ToString() != "0" && currentTag.LastEvent.Longitude.ToString() != "")
                                    log = currentTag.LastEvent.Longitude.ToString();

                                if (currentTag.LastEvent.Latitude.ToString() != "0" && currentTag.LastEvent.Latitude.ToString() != "")
                                    lat = currentTag.LastEvent.Latitude.ToString();

                                TagHistory newTagHistory = new TagHistory
                                {
                                    RawId = currentTag.LastEvent?.TagId.Trim() ?? "",
                                    Name = tagName,
                                    Rssi = currentTag.LastEvent.Rssi.ToString(),
                                    mZone1Rssi = currentMzone1Rssi,
                                    StatusCode = currentTag.LastEvent.StatusCode,
                                    LastUpdatedOn = DateTime.Now,
                                    ReceivedOn = DateTime.Parse(tagEvent.ReceivedOn),
                                    Battery = currentTag.LastEvent.Battery,
                                    TagType = currentTag.LastEvent.TagType,
                                    Raw = currentRaw,
                                    MicroZoneCurrent = currentTag.LastEvent?.Mzone1.Trim() ?? "",
                                    MicroZonePrevious = currentTag.LastEvent?.Mzone2.Trim() ?? "",
                                    Longitude = log,
                                    Latitude = lat,
                                    ReaderId = tagEvent.ReaderId,
                                    Version = tagEvent.Version,
                                    SequenceNumber = currentTag.LastEvent.SequenceNumber,
                                    BeaconCount = 1,
                                    Temperature = currentTag.LastEvent.Temperature,
                                    Humidity = currentTag.LastEvent.Humidity
                                };

                                if (currentTag.LastEvent.TagName != null)
                                {
                                    if (currentTag.LastEvent.TagName != string.Empty && currentTag.LastEvent.TagName != "")
                                    {
                                        newTagHistory.Name = currentTag.LastEvent.TagName;
                                    }
                                }

                                newTagHistory.Operation = "Update";
                                context.TagHistories.Add(newTagHistory);
                                Trace.TraceInformation($"Adding event.");
                            }
                        }
                    }

                    //foreach(TagEntity tag in context.TagEntities)
                    //{
                    //    if(DateTime.Now > tag.LastUpdatedOn.AddMinutes(7))
                    //    {
                    //            Trace.TraceInformation($"Tag={tag.RawId} hasn't been seen in over seven minutes. LastUpdatedOn={tag.LastUpdatedOn}, marking as 'missing'.");

                    //            //if we haven't seen a tag in over five minuteS, mark it as "missing" by replacing its mZone w/ 000000
                    //            tag.MicroZonePrevious = tag.MicroZoneCurrent;
                    //            tag.MicroZoneCurrent = "000000";
                    //            tag.LastUpdatedOn = DateTime.Now;

                    //            //if we change the mZone, we need another tagHistory for that change
                    //            TagHistory tagEventHistory = new TagHistory
                    //            {
                    //                RawId = tag.RawId,
                    //                Name = tag.Name,
                    //                Rssi = tag.Rssi,
                    //                mZone1Rssi = tag.mZone1Rssi,
                    //                StatusCode = tag.StatusCode,
                    //                LastUpdatedOn = DateTime.Now,
                    //                ReceivedOn = tag.ReceivedOn,
                    //                Battery = tag.Battery,
                    //                TagType = tag.TagType,
                    //                Raw = tag.Raw,
                    //                MicroZoneCurrent = tag.MicroZoneCurrent,
                    //                MicroZonePrevious = tag.MicroZonePrevious,
                    //                Longitude = tag.Longitude,
                    //                Latitude = tag.Latitude,
                    //                ReaderId = tag.ReaderId,
                    //                Version = tag.Version,
                    //                SequenceNumber = tag.SequenceNumber,
                    //                BeaconCount = 1,
                    //                Operation = "Missing"
                    //            };

                    //            context.TagHistories.Add(tagEventHistory);
                    //    }
                    //}

                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Trace.TraceError($"AddTag Exception: {e.ToString()}");
                logBuilder.AppendLine(e.ToString());
                logBuilder.AppendLine();
                logBuilder.AppendLine();
            }

            return logBuilder.ToString();
        }

        public string AddLegacyTag(LegacyTagEvent tagEvent)
        {
            StringBuilder logBuilder = new StringBuilder();
            try
            {
                Trace.TraceInformation("VypinApi.DataAccessLayer.DataAccessManager: Beginning Legacy AddTag");

                if (tagEvent == null)
                {
                    Trace.TraceError("TagEvent is Null");
                    return "TagEvent is Null";
                }

                Trace.TraceInformation("VypinApi.DataAccessLayer.DataAccessManager: Getting tags");
                var tags = tagEvent.Tags;

                if (tags == null)
                {
                    Trace.TraceError("List of TagEvent Tags is Null");
                    return "List of TagEvent Tags is Null";
                }

                if (tags.Count == 0)
                {
                    System.Diagnostics.Trace.TraceError("List of TagEvent Tags is empty now");

                    Trace.TraceError("List of TagEvent Tags is empty");
                    return "List of TagEvent Tags empty";
                }

                try
                {
                    using (var context = new AssetEntities())
                    {
                        //Trace.TraceInformation("VypinApi.DataAccessLayer.DataAccessManager: Foreaching through tags");
                        foreach (LegacyTag currentTag in tags)
                        {
                            //Trace.TraceInformation($"VypinApi.DataAccessLayer.DataAccessManager: tag {currentTag}");
                            var currentTagId = currentTag.TagId.Trim();
                            var currentTagName = String.IsNullOrEmpty(currentTag.TagName) == true ? String.Empty : currentTag.TagName;
                            var currentMzone1Rssi = String.IsNullOrEmpty(currentTag.Mzone1Rssi.ToString()) == true ? String.Empty : currentTag.Mzone1Rssi.ToString();
                            //var currentRaw = String.IsNullOrEmpty(currentTag.Raw) == true ? String.Empty : currentTag.Raw;

                            //if (currentTagId != null)
                            //    Trace.TraceInformation($"VypinApi.DataAccessLayer.DataAccessManager: Checking if {currentTagId} exists in database");
                            //else
                            //    Trace.TraceInformation($"VypinApi.DataAccessLayer.DataAccessManager: currentTagId is null");

                            List<TagEntity> tagEvents = context.TagEntities.Where(tagItem => tagItem.RawId == currentTagId).ToList();
                            if (tagEvents.Count == 0)
                            {
                                //logBuilder.AppendLine($"This is a new tag for tag {currentTagId}");
                                //logBuilder.AppendLine();

                                //if (currentTag.Mzone1 != "000000")
                                //{
                                //logBuilder.AppendLine($"This new tag has a valid mZone {currentTag.Mzone1}");
                                //logBuilder.AppendLine();

                                TagEntity tagEventEntity = new TagEntity
                                {
                                    RawId = currentTagId.Trim(),
                                    Name = currentTagName,
                                    Rssi = currentTag.Rssi.ToString(),
                                    mZone1Rssi = currentMzone1Rssi,
                                    StatusCode = currentTag.StatusCode,
                                    LastUpdatedOn = DateTime.Now,
                                    ReceivedOn = tagEvent.ReceivedOn,
                                    Battery = currentTag.Battery,
                                    TagType = currentTag.TagType,
                                    //Raw = currentRaw,
                                    MicroZoneCurrent = currentTag.Mzone1.Trim(),
                                    MicroZonePrevious = currentTag.Mzone2.Trim(),
                                    Longitude = currentTag.Longitude.ToString(),
                                    Latitude = currentTag.Latitude.ToString(),
                                    ReaderId = tagEvent.ReaderId,
                                    Version = tagEvent.Version,
                                    Temperature = currentTag.Temperature,
                                    Humidity = currentTag.Humidity
                                };

                                context.TagEntities.Add(tagEventEntity);

                                //get cartId for currentTag
                                CartEntity cart = context.CartEntities.Where(cartItem => cartItem.TagId.Trim() == currentTagId.Trim()).FirstOrDefault();

                                //make an empty card for tag without a card number
                                if (cart == null)
                                {
                                    cart = new CartEntity();
                                    cart.CartNumber = "0";
                                }

                                if (cart.CartNumber != "0" && currentTag.Mzone1 != "000000")
                                {
                                    logBuilder.AppendLine($"Checking for audits for cart {cart.CartNumber}");

                                    //get the latest tag from the audits table for this cart
                                    List<TagAudit> latestAudits = context.TagAudits.Where(audit => audit.CartId == cart.CartNumber).ToList();

                                    //if the list isnt empty, see if we need to add an audit to the table
                                    if (latestAudits.Count > 0)
                                    {
                                        logBuilder.AppendLine($"{latestAudits.Count} audits for cart {cart.CartNumber}");

                                        var orderedList = latestAudits.OrderByDescending(x => x.LastUpdatedOn).ToList();
                                        TagAudit latestAudit = orderedList[0];

                                        logBuilder.AppendLine($"Checking if {latestAudit.CurrentMicroZone} != {currentTag.Mzone1} for cart {cart.CartNumber}");

                                        //compare the latest audit to the currentTag and see if we need to add another audit to the table
                                        if (!latestAudit.CurrentMicroZone.Equals(currentTag.Mzone1))
                                        {
                                            logBuilder.AppendLine($"Checking time difference since the last audit");
                                            TimeSpan timeDiff = tagEventEntity.LastUpdatedOn - latestAudit.LastUpdatedOn;
                                            if (timeDiff.TotalMinutes > 2)
                                            {
                                                logBuilder.AppendLine($"{latestAudit.CurrentMicroZone} != {currentTag.Mzone1} adding audit for cart {cart.CartNumber}");
                                                //if the mZones dont match, then we changed mZones, so add it to the audit table
                                                MicroZoneEntity currentZone = context.MicroZoneEntities.Where(zoneItem => zoneItem.TagAssociationNumber.Trim() == currentTag.Mzone1.Trim()).FirstOrDefault();

                                                TagAudit audit = new TagAudit
                                                {
                                                    CartId = cart.CartNumber,
                                                    CurrentMicroZone = currentTag.Mzone1,
                                                    PreviousMicroZone = currentTag.Mzone2,
                                                    MicroZoneName = currentZone.MicroZoneName,
                                                    MicroZoneNumber = currentZone.MicroZoneNumber,
                                                    LastUpdatedOn = tagEventEntity.LastUpdatedOn,
                                                    Rssi = currentTag.Rssi.ToString()
                                                };

                                                context.TagAudits.Add(audit);
                                            }
                                        }
                                        else
                                            logBuilder.AppendLine($"{latestAudit.CurrentMicroZone} == {currentTag.Mzone1} skipping audit for cart {cart.CartNumber}");
                                    }
                                    else
                                    {
                                        logBuilder.AppendLine($"{latestAudits.Count} audits for cart {cart.CartNumber}. adding new audit");
                                        //if it is empty, this is the first time this cartId has been seen so add it to the audit table
                                        MicroZoneEntity currentZone = context.MicroZoneEntities.Where(zoneItem => zoneItem.TagAssociationNumber.Trim() == currentTag.Mzone1.Trim()).FirstOrDefault();

                                        TagAudit audit = new TagAudit
                                        {
                                            CartId = cart.CartNumber,
                                            CurrentMicroZone = tagEventEntity.MicroZoneCurrent,
                                            PreviousMicroZone = tagEventEntity.MicroZonePrevious,
                                            MicroZoneName = currentZone.MicroZoneName,
                                            MicroZoneNumber = currentZone.MicroZoneNumber,
                                            LastUpdatedOn = tagEventEntity.LastUpdatedOn,
                                            Rssi = tagEventEntity.Rssi
                                        };

                                        context.TagAudits.Add(audit);
                                    }
                                }
                                else
                                {
                                    //if (cart.CartNumber == "0")
                                    //    logBuilder.AppendLine($"Cart Id is 0, skipping audit");
                                    //else
                                    //    logBuilder.AppendLine($"Invalid microzone, skipping audit");
                                }

                                context.SaveChanges();
                            }
                            else
                            {
                                //logBuilder.AppendLine("This is an existing tag.");
                                //logBuilder.AppendLine();

                                //if (currentTag.Mzone1 != "000000")
                                //{
                                //    logBuilder.AppendLine($"This new tag has a valid mZone {currentTag.Mzone1} for tag {currentTagId}");
                                //    logBuilder.AppendLine();

                                //this is now just updating the existing tag with new data instead of inserting a new record every time
                                TagEntity tag = context.TagEntities.Where(tagItem => tagItem.RawId == currentTagId).FirstOrDefault();

                                //get cartId for currentTag
                                CartEntity cart = context.CartEntities.Where(cartItem => cartItem.TagId.Trim() == currentTagId.Trim()).FirstOrDefault();

                                //make an empty card for tag without a card number
                                if (cart == null)
                                {
                                    cart = new CartEntity();
                                    cart.CartNumber = "0";
                                }

                                if (cart.CartNumber != "0" && currentTag.Mzone1 != "000000")
                                {
                                    logBuilder.AppendLine($"Checking for audits for cart {cart.CartNumber}");
                                    //get the latest tag from the audits table for this cart
                                    List<TagAudit> latestAudits = context.TagAudits.Where(audit => audit.CartId == cart.CartNumber).ToList();

                                    //if the list isnt empty, see if we need to add an audit to the table
                                    if (latestAudits.Count > 0)
                                    {
                                        logBuilder.AppendLine($"{latestAudits.Count} audits for cart {cart.CartNumber}");
                                        var orderedList = latestAudits.OrderByDescending(x => x.LastUpdatedOn).ToList();
                                        TagAudit latestAudit = orderedList[0];

                                        logBuilder.AppendLine($"Checking if {latestAudit.CurrentMicroZone} != {currentTag.Mzone1} for cart {cart.CartNumber}");
                                        //compare the latest audit to the currentTag and see if we need to add another audit to the table
                                        if (!latestAudit.CurrentMicroZone.Equals(currentTag.Mzone1))
                                        {
                                            logBuilder.AppendLine($"Checking if time difference between last audit");
                                            TimeSpan timeDiff = DateTime.Now - latestAudit.LastUpdatedOn;
                                            if (timeDiff.TotalMinutes > 2)
                                            {
                                                logBuilder.AppendLine($"{latestAudit.CurrentMicroZone} != {currentTag.Mzone1} adding audit for cart {cart.CartNumber}");
                                                //if the mZones dont match, then we changed mZones, so add it to the audit table
                                                MicroZoneEntity currentZone = context.MicroZoneEntities.Where(zoneItem => zoneItem.TagAssociationNumber == currentTag.Mzone1).FirstOrDefault();

                                                TagAudit audit = new TagAudit
                                                {
                                                    CartId = cart.CartNumber,
                                                    CurrentMicroZone = currentTag.Mzone1,
                                                    PreviousMicroZone = currentTag.Mzone2,
                                                    MicroZoneName = currentZone.MicroZoneName,
                                                    MicroZoneNumber = currentZone.MicroZoneNumber,
                                                    LastUpdatedOn = DateTime.Now,
                                                    Rssi = currentTag.Rssi.ToString()
                                                };

                                                context.TagAudits.Add(audit);

                                            }
                                        }
                                        else
                                            logBuilder.AppendLine($"{latestAudit.CurrentMicroZone} == {currentTag.Mzone1} skipping audit for cart {cart.CartNumber}");
                                    }
                                    else
                                    {
                                        logBuilder.AppendLine($"{latestAudits.Count} audits for cart {cart.CartNumber}. adding a new audit");

                                        //if it is empty, this is the first time this cartId has been seen so add it to the audit table
                                        logBuilder.AppendLine($"checking if {currentTag.Mzone1} exists in microzone table");
                                        MicroZoneEntity currentZone = context.MicroZoneEntities.Where(zoneItem => zoneItem.TagAssociationNumber.Trim() == currentTag.Mzone1.Trim()).FirstOrDefault();

                                        TagAudit audit = new TagAudit
                                        {
                                            CartId = cart.CartNumber,
                                            CurrentMicroZone = currentTag.Mzone1,
                                            PreviousMicroZone = currentTag.Mzone2,
                                            MicroZoneName = currentZone.MicroZoneName,
                                            MicroZoneNumber = currentZone.MicroZoneNumber,
                                            LastUpdatedOn = DateTime.Now,
                                            Rssi = currentTag.Rssi.ToString()
                                        };

                                        context.TagAudits.Add(audit);
                                    }
                                }
                                else
                                {
                                    //if (cart.CartNumber == "0")
                                    //    logBuilder.AppendLine($"Cart Id is 0, skipping audit");
                                    //else
                                    //    logBuilder.AppendLine($"Invalid microzone, skipping audit");
                                }

                                tag.RawId = currentTag.TagId.Trim();
                                tag.Name = currentTagName;
                                tag.Rssi = currentTag.Rssi.ToString();
                                tag.mZone1Rssi = currentMzone1Rssi;
                                tag.StatusCode = currentTag.StatusCode;
                                tag.LastUpdatedOn = DateTime.Now;
                                tag.ReceivedOn = tagEvent.ReceivedOn;
                                tag.Battery = currentTag.Battery;
                                tag.TagType = currentTag.TagType;
                                // tag.Raw = currentRaw;
                                tag.MicroZoneCurrent = currentTag.Mzone1.Trim();
                                tag.MicroZonePrevious = currentTag.Mzone2.Trim();

                                //only update lat/lon if it isn't true zero
                                if (currentTag.Longitude.ToString() != "0" && currentTag.Longitude.ToString() != "")
                                    tag.Longitude = currentTag.Longitude.ToString();
                                if (currentTag.Latitude.ToString() != "0" && currentTag.Latitude.ToString() != "")
                                    tag.Latitude = currentTag.Latitude.ToString();

                                tag.ReaderId = tagEvent.ReaderId;
                                tag.Version = tagEvent.Version;
                                tag.Temperature = currentTag.Temperature;
                                tag.Humidity = currentTag.Humidity;

                                System.Diagnostics.Trace.TraceInformation($"DataAccessManager AddTag() updating existing tag locations with: Lat{currentTag.Latitude}, Lon{currentTag.Longitude}");
                                //logBuilder.AppendLine($"Updating tag in database {currentTag.Raw}");
                                //logBuilder.AppendLine();

                                context.SaveChanges();
                                //logBuilder.AppendLine();
                                //logBuilder.AppendLine();
                            }
                            context.SaveChanges();
                        }
                    }
                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            string dbErrorpath = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~"), "LogFile");
                            System.IO.File.WriteAllText(dbErrorpath + ".txt", logBuilder.ToString());
                            Trace.TraceError($"Legacy AddTag DbException: {validationError.ErrorMessage}");
                            return String.Format("Property: {0} Error: {1}",
                                                    validationError.PropertyName,
                                                    validationError.ErrorMessage);


                        }
                    }
                }
                catch (Exception e)
                {
                    Trace.TraceError($"Legacy AddTag Exception: {e.Message} Stack Trace: {e.StackTrace}, Inner Exception {e.InnerException}, Data {e.Data}, Source {e.Source}");
                    logBuilder.AppendLine(e.ToString());
                    logBuilder.AppendLine();
                    string exceptionpath = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~"), "LogFile");
                    logBuilder.AppendLine(exceptionpath);
                    logBuilder.AppendLine();
                    //System.IO.File.WriteAllText(exceptionpath + ".txt", logBuilder.ToString());
                }
            }
            catch (Exception error)
            {
                logBuilder.AppendLine(error.ToString());
            }

            Trace.TraceInformation("VypinApi.DataAccessLayer.DataAccessManager: Ending AddTag");

            logBuilder.AppendLine("Api.DataAccessLayer.DataAccessManager: Ending AddTag");
            logBuilder.AppendLine();

            //string path = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~"), "LogFile");
            //System.IO.File.WriteAllText(path + ".txt", logBuilder.ToString());
            return logBuilder.ToString();
        }

        public string AddTagType(TagType tagType)
        {
            //Trace.TraceInformation("VypinApi.DataAccessLayer.DataAccessManager: Beginning AddTagType");

            try
            {
                using (var context = new AssetEntities())
                {
                    context.TagTypes.Add(tagType);
                    context.SaveChanges();
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        return String.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
            }
            catch (Exception e)
            {
                return String.Format("Exception: {0}", e.ToString());
            }

            //Trace.TraceInformation("VypinApi.DataAccessLayer.DataAccessManager: Ending AddTagType");

            return "success";
        }
        public string AddTagCategory(TagCategory tagCategory)
        {
            //Trace.TraceInformation("VypinApi.DataAccessLayer.DataAccessManager: Beginning AddTagCategory");

            try
            {
                using (var context = new AssetEntities())
                {
                    context.TagCategories.Add(tagCategory);
                    context.SaveChanges();
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        return String.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
            }
            catch (Exception e)
            {
                return String.Format("Exception: {0}", e.ToString());
            }

            //Trace.TraceInformation("VypinApi.DataAccessLayer.DataAccessManager: Ending AddTagCategory");

            return "success";
        }

        public List<TagType> GetTagTypes()
        {
            //Trace.TraceInformation("VypinApi.DataAccessLayer.DataAccessManager: Beginning GetTagTypes");

            List<TagType> tags = new List<TagType>();
            try
            {
                using (var context = new AssetEntities())
                {
                    tags.AddRange(context.TagTypes.ToList());
                }
            }
            catch (Exception e)
            {
                //	Trace.TraceError("GetTagTypes Exception" + e.ToString());
            }

            //Trace.TraceInformation("VypinApi.DataAccessLayer.DataAccessManager: Ending GetTagTypes");
            return tags;
        }

        public List<TagCategory> GetTagCategories()
        {
            //Trace.TraceInformation("VypinApi.DataAccessLayer.DataAccessManager: Beginning GetTagCategories");

            List<TagCategory> tags = new List<TagCategory>();
            try
            {
                using (var context = new AssetEntities())
                {
                    tags.AddRange(context.TagCategories.AsNoTracking().ToList());
                }
            }
            catch (Exception e)
            {
                //Trace.TraceError("GetTagCategories Exception" + e.ToString());
            }

            //Trace.TraceInformation("VypinApi.DataAccessLayer.DataAccessManager: Ending GetTagCategories");
            return tags;
        }

        public string DeleteTagType(string tagType)
        {
            //Trace.TraceInformation("VypinApi.DataAccessLayer.DataAccessManager: Beginning DeleteTagType");

            try
            {
                using (var context = new AssetEntities())
                {
                    TagType tagTypeExisting = context.TagTypes.Where(tagItem => tagItem.Type == tagType).FirstOrDefault();

                    if (tagTypeExisting != null)
                    {
                        context.TagTypes.Remove(tagTypeExisting);
                        context.SaveChanges();

                        return "success";
                    }
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        //Trace.TraceError("DeleteTagType Exception" + validationError.ErrorMessage);
                        return String.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
            }
            catch (Exception e)
            {
                //Trace.TraceError("DeleteTagType Exception" + e.ToString());
                return String.Format("Exception: {0}", e.ToString());
            }

            return "success";
        }

        public string DeleteTagCategory(string tagCategory)
        {
            //Trace.TraceInformation("VypinApi.DataAccessLayer.DataAccessManager: Beginning DeleteTagCategory");

            try
            {
                using (var context = new AssetEntities())
                {
                    TagCategory tagCategoryExisting = context.TagCategories.Where(tagItem => tagItem.Category == tagCategory).FirstOrDefault();

                    if (tagCategoryExisting != null)
                    {
                        context.TagCategories.Remove(tagCategoryExisting);
                        context.SaveChanges();
                    }
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        //Trace.TraceError("DeleteTagCategory Exception" + validationError.ErrorMessage);
                        return String.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
            }
            catch (Exception e)
            {
                //Trace.TraceError("DeleteTagCategory Exception" + e.ToString());
                return String.Format("Exception: {0}", e.ToString());
            }

            return "success";
        }


        private TagEntity GetTagByName(DbSet<TagEntity> tagEntities, string tagName)
        {
            TagEntity tag = null;

            if (String.IsNullOrEmpty(tagName) == false)
            {
                tag = (from tagItem in tagEntities
                       where tagItem.Name.ToLower() == tagName
                       select tagItem).FirstOrDefault();
            }

            return tag;
        }

        private Asset GetAssetByAssetId(DbSet<Asset> assetEntities, string assetId)
        {
            Asset asset = null;

            if (String.IsNullOrEmpty(assetId) == false)
            {
                asset = (from assetItem in assetEntities
                         where assetItem.AssetId.ToString() == assetId
                         select assetItem).FirstOrDefault();
            }

            return asset;
        }

        private Asset GetAssetByName(DbSet<Asset> assetEntities, string name)
        {
            Asset asset = null;

            if (String.IsNullOrEmpty(name) == false)
            {
                asset = (from assetItem in assetEntities
                         where assetItem.Name.ToString() == name
                         select assetItem).FirstOrDefault();
            }

            return asset;
        }

        private Asset GetAssetBySlotId(DbSet<Asset> assetEntities, string slotId)
        {
            Asset asset = null;

            if (String.IsNullOrEmpty(slotId) == false)
            {
                asset = (from assetItem in assetEntities
                         where assetItem.SlotId.ToString() == slotId
                         select assetItem).FirstOrDefault();
            }

            return asset;
        }

        private Asset GetAssetByAssetIdentifier(DbSet<Asset> assetEntities, string assetIdentifier)
        {
            Asset asset = null;

            if (String.IsNullOrEmpty(assetIdentifier) == false)
            {
                asset = (from assetItem in assetEntities
                         where assetItem.AssetIdentifier.ToLower() == assetIdentifier
                         select assetItem).FirstOrDefault();
            }

            return asset;
        }

        private Asset GetAssetByTagId(DbSet<Asset> assetEntities, string tagId)
        {
            Asset asset = null;

            if (String.IsNullOrEmpty(tagId) == false)
            {
                asset = (from assetItem in assetEntities
                         where assetItem.TagId.ToString() == tagId
                         select assetItem).FirstOrDefault();
            }

            return asset;
        }

        public List<Asset> GetAssociatedAssetsByTagId(int tagId)
        {
            List<Asset> assets = new List<Asset>();

            try
            {
                using (var context = new AssetEntities())
                {
                    assets = context.Assets.Where(asset => asset.TagId == tagId).ToList();
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Failed to GetAssociatedAssets by TagId due to error: " + e.Message);
                Trace.TraceError("InnerException: " + e.InnerException);
            }

            return assets;
        }

        private TagEntity GetTagByRawId(DbSet<TagEntity> tagEntities, string rawId)
        {
            TagEntity tag = null;

            if (String.IsNullOrEmpty(rawId) == false)
            {
                tag = (from tagItem in tagEntities
                       where tagItem.RawId.ToLower() == rawId
                       select tagItem).FirstOrDefault();
            }

            return tag;
        }

        private TagEntity GetTagById(DbSet<TagEntity> tagEntities, string tagId)
        {
            TagEntity tag = null;

            if (String.IsNullOrEmpty(tagId) == false)
            {
                tag = (from tagItem in tagEntities
                       where tagItem.Id.ToString().ToLower() == tagId
                       select tagItem).FirstOrDefault();
            }

            return tag;
        }

        private static void AssignLatitudeAndLongitude(LegacyTag recentTag, TagEntity tagEventExisting, out string latitude, out string longitude)
        {
            Trace.TraceInformation($"VypinApi.DataAccessLayer.DataAccessManager: Beginning ProcessTagLocation for {recentTag.TagId}");
            if (tagEventExisting != null && (String.IsNullOrEmpty(recentTag.Latitude.ToString()) == true || String.IsNullOrEmpty(recentTag.Longitude.ToString()) == true))
            {
                Trace.TraceInformation($"VypinApi.DataAccessLayer.DataAccessManager: Found Tag Lat: {tagEventExisting.Latitude}");
                Trace.TraceInformation($"VypinApi.DataAccessLayer.DataAccessManager: Found Tag Lon: {tagEventExisting.Longitude}");
                latitude = String.IsNullOrEmpty(tagEventExisting.Latitude) == true ? String.Empty : tagEventExisting.Latitude;
                longitude = String.IsNullOrEmpty(tagEventExisting.Longitude) == true ? String.Empty : tagEventExisting.Longitude;
            }
            else
            {
                Trace.TraceInformation($"VypinApi.DataAccessLayer.DataAccessManager: New Lat: {recentTag.Latitude}");
                Trace.TraceInformation($"VypinApi.DataAccessLayer.DataAccessManager: New Lon: {recentTag.Longitude}");
                latitude = recentTag.Latitude.ToString();
                longitude = recentTag.Longitude.ToString();
            }
        }

        private TagEntity BuildTagEntity(LegacyTag tag, DateTime tagReceivedOn, string microZoneCurrent, string microZonePrevious)
        {
            string name = "";

            if (tag.TagName != null)
                name = tag.TagName;

            TagEntity tagEventEntity = new TagEntity
            {
                RawId = tag.TagId,
                Name = name,
                Rssi = tag.Rssi.ToString(),
                mZone1Rssi = tag.Mzone1Rssi.ToString(),
                StatusCode = tag.StatusCode,
                LastUpdatedOn = DateTime.Now,
                ReceivedOn = tagReceivedOn,
                Battery = tag.Battery,
                TagType = tag.TagType,
                Raw = "",
                MicroZoneCurrent = microZoneCurrent,
                MicroZonePrevious = microZonePrevious,
                Longitude = tag.Longitude.ToString(),
                Latitude = tag.Latitude.ToString(),
                ReaderId = tag.ReaderId
            };

            return tagEventEntity;
        }

        private TagHistory BuildTagHistory(LegacyTag tag, DateTime tagReceivedOn, string microZoneCurrent, string microZonePrevious)
        {
            string name = "";

            if (tag.TagName != null)
                name = tag.TagName;

            TagHistory tagEventEntity = new TagHistory
            {
                RawId = tag.TagId,
                Name = name,
                Rssi = tag.Rssi.ToString(),
                mZone1Rssi = tag.Mzone1Rssi.ToString(),
                StatusCode = tag.StatusCode,
                LastUpdatedOn = DateTime.Now,
                ReceivedOn = tagReceivedOn,
                Battery = tag.Battery,
                TagType = tag.TagType,
                Raw = "",
                MicroZoneCurrent = microZoneCurrent,
                MicroZonePrevious = microZonePrevious,
                Longitude = tag.Longitude.ToString(),
                Latitude = tag.Latitude.ToString(),
                ReaderId = tag.ReaderId
            };

            return tagEventEntity;
        }

        private string SwapBytes(string bytes)
        {
            string convertedHexValue = String.Empty;

            if (String.IsNullOrEmpty(bytes) == true)
            {
                return String.Empty;
            }

            try
            {
                ulong value = Convert.ToUInt64(bytes, 16);

                ulong convertedValue = (value & 0x00000000000000FFUL) << 56 | (value & 0x000000000000FF00UL) << 40 |
                                       (value & 0x0000000000FF0000UL) << 24 | (value & 0x00000000FF000000UL) << 8 |
                                       (value & 0x000000FF00000000UL) >> 8 | (value & 0x0000FF0000000000UL) >> 24 |
                                       (value & 0x00FF000000000000UL) >> 40 | (value & 0xFF00000000000000UL) >> 56;

                convertedHexValue = String.Format("{0:X6}", convertedValue).Substring(0, 6); // Only want the first 3 bytes of value
            }
            catch (Exception e)
            {
                //Trace.TraceError($"VypinApi.DataAccessLayer.DataAccessManager: Exception SwapBytes {Environment.NewLine + e.ToString()}");

            }

            return convertedHexValue;
        }
    }
}