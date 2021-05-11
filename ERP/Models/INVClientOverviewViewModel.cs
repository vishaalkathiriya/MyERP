using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Models
{
    public class INVClientOverviewViewModel
    {
        public int PKClientId { get; set; }
        public string ClientCode { get; set; }
        public string CompanyName { get; set; }
        public string CPrefix { get; set; }
        public string ContactPerson { get; set; }
        public string MobileNo { get; set; }
        public string CompanyAddress { get; set; }
        public string SourceName { get; set; }
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
        public string StateName { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string TelephoneNo { get; set; }
        public string FaxNo { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string BusinessType { get; set; }
        public Nullable<System.DateTime> BusinessStartDate { get; set; }
        public string LicenseNo { get; set; }
        public string IncomeTaxNo { get; set; }
        public string VATNo { get; set; }
        public string BankName { get; set; }
        public string BranchAddress { get; set; }
        public string BankType { get; set; }
        public string AccountNo { get; set; }
        public string IBANNumber { get; set; }
        public string SwiftCode { get; set; }
        public virtual ICollection<INVClientPersonViewModel> ClientPersonList { get; set; }
        public virtual ICollection<INVDocumentViewModel> ClientDocumentList { get; set; }
        
        public virtual ICollection<tblINVConversation> ClientConversationList { get; set; }
        public virtual ICollection<tblINVInquiry> ClientInquiryList { get; set; }
    }

    public class INVClientPersonViewModel {
        public int PKId { get; set; }
        public int FKClientId { get; set; }
        public string Prefix { get; set; }
        public string FullName { get; set; }
        public string Designation { get; set; }
        public string Identity { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
    }
}