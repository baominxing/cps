(function () {
    app.modals.MachineReportModal = function () {
        var _modalManager,
            _args,
            $form,
            templateRender = Handlebars.compile($("#machineReporttemplate").html()),
            service = abp.services.app.loginReport;

        this.init = function (modalManager, args) {
            _modalManager = modalManager;
            _args = args;
            $form = _modalManager.getModal().find("form[name='MachineReportForm']");

            //请求不良原因及数据
            service.getDefectiveReasonsForMachineReport({ id: _args.workOrderTaskId }).done(function(response) {
                $form.append(templateRender(response));
            }).fail(function () {
                abp.message.error(app.localize("FailureToObtainDefectiveProducts")+"！");
                return false;
            });

            //监控不良原因输入值
            $form.on("change",
                'input.defective-Reasons',
                function () {
                    var reasons = $("input.defective-Reasons");
                    var totalCount = _.reduce(reasons, function (result, input) {
                        var defectiveCount = 0;
                        if (!_.isNaN($(input).val() * 1)) {
                            defectiveCount = $(input).val() * 1;
                        }
                        return result + defectiveCount;
                    }, 0);
                    $form.find("input[name='defectiveCount']").val(totalCount);
                });
        }

        this.save = function () {
            if (!$form.valid()) {
                return false;
            }

            _modalManager.setBusy(true);
            var reasonArray = [];
            var param = $form.serializeFormToObject();
            _.each($("input.defective-Reasons"),
                function (item) {
                    var defectiveCount = 0;
                    if (!_.isNaN($(item).val() * 1)) {
                        defectiveCount = $(item).val() * 1;
                    }
                    reasonArray.push({ defectiveReasonsId: $(item).data().defectivereasonsid, count: defectiveCount});
                });
            param.reasonsDictionary = reasonArray;
            service.machineReport(param).done(function (response) {
                abp.notify.info(app.localize("SavedSuccessfully"));
                _modalManager.close();
                _modalManager.setResult(response);
            }).always(function () {
                _modalManager.setBusy(false);
            });
        }

    }
})();