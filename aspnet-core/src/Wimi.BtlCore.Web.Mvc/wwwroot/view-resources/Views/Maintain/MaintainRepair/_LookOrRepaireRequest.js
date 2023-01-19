//# sourceURL=dynamic_LookOrUpdateRequest.js
(function () {
    app.modals.LookRepaireRequest = function () {

        var _modalManager;
        var repaireRequestService = abp.services.app.repairRequest;
        var commonService = abp.services.app.commonLookup;
        var $IsEditMode;
        var $Status;
        var $startTime;
        var $endTime;
        var $lookOrRepairForm = null;
        var $dateTimePicker;
        this.init = function (modalManager) {
            _modalManager = modalManager;
            $lookOrRepairForm = _modalManager.getModal().find("form[name=lookOrRepairForm]");
            $IsEditMode = $lookOrRepairForm.find("#IsEditMode").val() == "1" ? true : false;
            $Status = $lookOrRepairForm.find("#Status").val()
            $dateTimePicker = $(".dateTimeRangePicker").daterangepicker({
                "singleDatePicker": true,
                "timePicker": true,
                "timePicker24Hour": true,
                "drops": "down",
                locale: {
                    format: 'YYYY-MM-DD HH:mm:ss',
                    applyLabel: app.localize("Confirm"),
                    cancelLabel: app.localize("Cancel"),
                    monthNames: ['一月', '二月', '三月', '四月', '五月', '六月',
                        '七月', '八月', '九月', '十月', '十一月', '十二月'],
                    daysOfWeek: ['日', '一', '二', '三', '四', '五', '六'],
                }
            });
            $startTime = $lookOrRepairForm.find("#RepaireStartTime");
            $endTime = $lookOrRepairForm.find("#RepairEndTime");
            var cost = $lookOrRepairForm.find("#Cost");
            if ($IsEditMode == true) {
                if ($Status == "Overtime") {
                    $(".dateTimeRangePicker").change(function () {
                        var startTime = $startTime.val();
                        var endTime = $endTime.val();
                        var duration = moment(endTime).diff(moment(startTime), "seconds");
                        if (duration > 0) {
                            var h = (Math.round(duration * 100 / 3600)) / 100;
                            cost.val(h);
                        }
                        else {
                            cost.val("0.00")
                        }
                    });
                }
            }
        };

        this.save = function () {
            if (!$lookOrRepairForm.valid()) {
                return;
            }
            var formobj = {};
            formobj.IsEditMode = $IsEditMode;
            formobj.Status = $Status;
            formobj.Id = $lookOrRepairForm.find("#Id").val();

            formobj.RepairMemo = $lookOrRepairForm.find("#RepairMemo").val();
            formobj.StartTime = $startTime.val();
            if ($IsEditMode == true) {
                if ($Status == "Overtime") {
                    if (moment($endTime.val()) <= moment($startTime.val())) {
                        abp.message.error(app.localize("RepairEndDateTimeCompare"));
                        return;
                    }
                    else {
                        formobj.Cost = $lookOrRepairForm.find("#Cost").val();
                        formobj.EndTime = $endTime.val();
                    }
                }
            }
            _modalManager.setBusy(true);
            repaireRequestService.lookOrRepair(formobj).done(function (result) {
                abp.notify.info(app.localize("SavedSuccessfully"));
                _modalManager.setResult(result);
                _modalManager.close();
            }).always(function () {
                _modalManager.setBusy(false);
            });
        };
    };
})();