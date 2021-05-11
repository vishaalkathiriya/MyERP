using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Models
{
    public class PMSProjectAndUserListViewModel
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public int UserId { get; set; }
        public string CandidateFirstName { get; set; }
        public string CandidateMiddleName { get; set; }
        public string CandidateLastName { get; set; }
        public int TodoId { get; set; }
        public string TodoText { get; set; }
        public DateTime TodoDate { get; set; }
        public int CommentId { get; set; }
        public decimal? ActualHours { get; set; }
        public string CommentText { get; set; }
    }

   
}