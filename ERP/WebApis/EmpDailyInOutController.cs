using ERP.Models;
using ERP.Utilities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Data.Entity.Core.Objects;
using System.Data.Entity;

namespace ERP.WebApis
{
    public class EmpDailyInOutViewModel
    {
        public string Comment { get; set; }
        public int InOutType { get; set; }
    }

    public class EmpInOutInfo
    {
        public DateTime date { get; set; }
        public string dayWeekName { get; set; }
        // public decimal hr { get; set; }
        //  public decimal minute { get; set; }
        //  public decimal seconds { get; set; }
        public decimal time { get; set; }

    }

    public class EmpDailyInOutController : ApiController
    {
        private ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "InOut";

        public EmpDailyInOutController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }

        public string GetLoginTypeName(int? intType)
        {
            var loginTypeName = string.Empty;
            switch (intType)
            {
                case 1:
                    loginTypeName = "CompanyIn";
                    break;
                case 2:
                    loginTypeName = "CompanyOut";
                    break;
                case 3:
                    loginTypeName = "LunchBreackIn";
                    break;
                case 4:
                    loginTypeName = "LunchBreackOut";
                    break;
                case 5:
                    loginTypeName = "CompanyWorkIn";
                    break;
                case 6:
                    loginTypeName = "CompanyWorkOut";
                    break;
                case 7:
                    loginTypeName = "PersonalWorkIn";
                    break;
                case 8:
                    loginTypeName = "PersonalWorkOut";
                    break;
            }
            return loginTypeName;
        }

        [HttpGet]
        public ApiResponse checkLoginStatus()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var empId = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());

                    //CompanyIn = 1              - InType
                    //CompanyOut = 2             - OutType
                    //LunchBreakStart = 3        - OutType
                    //LunchBreakEnd = 4          - InType
                    //CompanyWorkStart = 5       - OutType
                    //ComapnyWorkEnd = 6         - InType
                    //PersonalWorkStart = 7      - OutType
                    //PersonalWorkEnd = 8        - InType

                    var loginTypeName = string.Empty;
                    var isLogin = false;
                    DateTime? inDateTime = null;

                    var lastRecord = db.tblEmpDailyInOuts.Where(io => io.EmployeeId == empId).OrderByDescending(io => io.SrNo).FirstOrDefault();

                    if (lastRecord != null)
                    {
                        if (lastRecord.OutTime == null && lastRecord.OutType == 0 && lastRecord.InType == 1) //CompanyIn
                        {
                            isLogin = true;
                            inDateTime = lastRecord.Intime;
                            loginTypeName = GetLoginTypeName(lastRecord.InType);
                        }
                        else if (lastRecord.OutTime != null && lastRecord.OutType == 2) //CompanyOut
                        {
                            isLogin = false;
                            inDateTime = null;
                            loginTypeName = GetLoginTypeName(lastRecord.OutType);
                        }
                        else
                        {
                            //Other than CompanyIn/Out
                            isLogin = true;
                            if (lastRecord.OutType == 0)
                            {
                                //User login after lunch break/company work/personal work
                                //Getting InTime from CompanyIn record to display total time after lunch break/company work/personal work
                                var lastCompanyInRecord = db.tblEmpDailyInOuts.Where(io => io.EmployeeId == empId && io.Intime != null && io.InType == 1).OrderByDescending(io => io.SrNo).FirstOrDefault();

                                //Excluding personal work timings from total time
                                var lastPersonalWorkRecords = db.tblEmpDailyInOuts.Where(io => io.EmployeeId == empId &&
                                    ((io.OutTime != null && io.OutType == 7) || (io.Intime != null && io.InType == 8)) &&
                                    DbFunctions.TruncateTime(io.CreDate) == DbFunctions.TruncateTime(lastCompanyInRecord.CreDate))
                                    .OrderByDescending(io => io.SrNo).ToArray();
                                TimeSpan timeSpan = new TimeSpan();
                                for (int i = 0; i < lastPersonalWorkRecords.Length; i++)
                                {
                                    if (i > 0 && lastPersonalWorkRecords[i - 1].InType == 8 && lastPersonalWorkRecords[i].OutType == 7)
                                    {
                                        timeSpan += lastPersonalWorkRecords[i - 1].Intime - lastPersonalWorkRecords[i].OutTime.Value;
                                    }
                                }

                                inDateTime = lastCompanyInRecord.Intime.Add(timeSpan);
                                loginTypeName = GetLoginTypeName(lastRecord.InType);
                            }
                            else
                            {
                                //User logout for lunch break/company work/personal work
                                inDateTime = lastRecord.OutTime;
                                loginTypeName = GetLoginTypeName(lastRecord.OutType);
                            }
                        }
                    }

                    var response = new
                    {
                        isLogin = isLogin,
                        inDateTime = inDateTime,
                        loginTypeName = loginTypeName
                    };

                    apiResponse = new ApiResponse();
                    apiResponse.DataList = response;
                    apiResponse.IsValidUser = true;
                    apiResponse.MessageType = 1;
                    apiResponse.Message = "Login status got successfully";
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
                }
            }
            else
            {
                ERPUtilities.UnAuthorizedAccess(apiResponse);
            }
            return apiResponse;
        }

        [HttpPost]
        public ApiResponse SaveInOut(EmpDailyInOutViewModel empDailyInOutViewModel)
        {
            //mode 1 = In, 2 = Out
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var macAddress = ERPUtils.GetMacAddress();
                    var ipAddress = ERPUtils.GetLocalIPAddress();
                    var computerName = ERPUtils.GetComputerName();

                    var empId = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                    var empInfo = db.tblEmpPersonalInformations.SingleOrDefault(em => em.EmployeeId == empId);
                    var ecode = empInfo.EmployeeRegisterCode;

                    // Find out mode In or Out
                    var lastRecord = db.tblEmpDailyInOuts.Where(io => io.EmployeeId == empId).OrderByDescending(io => io.SrNo).FirstOrDefault();

                    var maxSrNo = 0;
                    var mode = "In";
                    if (lastRecord != null)
                    {
                        mode = lastRecord.OutTime == null ? "Out" : "In";
                        maxSrNo = Int32.Parse(lastRecord.SrNo.ToString()) + 1;
                    }

                    if (mode == "In")
                    {
                        if (lastRecord != null)
                        {
                            if ((lastRecord.OutType == 2 && empDailyInOutViewModel.InOutType == 1) || (lastRecord.OutType == 3 && empDailyInOutViewModel.InOutType == 4) || (lastRecord.OutType == 5 && empDailyInOutViewModel.InOutType == 6) || (lastRecord.OutType == 7 && empDailyInOutViewModel.InOutType == 8))
                            {
                                tblEmpDailyInOut tblInOut = new tblEmpDailyInOut();
                                tblInOut.SrNo = maxSrNo;
                                tblInOut.EmployeeId = empId;
                                tblInOut.Ecode = ecode;
                                tblInOut.Intime = DateTime.Now;
                                tblInOut.InComments = empDailyInOutViewModel.Comment;
                                tblInOut.InType = empDailyInOutViewModel.InOutType;
                                tblInOut.OutType = 0;
                                tblInOut.ComputerName = computerName;
                                tblInOut.MacAddress = macAddress;
                                tblInOut.IPAddress = ipAddress;
                                tblInOut.CreBy = empId;
                                tblInOut.CreDate = DateTime.Now;
                                tblInOut.ChangedBy = empId;
                                tblInOut.ChgDate = DateTime.Now;
                                db.tblEmpDailyInOuts.Add(tblInOut);

                            }
                            else
                            {
                                return apiResponse = ERPUtilities.GenerateApiResponse(true, 2, "You need to refresh page", null);
                            }
                        }
                        else
                        {
                            tblEmpDailyInOut tblInOut = new tblEmpDailyInOut();
                            tblInOut.SrNo = maxSrNo;
                            tblInOut.EmployeeId = empId;
                            tblInOut.Ecode = ecode;
                            tblInOut.Intime = DateTime.Now;
                            tblInOut.InComments = empDailyInOutViewModel.Comment;
                            tblInOut.InType = empDailyInOutViewModel.InOutType;
                            tblInOut.OutType = 0;
                            tblInOut.ComputerName = computerName;
                            tblInOut.MacAddress = macAddress;
                            tblInOut.IPAddress = ipAddress;
                            tblInOut.CreBy = empId;
                            tblInOut.CreDate = DateTime.Now;
                            tblInOut.ChangedBy = empId;
                            tblInOut.ChgDate = DateTime.Now;
                            db.tblEmpDailyInOuts.Add(tblInOut);
                        }

                    }
                    else
                    {
                        if (lastRecord != null)
                        {
                            if ((lastRecord.OutType == 0 || lastRecord.InType == 4 || lastRecord.InType == 6 || lastRecord.InType == 8) && (empDailyInOutViewModel.InOutType == 3 || empDailyInOutViewModel.InOutType == 5 || empDailyInOutViewModel.InOutType == 7 || empDailyInOutViewModel.InOutType == 2))
                            {
                                tblEmpDailyInOut tblInOut = lastRecord;
                                tblInOut.OutTime = DateTime.Now;
                                tblInOut.OutComments = empDailyInOutViewModel.Comment;
                                tblInOut.OutType = empDailyInOutViewModel.InOutType;
                            }
                            else
                            {
                                return apiResponse = ERPUtilities.GenerateApiResponse(true, 2, "You need to refresh page", null);
                            }
                        }
                        else
                        {
                            tblEmpDailyInOut tblInOut = lastRecord;
                            tblInOut.OutTime = DateTime.Now;
                            tblInOut.OutComments = empDailyInOutViewModel.Comment;
                            tblInOut.OutType = empDailyInOutViewModel.InOutType;
                        }
                    }
                    db.SaveChanges();
                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
                }
            }
            else
            {
                ERPUtilities.UnAuthorizedAccess(apiResponse);
            }
            return apiResponse;
        }



        [HttpGet]
        public ApiResponse GetInOutInformation()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    List<tblEmpDailyInOut> list = new List<tblEmpDailyInOut>();
                    List<EmpInOutInfo> EmpInOutList = new List<EmpInOutInfo>();
                    int employeeId = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                    var firstDate = db.tblEmpDailyInOuts.OrderByDescending(z => z.CreDate).Where(z => z.CreBy == employeeId).Select(z => EntityFunctions.TruncateTime(z.CreDate)).FirstOrDefault();
                    DateTime incrementDate = Convert.ToDateTime(firstDate);
                    int i = 0;
                    DateTime inDatetime, outDate;

                    while (i < 7)
                    {
                        list = db.tblEmpDailyInOuts.Where(z => EntityFunctions.TruncateTime(z.CreDate) == EntityFunctions.TruncateTime(incrementDate) && z.CreBy == employeeId).ToList();
                        String dayName = (incrementDate.DayOfWeek.ToString().Length > 3 ? incrementDate.DayOfWeek.ToString().Substring(0, 3) : incrementDate.DayOfWeek.ToString());
                        if (list.Count > 0)
                        {
                            int totalHours = 0, totalMinutes = 0, totalSeconds = 0;

                            foreach (var j in list)
                            {
                                inDatetime = j.Intime;
                                outDate = j.OutTime ?? DateTime.Now;
                                string deci = ERPUtils.GetDiffHourAndMinute(inDatetime, outDate);
                                totalHours += Convert.ToInt32(deci.ToString().Split('.')[0]);
                                totalMinutes += Convert.ToInt32(deci.ToString().Split('.')[1]);
                                totalSeconds += Convert.ToInt32(deci.ToString().Split('.')[2]);
                            }
                            totalHours += totalMinutes / 60;
                            totalMinutes = totalMinutes % 60 + (totalSeconds / 60);
                            totalSeconds = totalSeconds % 60;

                            EmpInOutList.Add(new EmpInOutInfo
                            {
                                date = incrementDate,
                                dayWeekName = dayName,
                                time = Convert.ToDecimal(totalHours + "." + totalMinutes)
                                // hr = totalHours,
                                // minute = totalHours,
                                // seconds = totalSeconds
                            });

                        }
                        else
                        {
                            EmpInOutList.Add(new EmpInOutInfo
                            {
                                date = incrementDate,
                                dayWeekName = dayName,
                                time = Convert.ToDecimal(00 + "." + 00)
                                //hr = 00,
                                //minute = 00,
                                //seconds = 00
                            });
                        }
                        incrementDate = incrementDate.AddDays(-1);
                        i++;
                    }
                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", EmpInOutList);
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