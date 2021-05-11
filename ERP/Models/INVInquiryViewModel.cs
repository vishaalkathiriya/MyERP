using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Models
{
    public class INVInquiryViewModel
    {
        public int PKInquiryId { get; set; }
        public int FKClientId { get; set; }
        public string InquiryCode { get; set; }
        public string InquiryTitle { get; set; }
        public int InquiryStatus { get; set; }
        public string InquiryStatusName { get; set; }
        public System.DateTime InquiryDate { get; set; }
        public string FKTechnologyIds { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int CreBy { get; set; }
        public int ChgBy { get; set; }
        public System.DateTime CreDate { get; set; }
        public System.DateTime ChgDate { get; set; }
        public virtual tblINVClient tblINVClient { get; set; }
        public virtual tblINVClientSource tblINVClientSource { get; set; }
        public virtual ICollection<tblINVProposal> tblINVProposals { get; set; }

        public int PKProjectId { get; set; } // > 0 if confirmed inquiry as project
    }
}