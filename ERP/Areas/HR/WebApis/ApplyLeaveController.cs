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
using System.IO;
using System.Data.Entity.Core.Objects;
using System.Globalization;
using System.Data.SqlClient;

namespace ERP.WebApis
{
    public class ApplyLeaveController : ApiController
    {
        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Apply Leave";

        public ApplyLeaveController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }

        /// <summary>
        /// GET api/ApplyLeave
        /// retrieve festival list 
        /// </summary>
        //[HttpGet]
        //public ApiResponse GetFestivalList()
        //{
        //    ApiResponse apiResponse = new ApiResponse();
        //    if (sessionUtils.HasUserLogin())
        //    {
        //        try
        //        {
        //            var list = (from ft in db.tblFestivalTypes
        //                        join f in db.tblFestivals on ft.FestivalTypeId equals f.FestivalTypeId into group1
        //                        from g1 in group1.DefaultIfEmpty()
        //                        where ft.IsActive == true
        //                        select new { g1.FestivalId, g1.FestivalDate, g1.FestivalTypeId, ft.FestivalType, g1.FestivalName, ft.DisplayColorCode, ft.IsWorkingDay }).ToList();

        //            //var list = db.tblFestivals.OrderBy(z => z.FestivalDate)
        //            //    .Where(z => z.IsActive == true)
        //            //    .Select(z => new { z.FestivalId, z.FestivalDate, z.FestivalTypeId, z.FestivalName}).ToList();

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

        List<tblEmpCompanyInformation> cList = new List<tblEmpCompanyInformation>();
        protected string DrillDownReporting(string strIds)
        {
            //strIds = 3,4,
            if (!string.IsNullOrEmpty(strIds))
            {
                string temp = string.Empty;
                string[] ids = strIds.Split(',');
                foreach (var id in ids)
                {
                    if (!string.IsNullOrEmpty(id))
                    {
                        if (db.tblEmpCompanyInformations.AsEnumerable().Where(z => z.ReportingTo == Convert.ToInt32(id)).Count() > 0)
                        {//take reporting id = 3 
                            var list = db.tblEmpCompanyInformations.AsEnumerable().Where(z => z.ReportingTo == Convert.ToInt32(id)).ToList();
                            foreach (var l in list)
                            { //employee id 5,6 having reporting id = 3 
                                if (l.EmployeeId != 1)
                                {
                                    cList.Add(l);
                                    temp += l.EmployeeId + ",";
                                }
                            }
                        }
                    }
                }
                return DrillDownReporting(temp);
            }
            return string.Empty;
        }

        /// <summary>
        /// GET api/ApplyLeave
        /// get user list 
        /// </summary>
        [HttpGet]
        public ApiResponse GetUserList()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {   // employee id = 2
                    int employeeId = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                    DrillDownReporting(employeeId.ToString());
                    List<SelectItemModel> eList = new List<SelectItemModel>();

                    // add logged in user line.... Checking in personal info becasue possibility to not have record for company info
                    var rLine = db.tblEmpPersonalInformations.Where(z => z.EmployeeId == employeeId).Select(z => new { z.CandidateFirstName, z.CandidateMiddleName, z.CandidateLastName }).SingleOrDefault();
                    SelectItemModel select1 = new SelectItemModel
                    {
                        Id = employeeId,
                        Label = string.Format("{0}{1}{2}", rLine.CandidateFirstName, ( rLine.CandidateMiddleName.Length > 1 ? rLine.CandidateMiddleName.Substring(0, 1) : rLine.CandidateMiddleName),(rLine.CandidateLastName.Length > 1 ? rLine.CandidateLastName.Substring(0, 1): rLine.CandidateLastName))
                    };
                    eList.Add(select1);

                    foreach (var l in cList)
                    {
                        var line = db.tblEmpPersonalInformations.Where(z => z.EmployeeId == l.EmployeeId).Select(z => new { z.CandidateFirstName, z.CandidateMiddleName, z.CandidateLastName }).SingleOrDefault();
                        SelectItemModel select2 = new SelectItemModel
                        {
                            Id = l.EmployeeId,
                            Label = string.Format("{0}{1}{2}", line.CandidateFirstName, (line.CandidateMiddleName.Length > 1 ? line.CandidateMiddleName.Substring(0, 1) : line.CandidateMiddleName ),( line.CandidateLastName.Length > 1 ? line.CandidateLastName.Substring(0, 1): line.CandidateLastName))
                        };
                        eList.Add(select2);
                    }

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, employeeId.ToString(), eList.OrderBy(z => z.Label).ToList()); //passing logged in user id in message
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
        /// Check Wether Logged User is Team Leader or Not
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiResponse CheckTeamLead()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int employeeId = Convert.ToInt32(nvc["employeeId"].ToString());

                    var list = db.tblEmpCompanyInformations.Where(z => z.EmployeeId == employeeId && z.IsTL == true).ToList();
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
        /// GET api/ApplyLeave
        /// retrieve calendar leave list 
        /// </summary>
        [HttpGet]
        public ApiResponse GetCalendarLeaveList()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    //Guid GroupId = new Guid();
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int employeeId = Convert.ToInt32(nvc["employeeId"].ToString());
                    bool? isAll = Convert.ToBoolean(nvc["IsAll"].ToString());
                    int timezone = Convert.ToInt32(nvc["timeZone"].ToString());

                    List<calenderSpLeaveList> Splist = new List<calenderSpLeaveList>();

                    var EmployeeId = new SqlParameter("@EmployeeId", employeeId);
                    var IsAll = new SqlParameter("@IsAll", isAll);
                    Splist = db.Database.SqlQuery<calenderSpLeaveList>("usp_getApplyLeaveList  @EmployeeId,@IsAll", EmployeeId, IsAll).AsEnumerable().ToList();

                    var eventList = Splist.Select(i =>
                    {
                        i.CreDate = Convert.ToDateTime(i.CreDate).AddMinutes(-1 * timezone);
                        i.ChgDate = Convert.ToDateTime(i.ChgDate).AddMinutes(-1 * timezone);
                        return i;
                    }).ToList();

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", eventList);


                    ////Guid GroupId = new Guid();
                    //NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    //List<tblApplyLeave> applyList = new List<tblApplyLeave>();
                    //int employeeId = Convert.ToInt32(nvc["employeeId"].ToString());
                    //Boolean IsAll = Convert.ToBoolean(nvc["IsAll"].ToString());
                    //int timezone = Convert.ToInt32(nvc["timeZone"].ToString());

                    ////isAll is true to be display member information
                    //var count = db.tblEmpCompanyInformations.Where(z => z.EmployeeId == employeeId && z.IsTL == true && IsAll == true).Count();
                    //if (count > 0)
                    //{
                    //    DrillDownReporting(employeeId.ToString());
                    //    foreach (var l in cList)
                    //    {
                    //        var items = db.tblApplyLeaves.Where(z => z.EmployeeId == l.EmployeeId && z.Status != "Cancel").ToList();
                    //        foreach (var item in items)
                    //        {
                    //            applyList.Add(item);
                    //        }
                    //    }
                    //    //used for login user
                    //    var loginUser = db.tblApplyLeaves.Where(z => z.EmployeeId == employeeId && z.Status != "Cancel").ToList();
                    //    foreach (var user in loginUser)
                    //    {
                    //        applyList.Add(user);
                    //    }
                    //}
                    //else
                    //{
                    //    applyList = db.tblApplyLeaves.Where(z => z.EmployeeId == employeeId && z.Status != "Cancel").ToList();
                    //}

                    //var eventList = applyList.Select(i =>
                    //{
                    //    i.CreDate = Convert.ToDateTime(i.CreDate).AddMinutes(-1 * timezone);
                    //    i.ChgDate = Convert.ToDateTime(i.ChgDate).AddMinutes(-1 * timezone);
                    //    return i;
                    //}).ToList();

                    //apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", eventList);
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
        /// GET api/ApplyLeave
        /// retrieve absent and leave list 
        /// </summary>
        [HttpGet]
        public ApiResponse GetLeaveAbsentList()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    //Guid GroupId = new Guid();
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int employeeId = Convert.ToInt32(nvc["employeeId"].ToString());
                    //Need to pass both absent and leave data together [Pending for absent records]
                    //urvish
                    var list = db.tblApplyLeaves
                        .Where(z => z.EmployeeId == employeeId && z.PartFullTime == "F" && z.Status != "Cancel")
                        .Select(z => new { Key = "P", Value = z.StartDate }).ToList();
                    //var list = db.tblApplyLeaves
                    //   .Where(z => z.EmployeeId == employeeId && z.PartFullTime == "F")
                    //   .Select(z => new { Key = "P", Value = z.StartDate }).ToList();


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
        /// GET api/ApplyLeave
        /// check for entry exists or not 
        /// </summary>
        /*[HttpGet]
        public ApiResponse IsEntryExists()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    string startDate = nvc["startDate"];
                    string endDate = nvc["endDate"];
                    int employeeId = Convert.ToInt32(nvc["employeeId"]);

                    bool isExists = false;
                    for (DateTime dt = Convert.ToDateTime(startDate); dt <= Convert.ToDateTime(endDate); dt = dt.AddDays(1)) {
                        if (db.tblApplyLeaves.Where(z => EntityFunctions.TruncateTime(z.StartDate) == dt.Date && z.EmployeeId == employeeId).Count() > 0)
                        {
                            isExists = true;
                        }
                    }
                    if (isExists)
                    {
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 2, generalMessages.msgEntryExists, isExists);
                    }
                    else {
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgEntryExists, isExists);
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
        }*/

        /// <summary>
        /// GET api/ApplyLeave
        /// cancel leave
        /// </summary>
        [HttpGet]
        public ApiResponse CancelLeave()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    string date = nvc["date"];
                    int employeeId = Convert.ToInt32(nvc["employeeId"]);
                    DateTime dtStart = new DateTime();
                    
                    //string[] dt = Convert.ToDateTime(date).ToString("dd-MM-yyyy").Split('-');
                    //dtStart = new DateTime(Convert.ToInt32(dt[2]), Convert.ToInt32(dt[1]), Convert.ToInt32(dt[0]), 0, 0, 0);
                    //urvish
                    dtStart = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                    //take row of applying cancel date from db
                    var leave = db.tblApplyLeaves.AsEnumerable().Where(z => z.StartDate.Date == dtStart.Date && z.EmployeeId == employeeId).FirstOrDefault();

                    if (leave != null)
                    {//save changes to db for edit operation
                        leave.Status = "Cancel";
                        leave.ChgDate = DateTime.Now.ToUniversalTime();
                        leave.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());

                        if (leave.PartFullTime == "F")
                        {
                            RemoveSandwichLeave(dtStart, leave.EmployeeId, false); //remove before sandwich leaves
                            RemoveSandwichLeave(dtStart, leave.EmployeeId, true); // remove after sandwich leaves
                        }
                        db.SaveChanges();

                        // GET INFORMATION FOR  MAIL TEMAPLE
                        var emp = db.tblEmpPersonalInformations.Where(z => z.EmployeeId == employeeId).FirstOrDefault();
                        var line = db.tblEmpPersonalInformations.AsEnumerable().Where(z => z.EmployeeId == Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString())).FirstOrDefault(); //Login user
                        var compInfo = db.tblEmpCompanyInformations.AsEnumerable().Where(ci => ci.EmployeeId == Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString())).FirstOrDefault();
                       
                        //send mail
                       string body = string.Empty;
                       using (var sr = new StreamReader(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["tmplCancelLeave"].ToString())))
                    {
                        body = sr.ReadToEnd();
                        body = body.Replace("##username##", emp.CandidateFirstName + " " + emp.CandidateLastName);
                        body = body.Replace("##LeaveDate##", date);
                        body = body.Replace("##ActionBy##", line.CandidateFirstName + " " + line.CandidateLastName);
                        body = body.Replace("##EmailId##", line.CompanyEmailId);
                        body = body.Replace("##Designation##", db.tblDesignations.AsEnumerable().Where(d => d.Id == compInfo.DesignationId).FirstOrDefault().Designation);
                        body = body.Replace("##Status##", "<b style='color: #008000;font-size: 17px;'>Approved</b>");
                        //Set Texture And Logo
                        var staticImagesPath = ConfigurationManager.AppSettings["StaticImagesPath"].ToString();
                        var logoImage = "logo-drc-mail.png";
                        var textureImage = "daimond_eyes.png";
                        body = body.Replace("##bgTexturePath##", staticImagesPath + textureImage);
                        body = body.Replace("##logoPath##", staticImagesPath + logoImage);
                    }
                         //var emp = db.tblEmpPersonalInformations.Where(z => z.EmployeeId == employeeId).FirstOrDefault();
                       // string body = generalMessages.msgCancel + "<br/>Date:" + date;
                        ERPUtilities.SendMailToMany(employeeId, ConfigurationManager.AppSettings["fromMail"].ToString(), emp.CompanyEmailId, "Successfully canceled", body, _pageName);
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgCancel, true);
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
        /// GET api/ApplyLeave
        /// approve disapprove leave
        /// </summary>
        [HttpGet]
        public ApiResponse ApproveDisapproveLeave()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    string date = nvc["leaveDate"];
                    int employeeId = Convert.ToInt32(nvc["employeeId"]);
                    bool isApproved = Convert.ToBoolean(nvc["isApproved"]);

                    var line = db.tblEmpPersonalInformations.AsEnumerable().Where(z => z.EmployeeId == Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString())).FirstOrDefault(); //Login user
                    var compInfo = db.tblEmpCompanyInformations.AsEnumerable().Where(ci => ci.EmployeeId == Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString())).FirstOrDefault();
                    string formattedReason = string.Format("{0}${1} {2}${3}${4}", isApproved ? "DisApproved" : "Approved", line.CandidateFirstName, line.CandidateLastName, nvc["reason"],string.Format("{0:dd-MMM-yyyy hh:mm:ss tt}" ,DateTime.Now)); //approved$Name$Reason$timestamp

                    DateTime dtStart = new DateTime();
                    //urvish
                    //string[] dt = Convert.ToDateTime(date).ToString("dd-MM-yyyy").Split('-');
                    //dtStart = new DateTime(Convert.ToInt32(dt[2]), Convert.ToInt32(dt[1]), Convert.ToInt32(dt[0]), 0, 0, 0);
                    dtStart = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);



                    //first update entry of date on which we are doing operation
                    var rLine = db.tblApplyLeaves.AsEnumerable().Where(z => z.StartDate.Date == dtStart.Date && z.EmployeeId == employeeId).FirstOrDefault();
                    var rlineUser = db.tblEmpPersonalInformations.AsEnumerable().Where(z => z.EmployeeId == employeeId).FirstOrDefault();
                    if (rLine != null)
                    {
                        rLine.Status = isApproved ? "DisApproved" : "Approved";
                        rLine.ApproveReason = string.Format("{0}|{1}", formattedReason, rLine.ApproveReason);
                        rLine.ChgDate = DateTime.Now.ToUniversalTime();
                        rLine.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                    }

                    //now check for sandwiche leave approve before and after
                    if (isApproved)
                    {//trying to disapprove
                        ApproveLeave(dtStart, employeeId, false);
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgDisApproved, true);
                    }
                    else
                    {//trying to approve
                        ApproveLeave(dtStart, employeeId, true);
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgApproved, true);
                    }

                    db.SaveChanges();

                    //send mail
                    string body = string.Empty;
                    //Read template file from folder

                    using (var sr = new StreamReader(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings[isApproved ? "tmplDisapproveLeave" : "tmplApproveLeave"].ToString())))
                    {
                        body = sr.ReadToEnd();
                        body = body.Replace("##username##", rlineUser.CandidateFirstName + " " + rlineUser.CandidateLastName);
                        body = body.Replace("##LeaveDate##", date);
                        body = body.Replace("##LeaveReason##", nvc["reason"].ToString());
                        body = body.Replace("##ActionBy##", line.CandidateFirstName + " " + line.CandidateLastName);
                        body = body.Replace("##EmailId##", line.CompanyEmailId);
                        body = body.Replace("##Designation##", db.tblDesignations.AsEnumerable().Where(d => d.Id == compInfo.DesignationId).FirstOrDefault().Designation);

                        if (isApproved)
                        {
                            body = body.Replace("##Status##", "<b style='color: #F00;font-size: 17px;'>DisApproved</b>");
                        }
                        else
                        {
                            body = body.Replace("##Status##", "<b style='color: #008000;font-size: 17px;'>Approved</b>");
                        }

                        //Set Texture And Logo
                        var staticImagesPath = ConfigurationManager.AppSettings["StaticImagesPath"].ToString();
                        var logoImage = "logo-drc-mail.png";
                        var textureImage = "daimond_eyes.png";
                        body = body.Replace("##bgTexturePath##", staticImagesPath + textureImage);
                        body = body.Replace("##logoPath##", staticImagesPath + logoImage);
                    }

                    var emp = db.tblEmpPersonalInformations.Where(z => z.EmployeeId == employeeId).FirstOrDefault();
                    //string body = string.Format("{0}<br/>Date: {1}", isApproved ? generalMessages.msgDisApproved : generalMessages.msgApproved, date);
                    string subject = string.Format("Successfully {0}", isApproved ? "DisApproved" : "Approved");
                    ERPUtilities.SendMailToMany(employeeId, ConfigurationManager.AppSettings["fromMail"].ToString(), emp.CompanyEmailId, subject, body, _pageName);
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
        /// POST api/ApplyLeave
        /// create and update record
        /// </summary>
        [HttpPost]
        public ApiResponse SaveApplyLeave(ApplyLeaveViewModel leave)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    //Guid uniqueId = leave.GroupId;
                    //uniqueId = uniqueId == Guid.Empty ? uniqueId = Guid.NewGuid() : leave.GroupId;

                    //if (!leave.IsSandwich && (leave.IsFestival || leave.IsSunday))
                    //{//Allow festival and sunday leave entry only in case of sandwich leave 
                    //    return apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, uniqueId);
                    //}

                    //datetime
                    DateTime dtStart = new DateTime();
                    DateTime dtEnd = new DateTime();
                    string[] dt = Convert.ToDateTime(leave.StartDate).ToString("dd-MM-yyyy").Split('-');

                    if (leave.PartFullTime == "P")
                    {
                        string[] t1 = leave.StartTime.Split(':');
                        string[] t2 = leave.EndTime.Split(':');
                        dtStart = new DateTime(Convert.ToInt32(dt[2]), Convert.ToInt32(dt[1]), Convert.ToInt32(dt[0]), Convert.ToInt32(t1[0]), Convert.ToInt32(t1[1]), 0);
                        dtEnd = new DateTime(Convert.ToInt32(dt[2]), Convert.ToInt32(dt[1]), Convert.ToInt32(dt[0]), Convert.ToInt32(t2[0]), Convert.ToInt32(t2[1]), 0);
                    }
                    else if (leave.PartFullTime == "F")
                    {
                        dtStart = new DateTime(Convert.ToInt32(dt[2]), Convert.ToInt32(dt[1]), Convert.ToInt32(dt[0]), 0, 0, 0);
                        dtEnd = new DateTime(Convert.ToInt32(dt[2]), Convert.ToInt32(dt[1]), Convert.ToInt32(dt[0]), 23, 59, 59);
                    }

                    if (leave.Mode == "Add")
                    {
                        var line = db.tblApplyLeaves.AsEnumerable().Where(z => z.EmployeeId == leave.EmployeeId && z.StartDate.Date == dtStart.Date).FirstOrDefault();
                        if (line != null)
                        {
                            if (line.Status == "Cancel")
                            {
                                //Update the entry
                                line.LeaveTitle = leave.LeaveTitle;
                                line.LeaveType = leave.LeaveType;
                                line.StartDate = dtStart;
                                line.EndDate = dtEnd;
                                line.PartFullTime = leave.PartFullTime;
                                line.Status = "Pending";
                                line.Comments = leave.Comments;
                                line.ChgDate = DateTime.Now.ToUniversalTime();
                                line.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());

                                db.SaveChanges();
                            }
                            //Return if entry is already exists and status is not cancel
                            return apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, true);
                        }

                        if (leave.EmployeeId > 0)
                        {// Mode == Add
                            tblApplyLeave l = new tblApplyLeave
                            {
                                EmployeeId = leave.EmployeeId,
                                LeaveTitle = leave.LeaveTitle,
                                LeaveType = leave.LeaveType,
                                StartDate = dtStart,
                                EndDate = dtEnd,
                                PartFullTime = leave.PartFullTime,
                                Comments = leave.Comments,
                                Status = "Pending",
                                ApproveReason = "",
                                CreDate = DateTime.Now.ToUniversalTime(),
                                CreBy = Convert.ToInt32(HttpContext.Current.Session["UserId"]),
                                ChgDate = DateTime.Now.ToUniversalTime(),
                                ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"])
                            };
                            db.tblApplyLeaves.Add(l);
                            db.SaveChanges();
                            apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, true);
                        }
                    }
                    else if (leave.Mode == "Edit")
                    { // it will be only single day
                        if (db.tblApplyLeaves.AsEnumerable().Where(z => z.EmployeeId == leave.EmployeeId && z.StartDate.Date == dtStart.Date).Count() > 0)
                        {
                            //save changes to db for edit operation
                            var line = db.tblApplyLeaves.AsEnumerable().Where(z => z.EmployeeId == leave.EmployeeId && z.StartDate.Date == dtStart.Date).FirstOrDefault();
                            line.LeaveTitle = leave.LeaveTitle;
                            line.LeaveType = leave.LeaveType;
                            line.StartDate = dtStart;
                            line.EndDate = dtEnd;
                            line.PartFullTime = leave.PartFullTime;
                            line.Comments = leave.Comments;
                            line.ChgDate = DateTime.Now.ToUniversalTime();
                            line.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());

                            if (leave.PartFullTime == "P")
                            {
                                RemoveSandwichLeave(dtStart, leave.EmployeeId, false); //remove before sandwich leaves
                                RemoveSandwichLeave(dtStart, leave.EmployeeId, true); // remove after sandwich leaves
                            }
                            else
                            { //for Full Time; looping the before and after days for sandwich case
                                InsertSandwichLeave(dtStart, leave.EmployeeId); //insert sandwich leave
                            }
                        }

                        db.SaveChanges();

                        //send mail
                        var emp = db.tblEmpPersonalInformations.Where(z => z.EmployeeId == leave.EmployeeId).FirstOrDefault();
                        string body = string.Format("{0}<br/>Date: {1}", generalMessages.msgUpdate, Convert.ToDateTime(leave.StartDate).ToString("dd-MM-yyyy"));
                        ERPUtilities.SendMailToMany(leave.EmployeeId, ConfigurationManager.AppSettings["fromMail"].ToString(), emp.CompanyEmailId, "Successfully Updated", body, _pageName);

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
        /// GET api/ApplyLeave
        /// Send mail on apply leave
        /// </summary>
        [HttpGet]
        public ApiResponse SendMail()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    string sDate = nvc["startDate"];
                    string eDate = nvc["endDate"];
                    int employeeId = Convert.ToInt32(nvc["employeeId"]);

                    //send mail
                    var emp = db.tblEmpPersonalInformations.Where(z => z.EmployeeId == employeeId).FirstOrDefault();
                    string body = string.Empty;

                    //Read template file from folder
                    using (var sr = new StreamReader(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["tmplApplyLeave"].ToString())))
                    {
                        body = sr.ReadToEnd();
                        if (sDate != eDate)
                        {
                            body = body.Replace("##LeaveDate##", string.Format("{0} to {1}", sDate, eDate));
                        }
                        else
                        {
                            body = body.Replace("##LeaveDate##", sDate);
                        }
                        body = body.Replace("##username##", emp.CandidateFirstName + " " + emp.CandidateLastName);
                        body = body.Replace("##LeaveReason##", nvc["comments"].ToString());

                        //Set Texture And Logo
                        var staticImagesPath = ConfigurationManager.AppSettings["StaticImagesPath"].ToString();
                        var logoImage = "logo-drc-mail.png";
                        var textureImage = "daimond_eyes.png";
                        body = body.Replace("##bgTexturePath##", staticImagesPath + textureImage);
                        body = body.Replace("##logoPath##", staticImagesPath + logoImage);
                    }

                    ERPUtilities.SendMailToMany(employeeId, ConfigurationManager.AppSettings["fromMail"].ToString(), emp.CompanyEmailId, "Successfully Applied", body, _pageName);

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgApplied, true);
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


        private void ApproveLeave(DateTime leaveDate, int employeeId, bool isApproving)
        {
            DateTime date = leaveDate;
            List<DateTime> tempArray = new List<DateTime>();

            //check for before dates
            bool keepGoing = true;
            while (keepGoing)
            {
                date = date.AddDays(-1);
                //if = sandwich leave > push in temp array 
                if (IsSandwichLeave(date, employeeId))
                {
                    tempArray.Add(date);
                }
                else
                {
                    tblApplyLeave leaveDetails = IsPreLeave(date, employeeId);
                    if (leaveDetails != null)
                    {
                        if (leaveDetails.Status == "Approved" && isApproving == true)
                        {
                            // make sandwich leave to approve holiday
                            ApproveSandwichLeave(tempArray, employeeId);
                        }
                        else if (leaveDetails.Status == "DisApproved" || (leaveDetails.Status == "Approved" && isApproving == false))
                        {
                            //make sandwich leave to disapprove holiday
                            DisApproveSandwichLeave(tempArray, employeeId);
                        }

                        //if approved/disapprove leave found > stop loop execution
                        tempArray.Clear();
                        keepGoing = false;
                    }
                    else
                    {// no approved record found
                        keepGoing = false;
                    }
                }
            }

            //check for after dates
            tempArray.Clear();
            keepGoing = true;
            date = leaveDate;
            while (keepGoing)
            {
                date = date.AddDays(1);
                //if sandwich leave > push in temp array 
                if (IsSandwichLeave(date, employeeId))
                {
                    tempArray.Add(date);
                }
                else
                {
                    tblApplyLeave leaveDetails = IsPreLeave(date, employeeId);
                    if (leaveDetails != null)
                    {
                        if (leaveDetails.Status == "Approved" && isApproving == true)
                        {
                            // make sandwich leave to approve holiday
                            ApproveSandwichLeave(tempArray, employeeId);
                        }
                        else if (leaveDetails.Status == "DisApproved" || (leaveDetails.Status == "Approved" && isApproving == false))
                        {
                            //make sandwich leave to disapprove holiday
                            DisApproveSandwichLeave(tempArray, employeeId);
                        }

                        //if approved/disapprove leave found > stop loop execution
                        tempArray.Clear();
                        keepGoing = false;
                    }
                    else
                    {// no pre-leave record found
                        keepGoing = false;
                    }
                }
            }
        }

        private void RemoveSandwichLeave(DateTime leaveDate, int employeeId, bool isNext)
        {
            DateTime date = leaveDate;
            bool keepGoing = true;
            while (keepGoing)
            {
                date = isNext ? date.AddDays(1) : date.AddDays(-1);
                var sLine = db.tblApplyLeaves.AsEnumerable().Where(z => z.EmployeeId == employeeId && z.StartDate == date.Date && z.LeaveTitle == "Sandwich Leave").SingleOrDefault();
                if (sLine != null)
                {
                    db.tblApplyLeaves.Remove(sLine);
                    keepGoing = true;
                }
                else
                {
                    keepGoing = false;
                }
            }
        }

        private void InsertSandwichLeave(DateTime leaveDate, int employeeId)
        {
            DateTime date = leaveDate;
            List<DateTime> dateArray = new List<DateTime>();
            List<DateTime> tempArray = new List<DateTime>();

            //check for before dates
            bool keepGoing = true;
            while (keepGoing)
            {
                date = date.AddDays(-1);
                //if = festival > push in temp array 
                if (IsFestival(date))
                {
                    tempArray.Add(date);
                }
                else
                {
                    tblApplyLeave leaveDetails = IsPreLeave(date, employeeId);
                    if (leaveDetails != null)
                    {
                        if (leaveDetails.Status != "Cancel" || leaveDetails.Status != "DisApproved")
                        {//if pre leave > stop loop execution and push entries from temp to main array
                            dateArray.AddRange(tempArray);
                            tempArray.Clear();
                            keepGoing = false;
                        }
                    }
                    else
                    {// no pre-leave record found
                        keepGoing = false;
                    }
                }
            }

            //check for after dates
            tempArray.Clear();
            keepGoing = true;
            date = leaveDate;
            while (keepGoing)
            {
                date = date.AddDays(1);
                //if festival > push in temp array 
                if (IsFestival(date))
                {
                    tempArray.Add(date);
                }
                else
                {
                    tblApplyLeave leaveDetails = IsPreLeave(date, employeeId);
                    if (leaveDetails != null)
                    {
                        if (leaveDetails.Status != "Cancel" || leaveDetails.Status != "DisApproved")
                        {//if pre leave > stop loop execution and push entries from temp to main array
                            dateArray.AddRange(tempArray);
                            tempArray.Clear();
                            keepGoing = false;
                        }
                    }
                    else
                    {// no pre-leave record found
                        keepGoing = false;
                    }
                }
            }

            //now insert main array to db. All entries will be sandwich leave
            foreach (var dt in dateArray)
            {
                if (db.tblApplyLeaves.AsEnumerable().Where(z => z.EmployeeId == employeeId && z.StartDate.Date == dt.Date).Count() == 0)
                {
                    tblApplyLeave leave = new tblApplyLeave()
                    {
                        EmployeeId = employeeId,
                        LeaveTitle = "Sandwich Leave",
                        LeaveType = 1,
                        StartDate = dt.Date + new TimeSpan(0, 0, 0),
                        EndDate = dt.Date + new TimeSpan(23, 59, 59),
                        PartFullTime = "F",
                        Comments = "",
                        Status = "Pending",
                        CreDate = DateTime.Now.ToUniversalTime(),
                        CreBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString()),
                        ChgDate = DateTime.Now.ToUniversalTime(),
                        ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString())
                    };
                    db.tblApplyLeaves.Add(leave);
                }
            }
        }

        private Boolean IsFestival(DateTime festivalDate)
        {
            var list = (from ft in db.tblFestivalTypes
                        join f in db.tblFestivals on ft.FestivalTypeId equals f.FestivalTypeId into group1
                        from g1 in group1.DefaultIfEmpty()
                        where ft.IsActive == true && ft.IsWorkingDay == false && g1.FestivalDate == festivalDate.Date
                        select new { g1.FestivalId }).ToList();

            if (list.Count() > 0)
            {
                return true;
            }
            return false;
        }

        private Boolean IsSandwichLeave(DateTime leaveDate, int employeeId)
        {
            var line = db.tblApplyLeaves.AsEnumerable().Where(z => z.EmployeeId == employeeId && z.StartDate.Date == leaveDate.Date && z.LeaveTitle == "Sandwich Leave").FirstOrDefault();
            if (line != null)
            {
                return true;
            }
            return false;
        }

        //private Boolean IsSunday(DateTime date) {
        //    if (date.DayOfWeek == DayOfWeek.Sunday) {
        //        return true;
        //    }
        //    return false;
        //}

        private tblApplyLeave IsPreLeave(DateTime date, int employeeId)
        {
            if (db.tblApplyLeaves.AsEnumerable().Where(z => z.EmployeeId == employeeId && z.StartDate.Date == date.Date && z.PartFullTime == "F").Count() > 0)
            {
                return db.tblApplyLeaves.AsEnumerable().Where(z => z.EmployeeId == employeeId && z.StartDate.Date == date.Date).FirstOrDefault();
            }
            return null;
        }

        private void ApproveSandwichLeave(List<DateTime> lstDate, int employeeId)
        {
            foreach (var d in lstDate)
            {
                var line = db.tblApplyLeaves.AsEnumerable().Where(z => z.StartDate.Date == d.Date && z.EmployeeId == employeeId).FirstOrDefault();
                if (line != null)
                {
                    line.Status = "Approved";
                }
            }
        }

        private void DisApproveSandwichLeave(List<DateTime> lstDate, int employeeId)
        {
            foreach (var d in lstDate)
            {
                var line = db.tblApplyLeaves.AsEnumerable().Where(z => z.StartDate.Date == d.Date && z.EmployeeId == employeeId).FirstOrDefault();
                if (line != null)
                {
                    line.Status = "DisApproved";
                }
            }
        }



    }
}
