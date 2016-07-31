(function (app) {
    'use strict';

    app.controller('topicCtrl', topicCtrl);

    topicCtrl.$inject = ['$scope', '$modal', 'apiService', 'webApiLocationService', 'notificationService', 'membershipService', '$cookieStore', 'dataProviderService', '$stateParams', 'permissionService', 'constantStrService'];

    function topicCtrl($scope, $modal, apiService, webApiLocationService, notificationService, membershipService, $cookieStore, dataProviderService, $stateParams, permissionService, constantStrService) {

        $scope.pageClass = 'page-test';
        $scope.filterTopic = '';
        $scope.Topics = [];
        $scope.newTopic = {};
        $scope.loadingTopics = true;
        $scope.page = 0;
        $scope.pagesCount = 0;
        $scope.search = search;
        $scope.loadStandard = loadStandard;
        $scope.loadSubject = loadSubject;
        $scope.clearSearch = clearSearch;
        $scope.ShowAdvancedSearch = ShowAdvancedSearch;
        $scope.advancedSearch = advancedSearch;
        $scope.deleteTopic = deleteTopic;
        $scope.topic = {};
        $scope.topic.topicstandard = [];
        $scope.topic.topicsubject = [];
        $scope.StandardChange = StandardChange;
        $scope.SubjectChange = SubjectChange;
        $scope.StandardChangeSearch = StandardChangeSearch;
        $scope.Standards = [];
        $scope.Subjects = [];
        $scope.addTopic = addTopic;
        $scope.editTopic = editTopic;
        $scope.updateTopic = updateTopic;
        $scope.Register = Register;
        $scope.IsEditMode = false;
        $scope.ClearTopic = ClearTopic;

        membershipService.redirectIfNotLoggedIn();
        var userInfo = $cookieStore.get('repository');
        $scope.userName = userInfo.loggedUser.username;

        var userId = userInfo.loggedUser.userid;

        // From standard Subject
        $scope.newTopic.StandardId = parseInt($stateParams.stdid);
        $scope.newTopic.SubjectId = parseInt($stateParams.subid);
        $scope.newTopic.MappingID = parseInt($stateParams.mapid);
        $scope.checkId = userId;

        var baseUrl = webApiLocationService.get('webapi');
        $scope.permissionADDTOPIC = permissionService.get(constantStrService.ADD_TOPIC());
        $scope.permissionUPDATETOPIC = permissionService.get(constantStrService.UPDATE_TOPIC());
        $scope.permissionDELETETOPIC = permissionService.get(constantStrService.DELETE_TOPIC());
        $scope.permissionADDTOQUESTIONTOPIC = permissionService.get(constantStrService.ADD_TO_QUESTION_TOPIC());

        function search(page, searchItem) {
            if (!searchItem) {
                page = page || 0;
                $scope.loadingTopic = true;

                var config = {
                    params: {
                        page: page,
                        pageSize: 5,
                        filter: $scope.filterTopic
                    }
                };

                apiService.get(baseUrl + '/api/topic/search/', config,
                    topicLoadCompleted,
                    topicLoadFailed);
            }
            else {
                $scope.advancedSearch(page, searchItem)
            }
        }

        function topicLoadCompleted(result) {
            $scope.Topics = result.data.Items;
            $scope.page = result.data.Page;
            $scope.pagesCount = result.data.TotalPages;
            $scope.totalCount = result.data.TotalCount;
            $scope.loadingTopic = false;
            if ($scope.filterTopic && $scope.filterTopic.length) {
                notificationService.displayInfo(result.data.Items.length + ' topics found');
            }
        }

        function topicLoadFailed(response) {
            notificationService.displayError(response.data);
        }
        $scope.search();
        $scope.loadStandard();
        $scope.loadSubject();

        //Load Subject and  Standard
        function loadStandard() {
            apiService.get(baseUrl + '/api/standards/', null,
                standardLoadCompleted,
                standardLoadFailed);
        }

        function standardLoadCompleted(result) {
            $scope.topic.topicstandard = result.data;
            $scope.Standards = result.data;
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
            $scope.topic.topicsubject = result.data;
            $scope.Subjects = result.data;
        }

        function subjectLoadFailed(response) {
            notificationService.displayError(response.data);
        }


        //Delete Topic
        function deleteTopic(topicid) {
            if (topicid != null) {
                var config = {
                    params: {
                        id: topicid
                    }
                };
                apiService.post(baseUrl + '/api/topic/delete/' + topicid, null,
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


        ///START*************** Event Handlers ****************************
        function StandardChange() {
            var config = {
                params: {
                    stdid: $scope.newTopic.StandardId
                }
            };

            apiService.get(baseUrl + '/api/subjects/filtersubject', config,
                standardChangeLoadCompleted,
                standardChangeLoadFailed);

            //Once Standard is changed we need to reset the subject
            $scope.newTopic.SubjectId = -1;
        }

        function standardChangeLoadCompleted(result) {
            $scope.Subjects = result.data.Items;
            if ($scope.Subjects.length == 0) {
                $scope.newTopic.SubjectId = -1;
            }
        }

        function standardChangeLoadFailed(response) {
            notificationService.displayError(response.data);
        }

        function SubjectChange() {
            var config = {
                params: {
                    stdid: $scope.newTopic.StandardId,
                    subid: $scope.newTopic.SubjectId
                }
            };

            apiService.get(baseUrl + '/api/mapping/filtermappings/', config,
                mappingLoadCompleted,
                mappingLoadFailed);

        }

        function mappingLoadCompleted(result) {
            var Mappings = result.data.Items;

            //if (Mappings.length > 0)
            $scope.newTopic.MappingID = Mappings[0]['ID'];

        }

        function mappingLoadFailed(response) {
            notificationService.displayError(response.data);
        }


        //For Search Standard
        function StandardChangeSearch() {
            var config = {
                params: {
                    stdid: $scope.SearchText.StandardId
                }
            };

            apiService.get(baseUrl + '/api/subjects/filtersubject', config,
                standardChangeSearchLoadCompleted,
                standardChangeSearchLoadFailed);

            //Once Standard is changed we need to reset the subject
            $scope.SearchText.SubjectId = -1;
        }

        function standardChangeSearchLoadCompleted(result) {
            $scope.topic.topicsubject = result.data.Items;
            if ($scope.Subjects.length == 0) {
                $scope.SearchText.SubjectId = -1;
            }
        }

        function standardChangeSearchLoadFailed(response) {
            notificationService.displayError(response.data);
        }





        //******************ADD Topic Start***************************************

        function addTopic() {
            {
                if ($scope.Register()) {
                    apiService.post(baseUrl + '/api/topic/add', $scope.newTopic,
                   addTopicSucceded,
                   addTopicFailed);
                }
            }
        }

        function addTopicSucceded(response) {
            console.log(response);
            var standardId = $scope.newTopic.StandardId;
            var subjectId = $scope.newTopic.SubjectId;
            var mappingId = $scope.newTopic.MappingID;
            notificationService.displayInfo('Data uploaded successfully');
            $scope.newTopic.Code = "";
            $scope.newTopic.Name = "";
            $scope.newTopic.Objective = "";
        }

        function addTopicFailed(response) {
            console.log(response);
            if (response.status == '400')
                notificationService.displayError(response.data);
            else
                notificationService.displayError(response.statusText);
        }

        //******************ADD Topic End***************************************


        //***********Edit Topic*Starts*****************************************//

        function editTopic(topic) {
            $scope.IsEditMode = true;
            $scope.newTopic.ID = topic.ID;
            $scope.newTopic.Code = topic.Code;
            $scope.newTopic.Name = topic.Name;
            $scope.newTopic.Objective = topic.Objective;
            $scope.newTopic.StandardId = topic.StandardId;
            $scope.newTopic.MappingID = topic.MappingID;
            $scope.StandardChange();
            $scope.newTopic.SubjectId = topic.SubjectId;
        }
        //***********Edit Topic*End*****************************************//

        //Update Topic
        function updateTopic() {
            var newTopic = $scope.newTopic;
            apiService.post(baseUrl + '/api/topic/update', newTopic,
                    updateTopicSucceded,
                    updateTopicFailed);
        }


        function updateTopicSucceded(response) {
            console.log(response);
            $scope.ClearTopic();
            $scope.IsEditMode = false;
            $scope.search($scope.page);
            notificationService.displayInfo('Data Updated successfully');

        }

        function updateTopicFailed(response) {
            console.log(response);

            if (response.status == '400')
                notificationService.displayError(response.data);
            else
                notificationService.displayError(response.statusText);
        }



        //Functions to show Advance search
        function ShowAdvancedSearch() {
            $('.questionControl .panel-heading').toggleClass('showAdvance');
            $scope.search();
        }

        function ToggleAddSearch() {
            $('.searchBlock').toggleClass('active');
            $('.addBlock').toggleClass('active');

            /* @TODO: update the add/show toggle code here*/
            /*$('.questionControl .panel-heading').removeClass('showAdvance');*/
        }

        //Advance Search start ------------->

        function advancedSearch(page, searchItem) {
            var item = searchItem
            if (searchItem != null) {
                if (angular.isUndefined(item.Code)) {
                    item.Code = "";
                }
                if (angular.isUndefined(item.Name)) {
                    item.Name = "";
                }
                if (angular.isUndefined(item.StandardId)) {
                    item.StandardId = -1;
                }
                if (angular.isUndefined(item.SubjectId)) {
                    item.SubjectId = -1;
                }
                var config = {
                    params: {
                        page: page,
                        pageSize: 5,
                        code: item.Code,
                        name: item.Name,
                        subjectid: item.SubjectId,
                        standardid: item.StandardId

                    }
                };


                apiService.get(baseUrl + '/api/topic/advancedsearch/', config,
                   advancedSearchCompleted,
                   advancedSearchFailed);
            }
            else {

                notificationService.displayError("Please select Search item");
            }
        }

        function advancedSearchCompleted(result) {
            console.log(result.data);
            $scope.Topics = result.data.Items;
            $scope.page = result.data.Page;
            $scope.pagesCount = result.data.TotalPages;
            $scope.totalCount = result.data.TotalCount;
            $scope.loadingTests = false;
            if ($scope.filterTopic && $scope.filterTopic.length) {
                notificationService.displayInfo(result.data.Items.length + ' topics(s) found');
            }

        }

        function advancedSearchFailed(response) {
            notificationService.displayError(response.data);
        }

        function clearSearch() {
            $scope.SearchText = {};
            search();
        }

        function ClearTopic() {
            $scope.newTopic = {};
        }

        //End ------------->

        //Validation function
        function Register() {
            if (isNaN($scope.newTopic.StandardId)) {
                $scope.vStandardId = true;
            }
            if (isNaN($scope.newTopic.SubjectId)) {
                $scope.vSubjectId = true;
            }
            if (angular.isUndefined($scope.newTopic.Code)) {
                $scope.vCode = true;
            }
            if (angular.isUndefined($scope.newTopic.Name)) {
                $scope.vName = true;
            }
            if (angular.isUndefined($scope.newTopic.Objective)) {
                $scope.vObjective = true;
                return false;
            }
            else {
                return true;
            }
        }

    }

})(angular.module('app-administration'));