/// <reference path="../../../../Scripts/libs/angular/angular.js" />

(function () {

    'use strict';

    // Controller Signature
    angular.module("ERPApp.Controllers")
        .controller("InquiryConversationCtrl", [
            "$scope", "$modal", "$rootScope", "$timeout", "ConversationService", "InquiryService", "ProposalService", "$http", "$filter", "ngTableParams", "$q",
            InquiryConversationCtrl
        ]);

    // Inquiry Controller Function
    function InquiryConversationCtrl($scope, $modal, $rootScope, $timeout, CS, IS, PS, $http, $filter, ngTableParams, $q) {
        $scope.timeZone = new Date().getTimezoneOffset().toString();
        $scope.isInqTableLoaded = false;

        $scope.SetOpenedTab = function (tab) {
            $scope.master.CurrentTab = tab;
            $scope.GetClient();
            $scope.LoadInquiryData();
        };

        $scope.$watch("master.filterData.ClientId", function (newValue) {
            if ($scope.master.CurrentTab == "Inquiry") {
                $rootScope.isInquiryFormVisible = false;
                $rootScope.isConversationFormVisible = false;
                $scope.IsConversationListDisplay = false;

                if (newValue == 0) {
                    $scope.ClientInfo = [];
                } else { //filter on inquiry
                    $scope.GetClient();
                    if (!$scope.isInqTableLoaded) {
                        $scope.RetrieveInquiries();
                    } else {
                        $scope.RefreshInquiryTable();
                    }
                }
            }
        });

        /*================INQUIRY SECTION=============*/
        $scope.Inquiry = {
            mode: "Add",
            SaveText: "Save"
        };
        $scope.editDataI = {};
        $scope.editDataI.multiFileList = [];
        $scope.oldMultifilelistInquiry = [];
        $scope.oldFilteredTechnologies = [];
        $scope.lastInquiry = {};
        $rootScope.isInquiryFormVisible = false;

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
        $scope.OpenInquiryDateCalender = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.isInquiryDateOpened = true;
        };
        $scope.$watch("editDataI.iDate", function (newValue) {
            $scope.editDataI.InquiryDate = $filter('date')(newValue, 'dd-MM-yyyy');
        });

        $scope.ValidateInquiryDate = function (mDate, frmInquiry) {
            if (!mDate) {
                frmInquiry.txtInquiryDate.$setValidity("invalidInquiryDate", true);
                return;
            } else if (mDate.length == 10) {
                if ($scope.ValidateDate(mDate)) {
                    if ($scope.IsGreterThanToday(mDate)) {
                        $scope.editDataI.InquiryDate = $filter('date')(new Date(), 'dd-MM-yyyy');
                    } else {
                        frmInquiry.txtInquiryDate.$setValidity("invalidInquiryDate", true);
                        var dt = mDate.split('-');
                        $scope.editDataI.iDate = dt[2] + "-" + dt[1] + "-" + dt[0] + "T00:00:00";
                    }
                } else {
                    frmInquiry.txtInquiryDate.$setValidity("invalidInquiryDate", false);
                }
            } else {
                frmInquiry.txtInquiryDate.$setValidity("invalidInquiryDate", false);
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

        // BEGIN COMBO BOX VALIDATION
        $scope.ValidateStatus = function () {
            if ($scope.editDataI.InquiryStatus && $scope.editDataI.InquiryStatus != 0) return false;
            return true;
        };

        $scope.LoadInquiryData = function () {
            $scope.IsAjaxLoading = true;
            //Retrieve Inquiry Status
            IS.RetrieveStatus().then(function (resStatus) {
                if (resStatus.data.IsValidUser) {
                    $scope.status = resStatus.data.DataList;
                    $scope.editDataI.InquiryStatus = 0;

                    //Retrieve Technologies
                    IS.RetrieveTechnologies().then(function (resTech) {
                        $scope.technologies = resTech.data.DataList;
                        if (!$scope.isInqTableLoaded) {
                            $scope.RetrieveInquiries();
                        } else {
                            $scope.RefreshInquiryTable();
                        }
                    });
                } else {
                    $rootScope.redirectToLogin();
                }
            });
            $scope.IsAjaxLoading = false;
        };

        $scope.RetrieveInquiries = function () {
            $scope.tableParams = new ngTableParams({
                page: 1,
                count: 10,
                sorting: {
                    Title: 'asc'
                },
                defaultSort: 'asc'
            }, {
                total: 0,
                filterDelay: 750,
                getData: function ($defer, params) {
                    $rootScope.IsAjaxLoading = true;
                    $scope.isRecord = false;
                    IS.RetrieveInquiries($scope.master.filterData.ClientId, $scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter().Title, params.filter().Status, params.filter().Code).then(function (result) {
                        if (result.data.IsValidUser) {
                            //display no data message
                            if (result.data.MessageType === 1) {
                                if (result.data.DataList.total == 0) {
                                    $scope.noRecord = true;
                                } else {
                                    $scope.noRecord = false;
                                }

                                $scope.isInqTableLoaded = true;
                                params.total(result.data.DataList.total);
                                $defer.resolve($scope.documents = result.data.DataList.result);
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
            });
        };

        $scope.RefreshInquiryTable = function () {
            $scope.tableParams.reload();
        };

        $scope.GetClient = function () {
            CS.GetClient($scope.master.filterData.ClientId).then(function (result) {
                $scope.ClientInfo = result.data.DataList;
            });
        };

        function ResetInquiryForm() {
            $scope.editDataI = {
                PKInquiryId: 0,
                FKClientId: 0,
                InquiryTitle: "",
                InquiryStatus: 0,
                InquiryDate: moment().format("DD-MM-YYYY"),
                FKTechnologyIds: "",
                Remarks: "",
                IsActive: true,
                IsDeleted: false
            };

            $scope.filterTechnologies = $scope.oldFilteredTechnologies;
            $scope.Inquiry.mode = "Add";
            $scope.Inquiry.SaveText = "Save";
            $scope.editDataI.multiFileList = [];
            $scope.oldMultifilelistInquiry = [];
            $scope.lastInquiry = {};
            $rootScope.isInquiryFormVisible = true;
            $scope.isFirstInquiryFocus = false;
            $timeout(function () {
                $scope.isFirstInquiryFocus = true;
            });
        };

        $scope.AddInquiry = function () {
            ResetInquiryForm();
            $scope.editDataI.InquiryStatus = 1;
            $scope.editDataI.InquiryDate = $filter('date')($scope.currentDate, 'dd-MM-yyyy');
            $scope.editDataI.iDate = $scope.currentDate;
            $scope.filterTechnologies = [];
        };

        function DoProcessInquiryDocument(inquiryId, arrayList, callback) {
            var newList = angular.copy(arrayList);
            var index = 0, length = newList.length;
            function saveData() {
                SaveInquiryDocument(newList[index], inquiryId).then(function (result) {
                    if (index == length - 1) {
                        callback(result);
                    } else {
                        index++;
                        saveData();
                    }
                });
            }
            saveData();
        };

        function SaveInquiryDocument(line, inquiryId) {
            line.tblRefId = inquiryId;
            line.PKDocId = 0;
            line.DocName = line.file;
            line.DocTypeId = 16;
            line.Remarks = line.caption;
            return IS.UploadDocument(line);
        };

        function DoProcessAfterInquirySave(result) {
            if ($scope.Inquiry.mode == "Add") {
                toastr.success(result.data.Message, 'Success');
                ResetInquiryForm();
            }
            else if ($scope.Inquiry.mode == "Edit") {
                toastr.success(result.data.Message, 'Success');
                ResetInquiryForm();
                $rootScope.isInquiryFormVisible = false;
            }
        };

        $scope.CreateUpdateInquiry = function (inquiry, frmInquiry) {
            var newInquiry = angular.copy(inquiry);
            var tempTechnologies = $scope.filterTechnologies;
            var tempArray = [];
            for (var i = 0 ; i < tempTechnologies.length; i++)
            { tempArray.push(tempTechnologies[i].Id); }
            newInquiry.FKTechnologyIds = tempArray.toString();
            newInquiry.FKClientId = $scope.master.filterData.ClientId;

            IS.CreateUpdateInquiry(newInquiry, $scope.timeZone).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType === 1) {
                        if (inquiry.multiFileList.length > 0) {
                            var inquiryId = result.data.DataList;
                            DoProcessInquiryDocument(inquiryId, inquiry.multiFileList, function (res) {
                                DoProcessAfterInquirySave(result);
                            });
                        } else {
                            DoProcessAfterInquirySave(result);
                        }

                        frmInquiry.$setPristine(); //reset form validation
                        //inquiry.multiFileList.length = 0; //reset
                        $scope.editDataI.multiFileList = [];
                        $scope.oldMultifilelistInquiry = [];

                        $scope.RefreshInquiryTable();
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
        $scope.DeleteUploadedInquiryFile = function (index, list) {
            list.splice(index, 1); // we are not deleting file from server
        };

        //GET TECHNOLOGIES LIST 
        $scope.filterTechnologies = [];
        $scope.oldFilterTechnologies = [];
        $scope.RetrieveTechnologies = function () {
            IS.RetrieveTechnologies().then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) {
                        $scope.technologies = result.data.DataList;
                    } else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
            });
        };
        $scope.loadTags = function (query) {
            var q = $q.defer();
            var data = [];
            setTimeout(function () {
                angular.forEach($scope.technologies, function (value, key) {
                    data.push({
                        'Id': value.Id,
                        'text': value.Label
                    });
                });
                q.resolve(data);
            }, 100);
            return q.promise;
        };

        // CHANGE INQUIRY STATUS
        $scope.ChangeInquiryStatus = function (inquiry) {
            PS.IsInquiryConfirmed(inquiry.PKInquiryId).then(function (res) {
                if (res.data.DataList) {
                    toastr.warning("This inquiry is confirmed as project. You can't change status.", 'Warning');
                } else {
                    IS.ChangeInquiryStatus(inquiry).then(function (result) {
                        if (result.data.IsValidUser) {
                            if (result.data.MessageType === 1) {
                                toastr.success(result.data.Message, 'Success');
                                $scope.RefreshInquiryTable();
                            } else if (result.data.MessageType == 2) { // 1:Warning
                                toastr.warning(result.data.Message, 'Record already exists');
                            } else {
                                toastr.error(result.data.Message, 'Opps, Something went wrong');
                            }
                        }
                        else {
                            $rootScope.redirectToLogin();
                        }
                    });
                }
            });
        };
        $scope.DeleteInquiry = function (id) {
            $scope.IsAjaxLoadingPMS = true;
            IS.DeleteInquiry(id).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType === 1) {
                        toastr.success(result.data.Message, 'Success');
                        $scope.RefreshInquiryTable();
                    }
                    else if (result.data.MessageType == 2) {
                        toastr.warning(result.data.Message, 'Warning');
                    }
                    else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
            });
            $scope.IsAjaxLoadingPMS = false
        };
        $scope.CloseInquiry = function (frmInquiry) {
            $scope.lastInquiry = {};
            $scope.editDataI.multiFileList.length = [];
            $scope.oldMultifilelistInquiry = [];
            $scope.Inquiry.mode = "Add";
            ResetInquiryForm();
            $rootScope.isInquiryFormVisible = false;
            $scope.isFirstInquiryFocus = false;
            frmInquiry.$setPristine();
        };

        $scope.UpdateInquiry = function (data) {
            var selectedTechnologies = [];
            var tempTechnologies = data.FKTechnologyIds;
            var temp = tempTechnologies.split(',').map(function (tempTechnologies) { return Number(tempTechnologies); });

            angular.forEach($scope.technologies, function (value, key) {
                for (var j = 0; j < temp.length ; j++) {
                    if (value.Id == temp[j]) {
                        selectedTechnologies.push({
                            'Id': value.Id,
                            'text': value.Label
                        });
                    }
                }
            });
            $scope.oldFilteredTechnologies = angular.copy(selectedTechnologies);
            $scope.filterTechnologies = selectedTechnologies;

            IS.RetrieveDocument(data.PKInquiryId).then(function (result) {
                var tempMultiFileList = [];
                angular.forEach(result.data.DataList, function (value, key) {
                    tempMultiFileList.push({
                        'PKDocId': value.PKDocId,
                        'file': value.DocName,
                        'caption': value.Remarks,
                        'ext': value.DocName.split('.')[1]
                    });
                });
                angular.copy(tempMultiFileList, $scope.oldMultifilelistInquiry); //new

                $scope.Inquiry.mode = "Edit";
                $scope.Inquiry.SaveText = "Update";
                $scope.editDataI = {
                    PKInquiryId: data.PKInquiryId,
                    FKClientId: data.FKClientId,
                    InquiryTitle: data.InquiryTitle,
                    InquiryStatus: data.InquiryStatus,
                    InquiryDate: $filter('date')(data.InquiryDate, 'dd-MM-yyyy'),
                    FKTechnologyIds: data.FKTechnologyIds,
                    Remarks: data.Remarks,
                    IsActive: data.IsActive,
                    multiFileList: tempMultiFileList,
                    iDate: data.InquiryDate
                };

                $scope.lastInquiry = {
                    PKInquiryId: data.PKInquiryId,
                    FKClientId: data.FKClientId,
                    InquiryTitle: data.InquiryTitle,
                    InquiryStatus: data.InquiryStatus,
                    InquiryDate: $filter('date')(data.InquiryDate, 'dd-MM-yyyy'),
                    FKTechnologyIds: data.FKTechnologyIds,
                    Remarks: data.Remarks,
                    multiFileList: [],
                    IsActive: data.IsActive,
                    iDate: data.InquiryDate
                };
                angular.copy(tempMultiFileList, $scope.lastInquiry.multiFileList);//new

                $rootScope.isInquiryFormVisible = true;
                $scope.isFirstInquiryFocus = false;
                $timeout(function () {
                    $scope.isFirstInquiryFocus = true;
                });
            });
        };
        $scope.ResetInquiry = function (frmInquiry) {
            if ($scope.Inquiry.mode === "Add") {
                $scope.editDataI.multiFileList = [];
                $scope.oldMultifilelistInquiry = [];
                $scope.filterTechnologies = [];
                $scope.oldFilteredTechnologies = [];
                ResetInquiryForm();
            }
            else if ($scope.Inquiry.mode === "Edit") {
                $scope.editDataI = {
                    PKInquiryId: $scope.lastInquiry.PKInquiryId,
                    FKClientId: $scope.lastInquiry.FKClientId,
                    InquiryTitle: $scope.lastInquiry.InquiryTitle,
                    InquiryStatus: $scope.lastInquiry.InquiryStatus,
                    InquiryDate: $scope.lastInquiry.InquiryDate,
                    FKTechnologyIds: $scope.lastInquiry.FKTechnologyIds,
                    Remarks: $scope.lastInquiry.Remarks,
                    IsActive: $scope.lastInquiry.IsActive,
                    multiFileList: [],
                    iDate: $scope.lastInquiry.iDate
                };

                angular.copy($scope.lastInquiry.multiFileList, $scope.editDataI.multiFileList);
                angular.copy($scope.lastInquiry.multiFileList, $scope.oldMultifilelistInquiry);

                $scope.filterTechnologies = angular.copy($scope.oldFilteredTechnologies);
                $scope.isFirstInquiryFocus = false;
                $timeout(function () {
                    $scope.isFirstInquiryFocus = true;
                });
            }

            frmInquiry.$setPristine();
        };
        $scope.InquiryStatusType = function (column) {
            var def = $q.defer();
            var arr = [];
            IS.RetrieveStatus().then(function (result) {
                angular.forEach(result.data.DataList, function (value, key) {
                    arr.push({ id: value.Id, title: value.Label });
                });
            });

            def.resolve(arr);
            return def;
        };

        // OPEN PROPOSAL POPUP
        $scope.ViewProposals = function (inquiry) {
            var modalInstance = $modal.open({
                templateUrl: 'Proposal.html',
                backdrop: 'static',
                controller: ProposalCtrl,
                scope: $scope,
                resolve: {
                    inquiry: function () { return inquiry; },
                    PS: function () { return PS },
                    RefreshTable: function () { return $scope.RefreshInquiryTable }
                },
            });
        };

        //OPEN FILE UPLOAD POPUP
        $scope.UploadFileDirectly = function (inquiry) {
            var modalInstance = $modal.open({
                templateUrl: 'FileUpload.html',
                backdrop: 'static',
                controller: FileUploadCtrl,
                scope: $scope,
                resolve: {
                    inquiry: function () { return inquiry; },
                    IS: function () { return IS },
                    CS: function () { return CS }
                },
            });
        };

        /*===========BEGIN CONVERSATION SECTION================ */
        $scope.editDataC = {};
        $scope.editDataC.multiFileList = [];
        $scope.editDataC.ConversationType = 2; // Type = Inquiry Conversation
        $scope.master.filterData.InquiryId = 0;
        $scope.lastConversation = {};
        $scope.IsConversationListDisplay = false; //for conversation
        $rootScope.isConversationFormVisible = false;

        $scope.contentTypes = [{ "Value": "FC", "Label": "From client" }, { "Value": "TC", "Label": "To client" }];
        $scope.oldMultifilelistConversation = [];

        //Date Picker
        $scope.OpenConversationDateCalender = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.isConversationDateOpened = true;
        };
        $scope.$watch("editDataC.cDate", function (newValue) {
            $scope.editDataC.ConversationDate = $filter('date')(newValue, 'dd-MM-yyyy');
        });
        $scope.ValidateConversationDate = function (mDate, frmConversation) {
            if (!mDate) {
                frmConversation.txtConversationDate.$setValidity("invalidConversationDate", true);
                return;
            } else if (mDate.length == 10) {
                if ($scope.ValidateDate(mDate)) {
                    if ($scope.IsGreterThanToday(mDate)) {
                        $scope.editDataC.ConversationDate = $filter('date')(new Date(), 'dd-MM-yyyy');
                    } else {
                        frmConversation.txtConversationDate.$setValidity("invalidConversationDate", true);
                        var dt = mDate.split('-');
                        $scope.editDataC.cDate = dt[2] + "-" + dt[1] + "-" + dt[0] + "T00:00:00";

                    }
                } else {
                    frmConversation.txtConversationDate.$setValidity("invalidConversationDate", false);
                }
            } else {
                frmConversation.txtConversationDate.$setValidity("invalidConversationDate", false);
            }
        };

        function ResetConversationForm() {
            var min = moment().minute();
            $scope.editDataC = {
                PKConversationId: 0,
                FKClientId: $scope.master.filterData.ClientId,
                ConversationTitle: "",
                ConversationDescription: "",
                ContentType: "FC",
                ConversationType: 2,
                ConversationDate: moment().format("DD-MM-YYYY"),
                ConversationHours: moment().hour(),
                ConversationMinutes: min < 10 ? "0" + min : min
            };
            $scope.master.mode = "Add";
            $scope.master.SaveText = "Save";
            $scope.editDataC.multiFileList = [];
            $scope.oldMultifilelistConversation = [];
            $scope.lastConversation = {};
            $rootScope.isConversationFormVisible = true;
            $scope.isFirstConversationFocus = false;
            $timeout(function () {
                $scope.isFirstConversationFocus = true;
            });
        };

        $scope.AddInquiryConversation = function () {
            $rootScope.isConversationFormVisible = true;
            ResetConversationForm();
        };

        $scope.ViewInquiryConversation = function (inquiryId) {
            $rootScope.isInquiryFormVisible = false;
            $rootScope.isConversationFormVisible = false;

            $scope.master.filterData.InquiryId = inquiryId; //for retrieving conversation

            $scope.RetrieveConversations();
            $scope.IsConversationListDisplay = true;
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
        };

        function SaveConversationDocument(line, clientId) {
            line.tblRefId = clientId;
            line.PKDocId = 0;
            line.DocName = line.file;
            line.DocTypeId = 15;
            line.Remarks = line.caption;
            return CS.UploadDocument(line);
        };

        function DoProcessAfterConversationSave(result) {
            if ($scope.master.mode == "Add") {
                toastr.success(result.data.Message, 'Success');
                ResetConversationForm();
            }
            else if ($scope.master.mode == "Edit") {
                toastr.success(result.data.Message, 'Success');
                ResetConversationForm();
                $rootScope.isConversationFormVisible = false;
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
                ReferenceId: $scope.master.filterData.InquiryId
            };

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
                        //data.multiFileList.length = 0; //reset
                        $scope.editDataC.multiFileList = [];
                        $scope.oldMultifilelistConversation = [];

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
        $scope.DeleteUploadedConversationFile = function (index, list) {
            list.splice(index, 1); // we are not deleting file from server
        };

        //GET CONVERSATION LIST
        $scope.RetrieveConversations = function () {
            $scope.IsAjaxLoadingPMS = true;
            CS.RetrieveConversations($scope.master.filterData.ClientId, $scope.master.filterData.InquiryId, $scope.timeZone, $scope.editDataC.ConversationType).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) {
                        $scope.conversationList = result.data.DataList;
                        angular.forEach($scope.conversationList, function (value, key) {
                            value.TempConversationTitle = value.ConversationTitle.length < 46 ? value.ConversationTitle : $filter('limitTo')(value.ConversationTitle, 45);
                            value.TempConversationDescription = value.ConversationDescription.length < 400 ? value.ConversationDescription : $filter('limitTo')(value.ConversationDescription, 400);
                        });
                    } else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
            });
            $scope.IsAjaxLoadingPMS = false;
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
            $scope.editDataC.multiFileList = [];
            $scope.oldMultifilelistConversation = [];
            $scope.master.mode = "Add";
            ResetConversationForm();
            $rootScope.isConversationFormVisible = false;
            $scope.isFirstConversationFocus = false;
            frmConversation.$setPristine();
        }

        $scope.UpdateConversation = function (data) {
            $rootScope.isConversationFormVisible = true;

            var tempMultiFileList = [];
            angular.forEach(data.DocumentList, function (value, key) {
                tempMultiFileList.push({
                    'file': value.DocName,
                    'caption': value.DocRemark,
                    'ext': value.DocName.split('.')[1]
                });
            });
            angular.copy(tempMultiFileList, $scope.oldMultifilelistConversation);

            $scope.master.mode = "Edit";
            $scope.master.SaveText = "Update";

            var min = moment(data.ConversationDate).minutes();

            $scope.editDataC = {
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

            $rootScope.isConversationFormVisible = true;
            $scope.isFirstConversationFocus = false;
            $timeout(function () {
                $scope.isFirstConversationFocus = true;
            });
        }

        $scope.ResetConversation = function (frmConversation) {
            if ($scope.master.mode === "Add") {
                $scope.editDataC.multiFileList = [];
                $scope.oldMultifilelistConversation = [];
                ResetConversationForm();
            }
            else if ($scope.master.mode === "Edit") {
                $scope.editDataC = {
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

                angular.copy($scope.lastConversation.multiFileList, $scope.editDataC.multiFileList);
                angular.copy($scope.lastConversation.multiFileList, $scope.oldMultifilelistConversation);

                $scope.isFirstConversationFocus = false;
                $timeout(function () {
                    $scope.isFirstConversationFocus = true;
                });
            }
            frmConversation.$setPristine();
        };
        /*================END CONVERSATION SECTION=============*/
    }


    var FileUploadCtrl = function ($scope, $modalInstance, inquiry, $rootScope, $timeout, IS, CS, $filter) {
        $scope.uploadData = {
            Inquiry: inquiry
        };
        $scope.uploadData.multiFileList = [];
        $scope.master.filterData.InquiryId = 0;
        $scope.oldMultifilelistUploadDirectly = [];

        function DoProcessUploadDirectly(inquiryId, arrayList, callback) {
            var newList = angular.copy(arrayList);
            var index = 0, length = newList.length;
            function saveData() {
                SaveUploadDirectlyDocument(newList[index], inquiryId).then(function (result) {
                    if (index == length - 1) {
                        callback(result);
                    } else {
                        index++;
                        saveData();
                    }
                });
            }
            saveData();
        };

        function SaveUploadDirectlyDocument(line, inquiryId) {
            line.tblRefId = inquiryId;
            line.PKDocId = 0;
            line.DocName = line.file;
            line.DocTypeId = 16; // for inquiry type
            line.Remarks = line.caption;
            return CS.UploadDocument(line);
        };

        //Save uploaded file
        $scope.SaveDirectlyUploadedFile = function () {
            if ($scope.uploadData.multiFileList.length > 0) {
                var inquiryId = $scope.uploadData.Inquiry.PKInquiryId;
                DoProcessUploadDirectly(inquiryId, $scope.uploadData.multiFileList, function (result) {
                    toastr.success("Document has been uploaded", "Success");
                    $scope.Close();
                });
            }
        };

        //delete uploaded file
        $scope.DeleteUploadedFileDirectly = function (index, list) {
            list.splice(index, 1); // we are not deleting file from server
        };

        $scope.Close = function () {
            $modalInstance.close();
        };
    };

    /*
    * BEGIN PROPOSAL CONTROLLER
    */
    var ProposalCtrl = function ($scope, $modalInstance, inquiry, $rootScope, $timeout, PS, RefreshTable, $filter) {
        $scope.Proposal = {
            mode: "Add",
            saveText: "Save",
            IsActive: true
        };

        $scope.inquiry = inquiry;
        $scope.proposals = [];
        $scope.formData = {};
        $scope.formData.multiFileList = [];
        $scope.oldFormData = {};
        $scope.oldMultifilelistProposal = [];
        $rootScope.isProposalFormVisible = false;
        $scope.IsAjaxLoading = false;

        // BEGIN DATE PICKER
        $scope.OpenProposalDateCalender = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.isProposalDateOpened = true;
        };
        $scope.$watch("formData.pDate", function (newValue) {
            $scope.formData.ProposalDate = $filter('date')(newValue, 'dd-MM-yyyy');
        });
        $scope.ValidateProposalDate = function (pDate, frmProposal) {
            if (!pDate) {
                frmProposal.txtProposalDate.$setValidity("invalidProposalDate", true);
                return;
            } else if (pDate.length == 10) {
                if ($scope.ValidateDate(pDate)) {
                    if ($scope.IsGreterThanToday(pDate)) {
                        $scope.formData.ProposalDate = $filter('date')(new Date(), 'dd-MM-yyyy');
                    } else {
                        frmProposal.txtProposalDate.$setValidity("invalidProposalDate", true);
                        var dt = pDate.split('-');
                        $scope.formData.pDate = dt[2] + "-" + dt[1] + "-" + dt[0] + "T00:00:00";
                    }
                } else {
                    frmProposal.txtProposalDate.$setValidity("invalidProposalDate", false);
                }
            } else {
                frmProposal.txtProposalDate.$setValidity("invalidProposalDate", false);
            }
        };
        // END DATE PICKER

        function DoProcessProposalDocument(proposalId, arrayList, callback) {
            var newList = angular.copy(arrayList);
            var index = 0, length = newList.length;
            function SaveData() {
                SaveProposalDocument(newList[index], proposalId).then(function (result) {
                    if (index == length - 1) {
                        callback(result);
                    } else {
                        index++;
                        SaveData();
                    }
                });
            }
            SaveData();
        }
        function SaveProposalDocument(line, proposalId) {
            line.tblRefId = proposalId;
            line.PKDocId = 0;
            line.DocName = line.file;
            line.DocTypeId = 17;
            line.Remarks = line.caption;
            return PS.UploadDocument(line);
        }
        function DoProcessAfterProposalSave(result) {
            if ($scope.Proposal.mode == "Add") {
                toastr.success(result.data.Message, 'Success');
                ResetProposalForm();
            }
            else if ($scope.Proposal.mode == "Edit") {
                toastr.success(result.data.Message, 'Success');
                ResetProposalForm();
                $rootScope.isProposalFormVisible = false;
            }
        };

        // BEGIN CRUD FUNCTIONS
        $scope.CreateUpdateProposal = function (data, frmProposal) {

            var dateArray = data.ProposalDate.split('-');
            var date = new Date(parseInt(dateArray[2]), parseInt(dateArray[1]) - 1, parseInt(dateArray[0]), moment().hours(), moment().minute(), moment().second());
            var proposalDate = $filter('date')(date, 'MM-dd-yyyy HH:mm:ss');

            var _data = {
                PKProposalId: data.PKProposalId,
                FKInquiryId: inquiry.PKInquiryId,
                ProposalTitle: data.ProposalTitle,
                ProposalDate: proposalDate,
                IsFinalized: data.IsFinalized,
                Remarks: data.Remarks,
                IsActive: true,
                IsDeleted: false,
            }

            PS.CreateUpdateProposal(_data, $scope.timeZone).then(function (result) {
                $scope.IsAjaxLoading = true;
                if (result.data.IsValidUser) {
                    if (result.data.MessageType === 1) {
                        if (data.multiFileList.length > 0) {
                            var proposalId = result.data.DataList;
                            DoProcessProposalDocument(proposalId, data.multiFileList, function (res) {
                                DoProcessAfterProposalSave(result);
                            });
                        } else {
                            DoProcessAfterProposalSave(result);
                        }

                        $scope.formData.multiFileList = [];
                        $scope.oldMultifilelistProposal = [];
                        $scope.RetrieveProposals();
                        RefreshTable();
                        $scope.IsAjaxLoading = false;
                        frmProposal.$setPristine(); //reset form validation
                    }
                    else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                }
                else {
                    $rootScope.redirectToLogin();
                }
            });
        };
        $scope.RetrieveProposals = function () {
            $scope.IsAjaxLoading = true;

            PS.IsInquiryConfirmed(inquiry.PKInquiryId).then(function (res) {
                //Lock proposal actions if project is confirmed
                $scope.isInquiryLocked = res.data.DataList;

                PS.RetrieveProposals(inquiry.PKInquiryId, $scope.timeZone).then(function (result) {
                    if (result.data.IsValidUser) {
                        if (result.data.MessageType === 1) {
                            $scope.proposals = result.data.DataList;
                            $scope.IsAjaxLoading = false;
                        } else {
                            toastr.error(result.data.Message, 'Opps, Something went wrong');
                        }
                    }
                    else {
                        $rootScope.redirectToLogin();
                    }
                });
            });
        };
        $scope.RetrieveProposals();
        $scope.DeleteProposal = function (id, IsFinalized) {
            $scope.IsAjaxLoadingPMS = true;

            if (IsFinalized == false) {
                PS.DeleteProposal(id).then(function (result) {
                    if (result.data.IsValidUser) {
                        if (result.data.MessageType === 1) {
                            toastr.success(result.data.Message, 'Success');
                            $scope.RetrieveProposals();
                        } else {
                            toastr.error(result.data.Message, 'Opps, Something went wrong');
                        }
                    } else {
                        $rootScope.redirectToLogin();
                    }
                });
            } else {
                toastr.warning('Sorry, you can\'t delete this proposal.');
            }

            $scope.IsAjaxLoadingPMS = false
        };
        // END CRUD FUNCTIONS

        // BEGIN OTHER FUNCTIONS
        $scope.AddProposal = function () {
            ResetProposalForm();
        };

        $scope.ResetProposal = function (frmProposal) {
            if ($scope.Proposal.mode === "Add") {
                ResetProposalForm();
            }
            else if ($scope.Proposal.mode === "Edit") {
                $scope.formData = angular.copy($scope.oldFormData);
                angular.copy($scope.oldFormData.multiFileList, $scope.formData.multiFileList);
                angular.copy($scope.oldFormData.multiFileList, $scope.oldMultifilelistProposal);

                $scope.isProposalFirstFocus = false;
                $timeout(function () {
                    $scope.isProposalFirstFocus = true;
                });
            }
            frmProposal.$setPristine();
        };

        $scope.UpdateProposal = function (_data) {
            var data = angular.copy(_data);
            PS.RetrieveDocument(data.PKProposalId).then(function (result) {
                var tempMultiFileList = [];
                angular.forEach(result.data.DataList, function (value, key) {
                    tempMultiFileList.push({
                        'PKDocId': value.PKDocId,
                        'file': value.DocName,
                        'caption': value.Remarks,
                        'ext': value.DocName.split('.')[1]
                    });
                });
                angular.copy(tempMultiFileList, $scope.oldMultifilelistProposal);

                $scope.formData = {
                    PKProposalId: data.PKProposalId,
                    FKInquiryId: data.FKInquiryId,
                    ProposalDate: $filter('date')(data.ProposalDate, 'dd-MM-yyyy'),
                    ProposalTitle: data.ProposalTitle,
                    Remarks: data.Remarks,
                    IsFinalized: data.IsFinalized,
                    IsDeleted: data.IsDeleted,
                    multiFileList: []
                };
                angular.copy(tempMultiFileList, $scope.formData.multiFileList);

                $scope.oldFormData = {
                    PKProposalId: data.PKProposalId,
                    FKInquiryId: data.FKInquiryId,
                    ProposalDate: $filter('date')(data.ProposalDate, 'dd-MM-yyyy'),
                    ProposalTitle: data.ProposalTitle,
                    Remarks: data.Remarks,
                    IsFinalized: data.IsFinalized,
                    IsDeleted: data.IsDeleted,
                    multiFileList: []
                };
                angular.copy(tempMultiFileList, $scope.oldFormData.multiFileList);

                $scope.Proposal.mode = "Edit";
                $scope.Proposal.saveText = "Update";
                $rootScope.isProposalFormVisible = true;
                $scope.isProposalFirstFocus = false;
                $timeout(function () {
                    $scope.isProposalFirstFocus = true;
                });
            });
        };
        $scope.CloseProposal = function (frmProposal) {
            $scope.formData = {};
            $scope.oldFormData = {};
            $scope.Proposal.mode = "Add";
            $rootScope.isProposalFormVisible = false;
            $scope.isProposalFirstFocus = false;
            frmProposal.$setPristine();
        };
        function ResetProposalForm() {
            $scope.formData = {
                PKProposalId: 0,
                FKInquiryId: 0,
                ProposalTitle: "",
                ProposalDate: moment().format("DD-MM-YYYY"),
                IsFinalized: true,
                Remarks: "",
                IsActive: true,
                IsDeleted: false,
                multiFileList: []
            };

            $scope.oldFormData = {};
            $scope.Proposal.mode = "Add";
            $scope.Proposal.saveText = "Save";
            $scope.oldMultifilelistProposal = [];
            $rootScope.isProposalFormVisible = true;
            $scope.isProposalFirstFocus = false;
            $timeout(function () {
                $scope.isProposalFirstFocus = true;
            });
        };

        $scope.DeleteUploadedProposalFile = function (index, list) {
            list.splice(index, 1); // we are not deleting file from server
        };
        $scope.ChangeProposalStatus = function (id) {
            PS.ChangeProposalStatus(id).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType === 1) {
                        toastr.success(result.data.Message, 'Success');
                        $scope.RetrieveProposals();
                    } else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
            });
        };
        $scope.Close = function () {
            $modalInstance.close();
        };
        //END OTHER FUNCTIONS
    };
    /* END PROPOSAL CONTROLLER */
})();