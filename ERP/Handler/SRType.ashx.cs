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
    /// Summary description for SRType
    /// </summary>
    public class SRType : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            int timezone = Convert.ToInt32(context.Request.QueryString["timezone"].ToString());
            ERPContext db = new ERPContext();
            var DesGrp = db.tblSRTypes.Select(a => new { a.TypeName, a.TypePrefix, a.Remarks, ChangedOn = a.ChgDate }).ToList().OrderBy(a => a.TypeName).ToList();


            DataTable dt = ERPUtilities.ConvertToDataTable(DesGrp);
            ERPUtilities.ExportExcel(context, timezone, dt, "MachineType List", "MachineType List", "SRTypeList");
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