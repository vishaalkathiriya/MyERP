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
    public class SRSubTypeController : ApiController
    {
        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Machine Sub-Type";

        public SRSubTypeController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }

        /// <summary>
        /// POST api/SRSubType
        /// create and update record
        /// </summary>
        [HttpPost]
        public ApiResponse SaveSRSubType(tblSRSubType subType)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (subType.SubTypeId == 0) // Mode == Add
                    {
                        tblSRSubType s = new tblSRSubType
                        {
                            TypeId = subType.TypeId,
                            SubTypeName = subType.SubTypeName,
                            Selection = subType.Selection,
                            CreBy = subType.CreBy,
                            ChgBy = subType.ChgBy,
                            CreDate = DateTime.Now.ToUniversalTime(),
                            ChgDate = DateTime.Now.ToUniversalTime(),
                            Remarks = subType.Remarks
                        };
                        db.tblSRSubTypes.Add(s);
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);
                    }
                    else
                    { // Mode == Edit
                        var line = db.tblSRSubTypes.Where(z => z.SubTypeId == subType.SubTypeId).SingleOrDefault();
                        if (line != null)
                        {
                            line.TypeId = subType.TypeId;
                            line.SubTypeName = subType.SubTypeName;
                            line.Selection = subType.Selection;
                            line.ChgBy = subType.ChgBy;
                            line.ChgDate = DateTime.Now.ToUniversalTime();
                            line.Remarks = subType.Remarks;
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
        /// GET api/SRSubType
        /// retrieve SR Type list
        /// </summary>
        [HttpGet]
        public ApiResponse RetrieveSRTypeList()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var list = db.tblSRTypes.OrderBy(z => z.TypeName)
                        .Select(z => new SelectItemModel
                        {
                            Id = z.TypeId,
                            Label = z.TypeName
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
        /// POST api/SRSubType
        /// delete SR Sub-type
        /// </summary>
        [HttpPost]
        public ApiResponse DeleteSRSubType([FromBody]int id)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (DependancyStatus.SRSubTypeStatus(id))
                    {
                        var line = db.tblSRSubTypes.Where(z => z.SubTypeId == id).SingleOrDefault();
                        if (line != null)
                        {
                            db.tblSRSubTypes.Remove(line);
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
        /// GET api/SRSubType
        /// return SR-Subtype list with sorting and filtering  functionalities
        /// </summary>
        [HttpGet]
        public ApiResponse GetSRSubTypeList()
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
                    string srSubTypeFilter = nvc["filter1"]; //for col SR Sub-Type
                    string srTypeFilter = nvc["filter2"]; // for col SR Type
                    string selectionFilter = nvc["filter3"]; // for col Selection

                    int iDisplayLength = Convert.ToInt32(nvc["count"]);
                    int iDisplayStart = (page - 1) * iDisplayLength;

                    List<tblSRSubType> list = null;
                    List<SRSubTypeViewModel> lstSubType = new List<SRSubTypeViewModel>();
                    try
                    {
                        list = db.tblSRSubTypes.ToList();

                        //1. filter SR Sub-Type column if exists
                        if (!string.IsNullOrEmpty(srSubTypeFilter) && srSubTypeFilter != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.SubTypeName.ToLower().Contains(srSubTypeFilter.ToLower())).ToList();
                        }

                        //2. convert returned datetime to local timezone & insert TypeName col from different table
                        foreach (var l in list)
                        {
                            SRSubTypeViewModel s = new SRSubTypeViewModel
                            {
                                SubTypeId = l.SubTypeId,
                                SubTypeName = l.SubTypeName,
                                TypeId = l.TypeId,
                                TypeName = db.tblSRTypes.Where(z => z.TypeId == l.TypeId).Select(z => z.TypeName).SingleOrDefault(),
                                Selection = l.Selection,
                                CreBy = l.CreBy,
                                ChgBy = l.ChgBy,
                                CreDate = Convert.ToDateTime(l.CreDate).AddMinutes(-1 * timezone),
                                ChgDate = Convert.ToDateTime(l.ChgDate).AddMinutes(-1 * timezone),
                                Remarks = l.Remarks
                            };
                            lstSubType.Add(s);
                        }

                        //3. filter on SR Type col if exists
                        if (!string.IsNullOrEmpty(srTypeFilter) && srTypeFilter != "undefined")
                        {
                            iDisplayStart = 0;
                            lstSubType = lstSubType.Where(z => z.TypeName.ToLower().Contains(srTypeFilter.ToLower())).ToList();
                        }

                        //4. filter on Selection col if exists
                        if (!string.IsNullOrEmpty(selectionFilter) && selectionFilter != "undefined")
                        {
                            iDisplayStart = 0;
                            lstSubType = lstSubType.Where(z => z.Selection.ToLower().Contains(selectionFilter.ToLower())).ToList();
                        }

                        //5. do sorting on list
                        lstSubType = DoSorting(lstSubType, orderBy.Trim());

                        //6. take total count to return for ng-table
                        var Count = lstSubType.Count();

                        var resultData = new
                        {
                            total = Count,
                            result = lstSubType.Skip(iDisplayStart).Take(iDisplayLength).ToList()
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
                        lstSubType = null;
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
        public List<SRSubTypeViewModel> DoSorting(List<SRSubTypeViewModel> list, string orderBy)
        {
            try
            {
                if (orderBy == "SubTypeName")
                {
                    list = list.OrderBy(z => z.SubTypeName).ToList();
                }
                else if (orderBy == "-SubTypeName")
                {
                    list = list.OrderByDescending(z => z.SubTypeName).ToList();
                }
                else if (orderBy == "TypeName")
                {
                    list = list.OrderBy(z => z.TypeName).ToList();
                }
                else if (orderBy == "-TypeName")
                {
                    list = list.OrderByDescending(z => z.TypeName).ToList();
                }
                else if (orderBy == "Selection")
                {
                    list = list.OrderBy(z => z.Selection).ToList();
                }
                else if (orderBy == "-Selection")
                {
                    list = list.OrderByDescending(z => z.Selection).ToList();
                }
                else if (orderBy == "Remarks")
                {
                    list = list.OrderBy(z => z.Remarks).ToList();
                }
                else if (orderBy == "-Remarks")
                {
                    list = list.OrderByDescending(z => z.Remarks).ToList();
                }
                else if (orderBy == "ChgDate")
                {
                    list = list.OrderBy(z => z.ChgDate).ToList();
                }
                else if (orderBy == "-ChgDate")
                {
                    list = list.OrderByDescending(z => z.ChgDate).ToList();
                }
                return list.ToList<SRSubTypeViewModel>();
            }
            catch
            {
                return null;
            }
        }

        
    }
}
