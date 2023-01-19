//# sourceURL=dynamicLoadOrUnloadModalScript.js
(function () {
    app.modals.LoadOrUnloadModal = function () {
        var _manager,
            _args,
            _form = null,
            $OperationType = $("#operationType"),
            cutterAppService = abp.services.app.cutter;

        this.init = function (manager, args) {
            _manager = manager;
            _args = args;

            _form = _manager.getModal().find("form[name='loadOrUnLoadCutterModalForm']");
            var operTypeVlaue = $OperationType.val();
            if (operTypeVlaue === "0") {
                $(".cutter-loading").css("display", "none");
            }

        }

        this.shown = function () {
            $(".drop-downlist")
                .select2({
                    minimumResultsForSearch: -1
                });

            $OperationType.select2({ minimumResultsForSearch: -1 })
                .on("change",
                    function () {
                        if ($OperationType.select2('data')[0].id === "0") {
                            $(".cutter-loading").css("display", "none");
                        } else {
                            $(".cutter-loading").css("display", "block");
                            $(".drop-downlist")
                                .select2({
                                    minimumResultsForSearch: -1
                                });
                        }
                    });
        }

        this.save = function () {
            _form.validate({
                rules: {
                    CutterTVlaue: { required: true, digits: true, min: 1 }
                },
                messages: {
                    CutterTVlaue: { required: app.localize("EnterToolPosition"), digits: app.localize("EnterInteger"), min: app.localize("EnterAPositiveIntegerGreaterThanOne") }
                }
            });

            if (!_form.valid()) {
                return false;
            }
            var entity = _form.serializeFormToObject();
            _manager.setBusy(true);
            cutterAppService.cutterLoadOrUnLoad(entity)
                .done(function () {
                    abp.notify.info(app.localize("SavedSuccessfully"));
                    _manager.close();
                    _manager.setResult();
                })
                .always(function () {
                    _manager.setBusy(false);
                });
        }
    }
})();
