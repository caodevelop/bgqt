angular.module('app').controller('PDFWMCtrl', function (AuthVerify, $scope, $http, $state, $stateParams, $location, $window, $timeout) {
    $scope.currentPage = 1;
    $scope.totalPage = 0;
    $scope.pSize = 10;
    $scope.maxSize = 5;
    $scope.searchtext = '';
    $scope.Lists = [];
    $scope.SearchPhone = '';
    $scope.Role = {};
    $scope.Role.RoleName = '';
    $scope.Role.ControlLimit = 1;
    $scope.Role.RoleList = [];
    $scope.Searchstr = '';
    $scope.op = '';
    $scope.seachNode = '';
    $scope.ModifyPDFWaterMakingObjectID = '';
    $scope.ObjectID = '';
    $scope.ObjectLimits = [];
    $scope.ViewOrAdd = true;
    $scope.PageSizes = [{ "text": "10条/页", "value": "10" }, { "text": "20条/页", "value": "20" }, { "text": "50条/页", "value": "50" }, { "text": "100条/页", "value": "100" }];
    $scope.pSize = $scope.PageSizes[0].value;

    if ($window.innerWidth >= 1024) {
        $(".content-left").css("height", $window.innerHeight - 84);
    } else {
        $(".content-left").css("height", $window.innerHeight - 4);
    }
    window.onresize = function () {
        $scope.$apply(function () {
            if ($window.innerWidth >= 1024) {
                $(".content-left").css("height", $window.innerHeight - 84);
            } else {
                $(".content-left").css("height", $window.innerHeight - 4);
            }
        })
    };

    $scope.GetPDFWaterMakingList = function (currentPage) {
        editFlag = true;
        $scope.currentPage = currentPage;
        $scope.ViewOrAdd = true;
        $scope.Lists = [];
        $scope.tabNum = 'tab1';
        //angular.element(".panel-heading").css("border-bottom", "none");
        $http({
            method: 'GET',
            url: storage.getItem("InterfaceUrl") + 'WaterMarking.ashx?op=GetPDFWaterMakingList&accessToken=' + storage.getItem("userAdminToken") + '&PageSize=' + $scope.pSize + '&CurPage=' + $scope.currentPage + '&Searchstr=' + encodeURI($scope.Searchstr),
        }).then(function successCallback(response) {
            var data = response.data;
            if (data.result == "true") {
                var ListData = data.data.Lists;
                $scope.bigTotalItems = data.data.RecordCount;
                $scope.totalPage = data.data.PageCount;
                for (n = 0; n < ListData.length; n++) {
                    $scope.arr = {
                        "ID": ListData[n].ID,
                        "Name": ListData[n].Name,
                        "PDFCondition": ListData[n].PDFCondition.PDFCondition,
                        "ObjectType": ListData[n].ObjectType,
                        "ObjectNames": ListData[n].ObjectNames,
                        "Keywords": ListData[n].Keywords,
                        "CreateTimeName": ListData[n].CreateTimeName.substr(0, 16),
                        "StartTimeName": ListData[n].StartTimeName.substr(0, 16),
                        "EndTimeName": ListData[n].EndTimeName.substr(0, 16),
                        "ExecuteTimeName": ListData[n].ExecuteTimeName.substr(0, 16),
                        "ExecuteResult": ListData[n].ExecuteResult,
                        "trueData": 1
                    };
                    $scope.Lists.push($scope.arr);
                }
                if (ListData.length < $scope.pSize) {
                    for (i = 0; i < $scope.pSize - ListData.length; i++) {
                        var nulltr = {
                            "$$hashKey": i,
                            "ID": "",
                            "Name": "",
                            "ObjectID": "",
                            "ObjectType": "",
                            "ObjectNames": "",
                            "Keywords": "",
                            "CreateTimeName": "",
                            "EndTimeName": "",
                            "ExecuteTimeName": "",
                            "ExecuteResult": "",
                            "StatusName": "",
                            "TimePeriod": "",
                            "trueData": 0
                        }
                        $scope.Lists.push(nulltr);
                    }
                }
            } else {
                ErrorHandling(data.result, data.errCode, data.errMsg);
            }
        }, function errorCallback(e) {
        });
    }

    $scope.pageChanged = function (currentPage) {
        $scope.GetPDFWaterMakingList(currentPage);
    };


});