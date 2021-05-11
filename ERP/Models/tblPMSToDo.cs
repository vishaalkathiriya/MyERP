using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblPMSToDo
    {
        public tblPMSToDo()
        {
            this.tblPMSComments = new List<tblPMSComment>();
        }

        public int TodoId { get; set; }
        public int ModuleId { get; set; }
        public string TodoText { get; set; }
        public Nullable<int> AssignedUser { get; set; }
        public Nullable<decimal> AssignedHours { get; set; }
        public Nullable<int> TodoType { get; set; }
        public System.DateTime StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public int Status { get; set; }
        public int Priority { get; set; }
        public bool IsArchived { get; set; }
        public bool IsActive { get; set; }
        public Nullable<bool> CanFinish { get; set; }
        public System.DateTime CreDate { get; set; }
        public int CreBy { get; set; }
        public System.DateTime ChgDate { get; set; }
        public int ChgBy { get; set; }
        public virtual ICollection<tblPMSComment> tblPMSComments { get; set; }
    }
}
