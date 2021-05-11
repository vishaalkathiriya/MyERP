/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services")
    .service("SRMachineReportService", [
        "$http",
        function ($http) {

            var machinereportlist = {};

            /*retrieve sr type list*/
            machinereportlist.RetrieveSRType = function () {
                return $http({
                    method: 'GET',
                    url: "./../api/SRMachineReport/RetrieveSRTypeList?ts=" + new Date().getTime(),
                });
            }

            /*retrieve sr sub type list by type id*/
            machinereportlist.RetrieveSRSubTypeList = function (filterTypes) {
                return $http({
                    method: 'GET',
                    url: "./../api/SRMachineReport/RetrieveSRSubTypeList?ts=" + new Date().getTime() + "&filterTypes=" + filterTypes,
                });
            }

            /*retrieve sr parameter list by sub Type Id*/
            machinereportlist.RetrieveSRParameterList = function (filterSubTypes) {
                return $http({
                    method: 'GET',
                    url: "./../api/SRMachineReport/RetrieveSRParameterList?ts=" + new Date().getTime() + "&filterSubTypes=" + filterSubTypes,
                });
            }

            ///*retrieve sr Floor-Wing*/
            //machinereportlist.RetrieveSRFloorWingsList = function () {
            //    return $http({
            //        method: 'GET',
            //        url: "./../api/SRMachineReport/RetrieveSRFloorWingsList?ts=" + new Date().getTime(),
            //    });
            //}

            /*retrieve sr Locations*/
            machinereportlist.RetrieveSRLocationList = function () {
                return $http({
                    method: 'GET',
                    url: "./../api/SRMachineReport/RetrieveLocationsList?ts=" + new Date().getTime(),
                });
            }

            /*retrieve Machine-Report Data*/
            machinereportlist.GetMachineReportData = function (timezone, page, count, orderby, filterTypes, filterSubTypes, filterParameters, filterLocations, startDate, endDate, filter1, filter2, filter3, filter4, filter5, filter6, filter7) {
                return $http({
                    method: 'GET',
                    url: "./../api/SRMachineReport/GetMachineReportData?ts=" + new Date().getTime() + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&filterTypes=" + filterTypes + "&filterSubTypes=" + filterSubTypes + "&filterParameters=" + filterParameters + "&filterLocations=" + filterLocations + "&startDate=" + startDate + "&endDate=" + endDate + "&filter1=" + filter1 + "&filter2=" + filter2 + "&filter3=" + filter3 + "&filter4=" + filter4 + "&filter5=" + filter5 + "&filter6=" + filter6 + "&filter7=" + filter7,
                });
            }

            return machinereportlist;
        }
    ]);