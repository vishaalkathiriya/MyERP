using ERP.Models;
using ERP.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace ERP.Areas.Invoice.WebApis
{
    public class PaymentController : ApiController
    {
        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Invoice Payment";

        public PaymentController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }

        [HttpGet]
        public ApiResponse RetrievePayments()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                using (db)
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int invoiceId = Convert.ToInt32(nvc["invoiceId"]);
                    int timezone = Convert.ToInt32(nvc["timezone"]);
                    IEnumerable<tblINVPayment> payments = null;
                    try
                    {
                        payments = db.tblINVPayments.Where(z => z.FKInvoiceId == invoiceId && z.IsDeleted == false).ToList();
                        payments = payments.Select(i =>
                        {
                            i.CreDate = Convert.ToDateTime(i.CreDate).AddMinutes(-1 * timezone);
                            i.ChgDate = Convert.ToDateTime(i.ChgDate).AddMinutes(-1 * timezone);
                            return i;
                        });
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", payments);
                    }
                    catch (Exception ex)
                    {
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 0, generalMessages.msgError, null);
                    }
                    finally
                    {
                        payments = null;
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
        public ApiResponse DeletePayment()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int paymentId = Convert.ToInt32(nvc["paymentId"].ToString());

                    var line = db.tblINVPayments.Where(z => z.PKPaymentId == paymentId).SingleOrDefault();
                    if (line != null)
                    {
                        line.IsDeleted = true;
                        line.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                        line.ChgDate = DateTime.Now;
                        db.SaveChanges();

                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgDelete, null);
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
        public ApiResponse RetrievePaymentReceivedInvoiceList()
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

                    List<PaymentInvoiceListViewModel> priList = new List<PaymentInvoiceListViewModel>();
                    try 
                    {
                        SqlParameter paramClientId = new SqlParameter();
                        paramClientId = new SqlParameter("@ClinetId", clientId);
                        priList = db.Database.SqlQuery<PaymentInvoiceListViewModel>("usp_getPaymentReceivedInvoiceList @ClinetId ", paramClientId).ToList();


                        // FILTERING DATA ON BASIS OF CODE
                        if (!string.IsNullOrEmpty(code) && code != "undefined")
                        {
                            iDisplayStart = 0;
                            priList = priList.Where(z => z.InvoiceCode.ToLower().Contains(code.ToLower())).ToList();
                        }

                        // FILTERING DATA ON BASIS OF CURRENCY
                        if (!string.IsNullOrEmpty(currency) && currency != "undefined")
                        {
                            iDisplayStart = 0;
                            priList = priList.Where(z => z.Currency.ToLower().Contains(currency.ToLower())).ToList();
                        }

                        // SORTING DATA
                        priList = DoSorting(priList, orderBy.Trim());

                        var resultData = new
                        {
                            total = priList.Count(),
                            result = priList.Skip(iDisplayStart).Take(iDisplayLength).ToList()
                        };
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", resultData);
                    }
                    catch (Exception ex) {
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 0, generalMessages.msgError, null);
                    }
                    finally {
                        priList = null;
                    }
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }
            return apiResponse;
        }

        [HttpPost]
        public ApiResponse CreateUpdateInvoicePayment(tblINVPayment payment)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int timezone = Convert.ToInt32(nvc["timezone"].ToString());
                    
                    if (payment.PKPaymentId == 0) // ADD
                    {
                        tblINVPayment tbl = new tblINVPayment
                        {
                            FKInvoiceId = payment.FKInvoiceId,
                            PaymentReceivedDate = payment.PaymentReceivedDate,
                            OnHandReceivedAmount = payment.OnHandReceivedAmount,
                            OtherCharges = payment.OtherCharges,
                            ExchangeRateINR = payment.ExchangeRateINR,
                            Remarks = payment.Remarks,
                            IsActive = true,
                            IsDeleted = false,
                            CreDate = DateTime.Now.ToUniversalTime(),
                            CreBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString()),
                            ChgDate = DateTime.Now.ToUniversalTime(),
                            ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString())
                        };
                        db.tblINVPayments.Add(tbl);
                        db.SaveChanges();

                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, tbl);
                    }
                    else // EDIT
                    {
                        var tbl = db.tblINVPayments.Where(z => z.PKPaymentId == payment.PKPaymentId).SingleOrDefault();
                        if (tbl != null) 
                        {
                            tbl.FKInvoiceId = payment.FKInvoiceId;
                            tbl.PaymentReceivedDate = payment.PaymentReceivedDate;
                            tbl.OnHandReceivedAmount = payment.OnHandReceivedAmount;
                            tbl.OtherCharges = payment.OtherCharges;
                            tbl.ExchangeRateINR = payment.ExchangeRateINR;
                            tbl.Remarks = payment.Remarks;
                            tbl.ChgDate = DateTime.Now.ToUniversalTime();
                            tbl.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                        }
                        db.SaveChanges();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgUpdate, tbl);
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
        public ApiResponse ChangePaymentStatus()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int paymentId = Convert.ToInt32(nvc["paymentId"].ToString());
                    var payment = db.tblINVPayments.Where(z => z.PKPaymentId == paymentId).SingleOrDefault();
                    if (payment != null) {
                        payment.IsActive = !payment.IsActive;
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
        /// <summary>
        /// return sorted list based on passed column
        /// </summary>
        public List<PaymentInvoiceListViewModel> DoSorting(IEnumerable<PaymentInvoiceListViewModel> list, string orderBy)
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
                if (orderBy == "TotalInvoiceAmount")
                {
                    list = list.OrderBy(z => z.TotalInvoiceAmount).ToList();
                }
                else if (orderBy == "-TotalInvoiceAmount")
                {
                    list = list.OrderByDescending(z => z.TotalInvoiceAmount).ToList();
                }
                if (orderBy == "PaymentReceivedAmount")
                {
                    list = list.OrderBy(z => z.PaymentReceivedAmount).ToList();
                }
                else if (orderBy == "-PaymentReceivedAmount")
                {
                    list = list.OrderByDescending(z => z.PaymentReceivedAmount).ToList();
                }

                return list.ToList<PaymentInvoiceListViewModel>();
            }
            catch
            {
                return null;
            }
        }
    }
}
