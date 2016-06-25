(function (app) {
    'use strict';

    app.factory('webApiLocationService', webApiLocationService);

    webApiLocationService.$inject = ['$http','$rootScope', '$cookieStore'];

    function webApiLocationService($http, $rootScope, $cookieStore) {

        var webapipath = 'http://localhost:51642';

        var service = {
            get: get,
            set: set
        };

        function get(servicename) {
            return webapipath;
        }

        function set(servicename) {

        }

        return service;
    }
})(angular.module('common.core'));