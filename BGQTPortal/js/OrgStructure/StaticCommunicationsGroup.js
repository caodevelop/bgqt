angular.module('app').controller('SCGCtrl', function (AuthVerify, $scope, $http, $state, $stateParams, $location, $window) {
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
    $scope.Admins = [];
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
   
    $scope.GetStaticGroupList = function (currentPage) {
        $scope.currentPage = currentPage;
        $scope.Lists = [];
        $scope.tabNum = 'tab1';
        //angular.element(".panel-heading").css("border-bottom", "none");
        angular.element(".preloader").css("display", "block");
        $http({
            method: 'GET',
            url: storage.getItem("InterfaceUrl") + 'StaticGroup.ashx?op=GetStaticGroupList&accessToken=' + storage.getItem("userAdminToken") + '&PageSize=' + $scope.pSize + '&CurPage=' + $scope.currentPage + '&Searchstr=' + encodeURI($scope.Searchstr),
        }).then(function successCallback(response) {
            angular.element(".preloader").css("display", "none");
            var data = response.data;
            if (data.result == "true") {
                var ListData = data.data.Lists;
                $scope.bigTotalItems = data.data.RecordCount;
                $scope.totalPage = data.data.PageCount;
                for (n = 0; n < ListData.length; n++) {
                    $scope.arr = {
                        "ID": ListData[n].GroupID,
                        "Account": ListData[n].Account,
                        "DisplayName": ListData[n].DisplayName,
                        "Account": ListData[n].Account,
                        "GroupObj": ListData[n].DisplayName + "(" + ListData[n].Account + ")",
                        "Admins": ListData[n].Admins,
                        "AdminsName": ListData[n].AdminsName,
                        "AdminsCount": ListData[n].AdminsCount,
                        "Members": ListData[n].Members,
                        "Type": ListData[n].Type,
                        "trueData": 1
                    };
                    $scope.Lists.push($scope.arr);
                }
                if (ListData.length < $scope.pSize) {
                    for (i = 0; i < $scope.pSize - ListData.length; i++) {
                        var nulltr = {
                            "$$hashKey": i,
                            "ID": "",
                            "Account": "",
                            "DisplayName": "",
                            "Account":"",
                            "GroupObj": "",
                            "Admins": "",
                            "AdminsName": "",
                            "AdminsCount":"",
                            "Members": "",
                            "Type": "",
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
        $scope.GetStaticGroupList(currentPage);
    };
    //编辑弹出框
    $scope.gotoStaticGroupModal = function (id) {
        $("#chooseAdmin_Modal").modal("show");
        $scope.nomalModal();
        $scope.GetCompanyTree(); 
        $scope.ChangeStaticGroupID = id;
        $scope.Admins = [];
        $scope.seachNode = "";
        var paramObj = {
            "GroupID": id
        }
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'StaticGroup.ashx?op=GetStaticGroupInfo&accessToken=' + storage.getItem("userAdminToken") + '&ID=' + id,
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            var ListData = response.data;
            if (ListData.result == "true") {
                $scope.Admins = ListData.data.Admins;
            } else {
                ErrorHandling(ListData.result, ListData.errCode, ListData.errMsg);
            }
        }, function errorCallback(e) {

            });
        
    }
  
    //添加管理员
    $scope.AddAdmins = function (node) {
        var nodes;
        if (node != '' && node != undefined) {
            nodes = node;
        } else {
            nodes = CompanyTree.getSelectedNodes()[0];
        }
        if (nodes.Type == 2) {
            var arrObj = { "UserID": nodes.id, "DisplayName": nodes.Name, "UserAccount": nodes.UserAccount != "" ? nodes.UserAccount : "-" };
            $scope.Admins.push(arrObj);
            for (i = 0; i < $scope.Admins.length; i++) {
                for (x = i + 1; x < $scope.Admins.length; x++) {
                    if ($scope.Admins[i].UserID == $scope.Admins[x].UserID) {
                        $scope.Admins.splice(x, 1);
                    }
                }
            }
        }
        $scope.$applyAsync();
        console.log($scope.Admins);
    }
    //移除管理员
    $scope.removeThis = function (tableid) {
        for (var i in $scope.Admins) {
            if ($scope.Admins[i].UserID == tableid) {
                $scope.Admins.splice(i, 1);
            }
        }
        console.log($scope.Admins);
    }
    //编辑
    $scope.ChangeStaticGroupInfo = function () {
        if ($scope.Admins.length == 0) {
            $scope.showMsg('error', false, '请添加静态通讯组管理员。', '');
            return;
        }
        var paramObj = {
            "GroupID": $scope.ChangeStaticGroupID,
            "Admins": $scope.Admins
        };
        $scope.operateProgress();
        $http({
            method: 'post',
            url: storage.getItem("InterfaceUrl") + 'StaticGroup.ashx?op=ChangeStaticGroupInfo&accessToken=' + storage.getItem("userAdminToken"),
            data: JSON.stringify(paramObj)
        }).then(function successCallback(response) {
            var callbackObj = response.data;
            if (callbackObj.result == "true") {
                $scope.showMsg('success', false, callbackObj.errMsg, 1);
                $scope.showModalFoot = false;
                $scope.GetStaticGroupList(1);
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
                            autoCheckTrigger: true,
                            chkboxType: { "Y": "", "N": "" }
                        },
                        callback: {
                            onClick: CTOnClick, //点击节点时 回调,
                            onAsyncSuccess: zTreeOnAsyncSuccess,
                            onAsyncError: zTreeOnAsyncError,
                            onDblClick: zTreeOnDblClick
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
            //CompanyTree.selectNode(treeNode);
        }

        function zTreeOnAsyncError(event, treeId, treeNode, XMLHttpRequest, textStatus, errorThrown) {
            console.log(treeNode);
        };

        function CTOnClick(event, treeId, treeNode) {
            CompanyTree.selectNode(treeNode);
            CompanyTree.expandNode(treeNode);
            $(".ztree li a").css({ "padding-left": "4px", "margin-left": "-4px" });
        }

        function zTreeOnDblClick(event, treeId, treeNode) {
            $scope.AddAdmins(treeNode);
            $(".ztree li a").css({ "padding-left": "4px", "margin-left": "-4px" });
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
                                    enable: false,//开启树的checkbox，也可以设置为radio
                                    autoCheckTrigger: true,
                                    chkboxType: { "Y": "", "N": "" }
                                },
                                callback: {
                                    onClick: CTOnClick, //点击节点时 回调,
                                    onAsyncSuccess: zTreeOnAsyncSuccess,
                                    onAsyncError: zTreeOnAsyncError,
                                    onDblClick: zTreeOnDblClick
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

    $scope.changePageSize = function () {
        $scope.GetStaticGroupList(1);
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
        $scope.GetStaticGroupList(1);
    }
});