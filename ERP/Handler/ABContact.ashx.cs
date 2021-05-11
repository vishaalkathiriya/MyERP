using ERP.Models;
using ERP.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Text;
using OfficeOpenXml.Style;

namespace ERP.Handler
{
    /// <summary>
    /// Summary description for ABContact
    /// </summary>
    public class ABContact : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            if (context.Request.QueryString["mode"].ToString() == "Download")
            {
                int timezone = Convert.ToInt32(context.Request.QueryString["timezone"].ToString());
                ERPContext db = new ERPContext();

                DataTable dtContact = CreateDataTableColumn();
                var pck = new OfficeOpenXml.ExcelPackage();
                var ws = pck.Workbook.Worksheets.Add("Contact");
                var contact = db.tblABContacts.ToList();

                for (int i = 0; i < contact.Count; i++)
                {
                    // Generate Data in Excel
                    DataRow dr = dtContact.NewRow();
                    dr["Contact Id"] = contact[i].ContactId;
                    dr["Name"] = contact[i].Name;
                    dr["Phone No"] = contact[i].PhoneNo;
                    dr["Landline No"] = contact[i].LandlineNo;
                    dr["Line 1"] = contact[i].Address1;
                    dr["Line 2"] = contact[i].Address2;
                    dr["City"] = contact[i].City;
                    dr["Area"] = contact[i].Area;
                    dr["Pincode"] = contact[i].Pincode;
                    dr["Note"] = contact[i].Note;
                    dr["Language Id"] = contact[i].LangId;
                    var list = db.tblABGrp_Contact.ToList();
                    list = list.Where(z => z.ContactId == contact[i].ContactId).ToList();
                    dr["Total Group"] = list.Count;
                    dtContact.Rows.Add(dr);

                    // apply font style to perticular column
                    if (contact[i].LangId == 2)
                    {
                        using (var rng = ws.Cells["A" + (i + 2) + ":L" + (i + 2) + ""])
                        {
                            rng.Style.Font.Name = "HARIKRISHNA";
                            rng.Style.Font.Size = 12;

                        }
                    }
                }

                ws.Cells["A1:L1"].Style.Font.Bold = true; // apply style to first row of excel  
                ws.Cells["A1"].LoadFromDataTable(dtContact, true); // starting column to generate data
                // convert current time to milliseconds
                var milsec = DateTime.UtcNow.Ticks;
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                HttpContext.Current.Response.AddHeader("content-disposition", "attachment;  filename=ContactList-" + milsec + ".xlsx");
                HttpContext.Current.Response.BinaryWrite(pck.GetAsByteArray());
            }
       
        }
        public DataTable CreateDataTableColumn()
        {   // Heading of Excel Data
            DataTable dt = new DataTable();
            dt.Columns.Add("Contact Id", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Phone No", typeof(string));
            dt.Columns.Add("Landline No", typeof(string));
            dt.Columns.Add("Line 1", typeof(string));
            dt.Columns.Add("Line 2", typeof(string));
            dt.Columns.Add("City", typeof(string));
            dt.Columns.Add("Area", typeof(string));
            dt.Columns.Add("Pincode", typeof(string));
            dt.Columns.Add("Note", typeof(string));
            dt.Columns.Add("Language Id", typeof(string));
            dt.Columns.Add("Total Group");
            return dt;
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