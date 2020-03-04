/**
 * User: tom
 * Date: 2015-3-05
 * Time: 18:05
 */
!function () {
    ko.bindingHandlers.showIcon = {
        update: function (element, valueAccessor, allBindingsAccessor) {
            var value = ko.utils.unwrapObservable(valueAccessor());
            $(element).addClass(value);
        }
    };

    ko.bindingHandlers.showDate = {
        update: function (element, valueAccessor, allBindingsAccessor) {
            var value = ko.utils.unwrapObservable(valueAccessor());
            value = app.formatDate(app.parseDate(value));
            ko.bindingHandlers.text.update(element, function () {
                return value;
            });
        }
    };

    ko.bindingHandlers.fadeVisible = {
        update: function (element, valueAccessor) {
            var value = valueAccessor(),
                jq = $(element);
            if (ko.unwrap(value)) {
                jq.stop(true, true).animate({ top: 0 }, 500);
            } else {
                jq.stop(true, true).animate({ top: -80 }, 500);
            }
        }
    };

    ko.bindingHandlers.percentageWidth = {
        update: function (element, valueAccessor) {
            var value = valueAccessor(),
                jq = $(element);
            if (ko.unwrap(value)) {
                if (value > 90) {
                    jq.css({ 'width': value + '%', 'backgroundColor': 'red' });
                } else {
                    jq.css({ 'width': value + '%' });
                }
            } else {
                jq.css({ 'width': '0%' });
            }
        }
    };

    ko.bindingHandlers.enterkey = {
        init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
            var allBindings = allBindingsAccessor();
            $(element).keydown(function (event) {
                var keyCode = (event.which ? event.which : event.keyCode);
                if (keyCode === 13) {
                    allBindings.enterkey.call(viewModel);
                    return false;
                }
                return true;
            });
        }
    };

}(ko);