using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ERP.Models;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using System.Data.SqlClient;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using ERP.Utilities;

namespace ERP.Handler
{
    /// <summary>
    /// Summary description for Document
    /// </summary>
    public class Document : IHttpHandler
    {
        int timezone;
        public void ProcessRequest(HttpContext context)
        {
            timezone = Convert.ToInt32(context.Request.QueryString["timezone"].ToString());
            ERPContext db = new ERPContext();
            var doc = db.tblDocuments.Select(a => new { a.Documents, a.IsActive, a.ChgDate }).ToList().OrderBy(a => a.Documents).ToList();

            DataTable dt = ERPUtilities.ConvertToDataTable(doc);
            ERPUtilities.ExportExcel(context, timezone, dt, "Document List", "Document List", "DocumentList");
            context = null;
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