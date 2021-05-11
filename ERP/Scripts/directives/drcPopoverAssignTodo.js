/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Directives")
    .directive('drcPopoverAssignTodo', ["$compile", "$http", "$rootScope", "$timeout", "PMSModuleService", function ($compile, $http, $rootScope, $timeout, PMSModuleService) {
        return {
            scope: {
                username: "@",
                date: "=",
                projectid: "@",
                todoid: "@",
                userid: "=",
                userlist: "=",
                iscanfinish: "=",
                assignuser:"=",
                loginid: "=",
                hidecanfinish:"@",
                callbackevent: "&"
            },
            restrict: 'A',
            link: function (scope, element, attrs) {
                scope.currentDate = new Date();
                element.popover({
                    placement: get_popover_placement,
                    offset: 15,
                    trigger: 'manual',
                    container: "body",
                    html: true,
                    title: scope.username == "UnAssigned" ? "<strong>Assign User</strong>" : "<strong>" + scope.username + "</strong>",
                    content: function () {
                        scope.dt = scope.date;
                        scope.uid = scope.userid;
                        scope.iscanfinish = scope.iscanfinish;
                        if (scope.assignuser == 0) {
                            scope.iscanfinish = true;
                        }

                        if (scope.loginid == scope.uid && scope.iscanfinish == false && scope.loginid != 1)
                        {
                            scope.hidecanfinish = false;
                        } else {
                            scope.hidecanfinish = true;
                        }

                        return '<div class="popover-inner">' +
                                '<div class="row">' +
                                '<div class="col-sm-12"><div class="form-group"><label>Assign User</label>' +
                                '<select class="form-control" name="drpUserList" data-ng-model="uid" ng-options="a.Id as a.Label for a in userlist" required>' +
                                    '<option style="display: none" value="">--Select--</option>' +
                                '</select>' +
                                '</div></div>' +
                                '</div>' +

                                '<div class="row">' +
                                '<div class="col-sm-12"><div class="form-group"><label>Select DueDate</label>' +
                                '<div ng-model="dt"><datepicker min="currentDate" show-weeks="showWeeks"></datepicker></div>' +
                                '</div></div>' +
                                '</div>' +

                                '<div class="row" data-ng-show="(uid != 0 || loginid == 1 ) && hidecanfinish ">' +
                                '<div class="col-sm-12"><div class="form-group"><input type="checkbox" name="iscanfinish" data-ng-model="iscanfinish" data-ng-checked="iscanfinish"> can finish' +
                                '</div></div></div>' +

                                '<div class="row">' +
                                '<div class="col-sm-12" style="text-align:center;">' +
                                '<button class="btn btn-sm btn-danger" data-ng-click="removeDueDate()">No Due Date</button>&nbsp;' +
                                '<button class="btn btn-sm btn-success" data-ng-click="callback(uid, dt)">Assign</button>&nbsp;' +
                                '<button class="btn btn-sm btn-info" data-ng-click="hidePopover()">Cancel</button>' +
                                '</div></div>' +
                                '</div>';
                    }
                });

                function get_popover_placement(pop, dom_el) {
                    var width = window.innerWidth;
                    if (width < 500) return 'bottom';
                    var left_pos = $(dom_el).offset().left;
                    if (width - left_pos > 400) return 'right';
                    return 'left';
                }

                scope.hidePopover = function () {
                    $(element).popover('hide');
                };
                scope.removeDueDate = function () {
                    scope.dt = "";
                };

                scope.callback = function (id, calDate) {
                    scope.callbackevent({ date: calDate, userId: id, projectId: scope.projectid, todoId: scope.todoid, iscanfinish: scope.iscanfinish });
                    $(element).popover('hide');
                };

                var timer, popover_parent;
                function hidePopover(elem) {
                    $(elem).popover('hide');
                };

                element.click(function (e) {
                    var self = this;
                    clearTimeout(timer);
                    $('.popover').hide(); //Hide any open popovers on other elements.
                    popover_parent = self
                    $(self).popover('show', function () {

                    });
                    $rootScope.$safeApply(scope, function () {
                        $compile($('.popover').contents())(scope);
                    });
                });
                //,
                //function () {
                //    var self = this;
                //    timer = setTimeout(function () { hidePopover(self) }, 300);
                //});

                $(document).on({
                    mouseenter: function () {
                        clearTimeout(timer);
                    },
                    mouseleave: function () {
                        var self = this;
                        timer = setTimeout(function () { hidePopover(popover_parent) }, 300);
                    }
                }, '.popover');
            }
        }
    }]);