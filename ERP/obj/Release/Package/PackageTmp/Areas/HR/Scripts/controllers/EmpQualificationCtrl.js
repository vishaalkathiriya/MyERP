/// <reference path="../libs/angular/angular.min.js" />


(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("EmpQualificationCtrl", [
            "$scope", "$rootScope", "$timeout", "EmployeeCreateService", "$http", "$filter", "ngTableParams",
            empQualificationCtrl
        ]);


    //Main controller function
    function empQualificationCtrl($scope, $rootScope, $timeout, EmployeeCreateService, $http, $filter, ngTableParams) {
        $scope.qualification = $scope.qualification || {};
        $scope.saveText = "Save";
        $scope.PageMode = "Add";
        $scope.timeZone = new Date().getTimezoneOffset().toString(); //timezone difference in minutes
        $scope.isFocusFirst = false;
        $scope.storage = { lastRecord: "" };
        $scope.qualification.IsActive = true;

        /*validate dropdown*/
        $scope.validateDropQAcedamic = function () {
            if ($scope.qualification.Acedamic && $scope.qualification.Acedamic != 0) return false;
            return true;
        };
        $scope.validateDropQDegree = function () {
            if ($scope.qualification.Degree && $scope.qualification.Degree != 0) return false;
            return true;
        };
        $scope.validateDropDiscipline = function () {
            if ($scope.qualification.Discipline && $scope.qualification.Discipline != 0) return false;
            return true;
        };
        $scope.validateDropUniversity = function () {
            if ($scope.qualification.University && $scope.qualification.University != 0) return false;
            return true;
        };
        $scope.validateDropInstitute = function () {
            if ($scope.qualification.Institute && $scope.qualification.Institute != 0) return false;
            return true;
        };
        $scope.validateDropPassingMonth = function () {
            if ($scope.qualification.PassingMonth && $scope.qualification.PassingMonth != 0) return false;
            return true;
        };
        $scope.validateDropPassingYear = function () {
            if ($scope.qualification.PassingYear && $scope.qualification.PassingYear != 0) return false;
            return true;
        };

        /*restrict other keyword in few textboxes*/
        $scope.$watch('qualification.DegreeOther', function (newValue) {
            var lower = angular.lowercase(newValue);
            if (lower == "other" || lower == "others") {
                $scope.empQform.txtDegreeOther.$setValidity("errorDegreeOther", false);
            } else if (newValue) {
                $scope.empQform.txtDegreeOther.$setValidity("errorDegreeOther", true);
            }
        });
        $scope.$watch('qualification.DisciplineOther', function (newValue) {
            var lower = angular.lowercase(newValue);
            if (lower == "other" || lower == "others") {
                $scope.empQform.txtDisciplineOther.$setValidity("errorDisciplineOther", false);
            } else if (newValue) {
                $scope.empQform.txtDisciplineOther.$setValidity("errorDisciplineOther", true);
            }
        });
        $scope.$watch('qualification.UniversityOther', function (newValue) {
            var lower = angular.lowercase(newValue);
            if (lower == "other" || lower == "others") {
                $scope.empQform.txtUniversityOther.$setValidity("errorUniversityOther", false);
            } else if (newValue) {
                $scope.empQform.txtUniversityOther.$setValidity("errorUniversityOther", true);
            }
        });
        $scope.$watch('qualification.InstituteOther', function (newValue) {
            var lower = angular.lowercase(newValue);
            if (lower == "other" || lower == "others") {
                $scope.empQform.txtInstituteOther.$setValidity("errorInstituteOther", false);
            } else if (newValue) {
                $scope.empQform.txtInstituteOther.$setValidity("errorInstituteOther", true);
            }
        });

        $scope.years = [];
        for (var i = 1980; i <= new Date().getFullYear() ; i++) {
            $scope.years.push(i);
        }

        $scope.getAcedamicList = function () {
            return EmployeeCreateService.RetrieveAcedamic();
        };
        $scope.getDegreeList = function (acedamicId) {
            return EmployeeCreateService.RetrieveDegree(acedamicId);
        };
        $scope.getDisciplineList = function (degreeId) {
            return EmployeeCreateService.RetrieveDiscipline(degreeId);
        };
        $scope.getUniversityList = function () {
            return EmployeeCreateService.RetrieveUniversity();
        };
        $scope.getInstituteList = function (universityId) {
            return EmployeeCreateService.RetrieveInstitute(universityId);
        };

        /* getting list of acedamic */
        function loadQAcedamicDrop() {
            $scope.getAcedamicList().then(function (result) {
                $scope.QAcedamic = result.data.DataList;
                $timeout(function () {
                    $scope.qualification.Acedamic = 0;
                    $scope.qualification.Degree = 0;
                    $scope.qualification.Discipline = 0;
                    $scope.qualification.Institute = 0;
                });
            });
        };
        loadQAcedamicDrop();

        /* getting list of degree */
        $scope.loadQDegreeDrop = function (acedamicId) {
            if (acedamicId != 0) {
                $scope.getDegreeList(acedamicId).then(function (result) {
                    $scope.QDegree = result.data.DataList;
                });
            } else {
                $scope.QDegree = {};
            }
        };

        /* getting list of discipline */
        $scope.loadQDisciplineDrop = function (degreeId) {
            if (degreeId != 0) {
                $scope.getDisciplineList(degreeId).then(function (result) {
                    $scope.QDiscipline = result.data.DataList;
                });
            } else {
                $scope.QDiscipline = {};
            }
        };

        /* getting list of university */
        function loadUniversityDrop() {
            $scope.getUniversityList().then(function (result) {
                $scope.QUniversity = result.data.DataList;
                $timeout(function () {
                    $scope.qualification.University = 0;
                });
            });
        };
        loadUniversityDrop();

        /* getting list of institute */
        $scope.loadInstituteDrop = function (universityId) {
            if (universityId != 0) {
                $scope.getInstituteList(universityId).then(function (result) {
                    $scope.QInstitute = result.data.DataList;
                });
            } else {
                $scope.QInstitute = {};
            }
        };

        $scope.$watch('qualification.Acedamic', function (newValue) {
            $scope.loadQDegreeDrop(newValue);
        });
        $scope.$watch('qualification.Degree', function (newValue) {
            $scope.loadQDisciplineDrop(newValue);
        });

        $scope.$watch('qualification.University', function (newValue) {
            $scope.loadInstituteDrop(newValue);
        });

        /*reset the form*/
        function ResetForm() {
            $scope.qualification = {
                SrNo: 0, EmployeeId: 0, Acedamic: 0, Degree: 0, DegreeOther: "",
                Discipline: 0, DisciplineOther: "", University: 0, UniversityOther: "", Institute: 0, InstituteOther: "",
                PassingMonth: "", PassingYear: "", Percentages: "", IsActive: true
            };

            $scope.saveText = "Save";
            $scope.PageMode = "Add";
            $scope.empQform.$setPristine();
            $scope.SetFocus();
        };

        /*save employee qualification*/
        $scope.CreateUpdateQualification = function (q) {
            $scope.qualification.EmployeeId = $scope.master.EmployeeId;
            EmployeeCreateService.CreateUpdateQualification(q).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        toastr.success(result.data.Message, 'Success');
                        $scope.RefreshTable();
                        ResetForm();
                        $scope.storage.lastRecord = {};
                        $scope.SetFocus();
                        $scope.empQform.$setPristine();
                    } else if (result.data.MessageType == 2) { // 2:Warning
                        toastr.warning(result.data.Message, 'Record already exists');
                    }
                    else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
                $rootScope.IsAjaxLoading = false;
            });
        }

        /*refresh table after */
        $scope.RefreshTable = function () {
            $scope.tableParams.reload();
        };

        /*reset the form*/
        $scope.ResetQualification = function () {
            if ($scope.PageMode == "Edit") {
                $scope.UpdateEmpQualification($scope.storage.lastRecord);
            } else { //mode == add
                ResetForm();
            }
        };

        /*get record for edit Qualification*/
        $scope.UpdateEmpQualification = function (_emp) {
            $scope.PageMode = "Edit";
            $scope.saveText = "Update";

            $scope.storage.lastRecord = _emp;
            $scope.getDegreeList(_emp.Acedamic).then(function (degree) {
                $scope.QDegree = degree.data.DataList;
                $scope.getDisciplineList(_emp.Degree).then(function (discipline) {
                    $scope.QDiscipline = discipline.data.DataList;
                    $scope.getInstituteList(_emp.University).then(function (institute) {
                        $scope.QInstitute = institute.data.DataList;

                        $scope.qualification.SrNo = _emp.SrNo;
                        $scope.qualification.Acedamic = _emp.Acedamic;
                        $scope.qualification.Degree = _emp.Degree;
                        $scope.qualification.Discipline = _emp.Discipline;
                        $scope.qualification.University = _emp.University;
                        $scope.qualification.Institute = _emp.Institute;
                        $scope.qualification.PassingYear = _emp.PassingYear;
                        $scope.qualification.PassingMonth = _emp.PassingMonthDigit;
                        $scope.qualification.Percentage = _emp.Percentage;

                        $scope.empQform.$setPristine();
                        $scope.SetFocus();
                    });
                });
            });

            
        };

        /*active/inactive employee qualification*/
        $scope.ChangeStatusEmpQualification = function (id, status) {
            var oper = status == true ? "InActive" : "Active";
            $rootScope.IsAjaxLoading = true;
            EmployeeCreateService.ChangeStatusEmpQualification(id, status).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        toastr.success(result.data.Message, 'Success');
                        $scope.RefreshTable();
                    } else if (result.data.MessageType == 2) { // 2:Warning
                        toastr.warning(result.data.Message, 'Warning!');
                    } else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
                $rootScope.IsAjaxLoading = false;
            });
        };

        /*delete employee qualification*/
        $scope.DeleteEmpQualification = function (id) {
            $rootScope.IsAjaxLoading = true;
            EmployeeCreateService.DeleteEmpQualification(id).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType != 0) { // 0:Error
                        toastr.success(result.data.Message, 'Success');
                        $scope.RefreshTable();
                    } else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
                $rootScope.IsAjaxLoading = false;
            });
        };

        /*datatable*/
        $scope.RetrieveEmpQualification = function () {
            ResetForm();
            $scope.tableParams = new ngTableParams({
                page: 1,
                count: 10,
                sorting: {
                    AcedamicName: 'asc'
                },
                defaultSort: 'asc'
            }, {
                total: 0,
                filterDelay: 750,
                getData: function ($defer, params) {
                    $rootScope.IsAjaxLoading = true;
                    $scope.isRecord = false;
                    EmployeeCreateService.GetEmployeeQualificationList($scope.master.EmployeeId, $scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter().AcedamicName).then(function (result) {
                        if (result.data.IsValidUser) {
                            if (result.data.MessageType != 0) { // 0:Error
                                //display no data message
                                if (result.data.DataList.total == 0) {
                                    $scope.noRecord = true;
                                } else {
                                    $scope.noRecord = false;
                                }

                                params.total(result.data.DataList.total);
                                $defer.resolve($scope.documents = result.data.DataList.result);
                            } else {
                                toastr.error(result.Message, 'Opps, Something went wrong');
                            }
                        } else {
                            $rootScope.redirectToLogin();
                        }
                        $rootScope.IsAjaxLoading = false;
                    });
                }
            });
        }

        $scope.SetFocus = function () {
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        }
    };


})();

