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
    public class SRRepairController : ApiController
    {
        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Repair Entry";

        public SRRepairController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }

        /// <summary>
        /// POST api/SRRepair
        /// create and update record
        /// </summary>
        [HttpPost]
        public ApiResponse SaveSRRepair(tblSRRepair srRepair)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (srRepair.RepairId == 0) // Mode == Add
                    {
                        tblSRRepair r = new tblSRRepair
                        {
                            MachineId = srRepair.MachineId,
                            PartId = srRepair.PartId,
                            Problem = srRepair.Problem,
                            RepairedBy = srRepair.RepairedBy,
                            Others = srRepair.Others,
                            IssueDate = srRepair.IssueDate,
                            ReceiveDate = srRepair.ReceiveDate == DateTime.MinValue ? null : srRepair.ReceiveDate,
                            CreBy = srRepair.CreBy,
                            ChgBy = srRepair.ChgBy,
                            CreDate = DateTime.Now.ToUniversalTime(),
                            ChgDate = DateTime.Now.ToUniversalTime(),
                            Remarks = srRepair.Remarks
                        };
                        db.tblSRRepairs.Add(r);
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);
                    }
                    else
                    { // Mode == Edit
                        var line = db.tblSRRepairs.Where(z => z.RepairId == srRepair.RepairId).SingleOrDefault();
                        if (line != null)
                        {
                            line.MachineId = srRepair.MachineId;
                            line.PartId = srRepair.PartId;
                            line.Problem = srRepair.Problem;
                            line.RepairedBy = srRepair.RepairedBy;
                            line.Others = srRepair.Others;
                            line.IssueDate = srRepair.IssueDate;
                            line.ReceiveDate = srRepair.ReceiveDate == DateTime.MinValue ? null : srRepair.ReceiveDate;
                            line.ChgBy = srRepair.ChgBy;
                            line.ChgDate = DateTime.Now.ToUniversalTime();
                            line.Remarks = srRepair.Remarks;
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
        /// GET api/SRRepair
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
                            Label = z.MachineName + " - " + z.SerialNo
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
        /// GET api/SRRepair
        /// retrieve SR Part list
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
        /// POST api/SRRepair
        /// delete SR-Repair Entry
        /// </summary>
        [HttpPost]
        public ApiResponse DeleteSRRepair([FromBody]int id)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var line = db.tblSRRepairs.Where(z => z.RepairId == id).SingleOrDefault();
                    if (line != null)
                    {
                        db.tblSRRepairs.Remove(line);
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
        /// GET api/SRRepair
        /// return SR-Repair list with sorting and filtering  functionalities
        /// </summary>
        [HttpGet]
        public ApiResponse GetSRepairList()
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
                    string srProblemFilter = nvc["filter3"];
                    string srRepairByFilter = nvc["filter4"];
                    string srSerialNo = nvc["filter5"];


                    int iDisplayLength = Convert.ToInt32(nvc["count"]);
                    int iDisplayStart = (page - 1) * iDisplayLength;

                    List<tblSRRepair> list = null;
                    List<SRRepairViewModel> lstRepair = new List<SRRepairViewModel>();
                    try
                    {
                        list = db.tblSRRepairs.ToList();

                        //1. filter SR Problem column if exists
                        if (!string.IsNullOrEmpty(srProblemFilter) && srProblemFilter != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.Problem.ToLower().Contains(srProblemFilter.ToLower())).ToList();
                        }

                        //2. convert returned datetime to local timezone & insert MachineName,SerialNo and Problem col from different table
                        foreach (var l in list)
                        {
                            SRRepairViewModel r = new SRRepairViewModel
                            {
                                RepairId = l.RepairId,
                                MachineId = l.MachineId,
                                MachineName = db.tblSRMachines.Where(z => z.MachineId == l.MachineId).Select(z => z.MachineName).SingleOrDefault(),
                                SerialNo = db.tblSRMachines.Where(z => z.MachineId == l.MachineId).Select(z => z.SerialNo).SingleOrDefault(),
                                PartId = l.PartId,
                                PartName = db.tblSRParts.Where(z => z.PartId == l.PartId).Select(z => z.PartName).SingleOrDefault(),
                                Problem = l.Problem,
                                RepairedBy = l.RepairedBy,
                                RepairMansName = l.RepairedBy.Equals("S") ? "Sarin" : l.Others,
                                Others = l.Others,
                                IssueDate = l.IssueDate,
                                ReceiveDate = l.ReceiveDate,
                                CreBy = l.CreBy,
                                ChgBy = l.ChgBy,
                                CreDate = Convert.ToDateTime(l.CreDate).AddMinutes(-1 * timezone),
                                ChgDate = Convert.ToDateTime(l.ChgDate).AddMinutes(-1 * timezone),
                                Remarks = l.Remarks
                            };
                            lstRepair.Add(r);
                        }

                        //3. filter SR Repairby column if exists
                        if (!string.IsNullOrEmpty(srRepairByFilter) && srRepairByFilter != "undefined")
                        {
                            iDisplayStart = 0;
                            lstRepair = lstRepair.Where(z => z.RepairMansName.ToLower().Contains(srRepairByFilter.ToLower())).ToList();
                        }
                        //4. filter on SR Machine Name col if exists
                        if (!string.IsNullOrEmpty(srMachineNameFilter) && srMachineNameFilter != "undefined")
                        {
                            iDisplayStart = 0;
                            lstRepair = lstRepair.Where(z => z.MachineName.ToLower().Contains(srMachineNameFilter.ToLower())).ToList();
                        }

                        //5. filter on Part Name col if exists
                        if (!string.IsNullOrEmpty(srPartNameFilter) && srPartNameFilter != "undefined")
                        {
                            iDisplayStart = 0;
                            lstRepair = lstRepair.Where(z => z.PartName.ToLower().Contains(srPartNameFilter.ToLower())).ToList();
                        }

                        //6. filter on SerialNO col if exists
                        if (!string.IsNullOrEmpty(srSerialNo) && srSerialNo != "undefined")
                        {
                            iDisplayStart = 0;
                            lstRepair = lstRepair.Where(z => z.SerialNo.ToLower().Contains(srSerialNo.ToLower())).ToList();
                        }

                        //7. do sorting on list
                        lstRepair = DoSorting(lstRepair, orderBy.Trim());

                        //8. take total count to return for ng-table
                        var Count = lstRepair.Count();

                        var resultData = new
                        {
                            total = Count,
                            result = lstRepair.Skip(iDisplayStart).Take(iDisplayLength).ToList()
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
                        lstRepair = null;
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
        public List<SRRepairViewModel> DoSorting(List<SRRepairViewModel> list, string orderBy)
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

                else if (orderBy == "SerialNo")
                {
                    list = list.OrderBy(z => z.SerialNo).ToList();
                }
                else if (orderBy == "-SerialNo")
                {
                    list = list.OrderByDescending(z => z.SerialNo).ToList();
                }
                else if (orderBy == "PartName")
                {
                    list = list.OrderBy(z => z.PartName).ToList();
                }
                else if (orderBy == "-PartName")
                {
                    list = list.OrderByDescending(z => z.PartName).ToList();
                }
                else if (orderBy == "Problem")
                {
                    list = list.OrderBy(z => z.Problem).ToList();
                }
                else if (orderBy == "-Problem")
                {
                    list = list.OrderByDescending(z => z.Problem).ToList();
                }

                else if (orderBy == "RepairMansName")
                {
                    list = list.OrderBy(z => z.RepairMansName).ToList();
                }
                else if (orderBy == "-RepairMansName")
                {
                    list = list.OrderByDescending(z => z.RepairMansName).ToList();
                }

                else if (orderBy == "Others")
                {
                    list = list.OrderBy(z => z.Others).ToList();
                }
                else if (orderBy == "-Others")
                {
                    list = list.OrderByDescending(z => z.Others).ToList();
                }


                else if (orderBy == "IssueDate")
                {
                    list = list.OrderBy(z => z.IssueDate).ToList();
                }
                else if (orderBy == "-IssueDate")
                {
                    list = list.OrderByDescending(z => z.IssueDate).ToList();
                }
                else if (orderBy == "ReceiveDate")
                {
                    list = list.OrderBy(z => z.ReceiveDate).ToList();
                }
                else if (orderBy == "-ReceiveDate")
                {
                    list = list.OrderByDescending(z => z.ReceiveDate).ToList();
                }
                else if (orderBy == "ChgDate")
                {
                    list = list.OrderBy(z => z.ChgDate).ToList();
                }
                else if (orderBy == "-ChgDate")
                {
                    list = list.OrderByDescending(z => z.ChgDate).ToList();
                }
                return list.ToList<SRRepairViewModel>();
            }
            catch
            {
                return null;
            }
        }
    }
}
