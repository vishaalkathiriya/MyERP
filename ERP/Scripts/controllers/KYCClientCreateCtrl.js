/// <reference path="../libs/angular/angular.min.js" />


(function () {

    'use strict';

    //Define controller signature
    angular.module("ERPApp.Controllers")
        .controller("KYCClientCreateCtrl", [
            "$scope", "$rootScope", "$timeout", "KYCClientCreateService", "$http", "$filter","$q",
            kycClientCreateCtrl
        ]);


    //Main controller function
    function kycClientCreateCtrl($scope, $rootScope, $timeout, KYCClientCreateService, $http, $filter, $q) {
        $scope.btnFSaveText = "Update";
        $scope.editDataF = $scope.editDataF || {};
        $scope.editDataF.CPrefix = "Mr.";
        $scope.TelPrefix = "+";
        $scope.director = $scope.director || {};
        $scope.director.Prefix = "Mr.";

        $scope.validateDropCountry = function () {
            if ($scope.editDataF.CountryId && $scope.editDataF.CountryId != 0) return false;
            return true;
        };

        $scope.validateDropState = function () {
            if ($scope.editDataF.StateId && $scope.editDataF.StateId != 0) return false;
            return true;
        };

        $scope.validateDropBusinessType = function () {
            if ($scope.editDataF.BusinessTypeId && $scope.editDataF.BusinessTypeId != 0) return false;
            return true;
        };

        $scope.$watch('isValidRequest', function (newValue) {
            if (newValue == false) {
                $scope.msgLinkExpired = "Sorry, We can't process your request. Your URL is either expired or not valid.";
            }
        });
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

        $scope.calendarOpenDate = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.editDataF.calOpenBSDate = true;

        };

        $scope.$watch("editDataF.businessStartDate", function (newValue) {
            $scope.editDataF.BusinessStartDate = $filter('date')(newValue, 'dd-MM-yyyy');
        });

        $scope.ValidateBusinessStartDate = function (mDate) {
            if (!mDate) {
                $scope.ccfform.txtFBEDate.$setValidity("invalidBEDate", true);
                return;
            } else if (mDate.length == 10) {
                if ($scope.ValidateDate(mDate)) {
                    var dt = mDate.split('-');
                    $scope.editDataF.businessStartDate = dt[2] + "-" + dt[1] + "-" + dt[0] + "T00:00:00";
                    $scope.ccfform.txtFBEDate.$setValidity("invalidBEDate", true);
                } else {
                    $scope.ccfform.txtFBEDate.$setValidity("invalidBEDate", false);
                }
            } else {
                $scope.ccfform.txtFBEDate.$setValidity("invalidBEDate", false);
            }
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

        $scope.validateDropSource = function () {
            if ($scope.editDataF.SourceId && $scope.editDataF.SourceId != 0) return false;
            return true;
        };

        /*getting list of present states by selected country*/
        $scope.GetPresentStatesByCountry = function (countryId) {
            return KYCClientCreateService.RetrieveState(countryId);
        };

        $scope.$watch('editDataF.CountryId', function (newValue) {
            if (newValue != 0) {
                $scope.GetPresentStatesByCountry(newValue).then(function (result) {
                    $scope.States = result.data.DataList;
                });

                //get dial code +91
                angular.forEach($scope.Country, function (value, key) {
                    if (value.CountryId == newValue) {
                        $scope.TelPrefix = value.DialCode;
                    }
                });
            } else {
                $scope.States = {};
                $scope.editDataF.StateId = 0;
                $scope.TelPrefix = "+";
            }
        });

        /*DIRECTOR SECTION*/
        $scope.directorList = [];
        $scope.IdentityDocumentList = [];
        $scope.isDirectorFound = false;
        $scope.AddDirector = function (record, form) {
            record.PKId = 0;
            GetDocumentName(record.IdentityDocId, function (docName) {
                record.IdentityDocName = docName;
                $scope.directorList.push(record);
                $scope.director = {};
                $scope.director.IdentityDocId = 0;
                $scope.director.Prefix = "Mr.";
                $scope.isDirectorFound = true;
                form.$setPristine();
            });
        };
        function GetDocumentName(docId, callback) {
            angular.forEach($scope.IdentityDocumentList, function (value, key) {
                if (value.Id == docId) {
                    callback(value.Documents);
                }
            });
        };
        $scope.LoadDirectorIdentityDrop = function (list) {
            angular.forEach(list, function (value, key) {
                if (value.DocumentTypeId == 1) {
                    $scope.IdentityDocumentList.push(value);
                }
            });
        };
        $scope.LoadDirectorList = function (list) {
            var prom = [];
            angular.forEach(list, function (v, k) {
                GetDocumentName(v.IdentityDocId, function (docName) {
                    v.IdentityDocName = docName;
                    prom.push(v);
                });
            });

            $q.all(prom).then(function () {
                $scope.directorList = list;
                $scope.isDirectorFound = $scope.directorList.length > 0 ? true : false;
            });
        };

        $scope.DeleteDirector = function ($index) {
            if ($scope.directorList[$index].PKId != 0) {
                $rootScope.IsAjaxLoading = true;
                KYCClientCreateService.DeleteDirector($scope.directorList[$index].PKId).then(function (result) {
                    if (result.data.IsValidUser) {
                        if (result.data.MessageType == 1) { // 1:Success
                            $scope.directorList.splice($index, 1);
                            CheckDirectorAvails();
                            toastr.success(result.data.Message, 'Success');
                        } else {
                            toastr.error(result.data.Message, 'Opps, Something went wrong');
                        }
                    } else {
                        $rootScope.redirectToLogin();
                    }
                    $rootScope.IsAjaxLoading = false;
                    
                });
            } else {
                $scope.directorList.splice($index, 1);
                CheckDirectorAvails();
            }
        };

        function CheckDirectorAvails() {
            $scope.isDirectorFound = false;
            if ($scope.directorList.length > 0) {
                $scope.isDirectorFound = true;
            }
        }

        /*DOCUMENT SECTION*/
        $scope.document = $scope.document || {};
        $scope.document = {
            DocId: 0,
            fileName: ""
        };
        $scope.isChanged = false;
        $scope.documentList = [];
        $scope.isIdentityProofFound = false;
        $scope.isCompanyProofFound = false;

        $scope.validateDropDocuments = function () {
            if ($scope.document.DocId && $scope.document.DocId != 0) return false;
            return true;
        };
        $scope.validateImage = function () {
            if ($scope.document.fileName || $scope.document.fileName == "") return false;
            return true;
        };

        $scope.AddDocument = function (record, form) {
            angular.forEach($scope.DocumentList, function (value, key) {
                if (value.Id == record.DocId) {
                    if (value.DocumentTypeId == 1) { //DocumentTypeId of tblDocuments
                        $scope.isIdentityProofFound = true;
                    } else if (value.DocumentTypeId == 2) {
                        $scope.isCompanyProofFound = true;
                    }

                    record.DocName = value.Documents;
                    record.DocTypeId = value.DocumentTypeId;
                    record.PKDocId = 0;
                    $scope.documentList.push(record);
                    $scope.document = { DocId: 0, fileName: "" };
                    form.$setPristine();
                }
            });
        };
        $scope.DeleteDocument = function ($index) {
            if ($scope.documentList[$index].PKDocId != 0) {
                $rootScope.IsAjaxLoading = true;
                KYCClientCreateService.DeleteDocument($scope.documentList[$index].PKDocId).then(function (result) {
                    if (result.data.IsValidUser) {
                        if (result.data.MessageType == 1) { // 1:Success
                            $scope.documentList.splice($index, 1);
                            CheckDocAvails();
                            toastr.success(result.data.Message, 'Success');
                        } else {
                            toastr.error(result.data.Message, 'Opps, Something went wrong');
                        }
                    } else {
                        $rootScope.redirectToLogin();
                    }
                    $rootScope.IsAjaxLoading = false;
                    
                });
            } else {
                $scope.documentList.splice($index, 1);
                CheckDocAvails();
            }
        };
        function CheckDocAvails() {
            $scope.isIdentityProofFound = false;
            $scope.isCompanyProofFound = false;
            angular.forEach($scope.documentList, function (value, key) {
                if (value.DocTypeId == 1) {
                    $scope.isIdentityProofFound = true;
                } else if (value.DocTypeId == 2) {
                    $scope.isCompanyProofFound = true;
                }
            });
        }

        /*VALIDATION BETWEEN MAIN AND SUB FORMS*/
        $scope.validateDirectorForm = function () {
            if ($scope.directorList.length > 0) {
                $scope.isDirectorFound = true;
                return false;
            }

            $scope.isDirectorFound = false;
            return true;
        };
        $scope.validateDocumentForm = function () {
            if ($scope.isIdentityProofFound == true && $scope.isCompanyProofFound == true) {
                return false;
            }
            return true;
        };

        /*initial load of data*/
        $scope.LoadFullData = function (urlKey) {
            if (urlKey) {
                KYCClientCreateService.ValidateURL(urlKey).then(function (resClient) { //validate URL
                    if (resClient.data.DataList) {
                        $scope.isValidRequest = true;
                        var countryId = resClient.data.DataList.CountryId;

                        KYCClientCreateService.RetrieveCountry().then(function (r1) {
                            $scope.Country = r1.data.DataList;

                            KYCClientCreateService.LoadClientSource().then(function (resClientSource) {
                                $scope.ClientSource = resClientSource.data.DataList;

                                KYCClientCreateService.GetBusinessTypeList().then(function (r2) {
                                    $scope.BusinessType = r2.data.DataList;

                                    KYCClientCreateService.GetDocuments().then(function (result) {
                                        $scope.DocumentList = result.data.DataList;
                                        $scope.LoadDirectorIdentityDrop($scope.DocumentList);

                                        $scope.GetPresentStatesByCountry(countryId).then(function (resState) {
                                            $timeout(function () {
                                                $scope.States = resState.data.DataList; //bind state dropdown list
                                            }, 1000);

                                            $scope.editDataF = resClient.data.DataList; //Assigned full object
                                            $scope.editDataF.SourceId = resClient.data.DataList.FKSourceId;
                                            $scope.editDataF.BusinessStartDate = $filter('date')(resClient.data.DataList.BusinessStartDate, 'dd-MM-yyyy');
                                            var dt = resClient.data.DataList.BusinessStartDate.split('-');
                                            $scope.editDataF.businessStartDate = dt[2] + "-" + dt[1] + "-" + dt[0] + "T00:00:00";

                                            //update both director and document list
                                            //$scope.directorList = resClient.data.DataList.ClientPersonList;
                                            $scope.LoadDirectorList(resClient.data.DataList.ClientPersonList);
                                            $scope.director.IdentityDocId = 0;
                                            //$scope.isDirectorFound = $scope.directorList.length > 0 ? true : false;


                                            angular.forEach(resClient.data.DataList.ClientDocumentList, function (v1, k1) {
                                                angular.forEach($scope.DocumentList, function (v2, k2) {
                                                    if (v1.DocId == v2.Id) {
                                                        if (v2.DocumentTypeId == 1) { //DocumentTypeId of tblDocuments
                                                            $scope.isIdentityProofFound = true;
                                                        } else if (v2.DocumentTypeId == 2) {
                                                            $scope.isCompanyProofFound = true;
                                                        }

                                                        v1.fileName = angular.copy(v1.DocName);
                                                        v1.DocName = angular.copy(v2.Documents); //replace filename with document name to display it on screen
                                                    }
                                                });
                                            });
                                            $scope.documentList = resClient.data.DataList.ClientDocumentList;
                                        });
                                    });
                                });
                            });
                        });
                    } else {
                        $scope.isValidRequest = false;
                    }
                });
            } else {
                $scope.isValidRequest = false;
            }
        };

        /*process document list*/
        function DoProcessDocumentList(arrayList, clientId, callback) {
            var newList = angular.copy(arrayList);
            var index = 0, length = newList.length;
            function saveData() {
                var currentData = newList[index];
                SaveDocument(currentData, clientId).then(function (result) {
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
        function SaveDocument(line, clientId) {
            line.tblRefId = clientId;
            line.DocName = line.fileName; //pass filename which is stored in file directory
            return KYCClientCreateService.SaveDocument(line);
        };

        /*process director list*/
        function DoProcessDirectorList(arrayList, clientId, callback) {
            var index = 0, length = arrayList.length;
            function saveData() {
                var currentData = arrayList[index];
                SaveDirector(currentData, clientId).then(function (result) {
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
        function SaveDirector(line, clientId) {
            line.FKClientId = clientId;
            return KYCClientCreateService.SaveDirector(line);
        };

        function GetCountryCode(cid, callback) {
            var prom = [];
            var cc = "";
            angular.forEach($scope.Country, function (value, key) {
                if (value.CountryId == cid) {
                    cc = value.CountryCode;
                }
                prom.push(value);
            });

            $q.all(prom).then(function () {
                callback(cc);
            });
        };

        $scope.CreateUpdateFullClient = function (data) {
            $rootScope.IsAjaxLoading = true;
            //data.PKClientId = $scope.master.clientId;
            data.FKSourceId = data.SourceId;
            data.BusinessStartDateInString = $filter('date')(data.BusinessStartDate, 'dd-MM-yyyy');
            GetCountryCode(data.CountryId, function (ccode) {
                data.CountryCode = ccode;
                KYCClientCreateService.CreateUpdateFullClient(data).then(function (r1) {
                    if (r1.data.IsValidUser) {
                        if (r1.data.MessageType == 1) { // 1:Success
                            var clientId = r1.data.DataList;

                            //save director list
                            DoProcessDirectorList($scope.directorList, clientId, function (r2) {
                                
                                //save document list
                                DoProcessDocumentList($scope.documentList, clientId, function (r3) {
                                    //display final notification
                                    toastr.success(r1.data.Message, 'Success');
                                });
                            });
                        }
                        else {
                            toastr.error(r1.data.Message, 'Opps, Something went wrong');
                        }
                    } else {
                        $rootScope.redirectToLogin();
                    }
                    $rootScope.IsAjaxLoading = false;
                });
            });
            
        };
    };
})();

