using System;
using System.Linq;
using System.Web;
using ERP.Models;
using System.Data;
using ERP.Utilities;

namespace ERP.Handler
{
    /// <summary>
    /// Summary description for Brand
    /// </summary>
    public class Brand : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            int timezone = Convert.ToInt32(context.Request.QueryString["timezone"].ToString());
            ERPContext db = new ERPContext();
            var brand = db.tblBrands.Select(a => new { a.BrandName, a.IsActive, ChangedOn = a.ChgDate }).ToList().OrderBy(a => a.BrandName).ToList();

            DataTable dt = ERPUtilities.ConvertToDataTable(brand);

            ERPUtilities.ExportExcel(context, timezone, dt, "Brand List", "Brand List", "BrandList");
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