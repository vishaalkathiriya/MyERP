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
    public class InvoiceClientController : ApiController
    {
        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Client";
        string _pageNameDirector = "Director/Partner";
        string _pageNameDocument = "Client Document";

        public InvoiceClientController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }

        [HttpGet]
        public ApiResponse RetrieveCountry()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    //var list = db.tblCountries.OrderBy(z => z.CountryName)
                    //    .Where(z => z.IsActive == true)
                    //    .Select(z => new SelectItemModel
                    //    {
                    //        Id = z.CountryId,
                    //        Label = z.CountryName
                    //    }).ToList();
                    var list = db.tblCountries.Where(z=>z.IsActive == true).OrderBy(z => z.CountryName).ToList();

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
        /// GET api/invoice client
        /// retrieve state list 
        /// </summary>
        [HttpGet]
        public ApiResponse RetrieveState()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
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
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }

        [HttpGet]
        public ApiResponse LoadClientSource()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var list = db.tblINVClientSources.OrderBy(z => z.SourceName)
                        .Where(z => z.IsActive == true && z.IsDeleted == false)
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
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
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
            if (sessionUtils.HasUserLogin())
            {
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
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
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
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var data = db.tblDocuments.Where(z => z.DocumentTypeId == 1 || z.DocumentTypeId == 2 && z.IsActive == true).OrderBy(z=>z.Documents).ToList();

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", data);
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
        /// GET api/InvoiceClient
        /// return client
        /// </summary>
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

                    var line = db.tblINVClients.Where(z=>z.PKClientId == clientId).SingleOrDefault();
                    var personlist = db.tblINVClientPersons.Where(z => z.FKClientId == line.PKClientId && z.IsActive == true).ToList();
                    var documentlist = db.tblINVDocuments.Where(z => z.tblRefId == line.PKClientId && z.IsActive == true && (z.DocTypeId == 1 || z.DocTypeId == 2)).ToList();

                    INVClientViewModel invoice = new INVClientViewModel() {
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
                        URLKey = line.URLKey,
                        IsKYCApproved = line.IsKYCApproved,
                        ClientPersonList = personlist,
                        ClientDocumentList = documentlist
                    };

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", invoice);
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
        /// POST api/InvoiceClient
        /// create and update record
        /// </summary>
        [HttpPost]
        public ApiResponse SaveInvoiceClientPartial(INVClientViewModel cl)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
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
                            IsKYCApproved = false,

                            URLKey = string.Empty,

                            IsActive = true,
                            IsDeleted = false,
                            CreDate = DateTime.Now.ToUniversalTime(),
                            CreBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString()),
                            ChgDate = DateTime.Now.ToUniversalTime(),
                            ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString())
                        };
                        db.tblINVClients.Add(client);
                        db.SaveChanges();

                        //return last inserted client id
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, client.PKClientId);
                    }
                    else {
                        // Mode == Edit
                        var line = db.tblINVClients.Where(z => z.PKClientId == cl.PKClientId).SingleOrDefault();
                        if (line != null) { 
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
                            line.IsKYCApproved = false;
                            line.ChgDate = DateTime.Now.ToUniversalTime();
                            line.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());

                            db.SaveChanges();
                            apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgUpdate, cl.PKClientId);
                        }
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
        /// POST api/InvoiceClient
        /// create and update record
        /// </summary>
        [HttpPost]
        public ApiResponse SaveInvoiceClientFull(INVClientViewModel cl)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    int cId;
                    DateTime dtBuzz = new DateTime();
                    if (!string.IsNullOrEmpty(cl.BusinessStartDateInString)) {
                        dtBuzz = DateTime.ParseExact(cl.BusinessStartDateInString, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    }

                    if (cl.PKClientId == 0)
                    {// Mode == Add
                        tblINVClient client = new tblINVClient
                        {
                            //ClientCode = ERPUtilities.GetClientCode(maxClientId, cl.CompanyName, cl.CountryCode),
                            ClientCode = cl.ClientCode.ToUpper(),
                            CompanyName = cl.CompanyName,
                            CPrefix = cl.CPrefix,
                            ContactPerson = cl.ContactPerson,
                            MobileNo = cl.MobileNo,
                            CompanyAddress = cl.CompanyAddress,
                            FKSourceId = cl.FKSourceId,
                            CountryId = cl.CountryId,
                            StateId = cl.StateId,
                            City = cl.City == null ? string.Empty : cl.City,
                            PostalCode = cl.PostalCode == null? string.Empty : cl.PostalCode,
                            TelephoneNo = cl.TelephoneNo == null ? string.Empty : cl.TelephoneNo,
                            FaxNo = cl.FaxNo == null ? string.Empty : cl.FaxNo,
                            Email = cl.Email == null ? string.Empty : cl.Email,
                            Website = cl.Website == null ? string.Empty : cl.Website,
                            BusinessTypeId = cl.BusinessTypeId,
                            BusinessStartDate = !string.IsNullOrEmpty(cl.BusinessStartDateInString) ? dtBuzz : (DateTime?)null,
                            LicenseNo = cl.LicenseNo == null ? string.Empty : cl.LicenseNo,
                            IncomeTaxNo = cl.IncomeTaxNo == null ? string.Empty : cl.IncomeTaxNo,
                            VATNo = cl.VATNo == null ? string.Empty : cl.VATNo,
                            BankName = cl.BankName == null ? string.Empty : cl.BankName,
                            BranchAddress = cl.BranchAddress == null ? string.Empty : cl.BranchAddress,
                            BankType = cl.BankType == null ? string.Empty : cl.BankType,
                            AccountNo = cl.AccountNo == null ? string.Empty : cl.AccountNo,
                            IBANNumber = cl.IBANNumber == null ? string.Empty : cl.IBANNumber,
                            SwiftCode = cl.SwiftCode == null ? string.Empty : cl.SwiftCode,
                            IsConfirmed = true,
                            IsKYCApproved = cl.IsKYCApproved,
                            URLKey = string.Empty,
                            IsActive = true,
                            IsDeleted = false,
                            CreDate = DateTime.Now.ToUniversalTime(),
                            CreBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString()),
                            ChgDate = DateTime.Now.ToUniversalTime(),
                            ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString())
                        };
                        db.tblINVClients.Add(client);
                        db.SaveChanges();

                        //getting last inserted client id
                        cId = db.tblINVClients.Max(z => z.PKClientId);
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, cId);
                    }
                    else {
                        // Mode == Edit
                        var line = db.tblINVClients.Where(z => z.PKClientId == cl.PKClientId).SingleOrDefault();
                        if (line != null)
                        {
                            //line.ClientCode = !line.IsConfirmed ? ERPUtilities.GetClientCode(cl.PKClientId, cl.CompanyName, cl.CountryCode) : line.ClientCode;
                            line.ClientCode = cl.ClientCode.ToUpper();
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
                            line.BusinessStartDate = !string.IsNullOrEmpty(cl.BusinessStartDateInString) ? dtBuzz : (DateTime?)null;
                            line.LicenseNo = cl.LicenseNo == null ? string.Empty : cl.LicenseNo;
                            line.IncomeTaxNo = cl.IncomeTaxNo == null ? string.Empty : cl.IncomeTaxNo;
                            line.VATNo = cl.VATNo == null ? string.Empty : cl.VATNo;
                            line.BankName = cl.BankName == null ? string.Empty : cl.BankName;
                            line.BranchAddress = cl.BranchAddress == null ? string.Empty : cl.BranchAddress;
                            line.BankType = cl.BankType == null ? string.Empty : cl.BankType;
                            line.AccountNo = cl.AccountNo == null ? string.Empty : cl.AccountNo;
                            line.IBANNumber = cl.IBANNumber == null ? string.Empty : cl.IBANNumber;
                            line.SwiftCode = cl.SwiftCode == null ? string.Empty : cl.SwiftCode;
                            line.IsConfirmed = true;
                            line.IsKYCApproved = cl.IsKYCApproved;
                            line.ChgDate = DateTime.Now.ToUniversalTime();
                            line.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());

                            db.SaveChanges();
                            apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgUpdate, cl.PKClientId);
                        }
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
        /// POST api/InvoiceClient
        /// create and update director list
        /// </summary>
        [HttpPost]
        public ApiResponse SaveDirector(tblINVClientPerson cp)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (cp.PKId == 0) {// Mode == Add
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
                            CreBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString()),
                            ChgDate = DateTime.Now.ToUniversalTime(),
                            ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString())
                        };
                        db.tblINVClientPersons.Add(person);
                        db.SaveChanges();

                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);
                    }
                    else {
                        // Mode == Edit
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

        /// <summary>
        /// POST api/InvoiceClient
        /// create and update document list
        /// </summary>
        [HttpPost]
        public ApiResponse SaveDocument(tblINVDocument d)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (d.PKDocId == 0) {// Mode == Add
                        tblINVDocument doc = new tblINVDocument
                        {
                            tblRefId = d.tblRefId,
                            DocId = d.DocId,
                            DocName = d.DocName,
                            DocTypeId = d.DocTypeId,
                            IsActive = true,
                            CreDate = DateTime.Now.ToUniversalTime(),
                            CreBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString()),
                            ChgDate = DateTime.Now.ToUniversalTime(),
                            ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString())
                        };
                        db.tblINVDocuments.Add(doc);
                        db.SaveChanges();

                        MoveDocumentFile(d.DocName); //move uploaded file from temp folder

                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, null);
                    }
                    else {
                        // Mode == Edit
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
        /// POST api/InvoiceClient
        /// delete document 
        /// </summary>
        [HttpGet]
        public ApiResponse DeleteDocument()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int docId = Convert.ToInt32(nvc["docId"].ToString());

                    var line = db.tblINVDocuments.Where(z => z.PKDocId == docId).SingleOrDefault();
                    if (line != null) {
                        db.tblINVDocuments.Remove(line);
                    }

                    db.SaveChanges();
                    DeleteClientDocument(line.DocName);

                    GeneralMessages generalM2 = new GeneralMessages(_pageNameDocument);
                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalM2.msgDelete, null);
                }
                catch (Exception ex) {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageNameDocument, true);
                }
            }
            else {
                apiResponse = ERPUtilities.GenerateApiResponse();
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
            if (sessionUtils.HasUserLogin())
            {
                try {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int Id = Convert.ToInt32(nvc["Id"].ToString());

                    var line = db.tblINVClientPersons.Where(z => z.PKId == Id).SingleOrDefault();
                    if (line != null) {
                        db.tblINVClientPersons.Remove(line);
                    }

                    db.SaveChanges();
                    GeneralMessages generalM1 = new GeneralMessages(_pageNameDirector);
                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalM1.msgDelete, null);
                }
                catch (Exception ex) {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageNameDirector, true);
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }

        /// <summary>
        /// GET api/InvoiceClient
        /// return client confirm or not
        /// </summary>
        [HttpGet]
        public ApiResponse IsClientConfirmed()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try 
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int cId = Convert.ToInt32(nvc["cId"].ToString());

                    var isConfirmed = db.tblINVClients.Where(z => z.PKClientId == cId).Select(z=>z.IsConfirmed).SingleOrDefault();

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

        /// <summary>
        /// POST api/InvoiceClient
        /// delete invoice client
        /// </summary>
        [HttpPost]
        public ApiResponse DeleteClient([FromBody]int id)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    //Director list
                    var lstDirector = db.tblINVClientPersons.Where(z => z.FKClientId == id).ToList();
                    foreach (var l in lstDirector)
                    {
                        l.IsActive = false;
                    }

                    //Document list
                    var lstDocs = db.tblINVDocuments.Where(z => z.tblRefId == id).ToList();
                    foreach (var l in lstDocs)
                    {
                        //DeleteClientDocument(l.DocName);
                        l.IsActive = false;
                    }

                    //Conversation list
                    var lstConversation = db.tblINVConversations.Where(z => z.FKClientId == id).ToList();
                    foreach (var l in lstConversation) {
                        var lstDoc = db.tblINVDocuments.Where(z => z.tblRefId == l.PKConversationId).ToList();
                        foreach (var d in lstDoc) { 
                            //DeleteConversationDocument(d.DocName);
                            d.IsActive = false;
                        }
                        l.IsDeleted = true;
                    }

                    db.SaveChanges();

                    //Child entries are deleted; now delete main entry
                    var line = db.tblINVClients.Where(z => z.PKClientId == id).SingleOrDefault();
                    if (line != null) {
                        line.IsDeleted = true;
                    }
                    db.SaveChanges();
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
        /// POST api/invoiceclient
        /// active-inActive record 
        /// </summary>
        [HttpPost]
        public ApiResponse ChangeStatus(tblINVClient cl)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var line = db.tblINVClients.Where(z => z.PKClientId == cl.PKClientId).SingleOrDefault();
                    if (line != null)
                    {
                        if (cl.IsActive)
                        {
                            line.IsActive = false;
                        }
                        else if (!cl.IsActive)
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

        /// <summary>
        /// GET api/employee
        /// return employee list with sorting and filtering  functionalities
        /// </summary>
        [HttpPost]
        public ApiResponse GetClientList(INVClientViewModel cl)
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

                    string KYCApproved = nvc["KYCApproved"];
                    int countryId = Convert.ToInt32(nvc["countryId"]);

                    int iDisplayLength = Convert.ToInt32(nvc["count"]);
                    int iDisplayStart = (page - 1) * iDisplayLength;

                    List<tblINVClient> list = new List<tblINVClient>();
                    try
                    {
                        list = db.tblINVClients.Where(z=>z.IsDeleted == false).ToList();

                        //top filter
                        if (!string.IsNullOrEmpty(KYCApproved) && KYCApproved != "0")
                        {
                            bool isApproved = KYCApproved == "A" ? true : false;
                            list = list.Where(z => z.IsKYCApproved == isApproved).ToList();
                        }
                        if (countryId > 0)
                        {
                            list = list.Where(z => z.CountryId == countryId).ToList();
                        }

                        //filter for col client code
                        if (!string.IsNullOrEmpty(cl.ClientCode) && cl.ClientCode != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.ClientCode.ToLower().Contains(cl.ClientCode.ToLower())).ToList();
                        }
                        //filter for col company name
                        if (!string.IsNullOrEmpty(cl.CompanyName) && cl.CompanyName != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.CompanyName.ToLower().Contains(cl.CompanyName.ToLower())).ToList();
                        }
                        //filter for col contact person
                        if (!string.IsNullOrEmpty(cl.ContactPerson) && cl.ContactPerson != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.ContactPerson.ToLower().Contains(cl.ContactPerson.ToLower())).ToList();
                        }


                        //do sorting on list
                        list = DoSorting(list, orderBy.Trim());

                        //take total count to return for ng-table
                        var Count = list.Count();

                        var Clients = list.Select(i => 
                        {
                            i.ChgDate = Convert.ToDateTime(i.ChgDate).AddMinutes(-1 * timezone);
                            return i;
                        }).Skip(iDisplayStart).Take(iDisplayLength).ToList();

                        var resultData = new
                        {
                            total = Count,
                            result = Clients
                        };

                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", resultData);
                    }
                    catch (Exception ex)
                    {
                        apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
                    }
                    finally {
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
        public List<tblINVClient> DoSorting(IEnumerable<tblINVClient> list, string orderBy)
        {
            try
            {
                if (orderBy == "ClientCode")
                {
                    list = list.OrderBy(z => z.ClientCode).ToList();
                }
                else if (orderBy == "-ClientCode")
                {
                    list = list.OrderByDescending(z => z.ClientCode).ToList();
                }
                if (orderBy == "CompanyName")
                {
                    list = list.OrderBy(z => z.CompanyName).ToList();
                }
                else if (orderBy == "-CompanyName")
                {
                    list = list.OrderByDescending(z => z.CompanyName).ToList();
                }
                if (orderBy == "ContactPerson")
                {
                    list = list.OrderBy(z => z.ContactPerson).ToList();
                }
                else if (orderBy == "-ContactPerson")
                {
                    list = list.OrderByDescending(z => z.ContactPerson).ToList();
                }
                else if (orderBy == "ChgDate")
                {
                    list = list.OrderBy(z => z.ChgDate).ToList();
                }
                else if (orderBy == "-ChgDate")
                {
                    list = list.OrderByDescending(z => z.ChgDate).ToList();
                }
                return list.ToList<tblINVClient>();
            }
            catch
            {
                return null;
            }
        }

        [HttpGet]
        public ApiResponse GenerateLink()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int cId = Convert.ToInt32(nvc["cId"].ToString());

                    string urlkey = ERPUtilities.GenerateKey();
                    var line = db.tblINVClients.Where(z => z.PKClientId == cId).FirstOrDefault();

                    if (line != null)
                    {
                        line.URLKey = urlkey;
                        db.SaveChanges();

                        string link = string.Format("{0}{1}", ConfigurationManager.AppSettings["lnkKYC"].ToString(), urlkey);
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", link);
                    }
                    else { 
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", string.Empty);
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
        public ApiResponse DeleteLink()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int cId = Convert.ToInt32(nvc["cId"].ToString());

                    var line = db.tblINVClients.Where(z => z.PKClientId == cId).FirstOrDefault();
                    if (line != null) {
                        line.URLKey = string.Empty;
                    }

                    db.SaveChanges();
                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", null);
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

        [HttpGet]
        public ApiResponse ApproveKYC()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int cId = Convert.ToInt32(nvc["cId"].ToString());

                    var line = db.tblINVClients.Where(z => z.PKClientId == cId).FirstOrDefault();
                    if (line != null)
                    {
                        line.IsKYCApproved = !line.IsKYCApproved;
                        line.ChgDate = DateTime.Now.ToUniversalTime();
                        line.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                    }

                    db.SaveChanges();
                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgUpdate, line.IsKYCApproved);
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


        [HttpGet]
        public ApiResponse CheckClientCode()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    string cCode = nvc["code"].ToUpper().ToString();

                    if (db.tblINVClients.Where(z => z.ClientCode == cCode && z.IsActive == true).Count() > 0) 
                    { //Exists
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 2, "", true);
                    }
                    else { //Not Exists
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", true);
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

        /*client overview page*/
        /// <summary>
        /// GET api/InvoiceClient
        /// return client overview
        /// </summary>
        [HttpGet]
        public ApiResponse GetClientOverView()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int clientId = Convert.ToInt32(nvc["cId"].ToString());

                    var line = db.tblINVClients.Where(z => z.PKClientId == clientId).SingleOrDefault();
                    var personlist = db.tblINVClientPersons.Where(z => z.FKClientId == line.PKClientId && z.IsActive == true).ToList();
                    var documentlist = db.tblINVDocuments.Where(z => z.tblRefId == line.PKClientId && z.IsActive == true && (z.DocTypeId == 1 || z.DocTypeId == 2)).ToList();
                    var countryLine = db.tblCountries.Where(z => z.CountryId == line.CountryId).SingleOrDefault();
                    var cList = db.tblINVConversations.Where(z => z.FKClientId == line.PKClientId && z.IsDeleted == false).OrderByDescending(z=>z.ChgDate).Take(3).ToList();
                    var iList = db.tblINVInquiries.Where(z => z.FKClientId == line.PKClientId && z.IsActive == true && z.IsDeleted == false).OrderByDescending(z=>z.ChgDate).Take(3).ToList();
 
                    //Handle person list
                    ICollection<INVClientPersonViewModel> pList = new List<INVClientPersonViewModel>();
                    foreach (var p in personlist) {
                        INVClientPersonViewModel person = new INVClientPersonViewModel()
                        {
                            PKId = p.PKId,
                            FKClientId = p.FKClientId,
                            Prefix = p.Prefix,
                            FullName = p.FullName,
                            Designation = p.Designation,
                            Identity = string.Format("{0} : {1}", db.tblDocuments.Where(z=>z.Id == p.IdentityDocId).Select(z=>z.Documents).SingleOrDefault(), p.IdentityNo),
                            Email = p.Email,
                            MobileNo = p.MobileNo
                        };
                        pList.Add(person);
                    }

                    //Handle document list
                    ICollection<INVDocumentViewModel> dList = new List<INVDocumentViewModel>();
                    foreach (var d in documentlist) {
                        INVDocumentViewModel doc = new INVDocumentViewModel()
                        {
                            PKDocId = d.PKDocId,
                            DocId = d.DocId,
                            tblRefId = d.tblRefId,
                            DocName = d.DocName,
                            DocTypeName = db.tblDocuments.Where(z => z.Id == d.DocId).Select(z => z.Documents).SingleOrDefault()
                        };
                        dList.Add(doc);
                    }

                    INVClientOverviewViewModel invoice = new INVClientOverviewViewModel()
                    {
                        PKClientId = line.PKClientId,
                        ClientCode = line.ClientCode,
                        CompanyName = line.CompanyName,
                        CPrefix = line.CPrefix,
                        ContactPerson = line.ContactPerson,
                        MobileNo = string.Format("{0} {1}", countryLine.DialCode, line.MobileNo),
                        CompanyAddress = line.CompanyAddress,
                        SourceName = db.tblINVClientSources.Where(z=>z.PKSourceId == line.FKSourceId).Select(z=>z.SourceName).SingleOrDefault(),
                        CountryName = countryLine.CountryName,
                        StateName = db.tblStates.Where(z => z.StateId == line.StateId).Select(z => z.StateName).SingleOrDefault(),
                        City = line.City,
                        PostalCode = line.PostalCode,
                        TelephoneNo = string.Format("{0} {1}", countryLine.DialCode, line.TelephoneNo),
                        FaxNo = string.Format("{0} {1}", countryLine.DialCode, line.FaxNo),
                        Email = line.Email,
                        Website = line.Website,
                        BusinessType = Enum.GetName(typeof(ERPUtilities.BusinessType), line.BusinessTypeId),
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
                        ClientPersonList = pList,
                        ClientDocumentList = dList,
                        ClientConversationList = cList,
                        ClientInquiryList = iList
                    };

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", invoice);
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
