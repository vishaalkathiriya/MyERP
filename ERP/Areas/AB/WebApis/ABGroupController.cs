using ERP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ERP.Utilities;
using System.Collections.Specialized;
using System.Web;

namespace ERP.WebApis
{
    public class ABGroupController : ApiController
    {
        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Group";
        
        public ABGroupController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }

        /// <summary>
        /// Create / Update Group
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResponse CreateUpdateGroup(tblABGroup group)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    // CREATE MODE
                    if (group.GroupId == 0)
                    {
                        tblABGroup tbl = new tblABGroup
                        {
                            GroupName = group.GroupName,
                            GroupNote = group.GroupNote,
                            CreBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString()),
                            ChgBy=  Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString()),
                            CreDate = DateTime.Now.ToUniversalTime(),
                            ChgDate = DateTime.Now.ToUniversalTime()
                        };
                        db.tblABGroups.Add(tbl);
                       
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);
                        
                    }
                    else
                    {
                        // UPDATE MODE
                        
                        tblABGroup tbl = db.tblABGroups.Where(z => z.GroupId == group.GroupId).FirstOrDefault();
                        tbl.GroupName = group.GroupName;
                        tbl.GroupNote = group.GroupNote;
                        tbl.ChgDate = DateTime.Now.ToUniversalTime();
                        tbl.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
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
                ERPUtilities.UnAuthorizedAccess(apiResponse);
            }
            return apiResponse;
        }

        /// <summary>
        /// Retrieive list Group.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiResponse RetrieveGroup()
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
                    List<tblABGroup> list = null;

                    try
                    {
                        list = db.tblABGroups.ToList();

                        // FILTERING DATA
                        if (!string.IsNullOrEmpty(filter) && filter != "undefined")
                        {
                          
                            list = list.Where(z => z.GroupName.ToLower().Contains(filter.ToLower())).ToList();
                        }

                        // SORTING DATA
                        list = DoSorting(list, orderBy.Trim());

                        // TAKE TOTAL COUNT TO RETURN FOR NG-TABLE
                        var Count = list.Count();

                        // CONVERT RETURNED DATETIME TO LOCAL TIMEZONE
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
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 0, generalMessages.msgError, null);
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
        /// Return sorted list based on passed column
        /// </summary>
        public List<tblABGroup> DoSorting(IEnumerable<tblABGroup> list, string orderBy)
        {
            try
            {
                if (orderBy == "GroupName")
                {
                    list = list.OrderBy(z => z.GroupName).ToList();
                }
                else if (orderBy == "-GroupName")
                {
                    list = list.OrderByDescending(z => z.GroupName).ToList();
                }
                else if (orderBy == "ChgDate")
                {
                    list = list.OrderBy(z => z.ChgDate).ToList();
                }
                else if (orderBy == "-ChgDate")
                {
                    list = list.OrderByDescending(z => z.ChgDate).ToList();
                }
                return list.ToList<tblABGroup>();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Delete Group by its primary key[GroupId]
        /// </summary>
        /// <param name="GroupId"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResponse DeleteGroup([FromBody]int GroupId)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                var line = db.tblABGroups.Where(z => z.GroupId == GroupId).SingleOrDefault();
                if (line != null)
                {
                    db.tblABGroups.Remove(line);
                    db.SaveChanges();
                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgDelete, null);
                }

            }
            catch (Exception ex)
            {
                apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
            }
            return apiResponse;
        
        }

        // Get ALL Group
        [HttpGet]
        public ApiResponse GetGroupList()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                using (db)
                {

                    try
                    {
                        var groupList = db.tblABGroups.ToList();

                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", groupList);
                    }
                    catch (Exception ex)
                    {
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 0, generalMessages.msgError, null);
                    }
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }
            return apiResponse;
        }
    }
}
