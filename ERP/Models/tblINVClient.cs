using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblINVClient
    {
        public tblINVClient()
        {
            this.tblINVClientPersons = new List<tblINVClientPerson>();
            this.tblINVConversations = new List<tblINVConversation>();
            this.tblINVInquiries = new List<tblINVInquiry>();
            this.tblINVInvoices = new List<tblINVInvoice>();
        }

        public int PKClientId { get; set; }
        public string ClientCode { get; set; }
        public string CompanyName { get; set; }
        public string CPrefix { get; set; }
        public string ContactPerson { get; set; }
        public string MobileNo { get; set; }
        public string CompanyAddress { get; set; }
        public Nullable<int> FKSourceId { get; set; }
        public int CountryId { get; set; }
        public int StateId { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string TelephoneNo { get; set; }
        public string FaxNo { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public Nullable<int> BusinessTypeId { get; set; }
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
        public bool IsConfirmed { get; set; }
        public Nullable<bool> IsKYCApproved { get; set; }
        public bool IsDeleted { get; set; }
        public string URLKey { get; set; }
        public bool IsActive { get; set; }
        public int CreBy { get; set; }
        public int ChgBy { get; set; }
        public System.DateTime CreDate { get; set; }
        public System.DateTime ChgDate { get; set; }
        public virtual ICollection<tblINVClientPerson> tblINVClientPersons { get; set; }
        public virtual ICollection<tblINVConversation> tblINVConversations { get; set; }
        public virtual ICollection<tblINVInquiry> tblINVInquiries { get; set; }
        public virtual ICollection<tblINVInvoice> tblINVInvoices { get; set; }
    }
}
