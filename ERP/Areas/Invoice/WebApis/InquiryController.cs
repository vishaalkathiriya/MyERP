using ERP.Models;
using ERP.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace ERP.Areas.Invoice.WebApis
{
    public class InquiryController : ApiController
    {
        #region VARIABLE DECLARATION
        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Inquiry";
        #endregion

        #region CONSTRUCTOR
        public InquiryController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }
        #endregion

        #region RETRIEVING INITIALIZATION DATA
        [HttpGet]
        public ApiResponse RetrieveClients()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var lstClient = db.tblINVClients.Where(z => z.IsActive == true && z.IsDeleted == false).OrderBy(z => z.CompanyName).ToList();
                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", lstClient);
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

        [HttpGet]
        public ApiResponse RetrieveClientSources()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var lstSource = db.tblINVClientSources.Where(z => z.IsActive == true && z.IsDeleted == false).OrderBy(z => z.SourceName).ToList();
                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", lstSource);
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

        [HttpGet]
        public ApiResponse RetrieveTechnologies()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var list = db.tblTechnologies.Where(z => z.IsActive == true).OrderBy(z => z.Technologies)
                       .Where(z => z.IsActive == true)
                       .Select(z => new SelectItemModel
                       {
                           Id = z.Id,
                           Label = z.Technologies
                       }).ToList();
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

        [HttpGet]
        public ApiResponse RetrieveStatus()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    List<SelectItemModel> list = new List<SelectItemModel>();

                    foreach (int value in Enum.GetValues(typeof(ERPUtilities.InquiryStatus)))
                    {
                        list.Add(new SelectItemModel
                        {
                            Id = value,
                            Label = Enum.GetName(typeof(ERPUtilities.InquiryStatus), value)
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
        #endregion

        #region CRUD FUNCTIONS
        [HttpPost]
        public ApiResponse CreateUpdateInquiry(tblINVInquiry _tblINVInquiry)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int timezone = Convert.ToInt32(nvc["timezone"].ToString());
                    string idate = nvc["idate"].ToString();
                    int _ConversationId = 0;
                    string strLog = string.Empty;

                    DateTime dtInquiry = DateTime.ParseExact(idate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                    if (_tblINVInquiry.PKInquiryId == 0) // ADD
                    {
                        tblINVInquiry tbl = new tblINVInquiry
                        {
                            FKClientId = _tblINVInquiry.FKClientId,
                            InquiryCode = string.Format("INQ{0}{1}{2}", DateTime.Now.Month.ToString("00") , DateTime.Now.Year % 100, db.tblINVInquiries.Count() + 1),
                            InquiryTitle = _tblINVInquiry.InquiryTitle,
                            InquiryStatus = _tblINVInquiry.InquiryStatus,
                            InquiryDate = dtInquiry,
                            FKTechnologyIds = _tblINVInquiry.FKTechnologyIds,
                            Remarks = _tblINVInquiry.Remarks,
                            IsActive = _tblINVInquiry.IsActive,
                            IsDeleted = _tblINVInquiry.IsDeleted,
                            CreDate = DateTime.Now.ToUniversalTime(),
                            CreBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString()),
                            ChgDate = DateTime.Now.ToUniversalTime(),
                            ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString())
                        };
                        db.tblINVInquiries.Add(tbl);
                        db.SaveChanges();
                        _ConversationId = db.tblINVInquiries.Max(z => z.PKInquiryId);
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, _ConversationId);

                        //Get Log Of Inquiry while inserting
                        strLog = GetInquiryLog(_tblINVInquiry, "Add");

                    }
                    else // EDIT
                    {
                        var docList = db.tblINVDocuments.Where(z => z.tblRefId == _tblINVInquiry.PKInquiryId && z.IsActive == true && z.DocTypeId == 16).ToList();
                        foreach (var item in docList)
                        {
                            db.tblINVDocuments.Remove(item);
                        }
                        db.SaveChanges();

                        var tbl = db.tblINVInquiries.Where(z => z.PKInquiryId == _tblINVInquiry.PKInquiryId).SingleOrDefault();

                        //Get Log Of Inquiry while editing
                        _tblINVInquiry.InquiryDate = dtInquiry;
                        strLog = GetInquiryLog(_tblINVInquiry, "Edit");

                        if (tbl != null)
                        {
                            tbl.InquiryTitle = _tblINVInquiry.InquiryTitle;
                            tbl.InquiryStatus = _tblINVInquiry.InquiryStatus;
                            tbl.InquiryDate = dtInquiry;
                            tbl.FKTechnologyIds = _tblINVInquiry.FKTechnologyIds;
                            tbl.Remarks = _tblINVInquiry.Remarks;
                            tbl.IsActive = _tblINVInquiry.IsActive;
                            tbl.IsDeleted = _tblINVInquiry.IsDeleted;
                            tbl.ChgDate = DateTime.Now.ToUniversalTime();
                            tbl.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                        }

                        db.SaveChanges();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgUpdate, _tblINVInquiry.PKInquiryId);
                    }

                    //Log inquiry for both mode
                    tblINVLogInquiry log = new tblINVLogInquiry()
                    {
                        Description = strLog,
                        CreDate = DateTime.Now,
                        CreBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString())
                    };
                    db.tblINVLogInquiries.Add(log);
                    db.SaveChanges();
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
        /*
        protected string GetInquiryDocumentLog(tblINVDocument newDoc, string action) 
        {
            string strLog = string.Empty;
            try
            {
                int useId = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                var line = db.tblEmpPersonalInformations.Where(z => z.EmployeeId == useId).SingleOrDefault();
                tblINVDocument oldDoc = db.tblINVDocuments.Where(z => z.PKDocId == newDoc.PKDocId).SingleOrDefault();

                if (action == "Edit")
                {
                    // Chirag Zadafia has updated inquiry. colname(oldValue = newValue), .....
                    strLog = string.Format("{0} has updated inquiry document. FKClientId({1}={2})," +
                                   "FKSourceId({3}={4}), InquiryTitle({5}={6}), InquiryStatus({7}={8}), InquiryDate({9}={10}), FKTechnologyIds({11}={12})," +
                                   "Remarks({13}={14}), IsActive({15}={16}), IsDeleted({17}={18}), CreBy({19}={20}), ChgBy({21}={22}), CreDate({23}={24}), ChgDate({25}={26})",
                        line.CandidateFirstName + " " + line.CandidateLastName,
                        oldDoc.FKClientId, newDoc.FKClientId,
                        oldDoc.FKSourceId, newDoc.FKSourceId,
                        oldDoc.InquiryTitle, newDoc.InquiryTitle,
                        oldDoc.InquiryStatus, newDoc.InquiryStatus,
                        oldDoc.InquiryDate, newDoc.InquiryDate,
                        oldDoc.FKTechnologyIds, newDoc.FKTechnologyIds,
                        oldDoc.Remarks, newDoc.Remarks,
                        oldDoc.IsActive, newDoc.IsActive,
                        oldDoc.IsDeleted, newDoc.IsDeleted,
                        oldDoc.CreBy, newDoc.CreBy,
                        oldDoc.ChgBy, newDoc.ChgBy,
                        oldDoc.CreDate, newDoc.CreDate,
                        oldDoc.ChgDate, newDoc.ChgDate
                        );
                }
                else if (action == "Add")
                {
                    // Chirag Zadafia has inserted inquiry. colname(newValue), .....
                    strLog = string.Format("{0} has inserted inquiry. FKClientId({1})," +
                                   "FKSourceId({2}), InquiryTitle({3}), InquiryStatus({4}), InquiryDate({5}), FKTechnologyIds({6})," +
                                   "Remarks({7}), IsActive({8}), IsDeleted({9}), CreBy({10}), ChgBy({11}), CreDate({12}), ChgDate({13})",
                        line.CandidateFirstName + " " + line.CandidateLastName,
                        newInq.FKClientId,
                        newInq.FKSourceId,
                        newInq.InquiryTitle,
                        newInq.InquiryStatus,
                        newInq.InquiryDate,
                        newInq.FKTechnologyIds,
                        newInq.Remarks,
                        newInq.IsActive,
                        newInq.IsDeleted,
                        newInq.CreBy,
                        newInq.ChgBy,
                        newInq.CreDate,
                        newInq.ChgDate);
                }
                else if (action == "Delete")
                {
                    // Chirag Zadafia has deleted inquiry. colname(newValue), .....
                    strLog = string.Format("{0} has deleted inquiry. FKClientId({1})," +
                                   "FKSourceId({2}), InquiryTitle({3}), InquiryStatus({4}), InquiryDate({5}), FKTechnologyIds({6})," +
                                   "Remarks({7}), IsActive({8}), IsDeleted({9}), CreBy({10}), ChgBy({11}), CreDate({12}), ChgDate({13})",
                        line.CandidateFirstName + " " + line.CandidateLastName,
                        oldInq.FKClientId,
                        oldInq.FKSourceId,
                        oldInq.InquiryTitle,
                        oldInq.InquiryStatus,
                        oldInq.InquiryDate,
                        oldInq.FKTechnologyIds,
                        oldInq.Remarks,
                        oldInq.IsActive,
                        oldInq.IsDeleted,
                        oldInq.CreBy,
                        oldInq.ChgBy,
                        oldInq.CreDate,
                        oldInq.ChgDate);
                }
            }
            catch (Exception ex)
            {
                strLog = ex.ToString();
            }

            return strLog;
        }
        */
        protected string GetInquiryLog(tblINVInquiry newInq, string action)
        {
            string strLog = string.Empty;
            try
            {
                int useId = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                var line = db.tblEmpPersonalInformations.Where(z => z.EmployeeId == useId).SingleOrDefault();
                tblINVInquiry oldInq = db.tblINVInquiries.Where(z => z.PKInquiryId == newInq.PKInquiryId).SingleOrDefault();

                if (action == "Add")
                {
                    // Chirag Zadafia has inserted inquiry. colname(newValue), .....
                    strLog = string.Format("{0} has inserted inquiry. FKClientId({1})," +
                                   "InquiryCode({2}), InquiryTitle({3}), InquiryStatus({4}), InquiryDate({5}), FKTechnologyIds({6})," +
                                   "Remarks({7}), IsActive({8}), IsDeleted({9}), CreBy({10}), ChgBy({11}), CreDate({12}), ChgDate({13})",
                        line.CandidateFirstName + " " + line.CandidateLastName,
                        newInq.FKClientId,
                        newInq.InquiryCode,
                        newInq.InquiryTitle,
                        newInq.InquiryStatus,
                        newInq.InquiryDate,
                        newInq.FKTechnologyIds,
                        newInq.Remarks,
                        newInq.IsActive,
                        newInq.IsDeleted,
                        newInq.CreBy,
                        newInq.ChgBy,
                        newInq.CreDate,
                        newInq.ChgDate);
                }
                else if (action == "Edit")
                {
                    // Chirag Zadafia has updated inquiry. colname(oldValue = newValue), .....
                    strLog = string.Format("{0} has updated inquiry. FKClientId({1}={2})," +
                                   "InquiryCode({3}={4}), InquiryTitle({5}={6}), InquiryStatus({7}={8}), InquiryDate({9}={10}), FKTechnologyIds({11}={12})," +
                                   "Remarks({13}={14}), IsActive({15}={16}), IsDeleted({17}={18}), CreBy({19}={20}), ChgBy({21}={22}), CreDate({23}={24}), ChgDate({25}={26})",
                        line.CandidateFirstName + " " + line.CandidateLastName,
                        oldInq.FKClientId, newInq.FKClientId,
                        oldInq.InquiryCode, newInq.InquiryCode,
                        oldInq.InquiryTitle, newInq.InquiryTitle,
                        oldInq.InquiryStatus, newInq.InquiryStatus,
                        oldInq.InquiryDate, newInq.InquiryDate,
                        oldInq.FKTechnologyIds, newInq.FKTechnologyIds,
                        oldInq.Remarks, newInq.Remarks,
                        oldInq.IsActive, newInq.IsActive,
                        oldInq.IsDeleted, newInq.IsDeleted,
                        oldInq.CreBy, newInq.CreBy,
                        oldInq.ChgBy, newInq.ChgBy,
                        oldInq.CreDate, newInq.CreDate,
                        oldInq.ChgDate, newInq.ChgDate
                        );
                }
                else if (action == "Delete")
                {
                    // Chirag Zadafia has deleted inquiry. colname(newValue), .....
                    strLog = string.Format("{0} has deleted inquiry. FKClientId({1})," +
                                   "InquiryCode({2}), InquiryTitle({3}), InquiryStatus({4}), InquiryDate({5}), FKTechnologyIds({6})," +
                                   "Remarks({7}), IsActive({8}), IsDeleted({9}), CreBy({10}), ChgBy({11}), CreDate({12}), ChgDate({13})",
                        line.CandidateFirstName + " " + line.CandidateLastName,
                        oldInq.FKClientId,
                        oldInq.InquiryCode,
                        oldInq.InquiryTitle,
                        oldInq.InquiryStatus,
                        oldInq.InquiryDate,
                        oldInq.FKTechnologyIds,
                        oldInq.Remarks,
                        oldInq.IsActive,
                        oldInq.IsDeleted,
                        oldInq.CreBy,
                        oldInq.ChgBy,
                        oldInq.CreDate,
                        oldInq.ChgDate);
                }
            }
            catch (Exception ex)
            {
                strLog = ex.ToString();
            }
            return strLog;
        }

        [HttpPost]
        public ApiResponse ChangeInquiryStatus(tblINVInquiry inquiry)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                using (db)
                {
                    tblINVInquiry tbl = null;
                    try
                    {
                        tbl = db.tblINVInquiries.Where(z => z.PKInquiryId == inquiry.PKInquiryId).FirstOrDefault();
                        if (tbl != null)
                        {
                            if (tbl.IsActive)
                                tbl.IsActive = false;
                            else if (!tbl.IsActive)
                                tbl.IsActive = true;
                        }
                        tbl.ChgDate = DateTime.Now.ToUniversalTime();
                        db.SaveChanges();

                        // UPDATING STATUS OF RELATIVE PROPOSAL(S) IF ANY
                        var proposals = db.tblINVProposals.Where(z => z.FKInquiryId == tbl.PKInquiryId).ToList();
                        foreach (var proposal in proposals)
                        {
                            proposal.IsActive = tbl.IsActive;
                            proposal.ChgDate = DateTime.Now.ToUniversalTime();
                        }
                        db.SaveChanges();

                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgChangeStatus, null);
                    }
                    catch (Exception ex)
                    {
                        apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
                    }
                    finally
                    {
                        tbl = null;
                    }
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }
            return apiResponse;
        }

        [HttpGet]
        public ApiResponse RetrieveInquiries()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                using (db)
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int page = Convert.ToInt32(nvc["page"]);
                    int timezone = Convert.ToInt32(nvc["timezone"]);
                    int clientId = Convert.ToInt32(nvc["clientId"]);
                    string orderBy = nvc["orderby"];
                    string title = nvc["title"];
                    int statusId = (!string.IsNullOrEmpty(nvc["status"]) && nvc["status"] != "undefined") ? Convert.ToInt32(nvc["status"]) : 0;
                    string code = nvc["source"];

                    int iDisplayLength = Convert.ToInt32(nvc["count"]);
                    int iDisplayStart = (page - 1) * iDisplayLength;
                    int count = 0;

                    List<tblINVInquiry> list = null;
                    List<INVInquiryViewModel> invList = new List<INVInquiryViewModel>();

                    try
                    {
                        list = clientId > 0 ? db.tblINVInquiries.Where(z => z.FKClientId == clientId && z.IsDeleted == false).ToList()
                                            //: db.tblINVInquiries.Where(z => z.IsDeleted == false).ToList();
                                            : null;

                        if (list != null) {
                            // FILTERING DATA ON BASIS OF TITLE
                            if (!string.IsNullOrEmpty(title) && title != "undefined")
                            {
                                iDisplayStart = 0;
                                list = list.Where(z => z.InquiryTitle.ToLower().Contains(title.ToLower())).ToList();
                            }

                            // FILTERING DATA ON BASIS OF STATUS
                            if (statusId > 0)
                            {
                                iDisplayStart = 0;
                                list = list.Where(z => z.InquiryStatus == statusId).ToList();
                            }

                            // FILTERING DATA ON BASIS OF INQUIRY CODE
                            if (!string.IsNullOrEmpty(code) && code != "undefined")
                            {
                                iDisplayStart = 0;
                                list = list.Where(z => z.InquiryCode.ToLower() == code.ToLower()).ToList();
                            }

                            // SORTING DATA
                            list = DoSorting(list, orderBy.Trim());

                            // TAKE TOTAL COUNT TO RETURN FOR NG-TABLE
                            foreach (var l in list)
                            {
                                invList.Add(new INVInquiryViewModel()
                                {
                                    PKInquiryId = l.PKInquiryId,
                                    FKClientId = l.FKClientId,
                                    InquiryCode = l.InquiryCode,
                                    InquiryTitle = l.InquiryTitle,
                                    InquiryStatus = l.InquiryStatus,
                                    InquiryStatusName = Enum.GetName(typeof(ERP.Utilities.ERPUtilities.InquiryStatus), l.InquiryStatus),
                                    InquiryDate = l.InquiryDate,
                                    FKTechnologyIds = l.FKTechnologyIds,
                                    Remarks = l.Remarks,
                                    IsActive = l.IsActive,
                                    IsDeleted = l.IsDeleted,
                                    CreBy = l.CreBy,
                                    ChgBy = l.ChgBy,
                                    CreDate = Convert.ToDateTime(l.CreDate).AddMinutes(-1 * timezone),
                                    ChgDate = Convert.ToDateTime(l.ChgDate).AddMinutes(-1 * timezone)
                                });
                            }

                            count = invList.Count();
                            invList = invList.Skip(iDisplayStart).Take(iDisplayLength).ToList();
                        }

                        var resultData = new
                        {
                            total = count,
                            result = invList
                        };
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", resultData);
                    }
                    catch (Exception ex)
                    {
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 0, generalMessages.msgError, null);
                    }
                    finally
                    {
                        list = null;
                    }
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }
            return apiResponse;
        }

        /// <summary>
        /// Returns list of inquiries that have at least one finalized proposal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiResponse RetrieveFinalizedInquiries()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                using (db)
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int page = Convert.ToInt32(nvc["page"]);
                    int timezone = Convert.ToInt32(nvc["timezone"]);
                    int clientId = Convert.ToInt32(nvc["clientId"]);
                    string orderBy = nvc["orderby"];
                    string title = nvc["title"];
                    int statusId = (!string.IsNullOrEmpty(nvc["status"]) && nvc["status"] != "undefined") ? Convert.ToInt32(nvc["status"]) : 0;
                    string code = nvc["source"];

                    int iDisplayLength = Convert.ToInt32(nvc["count"]);
                    int iDisplayStart = (page - 1) * iDisplayLength;
                    int count = 0;
                    List<tblINVInquiry> list = null;
                    List<INVInquiryViewModel> invList = new List<INVInquiryViewModel>();

                    try
                    {
                        list = clientId > 0 ? db.tblINVInquiries.Include("tblINVProposals").Include("tblINVProjects").Where(z => z.FKClientId == clientId && z.IsDeleted == false && z.IsActive == true).ToList()
                                            //: db.tblINVInquiries.Include("tblINVProposals").Include("tblINVProjects").Where(z => z.IsDeleted == false).ToList();
                                            : null;
                        if (list != null) 
                        {
                            list = list.Where(z => z.tblINVProposals.Any(p => p.IsFinalized == true && p.IsDeleted == false)).ToList();

                            // FILTERING DATA ON BASIS OF TITLE
                            if (!string.IsNullOrEmpty(title) && title != "undefined")
                            {
                                iDisplayStart = 0;
                                list = list.Where(z => z.InquiryTitle.ToLower().Contains(title.ToLower())).ToList();
                            }

                            // FILTERING DATA ON BASIS OF STATUS
                            if (statusId > 0)
                            {
                                iDisplayStart = 0;
                                list = list.Where(z => z.InquiryStatus == statusId).ToList();
                            }

                            // FILTERING DATA ON BASIS OF INQUIRY CODE
                            if (!string.IsNullOrEmpty(code) && code != "undefined")
                            {
                                iDisplayStart = 0;
                                list = list.Where(z => z.InquiryCode.ToLower() == code.ToLower()).ToList();
                            }

                            // SORTING DATA
                            list = DoSorting(list, orderBy.Trim());

                            // TAKE TOTAL COUNT TO RETURN FOR NG-TABLE
                            foreach (var l in list)
                            {
                                invList.Add(new INVInquiryViewModel()
                                {
                                    PKInquiryId = l.PKInquiryId,
                                    FKClientId = l.FKClientId,
                                    InquiryCode = l.InquiryCode,
                                    InquiryTitle = l.InquiryTitle,
                                    InquiryStatus = l.InquiryStatus,
                                    InquiryStatusName = Enum.GetName(typeof(ERP.Utilities.ERPUtilities.InquiryStatus), l.InquiryStatus),
                                    InquiryDate = l.InquiryDate,
                                    FKTechnologyIds = l.FKTechnologyIds,
                                    Remarks = l.Remarks,
                                    IsActive = l.IsActive,
                                    IsDeleted = l.IsDeleted,
                                    CreBy = l.CreBy,
                                    ChgBy = l.ChgBy,
                                    CreDate = Convert.ToDateTime(l.CreDate).AddMinutes(-1 * timezone),
                                    ChgDate = Convert.ToDateTime(l.ChgDate).AddMinutes(-1 * timezone),
                                    tblINVProposals = l.tblINVProposals.Where(z => z.IsDeleted == false).ToList(),
                                    PKProjectId = l.tblINVProjects.Count() > 0 ? l.tblINVProjects.Select(z => z.PKProjectId).SingleOrDefault() : 0
                                });
                            }
                            count = invList.Count();
                            invList = invList.Skip(iDisplayStart).Take(iDisplayLength).ToList();
                        }

                        var resultData = new
                        {
                            total = count,
                            result = invList
                        };
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", resultData);
                    }
                    catch (Exception ex)
                    {
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 0, generalMessages.msgError, null);
                    }
                    finally
                    {
                        list = null;
                    }
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }
            return apiResponse;
        }

        [HttpGet]
        public ApiResponse GetInquiry()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                using (db)
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int inquiryId = Convert.ToInt32(nvc["inquiryId"]);

                    try
                    {
                        tblINVInquiry line = db.tblINVInquiries.Where(z => z.PKInquiryId == inquiryId).SingleOrDefault();

                        //if (line != null)
                        //{
                        //    line.tblINVClient = db.tblINVClients.Where(z => z.PKClientId == item.FKClientId).FirstOrDefault();
                        //    line.tblINVClientSource = db.tblINVClientSources.Where(z => z.PKSourceId == item.FKSourceId).FirstOrDefault();
                        //}

                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", line);
                    }
                    catch (Exception ex)
                    {
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 0, generalMessages.msgError, null);
                    }
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }
            return apiResponse;
        }

        [HttpGet]
        public ApiResponse RetrieveDocument()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int inquiryId = Convert.ToInt32(nvc["inquiryId"].ToString());
                    var documentList = db.tblINVDocuments.Where(z => z.tblRefId == inquiryId && z.IsActive == true && z.DocTypeId == 16).ToList();
                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgDelete, documentList);
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

        [HttpGet]
        public ApiResponse DeleteInquiry()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int inquiryId = Convert.ToInt32(nvc["inquiryId"].ToString());

                    //If active proposal found then don't allow to delete inquiry.
                    if (db.tblINVProposals.Where(z => z.FKInquiryId == inquiryId && z.IsActive == true && z.IsDeleted == false).Count() == 0)
                    {
                        var tblInquiry = db.tblINVInquiries.Where(z => z.PKInquiryId == inquiryId).SingleOrDefault();
                        var documentList = db.tblINVDocuments.Where(z => z.tblRefId == inquiryId && z.IsActive == true && z.DocTypeId == 16).ToList();

                        //Log inquiry
                        tblINVLogInquiry log = new tblINVLogInquiry()
                        {
                            Description = GetInquiryLog(tblInquiry, "Delete"),
                            CreDate = DateTime.Now,
                            CreBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString())
                        };
                        db.tblINVLogInquiries.Add(log);

                        if (tblInquiry != null) {
                            tblInquiry.IsDeleted = true;
                            foreach (var proposal in db.tblINVProposals.Where(z => z.FKInquiryId == inquiryId).ToList()) {
                                proposal.IsDeleted = true;
                            }

                            //Set IsDelete for documents
                            foreach (var item in documentList) {
                                item.IsActive = false;
                            }
                            db.SaveChanges();
                        }

                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgDelete, null);
                    }
                    else {
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 2, generalMessages.msgParentExists, null);
                    }
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

        #region OTHER FUNCTIONS
        public List<tblINVInquiry> DoSorting(List<tblINVInquiry> list, string orderBy)
        {
            try
            {
                // SETTING ORDER ON TITLE
                if (orderBy == "Title")
                {
                    list = list.OrderBy(z => z.InquiryTitle).ToList();
                }
                else if (orderBy == "-Title")
                {
                    list = list.OrderByDescending(z => z.InquiryTitle).ToList();
                }
                // SETTING ORDER ON SOURCE
                else if (orderBy == "Code")
                {
                    list = list.OrderBy(z => z.InquiryCode).ToList();
                }
                else if (orderBy == "-Code")
                {
                    list = list.OrderByDescending(z => z.InquiryCode).ToList();
                }
                return list;
            }
            catch
            {
                return null;
            }
        }

        [HttpPost]
        public ApiResponse UploadDocument(tblINVDocument INVDocument)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (INVDocument.PKDocId == 0)
                    {
                        tblINVDocument tbl = new tblINVDocument
                        {
                            tblRefId = INVDocument.tblRefId,
                            DocName = INVDocument.DocName,
                            DocTypeId = INVDocument.DocTypeId,
                            Remarks = INVDocument.Remarks,
                            IsActive = true,
                            CreDate = DateTime.Now.ToUniversalTime(),
                            CreBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString()),
                            ChgDate = DateTime.Now.ToUniversalTime(),
                            ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString())
                        };
                        db.tblINVDocuments.Add(tbl);
                        db.SaveChanges();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, true);
                    }
                    else
                    {
                        var tbl = db.tblINVDocuments.Where(z => z.PKDocId == INVDocument.PKDocId).SingleOrDefault();
                        if (tbl != null)
                        {
                            tbl.tblRefId = INVDocument.tblRefId;
                            tbl.DocName = INVDocument.DocName;
                            tbl.DocTypeId = INVDocument.DocTypeId;
                            tbl.Remarks = INVDocument.Remarks;
                            tbl.IsActive = true;
                            tbl.ChgDate = DateTime.Now.ToUniversalTime();
                            tbl.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                        }
                    }

                    // INSERT RECORED TIME TO MOVE FILE FORM TEMP FOLDER
                    MoveFile(INVDocument.DocName);
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
        /// Common function for moving picture from temp folder to main folder
        /// </summary>
        protected void MoveFile(string fileName)
        {
            if (System.IO.File.Exists(HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["TempUploadPath"].ToString()) + "/" + fileName))
            {
                var sourceFile = HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["TempUploadPath"].ToString()) + "/" + fileName;
                var destinationFile = HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["invInquiryUploads"].ToString()) + "/" + fileName;

                System.IO.File.Move(sourceFile, destinationFile);

                string ext = Path.GetExtension(fileName).ToLower();
                if (ext == ".jpg" || ext == ".jpeg" || ext == ".png")
                {
                    var sourceThumbFile = HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["TempThumbnails"].ToString()) + "/" + fileName;
                    var destinationThumbPath = HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["invInquiryThumbnails"].ToString()) + "/" + fileName;

                    System.IO.File.Move(sourceThumbFile, destinationThumbPath);
                }
            }
        }

        /// <summary>
        /// Common function for deleting picture
        /// </summary>
        private void DeletePicture(string uploadDoc)
        {
            string mainPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["invInquiryUploads"].ToString());
            string thumblinePath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["invInquiryThumbnails"].ToString());

            if (File.Exists(Path.Combine(mainPath, uploadDoc)))
            {
                File.Delete(Path.Combine(mainPath, uploadDoc));
            }

            if (File.Exists(Path.Combine(thumblinePath, uploadDoc)))
            {
                File.Delete(Path.Combine(thumblinePath, uploadDoc));
            }

        }
        #endregion
    }
}