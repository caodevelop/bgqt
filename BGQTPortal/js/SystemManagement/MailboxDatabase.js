angular.module('app').controller('MDCtrl', function (AuthVerify, $scope, $http, $state, $stateParams, $location, $window) {
    $scope.MailDBList = [];
    $scope.searchOU = '';
    $scope.currentPage = 1;
    $scope.totalPage = 0;
    $scope.pSize = 10;
    $scope.maxSize = 5;
    $scope.Lists = [];
    $scope.seachOrgNode = '';
    $scope.OuID = '';
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
  
    //获取 MailDB 列表
    $scope.GetMailDataBaseList = function (currentPage) {
        console.log(currentPage);
        $scope.ViewOrAdd = true;
        $scope.currentPage = currentPage;
        $scope.Lists = [];
        $http({
            method: 'GET',
            url: storage.getItem("InterfaceUrl") + 'MailDataBase.ashx?op=GetMailDataBaseList&accessToken=' + storage.getItem("userAdminToken") + '&PageSize=' + $scope.pSize + '&CurPage=' + currentPage + '&Searchstr=' + encodeURI($scope.searchOU),
        }).then(function successCallback(response) {
            var data = response.data;
            if (data.result == "true") {
                var ListData = data.data.Lists;
                $scope.bigTotalItems = data.data.RecordCount;
                $scope.totalPage = data.data.PageCount;
                for (n = 0; n < ListData.length; n++) {
                    $scope.arr = {
                        "ID": ListData[n].ID,
                        "OuName": ListData[n].OuName,
                        "MailboxDB": ListData[n].MailboxDB,
                        "OuID": ListData[n].OuID,
                        "OUdistinguishedName": ListData[n].OUdistinguishedName,
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
                            "OuName": "",
                            "MailboxDB": "",
                            "OuID": "",
                            "OUdistinguishedName": "",
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
        $scope.GetMailDataBaseList(currentPage);
    };

    //获取 MailDB 详情
    $scope.GetMailDataBaseInfo = function (id) {
        $scope.ViewOrAdd = false;
        $scope.ChangeMailDBID = id;
        var paramObj = {
            "ID": id
        }
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'MailDataBase.ashx?op=GetMailDataBaseInfo&accessToken=' + storage.getItem("userAdminToken") + '&ID='+id,
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            var ListData = response.data;
            if (ListData.result == "true") {
                $scope.ChangeMailDBID = id;
                $scope.OuID = ListData.data.OuID;
                $scope.MailboxDBValue = ListData.data.MailboxDBID;
                $scope.OuName = ListData.data.OuName;
            } else {
                ErrorHandling(data.result, data.errCode, data.errMsg);
            }
        }, function errorCallback(e) {

        });
    }
    //添加 Or 删除 MailDB
    $scope.gotoMailBoxDBModal = function (op, id) {
        $scope.ViewOrAdd = false;
        $scope.op = op;
        $scope.OuName = '';
        if (op == 'edit') {
            $scope.OpTitle = '添加';
            $scope.OuID = '';
            $scope.GetMailDataBaseInfo(id);
        } else {
            $scope.OpTitle = '编辑';
        }
        $scope.GetExchangeMailDBList(op);
    }
    $scope.gotoChooseOu = function () {
        $("#MailDB_Modal").modal("show");
        $scope.seachOrgNode = '';
        $scope.nomalModal();
        $scope.GetOrgTree();
    }

    // OU 树
    $scope.GetOrgTree = function () {
        function GetRootNode() {
            var obj = {
                name: "全局组织",
                Name: "全局组织",
                id: 0,
                pId: 0,
                type: 0,
                Type: 0,
                MemberCount: 0,
                isParent: true,
                iconSkin: "org"
            };

            var setting = {
                async: {
                    enable: true,
                    url: getOrgUrl,
                    dataFilter: ajaxDataOrgFilter
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
                    enable: false,//开启树的checkbox，也可以设置为radio
                    autoCheckTrigger: true,
                    chkboxType: { "Y": "", "N": "" }
                },
                callback: {
                    onClick: OrgOnClick, //点击节点时 回调,
                    onAsyncSuccess: OrgOnAsyncSuccess,
                    onAsyncError: OrgOnAsyncError
                }
            };
            OrgTree = $.fn.zTree.init($("#OrgTree"), setting, obj);
        }
        GetRootNode();

        function getOrgUrl(treeId, treeNode) {
            TreesType = 1;
            var nodeid = "";
            if (treeNode == undefined || treeNode.type == "0") {
                nodeid = "00000000-0000-0000-0000-000000000000";
            } else {
                nodeid = treeNode.id;
            }
            var Url = storage.getItem("InterfaceUrl") + 'TreeNode.ashx?op=GetMailBoxDataOrgTree&accessToken=' + storage.getItem("userAdminToken") + '&NodeID=' + nodeid;
            return Url;
        }

        function ajaxDataOrgFilter(treeId, parentNode, responseData) {
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

        function OrgOnAsyncSuccess(event, treeId, treeNode, msg) {
           
        }

        function OrgOnAsyncError(event, treeId, treeNode, XMLHttpRequest, textStatus, errorThrown) {
            console.log(treeNode);
        };

        function OrgOnClick(event, treeId, treeNode) {
            $(".ztree li a").css({ "padding-left": "4px", "margin-left": "-4px" });
            treeSelectNodeID = treeNode.id;
            treeSelectNode = treeNode;
            OrgTree.selectNode(treeNode);
            OrgTree.expandNode(treeNode);
            //$scope.OuID = treeNode.id;
        }

        $scope.SearchGroupTree = function () {
            var nodes = OrgTree.getSelectedNodes();
            if (nodes.length > 0) {
                OrgTree.cancelSelectedNode(nodes[0]);
                treeSelectNodeID = '';
                treeSelectNode = '';
            }
            if ($scope.seachOrgNode == "" || $scope.seachOrgNode == undefined || $scope.seachOrgNode == null) {
                $scope.GetOrgTree();
            } else {
                var searchValue = $.trim($scope.seachOrgNode);
                var myUrl = storage.getItem("InterfaceUrl") + 'TreeNode.ashx?op=SearchMailBoxDataOrgTree&accessToken=' + storage.getItem("userAdminToken") + '&Searchstr=' + encodeURI(searchValue);
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
                                    enable: false,//开启树的checkbox，也可以设置为radio
                                    autoCheckTrigger: true,
                                    chkboxType: { "Y": "", "N": "" }
                                },
                                callback: {
                                    onClick: OrgOnClick, //点击节点时 回调,
                                    onAsyncSuccess: OrgOnAsyncSuccess,
                                    onAsyncError: OrgOnAsyncError
                                }
                            };
                            OrgTree = $.fn.zTree.init($("#OrgTree"), setting, obj);
                        } else {

                        }

                    }).error(function (e) {

                    });
            }
        }
    }

    $scope.clearSearchInput = function () {
        $scope.seachOrgNode = '';
        $scope.GetOrgTree();
    }

    $scope.chooseOU = function () {
        //$scope.OuName = '';
        if (treeSelectNode != '') {
            if (treeSelectNode.type == 0) {
                $scope.showMsg('error', false, "请选择下级节点。", 1);
                return;
            } else {
                $scope.OuName = treeSelectNode.name;
                $scope.OuID = treeSelectNode.id;
            }
        }
        $("#MailDB_Modal").modal("hide");
    }

    //获取 MailBoxDB 列表
    $scope.GetExchangeMailDBList = function (op) {
        angular.element(".preloader").css("display", "block");
        $scope.MailDBList = [];
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'MailDataBase.ashx?op=GetExchangeMailDBList&accessToken=' + storage.getItem("userAdminToken"),
        }).then(function successCallback(response) {
            angular.element(".preloader").css("display", "none");
            var callbackObj = response.data;
            if (callbackObj.result == "true") {
                for (var i = 0; i < callbackObj.data.length; i++) {
                    var obj = {
                        "text": callbackObj.data[i].MailboxDB + "(" + callbackObj.data[i].MailboxServer+")", "value": callbackObj.data[i]
                    }
                    $scope.MailDBList.push(obj);
                }
                if (op == 'edit') {
                    for (var j in $scope.MailDBList) {
                        if ($scope.MailDBList[j].value.MailboxDBID == $scope.MailboxDBValue) {
                            $scope.MailboxDB = $scope.MailDBList[j];
                        }
                    }
                } else {
                    $scope.MailboxDB = $scope.MailDBList[0];
                }
            } else {
                $scope.showMsg('error', false, callbackObj.errMsg, 1);
                $scope.showModalFoot = true;
            }
        }, function errorCallback(e) {

        });
    }
    //添加 MailDB
    $scope.AddMailDB = function () {
        if ($scope.OuID == "") {
            ErrorHandling('false', '', '请选择一个 OU 对象。');
            return;
        }
        if ($scope.MailboxDB == "") {
            ErrorHandling('false', '', '请选择 MailboxDB。');
            return;
        }
        var paramObj = {
            "OuID": treeSelectNodeID,
            "MailboxDB": $scope.MailboxDB.value.MailboxDB,
            "MailboxServer": $scope.MailboxDB.value.MailboxServer,
            "MailboxDBID": $scope.MailboxDB.value.MailboxDBID
        };
        angular.element(".preloader").css("display", "block");
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'MailDataBase.ashx?op=AddMailDataBase&accessToken=' + storage.getItem("userAdminToken"),
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            angular.element(".preloader").css("display", "none");
            var callbackObj = response.data;
            ErrorHandling(callbackObj.result, callbackObj.errCode, callbackObj.errMsg);
            if (callbackObj.result == "true") {
                $scope.GetMailDataBaseList(1);
            } 
        }, function errorCallback(e) {

        });
    }
    //编辑 MailDB
    $scope.ChangeMailDB = function () {
        if ($scope.OuID == "") {
            ErrorHandling('false', '', '请选择一个 OU 对象。');
            return;
        }
        var paramObj = {
            "ID": $scope.ChangeMailDBID,
            "OuID": $scope.OuID,
            "MailboxDB": $scope.MailboxDB.value.MailboxDB,
            "MailboxServer": $scope.MailboxDB.value.MailboxServer,
            "MailboxDBID": $scope.MailboxDB.value.MailboxDBID
        };
        angular.element(".preloader").css("display", "block");
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'MailDataBase.ashx?op=ChangeMailDataBase&accessToken=' + storage.getItem("userAdminToken"),
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            angular.element(".preloader").css("display", "none");
            var callbackObj = response.data;
            ErrorHandling(callbackObj.result, callbackObj.errCode, callbackObj.errMsg);
            if (callbackObj.result == "true") {
                $scope.GetMailDataBaseList(1);
            } 
        }, function errorCallback(e) {

        });
    }
    //删除 MailDB
    $scope.gotoDelMailBoxDB = function (id) {
        $("#delMailDB_Modal").modal("show");
        $scope.nomalModal();
        $scope.DelMailDBID = id;
    }
    $scope.DelMailDB = function () {
        var paramObj = {
            "ID": $scope.DelMailDBID
        };
        $scope.operateProgress();
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'MailDataBase.ashx?op=DeleteMailDataBase&accessToken=' + storage.getItem("userAdminToken"),
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            var callbackObj = response.data;
            if (callbackObj.result == "true") {
                $scope.showMsg('success', false, callbackObj.errMsg, 1);
                $scope.showModalFoot = false;
                $scope.GetMailDataBaseList(1);
            } else {
                $scope.showMsg('error', false, callbackObj.errMsg, 1);
                $scope.showModalFoot = true;
            }
        }, function errorCallback(e) {

        });
    }

    $scope.changePageSize = function () {
        $scope.GetMailDataBaseList(1);
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
        $scope.searchOU = '';
        $scope.GetMailDataBaseList(1);
    }
});