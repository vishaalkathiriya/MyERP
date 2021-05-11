
angular.module("ERPApp.Controllers").controller("GroupCtrl", [
    "$scope",
    "$rootScope",
    "GroupService",
     "$http",
    "$filter",
    "ngTableParams",
    function ($scope, $rootScope, GS, $http, $filter, ngTableParams) {
        $scope.editData = $scope.editData || {};
        $scope.lastGroup = $scope.lastGroup || {};
        $scope.mode = "Add";
        $scope.SaveText = "Save";
        $rootScope.isFormVisible = false;
        $scope.isFirstFocus = false;

        $scope.AddGroup = function () {
            $scope.mode = "Add";
            $scope.SaveText = "Save";
            $scope.editData = {};
            $rootScope.isFormVisible = true;
        }

        $scope.CreateUpdateGroup = function (group) {
           
            GS.CreateUpdateGroup(group).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType === 1) {
                        $scope.editData = {
                            groupName: '',
                        };
                        $scope.RefreshTable();

                        if ($scope.mode === "Edit") {
                            $rootScope.isFormVisible = false;
                        }
                        $scope.isFirstFocus = false;
                        $scope.frmGroup.$setPristine();
                        if ($scope.mode == "Add") {
                            toastr.success(result.data.Message, 'Success');
                        }
                        else if ($scope.mode == "Edit") {
                            toastr.success(result.data.Message, 'Success');
                        }
                    } else if (result.data.MessageType === 2) {
                        toastr.warning(result.data.Message, 'Record already exists');
                    }
                    else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                }
                else {
                    $rootScope.redirectToLogin();
                }
            });
        }

        $scope.UpdateGroup = function (group) {

            $scope.editData = {
                groupId : group.GroupId,
                groupName: group.GroupName,
                groupNote: group.GroupNote,
               
            };
            $scope.lastGroup = {
                GroupId: group.GroupId,
                GroupName: group.GroupName,
                GroupNote: group.GroupNote,
               
            }

            $rootScope.isFormVisible = true;
            $scope.mode = "Edit";
            $scope.SaveText = "Update";
        }

        $scope.DeleteGroup = function (GroupId) {

            $rootScope.isFormVisible = false;
            GS.DeleteGroup(GroupId).then(
               function (result) {
                   if (result.data.IsValidUser) {
                       if (result.data.MessageType == 1) { // 0:Error
                           toastr.success('Your record is successfully deleted', 'Success');
                           $scope.RefreshTable();
                       } else if (result.data.MessageType == 2) { // 1:Warning
                           toastr.warning(result.data.Message, 'Warning!');
                       } else {
                           toastr.error(result.data.Message, 'Opps, Something went wrong');
                       }
                   }
                   else {
                       $rootScope.redirectToLogin();
                   }

               });

        }
      
        $scope.RetriveGroups = function () {

            $scope.tableParams = new ngTableParams({
                page: 1,
                count: 10,
                sorting: {
                    GroupName: 'asc'
                },
                defaultSort: 'asc'
            }, {
                total: 0,
                filterDelay: 750,
                getData: function ($defer, params) {
                    $rootScope.IsAjaxLoading = true;
                    $scope.isRecord = false;
                    GS.RetriveGroups($scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter().GroupName).then(function (result) {
                        if (result.data.IsValidUser) {
                            //display no data message
                            if (result.data.MessageType != 0) { // 0:Error
                                if (result.data.DataList.total == 0) {
                                    $scope.noRecord = true;
                                    $scope.filterText = params.filter().GroupName;
                                } else {
                                    $scope.noRecord = false;
                                }
                                params.total(result.data.DataList.total);
                                $defer.resolve($scope.Groups = result.data.DataList.result);
                                $rootScope.IsAjaxLoading = false;
                            }
                            else {
                                toastr.error(result.data.Message, 'Opps, Something went wrong');
                            }
                        }
                        else {
                            $rootScope.redirectToLogin();
                        }
                    });
                }
            })
        }
        $scope.RetriveGroups();

        $scope.CloseGroup = function () {
            $rootScope.isFormVisible = false;
            $scope.isFirstFocus = false;
        }
        $scope.ResetGroup = function () {
            if ($scope.mode === "Add") {
                $scope.editData = {
                    groupName: '',
                    groupNote: '',
           
                }
            }
            else if ($scope.mode === "Edit") {
                $scope.editData = {
                    groupId: $scope.lastGroup.GroupId,
                    groupName: $scope.lastGroup.GroupName,
                    groupNote: $scope.lastGroup.GroupNote,
           
                }
            }
        }
        $scope.RefreshTable = function () {
            $scope.tableParams.reload();
        };
    }

]);