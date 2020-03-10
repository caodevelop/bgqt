!function ($, app, win) {
    var page = app.initPage({
        pageInit: function () {
            var _ = this,
                opts = _.opts;
            opts.currentEmailAddress = win.currEmailAddr;
            if (app.request['language']) {
                opts.language = app.request['language'];
            } else {
                opts.language = app.cookie('language') || _.getLanguage();
            }

            var cookieUsers = app.cookie('BGQTUserToken')('Token');
            if (cookieUsers) {
                var users = JSON.parse(cookieUsers);

                if (users.indexOf(win.currEmailAddr) !== -1) {
                    _.autoSignin(function () {
                        _.redirectMain(opts.language, opts.currentEmailAddress);
                    });
                    return;
                }
            }
            else {
                _.autoSignin(function () {
                    _.redirectMain(opts.language, opts.currentEmailAddress);
                });
            }

            opts.lDefaultLoading.hide();
            opts.lDemoMainWrap.show();

            opts.agreeCheckBox.on('click.t', function () {
                if (opts.agreeTxt.hasClass('agreeState_pic')) {
                    opts.agreeTxt.removeClass('agreeState_pic');
                    page.beginBtn(false);
                } else {
                    opts.agreeTxt.addClass('agreeState_pic');
                    page.beginBtn(true);
                }
            });
            $.getScript('lang/' + opts.language + '.js', function () {
                win.languageFun(win.currLanguageDic);
                if (opts.language && opts.language == app.global.cn) {
                    opts.buoy.css('float', 'left').show();
                } else {
                    if (!opts.language) {
                        opts.buoy.hide();
                        return;
                    }
                    opts.buoy.css('float', 'right').show();
                }

                opts.languageItem.on('click.t', function () {
                    var $this = $(this),
                        $thisLang = $this.attr('lang');
                    if ($thisLang != opts.language) {
                        win.location.assign('demo.htm?language=' + $thisLang);
                    }
                });
            });
        },
        getLanguage: function () {
            if (win.language) {
                if (win.language != app.global.cn) {
                    win.language = app.global.en;
                }
                return win.language;
            }
            return app.global.defaultLanguage;
        },
        autoSignin: function (callback) {
            this.server.autoSignin.post({ mailAddress: this.opts.currentEmailAddress }, function (json) {
                if ($.isFunction(callback)) {
                    callback(json.data || '');
                }
            });
        },
        redirectMain: function (language, userName) {
            win.location.assign(app.global.mainUri
                + '?language=zh-CN' //+ language
                + '&userName=' + userName
                + '&hidesignout=' + '1');
        },
        beginClick: function () {
            var _ = this,
                opts = _.opts;
            if (page.beginBtn() == true) {
                app.cookie('language', opts.language, {
                    expires: app.global.cookieExpire,
                    path: app.global.cookieRootPath
                });
                _.autoSignin(function (data) {
                    var cookieUsers = app.cookie('users');
                    if (cookieUsers) {
                        var users = JSON.parse(cookieUsers);
                        if (users.indexOf(win.currEmailAddr) === -1) {
                            users.push(win.currEmailAddr);
                            app.cookie('users', users, {
                                expires: app.global.cookieExpire,
                                path: app.global.cookieRootPath
                            });
                        }
                    } else {
                        app.cookie('users', [win.currEmailAddr], {
                            expires: app.global.cookieExpire,
                            path: app.global.cookieRootPath
                        });
                    }
                    _.redirectMain(opts.language, opts.currentEmailAddress);
                });
            }
        }
    });
    page.agreeOpen = function () {
        page.maskOpen(true);
        page.opts.demoStatement.animate({
            height: 280
        });
    };
    page.maskClose = function () {
        page.opts.demoStatement.animate({
            height: 0
        }, function () {
            page.maskOpen(false);
        });
    };
    win.languageFun = function (textObj, strId) {
        var elem, document = window.document;
        if (strId === undefined) {
            var key;
            for (key in textObj) {
                var x = key.indexOf("::");
                if (x !== -1) {
                    var id = key.substr(0, x), tag = key.substr(x + 2);
                    elem = $(tag, "#" + id);
                    for (var f = 0, g = elem.length; f < g; f++) {
                        var el = elem[f];
                        el.innerText ? (el.innerText = textObj[key][f]) : (el.textContent = textObj[key][f]);
                    }
                }
                else {
                    x = key.indexOf("@");
                    if (x > -1) {
                        var id = key.substr(0, x), attr = key.substr(++x);
                        if (id === 'HTML') {
                            document[attr] = textObj[key];
                        }
                        else
                            $("#" + id).attr(attr, textObj[key]);
                    }
                    else {
                        elem = document.getElementById(key);
                        if (elem) $(elem).html(textObj[key]);
                    }
                }
            }
        } else {
            var tId;
            for (var i = 1, l = arguments.length; i < l; i++) {
                tId = arguments[i];
                elem = document.getElementById(tId);
                if (elem && textObj[tId]) $(elem).html(textObj[tId]);
            }
        }
    };

    page.server = page.server(app.global.debug ? {
        autoSignin: '/data/autosignin.json'
    } : {
            autoSignin: 'api/setting.ashx?op=AutoLogin'
        });

    page.beginBtn = ko.observable(false);
    page.maskOpen = ko.observable(false);

    win.initPage = function () {
        page.onReady({
            'languageItem': $('#languageItem').find('li'),
            'buoy': $('#buoy'),
            'agreeCheckBox': $('.agreeState_checkbox'),
            'agreeTxt': $('.agreeState_text'),
            'agreeOpen': $('.agreeState_open'),
            'demoMask': $('.l_demo_mask'),
            'demoStatement': $('.m_demo_statement'),
            'lDefaultLoading': $('#lDefaultLoading'),
            'lDemoMainWrap': $('#lDemoMainWrap')
        });
        ko.applyBindings(page);
    };
    win.onload = function () {
        app.global.debug ? (function () {
            if (window.ribbon) {
                win.currEmailAddr = window.external.GetCurrentMailAddress;
                win.language = window.external.GetMailItemLanguage == '2052' ? 'zh-CN' : 'en-US';
                win.initPage();
            } else {
                win.currEmailAddr = "zhuxiaojian@it-first.com.cn";
                win.language = "zh-CN";
                win.initPage();
            }
        })() : (function () {
            if (window.ribbon) {
                win.currEmailAddr = window.external.GetCurrentMailAddress;
                win.language = window.external.GetMailItemLanguage == '2052' ? 'zh-CN' : 'en-US';
                win.initPage();
            } else {
                Microsoft.Office.WebExtension.initialize = function ($p1_0) {
                    win.currEmailAddr = Office.context.mailbox.userProfile.emailAddress;
                    win.language = Office.context.displayLanguage;
                    //win.language = 'zh-CN';
                    win.initPage();
                };
            }
        })();
    };

}(jQuery, window.app, window);