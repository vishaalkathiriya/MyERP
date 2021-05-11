using ERP.Models;
using ERP.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace ERP.Areas.Invoice.WebApis
{
    public class ProjectController : ApiController
    {
        #region VARIABLE DECLARATION
        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Project";
        #endregion

        #region CONSTRUCTOR
        public ProjectController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }
        #endregion

        #region RETRIEVING INITIALIZATION DATA

        /// <summary>
        /// Retireive details of project if any for specified inquiry
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiResponse RetrieveProject()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int projectId = Convert.ToInt32(nvc["projectId"].ToString());

                    var line = db.tblINVProjects.Where(z => z.PKProjectId == projectId).SingleOrDefault();

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", line);
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

        /// <summary>
        /// Returns list of project statusses
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiResponse RetrieveProjectStatus()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    List<SelectItemModel> list = new List<SelectItemModel>();

                    foreach (int value in Enum.GetValues(typeof(ERPUtilities.ProjectStatus)))
                    {
                        list.Add(new SelectItemModel
                        {
                            Id = value,
                            Label = Enum.GetName(typeof(ERPUtilities.ProjectStatus), value)
                        });
                    }

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", list);
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

        /// <summary>
        /// Returns list of project types
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiResponse RetrieveProjectTypes()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    List<SelectItemModel> list = new List<SelectItemModel>();

                    foreach (int value in Enum.GetValues(typeof(ERPUtilities.ProjectTypes)))
                    {
                        list.Add(new SelectItemModel
                        {
                            Id = value,
                            Label = Enum.GetName(typeof(ERPUtilities.ProjectTypes), value)
                        });
                    }

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", list);
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

        /// <summary>
        /// Retireive confirmed project
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiResponse RetrieveConfirmedProjects()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int clientId = Convert.ToInt32(nvc["clientId"].ToString());
                    //int timezone = Convert.ToInt32(nvc["timezone"].ToString());

                    var list = (from p in db.tblINVProjects
                               join i in db.tblINVInquiries on p.FKInquiryId equals i.PKInquiryId
                               where i.FKClientId == clientId && p.IsActive == true && p.IsDeleted == false
                               select new
                               {
                                   p.PKProjectId,
                                   p.ProjectTitle,
                                   p.ProjectType,
                                   p.Price,
                                   p.ProjectStatus,
                                   p.Currency,
                                   i.FKClientId
                               }).ToList();

                    List<INVProjectViewModel> pList = new List<INVProjectViewModel>();
                    foreach (var l in list) {
                        pList.Add(new INVProjectViewModel
                        {
                            PKProjectId = l.PKProjectId,
                            ProjectTitle = l.ProjectTitle,
                            ProjectType = Enum.GetName(typeof(ERPUtilities.ProjectTypes), l.ProjectType),
                            ProjectStatus = Enum.GetName(typeof(ERPUtilities.ProjectStatus), l.ProjectStatus),
                            Currency=l.Currency,
                            Price = l.Price
                        });
                    }
                    //list = list.Select(i =>
                    //{
                    //    i.ProjectTitle
                    //    return i;
                    //}).OrderBy(z => z.ConversationDate).ToList();

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", pList);
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

        #region CRUD FUNCTIONS

        /// <summary>
        /// Create or update project details
        /// </summary>
        /// <param name="_tblINVProject"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResponse CreateUpdateProject(tblINVProject _tblINVProject)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int timezone = Convert.ToInt32(nvc["timezone"].ToString());

                    var line = db.tblINVProjects.Where(z => z.FKInquiryId == _tblINVProject.FKInquiryId).FirstOrDefault();
                    if (line != null) { //Edit mode
                        var tbl = db.tblINVProjects.Where(z => z.PKProjectId == line.PKProjectId).SingleOrDefault();
                        if (tbl != null)
                        {
                            tbl.FKInquiryId = _tblINVProject.FKInquiryId;
                            tbl.ProjectTitle = _tblINVProject.ProjectTitle;
                            tbl.ProjectType = _tblINVProject.ProjectType;
                            tbl.ProjectStatus = _tblINVProject.ProjectStatus;
                            tbl.Currency = _tblINVProject.Currency;
                            tbl.TotalHours = _tblINVProject.TotalHours;
                            tbl.StartDate = _tblINVProject.StartDate;
                            tbl.EndDate = _tblINVProject.EndDate;
                            tbl.Price = _tblINVProject.Price;
                            tbl.IsDeleted = _tblINVProject.IsDeleted;
                            tbl.Remarks = _tblINVProject.Remarks;
                            tbl.ChgDate = DateTime.Now.ToUniversalTime();
                            tbl.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                        }

                        db.SaveChanges();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgUpdate, line.PKProjectId);
                    }
                    else { //Add mode
                        tblINVProject tbl = new tblINVProject
                        {
                            FKInquiryId = _tblINVProject.FKInquiryId,
                            ProjectTitle = _tblINVProject.ProjectTitle,
                            ProjectType = _tblINVProject.ProjectType,
                            ProjectStatus = _tblINVProject.ProjectStatus,
                            TotalHours = _tblINVProject.TotalHours,
                            StartDate = _tblINVProject.StartDate,
                            EndDate = _tblINVProject.EndDate,
                            Price = _tblINVProject.Price,
                            IsActive = true,
                            IsDeleted = false,
                            Currency = _tblINVProject.Currency,
                            Remarks = _tblINVProject.Remarks,
                            CreDate = DateTime.Now.ToUniversalTime(),
                            CreBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString()),
                            ChgDate = DateTime.Now.ToUniversalTime(),
                            ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString())
                        };
                        db.tblINVProjects.Add(tbl);
                        db.SaveChanges();

                        //Check to change status of inquiry = Confirmed
                        if (tbl.FKInquiryId > 0) {
                            var inqLine = db.tblINVInquiries.Where(z => z.PKInquiryId == tbl.FKInquiryId).SingleOrDefault();
                            if (inqLine != null) {
                                inqLine.InquiryStatus = 5;
                                db.SaveChanges();
                            }
                        }

                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, tbl.PKProjectId);
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
        #endregion
    }
}