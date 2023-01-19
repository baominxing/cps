(function () {
    app.modals.CreateDeviceGroupModal = function () {

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
            _deviceGroupService.createDeviceGroup(
                deviceGroup
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