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
    public class DesignationController : ApiController
    {
        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Designation";

        public DesignationController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }

        /// <summary>
        /// POST api/Designation
        /// create and update record
        /// </summary>
        [HttpPost]
        public ApiResponse SaveDesignation(tblDesignation des)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (des.Id == 0) // Mode == Add
                    {
                        tblDesignation d = new tblDesignation
                        {
                            Designation = des.Designation,
                            DesignationGroupId = des.DesignationGroupId,
                            DesignationParentId = des.DesignationParentId,
                            IsActive = des.IsActive,
                            CreDate = DateTime.Now.ToUniversalTime(),
                            ChgDate = DateTime.Now.ToUniversalTime()
                        };
                        db.tblDesignations.Add(d);
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);
                    }
                    else
                    { // Mode == Edit
                        var line = db.tblDesignations.Where(z => z.Id == des.Id).SingleOrDefault();
                        if (line != null)
                        {
                            line.Designation = des.Designation;
                            line.DesignationGroupId = des.DesignationGroupId;
                            line.DesignationParentId = des.DesignationParentId;
                            line.IsActive = des.IsActive;
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
        /// GET api/Designation
        /// retrieve designation group list 
        /// </summary>
        [HttpGet]
        public ApiResponse RetrieveDesignationGroupList()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var list = db.tblDesignationGroups.OrderBy(z => z.DesignationGroup)
                        .Where(z => z.IsActive == true)
                        .Select(z => new SelectItemModel
                        {
                            Id = z.Id,
                            Label = z.DesignationGroup
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
        /// GET api/Designation
        /// retrieve designation parent list 
        /// </summary>
        [HttpGet]
        public ApiResponse RetrieveDesignationParentList()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var list = db.tblDesignationParents.OrderBy(z => z.DesignationParent)
                        .Where(z => z.IsActive == true)
                        .Select(z => new SelectItemModel
                        {
                            Id = z.Id,
                            Label = z.DesignationParent
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
        /// POST api/Designation
        /// delete designation
        /// </summary>
        [HttpPost]
        public ApiResponse DeleteDesignation([FromBody]int id)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (DependancyStatus.DesignationStatus(id))
                    {
                        var line = db.tblDesignations.Where(z => z.Id == id).SingleOrDefault();
                        if (line != null)
                        {
                            db.tblDesignations.Remove(line);
                        }
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

        /// <summary>
        /// POST api/Designation
        /// active-inActive record 
        /// </summary>
        [HttpPost]
        public ApiResponse ChangeStatus(tblDesignation des)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (DependancyStatus.DesignationStatus(des.Id))
                    {
                        var line = db.tblDesignations.Where(z => z.Id == des.Id).SingleOrDefault();
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
        /// GET api/Designation
        /// return designation list with sorting and filtering  functionalities
        /// </summary>
        [HttpGet]
        public ApiResponse GetDesignationList()
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
                    string filter1 = nvc["filter1"]; //for col Designation
                    string filter2 = nvc["filter2"]; // for col DesignationGroup
                    string filter3 = nvc["filter3"]; // for col DesignationParent

                    int iDisplayLength = Convert.ToInt32(nvc["count"]);
                    int iDisplayStart = (page - 1) * iDisplayLength;

                    List<tblDesignation> list = null;
                    List<DesignationViewModel> lstTech = new List<DesignationViewModel>();
                    try
                    {
                        list = db.tblDesignations.ToList();

                        //1. filter Technologies column if exists
                        if (!string.IsNullOrEmpty(filter1) && filter1 != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.Designation.ToLower().Contains(filter1.ToLower())).ToList();
                        }

                        //2. convert returned datetime to local timezone & insert DesignationGroup and DesignationParent col from different table
                        foreach (var l in list)
                        {
                            DesignationViewModel d = new DesignationViewModel
                            {
                                Id = l.Id,
                                DesignationName = l.Designation,
                                DesignationGroupId = l.DesignationGroupId,
                                DesignationGroup = db.tblDesignationGroups.Where(z => z.Id == l.DesignationGroupId).Select(z => z.DesignationGroup).SingleOrDefault(),
                                DesignationParentId = l.DesignationParentId,
                                DesignationParent = db.tblDesignationParents.Where(z => z.Id == l.DesignationParentId).Select(z => z.DesignationParent).SingleOrDefault(),
                                CreDate = Convert.ToDateTime(l.CreDate).AddMinutes(-1 * timezone),
                                ChgDate = Convert.ToDateTime(l.ChgDate).AddMinutes(-1 * timezone),
                                IsActive = l.IsActive
                            };
                            lstTech.Add(d);
                        }

                        //3. filter on DeignationGroup col if exists
                        if (!string.IsNullOrEmpty(filter2) && filter2 != "undefined")
                        {
                            iDisplayStart = 0;
                            lstTech = lstTech.Where(z => z.DesignationGroup.ToLower().Contains(filter2.ToLower())).ToList();
                        }
                        //4. filter on DeignationParent col if exists
                        if (!string.IsNullOrEmpty(filter3) && filter3 != "undefined")
                        {
                            iDisplayStart = 0;
                            lstTech = lstTech.Where(z => z.DesignationParent.ToLower().Contains(filter3.ToLower())).ToList();
                        }

                        //5. do sorting on list
                        lstTech = DoSorting(lstTech, orderBy.Trim());

                        //6. take total count to return for ng-table
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
                        lstTech = null;
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
        public List<DesignationViewModel> DoSorting(List<DesignationViewModel> list, string orderBy)
        {
            try
            {
                if (orderBy == "DesignationName")
                {
                    list = list.OrderBy(z => z.DesignationName).ToList();
                }
                else if (orderBy == "-DesignationName")
                {
                    list = list.OrderByDescending(z => z.DesignationName).ToList();
                }
                else if (orderBy == "DesignationGroup")
                {
                    list = list.OrderBy(z => z.DesignationGroup).ToList();
                }
                else if (orderBy == "-DesignationGroup")
                {
                    list = list.OrderByDescending(z => z.DesignationGroup).ToList();
                }
                else if (orderBy == "DesignationParent")
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
                return list.ToList<DesignationViewModel>();
            }
            catch
            {
                return null;
            }
        }




        /// <summary>
        /// List Of Active Designation for DropDowns
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<tblDesignation> GetActiveDesignations()
        {
            try
            {
                return db.tblDesignations.Where(z => z.IsActive == true).OrderBy(z => z.Designation).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}