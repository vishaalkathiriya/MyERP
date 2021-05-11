using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblHRDMedicalHelp
    {
        public int SrNo { get; set; }
        public string ECode { get; set; }
        public string EmployeeName { get; set; }
        public string PatientName { get; set; }
        public string Relation { get; set; }
        public string HospitalName { get; set; }
        public System.DateTime ChequeIssueDate { get; set; }
        public string ChequeNumber { get; set; }
        public string ReceiverName { get; set; }
        public string MobileNumber { get; set; }
        public Nullable<decimal> QuotationAmount { get; set; }
        public decimal Amount { get; set; }
        public string ApprovedBy { get; set; }
        public string Attachment { get; set; }
        public bool IsPatelSocialGroup { get; set; }
        public System.DateTime CreDate { get; set; }
        public int CreBy { get; set; }
        public System.DateTime ChgDate { get; set; }
        public int ChgBy { get; set; }
    }
}
