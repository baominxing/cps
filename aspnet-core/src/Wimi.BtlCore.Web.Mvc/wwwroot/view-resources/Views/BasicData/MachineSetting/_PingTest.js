//# sourceURL=dynamic_PingTest.js
(function () {
    app.modals.PingTestModal = function () {
        var basicDataService = abp.services.app.basicData,
            _modalManager,
            _$machineTypeForm = null,
            _args;

        this.init = function (modalManager, args) {
            _modalManager = modalManager;
            _args = args;
            _$machineTypeForm = _modalManager.getModal().find('form[name=MachineTypeForm]');

            var msg = _args.machineName + "-" + app.localize("PingTest");
            _modalManager.getModal().find(".modal-title span").text(msg);

            if (_args.ipAddress && _args.ipAddress.length > 0) {
                $("#IpAddress1").val(_args.ipAddress);
                $("#IpAddress1").attr('readonly', 'readonly');
            }
        };

        this.save = function () {
            if (!_$machineTypeForm.valid()) {
                return;
            }
            var param = _$machineTypeForm.serializeFormToObject();
            param.machineId = _args.machineId;

            _modalManager.setBusy(true);
            basicDataService.pingTestForMachine(param)
                .done(function () {
                    abp.notify.success(app.localize("TestSuccessfully"));
                    _modalManager.setResult();
                    _modalManager.close();
                }).always(function () {
                    _modalManager.setBusy(false);
                });
        };
    };
})(jQuery);