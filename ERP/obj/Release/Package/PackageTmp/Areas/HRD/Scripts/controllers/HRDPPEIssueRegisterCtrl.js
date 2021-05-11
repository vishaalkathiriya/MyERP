/// <reference path="../libs/angular/angular.min.js" />


(function () {
    'use strict';
    angular.module("ERPApp.Controllers")
       .controller("PPEIssueRegisterCtrl", [
            "$scope", "$rootScope", "$timeout", "$http", "$filter", "PPEIssueRegisterService", "ngTableParams", "$modal", "Upload",
           PPEIssueRegisterCtrl
       ]);

    function PPEIssueRegisterCtrl($scope, $rootScope, $timeout, $http, $filter, PPEIssueRegisterService, ngTableParams, $modal, Upload) {
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
        //END FILE UPLODER

        //BEGIN RESET FORM
        function ResetForm() {
            $scope.editData = {
                NameOfIssuer: "", NameOfRecievr: "", TypeOfPPE: "", Quanity: "", Department: "", ManagerName: "", Price: "", Remarks: "", Attachment: ""
            };
            $scope.rejFiles = [];
            $scope.PPEIssueRegisterform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
            $scope.isFirstFocus = true;
            });
        }
        //END RESET FORM



        //BEGIN ADD PPEIssueRegisterform BUTTON
        $scope.AddPPEIssueRegister = function () {
          
            $rootScope.isFormVisible = true;
            $scope.mode = "Add";
            $scope.saveText = "Save";
            ResetForm(); //reset form
        };
        //END ADD PPEIssueRegisterform BUTTON

  
        $scope.FilterByCreDate = function (filters) {
            $scope.filters = filters;
            $scope.RefreshTable();
        };

       

        //BEGIN CLOSE PPEIssueRegister FORM
        $scope.ClosePPEIssueRegister = function () {
            $scope.mode = "Add";
            ResetForm();
            $rootScope.isFormVisible = false;
        };
        //END CLOSE PPEIssueRegister FORM

        //BEGIN RESET SOCIAL WELFARE EXPENSE FORM
        $scope.ResetPPEIssueRegister = function () {
        if ($scope.mode == "Edit") {
                $scope.editData = {
                   
                    NameOfIssuer: $scope.storage.lastRecord.NameOfIssuer,
                    NameOfRecievr: $scope.storage.lastRecord.NameOfRecievr,
                    TypeOfPPE: $scope.storage.lastRecord.TypeOfPPE,
                    Quanity: $scope.storage.lastRecord.Quanity,
                    Department: $scope.storage.lastRecord.Department,
                    ManagerName: $scope.storage.lastRecord.ManagerName,
                    Price: $scope.storage.lastRecord.Price,
                    Remarks: $scope.storage.lastRecord.Remarks,
                    Attachment: $scope.storage.lastRecord.Attachment
                }
                $scope.rejFiles = [];
                $scope.file = $scope.storage.lastRecord.Attachment;

            } else { //mode == add
                ResetForm();
            }
        };
        //BEGIN RESET PPEIssueRegister FORM


        //BEGIN SAVE PPEIssueRegister INFORMATION
        $scope.CreateUpdatePPEIssueRegister = function (PPEIssueRegister) {
            PPEIssueRegisterService.CreateUpdatePPEIssueRegister(PPEIssueRegister, $scope.timeZone).then(function (result) {

                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        toastr.success(result.data.Message, 'Success');
                        $scope.editData = { NameOfIssuer: "", NameOfRecievr: "", TypeOfPPE: "", Quanity: "", Department: "", ManagerName: "", Price: "", Remarks: "",Attachment: "" };
                       
                        $scope.RefreshTable();
                        $scope.storage.lastRecord = {};
                        $scope.isFirstFocus = false;
                        $scope.PPEIssueRegisterform.$setPristine();
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
        //END SAVE PPEIssueRegister

        //BEGIN REFERESH TABLE AFTER INSERT,UPDATE AND DELETE
        $scope.RefreshTable = function () {
            $scope.tableParams.reload();
        };
        //END REFERESH TABLE AFTER INSERT,UPDATE AND DELETE

        //BEGIN RETRIVE PPEIssueRegister INFORMATION (DATATABLE)   
        $scope.RetrievePPEIssueRegister = function () {
            $scope.tableParams = new ngTableParams({
                page: 1,
                count: 10,
                sorting: {
                    NameOfIssuer: 'asc'
                },
                defaultSort: 'asc'
            }, {
                total: 0,
                filterDelay: 750,
                getData: function ($defer, params) {
                    $rootScope.IsAjaxLoading = true;
                    $scope.isRecord = false;
                
            
                    PPEIssueRegisterService.GetPPEIssueRegister($scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter().NameOfIssuer, params.filter().NameOfRecievr, params.filter().TypeOfPPE, params.filter().Quanity, params.filter().Department, params.filter().ManagerName, params.filter().Price, params.filter().Remarks, $scope.filterDate.dateRange.startDate, $scope.filterDate.dateRange.endDate).then(function (result)
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
        //END RETRIVE PPEIssueRegister INFORMATION (DATATABLE)   

        //BEGIN SET VALUES BEFORE UPDATE OPERATION PERFORM
        $scope.UpdatePPEIssueRegister = function (_PPEIssueRegister) {

            $scope.storage.lastRecord = _PPEIssueRegister;
            $scope.rejFiles = [];
            $rootScope.isFormVisible = true;
            $scope.mode = "Edit";
            $scope.saveText = "Update";

            $scope.editData = {
                NameOfIssuer: _PPEIssueRegister.NameOfIssuer,
                NameOfRecievr: _PPEIssueRegister.NameOfRecievr,
                TypeOfPPE: _PPEIssueRegister.TypeOfPPE,
                Quanity: _PPEIssueRegister.Quanity,
                Department: _PPEIssueRegister.Department,
                ManagerName: _PPEIssueRegister.ManagerName,
                Price: _PPEIssueRegister.Price,
                Remarks: _PPEIssueRegister.Remarks,
                Attachment: _PPEIssueRegister.Attachment,
                files: _PPEIssueRegister.Attachment
            };
          
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        };
        //END SET VALUES BEFORE UPDATE OPERATION PERFORM



        //change service name and button name
        //BEGIN DELETE PPEIssueRegister INFORMATION
        $scope.DeletePPEIssueRegister = function (id) {
            $rootScope.IsAjaxLoading = true;
            PPEIssueRegisterService.DeletePPEIssueRegister(id).then(function (result) {
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
        //END DELETE SOCIAL WELFARE EXPENSE INFORMATION


        //BEGIN EXPORT TO EXCEL
        $scope.ExportToExcel = function () {
            document.location.href = "../../Handler/HRDPPEIssueRegister.ashx?timezone=" + $scope.timeZone + "&startDate=" + $filter('date')($scope.filterDate.dateRange.startDate, 'dd-MM-yyyy') + "&endDate=" + $filter('date')($scope.filterDate.dateRange.endDate, 'dd-MM-yyyy')
        };
        //BEGIN EXPORT TO EXCEL



        ///Model PopUp
        $scope.ShowPPEIssueRegister = function (PPEIssueRegister) {
            var modalInstance = $modal.open({
                templateUrl: 'PPEIssueRegister.html',
                controller: ModalInstanceCtrl,
                resolve: {
                    PPEIssueRegisterDetails: function () { return PPEIssueRegister; }
                }
            });
        }
        var ModalInstanceCtrl = function ($scope, $modalInstance, PPEIssueRegisterDetails) {
            $scope.items = PPEIssueRegisterDetails;
            $scope.Close = function () {
                $modalInstance.close();
            };
        };




    }


})();
