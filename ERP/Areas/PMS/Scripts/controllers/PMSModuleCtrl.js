/// <reference path="../libs/angular/angular.min.js" />


(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("PMSModuleCtrl", [
            "$scope", "$rootScope", "$timeout", "PMSModuleService", "$http", "$filter",
            pmsModuleCtrl
        ]);


    //Main controller function
    function pmsModuleCtrl($scope, $rootScope, $timeout, PMSModuleService, $http, $filter) {
        $scope.editData = $scope.editData || {};
        $scope.timeZone = new Date().getTimezoneOffset().toString(); //timezone difference in minutes
        $rootScope.isFormVisible = false;
        $scope.projectId = 0;
        $scope.moduleId = 0;
        $scope.IsFullView = false;
        $scope.isFirstFocus = false;
        $scope.IsAjaxLoadingPMS = false;
        $scope.empLoginId = window.erpuid;
        

        //$scope.IsmoduleFinish = false;


        $scope.getUsercanFinish=function(empLoginId,CreBy,iscanfinish,assignUser)
        {
            var flag = false;
            if (empLoginId == 1)
            {
                flag = true;
            }else if(CreBy == empLoginId)
            {
                flag = true;
            } else if (assignUser == empLoginId && iscanfinish == true) {
                flag = true;
            } else {
                flag = false;
            }
            return flag;
        }

        $scope.ValidateDropModuleType = function () {
            if ($scope.editData.ModuleType && $scope.editData.ModuleType != 0) return false;
            return true;
        };

        /*reset the form*/
        function ResetForm() {
            $scope.editData = {
                ModuleId: 0,
                ProjectId: 0,
                ModuleName: "",
                ModuleType: 0,
                IsArchived: false,
                IsActive: true
            };

            $scope.modform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        };

        /*add new module*/
        $scope.AddPMSModule = function () {
            $rootScope.isFormVisible = true;
            ResetForm(); //reset form
        };

        /*save module*/
        $scope.CreateUpdateModule = function (mod) {
            mod.ProjectId = $scope.projectId;
            $rootScope.IsAjaxLoading = true;
            PMSModuleService.CreateUpdateModule(mod).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        toastr.success(result.data.Message, 'Success');
                        $scope.editData = { ModuleId: 0, ProjectId: 0, ModuleName: '', IsArchived: false };
                        $rootScope.isFormVisible = false;
                        $scope.GetModuleList();
                        $scope.isFirstFocus = false;
                        $scope.modform.$setPristine();
                    } else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
                $rootScope.IsAjaxLoading = false;
            });
        };

        $scope.GetModuleTypeList = function () {
            //$rootScope.IsAjaxLoading = true;
            PMSModuleService.GetModuleTypeList().then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        $scope.ModuleTypeList = result.data.DataList;
                    } else {
                        //error
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
            });
        };
        $scope.GetModuleTypeList();

        $scope.GetModuleList = function () {
            //$rootScope.IsAjaxLoading = true;
            $scope.IsAjaxLoadingPMS = true;
            PMSModuleService.GetModuleList($scope.timeZone, $scope.projectId).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        $scope.ModuleList = result.data.DataList;
                        //angular.forEach($scope.ModuleList, function (value, key) {
                        //    var AssignedH = 0; var ActualH = 0;
                        //    angular.forEach(value.TodoList, function (v, k) {
                        //        AssignedH += v.AssignedHours ? parseFloat(v.AssignedHours) : 0;
                        //        ActualH += v.ActualHours ? parseFloat(v.ActualHours) : 0;
                        //    });
                        //    value.ModuleHours = ActualH + "H / " + AssignedH + "H";
                        //});

                        //if (result.data.DataList.length == 0) {
                        PMSModuleService.GetProjectName($scope.projectId).then(function (res) {
                            $scope.ProjectName = res.data.DataList;
                            });
                        //} else {
                        //    $scope.ProjectName = $scope.ModuleList[0].ProjectName;
                        //}
                    } else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
                //$rootScope.IsAjaxLoading = false;
                $scope.IsAjaxLoadingPMS = false;
            });
        };

        
        $scope.getColorName = function (todoType) {
            var colorCode;

            switch(todoType) {
                case 1:
                    colorCode = '#AC1818';
                    break;
                case 2:
                    colorCode = '#2C7EA1';
                    break;
                case 3:
                    colorCode = '#737881';
                    break;
            }

            return colorCode;
        }

        /*cancel button click event*/
        $scope.CloseModule = function () {
            $scope.mode = "Add";
            ResetForm();
            $rootScope.isFormVisible = false;
        };

        /*update Module*/
        $scope.isEdit = [];
        $scope.oldData = [];
        $scope.UpdateModule = function (mod, index) {
            $scope.CreateUpdateModule(mod); // update entry
            $scope.isEdit[index] = false;
            $scope.oldData[index] = "";

        };

        /*cancel inline update of module*/
        $scope.CancelModuleUpdate = function (index, ModuleList) {
            $scope.isEdit[index] = false;
            ModuleList[index].ModuleName = $scope.oldData[index];
            $scope.oldData[index] = "";
        };

        /*delete module*/
        $scope.DeleteModule = function (id) {
            $rootScope.IsAjaxLoading = true;
            PMSModuleService.DeleteModule(id).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 0) { // 0:Error
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                    else {
                        toastr.success(result.data.Message, 'Success');
                        $scope.GetModuleList();
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
                $rootScope.IsAjaxLoading = false;
            });
        };

        /*set project id and get modules based on it*/
        $scope.setProjectId = function (id) {
            $scope.projectId = id;
            $scope.GetAssignedUserList();
            $scope.GetModuleList();
        };

        /*TODO ITEM SECTION*/
        $scope.todoText = [];
        $scope.todoHours = [];
        $scope.todoMinutes = [];
        $scope.todoType = [];
        $scope.AddTodoForm = [];
        $scope.SaveTodoItem = function (todoId, moduleId, todoText, todoHours, todoMinutes, todoType) {
            $scope.todoData = {
                TodoId: todoId,
                ModuleId: moduleId,
                TodoText: todoText,
                AssignedHours: (todoHours ? todoHours : 0) + "." + (todoMinutes ? todoMinutes : 0),
                TodoType: todoType
            }

            $rootScope.IsAjaxLoading = true;
            PMSModuleService.SaveTodoItem($scope.todoData).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 0) { // 0:Error
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                    else {
                        toastr.success(result.data.Message, 'Success');
                        if ($scope.IsFullView) {
                            $scope.GetModuleTodoList();
                        } else {
                            $scope.GetModuleList();
                        }
                        $rootScope.isFormVisible = false;
                        $scope.todoText = [];
                        $scope.todoHours = [];
                        $scope.todoMinutes = [];
                        //$scope.todoType = [];
                        $scope.isFirstFocus = true;
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
                $rootScope.IsAjaxLoading = false;
            });
        };

        /*update todo*/
        $scope.UpdateTodoItem = function (todo) {
            $scope.SaveTodoItem(todo.TodoId, todo.ModuleId, todo.TodoText, todo.AssignedHours, todo.AssignedMinutes,todo.TodoType);
        };

        /*cancel inline update of todo*/
        $scope.cancelTodoUpdate = function (t) {
            t.IsEdit = false;
            t.TodoText = t.oldTodoText;
            t.AssignedHours = t.oldTodoHours;
            t.TodoType = t.oldTodoType;

            
        }

        $scope.ShowHideAddTodo = function (index, flag) {
            if (flag) { //display todo add form
                $scope.AddTodoForm[index] = true;
                $scope.isFirstFocus = true;
            } else { //hide todo add form
                $scope.AddTodoForm[index] = false;
            }
        };

        $scope.HoldTodoItem = function (todoId) {
            $rootScope.IsAjaxLoading = true;
            PMSModuleService.HoldTodoItem(todoId).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 0) { // 0:Error
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                    else {
                        toastr.success(result.data.Message, 'Success');
                        if ($scope.IsFullView) {
                            $scope.GetModuleTodoList();
                        } else {
                            $scope.GetModuleList();
                        }
                        $rootScope.isFormVisible = false;
                        $scope.isFirstFocus = false;
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
                $rootScope.IsAjaxLoading = false;
            });
        };

        $scope.DeleteTodoItem = function (todoId) {
            $rootScope.IsAjaxLoading = true;
            PMSModuleService.DeleteTodoItem(todoId).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 0) { // 0:Error
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                    else {
                        toastr.success(result.data.Message, 'Success');
                        if ($scope.IsFullView) {
                            $scope.GetModuleTodoList();
                        } else {
                            $scope.GetModuleList();
                        }
                        $rootScope.isFormVisible = false;
                        $scope.isFirstFocus = false;
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
                $rootScope.IsAjaxLoading = false;
            });
        };

        $scope.CancelModuleFinish = function (index, m) {
            $timeout(function () {
                m.TodoList[index].IsChecked = false;
            });
        };

        $scope.FinishTodoItem = function (todoId, index, m) {
            $rootScope.IsAjaxLoading = true;

            m.TodoList[index].IsChecked = false;
            PMSModuleService.FinishTodoItem(todoId).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 0) { // 0:Error
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                    else {
                        toastr.success(result.data.Message, 'Success');
                        if ($scope.IsFullView) {
                            $scope.GetModuleTodoList();
                        } else {
                            $scope.GetModuleList();
                        }
                        $rootScope.isFormVisible = false;
                        $scope.isFirstFocus = false;
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
                $rootScope.IsAjaxLoading = false;
            });
        };

        /* Begin - Split Assigned Hours by . */
        $scope.GetHourMinutes = function (t) {
            t.AssignedMinutes = t.AssignedHours.toString().split('.')[1];
            t.AssignedHours = t.AssignedHours.toString().split('.')[0];
        }
        /* End - Split Assigned Hours by . */

        
        
        
        $scope.popoverCallback = function (date, userId, projectId, todoId, iscanfinish) {
            if ((userId == 0 || !userId) && !date) { // || (userId > 0 && !hour) || (!userId && hour) || (!userId && !hour)) { //if both is not added  || only user || only hours || no user and hours
                toastr.warning('You should assign either user or due date to this Todo', 'Warning');
            } else {
                var filteredDate = "";
                if (date) {
                    filteredDate = $filter('date')(date, 'dd-MM-yyyy');
                }

                $rootScope.IsAjaxLoading = true;
                PMSModuleService.AssignUser(filteredDate, userId, projectId, todoId, iscanfinish).then(function (result) {
                    if (result.data.IsValidUser) {
                        if (result.data.MessageType == 1) { // 1:Success
                            toastr.success(result.data.Message, 'Success');
                            if ($scope.IsFullView) {
                                $scope.GetModuleTodoList();
                            } else {
                                $scope.GetModuleList();
                            }
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
        };

        $scope.GetAssignedUserList = function () {
            //$rootScope.IsAjaxLoading = true;
            PMSModuleService.GetAssignedUserList($scope.projectId).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        $scope.UserList = result.data.DataList;
                    } else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
                //$rootScope.IsAjaxLoading = false;
            });
        };


        /*TODO LIST PAGE TO VIEW ALL TODO FOR SELECTED MODULE*/
        /*set module id and get todos based on it*/
        $scope.setModuleId = function (id) {
            $scope.IsFullView = true;
            $scope.moduleId = id;
            $scope.GetModuleTodoList();
        };

        $scope.GetModuleTodoList = function () {
            //$rootScope.IsAjaxLoading = true;
            $scope.IsAjaxLoadingPMS = true;
            PMSModuleService.GetModuleTodoList($scope.timeZone, $scope.moduleId).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        $scope.ModuleTodoList = result.data.DataList;
                        $scope.projectId = result.data.DataList[0].ProjectId;
                        $scope.ProjectName = $scope.ModuleTodoList[0].ProjectName;
                        $scope.ModuleName = $scope.ModuleTodoList[0].ModuleName;
                        $scope.GetAssignedUserList();
                    } else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
                //$rootScope.IsAjaxLoading = false;
                $scope.IsAjaxLoadingPMS = false;
            });
        };

        $scope.ActiveTodoItem = function (todoId, isFor) {
            $rootScope.IsAjaxLoading = true;
            PMSModuleService.ActiveTodoItem(todoId, isFor).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 0) { // 0:Error
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                    else {
                        toastr.success(result.data.Message, 'Success');
                        $scope.GetModuleTodoList();
                        $rootScope.isFormVisible = false;
                        $scope.isFirstFocus = false;
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
                $rootScope.IsAjaxLoading = false;
            });
        };

        /* PMS PROJECT PROGRESS */
        $scope.getAllProjectName = function () {
            PMSModuleService.getAllProjectName().then(function (res) {
                $scope.data = res.data.DataList;
            });
        }
    };
})();

