using ERP.Models;
using ERP.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web;
using System.Web.Http;

namespace ERP.WebApis
{
    public class PMSProjectController : ApiController
    {
        private ERPContext db = new ERPContext();
        SessionUtils sessionUtils = new SessionUtils();
        GeneralMessages generalMessages = null;
        string _pageName = "PMS Project";
        // private List<string> lstStaticPages = new List<string>();
        public List<SelectItemModel> ProjectTypeList = new List<SelectItemModel>();

        public PMSProjectController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);

            List<string> getProjType = ConfigurationManager.AppSettings["ProjectType"].ToString().Split(',').ToList<string>();
            foreach (var page in getProjType)
            {
                List<string> temp_projectList = page.ToString().Split(':').ToList<string>();

                ProjectTypeList.Add(new SelectItemModel
                {
                    Id = Convert.ToInt32(temp_projectList[1].ToString()),
                    Label = temp_projectList[0].ToString()
                });
            }
        }

        //GET TECHNOLOGY INFORMATION
        [HttpGet]
        public ApiResponse GetTechnologies()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var list = db.tblTechnologies.OrderBy(z => z.Technologies)
                       .Where(z => z.IsActive == true)
                       .Select(z => new SelectItemModel
                       {
                           Id = z.Id,
                           Label = z.Technologies
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

        //GET TECHNOLOGY GROUP INFORMATION
        [HttpGet]
        public ApiResponse GetTechnologiesGroup()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var list = db.tblTechnologiesGroups.OrderBy(z => z.TechnologiesGroup)
                       .Where(z => z.IsActive == true)
                       .Select(z => new SelectItemModel
                       {
                           Id = z.Id,
                           Label = z.TechnologiesGroup
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




        [HttpGet]
        public ApiResponse GetProjectTypeList()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", ProjectTypeList);
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
        /// GET api/PMSProject
        /// retrieve status list
        /// </summary>
        [HttpGet]
        public ApiResponse GetStatusList()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    List<SelectItemModel> list = new List<SelectItemModel>();

                    foreach (int value in Enum.GetValues(typeof(ERPUtilities.PMSStatus)))
                    {
                        list.Add(new SelectItemModel
                        {
                            Id = value,
                            Label = Enum.GetName(typeof(ERPUtilities.PMSStatus), value)
                        });
                    }

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
        /// GET api/PMSProject
        /// retrieve TL list
        /// </summary>
        [HttpGet]
        public ApiResponse GetTLList()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var list = db.tblEmpCompanyInformations.Where(z => z.IsActive == true && z.IsTL == true).ToList();
                    List<SelectItemModel> data = new List<SelectItemModel>();
                    foreach (var l in list)
                    {
                        var line = db.tblEmpPersonalInformations.Where(z => z.EmployeeId == l.EmployeeId).SingleOrDefault();
                        data.Add(new SelectItemModel
                        {
                            Id = l.EmployeeId,
                            Label = string.Format("{0}{1}{2}", line.CandidateFirstName, line.CandidateMiddleName.Substring(0, 1).ToUpper(), line.CandidateLastName.Substring(0, 1).ToUpper())
                        });
                    }
                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", data.OrderBy(z => z.Label).ToList());
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
        /// GET api/PMSProject
        /// retrieve user list on selection of TL
        /// </summary>
        [HttpGet]
        public ApiResponse GetUserList()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int employeeId = Convert.ToInt32(nvc["leadId"].ToString());
                    List<tblEmpCompanyInformation> cList = new List<tblEmpCompanyInformation>();
                    List<SelectItemAccessRights> eList = new List<SelectItemAccessRights>();

                    EmployeeUtils employeeUtils = new EmployeeUtils();

                    //cList = ERPUtilities.DrillDownReporting(employeeId.ToString(), cList);
                    cList = employeeUtils.DrillDownReporting(employeeId.ToString(), cList);


                    foreach (var l in cList)
                    {
                        var line = db.tblEmpPersonalInformations.Where(z => z.EmployeeId == l.EmployeeId).SingleOrDefault();
                        if (line.IsActive)
                        {
                            eList.Add(new SelectItemAccessRights
                            {
                                Id = l.EmployeeId,
                                Label = string.Format("{0}{1}{2}", line.CandidateFirstName, line.CandidateMiddleName.Substring(0, 1).ToUpper(), line.CandidateLastName.Substring(0, 1).ToUpper()),
                                IsSelected = false
                            });
                        }
                    }
                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", eList.OrderBy(z => z.Label).ToList());
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
        /// POST api/PMSProject
        /// create and update record
        /// </summary>
        [HttpPost]
        public ApiResponse SaveProject(PMSProjectViewModel prj)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    DateTime dtStart = new DateTime();
                    DateTime dtEnd = new DateTime();
                    //  string[] sDT = Convert.ToDateTime(prj.StartDate).ToString("dd-MM-yyyy").Split('-');
                    //   string[] eDT = Convert.ToDateTime(prj.EndDate).ToString("dd-MM-yyyy").Split('-');

                    string[] sDT = prj.StartDate.Split('-');
                    string[] eDT = prj.EndDate.Split('-');

                    dtStart = new DateTime(Convert.ToInt32(sDT[2]), Convert.ToInt32(sDT[1]), Convert.ToInt32(sDT[0]));
                    dtEnd = new DateTime(Convert.ToInt32(eDT[2]), Convert.ToInt32(eDT[1]), Convert.ToInt32(eDT[0]));

                    if (prj.ProjectId == 0)
                    {// Mode == Add
                        tblPMSProject d = new tblPMSProject
                        {
                            ProjectName = prj.ProjectName,
                            TechnologiesId = prj.Technologies,
                            ProjectType = prj.ProjectType,
                            StartDate = dtStart,
                            EndDate = dtEnd,
                            TotalEstDays = prj.TotalEstDays,
                            Status = prj.IsArchived == true ? 4 : prj.Status,
                            Description = prj.Description,
                            IsArchived = prj.IsArchived,
                            CreDate = DateTime.Now.ToUniversalTime(),
                            CreBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString()),
                            ChgDate = DateTime.Now.ToUniversalTime(),
                            ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString())
                        };
                        db.tblPMSProjects.Add(d);
                        db.SaveChanges();
                        int pid = db.tblPMSProjects.Max(z => z.ProjectId);
                        var line = db.tblPMSProjects.Where(z => z.ProjectId == pid).SingleOrDefault();
                        InsertActivityLogProject(line, "Insert"); // insert in activity log



                        String moduleName;
                        for (int i = 1; i <= 2; i++)
                        {
                            if (i == 1)
                            {
                                moduleName = "Bug Fix";
                            }
                            else
                            {
                                moduleName = "Change Request";
                            }
                            //default module 
                            int prio;
                            try
                            {
                                prio = db.tblPMSModules.Max(z => z.Priority + 1);
                            }
                            catch
                            {
                                prio = 1;
                            }

                            //module type: Bug = 1, CR = 2
                            tblPMSModule defultModule = new tblPMSModule
                            {
                                ProjectId = pid,
                                ModuleName = moduleName,
                                ModuleType = i,
                                Priority = prio,
                                IsArchived = false,
                                IsActive = true,
                                CreDate = DateTime.Now.ToUniversalTime(),
                                CreBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString()),
                                ChgDate = DateTime.Now.ToUniversalTime(),
                                ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString())
                            };
                            db.tblPMSModules.Add(defultModule);
                            db.SaveChanges();
                        }

                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, pid); // return project id
                    }
                    else
                    {// Mode == Edit
                        // first take old entry and put it in activity log
                        var line = db.tblPMSProjects.Where(z => z.ProjectId == prj.ProjectId).SingleOrDefault();
                        if (line != null)
                        {
                            InsertActivityLogProject(line, "Update");
                            // update old entry with new data in main project table
                            line.ProjectName = prj.ProjectName;
                            line.TechnologiesId = prj.Technologies;
                            line.ProjectType = prj.ProjectType;
                            line.StartDate = dtStart;
                            line.EndDate = dtEnd;
                            line.TotalEstDays = prj.TotalEstDays;
                            line.Status = prj.IsArchived == true ? 4 : prj.Status;
                            line.Description = prj.Description;
                            line.IsArchived = prj.IsArchived;
                            line.ChgDate = DateTime.Now.ToUniversalTime();
                            line.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                        }
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgUpdate, prj.ProjectId);
                        db.SaveChanges();
                        DeleteProjectUser(prj.ProjectId); //used to insert new entry of users each time
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

        private void InsertActivityLogProject(tblPMSProject line, string Action)
        {
            tblPMSActivityLogProject d = new tblPMSActivityLogProject
            {
                ProjectId = line.ProjectId,
                ProjectName = line.ProjectName,
                StartDate = line.StartDate,
                EndDate = line.EndDate,
                TotalEstDays = line.TotalEstDays,
                Status = line.Status,
                Description = line.Description,
                IsArchived = line.IsArchived,
                DBAction = Action,
                CreDate = DateTime.Now.ToUniversalTime(),
                CreBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString())
            };
            db.tblPMSActivityLogProjects.Add(d);
            db.SaveChanges();
        }

        private void DeleteProjectUser(int projectId)
        {
            var list = db.tblPMSProjectUsers.Where(z => z.ProjectId == projectId).ToList();
            foreach (var l in list)
            {
                db.tblPMSProjectUsers.Remove(l);
            }
            db.SaveChanges();
        }
        /// <summary>
        /// POST api/PMSProject
        /// create and update for project users
        /// </summary>
        [HttpPost]
        public ApiResponse SaveProjectUsers(tblPMSProjectUser prj)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    //if (db.tblPMSProjectUsers.Where(z => z.ProjectId == prj.ProjectId && z.EmployeeId == prj.EmployeeId && z.IsTL == prj.IsTL).Count() == 0) 
                    //{// Insert When : entry not exists
                    tblPMSProjectUser d = new tblPMSProjectUser
                    {
                        ProjectId = prj.ProjectId,
                        EmployeeId = prj.EmployeeId,
                        IsTL = prj.IsTL,
                        UserUnder = prj.UserUnder,
                        CreDate = DateTime.Now.ToUniversalTime(),
                        CreBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString()),
                        ChgDate = DateTime.Now.ToUniversalTime(),
                        ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString())
                    };
                    db.tblPMSProjectUsers.Add(d);
                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);
                    db.SaveChanges();
                    //}
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


        protected List<tblPMSProject> ReturnFilteredProjectList(string filterData)
        {
            List<tblPMSProject> projects = new List<tblPMSProject>();
            if (!string.IsNullOrEmpty(filterData))
            {
                foreach (var statusId in filterData.Split(','))
                {
                    if (!string.IsNullOrEmpty(statusId))
                    {
                        var list = db.tblPMSProjects.AsEnumerable().Where(z => z.Status == Convert.ToInt32(statusId)).ToList();
                        projects.AddRange(list);
                    }
                }
            }
            else
            { //no filter data + default: don't display archived record
                projects = db.tblPMSProjects.Where(z => z.Status == 1 && z.IsArchived == false).ToList();
            }

            return projects.ToList();
        }

        /// <summary>
        /// GET api/PMSProject
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
                    List<PMSProjectViewModel> pList = new List<PMSProjectViewModel>();
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int timezone = Convert.ToInt32(nvc["timezone"].ToString());
                    int loginId = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                    //int projTechnology = string.IsNullOrEmpty(nvc["technology"].ToString()) ? 0 : Convert.ToInt32(nvc["technology"].ToString());
                    string projTechnology = string.IsNullOrEmpty(nvc["technology"].ToString()) ? null : Convert.ToString(nvc["technology"].ToString());
                    int projectType = string.IsNullOrEmpty(nvc["projectType"].ToString()) ? 0 : Convert.ToInt32(nvc["projectType"].ToString());
                    int filterData = string.IsNullOrEmpty(nvc["filterData"].ToString()) ? 0 : Convert.ToInt32(nvc["filterData"].ToString());
                    int filterUserList = string.IsNullOrEmpty(nvc["filterUserList"].ToString()) ? 0 : Convert.ToInt32(nvc["filterUserList"].ToString());

                    List<PMSProjectSpViewModel> spList = new List<PMSProjectSpViewModel>();
                    List<int> temp_ProjectTechnology = new List<int>();

                    if (!string.IsNullOrEmpty(projTechnology))
                    {
                        string[] words = projTechnology.Split(',');
                        foreach (var word in words)
                        {
                            var id=Convert.ToInt32(word);
                            var techList = db.tblTechnologies.Where(z => z.TechnologiesGroupId == id).Select(z => z.Id).ToList();
                            foreach(var j in techList)
                            {
                                temp_ProjectTechnology.Add(j);
                            }
                        }
                    }
                    temp_ProjectTechnology = temp_ProjectTechnology.Distinct().ToList();
                    projTechnology = string.Join(",", temp_ProjectTechnology);


                    var EmployeeId = new SqlParameter("@EmployeeId", loginId);
                    //check project tech no is null or not
                    //var TechnologiesId = projTechnology != 0 ? new SqlParameter("@TechnologiesId", projTechnology) : new SqlParameter("@TechnologiesId", DBNull.Value);
                    var TechnologiesId = (!string.IsNullOrEmpty(projTechnology)) ? new SqlParameter("@TechnologiesId", projTechnology) : new SqlParameter("@TechnologiesId", DBNull.Value);
                    var ProjectType = projectType != 0 ? new SqlParameter("@ProjectType", projectType) : new SqlParameter("@ProjectType", DBNull.Value);
                    var ProjectStatus = filterData != 0 ? new SqlParameter("@ProjectStatus", filterData) : new SqlParameter("@ProjectStatus", DBNull.Value);
                    var FilterByUser = filterUserList != 0 ? new SqlParameter("@FilterUserList", filterUserList) : new SqlParameter("@FilterUserList", DBNull.Value);

                    spList = db.Database.SqlQuery<PMSProjectSpViewModel>("usp_getProjectList  @EmployeeId,@TechnologiesId,@ProjectType, @ProjectStatus,@FilterUserList", EmployeeId, TechnologiesId, ProjectType, ProjectStatus, FilterByUser).ToList();


                    foreach (var list in spList)
                    {
                        List<PMSProjectsers> uList = new List<PMSProjectsers>();
                        var users = db.tblPMSProjectUsers.Where(z => z.ProjectId == list.ProjectId).ToList();

                        if (users != null)
                        {
                            var TLs = users.Where(z => z.IsTL == true).ToList();
                            foreach (var tl in TLs)
                            {
                                List<SelectedUsers> sList = new List<SelectedUsers>();
                                var userlist = users.Where(z => z.UserUnder == tl.EmployeeId).ToList();
                                foreach (var u in userlist)
                                { // User list of TL
                                    var line = db.tblEmpPersonalInformations.Where(z => z.EmployeeId == u.EmployeeId).SingleOrDefault();
                                    sList.Add(new SelectedUsers
                                    {
                                        Id = u.EmployeeId,
                                        Label = string.Format("{0}{1}{2}", line.CandidateFirstName, line.CandidateMiddleName.Substring(0, 1).ToUpper(), line.CandidateLastName.Substring(0, 1).ToUpper()),
                                        IsSelected = true,
                                        ProfilePix = string.IsNullOrEmpty(line.ProfilePhoto)
                                                        ? string.Format("{0}/{1}", "Content/images", "thumb_User.png")
                                                        : string.Format("{0}/{1}", ConfigurationManager.AppSettings["UploadPath"].ToString(), line.ProfilePhoto)
                                    });
                                }

                                var lead = db.tblEmpPersonalInformations.Where(z => z.EmployeeId == tl.EmployeeId).SingleOrDefault();
                                uList.Add(new PMSProjectsers
                                {
                                    LeadId = tl.EmployeeId,
                                    LeadName = string.Format("{0}{1}{2}", lead.CandidateFirstName, lead.CandidateMiddleName.Substring(0, 1).ToUpper(), lead.CandidateLastName.Substring(0, 1).ToUpper()),
                                    SelectedUsers = sList.OrderBy(z => z.Label).ToList(),
                                    ProfilePix = string.IsNullOrEmpty(lead.ProfilePhoto)
                                                        ? string.Format("{0}/{1}", "Content/images", "thumb_User.png")
                                                        : string.Format("{0}/{1}", ConfigurationManager.AppSettings["UploadPath"].ToString(), lead.ProfilePhoto)
                                });
                            }
                        }

                        pList.Add(new PMSProjectViewModel
                        {
                            ProjectId = list.ProjectId,
                            ProjectName = list.ProjectName,
                            Technologies = list.TechnologyId,
                            TechnologyName = list.Technologies,
                            ProjectType = list.ProjectType,
                            StartDate = Convert.ToDateTime(list.StartDate).ToString("dd-MM-yyyy"),
                            EndDate = Convert.ToDateTime(list.EndDate).ToString("dd-MM-yyyy"),
                            TotalEstDays = list.TotalEstDays,
                            Status = list.Status,
                            Description = list.Description,
                            IsArchived = list.IsArchived,
                            ChgDate = list.ChgDate, // need to convert timezone
                            SelectedUsers = uList.OrderBy(z => z.LeadName).ToList(),
                            AssignedHours = list.EHours,
                            Hours = list.WHours,
                            totalTaskCount = list.NoOfToDo,
                            finishTaskCount = list.CNoOfToDo
                        });
                    }

                    pList = pList.Select(i =>
                    {
                        i.ChgDate = Convert.ToDateTime(i.ChgDate).AddMinutes(-1 * timezone);
                        return i;
                    }).ToList();

                    var temp_pList = pList.OrderBy(z => z.ProjectType).GroupBy(z => z.ProjectType).ToList();

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", temp_pList);
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
        /// GET api/PMSProject
        /// Send mail on assigned user to project
        /// </summary>
        [HttpPost]
        public ApiResponse SendMail(PMSProjectViewModel projData)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int projectId = Convert.ToInt32(nvc["projectId"]);
                    int loginId = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                    string mode = nvc["mode"];
                    string sendSubject = string.Empty;

                    var uList = db.tblPMSProjectUsers.Where(z => z.ProjectId == projectId).Select(z => new { EmployeeId = z.EmployeeId }).Distinct().ToList();
                    var project = db.tblPMSProjects.Where(z => z.ProjectId == projectId).SingleOrDefault();

                    //start mail template tmplProjectCreateUpdate
                    string body = string.Empty;
                    var emp = db.tblEmpPersonalInformations.Where(z => z.EmployeeId == project.CreBy).SingleOrDefault();
                    var tech = db.tblTechnologies.Where(z => z.Id == Convert.ToInt16(project.TechnologiesId)).Select(z => z.Technologies).FirstOrDefault();

                    using (var sr = new StreamReader(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["tmplProjectCreateUpdate"].ToString())))
                    {
                        body = sr.ReadToEnd();
                        if (mode == "Add")
                        {

                            List<int> tlList = new List<int>();
                            tlList = db.tblPMSProjectUsers.Where(z => z.IsTL == true && z.ProjectId == projectId).Select(z => z.EmployeeId).ToList();
                            string addTeamLeader = "<table><tr><td><ul>";
                            foreach (var item in tlList)
                            {
                                var tlName = db.tblEmpPersonalInformations.Where(z => z.EmployeeId == item).Select(z => z.CandidateFirstName + (z.CandidateMiddleName.Length > 1 ? z.CandidateMiddleName.Substring(0, 1) : z.CandidateMiddleName) + (z.CandidateLastName.Length > 1 ? z.CandidateLastName.Substring(0, 1) : z.CandidateLastName)).FirstOrDefault();
                                addTeamLeader = string.Concat(addTeamLeader, "<li>" + tlName + "(Team Leader)</li>");

                                var userlist = db.tblPMSProjectUsers.Where(z => z.IsTL == false && z.ProjectId == projectId && z.UserUnder == item).ToList();

                                string addUserList = "<ol>";
                                foreach (var ulist in userlist)
                                {
                                    var userName = db.tblEmpPersonalInformations.Where(z => z.EmployeeId == ulist.EmployeeId).Select(z => z.CandidateFirstName + (z.CandidateMiddleName.Length > 1 ? z.CandidateMiddleName.Substring(0, 1) : z.CandidateMiddleName) + (z.CandidateLastName.Length > 1 ? z.CandidateLastName.Substring(0, 1) : z.CandidateLastName)).FirstOrDefault();
                                    addUserList = string.Concat(addUserList, "<li>" + userName + "</li>");
                                }
                                addUserList = string.Concat(addUserList, "</ol>");
                                addTeamLeader = string.Concat(addTeamLeader, addUserList);

                            }
                            addTeamLeader = string.Concat(addTeamLeader, "</ul></td></tr></table>");
                            string username = "<table><tr><td>" + addTeamLeader + "</td></tr></table>";

                            String projType = string.Empty;
                            if (project.ProjectType == 1)
                            {
                                projType = "Dedicated";
                            }
                            else if (project.ProjectType == 2)
                            {
                                projType = "Hourly Base";
                            }
                            else
                            {
                                projType = "R&D";
                            }

                            body = body.Replace("##CreateUpdate##", "<b style='color:#8CC051;font-size: 17px;'> Project has been created successfully</b>");
                            body = body.Replace("##ProjectDetail##", "<table> <tr><td><strong>Project Name:</strong> " + project.ProjectName + "</td></tr><tr><td><strong>Technologies:</strong> " + tech + "</td></tr><tr><td><strong>Project Type:</strong> " + projType + "</td></tr><tr><td><strong>Start Date:</strong> " + project.StartDate.ToShortDateString() + "</td></tr><tr><td><strong>End Date:</strong> " + project.EndDate.ToShortDateString() + "</td></tr><tr><td><strong>Total Estimated Day:</strong> " + project.TotalEstDays + "</td></tr><tr><td><strong>Status:</strong> " + Enum.GetName(typeof(ERPUtilities.PMSStatus), project.Status) + "</td></tr> <tr><td><strong>Description:</strong> " + project.Description + "</td></tr><tr><td><strong>Project User:</strong><ul> " + username + "</ul></td></tr><tr><td><strong>Create By:</strong> " + emp.CandidateFirstName + (emp.CandidateMiddleName.Length > 1 ? emp.CandidateMiddleName.Substring(0, 1) : emp.CandidateMiddleName) + (emp.CandidateLastName.Length > 1 ? emp.CandidateLastName.Substring(0, 1) : emp.CandidateLastName) + "</td></tr></table>");
                        }
                        else
                        {
                            bool isChange = false;
                            string table = "<table>";
                            var newProjectData = db.tblPMSProjects.Where(z => z.ProjectId == projectId).FirstOrDefault();
                            table = string.Concat(table, "<tr><td><strong>Project Name:</strong> " + project.ProjectName + "</td></tr><tr>");

                            if (newProjectData.TechnologiesId != projData.Technologies)
                            {
                                table = string.Concat(table, "<tr><td><strong>Technologies:</strong> " + tech + "</td></tr>");
                                isChange = true;
                            }
                            if (newProjectData.ProjectType != projData.ProjectType)
                            {
                                String projType = string.Empty;
                                if (project.ProjectType == 1)
                                {
                                    projType = "Dedicated";
                                }
                                else if (project.ProjectType == 2)
                                {
                                    projType = "Hourly Base";
                                }
                                else
                                {
                                    projType = "R&D";
                                }
                                table = string.Concat(table, "<tr><td><strong>Project Type:</strong> " + projType + "</td></tr>");
                                isChange = true;
                            }
                            int compStratDate = DateTime.Compare(newProjectData.StartDate, Convert.ToDateTime(projData.StartDate));
                            if (compStratDate != 0)
                            {
                                table = string.Concat(table, "<tr><td><strong>Start Date:</strong> " + project.StartDate.ToShortDateString() + "</td></tr>");
                                isChange = true;
                            }
                            int compEndDate = DateTime.Compare(newProjectData.EndDate, Convert.ToDateTime(projData.EndDate));
                            if (compEndDate != 0)
                            {
                                table = string.Concat(table, "<tr><td><strong>End Date:</strong> " + project.EndDate.ToShortDateString() + "</td></tr>");
                                isChange = true;
                            }
                            if (newProjectData.TotalEstDays != projData.TotalEstDays)
                            {
                                table = string.Concat(table, "<tr><td><strong>Total Estimated Day:</strong> " + project.TotalEstDays + "</td></tr>");
                                isChange = true;
                            }
                            if (newProjectData.Status != projData.Status)
                            {
                                table = string.Concat(table, "<tr><td><strong>Status:</strong> " + Enum.GetName(typeof(ERPUtilities.PMSStatus), project.Status) + "</td></tr>");
                                isChange = true;
                            }

                            if (newProjectData.Description != projData.Description)
                            {
                                table = string.Concat(table, "<tr><td><strong>Description:</strong> " + project.Description + "</td></tr>");
                                isChange = true;
                            }

                            List<int> oldTlUserList = new List<int>();
                            List<int> newTlUserList = new List<int>();
                            oldTlUserList = projData.SelectedUsers.Select(z => z.LeadId).ToList();
                            newTlUserList = db.tblPMSProjectUsers.Where(z => z.IsTL == true && z.ProjectId == projectId).Select(z => z.EmployeeId).ToList();
                            List<int> insertTlId = newTlUserList.Except(oldTlUserList).ToList();
                            List<int> deleteTlId = oldTlUserList.Except(newTlUserList).ToList();

                            if (insertTlId.Count > 0)
                            {
                                string addTeamLeader = "<table><tr><td><ul>";
                                foreach (var insertIL in insertTlId)
                                {
                                    var name = db.tblEmpPersonalInformations.Where(z => z.EmployeeId == insertIL).Select(z => z.CandidateFirstName + (z.CandidateMiddleName.Length > 1 ? z.CandidateMiddleName.Substring(0, 1) : z.CandidateMiddleName) + (z.CandidateLastName.Length > 1 ? z.CandidateLastName.Substring(0, 1) : z.CandidateLastName)).FirstOrDefault();
                                    addTeamLeader = string.Concat(addTeamLeader, "<li>" + name + "(Team Leader)</li>");

                                    var userlist = db.tblPMSProjectUsers.Where(z => z.IsTL == false && z.ProjectId == projectId && z.UserUnder == insertIL).ToList();

                                    string addUserList = "<ol>";
                                    foreach (var ulist in userlist)
                                    {
                                        var userName = db.tblEmpPersonalInformations.Where(z => z.EmployeeId == ulist.EmployeeId).Select(z => z.CandidateFirstName + (z.CandidateMiddleName.Length > 1 ? z.CandidateMiddleName.Substring(0, 1) : z.CandidateMiddleName) + (z.CandidateLastName.Length > 1 ? z.CandidateLastName.Substring(0, 1) : z.CandidateLastName)).FirstOrDefault();
                                        addUserList = string.Concat(addUserList, "<li>" + userName + "</li>");
                                    }
                                    addUserList = string.Concat(addUserList, "</ol>");
                                    addTeamLeader = string.Concat(addTeamLeader, addUserList);
                                }
                                addTeamLeader = string.Concat(addTeamLeader, "</ul></td></tr></table>");
                                table = string.Concat(table, "<tr><td><strong>Insert Team Leader:</strong> " + addTeamLeader + "</td></tr>");
                                isChange = true;
                            }
                            if (deleteTlId.Count > 0)
                            {
                                string deleteTeamLeader = "<table><tr><td><ul>";
                                foreach (var deleteIL in deleteTlId)
                                {
                                    var name = db.tblEmpPersonalInformations.Where(z => z.EmployeeId == deleteIL).Select(z => z.CandidateFirstName + (z.CandidateMiddleName.Length > 1 ? z.CandidateMiddleName.Substring(0, 1) : z.CandidateMiddleName) + (z.CandidateLastName.Length > 1 ? z.CandidateLastName.Substring(0, 1) : z.CandidateLastName)).FirstOrDefault();
                                    deleteTeamLeader = string.Concat(deleteTeamLeader, "<li>" + name + "(Team Leader)</li>");
                                    isChange = true;
                                }
                                deleteTeamLeader = string.Concat(deleteTeamLeader, "</ul></td></tr></table>");
                                table = string.Concat(table, "<tr><td><strong>delete Team Leader:</strong> " + deleteTeamLeader + "</td></tr>");
                            }
                            if (isChange)
                            {
                                table = string.Concat(table, "</table>");
                                body = body.Replace("##ProjectDetail##", table);
                                body = body.Replace("##CreateUpdate##", "<b style='color: #008000;font-size: 17px;'> Project has been updated successfully</b>");
                            }
                        }

                        //body = body.Replace("##ProjectName##", project.ProjectName);
                        //body = body.Replace("##Description##", project.Description);
                        // body = body.Replace("##EmployeeName##", emp.CandidateFirstName + " " + emp.CandidateLastName);
                        var staticImagesPath = ConfigurationManager.AppSettings["StaticImagesPath"].ToString();
                        var textureImage = "daimond_eyes.png";
                        body = body.Replace("##bgTexturePath##", staticImagesPath + textureImage);
                    }

                    if (mode == "Add")
                    {
                        sendSubject = "Project is created";
                    }
                    else
                    {
                        sendSubject = "Project is updated";
                    }

                    //end
                    //string body = string.Empty;
                    //if (mode == "Add")
                    //{
                    //    var emp = db.tblEmpPersonalInformations.Where(z => z.EmployeeId == project.CreBy).SingleOrDefault();
                    // body = string.Format("{0}<br/>Project Name: {1}<br/>Description: {2}<br/>Created by: {3}", generalMessages.msgUpdate, project.ProjectName, project.Description, emp.CandidateFirstName);
                    //    sendSubject = "Project is created";
                    //}
                    //else
                    //{
                    //    var emp = db.tblEmpPersonalInformations.Where(z => z.EmployeeId == project.ChgBy).SingleOrDefault();
                    //    body = string.Format("{0}<br/>Project Name: {1}<br/>Description: {2}<br/>Changed by: {3}", generalMessages.msgUpdate, project.ProjectName, project.Description, emp.CandidateFirstName);
                    //    sendSubject = "Project is updated";
                    //}

                    MailAddress mFrom = new MailAddress(ConfigurationManager.AppSettings["fromMail"].ToString());
                    var loginEmail = db.tblEmpPersonalInformations.Where(z => z.EmployeeId == loginId).Select(z => z.CompanyEmailId).SingleOrDefault();
                    MailAddress mTo = new MailAddress(loginEmail);
                    System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage(mFrom, mTo);
                    message.Subject = sendSubject;
                    message.Body = body;

                    foreach (var l in uList)
                    {
                        if (l.EmployeeId != loginId)
                        { //Avoid duplicate entry
                            var email = db.tblEmpPersonalInformations.Where(z => z.EmployeeId == l.EmployeeId).Select(z => z.CompanyEmailId).SingleOrDefault();
                            if (!string.IsNullOrEmpty(email))
                            {
                                MailAddress ccEmail = new MailAddress(email);
                                message.CC.Add(ccEmail);
                            }
                        }
                    }

                    message.IsBodyHtml = true;
                    message.BodyEncoding = System.Text.Encoding.UTF8;

                    SmtpClient client = new SmtpClient(ConfigurationManager.AppSettings["smtp"].ToString());
                    client.EnableSsl = false;
                    client.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["mail"].ToString(), ConfigurationManager.AppSettings["Password"].ToString());
                    client.Send(message);
                    message.Dispose();
                    client.Dispose();

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", true);
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
