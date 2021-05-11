
angular.module("ERPApp.Services")
    .service("ABContactService", [
    "$http",
    function ($http) {
        var contact = {}
        contact.CreateUpdateContact = function (_contact, _selectedGroup)
        {
                       
            var contact = {
                ContactId:_contact.ContactId,
                Name:_contact.fullName,
                PhoneNo: _contact.phoneNo,
                LandLineNo: _contact.landlineNo,
                Address1: _contact.address1,
                Address2: _contact.address2,
                Area: _contact.area,
                City: _contact.city,
                Pincode: _contact.pincode,
                CompanyName: _contact.companyName,
                Note: _contact.contactNote,
                LangId: _contact.Language,
                IsActive: _contact.isActive,                
            }

            return $http({
                method: "Post",
                url: "/api/ABContact/CreateUpdateContact?ts=" + new Date().getTime()+"&selectedGroup="+_selectedGroup.join(),
                data:contact,
                contentType:"application/json"
            });
        }

        contact.ApplyGroupcontact = function (_selectedGroup, _selectedContact)
        {
            return $http({
                method: "POST",
                url: "/api/ABContact/ApplyGroupcontact?ts=" + new Date().getTime() + "&selectedGroup=" + _selectedGroup.join() + "&selectedContact=" + _selectedContact.join(),
                contentType: "application/json"
            })
        }

        contact.RetriveContacts = function (timezone, page, count, orderby, Name, address, PhoneNo, GroupName, LangId) {
          
            return $http({
                method: "GET",
                cache: false,
                url: "/api/ABContact/RetrieveContact?ts=" + new Date().getTime() + "&timezone=" + timezone + "&page=" + page + "&count=" + count + "&orderby=" + orderby + "&name=" + Name + "&address=" + address + "&phoneNo=" + PhoneNo + "&groupName=" + GroupName + "&langId=" + LangId
            });
        }

        contact.DeleteContact = function (ContactId)
        {
           
            return $http({
                method: "Post",
                url: "/api/ABContact/DeleteContact?ts=" + new Date().getTime(),
                data: ContactId,
                contentType:"application/json"
            })
        }

        contact.GroupList = function () {
            return $http({
                method: "GET",
                url: "/api/ABGroup/GetGroupList?ts=" + new Date().getTime()
            });
        }

        contact.languageList = function () {
            return $http({
                method: "GET",
                url: "/api/ABLanguage/GetlanguageList?ts=" + new Date().getTime()
            });
        }

        contact.ImportContact = function (_fileName) {
            return $http({
                method: "POST",
                url: "/api/ABContact/ImportContact?ts=" + new Date().getTime() + "&fileName=" + _fileName,
                contentType: "application/json"
            })
        }

        return contact;
    }
]);