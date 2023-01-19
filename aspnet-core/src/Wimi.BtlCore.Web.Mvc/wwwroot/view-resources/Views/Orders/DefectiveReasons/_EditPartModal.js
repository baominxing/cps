(function () {
    app.modals.EditDefectivePartModal = function () {

        var _modalManager;
        var _defectiveReasonsService = abp.services.app.defectiveReasons;
        var _$form = null;

        this.init = function (modalManager) {
            _modalManager = modalManager;
            _$form = _modalManager.getModal().find("form[name=DefectivePartForm]");
        };

        this.save = function () {
            if (!_$form.valid()) {
                return;
            }

            var defectivePart = _$form.serializeFormToObject();

            _modalManager.setBusy(true);
            _defectiveReasonsService.updateDefectivePart(
                defectivePart
            ).done(function (result) {
                abp.notify.info(app.localize("SavedSuccessfully"));
                _modalManager.close();
                _modalManager.setResult(result);
            }).always(function () {
                _modalManager.setBusy(false);
            });
        };
    };
})();