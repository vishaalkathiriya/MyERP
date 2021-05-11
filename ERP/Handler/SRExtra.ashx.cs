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
    /// Summary description for SRExtra
    /// </summary>
    public class SRExtra : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            int timezone = Convert.ToInt32(context.Request.QueryString["timezone"].ToString());
            ERPContext db = new ERPContext();

            var des = (from d in db.tblSRExtras
                       select new { Type = d.Type.Equals("S") ? "Scrap" : "Other", d.MachineNo, d.ExtraDate, d.Remark, d.ChgDate }).ToList();

            DataTable dt = ERPUtilities.ConvertToDataTable(des);
            ERPUtilities.ExportExcel(context, timezone, dt, "Extra-Entry List", "Extra-Entry List", "SRExtraList");
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