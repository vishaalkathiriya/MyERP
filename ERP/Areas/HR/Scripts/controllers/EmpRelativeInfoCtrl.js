/// <reference path="../libs/angular/angular.js" />

(function () {

    angular.module("ERPApp.Controllers").controller("EmpRelativeInfoCtrl", [
        "$scope",
        "$rootScope",
        "$timeout",
        "EmployeeCreateService",
        "$http",
        "$filter",
        "ngTableParams",
        "$q",
        EmpRelativeInfoCtrl
    ]);

    function EmpRelativeInfoCtrl($scope, $rootScope, $timeout, ES, $http, $filter, ngTableParams, $q) {

        // BEGIN VARIABLES
        $scope.relativeInfo = $scope.relativeInfo || {};
        $scope.lastRelativeInfo = $scope.lastRelativeInfo || {};
        $scope.saveText = "Save";
        $scope.mode = "Add";
        $scope.timeZone = new Date().getTimezoneOffset().toString();
        $scope.isFirstFocus = false;
        // END VARIABLES

        // BEGIN DATE PICKER
        $scope.dateOptions = { 'year-format': "'yy'", 'starting-day': 1 };
        $scope.formats = ['dd-MM-yyyy', 'yyyy/MM/dd', 'shortDate'];
        $scope.format = $scope.formats[0];
        $scope.today = function () {
            $scope.currentDate = new Date();
        };
        $scope.today();
        $scope.showWeeks = true;
        $scope.toggleWeeks = function () {
            $scope.showWeeks = !$scope.showWeeks;
        };
        $scope.clear = function () {
            $scope.currentDate = null;
        };
        $scope.maxDate = $scope.maxDate || new Date();
        $scope.BirthDateCalendarOpened = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.isBirthDateCalendarOpened = true;
        }
        // END DATE PICKER

        // BEGIN FUNCTIONS
        $scope.relativeInfo = {
            EmployeeId: $scope.master.EmployeeId,
            SrNo: 0,
            RelativeName: "",
            RelationId: 0,
            RelativeRelationName: "",
            RelativeRelationNameOther: "",
            BirthDate: "",
            AcedamicStatusId: 0,
            AcedamicStatus: "",
            DegreeId: 0,
            DegreeName: "",
            DegreeNameOther: "",
            TypeOfWork: ""
        }

        $scope.RetrieveRelations = function () {
            ES.RetrieveRelations().then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) {
                        $scope.Relations = result.data.DataList;
                        $scope.relativeInfo.RelationId = 0;
                    } else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                }
                else {
                    $rootScope.redirectToLogin();
                }
            });
        }
        $scope.RetrieveRelations();

        $scope.RetriveAcedamics = function () {
            ES.RetrieveAcedamic().then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) {
                        $scope.Acedamic = result.data.DataList;
                        $scope.relativeInfo.AcedamicStatusId = 0;
                    } else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                }
                else {
                    $rootScope.redirectToLogin();
                }
            });
        };
        $scope.RetriveAcedamics();
        $scope.$watch('relativeInfo.AcedamicStatusId', function (newValue) {
            $scope.RetrieveDegree(newValue);
        });

        $scope.RetrieveDegree = function (acedamicId) {
            if (acedamicId != 0) {
                ES.RetrieveDegree(acedamicId).then(function (result) {
                    if (result.data.IsValidUser) {
                        if (result.data.MessageType == 1) {
                            $scope.Degree = result.data.DataList;
                        } else {
                            toastr.error(result.data.Message, 'Opps, Something went wrong');
                        }
                    }
                    else {
                        $rootScope.redirectToLogin();
                    }
                });
            } else {
                $scope.Degree = {};
            }
        };

        $scope.CreateUpdateRelativeInfo = function (relativeInfo) {
            ES.CreateUpdateRelativeInfo(relativeInfo, $scope.timeZone).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType === 1) {
                        $scope.relativeInfo = {
                            EmployeeId: $scope.master.EmployeeId,
                            SrNo: 0,
                            RelativeName: "",
                            RelationId: 0,
                            RelativeRelationName: "",
                            RelativeRelationNameOther: "",
                            BirthDate: "",
                            AcedamicStatusId: 0,
                            AcedamicStatus: "",
                            DegreeId: 0,
                            DegreeName: "",
                            DegreeNameOther: "",
                            TypeOfWork: ""
                        }
                        $scope.RefreshTable();
                        $scope.frmRelativeInfo.$setPristine();
                        toastr.success(result.data.Message, 'Success');
                        $scope.mode = "Add";
                        $scope.saveText = "Save";
                        $scope.SetFocus();
                        $scope.RetrieveRelations();
                    }
                    else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                }
                else {
                    $rootScope.redirectToLogin();
                }
            });
        }

        $scope.RetrieveRelativeInfo = function () {
            $scope.mode = "Add";
            $scope.frmRelativeInfo.$setPristine();
            $scope.tableParams = new ngTableParams({
                page: 1,
                count: 10,
                sorting: {
                    EmployeeName: 'asc'
                },
                defaultSort: 'asc'
            }, {
                total: 0,
                filterDelay: 750,
                getData: function ($defer, params) {
                    $rootScope.IsAjaxLoading = true;
                    $scope.isRecord = false;
                    ES.RetrieveRelativeInfo($scope.master.EmployeeId, $scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter().RelativeName, params.filter().Relation, params.filter().AcedamicStatus, params.filter().DegreeName, params.filter().TypeOfWork).then(function (result) {
                        if (result.data.IsValidUser) {
                            if (result.data.MessageType != 0) {
                                if (result.data.DataList.total == 0) {
                                    $scope.noRecord = true;
                                    $scope.filterText = params.filter().RelativeName;
                                } else {
                                    $scope.noRecord = false;
                                }
                                params.total(result.data.DataList.total);
                                $defer.resolve($scope.ri = result.data.DataList.result);
                                $rootScope.IsAjaxLoading = false;
                            }
                            else {
                                toastr.error(result.data.Message, 'Opps, Something went wrong');
                            }
                        }
                        else {
                            $rootScope.redirectToLogin();
                        }
                    });
                }
            })
        }

        $scope.UpdateEmpRelativeInfo = function (_relativeInfo) {
            $scope.lastRelativeInfo = {
                EmployeeId: $scope.master.EmployeeId,
                SrNo: _relativeInfo.SrNo,
                RelativeName: _relativeInfo.RelativeName,
                RelationId: _relativeInfo.RelationId,
                RelativeRelationName: _relativeInfo.RelativeRelationName,
                RelativeRelationNameOther: _relativeInfo.RelativeRelationNameOther,
                bDate: _relativeInfo.BirthDate,
                BirthDate: $filter('date')(_relativeInfo.BirthDate, 'dd-MM-yyyy'),
                AcedamicStatusId: _relativeInfo.AcedamicStatusId,
                AcedamicStatus: _relativeInfo.AcedamicStatus,
                DegreeId: _relativeInfo.DegreeId,
                DegreeName: _relativeInfo.DegreeName,
                DegreeNameOther: _relativeInfo.DegreeNameOther,
                TypeOfWork: _relativeInfo.TypeOfWork
            }
            $scope.mode = "Edit";
            $scope.saveText = "Update";
            $scope.relativeInfo = {
                EmployeeId: $scope.master.EmployeeId,
                SrNo: _relativeInfo.SrNo,
                RelativeName: _relativeInfo.RelativeName,
                RelationId: _relativeInfo.RelationId,
                RelativeRelationName: _relativeInfo.RelativeRelationName,
                RelativeRelationNameOther: _relativeInfo.RelativeRelationNameOther,
                bDate: _relativeInfo.BirthDate,
                BirthDate: $filter('date')(_relativeInfo.BirthDate, 'dd-MM-yyyy'),
                AcedamicStatusId: _relativeInfo.AcedamicStatusId,
                AcedamicStatus: _relativeInfo.AcedamicStatus,
                DegreeId: _relativeInfo.DegreeId,
                DegreeName: _relativeInfo.DegreeName,
                DegreeNameOther: _relativeInfo.DegreeNameOther,
                TypeOfWork: _relativeInfo.TypeOfWork
            }
            $scope.SetFocus();
        }

        $scope.DeleteEmpRelativeInfo = function (detail) {
            ES.DeleteEmpRelativeInfo(detail).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType === 1) {
                        toastr.success(result.data.Message, 'Success');
                        $scope.RefreshTable();
                    } else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                }
                else {
                    $rootScope.redirectToLogin();
                }
            });
        }

        $scope.ResetRelativeInfo = function () {
            if ($scope.mode === "Add") {
                $scope.relativeInfo = {
                    EmployeeId: $scope.master.EmployeeId,
                    SrNo: 0,
                    RelativeName: "",
                    RelationId: 0,
                    RelativeRelationName: "",
                    RelativeRelationNameOther: "",
                    BirthDate: "",
                    AcedamicStatusId: 0,
                    AcedamicStatus: "",
                    DegreeId: 0,
                    DegreeName: "",
                    DegreeNameOther: "",
                    TypeOfWork: ""
                }
            }
            else if ($scope.mode === "Edit") {
                $scope.relativeInfo.EmployeeId = $scope.master.EmployeeId,
                $scope.relativeInfo.SrNo = $scope.lastRelativeInfo.SrNo,
                $scope.relativeInfo.RelativeName = $scope.lastRelativeInfo.RelativeName,
                $scope.relativeInfo.RelationId = $scope.lastRelativeInfo.RelationId,
                $scope.relativeInfo.RelativeRelationName = $scope.lastRelativeInfo.RelativeRelationName,
                $scope.relativeInfo.RelativeRelationNameOther = $scope.lastRelativeInfo.RelativeRelationNameOther,
                $scope.relativeInfo.BirthDate = $scope.lastRelativeInfo.BirthDate,
                $scope.relativeInfo.AcedamicStatusId = $scope.lastRelativeInfo.AcedamicStatusId,
                $scope.relativeInfo.AcedamicStatus = $scope.lastRelativeInfo.AcedamicStatus,
                $scope.relativeInfo.DegreeId = $scope.lastRelativeInfo.DegreeId,
                $scope.relativeInfo.DegreeName = $scope.lastRelativeInfo.DegreeName,
                $scope.relativeInfo.DegreeNameOther = $scope.lastRelativeInfo.DegreeNameOther,
                $scope.relativeInfo.TypeOfWork = $scope.lastRelativeInfo.TypeOfWork
            }
            $scope.SetFocus();
            $scope.frmRelativeInfo.$setPristine();
        }

        $scope.ClearRelativeInfo = function () {
            $scope.relativeInfo = {
                EmployeeId: $scope.master.EmployeeId,
                SrNo: 0,
                RelativeName: "",
                RelationId: 0,
                RelativeRelationName: "",
                RelativeRelationNameOther: "",
                BirthDate: "",
                AcedamicStatusId: 0,
                AcedamicStatus: "",
                DegreeId: 0,
                DegreeName: "",
                DegreeNameOther: "",
                TypeOfWork: ""
            }
            $scope.frmRelativeInfo.$setPristine();
            $scope.mode = "Add";
            $scope.saveText = "Save";
            $scope.SetFocus();
        }

        $scope.RefreshTable = function () {
            $scope.tableParams.reload();
        };

        $scope.SetFocus = function () {
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        }
        // END FUNCTIONS

        // BEGIN OTHER TEXTBOX VALIDATION
        $scope.$watch('relativeInfo.RelativeRelationNameOther', function (newValue) {
            var lower = angular.lowercase(newValue);
            if (lower == "other" || lower == "others") {
                $scope.frmRelativeInfo.txtRelativeRelationNameOther.$setValidity("errorRelationOther", false);
            } else if (newValue) {
                $scope.frmRelativeInfo.txtRelativeRelationNameOther.$setValidity("errorRelationOther", true);
            }
        });
        $scope.$watch('relativeInfo.DegreeNameOther', function (newValue) {
            var lower = angular.lowercase(newValue);
            if (lower == "other" || lower == "others") {
                $scope.frmRelativeInfo._txtDegreeOther.$setValidity("errorDegreeOther", false);
            } else if (newValue) {
                $scope.frmRelativeInfo._txtDegreeOther.$setValidity("errorDegreeOther", true);
            }
        });
        // END OTHER TEXTBOX VALIDATION

        // BEGIN DROPDOWN VALIDATION
        $scope.ValidateRelativeRelation = function () {
            if ($scope.relativeInfo.RelationId && $scope.relativeInfo.RelationId != 0) return false;
            return true;
        }
        $scope.ValidateAcedamic = function () {
            if ($scope.relativeInfo.AcedamicStatusId && $scope.relativeInfo.AcedamicStatusId != 0) return false;
            return true;
        };
        $scope.ValidateDegree = function () {
            if ($scope.relativeInfo.DegreeId && $scope.relativeInfo.DegreeId != 0) return false;
            return true;
        };

        $scope.ValidateBirthDate = function (birthDate) {
            if (!birthDate) {
                $scope.frmRelativeInfo.txtBirthDate.$setValidity("invalidBirthDateRI", true);
                return;
            } else if (birthDate.length == 10) {
                if ($scope.ValidateDate(birthDate)) {
                    if ($scope.IsGreterThanToday(birthDate)) {
                        $scope.frmRelativeInfo.txtBirthDate.$setValidity("invalidBirthDateRI", false);
                    } else {
                        $scope.frmRelativeInfo.txtBirthDate.$setValidity("invalidBirthDateRI", true);
                        var dt = birthDate.split('-');
                        $scope.relativeInfo.bDate = dt[2] + "-" + dt[1] + "-" + dt[0] + "T00:00:00";
                    }
                } else {
                    $scope.frmRelativeInfo.txtBirthDate.$setValidity("invalidBirthDateRI", false);
                }
            } else {
                $scope.frmRelativeInfo.txtBirthDate.$setValidity("invalidBirthDateRI", false);
            }
        }
        $scope.$watch('relativeInfo.bDate', function (newValue) {
            $scope.relativeInfo.BirthDate = $filter('date')(newValue, 'dd-MM-yyyy');
            $scope.frmRelativeInfo.txtBirthDate.$setValidity("invalidBirthDateRI", true);
        });
        // END DROPDOWN VALIDATION

        $scope.IsGreterThanToday = function (date) { //date should be in dd-MM-yyyy format
            var tDT = date.split('-');
            var todayDT = $filter('date')(new Date(), 'dd-MM-yyyy').split('-');

            var tDate = new Date(parseInt(tDT[2]), parseInt(tDT[1]) - 1, parseInt(tDT[0]));
            var todayDate = new Date(parseInt(todayDT[2]), parseInt(todayDT[1]) - 1, parseInt(todayDT[0]));

            if (tDate > todayDate) {
                return true;
            }
            return false;
        };
    }

})();