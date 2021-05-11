using System;
using System.Linq;
using System.Web;
using ERP.Models;
using System.Data;
using ERP.Utilities;

namespace ERP.Handler
{
    /// <summary>
    /// Summary description for Role
    /// </summary>
    public class Role : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            int timezone = Convert.ToInt32(context.Request.QueryString["timezone"].ToString());
            ERPContext db = new ERPContext();
            var roleList = db.tblRoles.Select(a => new { a.Roles, a.IsActive, ChangedOn = a.ChgDate }).ToList().OrderBy(a => a.Roles).ToList();

            DataTable dt = ERPUtilities.ConvertToDataTable(roleList);
            ERPUtilities.ExportExcel(context, timezone, dt, "Role List", "Role List", "RoleList");
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