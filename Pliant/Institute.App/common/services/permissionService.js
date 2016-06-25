(function (app) {
    'use strict';

    app.factory('permissionService', permissionService);

    permissionService.$inject = ['$http', '$rootScope', '$cookieStore'];

    function permissionService($http, $rootScope, $cookieStore) {

        var service = {
            get: get,
            set: set,
            removePermissions: removePermissions
        };

        function removePermissions() {
            $rootScope.permission = {};
            $cookieStore.remove('permission');
        };

        function get(accessname) {
            $rootScope.temp = $cookieStore.get('permission') || {};
            if ($rootScope.temp.hasOwnProperty('userAccess')) {
                if ($rootScope.temp.userAccess.hasOwnProperty(accessname)) {
                    return $rootScope.temp.userAccess[accessname];
                }
                else {
                    return 0;
                }
            }
            else {
                return 0;
            }
        }

        function set() {
            
            $rootScope.permission = {
                userAccess: {
                    STANDARD: 1,
                    SUBJECT: 1,
                    QUESTION: 1,
                    TEST: 1,
                    STDSUBMAP: 1,

                    TOPIC: 1,
                    ADDTOPIC: 1,
                    EDITTOPIC: 1,
                    DELETETOPIC: 1,

                    STD: 1,
                    ADDSTD: 0,
                    EDITSTD: 0,
                    DELETESTD: 0
                }
            };

            $cookieStore.put('permission', $rootScope.permission);
        }

        return service;
    }
})(angular.module('common.core'));