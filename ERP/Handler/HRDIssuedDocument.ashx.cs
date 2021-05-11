using ERP.Models;
using ERP.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ERP.Handler
{
    /// <summary>
    /// Summary description for HRDIssuedDocument
    /// </summary>
    public class HRDIssuedDocument : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                int timezone = Convert.ToInt32(context.Request.QueryString["timezone"].ToString());
                string startDate = context.Request.QueryString["startDate"].ToString();
                string endDate = context.Request.QueryString["endDate"].ToString();
                ERPContext db = new ERPContext();

                var list = (from sc in db.tblHRDIssuedDocuments
                              join c in db.tblDocuments
                              on sc.DocumentTypeId equals c.Id
                              select new { sc.ECode, sc.FullName, sc.DepartmentName, c.Documents,sc.IssuedOn, ChangedOn = sc.ChgDate }).ToList();

                //top filter
                if (!string.IsNullOrEmpty(startDate))
                {
                    string[] fdate = startDate.Split('/');
                    DateTime fromDate = new DateTime(Convert.ToInt32(fdate[2]), Convert.ToInt32(fdate[1]), Convert.ToInt32(fdate[0]));
                    if (startDate == endDate)
                    {
                        list = list.Where(z => z.IssuedOn.Date == fromDate.Date).ToList();
                    }
                    else
                    {//date range
                        string[] tdate = endDate.Split('/');
                        DateTime toDate = new DateTime(Convert.ToInt32(tdate[2]), Convert.ToInt32(tdate[1]), Convert.ToInt32(tdate[0]));
                        list = list.Where(z => z.IssuedOn.Date >= fromDate.Date && z.IssuedOn.Date <= toDate.Date).ToList();
                    }
                }

                DataTable dt = ERPUtilities.ConvertToDataTable(list);
                ERPUtilities.ExportExcel(context, timezone, dt, "Issued Document List", "Issued Document List", "IssuedDocumentList");
                context = null;
            }
            catch { 
                
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