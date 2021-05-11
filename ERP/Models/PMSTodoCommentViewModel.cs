using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Models
{
    public class PMSTodoCommentViewModel
    {
        public int CommentId { get; set; }
        public int TodoId { get; set; }
        public string CommentText { get; set; }
        public Boolean IsDelete { get; set; }
        public Nullable<decimal> Hours { get; set; }
        public System.DateTime CreDate { get; set; }
        public string CreByName { get; set; }
        public string ProfilePix { get; set; }
        public List<tblPMSCommentFile> lstUploadedFile { get; set; }
    }

    public class PMSTodoModuleCommentViewModel
    {
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public string todoCommentText { get; set; }
        public int CreBy { get; set; }
        public int? AssignUser { get; set; }
        public Boolean? CanFinish { get; set; }
        public string TodoText { get; set; }
    }
}