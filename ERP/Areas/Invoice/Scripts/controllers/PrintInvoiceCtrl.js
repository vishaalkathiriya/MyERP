/// <reference path="../../../../Scripts/libs/angular/angular.js" />

(function () {

    'use strict';

    // CONTROLLER SIGNATURE
    angular.module("ERPApp.Controllers")
        .controller("PrintInvoiceCtrl", [
            "$scope", "$rootScope","$filter", "$timeout", "InvoiceService", PrintInvoiceCtrl
        ]);

    function PrintInvoiceCtrl($scope, $rootScope, $filter, $timeout, InvoiceService) {
        $scope.timeZone = new Date().getTimezoneOffset().toString();
        $scope.invoiceData = {
            invoice: [],
            milestone: [],
            milestonePrice: 0
        };

        $scope.LoadInvoiceData = function (invoiceId) {
            InvoiceService.GetInvoice(invoiceId).then(function (resI) {
                $scope.invoiceData.invoice = resI.data.DataList;

                //dynamic page title
                document.title = $scope.invoiceData.invoice.InvoiceCode + "_" + $scope.invoiceData.invoice.tblINVClient.CompanyName + "_" + $filter('date')($scope.invoiceData.invoice.InvoiceDate, "ddMMyyyy");

                //get client country and state name
                InvoiceService.GetClientCountry(resI.data.DataList.FKClientId).then(function (resC) {
                    if (resC.data.DataList) {
                        $scope.invoiceData.invoice.CountryName = resC.data.DataList.CountryName;

                        if (resC.data.DataList.tblStates.length > 0) {
                            $scope.invoiceData.invoice.StateName = resC.data.DataList.tblStates[0].StateName;
                        }
                    }
                });

                InvoiceService.GetMilestoneList(resI.data.DataList.MilestoneIds).then(function (resM) {
                    $scope.invoiceData.milestone = resM.data.DataList;
                    
                    angular.forEach($scope.invoiceData.milestone, function (value, key) {
                        $scope.invoiceData.milestonePrice += value.Price;
                    });
                });
            });
        };
    }
})();