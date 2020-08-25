var regPwd = /^[^&<\ \u4e00-\u9fa5]+$/;
var regSpecialCharacter = /^[^!/@#$%^*()-_=+\\|{}];:\'\",<.>]+$ /;
var storage = window.localStorage;
var timestamp = new Date().getTime();
var AccountExpiration = '用户身份';
var httpUrl;
var CompanyTree, OrgTree, OUAndUserTree, PublicUserTree, GroupTree, SameOUTree, HabGroupTree, HabGroupTreeModal, treeSelectNode, treeSelectNodeID;
var TreesType = 0;
var selectTree = false;
var currentAdminRoleLists = [];
var editFlag = false;

var app = angular.module('app', ['ui.router', 'oc.lazyLoad', 'ngFileUpload', 'ui.bootstrap','highcharts-ng'], function ($httpProvider) {
    baseRequest($httpProvider);
});

app.factory('AuthVerify', function ($http, $q) {
    return {
        GetBasicConfig: function () {
            return $http.get('Config.xml');
        }
    }
});
//显示页面遮罩层
function showParentMask(displayStatus) {
    var maskObj = {
        "MaskStatus": displayStatus
    }
    window.parent.postMessage(maskObj, '*');
}

//接口返回数据提示
function ErrorHandling(result, errCode, errMsg) {
    $(".msgBoxDIV").css("display", "block");
    $("#msgBoxDIV").html(errMsg);
    setTimeout("$('.msgBoxDIV').fadeOut('slow')", 3000);
    if (result != 'false') {
        $(".msgBoxDIV").addClass("success").removeClass("error");
    } else {
        if (errCode == '8002' || errCode == '8003' || errCode == '8004') {
            loginoutSystem();
        } else {
            $(".msgBoxDIV").addClass("error").removeClass("success");
        }
    }
}

// 退出
function loginoutSystem() {
    storage.removeItem("ContactID");
    storage.removeItem("userAdminToken");
    storage.removeItem("ActiveNavigation");
    storage.removeItem("currentAdminRoleObj"); 
    window.location.href = storage.getItem("loginUrl");
}

//封装post请求
function baseRequest($httpProvider) {
    // Use x-www-form-urlencoded Content-Type
    $httpProvider.defaults.headers.post['Content-Type'] = 'application/x-www-form-urlencoded;charset=utf-8';
    /**
     * The workhorse; converts an object to x-www-form-urlencoded serialization.
     * @param {Object} obj
     * @return {String}
     */
    var param = function (obj) {
        var query = '', name, value, fullSubName, subName, subValue, innerObj, i;

        for (name in obj) {
            value = obj[name];

            if (value instanceof Array) {
                for (i = 0; i < value.length; ++i) {
                    subValue = value[i];
                    fullSubName = name + '[' + i + ']';
                    innerObj = {};
                    innerObj[fullSubName] = subValue;
                    query += param(innerObj) + '&';
                }
            }
            else if (value instanceof Object) {
                for (subName in value) {
                    subValue = value[subName];
                    fullSubName = name + '[' + subName + ']';
                    innerObj = {};
                    innerObj[fullSubName] = subValue;
                    query += param(innerObj) + '&';
                }
            }
            else if (value !== undefined && value !== null)
                query += encodeURIComponent(name) + '=' + encodeURIComponent(value) + '&';
        }

        return query.length ? query.substr(0, query.length - 1) : query;
    };

    // Override $http service's default transformRequest
    $httpProvider.defaults.transformRequest = [function (data) {
        return angular.isObject(data) && String(data) !== '[object File]' ? param(data) : data;
    }];
}

function GetQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]); return null;
}