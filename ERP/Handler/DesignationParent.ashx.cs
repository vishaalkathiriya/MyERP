using System;
using System.Linq;
using System.Web;
using ERP.Models;
using System.Data;
using ERP.Utilities;
namespace ERP.Handler
{
    /// <summary>
    /// Summary description for DesignationParent
    /// </summary>
    public class DesignationParent : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            int timezone = Convert.ToInt32(context.Request.QueryString["timezone"].ToString());
            ERPContext db = new ERPContext();
            var DesPrnt = db.tblDesignationParents.Select(a => new { a.DesignationParent, a.IsActive, ChangedOn = a.ChgDate }).ToList().OrderBy(a => a.DesignationParent).ToList();


            DataTable dt = ERPUtilities.ConvertToDataTable(DesPrnt);
            ERPUtilities.ExportExcel(context, timezone, dt, "DesignationParent List", "DesignationParent List", "DesignationParentList");
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