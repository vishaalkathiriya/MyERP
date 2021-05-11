/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services")
    .service("SRPartIssueService", [
        "$http",
        function ($http) {

            var partissuelist = {};

            /*retrieve SR Machine list*/
            partissuelist.RetrieveSRMachine = function () {
                return $http({
                    method: 'GET',
                    url: "./../api/SRPartIssue/RetrieveSRMachineList?ts=" + new Date().getTime(),
                });
            }

            /*retrieve SR Part list*/
            partissuelist.RetrieveSRParts = function () {
                return $http({
                    method: 'GET',
                    url: "./../api/SRPartIssue/RetrieveSRPartList?ts=" + new Date().getTime(),
                });
            }

            /*get SR-Part-Issue list*/
            partissuelist.GetSRPartIssueList = function (timezone, page, count, orderby, filter1, filter2, filter3, filter4, filter5) {
                return $http({
                    method: "GET",
                    url: "./../api/SRPartIssue/GetSRPartIssueList?ts=" + new Date().getTime() + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&filter1=" + filter1 + "&filter2=" + filter2 + "&filter3=" + filter3 + "&filter4=" + filter4 + "&filter5=" + filter5
                });
            };

            /*add edit Part-Issue*/
            partissuelist.CreateUpdateSRPartIssue = function (data) {
                return $http({
                    method: "POST",
                    url: "./../api/SRPartIssue/SaveSRPartIssue?ts=" + new Date().getTime(),
                    data: data,
                    contentType: "application/json"
                });
            };

            /*delete Part-Issue*/
            partissuelist.DeleteSRPartIssue = function (id) {
                return $http({
                    method: "POST",
                    url: "./../api/SRPartIssue/DeleteSRPartIssue?ts=" + new Date().getTime(),
                    data: id,
                    contentType: "application/json"
                });
            };

            return partissuelist;
        }
    ]);