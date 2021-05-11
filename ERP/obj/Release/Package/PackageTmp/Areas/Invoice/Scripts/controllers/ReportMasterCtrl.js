/// <reference path="../../../../Scripts/libs/angular/angular.js" />

(function () {

    'use strict';

    angular.module("ERPApp.Controllers")
        .controller("ReportMasterCtrl", [
            "$scope", "$rootScope", "$timeout", "ReportMasterService", "$http", "$filter", "$q", "$document", "$window",
            ReportMasterCtrl]);

    function ReportMasterCtrl($scope, $rootScope, $timeout, RMS, $http, $filter, $q, $document, $window) {
        
    }
})();