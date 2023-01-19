(function () {
    app.modals.CreateDefectivePartModal = function () {

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
            _defectiveReasonsService.createDefectivePart(
                defectivePart
            ).done(function (result) {
                abp.notify.info(app.localize("SavedSuccessfully"));
                _modalManager.setResult(result);
                _modalManager.close();
            }).always(function () {
                _modalManager.setBusy(false);
            });
        };
    };
})();