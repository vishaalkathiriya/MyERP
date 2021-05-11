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
    public class SRTypeController : ApiController
    {
        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Machine Type";

        public SRTypeController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }

        /// <summary>
        /// POST api/SRType
        /// create and update record
        /// </summary>
        public ApiResponse SaveSRType(tblSRType SRType)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (SRType.TypeId == 0)
                    {
                        int count = db.tblSRTypes.Where(z => z.TypeName.ToLower().Equals(SRType.TypeName.ToLower())).Count();
                        if (count > 0)
                        {
                            apiResponse = ERPUtilities.GenerateApiResponse(true, 2, generalMessages.msgEntryExists, null);
                            return apiResponse;
                        }
                        tblSRType s = new tblSRType
                        {
                            TypePrefix = SRType.TypePrefix,
                            TypeName = SRType.TypeName,
                            CreDate = DateTime.Now.ToUniversalTime(),
                            ChgDate = DateTime.Now.ToUniversalTime(),
                            Remarks = SRType.Remarks
                        };
                        db.tblSRTypes.Add(s);
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);
                    }
                    else
                    {
                        var line = db.tblSRTypes.Where(z => z.TypeId == SRType.TypeId).SingleOrDefault();
                        if (line != null)
                        {
                            line.TypePrefix = SRType.TypePrefix;
                            line.TypeName = SRType.TypeName;
                            line.ChgDate = DateTime.Now.ToUniversalTime();
                            line.Remarks = SRType.Remarks;
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
        /// POST api/SRType
        /// delete SR Type
        /// </summary>
        [HttpPost]
        public ApiResponse DeleteSRType([FromBody]int id)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (DependancyStatus.SRTypeStatus(id))
                    {
                        var line = db.tblSRTypes.Where(z => z.TypeId == id).SingleOrDefault();
                        if (line != null)
                        {
                            db.tblSRTypes.Remove(line);
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
        /// GET api/SRType
        /// return SR-Types list with sorting and filtering  functionalities
        /// </summary>
        [HttpGet]
        public ApiResponse GetSRTypeList()
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
                    string typeNameFilter = nvc["filter"];
                    string typePrefixFilter = nvc["filter1"];

                    int iDisplayLength = Convert.ToInt32(nvc["count"]);
                    int iDisplayStart = (page - 1) * iDisplayLength;

                    List<tblSRType> list = null;

                    try
                    {
                        list = db.tblSRTypes.ToList();

                        //1. filter data
                        if (!string.IsNullOrEmpty(typeNameFilter) && typeNameFilter != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.TypeName.ToLower().Contains(typeNameFilter.ToLower())).ToList();
                        }

                        //2. filter data by type prefix
                        if (!string.IsNullOrEmpty(typePrefixFilter) && typePrefixFilter != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.TypePrefix.ToLower().Contains(typePrefixFilter.ToLower())).ToList();
                        }

                        //3. do sorting on list
                        list = DoSorting(list, orderBy.Trim());

                        //4. take total count to return for ng-table
                        var Count = list.Count();

                        //5. convert returned datetime to local timezone
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
        public List<tblSRType> DoSorting(IEnumerable<tblSRType> list, string orderBy)
        {
            try
            {
                if (orderBy == "TypeName")
                {
                    list = list.OrderBy(z => z.TypeName).ToList();
                }
                else if (orderBy == "-TypeName")
                {
                    list = list.OrderByDescending(z => z.TypeName).ToList();
                }
                else if (orderBy == "TypePrefix")
                {
                    list = list.OrderBy(z => z.TypePrefix).ToList();
                }
                else if (orderBy == "-TypePrefix")
                {
                    list = list.OrderByDescending(z => z.TypePrefix).ToList();
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
                return list.ToList<tblSRType>();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// GET api/SRType
        /// fetch Type 
        /// </summary>
        [HttpGet]
        public ApiResponse FetchType()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int typeId = Convert.ToInt32(nvc["TypeId"]);
                    if (typeId != 1)
                    {
                        var list = db.tblSRTypes.Where(z => z.TypeId== typeId).ToList();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", list);
                    }
                    else
                    {
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 0, generalMessages.msgAccessDenied, null);
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

    }
}
