(function (app) {
    'use strict';

    app.factory('dataProviderService', dataProviderService);

    function dataProviderService() {
        var dataObjList = [];

        var addDataObj = function (list) {
            dataObjList.push(list);
        };

        var getDataObj = function () {
            return dataObjList;
        };

        return {
            addDataObj: addDataObj,
            getDataObj: getDataObj
        };



    }

})(angular.module('common.core'));




//app.service('dataProviderService', function () {
//    var dataObj = [];

//    var addDataObj = function (newObj) {
//        dataObj.push(newObj);
//    };

//    var getDataObj = function () {
//        return dataObj;
//    };

//    return {
//        addDataObj: addDataObj,
//        getDataObj: getDataObj
//    };

//});
