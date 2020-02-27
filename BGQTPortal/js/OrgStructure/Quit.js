angular.module('app').controller('QuitCtrl', function (AuthVerify, $scope, $http, $state, $stateParams, $location, $window) {
    $scope.TargetNode = '';
    $(".last-col").hide();
    $("#HelpInstruction").show();
    $scope.seachNode = '';
    $scope.seachOrgNode = "";
    if ($window.innerWidth >= 1024) {
        $(".content-left").css("height", $window.innerHeight - 84);
        $("#DelOrgTree").css("height", (window.innerHeight - 168) + "px");
        $(".last-col").css({ "width": (window.innerWidth - 580) + "px", "height": (window.innerHeight - 130) + "px" });
    } else {
        $(".content-left").css("height", $window.innerHeight - 4);
        $("#DelOrgTree").css("height", (window.innerHeight - 114) + "px");
        $(".last-col").css({ "width": (window.innerWidth - 580) + "px", "height": (window.innerHeight - 76) + "px" });
    }
    window.onresize = function () {
        $scope.$apply(function () {
            if ($window.innerWidth >= 1024) {
                $(".content-left").css("height", $window.innerHeight - 84);
                $("#DelOrgTree").css("height", (window.innerHeight - 168) + "px");
                $(".last-col").css({ "width": (window.innerWidth - 580) + "px", "height": (window.innerHeight - 130) + "px" });
            } else {
                $(".content-left").css("height", $window.innerHeight - 4);
                $("#DelOrgTree").css("height", (window.innerHeight - 114) + "px");
                $(".last-col").css({ "width": (window.innerWidth - 580) + "px", "height": (window.innerHeight - 76) + "px" });
            }
        })
    };
   
    //公司树
    $scope.GetDeleteOrgTree = function () {
        function GetRootNode() {
            var nodeid = "00000000-0000-0000-0000-000000000000";
            $http({
                method: 'GET',
                url: storage.getItem("InterfaceUrl") + 'TreeNode.ashx?op=GetDeleteOrgTree&accessToken=' + storage.getItem("userAdminToken") + '&NodeID=' + nodeid,
            }).then(function successCallback(response) {
                var data = response.data;
                console.log(data)
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
                            onClick: DelOrgOnClick, //点击节点时 回调,
                            onAsyncSuccess: DelOrgOnAsyncSuccess,
                            onAsyncError: DelOrgOnAsyncError
                        }
                    };
                    DelOrgTree = $.fn.zTree.init($("#DelOrgTree"), setting, obj);//初始化
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
            var Url = storage.getItem("InterfaceUrl") + 'TreeNode.ashx?op=GetDeleteOrgTree&accessToken=' + storage.getItem("userAdminToken") + '&NodeID=' + nodeid;
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

        function DelOrgOnAsyncSuccess(event, treeId, treeNode, msg) {
            //DelOrgTree.selectNode(treeNode);
            //if (treeNode.Type == 2) {//User
            //    $scope.UserPanel(treeNode);
            //} 
        }

        function DelOrgOnAsyncError(event, treeId, treeNode, XMLHttpRequest, textStatus, errorThrown) {
            console.log(treeNode);
        };

        function DelOrgOnClick(event, treeId, treeNode) {
            treeSelectNodeID = treeNode.id;
            treeSelectNode = treeNode;
            DelOrgTree.selectNode(treeNode);
            DelOrgTree.expandNode(treeNode);
            $(".ztree li a").css({ "padding-left": "4px", "margin-left": "-4px" });
            if (treeNode.Type == 2) {//User
                $scope.UserPanel(treeNode.id);
            } 
        }

        $scope.SearchDelOrgTree = function () {
            $(".last-col").hide();
            if ($scope.seachNode == "" || $scope.seachNode == undefined || $scope.seachNode == null) {
                $scope.GetCompanyTree();
            } else {
                var searchValue = $.trim($scope.seachNode);
                var myUrl = storage.getItem("InterfaceUrl") + 'TreeNode.ashx?op=SearchDeleteOrgTree&accessToken=' + storage.getItem("userAdminToken") + '&Searchstr=' + encodeURI(searchValue);
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
                                    onClick: DelOrgOnClick, //点击节点时 回调,
                                    onAsyncSuccess: DelOrgOnAsyncSuccess,
                                    onAsyncError: DelOrgOnAsyncError
                                }
                            };
                            DelOrgTree = $.fn.zTree.init($("#DelOrgTree"), setting, obj);//初始化
                        } else {

                        }

                    }).error(function (e) {

                    });
            }
        }
    }

    $scope.clearSearchInput = function () {
        $(".last-col").hide();
        $scope.seachNode = '';
        $scope.GetDeleteOrgTree();
    }

    $scope.UserPanel = function (userid) {
        $(".last-col").hide();
        $("#userPanel").show();
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
                $scope.UserStatus = callbackObj.data.UserStatus;
                $scope.BelongGroups = callbackObj.data.BelongGroups;
                $scope.addBelongGroups = callbackObj.data.BelongGroups;
            } else {
                ErrorHandling(callbackObj.result, callbackObj.errCode, callbackObj.errMsg);
            }
        }, function errorCallback(e) {

        });
    }

    //恢复用户
    $scope.ResumeUserPanel = function () {
        $(".last-col").hide();
        $("#userPanel").show();
        $("#chooseOU_Modal").modal("show");
        $scope.seachOrgNode = '';
        $scope.IsCreateMail = false;
        $scope.UseDefaultPassword = true;
        $scope.GetOrgTree();
        $scope.nomalModal();
    }

    $scope.ResumeUser = function () {
        if ($scope.TargetNode == '') {
            $scope.showMsg('error', false, '请选择你要恢复到的 OU。', '');
            return;
        }
        var paramObj = {
            "ParentOuId": $scope.TargetNode,
            "UserID": treeSelectNodeID,
            "IsCreateMail": $scope.IsCreateMail,
            "UseDefaultPassword": $scope.UseDefaultPassword
        }
        $scope.operateProgress();
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'OrgAndUser.ashx?op=ResumeUser&accessToken=' + storage.getItem("userAdminToken"),
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            var callbackObj = response.data;
            if (callbackObj.result == "true") {
                $scope.showMsg('success', false, callbackObj.errMsg, '');
                var parentNodes = DelOrgTree.getNodesByParam("id", treeSelectNodeID, null)[0].getParentNode();
                if (parentNodes != null) {
                    if (!parentNodes.open) {
                        DelOrgTree.expandNode(parentNodes, true, true, true);
                        DelOrgTree.reAsyncChildNodes(parentNodes, "refresh");
                    } else {
                        DelOrgTree.reAsyncChildNodes(parentNodes, "refresh");
                    }
                    DelOrgTree.selectNode(parentNodes);
                } else {
                    $scope.SearchDelOrgTree();
                }
                $(".last-col").hide();
                $("#userPanel").hide();
            } else {
                $scope.showMsg('error', false, callbackObj.errMsg, '');
            }
        }, function errorCallback(e) {

        });
    }

    // OU 树
    $scope.GetOrgTree = function () {
        function GetRootNode() {
            TreesType = 1;
            var nodeid = "00000000-0000-0000-0000-000000000000";
            $http({
                method: 'GET',
                url: storage.getItem("InterfaceUrl") + 'TreeNode.ashx?op=GetOrgTree&accessToken=' + storage.getItem("userAdminToken") + '&NodeID=' + nodeid,
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
            var Url = storage.getItem("InterfaceUrl") + 'TreeNode.ashx?op=GetOrgTree&accessToken=' + storage.getItem("userAdminToken") + '&NodeID=' + nodeid;
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
            //OrgTree.selectNode(treeNode);
        }

        function OrgOnAsyncError(event, treeId, treeNode, XMLHttpRequest, textStatus, errorThrown) {
            console.log(treeNode);
        };

        function OrgOnClick(event, treeId, treeNode) {
            $scope.TargetNode = treeNode.id;
            OrgTree.selectNode(treeNode);
            OrgTree.expandNode(treeNode);
            $(".ztree li a").css({ "padding-left": "4px", "margin-left": "-4px" });
        }

        $scope.SearchOrgTree = function () {
            if ($scope.seachOrgNode == "" || $scope.seachOrgNode == undefined || $scope.seachOrgNode == null) {
                $scope.GetOrgTree();
            } else {
                var searchValue = $.trim($scope.seachOrgNode);
                var myUrl = storage.getItem("InterfaceUrl") + 'TreeNode.ashx?op=SearchOrgTree&accessToken=' + storage.getItem("userAdminToken") + '&Searchstr=' + encodeURI(searchValue);
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

    $scope.clearSearchOrgInput = function () {
        $scope.seachOrgNode = '';
        $scope.GetOrgTree();
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