angular.module('app').controller('CUCtrl', function (AuthVerify, $scope, $http, $state, $stateParams, $location, $window) {
    $scope.StartTime = '';
    $scope.EndTime = '';
    $scope.Searchstr = '';
    $scope.pSize = 10;
    $scope.maxSize = 5;
    $scope.Lists = [];
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
    //获取 MailDB 列表
    $scope.GetUserCreateTime = function (currentPage) {
        $scope.currentPage = currentPage;
        $scope.Lists = [];
        $http({
            method: 'GET',
            url: storage.getItem("InterfaceUrl") + 'SystemReport.ashx?op=GetUserCreateTime&accessToken=' + storage.getItem("userAdminToken") + '&PageSize=' + $scope.pSize + '&CurPage=' + currentPage + '&Searchstr=' + encodeURI($scope.Searchstr) + '&StartTime=' + $scope.StartTime + '&EndTime=' + $scope.EndTime,
        }).then(function successCallback(response) {
            var data = response.data;
            if (data.result == "true") {
                var ListData = data.data.Lists;
                $scope.bigTotalItems = data.data.RecordCount;
                $scope.totalPage = data.data.PageCount;
                for (n = 0; n < ListData.length; n++) {
                    $scope.arr = {
                        "ID": ListData[n].ID,
                        "DisplayName": ListData[n].DisplayName,
                        "UserAccount": ListData[n].UserAccount,
                        "CreateTimeName": ListData[n].CreateTimeName,
                        "LastLoginTimeName": ListData[n].LastLoginTimeName,
                        "trueData": 1
                    };
                    $scope.Lists.push($scope.arr);
                }
                console.log($scope.Lists);
                if (ListData.length < $scope.pSize) {
                    for (i = 0; i < $scope.pSize - ListData.length; i++) {
                        var nulltr = {
                            "$$hashKey": i,
                            "ID": "",
                            "DisplayName": "",
                            "UserAccount": "",
                            "CreateTimeName": "",
                            "LastLoginTimeName": "",
                            "trueData": 0
                        }
                        $scope.Lists.push(nulltr);
                    }
                }
                if ($scope.pSize > 10) {
                    $(".content-left").css("height", $scope.pSize * 43 + 180);
                } else {
                    $(".content-left").css("height", $window.innerHeight - 84);
                }
            } else {
                ErrorHandling(data.result, data.errCode, data.errMsg);
            }
        }, function errorCallback(e) {

        });
    }
    $scope.pageChanged = function (currentPage) {
        $scope.GetUserCreateTime(currentPage);
    };
    $scope.clearSearch = function () {
        $scope.StartTime = '';
        $scope.EndTime = '';
        $scope.Searchstr = '';
        $scope.GetUserCreateTime(1);
    }

    $scope.changePageSize = function () {
        $scope.GetUserCreateTime(1);
    }

    $scope.ExportUserCreateTimeToExcel = function () {
        $('#Export').attr('href', storage.getItem("InterfaceUrl") + 'SystemReport.ashx?op=ExportUserCreateTimeToExcel&accessToken=' + storage.getItem("userAdminToken") + '&PageSize=10000&CurPage=1&Searchstr=' + $scope.Searchstr + '&StartTime=' + $scope.StartTime + '&EndTime=' + $scope.EndTime);
    }
});