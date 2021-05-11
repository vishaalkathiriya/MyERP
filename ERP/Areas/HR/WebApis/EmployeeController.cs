using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ERP.Models;
using System.Text;
using System.Collections.Specialized;
using System.Web;
using ERP.Utilities;
using System.Configuration;
using System.Data.Entity.Validation;
using System.IO;

namespace ERP.WebApis
{
    public class EmployeeController : ApiController
    {
        ERPContext db = null;
        SessionUtils sessionUtils = null;
        GeneralMessages generalMessages = null;
        string _pageName = "Employee";
        string _pageNameQualification = "Employee Qualification";
        string _pageNamePayRoll = "Employee PayRoll Information";
        string _pageNameWorkExp = "Employee Work Experience";
        string _pageNameRelativeInfo = "Employee Relative Information";
        string _pageNameDocument = "Employee Document";
        string _pageNameCompanyInfo = "Employee Company Information";
        string _pageNameLoginInfo = "Employee Login Information";
        string _pageNameCredentialInfo = "Employee Credential Information";
        public EmployeeController()
        {
            db = new ERPContext();
            sessionUtils = new SessionUtils();
            generalMessages = new GeneralMessages(_pageName);
        }

        #region EMPLOYEE

        /// <summary>
        /// GET api/employee
        /// fetch employee 
        /// </summary>
        [HttpGet]
        public ApiResponse FetchEmployee()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int employeeId = Convert.ToInt32(nvc["employeeId"]);
                    if (employeeId != 1)
                    {
                        var list = db.tblEmpPersonalInformations.Where(z => z.EmployeeId == employeeId).ToList();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", list);
                    }
                    else
                    {
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 0, generalMessages.msgAccessDenied, null);
                    }
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }

        /// <summary>
        /// GET api/employee
        /// retrieve country list 
        /// </summary>
        [HttpGet]
        public ApiResponse RetrieveCountry()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var list = db.tblCountries.OrderBy(z => z.CountryName)
                        .Where(z => z.IsActive == true)
                        .Select(z => new SelectItemModel
                        {
                            Id = z.CountryId,
                            Label = z.CountryName
                        }).ToList();

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", list);
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }

        /// <summary>
        /// GET api/employee
        /// retrieve state list 
        /// </summary>
        [HttpGet]
        public ApiResponse RetrieveState()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int countryId = Convert.ToInt32(nvc["countryId"]);

                    var list = db.tblStates.OrderBy(z => z.StateName)
                        .Where(z => z.CountryId == countryId)
                        .Select(z => new SelectItemModel
                        {
                            Id = z.StateId,
                            Label = z.StateName
                        }).ToList();

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", list);
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }

        /// <summary>
        /// GET api/employee
        /// retrieve blood group list 
        /// </summary>
        [HttpGet]
        public ApiResponse RetrieveBloodGroup()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var list = db.tblBloodGroups.OrderBy(z => z.BloodGroupName)
                        .Where(z => z.IsActive == true)
                        .Select(z => new SelectItemModel
                        {
                            Id = z.BloodGroupId,
                            Label = z.BloodGroupName
                        }).ToList();

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", list);
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }

        /// <summary>
        /// POST api/employee
        /// create and update record
        /// </summary>
        [HttpPost]
        public ApiResponse SaveEmployee(tblEmpPersonalInformation emp)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {

                    if (emp.MaritalStatus != "Married")
                    {
                        emp.MarriageAnniversaryDate = null;
                    }

                    int eId;
                    if (emp.EmployeeId == 0)
                    {// Mode == Add
                        tblEmpPersonalInformation e = new tblEmpPersonalInformation
                        {
                            EmployeeRegisterCode = emp.EmployeeRegisterCode,
                            CandidateFirstName = emp.CandidateFirstName,
                            CandidateMiddleName = emp.CandidateMiddleName,
                            CandidateLastName = emp.CandidateLastName,
                            GuardianFirstName = emp.GuardianFirstName,
                            GuardianMiddleName = emp.GuardianMiddleName,
                            GuardianLastName = emp.GuardianLastName,
                            ProfilePhoto = emp.ProfilePhoto,
                            Present_HouseNo = emp.Present_HouseNo,
                            Present_Location = emp.Present_Location,
                            Present_Area = emp.Present_Area,
                            Present_Country = emp.Present_Country,
                            Present_State = emp.Present_State,
                            Present_City = emp.Present_City,
                            Present_PostalCode = emp.Present_PostalCode,
                            Permanent_HouseNo = emp.Permanent_HouseNo,
                            Permanent_Location = emp.Permanent_Location,
                            Permanent_Area = emp.Permanent_Area,
                            Permanent_Country = emp.Permanent_Country,
                            Permanent_State = emp.Permanent_State,
                            Permanent_City = emp.Permanent_City,
                            Permanent_PostalCode = emp.Permanent_PostalCode,
                            MaritalStatus = emp.MaritalStatus,
                            MarriageAnniversaryDate = emp.MarriageAnniversaryDate,
                            BirthDate = emp.BirthDate,
                            Gender = emp.Gender,
                            DrivingLicenceNumber = emp.DrivingLicenceNumber,
                            PassportNumber = emp.PassportNumber,
                            PassportExpiryDate = emp.PassportExpiryDate,
                            AdharNumber = emp.AdharNumber,
                            PANCardNumber = emp.PANCardNumber,
                            PersonalEmailId = emp.PersonalEmailId,
                            PersonalMobile = emp.PersonalMobile,
                            NomineeMobile = emp.NomineeMobile,
                            CompanyEmailId = emp.CompanyEmailId,
                            CompanyMobile = emp.CompanyMobile,
                            BloodGroup = emp.BloodGroup,
                            IsActive = emp.IsActive,
                            CreDate = DateTime.Now.ToUniversalTime(),
                            CreBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString()),
                            ChgDate = DateTime.Now.ToUniversalTime(),
                            ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString())
                        };
                        db.tblEmpPersonalInformations.Add(e);
                        db.SaveChanges();

                        //getting last inserted record for next tabs
                        eId = db.tblEmpPersonalInformations.Max(z => z.EmployeeId);
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgInsert, db.tblEmpPersonalInformations.Where(z => z.EmployeeId == eId).First());
                    }
                    else
                    {// Mode == Edit
                        var line = db.tblEmpPersonalInformations.Where(z => z.EmployeeId == emp.EmployeeId).SingleOrDefault();

                        //delete profile picture
                        if (line.ProfilePhoto != emp.ProfilePhoto)
                        {
                            DeleteProfilePicture(line.ProfilePhoto);
                        }

                        if (line != null)
                        {
                            line.EmployeeRegisterCode = emp.EmployeeRegisterCode;
                            line.CandidateFirstName = emp.CandidateFirstName;
                            line.CandidateMiddleName = emp.CandidateMiddleName;
                            line.CandidateLastName = emp.CandidateLastName;
                            line.GuardianFirstName = emp.GuardianFirstName;
                            line.GuardianMiddleName = emp.GuardianMiddleName;
                            line.GuardianLastName = emp.GuardianLastName;
                            line.ProfilePhoto = emp.ProfilePhoto;
                            line.Present_HouseNo = emp.Present_HouseNo;
                            line.Present_Location = emp.Present_Location;
                            line.Present_Area = emp.Present_Area;
                            line.Present_Country = emp.Present_Country;
                            line.Present_State = emp.Present_State;
                            line.Present_City = emp.Present_City;
                            line.Present_PostalCode = emp.Present_PostalCode;
                            line.Permanent_HouseNo = emp.Permanent_HouseNo;
                            line.Permanent_Location = emp.Permanent_Location;
                            line.Permanent_Area = emp.Permanent_Area;
                            line.Permanent_Country = emp.Permanent_Country;
                            line.Permanent_State = emp.Permanent_State;
                            line.Permanent_City = emp.Permanent_City;
                            line.Permanent_PostalCode = emp.Permanent_PostalCode;
                            line.MaritalStatus = emp.MaritalStatus;
                            line.MarriageAnniversaryDate = emp.MarriageAnniversaryDate;
                            line.BirthDate = emp.BirthDate;
                            line.Gender = emp.Gender;
                            line.DrivingLicenceNumber = emp.DrivingLicenceNumber;
                            line.PassportNumber = emp.PassportNumber;
                            line.PassportExpiryDate = emp.PassportExpiryDate;
                            line.AdharNumber = emp.AdharNumber;
                            line.PANCardNumber = emp.PANCardNumber;
                            line.PersonalEmailId = emp.PersonalEmailId;
                            line.PersonalMobile = emp.PersonalMobile;
                            line.NomineeMobile = emp.NomineeMobile;
                            line.CompanyEmailId = emp.CompanyEmailId;
                            line.CompanyMobile = emp.CompanyMobile;
                            line.BloodGroup = emp.BloodGroup;
                            line.IsActive = emp.IsActive;
                            line.ChgDate = DateTime.Now.ToUniversalTime();
                            line.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                        }

                        db.SaveChanges();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgUpdate, db.tblEmpPersonalInformations.Where(z => z.EmployeeId == line.EmployeeId).First());
                    }

                    MoveFile(emp.ProfilePhoto);
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }

        /// <summary>
        /// POST api/employee
        /// delete employee
        /// </summary>
        [HttpPost]
        public ApiResponse DeleteEmployee([FromBody]int id)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (id != 1)
                    {
                        //Qualification
                        var lstQualification = db.tblEmpQualificationInformations.Where(z => z.EmployeeId == id).ToList();
                        foreach (var l in lstQualification)
                        {
                            db.tblEmpQualificationInformations.Remove(l);
                        }
                        //Work Experience
                        var lstWorkExp = db.tblEmpWorkExperiences.Where(z => z.EmployeeId == id).ToList();
                        foreach (var l in lstWorkExp)
                        {
                            db.tblEmpWorkExperiences.Remove(l);
                        }
                        //Documents
                        var lstDocs = db.tblEmpDocuments.Where(z => z.EmployeeId == id).ToList();
                        foreach (var l in lstDocs)
                        {
                            db.tblEmpDocuments.Remove(l);
                        }
                        //Company Information
                        var lstCompInfo = db.tblEmpCompanyInformations.Where(z => z.EmployeeId == id).SingleOrDefault();
                        if (lstCompInfo != null)
                        {
                            db.tblEmpCompanyInformations.Remove(lstCompInfo);
                        }
                        //Pay roll
                        var lstPayRoll = db.tblEmpPayRollInformations.Where(z => z.EmployeeId == id).SingleOrDefault();
                        if (lstPayRoll != null)
                        {
                            db.tblEmpPayRollInformations.Remove(lstPayRoll);
                        }
                        //Relatives
                        var lstRel = db.tblEmpRelativeInformations.Where(z => z.EmployeeId == id).ToList();
                        foreach (var l in lstRel)
                        {
                            db.tblEmpRelativeInformations.Remove(l);
                        }
                        //Company cred
                        var lstCompCred = db.tblEmpCredentialInformations.Where(z => z.EmployeeId == id).ToList();
                        foreach (var l in lstCompCred)
                        {
                            db.tblEmpCredentialInformations.Remove(l);
                        }
                        //Login
                        var lstLogin = db.tblEmpLoginInformations.Where(z => z.EmployeeId == id).SingleOrDefault();
                        if (lstLogin != null)
                        {
                            db.tblEmpLoginInformations.Remove(lstLogin);
                        }
                        db.SaveChanges();
                        //Main Entry Of Personal Information
                        var line = db.tblEmpPersonalInformations.Where(z => z.EmployeeId == id).SingleOrDefault();
                        if (line != null)
                        {
                            db.tblEmpPersonalInformations.Remove(line);
                            db.SaveChanges();
                        }

                        //delete profile picture
                        DeleteProfilePicture(line.ProfilePhoto);

                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgDelete, null);
                    }
                    else
                    {
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 0, generalMessages.msgAccessDenied, null);
                    }
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }

        /// <summary>
        /// POST api/employee
        /// active-inActive record 
        /// </summary>
        [HttpPost]
        public ApiResponse ChangeStatus(tblEmpPersonalInformation emp)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var line = db.tblEmpPersonalInformations.Where(z => z.EmployeeId == emp.EmployeeId).SingleOrDefault();
                    if (line != null)
                    {
                        if (emp.IsActive)
                        {
                            line.IsActive = false;
                        }
                        else if (!emp.IsActive)
                        {
                            line.IsActive = true;
                        }
                    }

                    line.ChgDate = DateTime.Now.ToUniversalTime();
                    db.SaveChanges();
                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgChangeStatus, null);
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }

        /// <summary>
        /// GET api/employee
        /// return employee list with sorting and filtering  functionalities
        /// </summary>
        [HttpPost]
        public ApiResponse GetEmployeeList(EmployeeViewModel emp)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                using (db)
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int page = Convert.ToInt32(nvc["page"]);
                    int timezone = Convert.ToInt32(nvc["timezone"]);
                    string orderBy = nvc["orderby"];
                    //string filter = nvc["filter"];

                    int iDisplayLength = Convert.ToInt32(nvc["count"]);
                    int iDisplayStart = (page - 1) * iDisplayLength;

                    List<EmployeeViewModel> list = new List<EmployeeViewModel>();
                    try
                    {
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
                                               DesignationId = (int?)g1.DesignationId,
                                               JoiningDate = (DateTime?)g2.JoiningDate ?? null,
                                               ReLeavingDate = (DateTime?)g2.ReLeavingDate ?? null
                                           }).ToList();



                        foreach (var l in lstEmployee)
                        {
                            //CALCULATE EXP IN COMPANY
                            var payroll = db.tblEmpPayRollInformations.Where(z => z.EmployeeId == l.EmployeeId).SingleOrDefault();
                            int daysDiffCompanyExp = 0;
                            int daysDiffWorkExp = 0;
                            if (payroll != null)
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


                        //filter for col emp code
                        if (!string.IsNullOrEmpty(emp.EmployeeRegisterCode) && emp.EmployeeRegisterCode != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.EmployeeRegisterCode.ToLower().Contains(emp.EmployeeRegisterCode.ToLower())).ToList();
                        }
                        //filter for col employee name
                        if (!string.IsNullOrEmpty(emp.EmployeeName) && emp.EmployeeName != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.EmployeeName.ToLower().Contains(emp.EmployeeName.ToLower())).ToList();
                        }
                        //filter for col designation
                        if (!string.IsNullOrEmpty(emp.Designation) && emp.Designation != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.Designation.ToLower().Contains(emp.Designation.ToLower())).ToList();
                        }


                        //do sorting on list
                        list = DoSorting(list, orderBy.Trim());

                        //take total count to return for ng-table
                        var Count = list.Count();

                        var resultData = new
                        {
                            total = Count,
                            result = list.Skip(iDisplayStart).Take(iDisplayLength).ToList()
                        };

                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", resultData);
                    }
                    catch (Exception ex)
                    {
                        apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
                    }
                    finally
                    {
                        list = null;
                    }
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }

        /// <summary>
        /// return sorted list based on passed column
        /// </summary>
        public List<EmployeeViewModel> DoSorting(IEnumerable<EmployeeViewModel> list, string orderBy)
        {
            try
            {
                if (orderBy == "EmployeeName")
                {
                    list = list.OrderBy(z => z.EmployeeName).ToList();
                }
                else if (orderBy == "-EmployeeName")
                {
                    list = list.OrderByDescending(z => z.EmployeeName).ToList();
                }
                if (orderBy == "EmployeeRegisterCode")
                {
                    list = list.OrderBy(z => z.EmployeeRegisterCode).ToList();
                }
                else if (orderBy == "-EmployeeRegisterCode")
                {
                    list = list.OrderByDescending(z => z.EmployeeRegisterCode).ToList();
                }
                if (orderBy == "Designation")
                {
                    list = list.OrderBy(z => z.Designation).ToList();
                }
                else if (orderBy == "-Designation")
                {
                    list = list.OrderByDescending(z => z.Designation).ToList();
                }
                else if (orderBy == "ChgDate")
                {
                    list = list.OrderBy(z => z.ChgDate).ToList();
                }
                else if (orderBy == "-ChgDate")
                {
                    list = list.OrderByDescending(z => z.ChgDate).ToList();
                }
                return list.ToList<EmployeeViewModel>();
            }
            catch
            {
                return null;
            }
        }


        // Employee View Information


        [HttpPost]
        public ApiResponse EditProfilePhoto()
        {

            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    string ProfilePhoto = nvc["ProfilePhoto"];

                    HttpContext.Current.Session["ProfilePhoto"] = ProfilePhoto;

                    int id = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                    var line = db.tblEmpPersonalInformations.Where(z => z.EmployeeId == id).SingleOrDefault();
                    if (line != null)
                    {
                        line.ProfilePhoto = ProfilePhoto;
                        line.ChgDate = DateTime.Now.ToUniversalTime();
                        line.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                    }
                    db.SaveChanges();
                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, generalMessages.msgUpdate, null);
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }
            return apiResponse;
        }


        public ApiResponse GetUserInformation()
        {
            ApiResponse apiResponse = new ApiResponse();

            if (sessionUtils.HasUserLogin())
            {
                using (db)
                {
                    int id = Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString());
                    try
                    {
                        // var list = db.tblEmpPersonalInformations.Where(z => z.EmployeeId == id).SingleOrDefault();
                        var list = (from empPersonalInfo in db.tblEmpPersonalInformations
                                    join payRollInfo in db.tblEmpPayRollInformations on empPersonalInfo.EmployeeId equals payRollInfo.EmployeeId into emp
                                    from payRoll in emp.DefaultIfEmpty()
                                    where empPersonalInfo.EmployeeId == id
                                    select new EmployeeViewProfileViewModel
                                    {
                                        EmployeeId = empPersonalInfo.EmployeeId,
                                        EmployeeRegisterCode = empPersonalInfo.EmployeeRegisterCode,
                                        CandidateFirstName = empPersonalInfo.CandidateFirstName,
                                        CandidateMiddleName = (empPersonalInfo.CandidateMiddleName.Length > 1 ? empPersonalInfo.CandidateMiddleName.Substring(0, 1) : empPersonalInfo.CandidateMiddleName),
                                        CandidateLastName = empPersonalInfo.CandidateLastName,
                                        GuardianFirstName = empPersonalInfo.GuardianFirstName,
                                        GuardianMiddleName = (empPersonalInfo.GuardianMiddleName.Length > 1 ? empPersonalInfo.GuardianMiddleName.Substring(0, 1) : empPersonalInfo.GuardianMiddleName),
                                        GuardianLastName = empPersonalInfo.GuardianLastName,
                                        ProfilePhoto = empPersonalInfo.ProfilePhoto,
                                        Present_HouseNo = empPersonalInfo.Present_HouseNo,
                                        Present_Location = empPersonalInfo.Present_Location,
                                        Present_Area = empPersonalInfo.Present_Area,
                                        Present_Country = db.tblCountries.Where(z => z.CountryId == empPersonalInfo.Present_Country).Select(z => z.CountryName).FirstOrDefault(),
                                        Present_State = db.tblStates.Where(z => z.StateId == empPersonalInfo.Present_State).Select(z => z.StateName).FirstOrDefault(),
                                        Present_City = empPersonalInfo.Present_City,
                                        Present_PostalCode = empPersonalInfo.Present_PostalCode,
                                        Permanent_HouseNo = empPersonalInfo.Permanent_HouseNo,
                                        Permanent_Location = empPersonalInfo.Permanent_Location,
                                        Permanent_Area = empPersonalInfo.Permanent_Area,
                                        Permanent_Country = db.tblCountries.Where(z => z.CountryId == empPersonalInfo.Permanent_Country).Select(z => z.CountryName).FirstOrDefault(),
                                        Permanent_State = db.tblStates.Where(z => z.StateId == empPersonalInfo.Permanent_State).Select(z => z.StateName).FirstOrDefault(),
                                        Permanent_City = empPersonalInfo.Permanent_City,
                                        Permanent_PostalCode = empPersonalInfo.Permanent_PostalCode,
                                        MaritalStatus = empPersonalInfo.MaritalStatus,
                                        MarriageAnniversaryDate = empPersonalInfo.MarriageAnniversaryDate,
                                        BirthDate = empPersonalInfo.BirthDate,
                                        Gender = empPersonalInfo.Gender,
                                        DrivingLicenceNumber = empPersonalInfo.DrivingLicenceNumber,
                                        PassportNumber = empPersonalInfo.PassportNumber,
                                        PassportExpiryDate = empPersonalInfo.PassportExpiryDate,
                                        AdharNumber = empPersonalInfo.AdharNumber,
                                        PANCardNumber = empPersonalInfo.PANCardNumber,
                                        PersonalEmailId = empPersonalInfo.PersonalEmailId,
                                        PersonalMobile = empPersonalInfo.PersonalMobile,
                                        NomineeMobile = empPersonalInfo.NomineeMobile,
                                        CompanyEmailId = empPersonalInfo.CompanyEmailId,
                                        CompanyMobile = empPersonalInfo.CompanyMobile,
                                        BloodGroup = empPersonalInfo.BloodGroup,
                                        CompanyBankAccount = payRoll.CompanyBankAccount,
                                        CompanyBankName = payRoll.CompanyBankName,
                                        JoiningDate = payRoll.JoiningDate

                                    }).SingleOrDefault();

                        //var bloodGroupId = short.Parse(list.BloodGroup);
                        //var bloodGroupName = db.tblBloodGroups.Where(bg => bg.BloodGroupId == bloodGroupId).Select(bg => bg.BloodGroupName).SingleOrDefault();
                        list.BloodGroup = list.BloodGroup == "0" ? "" : list.BloodGroup;
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", list);
                    }
                    catch (Exception ex)
                    {
                        apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
                    }
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }
            return apiResponse;
        }








        #endregion

        #region EMPLOYEE QUALIFICATION
        /// <summary>
        /// GET api/employee
        /// retrieve acedamic list 
        /// </summary>
        [HttpGet]
        public ApiResponse RetrieveAcedamic()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var list = db.tblEmpAcedamicStatus.OrderBy(z => z.AcedamicStatus)
                        .Where(z => z.IsActive == true)
                        .Select(z => new SelectItemModel
                        {
                            Id = z.AcedamicStatusId,
                            Label = z.AcedamicStatus
                        }).ToList();

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", list);
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageNameQualification, true);
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }

        /// <summary>
        /// GET api/employee
        /// retrieve degree list 
        /// </summary>
        [HttpGet]
        public ApiResponse RetrieveDegree()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int acedamicId = Convert.ToInt32(nvc["acedamicId"]);

                    var list = db.tblEmpDegrees.OrderBy(z => z.DegreeName)
                        .Where(z => z.AcedamicStatusId == acedamicId)
                        .Select(z => new SelectItemModel
                        {
                            Id = z.DegreeId,
                            Label = z.DegreeName
                        }).ToList();

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", list);
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageNameQualification, true);
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }

        /// <summary>
        /// GET api/employee
        /// retrieve discipline list 
        /// </summary>
        [HttpGet]
        public ApiResponse RetrieveDiscipline()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int degreeId = Convert.ToInt32(nvc["degreeId"]);

                    var list = db.tblEmpDisciplines.OrderBy(z => z.DisciplineName)
                        .Where(z => z.DegreeId == degreeId)
                        .Select(z => new SelectItemModel
                        {
                            Id = z.DisciplineId,
                            Label = z.DisciplineName
                        }).ToList();

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", list);
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageNameQualification, true);
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }

        /// <summary>
        /// GET api/employee
        /// retrieve university list 
        /// </summary>
        [HttpGet]
        public ApiResponse RetrieveUniversity()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var list = db.tblEmpUniversities.OrderBy(z => z.UniversityName)
                        .Where(z => z.IsActive == true)
                        .Select(z => new SelectItemModel
                        {
                            Id = z.UniversityId,
                            Label = z.UniversityName
                        }).ToList();

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", list);
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageNameQualification, true);
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }

        /// <summary>
        /// GET api/employee
        /// retrieve institute list 
        /// </summary>
        [HttpGet]
        public ApiResponse RetrieveInstitute()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int universityId = Convert.ToInt32(nvc["universityId"]);

                    var list = db.tblEmpInstitutes.OrderBy(z => z.InstituteName)
                        .Where(z => z.UniversityId == universityId)
                        .Select(z => new SelectItemModel
                        {
                            Id = z.InstituteId,
                            Label = z.InstituteName
                        }).ToList();

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", list);
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageNameQualification, true);
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }

        /// <summary>
        /// POST api/employee
        /// create and update record
        /// </summary>
        [HttpPost]
        public ApiResponse SaveEmployeeQualification(EmpQualificationViewModel emp)
        {
            GeneralMessages msg = new GeneralMessages(_pageNameQualification);
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    int daysInMonth = DateTime.DaysInMonth(emp.PassingYear, Convert.ToInt32(emp.PassingMonth));
                    DateTime dtPassingYear = new DateTime(emp.PassingYear, Convert.ToInt32(emp.PassingMonth), daysInMonth);

                    string degree = string.Empty;
                    string discipline = string.Empty;
                    string university = string.Empty;
                    string institute = string.Empty;

                    //if degree == Other
                    if (emp.Degree == -1)
                    { // insert new Degree to Degree master
                        tblEmpDegree d = new tblEmpDegree
                        {
                            AcedamicStatusId = Convert.ToInt16(emp.Acedamic),
                            DegreeName = emp.DegreeOther,
                            IsActive = emp.IsActive,
                            CreDate = DateTime.Now.ToUniversalTime(),
                            ChgDate = DateTime.Now.ToUniversalTime()
                        };
                        db.tblEmpDegrees.Add(d);
                        db.SaveChanges();
                        degree = emp.DegreeOther;
                    }
                    else
                    {
                        degree = db.tblEmpDegrees.Where(z => z.DegreeId == emp.Degree).Select(z => z.DegreeName).SingleOrDefault();
                    }

                    //if Discipline == Other
                    if (emp.Discipline == -1)
                    { // insert new Discipline to Discipline master
                        int _degree = emp.Degree != -1 ? emp.Degree : db.tblEmpDegrees.Where(z => z.DegreeName == degree).Select(z => z.DegreeId).SingleOrDefault();
                        tblEmpDiscipline d = new tblEmpDiscipline
                        {
                            DegreeId = Convert.ToInt16(_degree),
                            DisciplineName = emp.DisciplineOther,
                            IsActive = emp.IsActive,
                            CreDate = DateTime.Now.ToUniversalTime(),
                            ChgDate = DateTime.Now.ToUniversalTime()
                        };
                        db.tblEmpDisciplines.Add(d);
                        db.SaveChanges();
                        discipline = emp.DisciplineOther;
                    }
                    else
                    {
                        discipline = db.tblEmpDisciplines.Where(z => z.DisciplineId == emp.Discipline).Select(z => z.DisciplineName).SingleOrDefault();
                    }

                    //if University == Other
                    if (emp.University == -1)
                    { // insert new University to University master
                        tblEmpUniversity d = new tblEmpUniversity
                        {
                            UniversityName = emp.UniversityOther,
                            IsActive = emp.IsActive,
                            CreDate = DateTime.Now.ToUniversalTime(),
                            ChgDate = DateTime.Now.ToUniversalTime()
                        };
                        db.tblEmpUniversities.Add(d);
                        db.SaveChanges();
                        university = emp.UniversityOther;
                    }
                    else
                    {
                        university = db.tblEmpUniversities.Where(z => z.UniversityId == emp.University).Select(z => z.UniversityName).SingleOrDefault();
                    }

                    //if Institute == Other
                    if (emp.Institute == -1)
                    { // insert new Institute to Institute master
                        int _university = emp.University != -1 ? emp.University : db.tblEmpUniversities.Where(z => z.UniversityName == university).Select(z => z.UniversityId).SingleOrDefault();
                        tblEmpInstitute d = new tblEmpInstitute
                        {
                            UniversityId = Convert.ToInt16(_university),
                            InstituteName = emp.InstituteOther,
                            IsActive = emp.IsActive,
                            CreDate = DateTime.Now.ToUniversalTime(),
                            ChgDate = DateTime.Now.ToUniversalTime()
                        };
                        db.tblEmpInstitutes.Add(d);
                        db.SaveChanges();
                        institute = emp.InstituteOther;
                    }
                    else
                    {
                        institute = db.tblEmpInstitutes.Where(z => z.InstituteId == emp.Institute).Select(z => z.InstituteName).SingleOrDefault();
                    }

                    if (emp.SrNo == 0)
                    {// Mode == Add
                        tblEmpQualificationInformation e = new tblEmpQualificationInformation
                        {
                            EmployeeId = emp.EmployeeId,
                            Acedamic = db.tblEmpAcedamicStatus.Where(z => z.AcedamicStatusId == emp.Acedamic).Select(z => z.AcedamicStatus).SingleOrDefault(),
                            Degree = degree,
                            Discipline = discipline,
                            University = university,
                            Institute = institute,
                            PassingYear = dtPassingYear,
                            Percentages = emp.Percentage,
                            IsActive = emp.IsActive,
                            CreDate = DateTime.Now.ToUniversalTime(),
                            CreBy = 1,
                            ChgDate = DateTime.Now.ToUniversalTime(),
                            ChgBy = 1
                        };

                        db.tblEmpQualificationInformations.Add(e);
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, msg.msgInsert, null);
                    }
                    else
                    { // Mode == Edit
                        var line = db.tblEmpQualificationInformations.Where(z => z.SrNo == emp.SrNo).SingleOrDefault();

                        if (line != null)
                        {
                            line.EmployeeId = emp.EmployeeId;
                            line.Acedamic = db.tblEmpAcedamicStatus.Where(z => z.AcedamicStatusId == emp.Acedamic).Select(z => z.AcedamicStatus).SingleOrDefault();
                            line.Degree = degree;
                            line.Discipline = discipline;
                            line.University = university;
                            line.Institute = institute;
                            line.PassingYear = dtPassingYear;
                            line.Percentages = emp.Percentage;
                            line.IsActive = emp.IsActive;
                            line.CreDate = DateTime.Now.ToUniversalTime();
                            line.CreBy = 1;
                            line.ChgDate = DateTime.Now.ToUniversalTime();
                            line.ChgBy = 1;
                        }
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, msg.msgUpdate, null);
                    }
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageNameQualification, true);
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }

        /// <summary>
        /// POST api/employee
        /// delete employee qualification
        /// </summary>
        [HttpPost]
        public ApiResponse DeleteEmployeeQualification([FromBody]int id)
        {
            GeneralMessages msg = new GeneralMessages(_pageNameQualification);
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var line = db.tblEmpQualificationInformations.Where(z => z.SrNo == id).SingleOrDefault();
                    if (line != null)
                    {
                        db.tblEmpQualificationInformations.Remove(line);
                    }

                    db.SaveChanges();
                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, msg.msgDelete, null);
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageNameQualification, true);
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }

        /// <summary>
        /// POST api/employee
        /// active-inActive record 
        /// </summary>
        [HttpPost]
        public ApiResponse ChangeStatusEmpQualification(tblEmpQualificationInformation emp)
        {
            GeneralMessages msg = new GeneralMessages(_pageNameQualification);
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var line = db.tblEmpQualificationInformations.Where(z => z.SrNo == emp.SrNo).SingleOrDefault();
                    if (line != null)
                    {
                        if (emp.IsActive)
                        {
                            line.IsActive = false;
                        }
                        else if (!emp.IsActive)
                        {
                            line.IsActive = true;
                        }
                    }

                    line.ChgDate = DateTime.Now.ToUniversalTime();
                    db.SaveChanges();
                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, msg.msgChangeStatus, null);
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageNameQualification, true);
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }

        /// <summary>
        /// GET api/employee
        /// return employee list with sorting and filtering  functionalities
        /// </summary>
        [HttpPost]
        public ApiResponse GetEmployeeQualificationList()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                using (db)
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int page = Convert.ToInt32(nvc["page"]);
                    int timezone = Convert.ToInt32(nvc["timezone"]);
                    string orderBy = nvc["orderby"];
                    string filter = nvc["filter"];
                    int employeeId = Convert.ToInt32(nvc["employeeId"]);


                    int iDisplayLength = Convert.ToInt32(nvc["count"]);
                    int iDisplayStart = (page - 1) * iDisplayLength;

                    List<EmpQualificationViewModel> list = new List<EmpQualificationViewModel>();
                    try
                    {
                        var lines = db.tblEmpQualificationInformations.ToList();
                        if (employeeId > 0)
                        {
                            lines = lines.Where(z => z.EmployeeId == employeeId).ToList();
                        }
                        foreach (var l in lines)
                        {
                            var AcedamicStatusId = db.tblEmpAcedamicStatus.Where(z => z.AcedamicStatus == l.Acedamic).Select(z => z.AcedamicStatusId).FirstOrDefault();
                            var DegreeId = db.tblEmpDegrees.Where(z => z.DegreeName == l.Degree && z.AcedamicStatusId == AcedamicStatusId).Select(z => z.DegreeId).FirstOrDefault();
                            var DisciplineId = db.tblEmpDisciplines.Where(z => z.DisciplineName == l.Discipline && z.DegreeId == DegreeId).Select(z => z.DisciplineId).FirstOrDefault();
                            var UniversityId = db.tblEmpUniversities.Where(z => z.UniversityName == l.University).Select(z => z.UniversityId).FirstOrDefault();
                            var InstituteId = db.tblEmpInstitutes.Where(z => z.InstituteName == l.Institute && z.UniversityId == UniversityId).Select(z => z.InstituteId).FirstOrDefault();

                            EmpQualificationViewModel f = new EmpQualificationViewModel
                            {
                                SrNo = l.SrNo,
                                EmployeeId = l.EmployeeId,
                                Acedamic = AcedamicStatusId,
                                AcedamicName = l.Acedamic,
                                Degree = DegreeId,
                                DegreeName = l.Degree,
                                Discipline = DisciplineId,
                                DisciplineName = l.Discipline,
                                University = UniversityId,
                                UniversityName = l.University,
                                Institute = InstituteId,
                                InstituteName = l.Institute,
                                PassingMonth = l.PassingYear.ToString("MMMM"),
                                PassingMonthDigit = l.PassingYear.Month,
                                PassingYear = l.PassingYear.Year,
                                Percentage = l.Percentages,
                                ChgDate = Convert.ToDateTime(l.ChgDate).AddMinutes(-1 * timezone),
                                IsActive = l.IsActive
                            };
                            list.Add(f);
                        }

                        //filter for col emp code
                        if (!string.IsNullOrEmpty(filter) && filter != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.AcedamicName.ToLower().Contains(filter.ToLower())).ToList();
                        }

                        //do sorting on list
                        list = DoSortingForQualification(list, orderBy.Trim());

                        //take total count to return for ng-table
                        var Count = list.Count();


                        var resultData = new
                        {
                            total = Count,
                            result = list.Skip(iDisplayStart).Take(iDisplayLength).ToList()
                        };

                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", resultData);
                    }
                    catch (Exception ex)
                    {
                        apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageNameQualification, true);
                    }
                    finally
                    {
                        list = null;
                    }
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }

        /// <summary>
        /// return sorted list based on passed column
        /// </summary>
        public List<EmpQualificationViewModel> DoSortingForQualification(IEnumerable<EmpQualificationViewModel> list, string orderBy)
        {
            try
            {
                if (orderBy == "AcedamicName")
                {
                    list = list.OrderBy(z => z.Acedamic).ToList();
                }
                else if (orderBy == "-AcedamicName")
                {
                    list = list.OrderByDescending(z => z.Acedamic).ToList();
                }
                if (orderBy == "DegreeName")
                {
                    list = list.OrderBy(z => z.Degree).ToList();
                }
                else if (orderBy == "-DegreeName")
                {
                    list = list.OrderByDescending(z => z.Degree).ToList();
                }
                else if (orderBy == "ChgDate")
                {
                    list = list.OrderBy(z => z.ChgDate).ToList();
                }
                else if (orderBy == "-ChgDate")
                {
                    list = list.OrderByDescending(z => z.ChgDate).ToList();
                }
                return list.ToList<EmpQualificationViewModel>();
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region WORK EXPERIENCE
        /// <summary>
        /// Create or Update Employee Work Experience
        /// </summary>
        /// <param name="workExperience"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResponse CreateUpdateWorkExperience(tblEmpWorkExperience workExperience)
        {
            GeneralMessages msg = new GeneralMessages(_pageNameWorkExp);
            ApiResponse apiResponse = new ApiResponse();
            _pageName = "Employee Work Experience";
            if (sessionUtils.HasUserLogin())
            {
                using (db)
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int timezone = Convert.ToInt32(nvc["timezone"]);
                    try
                    {
                        // ADD MODE
                        if (workExperience.SrNo == 0)
                        {
                            tblEmpWorkExperience tbl = new tblEmpWorkExperience
                            {
                                EmployeeId = workExperience.EmployeeId,
                                CompanyName = workExperience.CompanyName,
                                Designation = workExperience.Designation,
                                Skills = workExperience.Skills,
                                FromDate = workExperience.FromDate.AddMinutes(-1 * timezone),
                                ToDate = workExperience.ToDate.AddMinutes(-1 * timezone),
                                Salary = workExperience.Salary,
                                Comments = workExperience.Comments,
                                IsActive = true,
                                CreBy = Convert.ToInt32(HttpContext.Current.Session["UserId"]),
                                CreDate = DateTime.Now.ToUniversalTime(),
                                ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"]),
                                ChgDate = DateTime.Now.ToUniversalTime()
                            };
                            db.tblEmpWorkExperiences.Add(tbl);
                            apiResponse = ERPUtilities.GenerateApiResponse(true, 1, msg.msgInsert, null);
                        }
                        // EDIT MODE
                        else
                        {
                            tblEmpWorkExperience tbl = db.tblEmpWorkExperiences.Where(z => z.SrNo == workExperience.SrNo).FirstOrDefault();
                            tbl.EmployeeId = workExperience.EmployeeId;
                            tbl.CompanyName = workExperience.CompanyName;
                            tbl.Designation = workExperience.Designation;
                            tbl.Skills = workExperience.Skills;
                            tbl.FromDate = workExperience.FromDate.AddMinutes(-1 * timezone);
                            tbl.ToDate = workExperience.ToDate.AddMinutes(-1 * timezone);
                            tbl.Salary = workExperience.Salary;
                            tbl.Comments = workExperience.Comments;
                            tbl.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"]);
                            tbl.ChgDate = DateTime.Now.ToUniversalTime();
                            apiResponse = ERPUtilities.GenerateApiResponse(true, 1, msg.msgUpdate, null);
                        }
                        db.SaveChanges();

                    }
                    catch (Exception ex)
                    {
                        apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageNameWorkExp, true);
                    }
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }

        /// <summary>
        /// Retrieves list of Technologies
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiResponse RetriveTechnology()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                using (db)
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    string searchText = nvc["searchText"];

                    List<string> list = new List<string>();
                    List<string> list1 = new List<string>();
                    try
                    {
                        list = db.tblTechnologies.Where(z => z.Technologies.Contains(searchText) && z.IsActive == true).Select(z => z.Technologies).ToList();

                        if (list != null)
                        {
                            foreach (string a in list)
                            {
                                var tech = a.Replace(" ", "").ToString();
                                list1.Add(tech);
                            }
                        }

                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", list1);
                    }
                    catch (Exception ex)
                    {
                        apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageNameWorkExp, true);
                    }
                    finally
                    {
                        list = null;
                    }
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }

        /// <summary>
        /// Retrives list of Employee Work Experience
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiResponse RetrieveEmpWorkExperience()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                using (db)
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int employeeId = Convert.ToInt16(nvc["employeeId"]);
                    int page = Convert.ToInt32(nvc["page"]);
                    int timezone = Convert.ToInt32(nvc["timezone"]);
                    string orderBy = nvc["orderby"];
                    string employeeName = nvc["employeeName"];
                    string companyName = nvc["companyName"];
                    string designation = nvc["designation"];
                    string skills = nvc["skills"];

                    int iDisplayLength = Convert.ToInt32(nvc["count"]);
                    int iDisplayStart = (page - 1) * iDisplayLength;

                    IEnumerable<tblEmpWorkExperience> list = null;
                    try
                    {
                        list = db.tblEmpWorkExperiences.Where(z => z.EmployeeId == employeeId).ToList();

                        // FILTERING DATA ON BASIS OF COMPANY NAME
                        if (!string.IsNullOrEmpty(companyName) && companyName != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.CompanyName.ToLower().Contains(companyName.ToLower())).ToList();
                        }

                        // FILTERING DATA ON BASIS OF DESIGNATION
                        if (!string.IsNullOrEmpty(designation) && designation != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.Designation.ToLower().Contains(designation.ToLower())).ToList();
                        }

                        // FILTERING DATA ON BASIS OF SKILLS
                        if (!string.IsNullOrEmpty(skills) && skills != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.Skills.ToLower().Contains(skills.ToLower())).ToList();
                        }

                        // SORTING DATA
                        list = DoSortingForEmpWorkExperience(list, orderBy.Trim());

                        // TAKE TOTAL COUNT TO RETURN FOR NG-TABLE
                        var Count = list.Count();

                        // CONVERT RETURNED DATETIME TO LOCAL TIMEZONE
                        list = list.Select(i =>
                        {
                            i.CreDate = Convert.ToDateTime(i.CreDate).AddMinutes(-1 * timezone);
                            i.ChgDate = Convert.ToDateTime(i.ChgDate).AddMinutes(-1 * timezone);
                            return i;
                        }).Skip(iDisplayStart).Take(iDisplayLength).ToList();

                        var resultData = new
                        {
                            total = Count,
                            result = list.ToList()
                        };

                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", resultData);
                    }
                    catch (Exception ex)
                    {
                        apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageNameWorkExp, true);
                    }
                    finally
                    {
                        list = null;
                    }
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }
            return apiResponse;
        }

        /// <summary>
        /// Delete specified Employee Work Experience
        /// </summary>
        /// <param name="detail"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResponse DeleteEmpWorkExperience(tblEmpWorkExperience detail)
        {
            GeneralMessages msg = new GeneralMessages(_pageNameWorkExp);
            ApiResponse apiResponse = new ApiResponse();
            _pageName = "Employee Work Experience";
            if (sessionUtils.HasUserLogin())
            {
                using (db)
                {

                    tblEmpWorkExperience tbl = null;
                    try
                    {
                        tbl = db.tblEmpWorkExperiences.Where(z => z.SrNo == detail.SrNo).FirstOrDefault();
                        db.tblEmpWorkExperiences.Remove(tbl);
                        db.SaveChanges();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, msg.msgDelete, null);
                    }
                    catch (Exception ex)
                    {
                        apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageNameWorkExp, true);
                    }
                    finally
                    {
                        tbl = null;
                    }
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }
            return apiResponse;
        }

        /// <summary>
        /// Change status of specified Employee Work Experience
        /// </summary>
        /// <param name="detail"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResponse ChangeIsActive(tblEmpWorkExperience detail)
        {
            GeneralMessages msg = new GeneralMessages(_pageNameWorkExp);
            ApiResponse apiResponse = new ApiResponse();
            _pageName = "Employee Work Experience";
            if (sessionUtils.HasUserLogin())
            {
                using (db)
                {
                    tblEmpWorkExperience tbl = null;
                    try
                    {
                        tbl = db.tblEmpWorkExperiences.Where(z => z.SrNo == detail.SrNo).FirstOrDefault();
                        if (tbl != null)
                        {
                            if (tbl.IsActive)
                                tbl.IsActive = false;
                            else if (!tbl.IsActive)
                                tbl.IsActive = true;
                        }
                        tbl.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"]);
                        tbl.ChgDate = DateTime.Now.ToUniversalTime();
                        db.SaveChanges();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, msg.msgChangeStatus, null);

                    }
                    catch (Exception ex)
                    {
                        apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageNameWorkExp, true);
                    }
                    finally
                    {
                        tbl = null;
                    }
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }
            return apiResponse;
        }

        /// <summary>
        /// Return sorted list based on passed column
        /// </summary>
        public IEnumerable<tblEmpWorkExperience> DoSortingForEmpWorkExperience(IEnumerable<tblEmpWorkExperience> list, string orderBy)
        {
            try
            {
                if (orderBy == "CompanyName")
                {
                    list = list.OrderBy(z => z.CompanyName).ToList();
                }
                else if (orderBy == "-CompanyName")
                {
                    list = list.OrderByDescending(z => z.CompanyName).ToList();
                }
                else if (orderBy == "Designation")
                {
                    list = list.OrderBy(z => z.Designation).ToList();
                }
                else if (orderBy == "-Designation")
                {
                    list = list.OrderByDescending(z => z.Designation).ToList();
                }
                else if (orderBy == "Skills")
                {
                    list = list.OrderBy(z => z.Skills).ToList();
                }
                else if (orderBy == "-Skills")
                {
                    list = list.OrderByDescending(z => z.Skills).ToList();
                }
                else if (orderBy == "FromDate")
                {
                    list = list.OrderBy(z => z.FromDate).ToList();
                }
                else if (orderBy == "-FromDate")
                {
                    list = list.OrderByDescending(z => z.FromDate).ToList();
                }
                else if (orderBy == "ToDate")
                {
                    list = list.OrderBy(z => z.ToDate).ToList();
                }
                else if (orderBy == "-ToDate")
                {
                    list = list.OrderByDescending(z => z.ToDate).ToList();
                }
                else if (orderBy == "Salary")
                {
                    list = list.OrderBy(z => z.Salary).ToList();
                }
                else if (orderBy == "-Salary")
                {
                    list = list.OrderByDescending(z => z.Salary).ToList();
                }
                else if (orderBy == "ChgDate")
                {
                    list = list.OrderBy(z => z.ChgDate).ToList();
                }
                else if (orderBy == "-ChgDate")
                {
                    list = list.OrderByDescending(z => z.ChgDate).ToList();
                }
                return list;
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region EMPLOYEE DOCUMENT

        /// <summary>
        /// Create Or Update Employee Document
        /// </summary>
        /// <param name="emp"></param>
        /// <returns></returns>

        [HttpPost]
        public ApiResponse SaveEmployeeDocument(tblEmpDocument empDoc)
        {
            GeneralMessages msg = new GeneralMessages(_pageNameDocument);
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (empDoc.SrNo == 0)
                    {// Mode == Add
                        var cnt = db.tblEmpDocuments.Where(z => z.EmployeeId == empDoc.EmployeeId && z.DocumentId == empDoc.DocumentId).Count();
                        if (cnt > 0)
                        {
                            return apiResponse = ERPUtilities.GenerateApiResponse(true, 2, "Document already exists. Please add another Document.", null);
                        }
                        tblEmpDocument emp = new tblEmpDocument
                        {
                            EmployeeId = empDoc.EmployeeId,
                            DocumentId = empDoc.DocumentId,
                            FileName = empDoc.FileName,
                            IsActive = true,
                            CreDate = DateTime.Now.ToUniversalTime(),
                            CreBy = Convert.ToInt16(HttpContext.Current.Session["UserId"]),
                            ChgDate = DateTime.Now.ToUniversalTime(),
                            ChgBy = Convert.ToInt16(HttpContext.Current.Session["UserId"])
                        };
                        db.tblEmpDocuments.Add(emp);
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, msg.msgInsert, null);
                    }
                    else
                    {// Mode == Edit
                        var cnt = db.tblEmpDocuments.Where(z => z.EmployeeId == empDoc.EmployeeId && z.DocumentId == empDoc.DocumentId && z.SrNo != empDoc.SrNo).Count();
                        if (cnt > 0)
                        {
                            return apiResponse = ERPUtilities.GenerateApiResponse(true, 2, "Document already exists. Please add another Document.", null);
                        }

                        var line = db.tblEmpDocuments.Where(z => z.SrNo == empDoc.SrNo).SingleOrDefault();

                        //delete profile picture
                        if (line.FileName != empDoc.FileName)
                        {
                            DeleteDocument(line.FileName);
                        }

                        if (line != null)
                        {
                            line.EmployeeId = empDoc.EmployeeId;
                            line.DocumentId = empDoc.DocumentId;
                            line.FileName = empDoc.FileName;
                            line.IsActive = empDoc.IsActive;
                            line.ChgDate = DateTime.Now.ToUniversalTime();
                            line.ChgBy = Convert.ToInt16(HttpContext.Current.Session["UserId"]);
                        }
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, msg.msgUpdate, null);
                    }

                    if (Path.GetExtension(empDoc.FileName) == ".pdf")
                    {
                        var source = HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["TempUploadPath"].ToString()) + "/" + empDoc.FileName;
                        var destination = HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["empPDFUploads"].ToString()) + "/" + empDoc.FileName;
                        System.IO.File.Move(source, destination);
                    }
                    else
                    {
                        // move employee document file after add or edit
                        if (System.IO.File.Exists(HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["TempThumbnails"].ToString()) + "/" + empDoc.FileName))
                        {
                            var source = HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["TempThumbnails"].ToString()) + "/" + empDoc.FileName;
                            var destination = HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["empDocThumbnails"].ToString()) + "/" + empDoc.FileName;

                            var ThumbSource = HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["TempUploadPath"].ToString()) + "/" + empDoc.FileName;
                            var ThumbDestination = HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["empDocUploads"].ToString()) + "/" + empDoc.FileName;

                            System.IO.File.Move(source, destination);
                            System.IO.File.Move(ThumbSource, ThumbDestination);
                        }

                    }

                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageNameDocument, true);
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }
            return apiResponse;
        }


        /// <summary>
        /// GET api/employee
        /// fetch employee 
        /// </summary>

        //public ApiResponse GetEmpDocumentList()
        //{
        //    ApiResponse apiResponse = new ApiResponse();
        //    if (sessionUtils.HasUserLogin())
        //    {
        //        using (db)
        //        {

        //            NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
        //            int page = Convert.ToInt32(nvc["page"]);
        //            int timezone = Convert.ToInt32(nvc["timezone"]);
        //            string orderBy = nvc["orderby"];

        //            int employeeId = Convert.ToInt32(nvc["employeeId"]);
        //            int iDisplayLength = Convert.ToInt32(nvc["count"]);
        //            int iDisplayStart = (page - 1) * iDisplayLength;

        //            IEnumerable<tblEmpDocument> list = null;

        //            try
        //            {
        //                list = db.tblEmpDocuments.Where(z => z.EmployeeId == employeeId).ToList();

        //                foreach (var item in list)
        //                {
        //                    item.tblDocument = db.tblDocuments.Where(z => z.Id == item.DocumentId).FirstOrDefault();
        //                }
        //                // SORTING DATA
        //                list = DoSorting(list, orderBy.Trim());

        //                // TAKE TOTAL COUNT TO RETURN FOR NG-TABLE
        //                var Count = list.Count();

        //                // CONVERT RETURNED DATETIME TO LOCAL TIMEZONE
        //                list = list.Select(i =>
        //                {
        //                    i.CreDate = Convert.ToDateTime(i.CreDate).AddMinutes(-1 * timezone);
        //                    i.ChgDate = Convert.ToDateTime(i.ChgDate).AddMinutes(-1 * timezone);
        //                    return i;
        //                }).Skip(iDisplayStart).Take(iDisplayLength).ToList();

        //                var resultData = new
        //                {
        //                    total = Count,
        //                    result = list.ToList()
        //                };
        //                apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", list);
        //            }
        //            catch (Exception ex)
        //            {
        //                apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageNameDocument, true);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        apiResponse = ERPUtilities.GenerateApiResponse();
        //    }

        //    return apiResponse;
        //}
        [HttpPost]
        public ApiResponse GetEmpDocumentList()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                using (db)
                {

                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int timezone = Convert.ToInt32(nvc["timezone"]);
                    int employeeId = Convert.ToInt32(nvc["employeeId"]);

                    IEnumerable<tblEmpDocument> list = null;

                    try
                    {
                        list = db.tblEmpDocuments.Where(z => z.EmployeeId == employeeId).ToList();

                        foreach (var item in list)
                        {
                            item.tblDocument = db.tblDocuments.Where(z => z.Id == item.DocumentId).FirstOrDefault();
                        }

                        // TAKE TOTAL COUNT TO RETURN FOR NG-TABLE
                        var Count = list.Count();

                        // CONVERT RETURNED DATETIME TO LOCAL TIMEZONE
                        list = list.Select(i =>
                        {
                            i.CreDate = Convert.ToDateTime(i.CreDate).AddMinutes(-1 * timezone);
                            i.ChgDate = Convert.ToDateTime(i.ChgDate).AddMinutes(-1 * timezone);

                            return i;
                        }).ToList();

                        var resultData = new
                        {
                            total = Count,
                            result = list.ToList()
                        };
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", list);
                    }
                    catch (Exception ex)
                    {
                        apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageNameDocument, true);
                    }
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }

        /// <summary>
        /// Return sorted list based on passed column
        /// </summary>
        public IEnumerable<tblEmpDocument> DoSorting(IEnumerable<tblEmpDocument> list, string orderBy)
        {
            try
            {
                // SETTING ORDER ON CATEGORYNAME
                if (orderBy == "Documents")
                {
                    list = list.OrderBy(z => z.tblDocument.Documents);
                }
                else if (orderBy == "-Documents")
                {
                    list = list.OrderByDescending(z => z.tblDocument.Documents);
                }

                else if (orderBy == "ChgDate")
                {
                    list = list.OrderBy(z => z.ChgDate).ToList();
                }
                else if (orderBy == "-ChgDate")
                {
                    list = list.OrderByDescending(z => z.ChgDate).ToList();
                }
                return list;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// POST api/employee
        /// delete employee
        /// </summary>
        [HttpPost]
        public ApiResponse DeleteEmpDoc([FromBody]int id)
        {
            GeneralMessages msg = new GeneralMessages(_pageNameDocument);
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var line = db.tblEmpDocuments.Where(z => z.SrNo == id).SingleOrDefault();
                    if (line != null)
                    {
                        db.tblEmpDocuments.Remove(line);
                    }
                    db.SaveChanges();

                    if (Path.GetExtension(line.FileName) == ".pdf")
                    {
                        //delete pdf document
                        DeletePDFDocument(line.FileName);
                    }
                    else
                    {
                        //delete employee document
                        DeleteDocument(line.FileName);
                    }

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, msg.msgDelete, null);
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageNameDocument, true);
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }

        /// <summary>
        /// POST api/employee
        /// active-inActive record 
        /// </summary>
        [HttpPost]
        public ApiResponse ChangeDocStatus(tblEmpDocument empDoc)
        {
            GeneralMessages msg = new GeneralMessages(_pageNameDocument);
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var line = db.tblEmpDocuments.Where(z => z.SrNo == empDoc.SrNo).SingleOrDefault();
                    if (line != null)
                    {
                        if (empDoc.IsActive)
                        {
                            line.IsActive = false;
                        }
                        else if (!empDoc.IsActive)
                        {
                            line.IsActive = true;
                        }
                    }

                    line.ChgDate = DateTime.Now.ToUniversalTime();
                    db.SaveChanges();
                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, msg.msgChangeStatus, null);
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageNameDocument, true);
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }

        #endregion

        #region RELATIVE INFORMATION
        /// <summary>
        /// Retrieves list of Relations order by Relation Name
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiResponse RetrieveRelations()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var list = db.tblEmpRelativeRelations.OrderBy(z => z.RelativeRelationName).Where(z => z.IsActive == true).OrderBy(z => z.RelativeRelationName).ToList();
                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", list);
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageNameRelativeInfo, true);
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }

        /// <summary>
        /// Create or Update Employee Relative Information
        /// </summary>
        /// <param name="empRelativeInfo"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResponse CreateUpdateRelativeInfo(EmpRelativeInfoViewModel empRelativeInfo)
        {
            GeneralMessages msg = new GeneralMessages(_pageNameRelativeInfo);
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    string relation = string.Empty;
                    string degree = string.Empty;
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int timezone = Convert.ToInt32(nvc["timezone"]);

                    // IF RELATIONID == -1
                    // INSERTING NEW RELATION TO ITS MASTER
                    if (empRelativeInfo.RelationId == -1)
                    {
                        try
                        {
                            tblEmpRelativeRelation tbl = new tblEmpRelativeRelation
                            {
                                RelativeRelationName = empRelativeInfo.RelativeRelationNameOther,
                                IsActive = true,
                                CreDate = DateTime.Now.ToUniversalTime(),
                                ChgDate = DateTime.Now.ToUniversalTime()
                            };
                            db.tblEmpRelativeRelations.Add(tbl);
                            db.SaveChanges();
                            relation = empRelativeInfo.RelativeRelationNameOther;
                        }
                        catch (Exception ex)
                        {
                            apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageNameRelativeInfo, true);
                        }
                    }
                    else
                    {
                        relation = db.tblEmpRelativeRelations.Where(z => z.RelationId == empRelativeInfo.RelationId).Select(z => z.RelativeRelationName).SingleOrDefault();
                    }

                    // IF DEGREE == -1
                    // INSERTING NEW DEGREE TO ITS MASTER
                    if (empRelativeInfo.DegreeId == -1)
                    {
                        try
                        {
                            tblEmpDegree d = new tblEmpDegree
                            {
                                AcedamicStatusId = Convert.ToInt16(empRelativeInfo.AcedamicStatusId),
                                DegreeName = empRelativeInfo.DegreeNameOther,
                                IsActive = true,
                                CreDate = DateTime.Now.ToUniversalTime(),
                                ChgDate = DateTime.Now.ToUniversalTime()
                            };
                            db.tblEmpDegrees.Add(d);
                            db.SaveChanges();
                            degree = empRelativeInfo.DegreeNameOther;
                        }
                        catch (Exception ex)
                        {
                            apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageNameRelativeInfo, true);
                        }
                    }
                    else
                    {
                        degree = db.tblEmpDegrees.Where(z => z.DegreeId == empRelativeInfo.DegreeId).Select(z => z.DegreeName).SingleOrDefault();
                    }

                    // ADD MODE
                    if (empRelativeInfo.SrNo == 0)
                    {
                        tblEmpRelativeInformation tbl = new tblEmpRelativeInformation
                        {
                            EmployeeId = empRelativeInfo.EmployeeId,
                            RelativeName = empRelativeInfo.RelativeName,
                            RelativeRelation = relation,
                            BirthDate = empRelativeInfo.BirthDate.AddMinutes(-1 * timezone),
                            Acedamic = db.tblEmpAcedamicStatus.Where(z => z.AcedamicStatusId == empRelativeInfo.AcedamicStatusId).Select(z => z.AcedamicStatus).SingleOrDefault(),
                            Degree = degree,
                            TypeOfWork = empRelativeInfo.TypeOfWork,
                            CreDate = DateTime.Now.ToUniversalTime(),
                            CreBy = 1,
                            ChgDate = DateTime.Now.ToUniversalTime(),
                            ChgBy = 1
                        };
                        db.tblEmpRelativeInformations.Add(tbl);
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, msg.msgInsert, null);
                    }
                    // EDIT MODE
                    else
                    {
                        tblEmpRelativeInformation tbl = db.tblEmpRelativeInformations.Where(z => z.SrNo == empRelativeInfo.SrNo).SingleOrDefault();
                        if (tbl != null)
                        {
                            tbl.EmployeeId = empRelativeInfo.EmployeeId;
                            tbl.RelativeName = empRelativeInfo.RelativeName;
                            tbl.RelativeRelation = relation;
                            tbl.BirthDate = empRelativeInfo.BirthDate.AddMinutes(-1 * timezone);
                            tbl.Acedamic = db.tblEmpAcedamicStatus.Where(z => z.AcedamicStatusId == empRelativeInfo.AcedamicStatusId).Select(z => z.AcedamicStatus).SingleOrDefault();
                            tbl.Degree = degree;
                            tbl.TypeOfWork = empRelativeInfo.TypeOfWork;
                            tbl.ChgDate = DateTime.Now.ToUniversalTime();
                            tbl.ChgBy = 1;
                        }
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, msg.msgUpdate, null);
                    }
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageNameRelativeInfo, true);
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }

        /// <summary>
        /// Retrives list of Employee Relatives Information
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiResponse RetrieveRelativeInfo()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                using (db)
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int page = Convert.ToInt32(nvc["page"]);
                    int timezone = Convert.ToInt32(nvc["timezone"]);
                    string orderBy = nvc["orderby"];

                    int employeeId = Convert.ToInt32(nvc["employeeId"]);
                    string relativeName = nvc["relativeName"];
                    string relation = nvc["relation"];
                    string acedamicStatus = nvc["acedamicStatus"];
                    string degreeName = nvc["degreeName"];
                    string typeOfWork = nvc["typeOfWork"];

                    int iDisplayLength = Convert.ToInt32(nvc["count"]);
                    int iDisplayStart = (page - 1) * iDisplayLength;

                    List<EmpRelativeInfoViewModel> list = new List<EmpRelativeInfoViewModel>();
                    try
                    {
                        var lines = db.tblEmpRelativeInformations.ToList();
                        if (employeeId > 0)
                        {
                            lines = lines.Where(z => z.EmployeeId == employeeId).ToList();
                        }
                        foreach (var l in lines)
                        {
                            EmpRelativeInfoViewModel vm = new EmpRelativeInfoViewModel
                            {
                                EmployeeId = l.EmployeeId,
                                SrNo = l.SrNo,
                                RelativeName = l.RelativeName,
                                RelationId = db.tblEmpRelativeRelations.Where(z => z.RelativeRelationName == l.RelativeRelation).Select(z => z.RelationId).FirstOrDefault(),
                                RelativeRelationName = l.RelativeRelation,
                                BirthDate = l.BirthDate,
                                AcedamicStatusId = db.tblEmpAcedamicStatus.Where(z => z.AcedamicStatus == l.Acedamic).Select(z => z.AcedamicStatusId).FirstOrDefault(),
                                AcedamicStatus = l.Acedamic,
                                DegreeId = db.tblEmpDegrees.Where(z => z.DegreeName == l.Degree).Select(z => z.DegreeId).FirstOrDefault(),
                                DegreeName = l.Degree,
                                TypeOfWork = l.TypeOfWork,
                                ChgDate = Convert.ToDateTime(l.ChgDate).AddMinutes(-1 * timezone)
                            };
                            list.Add(vm);
                        }

                        // FILTERING DATA
                        if (!string.IsNullOrEmpty(relativeName) && relativeName != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.RelativeName.ToLower().Contains(relativeName.ToLower())).ToList();
                        }
                        if (!string.IsNullOrEmpty(relation) && relation != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.RelativeRelationName.ToLower().Contains(relation.ToLower())).ToList();
                        }
                        if (!string.IsNullOrEmpty(acedamicStatus) && acedamicStatus != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.AcedamicStatus.ToLower().Contains(acedamicStatus.ToLower())).ToList();
                        }
                        if (!string.IsNullOrEmpty(degreeName) && degreeName != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.DegreeName.ToLower().Contains(degreeName.ToLower())).ToList();
                        }
                        if (!string.IsNullOrEmpty(typeOfWork) && typeOfWork != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.TypeOfWork.ToLower().Contains(typeOfWork.ToLower())).ToList();
                        }

                        // PERFORM SORTING
                        list = DoSortingForRelative(list, orderBy.Trim());

                        // GET COUNT
                        var Count = list.Count();

                        // SET RESULT
                        var resultData = new
                        {
                            total = Count,
                            result = list.Skip(iDisplayStart).Take(iDisplayLength).ToList()
                        };

                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", resultData);
                    }
                    catch (Exception ex)
                    {
                        apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageNameRelativeInfo, true);
                    }
                    finally
                    {
                        list = null;
                    }
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }

        /// <summary>
        /// Delete specific Employee Relative Information
        /// </summary>
        /// <param name="empRelativeInfo"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResponse DeleteEmpRelativeInfo(EmpRelativeInfoViewModel empRelativeInfo)
        {
            GeneralMessages msg = new GeneralMessages(_pageNameRelativeInfo);
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    tblEmpRelativeInformation tbl = db.tblEmpRelativeInformations.Where(z => z.SrNo == empRelativeInfo.SrNo).SingleOrDefault();
                    if (tbl != null)
                    {
                        db.tblEmpRelativeInformations.Remove(tbl);
                    }
                    db.SaveChanges();
                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, msg.msgDelete, null);
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageNameRelativeInfo, true);
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }

        /// <summary>
        /// Return sorted list based on passed column
        /// </summary>
        public List<EmpRelativeInfoViewModel> DoSortingForRelative(List<EmpRelativeInfoViewModel> list, string orderBy)
        {
            try
            {
                if (orderBy == "RelativeName")
                {
                    list = list.OrderBy(z => z.RelativeName).ToList();
                }
                else if (orderBy == "-RelativeName")
                {
                    list = list.OrderByDescending(z => z.RelativeName).ToList();
                }

                else if (orderBy == "Relation")
                {
                    list = list.OrderBy(z => z.RelativeRelationName).ToList();
                }
                else if (orderBy == "-Relation")
                {
                    list = list.OrderByDescending(z => z.RelativeRelationName).ToList();
                }

                else if (orderBy == "BirthDate")
                {
                    list = list.OrderBy(z => z.BirthDate).ToList();
                }
                else if (orderBy == "-BirthDate")
                {
                    list = list.OrderByDescending(z => z.BirthDate).ToList();
                }

                else if (orderBy == "AcedamicStatus")
                {
                    list = list.OrderBy(z => z.AcedamicStatus).ToList();
                }
                else if (orderBy == "-AcedamicStatus")
                {
                    list = list.OrderByDescending(z => z.AcedamicStatus).ToList();
                }

                else if (orderBy == "DegreeName")
                {
                    list = list.OrderBy(z => z.DegreeName).ToList();
                }
                else if (orderBy == "-DegreeName")
                {
                    list = list.OrderByDescending(z => z.DegreeName).ToList();
                }

                else if (orderBy == "TypeOfWork")
                {
                    list = list.OrderBy(z => z.TypeOfWork).ToList();
                }
                else if (orderBy == "-TypeOfWork")
                {
                    list = list.OrderByDescending(z => z.TypeOfWork).ToList();
                }

                else if (orderBy == "ChgDate")
                {
                    list = list.OrderBy(z => z.ChgDate).ToList();
                }
                else if (orderBy == "-ChgDate")
                {
                    list = list.OrderByDescending(z => z.ChgDate).ToList();
                }
                return list;
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region PAYROLL INFORMATION

        /// <summary>
        /// POST api/employee
        /// create and update record
        /// </summary>
        [HttpPost]
        public ApiResponse SavePayRollInformation(EmpPayRollViewModel payroll)
        {
            GeneralMessages msg = new GeneralMessages(_pageNamePayRoll);
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    int retPayRollId = payroll.PayRollId;
                    if (payroll.PayRollId == 0)
                    {// Mode == Add
                        tblEmpPayRollInformation p = new tblEmpPayRollInformation
                        {
                            EmployeeId = payroll.EmployeeId,
                            CTC = payroll.CTC,
                            BasicSalary = payroll.BasicSalary,
                            EmploymentTax = payroll.EmploymentTax,
                            ESIC = payroll.ESIC,
                            LeavesAllowedPerYear = payroll.LeavesAllowedPerYear,
                            PFAccountNumber = payroll.PFAccountNumber,
                            PF = payroll.PF,
                            CompanyBankName = payroll.CompanyBankName,
                            CompanyBankAccount = payroll.CompanyBankAccount,
                            PersonalBankName = payroll.PersonalBankName,
                            PersonalBankAccount = payroll.PersonalBankAccount,
                            AllocatedPassNo = payroll.AllocatedPassNo,
                            JoiningDate = payroll.JoiningDate,
                            ReLeavingDate = payroll.ReLeavingDate,
                            EmploymentStatus = payroll.EmploymentStatus,
                            PermanentFromDate = payroll.PermanentFromDate,
                            GName = payroll.GroupName,
                            SalaryBasedOn = payroll.SalaryBasedOn,
                            IsActive = payroll.IsActive,
                            CreDate = DateTime.Now.ToUniversalTime(),
                            CreBy = 1,
                            ChgDate = DateTime.Now.ToUniversalTime(),
                            ChgBy = 1
                        };

                        db.tblEmpPayRollInformations.Add(p);
                        db.SaveChanges();
                        retPayRollId = db.tblEmpPayRollInformations.Max(z => z.PayRollId);
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, msg.msgInsert, retPayRollId);
                    }
                    else
                    { // Mode == Edit
                        var line = db.tblEmpPayRollInformations.Where(z => z.PayRollId == payroll.PayRollId).SingleOrDefault();
                        if (line != null)
                        {
                            line.CTC = payroll.CTC;
                            line.BasicSalary = payroll.BasicSalary;
                            line.EmploymentTax = payroll.EmploymentTax;
                            line.ESIC = payroll.ESIC;
                            line.LeavesAllowedPerYear = payroll.LeavesAllowedPerYear;
                            line.PFAccountNumber = payroll.PFAccountNumber;
                            line.PF = payroll.PF;
                            line.CompanyBankName = payroll.CompanyBankName;
                            line.CompanyBankAccount = payroll.CompanyBankAccount;
                            line.PersonalBankName = payroll.PersonalBankName;
                            line.PersonalBankAccount = payroll.PersonalBankAccount;
                            line.AllocatedPassNo = payroll.AllocatedPassNo;
                            line.JoiningDate = payroll.JoiningDate;
                            line.ReLeavingDate = payroll.ReLeavingDate;
                            line.EmploymentStatus = payroll.EmploymentStatus;
                            line.PermanentFromDate = payroll.PermanentFromDate;
                            line.GName = payroll.GroupName;
                            line.SalaryBasedOn = payroll.SalaryBasedOn;
                            line.IsActive = payroll.IsActive;
                            line.ChgDate = DateTime.Now.ToUniversalTime();
                            line.ChgBy = 1;
                        }
                        db.SaveChanges();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, msg.msgUpdate, retPayRollId);
                    }
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageNamePayRoll, true);
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }


        /// <summary>
        /// GET api/employee
        /// fetch payroll information 
        /// </summary>
        [HttpGet]
        public ApiResponse FetchPayRollInfo()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int employeeId = Convert.ToInt32(nvc["employeeId"]);

                    var list = db.tblEmpPayRollInformations.Where(z => z.EmployeeId == employeeId).ToList();

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", list);
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageNamePayRoll, true);
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }
        #endregion

        #region COMPANY INFO
        /// <summary>
        /// GET api/employee
        /// retrieve Employee list for reporting 
        /// </summary>
        [HttpGet]
        public ApiResponse RetrieveReporting()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var list = (from ECI in db.tblEmpCompanyInformations
                                join EPI in db.tblEmpPersonalInformations on
                                ECI.EmployeeId equals EPI.EmployeeId
                                where ECI.IsTL == true
                                select new { EPI.EmployeeId, EPI.CandidateFirstName, EPI.CandidateMiddleName, EPI.CandidateLastName }).Distinct();

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", list);
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }

        /// <summary>
        /// GET api/employee
        /// fetch employee  company info
        /// </summary>
        [HttpGet]
        public ApiResponse FetchCompanyInfo()
        {
            GeneralMessages msg = new GeneralMessages(_pageNameCompanyInfo);
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int employeeId = Convert.ToInt32(nvc["employeeId"]);

                    var list = db.tblEmpCompanyInformations.Where(z => z.EmployeeId == employeeId).ToList();

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", list);
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageNameCompanyInfo, true);
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }


        // POST api/<controller>
        [HttpPost]
        public ApiResponse SaveCompanyInfo(tblEmpCompanyInformation company)
        {
            GeneralMessages msg = new GeneralMessages(_pageNameCompanyInfo);
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                tblEmpCompanyInformation tbl = new tblEmpCompanyInformation();
                try
                {
                    if (company.CompanyId > 0)
                    {
                        tbl = db.tblEmpCompanyInformations.Where(x => x.CompanyId == company.CompanyId).FirstOrDefault();
                    }
                    tbl.EmployeeId = company.EmployeeId;
                    tbl.TeamId = company.TeamId;
                    tbl.ReportingTo = company.ReportingTo;
                    tbl.DesignationId = company.DesignationId;
                    tbl.RolesId = company.RolesId;
                    tbl.IncrementCycle = company.IncrementCycle;
                    tbl.IsTL = company.IsTL;
                    tbl.IsBillable = company.IsBillable;
                    tbl.ModuleUser = company.ModuleUser;
                    tbl.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"]);
                    tbl.ChgDate = DateTime.Now.ToUniversalTime();
                    if (company.CompanyId <= 0)
                    {
                        tbl.IsActive = company.IsActive;
                        tbl.CreBy = Convert.ToInt32(HttpContext.Current.Session["UserId"]);
                        tbl.CreDate = DateTime.Now.ToUniversalTime();
                        db.tblEmpCompanyInformations.Add(tbl);
                        db.SaveChanges();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, msg.msgInsert, tbl);
                    }
                    else
                    {
                        db.SaveChanges();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, msg.msgUpdate, tbl);
                    }
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageNameCompanyInfo, true);
                }
                finally
                {
                    tbl = null;
                }
            }
            else
            {
                ERPUtilities.UnAuthorizedAccess(apiResponse);
            }
            return apiResponse;
        }

        #endregion

        #region LOGIN INFORMATION

        /// <summary>
        /// POST api/employee
        /// create and update record
        /// </summary>
        [HttpPost]
        public ApiResponse SaveLoginInformation(tblEmpLoginInformation login)
        {
            GeneralMessages msg = new GeneralMessages(_pageNameLoginInfo);
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    if (login.LoginInfoId == 0)
                    {// Mode == Add
                        tblEmpLoginInformation l = new tblEmpLoginInformation
                        {
                            EmployeeId = login.EmployeeId,
                            UserName = login.UserName,
                            Password = login.Password,
                            PasswordExpiresDays = login.PasswordExpiresDays,
                            IsRemoteLogin = login.IsRemoteLogin,
                            IsPermit = login.IsPermit,
                            IsActive = login.IsActive,
                            CreDate = DateTime.Now.ToUniversalTime(),
                            CreBy = 1,
                            ChgDate = DateTime.Now.ToUniversalTime(),
                            ChgBy = 1
                        };

                        db.tblEmpLoginInformations.Add(l);
                        db.SaveChanges();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, msg.msgInsert, db.tblEmpLoginInformations.Max(z => z.LoginInfoId));
                    }
                    else
                    { // Mode == Edit
                        var line = db.tblEmpLoginInformations.Where(z => z.LoginInfoId == login.LoginInfoId).SingleOrDefault();
                        if (line != null)
                        {
                            line.UserName = login.UserName;
                            line.Password = login.Password;
                            line.PasswordExpiresDays = login.PasswordExpiresDays;
                            line.IsRemoteLogin = login.IsRemoteLogin;
                            line.IsPermit = login.IsPermit;
                            line.IsActive = login.IsActive;
                            line.ChgDate = DateTime.Now.ToUniversalTime();
                            line.ChgBy = 1;
                        }
                        db.SaveChanges();
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, msg.msgUpdate, login.LoginInfoId);
                    }
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageNameLoginInfo, true);
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }


        /// <summary>
        /// GET api/employee
        /// fetch login information 
        /// </summary>
        [HttpGet]
        public ApiResponse FetchLoginInfo()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int employeeId = Convert.ToInt32(nvc["employeeId"]);

                    var list = db.tblEmpLoginInformations.Where(z => z.EmployeeId == employeeId).ToList();

                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", list);
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageNameLoginInfo, true);
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }
        #endregion

        #region COMPANY CREDENTIAL INFO
        /// <summary>
        /// GET api/employee
        /// Get Active Sources from source table
        /// </summary>
        [HttpGet]
        public ApiResponse GetActiveSources()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    var list = (from s in db.tblEmpSources
                                where s.IsActive == true
                                select new { s.SourceId, s.SourceName });
                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", list);
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageName, true);
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }
            return apiResponse;
        }

        /// <summary>
        /// POST api/employee
        /// create and update record
        /// </summary>
        [HttpPost]
        public ApiResponse CreateUpdateEmpCredentials(EmpCredentialViewModel empCre)
        {
            GeneralMessages msg = new GeneralMessages(_pageNameCredentialInfo);
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    Int16 Source = 0;

                    //if degree == Other
                    if (empCre.SourceId == -1)
                    { // insert new Degree to Degree master
                        tblEmpSource d = new tblEmpSource
                        {
                            SourceName = empCre.SourceOther,
                            IsActive = true,
                            CreDate = DateTime.Now.ToUniversalTime(),
                            ChgDate = DateTime.Now.ToUniversalTime()
                        };
                        db.tblEmpSources.Add(d);
                        db.SaveChanges();
                        Source = d.SourceId;
                    }

                    if (empCre.SrNo == 0)
                    {// Mode == Add
                        tblEmpCredentialInformation e = new tblEmpCredentialInformation
                        {
                            EmployeeId = empCre.EmployeeId,
                            SourceId = empCre.SourceId != -1 ? empCre.SourceId : Source,
                            UserName = empCre.UserName,
                            Password = empCre.Password,
                            BirthDate = Convert.ToDateTime(empCre.BirthDate).Date,
                            EmailId = empCre.EmailId,
                            SecurityQuestion1 = empCre.SecurityQuestion1,
                            SecurityAnswer1 = empCre.SecurityAnswer1,
                            SecurityQuestion2 = empCre.SecurityQuestion2,
                            SecurityAnswer2 = empCre.SecurityAnswer2,
                            Comments = empCre.Comments,
                            CreDate = DateTime.Now.ToUniversalTime(),
                            CreBy = Convert.ToInt32(HttpContext.Current.Session["UserId"]),
                            ChgDate = DateTime.Now.ToUniversalTime(),
                            ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"])
                        };

                        db.tblEmpCredentialInformations.Add(e);
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, msg.msgInsert, null);
                    }
                    else
                    { // Mode == Edit
                        var line = db.tblEmpCredentialInformations.Where(z => z.SrNo == empCre.SrNo).SingleOrDefault();

                        if (line != null)
                        {
                            line.SourceId = empCre.SourceId;
                            line.UserName = empCre.UserName;
                            line.Password = empCre.Password;
                            line.Password = empCre.Password;
                            line.BirthDate = Convert.ToDateTime(empCre.BirthDate).Date;
                            line.EmailId = empCre.EmailId;
                            line.SecurityQuestion1 = empCre.SecurityQuestion1;
                            line.SecurityAnswer1 = empCre.SecurityAnswer1;
                            line.SecurityQuestion2 = empCre.SecurityQuestion2;
                            line.SecurityAnswer2 = empCre.SecurityAnswer2;
                            line.Comments = empCre.Comments;
                            line.ChgDate = DateTime.Now.ToUniversalTime();
                            line.ChgBy = Convert.ToInt32(HttpContext.Current.Session["UserId"]);
                        }
                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, msg.msgUpdate, null);
                    }
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageNameCredentialInfo, true);
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }


        /// <summary>
        /// GET api/employee
        /// return employee Credential info list with sorting and filtering  functionalities
        /// </summary>
        [HttpPost]
        public ApiResponse GetEmpCredentialsList()
        {
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                using (db)
                {
                    NameValueCollection nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    int page = Convert.ToInt32(nvc["page"]);
                    int timezone = Convert.ToInt32(nvc["timezone"]);
                    string orderBy = nvc["orderby"];
                    string SourceName = nvc["SourceName"];
                    string UserName = nvc["UserName"];
                    string EmailId = nvc["EmailId"];
                    int employeeId = Convert.ToInt32(nvc["employeeId"]);


                    int iDisplayLength = Convert.ToInt32(nvc["count"]);
                    int iDisplayStart = (page - 1) * iDisplayLength;

                    IEnumerable<tblEmpCredentialInformation> list = null;
                    try
                    {
                        list = db.tblEmpCredentialInformations.Where(z => z.EmployeeId == employeeId).ToList();

                        // ADDING Source Detail
                        foreach (var item in list)
                        {
                            item.tblEmpSource = db.tblEmpSources.Where(z => z.SourceId == item.SourceId).FirstOrDefault();
                        }

                        // FILTERING DATA
                        if (!string.IsNullOrEmpty(SourceName) && SourceName != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.tblEmpSource.SourceName.ToLower().Contains(SourceName.ToLower())).ToList();
                        }
                        if (!string.IsNullOrEmpty(UserName) && UserName != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.UserName.ToLower().Contains(UserName.ToLower())).ToList();
                        }
                        if (!string.IsNullOrEmpty(EmailId) && EmailId != "undefined")
                        {
                            iDisplayStart = 0;
                            list = list.Where(z => z.EmailId.ToLower().Contains(EmailId.ToLower())).ToList();
                        }

                        // PERFORM SORTING
                        list = DoSortingForEmpCredential(list, orderBy.Trim());

                        //take total count to return for ng-table
                        var Count = list.Count();

                        list = list.Select(i =>
                        {
                            i.CreDate = Convert.ToDateTime(i.CreDate).AddMinutes(-1 * timezone);
                            i.ChgDate = Convert.ToDateTime(i.ChgDate).AddMinutes(-1 * timezone);
                            return i;
                        }).Skip(iDisplayStart).Take(iDisplayLength).ToList();

                        var resultData = new
                        {
                            total = Count,
                            result = list.Skip(iDisplayStart).Take(iDisplayLength).ToList()
                        };

                        apiResponse = ERPUtilities.GenerateApiResponse(true, 1, "", resultData);
                    }
                    catch (Exception ex)
                    {
                        apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageNameCredentialInfo, true);
                    }
                    finally
                    {
                        list = null;
                    }
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }


        /// <summary>
        /// Delete specific Employee Company Credentials Information
        /// </summary>
        /// <param name="empRelativeInfo"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResponse DeleteEmpCredentials(tblEmpCredentialInformation empCre)
        {
            GeneralMessages msg = new GeneralMessages(_pageNameCredentialInfo);
            ApiResponse apiResponse = new ApiResponse();
            if (sessionUtils.HasUserLogin())
            {
                try
                {
                    tblEmpCredentialInformation tbl = db.tblEmpCredentialInformations.Where(z => z.SrNo == empCre.SrNo).SingleOrDefault();
                    if (tbl != null)
                    {
                        db.tblEmpCredentialInformations.Remove(tbl);
                    }
                    db.SaveChanges();
                    apiResponse = ERPUtilities.GenerateApiResponse(true, 1, msg.msgDelete, null);
                }
                catch (Exception ex)
                {
                    apiResponse = ERPUtilities.GenerateExceptionResponse(ex, _pageNameCredentialInfo, true);
                }
            }
            else
            {
                apiResponse = ERPUtilities.GenerateApiResponse();
            }

            return apiResponse;
        }

        /// <summary>
        /// Return sorted list based on passed column
        /// </summary>
        public IEnumerable<tblEmpCredentialInformation> DoSortingForEmpCredential(IEnumerable<tblEmpCredentialInformation> list, string orderBy)
        {
            try
            {
                if (orderBy == "SourceName")
                {
                    list = list.OrderBy(z => z.tblEmpSource.SourceName);
                }
                else if (orderBy == "-SourceName")
                {
                    list = list.OrderByDescending(z => z.tblEmpSource.SourceName);
                }
                else if (orderBy == "UserName")
                {
                    list = list.OrderBy(z => z.UserName).ToList();
                }
                else if (orderBy == "-UserName")
                {
                    list = list.OrderByDescending(z => z.UserName).ToList();
                }
                else if (orderBy == "EmailId")
                {
                    list = list.OrderBy(z => z.EmailId).ToList();
                }
                else if (orderBy == "-EmailId")
                {
                    list = list.OrderByDescending(z => z.EmailId).ToList();
                }
                else if (orderBy == "ChgDate")
                {
                    list = list.OrderBy(z => z.ChgDate).ToList();
                }
                else if (orderBy == "-ChgDate")
                {
                    list = list.OrderByDescending(z => z.ChgDate).ToList();
                }
                return list;
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region COMMON FUNCTION
        /// <summary>
        /// common function for moving profile picture from temp folder to main folder
        /// </summary>
        protected void MoveFile(string fileName)
        {
            if (System.IO.File.Exists(HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["TempThumbnails"].ToString()) + "/" + fileName))
            {
                var ThumbSource = HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["TempThumbnails"].ToString()) + "/" + fileName;
                var ThumbDestination = HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["Thumbnails"].ToString()) + "/" + fileName;

                var ThumbsourceFile = HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["TempUploadPath"].ToString()) + "/" + fileName;
                var ThumbdestinationFile = HttpContext.Current.Server.MapPath("../../" + ConfigurationManager.AppSettings["UploadPath"].ToString()) + "/" + fileName;

                System.IO.File.Move(ThumbSource, ThumbDestination);
                System.IO.File.Move(ThumbsourceFile, ThumbdestinationFile);
            }
        }

        /// <summary>
        /// //delete profile picture
        /// </summary>
        private void DeleteProfilePicture(string profilePix)
        {
            string mainPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["UploadPath"].ToString());
            string thumbPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["Thumbnails"].ToString());

            if (File.Exists(Path.Combine(mainPath, profilePix)))
            {
                File.Delete(Path.Combine(mainPath, profilePix));
            }
            if (File.Exists(Path.Combine(thumbPath, profilePix)))
            {
                File.Delete(Path.Combine(thumbPath, profilePix));
            }
        }

        /// <summary>
        /// //delete employee document
        /// </summary>
        private void DeleteDocument(string filename)
        {
            string mainPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["empDocUploads"].ToString());
            string thumbPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["empDocThumbnails"].ToString());

            if (File.Exists(Path.Combine(mainPath, filename)))
            {
                File.Delete(Path.Combine(mainPath, filename));
            }
            if (File.Exists(Path.Combine(thumbPath, filename)))
            {
                File.Delete(Path.Combine(thumbPath, filename));
            }
        }

        private void DeletePDFDocument(string filename)
        {
            string empPDFDoc = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["empPDFUploads"].ToString());
            if (File.Exists(Path.Combine(empPDFDoc, filename)))
            {
                File.Delete(Path.Combine(empPDFDoc, filename));
            }

        }
        #endregion

    }
}
