(function (app) {
    'use strict';

    app.controller('permissionCtrl', permissionCtrl);

    permissionCtrl.$inject = ['$scope', '$modal', 'apiService', 'membershipService', 'webApiLocationService', 'notificationService'];

    function permissionCtrl($scope, $modal, apiService, membershipService, webApiLocationService, notificationService) {

        $scope.pageClass = 'page-permission';
        $scope.page = 0;
        $scope.pagesCount = 0;
        $scope.Permissions = [];
        $scope.AddPermission = [];
        $scope.UserRoles = [];
        $scope.Forms = [];
        $scope.newPermission = {};
        $scope.SearchText = {};
      //  $scope.search = search;
        $scope.savePermission = savePermission;
        $scope.permissionClicked = permissionClicked;
        $scope.advancedSearch = advancedSearch;
        $scope.clearSearch = clearSearch;
        $scope.loadRoles = loadRoles;
        $scope.loadForms = loadForms;
     

        membershipService.redirectIfNotLoggedIn();
        var baseUrl = webApiLocationService.get('webapi');


     //   $scope.search();
        $scope.loadRoles();
        $scope.loadForms();
        //function search(page, searchItem) {
        //    if (!searchItem) {
        //        page = page || 0;

        //        var config = {
        //            params: {
        //                page: page,
        //                pageSize: 50,
        //                filter: $scope.filterRoles
        //            }
        //        };

        //        apiService.get(baseUrl + '/api/permissions/search/', config,
        //            permissionLoadCompleted,
        //            permissionLoadFailed);
        //    }
        //    else {

        //        $scope.advancedSearch(page, searchItem);
        //    }
          
        //}

        //function permissionLoadCompleted(result) {
        //    $scope.Permissions = result.data.Items;
        //    $scope.page = result.data.Page;
        //    $scope.pagesCount = result.data.TotalPages;
        //    $scope.totalCount = result.data.TotalCount;
        //    $scope.SearchText.RoleID = result.data.Items[0].RoleID;
        //}

        //function permissionLoadFailed(response) {
        //    notificationService.displayError(response.data);
        //}

        function permissionClicked(permission, actionPermission)
        {
            var newPermission = {};
                newPermission.RoleID = permission.RoleID;
                newPermission.FormID = permission.FormID;
                newPermission.Action = actionPermission.Action;
            if (actionPermission.Permission) {
                newPermission.IsPermission = 0;
            }
            else {
                newPermission.IsPermission = 1;
            }
            newPermission.ID = permission.ID;
            $scope.AddPermission.push(newPermission);


            
        }

     

        function savePermission()
        {
                           
            var UpdatePermissions = $scope.AddPermission;
          
           
            apiService.post(baseUrl + '/api/permissions/update/', UpdatePermissions,
               savePermissionCompleted,
               savePermissionFailed);
        }
        function savePermissionCompleted(response)
        {
            console.log(response);
            notificationService.displayInfo('Data Updated successfully');
        }
        function savePermissionFailed(response)
        {
            console.log(response);

        }
        
     //   $scope.advancedSearch();

        function advancedSearch(page, searchItem) {
            var item = searchItem
            if (searchItem != null) {

                if (angular.isUndefined(item.RoleID)) {
                    item.RoleID = -1;
                }

                if (angular.isUndefined(item.FormID)) {
                    item.FormID = -1;
                }

                var config = {
                    params: {
                        page: page,
                        pageSize: 100,
                        roleid: item.RoleID,
                        formid: item.FormID
                    }
                };


                apiService.get(baseUrl + '/api/permissions/advancedsearch/', config,
                   advancedSearchCompleted,
                   advancedSearchFailed);
            }
            //else {

            //    notificationService.displayError("Please select Search item");
            //}
        }

        function advancedSearchCompleted(result) {
            console.log(result.data);
            $scope.Permissions = result.data.Items;
            $scope.page = result.data.Page;
            $scope.pagesCount = result.data.TotalPages;
            $scope.totalCount = result.data.TotalCount;

           
            //$scope.loadingPermissions = false;

            //if ($scope.filterTests && $scope.filterTests.length) {
            //    notificationService.displayInfo(result.data.Items.length + ' test(s) found');
            //}

        }

        function advancedSearchFailed(response) {
            notificationService.displayError(response.data);
        }

        function clearSearch() {
            $scope.SearchText = {};
            search();
        }

        function loadRoles() {

            apiService.get(baseUrl + '/api/roles/', null,
            rolesLoadCompleted,
            rolesardLoadFailed);

        }

        function rolesLoadCompleted(result) {
            $scope.UserRoles = result.data;
            $scope.SearchText.RoleID = result.data[1].ID;
            advancedSearch(0, $scope.SearchText);
        }

        function rolesardLoadFailed(response) {
            notificationService.displayError(response.data);
        }


        //Load form is currently hidden
        function loadForms() {

            apiService.get(baseUrl + '/api/permissions/forms', null,
            formsLoadCompleted,
            formsLoadFailed);

        }

        function formsLoadCompleted(result) {
            $scope.Forms = result.data;

        }

        function formsLoadFailed(response) {
            notificationService.displayError(response.data);
        }

    }
})(angular.module('app-sysconfig'));