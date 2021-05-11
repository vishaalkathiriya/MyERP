using System;
using System.Linq;
using System.Web;
using ERP.Models;
using System.Data;
using ERP.Utilities;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf.draw;
using System.IO;
using iTextSharp.text.xml;
using System.Runtime.InteropServices;
using System.Configuration;


namespace ERP.Handler
{

    public class HRDQuarterlyManagementMeetingPDFFile : IHttpHandler
    {
        ERPContext db = new ERPContext();

        public void  ProcessRequest(HttpContext context)
        {

                int id = Convert.ToInt32(context.Request.QueryString["SrNo"].ToString());
                var test_data = db.tblHRDQuarterlyManagementMeetings.Where(z => z.SrNo == id).SingleOrDefault();
                DateTime temp_date = (DateTime)test_data.DateOfMeeting;

                String table = "<html><body style='margin-left: 155px; margin-right: 155px;'><div style='text-align: center;'>";
                       table += "<b><h1><div style='color: #0B0B61;'> Quarterly Meeting Management</div><h1></b> </div>";
                       table += "<table>";
                       table += "<tr><td style='font-size: 15px;'> <b>" + test_data.Title + "</b></td></tr>";
                       table += "<tr><td style='font-size: 9px;'>" + String.Format("{0:dd-MMM-yyy}", temp_date) + "</td></tr>";
                       table += "<tr><td style='font-size: 12px;'>list of participant: ";
                    
                        //table += "<td><table>";
                       //table += "<td>1</td><td>sdfsdf</td>";
                      
                       int i = 1;
                       string[] words = test_data.ListOfParticipants.Split(',');
                       foreach (string word in words)
                       {
                           table += "<span style='font-size: 9px;'>" + i + "." + word +"  </span>";
                           i++;
                       }
                       table += "</td></tr></table>";


                       if (!string.IsNullOrEmpty(test_data.AgendaOfTraining))
                       {

                           table += "<br/><br/><h2 style='text-align: center;color: darkgray;font-size: 15px;'><b><u>Agenda of Training</u></b></h2>";
                           table += "<span style='text-align:justify;text-justify:inter-word; font-size: 9px;color: darkgray;'>" + test_data.AgendaOfTraining + "</span>";
                       }

                       if (!string.IsNullOrEmpty(test_data.DecisionTakenDuringMeeting))
                       {
                           table += "<br/><br/><h2 style='text-align: center;'><b style=' color: darkgray;font-size: 15px;'><u>Decision Taken</u></b></h2>";
                           table += "<p style='text-align:justify;text-justify:inter-word; font-size: 9px;'>" + test_data.DecisionTakenDuringMeeting + "</p>";
                       }
                       table += "</body></html>";

                String fileName = string.Format("{0}{1}", System.DateTime.Now.ToString("ddMMyyhhmmssfff"), ".pdf");
                HttpContext.Current.Response.ContentType = "application/pdf";
                HttpContext.Current.Response.Write(@"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.0 Transitional//EN"">");
                HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
                HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);

                iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document(PageSize.A4, 30f, 30f, 30f, 30);
                iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(pdfDoc, HttpContext.Current.Response.OutputStream);


                // // USE FOR PDF OPEN IN NEW TAB
                  // string mainPath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("../" + ConfigurationManager.AppSettings["TempUploadPath"].ToString()), fileName);
                 //  iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(pdfDoc, new FileStream(mainPath, FileMode.Create));
                    
                pdfDoc.Open();
                // HTMLWorker hw = new HTMLWorker(pdfDoc);
               //  writer.PageEvent = new Footer();
                iTextSharp.text.html.simpleparser.HTMLWorker hw = new iTextSharp.text.html.simpleparser.HTMLWorker(pdfDoc);
                hw.Parse(new StringReader(table));
                pdfDoc.Close();
               // HttpContext.Current.Response.Write(fileName);
               
        }


        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }

    
//    public partial class Footer : PdfPageEventHelper
//{
//        public override void OnEndPage(iTextSharp.text.pdf.PdfWriter writer,iTextSharp.text.Document doc)
//        {
            
//            Paragraph footer= new Paragraph("Page No.", FontFactory.GetFont(FontFactory.TIMES, 10, iTextSharp.text.Font.NORMAL));
//            footer.Alignment = Element.ALIGN_RIGHT;
//            PdfPTable footerTbl = new PdfPTable(1);
//            footerTbl.TotalWidth = 300;
//            footerTbl.HorizontalAlignment = Element.ALIGN_CENTER;
//            PdfPCell cell = new PdfPCell(footer);
//            cell.Border = 0;
//            cell.PaddingLeft = 10;
//            footerTbl.AddCell(cell);
//            footerTbl.WriteSelectedRows(0, -1, 415, 30, writer.DirectContent);
//        }
//}



}