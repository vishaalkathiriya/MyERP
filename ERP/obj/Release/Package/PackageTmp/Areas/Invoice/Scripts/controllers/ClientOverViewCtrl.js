/// <reference path="../libs/angular/angular.min.js" />


(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("ClientOverViewCtrl", [
            "$scope", "$rootScope", "$timeout", "ClientCreateService", "$http", "$filter","$q",
            clientOverViewCtrl
        ]);


    //Main controller function
    function clientOverViewCtrl($scope, $rootScope, $timeout, ClientCreateService, $http, $filter, $q) {
        $scope.editDataF = $scope.editDataF || {};
        $scope.directorList = [];
        $scope.documentList = [];


        /*initial load of data*/
        $scope.LoadOverViewData = function (clientId) {
            ClientCreateService.GetClientOverView(clientId).then(function (result) {
                $scope.editDataF = result.data.DataList;
                console.log($scope.editDataF);
            });
        };
    };
})();

