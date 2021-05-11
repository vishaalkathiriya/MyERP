using ERP.Models;
using ERP.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace ERP.Handler
{
    /// <summary>
    /// Summary description for InvoiceClient
    /// </summary>
    public class InvoiceClient : IHttpHandler
    {
        ERPContext db = null;

        public InvoiceClient()
        {
            db = new ERPContext();
        }

        public void ProcessRequest(HttpContext context)
        {
            string rtype = context.Request["rtype"].ToString();
            int clientId = Convert.ToInt32(context.Request["clientId"].ToString());
            if (rtype == "pdf")
            {
                var line = db.tblINVClients.Where(z => z.PKClientId == clientId).SingleOrDefault();
                var country = db.tblCountries.Where(z => z.CountryId == line.CountryId).SingleOrDefault();

                string bedate =  line.BusinessStartDate != null ? line.BusinessStartDate.Value.ToString("dd MMM yyyy") : string.Empty,
                       logo = string.Format("<img src='{0}'</img>", HostingEnvironment.MapPath("/Content/images/logo-drc-mail.png")),
                       header = string.Format("<img src='{0}'</img>", HostingEnvironment.MapPath("/Content/images/kyc-header.jpg")),
                       footer = string.Format("<img src='{0}'</img>", HostingEnvironment.MapPath("/Content/images/kyc-footer.jpg")),
                       space = "&nbsp;&nbsp;",
                       telNo = !string.IsNullOrEmpty(line.TelephoneNo) ? string.Format("{0}{1} {2}", space, country.DialCode, line.TelephoneNo) : string.Empty,
                       faxNo = !string.IsNullOrEmpty(line.FaxNo) ? string.Format("{0}{1} {2}", space, country.DialCode, line.FaxNo) : string.Empty,
                       mobileNo = !string.IsNullOrEmpty(line.MobileNo) ? string.Format("{0}{1} {2}", space, country.DialCode, line.MobileNo) : string.Empty;

                String HTML = "<html>" +
                                "<body>" +
                                    "<div class='upper'>" +
                                            "<div>" +
                                               header +
                                            "</div>" +
                                            "<br />" +

                                            "<div style='padding-left:30px;padding-right:30px;'>" +
                                            "<h4><b>Information about the Company</b></h4>" +
                                            "<br />" +
                                            "<table cellpadding='2' cellspacing='3'>" +
                                                "<tr>" +
                                                    "<td class='number'>1</td>" +
                                                    "<td class='label'><strong>&nbsp;Company Name </strong></td>" +
                                                    "<td colspan='3'> " + space + line.CompanyName + "</td>" +
                                                "</tr>" +
                                                "<tr>" +
                                                    "<td class='number'>2</td>" +
                                                    "<td class='label'><strong>&nbsp;Contact Person </strong></td>" +
                                                    "<td colspan='2'>" + space + line.CPrefix + " " + line.ContactPerson + "</td>" +
                                                    "<td style='width:30%'>" + "<strong>&nbsp;Mobile No : </strong>" + mobileNo + "</td>" +
                                                "</tr>" +
                                                "<tr>" +
                                                    "<td class='number'>3</td>" +
                                                    "<td class='label'><strong>&nbsp;Company Address </strong></td>" +
                                                    "<td colspan='3'>" + space + line.CompanyAddress + ", " + line.City + " " + line.PostalCode + "</td>" +
                                                "</tr>" +
                                                "<tr>" +
                                                    "<td class='number'>4</td>" +
                                                    "<td class='label'><strong>&nbsp;Country </strong></td>" +
                                                    "<td colspan='3' >" + space +
                                                        db.tblStates.Where(z => z.StateId == line.StateId).Select(z => z.StateName).SingleOrDefault() +
                                                        ", " +
                                                        country.CountryName +
                                                        "</td>" +
                                                "</tr>" +
                                                "<tr>" +
                                                    "<td class='number'>5</td>" +
                                                    "<td class='label'><strong>&nbsp;Telephone No </strong></td>" +
                                                    "<td colspan='2' >" + telNo + "</td>" +
                                                    "<td style='width:30%'>" + "<strong>&nbsp;Fax No. : </strong>" + faxNo + "</td>" +
                                                "</tr>" +
                                                    "<tr>" +
                                                    "<td class='number'>6</td>" +
                                                        "<td class='label'><strong>&nbsp;Email </strong></td>" +
                                                        "<td colspan='3' >" + space + line.Email.ToLower() + "</td>" +
                                                    "</tr>" +
                                                    "<tr>" +
                                                        "<td class='number'>7</td>" +
                                                        "<td class='label'><strong>&nbsp;Web Site </strong></td>" +
                                                        "<td colspan='3'>" + space + line.Website + "</td>" +
                                                    "</tr>" +
                                                    "<tr>" +
                                                        "<td class='number'>8</td>" +
                                                        "<td class='label'><strong>&nbsp;Nature Of Business </strong></td>" +
                                                        "<td colspan='3'>" + space +
                                                        Enum.GetName(typeof(ERPUtilities.BusinessType), line.BusinessTypeId)
                                                        + "</td>" +
                                                    "</tr>" +
                                                    "<tr>" +
                                                        "<td class='number'>9</td>" +
                                                        "<td class='label'><strong>&nbsp;Business Est. Date </strong></td>" +
                                                        "<td colspan='3'>" + space + bedate + "</td>" +
                                                    "</tr>" +
                                                    "<tr>" +
                                                        "<td class='number'>10</td>" +
                                                        "<td class='label'><strong>&nbsp;Business License No. </strong></td>" +
                                                        "<td colspan='3'>" + space + line.LicenseNo + "</td>" +
                                                    "</tr>" +
                                                    "<tr>" +
                                                        "<td class='number'>11</td>" +
                                                        "<td class='label'><strong>&nbsp;Permanent Income Tax No </strong></td>" +
                                                        "<td colspan='3'>" + space + line.IncomeTaxNo + "</td>" +
                                                    "</tr>" +
                                                    "<tr>" +
                                                        "<td class='number'>12</td>" +
                                                        "<td class='label'><strong>&nbsp;Value Added Tax No. </strong></td>" +
                                                        "<td colspan='3'>" + space + line.VATNo + "</td>" +
                                                    "</tr>" +
                                            "</table>" +
                                            "<br />" +
                                            "<h4><b>Bank Details - Incase return of Payment</b></h4>" +
                                            "<br />" +
                                            "<table cellpadding='2' cellspacing='3'>" +
                                                "<tr>" +
                                                    "<td class='number'>1</td>" +
                                                    "<td class='label'><strong>&nbsp;Name of the Bank </strong></td>" +
                                                    "<td colspan='2'>" + space + line.BankName + "</td>" +
                                                "</tr>" +
                                                "<tr>" +
                                                    "<td class='number'>2</td>" +
                                                    "<td class='label'><strong>&nbsp;Branch Address </strong></td>" +
                                                    "<td colspan='2'>" + space + line.BranchAddress + "</td>" +
                                                "</tr>" +
                                                "<tr>" +
                                                    "<td class='number'>3</td>" +
                                                    "<td class='label'><strong>&nbsp;Type of the Bank </strong></td>" +
                                                    "<td colspan='2'>" + space + line.BankType + "</td>" +
                                                "</tr>" +
                                                "<tr>" +
                                                    "<td class='number'>4</td>" +
                                                    "<td class='label'><strong>&nbsp;Account No. </strong></td>" +
                                                    "<td colspan='2' >" + space + line.AccountNo + "</td>" +
                                                "</tr>" +
                                                "<tr>" +
                                                    "<td class='number'>5</td>" +
                                                    "<td class='label'><strong>&nbsp;IBAN No. </strong></td>" +
                                                    "<td colspan='2'>" + space + line.IBANNumber + "</td>" +
                                                "</tr>" +
                                                "<tr>" +
                                                    "<td class='number'>6</td>" +
                                                    "<td class='label'><strong>&nbsp;Swift Code / BIC Code </strong></td>" +
                                                    "<td colspan='2'>" + space + line.SwiftCode + "</td>" +
                                                "</tr>" +
                                            "</table>" +
                                            "<br />" +
                                            "<h4><b >Client Contact Persons</b></h4>" +
                                            "<br />" +
                                            "<table cellpadding='2' cellspacing='3'>" +
                                                "<tr>" +
                                                    "<td class='number'>#</td>" +
                                                    "<td class='label' style='width:110px;'><strong>&nbsp;Name </strong></td>" +
                                                    "<td class='label' style='width:150px;'><strong>&nbsp;Designation </strong></td>" +
                                                    "<td class='label'><strong>&nbsp;Email </strong></td>" +
                                                    "<td class='label' style='width:100px;'><strong>&nbsp;Mobile </strong></td>" +
                                                    "<td class='label' style='width:200px;'><strong>&nbsp;Identity Card</strong></td>" +
                                                "</tr>" +
                                                GetDirectorListHTML(clientId, space, db) +
                                            "</table>" +
                                            "</div>" +
                                        "</div>" +
                                        "<div style='padding-top:3px;'>" +
                                                footer +
                                        "</div>" +
                                    "</body>" +
                                "</html>" +
                                "<div style='page-break-before:always'>&nbsp;</div>" +
                                "<h4 style='text-align:center;'><b>Attached Documents</b></h4>" +
                                GetDocumentListHTML(clientId, space, line.CompanyName, footer, db);

                String fileName = string.Format("{0}{1}{2}", "KYCExport", System.DateTime.Now.ToString("ddMMyyhhmmssfff"), ".pdf");
                ExportPdfHelper.Export(HTML, fileName, "~/Content/css/pdf.css");
            }
            
        }

        public static string GetDirectorListHTML(int clientId, string space, ERPContext db) 
        {
            try 
	        {	   
                string html = string.Empty;
                var lstDirector = db.tblINVClientPersons.Where(z=>z.FKClientId == clientId && z.IsActive == true).ToList();
                int index = 1;
                foreach(var l in lstDirector) {
                    html += "<tr>" +
                                "<td class='number'>" + index + "</td>" +
                                "<td>" + space + l.FullName + "</td>" +
                                "<td>" + space + l.Designation + "</td>" +
                                "<td>" + space + l.Email.ToLower() + "</td>" +
                                "<td>" + space + l.MobileNo + "</td>" +
                                "<td>" + space + db.tblDocuments.Where(z => z.Id == l.IdentityDocId).Select(z => z.Documents).SingleOrDefault() + ":" + l.IdentityNo + "</td>" +
                            "</tr>";
                    index++;
                }
                return html;
	        }
	        catch (Exception) {
               return string.Empty;
	        }
        }

        public static string GetDocumentListHTML(int clientId, string space, string companyName, string footer, ERPContext db)
        {
            try
            {
                string html = string.Empty;
                var lstDocument = db.tblINVDocuments.Where(z => (z.DocTypeId == 1 || z.DocTypeId == 2) && (z.tblRefId == clientId && z.IsActive == true)).ToList();
                int index = 1;
                foreach (var l in lstDocument)
                {
                    string docname = db.tblDocuments.Where(z=>z.Id == l.DocId).Select(z=>z.Documents).SingleOrDefault();
                    string doctype = l.DocTypeId == 1 ? "Identity Proof" : "Business Reg. Proof";

                    html += "<html>" +
                            "<body>" +
                                "<div class='upper' style='padding-left:30px;padding-right:30px;'>" +
                                        "<h6><strong>" + index + ". " + doctype + " : </strong>" + docname + " (" + companyName + ")</h6>" +
                                        "<br/><div class='img-responsive'>" +
                                        string.Format("<img src='{0}'</img>", Path.Combine(HostingEnvironment.MapPath("/" + ConfigurationManager.AppSettings["invClientUploads"].ToString()), l.DocName)) +
                                        "</div>" +
                                "</div>" +
                            "</body>" +
                            "</html>";

                    if (index != lstDocument.Count()) {
                        html += "<div style='page-break-before:always'>&nbsp;</div>";
                    }

                    index++;
                }
                return html;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}