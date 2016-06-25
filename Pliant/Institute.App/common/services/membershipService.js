(function (app) {
    'use strict';

    app.factory('membershipService', membershipService);

    membershipService.$inject = ['apiService', 'webApiLocationService', 'notificationService', '$http', '$base64', '$cookieStore', '$rootScope', '$location'];

    function membershipService(apiService, webApiLocationService, notificationService, $http, $base64, $cookieStore, $rootScope, $location) {

        var baseUrl = webApiLocationService.get('webapi');
        

        var service = {
            login: login,
            register: register,
            saveCredentials: saveCredentials,
            removeCredentials: removeCredentials,
            isUserLoggedIn: isUserLoggedIn,
            redirectIfNotLoggedIn :redirectIfNotLoggedIn
           
        }

        function login(user, completed) {
            apiService.post(baseUrl + '/api/account/authenticate', user,
            completed,
            loginFailed);
        }
        //function loginCompleted(response) {
        //    console.log(response);
        //    $cookieStore.put('userId', response.data.Items);
        //}


        function register(user, completed) {
            apiService.post(baseUrl + '/api/account/register', user,
            completed,
            registrationFailed);
        }

        function saveCredentials(user) {
            var membershipData = $base64.encode(user.username + ':' + user.password);

            $rootScope.repository = {
                loggedUser: {
                    username: user.username,
                    userid: user.userid,
                    authdata: membershipData
                }
            };

            $http.defaults.headers.common['Authorization'] = 'Basic ' + membershipData;
            $cookieStore.put('repository', $rootScope.repository);
        }

        function removeCredentials() {
            $rootScope.repository = {};
            $cookieStore.remove('repository');
            $http.defaults.headers.common.Authorization = '';
        };

        function loginFailed(response) {
            notificationService.displayError(response.data);
        }

        function registrationFailed(response) {

            notificationService.displayError('Registration failed. Try again.');
        }

        function isUserLoggedIn() {
            $rootScope.temp = $cookieStore.get('repository') || {};
            if ($rootScope.temp.hasOwnProperty('loggedUser')) {
                //$rootScope.temp = {};
                if ($rootScope.temp.loggedUser.hasOwnProperty('username'))
                    return $rootScope.repository.loggedUser != null;
                else {
                    $rootScope.repository.loggedUser = null;
                    return $rootScope.repository.loggedUser;
                }
            }
            else {
                //$rootScope.temp = {};
                $rootScope.repository.loggedUser = null;
                return $rootScope.repository.loggedUser;
            }
        }

     

      
        function redirectIfNotLoggedIn() {
            if (!isUserLoggedIn()) {
                $rootScope.previousState = $location.path();
                location.href = '../account/html/login.html';
            }
        }

        return service;
    }



})(angular.module('common.core'));