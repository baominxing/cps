(function () {
    app.modals.CreateOrUpdateModal = function () {
        var basicDataService = abp.services.app.basicData,
            _modalManager,
            _$machineTypeForm = null;

        this.init = function (modalManager) {
            _modalManager = modalManager;
            _$machineTypeForm = _modalManager.getModal().find('form[name=MachineTypeForm]');
            //_$machineTypeForm.find('.colorpicker-element').colorpicker();
        }

        this.save = function () {
            if (!_$machineTypeForm.valid()) {
                return;
            }
            var machineType = _$machineTypeForm.serializeFormToObject();

            _modalManager.setBusy(true);
            basicDataService.addOrUpdateMachineType(machineType)
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