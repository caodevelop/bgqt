!function ($, app, win) {
    app.errorFn = function (jqXhr, textStatus, errorThrown) {
        var code = jqXhr.status;
        if (code === 0 || code === 200) return;
        if (code === 401 || code === 403) {
            page.initShareFileDetail();
        }
        if (code === 500 || code === 302) {
            window.location.assign('../downloadnotfound.htm');
        }
    };

    var page = app.initPage({
        pageInit: function () {
            var _ = this,
                opts = _.opts;
                page.loginCodeShow();
            win.url = app.request['code'] || '';
            win.code = '';
            if (!win.url || win.url === 'undefined') {
                window.location.assign('../downloadnotfound.htm');
                return;
            }
            opts.language = 'zh-CN';

            $.getScript('lang/' + opts.language + '.js', function () {
                page.getShareFiles(function (files) {
                    // files = false;
                    if (files) {
                        page.getShareDetail(function (data) {
                            opts.m_centent_l_side.removeClass('disNone');
                            if (data.NeedAuthenticatioe) {
                                page.show_signin(true);
                            }
                            if (data.NeedVerificationCode) {
                                page.show_identifying_code(true);
                            }
                            page.bindShareFiles(files);
                            opts.m_centent_l_side.find('li').eq(2).addClass("current").siblings().removeClass('current');
                            $('.m_download_File').removeClass('disNone').siblings().addClass('disNone');
                            $('.m_download_File_btn').removeClass('disNone').siblings().addClass('disNone');

                            _.time(data.ExpireTime);
                        });
                    } else {
                        page.initShareFileDetail();
                    }
                });
            });
            opts.m_identifying_code_text.on('keydown.t', function (event) {//验证码回车
                _.checkVerificationCode_btn();
                event = event || win.event;
                var keyCode = (event.which ? event.which : event.keyCode);
                if (keyCode == 13) {
                    if (opts.m_identifying_code_btn.hasClass('next_btn_color')) {
                        var code = $.trim(opts.m_identifying_code_text.val());
                        _.checkVerificationCode(code);
                    }
                }
            });
        },
        initShareFileDetail: function () {
            var _ = page, opts = _.opts;
            if (win.dPrompt) {
                win.dPrompt.opts.hideCallback();
            }

            _.show_signin(false);
            _.show_identifying_code(false);
            _.show_download_File(false);
            ko.mapping.fromJS([], _.sharefiles);
            opts.m_centent_l_side.find('li').removeClass('current');
            _.downloadPackaged(false);


            _.all(false);
            _.choice();
            // opts.m_signin_username_text.on('keyup', function () {
            //     _.singIn_btn();
            // });
            // opts.m_signin_password_text.on('keyup', function () {
            //     _.singIn_btn();
            // });
            // opts.m_signin_code_text.on('keyup', function () {
            //     _.singIn_btn();
            // });
            // opts.m_identifying_code_text.on('keyup', function () {
            //     _.checkVerificationCode_btn();
            // });
        },
        getShareDetail: function (callback) {
            page.server.getsharedetail.get({
                code: win.url
            }, function (json) {
                callback(json.data);
            });
        },
        choice: function () {//选择信息
            var _ = this,
                opts = _.opts;
            page.getShareDetail(function (data) {
                if (data.NeedAuthenticatioe) {
                    opts.m_centent_l_side.removeClass('disNone');
                    opts.m_centent_l_side.find("li").eq(0).addClass("current");
                    $('.m_signin').removeClass('disNone');
                    opts.m_signin_btn.removeClass('disNone');
                    if (data.NeedVerificationCode) {
                        page.show_signin(true);
                        page.show_identifying_code(true);
                        opts.m_signin_btn.on("click.t", function () {
                            _.singIn_btn();
                            var u = $.trim(opts.m_signin_username_text.val()),
                                p = opts.m_signin_password_text.val(),
                                c = opts.m_signin_code_text.val();
                            if(page.loginFailedNumber >= app.global.loginNumber){
                                if (u && p && c) {
                                    _.singIn(u, p, true, c);
                                }
                            }else{
                                if (u && p) {
                                    _.singIn(u, p, true, c);
                                }
                            }
                        });
                        $('.m_signin_username_text,.m_signin_password_text,.m_signin_code_text').on('keydown.t', function (event) {
                            _.singIn_btn();
                            event = event || win.event;
                            var keyCode = (event.which ? event.which : event.keyCode);
                            if (keyCode == 13) {
                                opts.m_signin_btn.trigger('click.t');
                            }
                        });
                        opts.m_identifying_code_btn.on("click", function () {
                            if (opts.m_identifying_code_btn.hasClass('next_btn_color')) {
                                var code = $.trim(opts.m_identifying_code_text.val());
                                _.checkVerificationCode(code);
                            }
                        });
                    } else {
                        page.show_signin(true);
                        page.show_identifying_code(false);
                        opts.m_signin_btn.on("click.t", function () {
                            _.singIn_btn();
                            var u = $.trim(opts.m_signin_username_text.val()),
                                p = opts.m_signin_password_text.val(),
                                c = opts.m_signin_code_text.val();
                            if(page.loginFailedNumber >= app.global.loginNumber){
                                if (u && p && c) {
                                    _.singIn(u, p, false, c);
                                }
                            }else{
                                if (u && p) {
                                    _.singIn(u, p, false, c);
                                }
                            }
                        });
                        $('.m_signin_username_text,.m_signin_password_text,.m_signin_code_text').on('keydown.t', function (event) {
                            _.singIn_btn();
                            event = event || win.event;
                            var keyCode = (event.which ? event.which : event.keyCode);
                            if (keyCode == 13) {
                                opts.m_signin_btn.trigger('click.t');
                            }
                        });
                        
                    }
                } else {
                    opts.m_centent_l_side.removeClass('disNone');
                    if (data.NeedVerificationCode) {
                        page.show_signin(false);
                        page.show_identifying_code(true);
                        opts.m_centent_l_side.find("li").eq(1).addClass("current");
                        $('.m_identifying_code').removeClass('disNone');
                        $('.m_identifying_code_btn').removeClass('disNone');
                        opts.m_identifying_code_btn.on("click", function () {
                            if (opts.m_identifying_code_btn.hasClass('next_btn_color')) {
                                var code = $.trim(opts.m_identifying_code_text.val());
                                _.checkVerificationCode(code);
                            }
                        });
                    } else {
                        page.show_signin(false);
                        page.show_identifying_code(false);
                        page.bindShareFiles();
                        opts.m_centent_l_side.find('li').eq(2).addClass("current");
                        $('.m_download_File').removeClass('disNone');
                        $('.m_download_File_btn').removeClass('disNone');
                    }
                }
                _.time(data.ExpireTime);
            });
        },
        checked: function (file) {
            for (var i = 0; i < file.length; i++) {
                var tempData = file[i];
                tempData['isChecked'] = false;
            }
        }
    });

    page.time = function (time) {
        var shareDateStr = time;
        if (/^9999+/.test(shareDateStr)) {
            page.opts.timeLimit.html("永久").show();
        } else {
            page.opts.timeLimit.html(shareDateStr).show();
        }
    };
    $("input").on('focus blur', function (event) {//文本框焦点
        if (event.type == "focus") {
            $(this).css("border-color", "#72afe5");
        } else if (event.type == "blur") {
            $(this).css("border-color", "#ababab");
        }
    });
    page.singIn = function (userName, passWord, flag, authcode) {// 用户名，密码
        var _ = this,
            opts = _.opts;
        page.server.signin.post({
            "accountName": userName,
            "password": passWord,
            "uri": win.url,
            authcode: authcode
        }, function (json) {
            if (json.data) {
                app.cookie('loginFailed', '', {
                    expires: -1,
                    path: app.global.cookieRootPath
                });
                if (flag) {
                    opts.m_centent_l_side.find('li').eq(1).addClass("current").siblings().removeClass('current');
                    $('.m_identifying_code').removeClass('disNone').siblings().hide();
                    $('.m_identifying_code_btn').removeClass('disNone').siblings().addClass('disNone');
                } else {
                    _.bindShareFiles();
                    opts.m_centent_l_side.find('li').eq(2).addClass("current").siblings().removeClass('current');
                    $('.m_download_File').removeClass('disNone').siblings().hide();
                    $('.m_download_File_btn').removeClass('disNone').siblings().addClass('disNone');
                }
            }
        })
    };
    page.singIn_btn = function () {//用户名按钮
        var _ = this,
            opts = _.opts;
        opts.loginError.empty(); 
        setTimeout(function () {
            if(page.loginFailedNumber >= app.global.loginNumber){
            page.showNextBtn((opts.m_signin_username_text.val() && opts.m_signin_password_text.val() && opts.m_signin_code_text.val()));
            }else{
                page.showNextBtn((opts.m_signin_username_text.val() && opts.m_signin_password_text.val()));
            }
        }, 30);
    };
    page.checkVerificationCode_btn = function () {
        var _ = this,
            opts = _.opts;
        opts.codeError.empty();
        // if (opts.m_identifying_code_text.val().length > 0) {
        //     if (!opts.m_identifying_code_btn.hasClass('next_btn_color')) {
        //         opts.m_identifying_code_btn.addClass('next_btn_color');
        //     }

        // } else {
        //     opts.m_identifying_code_btn.removeClass('next_btn_color');

        // }
        setTimeout(function(){
            page.showCodeNext(opts.m_identifying_code_text.val());
        },30);      
    };
    page.checkVerificationCode = function (code) {//验证码请求
        var _ = this,
            opts = _.opts;
        page.server.checkVerificationCode.get({
            code: win.url || '',
            VerificationCode: code
        }, function (json) {
            if (json.data) {
                win.code = code;
                _.bindShareFiles();
                opts.m_centent_l_side.find('li').eq(2).addClass("current").siblings().removeClass('current');
                $('.m_identifying_code').hide();
                $('.m_identifying_code_btn').addClass('disNone');
                $('.m_download_File_btn').removeClass('disNone');
                $('.m_download_File').removeClass('disNone');
            } else {
                page.opts.codeError.html('验证码不正确').show();
            }
        })
    };

    page.getShareFiles = function (callback) {
        page.server.getsharefiles.get({
            code: win.url || '',
            valcode: win.code || ''
        }, function (json) {
            var files = json.data;
            callback(files);
        });
    };

    page.bindShareFiles = function (files) {
        if (files) {
            page.checked(files);
            ko.mapping.fromJS(files, page.sharefiles);
            page.show_download_File(true);
        } else {
            page.getShareFiles(function (files) {
                page.checked(files);
                ko.mapping.fromJS(files, page.sharefiles);
               page.show_download_File(true);
            });
        }
    };

    page.checkList = function (data, event) { //点击列表行改变isChecked的状态
        var e = window.event || event;
        window.event ? e.cancelBubble = true : e.stopPropagation();
        data.isChecked(!data.isChecked());

        page.btnClor(data.isChecked());
        var flag = true;
        var flagBtn = false;
        for (var i = 0; i < page.sharefiles().length; i++) {
            if (page.sharefiles()[i].isChecked() != true) {
                flag = false;
            } else {
                flagBtn = true;
            }
        }
        page.all(flag);
        page.btnClor(flagBtn);
    };
    page.checkOne = function (data, event) {//单击只选一个
        var e = window.event || event;
        window.event ? e.cancelBubble = true : e.stopPropagation();
        page.all(false);
        data.isChecked(false);
        if (!this.isChecked()) {
            for (var i = 0; i < page.sharefiles().length; i++) {
                page.sharefiles()[i].isChecked(false);
            }
            data.isChecked(true);
        }
        page.btnClor(true);
        if (page.sharefiles().length === 1 && data.isChecked()) {
            page.all(true);
        }
    };
    page.allCheck = function () {//全选
        page.all(!page.all());
        if (page.all()) {
            var i;
            for (i = 0; i < page.sharefiles().length; i++) {
                page.sharefiles()[i].isChecked(true);
                page.btnClor(true);
            }

        } else {
            for (i = 0; i < page.sharefiles().length; i++) {
                page.sharefiles()[i].isChecked(false);
                page.btnClor(false);
            }
        }

    };
    page.showIcon = function (item) {//文件图标
        var extensionName = item.ExtensionName().replace('.', '').toLowerCase();
        //后缀变成小写
        var iconClass = page.fileIconClass[extensionName] || 'fileFile';
        if (iconClass)
            return iconClass;//
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


    page.downloadSingleFile = function (data, event) {//单击下载
        var downloadUrl = win.location.protocol + "//" + win.location.host + page.server.singleFileDownload + '&fileid=' + data.ObjectID();
        downloadUrl += '&code=' + app.request['code'] || '';
        window.open(downloadUrl);
    };
    page.downloadClick = function (data, event) {//批量下载
        if (page.downloadPackaged()) {
            return;
        }
        var downloadArr = [], i;
        for (i = 0; i < page.sharefiles().length; i++) {
            var currentFile = page.sharefiles()[i];
            if (currentFile.isChecked()) {
                downloadArr.push(currentFile);
            }
        }

        if (downloadArr.length <= 0) {
            return;
        }

        if (downloadArr.length == 1) {
            page.downloadSingleFile(downloadArr[0], event);
            return;
        }

        var downloadFileIds = [];
        for (i = 0; i < downloadArr.length; i++) {
            var file = downloadArr[i];
            downloadFileIds.push(file.ObjectID());
        }


        var downloadField = $('<input type="hidden" />'),
            downloadFieldUri = $('<input type="hidden"/>'),
            downloadForm = $('<form class="hide" method="post"></form>')
                .append(downloadFieldUri).append(downloadField).appendTo('body');
        win.dPrompt = $().prompt({
            iconPath: 'images/',
            msg: '正在打包所选的文件，请稍后',
            hideCallback: function () {
                dPrompt.hide();
            }
        }).prompt;
        page.btnClor(false);
        page.downloadPackaged(true);
        var fileName = app.formatDate(new Date(), "yyyyMMddhhmmssS");
        page.server.buildCompressFiles.post({
            fileIds: downloadFileIds.join(','),
            //zipName: downloadArr[0].DisplayName() + '.zip',
            zipName: 'package' + fileName + '.zip',
            uri: win.url
        }, function (json) {
            win.dPrompt.upMsg('完成打包所选的文件，请准备下载');
            page.btnClor(true);
            page.downloadPackaged(false);
            var data = json.data;
            var filePath = encodeURIComponent(data.FilePath);
            var downloadUrl = win.location.protocol + "//" + win.location.host + page.server.downloadZip;
            downloadUrl += '?filePath=' + filePath;
            downloadUrl += '&uri=' + win.url;

            //兼容IE8
            var downloadWindow = window.open(downloadUrl,'_blank');
            downloadWindow.location.href = downloadUrl;

            win.dPrompt.opts.hideCallback();
            win.dPrompt = null;
        });
    };

    page.server = page.server(app.global.debug ? {
        getsharedetail: '/data/getsharedetail.json',
        checkVerificationCode: '/data/CheckVerificationCode.json',
        signin: '/data/signin.json',
        getsharefiles: '/data/getsharefiles.json',
        singleFileDownload: '/data/singleFileDownload.json',
        buildCompressFiles: '/data/buildCompressFiles.json',
        downloadZip: 'data/DownloadZip.json',
        authCode: 'https://exdriveplus.ifcloud.com/api/account/authCode'
    } : {
            getsharedetail: 'api/file.ashx?op=GetShareDetail', //链接详细信息
        signin: 'api/account/signin ',
        getsharefiles: 'api/file.ashx?op=GetShareFiles',
            checkVerificationCode: 'api/file.ashx?op=CheckVerificationCode',
            singleFileDownload: 'api/file.ashx?op=DownloadFileById',
        buildCompressFiles: 'api/share/BuildCompressFiles',
        downloadZip: 'api/share/DownloadZip',
        authCode: 'api/account/authCode'
    });

    page.loginFailedNumber = app.cookie('loginFailed') || 0;
    page.server.signin.onError = function (error) {
        var loginInfo = '';
        if (error == '1001') {
            loginInfo = win.currLanguageDic['loginJudge_empty'];
        }
        if (error == '1002') {
            loginInfo = win.currLanguageDic['loginJudge_enabled'];
        }
        if (error == '1003') {
            loginInfo = win.currLanguageDic['loginJudge_exist'];
        }
        if (error == '1004') {
            loginInfo = win.currLanguageDic['loginJudge_disabled'];
        }
        if (error == '1005') {
            loginInfo = win.currLanguageDic['loginJudge_incorrect'];
        }
        if(error == '1006'){
            loginInfo =  win.currLanguageDic['loginJudge_signinCode'];
        }
        if (error == '2000') {
            loginInfo = win.currLanguageDic['loginJudge_encount'];
        }
        page.opts.loginError.html(loginInfo).show();

        page.loginFailedNumber++;
        page.loginCodeShow();
        app.cookie('loginFailed', page.loginFailedNumber, {
            expires: app.global.cookieExpire,
            path: app.global.cookieRootPath
        });

        return false;
    };
    page.loginCodeShow = function(){ //判断是否显示验证码
        var _ = this,
            opts = _.opts;
        if(page.loginFailedNumber >= app.global.loginNumber){ 
            opts.codePic.attr('src', page.returnAuthCodeRefresh());
            opts.loginCode.show();
             page.showNextBtn((opts.m_signin_username_text.val() && opts.m_signin_password_text.val() && opts.m_signin_code_text.val()));  
        }
    };
    page.codeRefresh = function(){ //重新获取验证码
        var _ = this,
            opts = _.opts;
        opts.code.val('');
        page.showNextBtn(false);
        opts.codePic.attr('src', page.returnAuthCodeRefresh());
    };
    page.returnAuthCodeRefresh = function(){
        return page.server.authCode + '?t='+(new Date()).getTime();
    }

    page.server.checkVerificationCode.onError = function (err) {
        page.opts.codeError.html(err).show();
    };
    page.server.buildCompressFiles.onError = function (err) {
        if (err === '302') {
            window.location.assign('../downloadnotfound.htm');
            return;
        }
        page.btnClor(true);
        page.downloadPackaged(false);
        $().prompt({
            iconPath: 'images/',
            msg: '打包下载出现问题，请稍后再试或联系管理员（原因:' + err + ')',
            type: 'error'
        });
        if (win.dPrompt) {
            win.dPrompt.opts.hideCallback();
        }
    };
    page.server.getsharefiles.onError = function (err) {
        window.location.assign('../downloadnotfound.htm');
    };
    page.show_signin = ko.observable(false);
    page.show_identifying_code = ko.observable(false);
    page.show_download_File = ko.observable(false);
    page.all = ko.observable(false);
    page.btnClor = ko.observable(false);
    page.downloadPackaged = ko.observable(false);
    page.showNextBtn = ko.observable(false);

    page.showCodeNext = ko.observable(false);

    page.sharefiles = ko.mapping.fromJS([]);
    win.onload = function () {
        page.onReady({
            m_centent_l_side: $('.m_centent_l_side'),
            'm_signin_btn': $(".m_signin_btn"),
            'm_signin_username_text': $(".m_signin_username_text"),
            'm_signin_password_text': $(".m_signin_password_text"),
            'm_signin_code_text':$('.m_signin_code_text'),
            'm_identifying_code_text': $(".m_identifying_code_text"),
            'm_identifying_code_btn': $(".m_identifying_code_btn"),
            'loginError': $("#loginError"),
            'codeError': $("#codeError"),
            'timeLimit': $("#timeLimit"),
            code:$('#code'),
            codePic:$('.s_code_pic'),
            loginCode:$('.m_signin_code')
        });

        ko.applyBindings(page);
    }
}(jQuery, window.app, window);