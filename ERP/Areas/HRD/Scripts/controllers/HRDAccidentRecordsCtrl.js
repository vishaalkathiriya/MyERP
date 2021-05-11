/// <reference path="../libs/angular/angular.min.js" />

(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("AccidentRecordsCtrl", [
            "$scope", "$rootScope", "$timeout", "$http", "$filter", "AccidentRecordsService", "ngTableParams", "$q", "$modal", "Upload",
            AccidentRecordsCtrl
        ]);

    //Main controller function
    function AccidentRecordsCtrl($scope, $rootScope, $timeout, $http, $filter, AccidentRecordsService, ngTableParams, $q, $modal, Upload) {
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

        //BEGIN FILE UPLODER
        $scope.upload = function (files) {
            Upload.upload({
                url: "../Handler/FileUpload.ashx",
                file: files
            }).progress(function (evt) {
                var progressPercentage = parseInt(100.0 * evt.loaded / evt.total);

            }).success(function (data, status, headers, config) {

                $scope.editData.Attachment = data;
            });
        }
        $scope.$watch('rejFiles', function () {
            if (angular.isArray($scope.rejFiles)) {
                $scope.rejFiles.forEach(function (file) {
                    toastr.error('The selected file is not a valid file. Please select a valid *.xls, *.pdf or *.doc file and try again ', 'Invalid selected file');
                });
            }
        });
        //BEGIN FILE UPLODER
        
        $scope.FilterByCreDate = function (filters) {
            $scope.filters = filters;
            $scope.RefreshTable();
        };

        //BEGIN RESET FORM
        function ResetForm() {


            $scope.editData = {
                TypeOfAccident: "", Department: "", ManagerName: "", NameOfInjuredPerson: "", RootCauseOfAccident: "", NoOfCasualities: "", CorrectiveActionTaken: "", Hospitalized: ""
            };
            $scope.rejFiles = [];
            $scope.AccidentRecordsform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
            $scope.isFirstFocus = true;
            });
        }
        //END RESET FORM

        //BEGIN ADD ACCIDENT FORM BUTTON
        $scope.AddAccidentRecords = function () {
            $rootScope.isFormVisible = true;
            $scope.mode = "Add";
            $scope.saveText = "Save";
            ResetForm(); //reset form
        };
        //END  ADD ACCIDENT FORM BUTTON

       

        //BEGIN CLOSE ACCIDENT FORM
        $scope.CloseAccidentRecords = function () {
            $scope.mode = "Add";
            ResetForm();
            $rootScope.isFormVisible = false;
        };
        //END CLOSE ACCIDENT FORM

        //BEGIN RESET ACCIDENT FORM
        $scope.ResetAccidentRecords = function () {

            if ($scope.mode == "Edit") {
                $scope.editData = {
                    TypeOfAccident: $scope.storage.lastRecord.TypeOfAccident,
                    Department: $scope.storage.lastRecord.Department,
                    ManagerName: $scope.storage.lastRecord.ManagerName,
                    NameOfInjuredPerson: $scope.storage.lastRecord.NameOfInjuredPerson,
                    RootCauseOfAccident: $scope.storage.lastRecord.RootCauseOfAccident,
                    NoOfCasualities: $scope.storage.lastRecord.NoOfCasualities,
                    CorrectiveActionTaken: $scope.storage.lastRecord.CorrectiveActionTaken,
                    Attachment: $scope.storage.lastRecord.Attachment
                }
                $scope.rejFiles = [];
           
                if ($scope.storage.lastRecord.Hospitalized == true) {
                    $scope.editData.Hospitalized = $scope.storage.lastRecord.Hospitalized;
                    $scope.editData.NameOfHospital = $scope.storage.lastRecord.NameOfHospital;
                    $scope.editData.TreatmentExpenses = $scope.storage.lastRecord.TreatmentExpenses;
                }
            } else { //mode == add
                ResetForm();
            }
        };
        //BEGIN RESET ACCIDENT FORM

        //BEGIN SAVE ACCIDENT INFORMATION
        $scope.CreateUpdateAccidentRecords = function (AccidentRecords) {

           AccidentRecordsService.CreateUpdateAccidentRecords(AccidentRecords, $scope.timeZone).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        toastr.success(result.data.Message, 'Success');
                        $scope.editData = { TypeOfAccident: "", Department: "", ManagerName: "", NameOfInjuredPerson: "", RootCauseOfAccident: "", NoOfCasualities: "", CorrectiveActionTaken: "", Hospitalized: "", Attachment: "" };
                        $scope.RefreshTable();
                        $scope.storage.lastRecord = {};
                        $scope.isFirstFocus = false;
                        $scope.AccidentRecordsform.$setPristine();
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
        //END SAVE Accident INFORMATION


        //BEGIN REFERESH TABLE AFTER INSERT,UPDATE AND DELETE
        $scope.RefreshTable = function () {
            $scope.tableParams.reload();
        };
        ////END REFERESH TABLE AFTER INSERT,UPDATE AND DELETE
        //function validateHospitalized() {
        //    if ($scope.editData.Hospitalized) {
        //        if ($scope.editData.NameOfHospital == "" || $scope.editData.TreatmentExpenses == "")
        //        {
        //            return true;
        //        }

        //    }
        //}

        //BEGIN RETRIVE ACCIDENT INFORMATION (DATATABLE)
        $scope.RetrieveAccidentRecords = function () {
            $scope.tableParams = new ngTableParams({
                page: 1,
                count: 10,
                sorting: {
                    TypeOfAccident: 'asc'
                },
                defaultSort: 'asc'
            }, {
                total: 0,
                filterDelay: 750,
                getData: function ($defer, params) {
                    $rootScope.IsAjaxLoading = true;
                    $scope.isRecord = false;

                    console.log($scope.filterDate.dateRange.startDate +"***" + $scope.filterDate.dateRange.endDate)

                    AccidentRecordsService.GetAccidentRecords($scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter().TypeOfAccident, params.filter().Department, params.filter().ManagerName, params.filter().NameOfInjuredPerson, params.filter().RootCauseOfAccident, params.filter().NoOfCasualities, params.filter().CorrectiveActionTaken, params.filter().Hospitalized, $scope.filterDate.dateRange.startDate, $scope.filterDate.dateRange.endDate).then(function (result) {
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
        //END RETRIVE ACCIDENT INFORMATION


        //BEGIN DELETE ACCIDENT INFORMATION
        $scope.DeleteAccidentsRecords = function (id) {
            $rootScope.IsAjaxLoading = true;
            AccidentRecordsService.DeleteAccidentsRecords(id).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) {  //1:Success
                        toastr.success(result.data.Message, 'Success');
                        $scope.RefreshTable();
                    } else if (result.data.MessageType == 2) { // 2:Warning
                        toastr.warning(result.data.Message, 'Warning!');
                    } else {  // 0:Error
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
                $rootScope.IsAjaxLoading = false;
            });
        };
        //END DELETE ACCIDENT INFORMATION


 
        //BEGIN SET VALUES BEFORE UPDATE OPERATION PERFORM
        $scope.UpdateAccidentRecords = function (_accident) {
            $scope.storage.lastRecord = _accident;
            $scope.rejFiles = [];
            $rootScope.isFormVisible = true;
            $scope.mode = "Edit";
            $scope.saveText = "Update";

            if (_accident.Hospitalized == true) {

                $scope.editData = {
                    SrNo: _accident.SrNo,
                    TypeOfAccident: _accident.TypeOfAccident,
                    Department: _accident.Department,
                    ManagerName: _accident.ManagerName,
                    NameOfInjuredPerson: _accident.NameOfInjuredPerson,
                    RootCauseOfAccident: _accident.RootCauseOfAccident,
                    NoOfCasualities: _accident.NoOfCasualities,
                    CorrectiveActionTaken: _accident.CorrectiveActionTaken,
                    NameOfHospital: _accident.NameOfHospital,
                    TreatmentExpenses: _accident.TreatmentExpenses,
                    Attachment: _accident.Attachment,
                    Hospitalized: true
                }
            } else {

                $scope.editData = {
                    SrNo: _accident.SrNo,
                    TypeOfAccident: _accident.TypeOfAccident,
                    Department: _accident.Department,
                    ManagerName: _accident.ManagerName,
                    NameOfInjuredPerson: _accident.NameOfInjuredPerson,
                    RootCauseOfAccident: _accident.RootCauseOfAccident,
                    NoOfCasualities: _accident.NoOfCasualities,
                    CorrectiveActionTaken: _accident.CorrectiveActionTaken,
                    Attachment: _accident.Attachment
                }
            }
            $scope.issueDate = _accident.ChequeIssueDate;
            $scope.fileName = _accident.Attachment;
            $scope.AccidentRecordsform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        };
        //END SET VALUES BEFORE UPDATE OPERATION PERFORM

        //BEGIN EXPORT TO EXCEL
        $scope.ExportToExcel = function () {
            document.location.href = "../../Handler/HRDAccidentRecords.ashx?timezone=" + $scope.timeZone + "&startDate=" + $filter('date')($scope.filterDate.dateRange.startDate, 'dd-MM-yyyy') + "&endDate=" + $filter('date')($scope.filterDate.dateRange.endDate, 'dd-MM-yyyy')
        }
        //END EXPORT TO EXCEL

        //Show Accident Records

        $scope.ShowAccidentRecords = function (AccidentRecords) {
            var modalInstance = $modal.open({
                templateUrl: 'AccidentRecords.html',
                controller: AccidentRecordsCtrl,
                resolve: {
                    AccidentRecordsDetails: function () { return AccidentRecords; }
                }
            });
        }
        var AccidentRecordsCtrl = function ($scope, $modalInstance, AccidentRecordsDetails) {
            $scope.items = AccidentRecordsDetails;
            $scope.Close = function () {
                $modalInstance.close();
            };
        };




        $scope.FilterHospitalized = function (column) {

            var def = $q.defer(),
            arr = [
                { id: "Y", title: "Yes" },
                { id: "N", title: "No" }
            ];
            def.resolve(arr);
            return def;
        };
    }

})();
