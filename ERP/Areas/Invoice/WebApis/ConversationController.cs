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

namespace ERP.Areas.Invoice.WebApis
{
    public class ConversationController : ApiController
    {
        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Conversation";

        public ConversationController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }

        [HttpPost]
        public ApiResponse CreateUpdateConversation(tblINVConversation INVConversation)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int timezone = Convert.ToInt32(nvc["timezone"].ToString());
                    int _ConversationId = 0;

                    if (INVConversation.PKConversationId == 0) // ADD
                    {
                        tblINVConversation tbl = new tblINVConversation
                        {
                            FKClientId = INVConversation.FKClientId,
                            ConversationTitle = INVConversation.ConversationTitle,
                            ConversationDescription = INVConversation.ConversationDescription,
                            ContentType = INVConversation.ContentType,
                            ReferenceId = INVConversation.ReferenceId,
                            ConversationType = INVConversation.ConversationType,
                            ConversationDate =INVConversation.ConversationDate,
                            IsDeleted = false,
                            CreDate = DateTime.Now.ToUniversalTime(),
                            CreBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString()),
                            ChgDate = DateTime.Now.ToUniversalTime(),
                            ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString())
                        };
                        db.tblINVConversations.Add(tbl);
                        db.SaveChanges();
                        _ConversationId = db.tblINVConversations.Max(z => z.PKConversationId);
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, _ConversationId);
                    }
                    else // EDIT
                    {

                        var docList = db.tblINVDocuments.Where(z => z.tblRefId == INVConversation.PKConversationId && z.IsActive == true && z.DocTypeId == 15).ToList();
                        foreach (var item in docList)
                        {
                            db.tblINVDocuments.Remove(item);
                        }
                        db.SaveChanges();

                        var tbl = db.tblINVConversations.Where(z => z.PKConversationId == INVConversation.PKConversationId).SingleOrDefault();
                        if (tbl != null)
                        {
                            tbl.ConversationTitle = INVConversation.ConversationTitle;
                            tbl.ConversationDescription = INVConversation.ConversationDescription;
                            tbl.ContentType = INVConversation.ContentType;
                            tbl.ConversationType = INVConversation.ConversationType;
                            tbl.ConversationDate = INVConversation.ConversationDate;
                            tbl.ChgDate = DateTime.Now.ToUniversalTime();
                            tbl.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                        }
                        db.SaveChanges();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgUpdate, INVConversation.PKConversationId);
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
        public ApiResponse GetCurrencyList()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var list = db.tblCurrencies.Where(z => z.IsActive == true && z.IsDeleted == false).ToList();
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
        public ApiResponse RetrieveConversations()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int timezone = Convert.ToInt32(nvc["timezone"].ToString());
                    int clientId = Convert.ToInt32(nvc["clientId"].ToString());
                    int refId = Convert.ToInt32(nvc["refId"].ToString()); // 0 = Open, > 0 for PKInquiryId & PKProjectId
                    int conversationType = Convert.ToInt32(nvc["conversationType"].ToString());

                    List<INVConversationViewModel> conversationList = new List<INVConversationViewModel>();
                    var converInfo = db.tblINVConversations.Where(z => z.FKClientId == clientId && z.IsDeleted == false && z.ConversationType == conversationType && z.ReferenceId == refId).ToList();
                    var clientInfo = db.tblINVClients.Where(z => z.PKClientId == clientId).SingleOrDefault();
                    if (clientInfo != null && converInfo != null)
                    {
                        foreach (var list in converInfo)
                        {
                            List<INVDocumentViewModel> conDocumentList = new List<INVDocumentViewModel>();
                            var DocList = db.tblINVDocuments.Where(z => z.tblRefId == list.PKConversationId && z.IsActive == true && z.DocTypeId == 15).ToList();
                            foreach (var dList in DocList)
                            {
                                conDocumentList.Add(new INVDocumentViewModel
                                {
                                    DocId = dList.PKDocId,
                                    tblRefId = dList.tblRefId, //passing ConversationId
                                    DocName = dList.DocName,
                                    DocType = dList.DocTypeId,
                                    DocRemark = dList.Remarks,
                                    DocFileType = dList.DocName.Split('.')[1]
                                });
                            }
                            conversationList.Add(new INVConversationViewModel
                            {
                                PKConversationId = list.PKConversationId,
                                FKClientId = list.FKClientId,
                                CompanyName = clientInfo.CompanyName,
                                ClientCode = clientInfo.ClientCode,
                                ClientName = clientInfo.CPrefix + " " + clientInfo.ContactPerson,
                                ClientMobile = clientInfo.MobileNo,
                                ClientEmail = clientInfo.Email,
                                ContentType = list.ContentType,
                                ConversationType = list.ConversationType,
                                ConversationTitle = list.ConversationTitle,
                                ConversationDescription = list.ConversationDescription,
                                ConversationDate = list.ConversationDate.AddMinutes(timezone),
                                DocumentList = conDocumentList
                            });
                        }
                    }

                    if (conversationList != null)
                    {
                        conversationList = conversationList.Select(i =>
                        {
                            i.ConversationDate = Convert.ToDateTime(i.ConversationDate).AddMinutes(-1 * timezone);
                            return i;
                        }).OrderBy(z => z.ConversationDate).ToList();
                    }
                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", conversationList);
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
        public ApiResponse DeleteConversation()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int ConId = Convert.ToInt32(nvc["ConversationId"].ToString());
                    var conversationInfo = db.tblINVConversations.Where(z => z.PKConversationId == ConId).SingleOrDefault();
                    var docList = db.tblINVDocuments.Where(z => z.tblRefId == ConId && z.IsActive == true && z.DocTypeId == 15).ToList();

                    foreach (var item in docList)
                    {
                        item.IsActive = false;

                        //// USE FOR DELETE PARTICULAR IMGE IF YOU WANT TO DELETE ITEM
                        //if (!string.IsNullOrEmpty(item.DocName))
                        //{
                        //    DeletePicture(item.DocName);
                        //}
                    }
                    db.SaveChanges();

                    if (conversationInfo != null)
                    {
                        conversationInfo.IsDeleted = true;
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

        /// <summary>
        /// GET api/Conversation
        /// get client list
        /// </summary>
        [HttpGet]
        public ApiResponse GetClientList()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var list = db.tblINVClients.AsEnumerable().OrderBy(z => z.CompanyName)
                        .Where(z => z.IsDeleted == false && z.IsActive == true)
                        .Select(z => new SelectItemModel
                        {
                            Id = z.PKClientId,
                            Label = string.Format("{0}{1} - {2}", !string.IsNullOrEmpty(z.ClientCode) ? z.ClientCode +" - " : string.Empty, z.CompanyName, z.ContactPerson)
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
        public ApiResponse GetClient()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int clientId = Convert.ToInt32(nvc["cId"].ToString());

                    var line = db.tblINVClients.Where(z => z.PKClientId == clientId).SingleOrDefault();

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

        #region COMMON FUNCTION
        /// <summary>
        /// Common function for moving picture from temp folder to main folder
        /// </summary>
        protected void MoveFile(string fileName)
        {
            if (System.IO.File.Exists(HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["TempUploadPath"].ToString()) + "/" + fileName))
            {
                var sourceFile = HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["TempUploadPath"].ToString()) + "/" + fileName;
                var destinationFile = HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["invConversationUploads"].ToString()) + "/" + fileName;

                System.IO.File.Move(sourceFile, destinationFile);

                string ext = Path.GetExtension(fileName).ToLower();
                if (ext == ".jpg" || ext == ".jpeg" || ext == ".png")
                {
                    var sourceThumbFile = HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["TempThumbnails"].ToString()) + "/" + fileName;
                    var destinationThumbPath = HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["invConversationThumbnails"].ToString()) + "/" + fileName;

                    System.IO.File.Move(sourceThumbFile, destinationThumbPath);
                }
            }
        }

        /// <summary>
        /// Common function for deleting picture
        /// </summary>
        private void DeletePicture(string uploadDoc)
        {
            string mainPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["invConversationUploads"].ToString());
            string thumblinePath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["invConversationThumbnails"].ToString());

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