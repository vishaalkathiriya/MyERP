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
    /// Summary description for SRAmc
    /// </summary>
    public class SRAmc : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            int timezone = Convert.ToInt32(context.Request.QueryString["timezone"].ToString());
            ERPContext db = new ERPContext();

            var des = (from a in db.tblSRAMCs
                       join m in db.tblSRMachines on a.MachineId equals m.MachineId
                       select new { m.MachineName, a.StartDate, a.EndDate, a.Remarks ,a.ChgDate}).ToList();

            DataTable dt = ERPUtilities.ConvertToDataTable(des);
            ERPUtilities.ExportExcel(context, timezone, dt, "AMC List", "AMC List", "SRAMCList");
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