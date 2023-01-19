(function () {
    app.modals.CreateOrUpdateModal = function () {
        var processService = abp.services.app.process,
                _modalManager,
                $processForm = null;
        this.init = function (modalManager) {
            _modalManager = modalManager;
            $processForm = _modalManager.getModal().find("form[name=ProcessForm]");
        }

        this.save = function () {
            if (!$processForm.valid()) {
                return;
            }

            var process = $processForm.serializeFormToObject();

            _modalManager.setBusy(true);
            processService.createOrUpdateProcess(process)
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