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
    /// Summary description for Location
    /// </summary>
    public class Location : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            int timezone = Convert.ToInt32(context.Request.QueryString["timezone"].ToString());
            ERPContext db = new ERPContext();
            var cat = db.tblLocations.Select(a => new { a.LocationName, a.IsActive, ChangedOn = a.ChgDate }).ToList().OrderBy(a => a.LocationName).ToList();

            DataTable dt = ERPUtilities.ConvertToDataTable(cat);

            ERPUtilities.ExportExcel(context, timezone, dt, "Location List", "Location List", "LocationList");
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