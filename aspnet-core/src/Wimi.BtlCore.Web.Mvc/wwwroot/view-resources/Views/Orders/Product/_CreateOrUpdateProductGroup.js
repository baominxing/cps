//# sourceURL=dynamic_CreateOrUpdateProductGroup.js
(function () {
    app.modals.CreateOrUpdateProductGroup = function () {

        var _modalManager;
        var productAppService = abp.services.app.product;
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
                name: $(".name").val(),
                memo: $(".memo").val()
            }



            if (parameters.id === "0") {
                _modalManager.setBusy(true);
                productAppService.createProductGroup(parameters).done(function (result) {
                    abp.notify.info(app.localize("SavedSuccessfully"));
                    _modalManager.setResult(result);
                    _modalManager.close();
                }).always(function () {
                    _modalManager.setBusy(false);
                });
            } else {
                _modalManager.setBusy(true);
                productAppService.updateProductGroup(parameters).done(function (result) {
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