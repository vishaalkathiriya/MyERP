using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblINVConversation
    {
        public int PKConversationId { get; set; }
        public int FKClientId { get; set; }
        public string ContentType { get; set; }
        public string ConversationTitle { get; set; }
        public string ConversationDescription { get; set; }
        public int ConversationType { get; set; }
        public System.DateTime ConversationDate { get; set; }
        public int ReferenceId { get; set; }
        public bool IsDeleted { get; set; }
        public int CreBy { get; set; }
        public int ChgBy { get; set; }
        public System.DateTime CreDate { get; set; }
        public System.DateTime ChgDate { get; set; }
        public virtual tblINVClient tblINVClient { get; set; }
    }
}
