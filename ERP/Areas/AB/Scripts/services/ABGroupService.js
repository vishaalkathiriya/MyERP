angular.module("ERPApp.Services").service("GroupService", [
    "$http",
    function ($http) {
        var group = {};
        group.CreateUpdateGroup = function (_group) {
            var group = {
                GroupId: _group.groupId,
                GroupName: _group.groupName,
                GroupNote: _group.groupNote,
            };

            return $http({
                method: "POST",
                url: "/api/ABGroup/CreateUpdateGroup?ts=" + new Date().getTime(),
                data: group,
                contentType: "application/json"
            });
        }
        group.RetriveGroups = function (timezone, page, count, orderby, filter) {

            return $http({
                method: "GET",
                cache: false,
                url: "/api/ABGroup/RetrieveGroup?ts=" + new Date().getTime() + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&filter=" + filter
            });
        }
        group.DeleteGroup = function (GroupId) {
            return $http({
                method: "POST",
                url: "/api/ABGroup/DeleteGroup?ts=" + new Date().getTime(),
                data: GroupId,
                contentType: "application/json"
            });
        }
       
        return group;
    }

]);