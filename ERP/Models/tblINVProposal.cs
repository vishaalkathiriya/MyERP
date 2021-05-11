using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblINVProposal
    {
        public int PKProposalId { get; set; }
        public int FKInquiryId { get; set; }
        public string ProposalTitle { get; set; }
        public System.DateTime ProposalDate { get; set; }
        public bool IsFinalized { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int CreBy { get; set; }
        public int ChgBy { get; set; }
        public System.DateTime CreDate { get; set; }
        public System.DateTime ChgDate { get; set; }
        public virtual tblINVInquiry tblINVInquiry { get; set; }
    }
}
