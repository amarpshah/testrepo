(function (app) {
    'use strict';

    app.controller('permissionCtrl', permissionCtrl);

    permissionCtrl.$inject = ['$scope', '$modal', 'apiService', 'membershipService', 'webApiLocationService', 'notificationService'];

    function permissionCtrl($scope, $modal, apiService, membershipService, webApiLocationService, notificationService) {

        $scope.pageClass = 'page-permission';
        $scope.page = 0;
        $scope.pagesCount = 0;
        $scope.Permissions = [];
        $scope.search = search;
        $scope.savePermission = savePermission;

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
                    standardLoadCompleted,
                    standardLoadFailed);
          
        }

        function standardLoadCompleted(result) {
            $scope.Permissions = result.data.Items;
            $scope.page = result.data.Page;
            $scope.pagesCount = result.data.TotalPages;
            $scope.totalCount = result.data.TotalCount;
           
           
        }

        function standardLoadFailed(response) {
            notificationService.displayError(response.data);
        }

        function savePermission()
        {

            console.log("Save");
        }

    }
})(angular.module('app-sysconfig'));