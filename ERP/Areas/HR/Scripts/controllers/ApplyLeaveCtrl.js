/// <reference path="../libs/angular/angular.min.js" />


(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("ApplyLeaveCtrl", [
            "$scope", "$modal", "$rootScope", "$timeout", "ApplyLeaveService", "$http", "$filter", "ngTableParams", "$compile", "$q",
            applyLeaveCtrl
        ]);


    //Main controller function
    function applyLeaveCtrl($scope, $modal, $rootScope, $timeout, ApplyLeaveService, $http, $filter, ngTableParams, $compile, $q) {

        $scope.empId = 0;
        $scope.rows = [];
        $scope.events = [];
        $scope.eventFestivalList = [];
        $scope.eventEkadashiList = [];
        $scope.filterData = { EmployeeId: 0, LoginId: 0 };
        $scope.FestivalList = [];
        $scope.PreLeaveAbsentList = [];
        $scope.timeZone = new Date().getTimezoneOffset().toString(); //timezone difference in minutes


        var date = new Date();
        var d = date.getDate();
        var m = date.getMonth();
        var y = date.getFullYear();

        $scope.TimeArray = [
            { key: 1, val: '00:00' }, { key: 2, val: '00:30' }, { key: 3, val: '01:00' }, { key: 4, val: '01:30' }, { key: 5, val: '02:00' }, { key: 6, val: '02:30' }, { key: 7, val: '03:00' }, { key: 8, val: '03:30' }, { key: 9, val: '04:00' }, { key: 10, val: '04:30' },
            { key: 11, val: '05:00' }, { key: 12, val: '05:30' }, { key: 13, val: '06:00' }, { key: 14, val: '06:30' }, { key: 15, val: '07:00' }, { key: 16, val: '07:30' }, { key: 17, val: '08:00' }, { key: 18, val: '08:30' }, { key: 19, val: '09:00' }, { key: 20, val: '09:30' },
            { key: 21, val: '10:00' }, { key: 22, val: '10:30' }, { key: 23, val: '11:00' }, { key: 24, val: '11:30' }, { key: 25, val: '12:00' }, { key: 26, val: '12:30' }, { key: 27, val: '13:00' }, { key: 28, val: '13:30' }, { key: 29, val: '14:00' }, { key: 30, val: '14:30' },
            { key: 31, val: '15:00' }, { key: 32, val: '15:30' }, { key: 33, val: '16:00' }, { key: 34, val: '16:30' }, { key: 35, val: '17:00' }, { key: 36, val: '17:30' }, { key: 37, val: '18:00' }, { key: 38, val: '18:30' }, { key: 39, val: '19:00' }, { key: 40, val: '19:30' },
            { key: 41, val: '20:00' }, { key: 42, val: '20:30' }, { key: 43, val: '21:00' }, { key: 44, val: '21:30' }, { key: 45, val: '22:00' }, { key: 46, val: '22:30' }, { key: 47, val: '23:00' }, { key: 48, val: '23:30' }
        ];
        $scope.FixedColorList = [{ text: 'Half Day', colorCode: '#F6BB43' }, { text: 'Full Day', colorCode: '#4B89DC' },
                                 { text: 'Approved', colorCode: '#8DC153' }, { text: 'DisApproved', colorCode: '#E9573E' }];

        $scope.changeTo = 'Hungarian';

        /*get user list*/
        function GetUserList() {
            $rootScope.IsAjaxLoading = true;
            ApplyLeaveService.GetUserList().then(function (result) {
                if (result.data.IsValidUser) {
                    $scope.UserList = result.data.DataList;
                    $timeout(function () {
                        if ($scope.empId == 0) {
                            $scope.filterData.EmployeeId = result.data.Message;//we passed logged in user id in message from server side
                        }
                        if ($scope.empId != 0) {
                            $scope.filterData.EmployeeId = $scope.empId;//we passed logged in user id in message from server side
                        }
                        //$scope.filterData.EmployeeId = result.data.Message;//we passed logged in user id in message from server side
                        $scope.filterData.LoginId = result.data.Message;
                        $scope.LoadEvents($scope.filterData.EmployeeId, true); //load events for logged in user
                        $scope.LoadLeaveAbsentList($scope.filterData.EmployeeId); //load leave and absent list
                        CheckTeamLead($scope.filterData.LoginId);
                    });
                } else {
                    $rootScope.redirectToLogin();
                }
                $rootScope.IsAjaxLoading = false;
            });
        };
        GetUserList();

        // BEGIN CHECK IF LOGGED USER IS TEAM-LEAD OR NOT
        //$scope.isTeamLead = false;
        function CheckTeamLead(employeeId) {
            ApplyLeaveService.CheckTeamLead(employeeId).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType === 1) {
                        if (result.data.DataList.length > 0 || $scope.filterData.LoginId === 1) {
                            $scope.isTeamLead = true;
                        }
                    }
                }
            });
        }
        // END CHECK IF LOGGED USER IS TEAM-LEAD OR NOT

        $scope.eventSources = [$scope.events, $scope.eventFestivalList, $scope.eventEkadashiList];

        /* event source that contains custom events on the scope */
        $scope.LoadEvents = function (employeeId, isLoadOtherArray) {
            $rootScope.IsAjaxLoading = true;
            ApplyLeaveService.GetCalendarLeaveList(employeeId, $scope.timeZone).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) {
                        $scope.events.length = 0; //empty event array
                        var empLeaveList = _.filter(result.data.DataList, function (item) {
                            return item.CalLeaveType == "EmpLeave";
                        });

                        DoProcessLeave(empLeaveList, function () {
                            var festivalList = _.filter(result.data.DataList, function (item) {
                                return item.CalLeaveType == "Festival";
                            });
                            DoProcessFestival(festivalList, isLoadOtherArray, function () {
                                /* config object */
                                $scope.uiConfig = {
                                    calendar: {
                                        height: 600,
                                        editable: false,
                                        header: {
                                            left: 'title',
                                            right: 'month,agendaWeek,agendaDay today prev,next'
                                        },
                                        month: $scope.empId != 0 ? $scope.myMonth : parseInt(new Date().getMonth()),
                                        year: $scope.empId != 0 ? $scope.myYear : parseInt(new Date().getFullYear()),
                                        //month: $scope.filterData.EmployeeId != 0 ? 2 : 5,
                                        //year: $scope.filterData.EmployeeId != 0 ? 2014 : 2015,
                                        //events: function (start, end, callback) {
                                        //$scope.LoadEvents($scope.filterData.EmployeeId, false); // load event first for selected employee on every view change
                                        //callback($scope.events); // pass event array
                                        //},
                                        eventClick: $scope.alertOnEventClick,
                                        eventDrop: $scope.alertOnDrop,
                                        eventResize: $scope.alertOnResize,
                                        dayClick: $scope.alertOnDayClick,
                                        timeFormat: {
                                            // for agendaWeek and agendaDay do not display time in title
                                            // time already displayed in the view
                                            //agenda: '',

                                            // for all other views (19:00 - 20:30)
                                            '': 'H:mm{ - H:mm }'
                                        },
                                        eventRender: function (event, eventElement, monthView) {
                                            if (event.Status == "Pending") {
                                                eventElement.find("div.fc-event-inner").prepend("<i class='entypo-eye' title='Pending'></i>");
                                            }

                                            if (event.holiday) {
                                                $("td[data-date='" + $filter('date')(event.start, "yyyy-MM-dd") + "']").css({ "background-color": event.color, "border": "1px solid #FFFFFF" });
                                            } else if (event.ekadashi) {
                                                $("td[data-date='" + $filter('date')(event.start, "yyyy-MM-dd") + "']").css({ "background-color": event.color, "border": "1px solid #FFFFFF" });
                                            }


                                            if (event.Status && !event.IsSandwich) {
                                                var hasPermission = $scope.filterData.LoginId == 1 ? true
                                                                        : $scope.filterData.LoginId != $scope.filterData.EmployeeId ? true
                                                                        : $scope.filterData.LoginId == $scope.filterData.EmployeeId ? false
                                                                        : false;

                                                eventElement.attr('data-drc-popover', '');
                                                eventElement.attr('data-callbackevent', 'popoverCallback(date,approved,reason)');
                                                eventElement.attr('data-title', event.title);
                                                eventElement.attr('data-leavedate', $filter('date')(event.start, "dd-MM-yyyy"));
                                                eventElement.attr('data-comments', event.Comments);
                                                eventElement.attr('data-credate', event.CreDate);
                                                eventElement.attr('data-reasonlist', event.Reason);
                                                eventElement.attr('data-loginid', $scope.filterData.LoginId);

                                                //if ($scope.isTeamLead === false || $scope.filterData.EmployeeId != $scope.filterData.LoginId) {
                                                //if this work combox employee in not zero

                                                if ($scope.isTeamLead === false || $scope.filterData.LoginId != 0) {
                                                    eventElement.attr('data-haspermission', hasPermission);
                                                    eventElement.attr('data-status', event.Status);
                                                    eventElement.attr('data-ispending', event.Status == "Pending" ? true : false);
                                                }
                                            }
                                        },
                                        eventAfterAllRender: function () {
                                            $rootScope.$safeApply($scope, function () {
                                                $compile($(".fc-event-container"))($scope);
                                            });
                                        }
                                    }
                                };

                                $scope.eventSources = [$scope.events, $scope.eventFestivalList, $scope.eventEkadashiList];

                            });
                        });
                    } else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
                $rootScope.IsAjaxLoading = false;
            });
        };

        //set employee id for deshboard connection in apply leave infromation
        $scope.setEmployeeId = function (id, onDate) {

            if (id != 0) {
                $scope.empId = parseInt(id);
                //$scope.filterData.EmployeeId = id;
                var temp = onDate.split("-");
                $scope.myMonth = parseInt(temp[0]) - 1;
                $scope.myYear = parseInt(temp[1]);
            }
        }

        function DoProcessLeave(arrayList, callback) {
            var prom = [];
            angular.forEach(arrayList, function (value, key) {

                var colorCode = value.StatusOrColor == "Approved" ? $scope.FixedColorList[2].colorCode
                                : value.StatusOrColor == "DisApproved" ? $scope.FixedColorList[3].colorCode
                                : value.PartFullTime == "F" ? $scope.FixedColorList[1].colorCode
                                : value.PartFullTime == "P" ? $scope.FixedColorList[0].colorCode
                                : "#000"; // default value
                var _title = "";
                _title = value.PartFullTime == "F" && (value.StatusOrColor == "Approved" || value.StatusOrColor == "DisApproved") ? "(F) " + value.Title : value.Title;
                //if all member to be disaply a name with event name
                if ($scope.isTeamLead == true && $scope.filterData.EmployeeId == 0) {
                    //_title = $scope.GetUserName(value.EmployeeId).split(' ')[0] + $scope.GetUserName(value.EmployeeId).split(' ')[1].substr(0, 1) + $scope.GetUserName(value.EmployeeId).split(' ')[2].substr(0, 1) + " - " + (value.PartFullTime == "F" && (value.Status == "Approved" || value.Status == "DisApproved") ? "(F)" + value.LeaveTitle : value.LeaveTitle);
                    _title = $scope.GetUserName(value.EmpOrFestivalTypeId).split(' ')[0];
                }
                //change title used for update time disply title in with out "(F)"
                prom.push($scope.events.push({
                    title: _title,
                    changeTitle: value.Title,
                    start: value.StratDate,
                    end: value.EndDate,
                    Status: value.StatusOrColor,
                    PartFullTime: value.PartFullTime,
                    LeaveType: value.Type,
                    IsSandwich: value.Title == "Sandwich Leave" ? true : false,
                    Comments: value.Comment,
                    CreDate: $filter('date')(value.CreDate, 'dd-MM-yyyy HH:mm:ss'),
                    Reason: value.ApproveReason,
                    allDay: value.PartFullTime == "P" ? false : true,
                    color: colorCode
                }));
            });

            $q.all(prom).then(function () {
                callback();
            });
        }

        function DoProcessFestival(result, isLoadOtherArray, callback) {
            if (isLoadOtherArray) {
                var prom = [];
                var newarr = [];
                var unique = {};
                $scope.FestivalList.length = 0;
                $scope.eventFestivalList.length = 0;
                $scope.eventEkadashiList.length = 0;

                angular.forEach(result, function (value, key) {
                    if (!unique[value.Type]) {
                        newarr.push(value);
                        unique[value.Type] = value;
                    }

                    if (value.IsWorkingDay == 0) {
                        $scope.FestivalList.push({
                            key: value.Id,
                            val: $filter('date')(value.StratDate, 'dd-MM-yyyy')
                        });

                        //create event array for festival list
                        prom.push($scope.eventFestivalList.push({
                            title: value.Title,
                            start: value.StratDate,
                            color: value.StatusOrColor,
                            holiday: true
                        }));
                    } else if (value.IsWorkingDay == 1) {
                        //create event array for Ekadshi list
                        prom.push($scope.eventEkadashiList.push({
                            title: value.Title,
                            start: value.StratDate,
                            color: value.StatusOrColor,
                            ekadashi: true
                        }));
                    }
                });

                $scope.colorList = newarr; //for getting festival type color code to display on top of calendar

                $q.all(prom).then(function () {
                    callback();
                });
            } else {
                callback();
            }
        }

        $scope.FilterByUser = function (filter) {
            $scope.filterData.EmployeeId = filter.EmployeeId
            $scope.LoadEvents(filter.EmployeeId, false);
            $scope.LoadLeaveAbsentList(filter.EmployeeId);
        };

        /*alert on dayClick*/
        $scope.alertOnDayClick = function (date, allDay, jsEvent, view) {
            var check = $.fullCalendar.formatDate(date, 'yyyy-MM-dd');
            var today = $.fullCalendar.formatDate(new Date(), 'yyyy-MM-dd');
            if (check >= today || $scope.filterData.LoginId == 1) {// do operation only for date >= today or user is master admin
                $scope.ShowModel($filter('date')(date, 'dd-MM-yyyy'));
            }
        };
        /* alert on eventClick */
        $scope.alertOnEventClick = function (event, allDay, jsEvent, view) {

            if ($scope.filterData.EmployeeId != 0) {
                var check = $.fullCalendar.formatDate(event.start, 'yyyy-MM-dd');
                var today = $.fullCalendar.formatDate(new Date(), 'yyyy-MM-dd');

                if (check >= today || $scope.filterData.LoginId != $scope.filterData.EmployeeId || $scope.filterData.LoginId == 1) { // do edit operation only for date >= today
                    // if (event.Status == "Pending" && !event.IsSandwich || $scope.filterData.LoginId == 1) {//work only for leaves  // urvish
                    if (check >= today && !event.IsSandwich || $scope.filterData.LoginId == 1) {//work only for leaves  // urvish
                        var startKey = 1, endKey = 48;
                        if (event.PartFullTime == "P") { //Part Time
                            startKey = $scope.GetTimeKey($filter('date')(event.start, 'HH:mm'));
                            endKey = $scope.GetTimeKey($filter('date')(event.end, 'HH:mm'));
                        }

                        var arrayForModal = [{
                            Mode: "Edit",
                            LeaveTitle: event.changeTitle,
                            LeaveType: event.LeaveType,
                            LeaveDate: $filter('date')(event.start, "dd-MM-yyyy"),
                            PartFullTime: event.PartFullTime,
                            StartTime: startKey, EndTime: endKey,
                            Comments: event.Comments,
                            isFestival: $scope.HasFestival($filter('date')(event.start, "dd-MM-yyyy")), isSunday: $scope.HasSunday($filter('date')(event.start, "dd-MM-yyyy"))
                        }];
                        var modalInstance = $modal.open({
                            templateUrl: 'ApplyLeavePopup.html',
                            controller: ModalInstanceCtrl,
                            scope: $scope,
                            resolve: {
                                leaveDetails: function () { return arrayForModal; } //return array that you want to pass to model
                            }
                        });
                    } //else {//status is approved, dispproved, cancel
                    //    toastr.warning("Sorry, you can't update your leave record once someone has changed it's status", 'Warning');
                    //}
                } else {//date gone
                    toastr.warning("Sorry, you can't update event having a date before today's date", 'Warning');
                }
            }

        };
        /* alert on Drop */
        $scope.alertOnDrop = function (event, dayDelta, minuteDelta, allDay, revertFunc, jsEvent, ui, view) {
            $scope.alertMessage = ('Event Droped to make dayDelta ' + dayDelta);
        };
        /* alert on Resize */
        $scope.alertOnResize = function (event, dayDelta, minuteDelta, revertFunc, jsEvent, ui, view) {
            $scope.alertMessage = ('Event Resized to make dayDelta ' + minuteDelta);
        };
        /* add and removes an event source of choice */
        $scope.addRemoveEventSource = function (sources, source) {
            var canAdd = 0;
            angular.forEach(sources, function (value, key) {
                if (sources[key] === source) {
                    sources.splice(key, 1);
                    canAdd = 1;
                }
            });
            if (canAdd === 0) {
                sources.push(source);
            }
        };
        /* add custom event*/
        $scope.addEvent = function () {
            $scope.events.push({
                title: '###DRC###',
                start: new Date(y, m, 28),
                end: new Date(y, m, 29),
                className: ['openSesame']
            });
        };
        /* remove event */
        $scope.remove = function (index) {
            $scope.events.splice(index, 1);
        };
        /* Change View */
        $scope.changeView = function (view, calendar) {
            calendar.fullCalendar('changeView', view);
        };
        /* Change View */
        $scope.renderCalender = function (calendar) {
            calendar.fullCalendar('render');
        };

        /*callback fucntion to get button click of popup*/
        $scope.popoverCallback = function (date, approved, reason, username) {
            $rootScope.IsAjaxLoading = true;
            ApplyLeaveService.ApproveDisapproveLeave(date, $scope.filterData.EmployeeId, approved, reason).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) {
                        toastr.success(result.data.Message, 'Success');
                        $scope.LoadEvents($scope.filterData.EmployeeId, false); // load events in calendar
                    } else if (result.data.MessageType == 0) {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
                $rootScope.IsAjaxLoading = false;
            });

        };

        $scope.changeLang = function () {
            if ($scope.changeTo === 'Hungarian') {
                $scope.uiConfig.calendar.dayNames = ["Vasárnap", "Hétfő", "Kedd", "Szerda", "Csütörtök", "Péntek", "Szombat"];
                $scope.uiConfig.calendar.dayNamesShort = ["Vas", "Hét", "Kedd", "Sze", "Csüt", "Pén", "Szo"];
                $scope.changeTo = 'English';
            } else {
                $scope.uiConfig.calendar.dayNames = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];
                $scope.uiConfig.calendar.dayNamesShort = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];
                $scope.changeTo = 'Hungarian';
            }
        };

        /* event sources array*/
        //$scope.eventSources = [$scope.events, $scope.eventFestivalList, $scope.eventEkadashiList];

        $scope.ShowModel = function (date) {

            if ($scope.filterData.EmployeeId != 0) {
                //check date against event array, festival and absent list
                //if ($scope.HasSunday(date)) {
                //    toastr.warning("Sorry, You can't apply leave on sunday", 'Warning');
                //} else 
                if ($scope.HasFestival(date)) {
                    toastr.warning("Sorry, You can't apply leave on festival", 'Warning');
                } else if ($scope.HasAbsent(date)) {
                    toastr.warning("Sorry, We found absent record for this date", 'Warning');
                } else if ($scope.IsEvent(date)) {
                    var arrayForModal = [{
                        Mode: "Add",
                        LeaveTitle: "",
                        LeaveType: 1,
                        LeaveDate: date,
                        PartFullTime: "F",
                        StartTime: 1, EndTime: 48,
                        Comments: "",
                        isFestival: $scope.HasFestival(date), isSunday: $scope.HasSunday(date)
                    }];
                    var modalInstance = $modal.open({
                        templateUrl: 'ApplyLeavePopup.html',
                        controller: ModalInstanceCtrl,
                        scope: $scope,
                        resolve: {
                            leaveDetails: function () { return arrayForModal; } //return array that you want to pass to model
                        }
                    });
                }
            }
        }

        /*load absent and preleave list*/
        $scope.LoadLeaveAbsentList = function (employeeId) {
            $rootScope.IsAjaxLoading = true;
            ApplyLeaveService.GetLeaveAbsentList(employeeId).then(function (result) {
                $scope.PreLeaveAbsentList.length = 0;
                if (result.data.IsValidUser) {
                    for (var i = 0; i < result.data.DataList.length; i++) {
                        $scope.PreLeaveAbsentList.push({
                            key: result.data.DataList[i].Key,
                            val: $filter('date')(result.data.DataList[i].Value, 'dd-MM-yyyy')
                        });
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
                $rootScope.IsAjaxLoading = false;
            });
        };

        /*load festival list*/
        //$scope.LoadFestivalList = function () {
        //   // return ApplyLeaveService.GetFestivalList();
        //};

        $scope.GetTimeKey = function (timeInText) {
            var keepGoing = true;
            var timeKey;
            angular.forEach($scope.TimeArray, function (value, key) {
                if (keepGoing) {
                    if (value.val == timeInText) {
                        timeKey = key + 1;//because key will start from 0 index
                        keepGoing = false;
                    }
                }
            });

            return timeKey;
        };

        $scope.GetUserName = function (employeeId) {

            for (var i = 0; i < $scope.UserList.length; i++) {
                if (employeeId == $scope.UserList[i].Id) {
                    return $scope.UserList[i].Label;
                }
            }
            return "";
        };

        $scope.IsEvent = function (date) {
            var keepGoing = true;
            angular.forEach($scope.events, function (value, key) {
                if (keepGoing) {
                    if ($filter('date')(value._start, 'dd-MM-yyyy') == date) {
                        toastr.warning("you have already taken leave on selected date", 'Warning');
                        keepGoing = false;
                    }
                }
            });

            return keepGoing;
        };
        $scope.HasSunday = function (date) {//dd-MM-yyyy
            /*var dt = date.split('-');
            var myDate = new Date();
            myDate.setFullYear(parseInt(dt[2])); myDate.setMonth(parseInt(dt[1]) - 1); myDate.setDate(parseInt(dt[0]));
            if (myDate.getDay() == 0) return true;
            return false;*/
        };
        $scope.HasFestival = function (dt) {
            for (var i = 0; i < $scope.FestivalList.length; i++) {
                if (dt == $scope.FestivalList[i].val) {
                    return true;
                }
            }
            return false;
        };
        $scope.HasAbsent = function (dt) {
            for (var i = 0; i < $scope.PreLeaveAbsentList.length; i++) {
                if (dt == $scope.PreLeaveAbsentList[i].val && $scope.PreLeaveAbsentList[i].key == 'A') {
                    return true;
                }
            }
            return false;
        };
        $scope.HasPreLeave = function (dt) {
            for (var i = 0; i < $scope.PreLeaveAbsentList.length; i++) {
                if (dt == $scope.PreLeaveAbsentList[i].val && $scope.PreLeaveAbsentList[i].key == 'P') {
                    return true;
                }
            }
            return false;
        };
        $scope.IsWorkingDay = function (dt) {
            if ($scope.HasFestival(dt) == false && $scope.HasAbsent(dt) == false && $scope.HasPreLeave(dt) == false) {
                return true;
            } else {
                return false;
            }
        };
        $scope.IsFullDayLeave = function (time) {
            if (time == "F") return true;
            return false;
        };
    };

    // BEGIN MODAL INSTANCE CONTROLLER
    var ModalInstanceCtrl = function ($scope, $rootScope, ApplyLeaveService, $filter, $modalInstance, leaveDetails) {
        $scope.oldDate = leaveDetails[0].LeaveDate;
        $scope.Mode = leaveDetails[0].Mode;
        $scope.ApplyingFor = $scope.GetUserName($scope.filterData.EmployeeId);
        $scope.subTime2Array = [];

        //define here for load all the required functions
        $scope.rows = [{ LeaveTitle: leaveDetails[0].LeaveTitle, LeaveType: leaveDetails[0].LeaveType, LeaveDate: leaveDetails[0].LeaveDate, PartFullTime: leaveDetails[0].PartFullTime, StartTime: leaveDetails[0].StartTime, EndTime: leaveDetails[0].EndTime, Comments: leaveDetails[0].Comments, isFestival: leaveDetails[0].isFestival }];

        $scope.Close = function () {
            $modalInstance.close();
            $rootScope.IsAjaxLoading = false;
        };

        $scope.subTime2Array[0] = $scope.TimeArray;
        $scope.BindTime2 = function (time1, index) {
            $scope.subTime2Array[index] = $scope.TimeArray.slice(parseInt(time1));
            $scope.rows[index].EndTime = $scope.subTime2Array[index][0].key;
        };

        $scope.DeleteRow = function (index) {
            $scope.oldDate = dateAddDays($scope.oldDate, -1);
            $scope.rows.splice(index, 1);
            $scope.subTime2Array.splice(index, 1);
        }

        $scope.AddRow = function () {
            $scope.oldDate = dateAddDays($scope.oldDate, 1);
            $scope.isFestival = $scope.HasFestival($scope.oldDate); //check for date; if it is exists in festival list
            var lastRow = $scope.rows[$scope.rows.length - 1];
            if ($scope.isFestival) {
                $scope.rows.push({
                    LeaveTitle: "", LeaveType: 1, LeaveDate: $scope.oldDate, PartFullTime: "F", StartTime: 1, EndTime: 48, Comments: "", isFestival: $scope.isFestival
                });
                $scope.subTime2Array.push($scope.TimeArray);
            } else {
                if (lastRow.PartFullTime == "P") {
                    $scope.subTime2Array.push($scope.TimeArray.slice(parseInt(lastRow.StartTime)));
                    $scope.rows.push({
                        LeaveTitle: lastRow.LeaveTitle, LeaveType: lastRow.LeaveType, LeaveDate: $scope.oldDate, PartFullTime: "P", StartTime: lastRow.StartTime, EndTime: lastRow.EndTime, Comments: lastRow.Comments, isFestival: $scope.isFestival
                    });
                } else {
                    $scope.rows.push({
                        LeaveTitle: lastRow.LeaveTitle, LeaveType: lastRow.LeaveType, LeaveDate: $scope.oldDate, PartFullTime: "F", StartTime: 1, EndTime: 48, Comments: lastRow.Comments, isFestival: $scope.isFestival
                    });
                    $scope.subTime2Array.push($scope.TimeArray);
                }
            }
        };

        $scope.CancelLeave = function () {
            $rootScope.IsAjaxLoading = true;
            ApplyLeaveService.CancelLeave($scope.rows[0].LeaveDate, $scope.filterData.EmployeeId).then(function (result) {
                if (result.data.MessageType == 1) {
                    toastr.success(result.data.Message, 'Success');
                    $scope.Close();
                    //urvish
                    $scope.LoadLeaveAbsentList($scope.filterData.EmployeeId);
                    $scope.LoadEvents($scope.filterData.EmployeeId, false); // load events in calendar
                } else if (result.data.MessageType == 0) {
                    toastr.error(result.data.Message, 'Opps, Something went wrong');
                }
                $rootScope.IsAjaxLoading = false;
            });
        };

        $scope.SaveApplyLeave = function () {
            $rootScope.IsAjaxLoading = true;
            if ($scope.Mode == "Add") {
                /*CHECK FOR SANDWICH LEAVES*/
                $scope.isMiddleSundayHoliday = false;
                $scope.isFirstSandwich = false;
                $scope.isLastSandwich = false;
                $scope.postDate = $scope.rows[$scope.rows.length - 1].LeaveDate;
                $scope.postDatePartFullTime = $scope.rows[$scope.rows.length - 1].PartFullTime;

                //check for middle days
                if ($scope.rows.length == 1) {
                    $scope.postDate = $scope.rows[0].LeaveDate;
                    $scope.postDatePartFullTime = $scope.rows[0].PartFullTime;
                    if ($scope.rows[0].isFestival) {
                        $scope.isMiddleSundayHoliday = true;
                    }
                } else if ($scope.rows.length == 2) {
                    for (var i = 1; i < 2; i++) {
                        if ($scope.rows[i].isFestival) {
                            $scope.isMiddleSundayHoliday = true;
                        } else if (!$scope.IsFullDayLeave($scope.rows[i].PartFullTime)) {
                            $scope.isMiddleSundayHoliday = false;
                        }
                    }
                }
                else {
                    for (var i = 1; i < $scope.rows.length - 1; i++) {
                        if ($scope.rows[i].isFestival) {
                            $scope.isMiddleSundayHoliday = true;
                        } else if (!$scope.IsFullDayLeave($scope.rows[i].PartFullTime)) {
                            $scope.isMiddleSundayHoliday = false;
                        }
                    }
                }

                var arrayList = [];
                var arrayTemp = [];

                //check holiday/absent for before days
                $scope.preDate = $scope.rows[0].LeaveDate;
                $scope.startPreDate = $scope.preDate;
                arrayList.push({ key: "P", date: $scope.startPreDate });
                $scope.IsLeaveOnFirstEntry = false;
                if ($scope.IsFullDayLeave($scope.rows[0].PartFullTime) == true) {
                    if ($scope.IsWorkingDay($scope.preDate)) {
                        $scope.IsLeaveOnFirstEntry = true;
                    }
                    $scope.preDate = dateAddDays($scope.preDate, -1);

                    while ($scope.preDate != "") {
                        if ($scope.HasFestival($scope.preDate) == true) {
                            arrayTemp.push({ key: "P", date: $scope.preDate });
                            $scope.preDate = dateAddDays($scope.preDate, -1);
                        } else if ($scope.HasAbsent($scope.preDate) == true || $scope.HasPreLeave($scope.preDate) == true) {
                            $scope.startPreDate = $scope.preDate;
                            arrayList.push({ key: "P", date: $scope.startPreDate });
                            arrayList = arrayList.concat(arrayTemp);
                            arrayTemp.length = 0;// reset array
                            $scope.isFirstSandwich = true;
                            $scope.preDate = dateAddDays($scope.preDate, -1);
                            // BEGIN BY DRCVHK
                            if ($scope.HasFestival(dateAddDays($scope.startPreDate, 1)) == true) {
                                $scope.isMiddleSundayHoliday = true;
                            }
                            // END BY DRCVHK
                        }
                        else { //nothing on this date; present day
                            $scope.preDate = "";
                        }
                    }
                }
                arrayTemp.length = 0;

                //check holiday/absent for after days
                $scope.endPostDate = $scope.postDate;
                //arrayList.push({ key: "P", date: $scope.endPostDate });
                $scope.IsLeaveOnLastEntry = false;
                if ($scope.IsFullDayLeave($scope.postDatePartFullTime) == true) {
                    if ($scope.IsWorkingDay($scope.endPostDate)) {
                        $scope.IsLeaveOnLastEntry = true;
                        arrayList.push({ key: "P", date: $scope.endPostDate });
                        $scope.postDate = dateAddDays($scope.postDate, 1);
                    }
                    //$scope.postDate = dateAddDays($scope.postDate, 1);

                    while ($scope.postDate != "") {
                        if ($scope.HasFestival($scope.postDate) == true) {
                            arrayTemp.push({ key: "P", date: $scope.postDate });
                            $scope.postDate = dateAddDays($scope.postDate, 1);
                        } else if ($scope.HasAbsent($scope.postDate) == true || $scope.HasPreLeave($scope.postDate) == true) {
                            $scope.endPostDate = $scope.postDate;
                            arrayList.push({ key: "P", date: $scope.endPostDate });
                            arrayList = arrayList.concat(arrayTemp);
                            arrayTemp.length = 0;// reset array
                            $scope.isLastSandwich = true;
                            $scope.postDate = dateAddDays($scope.postDate, 1);
                            // BEGIN BY DRCVHK
                            if ($scope.HasFestival(dateAddDays($scope.endPostDate, -1)) == true) {
                                $scope.isMiddleSundayHoliday = true;
                            }
                            // END BY DRCVHK
                        }
                        else { //nothing on this date; present day
                            $scope.postDate = "";
                        }
                    }
                } else {
                    arrayList.push({ key: "P", date: $scope.endPostDate });
                }
                arrayTemp.length = 0;

                //check for middle days
                for (var i = 1; i < $scope.rows.length - 1; i++) {
                    //check for next sunday and festival + entry in arrayTemp
                    var newDate = $scope.rows[i].LeaveDate;
                    if ($scope.HasFestival(newDate)) {
                        arrayTemp.push({ key: "P", date: newDate });
                    } else if ($scope.rows[i].PartFullTime == "P") {
                        arrayList.push({ key: "P", date: newDate });
                        arrayTemp.length = 0;
                    } else if ($scope.rows[i].PartFullTime == "F" && $scope.IsWorkingDay(newDate)) {
                        if ($scope.IsLeaveOnFirstEntry) {
                            arrayList = arrayList.concat(arrayTemp);
                        }
                        arrayList.push({ key: "P", date: newDate });
                        arrayTemp.length = 0;// reset array
                    }
                    if ($scope.IsLeaveOnFirstEntry == true && $scope.IsLeaveOnLastEntry == true && $scope.rows[$scope.rows.length - 1].LeaveDate == dateAddDays(newDate, 1)) {
                        arrayList = arrayList.concat(arrayTemp);
                        arrayTemp.length = 0;// reset array
                    }
                }
                arrayTemp.length = 0;// reset array

                /*SAVE DATA*/
                //if (($scope.isFirstSandwich == true || $scope.isLastSandwich == true) || ($scope.isMiddleSundayHoliday == true && $scope.IsLeaveOnFirstEntry == true && $scope.IsLeaveOnLastEntry == true)) {
                if (($scope.isFirstSandwich == true || $scope.isLastSandwich == true) && ($scope.isMiddleSundayHoliday == true && $scope.IsLeaveOnFirstEntry == true && $scope.IsLeaveOnLastEntry == true)) {
                    bootbox.confirm("Your leave is sandwich leave. Are you sure want to apply??", function (result) {
                        if (result) {
                            ProcessLeave(arrayList);
                        }
                    });
                } else {
                    ProcessLeave(arrayList);
                }
            } else { // mode == edit

                SaveLeave($scope.rows[0]).then(function (result) {
                    if (result.data.MessageType == 1) {
                        toastr.success(result.data.Message, 'Success');
                        $scope.Close();
                        $scope.LoadEvents($scope.filterData.EmployeeId, false); // load events in calendar
                    } else if (result.data.MessageType == 0) {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                    $rootScope.IsAjaxLoading = false;
                });
            }
        };

        function ProcessLeave(arrayList) {
            //remove duplicate entries
            var newarr = [];
            var unique = {};
            angular.forEach(arrayList, function (item) {
                if (!unique[item.date]) {
                    newarr.push(item);
                    unique[item.date] = item;
                }
            });

            //process and save each entries
            ProcessList(newarr, function (result) {
                $scope.Close();
                $scope.LoadEvents($scope.filterData.EmployeeId, false); // load events in calendar
                $scope.LoadLeaveAbsentList($scope.filterData.EmployeeId);// update leave and absent array list

                //send mail on save operation
                ApplyLeaveService.SendMail($scope.rows[0].Comments, $scope.filterData.EmployeeId, $scope.rows[0].LeaveDate, $scope.rows[$scope.rows.length - 1].LeaveDate).then(function (result) {
                    if (result.data.IsValidUser) {
                        toastr.success(result.data.Message, 'Success');
                    } else {
                        $rootScope.redirectToLogin();
                    }
                    $rootScope.IsAjaxLoading = false;
                });
            });
        };

        function ProcessList(arrayList, callback) {
            var index = 0, length = arrayList.length;
            function saveData() {
                var currentData = arrayList[index];
                var isMatched = false, line;
                angular.forEach($scope.rows, function (v, k) {
                    if (!isMatched) {
                        if (currentData.date == v.LeaveDate) {
                            isMatched = true;
                            line = $scope.rows[k];
                        }
                    }
                });
                if (!isMatched) {
                    //Chance to have [Absent, PreLeave, Festival]. Create line and send it to save
                    line = ({ LeaveTitle: "Sandwich Leave", LeaveType: 1, LeaveDate: currentData.date, PartFullTime: "F", StartTime: 1, EndTime: 48, Comments: "", isFestival: false });
                }

                SaveLeave(line).then(function (result) {
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

        function SaveLeave(line) {
            line.EmployeeId = $scope.filterData.EmployeeId;
            line.Mode = $scope.Mode;
            line.StartDate = FormatToDateObject(line.LeaveDate);
            line.LeaveTitle = line.LeaveTitle == "" ? "Sandwich Leave" : line.LeaveTitle;

            if (line.PartFullTime == "P") {
                line.StartTime = $scope.TimeArray[parseInt(line.StartTime) - 1].val;
                line.EndTime = $scope.TimeArray[parseInt(line.EndTime) - 1].val;
            }

            return ApplyLeaveService.SaveApplyLeave(line);
        };

        function FormatToDateObject(date) {
            var fDate = $filter('date')(date, 'dd-MM-yyyy').split('-');
            var fdt = new Date(parseInt(fDate[2]), parseInt(fDate[1]) - 1, parseInt(fDate[0]), 0, 0, 0);

            return $filter('date')(fdt, 'MM-dd-yyyy');
        }

        function dateAddDays( /*string dd-mm-yyyy*/ datstr, /*int*/ ndays) {
            var dattmp = datstr.split('-').reverse().join('-'), myDate = new Date(dattmp);
            var dayOfMonth = myDate.getDate();
            myDate.setDate(dayOfMonth + ndays);
            return $filter('date')(myDate, 'dd-MM-yyyy');
        }
    };
    // END MODAL INSTANCE CONTROLLER
})();


