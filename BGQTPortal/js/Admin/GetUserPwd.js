angular.module("app").controller('GetUserPwdCtrl', function (AuthVerify, $scope, $http, $window, $interval, $state) {
    $scope.User = {};
    $scope.User.VCode = '';
    $scope.User.Account = '';
    $scope.User.UserID = '';
    $scope.formTitle = '邮箱和VPN 找回密码';
    $scope.getPwdStepOne = true;
    $scope.getPwdStepTwo = false;
    $scope.getPwdStepThree = false;
    $scope.getPwdStepFour = false;
    $scope.RePwdStepOne = true;
    $scope.RePwdStepTwo = true;
    $scope.RePwdStepThree = true;
    $scope.CodeID = '';
    $scope.reg = /^(\w-*\.*)+@(\w-?)+(\.\w{2,})+$/;
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

    $scope.next = function () {
        if (!isNaN($scope.User.Account)) {
            $scope.CheckValCode();
        } else {
            $scope.opResult = 'error';
            $scope.checkTips = false;
            $scope.errorMsg = "请输入员工编号和验证码";
        }
    }
    
    $scope.CheckValCode = function () {
        var loginObj = {
            "EMPLID": $scope.User.Account,
            "Key": $scope.User.loginCode,
            "Code": $scope.User.VCode
        };
        var json = encodeURIComponent(JSON.stringify(loginObj));
        $http({
            method: 'Post',
            url: storage.getItem("InterfaceUrl") + 'OrgAndUser.ashx?op=GetUserAndCheckValCode',
            data: JSON.stringify(loginObj)
        }).then(function successCallback(response) {
            var callbackObj = response.data;
            if (callbackObj.result == "true") {
                $scope.User.Mobile = callbackObj.data.Mobile;
                $scope.User.UserID = callbackObj.data.UserID;
                $scope.getPwdStepOne = false;
                $scope.getPwdStepTwo = true;
                $scope.getPwdStepThree = false;
                $scope.getPwdStepFour = false;
                var second = 60, timePromise = undefined;
                $scope.paraevent = true;
                $scope.Text = "重新发送（" + second + "s）";
                timePromise = $interval(function () {
                    if (second <= 0) {
                        $interval.cancel(timePromise);
                        timePromise = undefined;
                        second = 60;
                        $scope.Text = "重新发送";
                        $scope.paraevent = true;
                    } else {
                        $scope.Text = "重新发送（" + second + "s）";
                        $scope.paraevent = false;
                        second--;
                    }
                }, 1000);
                if ($scope.User.Mobile !='') {
                    $scope.SendFUserSmsCode("Mobile");
                } 
            } else {
                $scope.opResult = 'error';
                $scope.checkTips = false;
                $scope.errorMsg = callbackObj.errMsg;
            }
        }, function errorCallback(e) {

        });
    }

    $scope.reSend = function () {
        if ($scope.User.Mobile!='' && $scope.User.UserID != '') {
            $scope.SendFUserSmsCode("Mobile");
        } 
        var second = 60, timePromise = undefined;
        $scope.paraevent = true;
        $scope.Text = "重新发送（" + second + "s）";
        timePromise = $interval(function () {
            if (second <= 0) {
                $interval.cancel(timePromise);
                timePromise = undefined;

                second = 60;
                $scope.Text = "重新发送";
                $scope.paraevent = true;
            } else {
                $scope.Text = "重新发送（" + second + "s）";
                $scope.paraevent = false;
                second--;

            }
        }, 1000);
    }

    $scope.SendFUserSmsCode = function (type) {
        var AdminRePwdUrl = '';
        if (type == "Mobile") {
            var json = {
                "UserID": $scope.User.UserID,
                "Mobile": $scope.User.Mobile
            }
            AdminRePwdUrl = storage.getItem("InterfaceUrl") + 'OrgAndUser.ashx?op=SendFUserSmsCode';
        }
     $http({
            method: 'Post',
            url: AdminRePwdUrl,
            data: JSON.stringify(json)
        }).then(function successCallback(response) {
            var callbackObj = eval("(" + response.data + ")");
            if (callbackObj[0].result == "true") {

            } else {
                $scope.opResult = 'error';
                $scope.checkTips = false;
                $scope.errorMsg = callbackObj[0].errMsg;
            }
        }, function errorCallback(e) {

        });
    }

    $scope.Verification = function () {
        var json = {
            "UserID": $scope.User.UserID,
            "Code": $scope.User.TwoVCode
        }
        $http({
            method: 'Post',
            url: storage.getItem("InterfaceUrl") + "OrgAndUser.ashx?op=CheckFUserSmsCode",
            data: JSON.stringify(json)
        }).then(function successCallback(response) {
            var callbackObj = eval("(" + response.data + ")");
            if (callbackObj[0].result == "true") {
                $scope.getPwdStepOne = false;
                $scope.getPwdStepTwo = false;
                $scope.getPwdStepThree = true;
                $scope.getPwdStepFour = false;
                $scope.formTitle = '邮箱和VPN  重置密码';
                $scope.CodeID = callbackObj[0].data[0].CodeID;
            } else {
                $scope.opResult = 'error';
                $scope.checkTips = false;
                $scope.errorMsg = callbackObj[0].errMsg;
            }
        }, function errorCallback(e) {

        });
    }

    $scope.resetPwd = function () {
        if ($scope.passwordLevel($scope.User.Password) < 3 || !regPwd.test($scope.User.Password) || $scope.User.Password.length < 8 || $scope.User.Password.length > 20) {
            $scope.opResult = 'error';
            $scope.checkTips = false;
            $scope.errorMsg = '密码不满足密码策略的要求。检查最小密码长度、密码复杂性和密码历史的要求。';
            return;
        }
        if ($scope.User.rePassword != $scope.User.Password) {
            $scope.opResult = 'error';
            $scope.checkTips = false;
            $scope.errorMsg = '两次输入的密码必须相同！';
            return;
        }
        var json = {
            "CodeID": $scope.CodeID,
            "Password": $scope.User.Password
        }
        angular.element(".login-btn").text("执行中");
        $scope.RePwdStepThree = true;
        $(".forgetPwd").css("visibility", "hidden");
        $http({
            method: 'Post',
            url: storage.getItem("InterfaceUrl") + "OrgAndUser.ashx?op=SetFUserPass",
            data: JSON.stringify(json)
        }).then(function successCallback(response) {
            var callbackObj = eval("(" + response.data + ")");
            if (callbackObj[0].result == "true") {
                $scope.getPwdStepOne = false;
                $scope.getPwdStepTwo = false;
                $scope.getPwdStepThree = false;
                $scope.getPwdStepFour = true;
                $scope.formTitle = '邮箱和VPN  重置密码';
                $scope.RePwdStepThree = false;
                angular.element(".login-btn").text("确 定");
                $(".forgetPwd").css("visibility", "visible");
            } else {
                $scope.opResult = 'error';
                $scope.checkTips = false;
                $scope.errorMsg = callbackObj[0].errMsg;
                $scope.RePwdStepThree = false;
                angular.element(".login-btn").text("确 定");
                $(".forgetPwd").css("visibility", "visible");
            }
        }, function errorCallback(e) {

        });
    }
    
    $scope.checkSubmit = function (type) {
        if (type == 'step1') {
            if ($scope.User.Account == "" || $scope.User.VCode == "") {
                $scope.RePwdStepOne = true;
            } else {
                $scope.RePwdStepOne = false;
            }
        } else if (type == 'step2') {
            if ($scope.User.TwoVCode == "") {
                $scope.RePwdStepTwo = true;
            } else {
                $scope.RePwdStepTwo = false;
            }
        } else if (type == 'step3') {
            if ($scope.User.Password == "" || $scope.User.rePassword == "") {
                $scope.RePwdStepThree = true;
            } else {
                $scope.RePwdStepThree = false;
            }
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

    $scope.gotoAdminLogin = function () {
        $state.go('logOnOU');
    }
})