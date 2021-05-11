/// <reference path="../../../../Scripts/libs/angular/angular.js" />

(function () {

    'use strict';

    // CONTROLLER SIGNATURE
    angular.module("ERPApp.Controllers")
        .controller("PaymentReceivedCtrl", [
            "$scope", "$modal", "$rootScope", "$timeout", "ConversationService", "PaymentReceivedService", "InvoiceService", "$http", "$filter", "ngTableParams", "$q",
            PaymentReceivedCtrl
        ]);

    /*
     * START OF MAIN CONTROLLER
    */
    function PaymentReceivedCtrl($scope, $modal, $rootScope, $timeout, ConversationService, PaymentReceivedService, InvoiceService, $http, $filter, ngTableParams, $q) {
        $scope.timeZone = new Date().getTimezoneOffset().toString();

        $scope.SetOpenedTab = function (tab) {
            $scope.master.CurrentTab = tab;
        };

        $scope.GetClient = function () {
            ConversationService.GetClient($scope.master.filterData.ClientId).then(function (result) {
                $scope.ClientInfo = result.data.DataList;
            });
        };

        $scope.$watch("master.filterData.ClientId", function (newValue) {
            if ($scope.master.CurrentTab == "Payment") {
                //$scope.Close();
                if (newValue == 0) {
                    $scope.ClientInfo = [];
                } else { //filter on invoice list
                    $scope.GetClient();
                    if ($scope.isPaymentTableLoaded) {
                        $scope.RefreshInvoicePaymentTable();
                    } else {
                        $scope.GetPaymentReceivedInvoiceList();
                        //$scope.RetrieveInvoicePaymentList();
                    }
                }
            }
        });

        $scope.isPaymentTableLoaded = false;
        $scope.RetrieveInvoicePaymentList = function () {
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
                    PaymentReceivedService.RetrievePaymentReceivedInvoiceList($scope.master.filterData.ClientId, $scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter().InvoiceCode, params.filter().Currency).then(function (result) {
                        if (result.data.IsValidUser) {
                            //display no data message
                            if (result.data.MessageType === 1) {
                                if (result.data.DataList.total == 0) {
                                    $scope.noRecord = true;
                                } else {
                                    $scope.noRecord = false;
                                }

                                $scope.isPaymentTableLoaded = true;
                                params.total(result.data.DataList.total);
                                $defer.resolve($scope.documents = result.data.DataList.result);
                                $rootScope.IsAjaxLoading = false;
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

        //first function load when tab is active
        $scope.GetPaymentReceivedInvoiceList = function () {
            if ($scope.master.filterData.ClientId > 0) {
                if (!$scope.isPaymentTableLoaded) {
                    $scope.RetrieveInvoicePaymentList();
                } else {
                    $scope.RefreshInvoicePaymentTable();
                }
            }
        };

        $scope.RefreshInvoicePaymentTable = function () {
            $scope.tableParams.reload();
        };

        $scope.ViewInvoicePaymentHistory = function (invoice) {
            var modalInstance = $modal.open({
                templateUrl: 'Payment.html',
                controller: PaymentCtrl,
                scope: $scope,
                resolve: {
                    invoice: function () { return invoice; },
                    PRS: function () { return PaymentReceivedService },
                    RefreshTable: function () { return $scope.RefreshInvoicePaymentTable }
                },
                size:"lg"
            });
        };
    }

    /*
    * END OF MAIN CONTROLLER
    */

    /*
    * BEGIN PAYMENT CONTROLLER
    */
    var PaymentCtrl = function ($scope, $modalInstance, invoice, PRS, RefreshTable, $rootScope, $timeout, $filter) {
        $scope.Payment = {
            mode: "Add",
            saveText: "Save",
            IsActive: true
        };


        
        $scope.isProposalFirstFocus = true;
        $scope.invoice = invoice;

        $scope.paymentlist = [];
        $scope.formData = {};
        $scope.oldFormData = {};
        $rootScope.isPaymentFormVisible = false;
        $scope.IsAjaxLoading = false;

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
        $scope.maxDate = $scope.maxDate || new Date();

        $scope.OpenPaymentDateCalender = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.isPaymentDateOpened = true;
        };
        $scope.$watch("formData.pDate", function (newValue) {
            $scope.formData.PaymentDate = $filter('date')(newValue, 'dd-MM-yyyy');
        });
        $scope.ValidatePaymentDate = function (pDate, frmPayment) {
            if (!pDate) {
                frmPayment.txtPaymentDate.$setValidity("invalidPaymentDate", true);
                return;
            } else if (pDate.length == 10) {
                if ($scope.ValidateDate(pDate)) {
                    if ($scope.IsGreterThanToday(pDate)) {
                        $scope.formData.PaymentDate = $filter('date')(new Date(), 'dd-MM-yyyy');
                    } else {
                        frmPayment.txtPaymentDate.$setValidity("invalidPaymentDate", true);
                        var dt = pDate.split('-');
                        $scope.formData.pDate = dt[2] + "-" + dt[1] + "-" + dt[0] + "T00:00:00";
                    }
                } else {
                    frmPayment.txtPaymentDate.$setValidity("invalidPaymentDate", false);
                }
            } else {
                frmPayment.txtPaymentDate.$setValidity("invalidPaymentDate", false);
            }
        };

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
        // END DATE PICKER



        $scope.cmpReceviedAmountAndPending = function (OnHandReceivedAmount, frmPayment, TotalInvoiceAmount, PaymentReceivedAmount) {
            var pendingPayment = TotalInvoiceAmount - PaymentReceivedAmount;
            if (pendingPayment < OnHandReceivedAmount) {
                toastr.warning('Received amount is  greater than pending amount.');
                frmPayment.$invalid = true;
            } else {
                frmPayment.$invalid = false;
            }
        }

        // BEGIN CRUD FUNCTIONS
        $scope.CreateUpdateInvoicePayment = function (data, frmPayment) {
            var dateArray = data.PaymentDate.split('-');
            var date = new Date(parseInt(dateArray[2]), parseInt(dateArray[1]) - 1, parseInt(dateArray[0]), moment().hours(), moment().minute(), moment().second());
            var paymentDate = $filter('date')(date, 'MM-dd-yyyy HH:mm:ss');

            var _data = {
                PKPaymentId: data.PKPaymentId,
                FKInvoiceId: invoice.PKInvoiceId,
                PaymentReceivedDate: paymentDate,
                OnHandReceivedAmount: data.OnHandReceivedAmount,
                OtherCharges: data.OtherCharges,
                ExchangeRateINR: data.ExchangeRate,
                Remarks: data.Remarks
            }


            PRS.CreateUpdateInvoicePayment(_data, $scope.timeZone).then(function (result) {
                $scope.IsAjaxLoading = true;
                if (result.data.IsValidUser) {
                    if (result.data.MessageType === 1) {
                        $scope.RetrievePayments();
                        RefreshTable();
                        $scope.IsAjaxLoading = false;
                        frmPayment.$setPristine(); //reset form validation
                    }
                    else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                }
                else {
                    $rootScope.redirectToLogin();
                }
            });
        };
        $scope.RetrievePayments = function () {
            $scope.IsAjaxLoading = true;
            PRS.RetrievePayments(invoice.PKInvoiceId, $scope.timeZone).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType === 1) {
                        $scope.paymentlist = result.data.DataList;
                        $scope.IsAjaxLoading = false;
                    } else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                }
                else {
                    $rootScope.redirectToLogin();
                }
            });
        };
        $scope.RetrievePayments();

        $scope.DeletePayment = function (id) {
            $scope.IsAjaxLoadingPMS = true;
            PRS.DeletePayment(id).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType === 1) {
                        toastr.success(result.data.Message, 'Success');
                        $scope.RetrievePayments();
                    } else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
            });
            $scope.IsAjaxLoadingPMS = false
        };
        // END CRUD FUNCTIONS

        // BEGIN OTHER FUNCTIONS
        $scope.AddPayment = function () {
            ResetPaymentForm();
        };

        $scope.ResetPayment = function (frmPayment) {
            if ($scope.Payment.mode === "Add") {
                ResetPaymentForm();
            }
            else if ($scope.Payment.mode === "Edit") {
                $scope.formData = angular.copy($scope.oldFormData);
                $scope.isPaymentFirstFocus = false;
                $timeout(function () {
                    $scope.isPaymentFirstFocus = true;
                });
            }
            frmPayment.$setPristine();
        };

        $scope.UpdatePayment = function (_data) {
            var data = angular.copy(_data);
            
            $scope.formData = angular.copy(data);
            $scope.formData.PaymentDate = $filter('date')(data.PaymentReceivedDate, 'dd-MM-yyyy');
            $scope.formData.ExchangeRate = data.ExchangeRateINR;

            $scope.oldFormData = angular.copy(data);
            $scope.oldFormData.PaymentDate = $filter('date')(data.PaymentReceivedDate, 'dd-MM-yyyy'),
            $scope.oldFormData.ExchangeRate = data.ExchangeRateINR;

            $scope.Payment.mode = "Edit";
            $scope.Payment.saveText = "Update";
            $rootScope.isPaymentFormVisible = true;
            $scope.isPaymentFirstFocus = false;
            $timeout(function () {
                $scope.isPaymentFirstFocus = true;
            });
        };
        $scope.ClosePayment = function (frmPayment) {
            $scope.formData = {};
            $scope.oldFormData = {};
            $scope.Payment.mode = "Add";
            $rootScope.isPaymentFormVisible = false;
            $scope.isPaymentFirstFocus = false;
            frmPayment.$setPristine();
        };

        function ResetPaymentForm() {
            $scope.formData = {
                PKPaymentId: 0,
                FKInvoiceId: 0,
                OnHandReceivedAmount: "",
                OtherCharges: "",
                PaymentDate: moment().format("DD-MM-YYYY"),
                ExchangeRate: "",
                Remarks: "",
            };

            $scope.oldFormData = {};
            $scope.Payment.mode = "Add";
            $scope.Payment.saveText = "Save";
            $rootScope.isPaymentFormVisible = true;
            $scope.isPaymentFirstFocus = false;
            $timeout(function () {
                $scope.isPaymentFirstFocus = true;
            });
        };
        
        $scope.ChangePaymentStatus = function (id) {
            PRS.ChangePaymentStatus(id).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType === 1) {
                        toastr.success(result.data.Message, 'Success');
                        $scope.RetrievePayments();
                    } else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
            });
        };
        $scope.Close = function () {
            $modalInstance.close();
        };
        //END OTHER FUNCTIONS
    };
    /* END PAYMENT CONTROLLER */

})();