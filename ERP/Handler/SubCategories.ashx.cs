using System;
using System.Linq;
using System.Web;
using ERP.Models;
using System.Data;
using ERP.Utilities;

namespace ERP.Handler
{
    /// <summary>
    /// Summary description for SubCategories
    /// </summary>
    public class SubCategories : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            int timezone = Convert.ToInt32(context.Request.QueryString["timezone"].ToString());
            int CategoryId = Convert.ToInt16(context.Request.QueryString["category"]);
            ERPContext db = new ERPContext();
            var subCat = (from sc in db.tblSubCategories
                          join c in db.tblCategories
                          on sc.CategoryId equals c.CategoryId
                          select new { c.CategoryName, sc.CategoryId, sc.SubCategoryName, sc.IsActive, ChangedOn = sc.ChgDate });
            if (CategoryId != 0)
            {
                subCat = subCat.Where(p => p.CategoryId == CategoryId);
            }
            var subCatList = (from s in subCat
                      select new { s.CategoryName, s.SubCategoryName, s.IsActive, s.ChangedOn }).ToList();
            DataTable dt = ERPUtilities.ConvertToDataTable(subCatList);

            ERPUtilities.ExportExcel(context, timezone, dt, "SubCategory List", "SubCategory List", "SubCategoryList");
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