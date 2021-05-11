/// <reference path="../libs/angular/angular.min.js" />

(function () {

    'use strict';

    angular.module("ERPApp.Controllers")
    .controller("CurrencyCtrl", [
        "$scope", "$rootScope", "$timeout", "CurrencyService", "$http", "$filter", "ngTableParams",
        currencyCtrl
    ]);


    function currencyCtrl($scope, $rootScope, $timeout, CurrencyService, $http, $filter, ngTableParams) {

        $scope.editData = $scope.editData || {};
        $scope.mode = "Add";
        $scope.saveText = "Save";
        $scope.timeZone = new Date().getTimezoneOffset().toString(); //timezone difference in minutes
        $scope.isFocusFirst = false;
        $scope.storage = { lastRecord: "" };
        $scope.filterData = {};
        $scope.editData.CountryId = 0;
        $scope.editData.IsActive = true;


        // BEGIN RESET FORM
        function ResetForm() {
            $scope.editData = {};
            $scope.currencyForm.$setPristine();
            $scope.isFirstFocus = false;
            $scope.editData.CountryId = 0;
            $scope.editData.IsActive = true;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        }
        // END RESET FORM

        // BEGIN GET COUNTRY LIST
        function CountryList() {
            CurrencyService.CountryList().then(function (result) {
                $scope.countryList = result.data.DataList;
            });
        };
        CountryList();
        // END GET COUNTRY LIST

        //BEGIN ADD NEW CURRENCY
        $scope.AddCurrency = function () {
            $rootScope.isFormVisible = true;
            $scope.mode = "Add";
            $scope.saveText = "Save";
            ResetForm(); //reset form
        };
        // END ADD NEW CURRENCY

        //BEGIN TO EXPORT TO EXCEL
        $scope.ExportToExcel = function () {
            document.location.href = "../../Handler/Currency.ashx?timezone=" + $scope.timeZone
        }
        //END TO EXPORT TO EXCEL

        //BEGIN TO ADD/EDIT CURRENCY INFORMATION
        $scope.CreateUpdateCurrency = function (currencyInfo) {
            CurrencyService.CreateUpdateCurrency(currencyInfo).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        toastr.success(result.data.Message, 'Success');
                        $scope.editData = { CurrencyName: '', CurrencyCode: '',CountryId:0,Remark:'' ,IsActive: true };
                        $scope.RefreshTable();
                        $scope.storage.lastRecord = {};
                        $scope.isFirstFocus = false;
                        $scope.currencyForm.$setPristine();
                        if ($scope.mode === "Edit") {
                            $rootScope.isFormVisible = false;
                            $scope.saveText = "Save";
                        }
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
        };
        //END TO ADD/EDIT CURRENCY INFORMATION

        //BEGIN REFERESH TABLE CALL TO PERFORM SOME OPERATION
        $scope.RefreshTable = function () {
            $scope.tableParams.reload();
            $rootScope.IsAjaxLoading = false;
        };
        //END REFERESH TABLE CALL TO PERFORM SOME OPERATION

        //BEGIN TO CHANGE CURRENCY STATUS
        $scope.ChangeStatus = function (id) {
            $rootScope.IsAjaxLoading = true;
            CurrencyService.ChangeStatus(id).then(function (result) {
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
        }
        //END TO CHANGE CURRENCY STATUS


        $scope.validateDropCountryId = function () {
            if ($scope.editData.CountryId && $scope.editData.CountryId != 0) return false;
            return true;
        };

        //BEGIN TO RESET CURRENCY INFORMATION
        $scope.ResetCurrency = function () {
            if ($scope.mode == "Edit") {
                //$scope.editData.Currencies = $scope.storage.lastRecord.Currencies;
                //$scope.editData.CurrencyId = $scope.storage.lastRecord.CurrencyId;
                //$scope.editData.IsActive = $scope.storage.lastRecord.IsActive;

                $scope.editData = {
                    Id:$scope.storage.lastRecord.Id,
                    CurrencyName:$scope.storage.lastRecord.CurrencyName,
                    CurrencyCode:$scope.storage.lastRecord. CurrencyCode,
                    CountryId:$scope.storage.lastRecord.CountryId,
                    Remark:$scope.storage.lastRecord.Remark,
                    IsActive: $scope.storage.lastRecord.IsActive
                }

            } else { 
                ResetForm();
            }
        };
        //END TO RESET CURRENCY INFORMATION

        //BEGIN CANCEL BUTTON CLICK
        $scope.CloseCurrency = function () {
            $scope.mode = "Add";
            ResetForm();
            $rootScope.isFormVisible = false;
        };
        //END CANCEL BUTTON CLICK

        //BEGIN EDIT BUTTON CLICK TO LIST OF CURRENCY
        $scope.UpdateCurrency = function (currency) {
            $scope.storage.lastRecord = currency;
            $rootScope.isFormVisible = true;
            $scope.mode = "Edit";
            $scope.saveText = "Update";
            $scope.editData.Id = currency.Id;
            $scope.editData.CurrencyName = currency.CurrencyName;
            $scope.editData.CurrencyCode = currency.CurrencyCode;
            $scope.editData.CountryId = currency.CountryId;
            $scope.editData.Remark = currency.Remark;
            $scope.editData.IsActive = currency.IsActive;
            $scope.currencyForm.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        };
        //END EDIT BUTTON CLICK TO LIST OF CURRENCY

        //BEGIN DELETE CURRENCY INFORMTION
        $scope.DeleteCurrency = function (id)
        {
            $rootScope.IsAjaxLoading = true;
            CurrencyService.DeleteCurrency(id).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 0) { // 0:Error
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                    else if (result.data.MessageType == 2) { // 2:Warning
                        toastr.warning(result.data.Message, 'This record is in use');
                    }
                    else {
                        toastr.success(result.data.Message, 'Success');
                        $scope.RefreshTable();
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
                $rootScope.IsAjaxLoading = false;
            });
        }
        //END DELETE CURRENCY INFORMTION

        //BEGIN GET CURRENCY INFORMATION
        $scope.CurrencyList = function () {
            $scope.tableParams = new ngTableParams({
                page: 1,
                count: 10,
                sorting: {
                    Documents: 'asc'
                },
                defaultSort: 'asc'
            }, {
                total: 0,
                filterDelay: 750,
                getData: function ($defer, params) {
                    $rootScope.IsAjaxLoading = true;
                    $scope.isRecord = false;
                    CurrencyService.GetCurrencyList($scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter().CurrencyName, params.filter().CurrencyCode, params.filter().CountryName).then(function (result) {
                        if (result.data.IsValidUser) {
                            if (result.data.MessageType != 0) { // 0:Error
                                //display no data message
                                if (result.data.DataList.total == 0) {
                                    $scope.noRecord = true;
                                    $scope.filterText = params.filter().Documents;
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
        //END GET CURRENCY INFORMATION

        $scope.IsCurrencyCodeExists = function (code, frmCurrency) {
            if (code) {
                if ($scope.mode == "Edit" && angular.lowercase(code) == angular.lowercase($scope.storage.lastRecord.CurrencyCode)) {
                    frmCurrency.currencyCode.$setValidity("invalidCode", true);
                } else {
                    CurrencyService.IsCurrencyCodeExists(code).then(function (result) {
                        if (result.data.IsValidUser) {
                            if (result.data.DataList) { //Exists
                                frmCurrency.currencyCode.$setValidity("invalidCode", false);
                            } else { //Not Exists
                                frmCurrency.currencyCode.$setValidity("invalidCode", true);
                            }
                        } else {
                            $rootScope.redirectToLogin();
                        }
                    });
                }
            } else {
                frmCurrency.currencyCode.$setValidity("invalidCode", true);
            }
        };
    }

})();