(function (app) {
    'use strict';

    app.controller('loginCtrl', loginCtrl);

    loginCtrl.$inject = ['$scope', 'membershipService', 'notificationService', '$rootScope', '$location', 'permissionService']; //, '$modalInstance'

    function loginCtrl($scope, membershipService, notificationService, $rootScope, $location, permissionService) { //, $modalInstance
        $scope.pageClass = 'page-login';
        $scope.login = login;
        $scope.user = {};

        function login() {
            membershipService.login($scope.user, loginCompleted)
        }

        function loginCompleted(result) {
            if (result.data.success) {
                $('#curtain').removeClass('active');
                $scope.user.userid = result.data.userid;
                membershipService.saveCredentials($scope.user);
                notificationService.displaySuccess('Hello ' + $scope.user.username);
               
                permissionService.removePermissions();
            //    permissionService.set();

                //$scope.userData.displayUserInfo();
                ////if ($scope.userData == null)
                ////    $scope.$$prevSibling.userData.displayUserInfo();
                ////else
                ////    $scope.userData.displayUserInfo();


                ////$modalInstance.dismiss();

                if ($rootScope.previousState)
                    $location.path($rootScope.previousState);
                else{
                        //$location.path('/');
                    //self.location.replace('http://localhost:56387/account/html/login1.html');
                    //self.location.replace('http://localhost:56387/mainDashboard/MainDashboard.html');
                    //self.location.reload();
                    location.href = 'http://localhost:56387/mainDashboard/MainDashboard.html';
                    }
                }
            else {
                notificationService.displayError('Login failed. Try again.');
            }
        }
    }

})(angular.module('common.core'));