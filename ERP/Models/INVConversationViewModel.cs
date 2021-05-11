using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Models
{
    public class INVConversationViewModel
    {
        public int PKConversationId { get; set; }
        public int FKClientId { get; set; }
        public string CompanyName { get; set; }
        public string ClientCode { get; set; }
        public string ClientName { get; set; }
        public string ClientMobile { get; set; }
        public string ClientEmail { get; set; }
        public string ContentType { get; set; }
        public int ConversationType { get; set; }
        public string ConversationTitle  { get; set; }
        public string ConversationDescription { get; set; }
        public DateTime ConversationDate { get; set; }
        public List<INVDocumentViewModel> DocumentList { get; set; }
    }
    public class INVDocumentViewModel {
        public int PKDocId { get; set; }
        public int DocId { get; set; }
        public int tblRefId { get; set; }
        public string DocName { get; set; }
        public int DocType { get; set; }
        public string DocRemark { get; set; }
        public string DocFileType { get; set; }
        public string DocTypeName { get; set; }
    }
}