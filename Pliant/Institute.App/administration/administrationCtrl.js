(function (app) {
    'use strict';

    app.controller('administrationCtrl', administrationCtrl);

    administrationCtrl.$inject = ['$scope', '$modal', 'apiService', 'membershipService', 'webApiLocationService', 'notificationService', 'permissionService', 'constantStrService'];

    function administrationCtrl($scope, $modal, apiService, membershipService, webApiLocationService, notificationService, permissionService, constantStrService) {

        membershipService.redirectIfNotLoggedIn();

        $scope.permissionSTD = permissionService.get(constantStrService.STANDARD());
        $scope.permissionSUBJECT = permissionService.get(constantStrService.SUBJECT());
        $scope.permissionSTDSUBMAP = permissionService.get(constantStrService.STD_SUB_MAP());
        $scope.permissionTOPIC = permissionService.get(constantStrService.TOPIC());
        $scope.permissionQUESTION = permissionService.get(constantStrService.QUESTION());

        $scope.permissionTEST = permissionService.get(constantStrService.TEST());
        $scope.permissionPOOL = permissionService.get(constantStrService.POOL());
        $scope.permissionPOOLQUEMAP = permissionService.get(constantStrService.POOL_QUESTION_MAP());
    }

})(angular.module('app-administration'));