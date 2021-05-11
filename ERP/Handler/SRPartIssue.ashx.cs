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
    /// Summary description for SRPartIssue
    /// </summary>
    public class SRPartIssue : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {

            int timezone = Convert.ToInt32(context.Request.QueryString["timezone"].ToString());
            ERPContext db = new ERPContext();

            var des = (from i in db.tblSRPartIssues
                       join m in db.tblSRMachines on i.MachineId equals m.MachineId
                       join p in db.tblSRParts on i.PartId equals p.PartId
                       select new { m.MachineName, p.PartName, i.IssuedFrom, i.IssuedDate, i.ChallanNo, i.Problem,i.Remarks,i.ChgDate }).ToList();

            DataTable dt = ERPUtilities.ConvertToDataTable(des);
            ERPUtilities.ExportExcel(context, timezone, dt, "Part-Issue List", "Part-Issue List", "SRPartIssueList");
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