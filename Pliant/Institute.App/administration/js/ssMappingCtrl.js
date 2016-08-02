(function (app) {
    'use strict';

    app.controller('ssMappingCtrl', ssMappingCtrl);

    ssMappingCtrl.$inject = ['$scope', '$modal', 'apiService', 'membershipService', 'webApiLocationService', 'notificationService', '$stateParams', 'permissionService', 'constantStrService'];

    function ssMappingCtrl($scope, $modal, apiService, membershipService, webApiLocationService, notificationService, $stateParams, permissionService, constantStrService) {

        $scope.Mappings = [];
        $scope.Standards = [];
        $scope.Subjects = [];
        $scope.width = '100%';
        $scope.height = '100%';
        $scope.filterMapping = '';
        $scope.loadingMappings = true;
        $scope.loadSubject = loadSubject;
        $scope.loadStandard = loadStandard;
        $scope.search = search;
        $scope.AddMapping = AddMapping;
        $scope.deleteMapping = deleteMapping;
        $scope.advancedSearch = advancedSearch;
        $scope.clearSearch = clearSearch;

        var baseUrl = webApiLocationService.get('webapi');
        membershipService.redirectIfNotLoggedIn();
        $scope.permissionADDSTDSUBMAP = permissionService.get(constantStrService.ADD_STD_SUB_MAP());
        $scope.permissionADDTOTOPICSTDSUBMAP = permissionService.get(constantStrService.ADD_TO_TOPIC_STD_SUB_MAP());
        $scope.permissionDELETESTDSUBMAP = permissionService.get(constantStrService.DELETE_STD_SUB_MAP());

        //////////////////////////////////////////////

        function AddMapping() {

            var stds = $scope.standardOptions.api.getSelectedRows();
            var subj = $scope.subjectOptions.api.getSelectedRows();
            if (stds.length > 0 && subj.length > 0) {
                var allobj = [];
                for (var i = 0; i < subj.length; i++) {
                    var obj = {};
                    obj.StandardID = stds[0].ID;
                    obj.SubjectID = subj[i].ID;
                    obj.Standard = stds[0].Standard;
                    obj.Subject = subj[i].Subject;
                    obj.IsActive = true;
                    allobj.push(obj);
                }

                apiService.post(baseUrl + '/api/mapping/add', allobj,
                addSubjectSucceded,
                addSubjectFailed);
            }
            else {
                $scope.vMapping = true
            }
        }

        function addSubjectSucceded(response) {
            console.log(response);
            $scope.newSubject = response.data;
            notificationService.displayInfo('Subject added successfully');
            $scope.search();
            $scope.newSubject = {};
        }

        function addSubjectFailed(response) {
            console.log(response);

            if (response.status == '400')
                notificationService.displayError(response.data);
            else
                notificationService.displayError(response.statusText);
        }

        //////////////////////////////////////
        function loadStandard() {

            apiService.get(baseUrl + '/api/standards/', null,
                standardLoadCompleted,
                standardLoadFailed);

        }

        function standardLoadCompleted(result) {
            $scope.Standards = result.data;
            $scope.loadingStandards = false;
            $scope.standardOptions.api.setRowData(result.data);
        }

        function standardLoadFailed(response) {
            notificationService.displayError(response.data);
        }

        function loadSubject() {

            apiService.get(baseUrl + '/api/subjects/', null,
                subjectLoadCompleted,
                subjectLoadFailed);
        }

        function subjectLoadCompleted(result) {
            $scope.Subjects = result.data;
            $scope.loadingSubjects = false;
            $scope.subjectOptions.api.setRowData(result.data);
        }

        function subjectLoadFailed(response) {
            notificationService.displayError(response.data);
        }
        ///////////////////////////////////////////
        function search(page, searchItem) {

            if (!searchItem) {
                page = page || 0;
                $scope.loadingMappings = true;

                var config = {
                    params: {
                        page: page,
                        pageSize: 10,
                        filter: $scope.filterMapping
                    }
                };

                apiService.get(baseUrl + '/api/mapping/search/', config,
                    mappingLoadCompleted,
                    mappingLoadFailed);
            }
            else {
                $scope.advancedSearch(page, searchItem);
            }
        }

        function mappingLoadCompleted(result) {
            $scope.Mappings = result.data.Items;
            $scope.page = result.data.Page;
            $scope.pagesCount = result.data.TotalPages;
            $scope.totalCount = result.data.TotalCount;
            $scope.loadingMappings = false;

            if ($scope.filterMapping && $scope.filterMapping.length) {
                notificationService.displayInfo(result.data.Items.length + ' mappings found');
            }

        }

        function mappingLoadFailed(response) {
            notificationService.displayError(response.data);
        }


        $scope.standardOptions = {
            columnDefs: [
                            { field: 'ID', displayName: 'Id', hide: true },
                            { field: 'Code', displayName: 'Standard Code', hide: true },
                            { field: 'Standard', displayName: 'Standard' },
                            { field: 'Division', displayName: 'Division' }
            ],
            rowData: null,
            enableSorting: true,
            angularCompileRows: true,            rowSelection: 'single'
        };

        $scope.subjectOptions = {
            columnDefs: [
                            { field: 'ID', displayName: 'Id', hide: true },
                            { field: 'Code', displayName: 'Subject Code', hide: true },
                            { field: 'Subject', displayName: 'Subject' }
            ],
            rowData: null,
            enableSorting: true,
            angularCompileRows: true,            rowSelection: 'multiple'
        };


        $scope.search();
        $scope.loadStandard();
        $scope.loadSubject();

        //Delete Mapping
        function deleteMapping(mapping) {
            if (mapping.TopicCount > 0) {
                notificationService.displayError("Mapping is associated with " + mapping.TopicCount + " topics");
            }
            else {
                var mappingid = mapping.ID;
                if (mappingid != null) {
                    var config = {
                        params: {
                            id: mappingid
                        }
                    };
                    apiService.post(baseUrl + '/api/mapping/delete/' + mappingid, null,
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

        //Advance Search

        function advancedSearch(page, searchItem) {
            var item = searchItem
            if (searchItem != null) {
                if (angular.isUndefined(item.StandardId)) {
                    item.StandardId = -1;
                }
                if (angular.isUndefined(item.SubjectId)) {
                    item.SubjectId = -1;
                }

                var config = {
                    params: {
                        page: page,
                        pageSize: 10,
                        standardid: item.StandardId,
                        subjectid: item.SubjectId,
                    }
                };

                apiService.get(baseUrl + '/api/mapping/advancedsearch/', config,
                   advancedSearchCompleted,
                   advancedSearchFailed);
            }
            else {
                notificationService.displayError("Please select Search item");
            }
        }

        function advancedSearchCompleted(result) {
            console.log(result.data);
            $scope.Mappings = result.data.Items;
            $scope.page = result.data.Page;
            $scope.pagesCount = result.data.TotalPages;
            $scope.totalCount = result.data.TotalCount;
            $scope.loadingMappings = false;
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