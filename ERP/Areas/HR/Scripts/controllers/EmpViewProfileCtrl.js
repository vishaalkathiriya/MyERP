/// <reference path="../libs/angular/angular.min.js" />

(function () {

    'use strict';

    angular.module("ERPApp.Controllers")
        .controller("EmployeeViewProfile", [
             "$scope", "$rootScope", "$timeout", "$http", "$filter", "EmployeeCreateService", "ngTableParams", "$modal",
            EmployeeViewProfile
        ]);

    function EmployeeViewProfile($scope, $rootScope, $timeout, $http, $filter, EmployeeCreateService, ngTableParams, $modal) {

        $scope.$watch('editdata.ProfilePhoto', function (newValue) {

            if (newValue == null || newValue == 'undefined' || newValue == '') {
                return false
            }

            $scope.editdata.ProfilePhoto = newValue;

            EmployeeCreateService.EditProfilePhoto(newValue, $scope.timeZone).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        //            toastr.success(result.data.Message, 'Success');
                        $scope.editdata.isProfilePicVisible = false;
                    } else if (result.data.MessageType == 2) { // 2:Warning
                        toastr.warning(result.data.Message, 'Record already exists');
                    } else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                }
                else {
                    $rootScope.redirectToLogin();
                }
                $rootScope.IsAjaxLoading = false;
            });
        });


        $scope.showHideProfilePic = function () {
            if (!$scope.editdata.isProfilePicVisible) {
                $scope.editdata.isProfilePicVisible = true;
            }
            else {
                $scope.editdata.isProfilePicVisible = false;
            }
        }

        $scope.RetrieveEmpProfile = function () {
            $rootScope.IsAjaxLoading = true;

            EmployeeCreateService.RetrieveUserProfile().then(function (result) {
               

                if (result.data.IsValidUser) {
                    if (result.data.MessageType != 0) {
                        if (result.data.DataList.total == 0) {
                            $scope.noRecord = true;
                        } else {

                            $scope.editdata = {
                                EmployeeRegisterCode: result.data.DataList.EmployeeRegisterCode,
                                CandidateFirstName: result.data.DataList.CandidateFirstName,
                                // CandidateMiddleName: (result.data.DataList.CandidateMiddleName).substr(0, 1),
                                CandidateMiddleName: result.data.DataList.CandidateMiddleName,
                                CandidateLastName: result.data.DataList.CandidateLastName,
                                BirthDate: result.data.DataList.BirthDate,
                                Gender: result.data.DataList.Gender,
                                ProfilePhoto: result.data.DataList.ProfilePhoto,
                                Present_HouseNo: result.data.DataList.Present_HouseNo,
                                Present_Location: result.data.DataList.Present_Location,
                                Present_Area: result.data.DataList.Present_Area,
                                Present_Country: result.data.DataList.Present_Country,
                                Present_State: result.data.DataList.Present_State,
                                Present_City: result.data.DataList.Present_City,
                                Present_PostalCode: result.data.DataList.Present_PostalCode,
                                Permanent_HouseNo: result.data.DataList.Permanent_HouseNo,
                                Permanent_Location: result.data.DataList.Permanent_Location,
                                Permanent_Area: result.data.DataList.Permanent_Area,
                                Permanent_Country: result.data.DataList.Permanent_Country,
                                Permanent_State: result.data.DataList.Permanent_State,
                                Permanent_City: result.data.DataList.Permanent_City,
                                Permanent_PostalCode: result.data.DataList.Permanent_PostalCode,
                                CompanyEmailId: result.data.DataList.CompanyEmailId,
                                CompanyMobile: result.data.DataList.CompanyMobile,
                                PersonalEmailId: result.data.DataList.PersonalEmailId,
                                PersonalMobile: result.data.DataList.PersonalMobile,
                                NomineeMobile: result.data.DataList.NomineeMobile,
                                MaritalStatus: result.data.DataList.MaritalStatus,
                                MarriageAnniversaryDate: result.data.DataList.MarriageAnniversaryDate,
                                DrivingLicenceNumber: result.data.DataList.DrivingLicenceNumber,
                                PANCardNumber: result.data.DataList.PANCardNumber,
                                AdharNumber: result.data.DataList.AdharNumber,
                                PassportNumber: result.data.DataList.PassportNumber,
                                BloodGroup: result.data.DataList.BloodGroup,
                                GuardianFirstName: result.data.DataList.GuardianFirstName,
                                // GuardianMiddleName: (result.data.DataList.GuardianMiddleName).substr(0, 1),
                                GuardianMiddleName: result.data.DataList.GuardianMiddleName,
                                GuardianLastName: result.data.DataList.GuardianLastName,
                                CompanyBankAccount: result.data.DataList.CompanyBankAccount,
                                CompanyBankName: result.data.DataList.CompanyBankName,
                                JoiningDate: result.data.DataList.JoiningDate,
                                PassportExpiryDate: result.data.DataList.PassportExpiryDate

                            }
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
    }
})();