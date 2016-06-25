(function (app) {
    'use strict';

    app.controller('loginCtrl', loginCtrl);

    loginCtrl.$inject = ['$scope', 'membershipService', 'notificationService','$rootScope', '$location', '$modalInstance'];

    function loginCtrl($scope, membershipService, notificationService, $rootScope, $location, $modalInstance) {
        $scope.pageClass = 'page-login';
        $scope.login = login;
        $scope.user = {};

        function login() {
            membershipService.login($scope.user, loginCompleted)
        }

        function loginCompleted(result) {
            if (result.data.success) {
                membershipService.saveCredentials($scope.user);
                notificationService.displaySuccess('Hello ' + $scope.user.username);

                //$scope.userData.displayUserInfo();
                if ($scope.userData == null)
                    $scope.$$prevSibling.userData.displayUserInfo();
                else
                    $scope.userData.displayUserInfo();

                $modalInstance.dismiss();

                if ($rootScope.previousState)
                    $location.path($rootScope.previousState);
                else
                    $location.path('/');
            }
            else {
                notificationService.displayError('Login failed. Try again.');
            }
        }
    }

})(angular.module('common.core'));