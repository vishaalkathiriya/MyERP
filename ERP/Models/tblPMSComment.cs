using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblPMSComment
    {
        public tblPMSComment()
        {
            this.tblPMSCommentFiles = new List<tblPMSCommentFile>();
        }

        public int CommentId { get; set; }
        public int TodoId { get; set; }
        public string CommentText { get; set; }
        public Nullable<decimal> Hours { get; set; }
        public bool IsArchived { get; set; }
        public System.DateTime CreDate { get; set; }
        public int CreBy { get; set; }
        public virtual tblPMSToDo tblPMSToDo { get; set; }
        public virtual ICollection<tblPMSCommentFile> tblPMSCommentFiles { get; set; }
    }
}
