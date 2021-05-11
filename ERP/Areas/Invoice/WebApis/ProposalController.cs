using ERP.Models;
using ERP.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace ERP.Areas.Inventory.WebApis
{
    public class ProposalController : ApiController
    {
        #region VARIABLE DECLARATION
        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Proposal";
        #endregion

        #region CONSTRUCTOR
        public ProposalController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }
        #endregion

        #region CRUD FUNCTIONS
        [HttpPost]
        public ApiResponse CreateUpdateProposal(tblINVProposal tblINVProposal)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int timezone = Convert.ToInt32(nvc["timezone"].ToString());
                    int _ProposalId = 0;

                    if (tblINVProposal.PKProposalId == 0) // ADD
                    {
                        tblINVProposal tbl = new tblINVProposal
                        {
                            FKInquiryId = tblINVProposal.FKInquiryId,
                            ProposalTitle = tblINVProposal.ProposalTitle,
                            ProposalDate = tblINVProposal.ProposalDate,
                            IsFinalized = tblINVProposal.IsFinalized,
                            IsActive = tblINVProposal.IsActive,
                            IsDeleted = false,
                            Remarks = tblINVProposal.Remarks,
                            CreDate = DateTime.Now.ToUniversalTime(),
                            CreBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString()),
                            ChgDate = DateTime.Now.ToUniversalTime(),
                            ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString())
                        };
                        db.tblINVProposals.Add(tbl);
                        db.SaveChanges();

                        //get max id
                        int maxProposalId = db.tblINVProposals.Max(z => z.PKProposalId);
                        var list = db.tblINVProposals.Where(z => z.FKInquiryId == tblINVProposal.FKInquiryId && z.PKProposalId != maxProposalId && z.IsDeleted == false).ToList();
                        if (list != null)
                        {
                            foreach (var j in list)
                            {
                                j.IsFinalized = false;

                            }
                            db.SaveChanges();
                        }

                        //Check to change status of inquiry = ProposalSubmitted
                        if (db.tblINVProposals.Where(z => z.FKInquiryId == tblINVProposal.FKInquiryId && z.IsActive == true).Count() > 0)
                        {
                            var line = db.tblINVInquiries.Where(z => z.PKInquiryId == tblINVProposal.FKInquiryId).SingleOrDefault();
                            if (line != null)
                            {
                                line.InquiryStatus = 3;
                                db.SaveChanges();
                            }
                        }

                        _ProposalId = db.tblINVProposals.Max(z => z.PKProposalId);
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, _ProposalId);
                    }
                    else // EDIT
                    {
                        var docList = db.tblINVDocuments.Where(z => z.tblRefId == tblINVProposal.PKProposalId && z.IsActive == true && z.DocTypeId == 17).ToList();
                        foreach (var item in docList)
                        {
                            db.tblINVDocuments.Remove(item);
                        }
                        db.SaveChanges();

                        var tbl = db.tblINVProposals.Where(z => z.PKProposalId == tblINVProposal.PKProposalId).SingleOrDefault();
                        if (tbl != null)
                        {
                            tbl.FKInquiryId = tblINVProposal.FKInquiryId;
                            tbl.ProposalTitle = tblINVProposal.ProposalTitle;
                            tbl.ProposalDate = tblINVProposal.ProposalDate;
                            tbl.IsFinalized = tblINVProposal.IsFinalized;
                            tbl.IsActive = tblINVProposal.IsActive;
                            tbl.Remarks = tblINVProposal.Remarks;
                            tbl.ChgDate = DateTime.Now.ToUniversalTime();
                            tbl.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                        }
                        db.SaveChanges();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgUpdate, tblINVProposal.PKProposalId);
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

        [HttpGet]
        public ApiResponse RetrieveProposals()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                using (db)
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int inquiryId = Convert.ToInt32(nvc["inquiryId"]);
                    int timezone = Convert.ToInt32(nvc["timezone"]);
                    IEnumerable<tblINVProposal> proposals = null;
                    try
                    {
                        proposals = db.tblINVProposals.Where(z => z.FKInquiryId == inquiryId && z.IsDeleted == false).ToList();
                        proposals = proposals.Select(i =>
                        {
                            i.CreDate = Convert.ToDateTime(i.CreDate).AddMinutes(-1 * timezone);
                            i.ChgDate = Convert.ToDateTime(i.ChgDate).AddMinutes(-1 * timezone);
                            return i;
                        });
                        proposals = proposals.OrderByDescending(z => z.ChgDate).ToList();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", proposals);
                    }
                    catch (Exception ex)
                    {
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 0, generalMessages.msgError, null);
                    }
                    finally
                    {
                        proposals = null;
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
        public ApiResponse DeleteProposal()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int proposalId = Convert.ToInt32(nvc["proposalId"].ToString());
                    var proposal = db.tblINVProposals.Where(z => z.PKProposalId == proposalId).SingleOrDefault();
                    var documents = db.tblINVDocuments.Where(z => z.tblRefId == proposalId && z.IsActive == true && z.DocTypeId == 17).ToList();

                    foreach (var doc in documents)
                    {
                        doc.IsActive = false;
                        // USE FOR DELETE PARTICULAR IMGE IF YOU WANT TO DELETE ITEM
                        if (!string.IsNullOrEmpty(doc.DocName))
                        {
                            DeletePicture(doc.DocName);
                        }
                    }
                    db.SaveChanges();

                    if (proposal != null)
                    {
                        proposal.IsDeleted = true;
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

        [HttpGet]
        public ApiResponse ChangeProposalStatus()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int proposalId = Convert.ToInt32(nvc["proposalId"].ToString());

                    var inquiryId = db.tblINVProposals.Where(z => z.PKProposalId == proposalId).Select(z => z.FKInquiryId).FirstOrDefault();
                    var list = db.tblINVProposals.Where(z => z.FKInquiryId == inquiryId && z.PKProposalId != proposalId && z.IsDeleted == false).ToList();
                    if (list != null)
                    {
                        foreach (var j in list)
                        {
                            j.IsFinalized = false;
                            db.SaveChanges();
                        }
                    }

                    var proposal = db.tblINVProposals.Where(z => z.PKProposalId == proposalId).SingleOrDefault();
                    if (proposal != null)
                    {
                        //    proposal.IsFinalized = !proposal.IsFinalized;
                        proposal.IsFinalized = true;
                        proposal.ChgDate = DateTime.Now.ToUniversalTime();
                        db.SaveChanges();
                    }

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgUpdate, null);
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
        [HttpGet]
        public ApiResponse IsInquiryConfirmed()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int inquiryId = Convert.ToInt32(nvc["inquiryId"].ToString());
                    bool isConfirmed = false;

                    if (db.tblINVProjects.Where(z => z.FKInquiryId == inquiryId && z.IsActive == true && z.IsDeleted == false).Count() > 0)
                    {
                        isConfirmed = true;
                    }
                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", isConfirmed);
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
                    // INSERT RECORED TIME TO MOVE FILE FORM TEMP  CONVERSATION FOLDER
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
        [HttpGet]
        public ApiResponse RetrieveDocument()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int proposalId = Convert.ToInt32(nvc["proposalId"].ToString());
                    var documentList = db.tblINVDocuments.Where(z => z.tblRefId == proposalId && z.IsActive == true && z.DocTypeId == 17).ToList();
                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", documentList);
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
                var destinationFile = HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["invProposalUploads"].ToString()) + "/" + fileName;
                System.IO.File.Move(sourceFile, destinationFile);

                string ext = Path.GetExtension(fileName).ToLower();
                if (ext == ".jpg" || ext == ".jpeg" || ext == ".png")
                {
                    var sourceThumbFile = HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["TempThumbnails"].ToString()) + "/" + fileName;
                    var destinationThumbPath = HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["invProposalThumbnails"].ToString()) + "/" + fileName;

                    System.IO.File.Move(sourceThumbFile, destinationThumbPath);
                }
            }
        }
        /// <summary>
        /// Common function for deleting picture
        /// </summary>
        private void DeletePicture(string uploadDoc)
        {
            string mainPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["invProposalUploads"].ToString());
            string thumblinePath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["invProposalThumbnails"].ToString());
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