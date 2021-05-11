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
    public class ReportController : ApiController
    {
        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Report";

        public ReportController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }

        /// <summary>
        /// Retireive list of invoice milestones
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiResponse RetrieveInvoiceMilestones()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    string milestoneIds = nvc["milestoneIds"].ToString();

                    List<tblINVMilestone> mList = new List<tblINVMilestone>();
                    string[] parts = milestoneIds.Split(',');

                    foreach (var p in parts) {
                        mList.Add(db.tblINVMilestones.AsEnumerable().Where(z => z.PKMilestoneId == Convert.ToInt32(p)).SingleOrDefault());
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

        /// <summary>
        /// GET api/Conversation
        /// Returns list of clients available
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
                        .Where(z => z.IsDeleted == false || z.IsActive == true)
                        .Select(z => new SelectItemModel
                        {
                            Id = z.PKClientId,
                            Label = string.Format("{0} - {1}", z.CompanyName, z.ContactPerson)
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
        /// Returns list of invoices
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiResponse RetrieveInvoices()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                using (db)
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int page = Convert.ToInt32(nvc["page"]);
                    int timezone = Convert.ToInt32(nvc["timezone"]);
                    
                    string filterInvoiceType = nvc["filterInvoiceType"];
                    string filterCurrency = nvc["filterCurrency"];
                    string filterStatus = nvc["filterStatus"];
                    string startDate = nvc["startDate"];
                    string endDate = nvc["endDate"];
                    int filterClientId = Convert.ToInt32(nvc["filterClientId"]);
                    string orderBy = nvc["orderby"];
                    string code = nvc["code"];
                    string currency = nvc["currency"];

                    int iDisplayLength = Convert.ToInt32(nvc["count"]);
                    int iDisplayStart = (page - 1) * iDisplayLength;

                    List<tblINVInvoice> list = null;

                    try
                    {
                        list = filterClientId > 0 ? db.tblINVInvoices.Include("tblINVInvoiceTaxes").Where(z => z.FKClientId == filterClientId && z.IsDeleted == false && z.IsActive == true).ToList()
                                            : db.tblINVInvoices.Include("tblINVInvoiceTaxes").Where(z => z.IsDeleted == false && z.IsActive == true).ToList();
                        
                        foreach (var l in list) {
                            l.tblINVInvoiceTaxes = l.tblINVInvoiceTaxes.Where(z => z.IsActive == true && z.IsDeleted == false).ToList();
                            foreach (var i in l.tblINVInvoiceTaxes) {
                                i.tblINVTaxMaster = db.tblINVTaxMasters.Where(z => z.PKTaxId == i.FKTaxId).SingleOrDefault();
                            }

                            l.tblINVPayments = db.tblINVPayments.Where(z => z.FKInvoiceId == l.PKInvoiceId && z.IsActive == true && z.IsDeleted == false).ToList();
                        }

                        //top filter
                        if (!string.IsNullOrEmpty(startDate) && startDate != "undefined")
                        {
                            string[] fdate = startDate.Split('/');
                            DateTime fromDate = new DateTime(Convert.ToInt32(fdate[2]), Convert.ToInt32(fdate[1]), Convert.ToInt32(fdate[0]));
                            if (startDate == endDate)
                            {
                                list = list.Where(z => z.InvoiceDate.Date == fromDate.Date).ToList();
                            }
                            else
                            {//date range
                                string[] tdate = endDate.Split('/');
                                DateTime toDate = new DateTime(Convert.ToInt32(tdate[2]), Convert.ToInt32(tdate[1]), Convert.ToInt32(tdate[0]));
                                list = list.Where(z => z.InvoiceDate.Date >= fromDate.Date && z.InvoiceDate.Date <= toDate.Date).ToList();
                            }
                        }


                        // FILTERING INVOICE TYPE
                        if (!string.IsNullOrEmpty(filterInvoiceType) && filterInvoiceType != "undefined")
                        {
                            list = list.Where(z => z.InvoiceType == filterInvoiceType).ToList();
                        }

                        // FILTERING FOR CURRENCY
                        if (!string.IsNullOrEmpty(filterCurrency) && filterCurrency != "0")
                        {
                            list = list.Where(z => z.Currency == filterCurrency).ToList();
                        }
                        
                        // FILTERING DATA ON BASIS OF CODE
                        if (!string.IsNullOrEmpty(code) && code != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.InvoiceCode.ToLower().Contains(code.ToLower())).ToList();
                        }

                        // FILTERING DATA ON BASIS OF CURRENCY
                        if (!string.IsNullOrEmpty(currency) && currency != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.Currency.ToLower().Contains(currency.ToLower())).ToList();
                        }

                        // FILTERING FOR STATUS - PENDING OR CLEAR
                        if (!string.IsNullOrEmpty(filterStatus) && filterStatus != "undefined") {
                            List<tblINVInvoice> newlist = new List<tblINVInvoice>();

                            if (filterStatus == "Pending") {
                                foreach (var l in list) {
                                    if (GetInvoiceReceivedAmount(l.PKInvoiceId) <= l.TotalAmount) {
                                        newlist.Add(l);
                                    }
                                }
                            } else if (filterStatus == "Clear") {
                                foreach (var l in list) {
                                    if (GetInvoiceReceivedAmount(l.PKInvoiceId) > l.TotalAmount) {
                                        newlist.Add(l);
                                    }
                                }
                            }

                            list = newlist;
                        }

                        // SORTING DATA
                        list = DoSorting(list, orderBy.Trim());

                        //CONVERT RETURNED DATETIME TO LOCAL TIMEZONE
                        //list = list.Select(i =>
                        //{
                        //    i.CreDate = Convert.ToDateTime(i.CreDate).AddMinutes(-1 * timezone);
                        //    i.ChgDate = Convert.ToDateTime(i.ChgDate).AddMinutes(-1 * timezone);
                        //    return i;
                        //}).Skip(iDisplayStart).Take(iDisplayLength).ToList();

                        var resultData = new
                        {
                            total = list.Count(),
                            result = list.Skip(iDisplayStart).Take(iDisplayLength).ToList()
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

        public decimal GetInvoiceReceivedAmount(int invoiceId) 
        {
            decimal sum;
            try
            {
                var received = db.tblINVPayments.Where(z => z.FKInvoiceId == invoiceId && z.IsActive == true && z.IsDeleted == false).Sum(z => z.OnHandReceivedAmount);
                var charges = db.tblINVPayments.Where(z => z.FKInvoiceId == invoiceId && z.IsActive == true && z.IsDeleted == false).Sum(z => z.OtherCharges);
                sum = received + charges;
            }
            catch
            {
                sum = 0;
            }
            return sum;
        }
        /// <summary>
        /// return sorted list based on passed column
        /// </summary>
        public List<tblINVInvoice> DoSorting(IEnumerable<tblINVInvoice> list, string orderBy)
        {
            try
            {
                if (orderBy == "InvoiceCode")
                {
                    list = list.OrderBy(z => z.InvoiceCode).ToList();
                }
                else if (orderBy == "-InvoiceCode")
                {
                    list = list.OrderByDescending(z => z.InvoiceCode).ToList();
                }
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
                if (orderBy == "ChgDate")
                {
                    list = list.OrderBy(z => z.TotalAmount).ToList();
                }
                else if (orderBy == "-ChgDate")
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