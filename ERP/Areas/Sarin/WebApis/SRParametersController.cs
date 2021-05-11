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
    public class SRParametersController : ApiController
    {
        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Machine Sub-Type Parameters";

        public SRParametersController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }

        /// <summary>
        /// POST api/SRParameters
        /// create and update record
        /// </summary>
        [HttpPost]
        public ApiResponse SaveSRParameters(tblSRParameter srParameter)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (srParameter.ParameterId == 0) // Mode == Add
                    {
                        tblSRParameter p = new tblSRParameter
                        {
                            SubTypeId = srParameter.SubTypeId,
                            ParameterName = srParameter.ParameterName,
                            CreBy = srParameter.CreBy,
                            ChgBy = srParameter.ChgBy,
                            CreDate = DateTime.Now.ToUniversalTime(),
                            ChgDate = DateTime.Now.ToUniversalTime(),
                            Remarks = srParameter.Remarks

                        };
                        db.tblSRParameters.Add(p);
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);
                    }
                    else
                    { // Mode == Edit
                        var line = db.tblSRParameters.Where(z => z.ParameterId == srParameter.ParameterId).SingleOrDefault();
                        if (line != null)
                        {
                            line.SubTypeId = srParameter.SubTypeId;
                            line.ParameterName = srParameter.ParameterName;
                            line.ChgBy = srParameter.ChgBy;
                            line.ChgDate = DateTime.Now.ToUniversalTime();
                            line.Remarks = srParameter.Remarks;
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
        /// GET api/SRParameters
        /// retrieve SR-Sub-Type List
        /// </summary>
        [HttpGet]
        public ApiResponse RetrieveSRSubTypeList()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var list = db.tblSRSubTypes.OrderBy(z => z.SubTypeName)
                        .Select(z => new SelectItemModel
                        {
                            Id = z.SubTypeId,
                            Label = z.SubTypeName + " - " + z.Selection
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
        /// POST api/SRParameters
        /// delete SRParameters
        /// </summary>
        [HttpPost]
        public ApiResponse DeleteSRParameter([FromBody]int id)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (DependancyStatus.SRParameterStatus(id))
                    {
                        var line = db.tblSRParameters.Where(z => z.ParameterId == id).SingleOrDefault();
                        if (line != null)
                        {
                            db.tblSRParameters.Remove(line);
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
        /// GET api/SRParameters
        /// return SR-Parameters list with sorting and filtering  functionalities
        /// </summary>
        [HttpGet]
        public ApiResponse GetSRParameterList()
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
                    string srParameterFilter = nvc["filter1"]; //for col SR Parameter Name
                    string srSubTypeFilter = nvc["filter2"]; // for col SR Sub-Type Name


                    int iDisplayLength = Convert.ToInt32(nvc["count"]);
                    int iDisplayStart = (page - 1) * iDisplayLength;

                    List<tblSRParameter> list = null;
                    List<SRParameterViewModel> lstParameter = new List<SRParameterViewModel>();
                    try
                    {
                        list = db.tblSRParameters.ToList();

                        //1. filter SR Parameter Name column if exists
                        if (!string.IsNullOrEmpty(srParameterFilter) && srParameterFilter != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.ParameterName.ToLower().Contains(srParameterFilter.ToLower())).ToList();
                        }

                        //2. convert returned datetime to local timezone & insert sub type col from different table
                        foreach (var l in list)
                        {
                            SRParameterViewModel s = new SRParameterViewModel
                            {
                                ParameterId = l.ParameterId,
                                ParameterName = l.ParameterName,
                                SubTypeId = l.SubTypeId,
                                SubTypeName = db.tblSRSubTypes.Where(z => z.SubTypeId == l.SubTypeId).Select(z => z.SubTypeName).SingleOrDefault(),
                                CreBy = l.CreBy,
                                ChgBy = l.ChgBy,
                                CreDate = Convert.ToDateTime(l.CreDate).AddMinutes(-1 * timezone),
                                ChgDate = Convert.ToDateTime(l.ChgDate).AddMinutes(-1 * timezone),
                                Remarks = l.Remarks
                            };
                            lstParameter.Add(s);
                        }

                        //3. filter on SR sub Type col if exists
                        if (!string.IsNullOrEmpty(srSubTypeFilter) && srSubTypeFilter != "undefined")
                        {
                            iDisplayStart = 0;
                            lstParameter = lstParameter.Where(z => z.SubTypeName.ToLower().Contains(srSubTypeFilter.ToLower())).ToList();
                        }

                        //4. do sorting on list
                        lstParameter = DoSorting(lstParameter, orderBy.Trim());

                        //5. take total count to return for ng-table
                        var Count = lstParameter.Count();

                        var resultData = new
                        {
                            total = Count,
                            result = lstParameter.Skip(iDisplayStart).Take(iDisplayLength).ToList()
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
                        lstParameter = null;
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
        public List<SRParameterViewModel> DoSorting(List<SRParameterViewModel> list, string orderBy)
        {
            try
            {
                if (orderBy == "ParameterName")
                {
                    list = list.OrderBy(z => z.ParameterName).ToList();
                }
                else if (orderBy == "-ParameterName")
                {
                    list = list.OrderByDescending(z => z.ParameterName).ToList();
                }
                else if (orderBy == "SubTypeName")
                {
                    list = list.OrderBy(z => z.SubTypeName).ToList();
                }
                else if (orderBy == "-SubTypeName")
                {
                    list = list.OrderByDescending(z => z.SubTypeName).ToList();
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
                return list.ToList<SRParameterViewModel>();
            }
            catch
            {
                return null;
            }
        }
        
    }
}
