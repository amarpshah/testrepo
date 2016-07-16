(function (app) {
    'use strict';

    app.factory('permissionService', permissionService);

    permissionService.$inject = ['$http', '$rootScope', '$cookieStore', 'webApiLocationService', 'apiService', 'notificationService'];

    function permissionService($http, $rootScope, $cookieStore, webApiLocationService, apiService, notificationService) {
     
        var baseUrl = webApiLocationService.get('webapi');
   
        var service = {
            loadPermission: loadPermission,
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
            for (var i = 0; i < $rootScope.temp.length; i++)
            {
                if ($rootScope.temp[i].Action + $rootScope.temp[i].FormName == accessname) {
                    return $rootScope.temp[i].IsPermission;
                }
                
            }
             
           
        }


        function set() {
                     
            //   $rootScope.permission = {
            //    userAccess: {
            //        STANDARD: 1,
            //        SUBJECT: 1,
            //        QUESTION: 1,
            //        TEST: 1,
            //        STDSUBMAP: 1,

            //        TOPIC: 1,
            //        ADDTOPIC: 1,
            //        EDITTOPIC: 1,
            //        DELETETOPIC: 1,

            //        STD: 1,
            //        ADDSTD: 0,
            //        EDITSTD: 0,
            //        DELETESTD: 0
            //    }
            //};

            //$cookieStore.put('permission', $rootScope.permission);
        }
         
        loadPermission();
        function loadPermission()
        {
            if (!angular.isUndefined($rootScope.repository.loggedUser)) {
            var userid = $rootScope.repository.loggedUser.userid;
            
            var config = {
                params: {
                userid: userid
            }
            };

            apiService.get(baseUrl + '/api/permissions/getpermissions/'+ userid, null,
               loadPermissionCompleted,
                loadPermissionFailed);
        }
        }
      
  
        function loadPermissionCompleted(result)
        {
            $rootScope.permission = result.data;
            $cookieStore.put('permission', $rootScope.permission);
        }
        function loadPermissionFailed(response)
        {

            notificationService.displayError(response.data);
        }

        return service;

    }
})(angular.module('common.core'));