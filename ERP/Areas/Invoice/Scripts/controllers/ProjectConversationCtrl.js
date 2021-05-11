/// <reference path="../../../../Scripts/libs/angular/angular.js" />

(function () {

    'use strict';

    // CONTROLLER SIGNATURE
    angular.module("ERPApp.Controllers")
        .controller("ProjectConversationCtrl", [
            "$scope", "$modal", "$rootScope", "$timeout", "ConversationService", "InquiryService", "ProposalService", "ProjectService", "$http", "$filter", "ngTableParams", "$q", ProjectConversationCtrl
        ]);

    /*
     * END PROJECT CONVERSATION CONTROLLER
     */
    function ProjectConversationCtrl($scope, $modal, $rootScope, $timeout, ConversationService, InquiryService, ProposalService, ProjectService, $http, $filter, ngTableParams, $q) {

        $scope.timeZone = new Date().getTimezoneOffset().toString();
        $scope.projectData = {};
        $scope.isProjectTableLoaded = false;

        $scope.SetOpenedTab = function (tab) {
            $scope.master.CurrentTab = tab;
            $scope.GetClient();
            $scope.LoadProjectInquiryData();
        };

        $scope.GetClient = function () {
            ConversationService.GetClient($scope.master.filterData.ClientId).then(function (result) {
                $scope.ClientInfo = result.data.DataList;
            });
        };

        $scope.$watch("master.filterData.ClientId", function (newValue) {
            if ($scope.master.CurrentTab == "Project") {
                if (newValue == 0) {
                    $scope.ClientInfo = [];
                } else { //filter on inquiry
                    $scope.GetClient();
                    if (!$scope.isProjectTableLoaded) {
                        $scope.RetrieveFinalizedInquiries();
                    } else {
                        $scope.RefreshProjectInquiryData();
                    }
                }
            }
        });

        //BEGIN DATE PICKER
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
        //END DATE PICKER

        /*first function call on load*/
        $scope.LoadProjectInquiryData = function () {
            $scope.IsAjaxLoading = true;
            //Retrieve Inquiry Status
            InquiryService.RetrieveStatus().then(function (resStatus) {
                if (resStatus.data.IsValidUser) {
                    $scope.status = resStatus.data.DataList;
                    $scope.projectData.InquiryStatus = 0;
                    if (!$scope.isProjectTableLoaded) {
                        $scope.RetrieveFinalizedInquiries();
                    } else {
                        $scope.RefreshProjectInquiryData();
                    }
                    $scope.IsAjaxLoading = false;
                } else {
                    $rootScope.redirectToLogin();
                }
            });
        };

        $scope.RetrieveFinalizedInquiries = function () {
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
                    InquiryService.RetrieveFinalizedInquiries($scope.master.filterData.ClientId, $scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter().Title, params.filter().Status, params.filter().Code).then(function (result) {
                        if (result.data.IsValidUser) {
                            //display no data message
                            if (result.data.MessageType === 1) {
                                if (result.data.DataList.total == 0) {
                                    $scope.noRecord = true;
                                } else {
                                    $scope.noRecord = false;
                                }

                                $scope.isProjectTableLoaded = true;
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

        $scope.RefreshProjectInquiryData = function () {
            $scope.tableParams.reload();
        };

        $scope.InquiryStatusType = function (column) {
            var def = $q.defer();
            var arr = [];
            InquiryService.RetrieveStatus().then(function (result) {
                angular.forEach(result.data.DataList, function (value, key) {
                    arr.push({ id: value.Id, title: value.Label });
                });
            });

            def.resolve(arr);
            return def;
        };

        $scope.OpenProjectProject = function (inquiry) {
            var modalInstance = $modal.open({
                templateUrl: 'Project.html',
                scope: $scope,
                controller: ProjectCrtl,
                resolve: {
                    inquiry: function () { return inquiry; },
                    ConversationService: function () { return ConversationService },
                    InquiryService: function () { return InquiryService },
                    ProposalService: function () { return ProposalService },
                    ProjectService: function () { return ProjectService },
                    RefreshTable: function () { return $scope.RefreshProjectInquiryData }
                },
                size: 'lg'
            });
        };

        /*===========BEGIN CONVERSATION SECTION================ */
        $scope.editDataC = {};
        $scope.editDataC.multiFileList = [];
        $scope.editDataC.ConversationType = 3; // Type = Project Conversation
        $scope.master.filterData.ProjectId = 0;
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
                ConversationType: 3,
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

        $scope.AddProjectConversation = function () {
            $rootScope.isConversationFormVisible = true;
            ResetConversationForm();
        };

        $scope.ViewProjectConversation = function (projectId) {
            $rootScope.isConversationFormVisible = false;
            $scope.master.filterData.ProjectId = projectId; //for retrieving conversation

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
            return ConversationService.UploadDocument(line);
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
                ReferenceId: $scope.master.filterData.ProjectId
            };

            ConversationService.CreateUpdateConversation(_data, $scope.timeZone).then(function (result) {
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
            ConversationService.RetrieveConversations($scope.master.filterData.ClientId, $scope.master.filterData.ProjectId, $scope.timeZone, $scope.editDataC.ConversationType).then(function (result) {
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
            ConversationService.DeleteConversation(id).then(function (result) {
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
        /*===========END CONVERSATION SECTION================ */
    }
    /*
     * END PROJECT CONVERSATION CONTROLLER
     */

    /*
     * BEGIN PROJECT CONTROLLER 
     */
    var ProjectCrtl = function ($scope, $rootScope, $timeout, $filter, $modalInstance, inquiry, ConversationService, InquiryService, ProposalService, ProjectService, RefreshTable) {
        $scope.inquiry = inquiry;
        $scope.projectData = {
            ProjectStatus: 0,
            ProjectType: 0,
            Currency:0,
            IsActive: true
        };
        $scope.oldProjectData = {};
        $scope.IsAjaxLoading = false;

        if (inquiry.PKProjectId > 0) {
            $scope.project = { mode: "Edit", saveText: "Update" };
        } else {
            $scope.project = { mode: "Add", saveText: "Save" };
        }

        // BEGIN DATE PICKER
        $scope.OpenStartDateCalender = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.isStartDateOpened = true;
            $scope.isEndDateOpened = false;
        };
        $scope.OpenEndDateCalender = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.isEndDateOpened = true;
            $scope.isStartDateOpened = false;
        };
        $scope.$watch("projectData.sDate", function (newValue) {
            $scope.projectData.StartDate = $filter('date')(newValue, 'dd-MM-yyyy');

            if ($scope.CheckDate()) {
                var dtS = $scope.projectData.StartDate.split('-');
                $scope.projectData.EndDate = $scope.projectData.StartDate;
                $scope.projectData.eDate = dtS[2] + "-" + dtS[1] + "-" + dtS[0] + "T00:00:00";
            }
        });
        $scope.$watch("projectData.eDate", function (newValue) {
            $scope.projectData.EndDate = $filter('date')(newValue, 'dd-MM-yyyy');
        });

        $scope.ValidateStartDate = function (sDate, frmProject) {
            if (!sDate) {
                frmProject.txtStartDate.$setValidity("invalidStartDate", true);
                return;
            } else if (sDate.length == 10) {
                if ($scope.ValidateDate(sDate)) {
                    if ($scope.IsGreterThanToday(sDate)) {
                        $scope.projectData.StartDate = $filter('date')(new Date(), 'dd-MM-yyyy');
                    } else {
                        frmProject.txtStartDate.$setValidity("invalidStartDate", true);
                        var dt = sDate.split('-');
                        $scope.projectData.sDate = dt[2] + "-" + dt[1] + "-" + dt[0] + "T00:00:00";
                    }
                } else {
                    frmProject.txtStartDate.$setValidity("invalidStartDate", false);
                }
            } else {
                frmProject.txtStartDate.$setValidity("invalidStartDate", false);
            }
        };

        $scope.ValidateEndDate = function (eDate, frmProject) {
            if (!eDate) {
                frmProject.txtEndDate.$setValidity("invalidEndDate", true);
                return;
            } else if (eDate.length == 10) {
                if ($scope.ValidateDate(eDate)) {
                    frmProject.txtEndDate.$setValidity("invalidEndDate", true);
                    var dtE = eDate.split('-');
                    $scope.projectData.eDate = dtE[2] + "-" + dtE[1] + "-" + dtE[0] + "T00:00:00";

                    if ($scope.CheckDate()) {
                        var dtS = $scope.projectData.StartDate.split('-');
                        $scope.projectData.EndDate = $scope.projectData.StartDate;
                        $scope.projectData.eDate = dtS[2] + "-" + dtS[1] + "-" + dtS[0] + "T00:00:00";
                    }
                } else {
                    frmProject.txtEndDate.$setValidity("invalidEndDate", false);
                }
            } else {
                frmProject.txtEndDate.$setValidity("invalidEndDate", false);
            }
        };
        // END DATE PICKER

        $scope.CheckDate = function () {
            var sDate = new Date($filter('date')($scope.projectData.sDate, 'MM-dd-yyyy'));
            var eDate = new Date($filter('date')($scope.projectData.eDate, 'MM-dd-yyyy'));

            if (sDate > eDate) {
                return true;
            }
            return false;
        };

        $scope.validateDropProjectCurrency = function () {
            if ($scope.projectData.Currency) return false;
            return true;
        };

        $scope.RetrieveProjectStatus = function () {
            ProjectService.RetrieveProjectStatus().then(function (res) {
                if (res.data.IsValidUser) {
                    if (res.data.MessageType === 1) {
                        if (res.data.DataList) {
                            $scope.projectStatus = res.data.DataList;
                        }
                    }
                    else {
                        toastr.error(res.data.Message, 'Opps, Something went wrong');
                    }
                }
                else {
                    $rootScope.redirectToLogin();
                }
            });
        };
        $scope.RetrieveProjectTypes = function () {
            ProjectService.RetrieveProjectTypes().then(function (res) {
                if (res.data.IsValidUser) {
                    if (res.data.MessageType === 1) {
                        if (res.data.DataList) {
                            $scope.projectTypes = res.data.DataList;
                        }
                    }
                    else {
                        toastr.error(res.data.Message, 'Opps, Something went wrong');
                    }
                }
                else {
                    $rootScope.redirectToLogin();
                }
            });
        };

        $scope.RetrieveProject = function () {
            //Load Status
            ProjectService.RetrieveProjectStatus().then(function (resStatus) {
                if (resStatus.data.IsValidUser) {
                    $scope.projectStatus = resStatus.data.DataList;

                    //Load Type
                    ProjectService.RetrieveProjectTypes().then(function (resType) {
                        $scope.projectTypes = resType.data.DataList;

                        //Load Project 
                        if ($scope.inquiry.PKProjectId > 0) { //Only for edit mode
                            ProjectService.RetrieveProject($scope.inquiry.PKProjectId).then(function (resProject) {
                                if (resProject.data.IsValidUser) {
                                    if (resProject.data.MessageType === 1) {
                                        if (resProject.data.DataList) {
                                            $scope.projectData = resProject.data.DataList;
                                            $scope.projectData.StartDate = moment().format($scope.projectData.StartDate, "DD-MM-YYYY");
                                            $scope.projectData.EndDate = moment().format($scope.projectData.EndDate, "DD-MM-YYYY");

                                            $scope.projectData.sDate = $scope.projectData.StartDate;
                                            $scope.projectData.eDate = $scope.projectData.EndDate;
                                        }
                                    }
                                    else {
                                        toastr.error(resProject.data.Message, 'Opps, Something went wrong');
                                    }
                                }
                                else {
                                    $rootScope.redirectToLogin();
                                }
                            });
                        }
                    });
                }
                else {
                    $rootScope.redirectToLogin();
                }
            });
        };
        $scope.RetrieveProject();

        $scope.ValidateStatus = function () {
            if ($scope.projectData.ProjectStatus && $scope.projectData.ProjectStatus != 0) return false;
            return true;
        };
        $scope.ValidateType = function () {
            if ($scope.projectData.ProjectType && $scope.projectData.ProjectType != 0) return false;
            return true;
        };

        $scope.CreateUpdateProject = function (projectData, frmProject) {
            projectData.FKInquiryId = $scope.inquiry.PKInquiryId;

            ProjectService.CreateUpdateProject(projectData, $scope.timeZone).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType === 1) {
                        var projectId = result.data.DataList;

                        frmProject.$setPristine(); //reset form validation
                        RefreshTable();
                        toastr.success(result.data.Message, 'Success');
                        $scope.Close();
                    } else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                }
                else {
                    $rootScope.redirectToLogin();
                }
            });
        };

        $scope.Close = function () {
            $modalInstance.close();
        };
    }
    /*
     * END PROJECT CONTROLLER
     */

})();