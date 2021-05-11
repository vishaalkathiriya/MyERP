/// <reference path="../../../../Scripts/libs/angular/angular.js" />

(function () {

    'use strict';

    // Controller Signature
    angular.module("ERPApp.Controllers")
        .controller("ConversationCtrl", [
            "$scope", "$rootScope", "$timeout", "ConversationService", "$http", "$filter", "$q", "$document", "$window",
            ConversationCtrl
        ]);

    // Conversation Controller Function
    function ConversationCtrl($scope, $rootScope, $timeout, CS, $http, $filter, $q, $document, $window) {

        $scope.mode = "Add";
        $scope.SaveText = "Save";
        $scope.editData = {};
        $scope.editData.multiFileList = [];
        $scope.editData.ConversationType = 1; // Type = Open 
        $scope.lastConversation = {};
        $rootScope.isFormVisible = false;
        $scope.timeZone = new Date().getTimezoneOffset().toString();
        $scope.contentTypes = [{ "Value": "FC", "Label": "From client" }, { "Value": "TC", "Label": "To client" }];
        $scope.oldMultifilelist = [];
        $scope.IsFilterResultDisplay = false;

        // BEGIN DATE PICKER
        $scope.dateOptions = { 'year-format': "'yy'", 'starting-day': 1 };
        $scope.formats = ['dd-MM-yyyy', 'yyyy/MM/dd', 'shortDate'];
        $scope.format = $scope.formats[0];
        $scope.today = function () {
            $scope.currentDate = new Date();
        };
        $scope.today();
        $scope.showWeeks = true;
        $scope.toggleWeeks = function () {
            $scope.showWeeks = !$scope.showWeeks;
        };
        $scope.clear = function () {
            $scope.currentDate = null;
        };
        $scope.maxDate = $scope.maxDate || new Date();

        $scope.OpenConversationDateCalender = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.isConversationDateOpened = true;
        };

        $scope.$watch("editData.cDate", function (newValue) {
            $scope.editData.ConversationDate = $filter('date')(newValue, 'dd-MM-yyyy');
        });

        $scope.ValidateConversationDate = function (mDate, frmConversation) {
            if (!mDate) {
                frmConversation.txtConversationDate.$setValidity("invalidConversationDate", true);
                return;
            } else if (mDate.length == 10) {
                if ($scope.ValidateDate(mDate)) {
                    if ($scope.IsGreterThanToday(mDate)) {
                        $scope.editData.ConversationDate = $filter('date')(new Date(), 'dd-MM-yyyy');
                    } else {
                        frmConversation.txtConversationDate.$setValidity("invalidConversationDate", true);
                        var dt = mDate.split('-');
                        $scope.editData.cDate = dt[2] + "-" + dt[1] + "-" + dt[0] + "T00:00:00";

                    }
                } else {
                    frmConversation.txtConversationDate.$setValidity("invalidConversationDate", false);
                }
            } else {
                frmConversation.txtConversationDate.$setValidity("invalidConversationDate", false);
            }
        };

        $scope.IsGreterThanToday = function (date) { //date should be in dd-MM-yyyy format
            var tDT = date.split('-');
            var todayDT = $filter('date')(new Date(), 'dd-MM-yyyy').split('-');

            var tDate = new Date(parseInt(tDT[2]), parseInt(tDT[1]) - 1, parseInt(tDT[0]));
            var todayDate = new Date(parseInt(todayDT[2]), parseInt(todayDT[1]) - 1, parseInt(todayDT[0]));

            if (tDate > todayDate) {
                return true;
            }
            return false;
        };
        $scope.ValidateDate = function (date) {
            if (date) {
                var isError = false;
                var dates = date.split('-');
                if (dates[0].search("_") > 0 || dates[1].search("_") > 0 || dates[2].search("_") > 0) {
                    isError = true;
                }
                else {
                    if (!parseInt(dates[0]) || parseInt(dates[0]) > 31) { isError = true; }
                    if (!parseInt(dates[1]) || parseInt(dates[1]) > 12) { isError = true; }
                    if (!parseInt(dates[2]) || dates[2].length != 4) { isError = true; }
                }

                if (!isError) { return true; } // date is validated
                return false; // error in validation
            }
            return true;
        };
        // END DATE PICKER

        function ResetForm() {
            var min = moment().minute();
            $scope.editData = {
                PKConversationId: 0,
                FKClientId: $scope.master.filterData.ClientId,
                ConversationTitle: "",
                ConversationDescription: "",
                ContentType: "FC",
                ConversationType: 1,
                ConversationDate: moment().format("DD-MM-YYYY"),
                ConversationHours: moment().hour(),
                ConversationMinutes: min < 10 ? "0" + min : min
            };
            $scope.mode = "Add";
            $scope.SaveText = "Save";
            $scope.editData.multiFileList = [];
            $scope.oldMultifilelist = [];
            $scope.lastConversation = {};
            $rootScope.isFormVisible = true;
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        };

        $scope.$watch("master.filterData.ClientId", function (newValue) {
            if ($scope.master.CurrentTab == "Open") {
                if (newValue == 0) {
                    $scope.IsFilterResultDisplay = false;
                    $scope.ClientInfo = [];
                } else { //filter and display conversation
                    $scope.IsFilterResultDisplay = true;
                    $rootScope.isFormVisible = false;

                    $scope.GetClient();
                    $scope.RetrieveConversations();
                }
            }
        });

        $scope.SetOpenedTab = function (tab) {
            $scope.master.CurrentTab = tab;
        };

        $scope.AddConversation = function () {
            ResetForm();
        };

        function DoProcessConversationDocument(clientId, arrayList, callback) {
            var newList = angular.copy(arrayList);
            var index = 0, length = newList.length;
            function saveData() {
                SaveConversationDocument(newList[index], clientId).then(function (result) {
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

        function SaveConversationDocument(line, clientId) {
            line.tblRefId = clientId;
            line.PKDocId = 0;
            line.DocName = line.file;
            line.DocTypeId = 15;
            line.Remarks = line.caption;
            return CS.UploadDocument(line);
        }

        function DoProcessAfterConversationSave(result) {
            if ($scope.mode == "Add") {
                toastr.success(result.data.Message, 'Success');
                ResetForm();
            }
            else if ($scope.mode == "Edit") {
                toastr.success(result.data.Message, 'Success');
                ResetForm();
                $rootScope.isFormVisible = false;
            }
        };

        $scope.CreateUpdateConversation = function (data, frmConversation) {
            var dateArray = data.ConversationDate.split('-');
            var idt = new Date(parseInt(dateArray[2]), parseInt(dateArray[1]) - 1, parseInt(dateArray[0]), data.ConversationHours, data.ConversationMinutes, 0);
            var conversationDate = $filter('date')(idt, 'MM-dd-yyyy HH:mm:ss');

            var _data = {
                PKConversationId: data.PKConversationId,
                FKClientId: data.FKClientId,
                ConversationTitle: data.ConversationTitle,
                ConversationDescription: data.ConversationDescription,
                ConversationDate: conversationDate,
                ConversationType: data.ConversationType,
                ContentType: data.ContentType,
                ReferenceId: 0 // 0 for Open Conversation
            }
            CS.CreateUpdateConversation(_data, $scope.timeZone).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType === 1) {
                        if (data.multiFileList.length > 0) {
                            var clientId = result.data.DataList;
                            DoProcessConversationDocument(clientId, data.multiFileList, function (res) {
                                DoProcessAfterConversationSave(result);
                            });
                        } else {
                            DoProcessAfterConversationSave(result);
                        }

                        frmConversation.$setPristine(); //reset form validation
                        $scope.editData.multiFileList = [];
                        $scope.oldMultifilelist = [];
                        $scope.RetrieveConversations();

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

        //DELETE PARTICULAR IMAGE FROM MULTIFILELIST
        $scope.DeleteUploadedFile = function (index, list) {
            list.splice(index, 1); // we are not deleting file from server
        };

        //GET CONVERSATION LIST
        $scope.RetrieveConversations = function () {
            $scope.IsAjaxLoadingPMS = true;
            var referenceId = 0; //passing 0 as it is Open Conversation
            CS.RetrieveConversations($scope.master.filterData.ClientId, referenceId, $scope.timeZone, $scope.editData.ConversationType).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) {
                        $scope.conversationList = result.data.DataList;
                        angular.forEach($scope.conversationList, function (value, key) {
                            value.TempConversationTitle = value.ConversationTitle.length < 46 ? value.ConversationTitle : $filter('limitTo')(value.ConversationTitle, 45);
                            value.TempConversationDescription = value.ConversationDescription.length < 400 ? value.ConversationDescription : $filter('limitTo')(value.ConversationDescription, 400);
                        });

                        $scope.IsFilterResultDisplay = true;
                    } else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
            });
            $scope.IsAjaxLoadingPMS = false;
        };

        $scope.GetClient = function () {
            CS.GetClient($scope.master.filterData.ClientId).then(function (result) {
                $scope.ClientInfo = result.data.DataList;
            });
        };

        //DELETE CONVERSATION
        $scope.DeleteConversation = function (id) {
            $scope.IsAjaxLoadingPMS = true;
            CS.DeleteConversation(id).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType != 0) {
                        toastr.success(result.data.Message, 'Success');
                        $scope.RetrieveConversations();
                    } else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
            });
            $scope.IsAjaxLoadingPMS = false
        }

        $scope.CloseConversation = function (frmConversation) {
            $scope.lastConversation = {};
            $scope.editData.multiFileList.length = 0;
            $scope.oldMultifilelist.length = 0;
            $scope.mode = "Add";
            ResetForm();
            $rootScope.isFormVisible = false;
            $scope.isFirstFocus = false;
            frmConversation.$setPristine();
        }

        $scope.UpdateConversation = function (data) {
            var tempMultiFileList = [];
            angular.forEach(data.DocumentList, function (value, key) {
                tempMultiFileList.push({
                    'file': value.DocName,
                    'caption': value.DocRemark,
                    'ext': value.DocName.split('.')[1]
                });
            });
            angular.copy(tempMultiFileList, $scope.oldMultifilelist);

            $scope.mode = "Edit";
            $scope.SaveText = "Update";
            
            var min = moment(data.ConversationDate).minutes();

            $scope.editData = {
                PKConversationId: data.PKConversationId,
                FKClientId: data.FKClientId,
                ConversationTitle: data.ConversationTitle,
                ConversationDescription: data.ConversationDescription,
                ConversationType: data.ConversationType,
                ContentType: data.ContentType,
                ConversationDate: $filter('date')(data.ConversationDate, 'dd-MM-yyyy'),
                ConversationHours: moment(data.ConversationDate).hours(),
                ConversationMinutes: min < 10 ? "0" + min : min,
                multiFileList: tempMultiFileList,
                cDate: data.ConversationDate
            };
            $scope.lastConversation = {
                PKConversationId: data.PKConversationId,
                FKClientId: data.FKClientId,
                ConversationTitle: data.ConversationTitle,
                ConversationDescription: data.ConversationDescription,
                ConversationType: data.ConversationType,
                ContentType: data.ContentType,
                ConversationDate: $filter('date')(data.ConversationDate, 'dd-MM-yyyy'),
                ConversationHours: moment(data.ConversationDate).hours(),
                ConversationMinutes: min < 10 ? "0" + min : min,
                multiFileList: [],
                cDate: data.ConversationDate
            };
            angular.copy(tempMultiFileList, $scope.lastConversation.multiFileList);

            $rootScope.isFormVisible = true;
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        }

        $scope.ResetConversation = function (frmConversation) {
            if ($scope.mode === "Add") {
                $scope.editData.multiFileList.length = 0;
                $scope.oldMultifilelist.length = 0;
                ResetForm();
            }
            else if ($scope.mode === "Edit") {
                $scope.editData = {
                    PKConversationId: $scope.lastConversation.PKConversationId,
                    FKClientId: $scope.lastConversation.FKClientId,
                    ConversationTitle: $scope.lastConversation.ConversationTitle,
                    ConversationDescription: $scope.lastConversation.ConversationDescription,
                    ConversationType: $scope.lastConversation.ConversationType,
                    ContentType: $scope.lastConversation.ContentType,
                    ConversationDate: $scope.lastConversation.ConversationDate,
                    ConversationHours: $scope.lastConversation.ConversationHours,
                    ConversationMinutes: $scope.lastConversation.ConversationMinutes,
                    multiFileList: [],
                    cDate: $scope.lastConversation.cDate
                };

                angular.copy($scope.lastConversation.multiFileList, $scope.editData.multiFileList);
                angular.copy($scope.lastConversation.multiFileList, $scope.oldMultifilelist);

                $scope.isFirstFocus = false;
                $timeout(function () {
                    $scope.isFirstFocus = true;
                });
            }
            frmConversation.$setPristine();
        };
    }
})();