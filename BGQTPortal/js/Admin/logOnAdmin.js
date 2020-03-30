angular.module('app').controller('logOnAdminCtrl', function (AuthVerify, $scope, $http, $state, $stateParams, $location, $timeout) {
    //登录用户
    $scope.logOnUser = {};
    $scope.logOnUser.Account = '';
    $scope.logOnUser.ContactID = '';
    $scope.logOnUser.Password = '';
    $scope.logOnUser.loginCode = '';
    $scope.logOnUser.VCode = '';
    $scope.SignInBtn = false;
    $scope.opResult1 = 'true';
    $scope.checkTips1 = true;
    $scope.errorMsg1 = "";
    //重置密码用户
    $scope.resetUser = {};
    $scope.resetUser.Account = '';
    $scope.resetUser.OldPwd = '';
    $scope.resetUser.NewPwd = '';
    $scope.resetUser.RePwd = '';
    $scope.resetUser.loginCode = '';
    $scope.resetUser.VCode = '';
    $scope.RePwdBtn = false;
    $scope.opResult2 = 'true';
    $scope.checkTips2 = true;
    $scope.errorMsg2 = "";
    storage.removeItem("ActiveNavigation");
    AuthVerify.GetBasicConfig().success(function (response) {
        var productsElements = angular.element(response.trim()).find("configUrl");
        for (var i = 0; i < productsElements.length; i++) {
            storage.setItem(productsElements.eq(i).attr("name"), productsElements.eq(i).attr("value"));
        }
        $scope.RefreshCodeLogOnUser();
        $scope.RefreshCodeResetUser();
    })
    //登录用户
    $scope.RefreshCodeLogOnUser = function () {
        $http({
            method: 'GET',
            url: storage.getItem("InterfaceUrl") + 'OrgAndUser.ashx?op=GetValKey'
        }).then(function successCallback(response) {
            var callbackObj = response.data;
            if (callbackObj.result == "true") {
                $scope.logOnUser.loginCode = callbackObj.data[0].Key;
                angular.element('#loginUser-code').css('background', 'url(' + storage.getItem("InterfaceUrl") + 'ShowImg.ashx?type=codeImg&Key=' + $scope.logOnUser.loginCode + ')no-repeat center center transparent');
            } else {
                $scope.opResult1 = 'error';
                $scope.checkTips1 = false;
                $scope.errorMsg1 = "获取验证码失败。";
            }
        }, function errorCallback(e) {

        });
    }

    $scope.SignIn = function () {
        currentAdminRoleLists = [];
        $scope.errorMsg1 = "";
        $scope.opResult1 = "true";
        $scope.checkTips1 = true;
        var paramObj = {
            "account": $scope.logOnUser.Account,
            "password": $scope.logOnUser.Password,
            "key": $scope.logOnUser.loginCode,
            "code": $scope.logOnUser.VCode
        }
        $http({
              method: 'post',
              url: storage.getItem("InterfaceUrl") + 'OrgAndUser.ashx?op=Login',
              data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            var callbackObj = response.data;
            if (callbackObj.result == "true") {
                storage.setItem("userAdminToken", callbackObj.data.Token);
                storage.setItem("UserName", callbackObj.data.DisplayName);
                storage.setItem("IsCompanyAdmin", callbackObj.data.IsCompanyAdmin);
                for (var i = 0; i < callbackObj.data.RoleList.length; i++) {
                    for (var j = 0; j < callbackObj.data.RoleList[i].RoleParamList.length; j++) {
                        var arrObj = {
                            "ParamID": callbackObj.data.RoleList[i].RoleParamList[j].ParamID,
                            "RoleLevel": callbackObj.data.RoleList[i].ModuleType
                        }
                        currentAdminRoleLists.push(arrObj);
                    }
                }
                storage.setItem("currentAdminRoleObj", JSON.stringify(currentAdminRoleLists));
                if (callbackObj.data.ParamList.SpanOUMove == 9) {
                    storage.setItem("SpanOUMove", true);
                } else {
                    storage.setItem("SpanOUMove", false);
                }
                if (callbackObj.data.ParamList.ModifyProfessionalGroup == 18) {
                    storage.setItem("ModifyProfessionalGroup", true);
                } else {
                    storage.setItem("ModifyProfessionalGroup", false);
                }
                if (callbackObj.data.ParamList.SyncHab == 19) {
                    storage.setItem("SyncHab", true);
                } else {
                    storage.setItem("SyncHab", false);
                }                
                $state.go("Home");
            } else {
                $scope.opResult1 = 'error';
                $scope.checkTips1 = false;
                $scope.errorMsg1 = callbackObj.errMsg;
                $scope.logOnUser.VCode = '';
                $scope.RefreshCodeLogOnUser();
            }
        }, function errorCallback(e) {

        });
    }

    $scope.focusLogOn = function () {
        $scope.resetUser.Account = '';
        $scope.resetUser.OldPwd = '';
        $scope.resetUser.NewPwd = '';
        $scope.resetUser.RePwd = '';
        $scope.resetUser.VCode = '';
        $scope.RePwdBtn = false;
        $scope.opResult2 = 'true';
        $scope.checkTips2 = true;
        $scope.errorMsg2 = "";
    }

    $scope.InputChange = function () {
        if ($scope.logOnUser.Account != '' && $scope.logOnUser.Password != '' && $scope.logOnUser.VCode) {
            $scope.SignInBtn = true;
        } else {
            $scope.SignInBtn = false;
        }
    }
    //重置密码用户
    $scope.RefreshCodeResetUser = function () {
        $http({
            method: 'GET',
            url: storage.getItem("InterfaceUrl") + 'OrgAndUser.ashx?op=GetValKey'
        }).then(function successCallback(response) {
            var callbackObj = response.data;
            if (callbackObj.result == "true") {
                $scope.resetUser.loginCode = callbackObj.data[0].Key;
                angular.element('#resetUser-code').css('background', 'url(' + storage.getItem("InterfaceUrl") + 'ShowImg.ashx?type=codeImg&Key=' + $scope.resetUser.loginCode + ')no-repeat center center transparent');
            } else {
                $scope.opResult2 = 'error';
                $scope.checkTips2 = false;
                $scope.errorMsg2 = "获取验证码失败。";
            }
        }, function errorCallback(e) {

        });
    }

    $scope.ChangeUserPassword = function () {
        $scope.errorMsg2 = "";
        $scope.opResult2 = "true";
        $scope.checkTips2 = true;
        if ($scope.passwordLevel($scope.resetUser.NewPwd) < 3 || !regPwd.test($scope.resetUser.NewPwd) || $scope.resetUser.NewPwd.length < 8 || $scope.resetUser.NewPwd.length > 20) {
            $scope.opResult2 = 'error';
            $scope.checkTips2 = false;
            $scope.errorMsg2 = '密码格式不对。';
            return;
        }
        if ($scope.resetUser.NewPwd != $scope.resetUser.RePwd) {
            $scope.opResult2 = 'error';
            $scope.checkTips2 = false;
            $scope.errorMsg2 = "两次密码不一致。";
            return;
        }
        var paramObj = {
            "account": $scope.resetUser.Account,
            "oldpassword": $scope.resetUser.OldPwd,
            "newpassword": $scope.resetUser.NewPwd,
            "Key": $scope.resetUser.loginCode,
            "Code": $scope.resetUser.VCode
        }
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'OrgAndUser.ashx?op=ChangeUserPassword',
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            var callbackObj = response.data;
            if (callbackObj.result == "true") {
                $scope.opResult2 = callbackObj.result;
                $scope.checkTips2 = false;
                $scope.errorMsg2 = callbackObj.errMsg;
                $scope.timer = $timeout(function () {
                    $scope.resetUser.Account = '';
                    $scope.resetUser.OldPwd = '';
                    $scope.resetUser.NewPwd = '';
                    $scope.resetUser.RePwd = '';
                    $scope.resetUser.VCode = '';
                    $scope.RePwdBtn = false;
                    $scope.opResult2 = 'true';
                    $scope.checkTips2 = true;
                    $scope.errorMsg2 = "";
                }, 3000);
            } else {
                $scope.opResult2 = 'error';
                $scope.checkTips2 = false;
                $scope.errorMsg2 = callbackObj.errMsg;
                $scope.resetUser.VCode = '';
                $scope.RefreshCodeResetUser();
            }
        }, function errorCallback(e) {

        });
    }

    $scope.focusResetPwd = function () {
        $scope.logOnUser.Account = '';
        $scope.logOnUser.ContactID = '';
        $scope.logOnUser.Password = '';
        $scope.logOnUser.VCode = '';
        $scope.SignInBtn = false;
        $scope.opResult1 = 'true';
        $scope.checkTips1 = true;
        $scope.errorMsg1 = "";
    }

    $scope.checkSubmit = function () {
        if ($scope.resetUser.Account != '' && $scope.resetUser.OldPwd != '' && $scope.resetUser.NewPwd != '' && $scope.resetUser.RePwd != '' && $scope.resetUser.VCode) {
            $scope.RePwdBtn = true;
        } else {
            $scope.RePwdBtn = false;
        }
    }

    $scope.clearMsg = function () {
        setTimeout(function () {
            $scope.resetUser.Account = '';
            $scope.resetUser.OldPwd = '';
            $scope.resetUser.NewPwd = '';
            $scope.resetUser.RePwd = '';
            $scope.resetUser.VCode = '';
            $scope.RePwdBtn = false;
            $scope.opResult2 = 'true';
            $scope.checkTips2 = true;
            $scope.errorMsg2 = "";
        }, 3000);
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

    //跳转
    $scope.gotoFogotPwd = function () {
        $state.go('GetUserPwd');
    };
});