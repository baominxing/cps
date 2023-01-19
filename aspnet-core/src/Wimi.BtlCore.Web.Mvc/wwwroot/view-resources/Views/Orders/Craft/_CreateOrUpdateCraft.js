//# sourceURL=dynamic_CreateOrUpdateCraft.js
(function () {
    app.modals.CreateOrUpdateCraft = function () {
        var _modalManager;
        var craftAppService = abp.services.app.craft;
        var $createOrUpdateForm = null;
        this.init = function (modalManager) {
            _modalManager = modalManager;
            $createOrUpdateForm = _modalManager.getModal().find("form[name=createOrUpdateForm]");
            if ($("#Id").val() !== "0") {
                $(".code").attr("readonly", "readonly");
            }
        };

        this.save = function () {
            if (!$createOrUpdateForm.valid()) {
                return;
            }

            var parameters = {
                id: $("#Id").val(),
                code: $(".code").val(),
                name: $(".name").val()
            }



            if (parameters.id === "0") {
                _modalManager.setBusy(true);
                craftAppService.createCraft(parameters).done(function (result) {
                    abp.notify.info(app.localize("SavedSuccessfully"));
                    _modalManager.setResult(result);
                    _modalManager.close();
                }).always(function () {
                    _modalManager.setBusy(false);
                });
            } else {
                _modalManager.setBusy(true);
                craftAppService.updateCraft(parameters).done(function (result) {
                    abp.notify.info(app.localize("SavedSuccessfully"));
                    _modalManager.setResult(result);
                    _modalManager.close();
                }).always(function () {
                    _modalManager.setBusy(false);
                });
            }
        };
    };
})();