//# sourceURL=dynamic_CreateOrEditModal.js
(function() {
    app.modals.CreateOrUpdateModal = function() {
        var _alarmService = abp.services.app.alarms,
            commonService = abp.services.app.commonLookup,
            _modalManager,
            _$alarmInfoForm = null;

        this.init = function(modalManager) {
            _modalManager = modalManager;
            _$alarmInfoForm = _modalManager.getModal().find('form[name=AlarmInfoForm]');
            var machineId = _modalManager.getModal().find('#MachineId').val();
            initMachines(machineId);
        }

        this.save = function() {
            if (!_$alarmInfoForm.valid()) {
                return;
            }
            var alarmInfo = _$alarmInfoForm.serializeFormToObject();
            _modalManager.setBusy(true);
            alarmInfo.MachineId = $("#MachineSelect").val();

            _alarmService.updateOrEditAlarmInfo(alarmInfo)
                .done(function(result) {
                    abp.notify.info(app.localize("SavedSuccessfully"));
                    _modalManager.setResult(result);
                    abp.event.trigger("app.CreateOrUpdateModalSaved");
                    _modalManager.close();
                }).always(function() {
                    _modalManager.setBusy(false);
                });
        }

        function initMachines(machineId) {

            commonService.getDeviceGroupAndMachineWithPermissions().done(function(response) {
                var machines = _.chain(response.machines)
                    .filter(
                        function(item) {
                            return _.contains(response.grantedGroupIds, item.deviceGroupId);
                        })
                    .map(function(item) {
                        var machine = {
                            id: item.id,
                            text: item.name
                        };
                        return machine;
                    });

                var machinesData = [];
                _.each(_.groupBy(machines._wrapped, "id"),
                    function(item) {
                        machinesData.push(item[0]);
                    });


                var defaultValue = machineId ? machineId : _.first(machinesData).id;
                $("#MachineSelect").select2({
                    data: machinesData,
                    multiple: false,
                    placeholder: app.localize("PleaseSelect"),
                    minimumResultsForSearch: -1,
                    language: {
                        noResults: function () {
                            return app.localize("PleaseMaintainTheEquipment");
                        }
                    }
                }).val(defaultValue).trigger('change');
            });
        }
    }
})(jQuery);