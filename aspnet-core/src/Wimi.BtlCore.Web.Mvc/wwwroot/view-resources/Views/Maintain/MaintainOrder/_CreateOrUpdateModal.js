//# sourceURL=dynamic_CreateOrUpdateModal.js
(function () {
    app.modals.CreateOrUpdateModal = function () {
        var _manager,
            _args,
            _form = null;
        var $userContainer = $("#userContainer");
        var $startTime = $("#StartTime");
        var $endTime = $("#EndTime");
        var maintainOrderStatus = $("#Status").val();
        var staffPerformance = abp.services.app.staffPerformance;
        var maintainOrderAppService = abp.services.app.maintainOrder;

        var dateTimerangepickerOption = {
            singleDatePicker: true,
            locale: {
                format: 'YYYY-MM-DD HH:mm:ss',
                applyLabel: app.localize("Confirm"),
                cancelLabel: app.localize("Cancel"),
                monthNames: ['一月', '二月', '三月', '四月', '五月', '六月',
                    '七月', '八月', '九月', '十月', '十一月', '十二月'],
                daysOfWeek: ['日', '一', '二', '三', '四', '五', '六'],
            },
            timePickerIncrement: 1,
            timePicker24Hour: true,
            timePicker: true,
            autoApply: true,
            autoUpdateInput: true
        };

        this.init = function (manager, args) {
            _manager = manager;
            _args = args;
            _form = _manager.getModal().find("form[name='createOrUpdateForm']");

            initSelect2Plugin();
            calculateMaintenanceTimeConsuming();
            $startTime.daterangepicker(dateTimerangepickerOption);
            $endTime.daterangepicker(dateTimerangepickerOption);

            if (maintainOrderStatus === "Done") {
                $startTime.attr("disabled", "disabled");
                $endTime.attr("disabled", "disabled");
                $userContainer.attr("disabled", "disabled");
                $("#Memo").attr("disabled", "disabled");
                $(".save-button").css({ "display":"none"})
            }
        }

        this.save = function () {

            if (!_form.valid()) {
                return false;
            }

            //开始时间不能晚于结束时间
            if (moment($startTime.val()) >= moment($endTime.val())) {
                abp.message.error(app.localize("StartAndEndTimeValidate"));
                return;
            }

            var formObject = _form.serializeFormToObject();
            formObject.Memo = $("#Memo").val();
            _manager.setBusy(true);
            maintainOrderAppService.update(formObject)
                .done(function (response) {
                    abp.notify.info(app.localize("SavedSuccessfully"));
                    _manager.close();
                    _manager.setResult(response);
                })
                .always(function () {
                    _manager.setBusy(false);
                });
        };
        function calculateMaintenanceTimeConsuming() {
            if (maintainOrderStatus !== "Done") {
                $endTime.on("change", function () {
                    var startTime = new Date($startTime.val());
                    var endTime = new Date($endTime.val());
                    var result = ((endTime - startTime) / 3600000).toFixed(2);
                    $("#MaintenanceTimeConsuming").val(result);
                });
            }
        }
        function initSelect2Plugin() {
            staffPerformance.getStaffList().done(function (result) {
                if (result != null && result.length > 0) {

                    var data = _.map(result, function (item) {
                        return { id: item.userId, text: item.userName }
                    });

                    $userContainer.select2({
                        data: data,
                        multiple: false,
                        placeholder: app.localize("PleaseSelect"),
                        language: {
                            noResults: function () {
                                return app.localize("PleaseMaintenancePersonnel");
                            }
                        }
                    });
                }

                $userContainer.select2().val($("#MaintainUserId").val()).trigger("change");
            });
        }
    }
})();