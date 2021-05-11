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
    /// Summary description for HRDAccidentRecords
    /// </summary>
    public class HRDAccidentRecords : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {

            int timezone = Convert.ToInt32(context.Request.QueryString["timezone"].ToString());
            string startDate = context.Request.QueryString["startDate"].ToString();
            string endDate = context.Request.QueryString["endDate"].ToString();

            ERPContext db = new ERPContext();
            
            List<tblHRDAccidentRecord> filteredData = db.tblHRDAccidentRecords.ToList();
          
            //top filter
            if (!string.IsNullOrEmpty(startDate))
            {
                string[] fdate = startDate.Split('/');
                DateTime fromDate = new DateTime(Convert.ToInt32(fdate[2]), Convert.ToInt32(fdate[1]), Convert.ToInt32(fdate[0]));
                if (startDate == endDate)
                {
                    filteredData = filteredData.AsEnumerable().Where(z => z.CreDate.Date == fromDate.Date).ToList();
                }
                else
                {//date range
                    string[] tdate = endDate.Split('/');
                    DateTime toDate = new DateTime(Convert.ToInt32(tdate[2]), Convert.ToInt32(tdate[1]), Convert.ToInt32(tdate[0]));
                    filteredData = filteredData.AsEnumerable().Where(z => z.CreDate.Date >= fromDate.Date && z.CreDate.Date <= toDate.Date).ToList();
                }
            }



            var Accident_Records = (from AccidentRecords in filteredData
                                    select new
                                    {
                                        AccidentRecords.TypeOfAccident,
                                        AccidentRecords.Department,
                                        AccidentRecords.ManagerName,
                                        AccidentRecords.NameOfInjuredPerson,
                                        AccidentRecords.RootCauseOfAccident,
                                        AccidentRecords.NoOfCasualities,
                                        AccidentRecords.CorrectiveActionTaken,
                                        Hospitalized = AccidentRecords.Hospitalized ? "Yes" : "No",
                                        NameOfHospital = AccidentRecords.NameOfHospital == null ? "" : AccidentRecords.NameOfHospital,
                                        TreatmentExpenses = AccidentRecords.TreatmentExpenses == null ? 0 : AccidentRecords.TreatmentExpenses.Value,
                                      
                                    }).ToList();



       
            //var Accident_Records_new = Accident_Records.Select(z => new
            //   {
            //       z.TypeOfAccident,
            //       z.Department,
            //       z.ManagerName,
            //       z.NameOfInjuredPerson,
            //       z.RootCauseOfAccident,
            //       z.NoOfCasualities,
            //       z.CorrectiveActionTaken,
            //       z.Hospitalized,
            //       z.NameOfHospital,
            //       z.TreatmentExpenses
            //   }).ToList();

            DataTable dt = ERPUtilities.ConvertToDataTable(Accident_Records);
            ERPUtilities.ExportExcel(context, timezone, dt, "Accident Records List", "Accident Records List", "Accident Records List");
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
