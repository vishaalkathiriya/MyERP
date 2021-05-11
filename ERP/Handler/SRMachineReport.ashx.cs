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
    /// Summary description for SRMachineReport
    /// </summary>
    public class SRMachineReport : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            int timezone = Convert.ToInt32(context.Request.QueryString["timezone"].ToString());
            ERPContext db = new ERPContext();
            List<tblSRMachine> listMachine = null;
            List<tblSRFloor> listFloor = null;
            List<tblLocation> listLocation = null;
            List<tblSRAMC> listAMC = null;
            //List<SRMachineReportViewModel> lstMachineReport = new List<SRMachineReportViewModel>();

            listMachine = db.tblSRMachines.ToList();
            listFloor = db.tblSRFloors.ToList();
            listLocation = db.tblLocations.ToList();
            listAMC = db.tblSRAMCs.ToList();

            var lstMachineReport = (from m in listMachine
                                    join f in listFloor on m.MachineId equals f.MachineId
                                    join l in listLocation on f.LocationId equals l.LocationId
                                    join a in listAMC on m.MachineId equals a.MachineId
                                    select new
                                    {
                                        MachineName = m.MachineName,
                                        SerialNo = m.SerialNo,
                                        InstallationDate = m.InstallationDate,
                                        TypeName = db.tblSRTypes.Where(z => z.TypeId == m.TypeId).Select(z => z.TypeName).SingleOrDefault(),
                                        SubTypeName = db.tblSRSubTypes.Where(z => z.SubTypeId == m.SubTypeId).Select(z => z.SubTypeName).SingleOrDefault(),
                                        ParameterName = db.tblSRParameters.Where(z => z.ParameterId == m.ParameterId).Select(z => z.ParameterName).SingleOrDefault(),
                                        LocationName = l.LocationName,
                                        ManagerName = f.Manager,
                                        StartDate = a.StartDate,
                                        EndDate = a.EndDate
                                    }).ToList();

            DataTable dt = ERPUtilities.ConvertToDataTable(lstMachineReport);
            ERPUtilities.ExportExcel(context, timezone, dt, "Machine Report", "Machine Report", "SRMachineReportList");
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