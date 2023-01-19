//# sourceURL=dynamicFinishFeedbackModalModal.js
(function ($) {

    app.modals.FinishFeedbackModal = function () {
        var machineService = abp.services.app.reasonFeedback;;
        var _modalManager;
        var finishFeedbackForm = null;
        var btnChange = null;
        var startTime = null;
        var endTimeInput = null;
        var machineId = null;
        var duration = null;
        var changed = false;
        var dealTimeShow= function(date) {
            var result =date.toString().split('.');
            if (result.length === 2) {
                if (result[1].length === 2) {
                    return date;
                }
                else {
                    return date + "0";
                }
            }
            else {
                return date + ".00";
            }
        }
        var finishReasonFeedback= function(memo) {
            _modalManager.setBusy(true);
            machineService.finishReasonFeedbackRecord(memo)
                .done(function (result) {
                    abp.notify.info(app.localize("SavedSuccessfully"));
                    _modalManager.setResult();
                    _modalManager.close();

                }).always(function () {
                    _modalManager.setBusy(false);
                });
        }
        this.init = function (modalManager, args) {
            _modalManager = modalManager;
            machineId = args.Id;
            finishFeedbackForm = _modalManager.getModal().find("form[name=FinishFeedbackForm]");
            duration = _modalManager.getModal().find('.duration');
            endTimeInput = _modalManager.getModal().find('.endTime');
            startTime = _modalManager.getModal().find('.startTime');
            //初始化修改按钮事件
            endTimeInput = _modalManager.getModal().find('.endTime');
            btnChange = _modalManager.getModal().find("#btnChange");
            btnChange.click(function () {
                endTimeInput.removeAttr("readonly");
                endTimeInput.attr("disabled", false);
                changed = true;
            });
            //初始化 结束反馈Modal
            machineService.getReasonFeedbackRecord({ Id: machineId }).done(function (data) {
                var starttime = moment(data.startTime).format("YYYY-MM-DD HH:mm:ss");
                startTime.val(starttime);
                var currentTime = moment();
                var diffSecond = currentTime.diff(moment(data.startTime), 'second');
                duration.val(dealTimeShow(Math.round((diffSecond / 60 * 100)) / 100));
                var daterangepickerOption = {
                    timePicker: true,
                    singleDatePicker: true,
                    locale: {
                        format: 'YYYY-MM-DD HH:mm:ss',
                        applyLabel: app.localize("Confirm"),
                        cancelLabel: app.localize("Cancel")
                    },
                    timePickerIncrement: 1,
                    startDate: moment().format("YYYY-MM-DD HH:mm:ss"),
                    timePicker24Hour: true,
                    minDate: moment(data.startTime).format("YYYY-MM-DD HH:mm:ss"),
                    maxDate: null,
                    autoApply: true,
                    autoUpdateInput: true
                };
                endTimeInput.WIMIDaterangepicker(daterangepickerOption);
                endTimeInput.change(function () {
                    if (moment(endTimeInput.val()).isValid()) {
                        var seconds = moment(endTimeInput.val()).diff(moment(starttime), 'second');
                        duration.val(dealTimeShow(Math.round((seconds / 60 * 100)) / 100));
                    }
                });
                endTimeInput.attr("disabled", true);
            });
        };

        this.save = function () {
            if (!finishFeedbackForm.valid()) {
                return;
            }
            //判断时间格式是否正确
            if (!moment(endTimeInput.val()).isValid()) {
                abp.message.error(app.localize("PleaseEnterTheCorrectTimeFormat"));
                return;
            }
            var memo = finishFeedbackForm.serializeFormToObject();
            memo.MachineId = machineId;
            if (!changed) {
                var currentTime = moment();
                memo.EndTime = currentTime.format("YYYY-MM-DD HH:mm:ss");
                var seconds = currentTime.diff(moment(memo.StartTime), 'second');
                memo.Duration = dealTimeShow(Math.round((seconds / 60 * 100)) / 100);
            }
            else {
                if (moment(memo.EndTime) > moment()) {
                    abp.message.error(app.localize("EndTimeCannotBeLaterThanTheCurrentTime"));
                    var endTime = moment();
                    endTimeInput.val(endTime.format("YYYY-MM-DD HH:mm:ss"));
                    var diffSeconds = endTime.diff(moment(memo.StartTime), 'second');
                    duration.val(dealTimeShow(Math.round((diffSeconds / 60 * 100)) / 100));
                    return;
                }
            }
            machineService.checkEndTime(memo).done(function (response) {
                if (response.result === "True") {
                    abp.message.error(app.localize("TheEndTimeOverlapsWithTheExistingFeedback", response.errprTimeRange));
                }
                else {
                    finishReasonFeedback(memo);
                }
            });
        }
    };
})()