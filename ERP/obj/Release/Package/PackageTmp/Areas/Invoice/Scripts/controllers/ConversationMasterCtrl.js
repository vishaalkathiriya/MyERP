/// <reference path="../../../../Scripts/libs/angular/angular.js" />

(function () {

    'use strict';

    // Controller Signature
    angular.module("ERPApp.Controllers")
        .controller("ConversationMasterCtrl", [
            "$scope", "$rootScope", "$timeout", "ConversationService", "$http", "$filter", "$q", "$document", "$window",
            ConversationMasterCtrl
        ]);

    // Conversation Master Controller Function
    function ConversationMasterCtrl($scope, $rootScope, $timeout, CS, $http, $filter, $q, $document, $window) {

        $scope.master = {
            ClientId: 0,
            filterData: {},
            CurrentTab: "",
            IsInvoiceTabOpened: false, //for generate invoice
            SelectedMilestoneList: [], //for generate invoice
            CurrencyList: []
        };
        $scope.master.filterData.ClientId = 0;

        $scope.$watch('master.ClientId', function (newValue) {
            if (newValue == 0) {
                $scope.master.filterData.ClientId = 0;
            }
        });

        $scope.FilterByClient = function (clientId) {
            $scope.master.filterData.ClientId = clientId;
        };

        $scope.LoadClientList = function () {
            CS.GetClientList().then(function (result) {
                $scope.ClientList = result.data.DataList;
            });
        };

        $scope.LoadCurrencyList = function () {
            CS.GetCurrencyList().then(function (result) {
                $scope.master.CurrencyList = result.data.DataList;
            });
        };
        $scope.LoadCurrencyList();
    }
})();