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
    /// Summary description for HRDBoilPlantCleaningRecords
    /// </summary>
    public class HRDBoilPlantCleaningRecords : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            int timezone = Convert.ToInt32(context.Request.QueryString["timezone"].ToString());
            string startDate = context.Request.QueryString["startDate"].ToString();
            string endDate = context.Request.QueryString["endDate"].ToString();

            ERPContext db = new ERPContext();
            var BoilPlant = (from boilplant in db.tblHRDBoilPlantCleaningRecords
                            select new
                            {
                                boilplant.BoilPlantLocation,
                              //  DateOfCleaining=boilplant.DateOfCleaining.(),
                             //   boilplant = boilplant.DateOfCleaining.ToShortDateString(),.
                                boilplant.DateOfCleaining,
                                boilplant.NameOfCleaner,
                                boilplant.PlantIncharge,
                                Remark=boilplant.Remark==null?"":boilplant.Remark
                            }).ToList();

           
            if (!string.IsNullOrEmpty(startDate))
            {
                string[] fdate = startDate.Split('/');
                DateTime fromDate = new DateTime(Convert.ToInt32(fdate[2]), Convert.ToInt32(fdate[1]), Convert.ToInt32(fdate[0]));
                if (startDate == endDate)
                {
                    BoilPlant = BoilPlant.Where(z => z.DateOfCleaining.Date == fromDate.Date).ToList();
                }
                else
                {//date range
                    string[] tdate = endDate.Split('/');
                    DateTime toDate = new DateTime(Convert.ToInt32(tdate[2]), Convert.ToInt32(tdate[1]), Convert.ToInt32(tdate[0]));
                    BoilPlant = BoilPlant.Where(z => z.DateOfCleaining.Date >= fromDate.Date && z.DateOfCleaining.Date <= toDate.Date).ToList();
                }
            }

            DataTable dt = ERPUtilities.ConvertToDataTable(BoilPlant);
            ERPUtilities.ExportExcel(context, timezone, dt, "Boil Plant Cleaning Records", "Boil Plant Cleaning Records", "Boil Plant Cleaning Records");
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