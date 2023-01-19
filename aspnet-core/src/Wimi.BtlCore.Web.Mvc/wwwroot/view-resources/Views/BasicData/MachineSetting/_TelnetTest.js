//# sourceURL=dynamic_TelnetTest.js
(function () {
    app.modals.TelnetTestModal = function () {
        var basicDataService = abp.services.app.basicData,
            _modalManager,
            _$telnetTestForm = null,
            _args;

        this.init = function (modalManager, args) {
            _modalManager = modalManager;
            _args = args;
            _$telnetTestForm = _modalManager.getModal().find('form[name=TelnetTestForm]');

            var msg = _args.machineName + "-" + app.localize("TelnetTest");
            _modalManager.getModal().find(".modal-title span").text(msg);

            if (_args.ipAddress && _args.ipAddress.length > 0) {
                $("#IpAddress1").val(_args.ipAddress);
                $("#IpAddress1").attr('readonly', 'readonly');
            }

            if (_args.tcpPort && _args.tcpPort.toString().length > 0) {
                $("#TcpPort1").val(_args.tcpPort);
                $("#TcpPort1").attr('readonly', 'readonly');
            }

        };

        this.save = function () {
            if (!_$telnetTestForm.valid()) {
                return;
            }
            var param = _$telnetTestForm.serializeFormToObject();
            param.machineId = _args.machineId;
            _modalManager.setBusy(true);
            basicDataService.telnetTestForMachine(param)
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