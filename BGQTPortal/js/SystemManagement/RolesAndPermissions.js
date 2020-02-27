angular.module('app').controller('RPCtrl', function (AuthVerify, $scope, $http, $state, $stateParams, $location, $window) {
    $scope.ViewOrAdd = true;
    $scope.currentPage = 1;
    $scope.totalPage = 0;
    $scope.pSize = 10;
    $scope.maxSize = 5;
    $scope.searchtext = '';
    $scope.Lists = [];
    $scope.tabNum = 'tab1';
    $scope.SearchPhone = '';
    $scope.ControlLimit = '';
    $scope.ControlLimitLists = [{ "text": "全公司", "value": "1" }, { "text": "指定部门", "value": "2" }];
    $scope.SameOULists = [];
    $scope.RoleName = '';
    $scope.RoleList = [];
    $scope.ControlLimit = $scope.ControlLimitLists[0].value;
    RoleModuleLists = [];
    $scope.UserLists = [];
    $scope.currentAdminRoleLists = [];
    $scope.currentAdminRoleLists = JSON.parse(storage.getItem('currentAdminRoleObj'));
    console.log($scope.currentAdminRoleLists);
    $scope.SameOUID = '';
    $scope.searchRoleName = '';
    $scope.ControlLimitID = '';
    $scope.seachNode = '';
    $scope.seachOrgNode = '';
    $scope.SameLevelOuList = [];
    $scope.ControlLimitOuList = [];
    $scope.PageSizes = [{ "text": "10条/页", "value": "10" }, { "text": "20条/页", "value": "20" }, { "text": "50条/页", "value": "50" }, { "text": "100条/页", "value": "100" }];
    $scope.pSize = $scope.PageSizes[0].value;
    if ($window.innerWidth >= 1024) {
        console.log($window.innerHeight - 114);
        $(".content-left").css("height", $window.innerHeight - 84);
    } else {
        $(".content-left").css("height", $window.innerHeight - 4);
    }
    window.onresize = function () {
        $scope.$apply(function () {
            if ($window.innerWidth >= 1024) {
                console.log("hhhhh");
                $(".content-left").css("height", $window.innerHeight - 84);
            } else {
                $(".content-left").css("height", $window.innerHeight - 4);
            }
        })
    };

    //获取角色列表
    $scope.GetRoleList = function (currentPage) {
        $scope.currentPage = currentPage;
        $scope.ViewOrAdd = true;
        $scope.Lists = [];
        $scope.tabNum = 'tab1';
        //angular.element(".panel-heading").css("border-bottom", "none");
        $http({
            method: 'GET',
            url: storage.getItem("InterfaceUrl") + 'Role.ashx?op=GetRoleList&accessToken=' + storage.getItem("userAdminToken") + '&PageSize=' + $scope.pSize + '&CurPage=' + $scope.currentPage + '&Searchstr=' + encodeURI($scope.searchRoleName),
        }).then(function successCallback(response) {
            var data = response.data;
            if (data.result == "true") {
                var ListData = data.data.Lists;
                $scope.bigTotalItems = data.data.RecordCount;
                $scope.totalPage = data.data.PageCount;
                for (n = 0; n < ListData.length; n++) {
                    $scope.arr = {
                        "RoleID": ListData[n].RoleID,
                        "RoleName": ListData[n].RoleName,
                        "Count": ListData[n].Count,
                        "CreateTimeName": ListData[n].CreateTimeName,
                        "CreateUser": ListData[n].CreateUser,
                        "ControlLimit": ListData[n].ControlLimit,
                        "ControlLimitPath": ListData[n].ControlLimitPath,
                        "RoleList": ListData[n].RoleList,
                        "UserList": ListData[n].UserList,
                        "UserNameList": ListData[n].UserNameList,
                        "trueData": 1
                    };
                    $scope.Lists.push($scope.arr);
                }
                if (ListData.length < $scope.pSize) {
                    for (i = 0; i < $scope.pSize - ListData.length; i++) {
                        var nulltr = {
                            "$$hashKey": i,
                            "RoleID": "",
                            "RoleName": "",
                            "Count": "",
                            "CreateTimeName": "",
                            "CreateUser": "",
                            "ControlLimit": "",
                            "ControlLimitPath": "",
                            "RoleList": "",
                            "UserList": "",
                            "UserNameList": "",
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
        $scope.GetRoleList(currentPage);
    };

    //获取角色详情
    $scope.GetRoleInfo = function (id) {
        angular.element(".panel-heading").css("border-bottom", "1px solid #d8d8d8");
        $scope.ViewOrAdd = false;
        $scope.OpTitle = "编辑角色";
        var paramObj = {
            "RoleID": id
        }
        var CurrentPermission = [];
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'Role.ashx?op=GetRoleInfo&accessToken=' + storage.getItem("userAdminToken") + '&ID=' + id,
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            var ListData = response.data;
            if (ListData.result == "true") {
                $scope.ChangeRoleID = id;
                $scope.RoleName = ListData.data.RoleName;
                //for (var n = 0; n < $scope.ControlLimitLists.length; n++) {
                //    if ($scope.ControlLimitLists[n].value == ListData.data.ControlLimit) {
                //        $scope.ControlLimit = $scope.ControlLimitLists[n].value;
                //    }
                //}
                $scope.ControlLimitOuList = ListData.data.ControlLimitOuList;
                $scope.UserLists = ListData.data.UserList;
                console.log($scope.UserLists);
                for (var i = 0; i < ListData.data.RoleList.length; i++) {
                    for (var j = 0; j < ListData.data.RoleList[i].RoleParamList.length; j++) {
                        var arrObj = {
                            "ParamID": ListData.data.RoleList[i].RoleParamList[j].ParamID
                        }
                        CurrentPermission.push(arrObj);
                    }
                }
                $scope.SameLevelOuList = ListData.data.SameLevelOuList;
                $scope.GetRoleModuleList('edit', CurrentPermission);
                $scope.GetOrgTree();
            } else {
                ErrorHandling(ListData.result, ListData.errCode, ListData.errMsg);
            }
        }, function errorCallback(e) {

        });
    }
    
    //获取所有权限列表
    $scope.GetRoleModuleList = function (op, CurrentPermission) {
        $scope.op = op;
        console.log(CurrentPermission);
        $scope.PermissionLists = [];
        $scope.RoleParamList = [];
        $http({
            method: 'GET',
            url: storage.getItem("InterfaceUrl") + 'Role.ashx?op=GetRoleModuleList&accessToken=' + storage.getItem("userAdminToken"),
        }).then(function successCallback(response) {
            var ListData = response.data;
            if (ListData.result == "true") {
                for (var i = 0; i < ListData.data.length; i++) {
                    for (var j = 0; j < ListData.data[i].RoleParamList.length; j++) {
                        //权限默认选中和未选中
                        if (op == 'edit') {
                            for (var n = 0; n < CurrentPermission.length; n++) {
                                if (ListData.data[i].RoleParamList[j].ParamID == CurrentPermission[n].ParamID) {
                                    ListData.data[i].RoleParamList[j].checked = true;
                                    break;
                                } else {
                                    ListData.data[i].RoleParamList[j].checked = false;
                                }
                            }
                        } else {
                            ListData.data[i].RoleParamList[j].checked = false;
                        }
                        //当前管理员能创建角色的权限
                        for (var m = 0; m < $scope.currentAdminRoleLists.length; m++) {
                            if (ListData.data[i].RoleParamList[j].ParamID == $scope.currentAdminRoleLists[m].ParamID) {
                                ListData.data[i].RoleParamList[j].disabled = false;
                                break;
                            } else {
                                ListData.data[i].RoleParamList[j].disabled = true;
                            }
                        }
                    }
                    var arr1 = {
                        "ModuleTypeNum": "ModuleType" + ListData.data[i].ModuleType,
                        "ModuleType": ListData.data[i].ModuleType,
                        "ModuleTypeName": ListData.data[i].ModuleTypeName,
                        "RoleParamList": ListData.data[i].RoleParamList,
                        "checked": false,
                        "disabled": false
                    };
                    $scope.PermissionLists.push(arr1);

                }
                console.log($scope.PermissionLists);
                
            } else {
                ErrorHandling(ListData.result, ListData.errCode, ListData.errMsg);
            }
        }, function errorCallback(e) {

        });
    }

    //添加管理范围
    $scope.gotoAddControlLimitOu = function () {
        $('#ControlLimitOu_Modal').modal('show');
        $scope.seachOrgNode = '';
        $scope.nomalModal();
        $scope.GetOrgTree();
    }

    $scope.AddControlLimitOu = function () {
        $("#ControlLimitOu_Modal").modal("hide");
        var nodes = OrgTree.getCheckedNodes(true);
        for (var i = 0; i < nodes.length; i++) {
            var arrObj = { "OuID": nodes[i].id, "OUdistinguishedName": nodes[i].Path };
            $scope.ControlLimitOuList.push(arrObj);
            if ($scope.ControlLimitOuList.length != 0) {
                for (var j = 0; j < $scope.ControlLimitOuList.length; j++) {
                    for (var k = j + 1; k < $scope.ControlLimitOuList.length; k++) {
                        if ($scope.ControlLimitOuList[k].OuID == $scope.ControlLimitOuList[j].OuID) {
                            $scope.ControlLimitOuList.splice(k, 1);
                            }
                        }
                    }
                }
        }
        console.log($scope.ControlLimitOuList);
    }

    //移除
    $scope.removeControlLimitOu = function (tableid) {
        for (var i in $scope.ControlLimitOuList) {
            if ($scope.ControlLimitOuList[i].OuID == tableid) {
                $scope.ControlLimitOuList.splice(i, 1);
            }
        }
        console.log($scope.ControlLimitOuList);
    }

    //添加成员
    $scope.gotoAddMember = function () {
        $('#chooseUser_Modal').modal('show');
        $scope.seachNode = '';
        $scope.nomalModal();
        $scope.GetCompanyTree();
    }

    $scope.AddMembers = function () {
        $("#chooseUser_Modal").modal("hide");
        var nodes = CompanyTree.getCheckedNodes(true);
        for (var i = 0; i < nodes.length; i++) {
            if (nodes[i].Type == 2) {
                var arrObj = { "UserID": nodes[i].id, "DisplayName": nodes[i].Name, "UserAccount": nodes[i].UserAccount };
                $scope.UserLists.push(arrObj);
                if ($scope.UserLists.length != 0) {
                    for (var j = 0; j < $scope.UserLists.length; j++) {
                        for (var k = j + 1; k < $scope.UserLists.length; k++) {
                            if ($scope.UserLists[k].UserID == $scope.UserLists[j].UserID) {
                                $scope.UserLists.splice(k, 1);
                            }
                        }
                    }
                }
            }
        }
        console.log($scope.UserLists);
    }

    //移除成员
    $scope.removeThis = function (tableid) {
        for (var i in $scope.UserLists) {
            if ($scope.UserLists[i].UserID == tableid) {
                $scope.UserLists.splice(i, 1);
            }
        }
        console.log($scope.UserLists);
    }

    //添加角色
    $scope.gotoAddRole = function () {
        angular.element(".panel-heading").css("border-bottom", "1px solid #d8d8d8");
        $scope.ViewOrAdd = false;
        RoleModuleLists = [];
        $scope.UserLists = [];
        $scope.ControlLimitOuList = [];
        $scope.seachOrgNode = '';
        $scope.SameLevelOuList = [];
        $scope.GetOrgTree();
        $scope.OpTitle = "添加角色";
        $scope.GetRoleModuleList('add', '');
    }

    $scope.AddRole = function () {
        RoleModuleLists = [];
        $("input.chk_1:checked").each(function (j) {
            if (j >= 0) {
                RoleModuleLists.push({ "ParamID": $(this).attr("ParamID") });
            }
        })
        var nodes = OrgTree.getCheckedNodes(true);
        for (var i = 0; i < nodes.length; i++) {
            var arrObj = { "OuID": nodes[i].id};
            $scope.ControlLimitOuList.push(arrObj);
            if ($scope.ControlLimitOuList.length != 0) {
                for (var j = 0; j < $scope.ControlLimitOuList.length; j++) {
                    for (var k = j + 1; k < $scope.ControlLimitOuList.length; k++)
                        if ($scope.ControlLimitOuList[j].OuID == $scope.ControlLimitOuList[k].OuID) {
                            $scope.ControlLimitOuList.splice(k, 1);
                        }
                }
            }
        }
        
        if ($scope.RoleName == '') {
            ErrorHandling('false', 0, '请输入角色名称。');
            return;
        }
        if ($scope.ControlLimitOuList.length == 0) {
            ErrorHandling('false', 0, '请选择管理范围。');
            return;
        }
        if (RoleModuleLists.length == 0) {
            ErrorHandling('false', 0, '请给角色授予权限。');
            return;
        }
        console.log(RoleModuleLists);
        if ($scope.SameOUID != '') {
            var paramObj = {
                "RoleName": $scope.RoleName,
                "ControlLimit": 2,
                "ControlLimitOuList": $scope.ControlLimitOuList,
                "RoleList": [{ "RoleParamList": RoleModuleLists }],
                "UserList": $scope.UserLists,
                "SameLevelOuList": $scope.SameLevelOuList
            };
        } else {
            var paramObj = {
                "RoleName": $scope.RoleName,
                "ControlLimit": 2,
                "ControlLimitOuList": $scope.ControlLimitOuList,
                "RoleList": [{ "RoleParamList": RoleModuleLists }],
                "UserList": $scope.UserLists,
            };
        }
        console.log(paramObj);
        angular.element(".preloader").css("display", "block");
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'Role.ashx?op=AddRole&accessToken=' + storage.getItem("userAdminToken"),
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            angular.element(".preloader").css("display", "none");
            var callbackObj = response.data;
            ErrorHandling(callbackObj.result, callbackObj.errCode, callbackObj.errMsg);
            if (callbackObj.result == "true") {
                $scope.GetRoleList(1);
            } 
        }, function errorCallback(e) {

        });
    }
    
    //编辑角色
    $scope.ChangeRole = function () {
        RoleModuleLists = [];
        $("input.chk_1:checked").each(function (j) {
            if (j >= 0) {
                RoleModuleLists.push({ "ParamID": $(this).attr("ParamID") });
            }
        })
        //var nodes = OrgTree.getCheckedNodes(true);
        //for (var i = 0; i < nodes.length; i++) {
        //    var arrObj = { "OuID": nodes[i].id };
        //    $scope.ControlLimitOuList.push(arrObj);
        //    if ($scope.ControlLimitOuList.length != 0) {
        //        for (var j = 0; j < $scope.ControlLimitOuList.length; j++) {
        //            for (var k = j + 1; k < $scope.ControlLimitOuList.length; k++)
        //                if ($scope.ControlLimitOuList[j].OuID == $scope.ControlLimitOuList[k].OuID) {
        //                    $scope.ControlLimitOuList.splice(k, 1);
        //                }
        //        }
        //    }
        //}
        console.log(RoleModuleLists);
        if ($scope.RoleName == '') {
            ErrorHandling('false', 0, '请输入角色名称。');
            return;
        }
        if ($scope.ControlLimitOuList.length == 0) {
            ErrorHandling('false', 0, '请选择管理范围。');
            return;
        }
        if (RoleModuleLists.length == 0) {
            ErrorHandling('false', 0, '请给角色授予权限。');
            return;
        }
        var paramObj = {
            "RoleID": $scope.ChangeRoleID,
            "RoleName": $scope.RoleName,
            "ControlLimit": $scope.ControlLimit,
            "ControlLimitOuList": $scope.ControlLimitOuList,
            "RoleList": [{ "RoleParamList": RoleModuleLists }],
            "UserList": $scope.UserLists,
            "SameLevelOuList": $scope.SameLevelOuList
        };
        console.log(paramObj);
        angular.element(".preloader").css("display", "block");
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'Role.ashx?op=ChangeRole&accessToken=' + storage.getItem("userAdminToken"),
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            angular.element(".preloader").css("display", "none");
            var callbackObj = response.data;
            ErrorHandling(callbackObj.result, callbackObj.errCode, callbackObj.errMsg);
            if (callbackObj.result == "true") {
                $scope.GetRoleList(1);
            } 
        }, function errorCallback(e) {

        });
    }

    //删除角色
    $scope.delRoleModal = function (id) {
        $("#delRole_Modal").modal("show");
        $scope.nomalModal();
        $scope.delRoleID = id;
    }
    $scope.DelRole = function () {
        var paramObj = {
            "RoleID": $scope.delRoleID
        };
        $scope.operateProgress();
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'Role.ashx?op=DeleteRole&accessToken=' + storage.getItem("userAdminToken"),
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            var callbackObj = response.data;
            if (callbackObj.result == "true") {
                $scope.showMsg('success', false, callbackObj.errMsg, 1);
                $scope.showModalFoot = false;
                $scope.GetRoleList(1);
            } else {
                $scope.showMsg('error', false, callbackObj.errMsg, 1);
                $scope.showModalFoot = true;
            }
        }, function errorCallback(e) {

        });
    }

    //组织架构树（这里供添加成员使用）
    $scope.GetCompanyTree = function () {
        selectTree = false;
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
            
        }

        function zTreeOnAsyncError(event, treeId, treeNode, XMLHttpRequest, textStatus, errorThrown) {
            console.log(treeNode);
        };

        function CTOnClick(event, treeId, treeNode) {
            $(".ztree li a").css({ "padding-left": "4px", "margin-left": "-4px" });
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

    $scope.checkWay = function ($event) {
        var targetID = $(event.target).attr("RoleLevel");
        var targetChecked = $event.target.checked;
        $scope.SameOULists = [];
        if (targetID == 'ModuleType8' && targetChecked) {
            $('#SameOU_Modal').modal('show');
            $scope.nomalModal();
            $('#SameOUInput').attr('value','');
            $scope.SameLevelOuList = [];
            $scope.SameOUID = 'aaa';
            selectTree = true;
            $http({
                method: 'post',
                url: storage.getItem("InterfaceUrl") + 'Role.ashx?op=GetSameLevelOuList&accessToken=' + storage.getItem("userAdminToken"),
            }).then(function successCallback(response) {
                var callbackObj = response.data;
                if (callbackObj.result == "true") {
                    for (var i = 0; i < callbackObj.data.length; i++) {
                        var sameouObj = { "id": callbackObj.data[i].ID, "pId": 0, "name": callbackObj.data[i].SamelevelOuPath };
                        $scope.SameOULists.push(sameouObj);
                    }
                    var setting = {
                        check: {
                            enable: true,
                            chkboxType: { "Y": "", "N": "" }
                        },
                        view: {
                            dblClickExpand: false
                        },
                        data: {
                            simpleData: {
                                enable: true
                            }
                        },
                        callback: {
                            beforeClick: SameOUBeforeClick,
                            onCheck: SameOUOnCheck
                        }
                    };
                    $.fn.zTree.init($("#SameOUTree"), setting, $scope.SameOULists);
                } else {
                    $scope.showMsg('error', false, callbackObj.errMsg, 1);
                    $scope.showModalFoot = true;
                }
            }, function errorCallback(e) {

            });
        }
    }

    $scope.showSameOUContent = function () {
        $("#SameOUContent").css("display", "block");
    }

    $scope.hideSameOUContent = function () {
        $("#SameOUContent").css("display", "none");
    }

    function SameOUBeforeClick(treeId, treeNode) {
        SameOUTree = $.fn.zTree.getZTreeObj("SameOUTree");
        SameOUTree.checkNode(treeNode, !treeNode.checked, null, true);
        return false;
    }

    function SameOUOnCheck(e, treeId, treeNode) {
        SameOUTree = $.fn.zTree.getZTreeObj("SameOUTree"),
            nodes = SameOUTree.getCheckedNodes(true),
            v = "";
        for (var i = 0, l = nodes.length; i < l; i++) {
            v += nodes[i].name + ",";
        }
        if (v.length > 0) v = v.substring(0, v.length - 1);
        var SameOUInputObj = $("#SameOUInput");
        SameOUInputObj.attr("value", v);
    }

    // OU 树
    $scope.GetOrgTree = function () {
        selectTree = false;
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
                        Path: data[0].Path,
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
                            enable: true,//开启树的checkbox，也可以设置为radio
                            autoCheckTrigger: true,
                            chkboxType: { "Y": "", "N": "" }
                        },
                        callback: {
                            onClick: OrgOnClick, //点击节点时 回调,
                            onRightClick: OrgOnRightClick,
                            onAsyncSuccess: OrgOnAsyncSuccess,
                            onAsyncError: OrgOnAsyncError
                        }
                    };
                    OrgTree = $.fn.zTree.init($("#OrgTree"), setting, obj);
                } else {
                    $scope.showMsg('error', false, data.errMsg, 1);
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
           // OrgTree.expandNode(treeNode, true, false, true, true);
        }

        function OrgOnAsyncError(event, treeId, treeNode, XMLHttpRequest, textStatus, errorThrown) {
            console.log(treeNode);
        };

        function OrgOnClick(event, treeId, treeNode) {
            $scope.ControlLimitID = treeNode.id;
            $(".ztree li a").css({ "padding-left": "4px", "margin-left": "-4px" });
            OrgTree.selectNode(treeNode);
            OrgTree.expandNode(treeNode);
        }

        function OrgOnRightClick(event, treeId, treeNode) {
            $scope.ControlLimitID = treeNode.id;
            $(".ztree li a").css({ "padding-left": "4px", "margin-left": "-4px" });
            OrgTree.selectNode(treeNode);
            OrgTree.expandNode(treeNode);
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
                                    enable: true,//开启树的checkbox，也可以设置为radio
                                    autoCheckTrigger: true,
                                    chkboxType: { "Y": "", "N": "" }
                                },
                                callback: {
                                    onClick: OrgOnClick, //点击节点时 回调,
                                    onAsyncSuccess: OrgOnAsyncSuccess,
                                    onAsyncError: OrgOnAsyncError
                                }
                            };
                            OrgTree = $.fn.zTree.init($("#OrgTree"), setting, obj);//初始化
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

    $scope.closeModalBtn = function () {
        $(".modal").modal("hide");
        $scope.SameLevelOuList = [];
        var nodes = SameOUTree.getCheckedNodes(true);
        for (var i = 0, l = nodes.length; i < l; i++) {
            var sameouObj = { "ID": nodes[i].id };
            $scope.SameLevelOuList.push(sameouObj);
        }
        for (var j = 0; j < $scope.SameLevelOuList.length; j++) {
            for (var k = j + 1; k < $scope.SameLevelOuList.length; k++) {
                if ($scope.SameLevelOuList[k].ID == $scope.SameLevelOuList[j].ID) {
                    $scope.SameLevelOuList.splice(k, 1);
                }
            }
        }
        console.log($scope.SameLevelOuList);
    }

    $scope.changePageSize = function () {
        $scope.GetRoleList(1);
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
        $scope.searchRoleName = '';
        $scope.GetRoleList(1);
    }
});