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
    /// Summary description for SRPurchase
    /// </summary>
    public class SRPurchase : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            int timezone = Convert.ToInt32(context.Request.QueryString["timezone"].ToString());
            ERPContext db = new ERPContext();

            var des = (from d in db.tblSRPurchases
                       join dp in db.tblSRParts on d.PartId equals dp.PartId
                       select new { d.PurchaseDate,dp.PartName,d.Quantity, d.ApprovedBy,d.Remarks,d.ChgDate}).ToList();

            DataTable dt = ERPUtilities.ConvertToDataTable(des);
            ERPUtilities.ExportExcel(context, timezone, dt, "Purchase-Entry List", "Purchase-Entry List", "SRPurchaseEntryList");
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