
angular.module("ERPApp.Services")
    .service("ABPrintService", [
    "$http",
    function ($http) {
        var contact = {};
        contact.PrintAddress = function (group) {
       
            return $http({
                method: "POST",
                cache: false,
                url: "/api/ABContact/GetPrintAddress?ts=" + new Date().getTime(),
                data: group,
                contentType: 'application/json; charset=utf-8'
            });
        }
        contact.GroupList = function () {
            return $http({
                method: "GET",
                url: "/api/ABGroup/GetGroupList?ts=" + new Date().getTime()
            });
        }
        contact.TemplateList = function () {
            return $http({
                method: "GET",
                url: "/api/ABTemplate/GetTemplateList?ts=" + new Date().getTime()
            });
        }
        return contact;
    }
    ]);