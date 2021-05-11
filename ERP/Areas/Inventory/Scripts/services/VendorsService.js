/// <reference path="../libs/angular/angular.js" />

angular.module("ERPApp.Services").service("VendorsService", [
    "$http",
    function ($http) {
        var Vendor = {};

        //Get Vendor List
        Vendor.GetVendorList = function (timezone, page, count, orderby, fltrVendorName, fltrCompanyName) {
            return $http({
                method: "GET",
                url: "/api/Vendors/GetVendorsList?ts=" + new Date().getTime() + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&VendorName=" + fltrVendorName + "&CompanyName=" + fltrCompanyName
                //url: "./api/Vendors/GetVendorsList?ts=" + new Date().getTime() + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&filter=" + filter
            });
        };

        //add New Vendor
        Vendor.SaveVendor = function (_vendor) {
            var vendor = {
                VendorId: _vendor.VendorId,
                VendorName: _vendor.VendorName,
                CompanyName: _vendor.CompanyName,
                Email: _vendor.Email,
                Website: _vendor.Website,
                Mobile: _vendor.Mobile,
                PhoneNo: _vendor.PhoneNo,
                Services: _vendor.Services,
                Rating: _vendor.Rating,
                HouseNo: _vendor.HouseNo,
                Location: _vendor.Location,
                Area: _vendor.Area,
                Country: _vendor.Country,
                State: _vendor.State,
                City: _vendor.City,
                PostalCode: _vendor.PostalCode,
                IsActive: _vendor.IsActive,
                CreBy: _vendor.IsActive,
                ChgBy: _vendor.ChgBy

            };
            return $http({
                method: "POST",
                url: "/api/vendors/SaveVendor?ts=" + new Date().getTime(),
                data: vendor,
                contentType: "application/json;charset=utf-8"
            });
        };

        /*Delete Vendor*/
        Vendor.DeleteVendor = function (_vendor) {
          
            var vendor = {
                VendorId: _vendor.VendorId,
                VendorName: _vendor.VendorName,
                CompanyName: _vendor.CompanyName,
                Email: _vendor.Email,
                Website: _vendor.Website,
                Services: _vendor.Services,
                Rating: _vendor.Rating,
                HouseNo: _vendor.HouseNo,
                Location: _vendor.Location,
                Area: _vendor.Area,
                Country: _vendor.Country,
                State: _vendor.State,
                City: _vendor.City,
                PostalCode: _vendor.PostalCode,
                IsActive: _vendor.IsActive
            };
            return $http({
                method: "POST",
                url: "/api/Vendors/DeleteVendor",
                data: vendor,
                contentType: 'application/json; charset=utf-8'
            });
        }

        Vendor.IsActive = function (_vendor) {
            //var vendor = {
            //    VendorId: _vendor.VendorId,
            //    VendorName: _vendor.VendorName,
            //    IsActive: _vendor.IsActive
            //};
            return $http({
                method: "POST",
                url: "/api/Vendors/ChangeStatus",
                data: _vendor,
                contentType: "application/json"
            });
        }


        Vendor.GetCountries = function () {
            return $http({
                method: 'GET',
                url: "/api/General/GetCountries",
                contentType: 'application/json; charset=utf-8'
            });
        }

        Vendor.GetStatesByCountry = function (CountryName) {
            var _country = { CountryName: CountryName }
            return $http({
                method: 'POST',
                url: "/api/General/GetStatesByCountry",
                contentType: 'application/json; charset=utf-8',
                data: _country
            });
        }
        return Vendor;
    }

]);