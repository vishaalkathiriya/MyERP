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
    /// Summary description for HRDFireExtinguiserLogBook
    /// </summary>
    public class HRDFireExtinguiserLogBook : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            int timezone = Convert.ToInt32(context.Request.QueryString["timezone"].ToString());
            string startDate = context.Request.QueryString["startDate"].ToString();
            string endDate = context.Request.QueryString["endDate"].ToString();

            ERPContext db = new ERPContext();
           
            var FireExtinguiser = (from fireextinguiser in db.tblHRDFireExtinguiserLogBooks.AsEnumerable()
                                   select new
                                   {
                                       fireextinguiser.TypeOfFireExtinguiser,
                                       fireextinguiser.Capacity,
                                       fireextinguiser.Location,
                                       fireextinguiser.DateOfInspection,
                                       UsedOfFireExtinguiser= fireextinguiser.UsedOfFireExtinguiser==null?"":fireextinguiser.UsedOfFireExtinguiser.ToString(),
                                       DateOfRefilling = fireextinguiser.DateOfRefilling.ToString("dd/MM/yyyy"),
                                       DueDateForNextRefilling = fireextinguiser.DueDateForNextRefilling.ToString("dd/MM/yyyy"),
                                       Reason=fireextinguiser.Reason==null?"":fireextinguiser.Reason,
                                       Remark=fireextinguiser.Remark==null?"":fireextinguiser.Remark

                                   }).ToList();

            //top filter
            if (!string.IsNullOrEmpty(startDate))
            {
                string[] fdate = startDate.Split('/');
                DateTime fromDate = new DateTime(Convert.ToInt32(fdate[2]), Convert.ToInt32(fdate[1]), Convert.ToInt32(fdate[0]));
                if (startDate == endDate)
                {
                    FireExtinguiser = FireExtinguiser.Where(z => z.DateOfInspection.Date == fromDate.Date).ToList();
                }
                else
                {//date range
                    string[] tdate = endDate.Split('/');
                    DateTime toDate = new DateTime(Convert.ToInt32(tdate[2]), Convert.ToInt32(tdate[1]), Convert.ToInt32(tdate[0]));
                    FireExtinguiser = FireExtinguiser.Where(z => z.DateOfInspection.Date >= fromDate.Date && z.DateOfInspection.Date <= toDate.Date).ToList();
                }
            }

            DataTable dt = ERPUtilities.ConvertToDataTable(FireExtinguiser);
            ERPUtilities.ExportExcel(context, timezone, dt, "Fire Extinguiser Log Book", "Fire Extinguiser Log Book", "Fire Extinguiser Log Book");
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