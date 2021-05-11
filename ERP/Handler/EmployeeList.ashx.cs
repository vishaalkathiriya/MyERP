using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ERP.Models;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using System.Data.SqlClient;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using ERP.Utilities;

namespace ERP.Handler
{
    /// <summary>
    /// Summary description for EmployeeList
    /// </summary>
    public class EmployeeList : IHttpHandler
    {

        int timezone;
        public void ProcessRequest(HttpContext context)
        {
            try {
                timezone = Convert.ToInt32(context.Request.QueryString["timezone"].ToString());
                ERPContext db = new ERPContext();

                //var lstEmployee = db.tblEmpPersonalInformations.Select(a => new { a.EmployeeRegisterCode,a.CandidateFirstName, a.IsActive, a.ChgDate }).ToList().OrderBy(a => a.CandidateFirstName).ToList();

                List<EmployeeViewModel> list = new List<EmployeeViewModel>();
                var lstEmployee = (from pi in db.tblEmpPersonalInformations
                                   join ci in db.tblEmpCompanyInformations on pi.EmployeeId equals ci.EmployeeId into group1
                                   from g1 in group1.DefaultIfEmpty()
                                   join pr in db.tblEmpPayRollInformations on pi.EmployeeId equals pr.EmployeeId into group2
                                   from g2 in group2.DefaultIfEmpty()
                                   where pi.EmployeeId != 1
                                   select new
                                   {
                                       pi.EmployeeId,
                                       pi.EmployeeRegisterCode,
                                       pi.CandidateFirstName,
                                       pi.CandidateMiddleName,
                                       pi.CandidateLastName,
                                       pi.IsActive,
                                       pi.ChgDate,
                                       DesignationId = (int?)g1.DesignationId
                                       //,
                                       //g2.JoiningDate,
                                       //ReLeavingDate = (DateTime?)g2.ReLeavingDate
                                   }).ToList();

                foreach (var l in lstEmployee)
                {
                    //CALCULATE EXP IN COMPANY
                    var payroll = db.tblEmpPayRollInformations.Where(z => z.EmployeeId == l.EmployeeId).SingleOrDefault();
                    int daysDiffCompanyExp = 0;
                    int daysDiffWorkExp = 0;
                    bool isJoinDateExists = true;
                    if (payroll != null)
                    {
                        if (payroll.JoiningDate != null)
                        {
                            if (payroll.ReLeavingDate != null)
                            {
                                DateTime dtR = Convert.ToDateTime(payroll.ReLeavingDate);
                                DateTime dtJoin = new DateTime(payroll.JoiningDate.Year, payroll.JoiningDate.Month, payroll.JoiningDate.Day, 0, 0, 0);
                                DateTime dtReleaving = new DateTime(dtR.Year, dtR.Month, dtR.Day, 0, 0, 0);
                                daysDiffCompanyExp = (dtReleaving.Date - dtJoin.Date).Days;
                            }
                            else
                            {
                                DateTime dtJoin = new DateTime(payroll.JoiningDate.Year, payroll.JoiningDate.Month, payroll.JoiningDate.Day, 0, 0, 0);
                                DateTime dtNow = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                                daysDiffCompanyExp = (dtNow.Date - dtJoin.Date).Days;
                            }

                            var baseDate = new DateTime(1, 1, 1);
                            var end = baseDate.AddDays(daysDiffCompanyExp);

                            //CALCULATE TOTAL EXP
                            var work = db.tblEmpWorkExperiences.Where(z => z.EmployeeId == l.EmployeeId).ToList();
                            foreach (var w in work)
                            {
                                DateTime dtFrom = new DateTime(w.FromDate.Year, w.FromDate.Month, w.FromDate.Day, 0, 0, 0);
                                DateTime dtTo = new DateTime(w.ToDate.Year, w.ToDate.Month, w.ToDate.Day, 0, 0, 0);
                                daysDiffWorkExp += (dtTo.Date - dtFrom.Date).Days;
                            }
                            daysDiffWorkExp += daysDiffCompanyExp;

                            var baseDate1 = new DateTime(1, 1, 1);
                            var end1 = baseDate1.AddDays(daysDiffWorkExp);

                            EmployeeViewModel e = new EmployeeViewModel
                            {
                                EmployeeId = l.EmployeeId,
                                EmployeeRegisterCode = l.EmployeeRegisterCode,
                                EmployeeName = string.Format("{0} {1} {2}", l.CandidateFirstName, l.CandidateMiddleName, l.CandidateLastName),
                                Designation = db.tblDesignations.Where(z => z.Id == l.DesignationId).Select(z => z.Designation).SingleOrDefault(),
                                ExpInCompany = string.Format("{0} year,  {1} month", end.Year - baseDate.Year, end.Month - baseDate.Month),
                                ExpTotal = string.Format("{0} year, {1} month", end1.Year - baseDate1.Year, end1.Month - baseDate1.Month),
                                IsActive = l.IsActive,
                                ChgDate = Convert.ToDateTime(l.ChgDate).AddMinutes(-1 * timezone)
                            };
                            list.Add(e);
                        }
                        else {
                            isJoinDateExists = false;
                        }
                    }
                    else {
                        EmployeeViewModel e = new EmployeeViewModel
                        {
                            EmployeeId = l.EmployeeId,
                            EmployeeRegisterCode = l.EmployeeRegisterCode,
                            EmployeeName = string.Format("{0} {1} {2}", l.CandidateFirstName, l.CandidateMiddleName, l.CandidateLastName),
                            Designation = db.tblDesignations.Where(z => z.Id == l.DesignationId).Select(z => z.Designation).SingleOrDefault(),
                            ExpInCompany = "0",
                            ExpTotal = "0",
                            IsActive = l.IsActive,
                            ChgDate = Convert.ToDateTime(l.ChgDate).AddMinutes(-1 * timezone)
                        };
                        list.Add(e);
                    }

                    
                    

                    
                }


                DataTable dt = ERPUtilities.ConvertToDataTable(list);
                ERPUtilities.ExportExcel(context, timezone, dt, "Employee List", "Employee List", "EmployeeList");
                context = null;
            }
            catch { 
                // handle your exception
            }
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