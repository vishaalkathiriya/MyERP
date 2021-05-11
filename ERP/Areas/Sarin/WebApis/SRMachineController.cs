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
    public class SRMachineController : ApiController
    {
        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Machine";

        public SRMachineController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }


        /// <summary>
        /// POST api/SRMachine
        /// create and update record
        /// </summary>
        [HttpPost]
        public ApiResponse SaveSRMachien(tblSRMachine machine)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (machine.MachineId == 0) // Mode == Add
                    {
                        tblSRMachine m = new tblSRMachine
                        {
                            MachineName = machine.MachineName,
                            SerialNo = machine.SerialNo,
                            InstallationDate = machine.InstallationDate,
                            TypeId = machine.TypeId,
                            SubTypeId = machine.SubTypeId,
                            ParameterId = machine.ParameterId,
                            CreBy = machine.CreBy,
                            ChgBy = machine.ChgBy,
                            CreDate = DateTime.Now.ToUniversalTime(),
                            ChgDate = DateTime.Now.ToUniversalTime(),
                            Remarks = machine.Remarks
                        };
                        db.tblSRMachines.Add(m);
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);
                    }
                    else
                    { // Mode == Edit
                        var line = db.tblSRMachines.Where(z => z.MachineId == machine.MachineId).SingleOrDefault();
                        if (line != null)
                        {
                            line.MachineName = machine.MachineName;
                            line.SerialNo = machine.SerialNo;
                            line.InstallationDate = machine.InstallationDate;
                            line.TypeId = machine.TypeId;
                            line.SubTypeId = machine.SubTypeId;
                            line.ParameterId = machine.ParameterId;
                            line.ChgBy = machine.ChgBy;
                            line.ChgDate = DateTime.Now.ToUniversalTime();
                            line.Remarks = machine.Remarks;

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
        /// GET api/SRMachine
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
        /// GET api/SRMachine
        /// retrieve SR Sub Type list
        /// </summary>
        [HttpGet]
        public ApiResponse RetrieveSRSubTypeListByTypeId()
        {
            NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            int TypeId = Convert.ToInt32(nvc["TypeId"]);
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var list = db.tblSRSubTypes.OrderBy(z => z.SubTypeName)
                        .Where(t => t.TypeId == TypeId)
                        .Select(z => new SelectItemModel
                        {
                            Id = z.SubTypeId,
                            Label = z.SubTypeName
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
        /// GET api/SRMachine
        /// retrieve SR Parameter list
        /// </summary>
        [HttpGet]
        public ApiResponse RetrieveSRParameterListBySubTypeId()
        {
            NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            int SubTypeId = Convert.ToInt32(nvc["SubTypeId"]);
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var list = db.tblSRParameters.OrderBy(z => z.ParameterName)
                        .Where(t => t.SubTypeId == SubTypeId)
                        .Select(z => new SelectItemModel
                        {
                            Id = z.ParameterId,
                            Label = z.ParameterName
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
        /// POST api/SRMachine
        /// delete SR Machine
        /// </summary>
        [HttpPost]
        public ApiResponse DeleteSRMachine([FromBody]int id)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (DependancyStatus.SRMachineStatus(id))
                    {
                        var line = db.tblSRMachines.Where(z => z.MachineId == id).SingleOrDefault();
                        if (line != null)
                        {
                            db.tblSRMachines.Remove(line);
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
        /// GET api/SRMachine
        /// return SR-Machine list with sorting and filtering  functionalities
        /// </summary>
        [HttpGet]
        public ApiResponse GetSRMachineList()
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

                    string srMachineNameFilter = nvc["filter1"]; // for col machine
                    string srSerialNoFilter = nvc["filter2"]; //for col Serial no
                    string srTypeFilter = nvc["filter3"]; // for col SR Type
                    string srSubTypeFilter = nvc["filter4"]; //for col SR Sub-Type
                    string srParameterFilter = nvc["filter5"]; // for col parameter

                    int iDisplayLength = Convert.ToInt32(nvc["count"]);
                    int iDisplayStart = (page - 1) * iDisplayLength;

                    List<tblSRMachine> list = null;
                    List<SRMachineViewModel> lstMachine = new List<SRMachineViewModel>();
                    try
                    {
                        list = db.tblSRMachines.ToList();

                        //1. filter SR machine name column if exists
                        if (!string.IsNullOrEmpty(srMachineNameFilter) && srMachineNameFilter != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.MachineName.ToLower().Contains(srMachineNameFilter.ToLower())).ToList();
                        }

                        //2. filter Serial no column if exists
                        if (!string.IsNullOrEmpty(srSerialNoFilter) && srSerialNoFilter != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.SerialNo.ToLower().Contains(srSerialNoFilter.ToLower())).ToList();
                        }

                        //3. convert returned datetime to local timezone & insert Subtype,Type and Parameter col from different table
                        foreach (var l in list)
                        {
                            SRMachineViewModel m = new SRMachineViewModel
                            {
                                MachineId = l.MachineId,
                                MachineName = l.MachineName,
                                SerialNo = l.SerialNo,
                                InstallationDate = l.InstallationDate,
                                TypeId = l.TypeId,
                                TypeName = db.tblSRTypes.Where(z => z.TypeId == l.TypeId).Select(z => z.TypeName).SingleOrDefault(),
                                SubTypeId = l.SubTypeId,
                                SubTypeName = db.tblSRSubTypes.Where(z => z.SubTypeId == l.SubTypeId).Select(z => z.SubTypeName).SingleOrDefault(),
                                ParameterId = l.ParameterId,
                                ParameterName = db.tblSRParameters.Where(z => z.ParameterId == l.ParameterId).Select(z => z.ParameterName).SingleOrDefault(),
                                CreBy = l.CreBy,
                                ChgBy = l.ChgBy,
                                CreDate = Convert.ToDateTime(l.CreDate).AddMinutes(-1 * timezone),
                                ChgDate = Convert.ToDateTime(l.ChgDate).AddMinutes(-1 * timezone),
                                Remarks = l.Remarks
                            };
                            lstMachine.Add(m);
                        }

                        //4. filter on SR Type col if exists
                        if (!string.IsNullOrEmpty(srTypeFilter) && srTypeFilter != "undefined")
                        {
                            iDisplayStart = 0;
                            lstMachine = lstMachine.Where(z => z.TypeName.ToLower().Contains(srTypeFilter.ToLower())).ToList();
                        }

                        //5. filter on sub type col if exists
                        if (!string.IsNullOrEmpty(srSubTypeFilter) && srSubTypeFilter != "undefined")
                        {
                            iDisplayStart = 0;
                            lstMachine = lstMachine.Where(z => z.SubTypeName.ToLower().Contains(srSubTypeFilter.ToLower())).ToList();
                        }

                        //6. filter on parameter col if exists
                        if (!string.IsNullOrEmpty(srParameterFilter) && srParameterFilter != "undefined")
                        {
                            iDisplayStart = 0;
                            lstMachine = lstMachine.Where(z => z.ParameterName.ToLower().Contains(srParameterFilter.ToLower())).ToList();
                        }

                        //7. do sorting on list
                        lstMachine = DoSorting(lstMachine, orderBy.Trim());

                        //8. take total count to return for ng-table
                        var Count = lstMachine.Count();

                        var resultData = new
                        {
                            total = Count,
                            result = lstMachine.Skip(iDisplayStart).Take(iDisplayLength).ToList()
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
                        lstMachine = null;
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
        public List<SRMachineViewModel> DoSorting(List<SRMachineViewModel> list, string orderBy)
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
                else if (orderBy == "SubTypeName")
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
                else if (orderBy == "ParameterName")
                {
                    list = list.OrderBy(z => z.ParameterName).ToList();
                }
                else if (orderBy == "-ParameterName")
                {
                    list = list.OrderByDescending(z => z.ParameterName).ToList();
                }
                else if (orderBy == "ChgDate")
                {
                    list = list.OrderBy(z => z.ChgDate).ToList();
                }
                else if (orderBy == "-ChgDate")
                {
                    list = list.OrderByDescending(z => z.ChgDate).ToList();
                }
                else if (orderBy == "InstallationDate")
                {
                    list = list.OrderBy(z => z.InstallationDate).ToList();
                }
                else if (orderBy == "-InstallationDate")
                {
                    list = list.OrderByDescending(z => z.InstallationDate).ToList();
                }
                return list.ToList<SRMachineViewModel>();
            }
            catch
            {
                return null;
            }
        }


    }
}
