/// <reference path="../libs/angular/angular.min.js" />


(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("ClientCreatePartialCtrl", [
            "$scope", "$rootScope", "$timeout", "ClientCreateService", "$http", "$filter",
            clientCreatePartialCtrl
        ]);


    //Main controller function
    function clientCreatePartialCtrl($scope, $rootScope, $timeout, ClientCreateService, $http, $filter) {
        $scope.btnPSaveText = "Save";
        $scope.editDataP = $scope.editDataP || {};
        $scope.editDataP.CPrefix = "Mr.";
        $scope.ShowLink = false;

        $scope.validateDropSource = function () {
            if ($scope.editDataP.SourceId && $scope.editDataP.SourceId != 0) return false;
            return true;
        };

        $scope.ResetPClient = function () {
            $scope.editDataP = {
                PKClientId: 0,
                CompanyName: "",
                ContactPerson: "",
                CPrefix: "Mr.",
                CountryId: 0,
                SourceId: 0
            };
            
            $scope.ccpform.$setPristine();
        };

        $scope.CreateUpdatePartialClient = function (data) {
            $rootScope.IsAjaxLoading = true;
            data.PKClientId = $scope.master.clientId;
            data.FKSourceId = data.SourceId;
            ClientCreateService.CreateUpdatePartialClient(data).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        toastr.success(result.data.Message, 'Success');
                        $scope.master.isPartialTabDisabled = false;
                        $scope.master.isFullTabDisabled = true;
                        $scope.master.isConfirmButtonDisabled = false;

                        $scope.master.clientId = result.data.DataList;
                        $scope.master.mode = "Edit";
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

        //when initial loading of partial tab
        $scope.LoadPartialData = function () {
            $scope.loadCountryDrop().then(function (resCountry) {
                $scope.Country = resCountry.data.DataList;

                ClientCreateService.LoadClientSource().then(function (resClientSource) {
                    $scope.ClientSource = resClientSource.data.DataList;

                    $timeout(function () {
                        $scope.editDataP.CountryId = 0;
                        $scope.editDataP.SourceId = 0;

                        if ($scope.master.mode == "Edit") {
                            ClientCreateService.GetClient($scope.master.clientId).then(function (resClient) {
                                var line = resClient.data.DataList;

                                if (line.IsConfirmed) {
                                    $scope.master.isPartialTabActive = false;
                                    $scope.master.isFullTabActive = true;
                                    $scope.master.isPartialTabDisabled = true;
                                    $scope.master.isFullTabDisabled = false;
                                } else { //client is not confirmed yet
                                    $scope.editDataP = {
                                        CompanyName: line.CompanyName,
                                        CPrefix: line.CPrefix,
                                        ContactPerson: line.ContactPerson,
                                        CountryId: line.CountryId,
                                        SourceId: line.FKSourceId,
                                        URLKey: line.URLKey
                                    };
                                }
                            });
                        }
                    });
                });
            });
        };

        $scope.MakeClient = function () {
            $scope.master.isPartialTabActive = false;
            $scope.master.isFullTabActive = true;
            $scope.master.isPartialTabDisabled = true;
            $scope.master.isFullTabDisabled = false;
        };
    };
})();

