(function (app) {
    'use strict';

    app.controller('mainCtrl', rootCtrl);

    rootCtrl.$inject = ['$scope', '$location', 'membershipService', '$rootScope', 'permissionService', '$cookieStore'];
    function rootCtrl($scope, $location, membershipService, $rootScope, permissionService, $cookieStore) {

        $scope.userData = {};
        
        $scope.userData.displayUserInfo = displayUserInfo;
        $scope.logout = logout;
        membershipService.redirectIfNotLoggedIn();

        $scope.isUserLoggedIn = membershipService.isUserLoggedIn();

        function displayUserInfo() {
            $scope.userData.isUserLoggedIn = membershipService.isUserLoggedIn();

            if($scope.userData.isUserLoggedIn)
            {
                $scope.username = $rootScope.repository.loggedUser.username;
            }
        }

        function logout() {
            membershipService.removeCredentials();
            
        }
   }

})(angular.module('app-main'));