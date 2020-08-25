angular.module('app').controller('homeCtrl', function (AuthVerify, $scope, $http, $state, $stateParams, $location, $window) {
    $scope.adminSetting = true;
    $scope.maskDiv = true;
    $scope.maskDiv2 = true;
    $scope.orgName = "上海宝钢气体有限公司";
    $scope.modalStatus = '';
    $scope.UpdateData = {};
    $scope.data = {
        file: null
    };
    editFlag = false;
    $scope.timer = false;
    $scope.timeout = 60000;
    $scope.timerCount = $scope.timeout / 1000;
    $scope.RoleHAB = false;
    $scope.RolePAG = false;
    $scope.RoleQuit = false;
    $scope.RoleSCG = false;
    $scope.RoleRP = false;
    $scope.RoleMD = false;
    $scope.RoleSM = false;
    $scope.RoleMA = false;
    $scope.RolePDFWM = false;
    $scope.RoleBodyWM = false;
    $scope.text = "获取安全码";
    angular.element(".content-left").css("height", $window.innerHeight - 84);
    window.onresize = function () {
        $scope.$apply(function () {
            if ($window.innerWidth >= 1024) {
                $(".content-left").css("height", $window.innerHeight - 84);
            } else {
                $(".content-left").css("height", $window.innerHeight - 4);
            }
            console.log($window.innerWidth)
        })
    };
    $scope.adminName = storage.getItem("UserName");
    $scope.currentAdminRoleLists = [];
    $scope.currentAdminRoleLists = JSON.parse(storage.getItem('currentAdminRoleObj'));
    console.log($scope.currentAdminRoleLists);
    $scope.showOrHideProfile = function (type) {
        $scope.adminSetting = type;
        if ($scope.adminSetting) {
            $scope.maskDiv = true;
        } else {
            $scope.maskDiv = false;
        }
    }
    $scope.$watch('adminSetting', function (n, o) {
        if ($scope.adminSetting) {
            $scope.maskDiv = true;
        } else {
            $scope.maskDiv = false;
        }
    })
    // 退出
    $scope.loginoutSystem = function () {
        loginoutSystem();
    }

    $scope.closeMenu = function () {
        $scope.adminSetting = true;
    }
    console.log(storage.getItem("InterfaceUrl"));
    if (storage.getItem("userAdminToken") != null && storage.getItem("userAdminToken") != undefined) {
        if ($scope.currentAdminRoleLists.length != 0) {
            var RoleLevelArr = [];
            for (var i = 0; i < $scope.currentAdminRoleLists.length; i++) {
                var RoleLevelStr = $scope.currentAdminRoleLists[i].RoleLevel;
                RoleLevelArr.push(RoleLevelStr);
            }
            if (RoleLevelArr.indexOf(5) != -1) {
                $scope.RolePAG = true;
            } else {
                $scope.RolePAG = false;
            }
            if (RoleLevelArr.indexOf(6) != -1) {
                $scope.RoleSCG = true;
            } else {
                $scope.RoleSCG = false;
            }
            if (RoleLevelArr.indexOf(7) != -1) {
                $scope.RoleSM = true;
            } else {
                $scope.RoleSM = false;
            }
            if (RoleLevelArr.indexOf(8) != -1) {
                $scope.RoleSOU = true;
            } else {
                $scope.RoleSOU = false;
            }
            if (RoleLevelArr.indexOf(9) != -1) {
                $scope.RoleHAB = true;
            } else {
                $scope.RoleHAB = false;
            }
            if (RoleLevelArr.indexOf(10) != -1) {
                $scope.RoleQuit = true;
            } else {
                $scope.RoleQuit = false;
            }
            if (RoleLevelArr.indexOf(11) != -1) {
                $scope.RolePDFWM = true;
                $scope.RoleBodyWM = true;
            } else {
                $scope.RolePDFWM = false;
                $scope.RoleBodyWM = false;
            }
            if (RoleLevelArr.indexOf(12) != -1) {
                $scope.RoleMA = true;
            } else {
                $scope.RoleMA = false;
            }
        }
    } else {
        var thisCurrentUrl = window.location.href;
        var index = thisCurrentUrl.lastIndexOf("\#");
        var localUrl = thisCurrentUrl.substr(0, index);
        window.location.href = localUrl + "#/logOnOU";
        //var index = obj.lastIndexOf("\-");
        //obj = obj.substring(index + 1, obj.length);
        //  console.log(obj);
    }
   

    $(function () {
        $(".subNav").click(function () {
            $(this).toggleClass("currentDd").siblings(".subNav").removeClass("currentDd");
            $(this).toggleClass("currentDt").siblings(".subNav").removeClass("currentDt");
            $(this).next(".navContent").slideToggle(0).siblings(".navContent").slideUp(0); 
            $(this).next(".navContent").find("li:first-child a").click();
            if ($(this)[0].id == "nav_Log") {
                storage.setItem("ActiveNavigation", $(this)[0].id);
                $(this).addClass("selectedLogItem").siblings(".navContent li").removeClass("selectedItem");
                $(".subNavBox").find(".navContent li").removeClass("selectedItem");
                $(".navContent").css("display","none"); 
            } else {
                $("#nav_Log").removeClass("selectedLogItem");
            }
            if ($(this)[0].id == "nav_Dashboard") {
                storage.setItem("ActiveNavigation", $(this)[0].id);
                $(this).addClass("selectedItem").siblings(".navContent li").removeClass("selectedItem");
                $(".subNavBox").find(".navContent li").removeClass("selectedItem");
                $(".navContent").css("display", "none");
            } else {
                $("#nav_Dashboard").removeClass("selectedItem");
            }
        })
        $(".navContent li a").click(function () {
            storage.setItem("ActiveNavigation", $(this)[0].id);
            $(this).parents().find(".navContent li").removeClass("selectedItem");
            $(this).parent().addClass("selectedItem").siblings(".navContent li").removeClass("selectedItem");
            $("#nav_Log").removeClass("selectedLogItem");
            $("#nav_Dashboard").removeClass("selectedItem");
            //if (storage.getItem("ActiveNavigation") == $(this)[0].id) {
            //    window.location.reload();
            //}
        })
    })

    if (storage.getItem("ActiveNavigation") != null) {
        console.log(storage.getItem("ActiveNavigation"));
        if (storage.getItem("ActiveNavigation") == "nav_Log") {
            $("#nav_Log").addClass("selectedLogItem").siblings(".navContent li").removeClass("selectedItem");
            $(".subNavBox").find(".navContent li").removeClass("selectedItem");
        }
        else if (storage.getItem("ActiveNavigation") == "nav_Dashboard"){
            $("#nav_Dashboard").addClass("selectedItem").siblings(".navContent li").removeClass("selectedItem");
            $(".subNavBox").find(".navContent li").removeClass("selectedItem");
        }else {
            $("#" + storage.getItem("ActiveNavigation")).parent().addClass("selectedItem").siblings(".navContent li").removeClass("selectedItem");
            $("#" + storage.getItem("ActiveNavigation")).parents(".navContent").prev(".subNav").addClass("currentDd currentDt").siblings(".subNav").removeClass("currentDd currentDt");
            $("#" + storage.getItem("ActiveNavigation")).parents(".navContent").slideToggle(0).siblings(".navContent").slideUp(0);
        }
    } else {
        $(".OrgStructure").addClass("currentDd currentDt").siblings(".subNav").removeClass("currentDd currentDt");  
        $(".OrgStructure").next(".navContent").slideToggle(0).siblings(".navContent").slideUp(0);
        $(".OrgStructure").next(".navContent").find("li:first-child a").click();
        $state.go("Home.Org");
        //$("#nav_Dashboard").addClass("selectedItem").siblings(".navContent li").removeClass("selectedItem");
        //$(".subNavBox").find(".navContent li").removeClass("selectedItem");
        //$state.go("Home.Dashboard");
      
    }
  
});