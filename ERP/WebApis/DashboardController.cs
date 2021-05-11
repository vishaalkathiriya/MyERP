using ERP.Models;
using ERP.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace ERP.WebApis
{
    public class DashboardController : ApiController
    {
        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Dashboard";

        public DashboardController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
            // GetInOutInformation();
        }

        // empLeaveEndDate in  this values to be nullable
        protected DashboardInfoListViewModel AddToList(int eid, string empName, String profilePhoto, DateTime onDate, DateTime? empLeaveEndDate, string type, string colorCode, string onDateWeekName, string endDateWeekName, string leaveTime, Guid? festivalGroupId)
        {
            DashboardInfoListViewModel d = new DashboardInfoListViewModel
            {
                EmployeeId = eid,
                EmployeeName = empName,
                Type = type,
                OnDate = onDate.Date,
                EndDate = empLeaveEndDate,
                ColorCode = colorCode,
                ProfilePhoto = profilePhoto,
                OnDateWeekName = onDateWeekName,
                EndDateWeekName = endDateWeekName,
                LeaveTime = leaveTime,
                FestivalGroupId = festivalGroupId
            };

            return d;
        }

        //recursive function use for find out leave end date
        DateTime enddate;
        public DateTime EndDate(DateTime startDate, int employeeId)
        {
            List<DateTime> dateArray = new List<DateTime>();
            dateArray.Add(startDate);
            if (dateArray.Count() > 0)
            {
                enddate = dateArray.LastOrDefault().AddDays(1);
                var leave = db.tblApplyLeaves.AsEnumerable().Where(z => z.StartDate.Date == enddate && (z.Status == "Approved" || z.Status == "Pending") && z.EmployeeId == employeeId).FirstOrDefault();

                if (leave == null)
                {
                    return dateArray.LastOrDefault();
                }
            }
            return EndDate(enddate, employeeId);
        }


        //recursive function use for find out leave end date
        DateTime startDate;
        public DateTime StartDate(DateTime startDate, int employeeId)
        {
            List<DateTime> dateArray = new List<DateTime>();
            dateArray.Add(startDate);

            if (dateArray.Count() > 0)
            {
                startDate = dateArray.LastOrDefault().AddDays(-1);
                var leave = db.tblApplyLeaves.AsEnumerable().Where(z => z.StartDate.Date == startDate && (z.Status == "Approved" || z.Status == "Pending") && z.EmployeeId == employeeId).FirstOrDefault();

                if (leave == null)
                {
                    return dateArray.LastOrDefault();
                }
            }
            return StartDate(startDate, employeeId);
        }

        [HttpGet]
        public ApiResponse GetInformationList()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    List<DashboardInfoListViewModel> infoList = new List<DashboardInfoListViewModel>();
                    List<tblEmpCompanyInformation> cList = new List<tblEmpCompanyInformation>();
                    List<DashboardInfoListViewModel> spInfoList = new List<DashboardInfoListViewModel>();

                    int employeeId = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                    int repotingId = db.tblEmpCompanyInformations.Where(z => z.EmployeeId == employeeId).Select(z => z.ReportingTo).FirstOrDefault();
                    EmployeeUtils employeeUtils = new EmployeeUtils();

                    // BEGIN COMMENTED BY DRCVHK
                    //var myLine = db.tblEmpCompanyInformations.Where(z => z.EmployeeId == employeeId).FirstOrDefault();
                    //if (repotingId == 1)
                    //{
                    //    cList = employeeUtils.DrillDownReporting(myLine.ReportingTo.ToString(), cList);
                    //}
                    //else
                    //{
                    //    // pass login user's employee id

                    //    if (myLine != null)
                    //    {

                    //        cList = employeeUtils.DrillDownReporting(myLine.ReportingTo.ToString(), cList);
                    //        cList = cList.Where(z => z.ReportingTo != repotingId && z.EmployeeId != employeeId).ToList();
                    //    }
                    //}
                    // END COMMENTED BY DRCVHK

                    var sameLevelList = db.tblEmpCompanyInformations.Where(z => z.ReportingTo == repotingId).ToList();
                    foreach (var item in sameLevelList)
                    {
                        cList.Add(item);
                    }
                    cList = employeeUtils.DrillDownReporting(employeeId.ToString(), cList);
                    //if (repotingId != 1)
                    //{
                    //    cList = cList.Where(z => z.ReportingTo != repotingId && z.EmployeeId == employeeId).ToList();
                    //}


                    // iterate down level users out of for loop because to one time read employee id
                    //DateTime dat = DateTime.Now;
                    //foreach (var l in cList)
                    //{
                    //    var leave = db.tblApplyLeaves.AsEnumerable().Where(z => z.StartDate.Date == dat.Date && z.PartFullTime == "F" && z.Status == "Approved" && z.EmployeeId == l.EmployeeId).FirstOrDefault();
                    //    if (leave != null)
                    //    {
                    //        DateTime empLeaveEndDate = EndDate(leave.StartDate, leave.EmployeeId);
                    //        string empName = db.tblEmpPersonalInformations.Where(z => z.EmployeeId == leave.EmployeeId).Select(z => z.CandidateFirstName + " " + z.CandidateLastName).FirstOrDefault();
                    //        string profilePhoto = db.tblEmpPersonalInformations.Where(z => z.EmployeeId == l.EmployeeId).Select(z => z.ProfilePhoto).FirstOrDefault();
                    //        infoList.Add(AddToList(leave.EmployeeId, empName, profilePhoto, leave.StartDate, empLeaveEndDate, "Paid Leave", "#E9573E"));
                    //    }
                    //}




                    List<tblApplyLeave> leave1 = new List<tblApplyLeave>();
                    DateTime dt = DateTime.Now;

                    foreach (var l in cList)
                    {

                        leave1 = db.tblApplyLeaves.AsEnumerable().Where(z => z.StartDate.Date >= dt.Date && (z.Status == "Approved" || z.Status == "Pending" || z.Status == "DisApproved") && z.EmployeeId == l.EmployeeId).ToList();

                        if (leave1 != null)
                        {
                            foreach (var leave in leave1)
                            {
                                if (leave != null)
                                {
                                    DateTime empLeaveEndDate = EndDate(leave.StartDate, leave.EmployeeId);
                                    DateTime empLeaveStartDate = StartDate(leave.StartDate, leave.EmployeeId);
                                    string empName = db.tblEmpPersonalInformations.Where(z => z.EmployeeId == leave.EmployeeId).Select(z => z.CandidateFirstName + (z.CandidateMiddleName.Length > 1 ? z.CandidateMiddleName.Substring(0, 1) : z.CandidateMiddleName) + (z.CandidateLastName.Length > 1 ? z.CandidateLastName.Substring(0, 1) : z.CandidateLastName)).FirstOrDefault();
                                    string profilePhoto = db.tblEmpPersonalInformations.Where(z => z.EmployeeId == l.EmployeeId).Select(z => z.ProfilePhoto).FirstOrDefault();

                                    int recored_exist = infoList.Where(z => z.OnDate == empLeaveStartDate && z.EndDate == empLeaveEndDate && z.EmployeeId == leave.EmployeeId).Count();
                                    if (recored_exist <= 0)
                                    {
                                        if (leave.Status == "Approved")
                                        {
                                            if (leave.PartFullTime == "F")
                                            {
                                                infoList.Add(AddToList(leave.EmployeeId, empName, profilePhoto, empLeaveStartDate, empLeaveEndDate, "Approved Leave", "#8DC153", empLeaveStartDate.DayOfWeek.ToString().Substring(0, 3), empLeaveEndDate.DayOfWeek.ToString().Substring(0, 3), "0", null));

                                            }
                                            if (leave.PartFullTime == "P")
                                            {
                                                infoList.Add(AddToList(leave.EmployeeId, empName, profilePhoto, empLeaveStartDate, empLeaveEndDate, "Approved Leave", "#8DC153", empLeaveStartDate.DayOfWeek.ToString().Substring(0, 3), empLeaveEndDate.DayOfWeek.ToString().Substring(0, 3), leave.StartDate.TimeOfDay.Hours.ToString("00") + ":" + leave.StartDate.TimeOfDay.Minutes.ToString("00") + "-" + leave.EndDate.TimeOfDay.Hours.ToString("00") + ":" + leave.EndDate.TimeOfDay.Minutes.ToString("00"), null));
                                            }
                                        }
                                        if (leave.Status == "Pending")
                                        {
                                            if (leave.PartFullTime == "F")
                                            {
                                                infoList.Add(AddToList(leave.EmployeeId, empName, profilePhoto, empLeaveStartDate, empLeaveEndDate, "Pending Leave", "#f6bb4a", empLeaveStartDate.DayOfWeek.ToString().Substring(0, 3), empLeaveEndDate.DayOfWeek.ToString().Substring(0, 3), "0", null));
                                            }
                                            if (leave.PartFullTime == "P")
                                            {
                                                infoList.Add(AddToList(leave.EmployeeId, empName, profilePhoto, empLeaveStartDate, empLeaveEndDate, "Pending Leave", "#f6bb4a", empLeaveStartDate.DayOfWeek.ToString().Substring(0, 3), empLeaveEndDate.DayOfWeek.ToString().Substring(0, 3), leave.StartDate.TimeOfDay.Hours.ToString("00") + ":" + leave.StartDate.TimeOfDay.Minutes.ToString("00") + "-" + leave.EndDate.TimeOfDay.Hours.ToString("00") + ":" + leave.EndDate.TimeOfDay.Minutes.ToString("00"), null));
                                            }
                                        }
                                        if (leave.Status == "DisApproved")
                                        {
                                            if (leave.PartFullTime == "F")
                                            {
                                                infoList.Add(AddToList(leave.EmployeeId, empName, profilePhoto, empLeaveStartDate, empLeaveEndDate, "DisApproved Leave", "#E9573E", empLeaveStartDate.DayOfWeek.ToString().Substring(0, 3), empLeaveEndDate.DayOfWeek.ToString().Substring(0, 3), "0", null));
                                            }
                                            if (leave.PartFullTime == "P")
                                            {
                                                infoList.Add(AddToList(leave.EmployeeId, empName, profilePhoto, empLeaveStartDate, empLeaveEndDate, "DisApproved Leave", "#E9573E", empLeaveStartDate.DayOfWeek.ToString().Substring(0, 3), empLeaveEndDate.DayOfWeek.ToString().Substring(0, 3), leave.StartDate.TimeOfDay.Hours.ToString("00") + ":" + leave.StartDate.TimeOfDay.Minutes.ToString("00") + "-" + leave.EndDate.TimeOfDay.Hours.ToString("00") + ":" + leave.EndDate.TimeOfDay.Minutes.ToString("00"), null));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }




                    //for (DateTime dt = DateTime.Now; dt <= DateTime.Now.AddDays(15); dt = dt.AddDays(1))
                    //{
                    //    //iterate down level users
                    //    foreach (var l in cList)
                    //    {
                    //        var leave = db.tblApplyLeaves.AsEnumerable().Where(z => z.StartDate.Date == dt.Date && (z.Status == "Approved" || z.Status == "Pending") && z.EmployeeId == l.EmployeeId).FirstOrDefault();
                    //        if (leave != null)
                    //        {
                    //            DateTime empLeaveEndDate = EndDate(leave.StartDate, leave.EmployeeId);
                    //            DateTime empLeaveStartDate = StartDate(leave.StartDate, leave.EmployeeId);
                    //            string empName = db.tblEmpPersonalInformations.Where(z => z.EmployeeId == leave.EmployeeId).Select(z => z.CandidateFirstName + (z.CandidateMiddleName.Length > 1 ? z.CandidateMiddleName.Substring(0, 1) : z.CandidateMiddleName) + (z.CandidateLastName.Length > 1 ? z.CandidateLastName.Substring(0, 1) : z.CandidateLastName)).FirstOrDefault();
                    //            string profilePhoto = db.tblEmpPersonalInformations.Where(z => z.EmployeeId == l.EmployeeId).Select(z => z.ProfilePhoto).FirstOrDefault();

                    //            int recored_exist = infoList.Where(z => z.OnDate == empLeaveStartDate && z.EndDate == empLeaveEndDate && z.EmployeeId == leave.EmployeeId).Count();
                    //            if (recored_exist <= 0)
                    //            {
                    //                if (leave.Status == "Approved")
                    //                {
                    //                    if (leave.PartFullTime == "F")
                    //                    {
                    //                        infoList.Add(AddToList(leave.EmployeeId, empName, profilePhoto, empLeaveStartDate, empLeaveEndDate, "Approved Leave", "#8DC153", empLeaveStartDate.DayOfWeek.ToString().Substring(0, 3), empLeaveEndDate.DayOfWeek.ToString().Substring(0, 3), "0",null));

                    //                    }
                    //                    if (leave.PartFullTime == "P")
                    //                    {
                    //                        infoList.Add(AddToList(leave.EmployeeId, empName, profilePhoto, empLeaveStartDate, empLeaveEndDate, "Approved Leave", "#8DC153", empLeaveStartDate.DayOfWeek.ToString().Substring(0, 3), empLeaveEndDate.DayOfWeek.ToString().Substring(0, 3), leave.StartDate.TimeOfDay.Hours.ToString("00") + ":" + leave.StartDate.TimeOfDay.Minutes.ToString("00") + "-" + leave.EndDate.TimeOfDay.Hours.ToString("00") + ":" + leave.EndDate.TimeOfDay.Minutes.ToString("00"),null));
                    //                    }
                    //                }
                    //                if (leave.Status == "Pending")
                    //                {
                    //                    if (leave.PartFullTime == "F")
                    //                    {
                    //                        infoList.Add(AddToList(leave.EmployeeId, empName, profilePhoto, empLeaveStartDate, empLeaveEndDate, "Pending Leave", "#f6bb4a", empLeaveStartDate.DayOfWeek.ToString().Substring(0, 3), empLeaveEndDate.DayOfWeek.ToString().Substring(0, 3), "0", null));
                    //                    }
                    //                    if (leave.PartFullTime == "P")
                    //                    {
                    //                        infoList.Add(AddToList(leave.EmployeeId, empName, profilePhoto, empLeaveStartDate, empLeaveEndDate, "Pending Leave", "#f6bb4a", empLeaveStartDate.DayOfWeek.ToString().Substring(0, 3), empLeaveEndDate.DayOfWeek.ToString().Substring(0, 3), leave.StartDate.TimeOfDay.Hours.ToString("00") + ":" + leave.StartDate.TimeOfDay.Minutes.ToString("00") + "-" + leave.EndDate.TimeOfDay.Hours.ToString("00") + ":" + leave.EndDate.TimeOfDay.Minutes.ToString("00"),null));
                    //                    }
                    //                }
                    //            }

                    //        }
                    //    }

                    //    // //check for anniversary
                    //    //var anniversary = db.tblEmpPersonalInformations.AsEnumerable().Where(z => Convert.ToDateTime(z.MarriageAnniversaryDate).Month == dt.Month && Convert.ToDateTime(z.MarriageAnniversaryDate).Day == dt.Day && z.IsActive == true).ToList();
                    //    //foreach (var l in anniversary)
                    //    //{
                    //    //    DateTime dtA = new DateTime(dt.Year, Convert.ToDateTime(l.MarriageAnniversaryDate).Month, Convert.ToDateTime(l.MarriageAnniversaryDate).Day);
                    //    //    string empName = db.tblEmpPersonalInformations.Where(z => z.EmployeeId == l.EmployeeId).Select(z => z.CandidateFirstName + (z.CandidateMiddleName.Length > 1 ? z.CandidateMiddleName.Substring(0, 1) : z.CandidateMiddleName) + (z.CandidateLastName.Length > 1 ? z.CandidateLastName.Substring(0, 1) : z.CandidateLastName)).FirstOrDefault();
                    //    //    string profilePhoto = db.tblEmpPersonalInformations.Where(z => z.EmployeeId == l.EmployeeId).Select(z => z.ProfilePhoto).FirstOrDefault();

                    //    //    var payroll = db.tblEmpPayRollInformations.Where(z => z.EmployeeId == l.EmployeeId).FirstOrDefault();
                    //    //    if (payroll != null && payroll.ReLeavingDate != null)
                    //    //    {
                    //    //        if (payroll.ReLeavingDate >= DateTime.Now.Date)
                    //    //        {
                    //    //            infoList.Add(AddToList(l.EmployeeId, empName, profilePhoto, dtA, null, "Anniversary", "#F6BB43"));
                    //    //        }
                    //    //    }
                    //    //    else
                    //    //    {
                    //    //        infoList.Add(AddToList(l.EmployeeId, empName, profilePhoto, dtA, null, "Anniversary", "#F6BB43"));
                    //    //    }
                    //    //}

                    //    ////check for birthday
                    //    //var birthday = db.tblEmpPersonalInformations.AsEnumerable().Where(z => Convert.ToDateTime(z.BirthDate).Month == dt.Month && Convert.ToDateTime(z.BirthDate).Day == dt.Day && z.IsActive == true).ToList();
                    //    //foreach (var l in birthday)
                    //    //{
                    //    //    DateTime dtB = new DateTime(dt.Year, Convert.ToDateTime(l.BirthDate).Month, Convert.ToDateTime(l.BirthDate).Day);
                    //    //    string empName = db.tblEmpPersonalInformations.Where(z => z.EmployeeId == l.EmployeeId).Select(z => z.CandidateFirstName + (z.CandidateMiddleName.Length > 1 ? z.CandidateMiddleName.Substring(0, 1) : z.CandidateMiddleName) + (z.CandidateLastName.Length > 1 ? z.CandidateLastName.Substring(0, 1) : z.CandidateLastName)).FirstOrDefault();
                    //    //    string profilePhoto = db.tblEmpPersonalInformations.Where(z => z.EmployeeId == l.EmployeeId).Select(z => z.ProfilePhoto).FirstOrDefault();

                    //    //    var payroll = db.tblEmpPayRollInformations.Where(z => z.EmployeeId == l.EmployeeId).FirstOrDefault();
                    //    //    if (payroll != null && payroll.ReLeavingDate != null)
                    //    //    {
                    //    //        if (payroll.ReLeavingDate >= DateTime.Now.Date)
                    //    //        {
                    //    //            infoList.Add(AddToList(l.EmployeeId, empName, profilePhoto, dtB, null, "Birthday", "#4B89DC"));
                    //    //        }
                    //    //    }
                    //    //    else
                    //    //    {
                    //    //        infoList.Add(AddToList(l.EmployeeId, empName, profilePhoto, dtB, null, "Birthday", "#4B89DC"));
                    //    //    }
                    //    //}
                    //}
                    spInfoList = db.Database.SqlQuery<DashboardInfoListViewModel>("usp_getRemainderInformation").ToList();

                    foreach (var list in spInfoList)
                    {
                        infoList.Add(AddToList(list.EmployeeId, list.EmployeeName, list.ProfilePhoto, list.OnDate, list.EndDate, list.Type, list.ColorCode, list.OnDateWeekName.Substring(0, 3), null, null, list.FestivalGroupId));
                    };

                   // apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", infoList.OrderBy(z => z.Type));
                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", infoList.OrderBy(z => z.OnDate).ThenBy(z=>z.EmployeeName));
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


        [HttpGet]
        public ApiResponse GetEmpAttendanceReport()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                     NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                     int year = Convert.ToInt32(nvc["tempYear"].ToString());

                    int empId = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                    if (empId != 1)
                    {
                        List<EmpAttendanceRemainderViewModel> spList = new List<EmpAttendanceRemainderViewModel>();
                       // SqlParameter employeeId = new SqlParameter();
                       var employeeId = new SqlParameter("@employeeId", empId);
                       var tempYear = new SqlParameter("@Year", year);

                       spList = db.Database.SqlQuery<EmpAttendanceRemainderViewModel>("usp_RemainderAttendanceReport  @employeeId,@Year ", employeeId, tempYear).ToList();
                        if (spList.Count > 0)
                        {
                            string pendingLeave = new EmployeeUtils().GetEmployeePendingLeave(empId, year);
                            spList[0].pendingLeave = pendingLeave;
                            apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", spList);
                        }
                        else
                        {
                            apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", null);
                        }
                    }
                    else
                    {
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", null);
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
