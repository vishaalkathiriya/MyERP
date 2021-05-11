/// <reference path="../../../../Scripts/libs/angular/angular.js" />

angular.module("ERPApp.Services")
    .service("ProposalService", [
        "$http",
        function ($http) {
            var list = {};

            list.RetrieveProposals = function (inquiryId, timezone) {
                return $http({
                    method: "GET",
                    url: "/api/Proposal/RetrieveProposals?ts=" + new Date().getTime() + "&inquiryId=" + inquiryId + "&timezone=" + timezone,
                    contentType: "application/json"
                });
            };

            list.CreateUpdateProposal = function (data, timezone) {
                return $http({
                    method: "POST",
                    url: "/api/Proposal/CreateUpdateProposal?ts=" + new Date().getTime() + "&timezone=" + timezone,
                    data: data,
                    contentType: 'application/json; charset=utf-8'
                });
            };

            list.UploadDocument = function (document) {
                return $http({
                    method: "POST",
                    url: "/api/Proposal/UploadDocument?ts=" + new Date().getTime(),
                    data: document,
                    contentType: "application/json"
                });
            };

            list.DeleteProposal = function (id) {
                return $http({
                    method: "GET",
                    url: "/api/Proposal/DeleteProposal?ts=" + new Date().getTime() + "&proposalId=" + id,
                    contentType: "application/json"
                });
            };

            list.RetrieveDocument = function (id) {
                return $http({
                    method: "GET",
                    url: "/api/Proposal/RetrieveDocument?ts=" + new Date().getTime() + "&proposalId=" + id,
                    contentType: "application/json"
                });
            };

            list.ChangeProposalStatus = function (id) {
                return $http({
                    method: "GET",
                    url: "/api/Proposal/ChangeProposalStatus?ts=" + new Date().getTime() + "&proposalId=" + id,
                    contentType: "application/json"
                });
            };

            list.IsInquiryConfirmed = function (id) {
                return $http({
                    method: "GET",
                    url: "/api/Proposal/IsInquiryConfirmed?ts=" + new Date().getTime() + "&inquiryId=" + id,
                    contentType: "application/json"
                });
            };

            return list;
        }
    ]);