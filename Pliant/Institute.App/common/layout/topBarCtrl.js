(function (app) {
    'use strict';

    app.controller('topBarCtrl', topBarCtrl);

    topBarCtrl.$inject = ['$scope', 'membershipService', 'notificationService', '$rootScope', '$location', '$cookieStore'];

    function topBarCtrl($scope, membershipService, notificationService, $rootScope, $location, $cookieStore ) {
        $scope.pageClass = 'page-login';
        $scope.setUser = setUser;
        $scope.logout = logout;
        $scope.menuPersistState = menuPersistState;
        $scope.loggeduser = {};
        $scope.tempuser = {};
        
        var repository;

        function logout() {
                membershipService.removeCredentials();
                window.location.reload(true);
            //  location.href = "http://localhost:56387/account/html/login.html";

        }


        function setUser() {
            repository = $cookieStore.get('repository') || {};
            $scope.tempuser = repository;
            if ($scope.tempuser.hasOwnProperty('loggedUser')) {
                if ($scope.tempuser.loggedUser.hasOwnProperty('username')) {
                    $scope.loggeduser.username = repository.loggedUser.username;
                    $scope.loggeduser.userid = repository.loggedUser.userid;
                    $scope.loggeduser.authdata = repository.loggedUser.authdata;
                }
            }
        }
        function menuPersistState() {
            /* removed angular $cookieStore as it was not persisting in different page*/
            setTimeout(function(){
                if( $('#sidebar').is('.in') ){
                    document.cookie = "menu=open; path=/";
                    $("#page-wrapper").addClass('active');
                }else {
                    document.cookie = "menu=; path=/";
                    $("#page-wrapper").removeClass('active');
                }
            }, 500);
        }
        angular.element(document).ready(function () {
            var menuState = document.cookie.match(/menu=open/);
            if (menuState) {
                $('#sidebar').addClass('in');
                $("#page-wrapper").addClass('active');
            }
        });

        $scope.setUser();
    }

})(angular.module('common.core'));