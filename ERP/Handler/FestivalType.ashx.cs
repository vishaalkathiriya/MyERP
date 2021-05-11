using System;
using System.Linq;
using System.Web;
using ERP.Models;
using System.Data;
using ERP.Utilities;

namespace ERP.Handler
{
    /// <summary>
    /// Summary description for FestivalType
    /// </summary>
    public class FestivalType : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            int timezone = Convert.ToInt32(context.Request.QueryString["timezone"].ToString());
            ERPContext db = new ERPContext();
            var fList = db.tblFestivalTypes.Select(a => new { a.FestivalType, a.IsWorkingDay, a.PartFullTime, a.DisplayColorCode, a.IsActive, ChangedOn = a.ChgDate }).ToList().OrderBy(a => a.FestivalType).ToList();

            DataTable dt = ERPUtilities.ConvertToDataTable(fList);
            ERPUtilities.ExportExcel(context, timezone, dt, "FestivalType List", "FestivalType List", "FestivalTypeList");
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