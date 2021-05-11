using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ERP.Models;
using System.Text;
using System.Collections.Specialized;
using System.Web;
using ERP.Utilities;
using System.Data.Entity.Infrastructure;

namespace ERP.WebApis
{
    public class DesignationGroupController : ApiController
    {
        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Designation Group";

        public DesignationGroupController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }

        /// <summary>
        /// POST api/DesignationGroup
        /// create and update record
        /// </summary>
        [HttpPost]
        public ApiResponse SaveDesignationGroup(tblDesignationGroup des)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (des.Id == 0)
                    {// Mode == Add
                        tblDesignationGroup d = new tblDesignationGroup
                        {
                            DesignationGroup = des.DesignationGroup,
                            IsActive = des.IsActive,
                            CreDate = DateTime.Now.ToUniversalTime(),
                            ChgDate = DateTime.Now.ToUniversalTime()
                        };
                        db.tblDesignationGroups.Add(d);
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);
                    }
                    else
                    {
                        //if (!DependancyStatus.DesignationGroupStatus(des.Id))
                        //{
                        //    return apiResponse = ERPUtilities.GenerateApiResponse(true, 2, generalMessages.msgStatusError, null);
                        //}
                        // Mode == Edit
                        var line = db.tblDesignationGroups.Where(z => z.Id == des.Id).SingleOrDefault();
                        if (line != null)
                        {
                            line.DesignationGroup = des.DesignationGroup;
                            //line.IsActive = des.IsActive;
                            line.ChgDate = DateTime.Now.ToUniversalTime();
                        }
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgUpdate, null);
                    }
                    db.SaveChanges();
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

        /// <summary>
        /// POST api/DesignationGroup
        /// delete designation group 
        /// </summary>
        [HttpPost]
        public ApiResponse DeleteDesignationGroup([FromBody]int id)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (DependancyStatus.DesignationGroupStatus(id))
                    {
                        var line = db.tblDesignationGroups.Where(z => z.Id == id).SingleOrDefault();
                        if (line != null)
                        {
                            db.tblDesignationGroups.Remove(line);
                            db.SaveChanges();
                            apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgDelete, null);
                        }
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

        /// <summary>
        /// POST api/DesignationGroup
        /// active-inActive record 
        /// </summary>
        [HttpPost]
        public ApiResponse ChangeStatus(tblDesignationGroup des)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (DependancyStatus.DesignationGroupStatus(des.Id))
                    {
                        var line = db.tblDesignationGroups.Where(z => z.Id == des.Id).SingleOrDefault();
                        if (line != null)
                        {
                            if (des.IsActive)
                            {
                                line.IsActive = false;
                            }
                            else if (!des.IsActive)
                            {
                                line.IsActive = true;
                            }
                        }

                        line.ChgDate = DateTime.Now.ToUniversalTime();
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
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }


        /// <summary>
        /// GET api/DesignationGroup
        /// return designation group list with sorting and filtering  functionalities
        /// </summary>
        [HttpGet]
        public ApiResponse GetDesignationGroupList()
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

                    List<tblDesignationGroup> list = null;

                    try
                    {
                        list = db.tblDesignationGroups.ToList();

                        //1. filter data
                        if (!string.IsNullOrEmpty(filter) && filter != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.DesignationGroup.ToLower().Contains(filter.ToLower())).ToList();
                        }
                        //2. do sorting on list
                        list = DoSorting(list, orderBy.Trim());

                        //3. take total count to return for ng-table
                        var Count = list.Count();

                        //4. convert returned datetime to local timezone
                        var DesignationGroup = list.Select(i =>
                        {
                            i.CreDate = Convert.ToDateTime(i.CreDate).AddMinutes(-1 * timezone);
                            i.ChgDate = Convert.ToDateTime(i.ChgDate).AddMinutes(-1 * timezone);
                            return i;
                        }).Skip(iDisplayStart).Take(iDisplayLength).ToList();


                        var resultData = new
                        {
                            total = Count,
                            result = DesignationGroup.ToList()
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

        /// <summary>
        /// return sorted list based on passed column
        /// </summary>
        public List<tblDesignationGroup> DoSorting(IEnumerable<tblDesignationGroup> list, string orderBy)
        {
            try
            {
                if (orderBy == "DesignationGroup")
                {
                    list = list.OrderBy(z => z.DesignationGroup).ToList();
                }
                else if (orderBy == "-DesignationGroup")
                {
                    list = list.OrderByDescending(z => z.DesignationGroup).ToList();
                }
                else if (orderBy == "ChgDate")
                {
                    list = list.OrderBy(z => z.ChgDate).ToList();
                }
                else if (orderBy == "-ChgDate")
                {
                    list = list.OrderByDescending(z => z.ChgDate).ToList();
                }
                return list.ToList<tblDesignationGroup>();
            }
            catch
            {
                return null;
            }
        }
    }
}
