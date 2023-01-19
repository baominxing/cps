//# sourceURL=dynamic_EditModal.js
(function () {
    app.modals.GatherParamsSettingEditModal = function () {
        var _basicDataService = abp.services.app.basicData;
        var _modalManager,_args;
        var _$gatherParamsForm = null;

        this.init = function (modalManager,args) {

            _modalManager = modalManager;
            _args = args;

            _$gatherParamsForm = _modalManager.getModal().find("form[name=GatherParamForm]");
            _$gatherParamsForm.find(".colorpicker-element").colorpicker();

            _$gatherParamsForm.validate();

            checkIfShowStyleIsGauge();
            //只有数据类型为 number时才能选 “线性趋势”
            if ($.trim(_args.dataType.toLowerCase()) !== "number") {
                $("#DisplayStyle > option[value=0]").remove();  //value=0  线性趋势
                $("#DisplayStyle > option[value=3]").remove();  //value=3  仪表盘
            }

            var typeValue = document.getElementById("displayStyle").getAttribute("typevalue");
            document.getElementById('displayStyle').value = typeValue;

        };

        this.save = function () {
            if ($("#DisplayStyle").val() === "3") {
                var min = $("#min").val();
                var max = $("#max").val();

                if (!checkGaugeInput(min, max)) {
                    return;
                }
            }


            if (!_$gatherParamsForm.valid()) {
                return;
            }
            var group = _$gatherParamsForm.serializeFormToObject();

            _modalManager.setBusy(true);
            _basicDataService.updateGatherParams(group)
                .done(function (result) {
                    abp.notify.info(app.localize("SavedSuccessfully"));
                    _modalManager.setResult(result);
                    _modalManager.close();
                }).always(function () {
                    _modalManager.setBusy(false);
                });
        };

        $(document).on('change', '#DisplayStyle', function () {
            if ($(this).val() === "3") {
                $(".min").removeAttr("hidden");
                $(".max").removeAttr("hidden");
            }
            else {
                $(".min").attr("hidden", "hidden");
                $(".max").attr("hidden", "hidden");
                $("#min").val(0);
                $("#max").val(0);
            }
        });


        function checkGaugeInput(min, max) {
            if (min >= max) {
                abp.message.error(app.localize("MaxCannotSmallerThanMin"));
                return false;
            }
            return true;
        }

        function checkIfShowStyleIsGauge() {
            if ($("#DisplayStyle").val() === "3") {
                $(".min").removeAttr("hidden");
                $(".max").removeAttr("hidden");
            }
        }
    };
})()