
angular.module("ERPApp.Controllers").controller("ABContactCtrl", [
"$scope","$rootScope","ABContactService","$http","$filter","ngTableParams","$timeout","$q",
    function ($scope, $rootScope, ABContactService, $http, $filter, ngTableParams,$timeout,$q) {
        $scope.editData = $scope.editData || {};
        $scope.mode = "Add";
        $scope.loadcount = 1;
        $scope.tempmode = "Upload";
        $scope.SaveText = "Save";
        $rootScope.isFormVisible = false;
        $rootScope.isGroupFormVisible = false;
        $scope.isFirstFocus = false;
        $rootScope.isContactUploadFormVisible = false;
        $scope.lastContact = $scope.lastContact || {};
        $scope.selection = [];
        $scope.contactSelection = [];
        $scope.OriginPath = window.Origin;
        $scope.tempPath = window.temp;
        $scope.editData.Language = 0;
        $scope.Languagemode = 0;
        $scope.fontSize = 12;
        $scope.Contacts = [];
        $scope.reccount = 0;
        $scope.isChanged = true;
        $scope.fileName = "";
        
        $scope.filterData = "";
        $scope.AddContact = function () {
            $scope.selection = [];
            $scope.contactSelection = [];
            $('input[name="chkGroupName"]').each(function () {
                    $(this).prop('checked', false);
            });
            $scope.mode = "Add";
            $scope.SaveText = "Save"
            $scope.editData = {};
            $scope.editData.Language = 0;
            $rootScope.isFormVisible = true;
            $rootScope.isGroupFormVisible = false;
            $rootScope.isContactUploadFormVisible = false;
            $scope.frmContact.$setPristine();

        }

        // fill Language DropDown

        function languageList() {
            ABContactService.languageList().then(function (result) {
                $scope.languageList = result.data.DataList;
                return false;
            });
        }

        languageList();


        // Retrive Group
        function GroupList() {
            ABContactService.GroupList().then(function (result) {
                $scope.GroupList = result.data.DataList;
                return false;
               
            });
        }
        GroupList();

        $scope.FilterGroupName = function (column) {
          
            var def = $q.defer(), arr = [];
            $scope.group = [];
            ABContactService.GroupList().then(function (result) {
                $scope.Grouplist = result.data.DataList;
                angular.forEach($scope.Grouplist, function (Group) {
                    arr.push({ id: Group.GroupId, title: Group.GroupName })
                    $scope.group.push({ Id: Group.GroupId, Label: Group.GroupName })
                });
            });
            
            def.resolve(arr);
            return def;
        };
        
     //   $scope.filteList = [{ Id: 1, Label: 'Active' }, { Id: 2, Label: 'Hold' }, { Id: 3, Label: 'Finished' }, { Id: 4, Label: 'Deleted' }];

        $scope.FilterGroup = function () {
            $scope.filterData = "";
            angular.forEach($scope.selectedList, function (value, key) {
                $scope.filterData += value.Id + ",";
            });
            $scope.RefreshTable();
        };

       
        $scope.selectGroup = function (groupId)
        {
            var idx = $scope.selection.indexOf(groupId);
            // is currently selected
            if (idx > -1) {
                $scope.selection.splice(idx, 1);
            }
                // is newly selected
            else {
                $scope.selection.push(groupId);
            }
        }

        $scope.selectGroupName = function (GroupName) {
            $scope.selection = [];
            var groupNameList = GroupName.split(',');
            var groupList = $scope.GroupList;
            $('input[name="chkGroupName"]').each(function () {
                $(this).prop('checked', false);
            });
            for (var gl = 0; gl < groupList.length; gl++) {
                var glName = groupList[gl];
                for (var gn = 0; gn < groupNameList.length; gn++)
                {
                    var gnName = groupNameList[gn];
                    if (gnName == glName.GroupName)
                    {
                        $('input[name="chkGroupName"]').each(function () {
                            if ($(this).val() == glName.GroupId) {
                                $(this).prop('checked', true);
                            }
                        });
                        $scope.selection.push(glName.GroupId);
                    }
                }
            }
        }
        


       $scope.selectContact = function (ContactId) {
            var idx = $scope.contactSelection.indexOf(ContactId);
            // is currently selected
            if (idx > -1) {
                $scope.contactSelection.splice(idx, 1);
            }
                // is newly selected
            else {
                $scope.contactSelection.push(ContactId);
            }
        }


        // Open Group From 
        $scope.ApplyGroup=function ()
        {
            $rootScope.isFormVisible = false;
            $rootScope.isContactUploadFormVisible = false;
            $rootScope.isGroupFormVisible = true;
            $scope.selection = [];
            $('input[name="chkGroupName"]').each(function () {
                $(this).prop('checked', false);
            });
           
        }
        
        // Apply Contact To Group
        $scope.ApplyGroupContact = function ()
        {
            var selectedGroup = $scope.selection;
            var selectedContact = $scope.contactSelection;
            
            ABContactService.ApplyGroupcontact(selectedGroup, selectedContact).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType === 1) {
                        $scope.RefreshTable();
                        $rootScope.isGroupFormVisible = false;
                        toastr.success(result.data.Message, "Success");
                        $scope.contactSelection = [];
                        $scope.checkboxes = { 'checked': false, items: {} };
                        $scope.RefreshTable();
                       
                    }
                    else if (result.data.MessageType===2)
                    {
                        toastr.warning(result.data.Message, "Warning!");
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

        // Select Language To Display Records
        $scope.LanguageSelect = function (LangId) {
            $scope.Languagemode = LangId;
            $scope.RefreshTable();

        }

        // Create / Update Contact
        $scope.CreateUpdateContact = function (contact) {
            var selectedGroup = $scope.selection;
            ABContactService.CreateUpdateContact(contact, selectedGroup).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType === 1) {
                        $scope.editData = {
                            fullName: '',
                            companyName: '',
                            phoneNo: '',
                            landlineNo: '',
                            address1: '',
                            address2: '',
                            area: '',
                            city: '',
                            pincode: '',
                            note: '',
                           
                        };
                        $scope.RefreshTable();

                        if ($scope.mode === "Edit") {
                            $rootScope.isFormVisible = false;
                        }
                        $scope.isFirstFocus = false;
                        $scope.frmContact.$setPristine();
                        if ($scope.mode == "Add") {
                            $rootScope.isFormVisible = false;
                            toastr.success(result.data.Message, 'Success');
                        }
                        else if ($scope.mode == "Edit") {
                            toastr.success(result.data.Message, 'Success');
                        }
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

        // Delete contact
        $scope.DeleteContact = function (ContactId) {
                ABContactService.DeleteContact(ContactId).then(function(result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType == 1) { // 0:Error
                        $rootScope.isFormVisible = false;
                        toastr.success('Your record is successfully deleted', 'Success');
                        $scope.RefreshTable();
                    } else if (result.data.MessageType == 2) { // 1:Warning
                        toastr.warning(result.data.Message, 'Warning!');
                    } else {
                        toastr.error(result.data.Message, 'Opps, Something went wrong');
                    }
                }
                else {
                    $rootScope.redirectToLogin();
                }
            });
        }
        
        // Contact in Edit Mode
        $scope.UpdateContact = function (contact) {
            $scope.selectGroupName(contact.GroupName);
       
            $rootScope.isFormVisible = true;

            $scope.editData = {
                ContactId: contact.ContactId,
                fullName: contact.Name,
                companyName: contact.CompanyName,
                phoneNo: contact.PhoneNo,
                landlineNo: contact.LandlineNo,
                address1: contact.Address1,
                address2: contact.Address2,
                area: contact.Area,
                city: contact.City,
                pincode: contact.Pincode,
                contactNote: contact.Note,
                Language: contact.LangId,
                
            }

            $scope.lastContact = {
                ContactId: contact.ContactId,
                Name: contact.Name,
                CompanyName: contact.CompanyName,
                PhoneNo: contact.PhoneNo,
                LandlineNo: contact.LandlineNo,
                Address1: contact.Address1,
                Address2: contact.Address2,
                Area: contact.Area,
                City: contact.City,
                Pincode: contact.Pincode,
                Note: contact.Note,
                LangId: contact.LangId,
               
            }
            $scope.mode = "Edit";
            $scope.SaveText = "Update";


        }

        // List Contact Records

        $scope.RetriveContacts = function () {
            
            $scope.tableParams = new ngTableParams({
                page: 1,
                count: 10,
                sorting: {
                    Name: 'asc',
                },
                defaultSort: 'asc'
            }, {
                total: 0,
                filterDelay: 750,
                    getData: function ($defer, params) {
                        $rootScope.IsAjaxLoading = true;
                        $scope.isRecord = false;
                       
                        if ($scope.filterData == "") {
                            GroupIdList = params.filter().ddlGroupName+",";
                        }
                        else {
                            GroupIdList = $scope.filterData;
                        }
                   
                        ABContactService.RetriveContacts($scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter().Name, params.filter().AddressCity, params.filter().PhoneNo, GroupIdList, $scope.Languagemode).then(function (result) {
                            if (result.data.IsValidUser) {
                                //display no data message
                                if (result.data.MessageType != 0) { // 0:Error
                                    if (result.data.DataList.total == 0) {
                                        $scope.noRecord = true;
                                        $scope.filterText = params.filter().Name;
                                    } else {
                                        $scope.noRecord = false;
                                    }
                                    params.total(result.data.DataList.total);
                                    $defer.resolve($scope.Contacts = result.data.DataList.result);
                                    $scope.Contacts = result.data.DataList.result;
                                    $scope.checkboxes.checked = false;
                                    
                                    $rootScope.IsAjaxLoading = false;
                                }
                                else {
                                    toastr.error(result.data.Message, 'Opps, Something went wrong');
                                }
                            }
                            else {
                                $rootScope.redirectToLogin();
                            }
                        })
                    }
            })
        }
        $scope.RetriveContacts();


        $scope.checkboxes = { 'checked': false, items: {} };

        $scope.selectAll = function () {

            // $scope.checkboxes.checked = false;
                angular.forEach($scope.Contacts, function (contact) {

                    var idx = $scope.contactSelection.indexOf(contact.ContactId);
                    // is currently selected
                    if (idx > -1) {
                        $scope.contactSelection.splice(idx, 1);
                    }
                });
                if (!$scope.checkboxes.checked) {
                    
                angular.forEach($scope.Contacts, function (contact) {
                    $scope.checkboxes.items[contact.ContactId] = true;
                    $scope.contactSelection.push(contact.ContactId);

                });
            }
            else {
                angular.forEach($scope.Contacts, function (contact) {
                    $scope.checkboxes.items[contact.ContactId] = false;
                });
            }
        };

               
               

        // Reset Contact Form
        $scope.ResetContact = function () {
        
            if ($scope.mode === "Add") {
                $scope.editData = {
                    fullName: '',
                    companyName: '',
                    phoneNo: '',
                    landlineNo: '',
                    address1: '',
                    address2: '',
                    area: '',
                    city: '',
                    pincode: '',
                    contactNote: '',
                    Language: 0,
                    isActive: true,
                }
                
                $scope.frmContact.$setPristine();
                $scope.isFirstFocus = true;
            }
            else if ($scope.mode === "Edit") {
                $scope.editData = {
                    ContactId: $scope.lastContact.ContactId,
                    fullName: $scope.lastContact.Name,
                    companyName: $scope.lastContact.CompanyName,
                    phoneNo: $scope.lastContact.PhoneNo,
                    landlineNo: $scope.lastContact.LandlineNo,
                    address1: $scope.lastContact.Address1,
                    address2: $scope.lastContact.Address2,
                    area: $scope.lastContact.Area,
                    city: $scope.lastContact.City,
                    pincode: $scope.lastContact.Pincode,
                    contactNote: $scope.lastContact.Note,
                    Language: $scope.lastContact.LangId,
                    isActive: $scope.lastContact.IsActive,
                }
            }
        }
       
        // Close Contact Form

        $scope.CloseContact = function () {
            $rootScope.isFormVisible = false;
            $scope.isFirstFocus = false;
        }
        $scope.CloseGroup = function () {
            $rootScope.isGroupFormVisible = false;
        }

        // Validate Language Dropdown
        $scope.validateDropLanguage = function () {
            if ($scope.editData.Language && $scope.editData.Language != 0) return false;
            return true;
        }

        // Refresh Contact List
        $scope.RefreshTable = function () {
            $scope.tableParams.reload();
          
        };
        
        // Export Excel
        $scope.ExportToExcel = function ()
        {
            document.location.href = "../../Handler/ABContact.ashx?timezone=" + $scope.timeZone + "&mode=Download"
        }
        $scope.UploadExcel = function ()
        {
            $rootScope.isFormVisible = false;
            $rootScope.isGroupFormVisible = false;
            $rootScope.isContactUploadFormVisible = true;
        }
        // import Excel
        $scope.ImportExcel = function () {           
            var filename = $scope.fileName;
           
            ABContactService.ImportContact(filename).success(function (result) {
                if (result.IsValidUser) {
                   
                    if (result.MessageType === 1) {
                       
                        $scope.RefreshTable();
                        $scope.isFirstFocus = false;
                        $scope.frmContact.$setPristine();
                        $rootScope.isFormVisible = false;
                        $rootScope.isContactUploadFormVisible = false;
                        toastr.success(result.Message, 'Success');
                        $scope.fileName="";
                    } else if (result.MessageType === 2) {
                        toastr.warning(result.Message, 'Record already exists');
                    }
                    else {
                        toastr.error(result.Message, 'Opps, Something went wrong');
                    }
                }
                else {
                    $rootScope.redirectToLogin();
                }

            });
        }

        // Close Upload Excel Form
        $scope.CloseUploadExcel = function () {
            $rootScope.isContactUploadFormVisible = false;
            $scope.fileName="";
        }

        $scope.getExcelUploadData = function (data) {
           
            if (data) {
                var parseData = JSON.parse(data);
                $scope.excelData = parseData;
                $scope.isDataLoaded = true;
          
            }
        };
        $scope.$watch('fileName', function (newValue, oldValue) {
                    if ($scope.isChanged && newValue != null) {
                        $scope.filePath = $scope.tempPath;
                        $scope.tempmode = "Save";
                    } else {
                        $scope.filePath = $scope.OriginPath;
                    }
        }, true);
        
        //set Font-Size
        $scope.SetFontSize = function (req)
        {
            
            if (req == "minus") {
              
                $scope.fontSize = $scope.fontSize - 2;
              
            }
            else if (req == "plus") {
                $scope.fontSize = $scope.fontSize + 2;
            }
            $('.fontsize').css({
                'font-size': $scope.fontSize + 'px',
                
            });
            $('.chkSize').css({
                'width': $scope.fontSize + 'px',
                'height': $scope.fontSize + 'px',
            })
            
        }
    }
]);