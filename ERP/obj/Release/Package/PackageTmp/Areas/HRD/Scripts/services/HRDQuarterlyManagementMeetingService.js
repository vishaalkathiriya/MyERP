/// <reference path="../libs/angular/angular.js" />


angular.module("ERPApp.Services")
    .service("QuarterlyManagementMeetingService", [
    "$http",
    function ($http) {
        var list = {};

        //BEGIN ADD AND UPDATE 
        list.CreateUpdateMeeting = function (data, _timezone) {
            return $http({
                method: "POST",
                url: "/api/HRDQuarterlyManagementMeeting/CreateUpdateMeeting?ts=" + new Date().getTime() + "&timezone=" + _timezone,
                data: data,
                contentType: "application/json"
            });
        };
        //END ADD AND UPDATE 


        //BEGIN GET A LIST SOCIAL WELFARE EXPENSE INFOMATION 
        list.GetMeetingList = function (timezone, page, count, orderby, Title, startDate, endDate) {
            return $http({
                method: "GET",
                url: "/api/HRDQuarterlyManagementMeeting/GetMeetingList?ts=" + new Date().getTime() + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&Title=" + Title + "&startDate=" + startDate + "&endDate=" + endDate
            });
        };
        //END GET A LIST SOCIAL WELFARE EXPENSE INFOMATION

        //BEGIN DELETE SOCIAL WELFARE EXPENSE INFOMATION
        list.DeleteMeeting = function (id) {
            return $http({
                method: "POST",
                url: "/api/HRDQuarterlyManagementMeeting/DeleteMeeting?ts=" + new Date().getTime(),
                data: id,
                contentType: "application/json"
            });
        };
        //END DELETE SOCIAL WELFARE EXPENSE HELP INFOMATION

        // // USE FOR PDF OPEN IN NEW TAB
        //list.ShowMeetingInfo = function (id) {
        //    console.log(id);
        //    return $http({
        //        method: "POST",
        //        url:  "./Handler/HRDQuarterlyManagementMeetingPDFFile.ashx?SrNo=" + id    
        //    });
        //};

        return list;
    }
]);