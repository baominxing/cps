(function () {
    app.modals.WorkOrderReportModal = function () {
        var _modalManager,
            _args,
            $form,
            service = abp.services.app.loginReport;

        this.init = function (modalManager, args) {
            _modalManager = modalManager;
            _args = args;
            $form = _modalManager.getModal().find('form[name=ReportForm]');
            abp.ui.setBusy();

            //得到设备报功的数据 
            service.getOutputQuantityForOrderReport({ id: _args.workOrderId })
                .done(function (response) {
                    $form.find('input[name="defectiveCount"]').val(response.defectiveCount);
                    $form.find('input[name="qualifiedCount"]').val(response.qualifiedCount);
                }).always(function () {
                    abp.ui.clearBusy();
                });
        }

        this.save = function () {
            //验证form输入值
            if (!$form.valid()) {
                return false;
            }
            var param = $form.serializeFormToObject();
            param.id = _args.workOrderId;
            _modalManager.setBusy(true);
            service.workOrderReport(param).done(function (response) {
                abp.notify.info(app.localize("SavedSuccessfully"));
                _modalManager.close();
                _modalManager.setResult(response);
            }).always(function () {
                _modalManager.setBusy(false);
            });
        }
    }
})();
