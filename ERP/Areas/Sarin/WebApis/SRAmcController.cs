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
    public class SRAmcController : ApiController
    {
        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "AMC";

        public SRAmcController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }

        /// <summary>
        /// POST api/SRAmc
        /// create and update record
        /// </summary>
        [HttpPost]
        public ApiResponse SaveSRAmc(tblSRAMC srAMC)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    string[] selectedMachine = nvc["selectedMachine"].Split(',');
                    if (selectedMachine[0] != "")
                    {
                        for (int i = 0; i < selectedMachine.Length; i++)
                        {
                            int machineId = Convert.ToInt32(selectedMachine[i]);
                            var line = db.tblSRAMCs.Where(z => z.MachineId == machineId).SingleOrDefault();
                            if (line != null)
                            {
                                line.StartDate = srAMC.StartDate;
                                line.EndDate = srAMC.EndDate;
                                line.ChgBy = srAMC.ChgBy;
                                line.ChgDate = DateTime.Now.ToUniversalTime();
                                line.Remarks = srAMC.Remarks;
                            }
                            else
                            {
                                tblSRAMC objTblSRAMC = new tblSRAMC
                                {
                                    MachineId = machineId,
                                    StartDate = srAMC.StartDate,
                                    EndDate = srAMC.EndDate,
                                    CreBy = srAMC.CreBy,
                                    ChgBy = srAMC.ChgBy,
                                    CreDate = DateTime.Now.ToUniversalTime(),
                                    ChgDate = DateTime.Now.ToUniversalTime(),
                                    Remarks = srAMC.Remarks
                                };
                                db.tblSRAMCs.Add(objTblSRAMC);
                            }
                        }
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);
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
        /// GET api/SRAmc
        /// retrieve SR-Machine
        /// </summary>
        [HttpGet]
        public ApiResponse RetrieveSRMachineList()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var listM = db.tblSRMachines.Except(from a in db.tblSRAMCs
                                                        from m in db.tblSRMachines
                                                        where a.MachineId == m.MachineId
                                                        select m).ToList();
                    
                    var list = listM.OrderBy(z => z.MachineName)
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
        /// POST api/SRAmc
        /// delete SR SR-AMC
        /// </summary>
        [HttpPost]
        public ApiResponse DeleteSRAmc([FromBody]int id)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var line = db.tblSRAMCs.Where(z => z.AMCId == id).SingleOrDefault();
                    if (line != null)
                    {
                        db.tblSRAMCs.Remove(line);
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
        /// GET api/SRAmc
        /// return SR-AMC list with sorting and filtering  functionalities
        /// </summary>
        [HttpGet]
        public ApiResponse GetSRAmcList()
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
                    string srMachineNameFilter = nvc["filter1"]; //for col SR Machine name

                    int iDisplayLength = Convert.ToInt32(nvc["count"]);
                    int iDisplayStart = (page - 1) * iDisplayLength;

                    List<tblSRAMC> list = null;
                    List<SRAmcViewModel> lstAmc = new List<SRAmcViewModel>();
                    try
                    {
                        list = db.tblSRAMCs.ToList();

                        //1. convert returned datetime to local timezone & insert machine name from different table
                        foreach (var l in list)
                        {
                            SRAmcViewModel s = new SRAmcViewModel
                            {
                                AMCId = l.AMCId,
                                MachineId = l.MachineId,
                                MachineName = db.tblSRMachines.Where(z => z.MachineId == l.MachineId).Select(z => z.MachineName).SingleOrDefault(),
                                StartDate = l.StartDate,
                                EndDate = l.EndDate,
                                CreBy = l.CreBy,
                                ChgBy = l.ChgBy,
                                CreDate = Convert.ToDateTime(l.CreDate).AddMinutes(-1 * timezone),
                                ChgDate = Convert.ToDateTime(l.ChgDate).AddMinutes(-1 * timezone),
                                Remarks = l.Remarks
                            };
                            lstAmc.Add(s);
                        }

                        //2. filter on Machine Name col if exists
                        if (!string.IsNullOrEmpty(srMachineNameFilter) && srMachineNameFilter != "undefined")
                        {
                            iDisplayStart = 0;
                            lstAmc = lstAmc.Where(z => z.MachineName.ToLower().Contains(srMachineNameFilter.ToLower())).ToList();
                        }

                        //3. do sorting on list
                        lstAmc = DoSorting(lstAmc, orderBy.Trim());

                        //4 take total count to return for ng-table
                        var Count = lstAmc.Count();

                        var resultData = new
                        {
                            total = Count,
                            result = lstAmc.Skip(iDisplayStart).Take(iDisplayLength).ToList()
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
                        lstAmc = null;
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
        public List<SRAmcViewModel> DoSorting(List<SRAmcViewModel> list, string orderBy)
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
                else if (orderBy == "StartDate")
                {
                    list = list.OrderBy(z => z.StartDate).ToList();
                }
                else if (orderBy == "-StartDate")
                {
                    list = list.OrderByDescending(z => z.StartDate).ToList();
                }
                else if (orderBy == "EndDate")
                {
                    list = list.OrderBy(z => z.EndDate).ToList();
                }
                else if (orderBy == "-EndDate")
                {
                    list = list.OrderByDescending(z => z.EndDate).ToList();
                }
                else if (orderBy == "ChgDate")
                {
                    list = list.OrderBy(z => z.ChgDate).ToList();
                }
                else if (orderBy == "-ChgDate")
                {
                    list = list.OrderByDescending(z => z.ChgDate).ToList();
                }
                return list.ToList<SRAmcViewModel>();
            }
            catch
            {
                return null;
            }
        }

    }
}
