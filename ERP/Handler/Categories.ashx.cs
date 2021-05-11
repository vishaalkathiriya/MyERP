using System;
using System.Linq;
using System.Web;
using ERP.Models;
using System.Data;
using ERP.Utilities;

namespace ERP.Handler
{
    /// <summary>
    /// Summary description for Categories
    /// </summary>
    public class Categories : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            int timezone = Convert.ToInt32(context.Request.QueryString["timezone"].ToString());
            ERPContext db = new ERPContext();
            var cat = db.tblCategories.Select(a => new { a.CategoryName, a.IsActive, ChangedOn = a.ChgDate }).ToList().OrderBy(a => a.CategoryName).ToList();

            DataTable dt = ERPUtilities.ConvertToDataTable(cat);

            ERPUtilities.ExportExcel(context, timezone, dt, "Category List", "Category List", "CategoryList");
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