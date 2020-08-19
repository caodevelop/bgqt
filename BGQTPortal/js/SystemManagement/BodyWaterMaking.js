angular.module('app').controller('BodyWMCtrl', function (AuthVerify, $scope, $http, $state, $stateParams, $location, $window, $timeout) {
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
    $scope.ModifyBodyWaterMakingObjectID = '';
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

    $scope.GetBodyWaterMakingList = function (currentPage) {
        editFlag = true;
        $scope.currentPage = currentPage;
        $scope.ViewOrAdd = true;
        $scope.Lists = [];
        $scope.tabNum = 'tab1';
        //angular.element(".panel-heading").css("border-bottom", "none");
        $http({
            method: 'GET',
            url: storage.getItem("InterfaceUrl") + 'WaterMarking.ashx?op=GetBodyWaterMakingList&accessToken=' + storage.getItem("userAdminToken") + '&PageSize=' + $scope.pSize + '&CurPage=' + $scope.currentPage + '&Searchstr=' + encodeURI($scope.Searchstr),
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
                        "BodyCondition": ListData[n].BodyCondition.BodyCondition,
                        "WaterMakingContent": ListData[n].WaterMakingContent.WaterMakingContent,
                        "CreateTimeName": ListData[n].CreateTimeName.substr(0, 16),
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
                            "BodyCondition": "",
                            "WaterMakingContent": "",
                            "CreateTimeName": "",
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
        $scope.GetBodyWaterMakingList(currentPage);
    };

    $scope.clearSearch = function () {
        $scope.Searchstr = '';
        $scope.GetBodyWaterMakingList(1);
    };

    $scope.changePageSize = function () {
        $scope.GetBodyWaterMakingList(1);
    };

    //添加 or 编辑
    $scope.gotoBodyWMModal = function (op, id) {
        $scope.ViewOrAdd = false;
        angular.element(".panel-heading").css("border-bottom", "1px solid #d8d8d8");
        $scope.seachNode = '';
        $scope.op = op;
        if (op == 'edit') {
            $scope.OpTitle = "编辑规则";
            $scope.GetBodyWaterMakingInfo(id);
        } else {
            $scope.OpTitle = "添加规则";
            $scope.WaterMakingName = '';
            $scope.From = '';
            $scope.Recipients = '';
            $scope.Subject = '';
            $scope.IsContainsAttachment = false;
            $scope.AttachmentName = '';
            $scope.Content = '';
            $scope.IsAllRecipients = 1;
        }
    };

    //添加
    $scope.AddBodyWaterMaking = function () {
        if ($scope.WaterMakingName == "") {
            ErrorHandling('false', 0, '请输入添加邮件正文水印规则名称。');
            return;
        }
        if ($scope.From == "" && $scope.Recipients == ""
            && $scope.Subject == "" && $scope.AttachmentName == "") {
            ErrorHandling('false', 0, '请输入添加邮件正文水印规则条件。');
            return;
        }

        if ($scope.IsAllRecipients == "0" && $scope.Content == "") {
            ErrorHandling('false', 0, '请输入添加邮件正文水印规则内容。');
            return;
        }

        var BodyConditionObj = {
            "From": $scope.From,
            "Recipients": $scope.Recipients,
            "Subject": $scope.Subject,
            "IsContainsAttachment": $scope.IsContainsAttachment,
             "AttachmentName": $scope.AttachmentName
        };

        var WaterMakingContent = {
            "IsAllRecipients": $scope.IsAllRecipients == "1" ? "true" : "false",
            "Content": $scope.Content
        };

        var paramObj = {
            "Name": $scope.WaterMakingName,
            "BodyCondition": BodyConditionObj,
            "WaterMakingContent": WaterMakingContent
        };

        angular.element(".preloader").css("display", "block");
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'WaterMarking.ashx?op=AddBodyWaterMaking&accessToken=' + storage.getItem("userAdminToken"),
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            angular.element(".preloader").css("display", "none");
            var callbackObj = response.data;
            ErrorHandling(callbackObj.result, callbackObj.errCode, callbackObj.errMsg);
            if (callbackObj.result == "true") {
                $scope.GetBodyWaterMakingList(1);
            }
        }, function errorCallback(e) {

        });
    }

    //编辑
    $scope.ModifyBodyWaterMaking = function () {
        if ($scope.WaterMakingName == "") {
            ErrorHandling('false', 0, '请输入添加邮件正文水印规则名称。');
            return;
        }
        if ($scope.From == "" && $scope.Recipients == ""
            && $scope.Subject == "" && $scope.AttachmentName == "") {
            ErrorHandling('false', 0, '请输入添加邮件正文水印规则条件。');
            return;
        }

        if ($scope.IsAllRecipients == "0" && $scope.Content == "") {
            ErrorHandling('false', 0, '请输入添加邮件正文水印规则内容。');
            return;
        }

        var BodyConditionObj = {
            "From": $scope.From,
            "Recipients": $scope.Recipients,
            "Subject": $scope.Subject,
            "IsContainsAttachment": $scope.IsContainsAttachment,
            "AttachmentName": $scope.AttachmentName
        };

        var WaterMakingContent = {
            "IsAllRecipients": $scope.IsAllRecipients == "1" ? "true" : "false",
            "Content": $scope.Content
        };

        var paramObj = {
            "ID": $scope.ModifyPDFWaterMakingObjectID,
            "Name": $scope.WaterMakingName,
            "BodyCondition": BodyConditionObj,
            "WaterMakingContent": WaterMakingContent
        };

        angular.element(".preloader").css("display", "block");
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'WaterMarking.ashx?op=ModifyBodyWaterMaking&accessToken=' + storage.getItem("userAdminToken"),
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            angular.element(".preloader").css("display", "none");
            var callbackObj = response.data;
            ErrorHandling(callbackObj.result, callbackObj.errCode, callbackObj.errMsg);
            if (callbackObj.result == "true") {
                $scope.GetBodyWaterMakingList(1);
            }
        }, function errorCallback(e) {

        });
    }

    //删除
    $scope.delBodyWMModal = function (id) {
        $("#delBodyWM_Modal").modal("show");
        $scope.nomalModal();
        $scope.delBodyWmID = id;
    };

    $scope.DelBodyWM = function () {
        var paramObj = {
            "ID": $scope.delBodyWmID
        };
        $scope.operateProgress();
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'WaterMarking.ashx?op=DeleteBodyWaterMaking&accessToken=' + storage.getItem("userAdminToken"),
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            var callbackObj = response.data;
            if (callbackObj.result == "true") {
                $scope.showMsg('success', false, callbackObj.errMsg, 1);
                $scope.showModalFoot = false;
                $scope.GetBodyWaterMakingList(1);
            } else {
                $scope.showMsg('error', false, callbackObj.errMsg, 1);
                $scope.showModalFoot = true;
            }
        }, function errorCallback(e) {

        });
    }

    //获取详情
    $scope.GetBodyWaterMakingInfo = function (id) {
        $scope.ModifyBodyWaterMakingObjectID = id;
        var paramObj = {
            "ID": id
        };

        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'WaterMarking.ashx?op=GetBodyWaterMakingInfo&accessToken=' + storage.getItem("userAdminToken") + '&ID=' + id,
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            var ListData = response.data;
            if (ListData.result == "true") {
                $scope.ModifyPDFWaterMakingObjectID = id;
                $scope.WaterMakingName = ListData.data.Name;
                $scope.From = ListData.data.BodyCondition.From;
                $scope.Recipients = ListData.data.BodyCondition.Recipients;
                $scope.Subject = ListData.data.BodyCondition.Subject;
                $scope.IsContainsAttachment = ListData.data.BodyCondition.IsContainsAttachment;
                $scope.AttachmentName = ListData.data.BodyCondition.AttachmentName;
                if (ListData.data.WaterMakingContent.IsAllRecipients == true) {
                    $scope.IsAllRecipients = 1;
                }
                else {
                    $scope.IsAllRecipients = 0;
                    $scope.Content = ListData.data.WaterMakingContent.Content;
                }
            } else {
                $scope.showMsg('error', false, callbackObj.errMsg, 1);
                $scope.showModalFoot = true;
            }
        }, function errorCallback(e) {

        });
    };

    $scope.CheckContainsAttachment = function ($event) {
        if ($scope.IsContainsAttachment == false) {
            $scope.AttachmentName = "";
        }
    };

    $scope.CheckIsAllRecipients = function ($event) {
        if ($scope.IsAllRecipients == 1) {
            $scope.Content = "";
        }
    };

    

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