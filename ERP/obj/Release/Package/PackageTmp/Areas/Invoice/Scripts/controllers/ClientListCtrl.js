/// <reference path="../libs/angular/angular.min.js" />


(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("ClientListCtrl", [
            "$scope", "$modal", "$rootScope", "$timeout", "ClientCreateService", "$http", "$filter", "ngTableParams",
            clientListCtrl
        ]);


    //Main controller function
    function clientListCtrl($scope, $modal, $rootScope, $timeout, ClientCreateService, $http, $filter, ngTableParams) {
        $scope.timeZone = new Date().getTimezoneOffset().toString(); //timezone difference in minutes
        $scope.filterData = {
            KYCApproved: 0,
            CountryId: 0
        };

        /*refresh table after */
        $scope.RefreshTable = function () {
            $scope.tableParams.reload();
        };

        /*active/inactive employee*/
        $scope.ChangeStatus = function (id, status) {
            var oper = status == true ? "InActive" : "Active";
            $rootScope.IsAjaxLoading = true;
            ClientCreateService.ChangeStatus(id, status).then(function (result) {
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
        };

        /*delete client*/
        $scope.DeleteClient = function (id) {
            $rootScope.IsAjaxLoading = true;
            ClientCreateService.DeleteClient(id).then(function (result) {
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

        $scope.ShowModel = function (clientId) {
            //$scope.GenerateLink(clientId);
            var modalInstance = $modal.open({
                templateUrl: 'GeneratedLink.html',
                controller: ModalInstanceCtrl,
                scope: $scope,
                resolve: {
                    ClientId: function () { return clientId; }
                }
            });
        };

        $scope.GenerateLink = function (ClientId) {
            $rootScope.IsAjaxLoading = true;
            ClientCreateService.GenerateLink(ClientId).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        $scope.kycLink = result.data.DataList;
                        //toastr.success($scope.kycLink, 'KYC Link Created'); //place temporary: becasue modal is not working
                        $scope.RefreshTable();
                    } else {
                        $scope.kycLink = "";
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
                $rootScope.IsAjaxLoading = false;
            });
        };

        $scope.DeleteLink = function (ClientId) {
            $rootScope.IsAjaxLoading = true;
            ClientCreateService.DeleteLink(ClientId).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
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

        $scope.GetCountryList = function () {
            ClientCreateService.RetrieveCountry().then(function (result) {
                $scope.Country = result.data.DataList;
            });
        };
        $scope.GetCountryList();


        $scope.FilterClient = function (filterData) {
            $scope.RefreshTable();
        };

        $scope.ExportToPDF = function (clientId) {
            document.location.href = "../../Handler/InvoiceClient.ashx?rtype=pdf&clientId=" + clientId
        };
        /*datatable*/
        $scope.RetrieveClient = function () {
            $scope.tableParams = new ngTableParams({
                page: 1,
                count: 10,
                sorting: {
                    ClientCode: 'asc'
                },
                defaultSort: 'asc'
            }, {
                total: 0,
                filterDelay: 750,
                getData: function ($defer, params) {
                    $rootScope.IsAjaxLoading = true;
                    ClientCreateService.GetClientList($scope.filterData.KYCApproved, $scope.filterData.CountryId, $scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter()).then(function (result) {
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
    };
   
    var ModalInstanceCtrl = function ($scope, $rootScope, ClientId, $modalInstance) {
        $scope.GenerateLink(ClientId);

        $scope.ClosePopup = function () {
            $modalInstance.close();
        };
    };

})();

