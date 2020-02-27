angular.module('app').controller('OrgCtrl', function (AuthVerify, $scope, $http, $state, $stateParams, $location, $window) {
    $(".last-col").hide();
    $("#HelpInstruction").show();
    var currentUserNode = '', autoDeployment=[];
    $scope.orgDomains = [{
        "text": "@baosteelgas.com", "value":"@baosteelgas.com"
    }]
    $scope.addUdomain = $scope.orgDomains[0].value;
    $scope.addGdomain = $scope.orgDomains[0].value;
    $scope.DistributionGroupTypes = [
        { "text": "通用域通讯组", "value": "1" },
        //{ "text": "全局域通讯组", "value": "2" },
        //{ "text": "通用域安全组", "value": "3"},
        { "text": "全局域安全组", "value": "4" }];
    $scope.seachOrgNode = '';
    var TargetNodeName = '';
    $scope.addGroupType = '';
    $scope.GroupType = '';
    $scope.ModifyOuID = '';
    $scope.seachNode = '';
    $scope.seachGroupNode = '';
    $scope.seachAdminNode = '';
    $scope.seachUserNode = '';
    $scope.seachBatchMoveNode = '';
    $scope.TargetOUNode = '';
    $scope.moveOp = '';
    $scope.addGroupAdmins = [];
    $scope.GroupAdmins = [];
    $scope.addGroupMembers = [];
    $scope.GroupMembers = [];
    $scope.BatchMoveNodes = [];
    $scope.addBelongGroups = [];
    $scope.MailSize = "";
    var DirectionalExpand = [];//刷新树逐级展开的数组
    var currentNodeID = '';
    $scope.addIsProfessionalGroups = false;
    $scope.orgCannotEdit = false;
    angular.element(".panel-heading").css("border-bottom", "1px solid #d8d8d8");
    $(document).ready(function () {
        if ($window.innerWidth >= 1024) {
            $(".content-left").css("height", $window.innerHeight - 84);
            $("#CompanyTree").css("height", $window.innerHeight - 168);
            $(".last-col").css({ "width": $window.innerWidth - 580, "height": $window.innerHeight - 130});
        } else {
            $(".content-left").css("height", $window.innerHeight - 4);
            $("#CompanyTree").css("height", $window.innerHeight - 114);
            $(".last-col").css({ "width": $window.innerWidth - 580, "height": $window.innerHeight - 76 });
        }
    })

    window.onresize= function () {
        $scope.$apply(function () {
            if ($window.innerWidth >= 1024) {
                $(".content-left").css("height", $window.innerHeight - 84);
                $("#CompanyTree").css("height", $window.innerHeight - 168);
                $(".last-col").css({ "width": $window.innerWidth - 580, "height": $window.innerHeight - 130});
            } else {
                $(".content-left").css("height", $window.innerHeight - 4);
                $("#CompanyTree").css("height", $window.innerHeight - 114);
                $(".last-col").css({ "width": $window.innerWidth - 580, "height": $window.innerHeight - 76 });
            }
        })
    };
    if (storage.getItem("ModifyProfessionalGroup") == 'true') {
        $scope.ModifyProfessionalGroup = true;
    } else {
        $scope.ModifyProfessionalGroup = false;
    }
    //公司树
    $scope.GetCompanyTree = function () {
        function GetRootNode() {
            TreesType = 1;
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
                            enable: false,//开启树的checkbox，也可以设置为radio
                            autoCheckTrigger: false,
                            chkboxType: { "Y": "", "N": "" }
                        },
                        callback: {
                            onClick: CTOnClick, //点击节点时 回调,
                            onRightClick: CTOnRightClick,
                            //beforeAsync: zTreeBeforeAsync,
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
            console.log(currentNodeID);
            if (currentNodeID != '') {
                for (var i = 0; i < treeNode.children.length; i++) {
                    if (treeNode.children[i].id == currentNodeID) {
                        CompanyTree.selectNode(treeNode.children[i]);
                    }
                }
            }
        }

        function zTreeOnAsyncError(event, treeId, treeNode, XMLHttpRequest, textStatus, errorThrown) {
            console.log(treeNode);
        };

        function CTOnClick(event, treeId, treeNode) {
            treeSelectNodeID = treeNode.id;
            treeSelectNode = treeNode;
            CompanyTree.selectNode(treeNode);
            CompanyTree.expandNode(treeNode);
            $(".ztree li a").css({ "padding-left": "4px", "margin-left": "-4px" });
            if (treeNode.Type == 1) {//OU
                if (treeNode.level == 0) {
                    $scope.orgCannotEdit = true;
                } else {
                    $scope.orgCannotEdit = false;
                }
                $scope.ModifyOuPanel(treeNode.id);
            } else if (treeNode.Type == 2) {//User
                $scope.ModifyUserPanel(treeNode.id, 'edit');
            } else if (treeNode.Type == 3) {//通讯组
                $scope.ModifyGroupPanel(treeNode.id);
            }
        }

        function CTOnRightClick(event, treeId, treeNode) {
            $("#rMenu ul").show();
            HideRMenu();
            if (!treeNode == "") {
                if (treeNode.Type == 1) {//OU
                    $scope.ModifyOuPanel(treeNode.id);
                } else if (treeNode.Type == 2) {//User
                    $scope.ModifyUserPanel(treeNode.id, 'edit');
                } else if (treeNode.Type == 3) {//通讯组
                    $scope.ModifyGroupPanel(treeNode.id);
                }
                treeSelectNodeID = treeNode.id;
                treeSelectNode = treeNode;
                CompanyTree.selectNode(treeNode);
                if (treeNode.Type == 1) {//OU
                    $("#r_addou").show();
                    $("#r_addgroup").show();
                    $("#r_adduser").show();
                    $("#r_moveou").show();
                    $("#r_delou").show();
                } else if (treeNode.Type == 2) {//User
                    $scope.ResetPwdUser = treeNode.Name + "(" + treeNode.UserAccount + ")";
                    if (treeNode.Status == true) {
                        $("#r_enableuser").hide();
                        $("#r_disableuser").show();
                    } else {
                        $("#r_enableuser").show();
                        $("#r_disableuser").hide();
                    }
                    $("#r_moveuser").show();
                    $("#r_resetpwd").show();
                    $("#r_deluser").show();
                } else if (treeNode.Type == 3) {//通讯组
                    $("#r_movegroup").show();
                    $("#r_delgroup").show();
                }
                if (treeNode.level == 0 && treeNode.iconSkin == "org") {
                    $("#r_batchmove").show();
                    //$("#r_delou").hide();
                    if (storage.getItem("IsCompanyAdmin") == 'true') {
                        $("#r_addou").show();
                        $("#r_addgroup").show();
                        $("#r_adduser").show();
                        $("#r_moveou").show();
                    } else {
                        $("#r_addou").hide();
                        $("#r_addgroup").hide();
                        $("#r_adduser").hide();
                        $("#r_moveou").hide();
                    }
                } else {
                    $("#r_batchmove").hide();
                }
                $("#rMenu").css({ "top": event.clientY + "px", "left": event.clientX + "px", "visibility": "visible" });
                $("body").bind("mousedown", onBodyMouseDown);
                //document.getElementById("CompanyTree").scrollTop = storage.getItem("scrollTop");
            }
            else {
                ErrorHandling('false', '', '请选择节点。');
            }
        }

        $scope.SearchCompanyTree = function () {
            currentNodeID = '';
            $(".last-col").hide();
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
                                //obj[i].level = 2;
                                if (obj[i].type == 0) {
                                    obj[i].iconSkin = "org";
                                }
                                if (obj[i].type == 1) {
                                    obj[i].iconSkin = "depart";
                                }
                                else if (obj[i].type == 2) {
                                    if (obj[i].Status == true) {
                                        obj[i].iconSkin = "Nuser";
                                    } else {
                                        obj[i].iconSkin = "Duser";
                                    }
                                }
                                else if (obj[i].type == 3) {
                                    obj[i].iconSkin = "group";
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
                                        onClick: CTOnClick, //点击节点时 回调,
                                        onRightClick: CTOnRightClick,
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
        currentNodeID = '';
        $(".last-col").hide();
        $scope.GetCompanyTree();
    }

    function refreshNode() {
        var type = "refresh", silent = false, nodes = CompanyTree.getSelectedNodes();
        CompanyTree.selectNode(nodes);
        CompanyTree.expandNode(nodes);
        CompanyTree.reAsyncChildNodes(nodes[0], type, silent);
    }

    function refreshParentNode() {
        var type = "refresh",silent = false,nodes = CompanyTree.getSelectedNodes();
        var parentNode = CompanyTree.getNodeByTId(nodes[0].parentTId);
        CompanyTree.selectNode(parentNode);
        CompanyTree.expandNode(parentNode);
        CompanyTree.reAsyncChildNodes(parentNode, type, silent);
    }

    //-------------------------------------   OU   ---------------------------------------
    //添加 OU
    $scope.AddOUPanel = function () {
        $("#rMenu ul").hide();
        $(".last-col").hide();
        $("#addOUPanel").show(); 
        $scope.addOUName = '';
        $scope.addOUDesc = '';
        $scope.addIsProfessionalGroups = false;
        $("body").focus();
    }

    $scope.AddOU = function () {
        if ($scope.addOUName == '') {
            ErrorHandling('false', '0000', '请输入 OU 名称。');
            return;
        }
        var paramObj = {
            "parentid": treeSelectNodeID,
            "name": $scope.addOUName,
            "description": $scope.addOUDesc,
            "IsProfessionalGroups": $scope.addIsProfessionalGroups
        }
        angular.element(".preloader").css("display", "block");
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'OrgAndUser.ashx?op=AddOu&accessToken=' + storage.getItem("userAdminToken"),
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            angular.element(".preloader").css("display", "none");
            var callbackObj = response.data;
            ErrorHandling(callbackObj.result, callbackObj.errCode, callbackObj.errMsg);
            if (callbackObj.result == "true") {
                CompanyTree.refresh();
                var parentNodes = CompanyTree.getNodesByParam("id", treeSelectNodeID, null);
                if (!parentNodes[0].open) {
                    CompanyTree.expandNode(parentNodes[0], true, true, true);
                    CompanyTree.reAsyncChildNodes(parentNodes[0], "refresh");
                } else {
                    CompanyTree.reAsyncChildNodes(parentNodes[0], "refresh");
                }
                currentNodeID = callbackObj.data.id;
                $scope.ModifyOuPanel(callbackObj.data.id);
            }
        }, function errorCallback(e) {

        });
    }

    //编辑 OU
    $scope.ModifyOuPanel = function(ouid) {
        $(".last-col").hide();
        $("#ouPanel").show();
        var paramObj = {
            "id": ouid
        }
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'OrgAndUser.ashx?op=GetOuInfo&accessToken=' + storage.getItem("userAdminToken"),
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            var callbackObj = response.data;
            if (callbackObj.result == "true") {
                $scope.ModifyOuID = ouid;
                $scope.OUName = callbackObj.data.name;
                $scope.OUDesc = callbackObj.data.description;
                $scope.IsProfessionalGroups = callbackObj.data.IsProfessionalGroups;
            } else {
                ErrorHandling(callbackObj.result, callbackObj.errCode, callbackObj.errMsg);
            }
        }, function errorCallback(e) {

        });
    }

    $scope.ModifyOu = function () {
        if ($scope.OUName == '') {
            ErrorHandling('false', '0000', '请输入 OU 名称。');
            return;
        }
        var paramObj = {
            "id": $scope.ModifyOuID,
            "name": $scope.OUName,
            "description": $scope.OUDesc,
            "IsProfessionalGroups": $scope.IsProfessionalGroups
        }
        angular.element(".preloader").css("display", "block");
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'OrgAndUser.ashx?op=ModifyOu&accessToken=' + storage.getItem("userAdminToken"),
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            angular.element(".preloader").css("display", "none");
            var callbackObj = response.data;
            ErrorHandling(callbackObj.result, callbackObj.errCode, callbackObj.errMsg);
            if (callbackObj.result == "true") {
                var parentNodes = CompanyTree.getNodesByParam("id", $scope.ModifyOuID, null)[0].getParentNode();
                if (parentNodes != null) {
                    if (!parentNodes.open) {
                        CompanyTree.expandNode(parentNodes, true, true, true);
                        CompanyTree.reAsyncChildNodes(parentNodes, "refresh");
                    } else {
                        CompanyTree.reAsyncChildNodes(parentNodes, "refresh");
                    }
                    currentNodeID = $scope.ModifyOuID;
                } else {
                    treeSelectNode.name = $scope.OUName;
                    treeSelectNode.Name = $scope.OUName;
                    CompanyTree.updateNode(treeSelectNode);
                }
                
            }
        }, function errorCallback(e) {

        });
    }

    //移动 OU
    $scope.MoveOuPanel = function () {
        HideRMenu();
        $scope.seachOrgNode = '';
        $scope.MoveTargetNodeName = '';
        $scope.TargetOUNode = '';
        $(".last-col").hide();
        $("#moveOUPanel").show();
        $scope.MoveObjTitle = '移动';
        $scope.moveOp = 'moveou';
        var paramObj = {
            "id": treeSelectNodeID
        }
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'OrgAndUser.ashx?op=GetOuInfo&accessToken=' + storage.getItem("userAdminToken"),
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            var callbackObj = response.data;
            if (callbackObj.result == "true") {
                $scope.MoveObjName = callbackObj.data.name;
                $scope.MoveOuID = treeSelectNodeID;
            } else {
                ErrorHandling(callbackObj.result, callbackObj.errCode, callbackObj.errMsg);
            }
        }, function errorCallback(e) {

        });
    }

    $scope.MoveOu = function () {
        if ($scope.TargetOUNode == '') {
            ErrorHandling('false', 0, '请选择你要移动到的 OU。');
            return;
        }
        var paramObj = {
            "TargetNode": { "NodeID": $scope.TargetOUNode },
            "NodeList": [{ "NodeID": treeSelectNodeID,"Type":1 }]
        }
        angular.element(".preloader").css("display", "block");
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'OrgAndUser.ashx?op=MoveOu&accessToken=' + storage.getItem("userAdminToken"),
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            angular.element(".preloader").css("display", "none");
            var callbackObj = response.data;
            ErrorHandling(callbackObj.result, callbackObj.errCode, callbackObj.errMsg);
            if (callbackObj.result == "true") {
                $(".last-col").hide();
                $scope.GetCompanyTree();
            } 
        }, function errorCallback(e) {

        });
    }

    //删除 OU
    $scope.DelOUPanel = function () {
        $(".last-col").hide();
        $("#ouPanel").show();
        $("#delOU_Modal").modal("show");
        HideRMenu();
        $scope.nomalModal();
        var paramObj = {
            "id": treeSelectNodeID
        }
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'OrgAndUser.ashx?op=GetOuInfo&accessToken=' + storage.getItem("userAdminToken"),
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            var callbackObj = response.data;
            if (callbackObj.result == "true") {
                $scope.DeleteOuID = treeSelectNodeID;
            } else {
                $scope.showMsg('error', false, callbackObj.errorMsg, '');
            } 
        }, function errorCallback(e) {

        });
    }

    $scope.DeleteOu = function () {
        var paramObj = {
            "id": $scope.DeleteOuID
        }
        $scope.operateProgress();
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'OrgAndUser.ashx?op=DeleteOu&accessToken=' + storage.getItem("userAdminToken"),
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            var callbackObj = response.data;
            if (callbackObj.result == "true") {
                $scope.showMsg('success', false, callbackObj.errMsg, '');
                currentNodeID = '';
                var parentNodes = CompanyTree.getNodesByParam("id", $scope.DeleteOuID, null)[0].getParentNode();
                if (parentNodes != null) {
                    if (!parentNodes.open) {
                        CompanyTree.expandNode(parentNodes, true, true, true);
                        CompanyTree.reAsyncChildNodes(parentNodes, "refresh");
                    } else {
                        CompanyTree.reAsyncChildNodes(parentNodes, "refresh");
                    }
                    CompanyTree.selectNode(parentNodes);
                    $scope.ModifyOuPanel(parentNodes.id);
                } else {
                    $(".last-col").hide();
                    $scope.SearchCompanyTree();
                }
            } else {
                $scope.showMsg('error', false, callbackObj.errMsg, '');
            }
        }, function errorCallback(e) {

        });
    }

    //-------------------------------------   Group   ---------------------------------------
    //添加通讯组
    $scope.AddGroupPanel = function () {
        $("#rMenu ul").hide();
        $(".last-col").hide();
        $("#addGroupPanel").show(); 
        $scope.addGroupName = '';
        $scope.addGroupText = '';
        $scope.addGroupType = $scope.DistributionGroupTypes[0].value;
        $scope.addGroupAdmins = [];
        $scope.addGroupMembers = [];
        $scope.addGroupDesc = '';
        $("body").focus();
    }

    //选择管理员
    $scope.gotoAddGroupAdmins = function (op) {
        $("#chooseAdmins_Modal").modal("show");
        $scope.seachAdminNode = '';
        $scope.AddGroupAdminsOp = op;
        $scope.nomalModal();
        $scope.GetAdminTree();
    }
    //树（给通讯组添加管理员）
    $scope.GetAdminTree = function () {
        function GetRootNode() {
            TreesType = 1;
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
                            url: getAdminUrl,
                            dataFilter: ajaxDataAdminFilter
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
                            autoCheckTrigger: true,
                            chkboxType: { "Y": "", "N": "" }
                        },
                        callback: {
                            onClick: AdminOnClick, //点击节点时 回调,
                            onAsyncSuccess: AdminOnAsyncSuccess,
                            onAsyncError: AdminOnAsyncError
                        }
                    };
                    AdminTree = $.fn.zTree.init($("#AdminTree"), setting, obj);//初始化
                } else {
                    ErrorHandling(data.result, data.errCode, data.errMsg);
                }
            }, function errorCallback(e) {

            });
        }
        GetRootNode();

        function getAdminUrl(treeId, treeNode) {
            TreesType = 1;
            var nodeid = "";
            if (treeNode == undefined || treeNode.type == "0") {
                nodeid = "00000000-0000-0000-0000-000000000000";
            } else {
                nodeid = treeNode.id;
            }
            var Url = storage.getItem("InterfaceUrl") + 'TreeNode.ashx?op=GetCompanyTree&accessToken=' + storage.getItem("userAdminToken") + '&NodeID=' + nodeid;
            return Url;
        }

        function ajaxDataAdminFilter(treeId, parentNode, responseData) {
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

        function AdminOnAsyncSuccess(event, treeId, treeNode, msg) {
           
        }

        function AdminOnAsyncError(event, treeId, treeNode, XMLHttpRequest, textStatus, errorThrown) {
            console.log(treeNode);
        };

        function AdminOnClick(event, treeId, treeNode) {
            $(".ztree li a").css({ "padding-left": "4px", "margin-left": "-4px" });
            AdminTree.selectNode(treeNode);
            AdminTree.expandNode(treeNode);
        }

        $scope.SearchAdminTree = function () {
            if ($scope.seachAdminNode == "" || $scope.seachAdminNode == undefined || $scope.seachAdminNode == null) {
                $scope.GetAdminTree();
            } else {
                var searchValue = $.trim($scope.seachAdminNode);
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
                                    onClick: AdminOnClick, //点击节点时 回调,
                                    onAsyncSuccess: AdminOnAsyncSuccess,
                                    onAsyncError: AdminOnAsyncError
                                }
                            };
                            AdminTree = $.fn.zTree.init($("#AdminTree"), setting, obj);//初始化
                        } else {

                        }

                    }).error(function (e) {

                    });
            }
        }
    }

    $scope.clearAdminInput = function () {
        $scope.seachAdminNode = '';
        $scope.GetAdminTree();
    }

    //添加通讯组管理员
    $scope.AddGroupAdmins = function (op) {
        $("#chooseAdmins_Modal").modal("hide");
        var nodes = AdminTree.getCheckedNodes(true);
        for (var i = 0; i < nodes.length; i++) {
            if (nodes[i].Type == 2) {
                var arrObj = { "UserID": nodes[i].id, "DisplayName": nodes[i].Name, "UserAccount": nodes[i].UserAccount != "" ? nodes[i].UserAccount : "-" };
                if (op == 'add') {
                    $scope.addGroupAdmins.push(arrObj);
                    if ($scope.addGroupAdmins.length != 0) {
                        for (var j = 0; j < $scope.addGroupAdmins.length; j++) {
                            for (var k = j + 1; k < $scope.addGroupAdmins.length; k++)
                                if ($scope.addGroupAdmins[j].UserID == $scope.addGroupAdmins[k].UserID) {
                                    $scope.addGroupAdmins.splice(k, 1);
                                }
                        }
                    }
                } else {
                    $scope.GroupAdmins.push(arrObj);
                    if ($scope.GroupAdmins.length != 0) {
                        for (var m = 0; m < $scope.GroupAdmins.length; m++) {
                            for (var n = m + 1; n < $scope.GroupAdmins.length; n++)
                                if ($scope.GroupAdmins[m].UserID == $scope.GroupAdmins[n].UserID) {
                                    $scope.GroupAdmins.splice(n, 1);
                                }
                        }
                    }
                }
            }
        }
        console.log($scope.GroupAdmins);
    }

    //移除用户通讯组
    $scope.removeGroupAdmins = function (tableid, op) {
        if (op == 'add') {
            for (var i in $scope.addGroupAdmins) {
                if ($scope.addGroupAdmins[i].UserID == tableid) {
                    $scope.addGroupAdmins.splice(i, 1);
                }
            }
            console.log($scope.addGroupAdmins);
        } else {
            for (var j in $scope.GroupAdmins) {
                if ($scope.GroupAdmins[j].UserID == tableid) {
                    $scope.GroupAdmins.splice(j, 1);
                }
            }
            console.log($scope.GroupAdmins);
        }

    }

    //选择成员
    $scope.gotoAddGroupMembers = function (op) {
        $("#chooseMembers_Modal").modal("show");
        $scope.AddGroupMembersOp = op;
        $scope.nomalModal();
        $scope.GetUserTree();
    }
    //用户树（给通讯组添加成员）
    $scope.GetUserTree = function () {
        function GetRootNode() {
            TreesType = 1;
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
                            url: getUserUrl,
                            dataFilter: ajaxDataUserFilter
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
                            autoCheckTrigger: true,
                            chkboxType: { "Y": "", "N": "" }
                        },
                        callback: {
                            onClick: UserOnClick, //点击节点时 回调,
                            onAsyncSuccess: UserOnAsyncSuccess,
                            onAsyncError: UserOnAsyncError
                        }
                    };
                    UserTree = $.fn.zTree.init($("#UserTree"), setting, obj);//初始化
                } else {
                    ErrorHandling(data.result, data.errCode, data.errMsg);
                }
            }, function errorCallback(e) {

            });
        }
        GetRootNode();

        function getUserUrl(treeId, treeNode) {
            TreesType = 1;
            var nodeid = "";
            if (treeNode == undefined || treeNode.type == "0") {
                nodeid = "00000000-0000-0000-0000-000000000000";
            } else {
                nodeid = treeNode.id;
            }
            var Url = storage.getItem("InterfaceUrl") + 'TreeNode.ashx?op=GetCompanyTree&accessToken=' + storage.getItem("userAdminToken") + '&NodeID=' + nodeid;
            return Url;
        }

        function ajaxDataUserFilter(treeId, parentNode, responseData) {
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

        function UserOnAsyncSuccess(event, treeId, treeNode, msg) {
            //UserTree.selectNode(treeNode);
        }

        function UserOnAsyncError(event, treeId, treeNode, XMLHttpRequest, textStatus, errorThrown) {
            console.log(treeNode);
        };

        function UserOnClick(event, treeId, treeNode) {
            $(".ztree li a").css({ "padding-left": "4px", "margin-left": "-4px" });
            UserTree.selectNode(treeNode);
            UserTree.expandNode(treeNode);
        }

        $scope.SearchUserTree = function () {
            if ($scope.seachUserNode == "" || $scope.seachUserNode == undefined || $scope.seachUserNode == null) {
                $scope.GetUserTree();
            } else {
                var searchValue = $.trim($scope.seachUserNode);
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
                                    onClick: UserOnClick, //点击节点时 回调,
                                    onAsyncSuccess: UserOnAsyncSuccess,
                                    onAsyncError: UserOnAsyncError
                                }
                            };
                            UserTree = $.fn.zTree.init($("#UserTree"), setting, obj);//初始化
                        } else {

                        }

                    }).error(function (e) {

                    });
            }
        }
    }

    $scope.clearUserInput = function () {
        $scope.seachUserNode = '';
        $scope.GetUserTree();
    }

    //添加通讯组成员
    $scope.AddGroupMembers = function (op) {
        $("#chooseMembers_Modal").modal("hide");
        var nodes = UserTree.getCheckedNodes(true);
        for (var i = 0; i < nodes.length; i++) {
            if (nodes[i].Type == 2 || nodes[i].Type == 3) {
                var arrObj = { "ID": nodes[i].id, "DisplayName": nodes[i].Name, "Account": nodes[i].UserAccount != "" ? nodes[i].UserAccount:"-" };
                if (op == 'add') {
                    $scope.addGroupMembers.push(arrObj);
                    if ($scope.addGroupMembers.length != 0) {
                        for (var j = 0; j < $scope.addGroupMembers.length;j++) {
                            for (var k = j + 1; k < $scope.addGroupMembers.length;k++)
                                if ($scope.addGroupMembers[j].ID == $scope.addGroupMembers[k].ID) {
                                    $scope.addGroupMembers.splice(k, 1);
                            }
                        }
                    }
                } else {
                    $scope.GroupMembers.push(arrObj);
                    if ($scope.GroupMembers.length != 0) {
                        for (var m = 0; m < $scope.GroupMembers.length; m++) {
                            for (var n = m + 1; n < $scope.GroupMembers.length; n++)
                                if ($scope.GroupMembers[m].ID == $scope.GroupMembers[n].ID) {
                                    $scope.GroupMembers.splice(n, 1);
                                }
                        }
                    }
                }
            }
        }
        console.log($scope.GroupMembers);
    }

    //移除用户通讯组
    $scope.removeGroupMembers = function (tableid, op) {
        if (op == 'add') {
            for (var i in $scope.addGroupMembers) {
                if ($scope.addGroupMembers[i].ID == tableid) {
                    $scope.addGroupMembers.splice(i, 1);
                }
            }
            console.log($scope.addGroupMembers);
        } else {
            for (var j in $scope.GroupMembers) {
                if ($scope.GroupMembers[j].ID == tableid) {
                    $scope.GroupMembers.splice(j, 1);
                }
            }
            console.log($scope.GroupMembers);
        }
       
    }

    $scope.AddGroup = function () {
        currentNodeID = '';
        if ($scope.addGroupName == '') {
            ErrorHandling('false', '', '请输入通讯组名称。');
            return;
        }
        if ($scope.addGroupType == '') {
            ErrorHandling('false', '', '请选择通讯组类型。');
            return;
        }
        if ($scope.addGroupType == 1 || $scope.addGroupType == 3) {
            if ($scope.addGroupText == '') {
                ErrorHandling('false', '', '请输入通讯组账号。');
                return;
            }
        }
        var paramObj = {
            "DisplayName": $scope.addGroupName,
            "Account": $scope.addGroupType == 1 || $scope.addGroupType == 3 ? ($scope.addGroupText + $scope.addGdomain) : "" ,
            "Type": $scope.addGroupType,
            "Members": $scope.addGroupMembers,
            "Admins": $scope.addGroupAdmins,
            "Description": $scope.addGroupDesc,
            "ParentOuId": treeSelectNodeID
        }
        angular.element(".preloader").css("display", "block");
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'OrgAndUser.ashx?op=AddGroup&accessToken=' + storage.getItem("userAdminToken"),
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            angular.element(".preloader").css("display", "none");
            var callbackObj = response.data;
            ErrorHandling(callbackObj.result, callbackObj.errCode, callbackObj.errMsg);
            if (callbackObj.result == "true") {
                CompanyTree.refresh();
                var parentNodes = CompanyTree.getNodesByParam("id", treeSelectNodeID, null);
                if (!parentNodes[0].open) {
                    CompanyTree.expandNode(parentNodes[0], true, true, true);
                    CompanyTree.reAsyncChildNodes(parentNodes[0], "refresh");
                } else {
                    CompanyTree.reAsyncChildNodes(parentNodes[0], "refresh");
                }
                currentNodeID = callbackObj.data.GroupID;
                $scope.ModifyGroupPanel(currentNodeID);
            }
        }, function errorCallback(e) {

        });
    }

    //编辑通讯组
    $scope.ModifyGroupPanel = function (groupid) {
        $(".last-col").hide();
        $("#groupPanel").show();
        var paramObj = {
            "GroupID": groupid
        }
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'OrgAndUser.ashx?op=GetGroupInfo&accessToken=' + storage.getItem("userAdminToken"),
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            var callbackObj = response.data;
            if (callbackObj.result == "true") {
                $scope.ModifyGroupID = groupid;
                $scope.GroupName = callbackObj.data.DisplayName;
                $scope.GroupAccount = callbackObj.data.Account;
                for (var i in $scope.DistributionGroupTypes) {
                    if ($scope.DistributionGroupTypes[i].value == callbackObj.data.Type) {
                        $scope.GroupType = $scope.DistributionGroupTypes[i].value;
                    }
                }
                $scope.GroupAdmins = callbackObj.data.Admins;
                $scope.GroupMembers = callbackObj.data.Members;
                $scope.GroupDesc = callbackObj.data.Description;
            } else {
                ErrorHandling(callbackObj.result, callbackObj.errCode, callbackObj.errMsg);
            }
        }, function errorCallback(e) {

        });
    }

    $scope.ModifyGroup = function () {
        if ($scope.GroupName == '') {
            ErrorHandling('false', '', '请输入通讯组名称。');
            return;
        }
        var paramObj = {
            "GroupID": $scope.ModifyGroupID,
            "DisplayName": $scope.GroupName,
            "Account": $scope.GroupAccount,
            "Type": $scope.GroupType,
            "Members": $scope.GroupMembers,
            "Admins": $scope.GroupAdmins.length != 0 ? $scope.GroupAdmins:[],
            "Description": $scope.GroupDesc,
        }
        angular.element(".preloader").css("display", "block");
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'OrgAndUser.ashx?op=ModifyGroup&accessToken=' + storage.getItem("userAdminToken"),
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            angular.element(".preloader").css("display", "none");
            var callbackObj = response.data;
            ErrorHandling(callbackObj.result, callbackObj.errCode, callbackObj.errMsg);
            if (callbackObj.result == "true") {
                var parentNodes = CompanyTree.getNodesByParam("id", treeSelectNodeID, null)[0].getParentNode();
                if (parentNodes != null) {
                    if (!parentNodes.open) {
                        CompanyTree.expandNode(parentNodes, true, true, true);
                        CompanyTree.reAsyncChildNodes(parentNodes, "refresh");
                    } else {
                        CompanyTree.reAsyncChildNodes(parentNodes, "refresh");
                    }
                    currentNodeID = $scope.ModifyGroupID;
                } else {
                    treeSelectNode.name = $scope.GroupName;
                    treeSelectNode.Name = $scope.GroupName;
                    CompanyTree.updateNode(treeSelectNode);
                }
            } 
        }, function errorCallback(e) {

        });
    }

    //删除通讯组
    $scope.DelGroupPanel = function () {
        $(".last-col").hide();
        $("#groupPanel").show();
        $("#delGroup_Modal").modal("show");
        $scope.nomalModal();
        HideRMenu();
    }

    $scope.DeleteGroup = function () {
        var paramObj = {
            "GroupID": treeSelectNodeID
        }
        $scope.operateProgress();
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'OrgAndUser.ashx?op=DeleteGroup&accessToken=' + storage.getItem("userAdminToken"),
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            var callbackObj = response.data;
            if (callbackObj.result == "true") {
                $scope.showMsg('success', false, callbackObj.errMsg, '');
                currentNodeID = '';
                var parentNodes = CompanyTree.getNodesByParam("id", treeSelectNodeID, null)[0].getParentNode();
                if (parentNodes != null) {
                    if (!parentNodes.open) {
                        CompanyTree.expandNode(parentNodes, true, true, true);
                        CompanyTree.reAsyncChildNodes(parentNodes, "refresh");
                    } else {
                        CompanyTree.reAsyncChildNodes(parentNodes, "refresh");
                    }
                    CompanyTree.selectNode(parentNodes);
                    $scope.ModifyOuPanel(parentNodes.id);
                } else {
                    $(".last-col").hide();
                    $scope.SearchCompanyTree();
                }                
            } else {
                $scope.showMsg('error', false, callbackObj.errMsg, '');
            }
        }, function errorCallback(e) {

        });
    }

    //-------------------------------------   User   ---------------------------------------
    //添加用户
    $scope.AddUserPanel = function () {
        $("#rMenu ul").hide();
        $(".last-col").hide();
        $("#addUserPanel").show();
        $scope.gotoTab(1);
        $scope.addUserStep = 1;
        $scope.addULastName = '';
        $scope.addUFirstName = '';
        $scope.addUDisplayName = '';
        $scope.addUAliasName = '';
        $scope.addUText = '';
        $scope.addUpwd = 'BGQT2020@';
        $scope.addUNextLoginChangePassword = false;
        $scope.addUIsCreateMail = true;
        $("#newadd1").parent().addClass("active");
        $("#newadd2").parent().removeClass("active");
        $("#newadd3").parent().removeClass("active");
        $("body").focus();
    }

    $scope.AddUser = function () {
        if ($scope.addULastName == '') {
            ErrorHandling('false', '', '请输入用户名称。');
            return;
        }
        if ($scope.addUFirstName == '') {
            ErrorHandling('false', '', '请输入用户姓氏。');
            return;
        }
        if ($scope.addUDisplayName == '') {
            ErrorHandling('false', '', '请输入用户显示名称。');
            return;
        }
        if ($scope.addUDisplayName.length > 30) {
            ErrorHandling('false', '', '用户显示名称长度不能超过 30 个字符。');
            return;
        }
        if ($scope.addUText == '') {
            ErrorHandling('false', '', '请输入用户账号。');
            return;
        }
        if ($scope.addUText.length > 20) {
            ErrorHandling('false', '', '用户账号长度不能超过 20 个字符。');
            return;
        }
        if ($scope.addUAliasName == '') {
            ErrorHandling('false', '', '请输入邮箱别名。');
            return;
        }
        if ($scope.addUpwd == '') {
            ErrorHandling('false', '', '请输入用户密码。');
            return;
        }
        if ($scope.passwordLevel($scope.addUpwd) < 3 || !regPwd.test($scope.addUpwd) || $scope.addUpwd.length < 8 || $scope.addUpwd.length > 20) {
            ErrorHandling('false', '', '密码支持 8~20 个字符，包含字母、数字、标点符号；不支持空格，"&"，"<"；');
            return;
        }
        var paramObj = {
            "LastName": $scope.addULastName,
            "FirstName": $scope.addUFirstName,
            "DisplayName": $scope.addUDisplayName,
            "SAMAccountName": $scope.addUText,
            "UserAccount": $scope.addUText + $scope.addUdomain,
            "AliasName": $scope.addUAliasName,
            "Password": $scope.addUpwd,
            "NextLoginChangePassword": $scope.addUNextLoginChangePassword,
            "IsCreateMail": $scope.addUIsCreateMail,
            "ParentOuId": treeSelectNodeID
        };
        angular.element(".preloader").css("display", "block");
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'OrgAndUser.ashx?op=AddUser&accessToken=' + storage.getItem("userAdminToken"),
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            angular.element(".preloader").css("display", "none");
            var callbackObj = response.data;
            ErrorHandling(callbackObj.result, callbackObj.errCode, callbackObj.errMsg);
            if (callbackObj.result == "true") {
                $scope.ModifyUserID = callbackObj.data.UserID;
                $("#newadd1").parent().removeClass("active");
                $("#newadd2").parent().addClass("active");
                $("#newadd3").parent().removeClass("active");
                $scope.addUserStep = 2;
                $scope.gotoTab(2);
                $scope.addUPhone = '';
                $scope.addUMobile = '';
                $scope.addUOffice = '';
                $scope.addUFax = '';
                $scope.addUCompany = '';
                $scope.addUDepartment = '';
                $scope.addUPost = '';
                $scope.addUDesc = '';
                CompanyTree.refresh();
                var parentNodes = CompanyTree.getNodesByParam("id", treeSelectNodeID, null);
                if (!parentNodes[0].open) {
                    CompanyTree.expandNode(parentNodes[0], true, true, true);
                    CompanyTree.reAsyncChildNodes(parentNodes[0], "refresh");
                } else {
                    CompanyTree.reAsyncChildNodes(parentNodes[0], "refresh");
                }
                currentNodeID = callbackObj.data.UserID;
                $scope.ModifyUserPanel(callbackObj.data.UserID, 'add');
            } 
        }, function errorCallback(e) {

        });
    }

    //编辑用户
    $scope.ModifyUserPanel = function (userid, op) {
        if (op == 'add') {

        } else {
            $(".last-col").hide();
            $("#userPanel").show();
            $scope.editUserStep = 1;
            $scope.gotoTab(4);
        }
        var paramObj = {
            "UserID": userid
        }
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'OrgAndUser.ashx?op=GetUserInfo&accessToken=' + storage.getItem("userAdminToken"),
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            var callbackObj = response.data;
            if (callbackObj.result == "true") {
                $scope.ModifyUserID = userid;
                $scope.ParentOu = callbackObj.data.ParentOu;
                $scope.ULastName = callbackObj.data.LastName;
                $scope.UFirstName = callbackObj.data.FirstName;
                $scope.UDisplayName = callbackObj.data.DisplayName;
                $scope.UPhone = callbackObj.data.Phone;
                $scope.UMobile = callbackObj.data.Mobile;
                $scope.UOffice = callbackObj.data.Office;
                $scope.UCompany = callbackObj.data.Company;
                $scope.UDepartment = callbackObj.data.Department;
                $scope.UPost = callbackObj.data.Post;
                $scope.UDesc = callbackObj.data.Description;
                $scope.UserAccounts = callbackObj.data.DisplayName + "(" + callbackObj.data.UserAccount + ")";
                $scope.ResetPwdUser = callbackObj.data.DisplayName + "(" + callbackObj.data.UserAccount + ")";
                $scope.UserStatus = callbackObj.data.UserStatus;
                $scope.BelongGroups = callbackObj.data.BelongGroups;
                $scope.addBelongGroups = callbackObj.data.BelongGroups;
                $scope.MoveObjName = callbackObj.data.DisplayName + "(" + callbackObj.data.UserAccount + ")";
            } else {
                ErrorHandling(callbackObj.result, callbackObj.errCode, callbackObj.errMsg);
            }
        }, function errorCallback(e) {

        });
    }

    $scope.ChangeUser = function (op) {
        var paramObj;
        if (op == 'add') {
            paramObj = {
                "UserID": $scope.ModifyUserID,
                "LastName": $scope.addULastName,
                "FirstName": $scope.addUFirstName,
                "DisplayName": $scope.addUDisplayName,
                "Phone": $scope.addUPhone,
                "Mobile": $scope.addUMobile,
                "Office": $scope.addUOffice,
                "Fax": $scope.addUFax,
                "Company": $scope.addUCompany,
                "Department": $scope.addUDepartment,
                "Post": $scope.addUPost,
                "Description": $scope.addUDesc
            }
        } else {
            if ($scope.ULastName == '') {
                ErrorHandling('false', '', '请输入用户名称。');
                return;
            }
            if ($scope.UFirstName == '') {
                ErrorHandling('false', '', '请输入用户姓氏。');
                return;
            }
            if ($scope.UDisplayName == '') {
                ErrorHandling('false', '', '请输入用户显示名称。');
                return;
            }
            if ($scope.UDisplayName.length > 30) {
                ErrorHandling('false', '', '用户显示名称长度不能超过 30 个字符。');
                return;
            }
            paramObj = {
                "UserID": $scope.ModifyUserID,
                "LastName": $scope.ULastName,
                "FirstName": $scope.UFirstName,
                "DisplayName": $scope.UDisplayName,
                "Phone": $scope.UPhone,
                "Mobile": $scope.UMobile,
                "Office": $scope.UOffice,
                "Fax": $scope.UFax,
                "Company": $scope.UCompany,
                "Department": $scope.UDepartment,
                "Post": $scope.UPost,
                "Description": $scope.UDesc
            }
        }
        angular.element(".preloader").css("display", "block");
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'OrgAndUser.ashx?op=ChangeUser&accessToken=' + storage.getItem("userAdminToken"),
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            angular.element(".preloader").css("display", "none");
            var callbackObj = response.data;
            ErrorHandling(callbackObj.result, callbackObj.errCode, callbackObj.errMsg);
            if (callbackObj.result == "true") {
                if (op == 'add') {
                    $("#newadd1").parent().removeClass("active");
                    $("#newadd2").parent().removeClass("active");
                    $("#newadd3").parent().addClass("active");
                    $scope.addUserStep = 3;
                    $scope.gotoTab(3);
                } else {
                    $scope.editUserStep = 1;
                    $scope.gotoTab(4);
                    var parentNodes = CompanyTree.getNodesByParam("id", $scope.ModifyUserID, null)[0].getParentNode();
                    if (parentNodes != null) {
                        if (!parentNodes.open) {
                            CompanyTree.expandNode(parentNodes, true, true, true);
                            CompanyTree.reAsyncChildNodes(parentNodes, "refresh");
                        } else {
                            CompanyTree.reAsyncChildNodes(parentNodes, "refresh");
                        }
                        currentNodeID = $scope.ModifyUserID;
                    } else {
                        treeSelectNode.name = $scope.UDisplayName;
                        treeSelectNode.Name = $scope.UDisplayName;
                        CompanyTree.updateNode(treeSelectNode);
                    }
                }
                
            }
        }, function errorCallback(e) {

        });
    }

    //移动用户
    $scope.MoveUserPanel = function () {
        //HideRMenu();
        //$scope.seachOrgNode = '';
        //$scope.MoveTargetNodeName = '';
        //$scope.TargetOUNode = '';
        //$(".last-col").hide();
        //$("#moveOUPanel").show();
        //$scope.MoveObjTitle = '移动';
        //$scope.moveOp = 'moveuser';
        HideRMenu();
        $scope.seachOrgNode = '';
        $scope.BatchMoveTargetNodeName = '';
        $scope.TargetOUNode = '';
        $("#rMenu ul").hide();
        $(".last-col").hide();
        $("#batchMovePanel").show();
        $scope.BatchMoveNodes = [];
        var paramObj = {
            "UserID": treeSelectNodeID
        }
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'OrgAndUser.ashx?op=GetUserInfo&accessToken=' + storage.getItem("userAdminToken"),
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            var callbackObj = response.data;
            if (callbackObj.result == "true") {
                $scope.MoveObjName = callbackObj.data.DisplayName + "(" + callbackObj.data.UserAccount + ")";
                $scope.MoveObjID = treeSelectNodeID;
                var arrObj = { "NodeID": callbackObj.data.UserID, "DisplayName": callbackObj.data.DisplayName, "Type": 2 };
                $scope.BatchMoveNodes.push(arrObj);
            } else {
                ErrorHandling(callbackObj.result, callbackObj.errCode, callbackObj.errMsg);
            }
        }, function errorCallback(e) {

        });
    }

    $scope.gotoChooseTargetNode = function (type) {
        $scope.nomalModal();
        $scope.seachOrgNode = '';
        HideRMenu();
        $("#MoveNodeforTargetNode_Modal").modal("show");
        $scope.GetOrgTree(type);
    }

    $scope.MoveUser = function () {
        if ($scope.TargetOUNode == '') {
            ErrorHandling('false', 0, '请选择你要移动到的 OU。');
            return;
        }
        var paramObj = {
            "TargetNode": { "NodeID": $scope.TargetOUNode },
            "NodeList": [{ "NodeID": treeSelectNodeID }]
        }
        angular.element(".preloader").css("display", "block");
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'OrgAndUser.ashx?op=MoveNodes&accessToken=' + storage.getItem("userAdminToken"),
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            angular.element(".preloader").css("display", "none");
            var callbackObj = response.data;
            ErrorHandling(callbackObj.result, callbackObj.errCode, callbackObj.errMsg);
            if (callbackObj.result == "true") {
                $(".last-col").hide();
                $scope.GetCompanyTree();
            }
        }, function errorCallback(e) {

        });
    }

    $scope.gotoAddGroups = function () {
        $("#chooseGroups_Modal").modal("show");
        $scope.seachGroupNode = '';
        $scope.nomalModal();
        $scope.GetGroupTree();
    }

    //获取用户邮箱属性
    $scope.GetUserExchangeInfo = function () {
        $scope.editUserStep = 3;
        paramObj = {
            "UserID": $scope.ModifyUserID
        }
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'OrgAndUser.ashx?op=GetUserExchangeInfo&accessToken=' + storage.getItem("userAdminToken"),
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            var callbackObj = response.data;
            if (callbackObj.result == "true") {
                $scope.ExchangeStatus = callbackObj.data.UserExchange.ExchangeStatus;
                $scope.Activesync = callbackObj.data.UserExchange.Activesync;
                $scope.Mapi = callbackObj.data.UserExchange.Mapi;
                $scope.POP3 = callbackObj.data.UserExchange.POP3;
                $scope.Imap4 = callbackObj.data.UserExchange.Imap4;
                $scope.OWA = callbackObj.data.UserExchange.OWA;
                $scope.Database = callbackObj.data.UserExchange.Database;
                $scope.UAliasName = callbackObj.data.AliasName;
                if (callbackObj.data.UserExchange.MailSize == -1 || callbackObj.data.UserExchange.MailSize == 0) {
                    $scope.MailSize = "";
                    $scope.DefaultSet = true;
                } else {
                    $scope.DefaultSet = false;
                    $scope.MailSize = callbackObj.data.UserExchange.MailSize;
                }
            } else {
                ErrorHandling(callbackObj.result, callbackObj.errCode, callbackObj.errMsg);
            }
        }, function errorCallback(e) {

        });
    }

    $scope.CheckDefaultMailSize = function ($event) {
        if ($scope.DefaultSet) {
            $scope.MailSize = "";
        }
    }

    //修改用户邮箱属性
    $scope.ChangeUserExchangeInfo = function () {
        if ($scope.DefaultSet == false) {
            if ($scope.MailSize == null) {
                ErrorHandling('false', '0', '请输入邮箱空间大小。');
                return;
            } else if ($scope.MailSize < 1 || $scope.MailSize > 100) {
                ErrorHandling('false', '0', '邮箱空间大小为 1-100 GB。');
                return;
            }
        }
        if ($scope.ExchangeStatus == true) {
            if ($scope.UAliasName == '') {
                ErrorHandling('false', '', '请输入邮箱别名。');
                return;
            }
        }
        var paramObj;
        paramObj = {
            "UserID": $scope.ModifyUserID,
            "AliasName": $scope.UAliasName,
            "UserExchange": {
                "ExchangeStatus": $scope.ExchangeStatus,
                "Activesync": $scope.Activesync,
                "Mapi": $scope.Mapi,
                "POP3": $scope.POP3,
                "Imap4": $scope.Imap4,
                "OWA": $scope.OWA,
                "MailSize": $scope.MailSize != "" ? $scope.MailSize : -1
            }
        }
        angular.element(".preloader").css("display", "block");
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'OrgAndUser.ashx?op=ChangeUserExchangeInfo&accessToken=' + storage.getItem("userAdminToken"),
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            angular.element(".preloader").css("display", "none");
            var callbackObj = response.data;
            ErrorHandling(callbackObj.result, callbackObj.errCode, callbackObj.errMsg);
            if (callbackObj.result == "true") {
                //$scope.editUserStep = 3;
                //$scope.gotoTab(6);
                $scope.ModifyUserPanel($scope.ModifyUserID, 'edit');
            }
        }, function errorCallback(e) {

        });
    }

    //添加用户通讯组
    $scope.AddBelongGroups = function () {
        $("#chooseGroups_Modal").modal("hide");
        var nodes = GroupTree.getCheckedNodes(true);
        for (var i = 0; i < nodes.length; i++) {
            if (nodes[i].Type == 3) {
                var arrObj = { "GroupID": nodes[i].id, "DisplayName": nodes[i].Name };
                if ($scope.addUserStep == 3) {//添加用户
                    $scope.addBelongGroups.push(arrObj);
                    if ($scope.addBelongGroups.length != 0) {       
                        for (var j = 0; j < $scope.addBelongGroups.length; j++) {
                            for (var k = j + 1; k < $scope.addBelongGroups.length; k++)
                                if ($scope.addBelongGroups[j].GroupID == $scope.addBelongGroups[k].GroupID) {
                                    $scope.addBelongGroups.splice(k, 1);
                                }
                        }
                    }
                } else {//编辑用户
                    $scope.BelongGroups.push(arrObj);
                    if ($scope.BelongGroups.length != 0) {
                        for (var j = 0; j < $scope.BelongGroups.length; j++) {
                            for (var k = j + 1; k < $scope.BelongGroups.length; k++)
                                if ($scope.BelongGroups[j].GroupID == $scope.BelongGroups[k].GroupID) {
                                    $scope.BelongGroups.splice(k, 1);
                                }
                        }
                    }
                }
                
            }
        }
        console.log($scope.BelongGroups);
    }

    //移除用户通讯组
    $scope.removeThis = function (tableid, op) {
        if (op == 'add') {
            for (var i in $scope.addBelongGroups) {
                if ($scope.addBelongGroups[i].GroupID == tableid) {
                    $scope.addBelongGroups.splice(i, 1);
                }
            }
            console.log($scope.addBelongGroups);
        } else {
            for (var i in $scope.BelongGroups) {
                if ($scope.BelongGroups[i].GroupID == tableid) {
                    $scope.BelongGroups.splice(i, 1);
                }
            }
            console.log($scope.BelongGroups);
        }
       
    }

    //修改用户通讯组
    $scope.ChangeUserMemberof = function (op) {
        if (op == 'add') {
            var paramObj = {
                "UserID": $scope.ModifyUserID,
                "BelongGroups": $scope.addBelongGroups
            }
        } else {
            var paramObj = {
                "UserID": $scope.ModifyUserID,
                "BelongGroups": $scope.BelongGroups
            }
        }
        angular.element(".preloader").css("display", "block");
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'OrgAndUser.ashx?op=ChangeUserMemberof&accessToken=' + storage.getItem("userAdminToken"),
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            angular.element(".preloader").css("display", "none");
            var callbackObj = response.data;
            ErrorHandling(callbackObj.result, callbackObj.errCode, callbackObj.errMsg);
            if (callbackObj.result == "true") {
                $scope.ModifyUserPanel($scope.ModifyUserID, 'edit');
            }
        }, function errorCallback(e) {

        });
    }

    // 通讯组树(给用户设置通讯组)
    $scope.GetGroupTree = function () {
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
                            url: getGroupUrl,
                            dataFilter: ajaxDataGroupFilter
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
                            autoCheckTrigger: true,
                            chkboxType: { "Y": "", "N": "" }
                        },
                        callback: {
                            onClick: GroupOnClick, //点击节点时 回调,
                            onAsyncSuccess: GroupOnAsyncSuccess,
                            onAsyncError: GroupOnAsyncError
                        }
                    };
                    GroupTree = $.fn.zTree.init($("#GroupTree"), setting, obj);//初始化
                } else {
                    ErrorHandling(data.result, data.errCode, data.errMsg);
                }
            }, function errorCallback(e) {

            });

        }
        GetRootNode();

        function getGroupUrl(treeId, treeNode) {
            TreesType = 1;
            var nodeid = "";
            if (treeNode == undefined || treeNode.type == "0") {
                nodeid = "00000000-0000-0000-0000-000000000000";
            } else {
                nodeid = treeNode.id;
            }
            var Url = storage.getItem("InterfaceUrl") + 'TreeNode.ashx?op=GetGroupTree&accessToken=' + storage.getItem("userAdminToken") + '&NodeID=' + nodeid;
            return Url;
        }

        function ajaxDataGroupFilter(treeId, parentNode, responseData) {
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

        function GroupOnAsyncSuccess(event, treeId, treeNode, msg) {
            //GroupTree.selectNode(treeNode);
        }

        function GroupOnAsyncError(event, treeId, treeNode, XMLHttpRequest, textStatus, errorThrown) {
            console.log(treeNode);
        };

        function GroupOnClick(event, treeId, treeNode) {
            GroupTree.selectNode(treeNode);
            GroupTree.expandNode(treeNode);
            $(".ztree li a").css({ "padding-left": "4px", "margin-left": "-4px" });
        }

        $scope.SearchGroupTree = function () {
            if ($scope.seachGroupNode == "" || $scope.seachGroupNode == undefined || $scope.seachGroupNode == null) {
                $scope.GetGroupTree();
            } else {
                var searchValue = $.trim($scope.seachGroupNode);
                var myUrl = storage.getItem("InterfaceUrl") + 'TreeNode.ashx?op=SearchGroupTree&accessToken=' + storage.getItem("userAdminToken") + '&Searchstr=' + encodeURI(searchValue);
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
                                    onClick: GroupOnClick, //点击节点时 回调,
                                    onAsyncSuccess: GroupOnAsyncSuccess,
                                    onAsyncError: GroupOnAsyncError
                                }
                            };
                            GroupTree = $.fn.zTree.init($("#GroupTree"), setting, obj);//初始化
                        } else {

                        }

                    }).error(function (e) {

                    });
            }
        }
    }

    $scope.clearGroupInput = function () {
        $scope.seachGroupNode = '';
        $scope.GetGroupTree();
    }

    //启用用户
    $scope.EnableUser = function () {
        HideRMenu();
        var paramObj = {
            "UserID": $scope.ModifyUserID
        }
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'OrgAndUser.ashx?op=EnableUser&accessToken=' + storage.getItem("userAdminToken"),
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            var callbackObj = response.data;
            ErrorHandling(callbackObj.result, callbackObj.errCode, callbackObj.errMsg);
            if (callbackObj.result == "true") {
                var node = CompanyTree.getNodesByParam("id", $scope.ModifyUserID, null);
                node[0].Status = true;
                node[0].iconSkin = 'Nuser';
                CompanyTree.updateNode(node[0]);
                $scope.ModifyUserPanel($scope.ModifyUserID, 'edit');
            }
        }, function errorCallback(e) {

        });
    }

    //停用用户
    $scope.DisableUserPanel = function () {
        $(".last-col").hide();
        $("#userPanel").show();
        $("#ChangeUserState_Modal").modal("show");
        $scope.nomalModal();
        HideRMenu();
    }

    $scope.DisableUser = function () {
        var paramObj = {
            "UserID": $scope.ModifyUserID
        }
        $scope.operateProgress();
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'OrgAndUser.ashx?op=DisableUser&accessToken=' + storage.getItem("userAdminToken"),
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            var callbackObj = response.data;
            if (callbackObj.result == "true") {
                $scope.showMsg('success', false, callbackObj.errMsg, '');
                var node = CompanyTree.getNodesByParam("id", $scope.ModifyUserID, null);
                node[0].Status = false;
                node[0].iconSkin = 'Duser';
                CompanyTree.updateNode(node[0]);
                $scope.ModifyUserPanel($scope.ModifyUserID, 'edit');
            } else {
                $scope.showMsg('error', false, callbackObj.errMsg, '');
            }
        }, function errorCallback(e) {

        });
    }

    //重置密码
    $scope.ResetPwdPanel = function () {
        $("#resetPassword_Modal").modal("show");
        $scope.nomalModal();
        $scope.reSetPwd = 'BGQT2020@';
        $scope.reNextLoginChangePassword = false;
        $scope.rePasswordNeverExpire = false;
        HideRMenu();
    }

    $scope.ResetPassword = function () {
        if ($scope.reSetPwd == '') {
            $scope.showMsg('error', false, '请输入用户密码。', '');
            return;
        }
        if ($scope.passwordLevel($scope.reSetPwd) < 3 || !regPwd.test($scope.reSetPwd) || $scope.reSetPwd.length < 8 || $scope.reSetPwd.length > 20) {
            $scope.showMsg('error', false, '密码支持 8~20 个字符，包含字母、数字、标点符号；不支持空格，"&"，"<"；', '');
            return;
        }
        var paramObj = {
            "UserID": $scope.ModifyUserID,
            "Password": $scope.reSetPwd,
            "NextLoginChangePassword": $scope.reNextLoginChangePassword,
            "PasswordNeverExpire": $scope.rePasswordNeverExpire
        }
        $scope.operateProgress();
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'OrgAndUser.ashx?op=ResetUserPassword&accessToken=' + storage.getItem("userAdminToken"),
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            var callbackObj = response.data;
            if (callbackObj.result == "true") {
                $scope.showMsg('success', false, callbackObj.errMsg, '');
            } else {
                $scope.showMsg('error', false, callbackObj.errMsg, '');
            }
        }, function errorCallback(e) {

        });
    }

    //删除用户
    $scope.DelUserPanel = function () {
        $(".last-col").hide();
        $("#userPanel").show();
        $("#delUser_Modal").modal("show");
        $scope.nomalModal();
    }

    $scope.DeleteUser = function () {
        var paramObj = {
            "UserID": treeSelectNodeID
        }
        $scope.operateProgress();
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'OrgAndUser.ashx?op=DeleteUser&accessToken=' + storage.getItem("userAdminToken"),
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            var callbackObj = response.data;
            if (callbackObj.result == "true") {
                $scope.showMsg('success', false, callbackObj.errMsg, '');
                currentNodeID = '';
                var parentNodes = CompanyTree.getNodesByParam("id", treeSelectNodeID, null)[0].getParentNode();
                if (parentNodes != null) {
                    if (!parentNodes.open) {
                        CompanyTree.expandNode(parentNodes, true, true, true);
                        CompanyTree.reAsyncChildNodes(parentNodes, "refresh");
                    } else {
                        CompanyTree.reAsyncChildNodes(parentNodes, "refresh");
                    }
                    CompanyTree.selectNode(parentNodes);
                    $scope.ModifyOuPanel(parentNodes.id);
                } else {
                    $(".last-col").hide();
                    $scope.SearchCompanyTree();
                }
            } else {
                $scope.showMsg('error', false, callbackObj.errMsg, '');
            }
        }, function errorCallback(e) {

        });
    }

    //批量移动
    $scope.BatchMovePanel = function () {
        HideRMenu();
        $scope.seachOrgNode = '';
        $scope.BatchMoveTargetNodeName = '';
        $scope.TargetOUNode = '';
        $("#rMenu ul").hide();
        $(".last-col").hide();
        $("#batchMovePanel").show(); 
        $scope.BatchMoveNodes = [];
    }

    // OU 树(移动 OU 和批量移动目标节点)
    $scope.GetOrgTree = function (treeStr) {
        function GetRootNode() {
            var nodeid = "00000000-0000-0000-0000-000000000000";
            var Url = '';
            if (storage.getItem("SpanOUMove") == 'true') {
                Url = storage.getItem("InterfaceUrl") + 'TreeNode.ashx?op=GetDefaultOrgTree&accessToken=' + storage.getItem("userAdminToken") + '&NodeID=' + nodeid;
            } else {
                Url = storage.getItem("InterfaceUrl") + 'TreeNode.ashx?op=GetOrgTree&accessToken=' + storage.getItem("userAdminToken") + '&NodeID=' + nodeid;
            }
            $http({
                method: 'GET',
                url: Url,
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
                } else {
                    ErrorHandling(data.result, data.errCode, data.errMsg);
                }
            }, function errorCallback(e) {

            });

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
            var Url = '';
            if (storage.getItem("SpanOUMove") == 'true') {
                Url = storage.getItem("InterfaceUrl") + 'TreeNode.ashx?op=GetDefaultOrgTree&accessToken=' + storage.getItem("userAdminToken") + '&NodeID=' + nodeid;
            } else {
                Url = storage.getItem("InterfaceUrl") + 'TreeNode.ashx?op=GetOrgTree&accessToken=' + storage.getItem("userAdminToken") + '&NodeID=' + nodeid;
            }
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
            OrgTree.selectNode(treeNode);
            OrgTree.expandNode(treeNode);
            $(".ztree li a").css({ "padding-left": "4px", "margin-left": "-4px" });
            if (treeNode.Type == 1) {
                $scope.TargetOUNode = treeNode.id;
                TargetNodeName = treeNode.name;
            } else {
                $scope.TargetOUNode = '';
            }
        }

        $scope.SearchOrgTree = function () {
            if ($scope.seachOrgNode == "" || $scope.seachOrgNode == undefined || $scope.seachOrgNode == null) {
                $scope.GetOrgTree(treeStr);
            } else {
                var searchValue = $.trim($scope.seachOrgNode);
                var myUrl = '';
                if (storage.getItem("SpanOUMove") == 'true') {
                    myUrl = storage.getItem("InterfaceUrl") + 'TreeNode.ashx?op=SearchDefaultOrgTree&accessToken=' + storage.getItem("userAdminToken") + '&Searchstr=' + searchValue;
                } else {
                    myUrl = storage.getItem("InterfaceUrl") + 'TreeNode.ashx?op=SearchOrgTree&accessToken=' + storage.getItem("userAdminToken") + '&Searchstr=' + searchValue;
                }
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

        $scope.clearSearchOrgNodeInput = function () {
            $scope.seachOrgNode = '';
            $scope.GetOrgTree(treeStr);
        }
    }

    $scope.chooseTargetNode = function () {
        $("#MoveNodeforTargetNode_Modal").modal("hide");
        if ($scope.TargetOUNode != '') {
            $scope.BatchMoveTargetNodeName = TargetNodeName;
            $scope.MoveTargetNodeName = TargetNodeName;
        } else {
            $scope.BatchMoveTargetNodeName = '';
            $scope.MoveTargetNodeName = '';
        }
    }

   
    //批量移动选择成员
    $scope.GetBatchMoveTree = function () {
        function GetRootNode() {
            var nodeid = "00000000-0000-0000-0000-000000000000";
            var Url = '';
            if (storage.getItem("SpanOUMove") == true) {
                Url = storage.getItem("InterfaceUrl") + 'TreeNode.ashx?op=GetDefaultCompanyTree&accessToken=' + storage.getItem("userAdminToken") + '&NodeID=' + nodeid;
            } else {
                Url = storage.getItem("InterfaceUrl") + 'TreeNode.ashx?op=GetCompanyTree&accessToken=' + storage.getItem("userAdminToken") + '&NodeID=' + nodeid;
            }
            $http({
                method: 'GET',
                url: Url,
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
                            url: getBatchMoveUrl,
                            dataFilter: ajaxBatchMoveDataFilter
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
                            chkboxType: { "Y": "s", "N": "s" }
                        },
                        callback: {
                            onClick: BatchMoveOnClick, //点击节点时 回调,
                            onAsyncSuccess: BatchMoveOnAsyncSuccess,
                            onAsyncError: BatchMoveOnAsyncError
                        }
                    };
                    BatchMoveTree = $.fn.zTree.init($("#BatchMoveTree"), setting, obj);//初始化
                } else {
                    ErrorHandling(data.result, data.errCode, data.errMsg);
                }
            }, function errorCallback(e) {

            });

           
        }
        GetRootNode();

        function getBatchMoveUrl(treeId, treeNode) {
            TreesType = 0;
            var nodeid = "";
            if (treeNode == undefined || treeNode.type == "0") {
                nodeid = "00000000-0000-0000-0000-000000000000";
            } else {
                nodeid = treeNode.id;
            }
            var Url = '';
            if (storage.getItem("SpanOUMove") == true) {
                Url = storage.getItem("InterfaceUrl") + 'TreeNode.ashx?op=GetDefaultCompanyTree&accessToken=' + storage.getItem("userAdminToken") + '&NodeID=' + nodeid;
            } else {
                Url = storage.getItem("InterfaceUrl") + 'TreeNode.ashx?op=GetCompanyTree&accessToken=' + storage.getItem("userAdminToken") + '&NodeID=' + nodeid;
            }
            return Url;
        }

        function ajaxBatchMoveDataFilter(treeId, parentNode, responseData) {
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

        function BatchMoveOnAsyncSuccess(event, treeId, treeNode, msg) {
            //BatchMoveTree.selectNode(treeNode);
        }

        function BatchMoveOnAsyncError(event, treeId, treeNode, XMLHttpRequest, textStatus, errorThrown) {
            console.log(treeNode);
        };

        function BatchMoveOnClick(event, treeId, treeNode) {
            BatchMoveTree.selectNode(treeNode);
            BatchMoveTree.expandNode(treeNode);
            $(".ztree li a").css({ "padding-left": "4px", "margin-left": "-4px" });
        }

        $scope.SearchBatchMoveTree = function () {
            if ($scope.seachBatchMoveNode == "" || $scope.seachBatchMoveNode == undefined || $scope.seachBatchMoveNode == null) {
                $scope.GetBatchMoveTree();
            } else {
                var searchValue = $.trim($scope.seachBatchMoveNode);
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
                                    onClick: BatchMoveOnClick, //点击节点时 回调,
                                    onAsyncSuccess: BatchMoveOnAsyncSuccess,
                                    onAsyncError: BatchMoveOnAsyncError
                                }
                            };
                            BatchMoveTree = $.fn.zTree.init($("#BatchMoveTree"), setting, obj);//初始化
                        } else {

                        }

                    }).error(function (e) {

                    });
            }
        }
    }

    $scope.clearBatchMoveInput = function () {
        $scope.seachBatchMoveNode = '';
        $scope.GetBatchMoveTree();
    }

    $scope.gotoAddBatchMoveNodes = function () {
        $("#AddBatchMoveNode_Modal").modal("show");
        $scope.seachBatchMoveNode = '';
        $scope.nomalModal();
        $scope.GetBatchMoveTree();
    }

    $scope.AddBatchMoveNodes = function () {
        $("#AddBatchMoveNode_Modal").modal("hide");
        var nodes = BatchMoveTree.getCheckedNodes(true);
        for (var i = 0; i < nodes.length; i++) {
            if (nodes[i].type == 2 || nodes[i].type == 3) {
                var arrObj = { "NodeID": nodes[i].id, "DisplayName": nodes[i].Name, "Type": nodes[i].Type };
                $scope.BatchMoveNodes.push(arrObj);
                if ($scope.BatchMoveNodes.length != 0) {
                    for (var j = 0; j < $scope.BatchMoveNodes.length; j++) {
                        for (var k = j + 1; k < $scope.BatchMoveNodes.length; k++) {
                            if ($scope.BatchMoveNodes[k].NodeID == $scope.BatchMoveNodes[j].NodeID) {
                                $scope.BatchMoveNodes.splice(k, 1);
                            }
                        }
                    }
                }
            }
        }
        console.log($scope.BatchMoveNodes);
    }

    //移除用户通讯组
    $scope.removeBatchMoveNodes = function (tableid) {
        for (var i in $scope.BatchMoveNodes) {
            if ($scope.BatchMoveNodes[i].NodeID == tableid) {
                $scope.BatchMoveNodes.splice(i, 1);
            }
        }
        console.log($scope.BatchMoveNodes);
    }

    $scope.BatchMoveNode = function () {
        if ($scope.TargetOUNode == '') {
            ErrorHandling('false', 0, '请选择你要移动到的 OU。');
            return;
        }
        if ($scope.BatchMoveNodes.length == 0) {
            ErrorHandling('false', 0, '请选择你要移动的节点。');
            return;
        }
        var paramObj = {
            "TargetNode": { "NodeID": $scope.TargetOUNode},
            "NodeList": $scope.BatchMoveNodes
        }
        angular.element(".preloader").css("display", "block");
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'OrgAndUser.ashx?op=MoveNodes&accessToken=' + storage.getItem("userAdminToken"),
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            angular.element(".preloader").css("display", "none");
            var callbackObj = response.data;
            ErrorHandling(callbackObj.result, callbackObj.errCode, callbackObj.errMsg);
            if (callbackObj.result == "true") {
                $(".last-col").hide();
                $scope.GetCompanyTree();
            } 
        }, function errorCallback(e) {

        });
    }

    //生成随机密码
    $scope.randPassword = function (type, randomFlag, min, max) {
        var ischeck = true;
        if (ischeck) {
            var str = "",
                range = min,
                arr = ['1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'm', 'n', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'];
            // 随机产生
            if (randomFlag) {
                range = Math.round(Math.random() * (max - min)) + min;
            }
            for (var i = 0; i < range; i++) {
                if (i == 6 && (!str.match(/\d+/g))) {
                    var arr1 = ['1', '2', '3', '4', '5', '6', '7', '8', '9'];
                    pos = Math.round(Math.random() * (arr1.length - 1));
                    str += arr[pos];
                } else if (i == 7) {
                    var arr2 = ['!', '@', '#', '$', '%', '*', '(', ')', '.'];
                    pos = Math.round(Math.random() * (arr2.length - 1));
                    str += arr2[pos];
                }
                else {
                    pos = Math.round(Math.random() * (arr.length - 1));
                    str += arr[pos];
                }
            }
            $scope.reSetPwd = str;
            $scope.addUpwd = str;
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

    //切换 tab 页
    $scope.gotoTab = function (type) {
        $scope.tabNum = 'tab' + type;
        if (type == 5) {
            $scope.editUserStep = 2;
        }
        angular.element("#tab" + type).addClass("active").siblings().removeClass("active");
    }

    //隐藏右键菜单
    function HideRMenu() {
        $("#r_addou").hide();
        $("#r_addgroup").hide();
        $("#r_adduser").hide();
        $("#r_movegroup").hide();
        $("#r_delgroup").hide();
        $("#r_moveou").hide();
        $("#r_delou").hide();
        $("#r_moveuser").hide();
        $("#r_enableuser").hide();
        $("#r_disableuser").hide();
        $("#r_resetpwd").hide(); 
        $("#r_deluser").hide();
        $("#r_batchmove").hide();
    }
    
    function onBodyMouseDown(event) {
        if (!(event.target.id == "rMenu" || $(event.target).parents("#rMenu").length > 0)) {
            $("#rMenu").css({ "visibility": "hidden" });
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