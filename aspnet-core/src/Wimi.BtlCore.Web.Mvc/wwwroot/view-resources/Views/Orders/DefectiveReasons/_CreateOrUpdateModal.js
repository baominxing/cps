(function () {
    app.modals.CreateOrUpdateModal = function () {
        var defectiveReasons = abp.services.app.defectiveReasons,
            _modalManager,
            $defectiveReasonForm = null;

        this.init = function (modalManager) {
            _modalManager = modalManager;
            $defectiveReasonForm = _modalManager.getModal().find("form[name=DefectiveReasonForm]");
        }

        this.save = function () {
            if (!$defectiveReasonForm.valid()) {
                return;
            }

            var defectiveReason = $defectiveReasonForm.serializeFormToObject();

            _modalManager.setBusy(true);
            defectiveReasons.createOrUpdateDefectiveReason(defectiveReason)
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