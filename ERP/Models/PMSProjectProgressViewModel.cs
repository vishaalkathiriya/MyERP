using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace ERP.Models
{
    public class PMSProjectProgressViewModel
    {


        public int ModuleId { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ModuleName { get; set; }
        public int TodoId { get; set; }
        public decimal? AssignedHours { get; set; }
        public decimal? ActualHours { get; set; }
        public int CommentId { get; set; }
        public Nullable<decimal> Hours { get; set; }

        

    }
}