//# sourceURL=dynamicFeedbackModalModal.js

(function() {
    app.modals.FeedbackModal = function() {
        var reasonFeedbackService = abp.services.app.reasonFeedback;
        var _modalManager;
        var feedbackForm = null;
        var btnChange = null;
        var startTimeInput = null;
        var checkBoxContainer = null;
        var machineId = null;
        var changed = false;
        var insertReasonFeedbackRecord = function(memo) {
            _modalManager.setBusy(true);
            reasonFeedbackService.createReasonFeedback({
                    MachineId: machineId,
                    StateId: memo.stateId,
                    StateCode: memo.stateCode,
                    StartTime: memo.startTime
                })
                .done(function(response) {
                    abp.notify.info(app.localize("SavedSuccessfully"));
                    _modalManager.setResult();
                    _modalManager.close();

                }).always(function() {
                    _modalManager.setBusy(false);
                });
        };

        this.init = function(modalManager, args) {
            machineId = args.Id;
            _modalManager = modalManager;
            feedbackForm = _modalManager.getModal().find("form[name=FeedbackForm]");
            startTimeInput = _modalManager.getModal().find('.startTime');
            btnChange = _modalManager.getModal().find("#btnChange");
            checkBoxContainer = _modalManager.getModal().find(".checkContainer");
            var template = _modalManager.getModal().find("#machineFeedback-template");

            reasonFeedbackService.listFeedbackType().done(function(data) {
                //初始化模板
                var source = template.html();
                template = Handlebars.compile(source);
                var html = template(data);
                checkBoxContainer.html(html);
                var currentTime = moment();
                var minTime = moment().subtract(7, 'day').format("YYYY-MM-DD HH:mm:ss");
                var daterangepickerOption = {
                    timePicker: true,
                    singleDatePicker: true,
                    locale: {
                        format: 'YYYY-MM-DD HH:mm:ss',
                        applyLabel: app.localize("Confirm"),
                        cancelLabel: app.localize("Cancel")
                    },
                    timePickerIncrement: 1,
                    startDate: currentTime.format("YYYY-MM-DD HH:mm:ss"),
                    timePicker24Hour: true,
                    minDate: minTime,
                    maxDate: currentTime.format("YYYY-MM-DD HH:mm:ss"),
                    autoApply: true,
                    autoUpdateInput: true
                };
                startTimeInput.WIMIDaterangepicker(daterangepickerOption);
                startTimeInput.attr("disabled", true);
            });
        };

        this.shown = function() {
            btnChange.click(function() {
                startTimeInput.attr("disabled", false);
                startTimeInput.removeAttr("readonly");
                changed = true;
            });
            checkBoxContainer.find("input").first().attr("checked", true);
        }

        this.save = function() {
            if (!feedbackForm.valid()) {
                return;
            }

            var memo = feedbackForm.serializeFormToObject();
            memo.stateId = _modalManager.getModal().find("input:checked").data("stateid");

            if (!memo.stateCode) {
                abp.message.error(app.localize("ReasonsForSelectionFeedback"));
                return;
            }
            memo.startTime = $(".startTime").val();
            //判断修改的时间是否与已有的反馈时间重合
            reasonFeedbackService.checkStartTime({ StartTime: memo.startTime, MachineId: machineId }).done(
                function (response) {
                    if (response.result === "Over") {
                        abp.message.error(app.localize("StartTimeCannotExceedCurrentTime7Days"));
                    } else if (response.result === "Error") {
                        abp.message.error(
                            app.localize(
                                "TheStartTimeOverlapsWithTheExistingFeedback",
                                wimi.btl.dateTimeFormat(response.errorReason.startTime),
                                wimi.btl.dateTimeFormat(response.errorReason.endTime)
                            )
                        );
                    } else {
                        insertReasonFeedbackRecord(memo);
                    }
                });

        };
    };
})();