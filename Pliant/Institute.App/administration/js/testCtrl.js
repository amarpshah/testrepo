(function (app) {
    'use strict';

    app.controller('testCtrl', testCtrl);

    testCtrl.$inject = ['$scope', '$modal', 'apiService', 'webApiLocationService', 'notificationService', 'membershipService', '$cookieStore', 'dataProviderService'];

    function testCtrl($scope, $modal, apiService, webApiLocationService, notificationService, membershipService, $cookieStore, dataProviderService) {

        $scope.pageClass = 'page-test';
        $scope.filterTests = '';
        $scope.Tests = [];
        $scope.loadingTests = true;
        $scope.page = 0;
        $scope.pagesCount = 0;
        $scope.search = search;
    //    $scope.SearchText = {};
        $scope.clearSearch = clearSearch;
        $scope.deleteTest = deleteTest;
        $scope.testLock = testLock;
        $scope.lock = lock;
        $scope.unLock = unLock;
       // $scope.flag = 1;
        $scope.disableLock = false;
        $scope.disableDelete = false;
        $scope.checkBoxId = {};
        $scope.showSelected = showSelected;
        $scope.ShowAdvancedSearch = ShowAdvancedSearch;
        
        $scope.advancedSearch = advancedSearch;
        $scope.loadvalues = loadvalues;
        $scope.test = {};

        membershipService.redirectIfNotLoggedIn();
        var userInfo = $cookieStore.get('repository');
        $scope.userName = userInfo.loggedUser.username;
        var userId = userInfo.loggedUser.userid;
        $scope.checkId = userId;

        var baseUrl = webApiLocationService.get('webapi');

              

        function loadvalues() {


            $scope.test.status = [{ value: 1, Text: "Draft" },
                                  { value: 2, Text: "Ready" },
                                  { value: 3, Text: "Locked" }
            ];
            $scope.test.difficulty = [{ value: 1, Text: "Easy" },
                                                  { value: 2, Text: "Medium" },
                                                  { value: 3, Text: "Difficult" }
            ];


        }
        $scope.loadvalues();
     
        function showSelected(button) {
            $scope.button = button;
            $scope.newArray = [];
            angular.forEach($scope.Tests, function (test) {

                if(!!test.selected)$scope.newArray.push(test.ID)  //Get all  testID in newArray
            })

            angular.forEach($scope.newArray, function (testId) {  //Find object of each testID
                                                                   //and Call web api 
                var config = {
                    params: {
                        id: testId
                    }
                };

                apiService.get(baseUrl + '/api/tests/test/' + testId, null,
                  testCheckCompleted,
                  testCheckFailed);

            })
            

        }

        function testCheckCompleted(result) {
            var data = result.data;
            if ($scope.button == 'lock' && data[0].Lock == 0)  //if Multi Lock button clicked
            {                                                    // then check if unlock             

                $scope.testLock(data[0]);
              
            }
            if ($scope.button == 'unLock' && data[0].Lock == 1) //if Multi Unlock button clicked
            {                                                     //then check if locked               
                $scope.testLock(data[0]);
              
            }
            $scope.search();
        }

        function testCheckFailed(response) {
            notificationService.displayError(response.data);
        }

              

        function search(page, searchItem) {

            if (!searchItem) {
                page = page || 0;

                $scope.loadingTests = true;

                var config = {
                    params: {
                        page: page,
                        pageSize: 4,
                        filter: $scope.filterTests
                    }
                };


                apiService.get(baseUrl + '/api/tests/search/', config,
                    testLoadCompleted,
                    testLoadFailed);
            }
            else {

                $scope.advancedSearch(page, searchItem);
            }
        }

        function testLoadCompleted(result) {
            $scope.Tests = result.data.Items;
            $scope.page = result.data.Page;
            $scope.pagesCount = result.data.TotalPages;
            $scope.totalCount = result.data.TotalCount;
            $scope.loadingTests = false;

           
            

            if ($scope.filterTests && $scope.filterTests.length) {
                notificationService.displayInfo(result.data.Items.length + ' test(s) found');
            }

        }

        function testLoadFailed(response) {
            notificationService.displayError(response.data);
        }

        $scope.search();


        function deleteTest(testid)
        {
          
            
            if ( testid != null) {
                
                params: {
                    id: testid
                }
                apiService.post(baseUrl + '/api/tests/delete/' + testid, null,
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


        

        function testLock(value) {
           
            if (value.Lock==1 && value.LockedBy==userId ) {
                                                      //Unlock Test if valid user
                //value.Lock = 0;

                value.LockedBy = 0;
                           
                unLock(value);
               
               
            }
            else if (value.Lock == 0) {
                                                       //Lock Test if unlock
                //value.Lock = 1;
                
                value.LockedBy = userId;
               
               lock(value);
               
            }
            else {
                
               // alert("Cant Unlock");
            }

           
           
        }
      

         function lock(value) {
             
            apiService.post(baseUrl + '/api/tests/lockTest/', value,
              lockSucceded,
              lockFailed);


        }

        function lockSucceded(response) {
            console.log(response);
            $scope.search($scope.page);
                 
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

            apiService.post(baseUrl + '/api/tests/unLockTest/', value,
              unLockSucceded,
              unLockFailed);

        }

        function unLockSucceded(response) {
            console.log(response);
           
            $scope.search($scope.page);
        
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
                if (angular.isUndefined(item.Text)) {
                    item.Text = "";
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
                        code: item.Code,
                        text: item.Text,
                        status: item.Status,
                        difficultyLevel: item.DifficultyLevel
                    }
                };


                apiService.get(baseUrl + '/api/tests/advancedsearch/', config,
                   advancedSearchCompleted,
                   advancedSearchFailed);
            }
            else {

                notificationService.displayError("Please select Search item");
            }
        }

        function advancedSearchCompleted(result) {
            console.log(result.data);
            $scope.Tests = result.data.Items;
            $scope.page = result.data.Page;
            $scope.pagesCount = result.data.TotalPages;
            $scope.totalCount = result.data.TotalCount;
            $scope.loadingTests = false;

            if ($scope.filterTests && $scope.filterTests.length) {
                notificationService.displayInfo(result.data.Items.length + ' test(s) found');
            }

        }

        function advancedSearchFailed(response) {
            notificationService.displayError(response.data);
        }

        function clearSearch() {
            $scope.SearchText = {};
            search();
        }
        //End ------------->

       
    }

})(angular.module('app-administration'));