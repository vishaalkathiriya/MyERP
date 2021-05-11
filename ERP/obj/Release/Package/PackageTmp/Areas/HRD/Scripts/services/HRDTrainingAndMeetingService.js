/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services")
          .service("TrainingAndMeetingService", [
          "$http",
          function ($http) {
              var list = {};
              list.CreateUpdateTrainingAndMeeting = function (data, _timezone) {

                  return $http({
                      method: "POST",
                      url: "/api/HRDTrainingsAndMeeting/SaveTrainingAndMeeting?ts=" + new Date().getTime() + "&timezone=" + _timezone,
                      data: data,
                      contentType: "application/json"
                  });
              };

              list.DeleteTrainingMeeting = function (id) {
                  
                  return $http({
                      method: "POST",
                      url: "/api/HRDTrainingsAndMeeting/DeleteTrainingAndMeeting?ts=" + new Date().getTime(),
                      data: id,
                      contentType: "application/json"
                  });
              };

              list.GetTrainingMeetingList = function (timezone, page, count, orderby, Department, Manager, Subject, NoOfParticipant, Intercom, startDate, endDate) {
                  return $http({
                      method: "GET",
                      url: "/api/HRDTrainingsAndMeeting/GetTrainingAndMeetingList?ts=" + new Date().getTime() + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&Department=" + Department + "&Manager=" + Manager + "&Subject=" + Subject + "&NoOfParticipant=" + NoOfParticipant + "&Intercom=" + Intercom + "&startDate=" + startDate + "&endDate=" + endDate
                  });
              };
              return list;
          }]);