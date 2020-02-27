angular.module('app').controller('PVPCtrl', function (AuthVerify, $scope, $http, $state, $stateParams, $location, $window) {
    $scope.allType = [
        { text: "永不过期", value: "1" },
        { text: "已过期", value: "3" }
    ];
    $scope.PwdType = $scope.allType[0].value;
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
    $scope.GetPasswordStateUsers = function (currentPage) {
        $scope.currentPage = currentPage;
        $scope.Lists = [];
        $http({
            method: 'GET',
            url: storage.getItem("InterfaceUrl") + 'SystemReport.ashx?op=GetPasswordStateUsers&accessToken=' + storage.getItem("userAdminToken") + '&PageSize=' + $scope.pSize + '&CurPage=' + currentPage + '&Searchstr=' + encodeURI($scope.Searchstr) + '&Type=' + $scope.PwdType,
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
                        "Type": ListData[n].Type,
                        "PasswordExpireTimeName": ListData[n].Type == 1 ? "-" : ListData[n].PasswordExpireTimeName,
                        "PasswordTypeName": ListData[n].PasswordTypeName,
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
                            "Type": "",
                            "PasswordExpireTimeName": "",
                            "PasswordTypeName":"",
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
        $scope.GetPasswordStateUsers(currentPage);
    };
    $scope.clearSearch = function () {
        $scope.Searchstr = '';
        $scope.PwdType = $scope.allType[0].value;
        $scope.GetPasswordStateUsers(1);
    }

    $scope.changePageSize = function () {
        $scope.GetPasswordStateUsers(1);
    }

    $scope.ExportPasswordStateUsersToExcel = function () {
        $('#Export').attr('href', storage.getItem("InterfaceUrl") + 'SystemReport.ashx?op=ExportPasswordStateUsersToExcel&accessToken=' + storage.getItem("userAdminToken") + '&PageSize=10000&CurPage=1&Searchstr=' + $scope.Searchstr + '&Type=' + $scope.PwdType);
    }
});