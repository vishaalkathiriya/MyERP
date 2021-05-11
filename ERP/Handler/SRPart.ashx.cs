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
    /// Summary description for SRPart
    /// </summary>
    public class SRPart : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            int timezone = Convert.ToInt32(context.Request.QueryString["timezone"].ToString());
            ERPContext db = new ERPContext();
            var DesGrp = db.tblSRParts.Select(a => new { a.PartName, a.Remarks, ChangedOn = a.ChgDate }).ToList().OrderBy(a => a.PartName).ToList();


            DataTable dt = ERPUtilities.ConvertToDataTable(DesGrp);
            ERPUtilities.ExportExcel(context, timezone, dt, "Part List", "Part List", "SRPartList");
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