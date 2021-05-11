using ERP.Models;
using ERP.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web;
using System.Web.Http;

namespace ERP.WebApis
{
    public class AdminMasterController : ApiController
    {

        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Master";
        private List<string> lstStaticPages = new List<string>();

        public AdminMasterController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);

            //Add Static Pages
            List<string> staticPages = ConfigurationManager.AppSettings["StaticPages"].ToString().Split(',').ToList<string>();
            foreach (var page in staticPages)
            {
                lstStaticPages.Add(page);
            }
        }

        /// <summary>
        /// GET api/AdminMaster
        /// retrieve menu list 
        /// </summary>
        //[HttpGet]
        //public ApiResponse GetMenuList()
        //{
        //    ApiResponse apiResponse = new ApiResponse();
        //    if (sessionUtils.HasUserLogin())
        //    {
        //        try
        //        {
        //            List<ARAssignPermissionViewModel> list = new List<ARAssignPermissionViewModel>();
        //            //IF USER IS ADMIN THEN TAKE ALL MENUS

        //            //GET: role id using logged in user
        //            int employeeid = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
        //            if (employeeid == 1) { //ADMIN
        //                var mList = db.tblARModules.Where(z => z.IsActive == true).ToList();
        //                foreach (var m in mList) {
        //                    var smList = db.tblARSubModules.Where(z => z.ModuleId == m.ModuleId && z.IsActive == true).ToList();
        //                    foreach (var sm in smList) {
        //                        list.Add(new ARAssignPermissionViewModel
        //                        {
        //                            ModuleId = m.ModuleId,
        //                            ModuleName = m.ModuleName,
        //                            SubModuleId = sm.SubModuleId,
        //                            SubModuleName = sm.SubModuleName,
        //                            SubModuleURL = sm.URL,
        //                            ModuleSeqNo = m.SeqNo,
        //                            SubModuleSeqNo = sm.SeqNo
        //                        });
        //                    }
        //                }

        //            }
        //            else {//NORMAL USER
        //                var cLine = db.tblEmpCompanyInformations.Where(z => z.EmployeeId == employeeid).SingleOrDefault();
        //                if (cLine != null)
        //                {
        //                    var pList = db.tblARPermissionAssigneds.Where(z => z.RoleId == cLine.RolesId).ToList();
        //                    if (pList != null)
        //                    {
        //                        foreach (var p in pList)
        //                        {
        //                            //CHECK: if sub module entry is exists && active
        //                            if (db.tblARSubModules.Where(z => z.SubModuleId == p.SubModuleId && z.IsActive == true).Count() > 0)
        //                            {
        //                                var sLine = db.tblARSubModules.Where(z => z.SubModuleId == p.SubModuleId).Select(z => new { z.SubModuleName, z.URL, z.SeqNo }).FirstOrDefault();
        //                                var mLine = db.tblARModules.Where(z => z.ModuleId == p.ModuleId).Select(z => new { z.ModuleName, z.SeqNo }).FirstOrDefault();

        //                                list.Add(new ARAssignPermissionViewModel
        //                                {
        //                                    ModuleId = p.ModuleId,
        //                                    ModuleName = mLine.ModuleName,
        //                                    SubModuleId = p.SubModuleId,
        //                                    SubModuleName = sLine.SubModuleName,
        //                                    SubModuleURL = sLine.URL,
        //                                    ModuleSeqNo = mLine.SeqNo,
        //                                    SubModuleSeqNo = sLine.SeqNo
        //                                });
        //                            }
        //                        }
        //                    }
        //                }
        //            }

        //            //RESPONSE: return all active modules/submodules for assigned role
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
        /// GET api/AdminMaster
        /// Check permission for page against logged in user role
        /// </summary>
        [HttpGet]
        public ApiResponse HasPermission()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    string ctrl = nvc["ctrl"].ToString();

                    bool hasPermission = false;
                    int employeeid = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                    if (employeeid == 1)
                    { //ADMIN
                        hasPermission = true;
                    }
                    else
                    {//NORMAL USER 
                        //Check for static pages
                        if (lstStaticPages.Contains(ctrl))
                        {
                            hasPermission = true;
                        }
                        else
                        {
                            //Check for dynamic pages
                            var cLine = db.tblEmpCompanyInformations.Where(z => z.EmployeeId == employeeid).SingleOrDefault();
                            if (cLine != null)
                            {
                                //var pList = db.tblARPermissionAssigneds.Where(z => z.RoleId == cLine.RolesId && z.IsActive == true).ToList();
                                var pList = db.tblARPermissionAssigneds.Where(z => z.RoleId == cLine.RolesId).ToList();
                                foreach (var p in pList)
                                {
                                    var sName = db.tblARSubModules.Where(z => z.SubModuleId == p.SubModuleId).Select(z => z.URL).SingleOrDefault();
                                    if (sName.Trim().ToLower().Contains("/pms/"))
                                    {//For PMS Module
                                        if (ctrl.Trim().ToLower().Replace("/", "") == sName.ToLower().Replace("/pms/", "").Trim().ToLower())
                                        {
                                            hasPermission = true;
                                        }
                                    }
                                    else
                                    {
                                        if (ctrl.Trim().ToLower() == sName.Replace('/', ' ').Trim().ToLower())
                                        {
                                            hasPermission = true;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", hasPermission);
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
        /// GET api/AdminMaster
        /// Check permission for page against logged in user role
        /// </summary>
        [HttpGet]
        public ApiResponse ChangePassword()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    string newPassword = nvc["newPassword"].ToString();
                    int employeeId = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());

                    var line = db.tblEmpLoginInformations.Where(z => z.EmployeeId == employeeId).FirstOrDefault();
                    if (line != null)
                    {
                        line.Password = newPassword;
                        line.ChgDate = DateTime.Now;
                        line.ChgBy = employeeId;
                    }
                    db.SaveChanges();
                    SendMail(newPassword);
                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgChangePassword, true);
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


        public void SendMail(string password)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    int employeeId = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                    var emp = db.tblEmpPersonalInformations.Where(z => z.EmployeeId == employeeId).SingleOrDefault();
                    
                    string sendSubject = string.Format("your new Password: {0} {1} {2}", emp.CandidateFirstName, emp.CandidateLastName,password);
                    string body = string.Empty;
                    body = string.Format("Employee name:{0}<br/> New Password: {1}", emp.CandidateFirstName,password);
                    
                    MailAddress mFrom = new MailAddress(ConfigurationManager.AppSettings["fromMail"].ToString());
                    MailAddress mTo = new MailAddress(emp.CompanyEmailId);
                    
                    MailMessage message = new MailMessage(mFrom, mTo);
                    message.Subject = sendSubject;
                    message.Body = body;
                    message.IsBodyHtml = true;
                    message.BodyEncoding = System.Text.Encoding.UTF8;
                    
                    SmtpClient client = new SmtpClient(ConfigurationManager.AppSettings["smtp"].ToString());
                    client.EnableSsl = false;
                    client.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["mail"].ToString(), ConfigurationManager.AppSettings["Password"].ToString());
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
        }

        /// <summary>
        /// GET api/AdminMaster
        /// Switch menu view
        /// </summary>
        [HttpGet]
        public ApiResponse SwitchMenuView()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (HttpContext.Current.Session["MenuView"].ToString() == "S")
                    {
                        HttpContext.Current.Session["MenuView"] = "T"; //Top
                    }
                    else
                    {
                        HttpContext.Current.Session["MenuView"] = "S"; //side bar - left side
                    }
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



        [HttpGet]
        public ApiResponse activeModuleTodoList()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int employeeId = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                    List<PMSTodo> list = new List<PMSTodo>();
                    //find routing id
                    
                    var test = db.tblPMSToDoes.Where(z => z.AssignedUser == employeeId && z.Status == 1 && z.IsArchived == false).ToList();
                        foreach (var l in test){

                            var moduleInfo = db.tblPMSModules.Where(z => z.ModuleId == l.ModuleId).FirstOrDefault();
                            var projectInfo = db.tblPMSProjects.Where(z => z.ProjectId == moduleInfo.ProjectId ).FirstOrDefault();

                            /*
                            int users = db.tblPMSProjectUsers.Where(z => z.ProjectId == projectInfo.ProjectId && z.EmployeeId == employeeId).Count();
                            if (users > 0)
                            {*/
                                list.Add(new PMSTodo
                                {
                                    ProjectName = projectInfo.ProjectName,
                                    ModuleName = moduleInfo.ModuleName,
                                    oldTodoText = l.TodoText,
                                    AssignedHours = l.AssignedHours,
                                    TodoId = l.TodoId,
                                    ModuleId = l.ModuleId
                                });
                            /*}
                            else { }*/

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


    }
}
