angular.module('app').controller('SMCtrl', function (AuthVerify, $scope, $http, $state, $stateParams, $location, $window, $timeout) {
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
    $scope.StartTime = '';
    $scope.EndTime = '';
    $scope.Searchstr = '';
    $scope.op = '';
    $scope.seachNode = '';
    $scope.ModifySensitiveMailObjectID = '';
    $scope.ObjectID = '';
    $scope.ObjectLimits = [];
    $scope.ViewOrAdd = true;
    $scope.PageSizes = [{ "text": "10条/页", "value": "10" }, { "text": "20条/页", "value": "20" }, { "text": "50条/页", "value": "50" }, { "text": "100条/页", "value": "100" }];
    $scope.pSize = $scope.PageSizes[0].value;
    editFlag = true;
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

    $scope.GetSensitiveMailList = function (currentPage) {
        editFlag = true;
        $scope.currentPage = currentPage;
        $scope.ViewOrAdd = true;
        $scope.Lists = [];
        $scope.tabNum = 'tab1';
        //angular.element(".panel-heading").css("border-bottom", "none");
        $http({
            method: 'GET',
            url: storage.getItem("InterfaceUrl") + 'SensitiveMail.ashx?op=GetSensitiveMailList&accessToken=' + storage.getItem("userAdminToken") + '&PageSize=' + $scope.pSize + '&CurPage=' + $scope.currentPage + '&Searchstr=' + encodeURI($scope.Searchstr) + '&StartTime=' + $scope.StartTime + '&EndTime=' + $scope.EndTime,
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
                        "ObjectID": ListData[n].ObjectID,
                        "ObjectType": ListData[n].ObjectType,
                        "ObjectNames": ListData[n].ObjectNames,
                        "Keywords": ListData[n].Keywords,
                        "CreateTimeName": ListData[n].CreateTimeName.substr(0, 16),
                        "StartTimeName": ListData[n].StartTimeName.substr(0, 16),
                        "EndTimeName": ListData[n].EndTimeName.substr(0, 16),
                        "ExecuteTimeName": ListData[n].ExecuteTimeName.substr(0, 16),
                        "ExecuteResult": ListData[n].ExecuteResult,
                        "StatusName": ListData[n].Status == 3 ? ListData[n].StatusName + "，进度 " + ListData[n].PercentageComplete : ListData[n].StatusName,
                        "TimePeriod": ListData[n].StartTimeName + "~" + ListData[n].EndTimeName,
                        "trueData": 1
                    };
                    $scope.Lists.push($scope.arr);
                }
                if (ListData.length < $scope.pSize) {
                    for (i = 0; i < $scope.pSize - ListData.length; i++) {
                        var nulltr = {
                            "$$hashKey": i,
                            "ID": "",
                            "Name":"",
                            "ObjectID": "",
                            "ObjectType": "",
                            "ObjectNames": "",
                            "Keywords": "",
                            "CreateTimeName": "",
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
                $scope.timer = $timeout(function () {
                    if (editFlag) {
                        $scope.GetSensitiveMailList(currentPage);
                    }
                }, 30000);
            } else {
                ErrorHandling(data.result, data.errCode, data.errMsg);
            }
        }, function errorCallback(e) {
            setTimeout(function () {
                $scope.GetSensitiveMailList(currentPage);
            }, 30000);
        });
    }

    $scope.pageChanged = function (currentPage) {
        $scope.GetSensitiveMailList(currentPage);
    };



    //添加 or 编辑
    $scope.gotoSMailModal = function (op, id) {
        editFlag = false;
        $scope.ViewOrAdd = false;
        angular.element(".panel-heading").css("border-bottom", "1px solid #d8d8d8");
        $scope.seachNode = '';
        $scope.op = op;
        if (op == 'edit') {
            $scope.OpTitle = "编辑规则";
            $scope.GetSensitiveMailInfo(id);
        } else {
            $scope.OpTitle = "添加规则";
            $scope.SensitiveMailType = 1;
            $scope.ObjectLimits = [];
            $scope.SensitiveMailName = '';
            $scope.Keywords = '';
            $scope.SMailStartTime = '';
            $scope.SMailEndTime = '';
        }
    }

    //获取详情
    $scope.GetSensitiveMailInfo = function (id) {
        $scope.ChangeMailDBID = id;
        var paramObj = {
            "ID": id
        }

        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'SensitiveMail.ashx?op=GetSensitiveMailInfo&accessToken=' + storage.getItem("userAdminToken") + '&ID=' + id,
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            var ListData = response.data;
            if (ListData.result == "true") {
                $scope.ModifySensitiveMailID = id;
                for (var i = 0; i < ListData.data.Objects.length; i++) {
                    if (ListData.data.Objects[i].ObjectType == 1) {
                        ListData.data.Objects[i].TypeName = "OU";
                    } else {
                        ListData.data.Objects[i].TypeName = "用户";
                    }
                }
                $scope.SensitiveMailName = ListData.data.Name;
                $scope.ObjectLimits = ListData.data.Objects;
                $scope.Keywords = ListData.data.Keywords;
                $scope.SMailStartTime = ListData.data.StartTimeName.substr(0, 16);
                $scope.SMailEndTime = ListData.data.EndTimeName.substr(0, 16);
                $scope.SensitiveMailType = ListData.data.MailType;
            } else {
                $scope.showMsg('error', false, callbackObj.errMsg, 1);
                $scope.showModalFoot = true;
            }
        }, function errorCallback(e) {

        });
    }

    //选择对象范围
    $scope.gotoAddObjectLimit = function () {
        $("#SMail_Modal").modal("show");
        $scope.seachNode = '';
        $scope.nomalModal();
        $scope.GetCompanyTree();
    }

    $scope.AddObjectLimit = function () {
        $("#SMail_Modal").modal("hide");
        var nodes = CompanyTree.getCheckedNodes(true);
        for (var i = 0; i < nodes.length; i++) {
            if (nodes[i].type == 1 || nodes[i].type == 2) {
                var arrObj = { "ObjectID": nodes[i].id, "ObjectName": nodes[i].Name, "TypeName": nodes[i].type == 1 ? "OU" : "用户" };
                $scope.ObjectLimits.push(arrObj);
                if ($scope.ObjectLimits.length != 0) {
                    for (var j = 0; j < $scope.ObjectLimits.length; j++) {
                        for (var k = j + 1; k < $scope.ObjectLimits.length; k++) {
                            if ($scope.ObjectLimits[k].ObjectID == $scope.ObjectLimits[j].ObjectID) {
                                $scope.ObjectLimits.splice(k, 1);
                            }
                        }
                    }
                }
            }
        }
        console.log($scope.ObjectLimits);
    }

    //移除
    $scope.removeObjectLimit = function (tableid) {
        for (var i in $scope.ObjectLimits) {
            if ($scope.ObjectLimits[i].ObjectID == tableid) {
                $scope.ObjectLimits.splice(i, 1);
            }
        }
        console.log($scope.ObjectLimits);
    }

    //添加
    $scope.AddSensitiveMail = function () {
        if ($scope.SensitiveMailName == "") {
            ErrorHandling('false', 0, '请输入敏感邮件删除规则名称。');
            return;
        }
        if ($scope.ObjectLimits.length == 0) {
            ErrorHandling('false', 0, '请选择敏感邮件删除规则范围。');
            return;
        }
        if ($scope.SMailStartTime == "" || $scope.SMailEndTime == "") {
            ErrorHandling('false', 0, '请选择时间段。');
            return;
        }
        var StartTimeStamp = new Date(Date.parse($scope.SMailStartTime.replace(/-/g, "/"))).getTime();
        var EndTimeStamp = new Date(Date.parse($scope.SMailEndTime.replace(/-/g, "/"))).getTime();
        if (StartTimeStamp > EndTimeStamp) {
            ErrorHandling('false', 0, '结束时间不能小于开始时间。');
            return;
        }
        if ($scope.Keywords == "") {
            ErrorHandling('false', 0, '请输入关键字。');
            return;
        }
        var paramObj = {
            "Name": $scope.SensitiveMailName,
            "Objects": $scope.ObjectLimits,
            "Keywords": $scope.Keywords,
            "StartTime": $scope.SMailStartTime,
            "EndTime": $scope.SMailEndTime,
            "MailType": $scope.SensitiveMailType
        };
        angular.element(".preloader").css("display", "block");
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") +  'SensitiveMail.ashx?op=AddSensitiveMail&accessToken=' + storage.getItem("userAdminToken"),
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            angular.element(".preloader").css("display", "none");
            var callbackObj = response.data;
            ErrorHandling(callbackObj.result, callbackObj.errCode, callbackObj.errMsg);
            if (callbackObj.result == "true") {
                $scope.GetSensitiveMailList(1);
            } 
        }, function errorCallback(e) {

        });
    }

    //编辑
    $scope.ModifySensitiveMail = function () {
        if ($scope.SensitiveMailName == "") {
            ErrorHandling('false', 0, '请输入敏感邮件删除规则名称。');
            return;
        }
        if ($scope.ObjectLimits.length == 0) {
            ErrorHandling('false', 0, '请选择敏感邮件删除规则范围。');
            return;
        }
        if ($scope.SMailStartTime == "" || $scope.SMailEndTime == "") {
            ErrorHandling('false', 0, '请选择时间段。');
            return;
        }
        var StartTimeStamp = new Date(Date.parse($scope.SMailStartTime.replace(/-/g, "/"))).getTime();
        var EndTimeStamp = new Date(Date.parse($scope.SMailEndTime.replace(/-/g, "/"))).getTime();
        if (StartTimeStamp > EndTimeStamp) {
            ErrorHandling('false', 0, '结束时间不能小于开始时间。');
            return;
        }
        if ($scope.Keywords == "") {
            ErrorHandling('false', 0, '请输入关键字。');
            return;
        }
        var paramObj = {
            "ID": $scope.ModifySensitiveMailID,
            "Name": $scope.SensitiveMailName,
            "Objects": $scope.ObjectLimits,
            "Keywords": $scope.Keywords,
            "StartTime": $scope.SMailStartTime,
            "EndTime": $scope.SMailEndTime,
            "MailType": $scope.SensitiveMailType
        };
        angular.element(".preloader").css("display", "block");
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'SensitiveMail.ashx?op=ModifySensitiveMail&accessToken=' + storage.getItem("userAdminToken"),
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            angular.element(".preloader").css("display", "none");
            var callbackObj = response.data;
            ErrorHandling(callbackObj.result, callbackObj.errCode, callbackObj.errMsg);
            if (callbackObj.result == "true") {
                $scope.GetSensitiveMailList(1);
            } 
        }, function errorCallback(e) {

        });
    }

    //删除
    $scope.delSMailModal = function (id) {
        $("#delSMail_Modal").modal("show");
        $scope.nomalModal();
        $scope.delSMailID = id;
    }

    $scope.DelSMail = function () {
        var paramObj = {
            "ID": $scope.delSMailID
        };
        $scope.operateProgress();
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'SensitiveMail.ashx?op=DeleteSensitiveMail&accessToken=' + storage.getItem("userAdminToken"),
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            var callbackObj = response.data;
            if (callbackObj.result == "true") {
                $scope.showMsg('success', false, callbackObj.errMsg, 1);
                $scope.showModalFoot = false;
                $scope.GetSensitiveMailList(1);
            } else {
                $scope.showMsg('error', false, callbackObj.errMsg, 1);
                $scope.showModalFoot = true;
            }
        }, function errorCallback(e) {

        });
    }
    //立即执行
    $scope.gotoExecuteSensitiveMailModal = function (id) {
        $("#ExecuteSMail_Modal").modal("show");
        $scope.nomalModal();
        $scope.ExecuteSMailID = id;
    }

    $scope.ExecuteSensitiveMail = function () {
        var paramObj = {
            "ID": $scope.ExecuteSMailID
        };
        $scope.operateProgress();
        //angular.element(".preloader").css("display", "block");
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'SensitiveMail.ashx?op=ExecuteSensitiveMail&accessToken=' + storage.getItem("userAdminToken"),
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            //angular.element(".preloader").css("display", "none");
            var callbackObj = response.data;
            //ErrorHandling(callbackObj.result, callbackObj.errCode, callbackObj.errMsg);
            if (callbackObj.result == "true") {
                $scope.showMsg('success', false, callbackObj.errMsg, 1);
                $scope.showModalFoot = false;
                $scope.GetSensitiveMailList(1);
            } else {
                $scope.showMsg('error', false, callbackObj.errMsg, 1);
                $scope.showModalFoot = true;
            }
        }, function errorCallback(e) {

        });
    }
    //公司树
    $scope.GetCompanyTree = function () {
        function GetRootNode() {
            var nodeid = "00000000-0000-0000-0000-000000000000";
            $http({
                method: 'GET',
                url: storage.getItem("InterfaceUrl") + 'TreeNode.ashx?op=GetDefaultCompanyTree&accessToken=' + storage.getItem("userAdminToken") + '&NodeID=' + nodeid,
            }).then(function successCallback(response) {
                var data = response.data;
                if (data.result != "false") {
                    var obj = {
                        name: data[0].name,
                        Name: data[0].Name,
                        id: data[0].id,
                        pId: 0,
                        type: data[0].type,
                        Type: data[0].Type,
                        MemberCount: data[0].MemberCount,
                        isParent: true,
                        iconSkin: "org",
                        Path: data[0].Path
                    };

                    var setting = {
                        async: {
                            enable: true,
                            url: getUrl,
                            dataFilter: ajaxDataFilter
                        },
                        view: {
                            dblClickExpand: false,
                            showLine: true,
                            selectedMulti: false
                        },
                        data: {
                            key: {
                                name: "Name",//设置树显示的字段与接口里的字段对应关系
                                tId: "id",
                                children: "children",//子节点名称与接口字段对应关系，梯形数据结构需要
                            },
                            simpleData: {
                                enable: true,//禁用简单的json数据结构，即用梯形结构
                            }
                        },
                        check: {
                            enable: true,//开启树的checkbox，也可以设置为radio
                            autoCheckTrigger: false,
                            chkboxType: { "Y": "", "N": "" }
                        },
                        callback: {
                            onClick: CTOnClick, //点击节点时 回调,
                            onAsyncSuccess: zTreeOnAsyncSuccess,
                            onAsyncError: zTreeOnAsyncError
                        }
                    };
                    CompanyTree = $.fn.zTree.init($("#CompanyTree"), setting, obj);//初始化
                } else {
                    ErrorHandling(data.result, data.errCode, data.errMsg);
                }
            }, function errorCallback(e) {

            });
        }
        GetRootNode();

        function getUrl(treeId, treeNode) {
            TreesType = 0;
            var nodeid = "";
            if (treeNode == undefined || treeNode.type == "0") {
                nodeid = "00000000-0000-0000-0000-000000000000";
            } else {
                nodeid = treeNode.id;
            }
            var Url = storage.getItem("InterfaceUrl") + 'TreeNode.ashx?op=GetDefaultCompanyTree&accessToken=' + storage.getItem("userAdminToken") + '&NodeID=' + nodeid;
            return Url;
        }

        function ajaxDataFilter(treeId, parentNode, responseData) {
            if (responseData.result != 'false') {
                for (var i = 0; i < responseData.length; i++) {
                    if (responseData[i].type == "0") {
                        responseData[i].isParent = true;
                        responseData[i].iconSkin = "org";
                    } else if (responseData[i].type == "1") {
                        responseData[i].iconSkin = "depart";
                    } else if (responseData[i].type == "2") {
                        if (responseData[i].Status == true) {
                            responseData[i].iconSkin = "Nuser";
                        } else {
                            responseData[i].iconSkin = "Duser";
                        }
                    } else if (responseData[i].type == "3") {
                        responseData[i].iconSkin = "group";
                    }
                }
                return responseData;
            } else {
                ErrorHandling(responseData.result, responseData.errCode, responseData.errMsg);
            }
        }

        function zTreeOnAsyncSuccess(event, treeId, treeNode, msg) {

        }

        function zTreeOnAsyncError(event, treeId, treeNode, XMLHttpRequest, textStatus, errorThrown) {
            console.log(treeNode);
        };

        function CTOnClick(event, treeId, treeNode) {
            $scope.ObjectID = treeNode.id;
            treeSelectNodeID = treeNode.id;
            treeSelectNode = treeNode;
            CompanyTree.selectNode(treeNode);
            CompanyTree.expandNode(treeNode);
        }

        $scope.SearchCompanyTree = function () {
            if ($scope.seachNode == "" || $scope.seachNode == undefined || $scope.seachNode == null) {
                $scope.GetCompanyTree();
            } else {
                var searchValue = $.trim($scope.seachNode);
                var myUrl = storage.getItem("InterfaceUrl") + 'TreeNode.ashx?op=SearchCompanyTree&accessToken=' + storage.getItem("userAdminToken") + '&Searchstr=' + encodeURI(searchValue);
                $http({ method: 'GET', url: myUrl }).success(
                    function (response) {
                        var obj = response;
                        if (response.result != "false") {
                            for (var i = 0; i < obj.length; i++) {
                                obj[i].isParent = false;
                                if (obj[i].type == 0) {
                                    obj[i].iconSkin = "org";
                                    obj[i].level = '2';
                                }
                                if (obj[i].type == 1) {
                                    obj[i].iconSkin = "depart";
                                    obj[i].level = '2';
                                }
                                else if (obj[i].type == 2) {
                                    if (obj[i].Status == true) {
                                        obj[i].iconSkin = "Nuser";
                                    } else {
                                        obj[i].iconSkin = "Duser";
                                    }
                                    obj[i].level = '2';
                                }
                                else if (obj[i].type == 3) {
                                    obj[i].iconSkin = "group";
                                    obj[i].level = '2';
                                }
                            }
                            var setting = {
                                view: {
                                    dblClickExpand: false,
                                    showLine: true,
                                    selectedMulti: false
                                },
                                data: {
                                    key: {
                                        name: "Name",//设置树显示的字段与接口里的字段对应关系
                                        tId: "id",
                                        children: "children",//子节点名称与接口字段对应关系，梯形数据结构需要
                                    },
                                    simpleData: {
                                        enable: true,//禁用简单的json数据结构，即用梯形结构
                                    }
                                },
                                check: {
                                    enable: true,//开启树的checkbox，也可以设置为radio
                                    autoCheckTrigger: true,
                                    chkboxType: { "Y": "", "N": "" }
                                },
                                callback: {
                                    onClick: CTOnClick, //点击节点时 回调,
                                    onAsyncSuccess: zTreeOnAsyncSuccess,
                                    onAsyncError: zTreeOnAsyncError
                                }
                            };
                            CompanyTree = $.fn.zTree.init($("#CompanyTree"), setting, obj);//初始化
                        } else {

                        }

                    }).error(function (e) {

                    });
            }
        }
    }

    $scope.clearSearchInput = function () {
        $scope.seachNode = '';
        $scope.GetCompanyTree();
    }


    $scope.ExportToExcel = function (mailid) {
        $scope.ExportToExcel = storage.getItem("InterfaceUrl") + 'SensitiveMail.ashx?op=ExportToExcel&accessToken=' + storage.getItem("userAdminToken") + '&ID=' + mailid;
        angular.element("#" + mailid).attr("href", $scope.ExportToExcel);
    }

    //弹出框报错信息
    $scope.showMsg = function (opResult, checkTips, errorMsg, inputIndex) {
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
    }
    //模态框按钮执行中状态
    $scope.operateProgress = function () {
        $(".loading").css("visibility", "visible");
        angular.element(".modalBtn").text("执行中");
        $scope.showModalFoot = true;
        $scope.modalButton = true;
    }

    $scope.clearSearch = function () {
        $scope.Searchstr = '';
        $scope.StartTime = '';
        $scope.EndTime = '';
        $scope.GetSensitiveMailList(1);
    }

    $scope.changePageSize = function () {
        $scope.GetSensitiveMailList(1);
    }
});