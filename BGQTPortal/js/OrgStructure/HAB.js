angular.module('app').controller('HABCtrl', function (AuthVerify, $scope, $http, $state, $stateParams, $location, $window) {
    var BatchSetupHabIndex = [];
    $(".last-col").hide();
    $("#HelpInstruction").show();
    $scope.Lists = [];
    $scope.HabGroups = [];
    $scope.HabGroupMembers = [];
    $scope.seachNode = '';
    $scope.seachHabGroupNode = '';
    $scope.ExecuteFun = '';
    $scope.ExecuteObj = '';
    $scope.orgCannotEdit = false;
    $scope.seachHabGroupNode = '';
    if ($window.innerWidth >= 1024) {
        $(".content-left").css("height", $window.innerHeight - 84);
        $("#HabGroupTree").css("height", (window.innerHeight - 168) + "px");
        $(".last-col").css({ "width": (window.innerWidth - 580) + "px", "height": (window.innerHeight - 130) + "px" });
    } else {
        $(".content-left").css("height", $window.innerHeight - 4);
        $("#HabGroupTree").css("height", (window.innerHeight - 114) + "px");
        $(".last-col").css({ "width": (window.innerWidth - 580) + "px", "height": (window.innerHeight - 76) + "px" });
    }
    window.onresize = function () {
        $scope.$apply(function () {
            if ($window.innerWidth >= 1024) {
                $(".content-left").css("height", $window.innerHeight - 84);
                $("#HabGroupTree").css("height", (window.innerHeight - 168) + "px");
                $(".last-col").css({ "width": (window.innerWidth - 580) + "px", "height": (window.innerHeight - 130) + "px" });
            } else {
                $(".content-left").css("height", $window.innerHeight - 4);
                $("#HabGroupTree").css("height", (window.innerHeight - 114) + "px");
                $(".last-col").css({ "width": (window.innerWidth - 580) + "px", "height": (window.innerHeight - 76) + "px" });
            }
        })
    };
   
    // Hab树（页面）
    $scope.GetHabGroupTree = function () {
        TreesType = 0;
        function GetRootNode() {
            var nodeid = "00000000-0000-0000-0000-000000000000";
            $http({
                method: 'GET',
                url: storage.getItem("InterfaceUrl") + 'TreeNode.ashx?op=GetHabGroupTree&accessToken=' + storage.getItem("userAdminToken") + '&NodeID=' + nodeid,
            }).then(function successCallback(response) {
                var data = response.data;
                if (data.result != 'false') {
                    var obj = {
                        name: data[0].name,
                        Name: data[0].Name,
                        id: data[0].id,
                        pId: 0,
                        type: data[0].type,
                        Type: data[0].Type,
                        MemberCount: data[0].MemberCount,
                        isParent: true,
                        isRoot: data[0].isRoot,
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
                            onRightClick: HabGroupTreeOnRightClick,
                            onAsyncSuccess: HabGroupTreeOnAsyncSuccess,
                            onAsyncError: HabGroupTreeOnAsyncError
                        }
                    };
                    HabGroupTree = $.fn.zTree.init($("#HabGroupTree"), setting, obj);//初始化
                } else {
                    ErrorHandling(data.result, data.errCode, data.errMsg);
                }
               
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
            //HabGroupTree.selectNode(treeNode);
            //treeSelectNodeID = treeNode.id;
            //treeSelectNode = treeNode;
            //$scope.gotoHabGroupInfoPanel(treeNode);
        }

        function HabGroupTreeOnAsyncError(event, treeId, treeNode, XMLHttpRequest, textStatus, errorThrown) {
            alert(XMLHttpRequest);
        };

        function HabGroupTreeOnClick(event, treeId, treeNode) {
            $(".ztree li a").css({ "padding-left": "4px", "margin-left": "-4px" });
            treeSelectNodeID = treeNode.id;
            treeSelectNode = treeNode;
            HabGroupTree.selectNode(treeNode);
            HabGroupTree.expandNode(treeNode);
            if (treeNode.level == 0) {
                $scope.orgCannotEdit = true;
            } else {
                $scope.orgCannotEdit = false;
            }
            $scope.gotoHabGroupInfoPanel(treeNode);
        }

        function HabGroupTreeOnRightClick(event, treeId, treeNode) {
            HideRMenu();
            $("#rMenu ul").show();
            if (!treeNode == "") {
                if (storage.getItem("SyncHab") == 'true') {
                    if (treeNode.type == 4) {
                        if (treeNode.isRoot == true) {//根节点
                            $("#r_hab").show();
                            $("#r_habgroupuser").show();
                            $("#r_adduser").show();
                        } else {
                            $("#r_ouhab").show();
                            $("#r_adduser").show();
                        }
                    }
                }
                HabGroupTree.selectNode(treeNode);
                treeSelectNodeID = treeNode.id;
                treeSelectNode = treeNode;
                $scope.gotoHabGroupInfoPanel(treeNode);
                $("#rMenu").css({ "top": event.clientY + "px", "left": event.clientX + "px", "visibility": "visible" });
                $("body").bind("mousedown", onBodyMouseDown);
                //document.getElementById("HabGroupTree").scrollTop = storage.getItem("scrollTop");
            }
            else {
                //alert("请选择节点！");
            }
        }

        $scope.SearchHabGroupTree = function () {
            if ($scope.seachNode == "" || $scope.seachNode == undefined || $scope.seachNode == null) {
                $scope.GetHabGroupTree();
            } else {
                var searchValue = $.trim($scope.seachNode);
                var myUrl = storage.getItem("InterfaceUrl") + 'TreeNode.ashx?op=SearchHabGroupTree&accessToken=' + storage.getItem("userAdminToken") + '&Searchstr=' + encodeURI(searchValue);
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
                                } else if (obj[i].type == 4) {
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
                                    onRightClick: HabGroupTreeOnRightClick,
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

    $scope.clearSearchInput = function () {
        $scope.seachNode = '';
        $scope.GetHabGroupTree();
    }

    $scope.gotoHabGroupInfoPanel = function (node) {
        $("#chkall").prop("checked", false);
        $scope.Lists = [];
        $(".last-col").hide();
        $("#HABList").show();
        $http({
            method: 'get',
            url: storage.getItem("InterfaceUrl") + 'HabGroup.ashx?op=GetHabGroupInfo&accessToken=' + storage.getItem("userAdminToken") + '&PageSize=10000&CurPage=1&GroupID=' + node.id,
        }).then(function successCallback(response) {
            var callbackObj = response.data;
            if (callbackObj.result == "true") {
                if (node.type == 4 && callbackObj.data.length != 0) {
                    $scope.changeMembersGroupID = node.id;
                    for (var i = 0; i < callbackObj.data.Lists.length; i++) {
                        if (callbackObj.data.Lists[i].Type == 2) {
                            callbackObj.data.Lists[i].TypeName = '用户';
                        } else if (callbackObj.data.Lists[i].Type == 3) {
                            callbackObj.data.Lists[i].TypeName = '通讯组';
                        } else if (callbackObj.data.Lists[i].Type == 4) {
                            callbackObj.data.Lists[i].TypeName = 'HAB 组';
                        }
                    }
                    $scope.Lists = callbackObj.data.Lists;
                } else {
                    var arrObj = { "ID": node.id, "DisplayName": node.Name, "Account": node.UserAccount, "Index": node.Index };
                    $scope.Lists.push(arrObj);
                }
            }
        }, function errorCallback(e) {

        });
    }

    //设置单个通讯组或者用户编号
    $scope.SingleSetup = function ($event, groupid) {
        var paramObj = [{ "ID": groupid, "Index": $event.target.value }];
        $scope.SetHabGroupIndex(paramObj);
    }

    //批量设置通讯组或者用户编号
    $scope.BatchSetup = function () {
        BatchSetupHabIndex = [];
        $("#HabTable input[type=checkbox]:checked").each(function (j) {
            if (j >= 0 && $(this)[0].id != "chkall") {
                BatchSetupHabIndex.push({ "ID": $(this).attr("GroupID"), "Index": 1 });
            }
        })
        console.log(BatchSetupHabIndex);
        $scope.SetHabGroupIndex(BatchSetupHabIndex);
    }

    //设置编号
    $scope.SetHabGroupIndex = function (paramObj) {
        if (paramObj.length == 0) {
            ErrorHandling('false', 0, '请选择你要恢复编号的对象。');
            return;
        }
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'HabGroup.ashx?op=SetHabGroupIndex&accessToken=' + storage.getItem("userAdminToken"),
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            var callbackObj = response.data;
            ErrorHandling(callbackObj.result, callbackObj.errCode, callbackObj.errMsg);
            if (callbackObj.result == "true") {
                var node = HabGroupTree.getNodeByParam("id", treeSelectNodeID, null);
                //HabGroupTree.reAsyncChildNodes(node, "refresh");
                $scope.gotoHabGroupInfoPanel(node);
                //$scope.GetHabGroupTree();
            }
        }, function errorCallback(e) {

        });
    }

    $scope.Execute = function () {
        if ($scope.ExecuteFun == 'SyncAllHabData') {
            $scope.SyncAllHabData($scope.ExecuteObj);
        } else if ($scope.ExecuteFun == 'SyncHabMembers') {
            $scope.SyncHabMembers($scope.ExecuteObj);
        } else if ($scope.ExecuteFun == 'SyncAppointHabData') {
            $scope.SyncAppointHabData($scope.ExecuteObj);
        }
    }

    $scope.SyncAllHabDataPanel = function (paramObj) {
        $("#Reconfirm_Modal").modal("show");
        $scope.nomalModal();
        $scope.ExecuteFun = 'SyncAllHabData';
        $scope.ExecuteObj = paramObj;
    }

    //全量同步Hab通讯组和成员
    $scope.SyncAllHabData = function (paramObj) {
        HideRMenu();
        $scope.operateProgress();
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'HabGroup.ashx?op=SyncAllHabData&accessToken=' + storage.getItem("userAdminToken"),
        }).then(function successCallback(response) {
            var callbackObj = response.data;
            if (callbackObj.result == "true") {
                $scope.showMsg('success', false, callbackObj.errMsg, 1);
                $scope.GetHabGroupTree();
            } else {
                $scope.showMsg('error', false, callbackObj.errMsg, 1);
            }
        }, function errorCallback(e) {

        });
    }

    $scope.SyncHabMembersPanel = function (paramObj) {
        $("#Reconfirm_Modal").modal("show");
        $scope.nomalModal();
        $scope.ExecuteFun = 'SyncHabMembers';
        $scope.ExecuteObj = paramObj;
    }

    //增量同步 HAB 成员
    $scope.SyncHabMembers = function (paramObj) {
        HideRMenu();
        $scope.operateProgress();
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'HabGroup.ashx?op=SyncHabMembers&accessToken=' + storage.getItem("userAdminToken"),
            //data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            var callbackObj = response.data;
            if (callbackObj.result == "true") {
                $scope.showMsg('success', false, callbackObj.errMsg, 1);
                $scope.GetHabGroupTree();
            } else {
                $scope.showMsg('error', false, callbackObj.errMsg, 1);
            }
        }, function errorCallback(e) {

        });
    }

    $scope.SyncAppointHabDataPanel = function (paramObj) {
        $("#Reconfirm_Modal").modal("show");
        $scope.nomalModal();
        $scope.ExecuteFun = 'SyncAppointHabData';
        $scope.ExecuteObj = paramObj;
    }

    //同步指定 OU Hab
    $scope.SyncAppointHabData = function (paramObj) {
        HideRMenu();
        var paramObj = { "GroupID": treeSelectNodeID };
        $scope.operateProgress();
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'HabGroup.ashx?op=SyncAppointHabData&accessToken=' + storage.getItem("userAdminToken"),
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            var callbackObj = response.data;
            if (callbackObj.result == "true") {
                $scope.showMsg('success', false, callbackObj.errMsg, 1);
                $scope.GetHabGroupTree();
            } else {
                $scope.showMsg('error', false, callbackObj.errMsg, 1);
            }
        }, function errorCallback(e) {

        });
    }

    $scope.EditMemberPanel = function () {
        HideRMenu();
        $scope.seachHabGroupNode = '';
        $("#EditMembers_Modal").modal("show");
        $scope.nomalModal();
        $scope.GetHabGroupTreeModal();
        $scope.HabGroupMembers = [];
        $http({
            method: 'get',
            url: storage.getItem("InterfaceUrl") + 'HabGroup.ashx?op=GetHabGroupInfo&accessToken=' + storage.getItem("userAdminToken") + '&PageSize=10000&CurPage=1&GroupID=' + treeSelectNode.id,
        }).then(function successCallback(response) {
            var callbackObj = response.data;
            if (callbackObj.result == "true") {
                if (treeSelectNode.type == 4 && callbackObj.data.length != 0) {
                    for (var i = 0; i < callbackObj.data.Lists.length; i++) {
                        if (callbackObj.data.Lists[i].Type == 2) {
                            callbackObj.data.Lists[i].TypeName = '用户';
                        } else if (callbackObj.data.Lists[i].Type == 3) {
                            callbackObj.data.Lists[i].TypeName = '通讯组';
                        } else if (callbackObj.data.Lists[i].Type == 4) {
                            callbackObj.data.Lists[i].TypeName = 'HAB 组';
                        }
                    }
                    $scope.HabGroupMembers = callbackObj.data.Lists;
                } else {
                    var arrObj = { "ID": treeSelectNode.id, "DisplayName": treeSelectNode.Name, "Account": treeSelectNode.UserAccount, "Index": treeSelectNode.Index, "TypeName": treeSelectNode.Type==2?"用户":"通讯组" };
                    $scope.HabGroupMembers.push(arrObj);
                }
            }
        }, function errorCallback(e) {

        });
    }

    // 编辑成员(树)
    $scope.GetHabGroupTreeModal = function () {
        TreesType = 0;
        function GetRootNode() {
            var nodeid = "00000000-0000-0000-0000-000000000000";
            $http({
                method: 'GET',
                url: storage.getItem("InterfaceUrl") + 'TreeNode.ashx?op=GetDefaultCompanyTree&accessToken=' + storage.getItem("userAdminToken") + '&NodeID=' + nodeid,
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
                    isParent: true,
                    iconSkin: "org"
                };

                var setting = {
                    async: {
                        enable: true,
                        url: getHabGroupTreeModalUrl,
                        dataFilter: ajaxHabGroupTreeDataModalFilter
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
                        onClick: HabGroupTreeModalOnClick,
                        onAsyncSuccess: HabGroupTreeModalOnAsyncSuccess,
                        onAsyncError: HabGroupTreeModalOnAsyncError,
                        onDblClick: HabGroupTreeModalOnDblClick
                    }
                };
                HabGroupTreeModal = $.fn.zTree.init($("#HabGroupTreeModal"), setting, obj);//初始化
            }, function errorCallback(e) {

            });
        }
        GetRootNode();

        function getHabGroupTreeModalUrl(treeId, treeNode) {
            TreesType = 0;
            var nodeid = "";
            if (treeNode.type == "0" || treeNode == "undefined") {
                nodeid = "00000000-0000-0000-0000-000000000000";
            } else {
                nodeid = treeNode.id;
            }
            var Url = storage.getItem("InterfaceUrl") + 'TreeNode.ashx?op=GetCompanyTree&accessToken=' + storage.getItem("userAdminToken") + '&NodeID=' + nodeid;
            return Url;
        }

        function ajaxHabGroupTreeDataModalFilter(treeId, parentNode, responseData) {
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

        function HabGroupTreeModalOnAsyncSuccess(event, treeId, treeNode, msg) {
            //HabGroupTreeModal.selectNode(treeNode);
            if (treeNode.checked == true) {
                console.log("aaaa");
                for (var i = 0; i < treeNode.children.length; i++) {
                    if (treeNode.children[i].type == '3') {
                        HabGroupTreeModal.checkNode(treeNode.children[i], true, true);
                    }
                }
            }
        }

        function HabGroupTreeModalOnAsyncError(event, treeId, treeNode, XMLHttpRequest, textStatus, errorThrown) {
            alert(XMLHttpRequest);
        };

        function HabGroupTreeModalOnClick(event, treeId, treeNode) {
            HabGroupTreeModal.selectNode(treeNode);
            HabGroupTreeModal.expandNode(treeNode);
            $(".ztree li a").css({ "padding-left": "4px", "margin-left": "-4px" });
        }

        function HabGroupTreeModalOnDblClick(event, treeId, treeNode) {
            $scope.AddMembers(treeNode);
            $(".ztree li a").css({ "padding-left": "4px", "margin-left": "-4px" });
        }

        $scope.SearchHabGroupTreeModal = function () {
            if ($scope.seachHabGroupNode == "" || $scope.seachHabGroupNode == undefined || $scope.seachHabGroupNode == null) {
                $scope.GetHabGroupTree();
            } else {
                var searchValue = $.trim($scope.seachHabGroupNode);
                var myUrl = storage.getItem("InterfaceUrl") + 'TreeNode.ashx?op=SearchDefaultCompanyTree&accessToken=' + storage.getItem("userAdminToken") + '&Searchstr=' + encodeURI(searchValue);
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
                                } else if (obj[i].type == 4) {
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
                                    chkboxType: { "Y": "s", "N": "s" }
                                },
                                callback: {
                                    onClick: HabGroupTreeModalOnClick,
                                    onAsyncSuccess: HabGroupTreeModalOnAsyncSuccess,
                                    onAsyncError: HabGroupTreeModalOnAsyncError,
                                    onDblClick: HabGroupTreeModalOnDblClick
                                }
                            };
                            HabGroupTreeModal = $.fn.zTree.init($("#HabGroupTreeModal"), setting, obj);//初始化
                        } else {

                        }

                    }).error(function (e) {

                    });
            }
        }
    }

    $scope.clearHabGroupInput = function () {
        $scope.seachHabGroupNode = '';
        $scope.GetHabGroupTreeModal();
    }

    //添加成员
    $scope.AddMembers = function (node) {
        var nodes;
        if (node != '' && node != undefined) {
            nodes = node;
        } else {
            nodes = HabGroupTreeModal.getSelectedNodes()[0];
        }
        if (nodes.Type == 2 || nodes.Type == 3) {
            var arrObj = { "ID": nodes.id, "DisplayName": nodes.Name, "Account": nodes.UserAccount != "" ? nodes.UserAccount : "-", "TypeName": nodes.Type == 2 ? "用户" : "通讯组" };
            $scope.HabGroupMembers.push(arrObj);
            if ($scope.HabGroupMembers.length != 0) {
                for (var j = 0; j < $scope.HabGroupMembers.length; j++) {
                    for (var k = j + 1; k < $scope.HabGroupMembers.length; k++)
                        if ($scope.HabGroupMembers[j].ID == $scope.HabGroupMembers[k].ID) {
                            $scope.HabGroupMembers.splice(k, 1);
                        }
                }
            }
        }
        $scope.$applyAsync();
        console.log($scope.HabGroupMembers);
    }

    //移除成员
    $scope.removeThis = function (tableid) {
        for (var i in $scope.HabGroupMembers) {
            if ($scope.HabGroupMembers[i].ID == tableid) {
                $scope.HabGroupMembers.splice(i, 1);
            }
        }
        console.log($scope.HabGroupMembers);
    }

    //编辑
    $scope.ChangeHabGroupMembers = function () {
        var paramObj = {
            "GroupID": $scope.changeMembersGroupID,
            "Members": $scope.HabGroupMembers
        };
        $scope.operateProgress();
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'HabGroup.ashx?op=AddHabMembers&accessToken=' + storage.getItem("userAdminToken"),
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            var callbackObj = response.data;
            if (callbackObj.result == "true") {
                $scope.showMsg('success', false, callbackObj.errMsg, 1);
                var treeObj = $.fn.zTree.getZTreeObj("HabGroupTree");
                var nodes = treeObj.getNodesByParam("id", $scope.changeMembersGroupID, null);
                $scope.gotoHabGroupInfoPanel(nodes[0]);
                $scope.showModalFoot = false;
            } else {
                $scope.showMsg('error', false, callbackObj.errMsg, 1);
                $scope.showModalFoot = true;
            }
        }, function errorCallback(e) {

        });
    }

    //切换tab页
    $scope.gotoTab = function (type) {
        $scope.tabNum = 'tab' + type;
        if (type == 5) {
            $scope.editUserStep = 2;
        }
    }

    function onBodyMouseDown(event) {
        if (!(event.target.id == "rMenu" || $(event.target).parents("#rMenu").length > 0)) {
            $("#rMenu").css({ "visibility": "hidden" });
        }
    }

    function HideRMenu() {
        $("#r_ouhab").hide();
        $("#r_hab").hide();
        $("#r_habgroupuser").hide();
        $("#r_adduser").hide();
    }

    $scope.ToggleCheckThis = function ($event) {
        var targetID = $event.target.id;
        var inputChecked = $event.target.checked;
        if (inputChecked != false) {
            $event.target.checked = true;
        } else {
            $event.target.checked = false;
        }
        var count = 0;
        var checkArry = document.getElementsByName("HabGroupCheckBox");
        for (var i = 0; i < checkArry.length; i++) {
            if (checkArry[i].checked == true) {
                count++;
            }
        }
        if (count == 0) {
            $scope.BatchSetupClick = false;
        } else {
            $scope.BatchSetupClick = true;
        }
    }

    $scope.CheckAll = function ($event) {
        var targetID = $event.target.id;
        var inputChecked = $event.target.checked;
        if (inputChecked != false) {
            $("#HabTable input[name='HabGroupCheckBox']").prop("checked", true);
            $scope.BatchSetupClick = true;
        } else {
            $("#HabTable input[name='HabGroupCheckBox']").prop("checked", false);
            $scope.BatchSetupClick = false;
        }
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
});