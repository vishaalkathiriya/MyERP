angular.module("ERPApp.Services")
    .service("ABTemplateService", [
    "$http",
    function ($http) {
        var template = {};
       
        template.CreateUpdateTemplate = function (_template) {

            var template = {
                TemplateId: _template.templateId,
                TemplateName: _template.templateName,
                TemplateFormate: _template.templateFormate,
                IsActive: _template.isActive,
            }

            return $http({
                method: "Post",
                url: "/api/ABTemplate/CreateUpdateTemplate?ts=" + new Date().getTime(),
                data: template,
                contentType: "application/json"
            });
        }

        template.RetriveTemplates = function (timezone, page, count, orderby, filter) {
            return $http({
                method: "GET",
                cache: false,
                url: "/api/ABTemplate/RetriveTemplate?ts=" + new Date().getTime() + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&filter=" + filter,
            });
        }
      
        template.DeleteTemplate = function (TemplateId) {
            return $http({
                method: "POST",
                url: "/api/ABTemplate/DeleteTemplate?ts=" + new Date().getTime(),
                data: TemplateId,
                contentType: "application/json"
            });
        }
       

        return template;
    }
    ]);