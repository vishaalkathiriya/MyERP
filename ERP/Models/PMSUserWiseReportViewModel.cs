using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Models
{
    public class PMSUserWiseReportViewModel
    {
        public int userId { get; set; }
        public string userName { get; set; }
        public DateTime? workDate { get; set; }
        public decimal  userTotalwork { get; set; }
        public decimal? grantTotal { get; set; }
        public List<projectListUserWiseViewModel> projectList { get; set; }
        public List<pmsUserWiseWeekReport> userWiseKeekList { get; set; }
       
    }

    public class projectListUserWiseViewModel
    {
        public int projectId { get; set; }
        public string projectName { get; set; }
        public decimal projectTotalwork { get; set; }
        public List<UserTodouserWiseViewModel> todoList { get; set; }
    }

    public class UserTodouserWiseViewModel
    {
        public int todoId { get; set; }
        public string todoText { get; set; }
        public DateTime? commentDate { get; set; }
        public decimal? actualHours { get; set; }
        public String commentText { get; set; }
    }


    public class pmsUserWiseWeekReport
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