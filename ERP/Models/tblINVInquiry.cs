using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblINVInquiry
    {
        public tblINVInquiry()
        {
            this.tblINVProjects = new List<tblINVProject>();
            this.tblINVProposals = new List<tblINVProposal>();
        }

        public int PKInquiryId { get; set; }
        public int FKClientId { get; set; }
        public string InquiryCode { get; set; }
        public string InquiryTitle { get; set; }
        public int InquiryStatus { get; set; }
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
        public virtual ICollection<tblINVProject> tblINVProjects { get; set; }
        public virtual ICollection<tblINVProposal> tblINVProposals { get; set; }
    }
}
