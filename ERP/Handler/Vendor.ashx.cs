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
    /// Summary description for Vendor
    /// </summary>
    public class Vendor : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            int timezone = Convert.ToInt32(context.Request.QueryString["timezone"].ToString());
            ERPContext db = new ERPContext();
            var cat = db.tblVendors.Select(a =>
                new
                {
                    a.VendorName,
                    a.CompanyName,
                    a.Email,
                    a.Website,
                    a.Mobile,
                    a.PhoneNo,
                    a.Services,
                    a.Rating,
                    a.HouseNo,
                    a.Location,
                    a.Area,
                    a.PostalCode,
                    a.City,
                    a.State,
                    a.Country,
                    ChangedOn = a.ChgDate
                }).ToList().OrderBy(a => a.VendorName).ToList();

            DataTable dt = ERPUtilities.ConvertToDataTable(cat);

            ERPUtilities.ExportExcel(context, timezone, dt, "Vendor List", "Vendor List", "VendorList");
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