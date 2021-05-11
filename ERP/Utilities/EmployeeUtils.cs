using ERP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.Utilities
{
    public class EmployeeUtils
    {
        ERPContext db = null;
        public EmployeeUtils()
        {
            db = new ERPContext();
        }

        public EmpOrgChart GenerateChartData(EmpOrgChart chartItem)
        {
            var id = chartItem.id;
            if (db.tblEmpCompanyInformations.AsEnumerable().Where(z => z.ReportingTo == Convert.ToInt32(id)).Count() > 0)
            {
                var list = db.tblEmpCompanyInformations.AsEnumerable().Where(z => z.ReportingTo == Convert.ToInt32(id)).ToList();
                foreach (var l in list)
                {
                    var isActive = db.tblEmpPersonalInformations.Single(em => em.EmployeeId == l.EmployeeId).IsActive;
                    if (l.EmployeeId != 1 && isActive == true)
                    {
                        EmpOrgChart empOrgChart = new EmpOrgChart();
                        var employeeID = l.EmployeeId;
                        var employeeName = string.Empty;
                        var designationTitle = string.Empty;
                        var employeePersonalInfo = db.tblEmpPersonalInformations.Where(em => em.EmployeeId == l.EmployeeId).SingleOrDefault();
                        if (employeePersonalInfo != null && employeePersonalInfo.EmployeeId > 0)
                        {
                            employeeName = string.Format("{0}{1}{2}", employeePersonalInfo.CandidateFirstName, employeePersonalInfo.CandidateMiddleName.Substring(0, 1).ToUpper(), employeePersonalInfo.CandidateLastName.Substring(0, 1).ToUpper());
                        }

                        var designation = db.tblDesignations.Where(d => d.Id == l.DesignationId).SingleOrDefault();
                        if (designation != null && designation.Id > 0)
                        {
                            designationTitle = designation.Designation;
                        }

                        empOrgChart.id = employeeID;
                        empOrgChart.name = "<span class='org-chart-char'>" + employeeName.Substring(0, 1) + "</span><span class='org-chart-username'>" + employeeName + "</span>" + "<br/>" + designationTitle;
                        empOrgChart.children = new List<EmpOrgChart>();
                        chartItem.children.Add(empOrgChart);
                        GenerateChartData(empOrgChart);
                    }
                }
            }
            return chartItem;
        }

        // get list of drill down users; pass only login user id
        public List<tblEmpCompanyInformation> DrillDownReporting(string strIds, List<tblEmpCompanyInformation> cList)
        {
            if (!string.IsNullOrEmpty(strIds))
            {
                string temp = string.Empty;
                string[] ids = strIds.Split(',');
                foreach (var id in ids)
                {
                    if (!string.IsNullOrEmpty(id))
                    {
                        if (db.tblEmpCompanyInformations.AsEnumerable().Where(z => z.ReportingTo == Convert.ToInt32(id)).Count() > 0)
                        {
                            var list = db.tblEmpCompanyInformations.AsEnumerable().Where(z => z.ReportingTo == Convert.ToInt32(id)).ToList();
                            foreach (var l in list)
                            {
                                var isActive = db.tblEmpPersonalInformations.Single(em => em.EmployeeId == l.EmployeeId).IsActive;
                                if (l.EmployeeId != 1 && isActive == true)
                                {
                                    cList.Add(l);
                                    temp += l.EmployeeId + ",";
                                }
                            }
                        }
                    }
                }
                DrillDownReporting(temp, cList);
            }
            return cList;
        }


        /// <summary>
        /// return up level user list
        /// </summary>
        public List<tblEmpCompanyInformation> DrillUpReporting(int employeeId, List<tblEmpCompanyInformation> cList)
        { //employee id is reporting id
            var isActive = db.tblEmpPersonalInformations.Single(em => em.EmployeeId == employeeId).IsActive;
            if (employeeId != 1 && isActive == true) //1 is fixed for admin
            {
                if (db.tblEmpCompanyInformations.AsEnumerable().Where(z => z.EmployeeId == employeeId).Count() > 0)
                {//inside if exists
                    //get reportingTo id of that line and insert that line to cList 
                    var line = db.tblEmpCompanyInformations.AsEnumerable().Where(z => z.EmployeeId == employeeId).FirstOrDefault();
                    cList.Add(line);
                    return DrillUpReporting(line.ReportingTo, cList); //Recursive call : go for reporting id, it will be our next employee id
                }
            }
            else
            {
                if (Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString()) != 1)
                {
                    var line = db.tblEmpCompanyInformations.AsEnumerable().Where(z => z.EmployeeId == 1).FirstOrDefault();
                    cList.Add(line);
                }
            }
            return cList;
        }

        //Added by dipak moved from ERPUtilities Class
        public string GetEmployeePendingLeave(int employeeId, int year)
        {
            try
            {
                //Pending Leave Calculation
                if (db.tblEmpPayRollInformations.Where(z => z.EmployeeId == employeeId && z.PermanentFromDate != null).Count() > 0)
                {
                    #region UPDATED BY VISHALHK
                    tblEmpPayRollInformation objPayroll = db.tblEmpPayRollInformations.Where(z => z.EmployeeId == employeeId).FirstOrDefault();

                    if (objPayroll.EmploymentStatus == "Probation")
                    {
                        return "0";
                    }
                    else
                    {
                        float avgLeave = (float)objPayroll.LeavesAllowedPerYear / 12;
                        float allowedLeave = (float)objPayroll.LeavesAllowedPerYear - (avgLeave * (DateTime.Now.Month - 1));

                        return (allowedLeave).ToString();
                    }
                    #endregion

                    #region COMMENTED BY VISHALHK
                    ////First and Last Day of selected year
                    //DateTime firstDayOfYear = new DateTime(year, 1, 1);
                    //DateTime lastDayOfYear = new DateTime(year, 12, 31);

                    ////gEt Total Leave and Total OT between First and Last Day of Selected Year and between Permenant and Releaving Date
                    //var data = (from a in db.tblEmpAttendances
                    //            where a.PDate >= firstDayOfYear && a.PDate <= lastDayOfYear
                    //            group a by a.EmployeeId into grp
                    //            select new { EmpId = grp.Key, Total_L = grp.Sum(z => z.Leave), Total_O = grp.Sum(z => z.OT) })
                    //            .Where(z => z.EmpId == employeeId).FirstOrDefault();

                    ////Get Permenant-Join-Date and Releaving Date(If Exists) of Employee
                    //tblEmpPayRollInformation objPayroll = db.tblEmpPayRollInformations.Where(z => z.EmployeeId == employeeId).FirstOrDefault();

                    //if (objPayroll.EmploymentStatus == "Probation")
                    //{
                    //    return "0";
                    //}
                    //else
                    //{
                    //    float avgLeave = (float)objPayroll.LeavesAllowedPerYear / 12;

                    //    DateTime? releaveDate = objPayroll.ReLeavingDate == null ? lastDayOfYear : objPayroll.ReLeavingDate;
                    //    DateTime? permenatDate = objPayroll.PermanentFromDate < firstDayOfYear ? firstDayOfYear : objPayroll.PermanentFromDate;

                    //    int i = ((releaveDate.Value.Day <= 15 ? releaveDate.Value.Month - 1 : releaveDate.Value.Month) - (permenatDate.Value.Day <= 15 ? permenatDate.Value.Month - 1 : permenatDate.Value.Month)) + 12 * (releaveDate.Value.Year - permenatDate.Value.Year);
                    //    float allowedLeave = avgLeave * i;

                    //    return (allowedLeave - Convert.ToDouble(data.Total_L) + Convert.ToDouble(data.Total_O)).ToString();
                    //}
                    #endregion
                }
                else
                {
                    return string.Empty;
                }
                //End Pending Leave Calculation
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}