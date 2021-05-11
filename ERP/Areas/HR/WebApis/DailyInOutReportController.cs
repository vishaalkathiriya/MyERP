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
using System.Data.Entity;
using System.Globalization;
using System.Data.Entity.Core.Objects;

namespace ERP.WebApis
{
    public class DailyInOutReportController : ApiController
    {

        private ERPContext db = new ERPContext();
        SessionUtils sessionUtils = new SessionUtils();
        GeneralMessages generalMessages = null;
        string _pageName = "Employee In-Out Detail";

        public DailyInOutReportController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }

        /// <summary>
        /// GET api/DailyInOutReport
        /// return list with In-Out Time Details of Employees
        /// </summary>
        [HttpGet]
        public ApiResponse GetDailyInOutReprotData()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    string date = nvc["date"].ToString(); //dd-mm-yyyy
                    String groupName = nvc["groupName"].ToString() == "null" ? null : nvc["groupName"].ToString();

                    DateTime dtFilter = new DateTime();
                    string[] part = date.Split('-');
                    dtFilter = new DateTime(Convert.ToInt32(part[2]), Convert.ToInt32(part[1]), Convert.ToInt32(part[0]), 0, 0, 0);

                    //Check Whether Day is holiday or Festival Day
                    bool isHoliday = false;
                    var lstFestivals = db.tblFestivals.Where(z => EntityFunctions.TruncateTime(z.FestivalDate) == dtFilter.Date && z.tblFestivalType.IsWorkingDay == false).ToList();
                    if (lstFestivals.Count > 0)
                    {
                        isHoliday = true;
                    }

                    List<DailyInOutReportViewModel> lstDailyInOutReportViewModel = new List<DailyInOutReportViewModel>();

                    List<tblEmpPersonalInformation> list = new List<tblEmpPersonalInformation>();
                    if (String.IsNullOrEmpty(groupName))
                    {
                        list = (from m in db.tblEmpPersonalInformations
                                join p in db.tblEmpPayRollInformations
                                on m.EmployeeId equals p.EmployeeId
                                where m.EmployeeId != 1 && (p.ReLeavingDate == null || p.ReLeavingDate >= dtFilter.Date) && p.JoiningDate != null && p.JoiningDate <= dtFilter.Date
                                orderby m.CandidateFirstName
                                select m).ToList<tblEmpPersonalInformation>();
                    }
                    else
                    {
                        list = (from m in db.tblEmpPersonalInformations
                                join p in db.tblEmpPayRollInformations
                                on m.EmployeeId equals p.EmployeeId
                                where p.GName == groupName && m.EmployeeId != 1 && (p.ReLeavingDate == null || p.ReLeavingDate >= dtFilter.Date) && p.JoiningDate != null && p.JoiningDate <= dtFilter.Date
                                orderby m.CandidateFirstName
                                select m).ToList<tblEmpPersonalInformation>();
                    }

                    foreach (tblEmpPersonalInformation l in list)
                    {

                        // Get All Data of In-Out Table
                        List<tblEmpDailyInOut> lstInOut = db.tblEmpDailyInOuts.AsEnumerable().Where(z => z.Intime.Date == dtFilter.Date && z.EmployeeId == l.EmployeeId).ToList();

                        //Employee Information that is to be displayed on header eg.EmpName and Date
                        DailyInOutReportViewModel objDailyInOutReportViewModel = new DailyInOutReportViewModel
                        {
                            EmployeeId = l.EmployeeId,
                            Ecode = l.EmployeeRegisterCode,
                            EmpName = l.CandidateFirstName + (l.CandidateMiddleName.Length > 1 ? l.CandidateMiddleName.Substring(0, 1) : l.CandidateMiddleName) + (l.CandidateLastName.Length > 1 ? l.CandidateLastName.Substring(0, 1) : l.CandidateLastName),
                            Edate = dtFilter,
                            TotalPendingLeave = new EmployeeUtils().GetEmployeePendingLeave(l.EmployeeId, dtFilter.Year),
                            AppliedLeaveStatus = db.tblApplyLeaves.AsEnumerable().Where(z => z.EmployeeId == l.EmployeeId && z.StartDate.Date == dtFilter.Date && z.Status == "Approved").Select(z => z.PartFullTime).FirstOrDefault(),
                            SalaryBasedOn = db.tblEmpPayRollInformations.Where(z => z.EmployeeId == l.EmployeeId).Select(z => z.SalaryBasedOn).FirstOrDefault()
                        };

                        //Process In-Out of employee
                        CompanyInOutDetail objInOutDetail = new CompanyInOutDetail();

                        // Timespan variable to contain difference of hours in order-to calculate Total Hours
                        TimeSpan? tCompanyHrs = new TimeSpan();
                        TimeSpan? tLunchBreakHrs = new TimeSpan();
                        TimeSpan? tPersonalWorkHrs = new TimeSpan();
                        TimeSpan? tCompanyWorkHrs = new TimeSpan();

                        if (lstInOut.Count() > 0)
                        {
                            //Employee Company In-Out-Time Calculation
                            if (lstInOut.Where(z => z.InType == 1).Count() > 0)
                            {
                                objInOutDetail.InCompanyTime = lstInOut.Where(z => z.InType == 1).FirstOrDefault().Intime;
                            }
                            
                            if (lstInOut.Where(z => z.OutType == 2).Count() > 0)
                            {
                                objInOutDetail.OutCompanyTime = lstInOut.Where(z => z.OutType == 2).FirstOrDefault().OutTime;
                                tCompanyHrs = objInOutDetail.OutCompanyTime - objInOutDetail.InCompanyTime;
                                objInOutDetail.companyHrs = tCompanyHrs.Value.Hours + ":" + tCompanyHrs.Value.Minutes;
                            }

                            // Lunch Break-Time Calculate (When 3, Emp out for Lunch Break)
                            if (lstInOut.Where(z => z.OutType == 3).Count() > 0)
                            {
                                objInOutDetail.LunchBreakStart = lstInOut.Where(z => z.OutType == 3).FirstOrDefault().OutTime;
                            }

                            // (When 4 Emp come from Lunch Break, so calculate lunch-time here)
                            if (lstInOut.Where(z => z.InType == 4).Count() > 0)
                            {
                                objInOutDetail.LunchBreakEnd = lstInOut.Where(z => z.InType == 4).FirstOrDefault().Intime;
                                tLunchBreakHrs = objInOutDetail.LunchBreakEnd - objInOutDetail.LunchBreakStart;
                                objInOutDetail.lunchBreakHrs = tLunchBreakHrs.Value.Hours + ":" + tLunchBreakHrs.Value.Minutes;
                            }

                            // Company Work-Time Calculate (When 5 start Company Work)
                            if (lstInOut.Where(z => z.OutType == 5).Count() > 0)
                            {
                                objInOutDetail.CompanyWorkStart = lstInOut.Where(z => z.OutType == 5).FirstOrDefault().OutTime;
                            }

                            // (When 6,End company work, so calculate company-work time here)
                            if (lstInOut.Where(z => z.InType == 6).Count() > 0)
                            {
                                objInOutDetail.CompanyWorkEnd = lstInOut.Where(z => z.InType == 6).FirstOrDefault().Intime;
                                tCompanyWorkHrs = objInOutDetail.CompanyWorkEnd - objInOutDetail.CompanyWorkStart;
                                objInOutDetail.CompanyWorkHrs = tCompanyWorkHrs.Value.Hours + ":" + tCompanyWorkHrs.Value.Minutes;
                            }

                            // Personal Work-Time Calculate (When 7 start Personal Work)
                            if (lstInOut.Where(z => z.OutType == 7).Count() > 0)
                            {
                                objInOutDetail.PersonalWorkStart = lstInOut.Where(z => z.OutType == 7).FirstOrDefault().OutTime;
                            }

                            // (When 8 end personal work, so calculate personal-work time here)
                            if (lstInOut.Where(z => z.InType == 8).Count() > 0)
                            {
                                objInOutDetail.PersonalWorkEnd = lstInOut.Where(z => z.InType == 8).FirstOrDefault().Intime;
                                tPersonalWorkHrs = objInOutDetail.PersonalWorkEnd - objInOutDetail.PersonalWorkStart;
                                objInOutDetail.personalWorkHrs = tPersonalWorkHrs.Value.Hours + ":" + tPersonalWorkHrs.Value.Minutes;
                            }

                            //Finaly Calculate Total Work (if Employee is "Out", Means=2)
                            if (lstInOut.Where(z => z.OutType == 2).Count() > 0)
                            {
                                // Substract personal work hours from company hours( not company work hours)
                                objDailyInOutReportViewModel.TotalHours = ((TimeSpan)tCompanyHrs - (TimeSpan)tPersonalWorkHrs).Hours + ":" + ((TimeSpan)tCompanyHrs - (TimeSpan)tPersonalWorkHrs).Minutes;
                            }
                        }
                        else
                        {
                            objInOutDetail.InCompanyTime = null;
                            objInOutDetail.companyHrs = "0";
                            objInOutDetail.CompanyWorkHrs = "0";
                            objInOutDetail.personalWorkHrs = "0";
                            objInOutDetail.lunchBreakHrs = "0";
                            objDailyInOutReportViewModel.TotalHours = "0";
                        }

                        objDailyInOutReportViewModel.InOutDetail = objInOutDetail;

                        // Process Attendance data
                        var line = db.tblEmpAttendances.AsEnumerable().Where(z => z.EmployeeId == l.EmployeeId && z.PDate.Date == dtFilter.Date).FirstOrDefault();

                        if (line != null)
                        {
                            tblEmpAttendanceViewModel emp = new tblEmpAttendanceViewModel
                            {
                                Id = line.Id,
                                EmployeeId = line.EmployeeId,
                                PDate = line.PDate,
                                Presence = line.Presence,
                                Absence = line.Absence,
                                Leave = line.Leave,
                                OT = line.OT,
                                IsHoliday = line.IsHoliday,
                                WorkingHours = Convert.ToString(line.WorkingHours),
                                // WorkingHours = !string.IsNullOrEmpty(emp.WorkingHours) ? Convert.ToDecimal(emp.WorkingHours.Replace(":", ".")) : 0,
                                Remark = line.Remark,
                            };
                            objDailyInOutReportViewModel.Attendance = emp;
                        }

                        objDailyInOutReportViewModel.isHolidayDay = isHoliday;

                        //Add main object to list
                        lstDailyInOutReportViewModel.Add(objDailyInOutReportViewModel);

                    }

                    //order by employee name 
                    //lstDailyInOutReportViewModel = lstDailyInOutReportViewModel != null ? lstDailyInOutReportViewModel.OrderBy(z => z.EmpName).ToList() : lstDailyInOutReportViewModel;

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", lstDailyInOutReportViewModel);
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
        /// POST api/DailyInOutReport
        /// create and update attendance of employee
        /// </summary>
        [HttpPost]
        public ApiResponse SaveData(tblEmpAttendanceViewModel emp)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    DateTime dt = new DateTime();
                    string[] parts = emp.PDateInString.Split(' ')[0].Split('-');
                    dt = new DateTime(Convert.ToInt32(parts[2]), Convert.ToInt32(parts[1]), Convert.ToInt32(parts[0]), 0, 0, 0);
                    string mode = "Add";

                    //Check If Emp-Attendance Entry Exist For Date Then Mode set to "Edit"
                    if (emp.Id != 0 || db.tblEmpAttendances.Where(z => z.EmployeeId == emp.EmployeeId && EntityFunctions.TruncateTime(z.PDate) == dt.Date).Count() != 0)
                    {
                        mode = "Edit";
                    }

                    if (mode.Equals("Add"))
                    {// Mode == Add
                        tblEmpAttendance d = new tblEmpAttendance
                        {
                            EmployeeId = emp.EmployeeId,
                            PDate = dt,
                            Presence = emp.Presence,
                            Absence = emp.Absence,
                            Leave = emp.Leave,
                            OT = emp.OT,
                            WorkingHours = !string.IsNullOrEmpty(emp.WorkingHours) ? Convert.ToDecimal(emp.WorkingHours.Replace(":", ".")) : 0,
                            PersonalWorkHours = !string.IsNullOrEmpty(emp.PersonalWorkHours) ? Convert.ToDecimal(emp.PersonalWorkHours.Replace(":", ".")) : 0,
                            CompanyWorkHours = !string.IsNullOrEmpty(emp.CompanyWorkHours) ? Convert.ToDecimal(emp.CompanyWorkHours.Replace(":", ".")) : 0,
                            LunchBreakHours = !string.IsNullOrEmpty(emp.LunchBreakHours) ? Convert.ToDecimal(emp.LunchBreakHours.Replace(":", ".")) : 0,
                            IsHoliday = emp.IsHoliday,
                            Remark = emp.Remark,
                            CreDate = DateTime.Now.ToUniversalTime(),
                            CreBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString()),
                            ChgDate = DateTime.Now.ToUniversalTime(),
                            ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString())
                        };
                        db.tblEmpAttendances.Add(d);
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);
                    }
                    else
                    {// Mode == Edit
                        var line = db.tblEmpAttendances.Where(z => z.Id == emp.Id).SingleOrDefault();
                        if (line != null)
                        {
                            line.EmployeeId = emp.EmployeeId;
                            line.PDate = dt;
                            line.Presence = emp.Presence;
                            line.Absence = emp.Absence;
                            line.Leave = emp.Leave;
                            line.OT = emp.OT;
                            line.WorkingHours = !string.IsNullOrEmpty(emp.WorkingHours) ? Convert.ToDecimal(emp.WorkingHours.Replace(":", ".")) : 0;
                            line.PersonalWorkHours = !string.IsNullOrEmpty(emp.PersonalWorkHours) ? Convert.ToDecimal(emp.PersonalWorkHours.Replace(":", ".")) : 0;
                            line.CompanyWorkHours = !string.IsNullOrEmpty(emp.CompanyWorkHours) ? Convert.ToDecimal(emp.CompanyWorkHours.Replace(":", ".")) : 0;
                            line.LunchBreakHours = !string.IsNullOrEmpty(emp.LunchBreakHours) ? Convert.ToDecimal(emp.LunchBreakHours.Replace(":", ".")) : 0;
                            line.IsHoliday = emp.IsHoliday;
                            line.Remark = emp.Remark;
                            line.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                            line.ChgDate = DateTime.Now.ToUniversalTime();
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
        /// GET api/DailyInOutReport
        /// return list with In-Out Time Details of Employees for selected Date
        /// </summary>
        [HttpGet]
        public ApiResponse GetInOutDetailsOfEmployee()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    string date = nvc["date"].ToString(); //dd-mm-yyyy
                    int EmpId = Convert.ToInt32(nvc["EmpId"].ToString());

                    DateTime dtFilter = new DateTime();
                    string[] part = date.Split('-');
                    dtFilter = new DateTime(Convert.ToInt32(part[2]), Convert.ToInt32(part[1]), Convert.ToInt32(part[0]), 0, 0, 0);

                    var lstInOut = db.tblEmpDailyInOuts.Where(z => z.EmployeeId == EmpId && EntityFunctions.TruncateTime(z.Intime) == dtFilter.Date).ToList();

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", lstInOut);
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
        /// POST api/DailyInOutReport
        /// update In-Out detail entry of employee
        /// </summary>
        [HttpPost]
        public ApiResponse UpdateInOutDetailsOfEmployee(tblEmpDailyInOut obj)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var line = db.tblEmpDailyInOuts.Where(z => z.SrNo == obj.SrNo && z.EmployeeId == obj.EmployeeId).FirstOrDefault();
                    if (line != null)
                    {
                        if (obj.Intime.Date != Convert.ToDateTime("1/1/0001"))
                        {
                            line.Intime = obj.Intime;
                            line.InType = obj.InType;
                        }
                        line.OutTime = obj.OutTime;
                        line.OutType = obj.OutTime == null ? 0 : obj.OutType;
                        db.SaveChanges();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgUpdate, null);
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
        /// POST api/DailyInOutReport
        /// Delete In-Out detail entry of employee
        /// </summary>
        [HttpPost]
        public ApiResponse DeleteEmployeeInOutDelete(tblEmpDailyInOut obj)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var line = db.tblEmpDailyInOuts.Where(z => z.SrNo == obj.SrNo && z.EmployeeId == obj.EmployeeId && z.Ecode == obj.Ecode).FirstOrDefault();
                    if (line != null)
                    {
                        db.tblEmpDailyInOuts.Remove(line);
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

    }
}
