//# sourceURL=dynamic_CreateOrUpdateRequest.js
(function () {
    app.modals.CreateOrUpdateRepaireRequest = function () {

        var _modalManager;
        var repaireRequestService = abp.services.app.repairRequest;
        var commonService = abp.services.app.commonLookup;

        var $createOrUpdateForm = null;
        var $dateTimePicker;
        var initUserSelect2 = function () {
            repaireRequestService.listUser().done(function (response) {
                var userData = _.map(response,
                    function (item) {
                        return {
                            id: item.name,
                            text: item.value
                        }
                    });
                $("#RequestUserId").select2({
                    data: userData,
                    multiple: false,
                    language: {
                        noResults: function () {
                            return app.localize("PleaseMaintenancePersonnel");
                        }
                    }
                }).val(0).trigger('change');
            });
        }
        var initMachinesSelect2 = function () {
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
                $("#MachineCode").select2({
                    data: machinesData,
                    multiple: false,
                    language: {
                        noResults: function () {
                            return app.localize("PleaseMaintainTheEquipment");
                        }
                    }
                }).val(0).trigger('change');;
            });
            $("#MachineCode").on("select2:select", function (e) {
                var machineType = repaireRequestService.getMachineType({ id: e.params.data.id }).done(function (data) {
                    $("#MachineType").val(data.machineType);
                });
            });
        };
        this.init = function (modalManager) {
            _modalManager = modalManager;
            $createOrUpdateForm = _modalManager.getModal().find("form[name=createOrUpdateForm]");

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
                },

            });
            //初始化人员和设备
            initUserSelect2();
            initMachinesSelect2();
        };
        this.shown = function () {
            var machineCode = $("#MachineCode").attr("value");
            if (machineCode != null) {
                $("#MachineCode").val(machineCode).trigger("change");
            }
            machineCode = $("#RequestUserId").attr("value");
            if (machineCode != null) {
                $("#RequestUserId").val(machineCode).trigger("change");
            }
        }
        this.save = function () {
            if (!$createOrUpdateForm.valid()) {
                return;
            }
            var formobj = $createOrUpdateForm.serializeFormToObject();

            if ($createOrUpdateForm.data("isEdit")) {
                formobj.Id = $createOrUpdateForm.find("#Id").val();
            }
            if (formobj.IsShutdown == "0") {
                formobj.IsShutdown = false;
            }
            else {
                formobj.IsShutdown = true;
            }
            _modalManager.setBusy(true);
            repaireRequestService.createOrUpdate(formobj).done(function (result) {
                abp.notify.info(app.localize("SavedSuccessfully"));
                _modalManager.setResult(result);
                _modalManager.close();
            }).always(function () {
                _modalManager.setBusy(false);
            });
        };
    };
})();