/**
 * User: tom
 * Date: 2015-3-23
 * Time: 14:39
 */
!function ($) {
    var Prompt = function (options) {
        this.opts = $.extend({}, Prompt.defaults, options || {});
        this.create();
    };
    Prompt.zIndex = 1000;
    Prompt.defaults = {
        isAutoHide: 1,
        html: '<div class="promptMain"><div class="promptIcon"></div><div class="promptText"><span></span></div><div class="promptClose txtHover"></div></div>',
        type: 'info',
        iconPath: '../images/',
        msgColor: '#333333',
        delay: 2000,
        sDelay: 500,//显示收缩所要的时间
        backgroundSetting: {
            info: '',
            warning: '',
            error: ''
        }
    };
    Prompt.prototype = {
        create: function () {
            var _ = this, prompt = $(_.opts.html),
                promptMain = prompt.find('.promptMain'),
                promptIcon = prompt.find('.promptIcon'),
                promptText = prompt.find('.promptText').find('span'),
                promptClose = prompt.find('.promptClose'),
                icon, closeIcon, id = 'PROMPT' + new Date().getMilliseconds();
            switch (_.opts.type) {
                case 'info':
                    icon = 'img_success_24.png';
                    break;
                case 'warning':
                    icon = 'img_warning_24.png';
                    break;
                case 'error':
                    icon = 'img_error_24.png';
                    msgColor = 'red';
                    break;
            }
            icon = _.opts.iconPath + icon;

            promptIcon.css({
                'background-image': 'url(' + icon + ')',
                'background-repeat': 'no-repeat',
                'background-position-x': 'center',
                'background-position-y': 'center'
            });

            if (_.opts.backgroundSetting[_.opts.type]) {
                prompt.css('backgroundColor', _.opts.backgroundSetting[_.opts.type]);
            }

            if (!_.opts.isAutoHide) {
                closeIcon = _.opts.iconPath + 'close_icon.png';
                promptClose.css({
                    'background': 'url(' + closeIcon + ')',
                    'background-repeat': 'no-repeat',
                    'background-position-x': 'center',
                    'background-position-y': 'center'
                }).on('click', function () {
                    _.hide();
                });
            } else {
                if (!_.opts.hideCallback) {
                    setTimeout(function () {
                        _.hide();
                    }, _.opts.delay);
                }
            }

            if (_.opts.msg) {
                promptText.css('color', _.opts.msgColor).html(_.opts.msg);
                promptText.attr('title', _.opts.msg);
            }

            prompt.attr('id', id).attr('z-index', Prompt.zIndex);
            Prompt.zIndex++;
            prompt.hide().appendTo($('body')).slideDown(_.opts.sDelay);

            _.id = id;
            _.prompt = prompt;
            _.promptText = promptText;
        },
        hide: function () {
            var _ = this;
            _.prompt.slideUp(_.opts.sDelay, function () {
                $(this).remove();
            });
        }, upMsg: function (msg) {
            this.promptText.html(msg);
        }
    };
    $.extend($.fn, {
        prompt: function (options) {
            var $this = $(this);
            $this.prompt = new Prompt(options);
            return $this;
        }
    });
}(jQuery);
