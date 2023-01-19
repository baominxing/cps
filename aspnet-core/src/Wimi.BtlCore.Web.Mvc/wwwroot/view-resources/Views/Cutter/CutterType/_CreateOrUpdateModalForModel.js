//# sourceURL=dynamic_CreateOrUpdateModalForModel.js
(function () {
    app.modals.CreateOrUpdateModalForModel = function () {

        var _modalManager;
        var cutterAppService = abp.services.app.cutter;
        var _$createOrUpdateForm = null;
        var dynamicParameters = [];
        this.init = function (modalManager) {
            _modalManager = modalManager;

            $.validator.addMethod('minGT', function (value, el, param) {
                return value > param;
            }, app.localize("EnterValidNumberGreaterThanZero"));

            //填充刀具编号前缀
            fillCutterNoPrefix();
            //构造参数UI
            generateDynamicParameterUI();

            _$createOrUpdateForm = _modalManager.getModal().find("form[name=createOrUpdateForm]");
        };

        this.shown = function () {

            $("#CountingMethod").select2({
                multiple: false,
                minimumResultsForSearch: -1,
                language: {
                    noResults: function () {
                        return app.localize("NoMatchingData");
                    }
                }
            }).val($("#CountingMethodValue").val()).trigger("change");
        }

        this.save = function () {
            if (!_$createOrUpdateForm.valid()) {
                return;
            }

            //if ($(".originalLife").val() <= 0 || $(".warningLife").val() <= 0) {
            //    abp.message.error(app.localize("InitialOrEarlyWarningLifeTip"));
            //    return;
            //}

            var parameters = {
                id: $("#Id").val(),
                name: $(".name").val(),
                cutterTypeId: $("#CutterTypeId").val(),
                cutterNoPrefix: $(".cutterNoPrefix").val(),
                countingMethod: $(".countingMethod").val(),
                originalLife: $(".originalLife").val(),
                warningLife: $(".warningLife").val(),
                creationTime: $("#CreationTime").val(),
                creatorUserId: $("#CreatorUserId").val()
            }

            //加载动态参数值
            fillDynamicParameter(parameters);

            _modalManager.setBusy(true);
            cutterAppService.createOrUpdateCutterModel(
                parameters
            ).done(function (result) {
                abp.notify.info(app.localize("SavedSuccessfully"));
                _modalManager.setResult(result);
                _modalManager.close();
            }).always(function () {
                _modalManager.setBusy(false);
            });
        };

        function generateDynamicParameterUI() {
            cutterAppService.getCutterParameterList().done(function (result) {
                if (result.length > 0) {

                    $.each(result, function () {
                        var value = $("#" + this.code + "").val();
                        var div =
                            '<div class="form-group form-md-line-input form-md-floating-label col-xs-12 col-sm-6">' +
                                '<span class="text-red pull-left">※</span>' +
                                '<label>' + this.name + '</label>' +
                                '<input class="form-control ' + this.code + '" type="text" name="' + this.code + '" required number="true" minGT="0" maxlength="@CoreConsts.MaxLength" value="' + value + '">' +
                                '</div>';
                        _$createOrUpdateForm.find(".row").append(div);
                        dynamicParameters.push(this.code);
                    });
                }
            });
        }

        function fillCutterNoPrefix() {
            if ($("#IsCutterNoPrefixCanEdit").val() === "false") {
                $(".cutterNoPrefix").attr("disabled", "disabled");
            }
        }

        function fillDynamicParameter(parameters) {
            for (var i = 0; i < dynamicParameters.length; i++) {
                var name = dynamicParameters[i];
                var value = $("." + name + "").val();
                parameters[name] = value;
            }
        }
    };
})();