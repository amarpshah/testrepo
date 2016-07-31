(function (app) {
    'use strict';

    app.controller('poolCtrl', poolCtrl);

    poolCtrl.$inject = ['$scope', '$modal', 'apiService', 'membershipService', 'webApiLocationService', 'notificationService', '$stateParams', '$cookieStore', 'permissionService', 'constantStrService'];

    function poolCtrl($scope, $modal, apiService, membershipService, webApiLocationService, notificationService, $stateParams, $cookieStore, permissionService, constantStrService) {

        $scope.pageClass = 'page-test';
        $scope.filterPools = '';
        $scope.Pools = [];
        $scope.loadingPools = true;
        $scope.page = 0;
        $scope.pagesCount = 0;
        $scope.search = search;
        $scope.deletePool = deletePool;
        $scope.clearSearch = clearSearch;
        $scope.appliedClass = appliedClass;
        $scope.poolLock = poolLock;
        $scope.lock = lock;
        $scope.unLock = unLock;
        $scope.disableLock = false;
        $scope.disableDelete = false;
        $scope.advancedSearch = advancedSearch;
        $scope.showSelected = showSelected;
        $scope.loadValues = loadValues;
        $scope.pool = {};

        membershipService.redirectIfNotLoggedIn();
        $scope.TestId = parseInt($stateParams.testid);
        $scope.TestName = ($stateParams.testname);
        var userInfo = $cookieStore.get('repository');
        $scope.userName = userInfo.loggedUser.username;

        var userId = userInfo.loggedUser.userid;
        $scope.checkId = userId;
        var baseUrl = webApiLocationService.get('webapi');

        $scope.permissionADDPOOL = permissionService.get(constantStrService.ADD_POOL());
        $scope.permissionUPDATEPOOL = permissionService.get(constantStrService.UPDATE_POOL());
        $scope.permissionDELETEPOOL = permissionService.get(constantStrService.DELETE_POOL());
        $scope.permissionLOCKPOOL = permissionService.get(constantStrService.LOCK_POOL());
        $scope.permissionADDQUESTIONPOOL = permissionService.get(constantStrService.ADD_QUESTION_POOL());
        $scope.permissionVIEWQUESTIONPOOL = permissionService.get(constantStrService.VIEW_QUESTION_POOL());


        function loadValues() {
            $scope.pool.status = [{ value: 1, Text: "Draft" },
                                  { value: 2, Text: "Ready" },
                                  { value: 3, Text: "Locked" }
            ];

            $scope.pool.difficulty = [{ value: 1, Text: "Easy" },
                                                  { value: 2, Text: "Medium" },
                                                  { value: 3, Text: "Difficult" }
            ];
        }

        $scope.loadValues();

        function clearSearch() {
            $scope.filterPools = '';
            search();
        }

        function search(page, searchItem) {
            if (!searchItem) {
                page = page || 0;
                $scope.loadingPools = true;

                if (isNaN($scope.TestId)) {
                    $scope.TestId = -1;
                }
                var config = {
                    params: {
                        testid: $scope.TestId
                    }
                };

                apiService.get(baseUrl + '/api/pools/filtertests/', config,
                    poolLoadCompleted,
                    poolLoadFailed);
            }
            else {
                $scope.advancedSearch(page, searchItem);
            }
        }

        function poolLoadCompleted(result) {
            $scope.Pools = result.data.Items;
            $scope.page = result.data.Page;
            $scope.pagesCount = result.data.TotalPages;
            $scope.totalCount = result.data.TotalCount;
            $scope.loadingPools = false;

            if ($scope.filterPools && $scope.filterPools.length) {
                notificationService.displayInfo(result.data.Items.length + ' pool(s) found');
            }
        }

        function poolLoadFailed(response) {
            notificationService.displayError(response.data);
        }

        $scope.search();


        //Delete Subject
        function deletePool(poolid) {

            if (poolid != null) {
                var config = {
                    params: {
                        id: poolid
                    }
                };
                apiService.post(baseUrl + '/api/pools/delete/' + poolid, null,
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


        //Show lock , unlock and key icon
        function appliedClass(value) {
            if (value.IsLock == 1 && value.LockedBy != userId) {
                $scope.disableDelete = true;
                $scope.disableLock = true;//disable lock if  locked by other user
                return "fa fa-lock";
            }
            else if (value.IsLock == 1 && value.LockedBy == userId) {
                $scope.disableDelete = false;
                $scope.disableLock = false;
                return "fa fa-key";
            }
            else {
                $scope.disableDelete = false;
                $scope.disableLock = false; //enable lock if unlock
                return "fa fa-unlock";
            }
        }

        function poolLock(value) {

            if (value.IsLock == 1 && value.LockedBy == userId) {  //Unlock Test if valid user
                value.LockedBy = 0;
                unLock(value);
            }
            else if (value.IsLock == 0) {            //Lock Test if unlock
                value.LockedBy = userId;
                lock(value);
            }
        }


        function lock(value) {
            apiService.post(baseUrl + '/api/pools/lockTest/', value,
              lockSucceded,
              lockFailed);
        }

        function lockSucceded(response) {
            console.log(response);
            $scope.search($scope.page);
            $scope.appliedClass(response.data);
            notificationService.displayInfo('Locked successfully');
        }

        function lockFailed(response) {
            console.log(response);
            if (response.status == '400')
                notificationService.displayError(response.data);
            else
                notificationService.displayError(response.statusText);
        }

        function unLock(value) {

            apiService.post(baseUrl + '/api/pools/unLockTest/', value,
              unLockSucceded,
              unLockFailed);

        }

        function unLockSucceded(response) {
            console.log(response);
            $scope.search($scope.page);
            $scope.appliedClass(response.data);
            notificationService.displayInfo('Unlocked successfully');
        }

        function unLockFailed(response) {
            console.log(response);
            if (response.status == '400')
                notificationService.displayError(response.data);
            else
                notificationService.displayError(response.statusText);
        }

        //Functions to show Advance search

        function advancedSearch(page, searchItem) {
            var item = searchItem
            if (searchItem != null) {
                if (angular.isUndefined(item.Name)) {
                    item.Name = "";
                }
                if (angular.isUndefined(item.Status)) {
                    item.Status = -1;
                }
                if (angular.isUndefined(item.DifficultyLevel)) {
                    item.DifficultyLevel = -1;
                }

                var config = {
                    params: {
                        page: page,
                        pageSize: 4,
                        testid: $scope.TestId,
                        name: item.Name,
                        status: item.Status,
                        difficultyLevel: item.DifficultyLevel
                    }
                };
                apiService.get(baseUrl + '/api/pools/advancedsearch/', config,
                   advancedSearchCompleted,
                   advancedSearchFailed);
            }
            else {

                notificationService.displayError("Please select Search item");
            }
        }

        function advancedSearchCompleted(result) {
            console.log(result.data);
            $scope.Pools = result.data.Items;
            $scope.page = result.data.Page;
            $scope.pagesCount = result.data.TotalPages;
            $scope.totalCount = result.data.TotalCount;
            $scope.loadingPools = false;

            if ($scope.filterPools && $scope.filterPools.length) {
                notificationService.displayInfo(result.data.Items.length + ' Pools(s) found');
            }
        }

        function advancedSearchFailed(response) {
            notificationService.displayError(response.data);
        }

        function clearSearch() {
            $scope.SearchText = {};
            search();
        }


        //Multi Lock  and Unlock

        function showSelected(button) {
            $scope.button = button;
            $scope.newArray = [];
            angular.forEach($scope.Pools, function (pool) {
                if (!!pool.selected) $scope.newArray.push(pool.ID)  //Get all  testID in newArray
            })

            angular.forEach($scope.newArray, function (poolId) {  //Find object of each testID
                //and Call web api 
                var config = {
                    params: {
                        id: poolId
                    }
                };

                apiService.get(baseUrl + '/api/pools/pool/' + poolId, null,
                  poolCheckCompleted,
                  poolCheckFailed);

            })
        }

        function poolCheckCompleted(result) {
            var data = result.data;
            if ($scope.button == 'lock' && data[0].IsLock == 0)  //if Multi Lock button clicked
            {                                                    // then check if unlock             
                $scope.poolLock(data[0]);
            }
            if ($scope.button == 'unLock' && data[0].IsLock == 1) //if Multi Unlock button clicked
            {                                                     //then check if locked               
                $scope.poolLock(data[0]);
            }
            $scope.search();
        }

        function poolCheckFailed(response) {
            notificationService.displayError(response.data);
        }
    }

})(angular.module('app-administration'));