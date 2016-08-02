(function (app) {
    'use strict';

    app.controller('standardCtrl', standardCtrl);

    standardCtrl.$inject = ['$scope', '$modal', 'apiService', 'membershipService', 'webApiLocationService', 'notificationService', 'permissionService', 'constantStrService'];

    function standardCtrl($scope, $modal, apiService, membershipService, webApiLocationService, notificationService, permissionService, constantStrService) {

        $scope.register = register;
        $scope.filterStudents = '';
        $scope.page = 0;
        $scope.pagesCount = 0;
        $scope.Standards = [];
        $scope.search = search;
        $scope.loadingStandard = true;
        $scope.IsEditMode = false;
        $scope.newStandard = {};
        $scope.addStandard = addStandard;
        $scope.deleteStandard = deleteStandard;
        $scope.editStandard = editStandard;
        $scope.updateStandard = updateStandard;
        $scope.clearStandard = clearStandard;
        $scope.advancedSearch = advancedSearch;
        $scope.clearSearch = clearSearch;

        membershipService.redirectIfNotLoggedIn();
        var baseUrl = webApiLocationService.get('webapi');
        $scope.permissionADDSTD = permissionService.get(constantStrService.ADD_STANDARD());
        $scope.permissionUPDATESTD = permissionService.get(constantStrService.UPDATE_STANDARD());
        $scope.permissionDELETESTD = permissionService.get(constantStrService.DELETE_STANDARD());

        $scope.search();
        function search(page, searchItem) {
            if (!searchItem) {
                page = page || 0;
                $scope.loadingStandard = true;

                var config = {
                    params: {
                        page: page,
                        pageSize: 5,
                        filter: $scope.filterStudents
                    }
                };

                apiService.get(baseUrl + '/api/standards/search/', config,
                    standardLoadCompleted,
                    standardLoadFailed);
            }
            else {
                $scope.advancedSearch(page, searchItem);
            }
        }

        function standardLoadCompleted(result) {
            $scope.Standards = result.data.Items;
            $scope.page = result.data.Page;
            $scope.pagesCount = result.data.TotalPages;
            $scope.totalCount = result.data.TotalCount;
            $scope.loadingStandard = false;
            if ($scope.filterStudents && $scope.filterStudents.length) {
                notificationService.displayInfo(result.data.Items.length + ' Standards found');
            }
        }

        function standardLoadFailed(response) {
            notificationService.displayError(response.data);
        }

        //Add Standard

        function addStandard() {
            if ($scope.register()) {
                apiService.post(baseUrl + '/api/standards/add', $scope.newStandard,
               addStandardSucceded,
               addStandardFailed);
            }
        }

        function addStandardSucceded(response) {
            console.log(response);
            $scope.newStandard = response.data;
            notificationService.displayInfo('Standard added successfully');
            $scope.search();
            $scope.newStandard = {};
        }

        function addStandardFailed(response) {
            console.log(response);
            if (response.status == '400')
                notificationService.displayError(response.data);
            else
                notificationService.displayError(response.statusText);
        }

        //Validation function
        function register() {
            if (angular.isUndefined($scope.newStandard.Code)) {
                $scope.vCode = true;
            }
            if (angular.isUndefined($scope.newStandard.Standard)) {
                $scope.vName = true;
                return false;
            }
            else {
                return true;
            }
        }

        //Delete Standard
        function deleteStandard(standard) {
            if (standard.SubjectCount > 0) {
                notificationService.displayError("Standard is associated with " + standard.SubjectCount + " subjects");
            }

            else {
                var standardid = standard.ID
                if (standardid != null) {
                    var config = {
                        params: {
                            id: standardid
                        }
                    };
                    apiService.post(baseUrl + '/api/standards/delete/' + standardid, null,
                deleteSucceded,
                deleteFailed);
                }
            }
        }


        function deleteSucceded(response) {
            console.log(response);
            notificationService.displayInfo('Deleted successfully');
        }

        function deleteFailed(response) {
            console.log(response);
            if (response.status == '400')
                notificationService.displayError(response.data);
            else
                notificationService.displayError(response.statusText);
        }

        // Edit Standard
        function editStandard(standard) {
            $scope.IsEditMode = true;
            $scope.newStandard.ID = standard.ID;
            $scope.newStandard.Code = standard.Code;
            $scope.newStandard.Standard = standard.Standard;
            $scope.newStandard.Division = standard.Division;
        }

        //Update Standard
        function updateStandard() {
            var newStandard = $scope.newStandard;
            apiService.post(baseUrl + '/api/standards/update', newStandard,
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
            $scope.newStandard = {};
            $scope.IsEditMode = false;
        }


        //Advance Search

        function advancedSearch(page, searchItem) {
            var item = searchItem
            if (searchItem != null) {
                if (angular.isUndefined(item.Code)) {
                    item.Code = "";
                }
                if (angular.isUndefined(item.Standard)) {
                    item.Standard = "";
                }
                if (angular.isUndefined(item.Division)) {
                    item.Division = "";
                }

                var config = {
                    params: {
                        page: page,
                        pageSize: 5,
                        code: item.Code,
                        standard: item.Standard,
                        division: item.Division,
                    }
                };

                apiService.get(baseUrl + '/api/standards/advancedsearch/', config,
                   advancedSearchCompleted,
                   advancedSearchFailed);
            }
            else {
                notificationService.displayError("Please select Search item");
            }
        }

        function advancedSearchCompleted(result) {
            console.log(result.data);
            $scope.Standards = result.data.Items;
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
    }

})(angular.module('app-administration'));