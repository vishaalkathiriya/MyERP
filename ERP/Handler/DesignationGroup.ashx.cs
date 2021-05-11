using System;
using System.Linq;
using System.Web;
using ERP.Models;
using System.Data;
using ERP.Utilities;

namespace ERP.Handler
{
    /// <summary>
    /// Summary description for DesignationGroup
    /// </summary>
    public class DesignationGroup : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            int timezone = Convert.ToInt32(context.Request.QueryString["timezone"].ToString());
            ERPContext db = new ERPContext();
            var DesGrp = db.tblDesignationGroups.Select(a => new { a.DesignationGroup, a.IsActive, ChangedOn = a.ChgDate }).ToList().OrderBy(a => a.DesignationGroup).ToList();


            DataTable dt = ERPUtilities.ConvertToDataTable(DesGrp);
            ERPUtilities.ExportExcel(context, timezone, dt, "DesignationGroup List", "DesignationGroup List", "DesignationGroupList");
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