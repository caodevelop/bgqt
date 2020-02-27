angular.module('app').controller('MACtrl', function (AuthVerify, $scope, $http, $state, $stateParams, $location, $window) {
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
    $scope.ViewOrAdd = true;
    $scope.Audits = [];
    $scope.ViewAudits = [];
    $scope.ExchangeGroupLists = [];
    $scope.seachNode = '';
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
  
    $scope.GetMailAuditList = function (currentPage) {
        angular.element(".preloader").css("display", "block");
        $scope.currentPage = currentPage;
        $scope.Lists = [];
        $scope.ViewOrAdd = true;
        //angular.element(".panel-heading").css("border-bottom", "none");
        $http({
            method: 'GET',
            url: storage.getItem("InterfaceUrl") + 'MailAudit.ashx?op=GetMailAuditList&accessToken=' + storage.getItem("userAdminToken") + '&PageSize=' + $scope.pSize + '&CurPage=' + $scope.currentPage + '&Searchstr=' + encodeURI($scope.Searchstr) + '&StartTime=' + $scope.StartTime + '&EndTime=' + $scope.EndTime,
        }).then(function successCallback(response) {
            angular.element(".preloader").css("display", "none");
            var data = response.data;
            if (data.result == "true") {
                var ListData = data.data.Lists;
                $scope.bigTotalItems = data.data.RecordCount;
                $scope.totalPage = data.data.PageCount;
                for (n = 0; n < ListData.length; n++) {
                    $scope.arr = {
                        "ID": ListData[n].ID,
                        "AuditUsers": ListData[n].AuditUsers,
                        "GroupObj": ListData[n].Group.DisplayName + "(" + ListData[n].Group.Account + ")",
                        "CreateTimeName": ListData[n].CreateTimeName,
                        "trueData": 1
                    };
                    $scope.Lists.push($scope.arr);
                }
                if (ListData.length < $scope.pSize) {
                    for (i = 0; i < $scope.pSize - ListData.length; i++) {
                        var nulltr = {
                            "$$hashKey": i,
                            "ID": "",
                            "AuditUsers": "",
                            "GroupObj": "",
                            "CreateTimeName": "",
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
        $scope.GetMailAuditList(currentPage);
    };

    $scope.gotoMailAuditModal = function (op, id) {
        angular.element(".panel-heading").css("border-bottom", "1px solid #d8d8d8");
        $scope.ViewOrAdd = false;
        $scope.op = op;
        if (op == 'add') {
            $scope.Audits = [];
            $scope.OpTitle = '添加审批';
            $scope.GroupName = '';
            $scope.GetExchangeGroupList('add');
        } else {
            $scope.OpTitle = '编辑审批';
            $scope.GetMailAuditInfo(id);
        }
    }
    $scope.GetExchangeGroupList = function (op) {
        $scope.ExchangeGroupLists = [];
        $scope.ExchangeGroupLists.push({ "text": "请选择", "value": "0" });
        $http({
            method: 'GET',
            url: storage.getItem("InterfaceUrl") + 'TreeNode.ashx?op=GetExchangeGroupList&accessToken=' + storage.getItem("userAdminToken"),
        }).then(function successCallback(response) {
            var data = response.data;
            if (data.result == "true") {
                var ListData = data.data;
                for (n = 0; n < ListData.length; n++) {
                    $scope.arr = {
                        "text": ListData[n].DisplayName + "(" + ListData[n].Account + ")",
                        "value": ListData[n].GroupID
                    };
                    $scope.ExchangeGroupLists.push($scope.arr);
                }
                if (op == 'edit') {
                    for (var j in $scope.ExchangeGroupLists) {
                        if ($scope.ExchangeGroupLists[j].value == $scope.GroupID) {
                            $scope.GroupID = $scope.ExchangeGroupLists[j].value;
                        }
                    }
                } else {
                    $scope.GroupID = $scope.ExchangeGroupLists[0].value;
                }
            } else {
                ErrorHandling(data.result, data.errCode, data.errMsg);
            }
        }, function errorCallback(e) {

        });
    }

    $scope.gotoAddReviewers = function () {
        $("#chooseUser_Modal").modal("show");
        $scope.nomalModal();
        $scope.seachNode = '';
        $scope.showModalFoot = true;
        $scope.GetCompanyTree();
    }

    //添加
    $scope.AddMailAudit = function () {
        if ($scope.GroupID == "0") {
            ErrorHandling('false', '', '请选择邮件审批对象。');
            return;
        }
        if ($scope.Audits.length == 0) {
            ErrorHandling('false', '', '请选择审批人。');
            return;
        }
        var paramObj = {
            "Group": { "GroupID": $scope.GroupID },
            "Audits": $scope.Audits
        };
        angular.element(".preloader").css("display", "block");
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'MailAudit.ashx?op=AddMailAudit&accessToken=' + storage.getItem("userAdminToken"),
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            angular.element(".preloader").css("display", "none");
            var callbackObj = response.data;
            ErrorHandling(callbackObj.result, callbackObj.errCode, callbackObj.errMsg);
            if (callbackObj.result == "true") {
                $scope.GetMailAuditList($scope.currentPage);
            } 
        }, function errorCallback(e) {

        });
    }

    //获取详情
    $scope.GetMailAuditInfo = function (id) {
        $scope.ChangeMailDBID = id;
        var paramObj = {
            "ID": id
        }

        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'MailAudit.ashx?op=GetMailAuditInfo&accessToken=' + storage.getItem("userAdminToken") + '&ID=' + id,
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            var ListData = response.data;
            if (ListData.result == "true") {
                $scope.ModifyMailAuditID = id;
                $scope.GroupID = ListData.data.Group.GroupID;
                $scope.GroupName = ListData.data.Group.DisplayName + "(" + ListData.data.Group.Account + ")";
                $scope.Audits = ListData.data.Audits;
                $scope.GetExchangeGroupList('edit');
            } else {
                ErrorHandling(data.result, data.errCode, data.errMsg);
            }
        }, function errorCallback(e) {

        });
    }

    //获取详情1
    $scope.GetGetMailAuditInfoByExchange = function (groupid) {
        var paramObj = {
            "Group": { "GroupID": groupid }
        }
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'MailAudit.ashx?op=GetMailAuditInfoByExchange&accessToken=' + storage.getItem("userAdminToken"),
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            var ListData = response.data;
            if (ListData.result == "true") {
                $scope.Audits = ListData.data.Audits;
            } else {
                ErrorHandling(data.result, data.errCode, data.errMsg);
            }
        }, function errorCallback(e) {

        });
    }

    //添加审批人
    $scope.AddReviewers = function () {
        $("#chooseUser_Modal").modal("hide");
        var nodes = OUAndUserTree.getCheckedNodes(true);
        for (var i = 0; i < nodes.length; i++) {
            if (nodes[i].Type == 2) {
                var arrObj = { "UserID": nodes[i].id, "DisplayName": nodes[i].Name, "UserAccount": nodes[i].UserAccount };
                $scope.Audits.push(arrObj);
                if ($scope.Audits.length != 0) {
                    for (var j = 0; j < $scope.Audits.length; j++) {
                        for (var k = j + 1; k < $scope.Audits.length; k++) {
                            if ($scope.Audits[k].UserID == $scope.Audits[j].UserID) {
                                $scope.Audits.splice(k, 1);
                            }
                        }
                    }
                }               
            }
        }
        console.log($scope.Audits);
    }
    //移除审批人
    $scope.removeThis = function (tableid) {
        for (var i in $scope.Audits) {
            if ($scope.Audits[i].UserID == tableid) {
                $scope.Audits.splice(i, 1);
            }
        }
        console.log($scope.Audits);
    }
    //编辑
    $scope.ModifyMailAudit = function () {
        if ($scope.GroupID == "0") {
            ErrorHandling('false', '', '请选择邮件审批对象。');
            return;
        }
        if ($scope.Audits.length == 0) {
            ErrorHandling('false', '', '请选择审批人。');
            return;
        }
        var paramObj = {
            "ID": $scope.ModifyMailAuditID,
            "Group": { "GroupID": $scope.GroupID },
            "Audits": $scope.Audits
        };
        console.log($scope.Audits);
        angular.element(".preloader").css("display", "block");
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'MailAudit.ashx?op=ModifyMailAudit&accessToken=' + storage.getItem("userAdminToken"),
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            angular.element(".preloader").css("display", "none");
            var callbackObj = response.data;
            ErrorHandling(callbackObj.result, callbackObj.errCode, callbackObj.errMsg);
            if (callbackObj.result == "true") {
                $scope.GetMailAuditList($scope.currentPage);
            } else {
                $scope.showModalFoot = true;
            }
        }, function errorCallback(e) {

        });
    }
    //删除
    $scope.delMailAuditModal = function (id) {
        $("#delMailAudit_Modal").modal("show");
        $scope.nomalModal();
        $scope.delMailAuditID = id;
    }
    $scope.DelMailAudit = function () {
        var paramObj = {
            "ID": $scope.delMailAuditID
        };
        $scope.operateProgress();
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'MailAudit.ashx?op=DeleteMailAudit&accessToken=' + storage.getItem("userAdminToken"),
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            var callbackObj = response.data;
            if (callbackObj.result == "true") {
                $scope.showMsg('success', false, callbackObj.errMsg, 1);
                $scope.showModalFoot = false;
                $scope.GetMailAuditList(1);
            } else {
                $scope.showMsg('error', false, callbackObj.errMsg, 1);
                $scope.showModalFoot = true;
            }
        }, function errorCallback(e) {

        });
    }
    // Hab树
    $scope.GetHabGroupTree = function () {
        TreesType = 0;
        function GetRootNode() {
            var nodeid = "00000000-0000-0000-0000-000000000000";
            $http({
                method: 'GET',
                url: storage.getItem("InterfaceUrl") + 'TreeNode.ashx?op=GetHabGroupTree&accessToken=' + storage.getItem("userAdminToken") + '&NodeID=' + nodeid,
            }).then(function successCallback(response) {
                var data = response.data;
                var obj = {
                    name: data[0].name,
                    Name: data[0].Name,
                    id: data[0].id,
                    pId: 0,
                    type: data[0].type,
                    Type: data[0].Type,
                    MemberCount: data[0].MemberCount,
                    UserAccount: data[0].UserAccount,
                    isParent: true,
                    iconSkin: "org"
                };

                var setting = {
                    async: {
                        enable: true,
                        url: getHabGroupTreeUrl,
                        dataFilter: ajaxHabGroupTreeDataFilter
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
                        autoCheckTrigger: false,
                        chkboxType: { "Y": "", "N": "" }
                    },
                    callback: {
                        onClick: HabGroupTreeOnClick,
                        onAsyncSuccess: HabGroupTreeOnAsyncSuccess,
                        onAsyncError: HabGroupTreeOnAsyncError
                    }
                };
                HabGroupTree = $.fn.zTree.init($("#HabGroupTree"), setting, obj);//初始化
            }, function errorCallback(e) {

            });
        }
        GetRootNode();
        function getHabGroupTreeUrl(treeId, treeNode) {
            TreesType = 0;
            var nodeid = "";
            if (treeNode.type == "0" || treeNode == "undefined") {
                nodeid = "00000000-0000-0000-0000-000000000000";
            } else {
                nodeid = treeNode.id;
            }
            var Url = storage.getItem("InterfaceUrl") + 'TreeNode.ashx?op=GetHabGroupTree&accessToken=' + storage.getItem("userAdminToken") + '&NodeID=' + nodeid;
            return Url;
        }

        function ajaxHabGroupTreeDataFilter(treeId, parentNode, responseData) {
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
                    } else if (responseData[i].type == "4") {
                        responseData[i].iconSkin = "habgroup";
                    }
                }
                return responseData;
            } else {
                ErrorHandling(responseData.result, responseData.errCode, responseData.errMsg);
            }
        }

        function HabGroupTreeOnAsyncSuccess(event, treeId, treeNode, msg) {
            
        }

        function HabGroupTreeOnAsyncError(event, treeId, treeNode, XMLHttpRequest, textStatus, errorThrown) {
            alert(XMLHttpRequest);
        };

        function HabGroupTreeOnClick(event, treeId, treeNode) {
            $(".ztree li a").css({ "padding-left": "4px", "margin-left": "-4px" });
            HabGroupTree.selectNode(treeNode);
            HabGroupTree.expandNode(treeNode);
        }


        $scope.SearchHabGroupTree = function () {
            var nodes = HabGroupTree.getSelectedNodes();
            if (nodes.length > 0) {
                HabGroupTree.cancelSelectedNode(nodes[0]);
            }
            if ($scope.seachHabGroupNode == "" || $scope.seachHabGroupNode == undefined || $scope.seachHabGroupNode == null) {
                $scope.GetHabGroupTree();
            } else {
                var searchValue = $.trim($scope.seachHabGroupNode);
                var myUrl = storage.getItem("InterfaceUrl") + 'TreeNode.ashx?op=SearchExchangeGroupList&accessToken=' + storage.getItem("userAdminToken") + '&Searchstr=' + encodeURI(searchValue);
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
                                else if (obj[i].type == 4) {
                                    obj[i].iconSkin = "habgroup";
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
                                    onClick: HabGroupTreeOnClick, //点击节点时 回调,
                                    onAsyncSuccess: HabGroupTreeOnAsyncSuccess,
                                    onAsyncError: HabGroupTreeOnAsyncError
                                }
                            };
                            HabGroupTree = $.fn.zTree.init($("#HabGroupTree"), setting, obj);//初始化
                        } else {

                        }

                    }).error(function (e) {

                    });
            }
        }
    }

    $scope.clearHabGroupInput = function () {
        $scope.seachHabGroupNode = '';
        $scope.GetHabGroupTree();
    }

    $scope.gotoChooseObject = function () {
        $("#chooseObject_Modal").modal("show");
        $scope.seachHabGroupNode = '';
        $scope.nomalModal();
        $scope.GetHabGroupTree();
    }

    $scope.chooseObject = function () {
        var nodes = HabGroupTree.getSelectedNodes()[0];
        if (nodes != '' && nodes != undefined) {
            $scope.GroupName = nodes.name + "(" + nodes.UserAccount + ")";
            $scope.GroupID = nodes.id;
            $scope.GetGetMailAuditInfoByExchange($scope.GroupID);
            $("#chooseObject_Modal").modal("hide");
        } else {
            $scope.GroupName = "";
            $scope.GroupID = "";
        }
    }

    // 树
    $scope.GetCompanyTree = function () {
        function GetRootNode() {
            var nodeid = "00000000-0000-0000-0000-000000000000";
            $http({
                method: 'GET',
                url: storage.getItem("InterfaceUrl") + 'TreeNode.ashx?op=GetCompanyTree&accessToken=' + storage.getItem("userAdminToken") + '&NodeID=' + nodeid,
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
                        iconSkin: "org"
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
                    OUAndUserTree = $.fn.zTree.init($("#OUAndUserTree"), setting, obj);//初始化
                } else {
                    ErrorHandling(data.result, data.errCode, data.errMsg);
                }
            }, function errorCallback(e) {

            });
        }
        GetRootNode();
        
        function getUrl(treeId, treeNode) {
            TreesType = 1;
            var nodeid = "";
            if (treeNode.type == "0" || treeNode == "undefined") {
                nodeid = "00000000-0000-0000-0000-000000000000";
            } else {
                nodeid = treeNode.id;
            }
            var Url = storage.getItem("InterfaceUrl") + 'TreeNode.ashx?op=GetCompanyTree&accessToken=' + storage.getItem("userAdminToken") + '&NodeID=' + nodeid;
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
            alert(XMLHttpRequest);
        };

        function CTOnClick(event, treeId, treeNode) {
            $(".ztree li a").css({ "padding-left": "4px", "margin-left": "-4px" });
            treeSelectNodeID = treeNode.id;
            treeSelectNode = treeNode;
            OUAndUserTree.selectNode(treeNode);
            OUAndUserTree.expandNode(treeNode);
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
                            OUAndUserTree = $.fn.zTree.init($("#OUAndUserTree"), setting, obj);//初始化
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

    $scope.changePageSize = function () {
        $scope.GetMailAuditList(1);
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
        $scope.GetMailAuditList(1);
    }
});