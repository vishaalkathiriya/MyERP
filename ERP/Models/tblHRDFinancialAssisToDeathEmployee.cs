using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblHRDFinancialAssisToDeathEmployee
    {
        public int SrNo { get; set; }
        public string Ecode { get; set; }
        public string EmployeeName { get; set; }
        public System.DateTime DateOfDeath { get; set; }
        public decimal Amount { get; set; }
        public string ChequeNumber { get; set; }
        public Nullable<System.DateTime> ChequeIssueDate { get; set; }
        public string ReceiveBy { get; set; }
        public string Relation { get; set; }
        public string FamilyBackgroundDetail { get; set; }
        public System.DateTime CreDate { get; set; }
        public int CreBy { get; set; }
        public System.DateTime ChgDate { get; set; }
        public int ChgBy { get; set; }
    }
}
