//修改实例信息        
app.directive('editinfo', function () {
    return {
        //元素
        restrict: 'E',
        //html
        template:
            '<div class="modal fade" id="EditInfoModal" tabindex="-1" role="dialog" aria-labelledby="EditInfoModalLabel" aria-hidden="true">' +
            '<div class="modal-dialog">' +
            '<div class="modal-content">' +
            '<div class="modal-header">' +
            '<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>' +
            '<h4 class="modal-title">修改实例信息</h4>' +
            '</div>' +
            '<div class="modal-body">' +
            '<div class="errorMsg" ng-model="errorMsg" ng-hide="checkTips" ng-class="opResult">' +
            '{{errorMsg}}' +
            '</div>' +
            '<div class="modalForm">' +
            '<div class="form-group" style="margin-bottom: 12px;width:360px;">'+
            '< input type="text" id="treeTxt" ng-model="searchInput" class="form-control search_user" placeholder="搜索" ng-change="searchUserStr(searchInput)" />'+
            '<a class= "clear_search" id="clear_search1" href="javascript:void(0);" ></a> '+
            '</div> ' +
            '<div class="form-group treeHide" style="margin-bottom: 0px;width:360px;display:inline-block;vertical-align:top;" ng-init="GetNodeCounts()">'+
            '<ul class= "ztree scroll_bar" id = "AddressBook" style="margin: 0; padding: 9px 12px; height: 318px; overflow: auto;border:1px solid #d8d8d8;" ></ul> '+
            '</div> ' +
            '</div>' +
            '</div>' +
            '<div class="modal-footer" ng-show="showModalFoot">' +
            '<button type="button" class="btn btn-theme modalBtn" id="editBtn" ng-click="editInfo()" id="editBtn">确 定</button>' +
            '<div class="loading"><span></span><span></span><span></span></div>' +
            '</div>' +
            '</div>' +
            '</div>' +
            '</div>',
        //替换
        replace: true,
        //link函数
        link: function ($scope) {

        }
    }
});