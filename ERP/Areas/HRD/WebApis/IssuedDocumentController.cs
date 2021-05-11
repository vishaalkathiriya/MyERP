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
using System.IO;

namespace ERP.WebApis
{
    public class IssuedDocumentController : ApiController
    {
        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Issued Document";


        public IssuedDocumentController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }

        /// <summary>
        /// GET api/employee
        /// retrieve blood group list 
        /// </summary>
        [HttpGet]
        public ApiResponse RetrieveDocument()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var list = db.tblDocuments.OrderBy(z => z.Documents)
                        .Where(z => z.IsActive == true)
                        .Select(z => new SelectItemModel
                        {
                            Id = z.Id,
                            Label = z.Documents
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

        /// <summary>
        /// POST api/IssuedDocument
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
                    var line = db.tblHRDIssuedDocuments.Where(z => z.HRDIssuedDocId == id).SingleOrDefault();
                    if (line != null)
                    {
                        db.tblHRDIssuedDocuments.Remove(line);
                    }
                    db.SaveChanges();
                    if (!string.IsNullOrEmpty(line.AttachmentName))
                    {
                        DeleteProfilePicture(line.AttachmentName);
                    }

                    //if (line.AttachmentName != "" && line.AttachmentName != null)
                    //{
                    //    DeleteProfilePicture(line.AttachmentName);
                    //}
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
        /// POST api/IssuedDocument
        /// create and update record
        /// </summary>
        [HttpPost]
        public ApiResponse SaveDocument(tblHRDIssuedDocument doc)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int timezone = Convert.ToInt32(nvc["timezone"]);

                    if (doc.HRDIssuedDocId == 0)
                    {// Mode == Add
                        tblHRDIssuedDocument d = new tblHRDIssuedDocument
                        {
                            ECode = doc.ECode,
                            FullName = doc.FullName,
                            DocumentTypeId = doc.DocumentTypeId,
                            FromDate = doc.FromDate,
                            ToDate = doc.ToDate,
                            AttachmentName = doc.AttachmentName,
                            DepartmentName = doc.DepartmentName,
                            IntercomNo = doc.IntercomNo,
                            IssuedBy = doc.IssuedBy,
                            IssuedOn = doc.IssuedOn,
                            Remarks = doc.Remarks,
                            CreDate = DateTime.Now.ToUniversalTime(),
                            CreBy = 1,
                            ChgDate = DateTime.Now.ToUniversalTime(),
                            ChgBy = 1
                        };
                        db.tblHRDIssuedDocuments.Add(d);
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);
                    }
                    else
                    {// Mode == Edit
                        var line = db.tblHRDIssuedDocuments.Where(z => z.HRDIssuedDocId == doc.HRDIssuedDocId).SingleOrDefault();
                        if (line != null)
                        {
                            line.ECode = doc.ECode;
                            line.FullName = doc.FullName;
                            line.DocumentTypeId = doc.DocumentTypeId;
                            line.FromDate = doc.FromDate;
                            line.ToDate = doc.ToDate;
                            line.AttachmentName = doc.AttachmentName;
                            line.DepartmentName = doc.DepartmentName;
                            line.IntercomNo = doc.IntercomNo;
                            line.IssuedBy = doc.IssuedBy;
                            line.IssuedOn = doc.IssuedOn;
                            line.Remarks = doc.Remarks;
                            line.ChgBy = 1;
                            line.ChgDate = DateTime.Now.ToUniversalTime();
                        }
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgUpdate, null);
                    }
                    db.SaveChanges();

                    MoveAttachmentToMainPath(doc.AttachmentName);
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
        /// POST api/IssuedDocument
        /// return document list with sorting and filtering  functionalities
        /// </summary>
        [HttpPost]
        public ApiResponse GetDocumentList(tblHRDIssuedDocument doc)
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
                    int documentTypeId = Convert.ToInt16(nvc["DocumentTypeId"]);
                    string startDate = nvc["startDate"];
                    string endDate = nvc["endDate"];

                    int iDisplayLength = Convert.ToInt32(nvc["count"]);
                    int iDisplayStart = (page - 1) * iDisplayLength;

                    List<tblHRDIssuedDocument> list = null;
                    try
                    {
                        list = db.tblHRDIssuedDocuments.AsEnumerable().ToList();

                        if (documentTypeId != 0)
                        {
                            list = list.Where(z => z.DocumentTypeId == documentTypeId).ToList();
                        }

                        //top filter
                        if (!string.IsNullOrEmpty(startDate))
                        {
                            string[] fdate = startDate.Split('/');
                            DateTime fromDate = new DateTime(Convert.ToInt32(fdate[2]), Convert.ToInt32(fdate[1]), Convert.ToInt32(fdate[0]));
                            if (startDate == endDate)
                            {
                                list = list.AsEnumerable().Where(z => z.IssuedOn.Date == fromDate.Date).ToList();
                            }
                            else
                            {//date range
                                string[] tdate = endDate.Split('/');
                                DateTime toDate = new DateTime(Convert.ToInt32(tdate[2]), Convert.ToInt32(tdate[1]), Convert.ToInt32(tdate[0]));
                                list = list.AsEnumerable().Where(z => z.IssuedOn.Date >= fromDate.Date && z.IssuedOn.Date <= toDate.Date).ToList();
                            }
                        }



                        //filter on columns
                        if (!string.IsNullOrEmpty(doc.ECode) && doc.ECode != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.ECode.ToLower().Contains(doc.ECode.ToLower())).ToList();
                        }
                        if (!string.IsNullOrEmpty(doc.FullName) && doc.FullName != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.FullName.ToLower().Contains(doc.FullName.ToLower())).ToList();
                        }
                        if (!string.IsNullOrEmpty(doc.DepartmentName) && doc.DepartmentName != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.DepartmentName.ToLower().Contains(doc.DepartmentName.ToLower())).ToList();
                        }
                        if (doc.DocumentTypeId != 0)
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.DocumentTypeId == doc.DocumentTypeId).ToList();
                        }

                        //convert returned datetime to local timezone
                        var lstDocs = list.Select(i =>
                        {
                            i.CreDate = Convert.ToDateTime(i.CreDate).AddMinutes(-1 * timezone);
                            i.ChgDate = Convert.ToDateTime(i.ChgDate).AddMinutes(-1 * timezone);
                            return i;
                        });

                        //get sub row for document id
                        foreach (var item in list)
                        {
                            item.tblDocument = db.tblDocuments.Where(z => z.Id == item.DocumentTypeId).FirstOrDefault();
                        }
                        //do sorting on list
                        list = DoSorting(lstDocs, orderBy.Trim());

                        //take total count to return for ng-table
                        var Count = list.Count();


                        var resultData = new
                        {
                            total = Count,
                            result = list.Skip(iDisplayStart).Take(iDisplayLength).ToList()
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
        /// return sorted list based on passed column
        /// </summary>
        public List<tblHRDIssuedDocument> DoSorting(IEnumerable<tblHRDIssuedDocument> list, string orderBy)
        {
            try
            {
                if (orderBy == "ECode")
                {
                    list = list.OrderBy(z => z.ECode).ToList();
                }
                else if (orderBy == "-ECode")
                {
                    list = list.OrderByDescending(z => z.ECode).ToList();
                }
                if (orderBy == "FullName")
                {
                    list = list.OrderBy(z => z.FullName).ToList();
                }
                else if (orderBy == "-FullName")
                {
                    list = list.OrderByDescending(z => z.FullName).ToList();
                }
                if (orderBy == "DepartmentName")
                {
                    list = list.OrderBy(z => z.DepartmentName).ToList();
                }
                else if (orderBy == "-DepartmentName")
                {
                    list = list.OrderByDescending(z => z.DepartmentName).ToList();
                }
                if (orderBy == "IssuedOn")
                {
                    list = list.OrderBy(z => z.IssuedOn).ToList();
                }
                else if (orderBy == "-IssuedOn")
                {
                    list = list.OrderByDescending(z => z.IssuedOn).ToList();
                }
                if (orderBy == "ChgDate")
                {
                    list = list.OrderBy(z => z.ChgDate).ToList();
                }
                else if (orderBy == "-ChgDate")
                {
                    list = list.OrderByDescending(z => z.ChgDate).ToList();
                }
                return list.ToList<tblHRDIssuedDocument>();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// common function for moving profile picture from temp folder to main folder
        /// </summary>
        protected void MoveAttachmentToMainPath(string fileName)
        {
            if (System.IO.File.Exists(HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["TempUploadPath"].ToString()) + "/" + fileName))
            {
                var tempSourceFile = HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["TempUploadPath"].ToString()) + "/" + fileName;
                var mainDestinationFile = HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["UploadIssuedDocument"].ToString()) + "/" + fileName;

                System.IO.File.Move(tempSourceFile, mainDestinationFile);
            }
        }

        private void DeleteProfilePicture(string profilePix)
        {
            string mainPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["UploadIssuedDocument"].ToString());

            if (File.Exists(Path.Combine(mainPath, profilePix)))
            {
                File.Delete(Path.Combine(mainPath, profilePix));
            }

        }
    }
}
