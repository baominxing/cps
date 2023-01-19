//# sourceURL=dynamic_CreateOrUpdateModal.js
(function () {
    app.modals.CreateOrUpdateModal = function () {
        var service = abp.services.app.reasonFeedbackCalendar,
            _modalManager,
            $stateCoe = $("#StateCode"),
            $createOrUpdateForm = null;

        this.init = function (modalManager) {
            _modalManager = modalManager;
            $createOrUpdateForm = _modalManager.getModal().find("form[name=CreateOrUpdateForm]");

            // 初始化下拉框
            service.listFeedbackState().done(function(response) {
                var stateCode = [];
                _.each(response, function (item) {
                    stateCode.push({ id: item.value, text: item.name });
                });

                $stateCoe.select2({
                    data: stateCode,
                    multiple: false,
                    placeholder: app.localize("PleaseSelect"),
                    minimumResultsForSearch: -1,
                    language: {
                        noResults: function () {
                            return app.localize("PleaseMaintainTheFeedbackState");
                        }
                    }
                });
                var selectedValue = $("#stateSelected").val();
                $stateCoe.select2().attr("disabled", selectedValue.length !== 0 ? true : false).val(selectedValue).trigger("change");
            });

            $("#cronButton").click(function () {
               layer.open({
                    type: 2,
                    title: ['选择执行计划', 'font-size:18px;'],
                    area: ['800px', '780px'],
                    fixed: false,
                    resize: false,
                    content: 'easyCron/index.html',
                    btn: ['确定'],
                    yes: function (index, layero) {
                        // 获取core 数据的值
                        var body = layer.getChildFrame('body', index);
                        var result = $(body).find("#result").get(0).value;
                        $("#cron").val(result);

                        layer.close(index);
                    }
                });
            });
        };

        this.save = function () {
            if (!$createOrUpdateForm.valid()) {
                return;
            }

            var parameters = $createOrUpdateForm.serializeFormToObject();

            if (parameters.Id === "0") {
                _modalManager.setBusy(true);
                service.create(parameters)
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
                service.update(parameters)
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