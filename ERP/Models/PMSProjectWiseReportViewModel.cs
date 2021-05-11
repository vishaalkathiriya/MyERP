using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Models
{
    public class PMSProjectWiseReportViewModel
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public DateTime? workDate { get; set; }
        public decimal projectTotalwork { get; set; }
        public decimal? grantTotal { get; set; }
        public List<pmsProjectWiseWeekReport> userWiseKeekList { get; set; }

        public List<UserListViewModel> UserList { get; set; }
    }

    public class UserListViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public decimal userTotalwork { get; set; }
        public List<UserTodoViewModel> TodoList { get; set; }

    }

    public class UserTodoViewModel
    {
        public int TodoId { get; set; }
        public string TodoText { get; set; }
        public DateTime? TodoDate { get; set; }
        public decimal? ActualHours { get; set; }
        public String CommentText { get; set; }
    }

    public class pmsProjectWiseWeekReport
    {
        public int wkNo { get; set; }
        public String week { get; set; }
        public decimal? sun { get; set; }
        public decimal? mon { get; set; }
        public decimal? tue { get; set; }
        public decimal? wed { get; set; }
        public decimal? thu { get; set; }
        public decimal? fri { get; set; }
        public decimal? sat { get; set; }
        public decimal? weekTotal { get; set; }
        // public decimal? grantTotal { get; set; }
    }
}