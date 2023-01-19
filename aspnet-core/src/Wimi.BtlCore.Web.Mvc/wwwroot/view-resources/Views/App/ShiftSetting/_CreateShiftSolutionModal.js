(function () {
    app.modals.CreateShiftSolutionModal = function () {

        var _modalManager;
        var _shiftAppService = abp.services.app.shift;
        var _$form = null;

        this.init = function (modalManager) {
            _modalManager = modalManager;

            _$form = _modalManager.getModal().find("form[name=ShiftSolutionForm]");
        };

        this.save = function () {
            if (!_$form.valid()) {
                return;
            }

            var shiftSolution = _$form.serializeFormToObject();

            _modalManager.setBusy(true);

            if ($("#State").val() === "Create") {
                _shiftAppService.createShiftSolution(
                    shiftSolution
                    ).done(function (result) {
                        abp.notify.info(app.localize("SavedSuccessfully"));
                        _modalManager.setResult(result);
                        _modalManager.close();
                    }).always(function () {
                        _modalManager.setBusy(false);
                    });
            }
            else {
                _shiftAppService.editShiftSolution(
                    shiftSolution
                    ).done(function (result) {
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