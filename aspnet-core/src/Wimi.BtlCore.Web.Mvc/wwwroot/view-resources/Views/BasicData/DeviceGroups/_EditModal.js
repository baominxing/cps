(function () {
    app.modals.EditDeviceGroupsModal = function () {

        var _modalManager;
        var _deviceGroupService = abp.services.app.deviceGroup;
        var _$form = null;

        this.init = function (modalManager) {
            _modalManager = modalManager;

            _$form = _modalManager.getModal().find("form[name=DeviceGroupForm]");
        };

        this.save = function () {
            if (!_$form.valid()) {
                return;
            }

            var deviceGroup = _$form.serializeFormToObject();

            _modalManager.setBusy(true);
            _deviceGroupService.updateDeviceGroup(
                deviceGroup
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