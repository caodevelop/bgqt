angular.module('app').controller('resetPwdCtrl', function (AuthVerify, $scope, $http, $state, $stateParams, $location, $window) {
    $scope.formTitle = '修改密码';
    $scope.resetPwdStepOne = true;
    $scope.User = {};
    $scope.User.Account = '';
    $scope.User.OldPwd = '';
    $scope.User.NewPwd = '';
    $scope.User.loginCode = '';
    $scope.User.VCode = '';
    $scope.RePwdBtn = false;
    AuthVerify.GetBasicConfig().success(function (response) {
        var productsElements = angular.element(response.trim()).find("configUrl");
        for (var i = 0; i < productsElements.length; i++) {
            storage.setItem(productsElements.eq(i).attr("name"), productsElements.eq(i).attr("value"));
        }
        $scope.RefreshCode();
    })

    $scope.RefreshCode = function () {
        $http({
            method: 'GET',
            url: storage.getItem("InterfaceUrl") + 'OrgAndUser.ashx?op=GetValKey'
        }).then(function successCallback(response) {
            var callbackObj = response.data;
            if (callbackObj.result == "true") {
                $scope.User.loginCode = callbackObj.data[0].Key;
                angular.element('#login-code').css('background', 'url(' + storage.getItem("InterfaceUrl") + 'ShowImg.ashx?type=codeImg&Key=' + $scope.User.loginCode + ')no-repeat center center transparent');
            } else {
                $scope.opResult = 'error';
                $scope.checkTips = false;
                $scope.errorMsg = "获取验证码失败。";
            }
        }, function errorCallback(e) {

        });
    }

    $scope.ChangeUserPassword = function () {
        $scope.errorMsg = "";
        $scope.opResult = "true";
        $scope.checkTips = true;
        if ($scope.passwordLevel($scope.User.NewPwd) < 3 || !regPwd.test($scope.User.NewPwd) || $scope.User.NewPwd.length < 8 || $scope.User.NewPwd.length > 20) {
            $scope.opResult = 'error';
            $scope.checkTips = false;
            $scope.errorMsg = '密码支持 8~20 个字符，包含字母、数字、标点符号；不支持空格，"&"，"<"；';
            return;
        }
        if ($scope.User.NewPwd != $scope.User.RePwd) {
            $scope.opResult = 'error';
            $scope.checkTips = false;
            $scope.errorMsg = "两次密码不一致。";
            return;
        }
        var paramObj = {
            "account": $scope.User.Account,
            "oldpassword": $scope.User.OldPwd,
            "newpassword": $scope.User.NewPwd,
            "Key": $scope.User.loginCode,
            "Code": $scope.User.VCode
        }
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'OrgAndUser.ashx?op=ChangeUserPassword',
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            var callbackObj = response.data;
            if (callbackObj.result == "true") {
                $scope.formTitle = '密码重置成功';
                $scope.resetPwdStepOne = false;
            } else {
                $scope.opResult = 'error';
                $scope.checkTips = false;
                $scope.errorMsg = callbackObj.errMsg;
                $scope.User.VCode = '';
                $scope.RefreshCode();
            }
        }, function errorCallback(e) {

        });
    }

    $scope.checkSubmit = function () {
        if ($scope.User.Account != '' && $scope.User.OldPwd != '' && $scope.User.NewPwd != '' && $scope.User.RePwd != '' && $scope.User.VCode) {
            $scope.RePwdBtn = true;
        } else {
            $scope.RePwdBtn = false;
        }
    }

    //密码强度检测
    $scope.passwordLevel = function (password) {
        var Modes = 0;
        for (i = 0; i < password.length; i++) {
            Modes |= CharMode(password.charCodeAt(i));
        }
        return bitTotal(Modes);

        //CharMode函数
        function CharMode(iN) {
            if (iN >= 48 && iN <= 57)//数字
                return 1;
            if (iN >= 65 && iN <= 90) //大写字母
                return 2;
            if ((iN >= 97 && iN <= 122) || (iN >= 65 && iN <= 90)) //大小写
                return 4;
            else
                return 8; //特殊字符
        }

        //bitTotal函数
        function bitTotal(num) {
            modes = 0;
            for (i = 0; i < 4; i++) {
                if (num & 1) modes++;
                num >>>= 1;
            }
            return modes;
        }
    }
});