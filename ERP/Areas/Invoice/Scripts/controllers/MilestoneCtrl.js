/// <reference path="../../../../Scripts/libs/angular/angular.js" />

(function () {

    'use strict';

    // CONTROLLER SIGNATURE
    angular.module("ERPApp.Controllers")
        .controller("MilestoneCtrl", [
            "$scope", "$modal", "$rootScope", "$timeout", "ConversationService", "MilestoneService", "ProjectService", "$http", "$filter", "ngTableParams", "$q",
            MilestoneCtrl
        ]);

    /*
     * END PROJECT CONVERSATION CONTROLLER
    */
    function MilestoneCtrl($scope, $modal, $rootScope, $timeout, ConversationService, MilestoneService, ProjectService, $http, $filter, ngTableParams, $q) {
        $scope.timeZone = new Date().getTimezoneOffset().toString();
        $scope.SetOpenedTab = function (tab) {
            $scope.master.CurrentTab = tab;
        };
        $scope.projectList = {};

        $scope.GetClient = function () {
            ConversationService.GetClient($scope.master.filterData.ClientId).then(function (result) {
                $scope.ClientInfo = result.data.DataList;
            });
        };


        $scope.$watch("master.filterData.ClientId", function (newValue) {
            if ($scope.master.CurrentTab == "Milestone") {
                if (newValue == 0) {
                    $scope.ClientInfo = [];
                } else { //filter on confirmed project
                    $scope.GetClient();
                    $scope.RetrieveConfirmedProjects();
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

        $scope.RetrieveConfirmedProjects = function () {
            $scope.IsAjaxLoading = true;
            ProjectService.RetrieveConfirmedProjects($scope.master.filterData.ClientId).then(function (result) {
                if (result.data.IsValidUser) {
                    //display no data message
                    if (result.data.MessageType === 1) {
                        $scope.projectList = result.data.DataList;
                        angular.forEach($scope.projectList, function (value, key) {
                            value.isOpen = false;
                            value.milestonePrice = 0;
                        });

                        $scope.IsAjaxLoading = false;
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

        $scope.RetrieveProjectMilestones = function (projectId, isOpen, arrayIndex) {
            if (!isOpen) {
                MilestoneService.RetrieveProjectMilestones(projectId).then(function (result) {
                    if (result.data.IsValidUser) {
                        $scope.projectList[arrayIndex].milestoneList = result.data.DataList;
                        angular.forEach($scope.projectList[arrayIndex].milestoneList, function (value, key) {
                            value.isSelected = false;
                        });

                        $scope.projectList[arrayIndex].milestoneList.sort(function (a, b) {
                            return Date.parse(a.ChgDate) - Date.parse(b.ChgDate);
                        });

                        if ($scope.projectList[arrayIndex].milestoneList.length > 0) {
                            $scope.projectList[arrayIndex].maxMilestoneDate = $scope.projectList[arrayIndex].milestoneList[$scope.projectList[arrayIndex].milestoneList.length - 1].ChgDate;
                        }
                        //var minT = $scope.projectList[arrayIndex].milestoneList[0];
                    }
                    else {
                        $rootScope.redirectToLogin();
                    }
                });
            }
        };

        $scope.DeleteProjectMilestone = function (projectId, milestoneId, index) {
            $scope.IsAjaxLoading = true;
            MilestoneService.DeleteProjectMilestone(milestoneId).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType === 1) {
                        $scope.RetrieveProjectMilestones(projectId, false, index);
                        $scope.IsAjaxLoading = false;
                        toastr.success(result.data.Message, 'Success');
                    } else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
            });
        };

        $scope.oldMilestoneData = {};
        $scope.UpdateProjectMilestone = function (projectId, milestone, index, price, milestonePrice) {
            $scope.oldMilestoneData = {
                PKMilestoneId: milestone.PKMilestoneId,
                FKProjectId: milestone.FKProjectId,
                MilestoneName: milestone.MilestoneName,
                MilestoneDesc: milestone.MilestoneDesc,
                StartDate: milestone.StartDate,
                EndDate: milestone.EndDate,
                TotalHours: milestone.TotalHours,
                Price: milestone.Price,
                Currency: milestone.Currency,

            };
            $scope.oldMilestoneData.projectPrice = price;
            $scope.oldMilestoneData.milestonePrice = milestonePrice;
            $scope.oldMilestoneData.pendingPrice = price - milestonePrice;


            $scope.AddProjectMilestone(projectId, milestone.PKMilestoneId, milestone.Currency, price, milestonePrice, index);
        };

        $scope.AddProjectMilestone = function (projectId, milestoneId, Currency, price, milestonePrice, index) {
            var modalInstance = $modal.open({
                templateUrl: 'Milestone.html',
                scope: $scope,
                controller: ProjectMilestoneCrtl,
                resolve: {
                    projectId: function () { return projectId; },
                    Currency: function () { return Currency; },
                    projectPrice: function () { return price; },
                    milestonePrice: function () { return milestonePrice; },
                    milestoneId: function () { return milestoneId; },
                    index: function () { return index; },
                    ConversationService: function () { return ConversationService },
                    MilestoneService: function () { return MilestoneService },
                    ProjectService: function () { return ProjectService },
                },
                size: 'lg'
            });
        };

        $scope.GenerateInvoice = function (index) {
            $scope.master.SelectedMilestoneList = [];
            angular.forEach($scope.projectList[index].milestoneList, function (value, key) {
                if (value.isSelected) {
                    $scope.master.SelectedMilestoneList.push(value);
                }
            });

            $q.all($scope.master.SelectedMilestoneList).then(function () {
                if ($scope.master.SelectedMilestoneList.length > 0) {
                    $scope.master.IsInvoiceTabOpened = true;
                } else {
                    var len = $scope.projectList[index].milestoneList.length;
                    if (len > 0) {
                        toastr.warning("Please select at least one milestone", "Make proper selection");
                    }
                }
            });
        };

        $scope.CalculateMilestonePrice = function (price, index) {
            $scope.projectList[index].milestonePrice += price;
        };
    }

    /*
    * END PROJECT CONVERSATION CONTROLLER
    */

    var ProjectMilestoneCrtl = function ($scope, $rootScope, $timeout, $filter, $modalInstance, projectId, Currency, projectPrice, milestonePrice, milestoneId, index, ConversationService, MilestoneService, ProjectService) {
        $scope.milestoneData = {
            FKProjectId: projectId,
            IsActive: true,
            Currency: Currency,
            ProjectPrice: projectPrice,
            milestonePrice: milestonePrice,
            pendingPrice: projectPrice - milestonePrice
        };

        $scope.isMilestoneFirstFocus = true;

        $scope.validateDropMilestoneCurrency = function () {
            if ($scope.milestoneData.Currency != 0) return false;
            return true;
        };



        $scope.cmpProjectAndMilestonePrice = function (currentPrice, pendingPrice, frmMilestone) {
            if (pendingPrice < currentPrice) {
                toastr.warning('Total milestone price is greater than project price.');
                frmMilestone.$invalid = true;
            } else {
                frmMilestone.$invalid = false;
            }

        }


        if (milestoneId > 0) {
            $scope.milestone = { mode: "Edit", saveText: "Update" };
            $scope.milestoneData = {
                PKMilestoneId: $scope.oldMilestoneData.PKMilestoneId,
                FKProjectId: $scope.oldMilestoneData.FKProjectId,
                MilestoneName: $scope.oldMilestoneData.MilestoneName,
                MilestoneDesc: $scope.oldMilestoneData.MilestoneDesc,
                StartDate: $filter('date')($scope.oldMilestoneData.StartDate, 'dd-MM-yyyy'),
                EndDate: $filter('date')($scope.oldMilestoneData.EndDate, 'dd-MM-yyyy'),
                TotalHours: $scope.oldMilestoneData.TotalHours,
                Price: $scope.oldMilestoneData.Price,
                Currency: $scope.oldMilestoneData.Currency,
                sDate: $scope.oldMilestoneData.StartDate,
                eDate: $scope.oldMilestoneData.EndDate,
                ProjectPrice: $scope.oldMilestoneData.projectPrice,
                milestonePrice: $scope.oldMilestoneData.milestonePrice,
                pendingPrice: $scope.oldMilestoneData.projectPrice - milestonePrice

            };
        } else {
            $scope.milestone = { mode: "Add", saveText: "Save" };
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
        $scope.$watch("milestoneData.sDate", function (newValue) {
            $scope.milestoneData.StartDate = $filter('date')(newValue, 'dd-MM-yyyy');
        });
        $scope.$watch("milestoneData.eDate", function (newValue) {
            $scope.milestoneData.EndDate = $filter('date')(newValue, 'dd-MM-yyyy');
        });
        $scope.ValidateStartDate = function (sDate, frmMilestone) {
            if (!sDate) {
                frmMilestone.txtStartDate.$setValidity("invalidStartDate", true);
                return;
            } else if (sDate.length == 10) {
                if ($scope.ValidateDate(sDate)) {
                    if ($scope.IsGreterThanToday(sDate)) {
                        $scope.milestoneData.StartDate = $filter('date')(new Date(), 'dd-MM-yyyy');
                    } else {
                        frmMilestone.txtStartDate.$setValidity("invalidStartDate", true);
                        var dt = sDate.split('-');
                        $scope.milestoneData.sDate = dt[2] + "-" + dt[1] + "-" + dt[0] + "T00:00:00";
                    }
                } else {
                    frmMilestone.txtStartDate.$setValidity("invalidStartDate", false);
                }
            } else {
                frmMilestone.txtStartDate.$setValidity("invalidStartDate", false);
            }
        };
        $scope.ValidateEndDate = function (eDate, frmMilestone) {
            if (!eDate) {
                frmMilestone.txtEndDate.$setValidity("invalidEndDate", true);
                return;
            } else if (eDate.length == 10) {
                if ($scope.ValidateDate(eDate)) {
                    if ($scope.IsGreterThanToday(eDate)) {
                        $scope.milestoneData.EndDate = $filter('date')(new Date(), 'dd-MM-yyyy');
                    } else {
                        frmMilestone.txtEndDate.$setValidity("invalidEndDate", true);
                        var dt = eDate.split('-');
                        $scope.milestoneData.eDate = dt[2] + "-" + dt[1] + "-" + dt[0] + "T00:00:00";
                    }
                } else {
                    frmMilestone.txtEndDate.$setValidity("invalidEndDate", false);
                }
            } else {
                frmMilestone.txtEndDate.$setValidity("invalidEndDate", false);
            }
        };
        // END DATE PICKER

        $scope.CreateUpdateMilestone = function (milestoneData, frmMilestone) {
            if (milestoneData.StartDate) {
                var _StartDateArray = milestoneData.StartDate.split("-")
                var _StartDate = new Date(_StartDateArray[2], _StartDateArray[1] - 1, _StartDateArray[0], moment().hours(), moment().minute(), moment().second());
                milestoneData.StartDate = angular.copy(_StartDate);
            }

            if (milestoneData.EndDate) {
                var _EndDateArray = milestoneData.EndDate.split("-")
                var _EndDate = new Date(_EndDateArray[2], _EndDateArray[1] - 1, _EndDateArray[0], moment().hours(), moment().minute(), moment().second());
                milestoneData.EndDate = angular.copy(_EndDate);
            }

            MilestoneService.CreateUpdateMilestone(milestoneData, milestoneId, $scope.timeZone).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType === 1) {
                        var projectId = result.data.DataList;

                        //var projectPrice = 0;
                        //angular.forEach($scope.projectList, function (value, key) {
                        //    if (value.PKProjectId == projectId) {
                        //        projectPrice = value.Price;
                        //        return projectPrice;
                        //    }
                        //})
                        //var totalMilestonePrice = 0;
                        //MilestoneService.RetrieveProjectMilestones(projectId).then(function (result) {
                        //    if (result.data.IsValidUser) {
                        //        angular.forEach(result.data.DataList, function (value, key) {
                        //            totalMilestonePrice = totalMilestonePrice + value.Price;
                        //        });
                        //        if (projectPrice & totalMilestonePrice) {
                        //            if (projectPrice < totalMilestonePrice) {
                        //                toastr.warning('Milestone price greater than project price.');
                        //            }
                        //        }
                        //    }
                        //    else {
                        //        $rootScope.redirectToLogin();
                        //    }
                        //});

                        $scope.RetrieveProjectMilestones(projectId, false, index);

                        frmMilestone.$setPristine(); //reset form validation
                        $scope.Close();
                        toastr.success(result.data.Message, 'Success');
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
    }
})();