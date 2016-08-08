(function (app) {
    'use strict';

    app.controller('roleCreationCtrl', roleCreationCtrl);

    roleCreationCtrl.$inject = ['$scope', '$modal', 'apiService', 'membershipService', 'webApiLocationService', 'notificationService'];

    function roleCreationCtrl($scope, $modal, apiService, membershipService, webApiLocationService, notificationService) {

        $scope.register = register;
        $scope.filterRoles = '';
        $scope.page = 0;
        $scope.pagesCount = 0;
        $scope.Roles = [];
        $scope.search = search;
        $scope.loadingStandard = true;
        $scope.IsEditMode = false;
        $scope.newRole = {};
        $scope.addRole = addRole;
        $scope.deleteRole = deleteRole;
        $scope.editRoles = editRoles;
        $scope.updateRole = updateRole;
        $scope.clearStandard = clearStandard;
        $scope.advancedSearch = advancedSearch;
        $scope.clearSearch = clearSearch;
        $scope.clearRole = clearRole

        membershipService.redirectIfNotLoggedIn();
        var baseUrl = webApiLocationService.get('webapi');


        $scope.search();
        function search(page, searchItem) {
            if (!searchItem) {
                page = page || 0;

                $scope.loadingStandard = true;

                var config = {
                    params: {
                        page: page,
                        pageSize: 20,
                        filter: $scope.filterRoles
                    }
                };

                apiService.get(baseUrl + '/api/roles/search/', config,
                    standardLoadCompleted,
                    standardLoadFailed);
            }
            else {
                $scope.advancedSearch(page, searchItem);
            }
        }

        function standardLoadCompleted(result) {
            $scope.Roles = result.data.Items;
            $scope.page = result.data.Page;
            $scope.pagesCount = result.data.TotalPages;
            $scope.totalCount = result.data.TotalCount;
            $scope.loadingStandard = false;


            if ($scope.filterRoles && $scope.filterRoles.length) {
                notificationService.displayInfo(result.data.Items.length + ' Roles found');
            }

        }

        function standardLoadFailed(response) {
            notificationService.displayError(response.data);
        }

        //Add Role

        function addRole() {
            if ($scope.register()) {
                apiService.post(baseUrl + '/api/roles/add', $scope.newRole,
               addRoleSucceded,
               addRoleFailed);
            }
        }

        function addRoleSucceded(response) {
            console.log(response);
            $scope.newRole = response.data;
            notificationService.displayInfo('Role added successfully');
            $scope.search();
            $scope.newRole = {};
        }

        function addRoleFailed(response) {
            console.log(response);

            if (response.status == '400')
                notificationService.displayError(response.data);
            else
                notificationService.displayError(response.statusText);
        }

        //Validation function
        function register() {


            if (angular.isUndefined($scope.newRole.Code)) {
                $scope.vCode = true;

            }

            if (angular.isUndefined($scope.newRole.Name)) {
                $scope.vName = true;
                return false;
            }

            else {
                return true;
            }
        }

        //Delete Role
        function deleteRole(role) {
            if (role.UserCount > 0) {
                notificationService.displayError("Role is assigned to "+ role.UserCount + " users");
            }
            else {
                var roleid = role.ID
                if (roleid != null) {
                    var config = {
                        params: {
                            id: roleid
                        }
                    };
                    apiService.post(baseUrl + '/api/roles/delete/' + roleid, null,
                deleteSucceded,
                deleteFailed);
                }
            }
        }

        function deleteSucceded(response) {
            console.log(response);
            notificationService.displayInfo('Deleted successfully');
        }

        /* on click of the remove button */
        $(document).on('click', '.deleteQuestion', function (e) {
            e.preventDefault();
            var okDelete = confirm("Are you sure?")
            if (okDelete) {
                $(this).closest('.questionItem').remove();
            }
        })
        function deleteFailed(response) {
            console.log(response);

            if (response.status == '400')
                notificationService.displayError(response.data);
            else
                notificationService.displayError(response.statusText);
        }

        // Edit Role
        function editRoles(role) {
            $scope.IsEditMode = true;
            $scope.newRole.ID = role.ID;
            $scope.newRole.Code = role.Code;
            $scope.newRole.Name = role.Name;
        }

        //Update Role
        function updateRole() {

            var newRole = $scope.newRole;
            apiService.post(baseUrl + '/api/roles/update', newRole,
                    updateSucceded,
                    updateFailed);
        }


        function updateSucceded(response) {
            console.log(response);
            $scope.clearStandard();
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
        function clearStandard() {
            $scope.newRole = {};
            $scope.IsEditMode = false;
        }


        //Advance Search

        function advancedSearch(page, searchItem) {
            var item = searchItem
            if (searchItem != null) {


                if (angular.isUndefined(item.Code)) {
                    item.Code = "";
                }
                if (angular.isUndefined(item.Name)) {
                    item.Name = "";
                }

                var config = {
                    params: {
                        page: page,
                        pageSize: 20,
                        code: item.Code,
                        name: item.Name
                    }
                };


                apiService.get(baseUrl + '/api/roles/advancedsearch/', config,
                   advancedSearchCompleted,
                   advancedSearchFailed);
            }
            else {

                notificationService.displayError("Please select Search item");
            }
        }

        function advancedSearchCompleted(result) {
            console.log(result.data);
            $scope.Roles = result.data.Items;
            $scope.page = result.data.Page;
            $scope.pagesCount = result.data.TotalPages;
            $scope.totalCount = result.data.TotalCount;
            $scope.loadingStandards = false;
        }

        function advancedSearchFailed(response) {
            notificationService.displayError(response.data);
        }

        function clearSearch() {
            $scope.SearchText = {};
            search();
        }
        function clearRole() {
            $scope.newRole = {};
        }
    }
})(angular.module('app-sysconfig'));