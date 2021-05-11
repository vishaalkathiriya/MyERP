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
    /// Summary description for HRDFireHydrantSprinklerSystem
    /// </summary>
    public class HRDFireHydrantSprinklerSystem : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            int timezone = Convert.ToInt32(context.Request.QueryString["timezone"].ToString());
            string startDate = context.Request.QueryString["startDate"].ToString();
            string endDate = context.Request.QueryString["endDate"].ToString();

            ERPContext db = new ERPContext();
            var FireHydrant = (from fire_hydrant in db.tblHRDFireHydrantandSprinklerSystems
                                      select new
                                      {
                                          fire_hydrant.BuildingName,
                                          fire_hydrant.DateOfInspection,
                                          fire_hydrant.CheckedBy,
                                          fire_hydrant.Findings,
                                          fire_hydrant.RootCause,
                                          fire_hydrant.CorrectiveActionTaken,
                                          fire_hydrant.Remark,
                                      }).ToList();


            if (!string.IsNullOrEmpty(startDate))
            {
                string[] fdate = startDate.Split('/');
                DateTime fromDate = new DateTime(Convert.ToInt32(fdate[2]), Convert.ToInt32(fdate[1]), Convert.ToInt32(fdate[0]));
                if (startDate == endDate)
                {
                    FireHydrant = FireHydrant.Where(z => z.DateOfInspection.Date == fromDate.Date).ToList();
                }
                else
                {//date range
                    string[] tdate = endDate.Split('/');
                    DateTime toDate = new DateTime(Convert.ToInt32(tdate[2]), Convert.ToInt32(tdate[1]), Convert.ToInt32(tdate[0]));
                    FireHydrant = FireHydrant.Where(z => z.DateOfInspection.Date >= fromDate.Date && z.DateOfInspection.Date <= toDate.Date).ToList();
                }
            }

            DataTable dt = ERPUtilities.ConvertToDataTable(FireHydrant);
            ERPUtilities.ExportExcel(context, timezone, dt, "Fire Hydrant Sprinkler System", "Fire Hydrant Sprinkler System", "Fire Hydrant Sprinkler System");
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