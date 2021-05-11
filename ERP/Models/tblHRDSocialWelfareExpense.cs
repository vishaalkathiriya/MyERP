using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblHRDSocialWelfareExpense
    {
        public int SrNo { get; set; }
        public string ProgrammeName { get; set; }
        public string Venue { get; set; }
        public System.DateTime Date { get; set; }
        public string Time { get; set; }
        public decimal ExpenseAmount { get; set; }
        public string GuestName { get; set; }
        public string Attachment { get; set; }
        public System.DateTime CreDate { get; set; }
        public int CreBy { get; set; }
        public System.DateTime ChgDate { get; set; }
        public int ChgBy { get; set; }
    }
}
