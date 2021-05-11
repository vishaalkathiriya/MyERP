using ERP.Models;
using ERP.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace ERP.WebApis
{
    public class PMSModuleController : ApiController
    {
        private ERPContext db = new ERPContext();
        SessionUtils sessionUtils = new SessionUtils();
        GeneralMessages generalMessages = null;
        GeneralMessages generalMsg = null;
        string _pageName = "PMS Module";
        string _pageNameForTodo = "Todo";

        public PMSModuleController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
            generalMsg = new GeneralMessages(_pageNameForTodo);
        }


        /// <summary>
        /// POST api/PMSModule
        /// create module
        /// </summary>
        [HttpPost]
        public ApiResponse CreateUpdateModule(tblPMSModule mod)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (mod.ModuleId == 0)
                    {// Mode == Add
                        int prio;
                        try
                        {//calculate priority
                            prio = db.tblPMSModules.Max(z => z.Priority + 1);
                        }
                        catch
                        {
                            prio = 1;
                        }

                        tblPMSModule d = new tblPMSModule
                        {
                            ProjectId = mod.ProjectId,
                            ModuleName = mod.ModuleName,
                            ModuleType = mod.ModuleType,
                            Priority = prio,
                            IsArchived = mod.IsArchived,
                            IsActive = mod.IsActive,
                            CreDate = DateTime.Now.ToUniversalTime(),
                            CreBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString()),
                            ChgDate = DateTime.Now.ToUniversalTime(),
                            ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString())
                        };
                        db.tblPMSModules.Add(d);
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, true);
                    }
                    else
                    {// Mode == Edit
                        var line = db.tblPMSModules.Where(z => z.ModuleId == mod.ModuleId).SingleOrDefault();
                        if (line != null)
                        {
                            line.ModuleName = mod.ModuleName;
                            line.ModuleType = mod.ModuleType;
                            line.ChgDate = DateTime.Now.ToUniversalTime();
                            line.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                        }
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgUpdate, true);
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
        /// GET api/PMSModule
        /// retrieve project module list
        /// </summary>
        [HttpGet]
        public ApiResponse GetModuleList()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {

                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int timezone = Convert.ToInt32(nvc["timezone"].ToString());
                    int projectid = Convert.ToInt32(nvc["projectId"].ToString());

                    List<PMSModuleListSpViewModel> spModuleList = new List<PMSModuleListSpViewModel>();
                    List<PMSTodoListSpViewModel> spTodoList = new List<PMSTodoListSpViewModel>();
                    List<PMSModuleListSpViewModel> spModuleListViewModel = new List<PMSModuleListSpViewModel>();


                    var ProjectId = new SqlParameter("@ProjectId", projectid);
                    var ModuleId = new SqlParameter("@ModuleIdParam", DBNull.Value);
                    spModuleList = db.Database.SqlQuery<PMSModuleListSpViewModel>("usp_getModuleList  @ProjectId,@ModuleIdParam", ProjectId, ModuleId).ToList();

                    var ProjectIdTodo = new SqlParameter("@ProjectId", projectid);
                    var ModuleIdTodo = new SqlParameter("@ModuleId", DBNull.Value);
                    //spTodoList = db.Database.SqlQuery<PMSTodoListSpViewModel>("usp_getTodoList  @ProjectId,@ModuleId", ProjectIdTodo, ModuleIdTodo).ToList();
                    spTodoList = db.Database.SqlQuery<PMSTodoListSpViewModel>("usp_getTodoList  @ProjectId,@ModuleId", ProjectIdTodo, ModuleIdTodo).ToList();

                    foreach (var list in spModuleList)
                    {
                        List<PMSTodoListSpViewModel> spTodoListViewModel = new List<PMSTodoListSpViewModel>();
                        var todoListFilter = spTodoList.Where(z => z.ModuleId == list.ModuleId).ToList();

                        if (todoListFilter.Count > 0)
                        {
                            foreach (var todoList in todoListFilter)
                            {
                                spTodoListViewModel.Add(new PMSTodoListSpViewModel
                                {
                                    TodoId = todoList.TodoId,
                                    TodoText = todoList.TodoText,
                                    AssignedUser = todoList.AssignedUser,
                                    AssignedUserFullName = todoList.AssignedUserFullName,
                                    AssignedHours = todoList.AssignedHours,
                                    ActualHours = todoList.ActualHours,
                                    TodoType = todoList.TodoType,
                                    DueDate = todoList.DueDate,
                                    TotalComments = todoList.TotalComments,
                                    Status = todoList.Status,
                                    IsArchived = todoList.IsArchived,
                                    Priority = todoList.Priority,
                                    IsEdit = false,
                                    IsChecked = false,
                                    IsCanFinish = Convert.ToBoolean(todoList.IsCanFinish),
                                    creById = todoList.creById,
                                    CreDate=todoList.CreDate,
                                    CreBy=todoList.CreBy,
                                    oldTodoText = todoList.TodoText,
                                    oldTodoHours = todoList.AssignedHours,
                                    oldTodoType=todoList.TodoType
                                });
                            }

                            if (spTodoListViewModel != null)
                            {
                                spTodoListViewModel = spTodoListViewModel.OrderBy(z => z.Priority).ToList();
                            }
                        }
                        spModuleListViewModel.Add(new PMSModuleListSpViewModel
                        {
                            ModuleId = list.ModuleId,
                            ProjectId = list.ProjectId,
                            ProjectName = list.ProjectName,
                            ModuleName = list.ModuleName,
                            ModuleType = list.ModuleType,
                            Priority=list.Priority,
                            ActiveStatusCount = list.ActiveStatusCount,
                            totalTaskCount = list.totalTaskCount,
                            ActualHours = list.ActualHours,
                            AssignedHours = list.AssignedHours,
                            IsArchived = list.IsArchived,
                            TodoList = spTodoListViewModel
                        });
                    }


                    if (spModuleListViewModel != null)
                    {
                        spModuleListViewModel = spModuleListViewModel.Select(i =>
                        {
                          //  i.CreDate = Convert.ToDateTime(i.CreDate).AddMinutes(-1 * timezone);
                           // i.ChgDate = Convert.ToDateTime(i.ChgDate).AddMinutes(-1 * timezone);
                            return i;
                        }).OrderBy(z => z.Priority).ToList();
                    }
                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", spModuleListViewModel);



                    //NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    //int timezone = Convert.ToInt32(nvc["timezone"].ToString());
                    //int projectid = Convert.ToInt32(nvc["projectId"].ToString());
                    //List<PMSModuleTodoViewModel> main = new List<PMSModuleTodoViewModel>();
                    //var list = db.tblPMSModules.Where(z => z.ProjectId == projectid && z.IsArchived == false).ToList();
                    //foreach (var l in list)
                    //{
                    //    List<PMSTodo> todoList = new List<PMSTodo>();
                    //    var todo = db.tblPMSToDoes.Where(z => z.ModuleId == l.ModuleId && z.IsArchived == false).ToList();//Status 1:Active, 2:Hold, 3:Finished
                    //    var ActiveStatusCount = db.tblPMSToDoes.Where(z => z.ModuleId == l.ModuleId && z.IsArchived == false && z.Status == 1).Count();//Status 1:Active, 2:Hold, 3:Finished
                    //    var totalTaskCount = db.tblPMSToDoes.Where(z => z.ModuleId == l.ModuleId && z.IsArchived == false).Count();//Status 1:Active, 2:Hold, 3:Finished
                    //    foreach (var t in todo)
                    //    {
                    //        string fullName = "UnAssigned";
                    //        if (t.AssignedUser != 0)
                    //        {
                    //            var emp = db.tblEmpPersonalInformations.Where(z => z.EmployeeId == t.AssignedUser).FirstOrDefault();
                    //            fullName = string.Format("{0}{1}{2}", emp.CandidateFirstName, emp.CandidateMiddleName.Substring(0, 1).ToUpper(), emp.CandidateLastName.Substring(0, 1).ToUpper());
                    //        }
                    //        todoList.Add(new PMSTodo
                    //        {
                    //            TodoId = t.TodoId,
                    //            ModuleId = t.ModuleId,
                    //            TodoText = t.TodoText,
                    //            AssignedUser = t.AssignedUser,
                    //            AssignedHours = t.AssignedHours,
                    //            ActualHours = db.tblPMSComments.Where(z => z.TodoId == t.TodoId && z.IsArchived == false).Sum(z => z.Hours),
                    //            AssignedUserFullName = fullName,
                    //            Priority = t.Priority,
                    //            DueDate = t.DueDate,
                    //            TotalComments = db.tblPMSComments.Where(z => z.TodoId == t.TodoId && z.IsArchived == false).Count(),
                    //            IsEdit = false,
                    //            oldTodoText = t.TodoText,
                    //            oldTodoHours = t.AssignedHours,
                    //            Status = t.Status,
                    //            IsChecked = false
                    //        });
                    //    }
                    //    ERPUtils erpUtils = new ERPUtils();
                    //    List<decimal> finalHours = erpUtils.CalculateHoursForModule(todoList);
                    //    if (todoList != null)
                    //    {
                    //        todoList = todoList.OrderBy(z => z.Priority).ToList();
                    //    }
                    //    main.Add(new PMSModuleTodoViewModel
                    //    {
                    //        ModuleId = l.ModuleId,
                    //        ProjectId = l.ProjectId,
                    //        ProjectName = db.tblPMSProjects.Where(z => z.ProjectId == l.ProjectId).Select(z => z.ProjectName).SingleOrDefault(),
                    //        ModuleName = l.ModuleName,
                    //        ModuleType = l.ModuleType,
                    //        Priority = l.Priority,
                    //        IsArchived = l.IsArchived,
                    //        ActiveStatusCount = ActiveStatusCount,
                    //        totalTaskCount = totalTaskCount,
                    //        IsActive = l.IsActive,
                    //        CreDate = l.CreDate,
                    //        ChgDate = l.ChgDate,
                    //        ChgBy = l.ChgBy,
                    //        TodoList = todoList,
                    //        //ActualHours = todoList.Sum(tl => tl.ActualHours),
                    //        //AssignedHours = todoList.Sum(tl => tl.AssignedHours)
                    //        ActualHours = finalHours[0],
                    //        AssignedHours = finalHours[1]
                    //    });
                    //}
                    //if (main != null)
                    //{
                    //    main = main.Select(i =>
                    //    {
                    //        i.CreDate = Convert.ToDateTime(i.CreDate).AddMinutes(-1 * timezone);
                    //        i.ChgDate = Convert.ToDateTime(i.ChgDate).AddMinutes(-1 * timezone);
                    //        return i;
                    //    }).OrderBy(z => z.Priority).ToList();
                    //}
                    //apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", main);

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
        /// retrieve project name
        /// </summary>
        [HttpGet]
        public ApiResponse GetProjectName()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int projectId = Convert.ToInt32(nvc["projectId"].ToString());
                    string projectName = db.tblPMSProjects.Where(z => z.ProjectId == projectId).Select(z => z.ProjectName).SingleOrDefault();
                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", projectName);
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
        /// retrieve module todo list
        /// </summary>
        [HttpGet]
        public ApiResponse GetModuleTodoList()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int timezone = Convert.ToInt32(nvc["timezone"].ToString());
                    int moduleId = Convert.ToInt32(nvc["moduleId"].ToString());

                    List<PMSModuleListSpViewModel> spModuleList = new List<PMSModuleListSpViewModel>();
                    List<PMSTodoListSpViewModel> spTodoList = new List<PMSTodoListSpViewModel>();
                    List<PMSModuleListSpViewModel> spModuleListViewModel = new List<PMSModuleListSpViewModel>();

                    var ProjectId = new SqlParameter("@ProjectId", DBNull.Value);
                    var ModuleId = new SqlParameter("@ModuleIdParam", moduleId);
                    spModuleList = db.Database.SqlQuery<PMSModuleListSpViewModel>("usp_getModuleList  @ProjectId,@ModuleIdParam", ProjectId, ModuleId).ToList();

                    var ProjectIdTodo = new SqlParameter("@ProjectId", DBNull.Value);
                    var ModuleIdTodo = new SqlParameter("@ModuleId", moduleId);
                    spTodoList = db.Database.SqlQuery<PMSTodoListSpViewModel>("usp_getTodoList  @ProjectId,@ModuleId", ProjectIdTodo, ModuleIdTodo).ToList();

                    List<PMSTodoListSpViewModel> todoListActive = new List<PMSTodoListSpViewModel>();
                    List<PMSTodoListSpViewModel> todoListHold = new List<PMSTodoListSpViewModel>();
                    List<PMSTodoListSpViewModel> todoListFinished = new List<PMSTodoListSpViewModel>();
                    List<PMSTodoListSpViewModel> todoListArchived = new List<PMSTodoListSpViewModel>();

                    foreach (var model in spModuleList)
                    {

                        //Status 1:Active, 2:Hold, 3:Finished
                        var todoActive = spTodoList.Where(z => z.ModuleId == model.ModuleId && z.IsArchived == false && z.Status == 1).ToList();
                        var todoHold = spTodoList.Where(z => z.ModuleId == model.ModuleId && z.IsArchived == false && z.Status == 2).ToList();
                        var todoFinished = spTodoList.Where(z => z.ModuleId == model.ModuleId && z.IsArchived == false && z.Status == 3).ToList();
                        var todoArchived = spTodoList.Where(z => z.ModuleId == model.ModuleId && z.IsArchived == true).ToList();

                        /* Active: Assigned and UnAssigned */
                        foreach (var active in todoActive)
                        {
                            todoListActive.Add(new PMSTodoListSpViewModel
                            {
                                TodoId = active.TodoId,
                                TodoText = active.TodoText,
                                AssignedUser = active.AssignedUser,
                                AssignedUserFullName = active.AssignedUserFullName,
                                AssignedHours = active.AssignedHours,
                                TodoType = active.TodoType,
                                ActualHours = active.ActualHours,
                                DueDate = active.DueDate,
                                TotalComments = active.TotalComments,
                                Status = active.Status,
                                IsArchived = active.IsArchived,
                                Priority = active.Priority,
                                IsEdit = false,
                                IsCanFinish = active.IsCanFinish,
                                creById = active.creById,
                                CreDate=active.CreDate,
                                CreBy=active.CreBy,
                                oldTodoText = active.TodoText,
                                oldTodoHours = active.AssignedHours,
                                oldTodoType = active.TodoType
                            });
                        }

                        if (todoListActive != null)
                        {
                            todoListActive = todoListActive.OrderBy(z => z.Priority).ToList();
                        }
                        /* Hold: Assigned and UnAssigned */
                        foreach (var hold in todoHold)
                        {
                            todoListHold.Add(new PMSTodoListSpViewModel
                            {
                                TodoId = hold.TodoId,
                                TodoText = hold.TodoText,
                                AssignedUser = hold.AssignedUser,
                                AssignedUserFullName = hold.AssignedUserFullName,
                                AssignedHours = hold.AssignedHours,
                                ActualHours = hold.ActualHours,
                                TodoType = hold.TodoType,
                                DueDate = hold.DueDate,
                                TotalComments = hold.TotalComments,
                                Status = hold.Status,
                                IsArchived = hold.IsArchived,
                                Priority = hold.Priority,
                                IsEdit = false,
                                IsCanFinish = hold.IsCanFinish,
                                creById = hold.creById,
                                CreDate=hold.CreDate,
                                CreBy=hold.CreBy,
                                oldTodoText = hold.TodoText,
                                oldTodoHours = hold.AssignedHours,
                                oldTodoType = hold.TodoType
                            });
                        }

                        if (todoListHold != null)
                        {
                            todoListHold = todoListHold.OrderBy(z => z.Priority).ToList();
                        }

                        /* Finish: Assigned and UnAssigned */
                        foreach (var finish in todoFinished)
                        {
                            todoListFinished.Add(new PMSTodoListSpViewModel
                            {
                                TodoId = finish.TodoId,
                                TodoText = finish.TodoText,
                                AssignedUser = finish.AssignedUser,
                                AssignedUserFullName = finish.AssignedUserFullName,
                                AssignedHours = finish.AssignedHours,
                                ActualHours = finish.ActualHours,
                                TodoType = finish.TodoType,
                                DueDate = finish.DueDate,
                                TotalComments = finish.TotalComments,
                                Status = finish.Status,
                                IsArchived = finish.IsArchived,
                                Priority = finish.Priority,
                                IsEdit = false,
                                IsCanFinish = finish.IsCanFinish,
                                creById = finish.creById,
                                CreDate=finish.CreDate,
                                CreBy=finish.CreBy,
                                oldTodoText = finish.TodoText,
                                oldTodoHours = finish.AssignedHours,
                                oldTodoType = finish.TodoType
                            });
                        }

                        if (todoListFinished != null)
                        {
                            todoListFinished = todoListFinished.OrderBy(z => z.Priority).ToList();
                        }


                        /* Archived */
                        foreach (var archived in todoArchived)
                        {
                            todoListArchived.Add(new PMSTodoListSpViewModel
                            {
                                TodoId = archived.TodoId,
                                TodoText = archived.TodoText,
                                AssignedUser = archived.AssignedUser,
                                AssignedUserFullName = archived.AssignedUserFullName,
                                AssignedHours = archived.AssignedHours,
                                ActualHours = archived.ActualHours,
                                TodoType = archived.TodoType,
                                DueDate = archived.DueDate,
                                TotalComments = archived.TotalComments,
                                Status = archived.Status,
                                IsArchived = archived.IsArchived,
                                Priority = archived.Priority,
                                IsEdit = false,
                                IsCanFinish = archived.IsCanFinish,
                                creById = archived.creById,
                                CreDate=archived.CreDate,
                                CreBy=archived.CreBy,
                                oldTodoText = archived.TodoText,
                                oldTodoHours = archived.AssignedHours,
                                oldTodoType = archived.TodoType
                            });
                        }

                        if (todoListArchived != null)
                        {
                            todoListArchived = todoListArchived.OrderBy(z => z.Priority).ToList();
                        }

                        
                        spModuleListViewModel.Add(new PMSModuleListSpViewModel
                        {
                            ModuleId = model.ModuleId,
                            ProjectId = model.ProjectId,
                            ProjectName = model.ProjectName,
                            ModuleName = model.ModuleName,
                            ModuleType = model.ModuleType,
                            Priority= model.Priority,
                            ActiveStatusCount = model.ActiveStatusCount,
                            totalTaskCount = model.totalTaskCount,
                            ActualHours = model.ActualHours,
                            AssignedHours = model.AssignedHours,
                            IsArchived = model.IsArchived,
                            TodoList = todoListActive,
                            TodoListHold = todoListHold,
                            TodoListFinished = todoListFinished,
                            TodoListArchived = todoListArchived
                        });
                    }

                    if (spModuleListViewModel != null)
                    {
                        spModuleListViewModel = spModuleListViewModel.Select(i =>
                        {
                            return i;
                        }).OrderBy(z => z.Priority).ToList();
                    }
                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", spModuleListViewModel);

                    //NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    //int timezone = Convert.ToInt32(nvc["timezone"].ToString());
                    //int moduleId = Convert.ToInt32(nvc["moduleId"].ToString());
                    ////int projectId = db.tblPMSModules.Where(z => z.ModuleId == moduleId).Select(z => z.ProjectId).SingleOrDefault();

                    //List<PMSModuleTodoViewModel> main = new List<PMSModuleTodoViewModel>();

                    //var list = db.tblPMSModules.Where(z => z.ModuleId == moduleId && z.IsArchived == false).ToList();
                    //var ActiveStatusCount = db.tblPMSToDoes.Where(z => z.ModuleId == moduleId && z.IsArchived == false && z.Status == 1).Count();//Status 1:Active, 2:Hold, 3:Finished
                    //var totalTaskCount = db.tblPMSToDoes.Where(z => z.ModuleId == moduleId && z.IsArchived == false).Count();//Status 1:Active, 2:Hold, 3:Finished

                    //foreach (var l in list)
                    //{
                    //    List<PMSTodo> todoListActive = new List<PMSTodo>();
                    //    List<PMSTodo> todoListHold = new List<PMSTodo>();
                    //    List<PMSTodo> todoListFinished = new List<PMSTodo>();
                    //    List<PMSTodo> todoListArchived = new List<PMSTodo>();

                    //    //Status 1:Active, 2:Hold, 3:Finished
                    //    var todoActive = db.tblPMSToDoes.Where(z => z.ModuleId == l.ModuleId && z.IsArchived == false && z.Status == 1).ToList();
                    //    var todoHold = db.tblPMSToDoes.Where(z => z.ModuleId == l.ModuleId && z.IsArchived == false && z.Status == 2).ToList();
                    //    var todoFinished = db.tblPMSToDoes.Where(z => z.ModuleId == l.ModuleId && z.IsArchived == false && z.Status == 3).ToList();
                    //    var todoArchived = db.tblPMSToDoes.Where(z => z.ModuleId == l.ModuleId && z.IsArchived == true).ToList();
                    //    //var todoActive = db.tblPMSToDoes.Where(z => z.ModuleId == l.ModuleId).ToList();

                    //    /*Active: Assigned and UnAssigned*/
                    //    foreach (var t in todoActive)
                    //    {
                    //        string fullName = "UnAssigned";
                    //        if (t.AssignedUser != 0)
                    //        {
                    //            var emp = db.tblEmpPersonalInformations.Where(z => z.EmployeeId == t.AssignedUser).FirstOrDefault();
                    //            fullName = string.Format("{0} {1}", emp.CandidateFirstName, emp.CandidateLastName);
                    //        }

                    //        todoListActive.Add(new PMSTodo
                    //        {
                    //            TodoId = t.TodoId,
                    //            ModuleId = t.ModuleId,
                    //            TodoText = t.TodoText,
                    //            AssignedUser = t.AssignedUser,
                    //            AssignedHours = t.AssignedHours,
                    //            ActualHours = db.tblPMSComments.Where(z => z.TodoId == t.TodoId).Sum(z => z.Hours),
                    //            AssignedUserFullName = fullName,
                    //            Priority = t.Priority,
                    //            DueDate = t.DueDate,
                    //            TotalComments = db.tblPMSComments.Where(z => z.TodoId == t.TodoId).Count(),
                    //            IsEdit = false,
                    //            oldTodoText = t.TodoText,
                    //            oldTodoHours = t.AssignedHours,
                    //            Status = t.Status
                    //        });
                    //    }

                    //    /*Hold*/
                    //    foreach (var t in todoHold)
                    //    {
                    //        string fullName = "UnAssigned";
                    //        if (t.AssignedUser != 0)
                    //        {
                    //            var emp = db.tblEmpPersonalInformations.Where(z => z.EmployeeId == t.AssignedUser).FirstOrDefault();
                    //            fullName = string.Format("{0} {1}", emp.CandidateFirstName, emp.CandidateLastName);
                    //        }

                    //        todoListHold.Add(new PMSTodo
                    //        {
                    //            TodoId = t.TodoId,
                    //            ModuleId = t.ModuleId,
                    //            TodoText = t.TodoText,
                    //            AssignedUser = t.AssignedUser,
                    //            AssignedUserFullName = fullName,
                    //            Priority = t.Priority,
                    //            DueDate = t.DueDate,
                    //            TotalComments = db.tblPMSComments.Where(z => z.TodoId == t.TodoId).Count(),
                    //            IsEdit = false,
                    //            oldTodoText = t.TodoText,
                    //            AssignedHours = t.AssignedHours,
                    //            ActualHours = db.tblPMSComments.Where(z => z.TodoId == t.TodoId).Sum(z => z.Hours),
                    //            Status = t.Status
                    //        });
                    //    }

                    //    /*Finished*/
                    //    foreach (var t in todoFinished)
                    //    {
                    //        string fullName = "UnAssigned";
                    //        if (t.AssignedUser != 0)
                    //        {
                    //            var emp = db.tblEmpPersonalInformations.Where(z => z.EmployeeId == t.AssignedUser).FirstOrDefault();
                    //            fullName = string.Format("{0} {1}", emp.CandidateFirstName, emp.CandidateLastName);
                    //        }
                    //        bool isDelayed = false;
                    //        if (t.EndDate != null && t.DueDate != null)
                    //        {
                    //            DateTime edate = Convert.ToDateTime(t.EndDate);
                    //            DateTime ddate = Convert.ToDateTime(t.DueDate);
                    //            int days = (edate.Date - ddate.Date).Days;
                    //            if (days > 0)
                    //            {
                    //                isDelayed = true;
                    //            }
                    //        }

                    //        todoListFinished.Add(new PMSTodo
                    //        {
                    //            TodoId = t.TodoId,
                    //            ModuleId = t.ModuleId,
                    //            TodoText = t.TodoText,
                    //            AssignedUser = t.AssignedUser,
                    //            AssignedUserFullName = fullName,
                    //            Priority = t.Priority,
                    //            DueDate = t.DueDate,
                    //            TotalComments = db.tblPMSComments.Where(z => z.TodoId == t.TodoId).Count(),
                    //            IsEdit = isDelayed, //True: finish date > duedate, Else: false
                    //            oldTodoText = t.TodoText,
                    //            AssignedHours = t.AssignedHours,
                    //            ActualHours = db.tblPMSComments.Where(z => z.TodoId == t.TodoId).Sum(z => z.Hours),
                    //            Status = t.Status
                    //        });
                    //    }

                    //    /*Archived*/
                    //    foreach (var t in todoArchived)
                    //    {
                    //        string fullName = "UnAssigned";
                    //        if (t.AssignedUser != 0)
                    //        {
                    //            var emp = db.tblEmpPersonalInformations.Where(z => z.EmployeeId == t.AssignedUser).FirstOrDefault();
                    //            fullName = string.Format("{0} {1}", emp.CandidateFirstName, emp.CandidateLastName);
                    //        }

                    //        todoListArchived.Add(new PMSTodo
                    //        {
                    //            TodoId = t.TodoId,
                    //            ModuleId = t.ModuleId,
                    //            TodoText = t.TodoText,
                    //            AssignedUser = t.AssignedUser,
                    //            AssignedUserFullName = fullName,
                    //            Priority = t.Priority,
                    //            DueDate = t.DueDate,
                    //            TotalComments = db.tblPMSComments.Where(z => z.TodoId == t.TodoId).Count(),
                    //            IsEdit = false,
                    //            oldTodoText = t.TodoText,
                    //            Status = t.Status
                    //        });
                    //    }

                    //    todoListActive = todoListActive != null ? todoListActive.OrderBy(z => z.Priority).ToList() : null;
                    //    todoListHold = todoListHold != null ? todoListHold.OrderBy(z => z.Priority).ToList() : null;
                    //    todoListFinished = todoListFinished != null ? todoListFinished.OrderBy(z => z.Priority).ToList() : null;
                    //    todoListArchived = todoListArchived != null ? todoListArchived.OrderBy(z => z.Priority).ToList() : null;

                    //    ERPUtils erpUtils = new ERPUtils();
                    //    List<PMSTodo> mergedList = todoListActive.Concat(todoListHold).Concat(todoListFinished).ToList();
                    //    List<decimal> finalHours = erpUtils.CalculateHoursForModule(mergedList);

                    //    main.Add(new PMSModuleTodoViewModel
                    //    {
                    //        ModuleId = l.ModuleId,
                    //        ProjectId = l.ProjectId,
                    //        ProjectName = db.tblPMSProjects.Where(z => z.ProjectId == l.ProjectId).Select(z => z.ProjectName).SingleOrDefault(),
                    //        ModuleName = l.ModuleName,
                    //        Priority = l.Priority,
                    //        IsArchived = l.IsArchived,
                    //        ActiveStatusCount = ActiveStatusCount,
                    //        totalTaskCount = totalTaskCount,
                    //        IsActive = l.IsActive,
                    //        CreDate = l.CreDate,
                    //        ChgDate = l.ChgDate,
                    //        ChgBy = l.ChgBy,
                    //        TodoList = todoListActive,
                    //        TodoListHold = todoListHold,
                    //        TodoListFinished = todoListFinished,
                    //        TodoListArchived = todoListArchived,
                    //        //ActualHours = todoListActive.Sum(ta => ta.ActualHours) + todoListFinished.Sum(tf => tf.ActualHours) + todoListHold.Sum(th => th.ActualHours),
                    //        //AssignedHours = todoListActive.Sum(ta => ta.AssignedHours) + todoListFinished.Sum(tf => tf.AssignedHours) + todoListHold.Sum(th => th.AssignedHours)
                    //        ActualHours = finalHours[0],
                    //        AssignedHours = finalHours[1]
                    //    });
                    //}
                    //if (main != null)
                    //{
                    //    main = main.Select(i =>
                    //    {
                    //        i.CreDate = Convert.ToDateTime(i.CreDate).AddMinutes(-1 * timezone);
                    //        i.ChgDate = Convert.ToDateTime(i.ChgDate).AddMinutes(-1 * timezone);
                    //        return i;
                    //    }).OrderBy(z => z.Priority).ToList();
                    //}

                    //apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", main);



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
        /// delete module
        /// </summary>
        [HttpGet]
        public ApiResponse DeleteModule()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int moduleId = Convert.ToInt32(nvc["moduleId"].ToString());

                    var line = db.tblPMSModules.Where(z => z.ModuleId == moduleId).SingleOrDefault();
                    if (line != null)
                    {
                        line.IsArchived = true;
                        line.ChgDate = DateTime.Now.ToUniversalTime();
                        line.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());

                        var todolist = db.tblPMSToDoes.Where(z=>z.ModuleId == line.ModuleId).ToList();
                        foreach (var t in todolist) {
                            t.IsArchived = true;
                            t.ChgDate = DateTime.Now.ToUniversalTime();
                            t.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());

                            var commentlist = db.tblPMSComments.Where(z => z.TodoId == t.TodoId).ToList();
                            foreach (var c in commentlist) {
                              //  c.IsActive = false;
                                c.IsArchived = true;
                                // Comment file table has no IsActive column; so it's not needed to inactive.
                            }
                        }
                    }

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgDelete, true);
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
        /// GET api/PMSModule
        /// sort module
        /// </summary>
        [HttpGet]
        public ApiResponse DoSortingForModule()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    string sortOrder = nvc["sortOrder"].ToString();
                    if (!string.IsNullOrEmpty(sortOrder))
                    {
                        foreach (var word in sortOrder.Split(','))
                        {
                            if (!string.IsNullOrEmpty(word))
                            {
                                var data = word.Split(':');
                                //data[0] = module id
                                //data[1] = index number
                                var line = db.tblPMSModules.AsEnumerable().Where(z => z.ModuleId == Convert.ToInt32(data[0])).SingleOrDefault();
                                if (line != null)
                                {
                                    line.Priority = Convert.ToInt32(data[1]);
                                    line.ChgDate = DateTime.Now.ToUniversalTime();
                                    line.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                                }
                            }
                        }
                    }

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgUpdate, true);
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
        /// GET api/PMSModule
        /// sort module todo's
        /// </summary>
        [HttpGet]
        public ApiResponse DoSortingForTodo()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    string sortOrder = nvc["sortOrder"].ToString();
                    if (!string.IsNullOrEmpty(sortOrder))
                    {
                        foreach (var word in sortOrder.Split(','))
                        {
                            if (!string.IsNullOrEmpty(word))
                            {
                                var data = word.Split(':');
                                //data[0] = todo id
                                //data[1] = index number
                                var line = db.tblPMSToDoes.AsEnumerable().Where(z => z.TodoId == Convert.ToInt32(data[0])).SingleOrDefault();
                                if (line != null)
                                {
                                    line.Priority = Convert.ToInt32(data[1]);
                                    line.ChgDate = DateTime.Now.ToUniversalTime();
                                    line.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                                }
                            }
                        }
                    }

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMsg.msgUpdate, true);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageNameForTodo, true);
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
        /// save Todo item
        /// </summary>
        [HttpPost]
        public ApiResponse SaveTodoItem(tblPMSToDo todo)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (todo.TodoId == 0)
                    {// Mode == Add
                        int prio;
                        try
                        {//calculate priority
                            prio = db.tblPMSToDoes.Max(z => z.Priority + 1);
                        }
                        catch
                        {
                            prio = 1;
                        }

                        tblPMSToDo d = new tblPMSToDo
                        {
                            ModuleId = todo.ModuleId,
                            TodoText = todo.TodoText,
                            AssignedUser = 0,
                            AssignedHours = todo.AssignedHours,
                            TodoType = todo.TodoType,
                            StartDate = DateTime.Now,
                            Status = 1, //1: Active, Ref:Status Enum in ERP.Utilities
                            Priority = prio,
                            IsArchived = false,
                            CanFinish = false,
                            IsActive = true,
                            CreDate = DateTime.Now.ToUniversalTime(),
                            CreBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString()),
                            ChgDate = DateTime.Now.ToUniversalTime(),
                            ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString())
                        };
                        db.tblPMSToDoes.Add(d);
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMsg.msgInsert, true);
                    }
                    else
                    {// Mode == Edit
                        var line = db.tblPMSToDoes.Where(z => z.TodoId == todo.TodoId).SingleOrDefault();
                        if (line != null)
                        {
                            line.TodoText = todo.TodoText;
                            line.AssignedHours = todo.AssignedHours;
                            line.TodoType = todo.TodoType;
                            line.ChgDate = DateTime.Now.ToUniversalTime();
                            line.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                        }
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMsg.msgUpdate, true);
                    }
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageNameForTodo, true);
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
        /// hold todo item
        /// </summary>
        [HttpGet]
        public ApiResponse HoldTodoItem()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int todoId = Convert.ToInt32(nvc["todoId"].ToString());

                    var line = db.tblPMSToDoes.Where(z => z.TodoId == todoId).SingleOrDefault();
                    if (line != null)
                    {
                        line.Status = 2;
                        line.Priority = 0;
                        line.ChgDate = DateTime.Now.ToUniversalTime();
                        line.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                    }

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMsg.msgHold, true);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageNameForTodo, true);
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
        /// delete todo item
        /// </summary>
        [HttpGet]
        public ApiResponse DeleteTodoItem()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int todoId = Convert.ToInt32(nvc["todoId"].ToString());

                    var line = db.tblPMSToDoes.Where(z => z.TodoId == todoId).SingleOrDefault();
                    if (line != null)
                    {
                        line.IsArchived = true;
                        line.Priority = 0;
                        line.ChgDate = DateTime.Now.ToUniversalTime();
                        line.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                    }

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMsg.msgDelete, true);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageNameForTodo, true);
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
        /// finish todo item
        /// </summary>
        [HttpGet]
        public ApiResponse FinishTodoItem()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int todoId = Convert.ToInt32(nvc["todoId"].ToString());

                    var AssignedHour = db.tblPMSToDoes.Where(z => z.TodoId == todoId).Select(z => z.AssignedHours).FirstOrDefault();
                    var ActualHours = (from comment in db.tblPMSComments
                               where comment.TodoId == todoId
                               group comment by comment.TodoId into g
                               select new { hours= g.Sum(z=>z.Hours)}.hours).FirstOrDefault();

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
                        var line = db.tblPMSToDoes.Where(z => z.TodoId == todoId).SingleOrDefault();
                        if (line != null)
                        {
                            line.Status = 3;
                            line.Priority = 0;
                            line.EndDate = DateTime.Now;
                            line.ChgDate = DateTime.Now.ToUniversalTime();
                            line.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                        }
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMsg.msgUpdate, true);
                        db.SaveChanges();
                    }
                    else {
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 0 ,"Assigned/Actual hours not define", true);
                    }

                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageNameForTodo, true);
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
        /// retrieve project assigned user list
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
                    int projectId = Convert.ToInt32(nvc["projectId"].ToString());

                    List<SelectItemModel> sList = new List<SelectItemModel>();

                    var list = db.tblPMSProjectUsers.Where(z => z.ProjectId == projectId).Select(z => new { EmployeeId = z.EmployeeId }).Distinct().ToList();
                    foreach (var l in list)
                    {
                        var emp = db.tblEmpPersonalInformations.Where(z => z.EmployeeId == l.EmployeeId).FirstOrDefault();
                        if (emp != null)
                        {
                            sList.Add(new SelectItemModel
                            {
                                Id = emp.EmployeeId,
                                Label = string.Format("{0}{1}{2}", emp.CandidateFirstName, emp.CandidateMiddleName.Substring(0, 1).ToUpper(), emp.CandidateLastName.Substring(0, 1).ToUpper())
                            });
                        }
                    }

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", sList);
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageNameForTodo, true);
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
        /// assign user to todo
        /// </summary>
        [HttpGet]
        public ApiResponse AssignUser()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    string filteredDate = nvc["filteredDate"].ToString();
                    int userId = Convert.ToInt32(nvc["userId"].ToString());
                    int projectid = Convert.ToInt32(nvc["projectId"].ToString());
                    int todoId = Convert.ToInt32(nvc["todoId"].ToString());
                    Boolean iscanfinish = Convert.ToBoolean(nvc["iscanfinish"].ToString());

                    DateTime dtDue = new DateTime();

                    if (!string.IsNullOrEmpty(filteredDate))
                    {
                        //old
                        //string[] dt = Convert.ToDateTime(filteredDate).ToString("dd-MM-yyyy").Split('-');
                        //dtDue = new DateTime(Convert.ToInt32(dt[2]), Convert.ToInt32(dt[1]), Convert.ToInt32(dt[0]), 0, 0, 0);
                        dtDue = DateTime.ParseExact(filteredDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    }

                    var line = db.tblPMSToDoes.Where(z => z.TodoId == todoId).FirstOrDefault();
                    if (line != null)
                    {
                        line.AssignedUser = userId;
                        if (!string.IsNullOrEmpty(filteredDate))
                        {
                            line.DueDate = dtDue;
                        }
                        else
                        {
                            line.DueDate = null;
                        }
                    
                        line.CanFinish = iscanfinish;
                        line.ChgDate = DateTime.Now.ToUniversalTime();
                        line.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                    }

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMsg.msgUpdate, true);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageNameForTodo, true);
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
        /// Active todo item from unhold, finished and archived list
        /// </summary>
        [HttpGet]
        public ApiResponse ActiveTodoItem()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int todoId = Convert.ToInt32(nvc["todoId"].ToString());
                    string changeFor = nvc["for"].ToString();

                    var line = db.tblPMSToDoes.Where(z => z.TodoId == todoId).SingleOrDefault();
                    if (line != null)
                    {
                        int prio;
                        try
                        {//calculate priority
                            prio = db.tblPMSToDoes.Where(x => x.ModuleId == line.ModuleId).OrderByDescending(x => x.Priority).Take(1).Select(z => z.Priority).SingleOrDefault();
                            prio += 1;
                        }
                        catch
                        {
                            prio = 1;
                        }

                        line.Status = 1;
                        line.Priority = prio;

                        line.IsArchived = false;

                        line.ChgDate = DateTime.Now.ToUniversalTime();
                        line.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                    }
                    if (changeFor == "hold")
                    {
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMsg.msgUnHold, true);
                    }
                    else
                    {
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMsg.msgUpdate, true);
                    }

                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageNameForTodo, true);
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
        /// retrieve module type list
        /// </summary>
        [HttpGet]
        public ApiResponse GetModuleTypeList()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    List<SelectItemModel> list = new List<SelectItemModel>();

                    foreach (int value in Enum.GetValues(typeof(ERPUtilities.ModuleType)))
                    {
                        list.Add(new SelectItemModel
                        {
                            Id = value,
                            Label = Enum.GetName(typeof(ERPUtilities.ModuleType), value)
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

        [HttpGet]
        public ApiResponse getAllProjectName()
        {

            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {

                try
                {
                    List<PMSProjectProgressViewModel> list = new List<PMSProjectProgressViewModel>();

                    //list = (from project in db.tblPMSProjects
                    //        join module in db.tblPMSModules on project.ProjectId equals module.ProjectId
                    //        join todo in db.tblPMSToDoes on module.ModuleId equals todo.ModuleId
                    //     //   join comment in db.tblPMSComments on todo.TodoId equals comment.TodoId
                    //        group todo.AssignedHours  by project.ProjectName into p
                    //        select new PMSProjectProgressViewModel
                    //        {
                    //            ProjectName = p.Key,
                    //            AssignedHours = p.Sum()
                    //        }).ToList();

                    var list1 = (from project in db.tblPMSProjects
                                 join module in db.tblPMSModules on project.ProjectId equals module.ProjectId into module_join
                                 from module in module_join.DefaultIfEmpty()
                                 join todo in db.tblPMSToDoes on module.ModuleId equals todo.ModuleId into todo_join
                                 from todo in todo_join.DefaultIfEmpty()
                                 join actual_time in
                                     (
                                        (from comment in db.tblPMSComments
                                         group comment by new
                                         {
                                             comment.TodoId
                                         } into g
                                         select new
                                         {
                                             g.Key.TodoId,
                                             h = (decimal?)g.Sum(p => p.Hours)
                                         })) on new { TodoId = todo.TodoId } equals new { TodoId = actual_time.TodoId } into actual_time_join
                                 from actual_time in actual_time_join.DefaultIfEmpty()
                                 group new { project, todo, actual_time } by new
                                 {
                                     project.ProjectName
                                 } into g
                                 select new
                                 {
                                     g.Key.ProjectName,
                                     AssignedHours = (decimal?)g.Sum(p => p.todo.AssignedHours),
                                     Hours = (decimal?)g.Sum(p => p.actual_time.h)
                                 }).ToList();



                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", list1);
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
