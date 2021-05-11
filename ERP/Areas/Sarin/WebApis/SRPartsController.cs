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
    public class SRPartsController : ApiController
    {
        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Part";

        public SRPartsController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }

        /// <summary>
        /// POST api/SRParts
        /// create and update record
        /// </summary>
        [HttpPost]
        public ApiResponse SaveSRPart(tblSRPart SRPart)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (SRPart.PartId == 0)
                    {
                        int count = db.tblSRParts.Where(z => z.PartName.ToLower().Equals(SRPart.PartName.ToLower())).Count();
                        if (count > 0)
                        {
                            apiResponse = ERPUtilities.GenerateApiResponse(true, 2, generalMessages.msgEntryExists, null);
                            return apiResponse;
                        }
                        tblSRPart p = new tblSRPart
                        {
                            PartName = SRPart.PartName,
                            CreBy = SRPart.CreBy,
                            ChgBy = SRPart.ChgBy,
                            CreDate = DateTime.Now.ToUniversalTime(),
                            ChgDate = DateTime.Now.ToUniversalTime(),
                            Remarks = SRPart.Remarks
                        };
                        db.tblSRParts.Add(p);
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);
                    }
                    else
                    {
                        var line = db.tblSRParts.Where(z => z.PartId == SRPart.PartId).SingleOrDefault();
                        if (line != null)
                        {
                            line.PartName = SRPart.PartName;
                            line.ChgBy = SRPart.ChgBy;
                            line.ChgDate = DateTime.Now.ToUniversalTime();
                            line.Remarks = SRPart.Remarks;
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
        /// POST api/SRParts
        /// delete SR-Part
        /// </summary>
        [HttpPost]
        public ApiResponse DeleteSRPart([FromBody]int id)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (DependancyStatus.SRPartStatus(id))
                    {
                        var line = db.tblSRParts.Where(z => z.PartId == id).SingleOrDefault();
                        if (line != null)
                        {
                            db.tblSRParts.Remove(line);
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
        /// GET api/SRParts
        /// return SR-Part list with sorting and filtering  functionalities
        /// </summary>
        [HttpGet]
        public ApiResponse GetSRPartList()
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
                    string partNameFilter = nvc["filter"];

                    int iDisplayLength = Convert.ToInt32(nvc["count"]);
                    int iDisplayStart = (page - 1) * iDisplayLength;

                    List<tblSRPart> list = null;

                    try
                    {
                        list = db.tblSRParts.ToList();

                        //1. filter data
                        if (!string.IsNullOrEmpty(partNameFilter) && partNameFilter != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.PartName.ToLower().Contains(partNameFilter.ToLower())).ToList();
                        }

                        //2. do sorting on list
                        list = DoSorting(list, orderBy.Trim());

                        //3. take total count to return for ng-table
                        var Count = list.Count();

                        //4. convert returned datetime to local timezone
                        var PartGroup = list.Select(i =>
                        {
                            i.CreDate = Convert.ToDateTime(i.CreDate).AddMinutes(-1 * timezone);
                            i.ChgDate = Convert.ToDateTime(i.ChgDate).AddMinutes(-1 * timezone);
                            return i;
                        }).Skip(iDisplayStart).Take(iDisplayLength).ToList();


                        var resultData = new
                        {
                            total = Count,
                            result = PartGroup.ToList()
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
        public List<tblSRPart> DoSorting(IEnumerable<tblSRPart> list, string orderBy)
        {
            try
            {
                if (orderBy == "PartName")
                {
                    list = list.OrderBy(z => z.PartName).ToList();
                }
                else if (orderBy == "-PartName")
                {
                    list = list.OrderByDescending(z => z.PartName).ToList();
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
                return list.ToList<tblSRPart>();
            }
            catch
            {
                return null;
            }
        }
    }
}
