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
using System.Configuration;
using System.Data.Entity.Infrastructure;

namespace ERP.WebApis
{
    public class TechnologyController : ApiController
    {
        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Technology";

        public TechnologyController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }

        /// <summary>
        /// POST api/technology
        /// create and update record
        /// </summary>
        [HttpPost]
        public ApiResponse SaveTechnology(tblTechnology tech)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (tech.Id == 0) // Mode == Add
                    {
                        tblTechnology t = new tblTechnology
                        {
                            Technologies = tech.Technologies,
                            TechnologiesGroupId = tech.TechnologiesGroupId,
                            IsActive = tech.IsActive,
                            CreDate = DateTime.Now.ToUniversalTime(),
                            ChgDate = DateTime.Now.ToUniversalTime()
                        };
                        db.tblTechnologies.Add(t);
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);
                    }
                    else
                    { // Mode == Edit
                        var line = db.tblTechnologies.Where(z => z.Id == tech.Id).SingleOrDefault();
                        if (line != null)
                        {
                            line.Technologies = tech.Technologies;
                            line.TechnologiesGroupId = tech.TechnologiesGroupId;
                            line.IsActive = tech.IsActive;
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
        /// GET api/technology
        /// retrieve technology group list 
        /// </summary>
        [HttpGet]
        public ApiResponse RetrieveTechGroupList()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var list = db.tblTechnologiesGroups.OrderBy(z => z.TechnologiesGroup)
                        .Where(z => z.IsActive == true)
                        .Select(z => new SelectItemModel
                        {
                            Id = z.Id,
                            Label = z.TechnologiesGroup
                        }).ToList();

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", list);
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
        /// POST api/technology
        /// delete technology
        /// </summary>
        [HttpPost]
        public ApiResponse DeleteTechnology([FromBody]int id)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var line = db.tblTechnologies.Where(z => z.Id == id).SingleOrDefault();
                    if (line != null)
                    {
                        db.tblTechnologies.Remove(line);
                    }
                    db.SaveChanges();
                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgDelete, null);
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
        /// POST api/technology
        /// active-inActive record 
        /// </summary>
        [HttpPost]
        public ApiResponse ChangeStatus(tblTechnology tech)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var line = db.tblTechnologies.Where(z => z.Id == tech.Id).SingleOrDefault();
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
        /// GET api/technology
        /// return technology list with sorting and filtering  functionalities
        /// </summary>
        [HttpGet]
        public ApiResponse GetTechnologyList()
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
                    int topFilter = Convert.ToInt32(nvc["topFilter"]); //top main filter by TechnologyGroup combo selection 
                    string filter1 = nvc["filter1"]; //for col Technologies
                    string filter2 = nvc["filter2"]; // for col TechnologoesGroup

                    int iDisplayLength = Convert.ToInt32(nvc["count"]);
                    int iDisplayStart = (page - 1) * iDisplayLength;

                    List<tblTechnology> list = null;
                    try
                    {
                        list = db.tblTechnologies.ToList();
                        if (topFilter > 0)
                        {
                            list = list.Where(z => z.TechnologiesGroupId == topFilter).ToList();
                        }

                        //1. filter Technologies column if exists
                        if (!string.IsNullOrEmpty(filter1) && filter1 != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.Technologies.ToLower().Contains(filter1.ToLower())).ToList();
                        }

                        //2. convert returned datetime to local timezone & insert TechnologiesGroup col from different table
                        List<TechnologyViewModel> lstTech = new List<TechnologyViewModel>();
                        foreach (var l in list)
                        {
                            TechnologyViewModel t = new TechnologyViewModel
                            {
                                Id = l.Id,
                                Technologies = l.Technologies,
                                TechnologiesGroupId = l.TechnologiesGroupId,
                                TechnologiesGroup = db.tblTechnologiesGroups.Where(z => z.Id == l.TechnologiesGroupId).Select(z => z.TechnologiesGroup).SingleOrDefault(),
                                CreDate = Convert.ToDateTime(l.CreDate).AddMinutes(-1 * timezone),
                                ChgDate = Convert.ToDateTime(l.ChgDate).AddMinutes(-1 * timezone),
                                IsActive = l.IsActive
                            };
                            lstTech.Add(t);
                        }

                        //3. filter on TechnologiesGroup col if exists
                        if (!string.IsNullOrEmpty(filter2) && filter2 != "undefined")
                        {
                            iDisplayStart = 0;
                            lstTech = lstTech.Where(z => z.TechnologiesGroup.ToLower().Contains(filter2.ToLower())).ToList();
                        }

                        //4. do sorting on list
                        lstTech = DoSorting(lstTech, orderBy.Trim());

                        //5. take total count to return for ng-table
                        var Count = lstTech.Count();

                        var resultData = new
                        {
                            total = Count,
                            result = lstTech.Skip(iDisplayStart).Take(iDisplayLength).ToList()
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
        public List<TechnologyViewModel> DoSorting(List<TechnologyViewModel> list, string orderBy)
        {
            try
            {
                if (orderBy == "Technologies")
                {
                    list = list.OrderBy(z => z.Technologies).ToList();
                }
                else if (orderBy == "-Technologies")
                {
                    list = list.OrderByDescending(z => z.Technologies).ToList();
                }
                else if (orderBy == "ChgDate")
                {
                    list = list.OrderBy(z => z.ChgDate).ToList();
                }
                else if (orderBy == "-ChgDate")
                {
                    list = list.OrderByDescending(z => z.ChgDate).ToList();
                }
                else if (orderBy == "TechnologiesGroup")
                {
                    list = list.OrderBy(z => z.ChgDate).ToList();
                }
                else if (orderBy == "-TechnologiesGroup")
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