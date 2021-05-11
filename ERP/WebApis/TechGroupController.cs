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
using System.Configuration;

namespace ERP.WebApis
{
    public class TechGroupController : ApiController
    {
        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Technology Group";

        public TechGroupController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }

        /// <summary>
        /// POST api/techgroup
        /// create and update record
        /// </summary>
        [HttpPost]
        public ApiResponse SaveTechGroup(tblTechnologiesGroup techGroup)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (techGroup.Id == 0)
                    {
                        // Mode == Add
                        tblTechnologiesGroup t = new tblTechnologiesGroup
                        {
                            TechnologiesGroup = techGroup.TechnologiesGroup,
                            IsActive = techGroup.IsActive,
                            CreDate = DateTime.Now.ToUniversalTime(),
                            ChgDate = DateTime.Now.ToUniversalTime()
                        };
                        db.tblTechnologiesGroups.Add(t);
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);
                    }
                    else
                    {
                        //if (!DependancyStatus.TechnologiesGroupStatus(techGroup.Id))
                        //{
                        //    return apiResponse = ERPUtilities.GenerateApiResponse(true, 2, generalMessages.msgStatusError, null);
                        //}
                        // Mode == Edit
                        var line = db.tblTechnologiesGroups.Where(z => z.Id == techGroup.Id).SingleOrDefault();
                        if (line != null)
                        {
                            line.TechnologiesGroup = techGroup.TechnologiesGroup;
                            //line.IsActive = techGroup.IsActive;
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
        /// POST api/techgroup
        /// delete technology group 
        /// </summary>
        [HttpPost]
        public ApiResponse DeleteTechGroup([FromBody]int id)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (DependancyStatus.TechnologiesGroupStatus(id))
                    {
                        var line = db.tblTechnologiesGroups.Where(z => z.Id == id).SingleOrDefault();
                        if (line != null)
                        {
                            db.tblTechnologiesGroups.Remove(line);
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
        /// POST api/techgroup
        /// active-inActive record 
        /// </summary>
        [HttpPost]
        public ApiResponse ChangeStatus(tblTechnologiesGroup tech)
        {

            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (DependancyStatus.TechnologiesGroupStatus(tech.Id))
                    {
                        var line = db.tblTechnologiesGroups.Where(z => z.Id == tech.Id).SingleOrDefault();
                        if (line != null)
                        {
                            if (tech.IsActive)
                            {
                                line.IsActive = false;
                            }
                            else if (!tech.IsActive)
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
        /// GET api/techgroup
        /// return technology group list with sorting and filtering  functionalities
        /// </summary>
        [HttpGet]
        public ApiResponse GetTechGroupList()
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

                    List<tblTechnologiesGroup> list = null;

                    try
                    {
                        list = db.tblTechnologiesGroups.ToList();

                        //1. filter data
                        if (!string.IsNullOrEmpty(filter) && filter != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.TechnologiesGroup.ToLower().Contains(filter.ToLower())).ToList();
                        }
                        //2. do sorting on list
                        list = DoSorting(list, orderBy.Trim());

                        //3. take total count to return for ng-table
                        var Count = list.Count();

                        //4. convert returned datetime to local timezone
                        var TechGroup = list.Select(i =>
                        {
                            i.CreDate = Convert.ToDateTime(i.CreDate).AddMinutes(-1 * timezone);
                            i.ChgDate = Convert.ToDateTime(i.ChgDate).AddMinutes(-1 * timezone);
                            return i;
                        }).Skip(iDisplayStart).Take(iDisplayLength).ToList();


                        var resultData = new
                        {
                            total = Count,
                            result = TechGroup.ToList()
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
        public List<tblTechnologiesGroup> DoSorting(IEnumerable<tblTechnologiesGroup> list, string orderBy)
        {
            try
            {
                if (orderBy == "TechnologiesGroup")
                {
                    list = list.OrderBy(z => z.TechnologiesGroup).ToList();
                }
                else if (orderBy == "-TechnologiesGroup")
                {
                    list = list.OrderByDescending(z => z.TechnologiesGroup).ToList();
                }
                else if (orderBy == "ChgDate")
                {
                    list = list.OrderBy(z => z.ChgDate).ToList();
                }
                else if (orderBy == "-ChgDate")
                {
                    list = list.OrderByDescending(z => z.ChgDate).ToList();
                }
                return list.ToList<tblTechnologiesGroup>();
            }
            catch
            {
                return null;
            }
        }



        /// <summary>
        /// List Of Active Technology Groups
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<tblTechnologiesGroup> GetActiveTechnologiesGroup()
        {
            try
            {
                return db.tblTechnologiesGroups.Where(z => z.IsActive == true).OrderBy(z => z.TechnologiesGroup).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}