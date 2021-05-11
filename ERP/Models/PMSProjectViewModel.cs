using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Models
{
    public class PMSProjectViewModel
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string Technologies { get; set; }
        public string TechnologyName { get; set; }
        public int ProjectType { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public decimal TotalEstDays { get; set; }
        public int Status { get; set; }
        public string Description { get; set; }
        public bool IsArchived { get; set; }
        public System.DateTime ChgDate { get; set; }
        public List<PMSProjectsers> SelectedUsers { get; set; }
        public decimal finishTaskCount { get; set; }
        public decimal totalTaskCount { get; set; }
        public decimal AssignedHours { get; set; }
        public decimal Hours { get; set; }
    }




    public class PMSProjectSpViewModel
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string TechnologyId { get; set; }
        public string  Technologies { get; set; }
        public string Description { get; set; }
        public int ProjectType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalEstDays { get; set; }
        public int Status { get; set; }
        public bool IsArchived { get; set; }
        public string ProjectUser { get; set; }
        public decimal EHours { get; set; }
        public decimal WHours { get; set; }
        public int NoOfToDo { get; set; }
        public int CNoOfToDo { get; set; }
        public DateTime ChgDate { get; set; }
        public List<PMSProjectsers> SelectedUsers { get; set; }
    }

    public class PMSProjectsers
    {
        public int LeadId { get; set; }
        public string LeadName { get; set; }
        public string ProfilePix { get; set; }
        public List<SelectedUsers> SelectedUsers { get; set; }
    }

    public class SelectedUsers
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public bool IsSelected { get; set; }
        public string ProfilePix { get; set; }
    }



}