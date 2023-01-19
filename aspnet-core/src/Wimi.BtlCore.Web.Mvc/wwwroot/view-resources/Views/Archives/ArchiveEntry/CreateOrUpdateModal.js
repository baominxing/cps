﻿//# sourceURL=dynamic_CreateOrUpdateModal.js
(function () {
    app.modals.CreateOrUpdateModal = function () {
        var service = abp.services.app.archiveEntry,
            _modalManager,
            $createOrUpdateForm = null;

        this.init = function (modalManager) {
            _modalManager = modalManager;
            $createOrUpdateForm = _modalManager.getModal().find("form[name=CreateOrUpdateForm]");
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