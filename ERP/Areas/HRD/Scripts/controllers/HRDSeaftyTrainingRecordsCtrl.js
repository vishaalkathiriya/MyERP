/// <reference path="../libs/angular/angular.min.js" />

(function () {
    'use strict';
    angular.module("ERPApp.Controllers")
       .controller("HRDSeaftyTrainingRecordsCtrl", [
            "$scope", "$rootScope", "$timeout", "$http", "$filter", "SeaftyTrainigRecordsService", "ngTableParams", "$modal", "Upload",
           HRDSeaftyTrainingRecordsCtrl
       ]);

    function HRDSeaftyTrainingRecordsCtrl($scope, $rootScope, $timeout, $http, $filter, SeaftyTrainigRecordsService, ngTableParams, $modal, Upload) {
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
        $scope.calendarOpenDateOfTraining = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.editData.calOpenDateOfTraining = true;
        };



        $scope.$watch('date_training', function (newValue) {
            $scope.editData.DateOfTraining = $filter('date')(newValue, 'dd-MM-yyyy');
        });

        $scope.ValidateDateOfTraining = function (date_training, FireExtinguiserform) {
            if (!date_training) {
                $scope.FireExtinguiserform.txtDateOfTraining.$setValidity("invalidDateOfTraining", true);
                return;
            } else if (date_training.length == 10) {
                if ($scope.ValidateDate(date_training)) {
                    //test
                    $scope.date_training = $scope.StringToDateString(date_training);
                    $scope.FireExtinguiserform.txtDateOfTraining.$setValidity("invalidDateOfTraining", true);
                } else {
                    $scope.FireExtinguiserform.txtDateOfTraining.$setValidity("invalidDateOfTraining", false);
                }
            } else {
                $scope.FireExtinguiserform.txtDateOfTraining.$setValidity("invalidDateOfTraining", false);
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
                SubjectOfTraining: "", DateOfTraining: "", Department: "", ManagerName: "", NoOfParticipants: "", TrainersName: ""
            };
            $scope.rejFiles = [];
            $scope.editData.DateOfTraining = $filter('date')(new Date(), 'dd-MM-yyyy');
            $scope.DateOfTraining = $scope.StringToDateString($scope.editData.DateOfTraining);

            $scope.SeaftyTrainingRecordsform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        }
        //END RESET FORM

        //BEGIN ADD SEAFTY TRAINING RECORDS  RECORDS BUTTON
        $scope.AddSafetyTrainingRecords = function () {
            $rootScope.isFormVisible = true;
            $scope.mode = "Add";
            $scope.saveText = "Save";
            ResetForm();

            $scope.editData.DateOfTraining = $filter('date')(new Date(), 'dd-MM-yyyy');
            $scope.date_training = $scope.StringToDateString($scope.editData.DateOfTraining);

        };
        //END ADD SEAFTY TRAINING RECORDS FORM BUTTON


        $scope.FilterByCreDate = function (filters) {
            $scope.filters = filters;
            $scope.RefreshTable();
        };

        //BEGIN Close SEAFTY TRAINING RECORDS  FORM
        $scope.CloseSeaftyTrainingRecords = function () {
            $scope.mode = "Add";
            ResetForm();
            $rootScope.isFormVisible = false;
        };
        //END Close SEAFTY TRAINING RECORDS   FORM


        //BEGIN RESET SEAFTY TRAINING RECORDS  
        $scope.ResetSeaftyTraningRecords = function () {

            if ($scope.mode == "Edit") {
                $scope.editData = {
                    
                    SubjectOfTraining: $scope.storage.lastRecord.SubjectOfTraining,
                    DateOfTraining: $filter('date')($scope.storage.lastRecord.DateOfTraining, 'dd-MM-yyyy'),
                    Department: $scope.storage.lastRecord.Department,
                    ManagerName: $scope.storage.lastRecord.ManagerName,
                    NoOfParticipants: $scope.storage.lastRecord.NoOfParticipants,
                    TrainersName: $scope.storage.lastRecord.TrainersName,
                    Attachment: $scope.storage.lastRecord.Attachment
                }
                $scope.rejFiles = [];
                $scope.file = $scope.storage.lastRecord.Attachment;

            } else {
                ResetForm();
            }
        };
        //BEGIN END  SEAFTY TRAINING RECORDS 

        //BEGIN SAVE SEAFTY TRAINING RECORDS 
        $scope.CreateUpdateSeaftyTrainingRecords = function (SeaftyTrainingRecords) {

            var IDate = $filter('date')(SeaftyTrainingRecords.DateOfTraining, 'dd-MM-yyyy').split('-');
            var temp_Date = new Date(parseInt(IDate[2]), parseInt(IDate[1]) - 1, parseInt(IDate[0]), 0, 0, 0);
            SeaftyTrainingRecords.DateOfTraining = $filter('date')(temp_Date, 'MM-dd-yyyy HH:mm:ss');


            SeaftyTrainigRecordsService.CreateUpdateSeaftyTrainigRecords(SeaftyTrainingRecords, $scope.timeZone).then(function (result) {


                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        toastr.success(result.data.Message, 'Success');
                        $scope.editData = { SubjectOfTraining: "", DateOfTraining: "", Department: "", ManagerName: "", NoOfParticipants: "", TrainersName: "", Attachment: "" };

                        $scope.editData.DateOfTraining = $filter('date')(new Date(), 'dd-MM-yyyy');
                        $scope.date_training = $scope.StringToDateString($scope.editData.DateOfTraining);

                        $scope.fileName = "";
                        $scope.RefreshTable();
                        $scope.storage.lastRecord = {};
                        $scope.isFirstFocus = false;
                        $scope.SeaftyTrainingRecordsform.$setPristine();
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
        //END SAVE SEAFTY TRAINING RECORDS 

        //BEGIN REFERESH TABLE AFTER INSERT,UPDATE AND DELETE
        $scope.RefreshTable = function () {
            $scope.tableParams.reload();
        };
        //END REFERESH TABLE AFTER INSERT,UPDATE AND DELETE

        //BEGIN RETRIVE SEAFTY TRAINING RECORDS INFORMATION (DATATABLE)   
        $scope.RetriveSeaftyTrainigRecords = function () {



            $scope.tableParams = new ngTableParams({
                page: 1,
                count: 10,
                sorting: {
                    SubjectOfTraining: 'asc'
                },
                defaultSort: 'asc'
            }, {
                total: 0,
                filterDelay: 750,
                getData: function ($defer, params) {
                    $rootScope.IsAjaxLoading = true;
                    $scope.isRecord = false;

                    SeaftyTrainigRecordsService.GetSeaftyTrainingRecords($scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter().SubjectOfTraining, params.filter().DateOfTraining, params.filter().Department, params.filter().ManagerName, params.filter().NoOfParticipants, params.filter().TrainersName, $scope.filterDate.dateRange.startDate, $scope.filterDate.dateRange.endDate).then(function (result) {

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

        //END RETRIVE SEAFTY TRAINING RECORDS  RECORD (DATATABLE)   

        //BEGIN SET VALUES BEFORE UPDATE OPERATION PERFORM
        $scope.UpdateSeaftyTrainingRecords = function (_SeaftyTraining) {
            $scope.storage.lastRecord = _SeaftyTraining;
            $scope.rejFiles = [];
            $rootScope.isFormVisible = true;
            $scope.mode = "Edit";
            $scope.saveText = "Update";

            $scope.editData = {
                SrNo: _SeaftyTraining.SrNo,
                SubjectOfTraining: _SeaftyTraining.SubjectOfTraining,
                DateOfTraining: $filter('date')(_SeaftyTraining.DateOfTraining, 'dd-MM-yyyy'),
                Department: _SeaftyTraining.Department,
                ManagerName: _SeaftyTraining.ManagerName,
                NoOfParticipants: _SeaftyTraining.NoOfParticipants,
                TrainersName: _SeaftyTraining.TrainersName,
                Attachment: _SeaftyTraining.Attachment,
                files: _SeaftyTraining.Attachment
            };

            $scope.date_training = _SeaftyTraining.DateOfTraining;
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        };
        //END SET VALUES BEFORE UPDATE OPERATION PERFORM



        //change service name and button name
        //BEGIN DELETE SEAFTY TRAINING RECORDS INFORMATION
        $scope.DeleteSeaftyTrainingRecords = function (id) {
            $rootScope.IsAjaxLoading = true;

            SeaftyTrainigRecordsService.DeleteSeaftyTrainig(id).then(function (result) {
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
        //END DELETE SEAFTY TRAINING RECORDS INFORMATION


        //BEGIN EXPORT TO EXCEL

        //BEGIN EXPORT TO EXCEL
        $scope.ExportToExcel = function () {
            document.location.href = "../../Handler/HRDSeftyTrainingRecords.ashx?timezone=" + $scope.timeZone + "&startDate=" + $filter('date')($scope.filterDate.dateRange.startDate, 'dd-MM-yyyy') + "&endDate=" + $filter('date')($scope.filterDate.dateRange.endDate, 'dd-MM-yyyy')
        };
        //BEGIN EXPORT TO EXCEL



        ///Model PopUp
        $scope.ShowSeaftyTrainingRecords = function (SeaftyTrainingRecords) {
            var modalInstance = $modal.open({
                templateUrl: 'SeaftyTrainingRecords.html',
                controller: ModalInstanceCtrl,
                resolve: {
                    SeaftyTrainingRecordsDetails: function () { return SeaftyTrainingRecords; }
                }
            });
        }
        var ModalInstanceCtrl = function ($scope, $modalInstance, SeaftyTrainingRecordsDetails) {
            $scope.items = SeaftyTrainingRecordsDetails;
            $scope.Close = function () {
                $modalInstance.close();
            };
        };
    }
})();