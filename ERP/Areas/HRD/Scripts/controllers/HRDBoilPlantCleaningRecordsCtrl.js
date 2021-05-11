/// <reference path="../libs/angular/angular.min.js" />

(function () {
    'use strict';
    angular.module("ERPApp.Controllers")
       .controller("HRDBoilPlantCleaningCtrl", [
            "$scope", "$rootScope", "$timeout", "$http", "$filter", "BoilPlantCleaningService", "ngTableParams", "$modal", "Upload",
           HRDBoilPlantCleaningCtrl
       ]);

    function HRDBoilPlantCleaningCtrl($scope, $rootScope, $timeout, $http, $filter, BoilPlantCleaningService, ngTableParams, $modal, Upload) {
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
        $scope.calendarOpenDateOfCleaining = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.editData.calopenCleDate = true;
           
        };

        

        $scope.$watch('DateOfCleaining', function (newValue) {
            $scope.editData.DateOfCleaining = $filter('date')(newValue, 'dd-MM-yyyy');
        });

        $scope.ValidateDateOfCleaining = function (DateOfCleaining, BoliPlantCleaningform) {
            if (!DateOfCleaining) {
                $scope.BoliPlantCleaningform.txtDateOfCleaining.$setValidity("invalidIssueDate", true);
                return;
            } else if (DateOfCleaining.length == 10) {
                if ($scope.ValidateDate(DateOfCleaining)) {
                    //test
                    $scope.DateOfCleaining = $scope.StringToDateString(DateOfCleaining);
                    $scope.BoliPlantCleaningform.DateOfCleaining.$setValidity("invalid Cleaining Date  ", true);
                } else {
                    $scope.BoliPlantCleaningform.DateOfCleaining.$setValidity("invalid Cleaining Date", false);
                }
            } else {
                $scope.BoliPlantCleaningform.DateOfCleaining.$setValidity("invalid Cleaining Date", false);
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
                BoilPlantLocation: "", NameOfCleaner: "", PlantIncharge: "", Remark: "", Attachment: ""
            };
            $scope.rejFiles = [];
            $scope.editData.DateOfCleaining = $filter('date')(new Date(), 'dd-MM-yyyy');
            $scope.DateOfCleaining = $scope.StringToDateString($scope.editData.DateOfCleaining);

            $scope.BoliPlantCleaningform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        }
        //END RESET FORM

        //BEGIN ADD BOIL PLANT CLEANING RECORDS BUTTON
        $scope.AddBoilPlantCleaning = function () {
            $rootScope.isFormVisible = true;
            $scope.mode = "Add";
            $scope.saveText = "Save";
            ResetForm(); 

            $scope.editData.DateOfCleaining = $filter('date')(new Date(), 'dd-MM-yyyy');
            $scope.DateOfCleaining = $scope.StringToDateString($scope.editData.DateOfCleaining);

        };
        //END ADD FIRST AID LOG BOOK FORM BUTTON


        $scope.FilterByCreDate = function (filters) {
            $scope.filters = filters;
            $scope.RefreshTable();
        };

        //BEGIN Close BOIL PLANT CLANING FORM
        $scope.CloseBoliPlantCleaning = function () {
            $scope.mode = "Add";
            ResetForm();
            $rootScope.isFormVisible = false;
        };
        //END Close BOIL PLANT CLANING  FORM


        //BEGIN RESET BOIL PLANT
        $scope.ResetBoliPlantCleaning = function () {

            if ($scope.mode == "Edit") {
                $scope.editData = {
                    BoilPlantLocation: $scope.storage.lastRecord.BoilPlantLocation,
                    DateOfCleaining: $filter('date')($scope.storage.lastRecord.DateOfCleaining, 'dd-MM-yyyy'),
                    NameOfCleaner: $scope.storage.lastRecord.NameOfCleaner,
                    PlantIncharge: $scope.storage.lastRecord.PlantIncharge,
                    Remark: $scope.storage.lastRecord.Remark,
                    Attachment: $scope.storage.lastRecord.Attachment
                }
                $scope.rejFiles = [];
                $scope.file = $scope.storage.lastRecord.Attachment;

            } else {
                ResetForm();
            }
        };
        //BEGIN END  BOIL PLANT

        //BEGIN SAVE FIRST AID LOG BOOK INFORMATION
        $scope.CreateUpdatBoilPlantCleaning= function (BoilplantCleaning) {
            var IDate = $filter('date')(BoilplantCleaning.DateOfCleaining, 'dd-MM-yyyy').split('-');
            var temp_Date = new Date(parseInt(IDate[2]), parseInt(IDate[1]) - 1, parseInt(IDate[0]), 0, 0, 0);
            BoilplantCleaning.DateOfCleaining = $filter('date')(temp_Date, 'MM-dd-yyyy HH:mm:ss');


            BoilPlantCleaningService.CreateUpdatBoilPlantCleaning(BoilplantCleaning, $scope.timeZone).then(function (result) {

                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        toastr.success(result.data.Message, 'Success');
                        $scope.editData = { BoilPlantLocation: "", NameOfCleaner: "", PlantIncharge: "", Remark: "", Attachment: "" };

                        $scope.editData.DateOfCleaining = $filter('date')(new Date(), 'dd-MM-yyyy');
                        $scope.DateOfCleaining = $scope.StringToDateString($scope.editData.DateOfCleaining);

                        $scope.fileName = "";
                        $scope.RefreshTable();
                        $scope.storage.lastRecord = {};
                        $scope.isFirstFocus = false;
                        $scope.BoliPlantCleaningform.$setPristine();
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
        //END SAVE First Aid INFORMATION

        //BEGIN REFERESH TABLE AFTER INSERT,UPDATE AND DELETE
        $scope.RefreshTable = function () {
            $scope.tableParams.reload();
        };
        //END REFERESH TABLE AFTER INSERT,UPDATE AND DELETE

        //BEGIN RETRIVE BOIL PLANT CLEANING INFORMATION (DATATABLE)   
        $scope.RetriveBoilPlant = function () {


            $scope.tableParams = new ngTableParams({
                page: 1,
                count: 10,
                sorting: {
                    BoilPlantLocation: 'asc'
                },
                defaultSort: 'asc'
            }, {
                total: 0,
                filterDelay: 750,
                getData: function ($defer, params)
                {
                    $rootScope.IsAjaxLoading = true;
                    $scope.isRecord = false;
                    BoilPlantCleaningService.GetBoilPlant($scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter().BoilPlantLocation, params.filter().DateOfCleaining, params.filter().NameOfCleaner, params.filter().PlantIncharge, params.filter().Remark, $scope.filterDate.dateRange.startDate, $scope.filterDate.dateRange.endDate).then(function (result)
                    {
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
            
        //END RETRIVE BOIL PLANT CLEANING RECORD (DATATABLE)   

        //BEGIN SET VALUES BEFORE UPDATE OPERATION PERFORM
        $scope.UpdateBoilPlantCleaning = function (_BoilPlant) {
            $scope.storage.lastRecord = _BoilPlant;
            $scope.rejFiles = [];
            $rootScope.isFormVisible = true;
            $scope.mode = "Edit";
            $scope.saveText = "Update";

            $scope.editData = {
                SrNo: _BoilPlant.SrNo,
                BoilPlantLocation: _BoilPlant.BoilPlantLocation,
                DateOfCleaining: $filter('date')(_BoilPlant.DateOfCleaining, 'dd-MM-yyyy'),
                NameOfCleaner: _BoilPlant.NameOfCleaner,
                PlantIncharge: _BoilPlant.PlantIncharge,
                Remark: _BoilPlant.Remark,
                Attachment: _BoilPlant.Attachment,
                files: _BoilPlant.Attachment
            };

            $scope.DateOfCleaining = _BoilPlant.DateOfCleaining;
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        };
        //END SET VALUES BEFORE UPDATE OPERATION PERFORM



        //change service name and button name
        //BEGIN DELETE BOIL PLANT CLEANING RECORD INFORMATION
        $scope.DeleteBoilPlantCleaning = function (id) {
            $rootScope.IsAjaxLoading = true;

            BoilPlantCleaningService.DeleteBoilPlantCleaning(id).then(function (result) {
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
        //END DELETE BOIL PLANT CLEANING RECORD INFORMATION


        //BEGIN EXPORT TO EXCEL

        //BEGIN EXPORT TO EXCEL
        $scope.ExportToExcel = function () {
            document.location.href = "../../Handler/HRDBoilPlantCleaningRecords.ashx?timezone=" + $scope.timeZone + "&startDate=" + $filter('date')($scope.filterDate.dateRange.startDate, 'dd-MM-yyyy') + "&endDate=" + $filter('date')($scope.filterDate.dateRange.endDate, 'dd-MM-yyyy')
        };
        //BEGIN EXPORT TO EXCEL



        ///Model PopUp
        $scope.ShowBoilPlantCleaning = function (BoilPlantCleaning) {
            var modalInstance = $modal.open({
                templateUrl: 'BoilPlantCleaning.html',
                controller: ModalInstanceCtrl,
                resolve: {
                    BoilPlantCleaningDetails: function () { return BoilPlantCleaning; }
                }
            });
        }
        var ModalInstanceCtrl = function ($scope, $modalInstance, BoilPlantCleaningDetails) {
            $scope.items = BoilPlantCleaningDetails;
            $scope.Close = function () {
                $modalInstance.close();
            };
        };
    }
})();