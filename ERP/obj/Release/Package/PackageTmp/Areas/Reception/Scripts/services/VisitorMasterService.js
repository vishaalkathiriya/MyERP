/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services")
    .service("VisitorMasterService", [
        "$http",
        function ($http) {

            var list = {};

            /*retrieve Dept list*/
            list.RetrieveCompany = function () {
                return $http({
                    method: 'GET',
                    url: "./../api/VisitorMaster/RetrieveCompany?ts=" + new Date().getTime(),
                });
            }

            list.RetrieveDesignation = function () {

                return $http({
                    method: 'GET',
                    url: "./../api/VisitorMaster/RetrieveDesignation?ts=" + new Date().getTime(),
                });
            }

            list.RetrieveEcode = function () {

                return $http({
                    method: 'GET',
                    url: "./../api/VisitorMaster/RetrieveEcode?ts=" + new Date().getTime(),
                });
            }

            list.RetrieveRefName = function () {

                return $http({
                    method: 'GET',
                    url: "./../api/VisitorMaster/RetrieveRefName?ts=" + new Date().getTime(),
                });
            }

            list.RetrieveManager = function () {

                return $http({
                    method: 'GET',
                    url: "./../api/VisitorMaster/RetrieveManager?ts=" + new Date().getTime(),
                });
            }

            list.RetrieveDept = function () {

                return $http({
                    method: 'GET',
                    url: "./../api/VisitorMaster/RetrieveDept?ts=" + new Date().getTime(),
                });
            }

            list.GetVisitorData = function (MobileNo) {
                return $http({
                    method: 'GET',
                    url: "./../api/VisitorMaster/GetVisitorData?ts=" + new Date().getTime() + "&MobileNo=" + MobileNo,
                });
            }

            list.GetVisitorDataByVisitorID = function (VisitorID) {

                return $http({
                    method: 'GET',
                    url: "./../api/VisitorMaster/GetVisitorDataByVisitorID?ts=" + new Date().getTime() + "&VisitorID=" + VisitorID,
                });
            }

            list.CreateUpdateVisitor = function (data) {

                return $http({
                    method: "POST",
                    url: "/api/VisitorMaster/CreateUpdateVisitor?ts=" + new Date().getTime(),
                    data: data,
                    contentType: "application/json"
                });
            };

            list.VisitorIn = function (InData, MobileNo) {
                return $http({
                    method: "POST",
                    url: "/api/VisitorMaster/VisitorIn?MobileNo=" + MobileNo,
                    data: InData,
                    contentType: "application/json"
                });
            };
            list.GetInpersonList = function () {
                return $http({
                    method: "GET",
                    url: "/api/VisitorMaster/GetInpersonList?ts=" + new Date().getTime(),

                });
            };

            list.VisitorOut = function (SrNo, RefName, Ecode, VisitorId) {
                return $http({
                    method: "POST",
                    url: "/api/VisitorMaster/VisitorOut?SrNo=" + SrNo + "&RefName=" + RefName + "&Ecode=" + Ecode + "&VisitorId=" + VisitorId,

                });
            };

            list.GetVisitorDetail = function (timezone, page, count, orderby, filter1, filter2, filter3, filter4, filter5, filter6) {
                return $http({
                    method: "POST",
                    contentType: "application/json",
                    url: "/api/VisitorMaster/GetVisitorDetail?ts=" + new Date().getTime() + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&VisitorId=" + filter1 + "&Mobile=" + filter2 + "&Name=" + filter3 + "&Address=" + filter4 + "&Company=" + filter5 + "&Designation=" + filter6
                });
            };

            list.DeleteVisitorData = function (obj) {
                return $http({
                    method: "POST",
                    url: "/api/VisitorMaster/DeleteVisitorData?ts=" + new Date().getTime(),
                    data: obj,
                    contentType: "application/json"
                });
            };
            return list;
        }
    ]);