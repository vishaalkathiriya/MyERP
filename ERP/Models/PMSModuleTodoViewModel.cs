using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Models
{
    public class PMSModuleTodoViewModel
    {
        public int ModuleId { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ModuleName { get; set; }
        public int? ModuleType { get; set; }
        public int Priority { get; set; }
        public decimal? AssignedHours { get; set; }
        public decimal? ActualHours { get; set; }
        public int ActiveStatusCount { get; set; }
        public int totalTaskCount { get; set; }
        public bool IsArchived { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreDate { get; set; }
        public int CreBy { get; set; }
        public System.DateTime ChgDate { get; set; }
        public int ChgBy { get; set; }
        public List<PMSTodo> TodoList { get; set; }
        public List<PMSTodo> TodoListHold { get; set; }
        public List<PMSTodo> TodoListFinished { get; set; }
        public List<PMSTodo> TodoListArchived { get; set; }
    }

    public class PMSTodo
    {
        public int TodoId { get; set; }
        public int ModuleId { get; set; }
        public string TodoText { get; set; }
        public int? AssignedUser { get; set; }
        public decimal? AssignedHours { get; set; }
        public decimal? ActualHours { get; set; }
        public int TodoType { get; set; }
        public string AssignedUserFullName { get; set; }
        public int Priority { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public int TotalComments { get; set; }
        public int Status { get; set; }
        public bool IsEdit { get; set; }
        public string oldTodoText { get; set; }
        public decimal? oldTodoHours { get; set; }
        public bool IsChecked { get; set; }

        //use for notification
        public string ProjectName { get; set; }
        public string ModuleName { get; set; }
    }


    public class PMSModuleListSpViewModel
    {
        public int ModuleId { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ModuleName { get; set; }
        public int ModuleType { get; set; }
        public int? Priority { get; set; }
        public bool IsArchived { get; set; }
        public int ActiveStatusCount { get; set; }
        public int totalTaskCount { get; set; }
        public decimal ActualHours { get; set; }
        public decimal AssignedHours { get; set; }
        public List<PMSTodoListSpViewModel> TodoList { get; set; }
        public List<PMSTodoListSpViewModel> TodoListHold { get; set; }
        public List<PMSTodoListSpViewModel> TodoListFinished { get; set; }
        public List<PMSTodoListSpViewModel> TodoListArchived { get; set; }
    }

    public class PMSTodoListSpViewModel
    {
        public int TodoId { get; set; }
        public string TodoText { get; set; }
        public int ModuleId { get; set; }
        public int? AssignedUser { get; set; }
        public string AssignedUserFullName { get; set; }
        public decimal? AssignedHours { get; set; }
        public decimal? ActualHours { get; set; }
        public bool IsArchived { get; set; }
        public int? TodoType { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public int? Status { get; set; }
        public int? TotalComments { get; set; }
        public bool IsEdit { get; set; }
        public bool IsChecked { get; set; }
        public int? Priority { get; set; }
        public string oldTodoText { get; set; }
        public decimal? oldTodoHours { get; set; }
        public int? oldTodoType { get; set; }
        public DateTime CreDate { get; set; }
        public string CreBy { get; set;}
        //urvish
        public int creById { get; set; }
        public Boolean? IsCanFinish { get; set; }
        //
    }


}
