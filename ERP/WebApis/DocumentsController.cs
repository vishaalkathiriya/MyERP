using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ERP.Models;
using System.Text;
using System.Collections.Specialized;
using System.Web;
using ERP.Utilities;
using System.Configuration;
using System.Data.Entity.Infrastructure;

namespace ERP.WebApis
{
    public class DocumentsController : ApiController
    {
        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Document";


        public DocumentsController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }

        /// <summary>
        /// POST api/documents
        /// create and update record
        /// </summary>
        [HttpPost]
        public ApiResponse SaveDocument(tblDocument doc)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (doc.Id == 0)
                    {// Mode == Add
                        tblDocument d = new tblDocument
                        {
                            Documents = doc.Documents,
                            DocumentTypeId = doc.DocumentTypeId,
                            IsActive = doc.IsActive,
                            CreDate = DateTime.Now.ToUniversalTime(),
                            ChgDate = DateTime.Now.ToUniversalTime()
                        };
                        db.tblDocuments.Add(d);
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);
                    }
                    else
                    {// Mode == Edit
                        var line = db.tblDocuments.Where(z => z.Id == doc.Id).SingleOrDefault();
                        if (line != null)
                        {
                            line.Documents = doc.Documents;
                            line.DocumentTypeId = doc.DocumentTypeId;
                            line.IsActive = doc.IsActive;
                            line.ChgDate = DateTime.Now.ToUniversalTime();
                        }
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgUpdate, null);
                    }
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

        /// <summary>
        /// POST api/documents
        /// delete document 
        /// </summary>
        [HttpPost]
        public ApiResponse DeleteDocument([FromBody]int id)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (DependancyStatus.DocumentStatus(id)) {
                        var line = db.tblDocuments.Where(z => z.Id == id).SingleOrDefault();
                        if (line != null) {
                            db.tblDocuments.Remove(line);
                            db.SaveChanges();
                            apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgDelete, null);
                        }
                    } else {
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

        /// <summary>
        /// POST api/documents
        /// active-inActive record 
        /// </summary>
        [HttpPost]
        public ApiResponse ChangeStatus(tblDocument doc)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var line = db.tblDocuments.Where(z => z.Id == doc.Id).SingleOrDefault();
                    if (line != null)
                    {
                        if (doc.IsActive)
                        {
                            line.IsActive = false;
                        }
                        else if (!doc.IsActive)
                        {
                            line.IsActive = true;
                        }
                    }

                    line.ChgDate = DateTime.Now.ToUniversalTime();
                    db.SaveChanges();
                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgChangeStatus, null);
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

        // POST api/documents
        // COMMENTED FOR FUTURE USE
        /*[HttpPost]
        public bool DeleteSelectedDoc(int[] strIds)
        {
            try {
                foreach (int id in strIds)
                {
                    var line = db.tblDocuments.Where(z => z.Id == id).SingleOrDefault();
                    if (line != null)  {
                        db.tblDocuments.Remove(line);
                    }
                }
                db.SaveChanges();
                return true;
            }
            catch {
                return false;
            }
        }*/


        /// <summary>
        /// GET api/documents
        /// return document list with sorting and filtering  functionalities
        /// </summary>
        [HttpGet]
        public ApiResponse GetDocumentList()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                using (db)
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int page = Convert.ToInt32(nvc["page"]);
                    int timezone = Convert.ToInt32(nvc["timezone"]);
                    string orderBy = nvc["orderby"];
                    string filter = nvc["filter"];

                    int iDisplayLength = Convert.ToInt32(nvc["count"]);
                    int iDisplayStart = (page - 1) * iDisplayLength;

                    List<tblDocument> list = null;
                    try
                    {
                        list = db.tblDocuments.ToList();

                        //1. filter data
                        if (!string.IsNullOrEmpty(filter) && filter != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.Documents.ToLower().Contains(filter.ToLower())).ToList();
                        }
                        //2. do sorting on list
                        list = DoSorting(list, orderBy.Trim());

                        //3. take total count to return for ng-table
                        var Count = list.Count();

                        //4. convert returned datetime to local timezone
                        var Documents = list.Select(i =>
                        {
                            i.CreDate = Convert.ToDateTime(i.CreDate).AddMinutes(-1 * timezone);
                            i.ChgDate = Convert.ToDateTime(i.ChgDate).AddMinutes(-1 * timezone);
                            return i;
                        }).Skip(iDisplayStart).Take(iDisplayLength).ToList();


                        var resultData = new
                        {
                            total = Count,
                            result = Documents.ToList()
                        };

                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", resultData);
                    }
                    catch (Exception ex)
                    {
                        apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
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
        /// GET api/documents
        /// get document type list
        /// </summary>
        [HttpGet]
        public ApiResponse GetDocumentTypeList()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin()) {
                try {
                    List<SelectItemModel> list = new List<SelectItemModel>();
                    foreach (int value in Enum.GetValues(typeof(ERPUtilities.DocumentType))) 
                    {
                        if (value <= 7) { // This condition is for not to take inv document types in master entries
                            list.Add(new SelectItemModel
                            {
                                Id = value,
                                Label = Enum.GetName(typeof(ERPUtilities.DocumentType), value)
                            });
                        }
                    }

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", list.OrderBy(z=>z.Label).ToList());
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
                }
            }
            else {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }

        /// <summary>
        /// return sorted list based on passed column
        /// </summary>
        public List<tblDocument> DoSorting(IEnumerable<tblDocument> list, string orderBy)
        {
            try
            {
                if (orderBy == "Documents")
                {
                    list = list.OrderBy(z => z.Documents).ToList();
                }
                else if (orderBy == "-Documents")
                {
                    list = list.OrderByDescending(z => z.Documents).ToList();
                }
                else if (orderBy == "ChgDate")
                {
                    list = list.OrderBy(z => z.ChgDate).ToList();
                }
                else if (orderBy == "-ChgDate")
                {
                    list = list.OrderByDescending(z => z.ChgDate).ToList();
                }
                return list.ToList<tblDocument>();
            }
            catch
            {
                return null;
            }
        }


        /// <summary>
        /// List Of Active Documents
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<tblDocument> GetActiveDocuments()
        {
            try
            {
                return db.tblDocuments.Where(z => z.IsActive == true).OrderBy(z => z.Documents).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }


    }
}