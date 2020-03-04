!function ($, app, win) {
    app.errorFn = function (jqXhr, textStatus, errorThrown, ajaxObj) {
        var code = jqXhr.status;
        if (ajaxObj.url && (ajaxObj.url == page.server.deleteFile)) {
            page.server.deleteFile.onError();
            $().prompt({ type: 'error', msg: win.currLanguageDic['netWork_problem'], isAutoHide: 0 });
        }
        if (ajaxObj.url && (ajaxObj.url == page.server.recycleDelete)) {
            page.server.recycleDelete.onError();
            $().prompt({ type: 'error', msg: win.currLanguageDic['netWork_problem'], isAutoHide: 0 });
        }
        if (ajaxObj.url && (ajaxObj.url == page.server.recycleEmpty)) {
            page.server.recycleEmpty.onError();
            $().prompt({ type: 'error', msg: win.currLanguageDic['netWork_problem'], isAutoHide: 0 });
        }
        if (ajaxObj.url && (ajaxObj.url == page.server.recycleRecover)) {
            page.server.recycleRecover.onError();
            $().prompt({ type: 'error', msg: win.currLanguageDic['netWork_problem'], isAutoHide: 0 });
        }
        if (ajaxObj.url && (ajaxObj.url == page.server.deleteShare)) {
            page.server.deleteShare.onError();
            $().prompt({ type: 'error', msg: win.currLanguageDic['netWork_problem'], isAutoHide: 0 });
        }
        if (ajaxObj.url && (ajaxObj.url == page.server.share)) {
            page.server.share.onError();
            $().prompt({ type: 'error', msg: win.currLanguageDic['netWork_problem'], isAutoHide: 0 });
        }

        if (code === 0 || code === 200) return;
        if (code === 401 || code === 403) {
            var defaultUri = app.global.defaultUri;
            if (defaultUri) {
                $().prompt({
                    type: 'info',
                    msg: win.currLanguageDic['sessionExpire'],
                    backgroundSetting: { info: '#E0F4FF' }
                });
                setTimeout(function () {
                    window.location.assign(defaultUri);
                }, 2000);
            } else {
                $().prompt({
                    type: 'info',
                    msg: '未配置登录页',
                    backgroundSetting: { info: '#E0F4FF' }
                });
            }
        } else {
            //console.error("服务器错误：" + code + "，" + errorThrown);
        }
    };

    var page = app.initPage({
        pageInit: function () {
            var _ = this,
                opts = _.opts;

            page.eventName = 'keydown';
            if (window.ribbon) {
                page.eventName = 'keypress';
            }
            _.isShareing = false;
            _.hidesignoutParams = _.hidesignout(app.request['hidesignout']);
            _.bindDomEvents();
            _.initFiles();
            _.scrolling();
            _.initSearch();
            _.mainFileSort(); //排序
            _.mainRecycleSort();
            _.initRemoteDate();
            page.linkFun.initShare(); //分享链接
            $('#shareLink_filterFile').val(7);
            page.recycleFun.initRecycle(); //回收站
            page.recycleFun.initRecycleSearch();
            opts.searchTxt.attr('placeholder', win.currLanguageDic['fileShare_input']);
            opts.recycleTxt.attr('placeholder', win.currLanguageDic['fileShare_input']);
            //opts.aFSdTime = opts.adShareSeleteDate.calendarChoose().calendarChoose;
            opts.aFSdTime = { jq: opts.adShareSeleteDate };
            //console.log(opts.aFSdTime);
            page.prompt = {
                infoBackground: '#E0F4FF'
            };
            if (page.isQuotaOver == false) {
                page.skyDriveUse();
            }
            win.currEmailAddr = app.request['userName'];

            _.customDateTitle(win.currLanguageDic['advancedShare_custom_date_title']);


            var mFBtnUpload = $('#m_f_btnUpload');
            //1、判断是否是ribbon插件
            // window.ribbon = 1;
            //2、判断是否支持html5上传
            window.h5Upload = (!!window['Blob'] && !!window['FileReader']);

            // window.h5Upload = !window.h5Upload;
            //3、加载不同上传逻辑
            if (window.ribbon && !window.h5Upload) {
                var silverLightHtml = '<object id="silverlightObj" data="data:application/x-silverlight-2," type="application/x-silverlight-2" width="100%" height="100%">\
                                    <param name="source" value="/ClientBin/Sl.MailApp.Uploader.xap" />\
                                    <param name="onLoad" value="onSilverlightLoad" />\
                                    <param name="onError" value="onSilverlightError" />\
                                    <param name="background" value="white" />\
                                    <param name="minRuntimeVersion" value="5.0.61118.0" />\
                                    <param name="autoUpgrade" value="true" />\
                                    <a id="silverlightErrorImg" href="http://go.microsoft.com/fwlink/?LinkID=149156&v=5.0.61118.0" style="text-decoration: none;">\
                                    <img src="../images/slmedallion_enu.png" alt="获取 Microsoft Silverlight" style="border-style: none;width: 200px;height: 150px;margin-top:30px;margin-left:30px;margin-bottom: 10px;"  />\
                                    <span id="uninstalled_silverlight">' + win.currLanguageDic['uninstalled_silverlight'] + '</span>\
                                    </a>\
                                 </object>';
                mFBtnUpload.html(silverLightHtml);
            } else {
                $('#m_upload_h5').show();
            }

            $(document).ready(function () {
                page.widthJudge();
                // 浏览器拖动事件
                $(window).resize(function () {
                    page.widthJudge();
                });
            });
        },
        hidesignout: function (val) {
            if (val && val == '1') {
                $('#m_signOut').hide();
            }
            return val;
        },
        initRemoteDate: function () {
            var _ = this, opts = _.opts;
            _.server.getServerTime.get(function (json) {
                _.advancedFileShare.serverDateStr = json.data;
            });
        },
        bindDomEvents: function () {
            var _ = this,
                opts = _.opts;

            opts.headIcon.hover(function () {
                $(this).addClass('icon_background');
                opts.iconTxt.addClass('content_display');
                $('#m_f_btnUpload').hide();
            }, function () {
                $(this).removeClass('icon_background');
                opts.iconTxt.removeClass('content_display');
            });
            opts.iconTxt.hover(function () {
                opts.headIcon.addClass('icon_background');
                $(this).addClass('content_display');

            }, function () {
                opts.headIcon.removeClass('icon_background');
                $(this).removeClass('content_display');
                $('#m_f_btnUpload').show();
            });

            opts.adShareSeleteDate.on('keyup.t', function (e) {
                e = e || window.event;
                var $this = $(this), curVal = $this.val();

                curVal = $this.val($this.val().substr(0, 10)).val();
                var flag = page.advancedFileShare.inspectDate(curVal);
                if (curVal.length >= 10) {
                    e.preventDefault();
                    e.stopPropagation();
                    // curVal = $this.val($this.val().substr(0, 10)).val();
                    // var flag = page.advancedFileShare.inspectDate(curVal);
                    page.opts.adShareSeleteDate.css('border-color', flag ? '#999999' : '#fd5454');

                    page.advancedFileShare.shareOkBtnDisable(!flag);

                } else {
                    if (e.keyCode === 8) {
                        $this.css('border-color', '#999999');

                        page.advancedFileShare.shareOkBtnDisable(!flag);
                    }
                }
            });

            opts.adShareSeleteDate.on('blur', function (e) {
                e = e || window.event;
                var flag = page.advancedFileShare.inspectDate(opts.adShareSeleteDate.val());
                page.opts.adShareSeleteDate.css('border-color', flag ? '#999999' : '#fd5454');

                page.advancedFileShare.shareOkBtnDisable(!flag);
            });
        },
        initFiles: function () { //我的文件
            var _ = this,
                opts = _.opts;
            if (!page.fileParams.isLoading) {
                page.fileParams.isLoading = true;
                page.fileParams.loadParams.pageIndex = 1;
                page.fileParams.isMoreFiles = true;
                _.getFiles(function (data) {
                    var files = data.files;
                    if (files.length == 0) {
                        page.isHaveFiles(false);
                        opts.myFiles.show();
                        ko.mapping.fromJS([], _.files);
                    } else {
                        page.isHaveFiles(true);
                        opts.myFiles.hide();
                        ko.mapping.fromJS(files, _.files);
                    }
                    page.fileParams.isLoading = false;

                });
            }
        },
        getFiles: function (callback) { //我的文件
            var _ = this;
            this.server.files.get({
                pageIndex: page.fileParams.loadParams.pageIndex,
                pageSize: page.fileParams.loadParams.pageSize,
                orderbyField: page.fileParams.loadParams.orderbyField,
                orderbyType: page.fileParams.loadParams.orderbyType
            }, function (json) {
                if ($.isFunction(callback)) {
                    var files = json.data.files;
                    _.timeShift(files);
                    callback(json.data);
                    $('.m_file_download').html(win.currLanguageDic['fileDownload']);
                    $('.m_file_rename').html(win.currLanguageDic['fileRename']);
                    $('.m_file_delete').html(win.currLanguageDic['fileRemove']);
                }
            })
        },
        scrolling: function (item) { //瀑布流
            var _ = this,
                opts = _.opts;
            opts.search.on('scroll', function () {
                var elementHeight = opts.search.prop('scrollHeight');
                var scrollTop = opts.search.scrollTop();
                var height = opts.search.height();
                if (Math.round(scrollTop + height) >= elementHeight) {
                    if (!page.fileParams.isLoading && page.fileParams.isMoreFiles) {
                        page.fileParams.isLoading = true;
                        page.fileParams.loadParams.pageIndex++;
                        opts.loadingMore.show();
                        _.getFiles(function (data) {
                            if (page.fileParams.loadParams.pageIndex >= data.pageCount) {
                                page.fileParams.isMoreFiles = false;
                            }
                            var files = data.files;
                            for (var i = 0; i < files.length; i++) {
                                page.files.push(ko.mapping.fromJS(files[i]));
                            }
                            opts.loadingMore.hide();
                            page.fileParams.isLoading = false;
                        })
                    }
                }
            });
        },
        moreClick: function (data, event) { //点击更多
            var e = window.event || event;
            window.event ? e.cancelBubble = true : e.stopPropagation();
            var more = $(event.target);
            more.siblings('.m_file_option').show();
            more.addClass('more_checked');
            var pageY = event.pageY;
            var height = $(window).height() - $('.l_main_footer').height();
            var menuHeight = $('.m_file_option_more').height();
            var rowHeight = $('.m_file_box').height();
            //判断重命名框的位置
            if (height - pageY > menuHeight) {
                $('.m_file_option_more').css('top', '30px');
            } else {
                $('.m_file_option_more').css('top', '-88px');
            }
            var menuHeight1 = $('.m_recycle_option_more').height();
            //判断重命名框的位置
            if (height - pageY > menuHeight1) {
                $('.m_recycle_option_more').css('top', '30px');
            } else {
                $('.m_recycle_option_more').css('top', '-60px');
            }
            $(event.target).on('mouseout', function () {
                $(this).siblings('.m_file_option').hide();
            });
            $(event.target).siblings('.m_file_option').mouseenter(function () {
                $(this).show();
            });
            $(event.target).siblings('.m_file_option').mouseleave(function () {
                more.removeClass('more_checked');
                $(this).hide();
            });
        },
        timeShift: function (files, searchKeyWord) { //时间转换
            for (var i = 0; i < files.length; i++) {
                var tempData = files[i];
                var data = app.parseDate(tempData['LastUpdateTime']);
                // var data = new Date(tempData['LastUpdateTime']);
                if (!isNaN(data.getTime())) {
                    //tempData['UploadTime'] = app.formatDate(data);
                    var nowDate = app.formatDate(new Date());
                    ymDate = app.formatDate(data);
                    if (ymDate == nowDate) {
                        hDate = app.formatDate(data, 'hh:mm');
                        tempData['LastUpdateTime'] = hDate;
                    } else {
                        tempData['LastUpdateTime'] = app.formatDate(data);
                    }
                } else {
                    tempData['LastUpdateTime'] = '';
                }

                tempData['isChecked'] = false;
                tempData['isRename'] = false;

                if (searchKeyWord) {
                    var reg = new RegExp(searchKeyWord, 'gi');
                    var fileName = tempData['FileName'];
                    var regResults = reg.exec(fileName);
                    if (regResults) {
                        for (var j = 0; j < regResults.length; j++) {
                            fileName = fileName.replace(regResults[j], '<font class="hightClass">' + regResults[j] + '</font>');
                        }
                    }
                    tempData['hightFileName'] = fileName;
                }
            }
        },
        searchFiles: function (inputVal) { //搜索结果
            var _ = this,
                opts = _.opts;
            opts.resultMore.empty();
            page.server.search.get({
                keyword: inputVal, //查询关键字
                Top: 20 //默认显示多少条
            }, function (json) {
                var files = json.data.files,
                    recordCount = json.data.recordCount;
                page.isCheckAllSearch(false);
                if (files.length == 0) {
                    page.isHaveSearch(false);
                    opts.reSearch.show();
                    ko.mapping.fromJS([], _.search);
                } else {
                    page.isHaveSearch(true);
                    opts.reSearch.hide();
                    if (recordCount > 20) {
                        opts.resultMore.html(win.currLanguageDic['share_search_title']);
                    }
                    _.timeShift(files, inputVal);
                    ko.mapping.fromJS(files, _.search);
                }
                _.opts.search.scrollTop(0);
                opts.searchIcon.removeClass('icon_showtoggle');
                opts.resultWrap.slideDown();
                $('.m_file_download').html(win.currLanguageDic['fileDownload']);
                $('.m_file_rename').html(win.currLanguageDic['fileRename']);
                $('.m_file_delete').html(win.currLanguageDic['fileRemove']);
            })
        },
        initSearch: function () {
            var _ = this,
                opts = _.opts;
            opts.searchBtn.on('click.t', function (e) {
                var num = 0;
                for (var i = 0; i < page.files().length; i++) {
                    if (page.files()[i].isChecked() == true) {
                        num++;
                    }
                }
                if (num == 0) {
                    page.deselect();
                }
                var inputVal = $.trim(opts.searchTxt.val());
                opts.searchTxt.val($.trim(opts.searchTxt.val()));
                if (!inputVal) {
                    opts.searchTxt.val('');
                    return;
                } else {
                    if (/^[\u4e00-\u9fa5a-zA-Z0-9]+$/gi.test(inputVal)) {
                        opts.resultMore.html('');
                        page.showSearchResult(true);
                        opts.seachResultTitle.text(inputVal);
                        _.searchFiles(inputVal);
                    } else {
                        $().prompt({ type: 'warning', msg: win.currLanguageDic['share_search_key'] });
                    }
                }
            });

            //input进入焦点
            opts.searchTxt.focus(function () {
                opts.searchTxt.addClass('input_click');
            });
            //input失去焦点并判断input中是否有内容
            opts.searchTxt.blur(function () {
                var inputVal = $.trim(opts.searchTxt.val());
                if (!inputVal) {
                    opts.searchTxt.removeClass('input_click');
                }
            });
            //搜索键盘enter事件
            opts.searchTxt.off(page.eventName).on(page.eventName, function (event) {
                event = event || win.event;
                var keyCode = (event.which ? event.which : event.keyCode)
                if (keyCode == 13) {
                    opts.searchBtn.trigger('click.t');
                }
            });

            //清除搜索结果
            opts.clear.on('click.t', function () {
                var num = 0;
                for (var i = 0; i < page.files().length; i++) {
                    if (page.files()[i].isChecked() == true) {
                        num++;
                    }
                }
                if (num == 0) {
                    page.deselect();
                }
                opts.reSearch.hide();
                opts.searchTxt.val('');
                opts.seachResultTitle.text('');
                opts.searchTxt.removeClass('input_click');
                opts.resultMore.html('');
                page.showSearchResult(false);
                ko.mapping.fromJS([], _.search);
                page.isCheckAllSearch(false);
            })
        }
    });
    page.formatDate = function (action) { //日期
        var now = new Date(),
            dateInfo = {
                y: now.getFullYear(),
                m: now.getMonth(),
                d: now.getDate()
            },
            date = new Date();
        switch (action) {
            case 1:
                date = new Date(dateInfo.y, dateInfo.m, dateInfo.d + 1);
                break;
            case 2:
                date = new Date(dateInfo.y, dateInfo.m, dateInfo.d + 7);
                break;
            case 3:
                date = new Date(dateInfo.y, dateInfo.m + 1, dateInfo.d);
                break;
            case 4:
                date = new Date(dateInfo.y + 1, dateInfo.m, dateInfo.d);
                break;
        }
        return app.formatDate(date);
    };
    page.logOutFn = function () {
        app.cookie('username', '', {
            expires: -1,
            path: app.global.cookieRootPath
        });
        app.cookie('key', '', {
            expires: -1,
            path: app.global.cookieRootPath
        });
        win.location.assign(app.global.defaultUri);
    };
    page.logout = function () { //注销
        page.server.logOff.post(function () {
            page.logOutFn();
        });
    };
    page.downloadSingleFile = function (data, event) {
        //下载单个文件
        var e = window.event || event;
        window.event ? e.cancelBubble = true : e.stopPropagation();
        page.server.getdownloadhashvalue.get({ fileid: data.ID() }, function (json) {

            var nativeOpen, frame = document.getElementById('downloadFrame');
            if (!frame) {
                frame = document.createElement('iframe');
                frame.id = 'downloadFrame';
                frame.style.display = 'none';
                document.body.appendChild(frame);
            }

            nativeOpen = frame.contentWindow.open;
            if (nativeOpen) {
                window.open = nativeOpen;
            }

            var downloadFileHash = encodeURIComponent(json.data);
            var downloadUrl = win.location.protocol + "//" + win.location.host + page.server.singleFileDownload + '?downloadhashvalue=' + downloadFileHash;

            if (win.navigator.userAgent.indexOf('MSIE') !== -1
                || win.navigator.userAgent.indexOf('rv:11.0') !== -1
                || win.navigator.userAgent.indexOf('Edge') !== -1
                || (win.navigator.userAgent.indexOf('Safari') !== -1 && win.navigator.userAgent.indexOf('Chrome') === -1)
            ) {
                var flag = false;
                try {
                    var href = top.window.location.href;
                } catch (e) {
                    flag = true;
                }
                if (flag) {
                    if (win.navigator.userAgent.indexOf('Safari') !== -1 && win.navigator.userAgent.indexOf('Chrome') === -1) {
                        var str = win.location.protocol + "//" + win.location.host + page.server.singleFileDownload;
                        var form = $('<form>').attr('action', str);

                        var input = $('<input/>');
                        input.attr('name', 'downloadhashvalue');
                        input.val(downloadFileHash).appendTo(form);

                        var decodeInput = $('<input />');
                        decodeInput.attr('name', 'decode');
                        decodeInput.val('1').appendTo(form);

                        form.appendTo('body');
                        form.submit().remove();
                    } else {
                        downloadUrl = downloadUrl;
                        window.open(downloadUrl);
                    }
                    /*
                     var str = win.location.protocol + "//" + win.location.host + page.server.singleFileDownload;
                     var form = $('<form>').attr('action',str);

                     var input = $('<input/>');
                     input.attr('name','downloadhashvalue');
                     input.val(downloadFileHash).appendTo(form);

                     var decodeInput = $('<input />');
                     decodeInput.attr('name','decode');
                     decodeInput.val('1').appendTo(form);

                     form.appendTo('body');
                     form.submit().remove();*/
                } else {
                    window.open(downloadUrl);
                }
            } else {
                window.open(downloadUrl);
            }
        });
    };
    page.renameClick = function (data, event) { //我的文件重命名
        var e = window.event || event;
        window.event ? e.cancelBubble = true : e.stopPropagation();
        if (data.isRename() == true) { //判断是否在重命名
            return;
        }
        $('.m_file_more').removeClass('more_checked');
        $('.m_file_option').hide();
        var fileBox = $(event.target).parents('.m_file_box');
        var divName = fileBox.find('.m_file_name');
        var divP = fileBox.find('p');
        var oldTxt = divP.text();
        var tempInput = $('<p data-bind="text:FileName">' + '<input class="renameInput" type="text" />' + '<i class="nameSuffix"></i>' + '<span class="renameOk"></span>' + '<span class="renameCancel"></span>' + '</p>');
        oldTxt = oldTxt.replace(data.ExtensionName(), '');
        divName.append(tempInput);
        var divInput = divName.find('input');
        divInput.val(oldTxt);
        divP.hide();
        divInput.focus();
        divInput.select();
        var renameOk = divName.find('.renameOk');
        var renameCancel = divName.find('.renameCancel');
        data.isRename(true);
        $('.nameSuffix').html(data.ExtensionName());

        //input框取消冒泡
        divInput.on('click', function (event) {
            var e = window.event || event;
            window.event ? e.cancelBubble = true : e.stopPropagation();
        });
        //重命名回车
        divInput.off(page.eventName).on(page.eventName, function (event) {
            event = event || win.event;
            var keyCode = (event.which ? event.which : event.keyCode);
            if (keyCode == 13) {
                renameOk.trigger('click.t');
            }
        });
        //确定重命名
        renameOk.on('click.t', function (event) {
            var e = window.event || event;
            window.event ? e.cancelBubble = true : e.stopPropagation();
            var newVal = $.trim(divInput.val());
            var fileNamePattern = new RegExp('[/\\\\:\*\?"<>\|]');
            if (fileNamePattern.test(newVal)) {
                $().prompt({ type: 'warning', msg: win.currLanguageDic['renameFile_special'] });
                return;
            }
            if (!newVal) {
                $().prompt({ type: 'warning', msg: win.currLanguageDic['renameFile_empty'] });
                return;
            }
            if (newVal.length > 255) {
                $().prompt({ type: 'warning', msg: win.currLanguageDic['renameFile_exceed'] });
                return;
            }
            // newVal = newVal + data.ExtensionName();
            if (newVal == oldTxt) {
                renameCancel.trigger('click.t');
            } else {
                page.server.rename.post({
                    fileID: data.ID(),
                    newName: newVal + data.ExtensionName()
                }, function (json) {
                    if (json.data) {
                        data.FileName(json.data.FileName);
                        tempInput.remove();
                        divP.text(json.data.FileName).show();
                        data.isRename(false);
                        $().prompt({
                            type: 'info',
                            msg: win.currLanguageDic['renameFile_ok'],
                            backgroundSetting: { info: page.prompt.infoBackground }
                        });
                        page.totalRefresh();
                    } else {
                        $().prompt({ type: 'error', msg: win.currLanguageDic['renameFile_failed'], isAutoHide: 0 });
                    }
                });
            }

        });
        //取消重命名
        renameCancel.on('click.t', function (event) {
            var e = window.event || event;
            window.event ? e.cancelBubble = true : e.stopPropagation();
            data.isRename(false);
            tempInput.remove();
            divP.show();
        });

        $(document).on('click.t', function (e) {
            renameCancel.trigger('click.t');
        });
        $('.m_file_box').on('click.t', function (e) {
            renameCancel.trigger('click.t');
        });
        $('.m_file_more').on('click.t', function () {
            renameCancel.trigger('click.t');
        });
        $('.m_file_checkbox').on('click.t', function () {
            renameCancel.trigger('click.t');
        });
        $('.search_box_more').on('click.t', function () {
            renameCancel.trigger('click.t');
        });
    };
    page.deleteClick = function (data, event) { //我的文件删除
        $('.m_file_more').removeClass('more_checked');
        $('.m_file_option').hide();
        var e = window.event || event;
        window.event ? e.cancelBubble = true : e.stopPropagation();
        page.server.deleteFile.post({
            fileID: data.ID()
        }, function (json) {
            $().prompt({
                type: 'info',
                msg: win.currLanguageDic['deleteFile_ok'],
                backgroundSetting: { info: page.prompt.infoBackground }
            });
            page.totalRefresh();
            page.deselect();
        });
    };
    page.deleteSearch = function (data, event) { //搜索结果删除
        var e = window.event || event;
        window.event ? e.cancelBubble = true : e.stopPropagation();
        $('.search_box_more').removeClass('more_checked');
        $('.m_file_option').hide();
        page.server.deleteFile.post({
            fileID: data.ID()
        }, function (json) {
            if (json.data == true) {
                $().prompt({
                    type: 'info',
                    msg: win.currLanguageDic['deleteFile_ok'],
                    backgroundSetting: { info: page.prompt.infoBackground }
                });
                page.totalRefresh();
                page.deselect();
            } else {
                $().prompt({ type: 'error', msg: win.currLanguageDic['deleteFile_failed'], isAutoHide: 0 });
            }
        });
    };
    page.checkList = function (data, event) { //点击checkBox改变isChecked的状态
        var e = window.event || event;
        window.event ? e.cancelBubble = true : e.stopPropagation();
        if (data.isRename()) {
            return;
        }
        data.isChecked(!data.isChecked());
        page.toolBar();
        page.isCheckedAll();
    };
    page.checkOne = function (data, event) { //我的文件选中一个
        var e = window.event || event;
        window.event ? e.cancelBubble = true : e.stopPropagation();
        if (data.isRename()) {
            return;
        }
        page.deselect();
        data.isChecked(true);
        page.toolBar();
    };
    page.searchCheck = function (data, event) { //搜索结果选中一个
        var e = window.event || event;
        window.event ? e.cancelBubble = true : e.stopPropagation();

        page.deselect();
        data.isChecked(true);
        page.toolBar();
    };
    page.isCheckedAll = function () {
        var searchFlag = true;
        var filesFlag = true;
        for (var i = 0; i < page.search().length; i++) {
            if (page.search()[i].isChecked() == false) {
                searchFlag = false;
            }
        }
        for (var i = 0; i < page.files().length; i++) {
            if (page.files()[i].isChecked() == false) {
                filesFlag = false;
            }
        }
        page.isCheckAllSearch(searchFlag);
        page.isCheckAllFiles(filesFlag);
    };
    page.checkAllSearchBox = function () { //搜索文件全选
        page.isCheckAllSearch(!page.isCheckAllSearch());
        if (page.isCheckAllSearch()) {
            for (var i = 0; i < page.search().length; i++) {
                page.search()[i].isChecked(true);
            }
            page.showDeleteDiv(true);
            page.shareBg(true);
        } else {
            for (var i = 0; i < page.search().length; i++) {
                page.search()[i].isChecked(false);
            }
            var flag = false;
            for (var i = 0; i < page.files().length; i++) {
                if (page.files()[i].isChecked() == true) {
                    flag = true;
                }
            }
            page.showDeleteDiv(flag);
            page.shareBg(false);
        }
    };
    page.checkAllFilesBox = function () { //我的文件全选
        page.isCheckAllFiles(!page.isCheckAllFiles());
        if (page.isCheckAllFiles() == true) {
            for (var i = 0; i < page.files().length; i++) {
                page.files()[i].isChecked(true);
            }
            page.showDeleteDiv(true);
            page.shareBg(true);
        } else {
            for (var i = 0; i < page.files().length; i++) {
                page.files()[i].isChecked(false);
            }
            var flag = false;
            for (var i = 0; i < page.search().length; i++) {
                if (page.search()[i].isChecked() == true) {
                    flag = true;
                }
            }
            page.showDeleteDiv(flag);
            page.shareBg(false);
        }
    };
    page.batchDelete = function () { //批量删除
        page.shareSet(true);
        page.opts.batchDelete.animate({
            height: 185
        });
    };
    page.deleteOk = function () {
        if (page.shareBatch() == false) {
            return;
        }
        page.shareBatch(false);
        $().prompt({
            type: 'info',
            isAutoHide: 0,
            msg: win.currLanguageDic['dealing_prompt'],
            backgroundSetting: { info: page.prompt.infoBackground }
        });
        var arr = [];
        for (var i = 0; i < page.search().length; i++) {
            if (page.search()[i].isChecked() == true) {
                arr.push(page.search()[i].ID());
            }
        }
        for (var i = 0; i < page.files().length; i++) {
            if (page.files()[i].isChecked() == true) {
                arr.push(page.files()[i].ID());
            }
        }
        page.server.deleteFile.post({
            fileID: arr.join()
        }, function (json) {
            page.shareBatch(true);
            page.deleteClose();
            $('.promptMain').hide();
            $().prompt({
                type: 'info',
                msg: win.currLanguageDic['deleteFile_ok'],
                backgroundSetting: { info: page.prompt.infoBackground }
            });
            page.totalRefresh();
            page.deselect();

        })
    };
    page.deleteClose = function () {
        if (page.shareBatch() == false) {
            return;
        }
        page.opts.batchDelete.animate({
            height: 0
        }, function () {
            page.shareSet(false);
        });
    };
    page.maskClose = function () {
        if (page.shareBatch() == false
            || page.recycleFun.recyleBatch() == false
            || page.recycleFun.recyleRecover() == false
            || page.recycleFun.recyleEmpty() == false
            || page.linkFun.LinkCancel() == false
            || page.advancedFileShare.shareCancelBtnDisable()) {
            return;
        }
        page.deleteClose();
        page.advancedFileShare.close();
        page.recycleFun.emptyClose();
        page.recycleFun.recoverClose();
        page.recycleFun.reDeleteClose();
        page.linkFun.deleteShareCancel();
    };
    page.settings = function () { //设置
        var _ = this,
            opts = _.opts;
        var uping = 0;

        if (page.tasksFileUnUploaded) {
            uping = page.tasksFileUnUploaded();
        }
        page.titleDiv.change(2);

        opts.uploading.hide();
        $('#language_change').val(win.language);
        page.langBtn(false);
        page.opts.languageBtn.removeClass('txtHover');

        page.server.getCurrentUserInfo.get(function (json) {
            var data = json.data;
            opts.spaceAll.html(data.TotalSize);
            opts.spaceUsed.html(data.UseSize);
            opts.roundMiddle.html(data.Percentage + '%');
            var QuotaPer = Math.round(data.Percentage * 3.6);
            opts.roundMiddle.css('color', '#0173c7');
            opts.roundRight.css('background-color', '#0173c7');
            if (QuotaPer >= 0 && QuotaPer <= 180) {
                $('.space_round_top1').remove();
                var divDom = '<div class="space_round_top"></div>';
                opts.spaceRound.append(divDom);
                $('.space_round_top').css('transform', 'rotate(' + QuotaPer + 'deg)');
            }
            if (QuotaPer > 180 && QuotaPer <= 360) {
                $('.space_round_top').remove();
                var divDom = '<div class="space_round_top1"></div>';
                opts.spaceRound.append(divDom);
                $('.space_round_top1').css('transform', 'rotate(' + QuotaPer + 'deg)');
                if (QuotaPer > 324) {
                    $('.space_round_top1').css('background-color', '#cc3300');
                    opts.roundRight.css('background-color', '#cc3300');
                    opts.roundMiddle.css('color', '#cc3300');
                }
            }
            var showName = '';
            if (data.DisplayName == '') {
                showName = data.MailAddress;
            } else {
                showName = data.DisplayName;//用户名
            }
            opts.userId.attr('title', data.MailAddress).html(showName);
        });


        $('#language_change').change(function () {
            var Optselected = $('#language_change option:selected').val();
            if (win.language != Optselected) {
                page.langBtn(true);
                page.opts.languageBtn.addClass('txtHover');
                if (uping > 0) {
                    opts.uploading.show();
                } else {
                    opts.uploading.hide();
                }
            } else {
                page.opts.languageBtn.removeClass('txtHover');
                page.langBtn(false);
                opts.uploading.hide();
            }
        });
    };
    page.langClick = function () {
        var _ = this,
            opts = _.opts;
        if (page.langBtn()) {
            win.username = app.request['userName'] || '';
            win.language = $('#language_change option:selected').val();
            app.cookie('language', win.language, {
                expires: app.global.cookieExpire,
                path: app.global.cookieRootPath
            });

            if (_.hidesignoutParams && _.hidesignoutParams == '1') {
                win.location.assign('main.htm' + '?language=' + win.language + '&userName=' + win.username + '&hidesignout=' + _.hidesignoutParams);
            } else {
                win.location.assign('main.htm' + '?language=' + win.language + '&userName=' + win.username);
            }
        }
    };
    page.deselect = function () { //取消选定
        for (var i = 0; i < page.files().length; i++) {
            page.files()[i].isChecked(false);
        }
        for (var i = 0; i < page.search().length; i++) {
            page.search()[i].isChecked(false);
        }

        page.showDeleteDiv(false);
        page.shareBg(false);
        page.isCheckAllFiles(false);
        page.isCheckAllSearch(false);
    };
    page.toolBar = function () { //toolBar的top
        var flag = false, i;
        for (i = 0; i < page.files().length; i++) {
            if (page.files()[i].isChecked() == true) {
                flag = true;
            }
        }
        for (i = 0; i < page.search().length; i++) {
            if (page.search()[i].isChecked() == true) {
                flag = true;
            }
        }
        page.showDeleteDiv(flag);
        page.shareBg(flag);
    };
    page.totalRefresh = function () { //总刷新
        var _ = this,
            opts = _.opts;
        page.initFiles();
        var titleTxt = opts.seachResultTitle.text();
        if (titleTxt) {
            page.searchFiles(titleTxt);
            opts.searchTxt.addClass('input_click');
        } else {
            opts.searchTxt.removeClass('input_click');
        }
        opts.searchTxt.val(opts.seachResultTitle.text());
        page.deselect();
        opts.fileIcon.removeClass('icon_showtoggle');
        opts.searchIcon.removeClass('icon_showtoggle');

        opts.resultWrap.slideDown();

        if (opts.fileWrap.children().length == 0) {
            opts.myFiles.slideDown();
        } else {
            opts.fileWrap.slideDown();
        }
        _.opts.search.scrollTop(0);
    };
    page.fileIconClick = function () { //我的文件展开折叠
        var _ = this,
            opts = _.opts;

        opts.fileIcon.toggleClass('icon_showtoggle');

        if (opts.fileWrap.children().length == 0) {
            opts.myFiles.slideToggle();
        } else {
            opts.fileWrap.slideToggle();
        }
    };
    page.searchIconClick = function () { //搜索文件展开折叠
        var _ = this,
            opts = _.opts;
        opts.searchIcon.toggleClass('icon_showtoggle');

        if (opts.resultWrap.children().length == 0) {
            opts.reSearch.slideToggle();
        } else {
            opts.resultWrap.slideToggle();
        }
    };

    page.widthJudge = function () {
        width = $(window).width();//获取当前窗口宽度
        if (width < 400) {
            page.shareLinkMenu(false);
            page.recycleBinMenu(false);
            page.settingMenu(false);
        } else if (width >= 400 && width < 500) {
            page.shareLinkMenu(true);
            page.recycleBinMenu(false);
            page.settingMenu(false);
        } else if (width >= 500 && width < 600) {
            page.shareLinkMenu(true);
            page.recycleBinMenu(true);
            page.settingMenu(false);
        } else {
            page.shareLinkMenu(true);
            page.recycleBinMenu(true);
            page.settingMenu(true);
        }
    };

    page.recycleFun = {  //回收站
        showRecycleDiv: ko.observable(false),
        recycleToolBar: ko.observable(false),
        showRecycleSearch: ko.observable(false),
        recycleBg: ko.observable(false),
        isCheckRecycleFiles: ko.observable(false),
        isCheckRecycleSearch: ko.observable(false),
        isRecycleSearch: ko.observable(false),
        isRecycleFile: ko.observable(false),
        recyleBatch: ko.observable(true),
        recyleRecover: ko.observable(true),
        recyleEmpty: ko.observable(true),
        recycleList: ko.mapping.fromJS([]),
        recycleSearch: ko.mapping.fromJS([]),

        recycleBin: function () { //回收站
            page.titleDiv.change(3);
            // page.recycleFun.initRecycle();
            // page.recycleFun.initRecycleSearch();
            page.recycleFun.recycleScroll();
            // page.recycleFun.clearSearch();
            // page.recycleFun.recycleDeselect();
            page.opts.recycle.scrollTop(0);
            page.opts.mUpload.addClass('minHeight');
        },
        clearSearch: function () {
            var _ = this,
                opts = _.opts;
            page.opts.noSearch.hide();
            page.opts.recycleTxt.val('');
            page.opts.recycleSearchTitle.text('');
            page.opts.recycleTxt.removeClass('input_click');
            page.opts.recycleExceed.html('');
            page.recycleFun.showRecycleSearch(false);
            ko.mapping.fromJS([], page.recycleFun.recycleSearch);
            page.recycleFun.isCheckRecycleSearch(false);
        },
        dataShift: function (files, searchKeyWord) { //时间转换
            for (var i = 0; i < files.length; i++) {
                var tempData = files[i];

                var data = app.parseDate(tempData['DeleteTime']);
                // var data = new Date(tempData['DeleteTime']);
                if (!isNaN(data.getTime())) {
                    var nowDate = app.formatDate(new Date());
                    ymDate = app.formatDate(data);
                    if (ymDate == nowDate) {
                        hDate = app.formatDate(data, 'hh:mm');
                        tempData['DeleteTime'] = hDate;
                    } else {
                        tempData['DeleteTime'] = app.formatDate(data);
                    }
                } else {
                    tempData['DeleteTime'] = '';
                }


                tempData['isChecked'] = false;
                tempData['isRename'] = false;

                tempData['RecycleExpireTime'] = win.currLanguageDic['recycleExpire_prefixion'] + tempData['DisplayExpireTime'] + win.currLanguageDic['recycleExpire_suffix'];

                tempData['FileNameTitle'] = tempData['FileName'] + ' -' + tempData['RecycleExpireTime'];

                if (searchKeyWord) {
                    var reg = new RegExp(searchKeyWord, 'gi');
                    var fileName = tempData['FileName'];
                    var regResults = reg.exec(fileName);
                    if (regResults) {
                        for (var j = 0; j < regResults.length; j++) {
                            fileName = fileName.replace(regResults[j], '<font class="hightClass">' + regResults[j] + '</font>');
                        }
                    }
                    tempData['hightFileName'] = fileName;
                }
            }
        },
        getRecycle: function (callback) {
            var _ = this;
            page.server.recycleList.get({
                pageIndex: page.recycleParams.loadParams.pageIndex,
                pageSize: page.recycleParams.loadParams.pageSize,
                orderbyField: page.recycleParams.loadParams.orderbyField,
                orderbyType: page.recycleParams.loadParams.orderbyType
            }, function (json) {
                if ($.isFunction(callback)) {
                    var files = json.data.files;
                    page.recycleFun.dataShift(files);
                    callback(json.data);
                    $('.m_recycle_recovery').html(win.currLanguageDic['m_recycle_recovery']);
                    $('.m_recycle_delete').html(win.currLanguageDic['m_recycle_delete']);
                }
            })
        },
        initRecycle: function () {
            var _ = this,
                opts = _.opts;
            if (!page.recycleParams.isLoading) {
                page.recycleParams.isLoading = true;
                page.recycleParams.loadParams.pageIndex = 1;
                page.recycleParams.isMoreFiles = true;
                page.recycleFun.getRecycle(function (data) {
                    var files = data.files;
                    if (files.length == 0) {
                        page.recycleFun.isRecycleFile(false);
                        page.opts.myRecycleFiles.show();
                        ko.mapping.fromJS([], page.recycleFun.recycleList);
                    } else {
                        page.recycleFun.isRecycleFile(true);
                        page.opts.myRecycleFiles.hide();
                        ko.mapping.fromJS(files, page.recycleFun.recycleList);
                    }
                    page.recycleParams.isLoading = false;
                });
            }
        },
        recycleScroll: function () {
            var _ = this,
                opts = _.opts;
            page.opts.recycle.on('scroll', function () {
                var elementHeight = page.opts.recycle.prop('scrollHeight');
                var scrollTop = page.opts.recycle.scrollTop();
                var height = page.opts.recycle.height();
                if (scrollTop + height == elementHeight) {
                    if (!page.recycleParams.isLoading && page.recycleParams.isMoreFiles) {
                        page.recycleParams.isLoading = true;
                        page.recycleParams.loadParams.pageIndex++;
                        page.opts.recycleMore.show();
                        page.recycleFun.getRecycle(function (data) {
                            if (page.recycleParams.loadParams.pageIndex >= data.pageCount) {
                                page.recycleParams.isMoreFiles = false;
                            }
                            var files = data.files;
                            for (var i = 0; i < files.length; i++) {
                                page.recycleFun.recycleList.push(ko.mapping.fromJS(files[i]));
                            }
                            page.opts.recycleMore.hide();
                            page.recycleParams.isLoading = false;
                        })
                    }
                }
            });
        },
        recycleSearchFiles: function (inputVal) { //回收站搜索结果
            var _ = this,
                opts = _.opts;
            page.opts.recycleExceed.empty();
            page.server.recycleSearch.get({
                keyword: inputVal, //查询关键字
                Top: 20 //默认显示多少条
            }, function (json) {
                var files = json.data.files,
                    recordCount = json.data.recordCount;
                page.recycleFun.isCheckRecycleSearch(false);
                if (files.length == 0) {
                    page.recycleFun.isRecycleSearch(false);
                    page.opts.noSearch.show();
                    ko.mapping.fromJS([], page.recycleFun.recycleSearch);
                } else {
                    page.recycleFun.isRecycleSearch(true);
                    page.opts.noSearch.hide();
                    if (recordCount > 20) {
                        page.opts.recycleExceed.html(win.currLanguageDic['share_search_title']);
                    }

                    page.recycleFun.dataShift(files, inputVal);
                    ko.mapping.fromJS(files, page.recycleFun.recycleSearch);
                }
                page.opts.recycle.scrollTop(0);
                page.opts.recycleSearchIcon.removeClass('icon_showtoggle');
                page.opts.recycleSearchWrap.slideDown();
                $('.m_recycle_recovery').html(win.currLanguageDic['m_recycle_recovery']);
                $('.m_recycle_delete').html(win.currLanguageDic['m_recycle_delete']);
            })
        },
        initRecycleSearch: function () {
            var _ = this,
                opts = _.opts;
            page.opts.recycleSearchBtn.off('click.t').on('click.t', function (e) {
                var num = 0;
                for (var i = 0; i < page.recycleFun.recycleList().length; i++) {
                    if (page.recycleFun.recycleList()[i].isChecked() == true) {
                        num++;
                    }
                }
                if (num == 0) {
                    page.recycleFun.recycleDeselect();
                }
                var inputVal = $.trim(page.opts.recycleTxt.val());
                page.opts.recycleTxt.val($.trim(page.opts.recycleTxt.val()));
                if (!inputVal) {
                    page.opts.recycleTxt.val('');
                    return;
                } else {
                    if (/^[\u4e00-\u9fa5a-zA-Z0-9]+$/gi.test(inputVal)) {
                        page.opts.recycleExceed.html('');
                        page.recycleFun.showRecycleSearch(true);
                        page.opts.recycleSearchTitle.text(inputVal);
                        page.recycleFun.recycleSearchFiles(inputVal);
                    } else {
                        $().prompt({ type: 'warning', msg: win.currLanguageDic['share_search_key'] });
                    }
                }
            });
            //input进入焦点
            page.opts.recycleTxt.focus(function () {
                page.opts.recycleTxt.addClass('input_click');
            });
            //input失去焦点并判断input中是否有内容
            page.opts.recycleTxt.blur(function () {
                var inputVal = $.trim(page.opts.recycleTxt.val());
                if (!inputVal) {
                    page.opts.recycleTxt.removeClass('input_click');
                }
            });
            //搜索键盘enter事件
            page.opts.recycleTxt.off(page.eventName).on(page.eventName, function (event) {
                event = event || win.event;
                var keyCode = (event.which ? event.which : event.keyCode)
                if (keyCode == 13) {
                    page.opts.recycleSearchBtn.trigger('click.t');
                }
            });
            //清除搜索结果
            page.opts.recycleClear.on('click.t', function () {
                var num = 0;
                for (var i = 0; i < page.recycleFun.recycleList().length; i++) {
                    if (page.recycleFun.recycleList()[i].isChecked() == true) {
                        num++;
                    }
                }
                if (num == 0) {
                    page.recycleFun.recycleDeselect();
                }
                page.recycleFun.clearSearch();
            })
        },
        recyclefileIcon: function () { //我的回收站展开折叠
            var _ = this,
                opts = _.opts;
            page.opts.recycleFileIcon.toggleClass('icon_showtoggle');
            if (page.opts.recycleWrap.children().length == 0) {
                page.opts.myRecycleFiles.slideToggle();
            } else {
                page.opts.recycleWrap.slideToggle();
            }
        },
        recycleSearchIcon: function () { //搜索文件展开折叠
            var _ = this,
                opts = _.opts;
            page.opts.recycleSearchIcon.toggleClass('icon_showtoggle');
            if (page.opts.recycleSearchWrap.children().length == 0) {
                page.opts.noSearch.slideToggle();
            } else {
                page.opts.recycleSearchWrap.slideToggle();
            }
        },
        recycleTop: function () { //recycle的toolBar
            var flag = false, i;
            for (i = 0; i < page.recycleFun.recycleList().length; i++) {
                if (page.recycleFun.recycleList()[i].isChecked() == true) {
                    flag = true;
                }
            }
            for (i = 0; i < page.recycleFun.recycleSearch().length; i++) {
                if (page.recycleFun.recycleSearch()[i].isChecked() == true) {
                    flag = true;
                }
            }
            page.recycleFun.recycleToolBar(flag);
            page.recycleFun.recycleBg(flag);
        },
        recycleCheckBox: function (data, event) { //点击checkBox改变isChecked的状态
            var e = window.event || event;
            window.event ? e.cancelBubble = true : e.stopPropagation();
            data.isChecked(!data.isChecked());
            page.recycleFun.recycleTop();
            page.recycleFun.isCheckedAllRecycle();
        },
        recycleFile: function (data, event) { //我的回收站选中一个
            var e = window.event || event;
            window.event ? e.cancelBubble = true : e.stopPropagation();
            page.recycleFun.recycleDeselect();
            data.isChecked(true);
            page.recycleFun.recycleTop();
        },
        recycleFind: function (data, event) { //搜索结果选中一个
            var e = window.event || event;
            window.event ? e.cancelBubble = true : e.stopPropagation();
            page.recycleFun.recycleDeselect();
            data.isChecked(true);
            page.recycleFun.recycleTop();
        },
        recycleDeselect: function () { //取消选定
            for (var i = 0; i < page.recycleFun.recycleList().length; i++) {
                page.recycleFun.recycleList()[i].isChecked(false);
            }
            for (var i = 0; i < page.recycleFun.recycleSearch().length; i++) {
                page.recycleFun.recycleSearch()[i].isChecked(false);
            }

            page.recycleFun.recycleToolBar(false);
            page.recycleFun.recycleBg(false);
            page.recycleFun.isCheckRecycleFiles(false);
            page.recycleFun.isCheckRecycleSearch(false);
        },
        recycleRefresh: function () { //回收站总刷新
            var _ = this,
                opts = _.opts;
            page.recycleFun.initRecycle();
            var titleTxt = page.opts.recycleSearchTitle.text();
            if (titleTxt) {
                page.recycleFun.recycleSearchFiles(titleTxt);
                page.opts.recycleTxt.addClass('input_click');
            } else {
                page.opts.recycleTxt.removeClass('input_click');
            }
            page.opts.recycleTxt.val(page.opts.recycleSearchTitle.text());
            page.recycleFun.recycleDeselect();
            page.opts.recycleFileIcon.removeClass('icon_showtoggle');
            page.opts.recycleSearchIcon.removeClass('icon_showtoggle');

            page.opts.recycleSearchWrap.slideDown();

            if (page.opts.recycleWrap.children().length == 0) {
                page.opts.myRecycleFiles.slideDown();
            } else {
                page.opts.recycleWrap.slideDown();
            }
            page.opts.recycle.scrollTop(0);
        },
        isCheckedAllRecycle: function () {
            var searchFlag = true;
            var filesFlag = true;
            for (var i = 0; i < page.recycleFun.recycleSearch().length; i++) {
                if (page.recycleFun.recycleSearch()[i].isChecked() == false) {
                    searchFlag = false;
                }
            }
            for (var i = 0; i < page.recycleFun.recycleList().length; i++) {
                if (page.recycleFun.recycleList()[i].isChecked() == false) {
                    filesFlag = false;
                }
            }
            page.recycleFun.isCheckRecycleSearch(searchFlag);
            page.recycleFun.isCheckRecycleFiles(filesFlag);
        },
        recycleSearchAllChecked: function () { //搜索文件全选
            page.recycleFun.isCheckRecycleSearch(!page.recycleFun.isCheckRecycleSearch());
            if (page.recycleFun.isCheckRecycleSearch()) {
                for (var i = 0; i < page.recycleFun.recycleSearch().length; i++) {
                    page.recycleFun.recycleSearch()[i].isChecked(true);
                }
                page.recycleFun.recycleToolBar(true);
                page.recycleFun.recycleBg(true);
            } else {
                for (var i = 0; i < page.recycleFun.recycleSearch().length; i++) {
                    page.recycleFun.recycleSearch()[i].isChecked(false);
                }
                var flag = false;
                for (var i = 0; i < page.recycleFun.recycleList().length; i++) {
                    if (page.recycleFun.recycleList()[i].isChecked() == true) {
                        flag = true;
                    }
                }
                page.recycleFun.recycleToolBar(flag);
                page.recycleFun.recycleBg(false);
            }
        },
        recycleFilesAllChecked: function () { //我的文件全选
            page.recycleFun.isCheckRecycleFiles(!page.recycleFun.isCheckRecycleFiles());
            if (page.recycleFun.isCheckRecycleFiles() == true) {
                for (var i = 0; i < page.recycleFun.recycleList().length; i++) {
                    page.recycleFun.recycleList()[i].isChecked(true);
                }
                page.recycleFun.recycleToolBar(true);
                page.recycleFun.recycleBg(true);
            } else {
                for (var i = 0; i < page.recycleFun.recycleList().length; i++) {
                    page.recycleFun.recycleList()[i].isChecked(false);
                }
                var flag = false;
                for (var i = 0; i < page.recycleFun.recycleSearch().length; i++) {
                    if (page.recycleFun.recycleSearch()[i].isChecked() == true) {
                        flag = true;
                    }
                }
                page.recycleFun.recycleToolBar(flag);
                page.recycleFun.recycleBg(false);
            }
        },
        emptyRecycle: function () { //清空回收站
            page.shareSet(true);
            page.opts.emptyRecycle.animate({
                height: 185
            });
        },
        emptyClose: function () {
            if (page.recycleFun.recyleEmpty() == false) {
                return;
            }
            page.opts.emptyRecycle.animate({
                height: 0
            }, function () {
                page.shareSet(false);
            });
        },
        emptyOk: function () {
            if (page.recycleFun.recyleEmpty() == false) {
                return;
            }
            page.recycleFun.recyleEmpty(false);
            $().prompt({
                type: 'info',
                isAutoHide: 0,
                msg: win.currLanguageDic['dealing_prompt'],
                backgroundSetting: { info: page.prompt.infoBackground }
            });
            page.server.recycleEmpty.post(function (json) {
                page.recycleFun.recyleEmpty(true);
                page.recycleFun.emptyClose();
                $('.promptMain').hide();
                $().prompt({
                    type: 'info',
                    msg: win.currLanguageDic['recycleEmpty_success'],
                    backgroundSetting: { info: page.prompt.infoBackground }
                });
                page.recycleFun.recycleRefresh();
                page.recycleFun.recycleDeselect();
            });
        },
        recycleDelete: function (data, event) { //回收站单个删除
            var e = window.event || event;
            window.event ? e.cancelBubble = true : e.stopPropagation();
            $('.m_file_more').removeClass('more_checked');
            $('.search_box_more').removeClass('more_checked');
            $('.m_file_option').hide();
            page.server.recycleDelete.post({
                fileID: data.FileID()
            }, function (json) {
                if (json.data == true) {
                    $().prompt({
                        type: 'info',
                        msg: win.currLanguageDic['recycleDelete_success'],
                        backgroundSetting: { info: page.prompt.infoBackground }
                    });
                    page.recycleFun.recycleRefresh();
                    page.recycleFun.recycleDeselect();
                }
            });
        },
        singleRecover: function (data, event) { //回收站单个恢复
            var e = window.event || event;
            window.event ? e.cancelBubble = true : e.stopPropagation();
            $('.m_file_more').removeClass('more_checked');
            $('.search_box_more').removeClass('more_checked');
            $('.m_file_option').hide();
            page.server.recycleRecover.post({
                fileID: data.FileID()
            }, function (json) {
                if (json.data == true) {
                    $().prompt({
                        type: 'info',
                        msg: win.currLanguageDic['recycleRecover_success'],
                        backgroundSetting: { info: page.prompt.infoBackground }
                    });
                    page.recycleFun.recycleRefresh();
                    page.recycleFun.recycleDeselect();
                }
            });
        },
        recoverClose: function () {
            if (page.recycleFun.recyleRecover() == false) {
                return;
            }
            page.opts.recoverRecycle.animate({
                height: 0
            }, function () {
                page.shareSet(false);
            });
        },
        recoverBtn: function () { //回收站批量恢复
            if (!page.recycleFun.recycleBg()) {
                return;
            }
            page.shareSet(true);
            page.opts.recoverRecycle.animate({
                height: 185
            });
        },
        recoverOk: function () {
            if (page.recycleFun.recyleRecover() == false) {
                return;
            }
            page.recycleFun.recyleRecover(false);
            $().prompt({
                type: 'info',
                isAutoHide: 0,
                msg: win.currLanguageDic['dealing_prompt'],
                backgroundSetting: { info: page.prompt.infoBackground }
            });
            page.server.recycleRecover.post({
                fileID: page.recycleFun.recycleIsChecked()
            }, function (json) {
                page.recycleFun.recyleRecover(true);
                page.recycleFun.recoverClose();
                $('.promptMain').hide();
                $().prompt({
                    type: 'info',
                    msg: win.currLanguageDic['recycleRecover_success'],
                    backgroundSetting: { info: page.prompt.infoBackground }
                });
                page.recycleFun.recycleRefresh();
                page.recycleFun.recycleDeselect();
            })
        },
        deleteRecycle: function () { //回收站批量删除
            if (!page.recycleFun.recycleBg()) {
                return;
            }
            page.shareSet(true);
            page.opts.deleteRecycle.animate({
                height: 185
            });
        },
        reDeleteOk: function () {
            if (page.recycleFun.recyleBatch() == false) {
                return;
            }
            page.recycleFun.recyleBatch(false);
            $().prompt({
                type: 'info',
                isAutoHide: 0,
                msg: win.currLanguageDic['dealing_prompt'],
                backgroundSetting: { info: page.prompt.infoBackground }
            });
            page.server.recycleDelete.post({
                fileID: page.recycleFun.recycleIsChecked()
            }, function (json) {
                page.recycleFun.recyleBatch(true);
                page.recycleFun.reDeleteClose();
                $('.promptMain').hide();
                $().prompt({
                    type: 'info',
                    msg: win.currLanguageDic['recycleDelete_success'],
                    backgroundSetting: { info: page.prompt.infoBackground }
                });
                page.recycleFun.recycleRefresh();
                page.recycleFun.recycleDeselect();
            })
        },
        reDeleteClose: function () {
            if (page.recycleFun.recyleBatch() == false) {
                return;
            }
            page.opts.deleteRecycle.animate({
                height: 0
            }, function () {
                page.shareSet(false);
                page.recycleFun.recyleBatch(true);
            });
        },
        recycleIsChecked: function () {
            var arr = [];
            for (var i = 0; i < page.recycleFun.recycleSearch().length; i++) {
                if (page.recycleFun.recycleSearch()[i].isChecked() == true) {
                    arr.push(page.recycleFun.recycleSearch()[i].FileID());
                }
            }
            for (var i = 0; i < page.recycleFun.recycleList().length; i++) {
                if (page.recycleFun.recycleList()[i].isChecked() == true) {
                    arr.push(page.recycleFun.recycleList()[i].FileID());
                }
            };
            return arr.join();
        }
    };

    page.linkFun = { //分享链接
        showLinkDiv: ko.observable(false),
        linkBg: ko.observable(false),
        LinkCancel: ko.observable(true),
        shareList: ko.mapping.fromJS([]),
        expandShare: ko.mapping.fromJS([]),

        link: function () {
            page.titleDiv.change(4);
            // page.opts.linkTxt.val('');
            // page.linkFun.initShare();
            page.opts.linkTxt.attr('placeholder', win.currLanguageDic['keyFilter']);
            // $('#shareLink_filterFile').val(7); 
            page.linkFun.bindDomEvents();
            // page.linkFun.linkBg(false);               
        },
        bindDomEvents: function () {
            var filterShareLinkTimeout;
            page.opts.linkTxt.off(page.eventName).on(page.eventName, function (event) {
                event = event || win.event;
                var keyCode = (event.which ? event.which : event.keyCode);
                if (filterShareLinkTimeout) {
                    clearTimeout(filterShareLinkTimeout);
                }
                filterShareLinkTimeout = setTimeout(function () {
                    var linkTxt = $.trim(page.opts.linkTxt.val());
                    // page.opts.linkTxt.val(linkTxt);
                    // var start = (new Date()).getTime();
                    var filterData = page.linkFun.dataShift(page.linkFun.linkFunSourceData, linkTxt);
                    // var end = (new Date()).getTime();
                    // console.log(end - start);
                    page.linkFun.bindData(filterData);
                    page.linkFun.linkBg(false);
                }, 800);
            });
        },
        linkMore: function (data, event) { //更多
            var e = window.event || event;
            window.event ? e.cancelBubble = true : e.stopPropagation();
            var more = $(event.target);
            var menuHeight;
            more.siblings('.shareLink_file_option').show();
            more.addClass('more_checked');
            var pageY = event.pageY;
            var height = $(window).height() - $('.l_main_footer').height();

            // var menuHeight = $('.file_option_more').height();
            var rowHeight = $('.m_file_box').height();

            //判断重命名框的位置
            if (data.isInvalidShare() == true) {
                menuHeight = $('.file_option_more').children().height() * 3;
                if (height - pageY > menuHeight) {
                    $('.file_option_more').css('top', '30px');
                } else {
                    $('.file_option_more').css('top', '-88px');
                }
            } else {
                menuHeight = $('.file_option_more').children().height();
                if (height - pageY > menuHeight) {
                    $('.file_option_more').css('top', '30px');
                } else {
                    $('.file_option_more').css('top', '-32px');
                }
            }

            more.on('mouseout', function () {
                $(this).siblings('.shareLink_file_option').hide();
            });
            more.siblings('.shareLink_file_option').mouseenter(function () {
                $(this).show();
            });
            more.siblings('.shareLink_file_option').mouseleave(function () {
                more.removeClass('more_checked');
                $(this).hide();
            });
        },
        fileExpand: function (data, event) {
            var e = window.event || event;
            window.event ? e.cancelBubble = true : e.stopPropagation();
            $(event.target).toggleClass('shareLink_expend');
            $(event.target).siblings('.shareLink_expand_content').toggle();
        },
        getShare: function (callback) {
            var selShareLinkDateFilterVal = $('#shareLink_filterFile').val();
            page.server.getShareList.get({
                daysoffilterdate: parseInt(selShareLinkDateFilterVal)
            }, function (json) {
                if ($.isFunction(callback)) {
                    var data = json.data;
                    page.linkFun.linkFunSourceData = data;
                    var inputShareLinkeSearchVal = page.opts.linkTxt.val();
                    var filterData = page.linkFun.dataShift(data, inputShareLinkeSearchVal);
                    callback(filterData);
                }
            })
        },
        initShare: function () { //分享链接刷新
            page.linkFun.getShare(function (data) {
                page.linkFun.bindData(data);
                page.linkFun.linkDeselect();
            });
        },
        dataShift: function (files, searchKeyWord) {
            var result = [];
            for (var i = 0; i < files.length; i++) {
                var tempData = files[i];
                var expireDate = app.parseDate('9999-01-01 00:00:00');

                var data = app.parseDate(tempData['ExpireTime']);

                tempData['shareExpireTime'] = tempData['ExpireTime'];
                if (!isNaN(data.getTime())) {
                    if (data >= expireDate) {
                        tempData['ExpireDate'] = win.currLanguageDic['shareLink_expire'];
                        tempData['shareExpireTime'] = win.currLanguageDic['shareLink_expire'];
                    } else {
                        tempData['ExpireDate'] = app.formatDate(data);
                        tempData['shareExpireTime'] = win.currLanguageDic['shareLink_ex'] + app.formatDate(data);
                    }
                } else {
                    tempData['ExpireDate'] = win.currLanguageDic['shareLink_expire'];
                    tempData['shareExpireTime'] = tempData['ExpireTime'];
                }

                var shareTime = app.parseDate(tempData['LastUpdateTime']);
                if (!isNaN(shareTime.getTime())) {
                    var nowDate = app.formatDate(new Date());
                    ymDate = app.formatDate(shareTime);
                    if (ymDate == nowDate) {
                        hDate = app.formatDate(shareTime, 'hh:mm:ss');
                        tempData['LastReviseTime'] = hDate;
                    } else {
                        tempData['LastReviseTime'] = app.formatDate(shareTime);
                    }
                } else {
                    tempData['LastReviseTime'] = '';
                }


                if (!tempData['ValidateCode']) {
                    tempData['ValidateCode'] = win.currLanguageDic['shareLink_noCode'];
                }

                if (tempData['AuthType'] == 1) {
                    tempData['AuthTypeDisplay'] = win.currLanguageDic['shareLink_isAuth'];
                    tempData['AuthTypeName'] = win.currLanguageDic['shareLink_isAuthType'];
                } else {
                    tempData['AuthTypeDisplay'] = win.currLanguageDic['shareLink_noAuth'];
                    tempData['AuthTypeName'] = win.currLanguageDic['shareLink_noAuthType'];
                }

                tempData['isChecked'] = false;
                tempData['isRename'] = false;
                tempData['isDeleteAllFiles'] = false;
                tempData['isInvalidShare'] = true;


                tempData['shortUrlCode'] = tempData['ShortUrl'].slice(tempData['ShortUrl'].length - 6, tempData['ShortUrl'].length);

                var serachFileNames = '',
                    fileTitle = [];
                for (var k = 0; k < tempData['Files'].length; k++) {
                    serachFileNames += ' ' + tempData['Files'][k]['FileName'];
                    tempData['Files'][k]['HtmlFileName'] = tempData['Files'][k]['FileName'];
                    fileTitle.push(tempData['Files'][k]['FileName']);

                    if (tempData['Files'][k]['FileStatus'] == 1) {
                        tempData['isDeleteAllFiles'] = true;
                    }
                }



                tempData['expandFiles'] = fileTitle;
                tempData['hightDisplayName'] = tempData['DisplayName'];

                if (tempData['isDeleteAllFiles'] == false) {
                    tempData['hightDisplayName'] = '<span style = "color:#990000;">' + '[' + win.currLanguageDic['isInvalid'] + '] ' + tempData['hightDisplayName'] + '</span>';
                    tempData['isInvalidShare'] = false;
                }

                if (searchKeyWord) {
                    var reg = new RegExp(searchKeyWord, 'gi');
                    var displayName = tempData['DisplayName'];
                    var regResults = reg.exec(displayName);

                    if (regResults) {
                        for (var j = 0; j < regResults.length; j++) {
                            displayName = displayName.replace(regResults[j], '<font class="hightClass">' + regResults[j] + '</font>');
                        }
                        tempData['hightDisplayName'] = displayName;
                        if (tempData['isDeleteAllFiles'] == false) {
                            tempData['hightDisplayName'] = '<span style = "color:#990000;">' + '[' + win.currLanguageDic['isInvalid'] + '] ' + tempData['hightDisplayName'] + '</span>';
                        }

                        if (reg.test(serachFileNames)) {
                            page.linkFun.searchKeyWorkFiles(tempData['Files'], searchKeyWord)
                        }

                        result.push(tempData);

                    } else {

                        if (reg.test(serachFileNames)) {
                            page.linkFun.searchKeyWorkFiles(tempData['Files'], searchKeyWord)
                            result.push(tempData);
                        }
                    }
                } else {
                    result.push(tempData);
                }


                var fileObj = [], file = tempData['Files'],
                    deleteFileObj = [];
                for (var a = 0; a < file.length; a++) {
                    if (file[a]['FileStatus'] == 0 || file[a]['FileStatus'] == 2) {
                        file[a]['FileNameObj'] = '<span style = "text-decoration:line-through;">' + file[a]['HtmlFileName'] + ' (' + win.currLanguageDic['deleteFileObj'] + ')</span>';
                        deleteFileObj.push(file[a]['FileNameObj']);
                    } else {
                        fileObj.push(file[a]['HtmlFileName']);
                    }
                }
                file = fileObj.concat(deleteFileObj);
                if (file.length > 5) {
                    file = file.splice(0, 5);
                    file[5] = '...';
                }
                tempData['expandShare'] = file;
            }

            return result;
        },
        searchKeyWorkFiles: function (files, searchKeyWord) {
            for (var k = 0; k < files.length; k++) {
                var objectName = files[k]['FileName'];
                var reg = new RegExp(searchKeyWord, 'gi');
                var regExpand = reg.exec(objectName);
                if (regExpand) {
                    for (var j = 0; j < regExpand.length; j++) {
                        objectName = objectName.replace(regExpand[j], '<font class="hightClass">' + regExpand[j] + '</font>');
                    }
                }
                files[k]['HtmlFileName'] = objectName;
            }
            return files;
        },
        bindData: function (data) {
            var result = data;
            if (result.length == 0) {
                page.opts.noLinkSearch.show();
            } else {
                page.opts.noLinkSearch.hide();
            }
            ko.mapping.fromJS(result, page.linkFun.shareList);
            $('.shareLink_again').html(win.currLanguageDic['shareLink_again']);
            $('.shareLink_rename').html(win.currLanguageDic['shareLink_rename']);
            $('.shareLink_cancel').html(win.currLanguageDic['shareLink_cancel']);
            $('.shareLink_ex').html(win.currLanguageDic['shareLink_ex']);
            $('.expand_expire').html(win.currLanguageDic['expand_expire']);
            $('.expand_auth').html(win.currLanguageDic['expand_auth']);
            $('.expand_code').html(win.currLanguageDic['expand_code']);
        },
        linkListClick: function (data, event) {
            var e = window.event || event;
            window.event ? e.cancelBubble = true : e.stopPropagation();
            page.linkFun.linkDeselect();
            data.isChecked(true);
            page.linkFun.linkShareBtn();
        },
        linkCheckbox: function (data, event) {
            var e = window.event || event;
            window.event ? e.cancelBubble = true : e.stopPropagation();
            data.isChecked(!data.isChecked());
            page.linkFun.linkShareBtn();
        },
        cancelShare: function (data, event) { //单个取消分享
            var e = window.event || event;
            window.event ? e.cancelBubble = true : e.stopPropagation();
            $('.shareLink_file_option').hide();
            page.linkFun.shareDelete(data.shortUrlCode());
        },
        linkRename: function (data, event) { //重命名
            var e = window.event || event;
            window.event ? e.cancelBubble = true : e.stopPropagation();
            if (data.isRename() == true) { //判断是否在重命名
                return;
            }
            $('.shareLink_file_option').hide();
            var fileBox = $(event.target).parents('.m_file_box');
            var divName = fileBox.find('.shareLink_file_name');
            var divP = divName.find('p');
            var oldTxt = divP.text();
            var tempInput = $('<p data-bind="text:DisplayName">' + '<input class="renameInput" maxlength="50" type="text" />' + '<span class="renameOk"></span>' + '<span class="renameCancel"></span>' + '</p>');
            divName.append(tempInput);
            var divInput = divName.find('input');
            divInput.val(oldTxt);
            divP.hide();
            divInput.focus();
            divInput.select();
            var renameOk = divName.find('.renameOk');
            var renameCancel = divName.find('.renameCancel');
            data.isRename(true);

            //input框取消冒泡
            divInput.on('click', function (event) {
                var e = window.event || event;
                window.event ? e.cancelBubble = true : e.stopPropagation();
            });
            //重命名回车
            divInput.off(page.eventName).on(page.eventName, function (event) {
                event = event || win.event;
                var keyCode = (event.which ? event.which : event.keyCode);
                if (keyCode == 13) {
                    renameOk.trigger('click.t');
                }
            });
            //确定重命名
            renameOk.on('click.t', function (event) {
                var e = window.event || event;
                window.event ? e.cancelBubble = true : e.stopPropagation();
                var newVal = $.trim(divInput.val());
                var fileNamePattern = new RegExp('[/\\\\:\*\?"<>\|]');
                if (newVal == oldTxt) {
                    renameCancel.trigger('click.t');
                } else {
                    page.server.linkRename.post({
                        shorturlcode: data.shortUrlCode(),
                        newname: newVal
                    }, function (json) {
                        if (json.data) {
                            tempInput.remove();
                            divP.text(newVal).show();
                            data.isRename(false);
                            $().prompt({
                                type: 'info',
                                msg: win.currLanguageDic['renameLinkFile_ok'],
                                backgroundSetting: { info: page.prompt.infoBackground }
                            });
                            page.linkFun.initShare();
                        } else {
                            $().prompt({ type: 'error', msg: win.currLanguageDic['renameFile_failed'], isAutoHide: 0 });
                        }

                    });
                }

            });
            //取消重命名
            renameCancel.on('click.t', function (event) {
                var e = window.event || event;
                window.event ? e.cancelBubble = true : e.stopPropagation();
                data.isRename(false);
                tempInput.remove();
                divP.show();
            });

            $(document).on('click.t', function (e) {
                renameCancel.trigger('click.t');
            });
            $('.m_file_box').on('click.t', function (e) {
                renameCancel.trigger('click.t');
            });
            $('.shareLink_file_more').on('click.t', function () {
                renameCancel.trigger('click.t');
            });
            $('.shareLink_file_checkbox').on('click.t', function () {
                renameCancel.trigger('click.t');
            });
            $('.shareLink_file_expand').on('click.t', function () {
                renameCancel.trigger('click.t');
            });
        },
        shareAgain: function (data, event) { //再次分享
            var e = window.event || event;
            window.event ? e.cancelBubble = true : e.stopPropagation();
            $('.shareLink_file_option').hide();
            page.server.reuseShare.post({
                shorturlcode: data.shortUrlCode(),
                uilanguage: win.language == 'zh-CN' ? 'CN' : 'EN'
            }, function (json) {

                var data = json.data;
                win.setItemBody(data);
                // win.setItemBody(data, function () {
                //     page.linkFun.linkDeselect();
                //     page.linkFun.initShare();
                // });
            })
        },
        linkDownload: function (data, event) { //下载
            var e = window.event || event;
            window.event ? e.cancelBubble = true : e.stopPropagation();
            win.open(data.ShortUrl());
        },
        linkDeselect: function () { //取消选定
            for (var i = 0; i < page.linkFun.shareList().length; i++) {
                page.linkFun.shareList()[i].isChecked(false);
            }
            page.linkFun.linkShareBtn();
        },
        linkShareBtn: function () {
            var flag = false, i;
            for (i = 0; i < page.linkFun.shareList().length; i++) {
                if (page.linkFun.shareList()[i].isChecked() == true) {
                    flag = true;
                }
            }
            page.linkFun.linkBg(flag);
        },
        deleteShare: function () { //批量取消分享
            if (!page.linkFun.linkBg()) {
                return;
            }
            page.shareSet(true);
            page.opts.cancelShareLink.animate({
                height: 185
            });
        },
        deleteShareOk: function () {
            if (page.linkFun.LinkCancel() == false) {
                return;
            }
            page.linkFun.LinkCancel(false);
            $().prompt({
                type: 'info',
                isAutoHide: 0,
                msg: win.currLanguageDic['dealing_prompt'],
                backgroundSetting: { info: page.prompt.infoBackground }
            });
            var arr = [];
            for (var i = 0; i < page.linkFun.shareList().length; i++) {
                if (page.linkFun.shareList()[i].isChecked() == true) {
                    arr.push(page.linkFun.shareList()[i].shortUrlCode());
                }
            }
            // page.linkFun.shareDelete(arr.join()); 
            page.server.deleteShare.post({
                shorturlcode: arr.join()
            }, function (json) {
                if (json.data == true) {
                    page.linkFun.deleteShareCancel();
                    $('.promptMain').hide();
                    $().prompt({
                        type: 'info',
                        msg: win.currLanguageDic['cancelShareFile_ok'],
                        backgroundSetting: { info: page.prompt.infoBackground }
                    });
                    page.linkFun.initShare();
                    page.linkFun.linkDeselect();
                }
            })
        },
        deleteShareCancel: function () {
            page.opts.cancelShareLink.animate({
                height: 0
            }, function () {
                page.shareSet(false);
                page.linkFun.LinkCancel(true);
            });
        },
        shareDelete: function (urlCode) {
            page.server.deleteShare.post({
                shorturlcode: urlCode
            }, function (json) {
                if (json.data == true) {
                    $().prompt({
                        type: 'info',
                        msg: win.currLanguageDic['cancelShareFile_ok'],
                        backgroundSetting: { info: page.prompt.infoBackground }
                    });
                    page.linkFun.initShare();
                    page.linkFun.linkDeselect();
                }
            })
        }
    };


    //排序
    page.fileCurSort = ko.observable(); //分享文件排序
    page.filesSortOptions = ko.observableArray([]);
    page.mainFileSort = function () {
        page.filesSortOptions([{
            field: 'LastUpdateTime',
            text: win.currLanguageDic['fileSort_timeNear'],
            sortMethod: 'DESC'
        }, {
            field: 'LastUpdateTime',
            text: win.currLanguageDic['fileSort_timeFar'],
            sortMethod: 'ASC'
        }, {
            field: 'FileSize',
            text: win.currLanguageDic['fileSort_sizeSmall'],
            sortMethod: 'ASC'
        }, {
            field: 'FileSize',
            text: win.currLanguageDic['fileSort_sizeBig'],
            sortMethod: 'DESC'
        }, {
            field: 'FileName',
            text: win.currLanguageDic['fileSort_nameA'],
            sortMethod: 'ASC'
        }, {
            field: 'FileName',
            text: win.currLanguageDic['fileSort_nameZ'],
            sortMethod: 'DESC'
        }]);
    };
    page.isFirstSort = true;
    page.fileCurSort.subscribe(function (newValue) {
        page.shareBg(false);
        var falg = false;
        for (var i = 0; i < page.search().length; i++) {
            if (page.search()[i].isChecked() == true) {
                falg = true;
            }
        }
        page.shareBg(falg);
        page.isCheckAllFiles(false);
        page.fileParams.isMoreFiles = true;
        if (!page.isFirstSort) {
            page.fileParams.loadParams.orderbyField = newValue.field;
            page.fileParams.loadParams.orderbyType = newValue.sortMethod;
            page.initFiles();
            page.opts.fileIcon.removeClass('icon_showtoggle');
            if (page.opts.fileWrap.children().length == 0) {
                page.opts.myFiles.slideDown();
            } else {
                page.opts.fileWrap.slideDown();
            }
        } else {
            page.isFirstSort = false;
        }
    });

    page.recycleCurSort = ko.observable(); //回收站排序
    page.recycleSortOptions = ko.observableArray([]);
    page.mainRecycleSort = function () {
        page.recycleSortOptions([{
            field: 'DeleteTime',
            text: win.currLanguageDic['fileSort_timeNear'],
            sortMethod: 'DESC'
        }, {
            field: 'DeleteTime',
            text: win.currLanguageDic['fileSort_timeFar'],
            sortMethod: 'ASC'
        }, {
            field: 'FileSize',
            text: win.currLanguageDic['fileSort_sizeSmall'],
            sortMethod: 'ASC'
        }, {
            field: 'FileSize',
            text: win.currLanguageDic['fileSort_sizeBig'],
            sortMethod: 'DESC'
        }, {
            field: 'FileName',
            text: win.currLanguageDic['fileSort_nameA'],
            sortMethod: 'ASC'
        }, {
            field: 'FileName',
            text: win.currLanguageDic['fileSort_nameZ'],
            sortMethod: 'DESC'
        }]);
    };
    page.recycleFirstSort = true;
    page.recycleCurSort.subscribe(function (newValue) {
        page.recycleFun.recycleBg(false);
        var falg = false;
        for (var i = 0; i < page.recycleFun.recycleSearch().length; i++) {
            if (page.recycleFun.recycleSearch()[i].isChecked() == true) {
                falg = true;
            }
        }
        page.recycleFun.recycleBg(falg);
        page.recycleFun.isCheckRecycleFiles(false);
        page.recycleParams.isMoreFiles = true;
        if (!page.recycleFirstSort) {
            page.recycleParams.loadParams.orderbyField = newValue.field;
            page.recycleParams.loadParams.orderbyType = newValue.sortMethod;
            page.recycleFun.initRecycle();
            page.opts.fileIcon.removeClass('icon_showtoggle');
            if (page.opts.recycleWrap.children().length == 0) {
                page.opts.myRecycleFiles.slideDown();
            } else {
                page.opts.recycleWrap.slideDown();
            }
        } else {
            page.recycleFirstSort = false;
        }
    })

    page.showIcon = function (item) { //文件图标
        var extensionName = item.ExtensionName().replace('.', '').toLowerCase(); //后缀变成小写
        var iconClass = page.fileIconClass[extensionName] || 'fileFile';
        if (iconClass) return iconClass;
    };
    page.fileIconClass = {
        jpg: 'fileJpg',
        png: 'fileJpg',
        bmp: 'fileJpg',
        jpeg: 'fileJpg',
        tiff: 'fileJpg',
        gif: 'fileJpg',
        txt: 'fileTxt',
        text: 'fileTxt',
        html: 'fileTxt',
        htm: 'fileTxt',
        xml: 'fileTxt',
        css: 'fileTxt',
        sql: 'fileTxt',
        java: 'fileTxt',
        sln: 'fileTxt',
        log: 'fileTxt',
        config: 'fileTxt',
        rar: 'fileRar',
        zip: 'fileZip',
        mp3: 'fileMp3',
        wma: 'fileMp3',
        wav: 'fileMp3',
        mid: 'fileMp3',
        ogg: 'fileMp3',
        aac: 'fileMp3',
        mpeg: 'fileMp4',
        mp4: 'fileMp4',
        mpg: 'fileMp4',
        mov: 'fileMp4',
        wmv: 'fileMp4',
        avi: 'fileMp4',
        rmvb: 'fileMp4',
        rm: 'fileMp4',
        doc: 'fileDoc',
        docx: 'fileDoc',
        docm: 'fileDoc',
        dotx: 'fileDoc',
        dotm: 'fileDoc',
        xls: 'fileExcel',
        xlsx: 'fileExcel',
        xlsm: 'fileExcel',
        xltx: 'fileExcel',
        xltm: 'fileExcel',
        xlsb: 'fileExcel',
        xlam: 'fileExcel',
        ppt: 'filePPT',
        pps: 'filePPT',
        pptx: 'filePPT',
        ppsx: 'filePPT',
        pptm: 'filePPT',
        ppsm: 'filePPT',
        potx: 'filePPT',
        potm: 'filePPT',
        ppam: 'filePPT',
        vsd: 'fileVsd',
        vsdx: 'fileVsd',
        pdf: 'filePdf',
        mpp: 'fileProject',
        mpt: 'fileProject',
        folder: 'folder',
        sFolder: 'sFolder'
    };
    page.fileParams = { //分享文件排序
        loadParams: {
            pageIndex: 1,
            pageSize: 30,
            orderbyField: 'LastUpdateTime',
            orderbyType: 'DESC'
        },
        isMoreFiles: true,
        isLoading: false
    };
    page.recycleParams = { //回收站排序
        loadParams: {
            pageIndex: 1,
            pageSize: 30,
            orderbyField: 'DeleteTime',
            orderbyType: 'DESC'
        },
        isMoreFiles: true,
        isLoading: false
    };

    page.quickShare = function () { //分享文件快速分享
        var _ = this;
        var arr = [], i;
        for (i = 0; i < page.search().length; i++) {
            if (page.search()[i].isChecked() == true) {
                arr.push(page.search()[i]);
            }
        }
        for (i = 0; i < page.files().length; i++) {
            if (page.files()[i].isChecked() == true) {
                arr.push(page.files()[i]);
            }
        }

        if (arr.length <= 0) return;

        if (page.isShareing) {
            return;
        }

        page.isShareing = true;
        page.shareBg(false);


        var fileObjs = [];
        arr = ko.mapping.toJS(arr); //变成原生的js
        for (var i = 0; i < arr.length; i++) {
            fileObjs.push({
                "ObjectId": arr[i].ID,
                "ObjectClass": "file",
                "DisplayName": arr[i].FileName
            })
        }
        var shareParams = {
            "Objects": fileObjs,
            "IsAuth": null,
            "IsValidate": null,
            "ValidateCode": null,
            "ExpireTime": null,
            "FileShareLimit": null,
            "ShortUrl": "",
            "UiLanguage": win.language == 'zh-CN' ? 'CN' : 'EN'
        };

        page.getStrategy(function (strategyData) {
            shareParams.IsAuth = strategyData.MustAuth ? 1 : 0;
            shareParams.IsValidate = strategyData.ValidateCheckBoxChecked ? 1 : 0;
            shareParams.ValidateCode = strategyData.DefaultValidateCode;
            shareParams.ExpireTime = strategyData.ExpireTime;
            shareParams.FileShareLimit = strategyData.FileShareLimit;


            page.shareFiles(shareParams, function (data) {
                win.setItemBody(data, function () {
                    page.isShareing = false;
                    page.shareBg(true);
                    page.deselect();
                });
            });
        });
    };

    page.uploadQuickBtn = function () { //上传文件快速分享
        var _ = this,
            opts = _.opts;
        var arr = [];
        opts.arrayWrap.find('.upload_text').each(function () {
            var $this = $(this);
            if ($this.hasClass('upload_text_content')) {
                arr.push($this.parent().parent().parent().attr('id'));
            }
        });
        if (arr.length <= 0) return;

        if (page.isShareing) {
            return;
        }
        page.isShareing = true;
        page.shareStyle(false);

        var fileObjs = [];
        for (var i = 0; i < arr.length; i++) {
            var tempFile = page.tasks[arr[i]].remoteFileInfo;
            fileObjs.push({
                "ObjectId": tempFile.FileID,
                "ObjectClass": "file",
                "DisplayName": tempFile.FileName
            });
        }
        var shareParams = {
            "Objects": fileObjs,
            "IsAuth": null,
            "IsValidate": null,
            "ValidateCode": null,
            "ExpireTime": null,
            "FileShareLimit": null,
            "ShortUrl": "",
            "UiLanguage": win.language == 'zh-CN' ? 'CN' : 'EN'
        };

        page.getStrategy(function (strategyData) {
            shareParams.IsAuth = strategyData.MustAuth ? 1 : 0;
            shareParams.IsValidate = strategyData.ValidateCheckBoxChecked ? 1 : 0;
            shareParams.ValidateCode = strategyData.DefaultValidateCode;
            shareParams.ExpireTime = strategyData.ExpireTime;
            shareParams.FileShareLimit = strategyData.FileShareLimit;

            page.shareFiles(shareParams, function (data) {
                win.setItemBody(data, function () {
                    page.isShareing = false;
                    page.shareStyle(true);
                    opts.arrayWrap.find('.upload_text').removeClass('upload_text_content')
                    page.shareStyle(false);
                });
            });
        });
    };

    page.advancedFileShare = { //高级分享
        advancedSelectItems: ko.observableArray([]),
        mustAuth: ko.observable(false),
        expireTime: ko.observable(), //文件过期时间
        allowChangeAuth: ko.observable(false), //是否允许修改身份认证
        allowChangeExpireTime: ko.observable(false), //是否允许修改时间
        allowChangeValidateCode: ko.observable(false), //是否充许修改验证码
        mustValidateCode: ko.observable(false), //是否必须验证码
        validateCode: ko.observable(), //默认验证码
        isReBuildValidateCode: ko.observable(false),
        fileShareLimit: ko.observable(),
        showChoseTime: ko.observable(false),
        shareOkBtnDisable: ko.observable(false),
        shareCancelBtnDisable: ko.observable(false),
        /**
         * GET SERVER TODAY DATE
         * @returns {serverDateStr}
         */
        getServerDateStr: function () {
            return page.advancedFileShare.serverDateStr || app.formatDate(new Date());
        },
        inspectDate: function (val) {
            var dateReg = /^((?!0000)[0-9]{4}-((0[1-9]|1[0-2])-(0[1-9]|1[0-9]|2[0-8])|(0[13-9]|1[0-2])-(29|30)|(0[13578]|1[02])-31)|([0-9]{2}(0[48]|[2468][048]|[13579][26])|(0[48]|[2468][048]|[13579][26])00)-02-29)$/;
            var inputValDate = new Date(val);
            var serverDate = new Date(page.advancedFileShare.getServerDateStr());
            if (!dateReg.test(val) || inputValDate < serverDate) {
                return false;
            }
            // page.advancedFileShare.shareOkBtnDisable(false);
            return true;
        },
        init: function (strategyData) {

            page.advancedFileShare.showChoseTime(false);
            page.advancedFileShare.mustAuth(strategyData.MustAuth);
            page.advancedFileShare.mustValidateCode(strategyData.ValidateCheckBoxChecked);
            page.advancedFileShare.allowChangeAuth(strategyData.AllowChangeAtuth); //是否允许修改身份验证
            page.advancedFileShare.allowChangeExpireTime(strategyData.AllowChangeExpireTime); //是否允许修改过期时间
            page.advancedFileShare.allowChangeValidateCode(strategyData.AllowChangeValidateSetting); //是否允许修改验证码
            page.advancedFileShare.isReBuildValidateCode(strategyData.AllowChangeValidateSetting);

            page.advancedFileShare.fileShareLimit(strategyData.FileShareLimit);


            page.advancedFileShare.validateCode(strategyData.DefaultValidateCode); //默认验证码
            if (strategyData.FileShareLimit == 6) {
                //page.advancedFileShare.expireTime(strategyData.ExpireTime);
                //page.advancedFileShare.expireTime('');
                page.advancedFileShare.expireTime(page.advancedFileShare.getServerDateStr());
                page.advancedFileShare.showChoseTime(true);
            } else if (strategyData.FileShareLimit == 5) {
                page.advancedFileShare.expireTime('');
            } else {
                var v = page.opts.adShareSeleteItem.val();
                var item = page.advancedFileShare.advancedSelectItems()[v - 1];
                page.advancedFileShare.expireTime(item.txt);
            }

            page.shareSet(true);
            page.opts.advancedShare.animate({
                height: 185
            });
            page.opts.adShareSeleteDate.css('border-color', '#999999');
        },
        advancedBtnClick: function () { //分享文件高级分享
            if (page.isShareing) {
                return;
            }
            page.advancedFileShare.shareOkBtnDisable(false);
            page.advancedFileShare.shareCancelBtnDisable(false);

            page.advancedFileShare.fileObjs = null; //高级分享选中的文件
            page.advancedFileShare.fileIds = null; //高级分享选中文件的ID
            page.advancedSelect();
            var arr = [],
                fileIds = [], i;
            for (i = 0; i < page.search().length; i++) {
                var tempSearchFile = page.search()[i];
                if (tempSearchFile.isChecked()) {
                    arr.push({
                        "ObjectId": tempSearchFile.ID(),
                        "ObjectClass": "file",
                        "DisplayName": tempSearchFile.FileName()
                    });
                    fileIds.push(tempSearchFile.ID());
                }
            }
            for (i = 0; i < page.files().length; i++) {
                var tempFile = page.files()[i];
                if (tempFile.isChecked()) {
                    arr.push({
                        "ObjectId": tempFile.ID(),
                        "ObjectClass": "file",
                        "DisplayName": tempFile.FileName()
                    });
                    fileIds.push(tempFile.ID());
                }
            }
            if (arr.length <= 0) return;

            page.advancedFileShare.fileObjs = arr;
            page.advancedFileShare.fileIds = fileIds;

            page.getStrategy(function (strategyData) {
                page.advancedFileShare.init(strategyData);
            });
        },
        close: function () {
            if (page.advancedFileShare.shareCancelBtnDisable()) {
                return;
            }
            page.isShareing = false;
            //page.opts.aFSdTime.hideCalendarChoose(); //关闭日历
            page.opts.advancedShare.animate({
                height: 0
            }, function () {
                $('#m_f_btnUpload').show();
                page.shareSet(false);
                page.rebuild(false);
                page.advancedFileShare.mustValidateCode(false);
                page.advancedFileShare.validateCode('');

                page.opts.aFSdTime.jq.val('');
            });
        },
        rebuildValidateCode: function () {
            //TODO:重新生成验证码
            var flag = page.advancedFileShare.mustValidateCode();
            if (!flag) return;
            page.server.buildValidateCode.post({
                objectIds: page.advancedFileShare.fileIds.join()
            }, function (json) {
                page.advancedFileShare.validateCode(json.data);
            });
        },
        validateCodeCheckboxChange: function () { //改变重新生成的颜色
            if (!page.advancedFileShare.allowChangeValidateCode()) {
                return;
            }
            page.advancedFileShare.mustValidateCode(!page.advancedFileShare.mustValidateCode());

            page.advancedFileShare.isReBuildValidateCode(page.advancedFileShare.mustValidateCode());
            if (!page.advancedFileShare.mustValidateCode()) {
                page.opts.adReuildVcode.addClass('disableSpan');
                page.rebuild(false);
                page.advancedFileShare.validateCode('');
            } else {
                page.opts.adReuildVcode.removeClass('disableSpan');
                page.rebuild(true);
                page.advancedFileShare.rebuildValidateCode();
            }
        },
        shareOk: function () {
            if (page.advancedFileShare.shareOkBtnDisable()) {
                return;
            }
            page.advancedFileShare.shareOkBtnDisable(true);
            page.advancedFileShare.shareCancelBtnDisable(true);

            if (page.isShareing) {
                return;
            }
            page.isShareing = true;

            var shareParams = {
                "Objects": page.advancedFileShare.fileObjs,
                "IsAuth": page.advancedFileShare.mustAuth() ? 1 : 0,
                "IsValidate": null,
                "ValidateCode": '',
                "ExpireTime": null,
                "FileShareLimit": null,
                "ShortUrl": "",
                "UiLanguage": win.language == 'zh-CN' ? 'CN' : 'EN'
            };

            var flag = page.advancedFileShare.mustValidateCode();
            if (flag) {
                shareParams.IsValidate = 1;
                shareParams.ValidateCode = page.advancedFileShare.validateCode();
            } else {
                shareParams.IsValidate = 0;
            }
            var fileShareLimit = page.advancedFileShare.fileShareLimit();
            if (fileShareLimit == 6) {
                page.advancedFileShare.expireTime(page.opts.aFSdTime.jq.val());
                var nDate = new Date(),
                    sDate = new Date(page.advancedFileShare.expireTime().replace(/-/g, '/').replace('.000', '') + ' 23:59:59');


                if ((sDate.valueOf() - nDate.valueOf()) < 0) {
                    $().prompt({
                        type: 'warning',
                        msg: win.currLanguageDic['advanced_data']
                    });
                    page.isShareing = false;
                    return;
                }

            }
            if (fileShareLimit == 5) {
                page.advancedFileShare.expireTime('9999-01-01');
            }

            shareParams.ExpireTime = page.advancedFileShare.expireTime();
            shareParams.FileShareLimit = fileShareLimit;
            page.shareBg(false);
            page.shareFiles(shareParams, function (data) {
                win.setItemBody(data, function () {
                    page.isShareing = false;
                    page.shareBg(true);
                    if (page.showUploadDiv() == true) {
                        page.opts.arrayWrap.find('.upload_text').removeClass('upload_text_content');
                        page.shareStyle(false);
                    } else {
                        page.deselect();
                    }
                    page.advancedFileShare.shareOkBtnDisable(false);
                    page.advancedFileShare.shareCancelBtnDisable(false);
                    page.advancedFileShare.close();
                });
            });
        },
        uploadAdvancedBtn: function () { //上传文件高级分享
            var _ = this,
                opts = _.opts,
                arr = [];

            if (page.isShareing) {
                return;
            }
            page.advancedFileShare.shareOkBtnDisable(false);
            page.advancedFileShare.shareCancelBtnDisable(false);

            $('#m_f_btnUpload').hide();
            page.advancedSelect();
            opts.arrayWrap.find('.upload_text').each(function () {
                var $this = $(this);
                if ($this.hasClass('upload_text_content')) {
                    arr.push($this.parent().parent().parent().attr('id'));
                }
            });
            if (arr.length <= 0) return;
            var fileObjs = [];
            for (var i = 0; i < arr.length; i++) {
                var tempFile = page.tasks[arr[i]].remoteFileInfo;
                fileObjs.push({
                    "ObjectId": tempFile.FileID,
                    "ObjectClass": "file",
                    "DisplayName": tempFile.FileName
                });
            }
            if (arr.length <= 0) return;
            page.advancedFileShare.fileObjs = fileObjs;
            page.advancedFileShare.fileIds = arr;

            page.getStrategy(function (strategyData) {
                page.advancedFileShare.init(strategyData);
            });
        },
        changeAuthClick: function () {
            if (!page.advancedFileShare.allowChangeAuth()) {
                return;
            }
            page.advancedFileShare.mustAuth(!page.advancedFileShare.mustAuth());
        }
    };

    page.advancedSelect = function () {
        page.advancedFileShare.advancedSelectItems(
            [{
                i: 1,
                v: win.currLanguageDic['advShare_day'],
                txt: page.formatDate(1)
            }, {
                i: 2,
                v: win.currLanguageDic['advShare_week'],
                txt: page.formatDate(2)
            }, {
                i: 3,
                v: win.currLanguageDic['advShare_month'],
                txt: page.formatDate(3)
            }, {
                i: 4,
                v: win.currLanguageDic['advShare_year'],
                txt: page.formatDate(4)
            }, {
                i: 5,
                v: win.currLanguageDic['advShare_never'],
                txt: ''
            }, {
                i: 6,
                v: win.currLanguageDic['advShare_customized'],
                txt: ''
            }])
    };
    page.advancedFileShare.fileShareLimit.subscribe(function (newValue) {
        newValue = newValue - 1 + 1;
        // page.advancedFileShare.shareOkBtnDisable(false);
        if (newValue == 6) {
            //page.advancedFileShare.expireTime('');
            page.advancedFileShare.expireTime(page.advancedFileShare.getServerDateStr());
            page.advancedFileShare.showChoseTime(true);
            return;
        }
        page.advancedFileShare.showChoseTime(false);
        var item = page.advancedFileShare.advancedSelectItems()[newValue - 1];
        page.advancedFileShare.expireTime(item.txt);
    });

    page.getDriveUse = function (callback) {
        page.server.getuploadpar.get(function (json) {
            if ($.isFunction(callback)) {
                callback(json.data);
            }
        });
    };

    page.skyDriveUse = function () { //判断网盘使用大小
        var _ = this;
        _.getDriveUse(function (data) {
            var nowUsedQuota = (data.UserUsedQuota / data.UserQuota) * 100;
            if (nowUsedQuota > 90) {
                $().prompt({
                    type: 'warning',
                    msg: win.currLanguageDic['useSpace'] + Math.floor(nowUsedQuota) + '%)',
                    isAutoHide: 0
                });
                page.isQuotaOver = true;
                $('.promptClose').on('click.t', function () {
                    page.isQuotaOver = false;
                })
            }
        })
    };

    page.getStrategy = function (callback) { //获取全局策略
        page.server.strategy.get(function (json) {
            $.isFunction(callback) && callback(json.data);
        })
    };

    page.shareFiles = function (params, callback) { //分享文件
        page.server.share.post(params, function (json) {
            $.isFunction(callback) && callback(json.data);
        });
    };

    page.server = page.server(app.global.debug ? {
        files: '/data/files.json',
        rename: '/data/rename.json',
        deleteFile: '/data/delete.json',
        search: '/data/search.json',
        share: '/data/share.json',
        strategy: '/data/strategy.json',
        buildValidateCode: '/data/buildValidateCode.json',
        getdownloadhashvalue: '/data/getdownloadhashvalue.json',
        singleFileDownload: '/data/singleFileDownload.json',
        getuploadpar: '/data/getuploadpar.json',
        upload: '/data/upload.json',
        quickUpload: '/data/quickUpload.json',
        cancelUpload: '/data/cancelupload.json',
        getGlobalUploadSetting: '/data/getGlobalUploadSetting.json',
        getCurrentUserInfo: '/data/getCurrentUserInfo.json',
        logOff: '/data/logOff.json',
        getServerTime: '/data/GetServerTime.json',

        recycleList: '/data/recycleList.json',
        recycleSearch: '/data/recycleSearch.json',
        recycleEmpty: '/data/recycleEmpty.json',
        recycleDelete: '/data/recycleDelete.json',
        recycleRecover: '/data/recycleRecover.json',

        getShareList: '/data/getShareList.json',
        deleteShare: '/data/deleteShare.json',
        linkRename: '/data/renameShare.json',
        reuseShare: '/data/reuseShare.json'
    } : {
            files: 'api/file/list', //获取用户的所有文件
            rename: 'api/file/reName', //重命名文件
            deleteFile: 'api/file/delete', //删除文件
            search: 'api/file/search',
            share: 'api/share/share',
            strategy: 'api/setting/getShareSettings',
            buildValidateCode: 'api/share/buildValidateCode',
            getdownloadhashvalue: 'api/file/getdownloadhashvalue',
            singleFileDownload: 'api/file/downloadFileById',
            getuploadpar: 'api/setting.ashx?op=getuploadpar',
            upload: 'api/file/upload',
            quickUpload: 'api/file/QuickUpload',
            cancelUpload: 'api/file/cancelupload',
            getGlobalUploadSetting: 'api/setting.ashx?op=getGlobalUploadSetting',
            getCurrentUserInfo: 'api/account/getCurrentUserInfo',
            logOff: 'api/account/LogOff',
            getServerTime: 'api/share/GetServerTime',

            recycleList: 'api/recyclebin/list',
            recycleSearch: 'api/recyclebin/search',
            recycleEmpty: 'api/recyclebin/empty',
            recycleDelete: 'api/recyclebin/delete',
            recycleRecover: 'api/recyclebin/recover',

            getShareList: 'api/share/getsharelist',
            deleteShare: 'api/share/deleteshare',
            linkRename: 'api/share/renameshare',
            reuseShare: 'api/share/reuseshare'
        });

    page.server.rename.onError = function (error) {
        page.errorCode(error);
    }

    page.server.strategy.onError = function () {
        page.isShareing = false;
    };

    page.server.share.onError = function (error) {
        page.isShareing = false;
        page.advancedFileShare.shareOkBtnDisable(false);
        page.advancedFileShare.shareCancelBtnDisable(false);
        page.shareBg(true);
        page.errorCode(error);
    };
    page.server.logOff.onError = function () {
        page.logOutFn();
    };
    page.server.deleteFile.onError = function (error) {
        page.shareBatch(true);
        $().prompt({ type: 'error', msg: win.currLanguageDic['deleteFile_failed'], isAutoHide: 0 });
        page.errorCode(error);
    };

    page.server.recycleRecover.onError = function (error) { //恢复文件
        page.recycleFun.recyleRecover(true);
        page.errorCode(error);
    };
    page.server.recycleEmpty.onError = function (error) { //清空回收站
        page.recycleFun.recyleEmpty(true);
        page.errorCode(error);
    };
    page.server.recycleList.onError = function (error) { //获取回收站文件列表
        page.errorCode(error);
    };
    page.server.recycleSearch.onError = function (error) { //搜索回收站
        page.errorCode(error);
    };
    page.server.recycleDelete.onError = function (error) { //从回收站中删除文件
        page.recycleFun.recyleBatch(true);
        page.errorCode(error);
    };
    page.server.getShareList.onError = function (error) { //获取分享链接文件列表
        page.errorCode(error);
    };
    page.server.linkRename.onError = function (error) { //分享链接重命名
        page.errorCode(error);
    };
    page.server.reuseShare.onError = function (error) { //分享链接再次分享
        page.errorCode(error);
    };
    page.server.deleteShare.onError = function (error) { //分享链接删除链接
        page.linkFun.linkBg(true);
        page.errorCode(error);
    };
    page.server.getdownloadhashvalue.onError = function (err) {
        $().prompt({
            isAutoHide: 0,
            type: 'error',
            msg: '下载文件出错'
        })
        page.errorCode(err);
    };

    page.errorCode = function (error) {
        if (error == '2000') {
            $().prompt({
                isAutoHide: 0,
                type: 'error',
                msg: win.currLanguageDic['recycleJudge_encount']
            });
        } else {
            var msg = win.currLanguageDic['errorCode_' + error];
            if (msg) {
                $().prompt({
                    type: 'warning',
                    msg: msg
                });
            }
        }
    };


    page.shareBatch = ko.observable(true);

    page.showSettingDiv = ko.observable(false);
    page.showUploadDiv = ko.observable(true);
    page.showShareDiv = ko.observable(false);
    page.showDeleteDiv = ko.observable(false);
    page.showSearchResult = ko.observable(false);
    page.shareSet = ko.observable(false);
    page.shareBg = ko.observable(false);
    page.shareStyle = ko.observable(false);
    page.rebuild = ko.observable(false);
    page.langBtn = ko.observable(false);
    page.isCheckAllSearch = ko.observable(false);
    page.isCheckAllFiles = ko.observable(false);
    page.isHaveFiles = ko.observable(false);
    page.isHaveSearch = ko.observable(false);
    page.customDateTitle = ko.observable();
    page.rename = ko.mapping.fromJS([]);
    page.files = ko.mapping.fromJS([]);
    page.search = ko.mapping.fromJS([]);

    page.renaming = false; //重命名
    page.isQuotaOver = false; //内存容量超过90%
    page.netWorkInterrupTed = false; //网络异常

    page.shareLinkMenu = ko.observable(false);
    page.recycleBinMenu = ko.observable(false);
    page.settingMenu = ko.observable(false);


    page.titleDiv = { //title切换
        routes: {
            0: page.showUploadDiv,
            1: page.showShareDiv,
            2: page.showSettingDiv,
            3: page.recycleFun.showRecycleDiv,
            4: page.linkFun.showLinkDiv
        },
        currentKey: '0',
        change: function (key) {
            var currentKey = page.titleDiv.currentKey;
            if (currentKey == key) { return; }
            page.titleDiv.routes[currentKey](false);
            page.titleDiv.routes[key](true);
            page.titleDiv.currentKey = key;
        }
    }


    win.initPage = function () {
        page.onReady({
            'menuLis': $('#l_main_head_left').find('li'),
            'headRight': $('.l_main_head_right').find('li'),
            'search': $('#m_shareFiles_container'),
            'recycle': $('#m_recycle_container'),
            'searchBtn': $('#searchBtn'),
            'recycleSearchBtn': $('#recycleSearchBtn'),
            'searchTxt': $('#m_share_text'),
            'recycleTxt': $('#m_recycle_text'),
            'clear': $('.m_search_result_clear'),
            'recycleClear': $('.m_recycle_result_clear'),
            'seachResultTitle': $('.search_title_text'),
            'recycleSearchTitle': $('.recycleSearch_title_text'),
            'quickShareBtn': $('#share_shareBtn_right'),
            'advancedShareBtn': $('#share_shareBtn_left'),
            'fileIcon': $('#m_file_icon'),
            'searchIcon': $('#search_icon'),
            'recycleSearchIcon': $('.recycle_searchIcon'),
            'recycleFileIcon': $('.recycle_fileIcon'),
            'mask': $('#l_main_mask'),
            'advancedShare': $('#m_main_advancedShare'),
            'close': $('#m_advancedShare_close'),
            'headIcon': $('.l_main_head_icon'),
            'iconTxt': $('.l_main_icon_test'),
            'adReuildVcode': $('#adReuildVcode'),
            'adShareSeleteItem': $('#adShareSeleteItem'),
            'adShareSeleteDate': $('#adShareSeleteDate'),
            'mUploadArea': $('#mUploadArea')[0],
            'msettings': $('.m_main_settings'),
            'spaceUsed': $('.space_used'),
            'spaceAll': $('.space_all'),
            'spaceRound': $('.setting_space_round'),
            'languageBtn': $('.language_btn'),
            'loadingMore': $('#loading_moreFiles'),
            'recycleMore': $('#loading_recycleFiles'),
            'roundMiddle': $('.space_round_middle'),
            'roundRight': $('.space_round_right'),
            'uploading': $('.uploading_title'),
            'myFiles': $('.myfiles'),
            'myRecycleFiles': $('.myRecycleFiles'),
            'reSearch': $('.research'),
            'noSearch': $('.noSearch'),
            'noLinkSearch': $('.noLinkSearch'),
            'resultMore': $('.resultMore'),
            'recycleExceed': $('.recycleExceed'),
            'arrayWrap': $('.m_array_wrap'),
            'batchDelete': $('.m_main_batchDelete'),
            'emptyRecycle': $('.m_main_emptyRecycle'),
            'recoverRecycle': $('.m_main_recoverRecycle'),
            'deleteRecycle': $('.m_main_deleteRecycle'),
            'fileWrap': $('.m_file_wrap'),
            'recycleWrap': $('.m_recycle_wrap'),
            'resultWrap': $('.m_search_result_wrap'),
            'recycleSearchWrap': $('.m_recycle_search_wrap'),
            'dragArea': $('.m_upload_dragarea'),
            'userId': $('.userId'),
            'headCircle': $('.l_main_head_circle'),
            'linkTxt': $('.m_shareLink_text'),
            'mUpload': $('.m_upload'),
            'cancelShareLink': $('.m_main_cancelShareLink')
        });
        ko.applyBindings(page);
    };

    function uploadInit() {
        page.showUploadNumber = function () { //上传文件右侧计数红点
            var unUploadedCount = 0;
            if (page.tasksFileUnUploaded) {
                unUploadedCount = page.tasksFileUnUploaded();
            }
            if (unUploadedCount > 0) {
                page.opts.headCircle.html(unUploadedCount).show();
            } else if (unUploadedCount >= 99) {
                page.opts.headCircle.html('99').show();
            } else {
                page.opts.headCircle.html('').hide();
            }
        };
        page.shareBackground = function () {
            if (page.opts.arrayWrap.find('.upload_text').hasClass('upload_text_content') == true) {
                page.shareStyle(true);
            } else {
                page.shareStyle(false);
            }
        };
        if (window.ribbon && !(!!window['Blob'] && !!window['FileReader'])) {
            //silverlight上传
            !function (w, app, page) {
                page.myFiles = {
                    upFileDialogShow: ko.observable(true),//上传对话框最大化最小化控制
                    upFileItems: ko.observableArray([]),//正在上传的文件列表
                    upFileGlobalCount: ko.observable(0),
                    upFileGlobalPercent: ko.observable(0)
                };
                page.tasks = {};

                page.uploadQueueDetail = {
                    totalCount: 0,
                    uploadingCount: 0,
                    waitingCount: 0,
                    faildCount: 0,
                    pendingCount: 0,
                    finishedCount: 0
                };

                page.tasksFileUnUploaded = function () {
                    w.getUploadQueueDetail();
                    return page.uploadQueueDetail.totalCount - page.uploadQueueDetail.finishedCount;
                }

                function findUpFileItem(arr, findItem, key) {
                    return ko.utils.arrayFilter(page.myFiles.upFileItems(), function (item) {
                        return item[key] == findItem[key];
                    });
                };

                var UpFileInfo = function (jq, data) {
                    this.jq = jq;//上传外层容器
                    this.data = data;
                    this.template = '<div class="m_array_box" id="{guid}">\
                                        <div class="m_array_top">\
                                            <div class="upload_checkbox">\
                                                <div class="upload_text"></div>\
                                            </div>\
                                            <div class="m_array_top2"></div>\
                                            <div class="m_array_top3">{fileName}</div>\
                                        </div>\
                                        <div class="m_array_mid" id="upOperation{guid}">\
                                            <div class="m_array_mid1 txtHover" id="upFileAndBegin{guid}"></div>\
                                            <div class="m_array_mid2 txtHover" id="upFileAndClose{guid}"></div>\
                                        <div class="m_array_mid_right">\
                                            <div class="m_array_mid3" id="upFileTime{guid}"></div>\
                                            <div class="m_array_mid4"><span class="upSize" id="upFilePercent{guid}">0%</span> of {fileSize}</div>\
                                        </div>\
                                        </div>\
                                        <div class="m_array_bottom">\
                                            <div class="m_array_bottom_progress" id="upProcessed{guid}" ></div>\
                                        </div>\
                                    </div>';
                    this.timeInfo = {
                        h: 0,
                        m: 0,
                        s: 0
                    };
                };
                UpFileInfo.prototype = {
                    addUpFile: function () {
                        $('.noUploadFile').css('display', 'none');
                        this.initUpFileInfo();//初始化文件信息
                        return this;
                    },
                    begin: function () {
                        var _ = this;
                        _.isFileTransfer = 1;//文件已传送
                        _.initInterval();
                    },
                    rStart: function () {
                        this.initInterval();
                    },
                    push: function () {
                        this.stop();
                    },
                    stop: function () {
                        var _ = this;
                        if (_.time) {
                            window.clearInterval(_.time);
                        }
                    },
                    initUpFileInfo: function () {
                        var _ = this;
                        _.upFileInfo = {
                            guid: _.data.GUID,
                            fileName: _.data.FileName,
                            folderPath: _.data.folderPath,
                            fileSize: _.data.FileSize
                        };
                        _.isFileUp = 1;//控件上传 暂停按钮操作
                        _.isFileTransfer = 0;//文件是否正在传送
                        _.upFileLi = $(app.formatJSON(_.template, _.upFileInfo));
                        _.jq.prepend(_.upFileLi);
                        var guid = _.upFileInfo.guid;
                        _.upFileTime = _.upFileLi.find('#upFileTime' + guid);//上传时间
                        _.upFilePercent = _.upFileLi.find('#upFilePercent' + guid);//上传百分比
                        //_.upFileSize = _.upFileLi.find('#upFileSize' + guid);//上传的大小
                        _.upProcessed = _.upFileLi.find('#upProcessed' + guid);//上传的进度条
                        // _.upOperation = _.upFileLi.find('#upOperation' + guid);//上传操作
                        // _.upCompleted = _.upFileLi.find('#upCompleted' + guid);//上传完成
                        _.upCompletedText = _.upFileLi.find('#upCompletedText' + guid);//上传完成后的文字
                        _.upFileAndBegin = _.upFileLi.find('#upFileAndBegin' + guid);//上传及暂停按钮
                        _.upFileAndClose = _.upFileLi.find('#upFileAndClose' + guid);//取消按钮
                        _.upLoadCheckBox = _.upFileLi.find('.upload_checkbox');
                        _.upLoadTxt = _.upFileLi.find('.upload_text');

                        if (_.isFileUp) {
                            //_.upFileAndBegin.removeClass('upFileAndBegin').addClass('upFileAndPush');
                            _.upFileAndBegin.removeClass('m_array_mid1_start').addClass('m_array_mid1_pause');
                        }
                        _.upFileAndBegin.off('click.t').on('click.t', function () {
                            if (_.isFileUp) {
                                w.SuspendUpload(_.GUID);
                                //_.upFileAndBegin.removeClass('upFileAndPush').addClass('upFileAndBegin');
                                _.upFileAndBegin.removeClass('m_array_mid1_pause').addClass('m_array_mid1_start');
                                _.isFileUp = 0;
                            } else {
                                w.BeginUpload(_.GUID);
                                //_.upFileAndBegin.removeClass('upFileAndBegin').addClass('upFileAndPush');
                                _.upFileAndBegin.removeClass('m_array_mid1_start').addClass('m_array_mid1_pause');
                                _.isFileUp = 1;
                            }
                        });
                        _.upFileAndClose.off('click.t').on('click.t', function () {
                            w.CancelUpload(_.GUID);
                        });
                        _.upLoadTxt.on('click.t', function () {
                            var $this = $(this);
                            if ($this.hasClass('upload_text_content')) {
                                $this.removeClass('upload_text_content');
                            } else {
                                $this.addClass('upload_text_content');
                            }
                            page.shareBackground();
                        });
                        page.showUploadNumber();
                    },
                    initInterval: function () {
                        var _ = this;
                        if (_.time) window.clearInterval(_.time);
                        _.time = setInterval(function () {
                            _.timeInfo.s++;
                            if (_.timeInfo.s == 60) {
                                _.timeInfo.s = 0;
                                _.timeInfo.m++;

                                if (_.timeInfo.m == 60) {
                                    _.timeInfo.m = 0;
                                    _.timeInfo.h++;
                                }
                            }
                            _.upFileTime.html(_.formatStr());
                        }, 1000);
                    },
                    formatStr: function () {
                        var _ = this,
                            sY = (_.timeInfo.h < 10) ? '0' + _.timeInfo.h : _.timeInfo.h,
                            sM = (_.timeInfo.m < 10) ? '0' + _.timeInfo.m : _.timeInfo.m,
                            sS = (_.timeInfo.s < 10) ? '0' + _.timeInfo.s : _.timeInfo.s;
                        return sY + ':' + sM + ':' + sS;
                    },
                    cancel: function () {
                        this.stop();
                        this.upFileLi.remove();
                        var file = { GUID: this.GUID },
                            upItem = findUpFileItem(page.myFiles.upFileItems, file, "GUID");
                        if (upItem.length) {
                            var index = page.myFiles.upFileItems.indexOf(upItem[0]);
                            page.myFiles.upFileItems.splice(index, 1);
                        }
                    }
                };

                w.slProxy = null;
                w.onSilverlightLoad = function (sender) {
                    w.slProxy = sender.getHost();
                };

                //silverlight错误时候
                w.onSilverlightError = function (sender, args) {

                    var appSource = "";
                    if (sender != null && sender != 0) {
                        appSource = sender.getHost().Source;
                    }

                    var errorType = args.ErrorType;
                    var iErrorCode = args.ErrorCode;

                    if (errorType == "ImageError" || errorType == "MediaError") {
                        return;
                    }

                    var errMsg = "Silverlight 应用程序中未处理的错误 " + appSource + "\n";


                    errMsg += "代码: " + iErrorCode + "    \n";
                    errMsg += "类别: " + errorType + "       \n";
                    errMsg += "消息: " + args.ErrorMessage + "     \n";

                    if (errorType == "ParserError") {
                        errMsg += "文件: " + args.xamlFile + "     \n";
                        errMsg += "行: " + args.lineNumber + "     \n";
                        errMsg += "位置: " + args.charPosition + "     \n";
                    } else if (errorType == "RuntimeError") {
                        if (args.lineNumber != 0) {
                            errMsg += "行: " + args.lineNumber + "     \n";
                            errMsg += "位置: " + args.charPosition + "     \n";
                        }
                        errMsg += "方法名称: " + args.methodName + "     \n";
                    }
                    //console.error(errMsg);
                    //throw new Error(errMsg);
                };

                //记录错误日志
                w.SilverlightErrorLog = function (msg) {
                    var data = "mes=" + encodeURIComponent(msg);
                    $.ajax({
                        url: '/ServiceApi/SilverlightErrorLog',
                        async: false,
                        type: 'POST',
                        dataType: 'json',
                        data: data,
                        error: function () {
                            //alert('error');
                            //console.error('silverLightErrorLog');
                        },
                        success: function (result) {
                            // alert(result.data);
                            // console.info('silverlightErrorLogSuccess');
                        }

                    });
                };

                //记录调试日志
                w.SilverlightDebugLog = function (msg) {

                    var data = "mes=" + encodeURIComponent(msg);
                    //console.log('silverlightDebuglog:'+msg);
                };

                //宽度设置
                w.GetClientWidth = function () {
                    //return page.opts.mFBtnUpload.width();
                };

                //取消上传
                w.CancelUpload = function (guid) {
                    if (w.slProxy != null) {
                        var result = w.slProxy.Content.Uploader.CancelUpload(guid);
                        if (result) {
                            var file = { GUID: guid },
                                upItem = findUpFileItem(page.myFiles.upFileItems, file, "GUID");
                            if (upItem.length) {
                                upItem = upItem[0];
                                upItem.cancel();
                                page.showUploadNumber();
                            }

                            if (page.myFiles.upFileItems().length == 0) {
                                page.shareStyle(false);
                                $('.noUploadFile').css('display', 'block');
                            }
                        }
                    }
                };

                //暂停上传
                w.SuspendUpload = function (guid) {
                    if (w.slProxy != null) {
                        var result = w.slProxy.Content.Uploader.SuspendUpload(guid);
                        if (result) {
                            var file = { GUID: guid },
                                upItem = findUpFileItem(page.myFiles.upFileItems, file, "GUID");
                            if (upItem.length) {
                                upItem = upItem[0];
                                upItem.push();
                            }
                        }
                    }
                };

                //继续上传
                w.BeginUpload = function (guid) {
                    if (w.slProxy != null) {
                        var result = w.slProxy.Content.Uploader.BeginUpload(guid);
                        if (result) {
                            var file = { GUID: guid },
                                upItem = findUpFileItem(page.myFiles.upFileItems, file, "GUID");
                            if (upItem.length) {
                                upItem = upItem[0];
                                upItem.rStart();
                            }
                        }
                    }
                };

                //设置宽度
                w.SetUploadButtonWidth = function (width) {
                    if (w.slProxy != null) {
                        w.slProxy.Content.Uploader.SetUploadButtonWidth(width);
                    }
                };

                //添加文件到上传列表
                w.AppendFile = function (fileRow) {
                    var file = JSON.parse(fileRow).data;
                    file.folderPath = "";
                    if (file.FolderParentID == "-1") {
                        file.folderPath = file.FolderName + "\\";
                    }
                    else {
                        file.folderPath = "..\\" + file.FolderName + "\\";
                    }

                    //upFileItems
                    var upFileInfo = new UpFileInfo($('#mArraryWrapList'), file).addUpFile();
                    upFileInfo.GUID = file.GUID;
                    //page.myFiles.upFileItems.push(upFileInfo);
                    //TODO:将新添加的文件添加到第一条
                    page.myFiles.upFileItems.splice(0, 0, upFileInfo);

                    //page.myFiles.upFileDialogShow(true);
                    //判断是否已最小化 如果最小化则触发最大化
                    if (!page.myFiles.upFileDialogShow()) {
                        page.opts.upFileDialogHeadToolbarMin.trigger('click');
                        page.myFiles.upFileDialogShow(true);
                    }

                    setTimeout(function () {
                        $('#m_f_btnUpload').css('left', 0);
                    }, 1000);
                };

                //计算上传列表的个数
                w.getUploadQueueDetail = function () {
                    var slUploadQueueDetailStr = w.slProxy.Content.Uploader.GetUploadQueueDetail();

                    if (slUploadQueueDetailStr) {
                        try {
                            var uploadQueueDetailJson = JSON.parse(w.slProxy.Content.Uploader.GetUploadQueueDetail());
                            if (uploadQueueDetailJson.data) {
                                var resultQueueData = uploadQueueDetailJson.data;
                                page.uploadQueueDetail.totalCount = resultQueueData.TotalCount;
                                page.uploadQueueDetail.uploadingCount = resultQueueData.UploadingCount;
                                page.uploadQueueDetail.waitingCount = resultQueueData.WaitingCount;
                                page.uploadQueueDetail.faildCount = resultQueueData.FaildCount;
                                page.uploadQueueDetail.pendingCount = resultQueueData.PendingCount;
                                page.uploadQueueDetail.finishedCount = resultQueueData.FinishedCount;
                            }
                        } catch (e) {

                        }
                    }
                }

                //通知单个文件开始计时 此方法由Silverlight来调用
                w.StartSingleFileTimer = function (fileRow) {
                    var file = JSON.parse(fileRow).data,
                        upItem = findUpFileItem(page.myFiles.upFileItems, file, "GUID");
                    if (upItem.length) {
                        upItem[0].begin();
                    }
                };

                //更新单个文件信息
                w.UpdateSingleFileState = function (fileRow) {
                    var file = JSON.parse(fileRow).data,
                        upItem = findUpFileItem(page.myFiles.upFileItems, file, "GUID");
                    if (upItem.length) {
                        upItem = upItem[0];
                        if (file.Error) {
                            //upItem.upCompletedText.css('color', 'red').attr('title', file.Error).html('上传失败');
                            upItem.stop();

                            //upItem.upFileAndBegin.hide();
                            upItem.upFileAndBegin.css('visibility', 'hidden');

                            // upItem.upOperation.hide();
                            // upItem.upCompleted.show();
                        } else {
                            var percentage = file.Percentage + '%';
                            upItem.upFilePercent.html(percentage);
                            upItem.upProcessed.css('width', percentage);
                        }
                    }
                };

                //某个文件上传完成后
                w.UploadCompleted = function (fileRow) {
                    var file = JSON.parse(fileRow).data,
                        upItem = findUpFileItem(page.myFiles.upFileItems, file, "GUID");
                    if (upItem.length) {
                        upItem = upItem[0];
                        upItem.stop();
                        // upItem.upOperation.hide();
                        // upItem.upCompleted.show();

                        upItem.upFileAndBegin.css('visibility', 'hidden');

                        upItem.isFileTransfer = 0;//文件上传完成

                        page.shareStyle(true);
                        upItem.upLoadCheckBox.css('visibility', 'visible');
                        upItem.upLoadTxt.addClass('upload_text_content');
                        page.showUploadNumber();
                        page.tasks[file.GUID] = {
                            remoteFileInfo: {
                                "FileID": file.FileID,
                                "FileName": file.FileName
                            }
                        }
                    }
                };

                //全局文件信息
                w.UpdateGlobalFileInfo = function (globalInfo) {
                    //更新总进度
                    var global = JSON.parse(globalInfo).data;
                    page.myFiles.upFileGlobalCount(global.Count);
                    page.myFiles.upFileGlobalPercent(global.Percentage);
                };


                //返回当前用户的上传目录信息
                w.GetcurrentUploadPar = function () {
                    $('#m_f_btnUpload').css('left', -1000000);
                    var info = "", data;
                    if (!page.OutlookFolderID) {
                        $.ajax({
                            url: page.server.getuploadpar,
                            async: false,
                            type: 'GET',
                            dataType: 'json',
                            error: function () { },
                            success: function (json) {
                                var data = json.data;
                                page.OutlookFolderID = data.OutlookFolderID;
                            }
                        });
                    }

                    $.ajax({
                        url: '/ServiceApi/GetUploadPar',
                        async: false,
                        type: 'POST',
                        data: { FolderID: page.OutlookFolderID },
                        dataType: 'json',
                        error: function () { },
                        success: function (result) {
                            if (result.data != null && result.data != "") {
                                info = result.data;
                            }
                            else {
                                info = result.error;
                            }
                        }
                    });
                    return JSON.stringify(info);
                };

                //返回全局上传配置信息
                w.GetGlobalUploadSetting = function () {
                    var info = "";
                    $.ajax({
                        url: '/ServiceApi/GetGlobalUploadSetting',
                        async: false,
                        type: 'POST',
                        dataType: 'json',
                        error: function () {
                        },
                        success: function (result) {
                            if (result.data != null && result.data != "") {
                                info = result.data;
                            }
                        }
                    });
                    return JSON.stringify(info);
                };

                //上传出错警告
                w.ClientUploadWarInfo = function (wartext) {
                    var msg = win.currLanguageDic['silverlightError_' + wartext];
                    if (msg) {
                        $().prompt({
                            type: 'error',
                            msg: msg
                        });
                    }
                };

                w.DeleteAllUploading = function () {
                    if (w.slProxy != null) {
                        w.slProxy.Content.Uploader.DeleteAllUploading();
                    }
                };

                w.SendLyncMsg = function (data) {
                    if (w.slProxy) {
                        w.slProxy.Content.Uploader.SendLyncMsg('html', data);
                    }
                };
                w.GetDpi = function () {
                    return window.external.GetDpi();
                };
                w.GetZoom = function () {
                    return window.external.GetZoom();
                };
            }(win, app, page);
        } else {
            //html5 上传
            !function (app, page) {
                function FileHandle() {
                    var self = this;
                    this.beforeInit(function () {
                        self.init().initDomEvent();
                    });
                }

                FileHandle.prototype = {
                    beforeInit: function (callback) {
                        var self = this;
                        page.server.getGlobalUploadSetting.get(function (json) {
                            var gData = json.data;
                            self.maxUploads = gData.MaxUploads;
                            self.fileUploadBlackList = gData.FileUploadBlackList || '';
                            self.chunkSize = gData.ChunkSize;
                            self.allowDrop = gData.AllowDrop;
                            self.defaultExtension = gData.DefaultExtension;
                            self.maxFileSize = gData.MaxFileSize;
                            callback && callback();
                            //self.getFileUploadParams(callback);
                        });
                    },
                    init: function () {
                        this.mUploadArea = document.getElementById('mUploadArea');
                        this.mArraryWrapList = $('#mArraryWrapList');
                        this.mArraryWrapList.empty();
                        this.selFileUploadBtn = $('#selFileUploadBtn');
                        this.selFileUpload = $('#selFileUpload');

                        this.fileTemplate = '<div class="m_array_box" id="{id}">\
                                                <div class="m_array_top">\
                                                    <div class="upload_checkbox">\
                                                        <div class="upload_text"></div>\
                                                    </div>\
                                                    <div class="m_array_top2"></div>\
                                                    <div class="m_array_top3">{fileName}</div>\
                                                </div>\
                                                <div class="m_array_mid">\
                                                    <div class="m_array_mid1 txtHover"></div>\
                                                    <div class="m_array_mid2 txtHover"></div>\
                                                <div class="m_array_mid_right">\
                                                    <div class="m_array_mid3">{fileUpTime}</div>\
                                                    <div class="m_array_mid4"><span class="upSize">{fileUpSize}</span> of {fileSize}</div>\
                                                </div>\
                                                </div>\
                                                <div class="m_array_bottom">\
                                                    <div class="m_array_bottom_progress" style="width: {fileProgress}%"></div>\
                                                </div>\
                                           </div>';
                        this.tasks = {};
                        page.tasks = this.tasks;
                        page.tasksFileUpings = this.tasksFileUpings;
                        page.tasksFileUpingsCount = this.tasksFileUpingsCount;
                        page.tasksFileUnUploaded = this.tasksFileUnUploaded;
                        return this;
                    },
                    acceptFile: function (file) {
                        var rExt = /\.\w+$/,
                            fileBlackList = this.fileUploadBlackList.split(','),
                            fExt = rExt.exec(file.name);
                        var flag = fExt && (file.size || file.type);
                        if (flag) {
                            var fExtName = fExt[0].replace('.', '');
                            for (var i = 0; i < fileBlackList.length; i++) {
                                if (new RegExp(fExtName, 'i').test(fileBlackList[i])) {
                                    return false;
                                }
                            }
                        }
                        return flag;
                    },
                    initDomEvent: function () {
                        var self = this;
                        //上传文件选择
                        this.selFileUpload.on('change', function () {
                            var files = self.selFileUpload[0].files;
                            if (!files.length) { return; }
                            self.addFiles(files, function (newFiles) {
                                self.uploadHandler(newFiles);
                            });
                        });
                        this.selFileUploadBtn.on('click', function () {
                            self.selFileUpload.val('');
                            self.selFileUpload.trigger('click');
                        });


                        this.mUploadArea.ondragenter = function () {
                        };
                        this.mUploadArea.ondragleave = function () {
                            page.opts.dragArea.removeClass('dragClass');
                        };
                        this.mUploadArea.ondragover = function () {
                            page.opts.dragArea.addClass('dragClass');
                            return false;
                        };
                        //获取当前tasks中

                        this.mUploadArea.ondrop = function (e) {
                            page.opts.dragArea.removeClass('dragClass');
                            e.preventDefault();
                            if (self.allowDrop) {
                                var files = e.dataTransfer.files;
                                self.addFiles(files, function (newFiles) {
                                    self.uploadHandler(newFiles);
                                });
                            } else {
                                $().prompt({
                                    type: 'info',
                                    msg: win.currLanguageDic['upload_disabled'],
                                    backgroundSetting: { info: page.prompt.infoBackground }
                                });
                            }
                        }
                    },
                    upProgress: function (chunkIndex, chunks) { //进度条
                        this.progressDivIng.css('width', (chunkIndex / chunks) * 100 + '%');
                        this.upSize.text(((chunkIndex / chunks) * 100).toFixed(2) + '%');
                    },
                    timeHandle: function (timeStr) {
                        this.timeSpan.text(timeStr);
                    },
                    upComplete: function (data) {
                        var self = this;
                        this.progressDivIng.css('width', '100%');
                        this.upSize.text('100%');
                        this.upBtn.css('visibility', 'hidden');
                        //this.upFileCheckBox.prop('checked', true).show();
                        page.shareStyle(true);

                        this.upLoadCheckBox.css('visibility', 'visible');
                        this.upLoadTxt.addClass('upload_text_content');

                        this.remoteFileInfo = data;

                        //upload file size calc
                        // this.file.size = 0;

                        this.context.startUp();

                        page.showUploadNumber();

                        page.getDriveUse(function (data) {
                            var nowUsedQuota = (data.UserUsedQuota / data.UserQuota) * 100;
                            if (nowUsedQuota > 90) {
                                if (page.isQuotaOver == true) {
                                    $('.promptText').children('span').html(win.currLanguageDic['useSpace'] + Math.round(nowUsedQuota) + '%)');
                                    $('.promptClose').on('click.t', function () {
                                        page.isQuotaOver = false;
                                    })
                                } else {
                                    $().prompt({
                                        type: 'warning',
                                        msg: win.currLanguageDic['useSpace'] + Math.round(nowUsedQuota) + '%)',
                                        isAutoHide: 0
                                    });
                                    page.isQuotaOver = true;
                                    $('.promptClose').on('click.t', function () {
                                        page.isQuotaOver = false;
                                    })
                                }
                            }
                        });
                    },
                    remoteMd5FileHash: function (folderId, fileName, md5FileHash, callback) {
                        page.server.quickUpload.post({
                            FolderId: folderId,
                            FileName: fileName,
                            HashCode: md5FileHash
                        }, function (json) {
                            if (json.error == '3404') {
                                $().prompt({
                                    isAutoHide: 0,
                                    type: 'warning',
                                    msg: win.currLanguageDic['fileId_error']
                                });
                                return;
                            }
                            callback && callback(json.data);
                        });
                    },
                    getFileUploadParams: function (callback) {
                        var self = this;
                        page.server.getuploadpar.get(function (json) {
                            var data = json.data;
                            self.folderID = data.OutlookFolderID;
                            self.storageID = data.StorageID;
                            self.storageUri = data.StorageUri;
                            self.storageRelativePath = data.StorageRelativePath;

                            self.userQuota = data.UserQuota;
                            self.userUsedQuota = data.UserUsedQuota;
                            callback && callback();
                        });
                    },
                    pauseCallback: function (error) {
                        this.upBtn.removeClass('m_array_mid1_pause').addClass('m_array_mid1_start');
                        if (error) {
                            var msg;
                            if (error == '3404') {
                                $().prompt({
                                    type: 'warning',
                                    msg: win.currLanguageDic['errorCode_' + error]
                                });
                            }
                            if (error == 401 || error == 403) {
                                $().prompt({
                                    type: 'info',
                                    msg: win.currLanguageDic['sessionExpire'],
                                    backgroundSetting: { info: '#E0F4FF' }
                                });
                                setTimeout(function () {
                                    window.location.assign(app.global.defaultUri);
                                }, 2000);
                                return;
                            }

                            if (error == 404) {
                                msg = win.currLanguageDic['netWork_interrupted'];
                            } else {
                                msg = win.currLanguageDic['netWork_interrupted'] + error + ')';
                            }
                            if (page.netWorkInterrupTed == false) {
                                $().prompt({ type: 'error', msg: msg, isAutoHide: 0 });
                                page.netWorkInterrupTed = true;
                                $('.promptClose').on('click.t', function () {
                                    page.netWorkInterrupTed = false;
                                });
                            }
                        }
                    },
                    totalFileSize: function (files) {
                        var result = 0;
                        for (var i = 0; i < files.length; i++) {
                            result += files[i].size;
                        }
                        return result;
                    },
                    tasksFileSize: function () {
                        var self = this, result = 0;
                        for (var key in self.tasks) {
                            if (!self.tasks[key].isUploaded) {
                                result += self.tasks[key].file.size;
                            }
                        }
                        return result;
                    },
                    tasksFileUpings: function () {
                        var self = this, upingTasks = [];
                        for (var key in self.tasks) {
                            var curTask = self.tasks[key];
                            if (curTask.isUploading && !curTask.isUploaded) {
                                upingTasks.push(curTask);
                            }
                        }
                        return upingTasks;
                    },
                    tasksFileUpingsCount: function () {
                        var self = this, upingTasks = [];
                        for (var key in self.tasks) {
                            var curTask = self.tasks[key];
                            if ((curTask.isUploading || curTask.manualPaused) && !curTask.isUploaded) {
                                upingTasks.push(curTask);
                            }
                        }
                        return upingTasks.length;
                    },
                    tasksFileUnUploaded: function () { //没有上传完的文件个数
                        var self = this, unUploaded = [];
                        for (var key in self.tasks) {
                            var curTask = self.tasks[key];
                            if (!curTask.isUploaded) {
                                unUploaded.push(curTask);
                            }
                        }
                        return unUploaded.length;
                    },
                    tasksFilePause: function () {
                        var self = this, pauseTasks = [];
                        for (var key in self.tasks) {
                            var curTask = self.tasks[key];
                            if (!curTask.isUploaded && !curTask.isUploading && !curTask.manualPaused) {
                                pauseTasks.push(curTask);
                            }
                        }
                        return pauseTasks;
                    },
                    formatFileSize: function (fileSize, idx) {
                        var units = ["B", "KB", "MB", "GB"];
                        idx = idx || 0;
                        if (fileSize < 1024 || idx === units.length - 1) {
                            return fileSize.toFixed(1) + units[idx];
                        }
                        return this.formatFileSize(fileSize / 1024, ++idx);
                    },
                    addFiles: function (files, callback) {
                        var self = this,
                            newFiles = [],
                            arr = [],
                            arr1 = [];

                        if (!files || files.length == 0) {
                            $().prompt({ type: 'warning', msg: win.currLanguageDic['uploadFile_extension'] });
                            return;
                        }

                        for (var i = 0; i < files.length; i++) {
                            var file = files[i];
                            if (self.acceptFile(file)) {
                                if (file.size > self.maxFileSize) {
                                    arr.push(file);
                                } else {
                                    newFiles.push(file);
                                }
                            } else {
                                arr1.push(file);
                            }
                        }


                        if (arr1.length > 0) {
                            $().prompt({ type: 'warning', msg: win.currLanguageDic['uploadFile_extension'] });
                        }
                        if (arr.length > 0) {
                            $().prompt({ type: 'warning', msg: win.currLanguageDic['uploadFile_size'] });
                        }

                        //every upload file, get file upload params
                        //file upload completed,set current task file size zero
                        self.getFileUploadParams(function () {
                            if ((self.tasksFileSize() + self.totalFileSize(newFiles) + self.userUsedQuota) >= self.userQuota) {
                                $().prompt({ type: 'warning', msg: win.currLanguageDic['uploadFile_freeSpace'] });
                                return;
                            }
                            callback && callback(newFiles);
                        });
                    },
                    startUp: function () {
                        var self = this, tasksFileUpings = self.tasksFileUpings();
                        if (tasksFileUpings.length < self.maxUploads) {
                            var count = self.maxUploads - tasksFileUpings.length;
                            var tasksFilePause = self.tasksFilePause(), i = 0;
                            if (tasksFilePause.length <= count) {
                                for (i = 0; i < tasksFilePause.length; i++) {
                                    tasksFilePause[i].start();
                                    tasksFilePause[i].upBtn.addClass('m_array_mid1_pause');
                                }
                            } else {
                                for (i = 0; i < count; i++) {
                                    tasksFilePause[i].start();
                                    tasksFilePause[i].upBtn.addClass('m_array_mid1_pause');
                                }
                            }
                        }
                    },
                    uploadHandler: function (files) {
                        var x = 0, self = this;
                        for (x; x < files.length; x++) {
                            var taskName = app.uuid();
                            var file = files[x];
                            $('.noUploadFile').css('display', 'none');

                            self.tasks[taskName] = window.fileUpload({
                                file: file,
                                url: page.server.upload,
                                compileFileUrl: '',
                                autoUpload: 0,
                                isSendCompleteFile: 0,
                                taskName: taskName,
                                upProgress: self.upProgress,
                                upComplete: self.upComplete,
                                timeHandle: self.timeHandle,
                                remoteMd5FileHash: self.remoteMd5FileHash,
                                getFileUploadParams: self.getFileUploadParams,
                                pauseCallback: self.pauseCallback,
                                folderID: self.folderID,
                                storageID: self.storageID,
                                storageUri: self.storageUri,
                                storageRelativePath: self.storageRelativePath,
                                chunk: self.chunkSize,
                                context: self
                            });
                            var fileDom = app.formatJSON(self.fileTemplate, {
                                id: taskName,
                                fileName: file.name,
                                fileUpTime: '00:00:00',
                                fileUpSize: 0,
                                fileSize: self.formatFileSize(file.size),
                                fileProgress: 0
                            });
                            self.mArraryWrapList.prepend(fileDom);
                            var taskNameDom = $('#' + taskName);

                            self.tasks[taskName].taskNameDom = taskNameDom;
                            self.tasks[taskName].progressDivIng = taskNameDom.find('.m_array_bottom_progress');
                            self.tasks[taskName].timeSpan = taskNameDom.find('.m_array_mid3');
                            self.tasks[taskName].upBtn = taskNameDom.find('.m_array_mid1');
                            self.tasks[taskName].cancelBtn = taskNameDom.find('.m_array_mid2');
                            self.tasks[taskName].upSize = taskNameDom.find('.upSize');
                            self.tasks[taskName].upFileCheckBox = taskNameDom.find('.upFileCheckBox');


                            self.tasks[taskName].upLoadCheckBox = taskNameDom.find('.upload_checkbox');
                            self.tasks[taskName].upLoadTxt = taskNameDom.find('.upload_text');


                            self.tasks[taskName].upBtn.addClass('m_array_mid1_start');
                            self.tasks[taskName].upBtn.data('task', self.tasks[taskName]);
                            self.tasks[taskName].cancelBtn.data('task', self.tasks[taskName]);

                            self.tasks[taskName].upLoadTxt.on('click.t', function () {
                                var $this = $(this);
                                if ($this.hasClass('upload_text_content')) {
                                    $this.removeClass('upload_text_content');
                                } else {
                                    $this.addClass('upload_text_content');
                                }
                                page.shareBackground();
                            });

                            self.tasks[taskName].upBtn.on('click', function () {
                                var $this = $(this), currentUpTask = $this.data('task');

                                if (currentUpTask.isUploading) {
                                    currentUpTask.pause();
                                    currentUpTask.manualPaused = true;
                                    $this.removeClass('m_array_mid1_pause').addClass('m_array_mid1_start');
                                    self.startUp();
                                } else {
                                    var tasksFileUpings = currentUpTask.context.tasksFileUpings();
                                    if (tasksFileUpings.length >= currentUpTask.context.maxUploads) {
                                        $().prompt({
                                            type: 'warning',
                                            msg: win.currLanguageDic['uploadfile_sameTime_head'] + currentUpTask.context.maxUploads + win.currLanguageDic['uploadfile_sameTime_foot']
                                        });
                                        return;
                                    }
                                    currentUpTask.manualPaused = false;
                                    currentUpTask.start();
                                    $this.removeClass('m_array_mid1_start').addClass('m_array_mid1_pause');
                                }
                            });

                            self.tasks[taskName].cancelBtn.on('click', function () {
                                if (page.opts.arrayWrap.children().length == 1) {
                                    page.shareStyle(false);
                                    $('.noUploadFile').css('display', 'block');
                                }
                                var currentUpTask = $(this).data('task');

                                if (currentUpTask.isUploaded) {
                                    currentUpTask.taskNameDom.remove();
                                } else {
                                    currentUpTask.pause();
                                    currentUpTask.taskNameDom.remove();
                                    delete currentUpTask.context.tasks[currentUpTask.taskName];
                                    self.startUp();
                                    page.showUploadNumber();
                                    page.server.cancelUpload.post({
                                        StorageUri: currentUpTask.storageUri,
                                        StorageRelativePath: currentUpTask.storageRelativePath,
                                        Identifier: currentUpTask.taskName
                                    }, function (json) {
                                        if (json.data) {
                                        }
                                    });
                                }
                                page.shareBackground();
                            });
                        }
                        self.startUp();
                        page.showUploadNumber();
                    }
                };
                new FileHandle();
            }(app, page);
        }
    }

    uploadInit();

    //写入mailbody
    win.setItemBody = function (body, callback) {
        if (win.ribbon) {
            win.external.InsertAttach(body);
            $.isFunction(callback) && callback();
            page.isShareing = false;
            return;
        }
        if (win.mailItem) {
            win.mailItem.body.getTypeAsync(function (result) {
                if (result.status == Office.AsyncResultStatus.Failed) {
                    $().prompt({ type: 'warning', msg: '获取当前邮件类型出错！' });
                    return;
                }
                var bodyParams = {
                    coercionType: Office.CoercionType.Text,
                    asyncContext: {
                        var3: 1,
                        var4: 2
                    }
                };

                if (result.value == Office.MailboxEnums.BodyType.Html) {
                    bodyParams.coercionType = Office.CoercionType.Html;
                } else if (result.value == Office.MailboxEnums.BodyType.Text) {
                    body = body.replace(/<(.|\n)*?>/gi, '');
                    body = body.replace(/&nbsp;/gi, '');
                    body = $.trim(body);
                }

                win.mailItem.body.setSelectedDataAsync(body, bodyParams, function (asyncResult) {
                    if (asyncResult.status == Office.AsyncResultStatus.Failed) {
                        $().prompt({
                            type: 'error',
                            msg: win.currLanguageDic['shareLink_failes'],
                            isAutoHide: 0
                        });
                        page.isShareing = false;
                        return;
                    }
                    $.isFunction(callback) && callback();
                });
            });
        }
    };

    win.onload = function () {
        win.languageFun(win.currLanguageDic); // 初始化语言
        //初始化office
        app.global.debug ? (function () {
            win.initPage();
        })() : (function () {
            if (!win.ribbon) {
                Microsoft.Office.WebExtension.initialize = function ($p1_0) {
                    win.mailItem = Office.context.mailbox.item;
                    win.initPage();
                };
            } else {
                win.initPage();
            }
        })();
    };
}(jQuery, window.app, window);
