using ERP.Models;
using ERP.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web;


namespace ERP.Handler
{
    /// <summary>
    /// Summary description for SRRepair
    /// </summary>
    public class SRRepair : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            int timezone = Convert.ToInt32(context.Request.QueryString["timezone"].ToString());
            ERPContext db = new ERPContext();

            var des = (from r in db.tblSRRepairs
                       join m in db.tblSRMachines on r.MachineId equals m.MachineId
                       join p in db.tblSRParts on r.PartId equals p.PartId
                       select new
                       {
                           m.MachineName,
                           m.SerialNo,
                           p.PartName,
                           RepairBy = r.RepairedBy.Equals("S") ? "Sarin" : r.Others,
                           r.Problem,
                           IssueDate = SqlFunctions.DateName("day", r.IssueDate).Trim() + "-" +
                               SqlFunctions.DateName("month", r.IssueDate).TrimStart() + "-" +
                               SqlFunctions.DateName("year", r.IssueDate),
                           ReceiveDate = r.ReceiveDate == null ? "" : SqlFunctions.DateName("day", r.ReceiveDate).Trim() + "-" +
                               SqlFunctions.DateName("month", r.ReceiveDate).TrimStart() + "-" +
                               SqlFunctions.DateName("year", r.ReceiveDate)
                               ,
                           r.Remarks,
                           r.ChgDate
                       }).ToList();


            var list = des.Select(z => new
            {
                z.MachineName,
                z.SerialNo,
                z.PartName,
                z.RepairBy,
                z.Problem,
                z.IssueDate,
                z.ReceiveDate,
                z.Remarks,
                z.ChgDate
            }).OrderBy(z => z.MachineName).ToList();

            DataTable dt = ERPUtilities.ConvertToDataTable(list);
            ERPUtilities.ExportExcel(context, timezone, dt, "Repair-Entry List", "Repair-Entry List", "SRRepairEntryList");
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