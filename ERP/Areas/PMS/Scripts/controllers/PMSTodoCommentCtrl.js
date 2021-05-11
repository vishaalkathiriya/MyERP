/// <reference path="../libs/angular/angular.min.js" />


(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("PMSTodoCommentCtrl", [
            "$scope", "$modal", "$rootScope", "$timeout", "PMSTodoCommentService", "$http", "$filter", "$q",
            todoCommentCtrl
        ]);

    //Main controller function
    function todoCommentCtrl($scope, $modal, $rootScope, $timeout, PMSTodoCommentService, $http, $filter, $q) {
        $scope.timeZone = new Date().getTimezoneOffset().toString(); //timezone difference in minutes
        $scope.editData = {
            htmlcontent: "",
            multiFileList: [],
            myHour: 0,
            myMinute: 0,
            saveStatus: 1
        };
        $scope.todoId = 0;
        $scope.projectId = 0;
        $scope.isDisabled = true;
        $scope.IsAjaxLoadingPMS = false;
        $scope.empLoginId = window.erpuid;

        $scope.setTodoId = function (todoId, mainPath) {
            $scope.todoId = todoId;
            //$rootScope.IsAjaxLoading = true;
            PMSTodoCommentService.GetProjectId($scope.todoId).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        $scope.projectId = result.data.DataList;
                        $scope.MainPath = mainPath;
                        $scope.GetCommentList();
                        $scope.GetAssignedUserList();
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
                //$rootScope.IsAjaxLoading = false;
            });
        };

        $scope.setTodoStatus = function (todoId) {
            //$rootScope.IsAjaxLoading = true;
            PMSTodoCommentService.GetTodoStatus(todoId).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        $scope.todoStatus = result.data.DataList[0].Status;
                        $scope.todoIsArchived = result.data.DataList[0].IsArchived;
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
                //$rootScope.IsAjaxLoading = false;
            });
        }

        $scope.getStatusList = function () {
            //$rootScope.IsAjaxLoading = true;
            PMSTodoCommentService.GetStatusList().then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) {
                        $scope.todoStatusList = result.data.DataList;
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
                //$rootScope.IsAjaxLoading = false;
            });
        }

        $scope.GetAssignedUserList = function () {
            //$rootScope.IsAjaxLoading = true;
            PMSTodoCommentService.GetAssignedUserList($scope.todoId).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        $scope.AssignedUserList = result.data.DataList;
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
                //$rootScope.IsAjaxLoading = false;
            });
        };

        $scope.DeleteUploadedFile = function (index, list) {
            list.splice(index, 1); // we are not deleting file from server
        };

        $scope.GetCommentList = function () {
            //$rootScope.IsAjaxLoading = true;
            $scope.IsAjaxLoadingPMS = true;
            PMSTodoCommentService.GetCommentList($scope.timeZone, $scope.todoId).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) {
                        $scope.CommentList = result.data.DataList;

                        angular.forEach($scope.CommentList, function (value, key) {
                            value.ImageList = [];
                            value.VideoList = [];
                            value.FileList = [];

                            angular.forEach(value.lstUploadedFile, function (v, k) {
                                var part = v.FileName.split('.');
                                if (part[1] == "jpg" || part[1] == "jpeg" || part[1] == "png") {
                                    value.ImageList.push({ FileName: v.FileName, CaptionText: v.CaptionText });
                                } else if (part[1] == "mp4") {
                                    var file = "/" + $scope.MainPath + "/" + $scope.projectId + "/" + v.FileName;
                                    value.VideoList.push({ FileName: file, CaptionText: v.CaptionText });
                                } else if (part[1] == "txt" || part[1] == "csv" || part[1] == "pdf" || part[1] == "xls" || part[1] == "xlsx" || part[1] == "doc" || part[1] == "docx") {
                                    value.FileList.push({ FileName: v.FileName, CaptionText: v.CaptionText });
                                }
                            });
                        });
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
                //$rootScope.IsAjaxLoading = false;
                $scope.IsAjaxLoadingPMS = false;
            });
        };

        $scope.NotifyUsers = function (commentId, res) {
            var bcc = "";
            angular.forEach($scope.AssignedUserList, function (value, key) {
                if (value.IsSelected) {
                    bcc += value.Id + ",";
                }
            });

            if (bcc != "") {
                PMSTodoCommentService.SendMail(commentId, bcc).then(function (r) {
                    if (r.data.IsValidUser) {
                        if (r.data.MessageType == 1) {
                            toastr.success(r.data.Message, 'Success');
                            $scope.editData = {
                                htmlcontent: "",
                                multiFileList: [],
                                myHour: 0,
                                myMinute: 0
                            };

                        } else {
                            toastr.error(r.data.Message, 'Success');
                        }
                    } else {
                        $rootScope.redirectToLogin();
                    }
                    $rootScope.IsAjaxLoading = false;
                });
            } else { //if no notify user is selected
                toastr.success(res.data.Message, 'Success');
                $rootScope.IsAjaxLoading = false;
            }

            //Reload comment list
            $scope.editData.multiFileList.length = 0; //reset
            $scope.GetCommentList();
            $scope.GetAssignedUserList();
        };

        $scope.SaveComment = function (editData) {
            var totalHours = editData.myHour + "." + editData.myMinute;
            var line = ({ CommentId: 0, TodoId: $scope.todoId, CommentText: editData.htmlcontent, Hours: totalHours, Status: editData.saveStatus });

            $rootScope.IsAjaxLoading = true;
            PMSTodoCommentService.SaveComment(line).then(function (result) {

                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        $scope.editData.htmlcontent = ""; //reset
                        $scope.isDisabled = true;
                        var commentId = result.data.DataList;
                        if (editData.multiFileList.length > 0) {
                            ProcessList(commentId, editData.multiFileList, function (res) {
                                //Notify selected user after uploaded file process
                                $scope.NotifyUsers(commentId, res);
                            });
                        } else { //come here if no files are uploaded
                            $scope.NotifyUsers(commentId, result); //Notify selected user if no upload exists
                        }
                    } else { //Error
                        toastr.error(result.data.Message, 'Opps, something went wrong');
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
            });
        };

        $scope.DeleteModuleTodoComments = function (CommentId) {
            $rootScope.IsAjaxLoading = true;
            PMSTodoCommentService.DeleteModuleTodoComments(CommentId).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType != 0) { // 0:Error
                        toastr.success(result.data.Message, 'Success');
                        $scope.GetCommentList();
                    } else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
                $rootScope.IsAjaxLoading = false;
            });
        }

        function ProcessList(commentId, list, callback) {
            var index = 0, length = list.length;
            function saveData() {
                SaveLine(commentId, list[index]).then(function (result) {
                    if (index == length - 1) {
                        callback(result);
                    } else {
                        index++;
                        saveData();
                    }
                });
            }
            saveData();
        }

        function SaveLine(commentId, line) {
            var li = ({ CommentId: commentId, FileName: line.file, CaptionText: line.caption });
            return PMSTodoCommentService.SaveUploadedFile(li, $scope.projectId);
        }

        /*redirect user to view all original images*/
        $scope.ViewAllOriginal = function (images) {
            var prom = [];
            var data = "";
            angular.forEach(images, function (value, key) {
                data += value.FileName + "$";
                prom.push(value);
            });

            $q.all(prom).then(function () {
                window.open("/Scripts/template/imageList.html?pid=" + $scope.projectId + "&data=" + data, '_blank');
            });

        };

        /*MODEL FOR IMAGE SHOW*/
        $scope.ImageShow = function (iList, index) {
            var modalInstance = $modal.open({
                templateUrl: 'Slider.html',
                controller: ModalImageCtrl,
                scope: $scope,
                resolve: {
                    data: function () { return ({ list: iList, index: index }); } //return anything that you want to pass to model
                }
            });
        };

        /*MODEL FOR VIDEO SHOW*/
        $scope.VideoShow = function (line) {
            var modalInstance = $modal.open({
                templateUrl: 'Video.html',
                controller: ModalVideoCtrl,
                scope: $scope,
                resolve: {
                    line: function () { return line; } //return anything that you want to pass to model
                }
            });
        };

        /* MODEL USED FOR SET MODEL NAME INIT TIME */
        $scope.setModuleName = function (todoId) {
            $scope.todoId = todoId;
            //$rootScope.IsAjaxLoading = true;
            PMSTodoCommentService.setModuleName($scope.todoId).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        //$scope.moduleId = result.data.DataList[0];
                        //$scope.moduleName = result.data.DataList[1];
                        //$scope.todoCommentText = result.data.DataList[2];

                        $scope.moduleId = result.data.DataList[0].ModuleId;
                        $scope.moduleName = result.data.DataList[0].ModuleName;
                        $scope.todoCommentText = result.data.DataList[0].todoCommentText;
                        $scope.AssignUser = result.data.DataList[0].AssignUser;
                        $scope.CanFinish = result.data.DataList[0].CanFinish;
                        $scope.CreBy = result.data.DataList[0].CreBy;

                    }
                } else {
                    $rootScope.redirectToLogin();
                }
                //$rootScope.IsAjaxLoading = false;
            });
        }


        $scope.getUsercanFinish = function (id) {
            var flag = true;
            if (id == 3) {
                if ($scope.empLoginId == 1) {
                    flag = true;
                } else if ($scope.CreBy == $scope.empLoginId) {
                    flag = true;
                } else if ($scope.AssignUser == $scope.empLoginId && $scope.CanFinish == true) {
                    flag = true;
                } else {
                    flag = false;
                }
            }
            return flag;
        }


    };




  

    // BEGIN MODAL IMAGE CONTROLLER
    var ModalImageCtrl = function ($scope, $modalInstance, data) {
        $scope.slideImages = data.list;
        $scope.imgCurrentIndex = data.index;

        $scope.callback = function () {
            $modalInstance.close();
        };
    };

    // BEGIN MODAL VIDEO CONTROLLER
    var ModalVideoCtrl = function ($scope, $modalInstance, line) {
        $scope.videos = line;
        $scope.close = function () {
            $modalInstance.close();
        };
    };

})();

