using ERP.Models;
using ERP.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web;
using System.Web.Http;

namespace ERP.WebApis
{
    public class PMSTodoCommentController : ApiController
    {
        private ERPContext db = new ERPContext();
        SessionUtils sessionUtils = new SessionUtils();
        GeneralMessages generalMessages = null;
        string _pageName = "Todo Comment";
        EmployeeUtils employeeUtils;

        public PMSTodoCommentController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
            employeeUtils = new EmployeeUtils();
        }

        /// <summary>
        /// GET api/PMSModule
        /// Retrieve assigned project user list
        /// </summary>
        [HttpGet]
        public ApiResponse GetAssignedUserList()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    List<SelectedUsers> sList = new List<SelectedUsers>();
                    int todoId = Convert.ToInt32(nvc["todoId"].ToString());
                    int loginId = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                    int moduleId = db.tblPMSToDoes.Where(z => z.TodoId == todoId).Select(z => z.ModuleId).SingleOrDefault();
                    int projectId = db.tblPMSModules.Where(z => z.ModuleId == moduleId).Select(z => z.ProjectId).SingleOrDefault();

                    var list = db.tblPMSProjectUsers.Where(z => z.ProjectId == projectId).Select(z => new { EmployeeId = z.EmployeeId }).Distinct().ToList();
                    foreach (var l in list)
                    {
                        if (l.EmployeeId != loginId)
                        { //don't take logged in user
                            var emp = db.tblEmpPersonalInformations.Where(z => z.EmployeeId == l.EmployeeId).FirstOrDefault();
                            if (emp != null)
                            {
                                sList.Add(new SelectedUsers
                                {
                                    Id = emp.EmployeeId,
                                    Label = string.Format("{0} {1} {2}", emp.CandidateFirstName, emp.CandidateMiddleName, emp.CandidateLastName),
                                    IsSelected = true
                                });
                            }
                        }
                    }

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", sList);
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
        /// GET api/PMSModule
        /// Retrieve project id
        /// </summary>
        [HttpGet]
        public ApiResponse GetProjectId()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    List<SelectedUsers> sList = new List<SelectedUsers>();
                    int todoId = Convert.ToInt32(nvc["todoId"].ToString());
                    int moduleId = db.tblPMSToDoes.Where(z => z.TodoId == todoId).Select(z => z.ModuleId).SingleOrDefault();
                    int projectId = db.tblPMSModules.Where(z => z.ModuleId == moduleId).Select(z => z.ProjectId).SingleOrDefault();

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", projectId);
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
        /// Get Todo Status
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiResponse GetTodoStatus()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int todoId = Convert.ToInt32(nvc["todoId"].ToString());
                    var todoStatus = db.tblPMSToDoes.Where(z => z.TodoId == todoId).Select(z => new { z.Status, z.IsArchived }).ToList();
                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", todoStatus);
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
        /// Get List of Status Available While Adding New ToDo Comment
        /// </summary>
        /// <returns></returns>
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

        /*USED FOR SET MODEL NAME INIT TIME */

        [HttpGet]
        public ApiResponse setModuleName()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int todoId = Convert.ToInt32(nvc["todoId"].ToString());
                    int moduleId = db.tblPMSToDoes.Where(z => z.TodoId == todoId).Select(z => z.ModuleId).SingleOrDefault();
                    tblPMSModule module = db.tblPMSModules.Where(z => z.ModuleId == moduleId).SingleOrDefault();
                    //String todoCommentText = db.tblPMSToDoes.Where(z => z.TodoId == todoId).Select(z => z.TodoText).SingleOrDefault();
                    tblPMSToDo todoInfo=db.tblPMSToDoes.Where(z => z.TodoId == todoId).SingleOrDefault();

                    //List<String> ModuleAndTodoComment = new List<string>();
                    //ModuleAndTodoComment.Add(module.ModuleId.ToString());
                    //ModuleAndTodoComment.Add(module.ModuleName);
                    //ModuleAndTodoComment.Add(todoCommentText);

                    List<PMSTodoModuleCommentViewModel> ModuleAndTodoComment = new List<PMSTodoModuleCommentViewModel>();
                    ModuleAndTodoComment.Add(new PMSTodoModuleCommentViewModel
                    {
                        ModuleId=module.ModuleId,
                        ModuleName=module.ModuleName,
                        todoCommentText = todoInfo.TodoText,
                        AssignUser = todoInfo.AssignedUser,
                        CanFinish =todoInfo.CanFinish,
                        CreBy=todoInfo.CreBy
                    });



                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", ModuleAndTodoComment);
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
        /// GET api/PMSModule
        /// Retrieve comment list
        /// </summary>
        [HttpGet]
        public ApiResponse GetCommentList()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    Boolean IsDelete = false;
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    List<PMSTodoCommentViewModel> cList = new List<PMSTodoCommentViewModel>();
                    List<tblEmpCompanyInformation> cList1 = new List<tblEmpCompanyInformation>();
                    int employeeId = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                    int todoId = Convert.ToInt32(nvc["todoId"].ToString());
                    int timezone = Convert.ToInt32(nvc["timezone"].ToString());

                    //  var list = db.tblPMSComments.Where(z => z.TodoId == todoId && z.IsActive == true).ToList();
                    var list = db.tblPMSComments.Where(z => z.TodoId == todoId && z.IsArchived == false).ToList();

                    int repotingId = db.tblEmpCompanyInformations.Where(z => z.EmployeeId == employeeId).Select(z => z.ReportingTo).FirstOrDefault();
                    cList1 = employeeUtils.DrillDownReporting(repotingId.ToString(), cList1);
                    cList1 = cList1.Where(z => z.ReportingTo != repotingId || z.EmployeeId == employeeId).ToList();

                    foreach (var l in list)
                    {
                        List<tblPMSCommentFile> uList = new List<tblPMSCommentFile>();
                        var upload = db.tblPMSCommentFiles.Where(z => z.CommentId == l.CommentId).ToList();
                        foreach (var u in upload)
                        {
                            uList.Add(new tblPMSCommentFile
                            {
                                UploadedFileId = u.UploadedFileId,
                                CommentId = u.CommentId,
                                FileName = u.FileName,
                                CaptionText = u.CaptionText
                            });
                        }
                        if (employeeId == 1)
                        {
                            IsDelete = true;
                        }
                        else
                        {
                            //some change in logic to apply delete for all item
                            int t = cList1.Where(z => z.EmployeeId == l.CreBy).Count();
                            if (t > 0)
                            {
                                IsDelete = true;
                            }
                            else
                            {
                                IsDelete = false;
                            }
                        }

                        var emp = db.tblEmpPersonalInformations.Where(z => z.EmployeeId == l.CreBy).FirstOrDefault();
                        cList.Add(new PMSTodoCommentViewModel
                        {
                            CommentId = l.CommentId,
                            TodoId = l.TodoId,
                            CommentText = l.CommentText,
                            Hours = l.Hours,
                            IsDelete = IsDelete,
                            CreDate = Convert.ToDateTime(l.CreDate).AddMinutes(-1 * timezone),
                            CreByName = string.Format("{0} {1}", emp.CandidateFirstName, emp.CandidateLastName),
                            ProfilePix = emp.ProfilePhoto,
                            lstUploadedFile = uList
                        });
                    }
                    cList = cList.OrderByDescending(z => z.CreDate).ToList();

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", cList);
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
        /// POST api/PMSModule
        /// save comment
        /// </summary>
        [HttpPost]
        public ApiResponse SaveComment(tblPMSComment com)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (com.CommentId == 0)
                    {// Mode == Add
                        tblPMSComment d = new tblPMSComment
                        {
                            TodoId = com.TodoId,
                            CommentText = com.CommentText,
                            Hours = com.Hours,
                            //  IsActive = true,
                            IsArchived = false,
                            CreDate = DateTime.Now.ToUniversalTime(),
                            CreBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString())
                        };
                        db.tblPMSComments.Add(d);
                        db.SaveChanges();
                        int commentId = db.tblPMSComments.Max(z => z.CommentId);
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, commentId);

                        // BEGIN SAVE TO-DO STATUS
                        NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                        int status = Convert.ToInt32(nvc["status"].ToString());

                        if (status == 3)
                        {
                            var AssignedHour = db.tblPMSToDoes.Where(z => z.TodoId == com.TodoId).Select(z => z.AssignedHours).FirstOrDefault();
                            var ActualHours = (from comment in db.tblPMSComments
                                               where comment.TodoId == com.TodoId
                                               group comment by comment.TodoId into g
                                               select new { hours = g.Sum(z => z.Hours) }.hours).FirstOrDefault();

                            if (AssignedHour == 0)
                            {
                                apiResponse = ERPUtilities.GenerateApiResponse(true, 0, "Assigned hour not define", true);
                                return apiResponse;
                            }

                            if (ActualHours == null || Convert.ToDouble(ActualHours.Value) == 0)
                            {
                                apiResponse = ERPUtilities.GenerateApiResponse(true, 0, "Actual hour not define", true);
                                return apiResponse;
                            }

                            if (AssignedHour != 0 && Convert.ToDouble(ActualHours.Value) != 0)
                            {
                                var line = db.tblPMSToDoes.Where(z => z.TodoId == com.TodoId).SingleOrDefault();
                                if (line != null)
                                {
                                    line.Status = 3;
                                    line.Priority = 0;
                                    line.EndDate = DateTime.Now;
                                    line.ChgDate = DateTime.Now.ToUniversalTime();
                                    line.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                                }
                                apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgUpdate, commentId);
                                db.SaveChanges();
                            }
                            else
                            {
                                apiResponse = ERPUtilities.GenerateApiResponse(true, 0, "Assigned/Actual hours not define", true);
                            }
                        }
                        else
                        {
                            var line = db.tblPMSToDoes.Where(z => z.TodoId == com.TodoId).SingleOrDefault();
                            if (line != null)
                            {
                                line.Status = status;
                                line.Priority = 0;
                                line.EndDate = DateTime.Now;
                                line.ChgDate = DateTime.Now.ToUniversalTime();
                                line.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                            }
                            apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgUpdate, commentId);
                            db.SaveChanges();
                        }

                        // END SAVE TO-DO STATUS
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
        /// POST api/PMSModule
        /// save comment
        /// </summary>
        [HttpPost]
        public ApiResponse SaveUploadedFile(tblPMSCommentFile com)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int projectId = Convert.ToInt32(nvc["projectId"].ToString());

                    tblPMSCommentFile d = new tblPMSCommentFile
                    {
                        CommentId = com.CommentId,
                        FileName = com.FileName,
                        CaptionText = com.CaptionText,
                        CreDate = DateTime.Now.ToUniversalTime(),
                        CreBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString())
                    };
                    db.tblPMSCommentFiles.Add(d);
                    db.SaveChanges();
                    MoveFile(com.FileName, projectId);

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

        protected void MoveFile(string fileName, int projectId)
        {
            if (System.IO.File.Exists(HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["TempUploadPath"].ToString()) + "/" + fileName))
            {
                var sourceMainFile = HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["TempUploadPath"].ToString()) + "/" + fileName;
                var originalPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["pmsUploads"].ToString() + "/" + projectId + "/");

                bool folderExists = Directory.Exists(originalPath);
                if (!folderExists)
                {
                    Directory.CreateDirectory(originalPath);
                }

                System.IO.File.Move(sourceMainFile, originalPath + "/" + fileName);

                string ext = Path.GetExtension(fileName).ToLower();
                if (ext == ".jpg" || ext == ".jpeg" || ext == ".png")
                {
                    var sourceThumbFile = HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["TempThumbnails"].ToString()) + "/" + fileName;
                    var originalThumbPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["pmsThumbnails"].ToString() + "/" + projectId + "/");

                    bool folderthumbExists = Directory.Exists(originalThumbPath);
                    if (!folderthumbExists)
                    {
                        Directory.CreateDirectory(originalThumbPath);
                    }
                    System.IO.File.Move(sourceThumbFile, originalThumbPath + "/" + fileName);
                }
            }
        }


        /// <summary>
        /// GET api/PMSTodoComment
        /// Send mail to notify users
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
                    int commentId = Convert.ToInt32(nvc["commentId"]);
                    string bcc = nvc["bcc"].ToString();
                    int loginId = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                    var emp = db.tblEmpPersonalInformations.Where(z => z.EmployeeId == loginId).SingleOrDefault();

                    var comment = db.tblPMSComments.Where(z => z.CommentId == commentId).SingleOrDefault();
                    var todo = db.tblPMSToDoes.Where(z => z.TodoId == comment.TodoId).SingleOrDefault();
                    var module = db.tblPMSModules.Where(z => z.ModuleId == todo.ModuleId).SingleOrDefault();
                    var project = db.tblPMSProjects.Where(z => z.ProjectId == module.ProjectId).SingleOrDefault();

                    string sendSubject = string.Format("Comment is added on Todo by {0} {1}", emp.CandidateFirstName, emp.CandidateLastName);
                    
                    string body = string.Empty;
                    using (var sr = new StreamReader(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["tmplProjectTodoComment"].ToString())))
                    {
                        body = sr.ReadToEnd();
                        body = body.Replace("##ProjectName##", project.ProjectName);
                        body = body.Replace("##ModuleName##", module.ModuleName);
                        body = body.Replace("##Todo##", todo.TodoText);
                        body = body.Replace("##Comment##", comment.CommentText);
                        body = body.Replace("##EmployeeName##", emp.CandidateFirstName + " " + emp.CandidateLastName);
                        var staticImagesPath = ConfigurationManager.AppSettings["StaticImagesPath"].ToString();
                        var textureImage = "daimond_eyes.png";
                        body = body.Replace("##bgTexturePath##", staticImagesPath + textureImage);
                    }
                    
                    //body = string.Format("Todo: {0}<br/>Comment: {1}<br/>Commented by: {2} {3}", todo.TodoText, comment.CommentText, emp.CandidateFirstName, emp.CandidateLastName);

                    MailAddress mFrom = new MailAddress(ConfigurationManager.AppSettings["fromMail"].ToString());
                    //if (string.IsNullOrEmpty(emp.CompanyEmailId))
                    //{
                    //    apiResponse = ERPUtilities.GenerateApiResponse(true, 0, generalMessages.msgError, true);
                    //    return apiResponse;
                    //}

                    MailAddress mTo = new MailAddress(emp.CompanyEmailId);
                    System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage(mFrom, mTo);
                    message.Subject = sendSubject;
                    message.Body = body;

                    foreach (var employeeid in bcc.Split(','))
                    {
                        if (!string.IsNullOrEmpty(employeeid))
                        {
                            if (Convert.ToInt32(employeeid) != loginId)
                            { //Avoid duplicate entry
                                var email = db.tblEmpPersonalInformations.AsEnumerable().Where(z => z.EmployeeId == Convert.ToInt32(employeeid)).Select(z => z.CompanyEmailId).SingleOrDefault();
                                if (!string.IsNullOrEmpty(email))
                                {
                                    MailAddress ccEmail = new MailAddress(email);
                                    message.CC.Add(ccEmail);
                                }
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
                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, true);
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
        public ApiResponse DeleteModuleTodoComment()
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                int CommentId = Convert.ToInt32(nvc["CommentId"].ToString());

                int LoginId = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                var line = db.tblPMSComments.Where(z => z.CommentId == CommentId).SingleOrDefault();
                if (line != null)
                {
                    //db.tblPMSComments.Remove(line);
                    // line.IsActive = false;
                    line.IsArchived = true;

                    db.SaveChanges();
                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgDelete, null);
                }
            }
            catch (Exception ex)
            {
                apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
            }

            return apiResponse;
        }
    }
}
