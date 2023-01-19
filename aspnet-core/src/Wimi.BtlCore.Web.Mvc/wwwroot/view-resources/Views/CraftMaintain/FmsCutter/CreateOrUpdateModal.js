//# sourceURL=dynamic_CreateOrUpdateModal.js
(function () {
    app.modals.CreateOrUpdateModal = function () {
        var service = abp.services.app.fmsCutter,
            _modalManager,
            $createOrUpdateForm = null;

        this.init = function (modalManager) {
            _modalManager = modalManager;
            $createOrUpdateForm = _modalManager.getModal().find("form[name=CreateOrUpdateForm]");

            _.each($('.extend-field-create'),
                function(item, index) {
                    var customfield = $(item).data('customfield');
                    var $edit = $('#extendField_' + customfield);

                    var inputFinder = $(item).find('.type-input');
                    var selectFinder = $(item).find('.type-select');

                    _.each(inputFinder,
                        function (key) {
                            if (key.type !== "text") {
                                var value = $(key).val();
                                var hiddenValue = $edit.val();

                                if (value === hiddenValue || _.indexOf(hiddenValue, value) !==-1) {
                                    $(key)[0].checked = true;
                                } else {
                                    $(key)[0].checked = false;
                                }
                            } else {
                                $(key).val($edit.val());
                            }
                        });

                    if (selectFinder.length > 0) {
                        var selectKey = selectFinder.get(0);
                        $(selectKey).val($edit.val());
                    }
                });
        };

        this.shown = function () {
            $("#DisplayType").select2({
                multiple: false,
                minimumResultsForSearch: -1,
                language: {
                    noResults: function () {
                        return app.localize("NoMatchingData");
                    }
                }
            });

        }

        this.save = function () {
            if (!$createOrUpdateForm.valid()) {
                return;
            }

            var parameters = $createOrUpdateForm.serializeFormToObject();
            if (parseInt(parameters.OriginalLife) <= parseInt(parameters.WarningLife)) {
                abp.notify.warn("初始寿命需要大于预警值");
                return;
            }
            console.log(parameters)

            // 添加自定义扩展字段
            var fields = [];
            _.each($('.extend-field-create'),
                function(item, index) {
                    var customfield = $(item).data('customfield');

                    //分不同的情况获得值
                    var inputFinder = $(item).find('.type-input');
                    var selectFinder = $(item).find('.type-select');

                    if (inputFinder.length > 0) {
                        var name = "";
                        _.each(inputFinder,
                            function (key) {
                             
                                if (key.type !== "text") {
                                    if ($(key)[0].checked) {
                                        name += "," + $(key).val();
                                    }
                                } else {
                                    name = $(key).val();
                                }
                            });

                        fields.push({ name: name, value: customfield });
                    }

                    if (selectFinder.length > 0) {
                        var selectKey = selectFinder.get(0);
                        fields.push({ name: $(selectKey).val(), value: customfield });
                    }
                });

            parameters.ExtendFields = fields;

            if (parameters.Id === "0") {
                _modalManager.setBusy(true);
                service.createFmsCutter(parameters)
                    .done(function (result) {
                        abp.notify.info(app.localize("SavedSuccessfully"));
                        _modalManager.setResult(result);
                        abp.event.trigger("app.CreateOrUpdateModalSaved");
                        _modalManager.close();
                    }).always(function () {
                        _modalManager.setBusy(false);
                    });
            } else {
                _modalManager.setBusy(true);
                service.updateFmsCutter(parameters)
                    .done(function (result) {
                        abp.notify.info(app.localize("SavedSuccessfully"));
                        _modalManager.setResult(result);
                        abp.event.trigger("app.CreateOrUpdateModalSaved");
                        _modalManager.close();
                    }).always(function () {
                        _modalManager.setBusy(false);
                    });
            }
        };
    };
})(jQuery);