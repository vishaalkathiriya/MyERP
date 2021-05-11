using ERP.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using ERP.Utilities;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;

namespace ERP.WebApis
{
    public class LocationController : ApiController
    {
         ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Location";

        public LocationController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }
        // GET api/<controller>

        [HttpGet]
        public ApiResponse GetLocList()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                using (db)
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);

                    int page = Convert.ToInt32(nvc["page"]);
                    int timezone = Convert.ToInt32(nvc["timezone"]);
                    string orderBy = nvc["orderby"];
                    string filter = nvc["filter"];

                    int iDisplayLength = Convert.ToInt32(nvc["count"]);
                    int iDisplayStart = (page - 1) * iDisplayLength;
                    List<tblLocation> list = null;

                    try
                    {
                        list = db.tblLocations.ToList();

                        //1. filter data
                        if (!string.IsNullOrEmpty(filter) && filter != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.LocationName.ToLower().Contains(filter.ToLower())).ToList();
                        }

                        //2. do sorting on list
                        list = DoSorting(list, orderBy.Trim());

                        //3. take total count to return for ng-table
                        var Count = list.Count();

                        //4. convert returned datetime to local timezone
                        list = list.Select(i =>
                        {
                            i.CreDate = Convert.ToDateTime(i.CreDate).AddMinutes(-1 * timezone);
                            i.ChgDate = Convert.ToDateTime(i.ChgDate).AddMinutes(-1 * timezone);
                            return i;
                        }).Skip(iDisplayStart).Take(iDisplayLength).ToList();

                        var resultData = new
                        {
                            total = Count,
                            result = list.ToList()
                        };

                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", resultData);
                    }
                    catch (Exception ex)
                    {
                        apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
                    }
                    finally
                    {
                        list = null;
                    }
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }
            return apiResponse;
        }

        // POST api/<controller>
        [HttpPost]
        public IEnumerable<tblLocation> GetLocation(tblLocation location)
        {
            try
            {
                return db.tblLocations.Where(z => z.LocationId == location.LocationId).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        // POST api/<controller>
        [HttpPost]
        public ApiResponse SaveLocation(tblLocation location)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                tblLocation tbl = new tblLocation();
                try
                {
                    if (location.LocationId > 0)
                    {
                        tbl = db.tblLocations.Where(x => x.LocationId == location.LocationId).FirstOrDefault();
                        //if (location.IsActive == false)
                        //{
                        //    if (!DependancyStatus.LocationStatus(location.LocationId))
                        //    {
                        //        return apiResponse = ERPUtilities.GenerateApiResponse(true, 2, generalMessages.msgStatusError, null);
                        //    }
                        //}
                    }
                    tbl.LocationName = location.LocationName;
                    tbl.ChgDate = DateTime.Now.ToUniversalTime();
                    if (location.LocationId <= 0)
                    {
                        tbl.IsActive = location.IsActive;
                        tbl.CreDate = DateTime.Now.ToUniversalTime();
                        db.tblLocations.Add(tbl);
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);
                    }
                    else
                    {
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgUpdate, null);
                    }
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
                }
                finally
                {
                    tbl = null;
                }
            }
            else
            {
                ERPUtilities.UnAuthorizedAccess(apiResponse);
            }
            return apiResponse;
        }

        // POST api/<controller>
        [HttpPost]
        public ApiResponse DeleteLocation(tblLocation location)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (DependancyStatus.LocationStatus(location.LocationId))
                    {
                        var loc = db.tblLocations.Where(z => z.LocationId == location.LocationId).SingleOrDefault();
                        if (loc != null)
                            db.tblLocations.Remove(loc);

                        db.SaveChanges();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgDelete, null);
                    }
                    else
                    {
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 2, generalMessages.msgParentExists, null);
                    }
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }
            return apiResponse;
        }

        // POST api/<controller>
        [HttpPost]
        public ApiResponse ChangeStatus(tblLocation location)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                tblLocation tbl;
                try
                {
                    if (DependancyStatus.LocationStatus(location.LocationId))
                    {
                        tbl = db.tblLocations.Where(z => z.LocationId == location.LocationId).FirstOrDefault();
                        if (tbl != null)
                        {
                            if (tbl.IsActive)
                                tbl.IsActive = false;
                            else if (!tbl.IsActive)
                                tbl.IsActive = true;
                        }
                        tbl.ChgDate = DateTime.Now.ToUniversalTime();
                        db.SaveChanges();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgChangeStatus, null);
                    }
                    else
                    {
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 2, generalMessages.msgStatusError, null);
                    }
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
                }
                finally
                {
                    tbl = null;
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }

        /// <summary>
        /// return sorted list based on passed column
        /// </summary>
        public List<tblLocation> DoSorting(List<tblLocation> list, string orderBy)
        {
            try
            {
                if (orderBy == "LocationName")
                {
                    list = list.OrderBy(z => z.LocationName).ToList();
                }
                else if (orderBy == "-LocationName")
                {
                    list = list.OrderByDescending(z => z.LocationName).ToList();
                }
                else if (orderBy == "ChgDate")
                {
                    list = list.OrderBy(z => z.ChgDate).ToList();
                }
                else if (orderBy == "-ChgDate")
                {
                    list = list.OrderByDescending(z => z.ChgDate).ToList();
                }
                return list;
            }
            catch
            {
                return null;
            }
        }
    }
}
