/// <reference path="../../../../Scripts/libs/angular/angular.js" />

(function () {

    'use strict';

    // Controller Signature
    angular.module("ERPApp.Controllers")
        .controller("InquiryCtrl", [
            "$scope", "$modal", "$rootScope", "$timeout", "InquiryService", "ProposalService", "$http", "$filter", "ngTableParams", "$q",
            InquiryCtrl
        ]);

    // Inquiry Controller Function
    function InquiryCtrl($scope, $modal, $rootScope, $timeout, InquiryService, ProposalService, $http, $filter, ngTableParams, $q) {

        $scope.mode = "Add";
        $scope.SaveText = "Save";
        $scope.editData = {};
        $scope.editData.multiFileList = [];
        $scope.oldMultifilelist = [];
        $scope.oldFilteredTechnologies = [];
        $scope.lastInquiry = {};
        $rootScope.isFormVisible = false;
        $scope.timeZone = new Date().getTimezoneOffset().toString();
        $scope.filterData = {
            ClientId: 0
        };

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
        $scope.OpenInquiryDateCalender = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.isInquiryDateOpened = true;
        };
        $scope.$watch("editData.iDate", function (newValue) {
            $scope.editData.InquiryDate = $filter('date')(newValue, 'dd-MM-yyyy');
        });

        $scope.ValidateInquiryDate = function (mDate, frmInquiry) {
            if (!mDate) {
                frmInquiry.txtInquiryDate.$setValidity("invalidInquiryDate", true);
                return;
            } else if (mDate.length == 10) {
                if ($scope.ValidateDate(mDate)) {
                    if ($scope.IsGreterThanToday(mDate)) {
                        $scope.editData.InquiryDate = $filter('date')(new Date(), 'dd-MM-yyyy');
                    } else {
                        frmInquiry.txtInquiryDate.$setValidity("invalidInquiryDate", true);
                        var dt = mDate.split('-');
                        $scope.editData.iDate = dt[2] + "-" + dt[1] + "-" + dt[0] + "T00:00:00";
                    }
                } else {
                    frmInquiry.txtInquiryDate.$setValidity("invalidInquiryDate", false);
                }
            } else {
                frmInquiry.txtInquiryDate.$setValidity("invalidInquiryDate", false);
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

        $scope.AddInquiry = function () {
            ResetForm();
            $scope.filterTechnologies = [];
        }

        function ResetForm() {
            $scope.editData = {
                PKInquiryId: 0,
                FKClientId: 0,
                FKSourceId: 0,
                InquiryTitle: "",
                InquiryStatus: 0,
                InquiryDate: moment().format("DD-MM-YYYY"),
                FKTechnologyIds: "",
                Remarks: "",
                IsActive: true,
                IsDeleted: false
            };
            //$scope.filterData.ClientId = 0;
            $scope.filterTechnologies = $scope.oldFilteredTechnologies;

            $scope.mode = "Add";
            $scope.SaveText = "Save";
            $scope.editData.multiFileList = [];
            $scope.lastInquiry = {};
            $rootScope.isFormVisible = true;
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
            //$scope.frmInquiry.$setPristine();
            //$scope.frmInquiry.txtInquiryDate.$setValidity("invalidInquiryDate", true);
        }

        function DoProcessInquiryDocument(inquiryId, arrayList, callback) {
            var newList = angular.copy(arrayList);
            var index = 0, length = newList.length;
            function saveData() {
                SaveInquiryDocument(newList[index], inquiryId).then(function (result) {
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

        function SaveInquiryDocument(line, inquiryId) {
            line.tblRefId = inquiryId;
            line.PKDocId = 0;
            line.DocName = line.file;
            line.DocTypeId = 16;
            line.Remarks = line.caption;
            return InquiryService.UploadDocument(line);
        };

        function DoProcessAfterInquirySave(result) {
            if ($scope.mode == "Add") {
                toastr.success(result.data.Message, 'Success');
                ResetForm();
            }
            else if ($scope.mode == "Edit") {
                toastr.success(result.data.Message, 'Success');
                ResetForm();
                $rootScope.isFormVisible = false;
            }
        };

        $scope.CreateUpdateInquiry = function (inquiry, frmInquiry) {
            var newInquiry = angular.copy(inquiry);
            var tempTechnologies = $scope.filterTechnologies;
            var tempArray = [];
            for (var i = 0 ; i < tempTechnologies.length; i++)
            { tempArray.push(tempTechnologies[i].Id); }
            newInquiry.FKTechnologyIds = tempArray.toString();
            newInquiry.FKClientId = $scope.filterData.ClientId;

            InquiryService.CreateUpdateInquiry(newInquiry, $scope.timeZone).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType === 1) {

                        if (inquiry.multiFileList.length > 0) {
                            var inquiryId = result.data.DataList;
                            DoProcessInquiryDocument(inquiryId, inquiry.multiFileList, function (res) {
                                DoProcessAfterInquirySave(result);
                            });
                        } else {
                            DoProcessAfterInquirySave(result);
                        }

                        frmInquiry.$setPristine(); //reset form validation
                        inquiry.multiFileList.length = 0; //reset
                        $scope.RefreshTable();
                    } else if (result.data.MessageType === 2) {
                        toastr.warning(result.data.Message, 'Record already exists');
                    }
                    else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                }
                else {
                    $rootScope.redirectToLogin();
                }
            });
        }

        //DELETE PARTICULAR IMAGE FROM MULTIFILELIST
        $scope.DeleteUploadedFile = function (index, list) {
            list.splice(index, 1); // we are not deleting file from server
        };

        //GET TECHNOLOGIES LIST 
        $scope.filterTechnologies = [];
        $scope.oldFilterTechnologies = [];
        $scope.RetrieveTechnologies = function () {
            InquiryService.RetrieveTechnologies().then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) {
                        $scope.technologies = result.data.DataList;
                    } else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }

                } else {
                    $rootScope.redirectToLogin();
                }
            });
        };

        $scope.loadTags = function (query) {
            var q = $q.defer();
            var data = [];
            setTimeout(function () {
                angular.forEach($scope.technologies, function (value, key) {
                    data.push({
                        'Id': value.Id,
                        'text': value.Label
                    });
                });
                q.resolve(data);
            }, 100);
            return q.promise;
        };

        $scope.FilterInquiry = function (filter) {
            //$scope.IsFilterResultDisplay = true;
            $rootScope.isFormVisible = false;
            $scope.RefreshTable();
        }

        //GET INQUIRY LIST
        $scope.RetrieveInquiries = function () {
            $scope.tableParams = new ngTableParams({
                page: 1,
                count: 10,
                sorting: {
                    Title: 'asc'
                    //Client: 'asc',
                    //Source: 'asc'
                },
                defaultSort: 'asc'
            }, {
                total: 0,
                filterDelay: 750,
                getData: function ($defer, params) {
                    $rootScope.IsAjaxLoading = true;
                    $scope.isRecord = false;
                    InquiryService.RetrieveInquiries($scope.filterData.ClientId, $scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter().Title, params.filter().Client, params.filter().Source).then(function (result) {
                        if (result.data.IsValidUser) {
                            //display no data message
                            if (result.data.MessageType === 1) {
                                if (result.data.DataList.total == 0) {
                                    $scope.noRecord = true;
                                    //$scope.filterText = params.filter().SubCategoryName;
                                } else {
                                    $scope.noRecord = false;
                                }
                                params.total(result.data.DataList.total);
                                $defer.resolve($scope.documents = result.data.DataList.result);
                                $rootScope.IsAjaxLoading = false;
                            }
                            else {
                                toastr.error(result.data.Message, 'Opps, Something went wrong');
                            }
                        }
                        else {
                            $rootScope.redirectToLogin();
                        }
                    });
                }
                //$scope: { $data: {} }
            });
        }

        // CHANGE INQUIRY STATUS
        $scope.ChangeInquiryStatus = function (inquiry) {
            InquiryService.ChangeInquiryStatus(inquiry).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType === 1) {
                        toastr.success(result.data.Message, 'Success');
                        $scope.RefreshTable();
                    } else if (result.data.MessageType == 2) { // 1:Warning
                        toastr.warning(result.data.Message, 'Record already exists');
                    } else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                }
                else {
                    $rootScope.redirectToLogin();
                }
            });
        };

        //REFRESH TABLE
        $scope.RefreshTable = function () {
            $scope.tableParams.reload();
        };

        //DELETE INQUIRY
        $scope.DeleteInquiry = function (id) {
            $scope.IsAjaxLoadingPMS = true;
            InquiryService.DeleteInquiry(id).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType === 1) {
                        toastr.success(result.data.Message, 'Success');
                        $scope.RefreshTable();

                    } else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
            });
            $scope.IsAjaxLoadingPMS = false
        }

        $scope.CloseInquiry = function (frmInquiry) {
            $scope.lastInquiry = {};
            $scope.editData.multiFileList.length = 0;
            $scope.oldMultifilelist.length = 0;
            $scope.mode = "Add";
            ResetForm();
            $rootScope.isFormVisible = false;
            $scope.isFirstFocus = false;
            frmInquiry.$setPristine();
        }

        $scope.UpdateInquiry = function (data) {
            var selectedTechnologies = [];
            var tempTechnologies = data.FKTechnologyIds;
            var temp = tempTechnologies.split(',').map(function (tempTechnologies) { return Number(tempTechnologies); });

            angular.forEach($scope.technologies, function (value, key) {
                for (var j = 0; j < temp.length ; j++) {
                    if (value.Id == temp[j]) {
                        selectedTechnologies.push({
                            'Id': value.Id,
                            'text': value.Label
                        });
                    }
                }
            });
            $scope.oldFilteredTechnologies = angular.copy(selectedTechnologies);
            $scope.filterTechnologies = selectedTechnologies;

            InquiryService.RetrieveDocument(data.PKInquiryId).then(function (result) {
                var tempMultiFileList = [];
                angular.forEach(result.data.DataList, function (value, key) {
                    tempMultiFileList.push({
                        'file': value.DocName,
                        'caption': value.Remarks,
                        'ext': value.DocName.split('.')[1]
                    });
                });
                $scope.oldMultifilelist = tempMultiFileList;
                $scope.mode = "Edit";
                $scope.SaveText = "Update";
                $scope.editData = {
                    PKInquiryId: data.PKInquiryId,
                    FKClientId: data.FKClientId,
                    FKSourceId: data.FKSourceId,
                    InquiryTitle: data.InquiryTitle,
                    InquiryStatus: data.InquiryStatus,
                    InquiryDate: $filter('date')(data.InquiryDate, 'dd-MM-yyyy'),
                    FKTechnologyIds: data.FKTechnologyIds,
                    Remarks: data.Remarks,
                    IsActive: data.IsActive,
                    multiFileList: tempMultiFileList,
                    iDate: data.InquiryDate
                };

                $scope.filterData.ClientId = $scope.editData.FKClientId;

                $scope.lastInquiry = {
                    PKInquiryId: data.PKInquiryId,
                    FKClientId: data.FKClientId,
                    FKSourceId: data.FKSourceId,
                    InquiryTitle: data.InquiryTitle,
                    InquiryStatus: data.InquiryStatus,
                    InquiryDate: $filter('date')(data.InquiryDate, 'dd-MM-yyyy'),
                    FKTechnologyIds: data.FKTechnologyIds,
                    Remarks: data.Remarks,
                    IsActive: data.IsActive,
                    multiFileList: tempMultiFileList,
                    iDate: data.InquiryDate
                };
                $rootScope.isFormVisible = true;
                $scope.isFirstFocus = false;
                $timeout(function () {
                    $scope.isFirstFocus = true;
                });
            });
        };

        $scope.ResetInquiry = function (frmInquiry) {

            if ($scope.mode === "Add") {
                $scope.editData.multiFileList.length = 0;
                $scope.oldMultifilelist.length = 0;
                $scope.filterTechnologies = [];
                $scope.oldFilteredTechnologies = [];
                ResetForm();
            }
            else if ($scope.mode === "Edit") {
                var tempMultiFileList = [];
                angular.forEach($scope.lastInquiry.multiFileList, function (value, key) {
                    tempMultiFileList.push({
                        'file': value.file,
                        'caption': value.caption,
                        'ext': value.file.split('.')[1]
                    });
                });
                $scope.editData = {
                    PKInquiryId: $scope.lastInquiry.PKInquiryId,
                    FKClientId: $scope.lastInquiry.FKClientId,
                    FKSourceId: $scope.lastInquiry.FKSourceId,
                    InquiryTitle: $scope.lastInquiry.InquiryTitle,
                    InquiryStatus: $scope.lastInquiry.InquiryStatus,
                    InquiryDate: $scope.lastInquiry.InquiryDate,
                    FKTechnologyIds: $scope.lastInquiry.FKTechnologyIds,
                    Remarks: $scope.lastInquiry.Remarks,
                    IsActive: $scope.lastInquiry.IsActive,
                    multiFileList: tempMultiFileList,
                    iDate: $scope.lastInquiry.iDate
                };
                $scope.filterTechnologies = angular.copy($scope.oldFilteredTechnologies);
                $scope.isFirstFocus = false;
                $timeout(function () {
                    $scope.isFirstFocus = true;
                });
            }

            frmInquiry.$setPristine();
        };

        // BEGIN COMBO BOX VALIDATION
        $scope.ValidateSource = function () {
            if ($scope.editData.FKSourceId && $scope.editData.FKSourceId != 0) return false;
            return true;
        }
        $scope.ValidateStatus = function () {
            if ($scope.editData.InquiryStatus && $scope.editData.InquiryStatus != 0) return false;
            return true;
        }
        $scope.LoadData = function (clientId, inquiryId, action) {
            //$scope.IsAjaxLoading = true;
            InquiryService.RetrieveClients().then(function (result) {
                if (result.data.IsValidUser) {
                    $scope.FilterClientList = result.data.DataList;

                    //Retrieve Client Source
                    InquiryService.RetrieveClientSources().then(function (resSource) {
                        $scope.sources = resSource.data.DataList;
                        $scope.editData.FKSourceId = 0;

                        //Retrieve Inquiry Status
                        InquiryService.RetrieveStatus().then(function (resStatus) {
                            $scope.status = resStatus.data.DataList;
                            $scope.editData.InquiryStatus = 0;

                            //Retrieve Technologies
                            InquiryService.RetrieveTechnologies().then(function (resTech) {
                                $scope.technologies = resTech.data.DataList;
                                $timeout(function () {
                                    if (clientId) {
                                        $scope.filterData.ClientId = clientId;
                                        $rootScope.isFormVisible = false;
                                        $scope.RetrieveInquiries();

                                        if (inquiryId != 0) {//edit data and display form
                                            InquiryService.GetInquiry(inquiryId).then(function (res) {
                                                $scope.UpdateInquiry(res.data.DataList);
                                            });
                                        } else if (action == "add") {//add new option
                                            $rootScope.isFormVisible = true;
                                        }
                                    }
                                    else { //view all option
                                        $scope.filterData.ClientId = 0;
                                        $scope.RetrieveInquiries();
                                    }
                                });
                            });
                        });
                    });
                } else {
                    $rootScope.redirectToLogin();
                }
            });
            $scope.IsAjaxLoading = false;
        };

        // OPEN PROPOSAL POPUP
        $scope.ViewProposals = function (inquiry) {
            var modalInstance = $modal.open({
                templateUrl: 'Proposal.html',
                controller: ProposalCtrl,
                scope: $scope,
                resolve: {
                    inquiry: function () { return inquiry; }
                    //$rootScope: function () { return $rootScope },
                    //$timeout: function () { return $timeout },
                    //ProposalService: function () { return ProposalService },
                    //$filter: function () { return $filter },
                },
            });
        }
    }

    /*
    * BEGIN PROPOSAL CONTROLLER
    */
    var ProposalCtrl = function ($scope, $modalInstance, inquiry, $rootScope, $timeout, ProposalService, $filter) {
        $scope.mode = "Add";
        $scope.saveText = "Save";

        $scope.inquiry = inquiry;
        $scope.proposals = [];
        $scope.formData = {};
        $scope.formData.multiFileList = [];
        $scope.oldFormData = {};
        $scope.oldMultifilelist = [];
        $scope.timeZone = new Date().getTimezoneOffset().toString();
        $rootScope.isProposalFormVisible = false;
        $scope.IsAjaxLoading = false;

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
        $scope.OpenProposalDateCalender = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.isProposalDateOpened = true;
        };
        $scope.$watch("formData.pDate", function (newValue) {
            $scope.formData.ProposalDate = $filter('date')(newValue, 'dd-MM-yyyy');
        });
        $scope.ValidateProposalDate = function (pDate, frmProposal) {
            if (!pDate) {
                frmProposal.txtProposalDate.$setValidity("invalidProposalDate", true);
                return;
            } else if (pDate.length == 10) {
                if ($scope.ValidateDate(pDate)) {
                    if ($scope.IsGreterThanToday(pDate)) {
                        $scope.formData.ProposalDate = $filter('date')(new Date(), 'dd-MM-yyyy');
                    } else {
                        frmProposal.txtProposalDate.$setValidity("invalidProposalDate", true);
                        var dt = pDate.split('-');
                        $scope.formData.pDate = dt[2] + "-" + dt[1] + "-" + dt[0] + "T00:00:00";
                    }
                } else {
                    frmProposal.txtProposalDate.$setValidity("invalidProposalDate", false);
                }
            } else {
                frmInquiry.txtProposalDate.$setValidity("invalidProposalDate", false);
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

        function DoProcessProposalDocument(proposalId, arrayList, callback) {
            var newList = angular.copy(arrayList);
            var index = 0, length = newList.length;
            function SaveData() {
                SaveProposalDocument(newList[index], proposalId).then(function (result) {
                    if (index == length - 1) {
                        callback(result);
                    } else {
                        index++;
                        SaveData();
                    }
                });
            }
            SaveData();
        }
        function SaveProposalDocument(line, proposalId) {
            line.tblRefId = proposalId;
            line.PKDocId = 0;
            line.DocName = line.file;
            line.DocTypeId = 17;
            line.Remarks = line.caption;
            return ProposalService.UploadDocument(line);
        }
        function DoProcessAfterProposalSave(result) {
            if ($scope.mode == "Add") {
                toastr.success(result.data.Message, 'Success');
                ResetProposalForm();
            }
            else if ($scope.mode == "Edit") {
                toastr.success(result.data.Message, 'Success');
                ResetProposalForm();
                $rootScope.isProposalFormVisible = false;
            }
        };

        // BEGIN CRUD FUNCTIONS
        $scope.CreateUpdateProposal = function (data, frmProposal) {

            var dateArray = data.ProposalDate.split('-');
            var date = new Date(parseInt(dateArray[2]), parseInt(dateArray[1]) - 1, parseInt(dateArray[0]), moment().hours(), moment().minute(), moment().second());
            var proposalDate = $filter('date')(date, 'MM-dd-yyyy HH:mm:ss');

            var _data = {
                PKProposalId: data.PKProposalId,
                FKInquiryId: inquiry.PKInquiryId,
                ProposalTitle: data.ProposalTitle,
                ProposalDate: proposalDate,
                IsFinalized: data.IsFinalized,
                Remarks: data.Remarks,
                IsActive: true,
                IsDeleted: false,
            }

            ProposalService.CreateUpdateProposal(_data, $scope.timeZone).then(function (result) {
                $scope.IsAjaxLoading = true;
                if (result.data.IsValidUser) {
                    if (result.data.MessageType === 1) {
                        if (data.multiFileList.length > 0) {
                            var proposalId = result.data.DataList;
                            DoProcessProposalDocument(proposalId, data.multiFileList, function (res) {
                                DoProcessAfterProposalSave(result);
                            });
                        } else {
                            DoProcessAfterProposalSave(result);
                        }

                        data.multiFileList.length = 0; //reset
                        $scope.RetrieveProposals();
                        $scope.IsAjaxLoading = false;
                        frmProposal.$setPristine(); //reset form validation
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
        $scope.RetrieveProposals = function () {
            $scope.IsAjaxLoading = true;
            ProposalService.RetrieveProposals(inquiry.PKInquiryId, $scope.timeZone).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType === 1) {
                        $scope.proposals = result.data.DataList;
                        $scope.IsAjaxLoading = false;
                    } else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                }
                else {
                    $rootScope.redirectToLogin();
                }
            });
        };
        $scope.RetrieveProposals();
        $scope.DeleteProposal = function (id) {
            $scope.IsAjaxLoadingPMS = true;
            ProposalService.DeleteProposal(id).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType === 1) {
                        toastr.success(result.data.Message, 'Success');
                        $scope.RetrieveProposals();

                    } else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                } else {
                    $rootScope.redirectToLogin();
                }
            });
            $scope.IsAjaxLoadingPMS = false
        };
        // END CRUD FUNCTIONS

        // BEGIN OTHER FUNCTIONS
        $scope.AddProposal = function () {
            ResetProposalForm();
        };

        $scope.ResetProposal = function (frmProposal) {
            if ($scope.mode === "Add") {
                ResetProposalForm();
            }
            else if ($scope.mode === "Edit") {
                $scope.formData = angular.copy($scope.oldFormData);
                $scope.formData.multiFileList = $scope.oldFormData.multiFileList
                $scope.isFirstFocus = false;
                $timeout(function () {
                    $scope.isFirstFocus = true;
                });
            }
            frmProposal.$setPristine();
        };

        $scope.UpdateProposal = function (_data) {
            var data = angular.copy(_data);
            ProposalService.RetrieveDocument(data.PKProposalId).then(function (result) {
                var tempMultiFileList = [];
                angular.forEach(result.data.DataList, function (value, key) {
                    tempMultiFileList.push({
                        'file': value.DocName,
                        'caption': value.Remarks,
                        'ext': value.DocName.split('.')[1]
                    });
                });
                $scope.oldMultifilelist = tempMultiFileList;
                $scope.formData = angular.copy(data);
                $scope.formData.multiFileList = tempMultiFileList;
                $scope.formData.ProposalDate = $filter('date')(data.ProposalDate, 'dd-MM-yyyy');
                $scope.oldFormData = angular.copy(data);
                $scope.oldFormData.multiFileList = tempMultiFileList;
                $scope.oldFormData.ProposalDate = $filter('date')(data.ProposalDate, 'dd-MM-yyyy'),
                $scope.mode = "Edit";
                $scope.saveText = "Update";
                $rootScope.isProposalFormVisible = true;
                $scope.isFirstFocus = false;
                $timeout(function () {
                    $scope.isFirstFocus = true;
                });
            });
        };
        $scope.CloseProposal = function (frmProposal) {
            $scope.formData = {};
            $scope.oldFormData = {};
            $scope.mode = "Add";
            $rootScope.isProposalFormVisible = false;
            $scope.isFirstFocus = false;
            frmProposal.$setPristine();
        };
        function ResetProposalForm() {
            $scope.formData = {
                PKProposalId: 0,
                FKInquiryId: 0,
                ProposalTitle: "",
                ProposalDate: moment().format("DD-MM-YYYY"),
                IsFinalized: false,
                Remarks: "",
                IsActive: true,
                IsDeleted: false,
                multiFileList: []
            };

            $scope.oldFormData = {};
            $scope.mode = "Add";
            $scope.saveText = "Save";
            $rootScope.isProposalFormVisible = true;
            $scope.isFirstFocus = false;
            $timeout(function () {
                $scope.isFirstFocus = true;
            });
            //$scope.frmProposal.$setPristine();
        };

        $scope.DeleteUploadedFile = function (index, list) {
            list.splice(index, 1); // we are not deleting file from server
        };
        $scope.ChangeProposalStatus = function (id) {
            ProposalService.ChangeProposalStatus(id).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType === 1) {
                        toastr.success(result.data.Message, 'Success');
                        $scope.RetrieveProposals();

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
        // BEGIN OTHER FUNCTIONS
    };
    /*
    * END PROPOSAL CONTROLLER
    */
})();