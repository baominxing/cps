var abp = abp || {};
(function ($) {
    if (!sweetAlert || !$) {
        return;
    }

    /* DEFAULTS *************************************************/

    abp.libs = abp.libs || {};
    abp.libs.sweetAlert = {
        config: {
            'default': {

            },
            info: {
                type: 'info'
            },
            success: {
                type: 'success'
            },
            warn: {
                type: 'warning'
            },
            error: {
                type: 'error'
            },
            confirm: {
                type: 'warning',
                title: 'Are you sure?',
                showCancelButton: true,
                cancelButtonText: 'Cancel',
                confirmButtonColor: "#DD6B55",
                confirmButtonText: 'Yes'
            }
        }
    };

    /* MESSAGE **************************************************/

    var showMessage = function (type, message, title, callback, options) {
        options = options || {};
        var messageContent = {};
        if (title) {
            messageContent.title = title;
        } else {
            messageContent.title = message;
            message = undefined;
        }

        if (options.isHtml) {
            var el = document.createElement('div');
            //https://github.com/t4t5/sweetalert/issues/842
            el.style = 'position: relative;';
            el.innerHTML = message;

            messageContent.content = el;
        } else {
            messageContent.text = message;
        }

        var opts = $.extend(
            {},
            abp.libs.sweetAlert.config['default'],
            abp.libs.sweetAlert.config[type],
            messageContent,
            options
        );

        return $.Deferred(function ($dfd) {
            sweetAlert(opts, function (isConfirmed) {
                callback && setTimeout(function () { callback(isConfirmed); }, 300);
                $dfd.resolve(isConfirmed);
            });
        });
    };

    abp.message.info = function (message, title, options) {
        return showMessage('info', message, title, null, options);
    };

    abp.message.success = function (message, title, options) {
        return showMessage('success', message, title, null, options);
    };

    abp.message.warn = function (message, title, options) {
        return showMessage('warn', message, title, null, options);
    };

    abp.message.error = function (message, title, options) {
        return showMessage('error', message, title, null, options);
    };

    //abp.message.confirm = function (message, title, callback, options) {
    //    return showMessage('confirm', message, title, callback, options);
    //};

    abp.message.confirm = function (message, titleOrCallback, callback) {
        var userOpts = {
            text: message
        };

        if ($.isFunction(titleOrCallback)) {
            callback = titleOrCallback;
        } else if (titleOrCallback) {
            userOpts.title = titleOrCallback;
        };

        var opts = $.extend(
            {},
            abp.libs.sweetAlert.config.default,
            abp.libs.sweetAlert.config.confirm,
            userOpts
        );

        return $.Deferred(function ($dfd) {
            sweetAlert(opts, function (isConfirmed) {
                callback && setTimeout(function () { callback(isConfirmed); }, 300);
                $dfd.resolve(isConfirmed);
            });
        });
    };

    abp.event.on('abp.dynamicScriptsInitialized', function () {
        abp.libs.sweetAlert.config.confirm.title = abp.localization.abpWeb('AreYouSure');
        abp.libs.sweetAlert.config.confirm.buttons = [abp.localization.abpWeb('Cancel'), abp.localization.abpWeb('Yes')];
    });

})(jQuery);