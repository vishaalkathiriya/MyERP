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
using System.Xml;
using System.Xml.Linq;

namespace ERP.WebApis
{
    public class KYCClientController : ApiController
    {
        ERPContext db = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Client";
        string _pageNameDirector = "Director/Partner";
        string _pageNameDocument = "Client Document";

        public KYCClientController()
        {
            db = new ERPContext();
            generalMessages = new GeneralMessages(_pageName);
        }

        [HttpGet]
        public ApiResponse RetrieveCountry()
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                var list = db.tblCountries.Where(z => z.IsActive == true).OrderBy(z => z.CountryName).ToList();

                apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", list);
            }
            catch (Exception ex)
            {
                apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
            }

            return apiResponse;
        }

        /// <summary>
        /// GET api/invoice client
        /// retrieve state list 
        /// </summary>
        [HttpGet]
        public ApiResponse RetrieveState()
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                int countryId = Convert.ToInt32(nvc["countryId"]);

                var list = db.tblStates.OrderBy(z => z.StateName)
                    .Where(z => z.CountryId == countryId)
                    .Select(z => new SelectItemModel
                    {
                        Id = z.StateId,
                        Label = z.StateName
                    }).ToList();

                apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", list);
            }
            catch (Exception ex)
            {
                apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
            }

            return apiResponse;
        }

        [HttpGet]
        public ApiResponse LoadClientSource()
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                var list = db.tblINVClientSources.OrderBy(z => z.SourceName)
                    .Where(z => z.IsActive == true)
                    .Select(z => new SelectItemModel
                    {
                        Id = z.PKSourceId,
                        Label = z.SourceName
                    }).ToList();

                apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", list);
            }
            catch (Exception ex)
            {
                apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
            }

            return apiResponse;
        }

        /// <summary>
        /// GET api/InvoiceClient
        /// retrieve nature of business list
        /// </summary>
        [HttpGet]
        public ApiResponse GetBusinessTypeList()
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                List<SelectItemModel> list = new List<SelectItemModel>();

                foreach (int value in Enum.GetValues(typeof(ERPUtilities.BusinessType)))
                {
                    list.Add(new SelectItemModel
                    {
                        Id = value,
                        Label = Enum.GetName(typeof(ERPUtilities.BusinessType), value)
                    });
                }

                apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", list);
            }
            catch (Exception ex)
            {
                apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
            }

            return apiResponse;
        }

        /// <summary>
        /// GET api/InvoiceClient
        /// retrieve identity and business registration documents only
        /// 1 = Identity Proof, 2 = Business Registraion Proof
        /// </summary>
        [HttpGet]
        public ApiResponse GetDocuments()
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                var data = db.tblDocuments.Where(z => z.DocumentTypeId == 1 || z.DocumentTypeId == 2 && z.IsActive == true).OrderBy(z=>z.Documents).ToList();

                apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", data);
            }
            catch (Exception ex)
            {
                apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
            }

            return apiResponse;
        }

        /// <summary>
        /// POST api/InvoiceClient
        /// create and update record
        /// </summary>
        [HttpPost]
        public ApiResponse SaveInvoiceClientPartial(INVClientViewModel cl)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                int cId;
                if (cl.PKClientId == 0)
                {// Mode == Add
                    tblINVClient client = new tblINVClient
                    {
                        ClientCode = string.Empty,
                        CompanyName = cl.CompanyName,
                        CPrefix = cl.CPrefix,
                        ContactPerson = cl.ContactPerson,
                        MobileNo = string.Empty,
                        CompanyAddress = string.Empty,
                        FKSourceId = cl.FKSourceId,
                        CountryId = cl.CountryId,
                        StateId = 0,
                        City = string.Empty,
                        PostalCode = string.Empty,
                        TelephoneNo = string.Empty,
                        FaxNo = string.Empty,
                        Email = string.Empty,
                        Website = string.Empty,
                        BusinessTypeId = 0,
                        BusinessStartDate = null,
                        LicenseNo = string.Empty,
                        IncomeTaxNo = string.Empty,
                        VATNo = string.Empty,
                        BankName = string.Empty,
                        BranchAddress = string.Empty,
                        BankType = string.Empty,
                        AccountNo = string.Empty,
                        IBANNumber = string.Empty,
                        SwiftCode = string.Empty,
                        IsConfirmed = false,
                        URLKey = string.Empty,
                        IsActive = true,
                        CreDate = DateTime.Now.ToUniversalTime(),
                        CreBy = 0,
                        ChgDate = DateTime.Now.ToUniversalTime(),
                        ChgBy = 0
                    };
                    db.tblINVClients.Add(client);
                    db.SaveChanges();

                    //getting last inserted client id
                    cId = db.tblINVClients.Max(z => z.PKClientId);
                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, cId);
                }
                else
                {
                    // Mode == Edit
                    var line = db.tblINVClients.Where(z => z.PKClientId == cl.PKClientId).SingleOrDefault();
                    if (line != null)
                    {
                        line.ClientCode = string.Empty;
                        line.CompanyName = cl.CompanyName;
                        line.CPrefix = cl.CPrefix;
                        line.ContactPerson = cl.ContactPerson;
                        line.MobileNo = string.Empty;
                        line.CompanyAddress = string.Empty;
                        line.FKSourceId = cl.FKSourceId;
                        line.CountryId = cl.CountryId;
                        line.StateId = 0;
                        line.City = string.Empty;
                        line.PostalCode = string.Empty;
                        line.TelephoneNo = string.Empty;
                        line.FaxNo = string.Empty;
                        line.Email = string.Empty;
                        line.Website = string.Empty;
                        line.BusinessTypeId = 0;
                        line.BusinessStartDate = null;
                        line.LicenseNo = string.Empty;
                        line.IncomeTaxNo = string.Empty;
                        line.VATNo = string.Empty;
                        line.BankName = string.Empty;
                        line.BranchAddress = string.Empty;
                        line.BankType = string.Empty;
                        line.AccountNo = string.Empty;
                        line.IBANNumber = string.Empty;
                        line.SwiftCode = string.Empty;
                        line.IsConfirmed = false;
                        line.ChgDate = DateTime.Now.ToUniversalTime();
                        line.ChgBy = 0;

                        db.SaveChanges();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgUpdate, cl.PKClientId);
                    }
                }

            }
            catch (Exception ex)
            {
                apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
            }

            return apiResponse;
        }

        /// <summary>
        /// POST api/InvoiceClient
        /// create and update record
        /// </summary>
        [HttpPost]
        public ApiResponse SaveInvoiceClientFull(INVClientViewModel cl)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                int cId;
                DateTime dtBuzz = new DateTime();
                dtBuzz = DateTime.ParseExact(cl.BusinessStartDateInString, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                if (cl.PKClientId == 0)
                {// Mode == Add

                    int maxClientId = db.tblINVClients.Max(z => z.PKClientId) + 1;

                    tblINVClient client = new tblINVClient
                    {
                        ClientCode = ERPUtilities.GetClientCode(maxClientId, cl.CompanyName, cl.CountryCode),
                        CompanyName = cl.CompanyName,
                        CPrefix = cl.CPrefix,
                        ContactPerson = cl.ContactPerson,
                        MobileNo = cl.MobileNo,
                        CompanyAddress = cl.CompanyAddress,
                        FKSourceId = cl.FKSourceId,
                        CountryId = cl.CountryId,
                        StateId = cl.StateId,
                        City = cl.City,
                        PostalCode = cl.PostalCode,
                        TelephoneNo = cl.TelephoneNo == null ? string.Empty : cl.TelephoneNo,
                        FaxNo = cl.FaxNo == null ? string.Empty : cl.FaxNo,
                        Email = cl.Email,
                        Website = cl.Website == null ? string.Empty : cl.Website,
                        BusinessTypeId = cl.BusinessTypeId,
                        BusinessStartDate = dtBuzz,
                        LicenseNo = cl.LicenseNo == null ? string.Empty : cl.LicenseNo,
                        IncomeTaxNo = cl.IncomeTaxNo == null ? string.Empty : cl.IncomeTaxNo,
                        VATNo = cl.VATNo == null ? string.Empty : cl.VATNo,
                        BankName = cl.BankName,
                        BranchAddress = cl.BranchAddress,
                        BankType = cl.BankType,
                        AccountNo = cl.AccountNo,
                        IBANNumber = cl.IBANNumber == null ? string.Empty : cl.IBANNumber,
                        SwiftCode = cl.SwiftCode == null ? string.Empty : cl.SwiftCode,
                        IsConfirmed = true,
                        URLKey = string.Empty,
                        IsActive = true,
                        CreDate = DateTime.Now.ToUniversalTime(),
                        CreBy = 0,
                        ChgDate = DateTime.Now.ToUniversalTime(),
                        ChgBy = 0
                    };
                    db.tblINVClients.Add(client);
                    db.SaveChanges();

                    //getting last inserted client id
                    cId = db.tblINVClients.Max(z => z.PKClientId);
                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, cId);
                }
                else
                {
                    // Mode == Edit
                    var line = db.tblINVClients.Where(z => z.PKClientId == cl.PKClientId).SingleOrDefault();
                    if (line != null)
                    {
                        line.ClientCode = !line.IsConfirmed ? ERPUtilities.GetClientCode(cl.PKClientId, cl.CompanyName, cl.CountryCode) : line.ClientCode;
                        line.CompanyName = cl.CompanyName;
                        line.CPrefix = cl.CPrefix;
                        line.ContactPerson = cl.ContactPerson;
                        line.MobileNo = cl.MobileNo;
                        line.CompanyAddress = cl.CompanyAddress;
                        line.FKSourceId = cl.FKSourceId;
                        line.CountryId = cl.CountryId;
                        line.StateId = cl.StateId;
                        line.City = cl.City;
                        line.PostalCode = cl.PostalCode;
                        line.TelephoneNo = cl.TelephoneNo == null ? string.Empty : cl.TelephoneNo;
                        line.FaxNo = cl.FaxNo == null ? string.Empty : cl.FaxNo;
                        line.Email = cl.Email;
                        line.Website = cl.Website == null ? string.Empty : cl.Website;
                        line.BusinessTypeId = cl.BusinessTypeId;
                        line.BusinessStartDate = dtBuzz;
                        line.LicenseNo = cl.LicenseNo == null ? string.Empty : cl.LicenseNo;
                        line.IncomeTaxNo = cl.IncomeTaxNo == null ? string.Empty : cl.IncomeTaxNo;
                        line.VATNo = cl.VATNo == null ? string.Empty : cl.VATNo;
                        line.BankName = cl.BankName;
                        line.BranchAddress = cl.BranchAddress;
                        line.BankType = cl.BankType;
                        line.AccountNo = cl.AccountNo;
                        line.IBANNumber = cl.IBANNumber == null ? string.Empty : cl.IBANNumber;
                        line.SwiftCode = cl.SwiftCode == null ? string.Empty : cl.SwiftCode;
                        line.IsConfirmed = true;
                        line.ChgDate = DateTime.Now.ToUniversalTime();
                        line.ChgBy = 0;

                        db.SaveChanges();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgUpdate, cl.PKClientId);
                    }
                }
            }
            catch (Exception ex)
            {
                apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
            }

            return apiResponse;
        }

        /// <summary>
        /// POST api/InvoiceClient
        /// create and update director list
        /// </summary>
        [HttpPost]
        public ApiResponse SaveDirector(tblINVClientPerson cp)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (cp.PKId == 0)
                {// Mode == Add
                    tblINVClientPerson person = new tblINVClientPerson
                    {
                        FKClientId = cp.FKClientId,
                        Prefix = cp.Prefix,
                        FullName = cp.FullName,
                        Designation = cp.Designation,
                        IdentityDocId = cp.IdentityDocId,
                        IdentityNo = cp.IdentityNo,
                        Email = cp.Email,
                        MobileNo = cp.MobileNo,
                        IsActive = true,
                        CreDate = DateTime.Now.ToUniversalTime(),
                        CreBy = 0,
                        ChgDate = DateTime.Now.ToUniversalTime(),
                        ChgBy = 0
                    };
                    db.tblINVClientPersons.Add(person);
                    db.SaveChanges();

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);
                }
                else
                {
                    // Mode == Edit
                }
            }
            catch (Exception ex)
            {
                apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
            }
            return apiResponse;
        }

        /// <summary>
        /// POST api/InvoiceClient
        /// create and update document list
        /// </summary>
        [HttpPost]
        public ApiResponse SaveDocument(tblINVDocument d)
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                if (d.PKDocId == 0)
                {// Mode == Add
                    tblINVDocument doc = new tblINVDocument
                    {
                        tblRefId = d.tblRefId,
                        DocId = d.DocId,
                        DocName = d.DocName,
                        DocTypeId = d.DocTypeId,
                        IsActive = true,
                        CreDate = DateTime.Now.ToUniversalTime(),
                        CreBy = 0,
                        ChgDate = DateTime.Now.ToUniversalTime(),
                        ChgBy = 0
                    };
                    db.tblINVDocuments.Add(doc);
                    db.SaveChanges();

                    MoveDocumentFile(d.DocName); //move uploaded file from temp folder

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);
                }
                else
                {
                    // Mode == Edit
                }
            }
            catch (Exception ex)
            {
                apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
            }
            return apiResponse;
        }

        /// <summary>
        /// POST api/InvoiceClient
        /// delete document 
        /// </summary>
        [HttpGet]
        public ApiResponse DeleteDocument()
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                int docId = Convert.ToInt32(nvc["docId"].ToString());

                var line = db.tblINVDocuments.Where(z => z.PKDocId == docId).SingleOrDefault();
                if (line != null)
                {
                    db.tblINVDocuments.Remove(line);
                }

                db.SaveChanges();
                DeleteClientDocument(line.DocName);

                GeneralMessages generalM2 = new GeneralMessages(_pageNameDocument);
                apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalM2.msgDelete, null);
            }
            catch (Exception ex)
            {
                apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageNameDocument, true);
            }

            return apiResponse;
        }

        /// <summary>
        /// POST api/InvoiceClient
        /// delete director 
        /// </summary>
        [HttpGet]
        public ApiResponse DeleteDirector()
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                int Id = Convert.ToInt32(nvc["Id"].ToString());

                var line = db.tblINVClientPersons.Where(z => z.PKId == Id).SingleOrDefault();
                if (line != null)
                {
                    db.tblINVClientPersons.Remove(line);
                }

                db.SaveChanges();
                GeneralMessages generalM1 = new GeneralMessages(_pageNameDirector);
                apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalM1.msgDelete, null);
            }
            catch (Exception ex)
            {
                apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageNameDirector, true);
            }

            return apiResponse;
        }

        [HttpGet]
        public ApiResponse ValidateURL()
        {
            ApiResponse apiResponse = new ApiResponse();
            try
            {
                NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                string urlkey = nvc["key"].ToString();

                var line = db.tblINVClients.Where(z => z.URLKey == urlkey).FirstOrDefault();

                if (line != null)
                {
                    //var line = db.tblINVClients.Where(z => z.PKClientId == clientId).SingleOrDefault();
                    var personlist = db.tblINVClientPersons.Where(z => z.FKClientId == line.PKClientId && z.IsActive == true).ToList();
                    var documentlist = db.tblINVDocuments.Where(z => z.tblRefId == line.PKClientId && z.IsActive == true).ToList();

                    INVClientViewModel invoice = new INVClientViewModel()
                    {
                        PKClientId = line.PKClientId,
                        ClientCode = line.ClientCode,
                        CompanyName = line.CompanyName,
                        CPrefix = line.CPrefix,
                        ContactPerson = line.ContactPerson,
                        MobileNo = line.MobileNo,
                        CompanyAddress = line.CompanyAddress,
                        FKSourceId = line.FKSourceId,
                        CountryId = line.CountryId,
                        StateId = line.StateId,
                        City = line.City,
                        PostalCode = line.PostalCode,
                        TelephoneNo = line.TelephoneNo,
                        FaxNo = line.FaxNo,
                        Email = line.Email,
                        Website = line.Website,
                        BusinessTypeId = line.BusinessTypeId,
                        BusinessStartDate = line.BusinessStartDate,
                        LicenseNo = line.LicenseNo,
                        IncomeTaxNo = line.IncomeTaxNo,
                        VATNo = line.VATNo,
                        BankName = line.BankName,
                        BranchAddress = line.BranchAddress,
                        BankType = line.BankType,
                        AccountNo = line.AccountNo,
                        IBANNumber = line.IBANNumber,
                        SwiftCode = line.SwiftCode,
                        IsConfirmed = line.IsConfirmed,
                        ClientPersonList = personlist,
                        ClientDocumentList = documentlist
                    };

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", invoice);
                }
                else
                {
                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", line);
                }

            }
            catch (Exception ex)
            {
                apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
            }

            return apiResponse;
        }

        #region Common functions
        /// <summary>
        /// move profile picture from temp folder to it's main folder
        /// </summary>
        protected void MoveDocumentFile(string fileName)
        {
            if (System.IO.File.Exists(HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["TempThumbnails"].ToString()) + "/" + fileName))
            {
                var thumbSource = HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["TempThumbnails"].ToString()) + "/" + fileName;
                var thumbDestination = HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["invClientThumbnails"].ToString()) + "/" + fileName;

                var mainSource = HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["TempUploadPath"].ToString()) + "/" + fileName;
                var mainDestination = HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["invClientUploads"].ToString()) + "/" + fileName;

                System.IO.File.Move(thumbSource, thumbDestination);
                System.IO.File.Move(mainSource, mainDestination);
            }
        }

        /// <summary>
        /// //delete client document
        /// </summary>
        private void DeleteClientDocument(string filename)
        {
            string mainPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["invClientUploads"].ToString());
            string thumbPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["invClientThumbnails"].ToString());

            if (File.Exists(Path.Combine(mainPath, filename)))
            {
                File.Delete(Path.Combine(mainPath, filename));
            }
            if (File.Exists(Path.Combine(thumbPath, filename)))
            {
                File.Delete(Path.Combine(thumbPath, filename));
            }
        }

        /// <summary>
        /// //delete client conversation document
        /// </summary>
        private void DeleteConversationDocument(string filename)
        {
            string mainPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["invConversationUploads"].ToString());

            if (File.Exists(Path.Combine(mainPath, filename)))
            {
                File.Delete(Path.Combine(mainPath, filename));
            }
        }
        #endregion
    }
}
