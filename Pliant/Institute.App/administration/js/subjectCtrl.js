(function (app) {
    'use strict';

    app.controller('subjectCtrl', subjectCtrl);

    subjectCtrl.$inject = ['$scope', '$modal', 'apiService', 'membershipService', 'webApiLocationService', 'notificationService', 'permissionService', 'constantStrService'];

    function subjectCtrl($scope, $modal, apiService, membershipService, webApiLocationService, notificationService, permissionService, constantStrService) {

        $scope.search = search;
        $scope.page = 0;
        $scope.pagesCount = 0;
        $scope.Subjects = [];
        $scope.loadingSubjects = true;
        $scope.addSubject = addSubject;
        $scope.deleteSubject = deleteSubject;
        $scope.editSubject = editSubject;
        $scope.updateSubject = updateSubject;
        $scope.newSubject = {};
        $scope.IsEditMode = false;
        $scope.clearSubject = clearSubject;
        $scope.register = register
        $scope.advancedSearch = advancedSearch;
        $scope.clearSearch = clearSearch;


        membershipService.redirectIfNotLoggedIn();

        var baseUrl = webApiLocationService.get('webapi');
        $scope.permissionADDSUBJECT = permissionService.get(constantStrService.ADD_SUBJECT());
        $scope.permissionUPDATESUBJECT = permissionService.get(constantStrService.UPDATE_SUBJECT());
        $scope.permissionDELETESUBJECT = permissionService.get(constantStrService.DELETE_SUBJECT());

        // Show Subject List
        $scope.search();
        function search(page, searchItem) {

            if (!searchItem) {
                page = page || 0;

                $scope.loadingSubjects = true;

                var config = {
                    params: {
                        page: page,
                        pageSize: 5,
                        filter: $scope.filterSubjects
                    }
                };

                apiService.get(baseUrl + '/api/subjects/search/', config,
                    standardLoadCompleted,
                    standardLoadFailed);
            }
            else {

                $scope.advancedSearch(page, searchItem);
            }
        }

        function standardLoadCompleted(result) {
            $scope.Subjects = result.data.Items;
            //alert($scope.Subjects.length);

            $scope.page = result.data.Page;
            $scope.pagesCount = result.data.TotalPages;
            $scope.totalCount = result.data.TotalCount;
            $scope.loadingSubjects = false;


            if ($scope.filterSubjects && $scope.filterSubjects.length) {
                notificationService.displayInfo(result.data.Items.length + ' subjects found');
            }
        }

        function standardLoadFailed(response) {
            notificationService.displayError(response.data);
        }


        //Add Subjects

        function addSubject() {
            if ($scope.register()) {
                apiService.post(baseUrl + '/api/subjects/add', $scope.newSubject,
               addSubjectSucceded,
               addSubjectFailed);
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

        //Delete Subject
        function deleteSubject(subjectid) {

            if (subjectid != null) {
                var config = {
                    params: {
                        id: subjectid
                    }
                };
                apiService.post(baseUrl + '/api/subjects/delete/' + subjectid, null,
            deleteSucceded,
            deleteFailed);



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

        //***********Edit Subject*Starts*****************************************//

        function editSubject(subject) {
            $scope.IsEditMode = true;
            $scope.newSubject.ID = subject.ID;
            $scope.newSubject.Code = subject.Code;
            $scope.newSubject.Subject = subject.Subject;

        }
        //***********Edit Subject*End*****************************************//



        //Update Subject
        function updateSubject() {

            var newSubject = $scope.newSubject;
            apiService.post(baseUrl + '/api/subjects/update', newSubject,
                    updateSubjectSucceded,
                    updateSubjecFailed);
        }


        function updateSubjectSucceded(response) {
            console.log(response);
            $scope.clearSubject();
            $scope.IsEditMode = false;
            $scope.search();
            notificationService.displayInfo('Data Updated successfully');

        }

        function updateSubjecFailed(response) {
            console.log(response);

            if (response.status == '400')
                notificationService.displayError(response.data);
            else
                notificationService.displayError(response.statusText);
        }
        function clearSubject() {
            $scope.newSubject = {};
        }

        //Validation function
        function register() {


            if (angular.isUndefined($scope.newSubject.Code)) {
                $scope.vCode = true;

            }

            if (angular.isUndefined($scope.newSubject.Subject)) {
                $scope.vName = true;
                return false;
            }

            else {
                return true;
            }
        }


        //Advance Search

        function advancedSearch(page, searchItem) {
            var item = searchItem
            if (searchItem != null) {


                if (angular.isUndefined(item.Code)) {
                    item.Code = "";
                }
                if (angular.isUndefined(item.Subject)) {
                    item.Subject = "";
                }

               
                var config = {
                    params: {
                        page: page,
                        pageSize: 5,
                        code: item.Code,
                        subject: item.Subject,
                       

                    }
                };


                apiService.get(baseUrl + '/api/subjects/advancedsearch/', config,
                   advancedSearchCompleted,
                   advancedSearchFailed);
            }
            else {

                notificationService.displayError("Please select Search item");
            }
        }

        function advancedSearchCompleted(result) {
            console.log(result.data);
            $scope.Subjects = result.data.Items;
            $scope.page = result.data.Page;
            $scope.pagesCount = result.data.TotalPages;
            $scope.totalCount = result.data.TotalCount;
            $scope.loadingSubjects = false;

            //if ($scope.filterTopic && $scope.filterTopic.length) {
            //    notificationService.displayInfo(result.data.Items.length + ' topics(s) found');
            //}

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