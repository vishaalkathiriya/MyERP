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
    public class DesignationParentController : ApiController
    {
        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Designation Parent";

        public DesignationParentController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }

        /// <summary>
        /// POST api/DesignationParent
        /// create and update record
        /// </summary>
        [HttpPost]
        public ApiResponse SaveDesignationParent(tblDesignationParent des)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (des.Id == 0)
                    {// Mode == Add
                        tblDesignationParent d = new tblDesignationParent
                        {
                            DesignationParent = des.DesignationParent,
                            IsActive = des.IsActive,
                            CreDate = DateTime.Now.ToUniversalTime(),
                            ChgDate = DateTime.Now.ToUniversalTime()
                        };
                        db.tblDesignationParents.Add(d);
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);
                    }
                    else
                    {
                        //if (!DependancyStatus.DesignationParentStatus(des.Id))
                        //{
                        //    return apiResponse = ERPUtilities.GenerateApiResponse(true, 2, generalMessages.msgStatusError, null);
                        //}
                        // Mode == Edit
                        var line = db.tblDesignationParents.Where(z => z.Id == des.Id).SingleOrDefault();
                        if (line != null)
                        {
                            line.DesignationParent = des.DesignationParent;
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
        /// POST api/DesignationParent
        /// delete designation parent 
        /// </summary>
        [HttpPost]
        public ApiResponse DeleteDesignationParent([FromBody]int id)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (DependancyStatus.DesignationParentStatus(id))
                    {
                        var line = db.tblDesignationParents.Where(z => z.Id == id).SingleOrDefault();

                        if (line != null)
                        {
                            db.tblDesignationParents.Remove(line);
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
        /// POST api/DesignationParent
        /// active-inActive record 
        /// </summary>
        [HttpPost]
        public ApiResponse ChangeStatus(tblDesignationParent des)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (DependancyStatus.DesignationParentStatus(des.Id))
                    {
                        var line = db.tblDesignationParents.Where(z => z.Id == des.Id).SingleOrDefault();
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
        /// GET api/DesignationParent
        /// return designation parent list with sorting and filtering  functionalities
        /// </summary>
        [HttpGet]
        public ApiResponse GetDesignationParentList()
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

                    List<tblDesignationParent> list = null;

                    try
                    {
                        list = db.tblDesignationParents.ToList();

                        //1. filter data
                        if (!string.IsNullOrEmpty(filter) && filter != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.DesignationParent.ToLower().Contains(filter.ToLower())).ToList();
                        }
                        //2. do sorting on list
                        list = DoSorting(list, orderBy.Trim());

                        //3. take total count to return for ng-table
                        var Count = list.Count();

                        //4. convert returned datetime to local timezone
                        var DesignationParent = list.Select(i =>
                        {
                            i.CreDate = Convert.ToDateTime(i.CreDate).AddMinutes(-1 * timezone);
                            i.ChgDate = Convert.ToDateTime(i.ChgDate).AddMinutes(-1 * timezone);
                            return i;
                        }).Skip(iDisplayStart).Take(iDisplayLength).ToList();


                        var resultData = new
                        {
                            total = Count,
                            result = DesignationParent.ToList()
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
        public List<tblDesignationParent> DoSorting(IEnumerable<tblDesignationParent> list, string orderBy)
        {
            try
            {
                if (orderBy == "DesignationParent")
                {
                    list = list.OrderBy(z => z.DesignationParent).ToList();
                }
                else if (orderBy == "-DesignationParent")
                {
                    list = list.OrderByDescending(z => z.DesignationParent).ToList();
                }
                else if (orderBy == "ChgDate")
                {
                    list = list.OrderBy(z => z.ChgDate).ToList();
                }
                else if (orderBy == "-ChgDate")
                {
                    list = list.OrderByDescending(z => z.ChgDate).ToList();
                }
                return list.ToList<tblDesignationParent>();
            }
            catch
            {
                return null;
            }
        }

    }
}
