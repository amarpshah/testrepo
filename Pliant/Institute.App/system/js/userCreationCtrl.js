(function (app) {
    'use strict';

    app.controller('userCreationCtrl', userCreationCtrl);

    userCreationCtrl.$inject = ['$scope', 'membershipService', 'notificationService', '$rootScope', '$location', 'apiService', 'webApiLocationService', '$modal'];

    function userCreationCtrl($scope, membershipService, notificationService, $rootScope, $location, apiService, webApiLocationService, $modal) {
        $scope.pageClass = 'page-login';
        var baseUrl = webApiLocationService.get('webapi');
        $scope.register = register;
        $scope.page = 0;
        $scope.pagesCount = 0;
        $scope.user = {};
        $scope.Users = [];
        $scope.UserRoles = [];
        $scope.search = search;
        $scope.loadRoles = loadRoles;
        $scope.advancedSearch = advancedSearch;
        $scope.editUser = editUser;
        $scope.updateUser = updateUser;
        $scope.clearSearch = clearSearch;
        $scope.clearUser = clearUser;
        $scope.isValid = isValid;
        $scope.IsEditMode = false;
        $scope.loadingUsers = true;

        membershipService.redirectIfNotLoggedIn();
        function register() {
            if ($scope.isValid()) {
                membershipService.register($scope.user, registerCompleted);
            }
        }

        function registerCompleted(result) {
            if (result.data.success) {
                notificationService.displaySuccess('Registration successful.');
              
            }
            else {
                notificationService.displayError('Registration failed. Try again.');
            }
        }

        //Validation function
        function isValid() {

            if (isNaN($scope.user.roleid)) {
                $scope.vRoleId = true;
            }

            if (angular.isUndefined($scope.user.username)) {
                $scope.vName = true;
           
            }
            if (angular.isUndefined($scope.user.password)) {
                $scope.vPassword = true;
            
            }
            if (angular.isUndefined($scope.user.email)) {
                $scope.vEmail = true;
                return false;
            }
            else {
                return true;
            }
        }
        
        function updateUser()
        {
            var updateUser = $scope.user;
            if (updateUser.password == null) {
                updateUser.password = "Pass";
            }
            apiService.post(baseUrl + '/api/account/update', updateUser,
                    updateSucceded,
                    updateFailed);
        }


        function updateSucceded(response) {
            console.log(response);
            $scope.clearUser();
            $scope.IsEditMode = false;
            $scope.search();
            notificationService.displayInfo('Data Updated successfully');

        }

        function updateFailed(response) {
            console.log(response);

            if (response.status == '400')
                notificationService.displayError(response.data);
            else
                notificationService.displayError(response.statusText);
        }
        function clearUser()
        {

            $scope.user = {};
            $scope.IsEditMode = false;
        }

        function editUser(user)
        {
            $scope.user.ID = user.ID;
            $scope.user.username = user.Username;
            $scope.user.password = user.Password;
            $scope.user.email = user.Email;
            $scope.user.roleid = user.RoleID;
            $scope.IsEditMode = true;
        }

        function search(page, searchItem) {
            
            if (!searchItem) {
                page = page || 0;
                
                $scope.loadingUsers = true;

                var config = {
                    params: {
                        page: page,
                        pageSize: 3,
                        filter: $scope.filterUsers
                    }
                };

                apiService.get(baseUrl + '/api/account/search/', config,
                usersLoadCompleted,
                usersLoadFailed);
            }
            else {

                $scope.advancedSearch(page, searchItem);
            }
        }

        function usersLoadCompleted(result) {
            $scope.Users = result.data.Items;

            $scope.page = result.data.Page;
            $scope.pagesCount = result.data.TotalPages;
            $scope.totalCount = result.data.TotalCount;
            $scope.loadingUsers = false;

            if ($scope.filterUsers && $scope.filterUsers.length) {
                notificationService.displayInfo(result.data.Items.length + ' users found');
            }
        }

        function usersLoadFailed(response) {
            notificationService.displayError(response.data);
        }

        function clearSearch() {
            $scope.filterUsers = '';
            search();
        }

        $scope.search();
        $scope.loadRoles();


        function loadRoles()
        {

            apiService.get(baseUrl + '/api/roles/', null,
            rolesLoadCompleted,
            rolesardLoadFailed);

        }

        function rolesLoadCompleted(result) {
            $scope.UserRoles = result.data;
           
        }

        function rolesardLoadFailed(response) {
            notificationService.displayError(response.data);
        }


        //Advance Search

        function advancedSearch(page, searchItem) {
            var item = searchItem
            if (searchItem != null) {

                if (angular.isUndefined(item.RoleId)) {
                    item.RoleId = -1;
                }
                if (angular.isUndefined(item.Username)) {
                    item.Username = "";
                }
              
                if (angular.isUndefined(item.Email)) {
                    item.Email = "";
                }


                var config = {
                    params: {
                        page: page,
                        pageSize: 5,
                        roleid: item.RoleId,
                        username: item.Username,
                        email: item.Email,


                    }
                };


                apiService.get(baseUrl + '/api/Account/advancedsearch/', config,
                   advancedSearchCompleted,
                   advancedSearchFailed);
            }
            else {

                notificationService.displayError("Please select Search item");
            }
        }

        function advancedSearchCompleted(result) {
            console.log(result.data);
            $scope.Users = result.data.Items;
            $scope.page = result.data.Page;
            $scope.pagesCount = result.data.TotalPages;
            $scope.totalCount = result.data.TotalCount;
            $scope.loadingSubjects = false;
                        
        }

        function advancedSearchFailed(response) {
            notificationService.displayError(response.data);
        }

        function clearSearch() {
            $scope.SearchText = {};
            search();
        }


    }
})(angular.module('app-sysconfig'));