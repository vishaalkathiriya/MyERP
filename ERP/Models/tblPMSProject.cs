using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblPMSProject
    {
        public tblPMSProject()
        {
            this.tblPMSProjectUsers = new List<tblPMSProjectUser>();
        }

        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string TechnologiesId { get; set; }
        public int ProjectType { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public decimal TotalEstDays { get; set; }
        public int Status { get; set; }
        public string Description { get; set; }
        public bool IsArchived { get; set; }
        public System.DateTime CreDate { get; set; }
        public int CreBy { get; set; }
        public System.DateTime ChgDate { get; set; }
        public int ChgBy { get; set; }
        public virtual ICollection<tblPMSProjectUser> tblPMSProjectUsers { get; set; }
    }
}
