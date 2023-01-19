//# sourceURL=dynamic_CreateOrUpdateModal.js
(function () {
    app.modals.CreateOrUpdateModal = function () {
        var cutterAppService = abp.services.app.cutter,
            _modalManager,
            _$createOrUpdateForm = null,
            $code = $(".code");

        this.init = function (modalManager) {
            _modalManager = modalManager;

            //加载下拉框
            var inParameters = [];
            var isFirst = true;
            //剔除已再再数据库中的
            cutterAppService.getCutterParameterList().done(function(result) {
                for (var i = 0; i < result.length; i++) {
                    inParameters.push(result[i].code);
                }

                for (var j = 1; j <= 10; j++) {
                    var code = "Parameter" + j;
                    if (jQuery.inArray(code, inParameters) === -1 || $("#SelectedCode").val() === code) {
                        var option;
                        if (isFirst && $("#SelectedCode").val() !== code) {
                            option = "<option value =" + code + " selected>" + code + "</option>";
                            isFirst = false;
                        } else if ($("#SelectedCode").val() === code) {
                            option = "<option value =" + code + " selected>" + code + "</option>";
                            isFirst = false;
                        } else {
                            option = "<option value =" + code + ">" + code + "</option>";
                        }
                        $code.append(option);
                    }
                }
            });

            _$createOrUpdateForm = _modalManager.getModal().find('form[name=createOrUpdateForm]');
        }

        this.shown = function () {
            
            $(".code").select2({
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
            if (!_$createOrUpdateForm.valid()) {
                return;
            }

            var parameters = {
                Id: $("#Id").val(),
                code: $(".code").val(),
                name: $(".name").val(),
                creationTime: $("#CreationTime").val(),
                creatorUserId: $("#CreatorUserId").val()
            }

            _modalManager.setBusy(true);
            cutterAppService.createOrUpdateCutterParameter(parameters)
                .done(function (result) {
                    abp.notify.info(app.localize("SavedSuccessfully"));
                    _modalManager.setResult(result);
                    abp.event.trigger("app.CreateOrUpdateModalSaved");
                    _modalManager.close();
                }).always(function () {
                    _modalManager.setBusy(false);
                });
        }
    }
})(jQuery);