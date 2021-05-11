/// <reference path="../../../../Scripts/libs/angular/angular.js" />

(function () {

    'use strict';

    // CONTROLLER SIGNATURE
    angular.module("ERPApp.Controllers")
        .controller("InvoiceCtrl", [
            "$scope", "$modal", "$rootScope", "$timeout", "ConversationService", "MilestoneService", "InvoiceService", "$http", "$filter", "ngTableParams", "$q",
            InvoiceCtrl
        ]);

    /*
     * END PROJECT CONVERSATION CONTROLLER
    */
    function InvoiceCtrl($scope, $modal, $rootScope, $timeout, ConversationService, MilestoneService, InvoiceService, $http, $filter, ngTableParams, $q) {
        $scope.timeZone = new Date().getTimezoneOffset().toString();
        $scope.isFirstFocusInvoice = false;
        $scope.invoiceData = {
            PKInvoiceId: 0,
            milestoneList: [],
            invoiceTaxList: [],
            Currency: 0,
            RoundOff: 0,
            FinalTotal: 0,
            MilestonePrice: 0
        };


        $scope.invoice = { mode: "Add", saveText: "Save" };

        $scope.SetOpenedTab = function (tab) {
            $scope.master.CurrentTab = tab;
        };

        $scope.GetClient = function () {
            ConversationService.GetClient($scope.master.filterData.ClientId).then(function (result) {
                $scope.ClientInfo = result.data.DataList;
            });
        };

        $scope.$watch("master.filterData.ClientId", function (newValue) {
            if ($scope.master.CurrentTab == "Invoice") {
                $scope.Close();
                if (newValue == 0) {
                    $scope.ClientInfo = [];
                } else { //filter on invoice list
                    $scope.GetClient();
                    if ($scope.isInvoiceTableLoaded) {
                        $scope.RefreshInvoiceTable();
                    } else {
                        $scope.RetrieveInvoiceList();
                    }
                }
            }
        });

        $scope.validateDropCurrency = function () {
            if ($scope.invoiceData.Currency != 0) return false;
            return true;
        };
        //BEGIN DATE PICKER
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

        $scope.OpenInvoiceDateCalender = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.isInvoiceDateOpened = true;
        };
        $scope.$watch("invoiceData.iDate", function (newValue) {
            $scope.invoiceData.InvoiceDate = $filter('date')(newValue, 'dd-MM-yyyy');
        });
        $scope.ValidateInvoiceDate = function (invDate, frmInvoice) {
            if (!frmInvoice) {
                frmInvoice.txtInvoiceDate.$setValidity("invalidInvoiceDate", true);
                return;
            } else if (invDate.length == 10) {
                if ($scope.ValidateDate(invDate)) {
                    if ($scope.IsGreterThanToday(invDate)) {
                        $scope.invoiceData.InvoiceDate = $filter('date')(new Date(), 'dd-MM-yyyy');
                    } else {
                        frmInvoice.txtInvoiceDate.$setValidity("invalidInvoiceDate", true);
                        var dt = invDate.split('-');
                        $scope.invoiceData.iDate = dt[2] + "-" + dt[1] + "-" + dt[0] + "T00:00:00";
                    }
                } else {
                    frmInvoice.txtInvoiceDate.$setValidity("invalidInvoiceDate", false);
                }
            } else {
                frmInvoice.txtInvoiceDate.$setValidity("invalidInvoiceDate", false);
            }
        };
        //END DATE PICKER

        function ParseInvoiceTaxList(taxlist, callback) {
            var prom = [];
            angular.forEach(taxlist, function (value, key) {
                if (value.IsActive && !value.IsDeleted) { // display only active items
                    prom.push({
                        PKInvoiceTaxId: value.PKInvoiceTaxId,
                        PKTaxId: value.FKTaxId,
                        Percentage: value.TaxPercentage,
                        Mode: value.tblINVTaxMaster.Mode,
                        Amount: value.Amount
                    });
                }
            });

            $q.all().then(function () {
                callback(prom);
            });
        };

        $scope.UpdateInvoice = function (invoice) {
            if (invoice.tblINVPayments.length > 0) {
                toastr.warning("Sorry, We've found active payment received entry", "Warning");
                $scope.Close();
            } else {
                $scope.invoice = {
                    mode: "Edit",
                    saveText: "Update"
                };

                $scope.isFirstFocusInvoice = false;
                $timeout(function () {
                    $scope.isFirstFocusInvoice = true;
                });

                //get milestone list in edit mode
                InvoiceService.GetMilestoneList(invoice.MilestoneIds).then(function (result) {
                    if (result.data.IsValidUser) {
                        if (result.data.MessageType === 1) {
                            $scope.invoiceData.milestoneList = result.data.DataList; //for use in GetMilestoneTotalPrice

                            InvoiceService.GetTaxList().then(function (resTax) {
                                $scope.TaxList = resTax.data.DataList;

                                //Parse invoice tax list
                                ParseInvoiceTaxList(invoice.tblINVInvoiceTaxes, function (taxlist) {
                                    //Get milestone price
                                    GetMilestoneTotalPrice(function (price, ids) {
                                        $scope.invoiceData = {
                                            PKInvoiceId: invoice.PKInvoiceId,
                                            milestoneList: result.data.DataList,
                                            invoiceTaxList: taxlist,
                                            Currency: invoice.Currency,
                                            RoundOff: invoice.RoundOff,
                                            FinalTotal: invoice.TotalAmount,
                                            MilestonePrice: price,
                                            InvoiceType: invoice.InvoiceType,
                                            InvoiceDate: $filter('date')(invoice.InvoiceDate, 'dd-MM-yyyy'),
                                            InvoiceCode: invoice.InvoiceCode,
                                            iDate: invoice.InvoiceDate,
                                            Remarks: invoice.Remarks
                                        };
                                    });
                                });
                            });
                        } else {
                            toastr.error(result.data.Message, 'Opps, Something went wrong');
                        }
                        $scope.IsAjaxLoading = false;
                    } else {
                        $rootScope.redirectToLogin();
                    }
                });
            }
        };

        //first function load when tab is active
        $scope.isInvoiceTableLoaded = false;
        $scope.GetInvoiceMilestoneList = function () {
            $scope.invoiceData = [];
            $scope.invoiceData.Currency = 0;
            $scope.invoiceData.InvoiceType = "E";
            $scope.invoiceData.milestoneList = $scope.master.SelectedMilestoneList;

            if ($scope.master.SelectedMilestoneList.length > 0) {
                $scope.invoiceData.Currency = $scope.master.SelectedMilestoneList[0].Currency;
            }

            $scope.isFirstFocusInvoice = true;

            $scope.invoice = {
                mode: "Add",
                saveText: "Save"
            };
            
            GetMilestoneTotalPrice(function (price, ids) {
                $scope.invoiceData.MilestonePrice = price; //store milestone price to use in full page

                $scope.IsAjaxLoading = true;
                InvoiceService.GetTaxList().then(function (result) {
                    if (result.data.IsValidUser) {
                        if (result.data.MessageType === 1) {
                            $scope.TaxList = result.data.DataList;
                            $scope.invoiceData.invoiceTaxList = [];
                            $scope.TaxList[0].Amount = 0;

                            $scope.invoiceData.invoiceTaxList.push({ PKInvoiceTaxId: 0, PKTaxId: $scope.TaxList[0].PKTaxId, Percentage: $scope.TaxList[0], Mode: $scope.TaxList[0].Mode }); //Add initial line to display
                            $scope.CalculateTaxAmount(0, $scope.TaxList[0].Percentage);

                            if ($scope.master.filterData.ClientId > 0) {
                                if (!$scope.isInvoiceTableLoaded) {
                                    $scope.RetrieveInvoiceList();
                                } else {
                                    $scope.RefreshInvoiceTable();
                                }
                            }

                            $scope.IsAjaxLoading = false;
                        } else {
                            toastr.error(result.data.Message, 'Opps, Something went wrong');
                        }
                    } else {
                        $rootScope.redirectToLogin();
                    }
                });
            });
        };

        $scope.OnTaxTypeChange = function (taxId, index) {
            var prom = [];
            var percentage = 0;
            angular.forEach($scope.TaxList, function (v, k) {
                if (v.PKTaxId == taxId) {
                    percentage = v.Percentage;
                    $scope.invoiceData.invoiceTaxList[index] = {
                        PKInvoiceTaxId: $scope.invoiceData.invoiceTaxList[index].PKInvoiceTaxId,
                        PKTaxId: v.PKTaxId,
                        Percentage: v.Percentage,
                        Mode: v.Mode,
                        Amount: 0
                    };
                }
                prom.push(v);
            });
            $q.all(prom).then(function () {
                $scope.CalculateTaxAmount(index, percentage);
            });
        };

        function GetMilestoneTotalPrice(callback) {
            var price = 0;
            var Ids = "";
            var prom = [];
            angular.forEach($scope.invoiceData.milestoneList, function (value, key) {
                price += value.Price;
                Ids += value.PKMilestoneId + ",";
                prom.push(value);
            });

            $q.all().then(function () {
                callback(price, Ids);
            });
        };

        $scope.CalculateTaxAmount = function (index, percentage) {
            var perc = +percentage ? (angular.isNumber(+percentage) ? +percentage : 0) : 0;
            $scope.invoiceData.invoiceTaxList[index] = {
                PKInvoiceTaxId: $scope.invoiceData.invoiceTaxList[index].PKInvoiceTaxId,
                PKTaxId: $scope.invoiceData.invoiceTaxList[index].PKTaxId,
                Percentage: percentage,
                Mode: $scope.invoiceData.invoiceTaxList[index].Mode,
                Amount: ($scope.invoiceData.MilestonePrice * perc) / 100,
            };

            $scope.CalculateFinalTotal();
        };

        function GetCalculatedTaxAmount(callback) {
            var amount = 0;
            var prom = [];
            angular.forEach($scope.invoiceData.invoiceTaxList, function (value, key) {
                if (value.Mode == "P") {
                    amount += value.Amount;
                } else if (value.Mode == "M") {
                    amount -= value.Amount;
                }
                prom.push(value);
            });

            $q.all().then(function () {
                callback(amount);
            });
        };

        $scope.CalculateFinalTotal = function () {
            GetCalculatedTaxAmount(function (taxTotalPrice) {
                var roundoff = $scope.invoiceData.RoundOff ? +$scope.invoiceData.RoundOff : 0;
                roundoff = isNaN(roundoff) ? 0 : roundoff;

                var total = $scope.invoiceData.MilestonePrice + taxTotalPrice + roundoff;
                $scope.invoiceData.FinalTotal = $filter('number')(total, 2);
            });
        };

        $scope.AddNewTax = function () {
            var lenAsNewIndex = $scope.invoiceData.invoiceTaxList.length;

            $scope.TaxList[0].Amount = 0;
            //$scope.invoiceData.invoiceTaxList.push($scope.TaxList[0]);
            $scope.invoiceData.invoiceTaxList.push({
                PKInvoiceTaxId: 0,
                PKTaxId: $scope.TaxList[0].PKTaxId,
                Percentage: $scope.TaxList[0].Percentage,
                Mode: $scope.TaxList[0].Mode
            });
            $scope.CalculateTaxAmount(lenAsNewIndex, $scope.invoiceData.invoiceTaxList[lenAsNewIndex].Percentage);
        };

        $scope.DeleteTax = function (index, invoiceTaxId) {
            if (invoiceTaxId > 0) { //delete from db
                InvoiceService.DeleteInvoiceTax(invoiceTaxId).then(function (result) {
                    if (result.data.IsValidUser) {
                        if (result.data.MessageType === 1) { //success
                            //toastr.success(result.data.Message, 'Success');
                            $scope.invoiceData.invoiceTaxList.splice(index, 1);
                            $scope.CalculateFinalTotal();
                        } else {
                            toastr.error(result.data.Message, 'Opps, Something went wrong');
                        }
                    }
                    else {
                        $rootScope.redirectToLogin();
                    }
                });
            } else {
                $scope.invoiceData.invoiceTaxList.splice(index, 1);
                $scope.CalculateFinalTotal();
            }
        };

        $scope.$watch("invoiceData.RoundOff", function (newValue) {
            $scope.CalculateFinalTotal();
        });

        function DoProcessInvoiceTaxes(invoiceId, callback) {
            var index = 0, length = $scope.invoiceData.invoiceTaxList.length;
            function saveData() {
                SaveInvoiceTax($scope.invoiceData.invoiceTaxList[index], invoiceId).then(function (result) {
                    if (index == length - 1) {
                        callback(result);
                    } else {
                        index++;
                        saveData();
                    }
                });
            }
            saveData();
        };

        function SaveInvoiceTax(line, invoiceId) {
            line.FKInvoiceId = invoiceId;
            line.FKTaxId = line.PKTaxId;
            line.TaxPercentage = line.Percentage;
            line.Amount = line.Amount;
            return InvoiceService.SaveInvoiceTax(line);
        };

        $scope.CreateUpdateInvoice = function (invoiceData, frmInvoice) {
            GetMilestoneTotalPrice(function (price, ids) {
                var InvoiceDateArray = invoiceData.InvoiceDate.split("-");
                var InvoiceDate = new Date(InvoiceDateArray[2], InvoiceDateArray[1] - 1, InvoiceDateArray[0], moment().hours(), moment().minute(), moment().second());

                var invoiceLine = {
                    PKInvoiceId: invoiceData.PKInvoiceId,
                    FKClientId: $scope.master.filterData.ClientId,
                    MilestoneIds: ids,
                    InvoiceDate: angular.copy(InvoiceDate),
                    Currency: invoiceData.Currency,
                    InvoiceType: invoiceData.InvoiceType,
                    Remarks: invoiceData.Remarks,
                    RoundOff: invoiceData.RoundOff,
                    TotalAmount: invoiceData.FinalTotal
                };
                InvoiceService.CreateUpdateInvoice(invoiceLine).then(function (result) {
                    if (result.data.IsValidUser) {
                        if (result.data.MessageType === 1) {
                            //success
                            var invoiceId = result.data.DataList;
                            DoProcessInvoiceTaxes(invoiceId, function (res) {
                                toastr.success(result.data.Message, 'Success');
                                $scope.Close();
                                $scope.RefreshInvoiceTable();
                            });
                        } else {
                            toastr.error(result.data.Message, 'Opps, Something went wrong');
                        }
                    }
                    else {
                        $rootScope.redirectToLogin();
                    }
                });
            });
        };

        //GET INVOICE LIST
        $scope.RetrieveInvoiceList = function () {
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
                    InvoiceService.RetrieveInvoiceList($scope.master.filterData.ClientId, $scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter().InvoiceCode, params.filter().Currency).then(function (result) {
                        if (result.data.IsValidUser) {
                            //display no data message
                            if (result.data.MessageType === 1) {
                                if (result.data.DataList.total == 0) {
                                    $scope.noRecord = true;
                                } else {
                                    $scope.noRecord = false;
                                }
                                $scope.isInvoiceTableLoaded = true;
                                params.total(result.data.DataList.total);
                                $defer.resolve($scope.documents = result.data.DataList.result);
                                $rootScope.IsAjaxLoading = false;

                                //remove form from display
                                if ($scope.invoice.mode == "Edit") {
                                    $scope.master.SelectedMilestoneList = [];
                                    $scope.invoice.mode = "Add";
                                }

                            } else {
                                toastr.error(result.data.Message, 'Opps, Something went wrong');
                            }
                        } else {
                            $rootScope.redirectToLogin();
                        }
                    });
                }
            });
        }

        $scope.RefreshInvoiceTable = function () {
            $scope.tableParams.reload();
        };

        $scope.Close = function () {
            $scope.invoiceData = [];
            $scope.master.SelectedMilestoneList = [];
            $scope.invoice = {
                mode: "Add",
                saveText: "Save"
            };
            $scope.isFirstFocusInvoice = false;
        };

        $scope.GetClientVATNumber = function () {
            $scope.IsAjaxLoading = true;
            InvoiceService.GetClient($scope.master.filterData.ClientId).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType === 1) {
                        $scope.ClientVATNumber = result.data.DataList.VATNo;
                    } else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }

                    $scope.IsAjaxLoading = false;
                } else {
                    $rootScope.redirectToLogin();
                }
            });
        };

        $scope.DeleteInvoice = function (invoice) {
            if (invoice.tblINVPayments.length > 0) {
                toastr.warning("Sorry, We've found active payment received entry", "Warning");
                $scope.Close();
            } else {
                $scope.IsAjaxLoading = true;
                InvoiceService.DeleteInvoice(invoice.PKInvoiceId).then(function (result) {
                    if (result.data.IsValidUser) {
                        if (result.data.MessageType === 1) { //success
                            toastr.success(result.data.Message, 'Success');
                            $scope.RefreshInvoiceTable();
                        } else if (result.data.MessageType === 2) {
                            toastr.warning(result.data.Message, 'Warning');
                        } else {
                            toastr.error(result.data.Message, 'Opps, Something went wrong');
                        }
                        $scope.IsAjaxLoading = false;
                    } else {
                        $rootScope.redirectToLogin();
                    }
                });
            }
        };

        $scope.CopyInvoice = function (invoiceId) {
            $scope.IsAjaxLoading = true;
            InvoiceService.CopyInvoice(invoiceId).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType === 1) { //success
                        toastr.success(result.data.Message, 'Success');
                        $scope.RefreshInvoiceTable();
                    } else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                    $scope.IsAjaxLoading = false;
                } else {
                    $rootScope.redirectToLogin();
                }
            });
        };
    }

    /*
    * END PROJECT CONVERSATION CONTROLLER
    */

})();