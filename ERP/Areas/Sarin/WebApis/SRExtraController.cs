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
    public class SRExtraController : ApiController
    {
        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Extra Entry";

        public SRExtraController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }

        /// <summary>
        /// POST api/SRExtra
        /// create and update record
        /// </summary>
        [HttpPost]
        public ApiResponse SaveSRExtra(tblSRExtra srExtra)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (srExtra.ExtraId == 0) // Mode == Add
                    {
                        tblSRExtra e = new tblSRExtra
                        {
                            Type = srExtra.Type,
                            MachineNo = srExtra.MachineNo,
                            ExtraDate = srExtra.ExtraDate,
                            CreBy = srExtra.CreBy,
                            ChgBy = srExtra.ChgBy,
                            CreDate = DateTime.Now.ToUniversalTime(),
                            ChgDate = DateTime.Now.ToUniversalTime(),
                            Remark = srExtra.Remark
                        };
                        db.tblSRExtras.Add(e);
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);
                    }
                    else
                    { // Mode == Edit
                        var line = db.tblSRExtras.Where(z => z.ExtraId == srExtra.ExtraId).SingleOrDefault();
                        if (line != null)
                        {
                            line.Type = srExtra.Type;
                            line.MachineNo = srExtra.MachineNo;
                            line.ExtraDate = srExtra.ExtraDate;
                            line.ChgBy = srExtra.ChgBy;
                            line.ChgDate = DateTime.Now.ToUniversalTime();
                            line.Remark = srExtra.Remark;
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
        /// POST api/SRExtra
        /// delete SR Extra Entry
        /// </summary>
        [HttpPost]
        public ApiResponse DeleteSRExtra([FromBody]int id)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var line = db.tblSRExtras.Where(z => z.ExtraId == id).SingleOrDefault();
                    if (line != null)
                    {
                        db.tblSRExtras.Remove(line);
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
        /// GET api/SRExtra
        /// return SR-Extra-Entry list with sorting and filtering  functionalities
        /// </summary>
        [HttpGet]
        public ApiResponse GetSRExtraList()
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
                    string srTypeFilter = nvc["filter1"]; //for col Type
                    string srMachineNoFilter = nvc["filter2"]; // for col Machine No
                    string srFilterTypeMain = nvc["filterType"]; // for col Machine No

                    #region "Set StartDate-EndDate"
                    DateTime fromDate, toDate;
                    string startDate = nvc["startDate"];
                    string endDate = nvc["endDate"];

                    if (!string.IsNullOrEmpty(startDate))
                    {
                        string[] fdate = startDate.Split('/');
                        fromDate = new DateTime(Convert.ToInt32(fdate[2]), Convert.ToInt32(fdate[1]), Convert.ToInt32(fdate[0]));
                    }
                    else
                    {
                        fromDate = Convert.ToDateTime("1/1/1900");
                    }

                    if (!string.IsNullOrEmpty(endDate))
                    {
                        string[] tdate = endDate.Split('/');
                        toDate = new DateTime(Convert.ToInt32(tdate[2]), Convert.ToInt32(tdate[1]), Convert.ToInt32(tdate[0]));
                    }
                    else
                    {
                        toDate = Convert.ToDateTime("1/1/1900");
                    }
                    #endregion

                    int iDisplayLength = Convert.ToInt32(nvc["count"]);
                    int iDisplayStart = (page - 1) * iDisplayLength;

                    List<tblSRExtra> list = null;
                    List<SRExtraViewModel> lstExtra = new List<SRExtraViewModel>();
                    try
                    {
                        list = db.tblSRExtras.ToList();

                        //1. filter Type if exists
                        if (!string.IsNullOrEmpty(srFilterTypeMain) && srFilterTypeMain != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.Type.ToLower().Contains(srFilterTypeMain.ToLower())).ToList();
                        }

                        //2. filter Issue-date if exists
                        if (fromDate.ToString("d/M/yyyy") != "1/1/1900" && toDate.ToString("d/M/yyyy") != "1/1/1900")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.ExtraDate >= fromDate && z.ExtraDate <= toDate).ToList();
                        }

                        //3. filter Machine No column if exists
                        if (!string.IsNullOrEmpty(srMachineNoFilter) && srMachineNoFilter != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.MachineNo.ToLower().Contains(srMachineNoFilter.ToLower())).ToList();
                        }

                        //4. convert returned datetime to local timezone & insert TypeName col from different table
                        foreach (var l in list)
                        {
                            SRExtraViewModel e = new SRExtraViewModel
                            {
                                ExtraId = l.ExtraId,
                                Type = l.Type,
                                TypeName = l.Type.Equals("S") ? "Scrap" : "Other",
                                MachineNo = l.MachineNo,
                                ExtraDate = l.ExtraDate,
                                CreBy = l.CreBy,
                                ChgBy = l.ChgBy,
                                CreDate = Convert.ToDateTime(l.CreDate).AddMinutes(-1 * timezone),
                                ChgDate = Convert.ToDateTime(l.ChgDate).AddMinutes(-1 * timezone),
                                Remark = l.Remark
                            };
                            lstExtra.Add(e);
                        }

                        //5. filter Type column if exists
                        if (!string.IsNullOrEmpty(srTypeFilter) && srTypeFilter != "undefined")
                        {
                            iDisplayStart = 0;
                            lstExtra = lstExtra.Where(z => z.TypeName.ToLower().Contains(srTypeFilter.ToLower())).ToList();
                        }

                        //6. do sorting on list
                        lstExtra = DoSorting(lstExtra, orderBy.Trim());

                        //7. take total count to return for ng-table
                        var Count = lstExtra.Count();

                        var resultData = new
                        {
                            total = Count,
                            result = lstExtra.Skip(iDisplayStart).Take(iDisplayLength).ToList()
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
                        lstExtra = null;
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
        public List<SRExtraViewModel> DoSorting(List<SRExtraViewModel> list, string orderBy)
        {
            try
            {
                if (orderBy == "Type")
                {
                    list = list.OrderBy(z => z.Type).ToList();
                }
                else if (orderBy == "-Type")
                {
                    list = list.OrderByDescending(z => z.Type).ToList();
                }
                else if (orderBy == "MachineNo")
                {
                    list = list.OrderBy(z => z.MachineNo).ToList();
                }
                else if (orderBy == "-MachineNo")
                {
                    list = list.OrderByDescending(z => z.MachineNo).ToList();
                }
                else if (orderBy == "TypeName")
                {
                    list = list.OrderBy(z => z.TypeName).ToList();
                }
                else if (orderBy == "-TypeName")
                {
                    list = list.OrderByDescending(z => z.TypeName).ToList();
                }
                else if (orderBy == "ExtraDate")
                {
                    list = list.OrderBy(z => z.ExtraDate).ToList();
                }
                else if (orderBy == "-ExtraDate")
                {
                    list = list.OrderByDescending(z => z.ExtraDate).ToList();
                }
                else if (orderBy == "Remark")
                {
                    list = list.OrderBy(z => z.Remark).ToList();
                }
                else if (orderBy == "-Remark")
                {
                    list = list.OrderByDescending(z => z.Remark).ToList();
                }
                else if (orderBy == "ChgDate")
                {
                    list = list.OrderBy(z => z.ChgDate).ToList();
                }
                else if (orderBy == "-ChgDate")
                {
                    list = list.OrderByDescending(z => z.ChgDate).ToList();
                }
                return list.ToList<SRExtraViewModel>();
            }
            catch
            {
                return null;
            }
        }
    }
}
