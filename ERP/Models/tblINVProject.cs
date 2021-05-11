using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblINVProject
    {
        public tblINVProject()
        {
            this.tblINVMilestones = new List<tblINVMilestone>();
        }

        public int PKProjectId { get; set; }
        public int FKInquiryId { get; set; }
        public string ProjectTitle { get; set; }
        public int ProjectType { get; set; }
        public decimal Price { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public int ProjectStatus { get; set; }
        public decimal TotalHours { get; set; }
        public string Currency { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public System.DateTime CreDate { get; set; }
        public int CreBy { get; set; }
        public System.DateTime ChgDate { get; set; }
        public int ChgBy { get; set; }
        public string Remarks { get; set; }
        public virtual tblINVInquiry tblINVInquiry { get; set; }
        public virtual ICollection<tblINVMilestone> tblINVMilestones { get; set; }
    }
}
