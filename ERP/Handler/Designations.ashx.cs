using System;
using System.Linq;
using System.Web;
using ERP.Models;
using System.Data;
using ERP.Utilities;


namespace ERP.Handler
{
    /// <summary>
    /// Summary description for Designations
    /// </summary>
    public class Designations : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            int timezone = Convert.ToInt32(context.Request.QueryString["timezone"].ToString());
            ERPContext db = new ERPContext();

            var des = (from d in db.tblDesignations
                       join dp in db.tblDesignationParents on d.DesignationParentId equals dp.Id
                       join dg in db.tblDesignationGroups on d.DesignationGroupId equals dg.Id

                       select new { dg.DesignationGroup, dp.DesignationParent, d.Designation, d.IsActive, ChangedOn = d.ChgDate }).ToList();

            DataTable dt = ERPUtilities.ConvertToDataTable(des);
            ERPUtilities.ExportExcel(context, timezone, dt, "Designation List", "Designation List", "DesignationList");
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