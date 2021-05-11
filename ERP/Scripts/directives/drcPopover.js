/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Directives")
    .directive('drcPopover', ["$compile", "$http", "$rootScope", function ($compile, $http, $rootScope) {
        //var numtype = "";
        //var getTemplate = function (numtype) {
        //    var templateLoader,
        //        baseUrl = "PartialTemplates/",
        //        templateMap = {
        //            "round": "looseparcelpopoverforRound.html",
        //            "heart": "looseparcelpopover.html",
        //        };
        //    var templateUrl = baseUrl + templateMap[numtype];
        //    templateLoader = $http.get(templateUrl, { cache: $templateCache });
        //    return templateLoader;
        //};


        return {
            scope: {
                title: "@",
                leavedate: "@",
                comments: "@",
                credate: "@",
                reasonlist: "@",
                haspermission: "@",
                status: "@",
                ispending: "@",
                callbackevent: "&",
                loginid: "@"
            },
            link: function (scope, element, attrs) {
                scope.leaveApproveReason = "";
                scope.reasonString = "";
                if (scope.reasonlist) {
                    scope.reasonString = "<hr/>";
                    angular.forEach(scope.reasonlist.split('|'), function (value, key) {
                        if (value) {
                            var reasonArray = value.split('$');
                            scope.reasonString = scope.reasonString +
                                "<div class='reason clearfix'>" +
                                (reasonArray[0] == "Approved" ? "<label class='label-approve'><strong>" + reasonArray[0] + " By:</strong> " + reasonArray[1] + "</label>"
                                                              : "<label class='label-disapprove'><strong>" + reasonArray[0] + " By:</strong> " + reasonArray[1] + "</label>") +
                                 "<div class='reason-txt'>" + reasonArray[2] + "</div>" +
                                 "<i class='reason-date'>" + reasonArray[3] + "</i>" +
                                 "</div><hr/>";

                        }
                    });
                }
                element.popover({
                    placement: 'bottom',
                    offset: 15,
                    trigger: 'manual',
                    delay: { show: 350, hide: 100 },
                    container: "body",
                    html: true,
                    content: function () {
                        scope.className = scope.status == "Approved" ? "btn-danger" : "btn-success";
                        var a1 = new Date(new moment(new Date()).format("YYYY-MM-DD"));
                        var a2 = new Date(new moment(attrs.leavedate, "DD-MM-YYYY").format("YYYY-MM-DD"));
                        var str = '<div class="popover-inner">' +
                                '<div class="reason comment-box"><strong>Comments</strong>' + scope.comments + "</div>" +
                               // "<i class='comment-date' style='margin: 0 10px;'>" + scope.chgdate + "</i>" +
                                "<i class='comment-date' style='margin: 0 10px;'>" + moment(scope.credate, "DD-MM-YYYY hh:mm:ss a").format("DD-MMM-YYYY hh:mm:ss a") + "</i>" + scope.reasonString;

                        if (a2 >= a1 || attrs.loginid == 1) {
                            str += '<div data-ng-show="' + attrs.haspermission + '" class="reason-form"><textarea class="form-control" placeholder="Reason" data-ng-model="leaveApproveReason" maxlength="150" rows="2"></textarea><br/>' +
                                '<button class="btn btn-xs" data-ng-class="className" data-ng-click="callback(false)">{{status == "Approved" ? "DisApprove" : "Approve"}}</button>' +
                                '&nbsp;<button class="btn btn-danger btn-xs" data-ng-click="callback(true)" data-ng-show="ispending">DisApprove</button>' +
                                '</div>';
                        }

                        str += '</div>';
                        return str;
                    }
                });


                scope.callback = function (approved) {
                    if (approved) {
                        scope.isApproved = true;
                    } else {
                        scope.isApproved = scope.status == "Approved" ? true : false;
                    }
                    $(element).popover('hide');
                    scope.callbackevent({ date: scope.leavedate, approved: scope.isApproved, reason: scope.leaveApproveReason });
                };

                var timer, popover_parent;
                function hidePopover(elem) {
                    $(elem).popover('hide');
                };

                element.hover(function () {
                    var self = this;
                    clearTimeout(timer);
                    $('.popover').hide(); //Hide any open popovers on other elements.
                    popover_parent = self
                    $(self).popover('show', function () {
                    });
                    $rootScope.$safeApply(scope, function () {
                        $compile($('.popover').contents())(scope);
                    });
                },
                function () {
                    var self = this;
                    timer = setTimeout(function () { hidePopover(self) }, 300);
                });

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