/// <reference path="../libs/angular/angular.min.js" />

(function () {
    'use strict';
    angular.module("ERPApp.Controllers")
       .controller("HRDChemicalStorageInspactionCtrl", [
            "$scope", "$rootScope", "$timeout", "$http", "$filter", "ChemicalStorageInspactionService", "ngTableParams", "$modal", "Upload",
           HRDChemicalStorageInspactionCtrl
       ]);

    function HRDChemicalStorageInspactionCtrl($scope, $rootScope, $timeout, $http, $filter, ChemicalStorageInspactionService, ngTableParams, $modal, Upload) {
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
        //GETING NG FILE UPLODER
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
        //END NG FILE UPLODER

        //BEGIN RESET FORM
        function ResetForm() {
            $scope.editData = {
                DateOfInspection: "", CheckedyBy: "", Findings: "", RootCause: "", CorrectiveAction: "", Remark: ""
            };
            $scope.rejFiles = [];
            $scope.editData.DateOfInspection = $filter('date')(new Date(), 'dd-MM-yyyy');
            $scope.DateOfInspection = $scope.StringToDateString($scope.editData.DateOfInspection);

            $scope.ChemicalInspactionLogform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        }
        //END RESET FORM

        //BEGIN ADD CHEMICAL STORAGE INSPACTION LOG  RECORDS BUTTON
        $scope.AddChemicalInspaction = function () {
            $rootScope.isFormVisible = true;
            $scope.mode = "Add";
            $scope.saveText = "Save";
            ResetForm();

            $scope.editData.DateOfInspection = $filter('date')(new Date(), 'dd-MM-yyyy');
            $scope.dateofinspaction = $scope.StringToDateString($scope.editData.DateOfInspection);

        };
        //END ADD CHEMICAL STORAGE INSPACTION LOG FORM BUTTON


        $scope.FilterByCreDate = function (filters) {
            $scope.filters = filters;
            $scope.RefreshTable();
        };

        //BEGIN Close BCHEMICAL STORAGE INSPACTION LOG  FORM
        $scope.CloseChemicalInspactionLog = function () {
            $scope.mode = "Add";
            ResetForm();
            $rootScope.isFormVisible = false;
        };
        //END Close CHEMICAL STORAGE INSPACTION LOG   FORM


        //BEGIN RESET BCHEMICAL STORAGE INSPACTION LOG  
        $scope.ResetChemicalInspactionLog = function () {

            if ($scope.mode == "Edit") {
                $scope.editData = {
                    DateOfInspection: $filter('date')($scope.storage.lastRecord.DateOfInspection, 'dd-MM-yyyy'),
                    CheckedyBy: $scope.storage.lastRecord.CheckedyBy,
                    Findings: $scope.storage.lastRecord.Findings,
                    RootCause: $scope.storage.lastRecord.RootCause,
                    CorrectiveAction: $scope.storage.lastRecord.CorrectiveAction,
                    Remark: $scope.storage.lastRecord.Remark,
                    Attachment: $scope.storage.lastRecord.Attachment
                }
                $scope.file = $scope.storage.lastRecord.Attachment;
                $scope.rejFiles = [];
            } else {
                ResetForm();
            }
        };
        //BEGIN END  CHEMICAL STORAGE INSPACTION LOG 

        //BEGIN SAVE CHEMICAL STORAGE INSPACTION LOG 
        $scope.CreateUpdateChemicalInspaction = function (ChemicalStorageInspaction) {
            
            var IDate = $filter('date')(ChemicalStorageInspaction.DateOfInspection, 'dd-MM-yyyy').split('-');
            var temp_Date = new Date(parseInt(IDate[2]), parseInt(IDate[1]) - 1, parseInt(IDate[0]), 0, 0, 0);
            ChemicalStorageInspaction.DateOfInspection = $filter('date')(temp_Date, 'MM-dd-yyyy HH:mm:ss');


            ChemicalStorageInspactionService.CreateUpdateChemicalInspactionRecordLog(ChemicalStorageInspaction, $scope.timeZone).then(function (result) {


                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        toastr.success(result.data.Message, 'Success');
                        $scope.editData = { DateOfInspection: "", CheckedyBy: "", Findings: "", RootCause: "", CorrectiveAction: "", Remark: "", Attachment: "" };

                        $scope.editData.DateOfInspection = $filter('date')(new Date(), 'dd-MM-yyyy');
                        $scope.dateofinspaction = $scope.StringToDateString($scope.editData.DateOfInspection);

                        $scope.fileName = "";
                        $scope.RefreshTable();
                        $scope.storage.lastRecord = {};
                        $scope.isFirstFocus = false;
                        $scope.ChemicalInspactionLogform.$setPristine();
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
        //END SAVE CHEMICAL STORAGE INSPACTION LOG 

        //BEGIN REFERESH TABLE AFTER INSERT,UPDATE AND DELETE
        $scope.RefreshTable = function () {
            $scope.tableParams.reload();
        };
        //END REFERESH TABLE AFTER INSERT,UPDATE AND DELETE

        //BEGIN RETRIVE CHEMICAL STORAGE INSPACTION LOG INFORMATION (DATATABLE)   
        $scope.RetriveChemicalInspaction = function () {



            $scope.tableParams = new ngTableParams({
                page: 1,
                count: 10,
                sorting: {
                    CheckedyBy: 'asc'
                },
                defaultSort: 'asc'
            }, {
                total: 0,
                filterDelay: 750,
                getData: function ($defer, params) {
                    $rootScope.IsAjaxLoading = true;
                    $scope.isRecord = false;

                    ChemicalStorageInspactionService.GetChemicalInspactionLogRecord($scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter().DateOfInspection, params.filter().CheckedyBy, params.filter().Findings, params.filter().RootCause, params.filter().CorrectiveAction, params.filter().Remark, $scope.filterDate.dateRange.startDate, $scope.filterDate.dateRange.endDate).then(function (result) {

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

        //END RETRIVE CHEMICAL STORAGE INSPACTION LOG  RECORD (DATATABLE)   

        //BEGIN SET VALUES BEFORE UPDATE OPERATION PERFORM
        $scope.UpdateChemicalInspaction = function (_ChemicalInspaction) {
            $scope.storage.lastRecord = _ChemicalInspaction;
            $scope.rejFiles = [];
            $rootScope.isFormVisible = true;
            $scope.mode = "Edit";
            $scope.saveText = "Update";

            $scope.editData = {
                SrNo: _ChemicalInspaction.SrNo,
                DateOfInspection: $filter('date')(_ChemicalInspaction.DateOfInspection, 'dd-MM-yyyy'),
                CheckedyBy: _ChemicalInspaction.CheckedyBy,
                Findings: _ChemicalInspaction.Findings,
                RootCause: _ChemicalInspaction.RootCause,
                CorrectiveAction: _ChemicalInspaction.CorrectiveAction,
                Remark: _ChemicalInspaction.Remark,
                Attachment: _ChemicalInspaction.Attachment,
                files: _ChemicalInspaction.Attachment
            };
            $scope.dateofinspaction = _ChemicalInspaction.DateOfInspection;
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        };
        //END SET VALUES BEFORE UPDATE OPERATION PERFORM



        //change service name and button name
        //BEGIN DELETE CHEMICAL STORAGE INSPACTION LOG INFORMATION
        $scope.DeleteChemicalStorageInspactionLog = function (id) {
            $rootScope.IsAjaxLoading = true;

            ChemicalStorageInspactionService.DeleteChemicalInspactionLog(id).then(function (result) {
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
        //END DELETE CHEMICAL STORAGE INSPACTION LOG INFORMATION


        //BEGIN EXPORT TO EXCEL

        //BEGIN EXPORT TO EXCEL
        $scope.ExportToExcel = function () {
            document.location.href = "../../Handler/HRDChemicalStorageInspactionLog.ashx?timezone=" + $scope.timeZone + "&startDate=" + $filter('date')($scope.filterDate.dateRange.startDate, 'dd-MM-yyyy') + "&endDate=" + $filter('date')($scope.filterDate.dateRange.endDate, 'dd-MM-yyyy')
        };
        //BEGIN EXPORT TO EXCEL



        ///Model PopUp
        $scope.ShowChemicalStorageInspactionLog = function (ChemicalStorageInspaction) {
            var modalInstance = $modal.open({
                templateUrl: 'ChemicalStorageInspaction.html',
                controller: ModalInstanceCtrl,
                resolve: {
                    ChemicalStorageInspactionDetails: function () { return ChemicalStorageInspaction; }
                }
            });
        }
        var ModalInstanceCtrl = function ($scope, $modalInstance, ChemicalStorageInspactionDetails) {
            $scope.items = ChemicalStorageInspactionDetails;
            $scope.Close = function () {
                $modalInstance.close();
            };
        };
    }
})();