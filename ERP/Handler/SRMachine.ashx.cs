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
    /// Summary description for SRMachine
    /// </summary>
    public class SRMachine : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            int timezone = Convert.ToInt32(context.Request.QueryString["timezone"].ToString());
            ERPContext db = new ERPContext();

            var des = (from d in db.tblSRMachines
                       join t in db.tblSRTypes on d.TypeId equals t.TypeId
                       join s in db.tblSRSubTypes on d.SubTypeId equals s.SubTypeId
                       join p in db.tblSRParameters on d.ParameterId equals p.ParameterId
                       select new { d.MachineName,d.SerialNo,d.InstallationDate, t.TypeName, s.SubTypeName,p.ParameterName}).ToList();

            DataTable dt = ERPUtilities.ConvertToDataTable(des);
            ERPUtilities.ExportExcel(context, timezone, dt, "Machine List", "Machine List", "SRMachineList");
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