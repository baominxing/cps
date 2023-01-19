//# sourceURL=dynamicModalManagerScript.js

//tonny: add sourceURL for debug use, please refer: http://stackoverflow.com/questions/1705952/is-possible-to-debug-dynamic-loading-javascript-by-some-debugger-like-webkit-fi
//about vm, please refer http://stackoverflow.com/questions/20388563/chrome-console-vm

var app = app || {};
(function ($) {

    var _loadedScripts = [];

    app.modals = app.modals || {};

    app.ModalManager = (function () {

        var _normalizeOptions = function (options) {
            if (!options.modalId) {
                options.modalId = "Modal_" + (Math.floor((Math.random() * 1000000))) + new Date().getTime();
            }
        };

        function _removeContainer(modalId) {
            var _containerId = modalId + "Container";
            var _containerSelector = "#" + _containerId;

            var $container = $(_containerSelector);
            if ($container.length) {
                $container.remove();
            }
        };

        function _createContainer(modalId, modalSize) {
            _removeContainer(modalId);

            var _containerId = modalId + "Container";
            return $('<div id="' + _containerId + '"></div>')
                .append(
                    '<div id="' + modalId + '" class="modal fade" tabindex="-1" role="modal" aria-hidden="true">' +
                    '  <div class="modal-dialog ' + modalSize + '">' +
                    '    <div class="modal-content"></div>' +
                    "  </div>" +
                    "</div>"
                ).appendTo("body");
        }

        return function (options) {

            _normalizeOptions(options);

            var _options = options;

            if (options.modalSize && options.modalSize !== "modal-lg" && options.modalSize !== "modal-sm") {
                options.modalSize = "";
            }

            var _$modal = null;
            var _modalId = options.modalId;
            var _modalSelector = "#" + _modalId;
            var _modalObject = null;

            var _publicApi = null;
            var _args = null;
            var _getResultCallback = null;

            var _onCloseCallbacks = [];

            function _saveModal() {
                if (_modalObject && _modalObject.save) {
                    _modalObject.save();
                }
            }

            function _initAndShowModal() {
                _$modal = $(_modalSelector);

                _$modal.modal({
                    backdrop: "static"
                });

                _$modal.on("hidden.bs.modal", function () {
                    $("div.layui-layer-tips").remove();

                    _removeContainer(_modalId);              
                    for (var i = 0; i < _onCloseCallbacks.length; i++) {
                        _onCloseCallbacks[i]();
                    }
                });

                var modalClass = app.modals[options.modalClass];
                if (modalClass) {
                    _modalObject = new modalClass();
                    if (_modalObject.init) {
                        _modalObject.init(_publicApi, _args);
                        //Wimi.BtlCore.addRequiredTag(_$modal);
                        wimi.btl.addRequiredTag(_$modal);
                    }
                }

                _$modal.on("shown.bs.modal", function () {
                    if (_modalObject != null && _modalObject.shown) {
                        _modalObject.shown();
                    }
                    var $el = _$modal.find("input:not([type=hidden]):first");
                    if ($el.data('type') !== "calendar") {
                        $el.focus();
                    }

                    
                });

                _$modal.find(".save-button").click(function () {
                    _saveModal();
                });

                //添加验证配置，错误高亮显示
                _$modal.find('form').validate($.WIMI.options.validate);

                _$modal.find(".modal-body").keydown(function (e) {
                   
                    if (e.which*1 === 13) {
                        e.preventDefault();
                        _saveModal();
                    }
                });
                _$modal.find('.modal-header').css("cursor", "move");
                _$modal.modal("show");
            };

            var _open = function (args, getResultCallback) {

                _args = args || {};
                _getResultCallback = getResultCallback;

                _createContainer(_modalId, options.modalSize)
                    .find(".modal-content")
                    .load(options.viewUrl, _args, function (response, status, xhr) {
                        if (status === "error") {
                            abp.message.warn(abp.localization.abpWeb("InternalServerError"));
                            return;
                        };

                        if (options.scriptUrl && _.indexOf(_loadedScripts, options.scriptUrl) < 0) {
                            $.getScript(options.scriptUrl)
                                .done(function (script, textStatus) {
                                    _loadedScripts.push(options.scriptUrl);
                                    _initAndShowModal();
                                })
                                .fail(function (jqxhr, settings, exception) {
                                    abp.message.warn(abp.localization.abpWeb("InternalServerError"));
                                });
                        } else {
                            _initAndShowModal();
                        }
                    });

                $('.modal-content').draggable({
                    handle: ".modal-header",
                    stop: function (event, ui)
                    {
                        $(this).css("height", "");
                    }
                });
            };

            var _close = function () {
                if (!_$modal) {
                    return;
                }
                _$modal.modal("hide");
            };

            var _onClose = function (onCloseCallback) {
                _onCloseCallbacks.push(onCloseCallback);
            };

            function _setBusy(isBusy) {
                if (!_$modal) {
                    return;
                }

                _$modal.find(".modal-footer button").buttonBusy(isBusy);
            }

            _publicApi = {
                open: _open,

                reopen: function () {
                    _open(_args);
                },

                close: _close,

                getModalId: function () {
                    return _modalId;
                },

                getModal: function () {
                    return _$modal;
                },

                getArgs: function () {
                    return _args;
                },

                getOptions: function () {
                    return _options;
                },

                setBusy: _setBusy,

                setResult: function () {
                    _getResultCallback && _getResultCallback.apply(_publicApi, arguments);
                },

                onClose: _onClose
            };
            return _publicApi;

        };
    })();

})(jQuery);