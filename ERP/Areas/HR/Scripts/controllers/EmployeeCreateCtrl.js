/// <reference path="../libs/angular/angular.min.js" />
(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("EmployeeCreateCtrl", [
            "$scope","$modal", "$rootScope", "$timeout", "EmployeeCreateService", "$http", "$filter",
            employeeCreateCtrl
        ]);

    //Main controller function
    function employeeCreateCtrl($scope,$modal, $rootScope, $timeout, EmployeeCreateService, $http, $filter) {
        $scope.editData = $scope.editData || {};
        $scope.master.Mode = "Add";
        $scope.saveText = "Save";
        $scope.timeZone = new Date().getTimezoneOffset().toString(); //timezone difference in minutes
        $scope.isFirstFocus = true;
        $scope.storage = { lastRecord: "" };
        $scope.editData.IsActive = true;
        $scope.editData.Present_Country = 0;
        $scope.editData.Permanent_Country = 0;
        $scope.editData.Present_State = 0;
        $scope.editData.Permanent_State = 0;
        $scope.fileName = "";
        $scope.isMasterActive = true;

        $scope.OriginPath = window.Origin;
        $scope.tempPath = window.temp;
        $scope.tempEdit = true;
        $scope.ImagePath = $scope.tempPath;
        $scope.isChanged = false;

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

        $scope.calendarOpenBirthDate = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.editData.calOpenMrgDate = false;
            $scope.editData.calOpenPassExpDate = false;
            $scope.editData.calOpenBirthDate = true;
            
        };
        $scope.calendarOpenMrgDate = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.editData.calOpenBirthDate = false;
            $scope.editData.calOpenPassExpDate = false;
            $scope.editData.calOpenMrgDate = true;
            
        };
        $scope.calendarOpenPassExpDate = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.editData.calOpenBirthDate = false;
            $scope.editData.calOpenMrgDate = false;
            $scope.editData.calOpenPassExpDate = true;
        };

        $scope.$watch('editData.birthDate', function (newValue) {
            $scope.editData.BirthDate = $filter('date')(newValue, 'dd-MM-yyyy');
        });
        $scope.$watch("editData.marriageAnniversaryDate", function (newValue) {
            $scope.editData.MarriageAnniversaryDate = $filter('date')(newValue, 'dd-MM-yyyy');
        });
        $scope.$watch('editData.passportExpiryDate', function (newValue) {
            $scope.editData.PassportExpiryDate = $filter('date')(newValue, 'dd-MM-yyyy');
        });

        $scope.ValidateBirthDate = function (bDate) {
            if (!bDate) {
                $scope.empform.txtDOB.$setValidity("invalidBirthDate", true);
                return;
            } else if (bDate.length == 10) {
                if ($scope.ValidateDate(bDate)) {
                    if ($scope.IsGreterThanToday(bDate)) {
                        $scope.editData.BirthDate = $filter('date')(new Date(), 'dd-MM-yyyy');
                        $scope.editData.birthDate = $scope.StringToDateString($filter('date')(new Date(), 'dd-MM-yyyy'));
                    } else {
                        var dt = bDate.split('-');
                        $scope.editData.birthDate = dt[2] + "-" + dt[1] + "-" + dt[0] + "T00:00:00";
                        $scope.empform.txtDOB.$setValidity("invalidBirthDate", true);
                    }
                } else {
                    $scope.empform.txtDOB.$setValidity("invalidBirthDate", false);
                }
            } else {
                $scope.empform.txtDOB.$setValidity("invalidBirthDate", false);
            }
        };

        $scope.ValidateMrgAnniversaryDate = function (mDate) {
            if (!mDate) {
                $scope.empform.txtMarriageAnniversaryDate.$setValidity("invalidMrgAnniDate", true);
                return;
            } else if (mDate.length == 10) {
                if ($scope.ValidateDate(mDate)) {
                    var dt = mDate.split('-');
                    $scope.editData.marriageAnniversaryDate = dt[2] + "-" + dt[1] + "-" + dt[0] + "T00:00:00";
                    $scope.empform.txtMarriageAnniversaryDate.$setValidity("invalidMrgAnniDate", true);
                } else {
                    $scope.empform.txtMarriageAnniversaryDate.$setValidity("invalidMrgAnniDate", false);
                }
            } else {
                $scope.empform.txtMarriageAnniversaryDate.$setValidity("invalidMrgAnniDate", false);
            }
        };

        $scope.ValidatePassExpDate = function (pDate) {
            if (!pDate) {
                $scope.empform.txtPassportExpiryDate.$setValidity("invalidPassExpDate", true);
                return;
            } else if (pDate.length == 10) {
                if ($scope.ValidateDate(pDate)) {
                    var dt = pDate.split('-');
                    $scope.editData.passportExpiryDate = dt[2] + "-" + dt[1] + "-" + dt[0] + "T00:00:00";
                    $scope.empform.txtPassportExpiryDate.$setValidity("invalidPassExpDate", true);
                } else {
                    $scope.empform.txtPassportExpiryDate.$setValidity("invalidPassExpDate", false);
                }
            } else {
                $scope.empform.txtPassportExpiryDate.$setValidity("invalidPassExpDate", false);
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
        // END DATE PICKER

        /*validate dropdown*/
        $scope.validateDropPreCountry = function () {
            if ($scope.editData.Present_Country && $scope.editData.Present_Country != 0) return false;
            return true;
        };
        $scope.validateDropPreState = function () {
            if ($scope.editData.Present_State && $scope.editData.Present_State != 0) return false;
            return true;
        };
        $scope.validateDropPerCountry = function () {
            if ($scope.editData.Permanent_Country && $scope.editData.Permanent_Country != 0) return false;
            return true;
        };
        $scope.validateDropPerState = function () {
            if ($scope.editData.Permanent_State && $scope.editData.Permanent_State != 0) return false;
            return true;
        };
        $scope.validateDropGender = function () {
            if ($scope.editData.Gender && $scope.editData.Gender != 0) return false;
            return true;
        };

        /* getting list of blood group*/
        $scope.getBloodGroupList = function () {
            return EmployeeCreateService.RetrieveBloodGroup();
        };

        $scope.getBloodGroupList().then(function (result) {
            $scope.BloodGroupList = result.data.DataList;
            $timeout(function () {
                $scope.editData.BloodGroup = 0;
            });
        });

        /* getting list of countries*/
        function loadCountryDrop() {
            EmployeeCreateService.RetrieveCountry().then(function (result) {
                $scope.Country = result.data.DataList;
            });
        };
        loadCountryDrop();

        $scope.$watch('editData.Present_Country', function (newValue) {
            if (newValue != 0) {
                $scope.GetPresentStatesByCountry(newValue).then(function (PreStates) {
                    $scope.PresentStates = PreStates;
                });
            } else {
                $scope.PresentStates = {};
                $scope.editData.Present_State = 0;
            }
            
        });
        $scope.$watch('editData.Permanent_Country', function (newValue) {
            if (newValue != 0) {
                $scope.GetPermanentStatesByCountry(newValue).then(function (PerStates) {
                    $scope.PermanentStates = PerStates;
                });
            } else {
                $scope.PermanentStates = {};
                $scope.editData.Permanent_State = 0;
            }
        });

        /*getting list of present states by selected country*/
        $scope.GetPresentStatesByCountry = function (countryId) {
            return EmployeeCreateService.RetrieveState(countryId).then(function (result) {
                return result.data.DataList;
            });
        };
        $scope.GetPermanentStatesByCountry = function (countryId) {
            return EmployeeCreateService.RetrieveState(countryId).then(function (result) {
                return result.data.DataList;
            });
        };

        /*copy present address to permanent address*/
        $scope.CopyFromPresentAddress = function (checked) {
            if (checked) {
                $scope.editData.Permanent_HouseNo = $scope.editData.Present_HouseNo;
                $scope.editData.Permanent_Location = $scope.editData.Present_Location;
                $scope.editData.Permanent_Area = $scope.editData.Present_Area;
                $scope.editData.Permanent_Country = $scope.editData.Present_Country;
                $scope.editData.Permanent_State = $scope.editData.Present_State;
                $scope.editData.Permanent_City = $scope.editData.Present_City;
                $scope.editData.Permanent_PostalCode = $scope.editData.Present_PostalCode;
            } else {
                $scope.editData.Permanent_HouseNo = "";
                $scope.editData.Permanent_Location = "";
                $scope.editData.Permanent_Area = "";
                $scope.editData.Permanent_Country = 0;
                $scope.editData.Permanent_City = "";
                PermanentStates: {};
                $scope.editData.Permanent_PostalCode = "";
            }
        };

        /*save employee*/
        $scope.CreateUpdateEmployee = function (emp) {
            var IDate = $filter('date')(emp.BirthDate, 'dd-MM-yyyy').split('-');
            var temp_Date = new Date(parseInt(IDate[2]), parseInt(IDate[1]) - 1, parseInt(IDate[0]), 0, 0, 0);
            emp.BirthDate = $filter('date')(temp_Date, 'MM-dd-yyyy HH:mm:ss');

            $scope.editData.ProfilePhoto = $scope.fileName;
            $scope.editData.MarriageAnniversaryDate = $scope.editData.MaritalStatus == "Married" ? $scope.editData.MarriageAnniversaryDate : ""; //if marital status = married then only take mrg anniversary date
            $scope.editData.PassportExpiryDate = $scope.editData.PassportNumber != "" ? $scope.editData.PassportExpiryDate : ""; //if passport number exists then only take passport expiry date

            $rootScope.IsAjaxLoading = true;
            EmployeeCreateService.CreateUpdateEmployee(emp).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 1:Success
                        toastr.success(result.data.Message, 'Success');
                        $scope.master.EmployeeId = result.data.DataList.EmployeeId;
                        $scope.master.EmployeeName = result.data.DataList.CandidateFirstName + " " + result.data.DataList.CandidateLastName;
                        $scope.setEmpId($scope.master.EmployeeId);
                        $scope.master.Mode = "Edit";
                        $scope.storage.lastRecord = {};
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

        /*reset the form*/
        $scope.ResetEmployee = function () {
            if ($scope.master.Mode == "Edit") {
                $scope.tempEdit = false;
                BindData($scope.storage.lastRecord); //take last record and display it on form
            } else { //mode == add
                ResetForm();
            }
        };

        function ResetForm() {
            $scope.fileName = "";
            $scope.tempEdit = true;
            $scope.saveText = "Save";
            $scope.master.Mode = "Add";
            $scope.PresentStates = {};
            $scope.PermanentStates = {};

            $scope.editData = {
                EmployeeId: 0,
                EmployeeRegisterCode: "",
                CandidateFirstName: "",
                CandidateMiddleName: "",
                CandidateLastName: "",
                GuardianFirstName: "",
                GuardianMiddleName: "",
                GuardianLastName: "",
                ProfilePhoto: "",
                Present_HouseNo: "",
                Present_Location: "",
                Present_Area: "",
                Present_Country: 0,
                Present_State: {},
                Present_City: "",
                Present_PostalCode: "",
                Permanent_HouseNo: "",
                Permanent_Location: "",
                Permanent_Area: "",
                Permanent_Country: 0,
                Permanent_State: {},
                Permanent_City: "",
                Permanent_PostalCode: "",
                MaritalStatus: "",
                MarriageAnniversaryDate: "",
                BirthDate: "",
                Gender: "",
                DrivingLicenceNumber: "",
                PassportNumber: "",
                PassportExpiryDate: "",
                AdharNumber: "",
                PANCardNumber: "",
                PersonalEmailId: "",
                PersonalMobile: "",
                NomineeMobile: "",
                CompanyEmailId: "",
                CompanyMobile: "",
                BloodGroup: 0,
                IsActive: true,
            };

            $scope.copyAddress = false;
            $scope.fileName = "";

            $scope.empform.$setPristine();
            $scope.SetFocus();
        }

        /*catch edit mode*/
        $scope.setEmpId = function (eId) {
            if (eId) {
                $scope.saveText = "Update";
                $scope.master.Mode = "Edit";
                $scope.master.EmployeeId = eId;
                $rootScope.IsAjaxLoading = true;
                EmployeeCreateService.FetchEmployee(eId).then(function (result) {
                    if (result.data.IsValidUser) {
                        if (result.data.MessageType == 0) { // 0:Error
                            toastr.error(result.data.Message, 'Opps, Something went wrong');
                        } else if (result.data.MessageType == 1) { //1:Success
                            var line = result.data.DataList[0];
                            $scope.storage.lastRecord = line;
                            $scope.master.EmployeeName = line.CandidateFirstName + " " + line.CandidateLastName;
                            $scope.ImagePath = $scope.OriginPath;
                            $scope.tempEdit = false;
                            $scope.fileName = line.ProfilePhoto;
                            BindData(line);
                        }
                    }
                    else {
                        $rootScope.redirectToLogin();
                    }
                    $rootScope.IsAjaxLoading = false;
                });
            }
            //Add mode
            $scope.getBloodGroupList();
            $scope.SetFocus();
        }

        /*bind data to form in edit and reset operation in edit mode*/
        function BindData(line) {
            $scope.GetPresentStatesByCountry(line.Present_Country).then(function (allPresentStates) {
                $scope.PresentStates = allPresentStates;
                $scope.GetPermanentStatesByCountry(line.Permanent_Country).then(function (allPermenantStates) {
                    $scope.PermanentStates = allPermenantStates;
                    $scope.getBloodGroupList().then(function (bGroup) {
                        $scope.BloodGroupList = bGroup.data.DataList;

                        $scope.editData = {
                            EmployeeId: line.EmployeeId,
                            EmployeeRegisterCode: line.EmployeeRegisterCode,
                            CandidateFirstName: line.CandidateFirstName,
                            CandidateMiddleName: line.CandidateMiddleName,
                            CandidateLastName: line.CandidateLastName,
                            GuardianFirstName: line.GuardianFirstName,
                            GuardianMiddleName: line.GuardianMiddleName,
                            GuardianLastName: line.GuardianLastName,
                            ProfilePhoto: line.ProfilePhoto,
                            Present_HouseNo: line.Present_HouseNo,
                            Present_Location: line.Present_Location,
                            Present_Area: line.Present_Area,
                            Present_Country: line.Present_Country,
                            Present_State: line.Present_State,
                            Present_City: line.Present_City,
                            Present_PostalCode: line.Present_PostalCode,
                            Permanent_HouseNo: line.Permanent_HouseNo,
                            Permanent_Location: line.Permanent_Location,
                            Permanent_Area: line.Permanent_Area,
                            Permanent_Country: line.Permanent_Country,
                            Permanent_State: line.Permanent_State,
                            Permanent_City: line.Permanent_City,
                            Permanent_PostalCode: line.Permanent_PostalCode,
                            MaritalStatus: line.MaritalStatus,
                            MarriageAnniversaryDate: $filter('date')(line.MarriageAnniversaryDate, 'dd-MM-yyyy'),
                            BirthDate: $filter('date')(line.BirthDate, 'dd-MM-yyyy'),
                            Gender: line.Gender,
                            DrivingLicenceNumber: line.DrivingLicenceNumber,
                            PassportNumber: line.PassportNumber,
                            PassportExpiryDate: $filter('date')(line.PassportExpiryDate, 'dd-MM-yyyy'),
                            AdharNumber: line.AdharNumber,
                            PANCardNumber: line.PANCardNumber,
                            PersonalEmailId: line.PersonalEmailId,
                            PersonalMobile: line.PersonalMobile,
                            NomineeMobile: line.NomineeMobile,
                            CompanyEmailId: line.CompanyEmailId,
                            CompanyMobile: line.CompanyMobile,
                            BloodGroup: line.BloodGroup,
                            IsActive: true
                        };

                        $scope.editData.birthDate = line.BirthDate;
                        $scope.editData.marriageAnniversaryDate = line.MarriageAnniversaryDate;
                        $scope.editData.passportExpiryDate = line.PassportExpiryDate;
                    });
                });
            });

        }

        $scope.SetFocus = function () {
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
        };

        $scope.$watch('fileName', function (newValue, oldValue) {
            if ($scope.master.Mode == "Edit") {
                if (oldValue) {
                    if (newValue != oldValue) {
                        $scope.ImagePath = $scope.tempPath;
                    }
                    else {
                        $scope.ImagePath = $scope.OriginPath;
                    }
                }
                else {
                    if ($scope.isChanged && newValue != null) {
                        $scope.ImagePath = $scope.tempPath;
                    } else {
                        $scope.ImagePath = $scope.OriginPath;
                    }
                }
            }
            else {
                $scope.ImagePath = $scope.tempPath;
            }
        }, true);

        $scope.ViewProfilePicture = function (fileName) {
            var modalInstance = $modal.open({
                templateUrl: 'Profile.html',
                controller: ModalInstanceCtrl,
                scope: $scope,
                resolve: {
                    Profile: function () { return fileName; } //return anything that you want to pass to model
                }
            });
        };
    };

    // BEGIN MODAL INSTANCE CONTROLLER
    var ModalInstanceCtrl = function ($scope, $modalInstance, Profile) {
        $scope.profilePath = "/" + $scope.ImagePath.replace("/thumbnails", "") + "/" + Profile;

        $scope.CloseProfilePicture = function () {
            $modalInstance.close();
        };
    };
})();

