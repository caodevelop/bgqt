!function ($, app, win) {
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
    win.loadFile = function (filename) {
        var ss = document.getElementsByTagName("script");
        for (var i = 0; i < ss.length; i++) {
            if (ss[i].src && ss[i].src.indexOf(filename) != -1) {
                return;
            }
        }
        var fileRef = document.createElement('script');
        document.getElementsByTagName("head")[0].appendChild(fileRef);
        fileRef.setAttribute("type", "text/javascript");
        fileRef.setAttribute("src", filename);
        return fileRef;
    };
    win.language = app.request['language'] || '';

    win.currLanguageDic = {};
    app.getCurrentLanguageByKey = function (key) {
        return win.currLanguageDic[key] || '';
    };
    if (!win.language) {
        win.currLanguageDic = {};
        return;
    }
    var lanFile = '../lang/' + win.language + '.js';
    win.loadFile(lanFile);
}(jQuery, window.app, window);
