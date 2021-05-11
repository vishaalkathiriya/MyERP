/// <reference path="../libs/angular/angular.min.js" />


(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("PMSProjectCtrl", [
            "$scope", "$rootScope", "$timeout", "PMSProjectService", "$http", "$filter", "$q",
            projectCtrl
        ]);


    //Main controller function
    function projectCtrl($scope, $rootScope, $timeout, PMSProjectService, $http, $filter, $q) {
        $scope.editData = $scope.editData || {};
        $scope.mode = "Add";
        $scope.saveText = "Save";
        $scope.timeZone = new Date().getTimezoneOffset().toString(); //timezone difference in minutes
        $scope.isFocusFirst = false;
        $scope.storage = { lastRecord: "" };
        $scope.selectedTL = "";
        $scope.oldProjectData = {};
        $scope.filteList = [{ Id: 1, Label: 'Active' }, { Id: 2, Label: 'Hold' }, { Id: 3, Label: 'Finished' }, { Id: 4, Label: 'Deleted' }];
        $scope.empLoginId = window.erpuid;
        //  $scope.filterData = "";
        $scope.IsAjaxLoadingPMS = false;

        $scope.listOfProjectTypes = window.listOfProjectTypes;

        //Project filter apply on technology and project type
        // $scope.filterTechnologies = 0;
        $scope.filterTechnologies = [];
        $scope.filterProjectType = 0;
        $scope.filterData = 1;
        $scope.filterUserList = 0;

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
        $scope.minDate = $scope.minDate || new Date();
        $scope.startDate = new Date();

        $scope.calendarOpenStartDate = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.calOpenStartDate = true;
            $scope.calOpenEndDate = false;
        };

        $scope.calendarOpenEndDate = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.calOpenStartDate = false;
            $scope.calOpenEndDate = true;
        };

        $scope.$watch('startDate', function (newValue) {
            $scope.editData.StartDate = $filter('date')(newValue, 'dd-MM-yyyy');
            $scope.minDate = newValue;
            if ($scope.IsStartDateBig()) {
                $scope.editData.EndDate = $scope.editData.StartDate;
                $scope.endDate = newValue;
            }
        });
        $scope.$watch('endDate', function (newValue) {
            $scope.editData.EndDate = $filter('date')(newValue, 'dd-MM-yyyy');
        });

        $scope.ValidateStartDate = function (startDate) {
            if (startDate.length == 10) {
                if ($scope.ValidateDate(startDate)) {
                    $scope.startDate = $scope.StringToDateString(startDate);
                    $scope.projectform.txtStartDate.$setValidity("invalidStartDate", true);

                    if ($scope.IsStartDateBig()) {
                        $scope.editData.EndDate = startDate;
                        $scope.endDate = $scope.StringToDateString(startDate);
                    }
                } else {
                    $scope.projectform.txtStartDate.$setValidity("invalidStartDate", false);
                }
            } else {
                $scope.projectform.txtStartDate.$setValidity("invalidStartDate", false);
            }
        };

        $scope.ValidateEndDate = function (endDate) {
            if (endDate.length == 10) {
                if ($scope.ValidateDate(endDate)) {
                    $scope.endDate = $scope.StringToDateString(endDate);
                    $scope.projectform.txtEndDate.$setValidity("invalidEndDate", true);

                    if ($scope.IsStartDateBig()) {
                        $scope.editData.EndDate = $scope.editData.StartDate;
                        $scope.endDate = $scope.StringToDateString($scope.editData.StartDate);
                    }
                } else {
                    $scope.projectform.txtEndDate.$setValidity("invalidEndDate", false);
                }
            } else {
                $scope.projectform.txtEndDate.$setValidity("invalidEndDate", false);
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

        $scope.IsStartDateBig = function () {
            if ($scope.editData.StartDate && $scope.editData.EndDate) {
                var fDT = $scope.editData.StartDate.split('-');
                var tDT = $scope.editData.EndDate.split('-');

                var fDate = new Date(parseInt(fDT[2]), parseInt(fDT[1]) - 1, parseInt(fDT[0]));
                var tDate = new Date(parseInt(tDT[2]), parseInt(tDT[1]) - 1, parseInt(tDT[0]));

                if (fDate > tDate) {
                    return true;
                }
            }
            return false;
        };

        $scope.StringToDateString = function (dtValue) {
            var dt = dtValue.split('-');
            return dt[2] + "-" + dt[1] + "-" + dt[0] + "T00:00:00";
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

        /*reset the form*/
        function ResetForm() {

            //$scope.editData = {
            //    ProjectId: 0,
            //    ProjectName: "",
            //    StartDate: "",
            //    EndDate: "",
            //    TotalEstDays: "",
            //    Status: 1,
            //    IsArchived: false,
            //    Description: "",
            //    Technologies: 0,
            //    ProjectType: 0
            //};

            $scope.editData = {
                ProjectId: 0,
                ProjectName: "",
                StartDate: "",
                EndDate: "",
                TotalEstDays: "",
                Status: 1,
                IsArchived: false,
                Description: "",
                Technologies: [],
                ProjectType: 0
            };

            $scope.DisplayAssignedUserList = [];
            $scope.ReloadTLList();

            $scope.projectform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        };

        ////GET TECHNOLOGIES LIST 
        //function loadTechnologiesList() {
        //    PMSProjectService.GetTechnologies().then(function (result) {
        //        $scope.technologiesList = result.data.DataList;
        //        $scope.editData.Technologies = 0;
        //    });
        //};
        //loadTechnologiesList();

        //GET TECHNOLOGIES LIST 
        function loadTechnologiesList() {
            PMSProjectService.GetTechnologies().then(function (result) {
                $scope.technologiesList = result.data.DataList;
                //$scope.editData.Technologies = 0;
                //$scope.filterTechnologies = 0;
            });
        }
        loadTechnologiesList();

        $scope.loadTags = function (query) {
            var q = $q.defer();
            var data = [];
            setTimeout(function () {
                angular.forEach($scope.technologiesList, function (value, key) {
                    data.push({
                        'Id': value.Id,
                        'text': value.Label
                    });
                });
                q.resolve(data);
            }, 100);
            return q.promise;
        };


        //GET TECHNOLOGIES GROUP LIST
        function loadTechnologiesGroup() {
            PMSProjectService.GetTechnologiesGroup().then(function (result) {
                console.log(result);
                $scope.technologiesGroupList = result.data.DataList;
            });
        }
        loadTechnologiesGroup();


        $scope.loadTechnoGroup = function (query) {
            var q = $q.defer();
            var data = [];
            setTimeout(function () {
                angular.forEach($scope.technologiesGroupList, function (value, key) {
                    data.push({
                        'Id': value.Id,
                        'text': value.Label
                    });
                });
                q.resolve(data);
            }, 100);
            return q.promise;
        }



        /* getting list of project type  */
        function LoadProjectType() {
            PMSProjectService.GetProjectTypeList().then(function (result) {
                $scope.ProjectTypeList = result.data.DataList;
                $scope.editData.ProjectType = 0;
            });
        }
        LoadProjectType();

        /* getting list of status */
        function loadStatusList() {
            PMSProjectService.GetStatusList().then(function (result) {
                // ResetForm();
                $scope.StatusList = result.data.DataList;
                $timeout(function () {
                    $scope.editData.Status = 1;
                });
            });
        };
        loadStatusList();



        /*getting TL list*/
        $scope.loadTLList = function () {
            return PMSProjectService.GetTLList()
        };

        $scope.loadTLList().then(function (result) {
            $scope.TeamLeadList = result.data.DataList;
            $timeout(function () {
                $scope.editData.TeamLeadId = 0;
            }, 500);
        });

        /*getting user list on selection of TL*/
        $scope.loadUserList = function (leadId) {
            PMSProjectService.GetUserList(leadId).then(function (result) {
                $scope.UserList = result.data.DataList;
            });
        };

        /*getting user list on selection for login base list*/
        function loadProjectUserList() {
            PMSProjectService.GetUserList($scope.empLoginId).then(function (result) {
                $scope.ProjectUserList = result.data.DataList;
            });
        };
        loadProjectUserList();

        $scope.DisplayAssignedUserList = [];
        /*assigned TL and user to project*/
        $scope.AssignUsers = function () {
            $scope.SelectedUsers = [];
            angular.forEach($scope.UserList, function (value, key) {
                if (value.IsSelected) {
                    $scope.SelectedUsers.push(value);
                }
            });
            angular.forEach($scope.TeamLeadList, function (value, key) {
                if (value.Id == $scope.editData.TeamLeadId) {
                    $scope.DisplayAssignedUserList.push({ LeadId: $scope.editData.TeamLeadId, LeadName: value.Label, SelectedUsers: $scope.SelectedUsers });
                }
            });
            $scope.ReloadTLList(); // remove selected lead from list and reload tl list
        };

        /*delete item from display list*/
        $scope.DeleteAssignedEntry = function (index) {
            $scope.DisplayAssignedUserList.splice(index, 1);
            $scope.ReloadTLList(); // remove selected lead from list and reload tl list
        };

        /*Reload TL List*/
        $scope.ReloadTLList = function () {
            $scope.loadTLList().then(function (result) {
                $scope.TeamLeadList = [];
                angular.forEach(result.data.DataList, function (value, key) {
                    var isExists = false;
                    angular.forEach($scope.DisplayAssignedUserList, function (v, k) {
                        if (value.Id == v.LeadId) {
                            isExists = true;
                        }
                    });
                    if (!isExists) {
                        $scope.TeamLeadList.push(value);
                    }
                });
                $timeout(function () {
                    $scope.editData.TeamLeadId = 0;
                }, 500);
            });
        };

        /*add new project*/
        $scope.AddPMSProject = function () {
            $rootScope.isFormVisible = true;
            $scope.mode = "Add";
            $scope.saveText = "Save";
            ResetForm(); //reset form
        };

        /*cancel button click event*/
        $scope.CloseProject = function () {
            $scope.mode = "Add";
            ResetForm();
            $rootScope.isFormVisible = false;
        };

        /*save role*/
        $scope.CreateUpdateProject = function (prj) {

            var newPrj = angular.copy(prj);
            var sample = newPrj.Technologies;
            var temp = [];
            for (var i = 0 ; i < sample.length; i++)
            { temp.push(sample[i].Id); }
            newPrj.Technologies = temp.toString();
            $rootScope.IsAjaxLoading = true;
            PMSProjectService.CreateUpdateProject(newPrj).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        $scope.editData = {
                            ProjectId: 0, ProjectName: "", StartDate: "", EndDate: "", TotalEstDays: "", Status: 1, IsArchived: false, Description: "", Technologies: 0, ProjectType: 0
                        }; //reset the form content to disable save button
                        var projectId = result.data.DataList;

                        if ($scope.DisplayAssignedUserList.length > 0) {
                            ProcessList(projectId, $scope.DisplayAssignedUserList, function (res) {
                                //On Success: Send mail to users in single time
                                PMSProjectService.SendMail(projectId, $scope.mode, $scope.oldProjectData).then(function (r) {
                                    if (result.data.IsValidUser) {
                                        toastr.success(result.data.Message, 'Success');
                                        ResetForm();
                                        $scope.GetProjectList();
                                        $scope.storage.lastRecord = {};
                                        $scope.isFirstFocus = false;
                                        $scope.projectform.$setPristine();
                                        if ($scope.mode === "Edit") {
                                            $rootScope.isFormVisible = false;
                                            $scope.saveText = "Save";
                                        }
                                    } else {
                                        $rootScope.redirectToLogin();
                                    }
                                });
                            });
                        } else {
                            toastr.success(result.data.Message, 'Success');
                        }
                    } else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
                $rootScope.IsAjaxLoading = false;
            });
        }

        function ProcessList(projectId, list, callback) {
            var saveList = [];
            angular.forEach(list, function (value, key) {
                saveList.push({ ProjectId: projectId, EmployeeId: value.LeadId, IsTL: true, UserUnder: 0 });
                angular.forEach(value.SelectedUsers, function (v, k) {
                    saveList.push({ ProjectId: projectId, EmployeeId: v.Id, IsTL: false, UserUnder: value.LeadId });
                });
            });

            var index = 0, length = saveList.length;
            function saveData() {
                SaveLine(saveList[index]).then(function (result) {
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

        function SaveLine(line) {
            return PMSProjectService.CreateProjectUsers(line);
        }
        /*getting list of project*/
        $scope.GetProjectList = function () {

            //$rootScope.IsAjaxLoading = true;
            $scope.IsAjaxLoadingPMS = true;
            var filter_temp_tech = $scope.filterTechnologies;
            var temp = [];
            for (var i = 0 ; i < filter_temp_tech.length; i++)
            { temp.push(filter_temp_tech[i].Id); }
            //$scope.filterTechnologies = temp.toString();
            $scope.temp_technology = temp.toString();;

            PMSProjectService.GetProjectList($scope.timeZone, $scope.filterData, $scope.temp_technology, $scope.filterProjectType, $scope.filterUserList).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        $scope.ProjectList = result.data.DataList;
                    } else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
                //$rootScope.IsAjaxLoading = false;
                $scope.IsAjaxLoadingPMS = false;
            });
        }
        $scope.GetProjectList();

        /*get record for edit role*/
        $scope.UpdateProject = function (prj) {

            var data = [];
            var new_Technologies = prj.Technologies;
            var temp = new_Technologies.split(',').map(function (new_Technologies) { return Number(new_Technologies); });

            angular.forEach($scope.technologiesList, function (value, key) {
                for (var j = 0; j < temp.length ; j++) {

                    if (value.Id == temp[j]) {
                        data.push({
                            'Id': value.Id,
                            'text': value.Label
                        });
                    }
                }
            });
            $scope.editData.Technologies = data;
            prj.Technologies = angular.copy(data);

            $scope.oldProjectData = prj;

            $rootScope.isFormVisible = true;
            $scope.mode = "Edit";
            $scope.saveText = "Update";

            $scope.editData.ProjectId = prj.ProjectId;
            $scope.editData.ProjectName = prj.ProjectName;

            //  $scope.editData.Technologies = prj.Technologies;
            $scope.editData.ProjectType = prj.ProjectType;

            var sDT = prj.StartDate.split('-');
            var eDT = prj.EndDate.split('-');
            var sDate = new Date(parseInt(sDT[2]), parseInt(sDT[1]) - 1, parseInt(sDT[0]));
            var eDate = new Date(parseInt(eDT[2]), parseInt(eDT[1]) - 1, parseInt(eDT[0]));

            $scope.editData.StartDate = $filter('date')(sDate, 'dd-MM-yyyy');
            $scope.editData.EndDate = $filter('date')(eDate, 'dd-MM-yyyy');
            $scope.startDate = $scope.StringToDateString(prj.StartDate);
            $scope.endDate = $scope.StringToDateString(prj.EndDate);

            $scope.editData.TotalEstDays = prj.TotalEstDays;
            $scope.editData.Status = prj.Status;
            $scope.editData.IsArchived = prj.IsArchived;
            $scope.editData.Description = prj.Description;

            $scope.storage.lastRecord = {};
            angular.copy(prj, $scope.storage.lastRecord);
            $scope.storage.lastRecord = prj;

            angular.copy(prj.SelectedUsers, $scope.DisplayAssignedUserList);
            $scope.ReloadTLList();

            $scope.projectform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });

        };

        /*reset the form*/
        $scope.ResetProject = function () {
            if ($scope.mode == "Edit") {

                $scope.editData.ProjectId = $scope.storage.lastRecord.ProjectId;
                $scope.editData.ProjectName = $scope.storage.lastRecord.ProjectName;

                var sDT = $scope.storage.lastRecord.StartDate.split('-');
                var eDT = $scope.storage.lastRecord.EndDate.split('-');
                var sDate = new Date(parseInt(sDT[2]), parseInt(sDT[1]) - 1, parseInt(sDT[0]));
                var eDate = new Date(parseInt(eDT[2]), parseInt(eDT[1]) - 1, parseInt(eDT[0]));
                $scope.editData.StartDate = $filter('date')(sDate, 'dd-MM-yyyy');
                $scope.editData.EndDate = $filter('date')(eDate, 'dd-MM-yyyy');
                $scope.startDate = $scope.StringToDateString($scope.storage.lastRecord.StartDate);
                $scope.endDate = $scope.StringToDateString($scope.storage.lastRecord.EndDate);

                $scope.editData.TotalEstDays = $scope.storage.lastRecord.TotalEstDays;
                $scope.editData.Status = $scope.storage.lastRecord.Status;
                $scope.editData.IsArchived = $scope.storage.lastRecord.IsArchived;
                $scope.editData.Description = $scope.storage.lastRecord.Description;
                $scope.editData.ProjectType = $scope.storage.lastRecord.ProjectType;
                $scope.editData.Technologies = $scope.storage.lastRecord.Technologies;

                angular.copy($scope.storage.lastRecord.SelectedUsers, $scope.DisplayAssignedUserList);
                $scope.ReloadTLList();
            } else { //mode == add
                ResetForm();
            }
        };

        $scope.RedirectToModulePage = function (projectId) {
            window.location.href = "/PMS/Module/" + projectId;
        };

        $scope.FilterProject = function () {
            $rootScope.isFormVisible = false;
            //$scope.filterData = "";
            //angular.forEach($scope.selectedList, function (value, key) {
            //    $scope.filterData += value.Id + ",";
            //});
            $scope.GetProjectList();
        };
    };
})();

