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
        $scope.newPermission = {}
        $scope.search = search;
        $scope.savePermission = savePermission;
        $scope.permissionClicked = permissionClicked;
     

        membershipService.redirectIfNotLoggedIn();
        var baseUrl = webApiLocationService.get('webapi');


        $scope.search();
        function search(page) {
           
                page = page || 0;

                var config = {
                    params: {
                        page: page,
                        pageSize: 20,
                        filter: $scope.filterRoles
                    }
                };

                apiService.get(baseUrl + '/api/permissions/search/', config,
                    permissionLoadCompleted,
                    permissionLoadFailed);
          
        }

        function permissionLoadCompleted(result) {
            $scope.Permissions = result.data.Items;
            $scope.page = result.data.Page;
            $scope.pagesCount = result.data.TotalPages;
            $scope.totalCount = result.data.TotalCount;
           
           
        }

        function permissionLoadFailed(response) {
            notificationService.displayError(response.data);
        }

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
        

    }
})(angular.module('app-sysconfig'));