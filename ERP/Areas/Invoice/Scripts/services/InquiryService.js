/// <reference path="../../../../Scripts/libs/angular/angular.js" />

angular.module("ERPApp.Services")
    .service("InquiryService", [
        "$http",
        function ($http) {
            var list = {};

            list.RetrieveClients = function () {
                return $http({
                    method: "GET",
                    url: "/api/Inquiry/RetrieveClients?ts=" + new Date().getTime(),
                    contentType: "application/json"
                });
            };

            list.RetrieveClientSources = function () {
                return $http({
                    method: "GET",
                    url: "/api/Inquiry/RetrieveClientSources?ts=" + new Date().getTime(),
                    contentType: "application/json"
                });
            };

            list.RetrieveTechnologies = function () {
                return $http({
                    method: "GET",
                    url: "/api/Inquiry/RetrieveTechnologies?ts=" + new Date().getTime(),
                    contentType: "application/json"
                });

            };

            list.RetrieveStatus = function () {
                return $http({
                    method: "GET",
                    url: "/api/Inquiry/RetrieveStatus?ts=" + new Date().getTime()
                });
            };

            list.ChangeInquiryStatus = function (data) {
                var date = data.InquiryDate.split('-');
                var _InquiryDate = new Date(date[2], date[1] - 1, date[0]);
                var _data = {
                    FKClientId: data.FKClientId,
                    FKDocId: data.FKDocId,
                    FKSourceId: data.FKSourceId,
                    FKTechnologyIds: data.FKTechnologyIds,
                    InquiryDate: _InquiryDate,
                    InquiryStatus: data.InquiryStatus,
                    InquiryTitle: data.InquiryTitle,
                    IsActive: data.IsActive,
                    IsDeleted: data.IsDeleted,
                    PKInquiryId: data.PKInquiryId,
                    Remarks: data.Remarks,
                    StatusId: data.StatusId
                };
                return $http({
                    method: "POST",
                    url: "/api/Inquiry/ChangeInquiryStatus?ts=" + new Date().getTime(),
                    data: _data,
                    contentType: 'application/json; charset=utf-8'
                });
            };

            list.RetrieveInquiries = function (filterClientId, timezone, page, count, orderby, Title, Status, Source) {
                return $http({
                    method: "GET",
                    url: "/api/Inquiry/RetrieveInquiries?ts=" + new Date().getTime() + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&title=" + Title + "&status=" + Status + "&source=" + Source + "&clientId=" + filterClientId,
                    contentType: "application/json; charset=utf-8"
                });
            }

            list.RetrieveFinalizedInquiries = function (filterClientId, timezone, page, count, orderby, Title, Status, Source) {
                return $http({
                    method: "GET",
                    url: "/api/Inquiry/RetrieveFinalizedInquiries?ts=" + new Date().getTime() + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&title=" + Title + "&status=" + Status + "&source=" + Source + "&clientId=" + filterClientId,
                    contentType: "application/json; charset=utf-8"
                });
            }

            list.CreateUpdateInquiry = function (data, timezone) {
                var date = data.InquiryDate.split('-');
                var _InquiryDate = new Date(date[2], date[1] - 1, date[0]);
                var _data = {
                    FKClientId: data.FKClientId,
                    FKDocId: data.FKDocId,
                    FKSourceId: data.FKSourceId,
                    FKTechnologyIds: data.FKTechnologyIds,
                    InquiryDate: _InquiryDate,
                    InquiryStatus: data.InquiryStatus,
                    InquiryTitle: data.InquiryTitle,
                    IsActive: data.IsActive,
                    IsDeleted: data.IsDeleted,
                    PKInquiryId: data.PKInquiryId,
                    Remarks: data.Remarks,
                    StatusId: data.StatusId
                };
                return $http({
                    method: "POST",
                    url: "/api/Inquiry/CreateUpdateInquiry?ts=" + new Date().getTime() + "&timezone=" + timezone + "&idate=" + data.InquiryDate,
                    data: _data,
                    contentType: 'application/json; charset=utf-8'
                });
            };

            list.UploadDocument = function (document) {
                return $http({
                    method: "POST",
                    url: "/api/Inquiry/UploadDocument?ts=" + new Date().getTime(),
                    data: document,
                    contentType: "application/json"
                });
            };

            list.DeleteInquiry = function (id) {
                return $http({
                    method: "GET",
                    url: "/api/Inquiry/DeleteInquiry?ts=" + new Date().getTime() + "&inquiryId=" + id,
                    contentType: "application/json"
                });
            };

            list.RetrieveDocument = function (id) {
                return $http({
                    method: "GET",
                    url: "/api/Inquiry/RetrieveDocument?ts=" + new Date().getTime() + "&inquiryId=" + id,
                    contentType: "application/json"
                });
            };

            list.GetInquiry = function (inquiryId) {
                return $http({
                    method: "GET",
                    url: "/api/Inquiry/GetInquiry?ts=" + new Date().getTime() + "&inquiryId=" + inquiryId,
                    contentType: "application/json"
                });
            };

            
            return list;
        }
    ]);