using System;
using System.Collections.Generic;

namespace ERP.Models
{
    public partial class tblPMSCommentFile
    {
        public int UploadedFileId { get; set; }
        public int CommentId { get; set; }
        public string FileName { get; set; }
        public string CaptionText { get; set; }
        public System.DateTime CreDate { get; set; }
        public int CreBy { get; set; }
        public virtual tblPMSComment tblPMSComment { get; set; }
    }
}
