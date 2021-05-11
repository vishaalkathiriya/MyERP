/// <reference path="../libs/angular/angular.min.js" />

(function () {
    'use strict';
    angular.module("ERPApp.Controllers")
       .controller("HRDFireHydrantSprinklerCtrl", [
            "$scope", "$rootScope", "$timeout", "$http", "$filter", "FireHydrantSpprinklerService", "ngTableParams", "$modal", "Upload",
           HRDFireHydrantSprinklerCtrl
       ]);

    function HRDFireHydrantSprinklerCtrl($scope, $rootScope, $timeout, $http, $filter, FireHydrantSpprinklerService, ngTableParams, $modal, Upload) {
        $scope.editData = $scope.editData || {};
        $scope.mode = "Add";
        $scope.saveText = "Save";
        $scope.timeZone = new Date().getTimezoneOffset().toString(); //timezone difference in minutes
        $scope.isFocusFirst = false;
        $scope.storage = { lastRecord: "" };
        $scope.Attachment = "";
        $scope.ischanged = false;

        $scope.filterDate = {
            dateRange: { startDate: "", endDate: "" }
        };

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
        $scope.calendarOpenDateOfInspection = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.editData.calOpenDateOfInspection = true;
        };



        $scope.$watch('dateofinspaction', function (newValue) {
            $scope.editData.DateOfInspection = $filter('date')(newValue, 'dd-MM-yyyy');
        });

        $scope.ValidateDateOfInspection = function (dateofinspaction, FireExtinguiserform) {
            if (!dateofinspaction) {
                $scope.FireExtinguiserform.txtDateOfInspection.$setValidity("invalidDateOfInspection", true);
                return;
            } else if (dateofinspaction.length == 10) {
                if ($scope.ValidateDate(dateofinspaction)) {
                    //test
                    $scope.dateofinspaction = $scope.StringToDateString(dateofinspaction);
                    $scope.FireExtinguiserform.txtDateOfInspection.$setValidity("invalidDateOfInspection", true);
                } else {
                    $scope.FireExtinguiserform.txtDateOfInspection.$setValidity("invalidDateOfInspection", false);
                }
            } else {
                $scope.FireExtinguiserform.txtDateOfInspection.$setValidity("invalidDateOfInspection", false);
            }
        };

        $scope.ValidateDate = function (date) {
            if (date) {
                var isError = false;
                var dates = date.split('-');
                if (dates[0].search("_") > 0 || dates[1].search("_") > 0 || dates[2].search("_") > 0) {
                    isError = true;
                }
                else {
                    if (!parseInt(dates[0]) || parseInt(dates[0]) > 31) { isError = true; }
                    if (!parseInt(dates[1]) || parseInt(dates[1]) > 12) { isError = true; }
                    if (!parseInt(dates[2]) || dates[2].length != 4) { isError = true; }
                }

                if (!isError) { return true; } // date is validated
                return false; // error in validation
            }
            return true;
        };

        $scope.StringToDateString = function (dtValue) {
            if (dtValue) {
                var dt = dtValue.split('-');
                return dt[2] + "-" + dt[1] + "-" + dt[0] + "T00:00:00";
            }
            return dtValue;
        };

        $scope.maxDate = $scope.maxDate || new Date();
        // End DATE PICKER


        //BEGIN FILE UPLODER
        $scope.upload = function (files) {
            Upload.upload({
                url: "../Handler/FileUpload.ashx",
                file: files
            }).progress(function (evt) {
                var progressPercentage = parseInt(100.0 * evt.loaded / evt.total);
            }).success(function (data, status, headers, config) {
                $scope.editData.Attachment = data;
            })
        }
        $scope.$watch('rejFiles', function () {
            if (angular.isArray($scope.rejFiles)) {
                $scope.rejFiles.forEach(function (file) {
                    toastr.error('The selected file is not a valid file. Please select a valid *.xls, *.pdf or *.doc file and try again ', 'Invalid selected file');
                });
            }
        });
        //END FILE UPLODER

        //BEGIN RESET FORM
        function ResetForm() {
            $scope.editData = {
                BuildingName:"",    DateOfInspection: "", CheckedBy: "", Findings: "", RootCause: "", CorrectiveActionTaken: "", Remark: ""
            };
            $scope.rejFiles = [];
            $scope.editData.DateOfInspection = $filter('date')(new Date(), 'dd-MM-yyyy');
            $scope.DateOfInspection = $scope.StringToDateString($scope.editData.DateOfInspection);

            $scope.FireHydrantSprinklerform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        }
        //END RESET FORM

        //BEGIN ADD FIRE HYDRANT SPRINKLER SYSTEM RECORDS BUTTON
        $scope.AddFireHydrant = function () {
            $rootScope.isFormVisible = true;
            $scope.mode = "Add";
            $scope.saveText = "Save";
            ResetForm();

            $scope.editData.DateOfInspection = $filter('date')(new Date(), 'dd-MM-yyyy');
            $scope.dateofinspaction = $scope.StringToDateString($scope.editData.DateOfInspection);

        };
        //END ADD FIRE HYDRANT SPRINKLER SYSTEMFORM BUTTON


        $scope.FilterByCreDate = function (filters) {
            $scope.filters = filters;
            $scope.RefreshTable();
        };

        //BEGIN Close BFIRE HYDRANT SPRINKLER SYSTEM FORM
        $scope.CloseFireHydrantSprinkler = function () {
            $scope.mode = "Add";
            ResetForm();
            $rootScope.isFormVisible = false;
        };
        //END Close FIRE HYDRANT SPRINKLER SYSTEM  FORM


        //BEGIN RESET BFIRE HYDRANT SPRINKLER SYSTEM 
        $scope.ResetFireHydrantSprinkler = function () {

            if ($scope.mode == "Edit") {
                $scope.editData = {
                    BuildingName: $scope.storage.lastRecord.BuildingName,
                    DateOfInspection: $filter('date')($scope.storage.lastRecord.DateOfInspection, 'dd-MM-yyyy'),
                    CheckedBy: $scope.storage.lastRecord.CheckedBy,
                    Findings: $scope.storage.lastRecord.Findings,
                    RootCause: $scope.storage.lastRecord.RootCause,
                    CorrectiveActionTaken: $scope.storage.lastRecord.CorrectiveActionTaken,
                    Remark: $scope.storage.lastRecord.Remark,
                    Attachment: $scope.storage.lastRecord.Attachment
                }
                $scope.rejFiles = [];
                $scope.file = $scope.storage.lastRecord.Attachment;

            } else {
                ResetForm();
            }
        };
        //BEGIN END  FIRE HYDRANT SPRINKLER SYSTEM

        //BEGIN SAVE FIRE HYDRANT SPRINKLER SYSTEM
        $scope.CreateUpdateFireHydrant = function (FireHydrantSprinkler) {

            var IDate = $filter('date')(FireHydrantSprinkler.DateOfInspection, 'dd-MM-yyyy').split('-');
            var temp_Date = new Date(parseInt(IDate[2]), parseInt(IDate[1]) - 1, parseInt(IDate[0]), 0, 0, 0);
            FireHydrantSprinkler.DateOfInspection = $filter('date')(temp_Date, 'MM-dd-yyyy HH:mm:ss');


            FireHydrantSpprinklerService.CreateUpdateFireHydrantSprinkler(FireHydrantSprinkler, $scope.timeZone).then(function (result) {


                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        toastr.success(result.data.Message, 'Success');
                        $scope.editData = {BuildingName:"", DateOfInspection: "", CheckedBy: "", Findings: "", RootCause: "", CorrectiveActionTaken: "", Remark: "", Attachment: "" };

                        $scope.editData.DateOfInspection = $filter('date')(new Date(), 'dd-MM-yyyy');
                        $scope.dateofinspaction = $scope.StringToDateString($scope.editData.DateOfInspection);

                        $scope.fileName = "";
                        $scope.RefreshTable();
                        $scope.storage.lastRecord = {};
                        $scope.isFirstFocus = false;
                        $scope.FireHydrantSprinklerform.$setPristine();
                        if ($scope.mode === "Edit") {
                            $rootScope.isFormVisible = false;
                            $scope.saveText = "Save";
                        }
                    } else if (result.data.MessageType == 2) { // 2:Warning
                        toastr.warning(result.data.Message, 'Record already exists');
                    } else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                }
                else {
                    $rootScope.redirectToLogin();
                }
                $rootScope.IsAjaxLoading = false;
            });
        }
        //END SAVE FIRE HYDRANT SPRINKLER SYSTEM

        //BEGIN REFERESH TABLE AFTER INSERT,UPDATE AND DELETE
        $scope.RefreshTable = function () {
            $scope.tableParams.reload();
        };
        //END REFERESH TABLE AFTER INSERT,UPDATE AND DELETE

        ////BEGIN RETRIVE FIRE HYDRANT SPRINKLER SYSTEMINFORMATION (DATATABLE)   
        $scope.RetriveHydantSprinklerSystem = function () {



            $scope.tableParams = new ngTableParams({
                page: 1,
                count: 10,
                sorting: {
                    CheckedBy: 'asc'
                },
                defaultSort: 'asc'
            }, {
                total: 0,
                filterDelay: 750,
                getData: function ($defer, params) {
                    $rootScope.IsAjaxLoading = true;
                    $scope.isRecord = false;

                    FireHydrantSpprinklerService.GetFireHydrantSprinklerSystemLog($scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter().BuildingName, params.filter().DateOfInspection, params.filter().CheckedBy, params.filter().Findings, params.filter().RootCause, params.filter().CorrectiveActionTaken, params.filter().Remark, $scope.filterDate.dateRange.startDate, $scope.filterDate.dateRange.endDate).then(function (result) {

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
            });
        }

        //END RETRIVE FIRE HYDRANT SPRINKLER SYSTEM RECORD (DATATABLE)   

        //BEGIN SET VALUES BEFORE UPDATE OPERATION PERFORM
        $scope.UpdateFireHydrant = function (_FireHydrant) {
            $scope.storage.lastRecord = _FireHydrant;
            $scope.rejFiles = [];
            $rootScope.isFormVisible = true;
            $scope.mode = "Edit";
            $scope.saveText = "Update";

            $scope.editData = {
                SrNo: _FireHydrant.SrNo,
                BuildingName: _FireHydrant.BuildingName,
                DateOfInspection: $filter('date')(_FireHydrant.DateOfInspection, 'dd-MM-yyyy'),
                CheckedBy: _FireHydrant.CheckedBy,
                Findings: _FireHydrant.Findings,
                RootCause: _FireHydrant.RootCause,
                CorrectiveActionTaken: _FireHydrant.CorrectiveActionTaken,
                Remark: _FireHydrant.Remark,
                Attachment: _FireHydrant.Attachment,
                files: _FireHydrant.Attachment
            };

            $scope.dateofinspaction = _FireHydrant.DateOfInspection;
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        };
        //END SET VALUES BEFORE UPDATE OPERATION PERFORM



        //change service name and button name
        //BEGIN DELETE FIRE HYDRANT SPRINKLER SYSTEMINFORMATION
        $scope.DeleteFireHydrant = function (id) {
            $rootScope.IsAjaxLoading = true;

            FireHydrantSpprinklerService.DeleteFireHydrantSprinkler(id).then(function (result) {
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
        //END DELETE FIRE HYDRANT SPRINKLER SYSTEMINFORMATION


        //BEGIN EXPORT TO EXCEL

        //BEGIN EXPORT TO EXCEL
        $scope.ExportToExcel = function () {
            document.location.href = "../../Handler/HRDFireHydrantSprinklerSystem.ashx?timezone=" + $scope.timeZone + "&startDate=" + $filter('date')($scope.filterDate.dateRange.startDate, 'dd-MM-yyyy') + "&endDate=" + $filter('date')($scope.filterDate.dateRange.endDate, 'dd-MM-yyyy')
        };
        //BEGIN EXPORT TO EXCEL



        ///Model PopUp
        $scope.ShowFireHydrant = function (FireHydrantSprinkler) {
            var modalInstance = $modal.open({
                templateUrl: 'FireHydrantSprinkler.html',
                controller: ModalInstanceCtrl,
                resolve: {
                    FireHydrantSprinklerDetails: function () { return FireHydrantSprinkler; }
                }
            });
        }
        var ModalInstanceCtrl = function ($scope, $modalInstance, FireHydrantSprinklerDetails) {
            $scope.items = FireHydrantSprinklerDetails;
            $scope.Close = function () {
                $modalInstance.close();
            };
        };
    }
})();