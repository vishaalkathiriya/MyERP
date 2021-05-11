/// <reference path="../libs/angular/angular.min.js" />


(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("VisitorMasterCtrl", [
            "$scope", "$rootScope", "$timeout", "VisitorMasterService", "$http", "$filter", "ngTableParams", "$q",
            VisitorMasterCtrl
        ]);


    //Main controller function
    function VisitorMasterCtrl($scope, $rootScope, $timeout, VisitorMasterService, $http, $filter, ngTableParams, $q) {
        $scope.editData = $scope.editData || {};
        $scope.master = $scope.master || {};
        $scope.timeZone = new Date().getTimezoneOffset().toString(); //timezone difference in minutes
        $scope.isFocusFirst = false;
        $scope.editData.VisitorImg = "";
        $scope.saveText = "Save";
        $scope.storage = { lastRecord: "" };

        $scope.editData = {
            MobileNo: "",
            VisitorID: "",
            FullName: "",
            BlockNo: "",
            Soc: "",
            Street1: "",
            Street2: "",
            comapny: "",
            Designation: "",
            photo_url: ""
        }

        /*reset the form*/
        $scope.ResetVisitor = function () {
            if ($scope.master.isOuterTabActive == true) {
                $scope.editData = {
                    MobileNo: "",
                    VisitorID: "",
                    FullName: "",
                    BlockNo: "",
                    Soc: "",
                    Street1: "",
                    Street2: "",
                    comapny: "",
                    Designation: "",
                    photo_url: ""
                }

                $scope.editData.VisitorID = "";
                $scope.editData.ecode = "";
                $scope.editData.RefName = "";
                $scope.editData.Dept = "";
                $scope.editData.Manager = "";
                $scope.editData.ExtNo = "";

                $("#gallery").html('');
                $("#VisitorPhoto").html('');
                $("#visitorNameLable").html('');
                document.getElementById("txtMobileNo").readOnly = false;
                document.getElementById("txtMobileNo").focus();
            }
            else if ($scope.master.isInterTabActive == true) {
                $scope.editData.ecode = "0";
                $scope.editData.VisitorID = "";
                //$scope.editData.ecode = "";
                $scope.editData.RefName = "";
                $scope.editData.Dept = "";
                $scope.editData.Manager = "";
                $scope.editData.ExtNo = "";

                Reason: $scope.editData.Reason = "";
                Person: $scope.editData.TotalPerson = "";
                //$("#VisitorPhoto").html('');
                //$("#visitorNameLable").html('');
                document.getElementById("drpEcode").focus();
            }
            else if ($scope.master.isReportTabActive == true) {
                $scope.tableParams.$params.filter = {
                    MobileNo: "", Name: "", Designation: "", Company: "", Address: ""
                };
            }
            //$scope.employeedetailreportform.$setPristine();
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        }



        /* getting list of Depts*/
        function loadComapny() {

            VisitorMasterService.RetrieveCompany().then(function (result) {
                $scope.CompanyList = result.data.DataList;
            });
        };
        loadComapny();


        function loadDesignation() {
            VisitorMasterService.RetrieveDesignation().then(function (result) {
                $scope.DesignationList = result.data.DataList;
            });
        };
        loadDesignation();

        $scope.suggestionsArray = [];
        function loadEcode() {
            //$scope.suggestionsArray = new Array();
            VisitorMasterService.RetrieveEcode().then(function (result) {
                $scope.EcodeList = result.data.DataList;

                for (var i = 0; i < $scope.EcodeList.length; i++) {
                    $scope.EcodeList[i].Ecode = $scope.EcodeList[i].Ecode.toString();
                    $scope.suggestionsArray.push($scope.EcodeList[i].Ecode.toString());
                }
            });
        };
        loadEcode();

        //end ecode
        function loadRefName() {
            VisitorMasterService.RetrieveRefName().then(function (result) {

                $scope.RefNameList = result.data.DataList;
            });
        };
        loadRefName();

        function loadManger() {
            VisitorMasterService.RetrieveManager().then(function (result) {
                $scope.ManagerList = result.data.DataList;
            });
        };
        loadManger();

        function loadDept() {
            VisitorMasterService.RetrieveDept().then(function (result) {
                $scope.DeptList = result.data.DataList;
            });
        };
        loadDept();

        function encode(data) {
            var str = String.fromCharCode.call(null, data);
            return btoa(str).replace(/.{76}(?=.)/g, '$&\n');
        }

        $scope.GetData = function () {

            if ($scope.editData.MobileNo != null) {
                VisitorMasterService.GetVisitorData($scope.editData.MobileNo).then(function (result) {

                    $scope.VisitorData = result.data.DataList;
                    $scope.editData.VisitorID = "",
                    $scope.editData.FullName = "",
                    $scope.editData.BlockNo = "",
                    $scope.editData.Soc = "",
                    $scope.editData.Street1 = "",
                    $scope.editData.Street2 = "",
                    $scope.editData.comapny = "",
                    $scope.editData.Designation = ""
                    if ($scope.VisitorData) {

                        $scope.editData.MobileNo = $scope.VisitorData[0].MobileNo;
                        $scope.editData.VisitorID = $scope.VisitorData[0].VisitorId;
                        $scope.editData.FullName = $scope.VisitorData[0].Name;

                        var addressData = $scope.VisitorData[0].Address.split(",");
                        if (addressData.length > 0) {
                            $scope.editData.BlockNo = addressData[0];
                        }
                        if (addressData.length > 1) {
                            $scope.editData.Soc = addressData[1];
                        }
                        if (addressData.length > 2) { $scope.editData.Street1 = addressData[2]; }
                        if (addressData.length > 3) { $scope.editData.Street2 = addressData[3]; }

                        $scope.editData.comapny = $scope.VisitorData[0].Company;
                        $scope.editData.Designation = $scope.VisitorData[0].Designation;
                        $scope.editData.Remark = $scope.VisitorData[0].Remark;
                        $scope.master.isInterTabActive = true;

                        var imageStr = encode($scope.VisitorData[0].Photo);
                        $("#gallery").html('');
                        $("#gallery").append('<img  src="' + "data:image/png;base64," + $scope.VisitorData[0].Photo1 + '" >');
                        $("#VisitorPhoto").html('');
                        $("#VisitorPhoto").append('<img  src="' + "data:image/png;base64," + $scope.VisitorData[0].Photo1 + '" >');
                        $("#visitorNameLable").html('');
                        $("#visitorNameLable").append("Name:");
                        $("#visitorNameLable").append($scope.VisitorData[0].Name)

                        $scope.saveText = "Update";

                    }
                    else {
                        $scope.master.isOuterTabActive = true;
                        document.getElementById("txtFullName").focus();
                        $scope.editData.VisitorID = "",
                        $scope.editData.FullName = "",
                        $scope.editData.BlockNo = "",
                        $scope.editData.Soc = "",
                        $scope.editData.Street1 = "",
                        $scope.editData.Street2 = "",
                        $scope.editData.comapny = "",
                        $scope.editData.Designation = "",
                        $scope.editData.photo_url = ""
                        $scope.saveText = "Save";

                        toastr.info("", 'Sorry..No Data Available for this Mobile Number');
                    }
                });

            }

        }

        $scope.RefreshTable = function () {
            $scope.tableParams.reload();
        };

        /*delete Visitor*/
        $scope.DeleteVisitorData = function (d) {
            $rootScope.IsAjaxLoading = true;
            VisitorMasterService.DeleteVisitorData(d).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) {  //1:Success
                        toastr.success(result.data.Message, 'Success');
                        $scope.RefreshTable();
                    } else if (result.data.MessageType == 2) { // 2:Warning
                        toastr.warning(result.data.Message, 'Warning!');
                    } else {  // 0:Error
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
                $rootScope.IsAjaxLoading = false;
            });
        };

        $scope.GetVisitorData_Table = function () {
            $scope.tableParams = new ngTableParams({
                page: 1,
                count: 100,
                sorting: {
                    VisitorID: 'asc'
                },
                defaultSort: 'asc'
            }, {
                total: 0,
                filterDelay: 750,
                getData: function ($defer, params) {
                    $rootScope.IsAjaxLoading = true;
                    $scope.isRecord = false;
                    VisitorMasterService.GetVisitorDetail($scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter().VisitorId, params.filter().MobileNo, params.filter().Name, params.filter().Address, params.filter().Company, params.filter().Designation).then(function (result) {
                        if (result.data.IsValidUser) {
                            if (result.data.MessageType != 0) { // 0:Error
                                //display no data message
                                if (result.data.DataList.total == 0) {
                                    $scope.noRecord = true;
                                } else {
                                    $scope.noRecord = false;
                                }
                                params.total(result.data.DataList.total);
                                $scope.totalRecords = result.data.DataList.total;
                                $defer.resolve($scope.documents = result.data.DataList.result);
                            } else {
                                toastr.error(result.data.Message, 'Opps, Something went wrong');
                            }
                        } else {
                            $rootScope.redirectToLogin();
                        }
                        $rootScope.IsAjaxLoading = false;
                    });
                }
            });
        };



        $scope.GetDataByVisitorID = function () {
            if ($scope.editData.VisitorID != null) {
                VisitorMasterService.GetVisitorDataByVisitorID($scope.editData.VisitorID).then(function (result) {
                    $scope.VisitorData = result.data.DataList;
                    if ($scope.VisitorData) {

                        $scope.editData.MobileNo = $scope.VisitorData[0].MobileNo;
                        $scope.editData.VisitorID = $scope.VisitorData[0].VisitorId;
                        $scope.editData.FullName = $scope.VisitorData[0].Name;

                        var addressData = $scope.VisitorData[0].Address.split("{}");
                        if (addressData.length > 0) {
                            $scope.editData.BlockNo = addressData[0];
                        }
                        if (addressData.length > 1) {
                            $scope.editData.Soc = addressData[1];
                        }
                        if (addressData.length > 2) { $scope.editData.Street1 = addressData[2]; }
                        if (addressData.length > 3) { $scope.editData.Street2 = addressData[3]; }

                        $scope.editData.comapny = $scope.VisitorData[0].Company;
                        $scope.editData.Designation = $scope.VisitorData[0].Designation;
                        $scope.master.isInterTabActive = true;
                    }
                    //toastr.info("", 'Sorry..No Data Available for this Visitor');
                });
            }

        }



        /*save module*/
        $scope.CreateUpdateVisitor = function (mod) {
            var postData = {
                VisitorId: $scope.editData.VisitorID,
                MobileNo: $scope.editData.MobileNo,
                Name: $scope.editData.FullName,
                Address: $scope.editData.BlockNo + ',' + $scope.editData.Soc + ',' + $scope.editData.Street1 + ',' + $scope.editData.Street2,
                Company: $scope.editData.comapny,
                Designation: $scope.editData.Designation,
                Remark: $scope.editData.Remark,
                Photo: window.visitorphotourl,
                PDate: null

            }
            VisitorMasterService.CreateUpdateVisitor(postData).then(function (result) {
                if (result.data.IsValidUser) {
                    var type = result.data.MessageType;
                    if (result.data.MessageType == 1) { // 1:Success
                        toastr.success(result.data.Message, 'Success');

                        $scope.GetData();
                        $scope.GetInpersonList();
                        $scope.master.isInterTabActive = true;
                        $scope.storage.lastRecord = {};
                        $scope.isFirstFocus = false;

                        if ($scope.mode === "Edit") {
                            $rootScope.isFormVisible = false;
                            $scope.saveText = "Save";
                        }

                    } else if (result.data.MessageType == 2) { // 2:Warning
                        toastr.warning(result.data.Message, 'Record already exists');
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

        $scope.visitorIn = function (indata) {
            var Indata = {
                SrNo: null,
                VisitorId: $scope.editData.VisitorID,
                ECode: $scope.editData.ecode,
                RefName: $scope.editData.RefName,
                Department: $scope.editData.Dept,
                Manager: $scope.editData.Manager,
                ExtNo: $scope.editData.ExtNo,
                InTime: null,
                OutTime: null,
                Reason: $scope.editData.Reason,
                Person: $scope.editData.TotalPerson,
            }


            VisitorMasterService.VisitorIn(Indata, $scope.editData.MobileNo).then(function (result) {
                if (result.data.IsValidUser) {
                    var type = result.data.MessageType;
                    if (result.data.MessageType == 1) { // 1:Success
                        toastr.success(result.data.Message, 'Success');
                        loadEcode();

                        $scope.editData = {

                            MobileNo: "",
                            VisitorID: "",
                            FullName: "",
                            BlockNo: "",
                            Soc: "",
                            Street1: "",
                            Street2: "",
                            comapny: "",
                            Designation: "",
                            photo_url: "",
                            Ecode: "0"

                        }

                        $scope.GetInpersonList();

                        //document.getElementById("txtMobileNo").readOnly = false;
                        $("#gallery").html('');
                        $("#VisitorPhoto").html('');
                        $("#visitorNameLable").html('');
                        $scope.ResetVisitor();

                        $scope.isFirstFocus = false;

                        $scope.editData.VisitorID = "";
                        $scope.editData.ecode = "";
                        $scope.editData.RefName = "";
                        $scope.editData.Dept = "";
                        $scope.editData.Manager = "";
                        $scope.editData.ExtNo = "";

                        Reason: $scope.editData.Reason = "";
                        Person: $scope.editData.TotalPerson = "";


                        //$scope.exnumberform.$setPristine();

                    } else if (result.data.MessageType == 2) { // 2:Warning
                        toastr.warning(result.data.Message, 'Record already exists');
                    }
                    else {
                        $scope.editData.ecode = "";
                        $scope.editData.RefName = "";
                        $scope.editData.Dept = "";
                        $scope.editData.Manager = "";
                        $scope.editData.ExtNo = "";

                        Reason: $scope.editData.Reason = "";
                        Person: $scope.editData.TotalPerson = "";
                        toastr.error(result.data.Message, 'Opps, Something went wrong');

                    }
                } else {
                    $rootScope.redirectToLogin();
                }
                $rootScope.IsAjaxLoading = false;
            });
        }


        $scope.$watch('master.isInterTabActive', function (newValue) {
            if (newValue == true) {
                $scope.GetInpersonList();

            }
        })

        $scope.$watch('master.isReportTabActive', function (newValue) {
            if (newValue == true) {

                $scope.RefreshTable();
            }
        })

        $scope.GetInpersonList = function () {

            VisitorMasterService.GetInpersonList().then(function (result) {
                $scope.InPersonList = result.data.DataList;
            });
        }
        $scope.GetInpersonList();
        $scope.VisitorOut = function (d) {
            VisitorMasterService.VisitorOut(d.SrNo, d.RefName, d.ECode, d.VisitorId).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 0) { // 0:Error
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                    else {
                        toastr.success(result.data.Message, 'Success');
                        $scope.GetInpersonList();
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
                $rootScope.IsAjaxLoading = false;
            });


        }

        $scope.GetMeIn = function (d) {

            $scope.editData = {

                MobileNo: "",
                VisitorID: "",
                FullName: "",
                BlockNo: "",
                Soc: "",
                Street1: "",
                Street2: "",
                comapny: "",
                Designation: "",
                photo_url: "",


            }


            $("#VisitorPhoto").html('');
            $("#gallery").html('');
            $scope.master.isInterTabActive = true;
            $scope.editData.MobileNo = d.MobileNo;
            $scope.editData.VisitorID = d.VisitorId;
            $("#visitorNameLable").html('');
            $("#visitorNameLable").append("Name:");
            $("#visitorNameLable").append(d.Name)


            $scope.saveText = "Update";
            $scope.editData.MobileNo = d.MobileNo;
            $scope.editData.VisitorID = d.VisitorId;
            $scope.editData.FullName = d.Name;

            var addressData = d.Address.split(",");
            if (addressData.length > 0) {
                $scope.editData.BlockNo = addressData[0];
            }
            if (addressData.length > 1) {
                $scope.editData.Soc = addressData[1];
            }
            if (addressData.length > 2) { $scope.editData.Street1 = addressData[2]; }
            if (addressData.length > 3) { $scope.editData.Street2 = addressData[3]; }

            $scope.editData.comapny = d.Company;
            $scope.editData.Designation = d.Designation;
            $scope.editData.Remark = d.Remark;

            VisitorMasterService.GetVisitorData(d.MobileNo).then(function (result) {
                $scope.UpdateData = result.data.DataList;


                var imageStr = encode($scope.UpdateData[0].Photo);
                // $("#gallery").html('');
                $("#gallery").append('<img  src="' + "data:image/png;base64," + $scope.UpdateData[0].Photo1 + '" >')
                // $("#VisitorPhoto").html('');
                $("#VisitorPhoto").append('<img  src="' + "data:image/png;base64," + $scope.UpdateData[0].Photo1 + '" >');


            });


        }


        $scope.UpdateVisitor = function (d) {
            $scope.editData = {
                MobileNo: "",
                VisitorID: "",
                FullName: "",
                BlockNo: "",
                Soc: "",
                Street1: "",
                Street2: "",
                comapny: "",
                Designation: "",
                photo_url: ""
            }
            $("#gallery").html('');
            $("#VisitorPhoto").html('');
            $("#visitorNameLable").html('');
            $("#visitorNameLable").append("Name:");
            $("#visitorNameLable").append(d.Name)
            $scope.master.isOuterTabActive = true;
            $scope.saveText = "Update";
            $scope.editData.MobileNo = d.MobileNo;
            $scope.editData.VisitorID = d.VisitorId;
            $scope.editData.FullName = d.Name;

            var addressData = d.Address.split(",");
            if (addressData.length > 0) {
                $scope.editData.BlockNo = addressData[0];
            }
            if (addressData.length > 1) {
                $scope.editData.Soc = addressData[1];
            }
            if (addressData.length > 2) { $scope.editData.Street1 = addressData[2]; }
            if (addressData.length > 3) { $scope.editData.Street2 = addressData[3]; }

            $scope.editData.comapny = d.Company;
            $scope.editData.Designation = d.Designation;
            $scope.editData.Remark = d.Remark;

            VisitorMasterService.GetVisitorData(d.MobileNo).then(function (result) {
                $scope.UpdateData = result.data.DataList;


                var imageStr = encode($scope.UpdateData[0].Photo);
                $("#gallery").html('');
                $("#gallery").append('<img  src="' + "data:image/png;base64," + $scope.UpdateData[0].Photo1 + '" >')
                $("#VisitorPhoto").html('');
                $("#VisitorPhoto").append('<img  src="' + "data:image/png;base64," + $scope.UpdateData[0].Photo1 + '" >');
                window.visitorphotourl = $scope.UpdateData[0].Photo1;

            });


        }
    };

})();

