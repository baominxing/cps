//# sourceURL=dynamic_CreateOrUpdateModal.js
(function () {
    app.modals.CreateOrUpdateModal = function () {
        console.log(22)
        var _manager,
            _args,
            _form = null,
            $machineContainer = $("#machineContainer"),
            $userContainer = $("#userContainer");
        var $startDate = $("#StartDate");
        var $endDate = $("#EndDate");
        var pageState = $("#PageState").val();
        var maintainPlanStatus = $("#Status").val();
        var planName = $("#PlanName");
        var planMemo = $("#PlanMemo");
        var maintainPlanAppService = abp.services.app.maintainPlan;
        var commonService = abp.services.app.commonLookup;
        var staffPerformance = abp.services.app.staffPerformance;

        var daterangepickerOption = {
            singleDatePicker: true,
            locale: $.WIMI.options.dateRangePicker.commonLocal,
            autoApply: true,
            autoUpdateInput: true,
            dateForamt: "YYYY-MM-DD",
            datetimeForamt: "YYYY-MM-DD HH:mm"
        };

        this.init = function (manager, args) {
            _manager = manager;
            _args = args;
            _form = _manager.getModal().find("form[name='createOrUpdateForm']");

            initSelect2Plugin();
             
            $startDate.daterangepicker(daterangepickerOption);
            $endDate.daterangepicker(daterangepickerOption);

            if (maintainPlanStatus !== "New") {
                $machineContainer.attr("disabled", "disabled");
                $userContainer.attr("disabled", "disabled");
                $startDate.attr("disabled", "disabled");
                $("#IntervalDate").attr("disabled", "disabled");
            }
            if (maintainPlanStatus === "Done") {
                $endDate.attr("disabled", "disabled");
                planName.attr("disabled", "disabled");
                planMemo.attr("disabled", "disabled");
                $(".save-button").css({ "display": "none" })
            }
        }

        this.save = function () {
            if (!_form.valid()) {
                return false;
            }

            //开始时间不能晚于结束时间
            if (moment($startDate.val()) > moment($endDate.val())) {
                abp.message.error(app.localize("StartAndEndTimeValidate"));
                return false;
            }

            var formObject = _form.serializeFormToObject();

            if ($machineContainer.val() === "") {
                abp.message.error(app.localize("PleaseSelectEquipment"));
                return;
            } else {
                formObject.MachineId = $machineContainer.val();
            }

            if ($userContainer.val() === "") {
                abp.message.error(app.localize("PleaseSelectPerson"));
                return;
            } else {
                formObject.PersonInChargeId = $userContainer.val();
            }

            _manager.setBusy(true);
            if (pageState === "New") {
                maintainPlanAppService.create(formObject)
                    .done(function (response) {
                        abp.notify.info(app.localize("SavedSuccessfully"));
                        _manager.close();
                        _manager.setResult(response);
                    })
                    .always(function () {
                        _manager.setBusy(false);
                    });
            } else {
                maintainPlanAppService.update(formObject)
                    .done(function (response) {
                        abp.notify.info(app.localize("SavedSuccessfully"));
                        _manager.close();
                        _manager.setResult(response);
                    })
                    .always(function () {
                        _manager.setBusy(false);
                    });
            }
        };

        function initSelect2Plugin() {
            commonService.getDeviceGroupAndMachineWithPermissions().done(function (response) {
                var machines = _.chain(response.machines)
                    .filter(
                    function (item) {
                        return _.contains(response.grantedGroupIds, item.deviceGroupId);
                    })
                    .map(function (item) {
                        var machine = { id: item.id, text: item.name };
                        return machine;
                    });

                var machinesData = [];
                _.each(_.groupBy(machines._wrapped, "id"),
                    function (item) {
                        machinesData.push(item[0]);
                    });
                $machineContainer.select2({
                    data: machinesData,
                    multiple: false,
                    placeholder: app.localize("PleaseSelect"),
                    minimumResultsForSearch: -1,
                    language: {
                        noResults: function () {
                            return app.localize("PleaseMaintainTheEquipment");
                        }
                    }
                });

                $machineContainer.select2().val($("#MachineId").val()).trigger("change");
            });

            staffPerformance.getStaffList().done(function (result) {
                if (result != null && result.length > 0) {

                    var data = _.map(result, function (item) {
                        return { id: item.userId, text: item.userName };
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

                $userContainer.select2().val($("#PersonInChargeId").val()).trigger("change");
            });
        }
    }
})();