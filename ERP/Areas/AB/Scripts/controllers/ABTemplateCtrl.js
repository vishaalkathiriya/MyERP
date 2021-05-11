angular.module("ERPApp.Controllers").controller("ABTemplateCtrl", [
    "$scope",
    "$rootScope",
    "ABTemplateService",
     "$http",
    "$filter",
    "ngTableParams",
    function ($scope, $rootScope, ABTemplateService, $http, $filter, ngTableParams) {
        $scope.editData = $scope.editData || {};
        $scope.lastTemplate = $scope.lastTemplate || {};
        $scope.mode = "Add";
        $scope.SaveText = "Save";
        $rootScope.isFormVisible = false;
        $scope.isFirstFocus = false;

        $scope.AddTemplate = function () {
            
            $scope.mode = "Add";
            $scope.SaveText = "Save";
            $scope.editData = {};
            $rootScope.isFormVisible = true;
           
        }

        // Create / Update Template
        $scope.CreateUpdateTemplate = function (template) {
            ABTemplateService.CreateUpdateTemplate(template).then(function (result) {
                if (result.data.IsValidUser) {
                    if (result.data.MessageType === 1) {
                        $scope.editData = {
                            templateName: '',
                            templateFormate: '',
                         

                        };
                        $scope.RefreshTable();

                        if ($scope.mode === "Edit") {
                            $rootScope.isFormVisible = false;
                        }
                        $scope.isFirstFocus = false;
                        $scope.frmTemplate.$setPristine();
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
        // Update Template Detail 

        $scope.UpdateTemplate = function (template) {

            $scope.editData = {
                templateId: template.TemplateId,
                templateName: template.TemplateName,
                templateFormate: template.TemplateFormate,
               
            };
            $scope.lastTemplate = {
                TemplateId: template.TemplateId,
                TemplateName: template.TemplateName,
                TemplateFormate: template.TemplateFormate,
                
            }

            $rootScope.isFormVisible = true;
            $scope.mode = "Edit";
            $scope.SaveText = "Update";
        }

        // Delete Template
        $scope.DeleteTemplate = function (TemplateId) {

            $rootScope.isFormVisible = false;
            ABTemplateService.DeleteTemplate(TemplateId).then(
               function (result) {
                   if (result.data.IsValidUser) {
                       if (result.data.MessageType == 1) { // 0:Error
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
      
        // Retrive Template List
        $scope.RetriveTemplates = function () {

            $scope.tableParams = new ngTableParams({
                page: 1,
                count: 10,
                sorting: {
                    TemplateName: 'asc'
                },
                defaultSort: 'asc'
            }, {
                total: 0,
                filterDelay: 750,
                getData: function ($defer, params) {
                    $rootScope.IsAjaxLoading = true;
                    $scope.isRecord = false;
                    ABTemplateService.RetriveTemplates($scope.timeZone, params.page(), params.count(), params.orderBy(), params.filter().TemplateName).then(function (result) {
                        if (result.data.IsValidUser) {
                            //display no data message
                            if (result.data.MessageType != 0) { // 0:Error
                                if (result.data.DataList.total == 0) {
                                    $scope.noRecord = true;
                                    $scope.filterText = params.filter().TemplateName;
                                } else {
                                    $scope.noRecord = false;
                                }
                                params.total(result.data.DataList.total);
                                $defer.resolve($scope.Templates = result.data.DataList.result);
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
            })
        }
        $scope.RetriveTemplates();


        //Close Template Form
        $scope.CloseTemplate = function () {
            $rootScope.isFormVisible = false;
            $scope.isFirstFocus = false;
            $scope.editData = {};
            $scope.frmTemplate.$setPristine();
        }

        //Reset Template Form
        $scope.ResetTemplate = function () {
            if ($scope.mode === "Add") {
                $scope.editData = {
                    templateName: '',
                    templateFormate: '',
                  
                }
            }
            else if ($scope.mode === "Edit") {
                $scope.editData = {
                    templateId: $scope.lastTemplate.TemplateId,
                    templateName: $scope.lastTemplate.TemplateName,
                    templateFormate: $scope.lastTemplate.TemplateFormate,
                 
                }
            }
            $scope.frmTemplate.$setPristine();
            $scope.isFirstFocus = true;
        }
        // Reload List Template 
        $scope.RefreshTable = function () {
            $scope.tableParams.reload();
        };
    }
]);