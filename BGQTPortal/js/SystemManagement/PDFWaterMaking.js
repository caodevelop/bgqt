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
                        "WaterMakingContent": ListData[n].WaterMakingContent.WaterMakingContent,
                        "CreateTimeName": ListData[n].CreateTimeName.substr(0, 16),
                        "StatusName": ListData[n].StatusName,
                        "Status": ListData[n].Status,
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
                            "PDFCondition": "",
                            "WaterMakingContent": "",
                            "CreateTimeName": "",
                            "StatusName": "",
                            "Status": 1,
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

    $scope.clearSearch = function () {
        $scope.Searchstr = '';
        $scope.GetPDFWaterMakingList(1);
    };

    $scope.changePageSize = function () {
        $scope.GetPDFWaterMakingList(1);
    };

    //添加 or 编辑
    $scope.gotoPDFWMModal = function (op, id) {
        $scope.ViewOrAdd = false;
        angular.element(".panel-heading").css("border-bottom", "1px solid #d8d8d8");
        $scope.seachNode = '';
        $scope.op = op;
        if (op == 'edit') {
            $scope.OpTitle = "编辑规则";
            $scope.GetPDFWaterMakingInfo(id);
        } else {
            $scope.OpTitle = "添加规则";
            $scope.WaterMakingName = '';
            $scope.From = '';
            $scope.IsAllFrom = false;
            $scope.ExcludeFroms = '';
            $scope.Recipients = '';
            $scope.Subject = '';
            $scope.PDFName = '';
            $scope.Content = '';
            $scope.IsAllRecipients = 1;
            $scope.AddDateTime = false;
        }
    };
    
    //添加
    $scope.AddPDFWaterMaking = function () {
        if ($scope.WaterMakingName == "") {
            ErrorHandling('false', 0, '请输入添加PDF水印规则名称。');
            return;
        }
        if ($scope.From == "" && $scope.IsAllFrom == false && $scope.Recipients == ""
            && $scope.Subject == "" && $scope.PDFName == "") {
            ErrorHandling('false', 0, '请输入添加PDF水印规则条件。');
            return;
        }
       
        if ($scope.IsAllRecipients == "0" && $scope.Content =="") {
            ErrorHandling('false', 0, '请输入添加PDF水印规则内容。');
            return;
        }

        var PDFConditionObj = {
            "From": $scope.From,
            "IsAllFrom": $scope.IsAllFrom,
            "ExcludeFroms": $scope.ExcludeFroms,
            "Recipients": $scope.Recipients,
            "Subject": $scope.Subject,
            "PDFName": $scope.PDFName
        };

        var WaterMakingContent = {
            "IsAllRecipients": $scope.IsAllRecipients == "1" ? "true" : "false",
            "Content": $scope.Content,
            "IsAddDate": $scope.AddDateTime
        };

        var paramObj = {
            "Name": $scope.WaterMakingName,
           "PDFCondition": PDFConditionObj,
            "WaterMakingContent": WaterMakingContent
        };

        angular.element(".preloader").css("display", "block");
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'WaterMarking.ashx?op=AddPDFWaterMaking&accessToken=' + storage.getItem("userAdminToken"),
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            angular.element(".preloader").css("display", "none");
            var callbackObj = response.data;
            ErrorHandling(callbackObj.result, callbackObj.errCode, callbackObj.errMsg);
            if (callbackObj.result == "true") {
                $scope.GetPDFWaterMakingList(1);
            }
        }, function errorCallback(e) {

        });
    }

    //编辑
    $scope.ModifyPDFWaterMaking = function () {
        if ($scope.WaterMakingName == "") {
            ErrorHandling('false', 0, '请输入添加PDF水印规则名称。');
            return;
        }
        if ($scope.From == "" && $scope.IsAllFrom == false &&  $scope.Recipients == ""
            && $scope.Subject == "" && $scope.PDFName == "") {
            ErrorHandling('false', 0, '请输入添加PDF水印规则条件。');
            return;
        }

        if ($scope.IsAllRecipients == "0" && $scope.Content == "") {
            ErrorHandling('false', 0, '请输入添加PDF水印规则内容。');
            return;
        }

        var PDFConditionObj = {
            "From": $scope.From,
            "IsAllFrom": $scope.IsAllFrom,
            "ExcludeFroms": $scope.ExcludeFroms,
            "Recipients": $scope.Recipients,
            "Subject": $scope.Subject,
            "PDFName": $scope.PDFName
        };

        var WaterMakingContent = {
            "IsAllRecipients": $scope.IsAllRecipients == "1" ? "true" : "false",
            "Content": $scope.Content,
            "IsAddDate": $scope.AddDateTime
        };

        var paramObj = {
            "ID": $scope.ModifyPDFWaterMakingObjectID,
            "Name": $scope.WaterMakingName,
            "PDFCondition": PDFConditionObj,
            "WaterMakingContent": WaterMakingContent
        };

        angular.element(".preloader").css("display", "block");
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'WaterMarking.ashx?op=ModifyPDFWaterMaking&accessToken=' + storage.getItem("userAdminToken"),
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            angular.element(".preloader").css("display", "none");
            var callbackObj = response.data;
            ErrorHandling(callbackObj.result, callbackObj.errCode, callbackObj.errMsg);
            if (callbackObj.result == "true") {
                $scope.GetPDFWaterMakingList(1);
            }
        }, function errorCallback(e) {

        });
    }

    //删除
    $scope.delPDFWMModal = function (id) {
        $("#delpdfwm_Modal").modal("show");
        $scope.nomalModal();
        $scope.delPDFWmID = id;
    };

    $scope.DelPDFWM = function () {
        var paramObj = {
            "ID": $scope.delPDFWmID
        };
        $scope.operateProgress();
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'WaterMarking.ashx?op=DeletePDFWaterMaking&accessToken=' + storage.getItem("userAdminToken"),
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            var callbackObj = response.data;
            if (callbackObj.result == "true") {
                $scope.showMsg('success', false, callbackObj.errMsg, 1);
                $scope.showModalFoot = false;
                $scope.GetPDFWaterMakingList(1);
            } else {
                $scope.showMsg('error', false, callbackObj.errMsg, 1);
                $scope.showModalFoot = true;
            }
        }, function errorCallback(e) {

        });
    }

    //获取详情
    $scope.GetPDFWaterMakingInfo = function (id) {
        $scope.ModifyPDFWaterMakingObjectID = id;
        var paramObj = {
            "ID": id
        };

        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'WaterMarking.ashx?op=GetPDFWaterMakingInfo&accessToken=' + storage.getItem("userAdminToken") + '&ID=' + id,
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            var ListData = response.data;
            if (ListData.result == "true") {
                $scope.ModifyPDFWaterMakingObjectID = id;
                $scope.WaterMakingName = ListData.data.Name;
                $scope.From = ListData.data.PDFCondition.From;
                $scope.IsAllFrom = ListData.data.PDFCondition.IsAllFrom;
                $scope.ExcludeFroms = ListData.data.PDFCondition.ExcludeFroms;
                $scope.Recipients = ListData.data.PDFCondition.Recipients;
                $scope.Subject = ListData.data.PDFCondition.Subject;
                $scope.PDFName = ListData.data.PDFCondition.PDFName;
                $scope.Status = ListData.data.Status;
                if (ListData.data.WaterMakingContent.IsAllRecipients == true) {
                    $scope.IsAllRecipients = 1;
                }
                else {
                    $scope.IsAllRecipients = 0;
                    $scope.Content = ListData.data.WaterMakingContent.Content;
                }
                $scope.AddDateTime = ListData.data.WaterMakingContent.IsAddDate;
            } else {
                $scope.showMsg('error', false, callbackObj.errMsg, 1);
                $scope.showModalFoot = true;
            }
        }, function errorCallback(e) {

        });
    };

    $scope.CheckAllFrom = function ($event) {
        if ($scope.IsAllFrom == true) {
            $scope.From = "";
        }
    };

    $scope.CheckIsAllRecipients = function ($event) {
        if ($scope.IsAllRecipients == 1) {
            $scope.Content = "";
        }
    };

    //停用
    $scope.disablePDFWMModal = function (id) {
        $("#disPDFWM_Modal").modal("show");
        $scope.nomalModal();
        $scope.disPDFWmID = id;
    }

    $scope.DisablePDFWM = function () {
        var paramObj = {
            "ID": $scope.disPDFWmID
        };
        $scope.operateProgress();
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'WaterMarking.ashx?op=DisablePDFWaterMaking&accessToken=' + storage.getItem("userAdminToken"),
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            var callbackObj = response.data;
            if (callbackObj.result == "true") {
                $scope.showMsg('success', false, callbackObj.errMsg, 1);
                $scope.showModalFoot = false;
                $scope.GetPDFWaterMakingList(1);
            } else {
                $scope.showMsg('error', false, callbackObj.errMsg, 1);
                $scope.showModalFoot = true;
            }
        }, function errorCallback(e) {

        });
    }

    //启用
    $scope.EnablePDFWM = function (id) {
        var paramObj = {
            "ID": id
        };
        $scope.operateProgress();
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'WaterMarking.ashx?op=EnablePDFWaterMaking&accessToken=' + storage.getItem("userAdminToken"),
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            var callbackObj = response.data;
            if (callbackObj.result == "true") {
                //$scope.showMsg('success', false, callbackObj.errMsg, 1);
                //$scope.showModalFoot = false;
                $scope.GetPDFWaterMakingList(1);
            } else {
                //$scope.showMsg('error', false, callbackObj.errMsg, 1);
                //$scope.showModalFoot = true;
            }
        }, function errorCallback(e) {

        });
    }

    //正常模态框状态
    $scope.nomalModal = function () {
        angular.element("input").removeClass("error");
        angular.element("textarea").removeClass("error");
        angular.element("select").removeClass("error");
        $(".loading").css("visibility", "hidden");
        angular.element(".modalBtn").text("确 定");
        $scope.checkTips = false;
        $scope.showModalFoot = true;
        $scope.modalButton = false;
        $scope.errorMsg = "";
        $scope.opResult = "";
    };
    //模态框按钮执行中状态
    $scope.operateProgress = function () {
        $(".loading").css("visibility", "visible");
        angular.element(".modalBtn").text("执行中");
        $scope.showModalFoot = true;
        $scope.modalButton = true;
    };

    //弹出框报错信息
    $scope.showMsg = function (opResult, checkTips,
        errorMsg, inputIndex) {
        $(".loading").css("visibility", "hidden");
        $scope.opResult = opResult;
        $scope.checkTips = checkTips;
        $scope.errorMsg = errorMsg;
        angular.element(".modalBtn").text("确 定");
        $scope.redBorder = parseInt(inputIndex);
        if (opResult == 'error') {
            $scope.showModalFoot = true;
            $scope.modalButton = false;
        } else {
            $scope.showModalFoot = false;
        }
    };

});