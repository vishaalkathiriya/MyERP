using ERP.Models;
using ERP.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ERP.Models
{
  public class EmpOrgChart
  {
    public int id { get; set; }
    public string name { get; set; }
    public List<EmpOrgChart> children { get; set; }
  }
}

namespace ERP.WebApis
{
  public class EmployeeOrgChartController : ApiController
  {

    ERPContext db = null;
    SessionUtils sessionUtils = null;
    GeneralMessages generalMessages = null;
    string _pageName = "Employee Organizational Chart";
    public EmployeeOrgChartController()
    {
      db = new ERPContext();
      sessionUtils = new SessionUtils();
      generalMessages = new GeneralMessages(_pageName);
    }

    public EmpOrgChart GetItems()
    {
      EmpOrgChart empChart = new EmpOrgChart();
      EmpOrgChart firstNode = new EmpOrgChart() { id = 1, name = "<span class='org-chart-char'>M</span><span class='org-chart-username'>Master Admin</span>" + "<br/>Admin", children = new List<EmpOrgChart>() };
      EmployeeUtils employeeUtils = new EmployeeUtils();
      return employeeUtils.GenerateChartData(firstNode);
    }

    // GET api/<controller>/5
    public List<EmpOrgChart> Get(int id)
    {
      List<EmpOrgChart> lstEmpOrgChart = new List<EmpOrgChart>();
      if (db.tblEmpCompanyInformations.AsEnumerable().Where(z => z.ReportingTo == Convert.ToInt32(id)).Count() > 0)
      {
        var list = db.tblEmpCompanyInformations.AsEnumerable().Where(z => z.ReportingTo == Convert.ToInt32(id)).ToList();
        foreach (var l in list)
        {
          if (l.EmployeeId != 1)
          {
            EmpOrgChart empOrgChart = new EmpOrgChart();
            var employeeID = l.EmployeeId;
            var employeeName = string.Empty;
            var designationTitle = string.Empty;
            var employeePersonalInfo = db.tblEmpPersonalInformations.Where(em => em.EmployeeId == l.EmployeeId).SingleOrDefault();
            if (employeePersonalInfo != null && employeePersonalInfo.EmployeeId > 0)
            {
              employeeName = employeePersonalInfo.CandidateFirstName + " " + employeePersonalInfo.CandidateMiddleName + " " + employeePersonalInfo.CandidateLastName;
            }

            var designation = db.tblDesignations.Where(d => d.Id == l.DesignationId).SingleOrDefault();
            if (designation != null && designation.Id > 0)
            {
              designationTitle = designation.Designation;
            }

            empOrgChart.id = employeeID;
            empOrgChart.name = employeeName;
            //empOrgChart.Designation = designationTitle;
            lstEmpOrgChart.Add(empOrgChart);
          }
        }
      }
      return lstEmpOrgChart;

    }
   
  }
}