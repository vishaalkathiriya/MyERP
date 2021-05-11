using ERP.Models;
using ERP.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Data;



namespace ERP.WebApis
{
    public class PMSReportController : ApiController
    {
        private ERPContext db = new ERPContext();
        SessionUtils sessionUtils = new SessionUtils();
        GeneralMessages generalMessages = null;
        string _pageName = "PMS Report";

        public PMSReportController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }


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
        /// GET api/PMSReport
        /// retrieve user list 
        /// </summary>
        [HttpGet]
        public ApiResponse GetUserList()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    int employeeId = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                    DrillDownReporting(employeeId.ToString());

                    
                    // var lstUser = db.tblPMSProjectUsers.Select(z => z.EmployeeId).Distinct();
                    List<SelectItemModel> list = new List<SelectItemModel>();
                    foreach (var l in cList)
                    {
                        var line = db.tblEmpPersonalInformations.Where(z => z.EmployeeId == l.EmployeeId).SingleOrDefault();
                        list.Add(new SelectItemModel { Id = l.EmployeeId, Label = line.CandidateFirstName + (line.CandidateMiddleName.Length > 1 ? line.CandidateMiddleName.Substring(0, 1) : line.CandidateMiddleName) + ( line.CandidateLastName.Length > 1 ? line.CandidateLastName.Substring(0, 1) : line.CandidateLastName) });
                    }

                    var own_user = db.tblEmpPersonalInformations.Where(z => z.EmployeeId == employeeId).SingleOrDefault();
                    list.Add(new SelectItemModel { Id = own_user.EmployeeId, Label = own_user.CandidateFirstName + (own_user.CandidateMiddleName.Length > 1 ? own_user.CandidateMiddleName.Substring(0, 1) : own_user.CandidateMiddleName) + (own_user.CandidateLastName.Length > 1 ? own_user.CandidateLastName.Substring(0, 1) : own_user.CandidateLastName) });

                    list = list != null ? list.OrderBy(z => z.Label).ToList() : list;

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
        /// GET api/PMSReport
        /// retrieve project list 
        /// </summary>
        [HttpGet]
        public ApiResponse GetProjectList()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    //var lstProject = db.tblPMSProjectUsers.Select(z => z.ProjectId).Distinct();
                    //List<SelectItemModel> list = new List<SelectItemModel>();
                    //foreach (var l in lstProject)
                    //{
                    //    var line = db.tblPMSProjects.Where(z => z.ProjectId == l).SingleOrDefault();
                    //    list.Add(new SelectItemModel { Id = l, Label = line.ProjectName });
                    //}

                    //list = list != null ? list.OrderBy(z => z.Label).ToList() : list;
                    //apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", list);

                    //---------------------------------------------------------------------------

                    int loginId = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                    List<tblPMSProject> projects = new List<tblPMSProject>();
                    if (loginId == 1)
                    {
                        projects = db.tblPMSProjects.Where(z => z.Status == 1 && z.IsArchived == false).ToList();
                    }
                    else
                    {
                        var list = db.tblPMSProjects.Where(z => z.Status == 1 && z.IsArchived == false).ToList();
                        foreach (var l in list)
                        {
                            List<tblEmpCompanyInformation> lstEmpCompanyInformation = new List<tblEmpCompanyInformation>();
                            EmployeeUtils employeeUtils = new EmployeeUtils();
                            lstEmpCompanyInformation = employeeUtils.DrillDownReporting(loginId.ToString(), lstEmpCompanyInformation);
                            List<int> lstEmpIds = new List<int>();
                            lstEmpIds.Add(loginId);
                            foreach (var ecInfo in lstEmpCompanyInformation)
                            {
                                lstEmpIds.Add(ecInfo.EmployeeId);
                            }
                            if (db.tblPMSProjectUsers.Where(z => lstEmpIds.Contains(z.EmployeeId) && z.ProjectId == l.ProjectId).Count() > 0)
                            {
                                projects.Add(l);
                            }
                        }
                    }
                    List<SelectItemModel> list_project = new List<SelectItemModel>();

                    foreach (var p in projects)
                    {
                        list_project.Add(new SelectItemModel { Id = p.ProjectId, Label = p.ProjectName });

                    }
                    list_project = list_project != null ? list_project.OrderBy(z => z.Label).ToList() : list_project;
                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", list_project);
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

        protected Boolean isRecored(int todoid, int creBy, DateTime date)
        {
            List<tblPMSComment> list = new List<tblPMSComment>();
            // list = db.tblPMSComments.AsEnumerable().Where(z => z.TodoId == todoid && z.CreBy == creBy && z.CreDate.Date == date.Date && z.IsActive == true).ToList();
            list = db.tblPMSComments.AsEnumerable().Where(z => z.TodoId == todoid && z.CreBy == creBy && z.CreDate.Date == date.Date && z.IsArchived == false).ToList();
            if (list.Count() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
            //var commentList = db.tblPMSComments.AsEnumerable().Where(z => z.TodoId == todo.TodoId && z.CreBy == reportUserId && z.CreDate.Date == date.AddDays(-i).Date && z.IsActive == true).ToList();//z => z.CreDate.Date == dtStart.Date && 
        }


        [HttpGet]
        public ApiResponse reportUserAndProjectwise()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);



                    int LoadType = Convert.ToInt16(nvc["LoadType"].ToString());
                    int reportProjectId = Convert.ToInt32(nvc["reportProjectId"].ToString());
                    DateTime? date = Convert.ToDateTime(nvc["date"].ToString());
                    int counter = Convert.ToInt32(nvc["counter"].ToString());
                    int userTotalWorkHours = 0, userTotalWorkMinutes = 0;
                    int projectTotalWorkHours = 0, projectTotalWorkMinute = 0;
                    DateTime fromDate, toDate;


                    string startDate = nvc["startDate"];
                    string endDate = nvc["endDate"];

                    if (!string.IsNullOrEmpty(startDate))
                    {
                        string[] fdate = startDate.Split('/');
                        fromDate = new DateTime(Convert.ToInt32(fdate[2]), Convert.ToInt32(fdate[1]), Convert.ToInt32(fdate[0]));
                    }
                    else
                    {
                        fromDate = Convert.ToDateTime("1/1/1900");
                    }

                    if (!string.IsNullOrEmpty(endDate))
                    {
                        string[] tdate = endDate.Split('/');
                        toDate = new DateTime(Convert.ToInt32(tdate[2]), Convert.ToInt32(tdate[1]), Convert.ToInt32(tdate[0]));
                    }
                    else
                    {
                        toDate = Convert.ToDateTime("1/1/1900");
                    }



                    //loadtype:1-base userwise , 2-project wise
                    List<tblPMSToDo> pmsTodoList = new List<tblPMSToDo>();

                    // DateTime date= db.tblPMSComments.AsEnumerable().Where(z=>z.CreBy == reportUserId && z.IsActive == true)
                    //var commentList = db.tblPMSComments.AsEnumerable().Where(z => z.TodoId == todo.TodoId && z.CreBy == reportUserId && z.CreDate.Date == date.AddDays(-i).Date && z.IsActive == true).ToList();//z => z.CreDate.Date == dtStart.Date && 

                    if (LoadType == 1)
                    {//userwise report
                        //Stored SP List informaiton
                        List<PMSProjectAndUserListViewModel> spList = new List<PMSProjectAndUserListViewModel>();
                        List<PMSUserWiseReportViewModel> userWiseReportList = new List<PMSUserWiseReportViewModel>();
                        List<pmsUserWiseWeekReport> spUserWeekReport = new List<pmsUserWiseWeekReport>();
                        List<pmsUserWiseWeekReport> UserWeekReport = new List<pmsUserWiseWeekReport>();

                        decimal projectTotalWork, sum_week_grant_report=0;

                        var userProjectFlag = new SqlParameter("@userProjectFlag", LoadType);
                        var userIdParam = new SqlParameter("@userId", reportProjectId);
                        var startDateParam = new SqlParameter("@fromDate", fromDate);
                        var endDateParam = new SqlParameter("@toDate", toDate);

                        if (fromDate.ToString("d/M/yyyy") != "1/1/1900" && toDate.ToString("d/M/yyyy") != "1/1/1900")
                        {
                            spUserWeekReport = db.Database.SqlQuery<pmsUserWiseWeekReport>("usp_PMSWeekReport @userProjectFlag,@fromdate,@todate,@userid", userProjectFlag, startDateParam, endDateParam, userIdParam).ToList();
                            
                           // sum_week_grant_report = spUserWeekReport.Sum(z => Convert.ToDecimal(z.sun) + Convert.ToDecimal(z.mon) + Convert.ToDecimal(z.tue) + Convert.ToDecimal(z.wed) + Convert.ToDecimal(z.thu) + Convert.ToDecimal(z.fri) + Convert.ToDecimal(z.sat));

                            int grantTotalWeekHours = 0, grantTotalWeekMinute = 0;

                            foreach (var list in spUserWeekReport)
                            {
                                //decimal sum_week_total = list.Sum(z => Convert.ToDecimal(z.sun) + Convert.ToDecimal(z.mon) + Convert.ToDecimal(z.tue) + Convert.ToDecimal(z.wed) + Convert.ToDecimal(z.thu) + Convert.ToDecimal(z.fri) + Convert.ToDecimal(z.sat));
                                //Convert.ToDecimal(list.sun) + Convert.ToDecimal(list.mon) + Convert.ToDecimal(list.tue) + Convert.ToDecimal(list.wed) + Convert.ToDecimal(list.thu) + Convert.ToDecimal(list.fri) + Convert.ToDecimal(list.sat)
                                decimal totalWeekhr = 0;
                                int totalWeekHours = 0, totalWeekMinute = 0;

                                totalWeekHours = (list.sun != null ? Convert.ToInt32(list.sun.ToString().Split('.')[0]): 0 )+  (list.mon != null ? Convert.ToInt32(list.mon.ToString().Split('.')[0]) : 0 ) + ( list.tue != null ? Convert.ToInt32(list.tue.ToString().Split('.')[0]) : 0 ) + (list.wed != null ? Convert.ToInt32(list.wed.ToString().Split('.')[0]) : 0 )+ (list.thu != null ? Convert.ToInt32(list.thu.ToString().Split('.')[0]) : 0 )+ (list.fri != null ? Convert.ToInt32(list.fri.ToString().Split('.')[0]) : 0 )+ (list.sat != null ? Convert.ToInt32(list.sat.ToString().Split('.')[0]):0);
                               // totalWeekMinute = Convert.ToInt32(list.sun.ToString().Split('.')[1]) + Convert.ToInt32(list.mon.ToString().Split('.')[1]) + Convert.ToInt32(list.tue.ToString().Split('.')[1]) + Convert.ToInt32(list.wed.ToString().Split('.')[1]) + Convert.ToInt32(list.thu.ToString().Split('.')[1]) + Convert.ToInt32(list.fri.ToString().Split('.')[1]) + Convert.ToInt32(list.sat.ToString().Split('.')[1]);
                                totalWeekMinute = (list.sun != null ? Convert.ToInt32(list.sun.ToString().Split('.')[1]) : 0) + (list.mon != null ? Convert.ToInt32(list.mon.ToString().Split('.')[1]) : 0) + (list.tue != null ? Convert.ToInt32(list.tue.ToString().Split('.')[1]) : 0) + (list.wed != null ? Convert.ToInt32(list.wed.ToString().Split('.')[1]) : 0) + (list.thu != null ? Convert.ToInt32(list.thu.ToString().Split('.')[1]) : 0 ) + (list.fri != null ? Convert.ToInt32(list.fri.ToString().Split('.')[1]) : 0 ) + (list.sat != null ? Convert.ToInt32(list.sat.ToString().Split('.')[1]) : 0);
                                totalWeekHours += totalWeekMinute / 60;
                                totalWeekMinute = totalWeekMinute % 60;
                                totalWeekhr = Convert.ToDecimal(totalWeekHours.ToString() + "." + totalWeekMinute.ToString());

                                grantTotalWeekHours += totalWeekHours;
                                grantTotalWeekMinute += totalWeekMinute;

                                UserWeekReport.Add(new pmsUserWiseWeekReport
                                {
                                    wkNo = list.wkNo,
                                    week = list.week,
                                    sun = list.sun,
                                    mon = list.mon,
                                    tue = list.tue,
                                    wed = list.wed,
                                    thu = list.thu,
                                    fri = list.fri,
                                    sat = list.sat,
                                   // grantTotal = sum_week_grant_report,
                                    weekTotal = totalWeekhr
                                });
                            }

                            grantTotalWeekHours += grantTotalWeekMinute / 60;
                            grantTotalWeekMinute = grantTotalWeekMinute % 60;
                            sum_week_grant_report = Convert.ToDecimal(grantTotalWeekHours.ToString() + "." + grantTotalWeekMinute.ToString());

                        }

                        var reportUserParam = new SqlParameter("@userId", reportProjectId);
                        var dateParam = new SqlParameter("@commentDate", date);
                        var counterParam = new SqlParameter("@readmoreCounter", counter);
                        var fromDateParam = new SqlParameter("@fromDate", fromDate);
                        var toDateParam = new SqlParameter("@toDate", toDate);

                        //call stored procedure
                        spList = db.Database.SqlQuery<PMSProjectAndUserListViewModel>("usp_getPMSUserwiseReport  @userId,@commentDate,@readmoreCounter, @fromDate, @toDate ", reportUserParam, dateParam, counterParam, fromDateParam, toDateParam).ToList();

                        //date uper group
                        //grup by apply on date
                        var results = from p in spList
                                      group p by p.TodoDate.Date into g
                                      select new { ProjectId = g.Key, group_info = g.ToList() };

                        foreach (var projectInfo in results)
                        {
                            List<projectListUserWiseViewModel> ProjectList = new List<projectListUserWiseViewModel>();
                            //projectwise group
                            var project_wise_group = from g in projectInfo.group_info
                                                     group g by g.ProjectId into g
                                                     select new { projectId = g.Key, projInfo = g.ToList() };

                            foreach (var todoInfo in project_wise_group)
                            {
                                List<UserTodouserWiseViewModel> UserTodouserWiseViewMode = new List<UserTodouserWiseViewModel>();
                                projectTotalWork = 0;
                                projectTotalWorkHours = 0;
                                projectTotalWorkMinute = 0;
                                foreach (var divProj in todoInfo.projInfo)
                                {
                                    projectTotalWorkHours = projectTotalWorkHours + Convert.ToInt32(divProj.ActualHours.ToString().Split('.')[0]);
                                    projectTotalWorkMinute = projectTotalWorkMinute + Convert.ToInt32(divProj.ActualHours.ToString().Split('.')[1]);
                                    UserTodouserWiseViewMode.Add(new UserTodouserWiseViewModel
                                    {
                                        todoId = divProj.TodoId,
                                        todoText = divProj.TodoText,
                                        actualHours = divProj.ActualHours,
                                        commentDate = divProj.TodoDate,
                                        commentText = divProj.CommentText
                                    });
                                }

                                projectTotalWorkHours = projectTotalWorkHours + projectTotalWorkMinute / 60;
                                projectTotalWorkMinute = projectTotalWorkMinute % 60;
                                projectTotalWork = Convert.ToDecimal(projectTotalWorkHours.ToString() + "." + projectTotalWorkMinute.ToString());

                                //porject information add
                                ProjectList.Add(new projectListUserWiseViewModel
                                {
                                    projectId = todoInfo.projectId,
                                    projectName = todoInfo.projInfo.Select(a => a.ProjectName).FirstOrDefault(),
                                    todoList = UserTodouserWiseViewMode,
                                    projectTotalwork = projectTotalWork
                                });
                            }

                            int totalHours = 0, totalMinutes = 0;
                            decimal totalTime = 0;
                            foreach (var item in ProjectList)
                            {
                                totalHours += Convert.ToInt32(item.projectTotalwork.ToString().Split('.')[0]);
                                totalMinutes += Convert.ToInt32(item.projectTotalwork.ToString().Split('.')[1]);
                            }
                            totalHours += totalMinutes / 60;
                            totalMinutes = totalMinutes % 60;
                            totalTime = Convert.ToDecimal(totalHours.ToString() + "." + totalMinutes.ToString());

                            userWiseReportList.Add(new PMSUserWiseReportViewModel
                            {
                                userId = results.Select(z => z.group_info.Select(a => a.ProjectId).FirstOrDefault()).FirstOrDefault(),
                                userName = results.Select(z => z.group_info.Select(a => a.CandidateFirstName + " " + a.CandidateMiddleName + " " + a.CandidateLastName).FirstOrDefault()).FirstOrDefault(),
                                workDate = projectInfo.ProjectId.Date,
                                projectList = ProjectList,
                                userTotalwork = totalTime,
                                userWiseKeekList = UserWeekReport,
                               grantTotal = sum_week_grant_report
                            });
                        }
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", userWiseReportList);
                    }
                    else
                    {
                        //List variable
                        List<PMSProjectAndUserListViewModel> spList = new List<PMSProjectAndUserListViewModel>();
                        List<PMSProjectWiseReportViewModel> projectWiseReportList = new List<PMSProjectWiseReportViewModel>();
                        //List<UserListViewModel> projectWiseUserList = new List<UserListViewModel>();
                        // List<UserTodoViewModel> projectWiseTodoList = new List<UserTodoViewModel>();

                        List<pmsProjectWiseWeekReport> spUserWeekReport = new List<pmsProjectWiseWeekReport>();
                        List<pmsProjectWiseWeekReport> UserWeekReport = new List<pmsProjectWiseWeekReport>();


                        decimal projectTotalWork, sum_week_grant_report = 0;

                        var userProjectFlag = new SqlParameter("@userProjectFlag", LoadType);
                        var userIdParam = new SqlParameter("@userId", reportProjectId);
                        var startDateParam = new SqlParameter("@fromDate", fromDate);
                        var endDateParam = new SqlParameter("@toDate", toDate);

                        if (fromDate.ToString("d/M/yyyy") != "1/1/1900" && toDate.ToString("d/M/yyyy") != "1/1/1900")
                        {
                            spUserWeekReport = db.Database.SqlQuery<pmsProjectWiseWeekReport>("usp_PMSWeekReport @userProjectFlag,@fromdate,@todate,@userid", userProjectFlag, startDateParam, endDateParam, userIdParam).ToList();

                            // sum_week_grant_report = spUserWeekReport.Sum(z => Convert.ToDecimal(z.sun) + Convert.ToDecimal(z.mon) + Convert.ToDecimal(z.tue) + Convert.ToDecimal(z.wed) + Convert.ToDecimal(z.thu) + Convert.ToDecimal(z.fri) + Convert.ToDecimal(z.sat));

                            int grantTotalWeekHours = 0, grantTotalWeekMinute = 0;

                            foreach (var list in spUserWeekReport)
                            {
                                //decimal sum_week_total = list.Sum(z => Convert.ToDecimal(z.sun) + Convert.ToDecimal(z.mon) + Convert.ToDecimal(z.tue) + Convert.ToDecimal(z.wed) + Convert.ToDecimal(z.thu) + Convert.ToDecimal(z.fri) + Convert.ToDecimal(z.sat));
                                //Convert.ToDecimal(list.sun) + Convert.ToDecimal(list.mon) + Convert.ToDecimal(list.tue) + Convert.ToDecimal(list.wed) + Convert.ToDecimal(list.thu) + Convert.ToDecimal(list.fri) + Convert.ToDecimal(list.sat)
                                decimal totalWeekhr = 0;
                                int totalWeekHours = 0, totalWeekMinute = 0;

                                totalWeekHours = (list.sun != null ? Convert.ToInt32(list.sun.ToString().Split('.')[0]) : 0) + (list.mon != null ? Convert.ToInt32(list.mon.ToString().Split('.')[0]) : 0) + (list.tue != null ? Convert.ToInt32(list.tue.ToString().Split('.')[0]) : 0) + (list.wed != null ? Convert.ToInt32(list.wed.ToString().Split('.')[0]) : 0) + (list.thu != null ? Convert.ToInt32(list.thu.ToString().Split('.')[0]) : 0) + (list.fri != null ? Convert.ToInt32(list.fri.ToString().Split('.')[0]) : 0) + (list.sat != null ? Convert.ToInt32(list.sat.ToString().Split('.')[0]) : 0);
                                // totalWeekMinute = Convert.ToInt32(list.sun.ToString().Split('.')[1]) + Convert.ToInt32(list.mon.ToString().Split('.')[1]) + Convert.ToInt32(list.tue.ToString().Split('.')[1]) + Convert.ToInt32(list.wed.ToString().Split('.')[1]) + Convert.ToInt32(list.thu.ToString().Split('.')[1]) + Convert.ToInt32(list.fri.ToString().Split('.')[1]) + Convert.ToInt32(list.sat.ToString().Split('.')[1]);
                                totalWeekMinute = (list.sun != null ? Convert.ToInt32(list.sun.ToString().Split('.')[1]) : 0) + (list.mon != null ? Convert.ToInt32(list.mon.ToString().Split('.')[1]) : 0) + (list.tue != null ? Convert.ToInt32(list.tue.ToString().Split('.')[1]) : 0) + (list.wed != null ? Convert.ToInt32(list.wed.ToString().Split('.')[1]) : 0) + (list.thu != null ? Convert.ToInt32(list.thu.ToString().Split('.')[1]) : 0) + (list.fri != null ? Convert.ToInt32(list.fri.ToString().Split('.')[1]) : 0) + (list.sat != null ? Convert.ToInt32(list.sat.ToString().Split('.')[1]) : 0);
                                totalWeekHours += totalWeekMinute / 60;
                                totalWeekMinute = totalWeekMinute % 60;
                                totalWeekhr = Convert.ToDecimal(totalWeekHours.ToString() + "." + totalWeekMinute.ToString());

                                grantTotalWeekHours += totalWeekHours;
                                grantTotalWeekMinute += totalWeekMinute;

                                UserWeekReport.Add(new pmsProjectWiseWeekReport
                                {
                                    wkNo = list.wkNo,
                                    week = list.week,
                                    sun = list.sun,
                                    mon = list.mon,
                                    tue = list.tue,
                                    wed = list.wed,
                                    thu = list.thu,
                                    fri = list.fri,
                                    sat = list.sat,
                                    // grantTotal = sum_week_grant_report,
                                    weekTotal = totalWeekhr
                                });
                            }

                            grantTotalWeekHours += grantTotalWeekMinute / 60;
                            grantTotalWeekMinute = grantTotalWeekMinute % 60;
                            sum_week_grant_report = Convert.ToDecimal(grantTotalWeekHours.ToString() + "." + grantTotalWeekMinute.ToString());

                        }



                        //SQL stored procedure  parameter Query
                        var reportProjectIdParam = new SqlParameter("@projectId", reportProjectId);
                        var dateParam = new SqlParameter("@commentDate", date);
                        var counterParam = new SqlParameter("@readmoreCounter", counter);
                        var fromDateParam = new SqlParameter("@fromDate", fromDate);
                        var toDateParam = new SqlParameter("@toDate", toDate);


                        decimal userTotalWork;

                        //call stored procedure
                        spList = db.Database.SqlQuery<PMSProjectAndUserListViewModel>("usp_getPMSProjectWiseReport  @projectId, @commentDate, @readmoreCounter, @fromDate, @toDate ", reportProjectIdParam, dateParam, counterParam, fromDateParam, toDateParam).ToList();

                        //grup by apply on date
                        var results = from p in spList
                                      group p by p.TodoDate.Date into g
                                      select new { PersonID = g.Key, group_info = g.ToList() };


                        foreach (var userInfo in results)
                        {
                            List<UserListViewModel> projectWiseUserList = new List<UserListViewModel>();
                            //get all list of recored with same date
                            // var todoDate_list=spList.Where(z=>z.TodoDate.Date == results.Select(a=>a.PersonID.Date).FirstOrDefault()).ToList();
                            // var todoDate_list = spList.Where(z => z.TodoDate.Date == userInfo.group_info.Select(a => a.TodoDate.Date).FirstOrDefault()).ToList();
                            //grup base on user list
                            var user_wise_group = from g in userInfo.group_info
                                                  group g by g.UserId into g
                                                  select new { userId = g.Key, userInfo = g.ToList() };

                            foreach (var todoInfo in user_wise_group)
                            {
                                List<UserTodoViewModel> projectWiseTodoList = new List<UserTodoViewModel>();

                                userTotalWork = 0;
                                userTotalWorkHours = 0;
                                userTotalWorkMinutes = 0;
                                foreach (var divUser in todoInfo.userInfo)
                                {
                                    userTotalWorkHours = userTotalWorkHours + Convert.ToInt32(divUser.ActualHours.ToString().Split('.')[0]);
                                    userTotalWorkMinutes = userTotalWorkMinutes + Convert.ToInt32(divUser.ActualHours.ToString().Split('.')[1]);

                                    projectWiseTodoList.Add(new UserTodoViewModel
                                    {
                                        TodoId = divUser.TodoId,
                                        TodoText = divUser.TodoText,
                                        ActualHours = divUser.ActualHours,
                                        TodoDate = divUser.TodoDate,
                                        CommentText = divUser.CommentText
                                    });
                                }

                                userTotalWorkHours = userTotalWorkHours + userTotalWorkMinutes / 60;
                                userTotalWorkMinutes = userTotalWorkMinutes % 60;
                                userTotalWork = Convert.ToDecimal(userTotalWorkHours.ToString() + "." + userTotalWorkMinutes.ToString());

                                //User Information added 
                                projectWiseUserList.Add(new UserListViewModel
                                {
                                    //UserId = user_wise_group.Select(z => z.userId).FirstOrDefault(),
                                    UserId = todoInfo.userId,
                                    UserName = todoInfo.userInfo.Select(a => a.CandidateFirstName + "" + (a.CandidateMiddleName.Length > 1 ? a.CandidateMiddleName.Substring(0, 1) : a.CandidateMiddleName) + "" + (a.CandidateLastName.Length > 1 ? a.CandidateLastName.Substring(0, 1) : a.CandidateLastName)).FirstOrDefault(),
                                    //UserName = user_wise_group.Select(z => z.userInfo.Select(a => a.CandidateFirstName + " " + a.CandidateMiddleName + " " + a.CandidateLastName).FirstOrDefault()).FirstOrDefault(),
                                    TodoList = projectWiseTodoList,
                                    userTotalwork = userTotalWork
                                });

                            }

                            int totalHours = 0, totalMinutes = 0;
                            decimal totalTime = 0;
                            foreach (var item in projectWiseUserList)
                            {
                                totalHours += Convert.ToInt32(item.userTotalwork.ToString().Split('.')[0]);
                                totalMinutes += Convert.ToInt32(item.userTotalwork.ToString().Split('.')[1]);
                            }
                            totalHours += totalMinutes / 60;
                            totalMinutes = totalMinutes % 60;
                            totalTime = Convert.ToDecimal(totalHours.ToString() + "." + totalMinutes.ToString());

                            projectWiseReportList.Add(new PMSProjectWiseReportViewModel
                            {
                                ProjectId = results.Select(z => z.group_info.Select(a => a.ProjectId).FirstOrDefault()).FirstOrDefault(),
                                ProjectName = results.Select(z => z.group_info.Select(a => a.ProjectName).FirstOrDefault()).FirstOrDefault(),
                                UserList = projectWiseUserList,
                                // workDate = results.Select(a => a.PersonID).FirstOrDefault()
                                workDate = userInfo.PersonID.Date,
                                projectTotalwork = totalTime,
                                userWiseKeekList = UserWeekReport,
                                grantTotal = sum_week_grant_report
                            });

                        }

                        //project wise report list
                        //projectWiseReportList.Add(new PMSProjectWiseReportViewModel
                        // {
                        //     ProjectId=results.Select(z=>z.group_info.Select(a=>a.ProjectId).FirstOrDefault()).FirstOrDefault(),
                        //     ProjectName=results.Select(z=>z.group_info.Select(a=>a.ProjectName).FirstOrDefault()).FirstOrDefault(),
                        //     UserList = projectWiseUserList,
                        //     workDate=results.Select(a=>a.PersonID).FirstOrDefault()
                        // });
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", projectWiseReportList);
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
