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
using System.Data.Entity.Infrastructure;

namespace ERP.WebApis
{
    public class RoleController : ApiController
    {
        private ERPContext db = new ERPContext();
        SessionUtils sessionUtils = new SessionUtils();
        GeneralMessages generalMessages = null;
        string _pageName = "Role";
        string _pageNamePermission = "Access Rights";

        public RoleController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }

        /// <summary>
        /// POST api/role
        /// create and update record
        /// </summary>
        [HttpPost]
        public ApiResponse SaveRole(tblRole role)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (role.RolesId == 0)
                    {// Mode == Add
                        tblRole d = new tblRole
                        {
                            Roles = role.Roles,
                            IsActive = role.IsActive,
                            CreDate = DateTime.Now.ToUniversalTime(),
                            ChgDate = DateTime.Now.ToUniversalTime()
                        };
                        db.tblRoles.Add(d);
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);
                    }
                    else
                    {// Mode == Edit
                        var line = db.tblRoles.Where(z => z.RolesId == role.RolesId).SingleOrDefault();
                        if (line != null)
                        {
                            line.Roles = role.Roles;
                            line.IsActive = role.IsActive;
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
        /// POST api/role
        /// delete role 
        /// </summary>
        [HttpPost]
        public ApiResponse DeleteRole([FromBody]int id)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (DependancyStatus.RoleStatus(id))
                    {
                        var line = db.tblRoles.Where(z => z.RolesId == id).SingleOrDefault();
                        if (line != null)
                        {
                            db.tblRoles.Remove(line);
                        }

                        db.SaveChanges();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgDelete, null);
                    }
                    else
                    {
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 2, generalMessages.msgParentExists, null);
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
        /// POST api/role
        /// active-inActive record 
        /// </summary>
        [HttpPost]
        public ApiResponse ChangeStatus(tblRole role)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (DependancyStatus.RoleStatus(role.RolesId))
                    {
                        var line = db.tblRoles.Where(z => z.RolesId == role.RolesId).SingleOrDefault();
                        if (line != null)
                        {
                            if (role.IsActive)
                            {
                                line.IsActive = false;
                            }
                            else if (!role.IsActive)
                            {
                                line.IsActive = true;
                            }
                        }

                        line.ChgDate = DateTime.Now.ToUniversalTime();
                        db.SaveChanges();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgChangeStatus, null);
                    }
                    else
                    {
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 2, generalMessages.msgStatusError, null);
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
        /// GET api/role
        /// return role list with sorting and filtering  functionalities
        /// </summary>
        [HttpGet]
        public ApiResponse GetRoleList()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                using (db)
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int page = Convert.ToInt32(nvc["page"]);
                    int timezone = Convert.ToInt32(nvc["timezone"]);
                    string orderBy = nvc["orderby"];
                    string filter = nvc["filter"];

                    int iDisplayLength = Convert.ToInt32(nvc["count"]);
                    int iDisplayStart = (page - 1) * iDisplayLength;

                    List<tblRole> list = null;

                    try
                    {
                        list = db.tblRoles.ToList();

                        //1. filter data
                        if (!string.IsNullOrEmpty(filter) && filter != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.Roles.ToLower().Contains(filter.ToLower())).ToList();
                        }
                        //2. do sorting on list
                        list = DoSorting(list, orderBy.Trim());

                        //3. take total count to return for ng-table
                        var Count = list.Count();

                        //4. convert returned datetime to local timezone
                        var Roles = list.Select(i =>
                        {
                            i.CreDate = Convert.ToDateTime(i.CreDate).AddMinutes(-1 * timezone);
                            i.ChgDate = Convert.ToDateTime(i.ChgDate).AddMinutes(-1 * timezone);
                            return i;
                        }).Skip(iDisplayStart).Take(iDisplayLength).ToList();


                        var resultData = new
                        {
                            total = Count,
                            result = Roles.ToList()
                        };

                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", resultData);
                    }
                    catch (Exception ex)
                    {
                        apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
                    }
                    finally
                    {
                        list = null;
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
        public List<tblRole> DoSorting(IEnumerable<tblRole> list, string orderBy)
        {
            try
            {
                if (orderBy == "Roles")
                {
                    list = list.OrderBy(z => z.Roles).ToList();
                }
                else if (orderBy == "-Roles")
                {
                    list = list.OrderByDescending(z => z.Roles).ToList();
                }
                else if (orderBy == "ChgDate")
                {
                    list = list.OrderBy(z => z.ChgDate).ToList();
                }
                else if (orderBy == "-ChgDate")
                {
                    list = list.OrderByDescending(z => z.ChgDate).ToList();
                }
                return list.ToList<tblRole>();
            }
            catch
            {
                return null;
            }
        }


        /// <summary>
        /// List Of Active Roles
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<tblRole> GetActiveRoles()
        {
            try
            {
                return db.tblRoles.Where(z => z.IsActive == true).OrderBy(z => z.Roles).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }


        /// <summary>
        /// GET api/role
        /// return role list with sorting and filtering  functionalities
        /// </summary>
        [HttpGet]
        public ApiResponse GetModuleSubModuleList()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                using (db)
                {
                    List<ARAssignPermissionViewModel> list = new List<ARAssignPermissionViewModel>();
                    try
                    {
                        NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                        int roleId = Convert.ToInt32(nvc["roleId"]);

                        var smList = db.tblARSubModules.ToList();
                        foreach (var s in smList)
                        {
                            List<SelectItemAccessRights> iList = new List<SelectItemAccessRights>();
                            if (!string.IsNullOrEmpty(s.AllowedAccess))
                            {
                                foreach (string l in s.AllowedAccess.Split(','))
                                {
                                    if (!string.IsNullOrEmpty(l))
                                    {
                                        //send already exists access permission to display in form
                                        var permissionList = db.tblARPermissionAssigneds.Where(z => z.RoleId == roleId && z.ModuleId == s.ModuleId && z.SubModuleId == s.SubModuleId && z.IsActive == true).SingleOrDefault();
                                        var isExists = false;
                                        if (permissionList != null)
                                        {
                                            if (!string.IsNullOrEmpty(permissionList.Permission))
                                            {
                                                foreach (var p in permissionList.Permission.Split(','))
                                                {
                                                    if (!string.IsNullOrEmpty(p) && isExists == false)
                                                    {
                                                        if (p == l)
                                                        {
                                                            isExists = true;
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        if (s.ModuleId == 1 && s.SubModuleName.ToLower() == "dashboard")
                                        {
                                            iList.Add(new SelectItemAccessRights
                                            {
                                                Id = Convert.ToInt32(l),
                                                Label = Enum.GetName(typeof(ERPUtilities.DashboardWidget), Convert.ToInt32(l)),
                                                IsSelected = isExists
                                            });
                                        }
                                        else
                                        {
                                            if (Enum.GetName(typeof(ERPUtilities.AccessPermission), Convert.ToInt32(l)) != null)
                                            {
                                                iList.Add(new SelectItemAccessRights
                                                {
                                                    Id = Convert.ToInt32(l),
                                                    Label = Enum.GetName(typeof(ERPUtilities.AccessPermission), Convert.ToInt32(l)),
                                                    IsSelected = isExists
                                                });
                                            }
                                            else
                                            {
                                                if (s.SubModuleName.ToLower() == "manage employee")
                                                {
                                                    iList.Add(new SelectItemAccessRights
                                                    {
                                                        Id = Convert.ToInt32(l),
                                                        Label = Enum.GetName(typeof(ERPUtilities.AccessPermissionEmployeeTab), Convert.ToInt32(l)),
                                                        IsSelected = isExists
                                                    });
                                                }
                                                else if (s.SubModuleName.ToLower() == "conversation")
                                                {
                                                    iList.Add(new SelectItemAccessRights
                                                    {
                                                        Id = Convert.ToInt32(l),
                                                        Label = Enum.GetName(typeof(ERPUtilities.AccessPermissionInvoiceTab), Convert.ToInt32(l)),
                                                        IsSelected = isExists
                                                    });
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            list.Add(new ARAssignPermissionViewModel
                            {
                                ModuleId = s.ModuleId,
                                ModuleName = db.tblARModules.Where(z => z.ModuleId == s.ModuleId).Select(z => z.ModuleName).SingleOrDefault(),
                                SubModuleId = s.SubModuleId,
                                SubModuleName = s.SubModuleName,
                                AllowedAccess = iList
                            });
                        }

                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", list);
                    }
                    catch (Exception ex)
                    {
                        apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
                    }
                    finally
                    {
                        list = null;
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
        /// POST api/role
        /// delete access permission 
        /// </summary>
        [HttpPost]
        public ApiResponse DeleteAccessPermission([FromBody]int id)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var list = db.tblARPermissionAssigneds.Where(z => z.RoleId == id).ToList();
                    foreach (var l in list)
                    {
                        db.tblARPermissionAssigneds.Remove(l);
                    }

                    db.SaveChanges();
                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", null);
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageNamePermission, true);
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }

        /// <summary>
        /// POST api/role
        /// create and update access rights
        /// </summary>
        [HttpPost]
        public ApiResponse SaveAccessPermission(tblARPermissionAssigned AR)
        {
            ApiResponse apiResponse = new ApiResponse();
            GeneralMessages msg = new GeneralMessages(_pageNamePermission);
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (AR.ModuleId > 0)
                    {
                        tblARPermissionAssigned access = new tblARPermissionAssigned
                        {
                            RoleId = AR.RoleId,
                            ModuleId = AR.ModuleId,
                            SubModuleId = AR.SubModuleId,
                            Permission = AR.Permission,
                            IsActive = true,
                            CreDate = DateTime.Now.ToUniversalTime(),
                            CreBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString()),
                            ChgDate = DateTime.Now.ToUniversalTime(),
                            ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString())
                        };
                        db.tblARPermissionAssigneds.Add(access);
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, msg.msgInsert, null);
                    }
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageNamePermission, true);
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }
            return apiResponse;
        }


        [HttpGet]
        public ApiResponse GetUsersByRole()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                int roleId = Convert.ToInt32(nvc["roleId"]);
                using (db)
                {
                    try
                    {
                        var users = from u in db.tblEmpCompanyInformations
                                    join up in db.tblEmpPersonalInformations
                                    on u.EmployeeId equals up.EmployeeId
                                    where u.RolesId == roleId
                                    orderby up.CandidateFirstName
                                    select new
                                    {
                                        Id = up.EmployeeId,
                                        Name = up.CandidateFirstName + (up.CandidateMiddleName.Length > 1 ? up.CandidateMiddleName.Substring(0, 1) : up.CandidateMiddleName) + (up.CandidateLastName.Length > 1 ? up.CandidateLastName.Substring(0, 1) : up.CandidateLastName)
                                    };

                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", users.ToList());
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
    }
}
