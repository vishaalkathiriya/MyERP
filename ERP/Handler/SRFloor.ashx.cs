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
    /// Summary description for SRFloor
    /// </summary>
    public class SRFloor : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            int timezone = Convert.ToInt32(context.Request.QueryString["timezone"].ToString());
            ERPContext db = new ERPContext();

            var des = (from d in db.tblSRFloors
                       join m in db.tblSRMachines   on d.MachineId equals m.MachineId
                       join l in db.tblLocations on d.LocationId equals l.LocationId
                       select new { m.MachineName, l.LocationName, d.Manager, d.Remarks, d.ChgDate }).ToList();

            DataTable dt = ERPUtilities.ConvertToDataTable(des);
            ERPUtilities.ExportExcel(context, timezone, dt, "Floor List", "Floor List", "SRFloorList");
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