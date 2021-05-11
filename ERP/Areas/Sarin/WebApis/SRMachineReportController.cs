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
using System.Data.Entity.SqlServer;

namespace ERP.WebApis
{
    public class SRMachineReportController : ApiController
    {
        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Machine Report";

        public SRMachineReportController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }

        /// <summary>
        /// GET api/SRMachineReport
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
        /// GET api/SRMachineReport
        /// retrieve SR Sub Type list
        /// </summary>
        [HttpGet]
        public ApiResponse RetrieveSRSubTypeList()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    List<tblSRSubType> list = new List<tblSRSubType>();
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    string TypeId = nvc["filterTypes"];
                    list = ReturnFilteredSubTypesList(TypeId);
                    var l = list.Select(z => new SelectItemModel
                         {
                             Id = z.SubTypeId,
                             Label = z.SubTypeName
                         }).ToList();

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", l);
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
        /// retrieve SR Sub Type list filtered from selected Types
        /// </summary>
        protected List<tblSRSubType> ReturnFilteredSubTypesList(string filterTypes)
        {
            List<tblSRSubType> subtypes = new List<tblSRSubType>();
            if (!string.IsNullOrEmpty(filterTypes))
            {
                foreach (var typeid in filterTypes.Split(','))
                {
                    if (!string.IsNullOrEmpty(typeid))
                    {
                        var list = db.tblSRSubTypes.AsEnumerable().Where(z => z.TypeId == Convert.ToInt32(typeid)).ToList();
                        subtypes.AddRange(list);
                    }
                }
            }
            return subtypes.ToList();
        }

        /// <summary>
        /// GET api/SRMachineReport
        /// retrieve SR Parameter list
        /// </summary>
        [HttpGet]
        public ApiResponse RetrieveSRParameterList()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    List<tblSRParameter> list = new List<tblSRParameter>();
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    string SubTypeId = nvc["filterSubTypes"];
                    list = ReturnFilteredParameterList(SubTypeId);

                    var l = list.Select(z => new SelectItemModel
                        {
                            Id = z.ParameterId,
                            Label = z.ParameterName
                        }).ToList();

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", l);
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
        /// retrieve SR Parameter list filtered from selected Sub-Types
        /// </summary>
        protected List<tblSRParameter> ReturnFilteredParameterList(string filterSubTypes)
        {
            List<tblSRParameter> parameters = new List<tblSRParameter>();
            if (!string.IsNullOrEmpty(filterSubTypes))
            {
                foreach (var subtypeid in filterSubTypes.Split(','))
                {
                    if (!string.IsNullOrEmpty(subtypeid))
                    {
                        var list = db.tblSRParameters.AsEnumerable().Where(z => z.SubTypeId == Convert.ToInt32(subtypeid)).ToList();
                        parameters.AddRange(list);
                    }
                }
            }
            return parameters.ToList();
        }

        ///// <summary>
        ///// GET api/SRMachineReport
        ///// retrieve SR Floor Wing list
        ///// </summary>
        //[HttpGet]
        //public ApiResponse RetrieveSRFloorWingsList()
        //{
        //    ApiResponse apiResponse = new ApiResponse();
        //    if (sessionUtils.HasUserLogin())
        //    {
        //        try
        //        {
        //            var list = db.tblSRFloors
        //                .Select(z => new SelectItemModel
        //                {
        //                    Id = z.FloorId
        //                    //,Label = z.LocationId
        //                }).ToList();

        //            apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", list);
        //        }
        //        catch (Exception ex)
        //        {
        //            apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
        //        }
        //    }
        //    else
        //    {
        //        apiResponse = ERPUtilities.GenerateApiResponse();
        //    }

        //    return apiResponse;
        //}

        /// <summary>
        /// GET api/SRMachineReport
        /// retrieve Locations List
        /// </summary>
        [HttpGet]
        public ApiResponse RetrieveLocationsList()
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
        /// GET api/SRMachineReport
        /// return SR-Machine Report data with sorting and filtering  functionalities
        /// </summary>
        [HttpGet]
        public ApiResponse GetMachineReportData()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                using (db)
                {
                    try
                    {
                        #region "Set Filter Parameters"
                        NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                        int page = Convert.ToInt32(nvc["page"]);
                        int timezone = Convert.ToInt32(nvc["timezone"]);
                        string orderBy = nvc["orderby"];
                        string filterTypes = nvc["filterTypes"];
                        string filterSubTypes = nvc["filterSubTypes"];
                        string filterParameters = nvc["filterParameters"];
                        string filterFloorWings = nvc["filterFloorWings"];
                        string filterLocations = nvc["filterLocations"];

                        string srMachineNameFilter = nvc["filter1"]; // for col machine
                        string srSerialNoFilter = nvc["filter2"]; //for col Serial no
                        string srTypeFilter = nvc["filter3"]; // for col SR Type
                        string srSubTypeFilter = nvc["filter4"]; //for col SR Sub-Type
                        string srParameterFilter = nvc["filter5"]; // for col parameter
                        string srLocationFilter = nvc["filter6"]; // for col Location
                        string srManagerFilter = nvc["filter7"]; // for col Manager Name

                        //DateTime fromDate, toDate;
                        //string startDate = nvc["startDate"];
                        //string endDate = nvc["endDate"];

                        int iDisplayLength = Convert.ToInt32(nvc["count"]);
                        int iDisplayStart = (page - 1) * iDisplayLength;

                        //if (!string.IsNullOrEmpty(startDate))
                        //{
                        //    string[] fdate = startDate.Split('/');
                        //    fromDate = new DateTime(Convert.ToInt32(fdate[2]), Convert.ToInt32(fdate[1]), Convert.ToInt32(fdate[0]));
                        //}
                        //else
                        //{
                        //    fromDate = Convert.ToDateTime("1/1/1900");
                        //}

                        //if (!string.IsNullOrEmpty(endDate))
                        //{
                        //    string[] tdate = endDate.Split('/');
                        //    toDate = new DateTime(Convert.ToInt32(tdate[2]), Convert.ToInt32(tdate[1]), Convert.ToInt32(tdate[0]));
                        //}
                        //else
                        //{
                        //    toDate = Convert.ToDateTime("1/1/1900");
                        //}
                        #endregion

                        List<tblSRMachine> listMachine = null;
                        List<tblSRFloor> listFloor = null;
                        List<tblLocation> listLocation = null;
                        List<tblSRPartIssue> listPartIssue = null;
                        List<SRMachineReportViewModel> lstMachineReport = new List<SRMachineReportViewModel>();

                        listMachine = db.tblSRMachines.ToList();
                        listFloor = db.tblSRFloors.ToList();
                        listLocation = db.tblLocations.ToList();
                        listPartIssue = db.tblSRPartIssues.ToList();

                        lstMachineReport = (from m in listMachine
                                            join f in listFloor on m.MachineId equals f.MachineId
                                            join l in listLocation on f.LocationId equals l.LocationId
                                            //join a in listPartIssue on m.MachineId equals a.MachineId
                                            select new SRMachineReportViewModel
                                    {
                                        MachineId = m.MachineId,
                                        MachineName = m.MachineName,
                                        SerialNo = m.SerialNo,
                                        InstallationDate = m.InstallationDate,
                                        TypeId = m.TypeId,
                                        TypeName = db.tblSRTypes.Where(z => z.TypeId == m.TypeId).Select(z => z.TypeName).SingleOrDefault(),
                                        SubTypeId = m.SubTypeId,
                                        SubTypeName = db.tblSRSubTypes.Where(z => z.SubTypeId == m.SubTypeId).Select(z => z.SubTypeName).SingleOrDefault(),
                                        ParameterId = m.ParameterId,
                                        ParameterName = db.tblSRParameters.Where(z => z.ParameterId == m.ParameterId).Select(z => z.ParameterName).SingleOrDefault(),
                                        FloorId = f.FloorId,
                                        MachineLocationId = l.LocationId,
                                        LocationName = l.LocationName,
                                        ManagerName = f.Manager
                                        //,IssueDate = a.IssuedDate
                                    }).ToList();


                        //1. filter SR machine name column if exists
                        if (!string.IsNullOrEmpty(srMachineNameFilter) && srMachineNameFilter != "undefined")
                        {
                            iDisplayStart = 0;
                            lstMachineReport = lstMachineReport.Where(z => z.MachineName.ToLower().Contains(srMachineNameFilter.ToLower())).ToList();
                        }

                        //2. filter Serial no column if exists
                        if (!string.IsNullOrEmpty(srSerialNoFilter) && srSerialNoFilter != "undefined")
                        {
                            iDisplayStart = 0;
                            lstMachineReport = lstMachineReport.Where(z => z.SerialNo.ToLower().Contains(srSerialNoFilter.ToLower())).ToList();
                        }

                        //3. filter on SR Type col if exists
                        if (!string.IsNullOrEmpty(srTypeFilter) && srTypeFilter != "undefined")
                        {
                            iDisplayStart = 0;
                            lstMachineReport = lstMachineReport.Where(z => z.TypeName.ToLower().Contains(srTypeFilter.ToLower())).ToList();
                        }

                        //4. filter on sub type col if exists
                        if (!string.IsNullOrEmpty(srSubTypeFilter) && srSubTypeFilter != "undefined")
                        {
                            iDisplayStart = 0;
                            lstMachineReport = lstMachineReport.Where(z => z.SubTypeName.ToLower().Contains(srSubTypeFilter.ToLower())).ToList();
                        }

                        //4. filter on parameter col if exists
                        if (!string.IsNullOrEmpty(srParameterFilter) && srParameterFilter != "undefined")
                        {
                            iDisplayStart = 0;
                            lstMachineReport = lstMachineReport.Where(z => z.ParameterName.ToLower().Contains(srParameterFilter.ToLower())).ToList();
                        }

                        //5. filter on Location col if exists
                        if (!string.IsNullOrEmpty(srLocationFilter) && srLocationFilter != "undefined")
                        {
                            iDisplayStart = 0;
                            int locId = Convert.ToInt32(srLocationFilter);
                            lstMachineReport = lstMachineReport.Where(z => z.MachineLocationId == locId).ToList();
                        }

                        //6. filter on Location col if exists
                        if (!string.IsNullOrEmpty(srManagerFilter) && srManagerFilter != "undefined")
                        {
                            iDisplayStart = 0;
                            lstMachineReport = lstMachineReport.Where(z => z.ManagerName.ToLower().Contains(srManagerFilter.ToLower())).ToList();
                        }

                        // Filter Types If Exist
                        int[] arrType = convertStringToIntArray(filterTypes);
                        if (arrType.Length > 1)
                        {
                            lstMachineReport = (from m in lstMachineReport
                                                where arrType.Contains(m.TypeId)
                                                select m).ToList();
                        }

                        // Filter Sub Types If Exist
                        int[] arrSubType = convertStringToIntArray(filterSubTypes);
                        if (arrSubType.Length > 1)
                        {
                            lstMachineReport = (from m in lstMachineReport
                                                where arrSubType.Contains(m.SubTypeId)
                                                select m).ToList();
                        }

                        // Filter Paramteres If Exist
                        int[] arrParameterType = convertStringToIntArray(filterParameters);
                        if (arrParameterType.Length > 1)
                        {
                            lstMachineReport = (from m in lstMachineReport
                                                where arrParameterType.Contains(m.ParameterId)
                                                select m).ToList();
                        }

                        // Filter Locations If Exist
                        int[] arrLocation = convertStringToIntArray(filterLocations);
                        if (arrLocation.Length > 1)
                        {
                            lstMachineReport = (from m in lstMachineReport
                                                where arrLocation.Contains(m.MachineLocationId)
                                                select m).ToList();
                        }

                        //// Filter From Date If Exist
                        //if (fromDate.ToString("d/M/yyyy") != "1/1/1900" && toDate.ToString("d/M/yyyy") != "1/1/1900")
                        //{
                        //    lstMachineReport = (from m in lstMachineReport
                        //                        where m.IssueDate >= fromDate && m.IssueDate <= toDate
                        //                        select m).ToList();
                        //}

                        //5. do sorting on list
                        lstMachineReport = DoSorting(lstMachineReport, orderBy.Trim());

                        //6. take total count to return for ng-table
                        var Count = lstMachineReport.Count();

                        var resultData = new
                        {
                            total = Count,
                            result = lstMachineReport.Skip(iDisplayStart).Take(iDisplayLength).ToList()
                        };
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", resultData);

                    }
                    catch (Exception ex)
                    {
                        apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
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
        public List<SRMachineReportViewModel> DoSorting(List<SRMachineReportViewModel> list, string orderBy)
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
                else if (orderBy == "LocationName")
                {
                    list = list.OrderBy(z => z.LocationName).ToList();
                }
                else if (orderBy == "-LocationName")
                {
                    list = list.OrderByDescending(z => z.LocationName).ToList();
                }
                else if (orderBy == "ManagerName")
                {
                    list = list.OrderBy(z => z.ManagerName).ToList();
                }
                else if (orderBy == "-ManagerName")
                {
                    list = list.OrderByDescending(z => z.ManagerName).ToList();
                }
                else if (orderBy == "IssueDate")
                {
                    list = list.OrderBy(z => z.IssueDate).ToList();
                }
                else if (orderBy == "-IssueDate")
                {
                    list = list.OrderByDescending(z => z.IssueDate).ToList();
                }
                return list.ToList<SRMachineReportViewModel>();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// return int array converted from comma(,) separated string
        /// </summary>
        int[] convertStringToIntArray(string commaSeparate)
        {
            string[] ArrString = commaSeparate.Split(',');
            int[] ArrInt = new int[ArrString.Length];
            if (ArrString.Length > 1)
            {
                for (int i = 0; i < ArrString.Length - 1; i++)
                {
                    ArrInt[i] = int.Parse(ArrString[i]);
                }
            }
            return ArrInt;
        }

    }
}
