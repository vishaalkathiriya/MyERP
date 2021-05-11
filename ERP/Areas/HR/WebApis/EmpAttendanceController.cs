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
using System.Data.SqlClient;
using System.Globalization;
using System.Data.Entity.SqlServer;
using System.Data.Entity.Core.Objects;


namespace ERP.WebApis
{
    public class EmpAttendanceController : ApiController
    {
        private ERPContext db = new ERPContext();
        SessionUtils sessionUtils = new SessionUtils();
        GeneralMessages generalMessages = null;
        string _pageName = "Employee Attendance Report";

        public EmpAttendanceController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }

        //Convert Value To String And Format it
        public string GetValue(string value)
        {
            return Convert.ToDouble(value) == 0 ? "" : value.TrimEnd('0').TrimEnd('.');
        }

        //Add Monthly P,A,L,O,H to Total P,A,L,O,H
        public void addToTotal(ref EmpAttendanceReportDetailViewModel obj, string[] arr)
        {
            obj.Total_P += Convert.ToDouble(arr[0]);
            obj.Total_A += Convert.ToDouble(arr[1]);
            obj.Total_L += Convert.ToDouble(arr[2]);
            obj.Total_O += Convert.ToDouble(arr[3]);
            obj.Total_H += Convert.ToDouble(arr[4]);
            obj.Total_WORKHR += Convert.ToDouble(arr[5]);
        }

        // api/EmpAttendance
        // Get Employee Attendance Report Data
        [HttpGet]
        public ApiResponse GetEmpAttendanceReport()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int filterYear = Convert.ToInt32(nvc["filter1"]);
                    string groupName = nvc["groupName"].ToString() == "null" ? null : nvc["groupName"].ToString();

                    List<EmpAttendanceReportViewModel> spList = new List<EmpAttendanceReportViewModel>();
                    SqlParameter yearParam = new SqlParameter();
                    yearParam = new SqlParameter("@Year", filterYear);
                    spList = db.Database.SqlQuery<EmpAttendanceReportViewModel>("usp_getEmployeeAttendanceReport  @Year ", yearParam).ToList();

                    List<tblEmpPayRollInformation> lstPayroll = new List<tblEmpPayRollInformation>();
                    if (String.IsNullOrEmpty(groupName))
                    {
                        lstPayroll = db.tblEmpPayRollInformations.Where(z => z.JoiningDate.Year <= filterYear && (z.ReLeavingDate == null || z.ReLeavingDate.Value.Year >= filterYear)).ToList();
                    }
                    else
                    {
                        lstPayroll = db.tblEmpPayRollInformations.Where(z => z.GName == groupName && z.JoiningDate.Year <= filterYear && (z.ReLeavingDate == null || z.ReLeavingDate.Value.Year >= filterYear)).ToList();
                    }

                    List<EmpAttendanceReportDetailViewModel> lstReportData = new List<EmpAttendanceReportDetailViewModel>();
                    foreach (var item in spList)
                    {
                        tblEmpPayRollInformation objPayroll = (from o in lstPayroll where o.EmployeeId == item.EmployeeId select o).FirstOrDefault();

                        if (objPayroll != null)
                        {
                            EmpAttendanceReportDetailViewModel obj = new EmpAttendanceReportDetailViewModel();

                            #region "Personal Information"
                            obj.EmployeeId = item.EmployeeId;
                            obj.EmpName = (from l in db.tblEmpPersonalInformations.AsEnumerable() where l.EmployeeId == item.EmployeeId select new { EmpName = l.CandidateFirstName + (l.CandidateMiddleName.Length > 1 ? l.CandidateMiddleName.Substring(0, 1) : l.CandidateMiddleName) + (l.CandidateLastName.Length > 1 ? l.CandidateLastName.Substring(0, 1) : l.CandidateLastName) }).FirstOrDefault().EmpName;
                            obj.Total_P = obj.Total_A = obj.Total_L = obj.Total_O = obj.Total_H = 0;
                            #endregion

                            #region "Set Join-End Date"
                            obj.JOIN_MONTH = objPayroll.JoiningDate.Month;
                            obj.JOIN_YEAR = objPayroll.JoiningDate.Year;
                            if (objPayroll.ReLeavingDate != null)
                            {
                                obj.REL_MONTH = objPayroll.ReLeavingDate.Value.Month;
                                obj.REL_YEAR = objPayroll.ReLeavingDate.Value.Year;
                            }
                            #endregion

                            #region "Attendance Information"

                            //January
                            if (!string.IsNullOrEmpty(item.JAN))
                            {
                                string[] JAN = item.JAN.Split('|');
                                obj.JAN_P = GetValue(JAN[0]);
                                obj.JAN_A = GetValue(JAN[1]);
                                obj.JAN_L = GetValue(JAN[2]);
                                obj.JAN_O = GetValue(JAN[3]);
                                obj.JAN_H = GetValue(JAN[4]);
                                obj.JAN_WORKHR = GetValue(JAN[5]);

                                addToTotal(ref obj, JAN);
                            }
                            //February
                            if (!string.IsNullOrEmpty(item.FEB))
                            {
                                string[] FEB = item.FEB.Split('|');
                                obj.FEB_P = GetValue(FEB[0]);
                                obj.FEB_A = GetValue(FEB[1]);
                                obj.FEB_L = GetValue(FEB[2]);
                                obj.FEB_O = GetValue(FEB[3]);
                                obj.FEB_H = GetValue(FEB[4]);
                                obj.FEB_WORKHR = GetValue(FEB[5]);

                                addToTotal(ref obj, FEB);
                            }
                            //March
                            if (!string.IsNullOrEmpty(item.MAR))
                            {
                                string[] MAR = item.MAR.Split('|');
                                obj.MAR_P = GetValue(MAR[0]);
                                obj.MAR_A = GetValue(MAR[1]);
                                obj.MAR_L = GetValue(MAR[2]);
                                obj.MAR_O = GetValue(MAR[3]);
                                obj.MAR_H = GetValue(MAR[4]);
                                obj.MAR_WORKHR = GetValue(MAR[5]);

                                addToTotal(ref obj, MAR);
                            }
                            //April
                            if (!string.IsNullOrEmpty(item.APR))
                            {
                                string[] APR = item.APR.Split('|');
                                obj.APR_P = GetValue(APR[0]);
                                obj.APR_A = GetValue(APR[1]);
                                obj.APR_L = GetValue(APR[2]);
                                obj.APR_O = GetValue(APR[3]);
                                obj.APR_H = GetValue(APR[4]);
                                obj.APR_WORKHR = GetValue(APR[5]);

                                addToTotal(ref obj, APR);
                            }
                            //May
                            if (!string.IsNullOrEmpty(item.MAY))
                            {
                                string[] MAY = item.MAY.Split('|');
                                obj.MAY_P = GetValue(MAY[0]);
                                obj.MAY_A = GetValue(MAY[1]);
                                obj.MAY_L = GetValue(MAY[2]);
                                obj.MAY_O = GetValue(MAY[3]);
                                obj.MAY_H = GetValue(MAY[4]);
                                obj.MAY_WORKHR = GetValue(MAY[5]);

                                addToTotal(ref obj, MAY);
                            }
                            //June
                            if (!string.IsNullOrEmpty(item.JUN))
                            {
                                string[] JUN = item.JUN.Split('|');
                                obj.JUN_P = GetValue(JUN[0]);
                                obj.JUN_A = GetValue(JUN[1]);
                                obj.JUN_L = GetValue(JUN[2]);
                                obj.JUN_O = GetValue(JUN[3]);
                                obj.JUN_H = GetValue(JUN[4]);
                                obj.JUN_WORKHR = GetValue(JUN[5]);

                                addToTotal(ref obj, JUN);
                            }
                            //July
                            if (!string.IsNullOrEmpty(item.JUL))
                            {
                                string[] JUL = item.JUL.Split('|');
                                obj.JUL_P = GetValue(JUL[0]);
                                obj.JUL_A = GetValue(JUL[1]);
                                obj.JUL_L = GetValue(JUL[2]);
                                obj.JUL_O = GetValue(JUL[3]);
                                obj.JUL_H = GetValue(JUL[4]);
                                obj.JUL_WORKHR = GetValue(JUL[5]);

                                addToTotal(ref obj, JUL);
                            }
                            //August
                            if (!string.IsNullOrEmpty(item.AUG))
                            {
                                string[] AUG = item.AUG.Split('|');
                                obj.AUG_P = GetValue(AUG[0]);
                                obj.AUG_A = GetValue(AUG[1]);
                                obj.AUG_L = GetValue(AUG[2]);
                                obj.AUG_O = GetValue(AUG[3]);
                                obj.AUG_H = GetValue(AUG[4]);
                                obj.AUG_WORKHR = GetValue(AUG[5]);

                                addToTotal(ref obj, AUG);
                            }
                            //September
                            if (!string.IsNullOrEmpty(item.SEP))
                            {
                                string[] SEP = item.SEP.Split('|');
                                obj.SEP_P = GetValue(SEP[0]);
                                obj.SEP_A = GetValue(SEP[1]);
                                obj.SEP_L = GetValue(SEP[2]);
                                obj.SEP_O = GetValue(SEP[3]);
                                obj.SEP_H = GetValue(SEP[4]);
                                obj.SEP_WORKHR = GetValue(SEP[5]);

                                addToTotal(ref obj, SEP);
                            }
                            //October
                            if (!string.IsNullOrEmpty(item.OCT))
                            {
                                string[] OCT = item.OCT.Split('|');
                                obj.OCT_P = GetValue(OCT[0]);
                                obj.OCT_A = GetValue(OCT[1]);
                                obj.OCT_L = GetValue(OCT[2]);
                                obj.OCT_O = GetValue(OCT[3]);
                                obj.OCT_H = GetValue(OCT[4]);
                                obj.OCT_WORKHR = GetValue(OCT[5]);

                                addToTotal(ref obj, OCT);
                            }
                            //November
                            if (!string.IsNullOrEmpty(item.NOV))
                            {
                                string[] NOV = item.NOV.Split('|');
                                obj.NOV_P = GetValue(NOV[0]);
                                obj.NOV_A = GetValue(NOV[1]);
                                obj.NOV_L = GetValue(NOV[2]);
                                obj.NOV_O = GetValue(NOV[3]);
                                obj.NOV_H = GetValue(NOV[4]);
                                obj.NOV_WORKHR = GetValue(NOV[5]);

                                addToTotal(ref obj, NOV);
                            }
                            //December
                            if (!string.IsNullOrEmpty(item.DEC))
                            {
                                string[] DEC = item.DEC.Split('|');
                                obj.DEC_P = GetValue(DEC[0]);
                                obj.DEC_A = GetValue(DEC[1]);
                                obj.DEC_L = GetValue(DEC[2]);
                                obj.DEC_O = GetValue(DEC[3]);
                                obj.DEC_H = GetValue(DEC[4]);
                                obj.DEC_WORKHR = GetValue(DEC[5]);

                                addToTotal(ref obj, DEC);
                            }

                            //Pending Leave Calculation
                            if (lstPayroll.Where(z => z.EmployeeId == item.EmployeeId && z.PermanentFromDate != null).Count() > 0)
                            {
                                obj.PendingLeave = new EmployeeUtils().GetEmployeePendingLeave(item.EmployeeId, Convert.ToInt32(filterYear));
                            }
                            //End Pending Leave Calculation

                            lstReportData.Add(obj);

                            #endregion
                        }
                    }

                    //Orderby EmpName
                    lstReportData = lstReportData.OrderBy(z => z.EmpName).ToList();

                    //Return List of main object
                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", lstReportData);
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

        // api/EmpAttendance
        // Get Employee Attendance Report Data In Month Format
        [HttpGet]
        public ApiResponse GetEmpAttendanceReportMonthFormat()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int filterMonth = Convert.ToInt32(nvc["filter2"]);
                    int filterYear = Convert.ToInt32(nvc["filter1"]);
                    string groupName = nvc["groupName"].ToString() == "null" ? null : nvc["groupName"].ToString();

                    //Get First and Last Date Of Month
                    DateTime StartDt = new DateTime(filterYear, filterMonth, 1, 0, 0, 0);
                    DateTime EndDt = new DateTime(filterYear, filterMonth, DateTime.DaysInMonth(filterYear, filterMonth), 0, 0, 0);

                    //Get Payroll Information
                    List<tblEmpPayRollInformation> lstEmpPayrollData = new List<tblEmpPayRollInformation>();
                    if (String.IsNullOrEmpty(groupName))
                    {
                        lstEmpPayrollData = db.tblEmpPayRollInformations.Where(z => z.JoiningDate != null && z.JoiningDate <= EndDt.Date && (z.ReLeavingDate == null || z.ReLeavingDate >= StartDt)).ToList();
                    }
                    else
                    {
                        lstEmpPayrollData = db.tblEmpPayRollInformations.Where(z => z.JoiningDate != null && z.JoiningDate <= EndDt.Date && (z.ReLeavingDate == null || z.ReLeavingDate >= StartDt) && z.GName == groupName).ToList();
                    }

                    //Get Festivals List Of Month
                    List<tblFestival> lstFestivals = db.tblFestivals.Where(z => z.tblFestivalType.IsWorkingDay == false && z.FestivalDate >= StartDt && z.FestivalDate <= EndDt).ToList();

                    //Get Attendance Data Of Month
                    List<tblEmpAttendance> lstAttendanceData = db.tblEmpAttendances.Where(z => z.PDate.Month == filterMonth && z.PDate.Year == filterYear).ToList();

                    List<EmpAttendanceMonthFormat> lstEmps = new List<EmpAttendanceMonthFormat>();
                    foreach (tblEmpPayRollInformation item in lstEmpPayrollData)
                    {
                        EmpAttendanceMonthFormat obj = new EmpAttendanceMonthFormat();

                        #region "Employee Personal Information"
                        obj.EmpName = (from o in db.tblEmpPersonalInformations.AsEnumerable() where o.EmployeeId == item.EmployeeId select new { EmpName = o.CandidateFirstName + (o.CandidateMiddleName.Length > 1 ? o.CandidateMiddleName.Substring(0, 1) : o.CandidateMiddleName) + (o.CandidateLastName.Length > 1 ? o.CandidateLastName.Substring(0, 1) : o.CandidateLastName) }).FirstOrDefault().EmpName;
                        obj.Total_P = obj.Total_A = obj.Total_L = obj.Total_O = obj.Total_H = 0;
                        #endregion

                        #region "Employee Attendance Information"
                        // Get Whole Month Attendance Data of Employee
                        List<EmpAttendanceEmployeeMonthDetailViewModel> lstObjdetail = new List<EmpAttendanceEmployeeMonthDetailViewModel>();
                        foreach (DateTime date in AllDatesInMonth(filterYear, filterMonth))
                        {
                            EmpAttendanceEmployeeMonthDetailViewModel objD = new EmpAttendanceEmployeeMonthDetailViewModel();
                            objD.Edate = date;

                            //Check Whether Employee is joined or not on this date
                            tblEmpPayRollInformation objPayroll = lstEmpPayrollData.Where(z => z.EmployeeId == item.EmployeeId).SingleOrDefault();
                            if ((objPayroll.JoiningDate > date.Date) || (objPayroll.ReLeavingDate != null && objPayroll.ReLeavingDate < date))
                            {
                                objD.isJoined = false;
                            }
                            else
                            {
                                objD.isJoined = true;
                            }

                            //Check whether the day is Sunday or Festival Day
                            bool isHoliday = false;
                            var festival = lstFestivals.Where(z => z.FestivalDate.Date == date.Date).ToList();
                            if (festival.Count > 0)
                            {
                                isHoliday = true;
                                objD.P = "H";
                            }
                            objD.isHoliday = isHoliday;

                            // If Attendance Entry Exists
                            tblEmpAttendance objTblEmpAttendance = lstAttendanceData.Where(z => z.EmployeeId == item.EmployeeId && z.PDate.Date == date.Date).FirstOrDefault();
                            if (objTblEmpAttendance != null)
                            {
                                if (objTblEmpAttendance.Presence == 1)
                                {
                                    objD.P = "P";
                                }
                                if (objTblEmpAttendance.Leave == 1)
                                {
                                    objD.P = "L";
                                }
                                if ((objTblEmpAttendance.IsHoliday == false ? 0 : 1) == 1)
                                {
                                    objD.P = "H";
                                }
                                if (objTblEmpAttendance.Absence == 1)
                                {
                                    objD.P = "A";
                                }

                                if (objD.P == null)
                                {
                                    objD.P = "0.5";
                                }

                                objD.WORKHR = Convert.ToString(objTblEmpAttendance.WorkingHours);
                                objD.A = objTblEmpAttendance.Absence == Convert.ToDecimal(0.5) ? "0.5" : "";
                                objD.L = objTblEmpAttendance.Leave == Convert.ToDecimal(0.5) ? "0.5" : "";
                                objD.O = GetValue(objTblEmpAttendance.OT.ToString());

                                if (!string.IsNullOrEmpty(objTblEmpAttendance.Remark) || !string.IsNullOrWhiteSpace(objTblEmpAttendance.Remark))
                                {
                                    objD.Remark = "Remark : " + objTblEmpAttendance.Remark;
                                }
                                obj.Total_P += Convert.ToDouble(string.IsNullOrEmpty(objTblEmpAttendance.Presence.ToString()) ? "0" : objTblEmpAttendance.Presence.ToString());
                                obj.Total_A += Convert.ToDouble(string.IsNullOrEmpty(objTblEmpAttendance.Absence.ToString()) ? "0" : objTblEmpAttendance.Absence.ToString());
                                obj.Total_L += Convert.ToDouble(string.IsNullOrEmpty(objTblEmpAttendance.Leave.ToString()) ? "0" : objTblEmpAttendance.Leave.ToString());
                                obj.Total_O += Convert.ToDouble(string.IsNullOrEmpty(objTblEmpAttendance.OT.ToString()) ? "0" : objTblEmpAttendance.OT.ToString());
                                obj.Total_H += Convert.ToDouble(objTblEmpAttendance.IsHoliday == false ? "0" : "1");
                                //Calculate total salaried days
                                if (filterYear >= 2017)
                                {
                                    double avgLeavePerMonth = objPayroll.LeavesAllowedPerYear / 12;
                                    obj.Total_S = obj.Total_P + avgLeavePerMonth + obj.Total_O + obj.Total_H;
                                }
                                else
                                {
                                    obj.Total_S = obj.Total_P + obj.Total_O + obj.Total_H;
                                }
                                obj.Total_WORKHR += Convert.ToDouble(string.IsNullOrEmpty(objTblEmpAttendance.WorkingHours.ToString()) ? "0" : objTblEmpAttendance.WorkingHours.ToString());
                            }

                            lstObjdetail.Add(objD);
                        }
                        obj.objDetail = lstObjdetail;
                        #endregion

                        lstEmps.Add(obj);
                    }

                    //Order by EmpName Ascending
                    lstEmps = lstEmps.OrderBy(z => z.EmpName).ToList();

                    //Return Employees List Month Attendance Data
                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", lstEmps);
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

        //Get all the dates of particular month
        public static IEnumerable<DateTime> AllDatesInMonth(int year, int month)
        {
            int days = DateTime.DaysInMonth(year, month);
            for (int day = 1; day <= days; day++)
            {
                yield return new DateTime(year, month, day);
            }
        }

        // api/EmpAttendance
        // Get Employee's Month Attendance-Detail Data
        [HttpGet]
        public ApiResponse GetEmployeeMonthDetail()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int EmployeeId = Convert.ToInt32(nvc["EmpId"]);
                    int filterMonth = Convert.ToInt32(nvc["Mnth"]);
                    int filterYear = Convert.ToInt32(nvc["sYear"]);

                    // Get Whole Month Attendance Data of Employee
                    List<tblEmpAttendance> lstAttendance = db.tblEmpAttendances.Where(z => z.EmployeeId == EmployeeId && z.PDate.Month == filterMonth && z.PDate.Year == filterYear).ToList();
                    string EmployeeName = db.tblEmpPersonalInformations.Where(z => z.EmployeeId == EmployeeId).FirstOrDefault().CandidateFirstName;

                    List<EmpAttendanceEmployeeMonthDetailViewModel> lstDetail = new List<EmpAttendanceEmployeeMonthDetailViewModel>();
                    foreach (DateTime date in AllDatesInMonth(filterYear, filterMonth))
                    {
                        EmpAttendanceEmployeeMonthDetailViewModel obj = new EmpAttendanceEmployeeMonthDetailViewModel();
                        obj.EmpName = EmployeeName;
                        obj.Edate = date;

                        var inLine = db.tblEmpDailyInOuts.AsEnumerable().Where(z => z.EmployeeId == EmployeeId && z.InType == 1 && z.Intime.Date == date.Date).OrderByDescending(z => z.SrNo).FirstOrDefault();
                        var outLine = db.tblEmpDailyInOuts.AsEnumerable().Where(z => z.EmployeeId == EmployeeId && z.OutType == 2 && z.Intime.Date == date.Date).OrderByDescending(z => z.SrNo).FirstOrDefault();

                        tblEmpAttendance objTblEmpAttendance = lstAttendance.Where(z => z.EmployeeId == EmployeeId && z.PDate.Date == date.Date).FirstOrDefault();
                        if (objTblEmpAttendance != null)
                        {
                            obj.P = objTblEmpAttendance.Presence.ToString();
                            obj.A = objTblEmpAttendance.Absence.ToString();
                            obj.L = objTblEmpAttendance.Leave.ToString();
                            obj.O = objTblEmpAttendance.OT.ToString();
                            obj.H = objTblEmpAttendance.IsHoliday == false ? "0" : "1";
                            obj.Remark = objTblEmpAttendance.Remark;
                            obj.WORKHR = objTblEmpAttendance.WorkingHours.ToString();
                            obj.INTime = inLine != null ? inLine.Intime : (DateTime?)null;
                            obj.OUTTime = outLine != null ? Convert.ToDateTime(outLine.OutTime) : (DateTime?)null;
                        }
                        lstDetail.Add(obj);
                    }

                    //Return List Employee Month Attendance Data
                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", lstDetail);
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
