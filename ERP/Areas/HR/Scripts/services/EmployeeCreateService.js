/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services")
    .service("EmployeeCreateService", [
        "$http",
        function ($http) {

            var employee = {};

            //===================BEGIN EMPLOYEE===================//
            employee.RetrieveBloodGroup = function () {
                return $http({
                    method: 'GET',
                    url: "/api/Employee/RetrieveBloodGroup?ts=" + new Date().getTime()
                });
            };

            employee.RetrieveCountry = function () {
                return $http({
                    method: 'GET',
                    url: "/api/Employee/RetrieveCountry?ts=" + new Date().getTime()
                });
            };

            employee.RetrieveState = function (countryId) {
                return $http({
                    method: 'GET',
                    url: "/api/Employee/RetrieveState?ts=" + new Date().getTime() + "&countryId=" + countryId
                });
            };

            employee.FetchEmployee = function (employeeId) {
                return $http({
                    method: 'GET',
                    url: "/api/Employee/FetchEmployee?ts=" + new Date().getTime() + "&employeeId=" + employeeId
                });
            };

            employee.GetEmployeeList = function (timezone, page, count, orderby, filter) {
                return $http({
                    method: "POST",
                    data: filter,
                    contentType: "application/json",
                    url: "/api/Employee/GetEmployeeList?ts=" + new Date().getTime() + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby
                });
            };

            employee.CreateUpdateEmployee = function (data) {
                return $http({
                    method: "POST",
                    url: "/api/Employee/SaveEmployee?ts=" + new Date().getTime(),
                    data: data,
                    contentType: "application/json"
                });
            };

            employee.DeleteEmployee = function (id) {
                return $http({
                    method: "POST",
                    url: "/api/Employee/DeleteEmployee?ts=" + new Date().getTime(),
                    data: id,
                    contentType: "application/json"
                });
            };

            employee.ChangeStatus = function (id, status) {
                return $http({
                    method: "POST",
                    url: "/api/Employee/ChangeStatus?ts=" + new Date().getTime(),
                    data: JSON.stringify({ EmployeeId: id, IsActive: status }),
                    contentType: "application/json"
                });
            };
            //===================END EMPLOYEE===================//


            //===================BEGIN EMPLOYEE QUALIFICATION===================//
            employee.RetrieveAcedamic = function () {
                return $http({
                    method: 'GET',
                    url: "/api/Employee/RetrieveAcedamic?ts=" + new Date().getTime()
                });
            };

            employee.RetrieveDegree = function (acedamicId) {
                return $http({
                    method: 'GET',
                    url: "/api/Employee/RetrieveDegree?ts=" + new Date().getTime() + "&acedamicId=" + acedamicId
                });
            };

            employee.RetrieveDiscipline = function (degreeId) {
                return $http({
                    method: 'GET',
                    url: "/api/Employee/RetrieveDiscipline?ts=" + new Date().getTime() + "&degreeId=" + degreeId
                });
            };

            employee.RetrieveUniversity = function () {
                return $http({
                    method: 'GET',
                    url: "/api/Employee/RetrieveUniversity?ts=" + new Date().getTime()
                });
            };

            employee.RetrieveInstitute = function (universityId) {
                return $http({
                    method: 'GET',
                    url: "/api/Employee/RetrieveInstitute?ts=" + new Date().getTime() + "&universityId=" + universityId
                });
            };

            employee.ChangeStatusEmpQualification = function (id, status) {
                return $http({
                    method: "POST",
                    url: "/api/Employee/ChangeStatusEmpQualification?ts=" + new Date().getTime(),
                    data: JSON.stringify({ SrNo: id, IsActive: status }),
                    contentType: "application/json"
                });
            };

            employee.DeleteEmpQualification = function (id) {
                return $http({
                    method: "POST",
                    url: "/api/Employee/DeleteEmployeeQualification?ts=" + new Date().getTime(),
                    data: id,
                    contentType: "application/json"
                });
            };

            employee.CreateUpdateQualification = function (data) {
                return $http({
                    method: "POST",
                    url: "/api/Employee/SaveEmployeeQualification?ts=" + new Date().getTime(),
                    data: data,
                    contentType: "application/json"
                });
            };

            employee.GetEmployeeQualificationList = function (employeeId, timezone, page, count, orderby, filter) {
                return $http({
                    method: "POST",
                    data: filter,
                    contentType: "application/json",
                    url: "/api/Employee/GetEmployeeQualificationList?ts=" + new Date().getTime() + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&filter=" + filter + "&employeeId=" + employeeId
                });
            };

            employee.FetchEmpQualification = function (srNo) {
                return $http({
                    method: 'GET',
                    url: "/api/Employee/FetchEmpQualification?ts=" + new Date().getTime() + "&id=" + srNo
                });
            };
            //===================END EMPLOYEE QUALIFICATION===================//


            //===================BEGIN WORK EXPERIENCE===================//
            employee.CreateUpdateWorkExperience = function (_workExperience, _timeZone, _EmployeeId) {
                var dateFrom = _workExperience.FromDate.split('-');
                var dateTo = _workExperience.ToDate.split('-');
                var _FromDate = new Date(dateFrom[2], dateFrom[1] - 1, dateFrom[0]);
                var _ToDate = new Date(dateTo[2], dateTo[1] - 1, dateTo[0]);
                _workExperience.FromDate = _FromDate;
                _workExperience.ToDate = _ToDate;

                var workExperience = {
                    SrNo: _workExperience.SrNo,
                    EmployeeId: _EmployeeId,
                    CompanyName: _workExperience.CompanyName,
                    Designation: _workExperience.Designation,
                    //  Skills: _workExperience.Skills.slice(0, _workExperience.Skills.length).join(','),
                    Skills:_workExperience.Skills,
                    FromDate: _FromDate,
                    ToDate: _ToDate,
                    Salary: _workExperience.Salary,
                    Comments: _workExperience.Comments
                }
                return $http({
                    method: 'POST',
                    url: "/api/Employee/CreateUpdateWorkExperience?ts=" + new Date().getTime() + "&timezone=" + _timeZone,
                    data: workExperience
                });
            }

            employee.RetriveTechnology = function (searchText) {
                return $http({
                    method: "GET",
                    url: "/api/Employee/RetriveTechnology?ts=" + new Date().getTime() + "&searchText=" + searchText
                });
            }

            employee.RetrieveEmpWorkExperience = function (employeeId, timezone, page, count, orderby, EmployeeName, CompanyName, Designation, Skills) {
                return $http({
                    method: "GET",
                    url: "/api/Employee/RetrieveEmpWorkExperience?ts=" + new Date().getTime() + "&employeeId=" + employeeId + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&employeeName=" + EmployeeName + "&companyName=" + CompanyName + "&designation=" + Designation + "&skills=" + Skills
                });
            }

            employee.DeleteEmpWorkExperience = function (_detail) {
                var detail = {
                    SrNo: _detail.SrNo,
                };
                return $http({
                    method: 'POST',
                    url: "/api/Employee/DeleteEmpWorkExperience?ts=" + new Date().getTime(),
                    data: detail
                });
            }

            employee.ChangeIsActive = function (_detail) {
                var detail = {
                    SrNo: _detail.SrNo,
                    IsActive: _detail.IsActive
                };

                return $http({
                    method: 'POST',
                    url: "/api/Employee/ChangeIsActive?ts=" + new Date().getTime(),
                    data: detail
                });
            }
            //===================END WORK EXPERIENCE===================//


            //===================BEGIN EMPLOYEE DOCUMENT===================//
            employee.GetActiveDocuments = function () {
                return $http({
                    method: 'GET',
                    url: "/api/Documents/GetActiveDocuments?ts=" + new Date().getTime()
                });
            };

            employee.CreateUpdateEmpDocument = function (_data) {
                var empDoc = {
                    SrNo: _data.SrNo,
                    EmployeeId: _data.EmployeeId,
                    DocumentId: _data.DocumentId,
                    FileName: _data.FileName,
                    IsActive: _data.IsActive
                };
                return $http({
                    method: "POST",
                    url: "/api/Employee/SaveEmployeeDocument?ts=" + new Date().getTime(),
                    data: empDoc,
                    contentType: 'application/json; charset=utf-8'
                });
            };

            //employee.GetEmpDocumentList = function (employeeId, timezone, page, count, orderby) {
            employee.GetEmpDocumentList = function (employeeId, timezone) {
                return $http({
                    method: "POST",
                    data: employeeId,
                    contentType: "application/json",
                    url: "/api/Employee/GetEmpDocumentList?ts=" + new Date().getTime() + "&employeeId=" + employeeId + "&timezone=" + timezone
                });
            };

            employee.DeleteEmpDoc = function (id) {
                return $http({
                    method: "POST",
                    url: "/api/Employee/DeleteEmpDoc?ts=" + new Date().getTime(),
                    data: id,
                    contentType: "application/json"
                });
            };

            employee.ChangeDocStatus = function (_document) {
                return $http({
                    method: "POST",
                    url: "/api/Employee/ChangeDocStatus?ts=" + new Date().getTime(),
                    data: _document,
                    contentType: "application/json"
                });
            };
            //===================END EMPLOYEE DOCUMENT===================//



            //===================BEGIN COMPANY INFORMATION===================//
            employee.GetActiveDesignations = function () {
                return $http({
                    method: 'GET',
                    url: "/api/Designation/GetActiveDesignations?ts=" + new Date().getTime()
                });
            };

            employee.GetActiveRoles = function () {
                return $http({
                    method: 'GET',
                    url: "/api/Role/GetActiveRoles?ts=" + new Date().getTime()
                });
            };

            employee.GetActiveTechnologiesGroup = function () {
                return $http({
                    method: 'GET',
                    url: "/api/TechGroup/GetActiveTechnologiesGroup?ts=" + new Date().getTime()
                });
            };

            employee.GetReporting = function () {
                return $http({
                    method: 'GET',
                    url: "/api/Employee/RetrieveReporting?ts=" + new Date().getTime()
                });
            };

            employee.FetchCompanyInfo = function (employeeId) {
                return $http({
                    method: 'GET',
                    url: "/api/Employee/FetchCompanyInfo?ts=" + new Date().getTime() + "&employeeId=" + employeeId
                });
            };

            employee.CreateUpdateCompanyInfo = function (_company) {
                var com = {
                    CompanyId: _company.CompanyId,
                    EmployeeId: _company.EmployeeId,
                    TeamId: _company.TeamId,
                    ReportingTo: _company.ReportingTo,
                    DesignationId: _company.DesignationId,
                    RolesId: _company.RoleId,
                    IncrementCycle: _company.IncrementCycle,
                    IsTL: _company.IsTL,
                    IsBillable: _company.IsBillable,
                    ModuleUser: _company.ModuleUser,
                    IsActive: _company.IsActive
                }
                return $http({
                    method: "POST",
                    url: "/api/Employee/SaveCompanyInfo?ts=" + new Date().getTime(),
                    data: com,
                    contentType: 'application/json; charset=utf-8'
                });
            };

            //===================END COMPANY INFORMATION===================//


            //===================BEGIN EMPLOYEE RELATIVE INFO============//
            employee.RetrieveRelations = function () {
                return $http({
                    method: "GET",
                    url: "/api/Employee/RetrieveRelations?ts=" + new Date().getTime(),
                    contentType: "application/json"
                });
            }

            employee.RetrieveRelativeInfo = function (employeeId, timezone, page, count, orderby, RelativeName, Relation, AcedamicStatus, DegreeName, TypeOfWork) {
                return $http({
                    method: "GET",
                    url: "/api/Employee/RetrieveRelativeInfo?ts=" +
                        new Date().getTime() +
                        "&employeeId=" + employeeId +
                        "&timezone=" + timezone +
                        "&page=" + page +
                        "&count=" + count +
                        "&orderby=" + orderby +
                        "&relativeName=" + RelativeName +
                        "&relation=" + Relation +
                        "&acedamicStatus=" + AcedamicStatus +
                        "&degreeName=" + DegreeName +
                        "&typeOfWork=" + TypeOfWork
                });
            }

            employee.CreateUpdateRelativeInfo = function (relativeInfo, timeZone) {
                var date = relativeInfo.BirthDate.split('-');
                var _birthDate = new Date(date[2], date[1] - 1, date[0]);
                relativeInfo.BirthDate = _birthDate
                return $http({
                    method: "POST",
                    url: "/api/Employee/CreateUpdateRelativeInfo?ts=" + new Date().getTime() + "&timezone=" + timeZone,
                    data: relativeInfo
                });
            }

            employee.DeleteEmpRelativeInfo = function (relativeInfo) {
                return $http({
                    method: "POST",
                    url: "/api/Employee/DeleteEmpRelativeInfo?ts=" + new Date().getTime(),
                    data: relativeInfo,
                    contentType: "application/json"
                });
            }
            //===================BEGIN EMPLOYEE RELATIVE INFO============//

            //===================BEGIN PAYROLL INFORMATION===================//
            employee.FetchPayRollInfo = function (employeeId) {
                return $http({
                    method: 'GET',
                    url: "/api/Employee/FetchPayRollInfo?ts=" + new Date().getTime() + "&employeeId=" + employeeId
                });
            };

            employee.CreateUpdatePayRoll = function (data) {
                return $http({
                    method: "POST",
                    url: "/api/Employee/SavePayRollInformation?ts=" + new Date().getTime(),
                    data: data,
                    contentType: "application/json"
                });
            };
            //===================END PAYROLL INFORMATION===================//

            //===================BEGIN LOGIN INFORMATION===================//
            employee.FetchLoginInfo = function (employeeId) {
                return $http({
                    method: 'GET',
                    url: "/api/Employee/FetchLoginInfo?ts=" + new Date().getTime() + "&employeeId=" + employeeId
                });
            };

            employee.CreateUpdateEmpLogin = function (data) {
                return $http({
                    method: "POST",
                    url: "/api/Employee/SaveLoginInformation?ts=" + new Date().getTime(),
                    data: data,
                    contentType: "application/json"
                });
            };
            //===================END LOGIN INFORMATION===================//


            //===================BEGIN COMPANY CREDENTIAL INFORMATION===================//
            employee.GetActiveSources = function () {
                return $http({
                    method: 'GET',
                    url: "/api/Employee/GetActiveSources?ts=" + new Date().getTime()
                });
            };

            employee.CreateUpdateEmpCredentials = function (_data) {
                return $http({
                    method: "POST",
                    url: "/api/Employee/CreateUpdateEmpCredentials?ts=" + new Date().getTime(),
                    data: _data,
                    contentType: "application/json"
                });
            };

            employee.GetEmpCredentialsList = function (employeeId, timezone, page, count, orderby, SourceName, UserName, EmailId) {
                return $http({
                    method: "POST",
                    //data: filter,
                    contentType: "application/json",
                    url: "/api/Employee/GetEmpCredentialsList?ts=" + new Date().getTime() + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&SourceName=" + SourceName + "&UserName=" + UserName + "&EmailId=" + EmailId + "&employeeId=" + employeeId
                });
            };

            employee.DeleteEmpCredentials = function (_data) {
                return $http({
                    method: "POST",
                    url: "/api/Employee/DeleteEmpCredentials?ts=" + new Date().getTime(),
                    data: _data,
                    contentType: "application/json"
                });
            }
            //===================END COMPANY CREDENTIAL INFORMATION===================//




            //===================BEGIN EMPLOYEE VIEW INFORMATION===================//


            employee.EditProfilePhoto = function (data, _timezone) {
                return $http({
                    method: "POST",
                    url: "/api/Employee/EditProfilePhoto?ts=" + new Date().getTime() + "&timezone=" + _timezone + "&ProfilePhoto=" + data,
                    data: data,
                    contentType: "application/json"
                });
            };

            employee.RetrieveUserProfile = function () {

                return $http({
                    method: "GET",
                    url: "/api/Employee/GetUserInformation?ts=" + new Date().getTime()
                });
            };


            //===================END  EMPLOYEE VIEW INFORMATION===================//

            return employee;
        }
    ]);