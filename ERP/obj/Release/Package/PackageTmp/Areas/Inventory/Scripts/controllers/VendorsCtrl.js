/// <reference path="../libs/angular/angular.js" />


(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("VendorsCtrl", [
            "$scope", "$modal", "$rootScope", "$timeout", "VendorsService", "$filter", "ngTableParams",
            vendorsCtrl]);



    //Main controller function
    function vendorsCtrl($scope, $modal, $rootScope, $timeout, VS, $filter, ngTableParams) {

        //Initialize main data object
        $scope.editData = $scope.editData || {};

        /*reset the form*/
        $scope.reset = function () {
            $scope.vendor = $scope.lastVendor;
            if ($scope.vendor.VendorId) {
                $scope.EditVendor($scope.vendor);
            } else {
                $scope.clearControl();
                $scope.saveText = "Save";
                $scope.SetFocus();
            }
        };
        /*Clear all controls*/
        $scope.clearControl = function () {

            $scope.editData = {
                VendorId: 0,
                VendorName: '',
                CompanyName: '',
                Email: '',
                Website: '',
                Mobile: '',
                PhoneNo: '',
                //Services: [],
                Rating: 2,
                HouseNo: '',
                Location: '',
                Area: '',
                Country: "0",
                State: "0",
                City: '',
                PostalCode: '',
                IsActive: true,

            };

            $scope.lastVendor = {};
            //$scope.editData.VendorId = 0;
            //$scope.editData.VendorName = '';
            //$scope.editData.CompanyName = '';
            //$scope.editData.Email = '';
            //$scope.editData.Website = '';
            //$scope.editData.Mobile = '';
            //$scope.editData.PhoneNo = '';
            //$scope.editData.Services = [];
            //$scope.editData.Rating = 2;
            //$scope.editData.HouseNo = '';
            //$scope.editData.Location = '';
            //$scope.editData.Area = '';
            //$scope.editData.Country = 0;
            //$timeout(function () {
            //    $scope.editData.State = 0;
            //}, 50);
            //$scope.editData.City = '';
            //$scope.editData.PostalCode = '';
            //$scope.editData.IsActive = true;
            $scope.frmVendor.$setPristine();
            $scope.SetFocus();
        };

        $rootScope.isFormVisible = false;
        $scope.isFirstFocus = false;
        $scope.lastVendor = $scope.lastVendor || {};
        $scope.currentSelectedVendor = {};
        $scope.mode = "Add";
        $scope.saveText = "Save";
        $scope.timeZone = new Date().getTimezoneOffset().toString(); //timezone difference in minutes

        //Start Rating
        $scope.starRate = 2;
        $scope.starMax = 5;
        $scope.isStarRatingReadonly = false;



        //$scope.actions = {
        //    add: function () {

        //    },
        //    edit: function () {

        //    },
        //    del: function () {

        //    },
        //    clear: {

        //    }
        //};

        $scope.AddVendor = function () {
            $scope.mode = "Add";
            $scope.saveText = "Save";
            $rootScope.isFormVisible = true;
            $scope.clearControl();
        }
        // Get Vendors for Edit mode
        $scope.EditVendor = function (vendor) {
            $rootScope.isFormVisible = true;
            $scope.lastVendor = vendor;
            $scope.currentSelectedVendor = vendor;
            $scope.mode = "Edit";
            $scope.saveText = "Update";
            $scope.editData.VendorId = vendor.VendorId;
            $scope.editData.VendorName = vendor.VendorName;
            $scope.editData.CompanyName = vendor.CompanyName;
            $scope.editData.Email = vendor.Email;
            $scope.editData.Website = vendor.Website;
            $scope.editData.Mobile = vendor.Mobile;
            $scope.editData.PhoneNo = vendor.PhoneNo;
            var strService = vendor.Services.toString().split(',')
            $scope.editData.Services = strService;
            $scope.editData.Rating = vendor.Rating;
            $scope.editData.HouseNo = vendor.HouseNo;
            $scope.editData.Location = vendor.Location;
            $scope.editData.Area = vendor.Area;
            $scope.editData.Country = vendor.Country;
            $scope.GetStatesByCountry($scope.editData.Country, vendor.State);
            //$timeout(function () {
            //    $scope.editData.State = vendor.State;
            //}, 50);

            $scope.editData.City = vendor.City;
            $scope.editData.PostalCode = vendor.PostalCode;
            $scope.editData.IsActive = vendor.IsActive;
            $scope.frmVendor.$setPristine();
            $scope.SetFocus();
        }

        //Create And Update Vendor
        $scope.SaveVendor = function (vendor) {

            var newVendor = angular.copy(vendor);

            var temp_vendorService = vendor.Services;
            var temp = [];
            for (var i = 0 ; i < temp_vendorService.length; i++)
            { temp.push(temp_vendorService[i].text); }

            newVendor.Services = temp.toString();

            VS.SaveVendor(newVendor).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        //if (result.data === "true") {
                        toastr.success('Your data is successfully submitted', 'Success');
                        $scope.clearControl();
                        $scope.refreshTable();
                        $scope.isFirstFocus = false;
                        $scope.frmVendor.$setPristine();
                        if ($scope.mode == "Edit") {
                            $rootScope.isFormVisible = false;
                            $scope.saveText = "Save";
                        }
                        else { $scope.SetFocus(); }
                        //} else {
                        //    toastr.error('This entry is already exists or having some problem while entering the data', 'Opps, Something went wrong');
                        //}
                    } else if (result.data.MessageType == 2) { // 2:Warning
                        toastr.warning(result.data.Message, 'Record already exists');
                        $scope.SetFocus();
                    } else if (result.data.MessageType == 0) {//0:Error{
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
            });
        };

        /*delete Vendor*/
        $scope.DeleteVendor = function (vendor) {
            VS.DeleteVendor(vendor).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 0:Error
                        toastr.success('Your record is successfully deleted', 'Success');
                        $scope.refreshTable();
                    } else if (result.data.MessageType == 2) { // 1:Warning
                        toastr.warning(result.data.Message, 'Warning!');
                    } else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
            });
        }

        //Load All Vendors
        $scope.loadAllVendors = function () {
            // BEGIN GETTING LIST OF COUNTRIES
            VS.GetCountries().then(function (result) {
                $scope.Contries = result.data;
            });

            // BEGIN GETTING LIST OF VENDORS
            $scope.tableParams = new ngTableParams({
                page: 1,
                count: 10,  // Total No. of records per page
                sorting: {
                    VendorName: 'asc'
                },
                defaultSort: 'asc'
            }, {
                total: 0,
                filterDelay: 750,
                getData: function ($defer, params) {
                    //$rootScope.IsAjaxLoading = true;
                    VS.GetVendorList($scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter().VendorName, params.filter().CompanyName).success(function (result) {
                        if (result.IsValidUser) {
                            if (result.MessageType != 0) { // 0:Error
                                //display no data message
                                if (result.DataList.total == 0) {
                                    $scope.noRecord = true;
                                    // $scope.filterText = params.filter().VendorName;
                                } else {
                                    $scope.noRecord = false;
                                }
                                params.total(result.DataList.total);
                                $defer.resolve(result.DataList = result.DataList.result);
                                $rootScope.IsAjaxLoading = false;
                            } else {
                                toastr.error(result.Message, 'Opps, Something went wrong');
                            }
                        } else {
                            $rootScope.redirectToLogin();
                        }
                    });
                }
            });
        }

        /* Change the Status of Vendor */
        $scope.IsActive = function (vendor) {
            VS.IsActive(vendor).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        toastr.success('Status changed successfully.', 'Success');
                        $timeout(function () {
                            $scope.refreshTable();
                        }, 50);
                    } else if (result.data.MessageType == 2) { // 2:Warning
                        toastr.warning(result.data.Message, 'Warning!');
                    } else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
            });
        }

        /*cancel button click event*/
        $scope.cancel = function () {
            //$scope.editData = {};
            $scope.clearControl();
            $scope.mode = "Add";
            $rootScope.isFormVisible = false;
        }

        // For Exporting Excel
        $scope.ExportToExcel = function () {
            document.location.href = "../../Handler/Vendor.ashx?timezone=" + $scope.timeZone
        };

        /*refresh table after */
        $scope.refreshTable = function () {
            $scope.tableParams.reload();
        };

        // Reset Focus
        $scope.SetFocus = function () {
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        }

        $scope.GetCountries = function () {
            // BEGIN GETTING LIST OF COUNTRIES
            VS.GetCountries().then(function (result) {
                $scope.Contries = result.data;
            });
        }

        $scope.GetStatesByCountry = function (country, preState) {
            // BEGIN GETTING LIST OF States
            VS.GetStatesByCountry($scope.editData.Country).then(function (result) {
                $scope.States = result.data;
                $scope.editData.State = "0";
                if (!$scope.currentSelectedVendor.State) {
                    $scope.editData.State = "0";
                } else if ($scope.currentSelectedVendor.Country != $scope.editData.Country)
                { $scope.editData.State = "0"; } else
                {
                    $timeout(function () {
                        $scope.editData.State = $scope.currentSelectedVendor.State;
                    }, 0);
                }
            });
        }

        ///Model PopUp
        $scope.ShowVendor = function (vendor) {
            var modalInstance = $modal.open({
                templateUrl: 'VendorPopup.html',
                controller: ModalInstanceCtrl,
                resolve: {
                    vendorDetails: function () { return vendor; }
                }
            });
        }

        var ModalInstanceCtrl = function ($scope, $modalInstance, vendorDetails) {
            $scope.items = vendorDetails;
            $scope.Close = function () {
                $modalInstance.close();
            };

            //$scope.ClosePopUp = function () {
            //    $modalInstance.dismiss('cancel');
            //};
        };

    };
})();
