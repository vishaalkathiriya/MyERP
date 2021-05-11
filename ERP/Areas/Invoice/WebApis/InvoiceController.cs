using ERP.Models;
using ERP.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace ERP.Areas.Invoice.WebApis
{
    public class InvoiceController : ApiController
    {
        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Invoice";
        string _pageNameTax = "Invoice Tax";

        public InvoiceController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }

        [HttpGet]
        public ApiResponse DeleteInvoiceTax()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int invoiceTaxId = Convert.ToInt32(nvc["invoiceTaxId"].ToString());

                    GeneralMessages generalMsg = new GeneralMessages(_pageNameTax);

                    var line = db.tblINVInvoiceTaxes.Where(z => z.PKInvoiceTaxId == invoiceTaxId).SingleOrDefault();
                    if (line != null)
                    {
                        db.tblINVInvoiceTaxes.Remove(line);
                        //line.IsDeleted = true;
                        //line.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                        //line.ChgDate = DateTime.Now;
                        db.SaveChanges();
                    }

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMsg.msgDelete, null);
                }
                catch (Exception ex) {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageNameTax, true);
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }
            return apiResponse;
        }

        [HttpGet]
        public ApiResponse DeleteInvoice()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int invoiceId = Convert.ToInt32(nvc["invoiceId"].ToString());

                    if (DependancyStatus.InvoicePayment(invoiceId))
                    {
                        var line = db.tblINVInvoices.Where(z => z.PKInvoiceId == invoiceId).SingleOrDefault();
                        if (line != null) 
                        {
                            line.IsDeleted = true;
                            line.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                            line.ChgDate = DateTime.Now;
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

        [HttpGet]
        public ApiResponse GetTaxList()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try {
                    var list = db.tblINVTaxMasters.Where(z => z.IsActive == true).Select(z=> new { z.PKTaxId, z.TaxTypeName, z.Mode, z.Percentage}).ToList();
                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", list);
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
        public ApiResponse GetMilestoneList()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    string milestoneIds = nvc["milestoneIds"].ToString();

                    string[] words = milestoneIds.Split(',');
                    List<tblINVMilestone> mList = new List<tblINVMilestone>();

                    foreach (var id in words)
                    {
                        var line = db.tblINVMilestones.AsEnumerable().Where(z => z.PKMilestoneId == Convert.ToInt32(id)).SingleOrDefault();
                        line.tblINVProject = db.tblINVProjects.Where(z => z.PKProjectId == line.FKProjectId).FirstOrDefault();
                        mList.Add(line);
                    }

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", mList);
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
        public ApiResponse CreateUpdateInvoice(tblINVInvoice invoice)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (invoice.PKInvoiceId == 0) 
                    {
                        //generate invoice code
                        string invoiceNo = string.Empty;
                        int month = invoice.InvoiceDate.Month;
                        int year = invoice.InvoiceDate.Year;
                        string yr = (year - 1).ToString().Substring(2);
                        int iCount = db.tblINVInvoices.Count() + 1;

                        invoiceNo = string.Format("{0}{1}{2}{3}",
                            invoice.InvoiceType,
                            //db.tblINVClients.Where(z => z.PKClientId == invoice.FKClientId).Select(z => z.CompanyName).SingleOrDefault().Substring(0, 3).ToUpper(),
                            (char)('A' + Convert.ToInt32(yr)),
                            (char)('A' + month - 1),
                            iCount < 10 ? iCount.ToString("00") : iCount.ToString());

                        //Add mode
                        tblINVInvoice tbl = new tblINVInvoice
                        {
                            FKClientId = invoice.FKClientId,
                            MilestoneIds = invoice.MilestoneIds.Substring(0, invoice.MilestoneIds.Length -1),
                            InvoiceCode = invoiceNo,
                            InvoiceDate = invoice.InvoiceDate,
                            InvoiceType = invoice.InvoiceType,
                            Currency = invoice.Currency,
                            Remarks = invoice.Remarks,
                            RoundOff = invoice.RoundOff,
                            TotalAmount = invoice.TotalAmount,
                            IsActive = true,
                            IsDeleted = false,
                            CreBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString()),
                            ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString()),
                            CreDate = DateTime.Now.ToUniversalTime(),
                            ChgDate = DateTime.Now.ToUniversalTime()
                            
                        };
                        db.tblINVInvoices.Add(tbl);
                        db.SaveChanges();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, tbl.PKInvoiceId);
                    } 
                    else {
                        //Edit mode
                        var tbl = db.tblINVInvoices.Where(z => z.PKInvoiceId == invoice.PKInvoiceId).SingleOrDefault();
                        if (tbl != null)
                        {
                            tbl.FKClientId = invoice.FKClientId;
                            tbl.MilestoneIds = invoice.MilestoneIds.Substring(0, invoice.MilestoneIds.Length - 1);
                            tbl.InvoiceDate = invoice.InvoiceDate;
                            tbl.Currency = invoice.Currency;
                            tbl.Remarks = invoice.Remarks;
                            tbl.RoundOff = invoice.RoundOff;
                            tbl.TotalAmount = invoice.TotalAmount;
                            tbl.ChgDate = DateTime.Now.ToUniversalTime();
                            tbl.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                        }

                        db.SaveChanges();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgUpdate, tbl.PKInvoiceId);
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
        public ApiResponse SaveInvoiceTax(tblINVInvoiceTax tax)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (tax.PKInvoiceTaxId == 0)
                    {
                        //Add mode
                        tblINVInvoiceTax tbl = new tblINVInvoiceTax
                        {
                            FKInvoiceId = tax.FKInvoiceId,
                            FKTaxId = tax.FKTaxId,
                            TaxPercentage = tax.TaxPercentage,
                            Amount = tax.Amount,
                            IsActive = true,
                            IsDeleted = false,
                            CreBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString()),
                            ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString()),
                            CreDate = DateTime.Now.ToUniversalTime(),
                            ChgDate = DateTime.Now.ToUniversalTime()
                        };
                        db.tblINVInvoiceTaxes.Add(tbl);
                        db.SaveChanges();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, true);
                    } 
                    else {
                        //Edit mode
                        var tbl = db.tblINVInvoiceTaxes.Where(z => z.PKInvoiceTaxId == tax.PKInvoiceTaxId).SingleOrDefault();
                        if (tbl != null) 
                        {
                            tbl.FKInvoiceId = tax.FKInvoiceId;
                            tbl.FKTaxId = tax.FKTaxId;
                            tbl.TaxPercentage = tax.TaxPercentage;
                            tbl.Amount = tax.Amount;
                            tbl.ChgDate = DateTime.Now.ToUniversalTime();
                            tbl.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                        }

                        db.SaveChanges();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgUpdate, true);
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
        public ApiResponse RetrieveInvoiceList()
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
                    string code = nvc["code"];
                    string currency = nvc["currency"];

                    int iDisplayLength = Convert.ToInt32(nvc["count"]);
                    int iDisplayStart = (page - 1) * iDisplayLength;

                    List<tblINVInvoice> list = null;
                    
                    try
                    {
                        list = clientId > 0 ? db.tblINVInvoices.Include("tblINVInvoiceTaxes").Where(z => z.FKClientId == clientId && z.IsDeleted == false).ToList()
                                            : db.tblINVInvoices.Include("tblINVInvoiceTaxes").Where(z => z.IsDeleted == false).ToList();
                        foreach (var l in list) {
                            foreach(var i in l.tblINVInvoiceTaxes) {
                                i.tblINVTaxMaster = db.tblINVTaxMasters.Where(z => z.PKTaxId == i.FKTaxId).SingleOrDefault();
                            }
                            l.tblINVPayments = db.tblINVPayments.Where(z => z.FKInvoiceId == l.PKInvoiceId && z.IsActive == true && z.IsDeleted == false).ToList();
                        }

                        // FILTERING DATA ON BASIS OF CODE
                        if (!string.IsNullOrEmpty(code) && code != "undefined") {
                            iDisplayStart = 0;
                            list = list.Where(z => z.InvoiceCode.ToLower().Contains(code.ToLower())).ToList();
                        }

                        // FILTERING DATA ON BASIS OF CURRENCY
                        if (!string.IsNullOrEmpty(currency) && currency != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.Currency.ToLower().Contains(currency.ToLower())).ToList();
                        }
                        
                        // SORTING DATA
                        list = DoSorting(list, orderBy.Trim());

                        //CONVERT RETURNED DATETIME TO LOCAL TIMEZONE
                        list = list.Select(i =>
                        {
                            i.CreDate = Convert.ToDateTime(i.CreDate).AddMinutes(-1 * timezone);
                            i.ChgDate = Convert.ToDateTime(i.ChgDate).AddMinutes(-1 * timezone);
                            return i;
                        }).Skip(iDisplayStart).Take(iDisplayLength).ToList();
                        
                        var resultData = new
                        {
                            total = list.Count(),
                            result = list
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
        public ApiResponse GetInvoice()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                using (db)
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int invoiceId = Convert.ToInt32(nvc["invoiceId"]);
                    
                    try
                    {
                        tblINVInvoice line = db.tblINVInvoices.Include("tblINVInvoiceTaxes").Where(z => z.PKInvoiceId == invoiceId).SingleOrDefault();
                        line.tblINVClient = db.tblINVClients.Where(z => z.PKClientId == line.FKClientId).SingleOrDefault();
                        line.tblINVInvoiceTaxes = line.tblINVInvoiceTaxes.Where(z => z.IsDeleted == false && z.IsActive == true).ToList();

                        foreach (var i in line.tblINVInvoiceTaxes) {
                           i.tblINVTaxMaster = db.tblINVTaxMasters.Where(z => z.PKTaxId == i.FKTaxId).SingleOrDefault();
                        }

                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", line);
                    }
                    catch (Exception ex) {
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
        public ApiResponse GetClientCountry()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                using (db)
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int clientId = Convert.ToInt32(nvc["clientId"]);

                    try
                    {
                        tblCountry country = null;
                        tblINVClient line = db.tblINVClients.Where(z => z.PKClientId == clientId).SingleOrDefault();
                        if (line.CountryId != null || line.CountryId != 0)
                        {
                            country = db.tblCountries.Include("tblStates").Where(z => z.CountryId == line.CountryId).SingleOrDefault();
                        }
                        else {
                            country = null;
                        }

                        if (line.StateId != null || line.StateId != 0)
                        {
                            country.tblStates = country.tblStates.Where(z => z.StateId == line.StateId).ToList();
                        }
                        else {
                            country.tblStates = null;
                        }

                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", country);
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
        public ApiResponse GetClient()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                using (db)
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int clientId = Convert.ToInt32(nvc["clientId"]);

                    try {
                        var client = db.tblINVClients.Where(z => z.PKClientId == clientId).SingleOrDefault();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", client);
                    } 
                    catch (Exception ex) {
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 0, generalMessages.msgError, null);
                    }
                }
            }
            else {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }
            return apiResponse;
        }

        [HttpGet]
        public ApiResponse CopyInvoice()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int invoiceId = Convert.ToInt32(nvc["invoiceId"]);

                    var invoice = db.tblINVInvoices.Where(z => z.PKInvoiceId == invoiceId).SingleOrDefault();
                    if (invoice != null)
                    {
                        //generate invoice code
                        string invoiceNo = string.Empty;
                        int month = invoice.InvoiceDate.Month;
                        int year = invoice.InvoiceDate.Year;
                        string yr = (year - 1).ToString().Substring(2);
                        int iCount = db.tblINVInvoices.Count() + 1;

                        invoiceNo = string.Format("{0}{1}{2}{3}",
                            invoice.InvoiceType,
                            (char)('A' + Convert.ToInt32(yr)),
                            (char)('A' + month - 1),
                            iCount < 10 ? iCount.ToString("00") : iCount.ToString());

                        //create invoice
                        tblINVInvoice tbl = new tblINVInvoice
                        {
                            FKClientId = invoice.FKClientId,
                            MilestoneIds = invoice.MilestoneIds,
                            InvoiceCode = invoiceNo,
                            InvoiceDate = invoice.InvoiceDate,
                            InvoiceType = invoice.InvoiceType,
                            Currency = invoice.Currency,
                            Remarks = invoice.Remarks,
                            RoundOff = invoice.RoundOff,
                            TotalAmount = invoice.TotalAmount,
                            IsActive = invoice.IsActive,
                            IsDeleted = invoice.IsDeleted,
                            CreBy = invoice.CreBy,
                            ChgBy = invoice.ChgBy,
                            CreDate = invoice.CreDate,
                            ChgDate = invoice.ChgDate
                        };
                        db.tblINVInvoices.Add(tbl);
                        db.SaveChanges();

                        //create invoice taxes
                        var list = db.tblINVInvoiceTaxes.Where(z => z.FKInvoiceId == invoiceId && z.IsDeleted == false).ToList();
                        foreach(var l in list)
                        {
                            tblINVInvoiceTax tax = new tblINVInvoiceTax {
                                FKInvoiceId = tbl.PKInvoiceId,
                                FKTaxId = l.FKTaxId,
                                TaxPercentage = l.TaxPercentage,
                                Amount = l.Amount,
                                IsActive = l.IsActive,
                                IsDeleted = l.IsDeleted,
                                CreBy = l.CreBy,
                                ChgBy = l.ChgBy,
                                CreDate = l.CreDate,
                                ChgDate = l.ChgDate
                            };
                            db.tblINVInvoiceTaxes.Add(tax);
                            db.SaveChanges();
                        }

                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, tbl.PKInvoiceId);
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
        /// return sorted list based on passed column
        /// </summary>
        public List<tblINVInvoice> DoSorting(IEnumerable<tblINVInvoice> list, string orderBy)
        {
            try
            {
                if (orderBy == "InvoiceDate")
                {
                    list = list.OrderBy(z => z.InvoiceDate).ToList();
                }
                else if (orderBy == "-InvoiceDate")
                {
                    list = list.OrderByDescending(z => z.InvoiceDate).ToList();
                }
                if (orderBy == "Currency")
                {
                    list = list.OrderBy(z => z.Currency).ToList();
                }
                else if (orderBy == "-Currency")
                {
                    list = list.OrderByDescending(z => z.Currency).ToList();
                }
                if (orderBy == "TotalAmount")
                {
                    list = list.OrderBy(z => z.TotalAmount).ToList();
                }
                else if (orderBy == "-TotalAmount")
                {
                    list = list.OrderByDescending(z => z.TotalAmount).ToList();
                }
                
                return list.ToList<tblINVInvoice>();
            }
            catch
            {
                return null;
            }
        }
    }
}
