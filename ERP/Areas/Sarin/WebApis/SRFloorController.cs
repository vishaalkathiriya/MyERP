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
    public class SRFloorController : ApiController
    {
        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Floor";

        public SRFloorController()
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
        public ApiResponse SaveSRFloor(tblSRFloor srFloor)
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
                            var line = db.tblSRFloors.Where(z => z.MachineId == machineId).SingleOrDefault();
                            if (line != null)
                            {
                                line.LocationId = srFloor.LocationId;
                                line.Manager = srFloor.Manager;
                                line.ChgDate = DateTime.Now.ToUniversalTime();
                                line.Remarks = srFloor.Remarks;
                            }
                            else
                            {
                                tblSRFloor f = new tblSRFloor
                                {
                                    MachineId = machineId,
                                    LocationId = srFloor.LocationId,
                                    Manager = srFloor.Manager,
                                    CreBy = srFloor.CreBy,
                                    ChgBy = srFloor.ChgBy,
                                    CreDate = DateTime.Now.ToUniversalTime(),
                                    ChgDate = DateTime.Now.ToUniversalTime(),
                                    Remarks = srFloor.Remarks
                                };
                                db.tblSRFloors.Add(f);
                            }
                        }
                    }
                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);
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
        /// GET api/SRFloor
        /// retrieve SR-Machine list
        /// </summary>
        [HttpGet]
        public ApiResponse RetrieveSRMachineList()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var listM = db.tblSRMachines.Except(from a in db.tblSRFloors
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
        /// GET api/SRFloor
        /// retrieve Locations list
        /// </summary>
        [HttpGet]
        public ApiResponse RetrieveLocationList()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var list = db.tblLocations.OrderBy(z => z.LocationName)
                    .Select(z => new SelectItemModel
                    {
                        Id = z.LocationId,
                        Label = z.LocationName
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
        /// POST api/SRFloor
        /// delete SR Floor
        /// </summary>
        [HttpPost]
        public ApiResponse DeleteSRFloor([FromBody]int id)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var line = db.tblSRFloors.Where(z => z.FloorId == id).SingleOrDefault();
                    if (line != null)
                    {
                        db.tblSRFloors.Remove(line);
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
        /// GET api/SRFloor
        /// return SR-Floor list with sorting and filtering  functionalities
        /// </summary>
        [HttpGet]
        public ApiResponse GetSRFloorList()
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
                    string srLocationNameFilter = nvc["filter2"];
                    string srManagerFilter = nvc["filter3"];
                    string srMachineSerialNoFilter = nvc["filter4"];

                    int iDisplayLength = Convert.ToInt32(nvc["count"]);
                    int iDisplayStart = (page - 1) * iDisplayLength;

                    List<tblSRFloor> list = null;
                    List<SRFloorViewModel> lstFloor = new List<SRFloorViewModel>();
                    try
                    {
                        list = db.tblSRFloors.ToList();

                        //2. convert returned datetime to local timezone & insert MachineName col from different table
                        foreach (var l in list)
                        {
                            SRFloorViewModel f = new SRFloorViewModel
                            {
                                FloorId = l.FloorId,
                                MachineId = l.MachineId,
                                MachineName = db.tblSRMachines.Where(z => z.MachineId == l.MachineId).Select(z => z.MachineName).SingleOrDefault(),
                                SerialNo = db.tblSRMachines.Where(z => z.MachineId == l.MachineId).Select(z => z.SerialNo).SingleOrDefault(),
                                LocationId = l.LocationId,
                                LocationName = db.tblLocations.Where(z => z.LocationId == l.LocationId).Select(z => z.LocationName).SingleOrDefault(),
                                Manager = l.Manager,
                                CreBy = l.CreBy,
                                ChgBy = l.ChgBy,
                                CreDate = Convert.ToDateTime(l.CreDate).AddMinutes(-1 * timezone),
                                ChgDate = Convert.ToDateTime(l.ChgDate).AddMinutes(-1 * timezone),
                                Remarks = l.Remarks
                            };
                            lstFloor.Add(f);
                        }

                        //3. filter on machine name col if exists
                        if (!string.IsNullOrEmpty(srMachineNameFilter) && srMachineNameFilter != "undefined")
                        {
                            iDisplayStart = 0;
                            lstFloor = lstFloor.Where(z => z.MachineName.ToLower().Contains(srMachineNameFilter.ToLower())).ToList();
                        }

                        //4. filter on Location col if exists
                        if (!string.IsNullOrEmpty(srLocationNameFilter) && srLocationNameFilter != "undefined")
                        {
                            iDisplayStart = 0;
                            lstFloor = lstFloor.Where(z => z.LocationName.ToLower().Contains(srLocationNameFilter.ToLower())).ToList();
                        }

                        //5. filter on manager name col if exists
                        if (!string.IsNullOrEmpty(srManagerFilter) && srManagerFilter != "undefined")
                        {
                            iDisplayStart = 0;
                            lstFloor = lstFloor.Where(z => z.Manager.ToLower().Contains(srManagerFilter.ToLower())).ToList();
                        }

                        //6. filter on machine serialno col if exists
                        if (!string.IsNullOrEmpty(srMachineSerialNoFilter) && srMachineSerialNoFilter != "undefined")
                        {
                            iDisplayStart = 0;
                            lstFloor = lstFloor.Where(z => z.SerialNo.ToLower().Contains(srMachineSerialNoFilter.ToLower())).ToList();
                        }

                        //6. do sorting on list
                        lstFloor = DoSorting(lstFloor, orderBy.Trim());

                        //7. take total count to return for ng-table
                        var Count = lstFloor.Count();

                        var resultData = new
                        {
                            total = Count,
                            result = lstFloor.Skip(iDisplayStart).Take(iDisplayLength).ToList()
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
                        lstFloor = null;
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
        public List<SRFloorViewModel> DoSorting(List<SRFloorViewModel> list, string orderBy)
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
                else if (orderBy == "LocationName")
                {
                    list = list.OrderBy(z => z.LocationName).ToList();
                }
                else if (orderBy == "-LocationName")
                {
                    list = list.OrderByDescending(z => z.LocationName).ToList();
                }
                else if (orderBy == "Manager")
                {
                    list = list.OrderBy(z => z.Manager).ToList();
                }
                else if (orderBy == "-Manager")
                {
                    list = list.OrderByDescending(z => z.Manager).ToList();
                }
                else if (orderBy == "SerialNo")
                {
                    list = list.OrderBy(z => z.SerialNo).ToList();
                }
                else if (orderBy == "-SerialNo")
                {
                    list = list.OrderByDescending(z => z.SerialNo).ToList();
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
                return list.ToList<SRFloorViewModel>();
            }
            catch
            {
                return null;
            }
        }

    }
}
