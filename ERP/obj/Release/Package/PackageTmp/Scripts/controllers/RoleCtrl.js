/// <reference path="../libs/angular/angular.min.js" />


(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("RoleCtrl", [
            "$scope","$modal", "$rootScope", "$timeout", "RoleService", "$http", "$filter","$q", "ngTableParams",
            roleCtrl
        ]);


    //Main controller function
    function roleCtrl($scope, $modal, $rootScope, $timeout, RoleService, $http, $filter,$q, ngTableParams) {
        $scope.editData = $scope.editData || {};
        $scope.mode = "Add";
        $scope.saveText = "Save";
        $scope.timeZone = new Date().getTimezoneOffset().toString(); //timezone difference in minutes
        $scope.isFocusFirst = false;
        $scope.storage = { lastRecord: "" };

        /*reset the form*/
        function ResetForm() {
            $scope.editData = {
                RolesId:0,
                Roles: "",
                IsActive: true
            };
            $scope.roleform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        }

        /*add new role*/
        $scope.AddRole = function () {
            $rootScope.isFormVisible = true;
            $scope.mode = "Add";
            $scope.saveText = "Save";
            ResetForm(); //reset form
        };

        /*save role*/
        $scope.CreateUpdateRole = function (role) {
            RoleService.CreateUpdateRole(role).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        toastr.success(result.data.Message, 'Success');
                        $scope.editData = { RolesId:0, Roles: '', IsActive: true };
                        $scope.RefreshTable();
                        $scope.storage.lastRecord = {};
                        $scope.isFirstFocus = false;
                        $scope.roleform.$setPristine();
                        if ($scope.mode === "Edit") {
                            $rootScope.isFormVisible = false;
                            $scope.saveText = "Save";
                        }
                    } else if (result.data.MessageType == 2) { // 2:Warning
                        toastr.warning(result.data.Message, 'Record already exists');
                    }
                    else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
                $rootScope.IsAjaxLoading = false;
            });
        }

        /*refresh table after */
        $scope.RefreshTable = function () {
            $scope.tableParams.reload();
        };

        /*active/inactive role*/
        $scope.ChangeStatus = function (id, status) {
            var oper = status == true ? "InActive" : "Active";
            $rootScope.IsAjaxLoading = true;
            RoleService.ChangeStatus(id, status).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        toastr.success(result.data.Message, 'Success');
                        $scope.RefreshTable();
                    } else if (result.data.MessageType == 2) { // 2:Warning
                        toastr.warning(result.data.Message, 'Warning!');
                    } else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
                $rootScope.IsAjaxLoading = false;
            });
        };

        /*reset the form*/
        $scope.ResetRole = function () {
            if ($scope.mode == "Edit") {
                $scope.editData.RolesId = $scope.storage.lastRecord.RolesId;
                $scope.editData.Roles = $scope.storage.lastRecord.Roles;
                $scope.editData.IsActive = $scope.storage.lastRecord.IsActive;
            } else { //mode == add
                ResetForm();
            }
        };

        /*cancel button click event*/
        $scope.CloseRole = function () {
            $scope.mode = "Add";
            ResetForm();
            $rootScope.isFormVisible = false;
        };

        /*get record for edit role*/
        $scope.UpdateRole = function (_role) {
            $scope.storage.lastRecord = _role;
            $rootScope.isFormVisible = true;
            $scope.mode = "Edit";
            $scope.saveText = "Update";

            $scope.editData.RolesId = _role.RolesId;
            $scope.editData.Roles = _role.Roles;
            $scope.editData.IsActive = _role.IsActive;
            $scope.roleform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        };

        /*delete role*/
        $scope.DeleteRole = function (id) {
            $rootScope.IsAjaxLoading = true;
            RoleService.DeleteRole(id).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        toastr.success(result.data.Message, 'Success');
                        $scope.RefreshTable();
                    } else if (result.data.MessageType == 2) { // 2:Warning
                        toastr.warning(result.data.Message, 'Warning!');
                    } else {                                    //0:Error
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
                $rootScope.IsAjaxLoading = false;
            });
        };

        /*export to excel*/
        $scope.ExportToExcel = function () {
            document.location.href = "../../Handler/Role.ashx?timezone=" + $scope.timeZone
        };

        /*datatable*/
        $scope.RetrieveRole = function () {
            $scope.tableParams = new ngTableParams({
                page: 1,
                count: 10,
                sorting: {
                    Roles: 'asc'
                },
                defaultSort:'asc'
            }, {
                total: 0,
                getData: function ($defer, params) {
                    $rootScope.IsAjaxLoading = true;
                    $scope.isRecord = false;
                    RoleService.GetRoleList($scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter().Roles).then(function (result) {
                        if (result.data.IsValidUser) {
                            if (result.data.MessageType != 0) { // 0:Error
                                //display no data message
                                if (result.data.DataList.total == 0) {
                                    $scope.noRecord = true;
                                    $scope.filterText = params.filter().Roles;
                                } else {
                                    $scope.noRecord = false;
                                }

                                params.total(result.data.DataList.total);
                                $defer.resolve($scope.documents = result.data.DataList.result);
                            } else {
                                toastr.error(result.Message, 'Opps, Something went wrong');
                            }
                        } else {
                            $rootScope.redirectToLogin();
                        }
                        $rootScope.IsAjaxLoading = false;
                    });
                }
            });
        }

        $scope.ShowModel = function (role) {
            var modalInstance = $modal.open({
                templateUrl: 'AssignRightsPopup.html',
                controller: ModalInstanceCtrl,
                scope: $scope,
                resolve: {
                    Role: function () { return role; } //return anything that you want to pass to model
                }
            });
        };

        $scope.ShowUsersModel = function (role) {
            var userModelInstance = $modal.open({
                templateUrl: 'AssignedUsersPopup.html',
                controller: AssignedUsersCtrl,
                scope: $scope,
                resolve: {
                    UsersResult: function () {
                        return RoleService.GetUsersByRole(role.RolesId).then(function (result) {
                            return result;
                        });
                    }
                }
            });
        };
    };
   

    var AssignedUsersCtrl = function ($scope, $rootScope, UsersResult, $modalInstance) {
        
        var result = UsersResult.data;
        if (result.IsValidUser) {
            $scope.roleUsers = result.DataList;
        }
       
        $scope.CloseUserPopup = function () {
            $modalInstance.close();
        };
    };


    // BEGIN MODAL INSTANCE CONTROLLER
    var ModalInstanceCtrl = function ($scope, $rootScope, RoleService, $filter,$q, $modalInstance, Role, $timeout) {
        $scope.rows = [];
        $scope.RoleId = Role.RolesId;
        $scope.RoleName = Role.Roles;
        
        RoleService.GetModuleSubModuleList($scope.RoleId).then(function (result) {//Attr: Pass role id to get existing entries
            if (result.data.IsValidUser) {
                if (result.data.MessageType == 1) { // 1:Success
                    $scope.rows = _.groupBy(result.data.DataList, "ModuleName");
                    
                    angular.forEach($scope.rows, function (value, key) {
                        angular.forEach(value, function (v1, k1) {
                            //check if all access right entry is selected
                            var checkSub = true;
                            angular.forEach(v1.AllowedAccess, function (v2, k2) {
                                if (v2.IsSelected == false) {
                                    checkSub = false;
                                }
                            });
                            v1.IsSubSelected = checkSub;
                        });

                        //check if all sub module is selected
                        var checkMain = true;
                        angular.forEach(value, function (v3, k3) {
                            if (v3.IsSubSelected == false) {
                                checkMain = false;
                            }
                        });
                        value.IsMainSelected = checkMain;
                    });
                } else {
                    toastr.error(result.data.Message, 'Opps, Something went wrong');
                }
            } else {
                $rootScope.redirectToLogin();
            }
            $rootScope.IsAjaxLoading = false;
        });

        $scope.Close = function () {
            $modalInstance.close();
        };


        /*Select all sub modules and access rights checkbox*/
        $scope.ModuleCheckboxClick = function (module, $event) {
            $event.stopPropagation();

            angular.forEach($scope.rows, function (value, key) {
                if (module == key) {
                    angular.forEach(value, function (v1, k1) {
                        v1.IsSubSelected = value.IsMainSelected ? false : true; //process selection of sub module checkbox
                        angular.forEach(v1.AllowedAccess, function (v2, k2) {
                            v2.IsSelected = value.IsMainSelected ? false : true; //process selection of access rights checkbox
                        });
                    });
                    value.IsMainSelected = value.IsMainSelected ? false : true;
                }
            });
        };

        /*Sub module select all access right checkbox*/
        $scope.SubModuleCheckboxClick = function (event,list, $event) {
            $event.stopPropagation();
            angular.forEach(event.AllowedAccess, function (value, key) {
                value.IsSelected = event.IsSubSelected ? false : true; //process selection of sub module access right checkbox
            });

            //check if all sub module is selected then select main module also
            var checkMain = true;
            $timeout(function () {
              angular.forEach(list, function (val, kal) {
                  if (!val.IsSubSelected) {
                      checkMain = false;
                  }
              });
              list.IsMainSelected = checkMain;
            }, 0);
        };

        /*check sub module and main module on change of access right selection*/
        $scope.CheckSubModule = function (event, list) {
            var checkSub = true;
            angular.forEach(event.AllowedAccess, function (value, key) {
                if (value.IsSelected == false) {
                    checkSub = false;
                }
            });
            event.IsSubSelected = checkSub;

            //check if all sub module is selected then select main module also
            var checkMain = true;
            $timeout(function () {
                angular.forEach(list, function (val, kal) {
                    if (!val.IsSubSelected) {
                        checkMain = false;
                    }
                });
                list.IsMainSelected = checkMain;
            }, 0);
        };

        /*generate list of access rights [only selected rights]*/
        function GenerateAccessRightList(callback) {
            var prom = [];
            angular.forEach($scope.rows, function (v1, k1) {
                angular.forEach(v1, function (v2, k2) {
                    var ids = "";
                    angular.forEach(v2.AllowedAccess, function (value, key) {
                        if (value.IsSelected) {
                            ids += value.Id + ",";
                        }
                    });

                    if (ids != "") {//come inside if any single checkbox is checked
                        prom.push({
                            RoleId: $scope.RoleId,
                            ModuleId: v2.ModuleId,
                            SubModuleId: v2.SubModuleId,
                            Permission: ids
                        });
                    }
                });
            });
            
            $q.all(prom).then(function () {
                callback(prom);
            });
        };

        /*process each row of list and save it*/
        function ProcessAccessRights(list, callback) {
            var index = 0, length = list.length;
            $scope.DeleteAccessPermission().then(function (result) {//Delete: remove all entries of selected role before saving
                function saveData() {
                    var line = list[index];
                    SaveAccessPermission(line).then(function (result) {
                        if (index == length - 1) {
                            callback(result);
                        } else {
                            index++;
                            saveData();
                        }
                    });
                }
                saveData();
            });
        };

        /*save access rights*/
        function SaveAccessPermission(line) {
            return RoleService.SaveAccessPermission(line);
        };

        $scope.DeleteAccessPermission = function () {
            return RoleService.DeleteAccessPermission($scope.RoleId);
        };

        /*Assign access rights permission*/
        $scope.AssignRights = function () {
            $rootScope.IsAjaxLoading = true;
            GenerateAccessRightList(function (list) { //Callback: ensure to complete list generation
                if (list.length > 0) { //Process: this array is not empty
                    ProcessAccessRights(list, function (result) {//Save: create access rights
                        if (result.data.IsValidUser) {
                            if (result.data.MessageType == 1) {
                                $scope.Close();
                                toastr.success(result.data.Message, 'Success');
                            } else if (result.data.MessageType == 0) {
                                toastr.error(result.data.Message, 'Opps, Something went wrong');
                            }
                        } else {
                            $rootScope.redirectToLogin();
                        }
                        $rootScope.IsAjaxLoading = false;
                    });
                } else { //Alert: There is no any selection in form
                    toastr.warning("Please select something to assign access rights", 'Warning');
                }
            });
        };
    };

    // END MODAL INSTANCE CONTROLLER
})();

