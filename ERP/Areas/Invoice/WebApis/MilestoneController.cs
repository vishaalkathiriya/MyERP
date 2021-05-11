using ERP.Models;
using ERP.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace ERP.Areas.Invoice.WebApis
{
    public class MilestoneController : ApiController
    {
        #region VARIABLE DECLARATION
        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Milestone";
        #endregion

        #region CONSTRUCTOR
        public MilestoneController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }
        #endregion

        #region RETRIEVE FUNCTIONS
        /// <summary>
        /// Retireive list of project milestones
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiResponse RetrieveProjectMilestones()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin()) 
            {
                try 
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int projectId = Convert.ToInt32(nvc["projectId"].ToString());

                    //var list = db.tblINVMilestones.Where(z => z.FKProjectId == projectId && z.IsActive == true && z.IsDeleted == false).ToList();
                    //apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", list);

                    List<InvoiceMilestoneViewModel> imList = new List<InvoiceMilestoneViewModel>();
                    SqlParameter paramProjectId = new SqlParameter();
                    paramProjectId = new SqlParameter("@ProjectId", projectId);
                    imList = db.Database.SqlQuery<InvoiceMilestoneViewModel>("usp_getMilestoneList @ProjectId ", paramProjectId).ToList();

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", imList);

                }
                catch (Exception ex) {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }
        #endregion

        #region CRUD FUNCTIONS
        /// <summary>
        /// Create or update project details
        /// </summary>
        [HttpPost]
        public ApiResponse CreateUpdateMilestone(tblINVMilestone milestone)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int timezone = Convert.ToInt32(nvc["timezone"].ToString());
                    int milestoneId = Convert.ToInt32(nvc["milestoneId"].ToString());

                    if (milestoneId > 0) { //Edit mode
                        var tbl = db.tblINVMilestones.Where(z => z.PKMilestoneId == milestoneId).SingleOrDefault();
                        if (tbl != null)
                        {
                            tbl.MilestoneName = milestone.MilestoneName;
                            tbl.MilestoneDesc = milestone.MilestoneDesc;
                            tbl.StartDate = milestone.StartDate == null ? (DateTime?)null : milestone.StartDate;
                            tbl.EndDate = milestone.EndDate == null ? (DateTime?)null : milestone.EndDate;
                            tbl.TotalHours = milestone.TotalHours;
                            tbl.Price = milestone.Price;
                            tbl.Currency = milestone.Currency;
                            tbl.IsActive = true;
                            tbl.IsDeleted = false;
                            tbl.ChgDate = DateTime.Now.ToUniversalTime();
                            tbl.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                        }

                        db.SaveChanges();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgUpdate, milestone.FKProjectId);
                    }
                    else { //Add mode
                        tblINVMilestone tbl = new tblINVMilestone
                        {
                            FKProjectId = milestone.FKProjectId,
                            MilestoneName = milestone.MilestoneName,
                            MilestoneDesc = milestone.MilestoneDesc,
                            StartDate = milestone.StartDate == null ? (DateTime?)null : milestone.StartDate,
                            EndDate = milestone.EndDate == null ? (DateTime?)null : milestone.EndDate,
                            TotalHours = milestone.TotalHours,
                            Price = milestone.Price,
                            Currency = milestone.Currency,
                            IsActive = true,
                            IsDeleted = false,
                            CreDate = DateTime.Now.ToUniversalTime(),
                            CreBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString()),
                            ChgDate = DateTime.Now.ToUniversalTime(),
                            ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString())
                        };
                        db.tblINVMilestones.Add(tbl);
                        db.SaveChanges();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, milestone.FKProjectId);
                    }
                }
                catch (Exception ex) {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
                }
            }
            else {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }
            return apiResponse;
        }

        [HttpGet]
        public ApiResponse DeleteProjectMilestone()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int milestoneId = Convert.ToInt32(nvc["milestoneId"].ToString());

                    var line = db.tblINVMilestones.Where(z => z.PKMilestoneId == milestoneId).SingleOrDefault();
                    if (line != null)
                    {
                        line.IsDeleted = true;
                        line.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                        line.ChgDate = DateTime.Now;
                        db.SaveChanges();
                    }

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgDelete, null);
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }
            return apiResponse;
        }
        #endregion
    }
}