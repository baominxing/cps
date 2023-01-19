//# sourceURL=dynamic_CreateOrUpdateProduct.js
(function () {
    app.modals.CreateOrUpdateProduct = function () {

        var _modalManager;
        var productAppService = abp.services.app.product;
        var $createOrUpdateForm = null;
        this.init = function (modalManager) {
            _modalManager = modalManager;
            $createOrUpdateForm = _modalManager.getModal().find("form[name=createOrUpdateForm]");

            if ($("#Id").val() !== "0") {
                $(".ishalffinished").val($("#IsHalfFinished").val());
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
                spec: $(".spec").val(),
                DrawingNumber: $(".drawingNumber").val(),
                PartType: $(".partType").val(),
                desc: $(".desc").val(),
                ishalffinished: $(".ishalffinished").val(),
                memo: $(".memo").val(),
                productGroupId: $("#ProductGroupId").val()
            }

            if (parameters.id === "0") {
                _modalManager.setBusy(true);
                productAppService.createProduct(parameters).done(function (result) {
                    abp.notify.info(app.localize("SavedSuccessfully"));
                    _modalManager.setResult(result);
                    _modalManager.close();
                }).always(function () {
                    _modalManager.setBusy(false);
                });
            } else {
                _modalManager.setBusy(true);
                productAppService.updateProduct(parameters).done(function (result) {
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