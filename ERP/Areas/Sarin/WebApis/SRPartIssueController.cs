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
    public class SRPartIssueController : ApiController
    {
        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Part Issue";

        public SRPartIssueController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }

        /// <summary>
        /// POST api/SRPartIssue
        /// create and update record
        /// </summary>
        [HttpPost]
        public ApiResponse SaveSRPartIssue(tblSRPartIssue partIssue)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (partIssue.PartIssueId == 0) // Mode == Add
                    {
                        tblSRPartIssue p = new tblSRPartIssue
                        {
                            MachineId = partIssue.MachineId,
                            PartId = partIssue.PartId,
                            IssuedFrom = partIssue.IssuedFrom,
                            ChallanNo = partIssue.ChallanNo,
                            IssuedDate = partIssue.IssuedDate,
                            Problem = partIssue.Problem,
                            CreBy = partIssue.CreBy,
                            ChgBy = partIssue.ChgBy,
                            CreDate = DateTime.Now.ToUniversalTime(),
                            ChgDate = DateTime.Now.ToUniversalTime(),
                            Remarks = partIssue.Remarks
                        };
                        db.tblSRPartIssues.Add(p);
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);
                    }
                    else
                    { // Mode == Edit
                        var line = db.tblSRPartIssues.Where(z => z.PartIssueId == partIssue.PartIssueId).SingleOrDefault();
                        if (line != null)
                        {
                            line.MachineId = partIssue.MachineId;
                            line.PartId = partIssue.PartId;
                            line.IssuedFrom = partIssue.IssuedFrom;
                            line.ChallanNo = partIssue.ChallanNo;
                            line.IssuedDate = partIssue.IssuedDate;
                            line.Problem = partIssue.Problem;
                            line.ChgBy = partIssue.ChgBy;
                            line.ChgDate = DateTime.Now.ToUniversalTime();
                            line.Remarks = partIssue.Remarks;
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
        /// GET api/SRPartIssue
        /// retrieve SR Machine list
        /// </summary>
        [HttpGet]
        public ApiResponse RetrieveSRMachineList()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var list = db.tblSRMachines.OrderBy(z => z.MachineName)
                        .Select(z => new SelectItemModel
                        {
                            Id = z.MachineId,
                            Label = z.MachineName
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
        /// GET api/SRPartIssue
        /// retrieve SR MAchine-Part list
        /// </summary>
        [HttpGet]
        public ApiResponse RetrieveSRPartList()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var list = db.tblSRParts.OrderBy(z => z.PartName)
                        .Select(z => new SelectItemModel
                        {
                            Id = z.PartId,
                            Label = z.PartName
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
        /// POST api/SRPartIssue
        /// delete SR SR-Part
        /// </summary>
        [HttpPost]
        public ApiResponse DeleteSRPartIssue([FromBody]int id)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var line = db.tblSRPartIssues.Where(z => z.PartIssueId == id).SingleOrDefault();
                    if (line != null)
                    {
                        db.tblSRPartIssues.Remove(line);
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
        /// GET api/SRPartIssue
        /// return SR-Part list with sorting and filtering  functionalities
        /// </summary>
        [HttpGet]
        public ApiResponse GetSRPartIssueList()
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
                    string srMachineNameFilter = nvc["filter1"];
                    string srPartNameFilter = nvc["filter2"];
                    string srIssueFromFilter = nvc["filter3"];
                    string srChallanFilter = nvc["filter4"];
                    string srProblemFilter = nvc["filter5"];

                    int iDisplayLength = Convert.ToInt32(nvc["count"]);
                    int iDisplayStart = (page - 1) * iDisplayLength;

                    List<tblSRPartIssue> list = null;
                    List<SRPartIssueViewModel> lstPartIssue = new List<SRPartIssueViewModel>();
                    try
                    {
                        list = db.tblSRPartIssues.ToList();


                        //1. convert returned datetime to local timezone & insert MachineName and PartName col from different table
                        foreach (var l in list)
                        {
                            SRPartIssueViewModel p = new SRPartIssueViewModel
                            {
                                PartIssueId = l.PartIssueId,
                                MachineId = l.MachineId,
                                MachineName = db.tblSRMachines.Where(z => z.MachineId == l.MachineId).Select(z => z.MachineName).SingleOrDefault(),
                                PartId = l.PartId,
                                PartName = db.tblSRParts.Where(z => z.PartId == l.PartId).Select(z => z.PartName).SingleOrDefault(),
                                ChallanNo = l.ChallanNo,
                                IssuedFrom=l.IssuedFrom,
                                IssuedDate = l.IssuedDate,
                                Problem = l.Problem,
                                CreBy = l.CreBy,
                                ChgBy = l.ChgBy,
                                CreDate = Convert.ToDateTime(l.CreDate).AddMinutes(-1 * timezone),
                                ChgDate = Convert.ToDateTime(l.ChgDate).AddMinutes(-1 * timezone),
                                Remarks = l.Remarks
                            };
                            lstPartIssue.Add(p);
                        }

                        //2. filter on machine col if exists
                        if (!string.IsNullOrEmpty(srMachineNameFilter) && srMachineNameFilter != "undefined")
                        {
                            iDisplayStart = 0;
                            lstPartIssue = lstPartIssue.Where(z => z.MachineName.ToLower().Contains(srMachineNameFilter.ToLower())).ToList();
                        }

                        //3. filter on partname col if exists
                        if (!string.IsNullOrEmpty(srPartNameFilter) && srPartNameFilter != "undefined")
                        {
                            iDisplayStart = 0;
                            lstPartIssue = lstPartIssue.Where(z => z.PartName.ToLower().Contains(srPartNameFilter.ToLower())).ToList();
                        }
                        //4. filter on issued from col if exists
                        if (!string.IsNullOrEmpty(srIssueFromFilter) && srIssueFromFilter != "undefined")
                        {
                            iDisplayStart = 0;
                            lstPartIssue = lstPartIssue.Where(z => z.IssuedFrom.ToLower().Contains(srIssueFromFilter.ToLower())).ToList();
                        }

                        //5. filter on challan no col if exists
                        if (!string.IsNullOrEmpty(srChallanFilter) && srChallanFilter != "undefined")
                        {
                            iDisplayStart = 0;
                            lstPartIssue = lstPartIssue.Where(z => z.ChallanNo.ToLower().Contains(srChallanFilter.ToLower())).ToList();
                        }

                        //6. filter on problem col if exists
                        if (!string.IsNullOrEmpty(srProblemFilter) && srProblemFilter != "undefined")
                        {
                            iDisplayStart = 0;
                            lstPartIssue = lstPartIssue.Where(z => z.Problem.ToLower().Contains(srProblemFilter.ToLower())).ToList();
                        }

                        //7. do sorting on list
                        lstPartIssue = DoSorting(lstPartIssue, orderBy.Trim());

                        //8. take total count to return for ng-table
                        var Count = lstPartIssue.Count();

                        var resultData = new
                        {
                            total = Count,
                            result = lstPartIssue.Skip(iDisplayStart).Take(iDisplayLength).ToList()
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
                        lstPartIssue = null;
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
        public List<SRPartIssueViewModel> DoSorting(List<SRPartIssueViewModel> list, string orderBy)
        {
            try
            {
                if (orderBy == "MachineName")
                {
                    list = list.OrderBy(z => z.MachineName).ToList();
                }
                else if (orderBy == "-MachineName")
                {
                    list = list.OrderByDescending(z => z.MachineName).ToList();
                }
                else if (orderBy == "PartName")
                {
                    list = list.OrderBy(z => z.PartName).ToList();
                }
                else if (orderBy == "-PartName")
                {
                    list = list.OrderByDescending(z => z.PartName).ToList();
                }
                else if (orderBy == "ChallanNo")
                {
                    list = list.OrderBy(z => z.ChallanNo).ToList();
                }
                else if (orderBy == "-ChallanNo")
                {
                    list = list.OrderByDescending(z => z.ChallanNo).ToList();
                }

                else if (orderBy == "IssuedFrom")
                {
                    list = list.OrderBy(z => z.IssuedFrom).ToList();
                }
                else if (orderBy == "-IssuedFrom")
                {
                    list = list.OrderByDescending(z => z.IssuedFrom).ToList();
                }
                else if (orderBy == "IssuedDate")
                {
                    list = list.OrderBy(z => z.IssuedDate).ToList();
                }
                else if (orderBy == "-IssuedDate")
                {
                    list = list.OrderByDescending(z => z.IssuedDate).ToList();
                }
                else if (orderBy == "Problem")
                {
                    list = list.OrderBy(z => z.Problem).ToList();
                }
                else if (orderBy == "-Problem")
                {
                    list = list.OrderByDescending(z => z.Problem).ToList();
                }
                else if (orderBy == "ChgDate")
                {
                    list = list.OrderBy(z => z.ChgDate).ToList();
                }
                else if (orderBy == "-ChgDate")
                {
                    list = list.OrderByDescending(z => z.ChgDate).ToList();
                }
                return list.ToList<SRPartIssueViewModel>();
            }
            catch
            {
                return null;
            }
        }

    }
}
