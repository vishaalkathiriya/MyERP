/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services")
.service("SeaftyTrainigRecordsService", [
    "$http",
    function ($http) {
        var list = {};
        //BEGIN ADD AND UPDATE SEAFTY TRAINING RECORDS
        list.CreateUpdateSeaftyTrainigRecords = function (data, _timezone) {
            return $http({
                method: "POST",
                url: "/api/HRDSeaftyTrainingRecords/SaveSeftyTrainingRecords?ts=" + new Date().getTime() + "&timezone=" + _timezone,
                data: data,
                contentType: "application/json"
            });
        };
        //END ADD AND UPDATE SEAFTY TRAINING RECORDS

        //BEGIN GET SEAFTY TRAINING RECORDS
        list.GetSeaftyTrainingRecords = function (timezone, page, count, orderby, SubjectOfTraining, DateOfTraining, Department, ManagerName, NoOfParticipants, TrainersName, startDate, endDate) {
            return $http({
                method: "GET",
                url: "/api/HRDSeaftyTrainingRecords/GetSeaftyTrainigRecords?ts=" + new Date().getTime() + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&SubjectOfTraining=" + SubjectOfTraining + "&DateOfTraining=" + DateOfTraining + "&Department=" + Department + "&ManagerName=" + ManagerName + "&NoOfParticipants=" + NoOfParticipants + "&TrainersName=" + TrainersName + "&startDate=" + startDate + "&endDate=" + endDate
            });
        };
        //END GET SEAFTY TRAINING RECORDS

        //BEGIN DELETE SEAFTY TRAINING RECORDS
        list.DeleteSeaftyTrainig = function (id) {
            return $http({
                method: "POST",
                url: "/api/HRDSeaftyTrainingRecords/DeleteSaftyTrainingRecords?ts=" + new Date().getTime(),
                data: id,
                contentType: "application/json"
            });
        };
        //END DELETE SEAFTY TRAINING RECORDS
        return list;
    }]);