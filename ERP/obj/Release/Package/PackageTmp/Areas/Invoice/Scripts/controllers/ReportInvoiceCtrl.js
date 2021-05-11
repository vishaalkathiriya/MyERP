/// <reference path="../../../../Scripts/libs/angular/angular.js" />

(function () {

    'use strict';

    angular.module("ERPApp.Controllers")
        .controller("ReportInvoiceCtrl", [
            "$scope", "$modal", "$rootScope", "$timeout", "ReportInvoiceService", "$http", "$filter", "ngTableParams", "$q",
            ReportInvoiceCtrl]);

    function ReportInvoiceCtrl($scope, $modal, $rootScope, $timeout, RIS, $http, $filter, ngTableParams, $q) {
        $scope.timeZone = new Date().getTimezoneOffset().toString();
        $scope.filterData = {
            ClientId: 0,
            InvoiceType: "",
            Currency: 0,
            Status: "",
            dateRange: {
                startDate: "",
                endDate: ""
            }
        };

        $scope.FilterInvoice = function (filterData) {
            $scope.ReloadInvoiceTable();
        };

        $scope.ReloadInvoiceTable = function (e) {
            $scope.tableParams.reload();
        };

        $scope.LoadCurrencyList = function () {
            RIS.GetCurrencyList().then(function (result) {
                $scope.CurrencyList = result.data.DataList;
            });
        };
        $scope.LoadCurrencyList();

        $scope.LoadClientList = function () {
            RIS.GetClientList().then(function (result) {
                $scope.ClientList = result.data.DataList;
            });
        };

        //GET INVOICE LIST
        $scope.RetrieveInvoices = function () {
            $scope.tableParams = new ngTableParams({
                page: 1,
                count: 10,
                sorting: {
                    InvoiceCode: 'asc'
                },
                defaultSort: 'asc'
            }, {
                total: 0,
                filterDelay: 750,
                getData: function ($defer, params) {
                    $rootScope.IsAjaxLoading = true;
                    $scope.isRecord = false;
                    RIS.RetrieveInvoices(
                        $scope.filterData.ClientId,
                        $scope.filterData.InvoiceType,
                        $scope.filterData.Currency,
                        $scope.filterData.dateRange.startDate,
                        $scope.filterData.dateRange.endDate,
                        $scope.filterData.Status,
                        $scope.timeZone,
                        params.page(),
                        params.count(),
                        params.orderBy(),
                        params.filter().InvoiceCode,
                        params.filter().Currency).then(function (result) {
                            if (result.data.IsValidUser) {
                                //display no data message
                                if (result.data.MessageType === 1) {
                                    if (result.data.DataList.total == 0) {
                                        $scope.noRecord = true;
                                    } else {
                                        $scope.noRecord = false;
                                    }
                                    var invoceRecored = [];
                                    var groupByCurrencyList = _.groupBy(result.data.DataList.result, "Currency");
                                    angular.forEach(groupByCurrencyList, function (groupByCurrencyVal, groupByCurrencyKey) {
                                        $scope.SumTotalAmount = 0;
                                        $scope.SumRoundOffAmount = 0;
                                        $scope.SumReceivedAmount = 0;
                                        $scope.SumPendingAmount = 0;
                                        // fetch one by one record based on group
                                        angular.forEach(groupByCurrencyVal, function (val, key) {
                                            val.ReceivedAmount = 0;
                                            val.PendingAmount = 0;
                                            $scope.SumTotalAmount += val.TotalAmount;
                                            $scope.SumRoundOffAmount += val.RoundOff;
                                            angular.forEach(val.tblINVPayments, function (v2, k2) {
                                                val.ReceivedAmount += v2.OnHandReceivedAmount + v2.OtherCharges;
                                            });
                                            val.PendingAmount = val.TotalAmount - val.ReceivedAmount;
                                            $scope.SumReceivedAmount += val.ReceivedAmount;
                                            $scope.SumPendingAmount += val.PendingAmount;
                                            val.Flag = "rows";
                                            invoceRecored.push(val);
                                        });

                                        //Add  new one row to display total of amount
                                        var test = {
                                            Flag: "total",
                                            TotalAmount: $scope.SumTotalAmount,
                                            RoundOff: $scope.SumRoundOffAmount,
                                            ReceivedAmount: $scope.SumReceivedAmount,
                                            PendingAmount: $scope.SumPendingAmount,
                                            InvoiceCode: groupByCurrencyKey
                                        };
                                        invoceRecored.push(test);
                                    });


                                    $q.all(invoceRecored).then(function () {
                                        params.total(invoceRecored.total);
                                        $defer.resolve($scope.documents = invoceRecored);
                                        $rootScope.IsAjaxLoading = false;
                                    });

                                    //params.total(result.data.DataList.total);
                                    //$defer.resolve($scope.documents = result.data.DataList.result);
                                    //$rootScope.IsAjaxLoading = false;


                                    //var prom = [];
                                    //$scope.SumTotalAmount = 0;
                                    //$scope.SumRoundOffAmount = 0;
                                    //$scope.SumReceivedAmount = 0;
                                    //$scope.SumPendingAmount = 0;
                                    //angular.forEach(result.data.DataList.result, function (v1, k1) {
                                    //    v1.ReceivedAmount = 0;
                                    //    v1.PendingAmount = 0;

                                    //    $scope.SumTotalAmount += v1.TotalAmount;
                                    //    $scope.SumRoundOffAmount += v1.RoundOff;

                                    //    angular.forEach(v1.tblINVPayments, function (v2, k2) {
                                    //        v1.ReceivedAmount += v2.OnHandReceivedAmount + v2.OtherCharges;
                                    //    });
                                    //    v1.PendingAmount = v1.TotalAmount - v1.ReceivedAmount;

                                    //    $scope.SumReceivedAmount += v1.ReceivedAmount;
                                    //    $scope.SumPendingAmount += v1.PendingAmount;
                                    //    prom.push(v1);
                                    //});

                                    //$q.all(prom).then(function () {
                                    //    params.total(result.data.DataList.total);
                                    //    $defer.resolve($scope.documents = result.data.DataList.result);
                                    //    $rootScope.IsAjaxLoading = false;
                                    //});


                                } else {
                                    toastr.error(result.data.Message, 'Opps, Something went wrong');
                                }
                            } else {
                                $rootScope.redirectToLogin();
                            }
                        });
                }
            });
        };

        $scope.clearDateBox = function (e) {
            var target = $(e.target).parent().find("input[type=text]").val("");
            $scope.filterData.dateRange.startDate = "";
            $scope.filterData.dateRange.endDate = "";
        };

        $scope.ViewMore = function (invoice) {
            var modalInstance = $modal.open({
                templateUrl: 'ReportInvoice.html',
                scope: $scope,
                controller: ReportInfoCrtl,
                resolve: {
                    invoice: function () { return invoice },
                    RIS: function () { return RIS }
                },
                size: 'lg'
            });


        };
    };

    var ReportInfoCrtl = function ($scope, $modalInstance, invoice, RIS) {
        RIS.RetrieveInvoiceMilestones(invoice.MilestoneIds).then(function (result) {
            if (result.data.IsValidUser) {
                if (result.data.MessageType == 1) {
                    $scope.invoice = 0;
                    $scope.milestoneList = result.data.DataList;
                    $scope.paymentList = invoice.tblINVPayments;
                    $scope.taxList = invoice.tblINVInvoiceTaxes;
                    $scope.moreInfoTotalPaymentReceived = 0;

                    angular.forEach($scope.paymentList, function (value, key) {
                        $scope.invoice += ((parseInt(value.OnHandReceivedAmount) * 1) + (parseInt(value.OtherCharges) * 1));
                    });

                    //angular.forEach(invoice.tblINVInvoiceTaxes, function (v1, k1) {
                    //    v1.TaxTypeName = v1.tblINVTaxMaster.TaxTypeName;
                    //    $scope.taxList.push(v1);
                    //});
                } else {
                    toastr.error(result.data.Message, "Oops, Something went wrong")
                }
            } else {
                $rootScope.redirectToLogin();
            }
        });

        $scope.Close = function () {
            $modalInstance.close();
        };

    };


})();