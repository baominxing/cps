//# sourceURL=dynamic_ImportModal.js
(function () {
    app.modals.ImportModal = function () {
        var _basicDataService = abp.services.app.basicData,
            commonService = abp.services.app.commonLookup;
        var _modalManager;
        this.init = function (modalManager) {
            _modalManager = modalManager;
            initMachines();
        }

        function initMachines(machineId) {
            commonService.getDeviceGroupAndMachineWithPermissions().done(function (response) {
                var machines = _.chain(response.machines)
                    .filter(
                        function (item) {
                            return _.contains(response.grantedGroupIds, item.deviceGroupId);
                        })
                    .map(function (item) {
                        var machine = {
                            id: item.id,
                            text: item.name
                        };
                        return machine;
                    });

                var machinesData = [];
                _.each(_.groupBy(machines._wrapped, "id"),
                    function (item) {
                        machinesData.push(item[0]);
                    });

                $("#MachineId").select2({
                    data: machinesData,
                    multiple: false,
                    placeholder: app.localize("PleaseSelect"),
                    language: {
                        noResults: function () {
                            return app.localize("PleaseMaintainTheEquipment");
                        }
                    }
                }).val(_.first(machinesData).id).trigger('change');
            });
        }

        $("#Save")
            .click(function (e) {
                $("#ImportDataForm").submit();
            });

        $("#ImportDataForm").ajaxForm({
            async: false,
            beforeSubmit: function (formData, jqForm, options) {

                var $fileInput = $("#ImportDataForm input[name=UploadFiles]");
                var files = $fileInput.get()[0].files;

                if (!files.length) {
                    abp.message.warn(app.localize("ChooseFiles"));
                    return false;
                }
                var file = files[0];

                var type = "|" + file.name.slice(file.name.lastIndexOf(".") + 1) + "|";
                if ("|xlsx|xls|".indexOf(type) === -1) {
                    abp.ui.clearBusy();
                    abp.message.warn(app.localize("PleaseChooseExcel"));
                    return false;
                }

                //File size check
                if (file.size > 5242880) //1MB
                {
                    abp.ui.clearBusy();
                    abp.message.warn(app.localize("FileChooseSizeLimit"));
                    return false;
                }
                return true;
            },
            success: function (response) {
                if (response.success) {
                    abp.ui.clearBusy();
                    if (response.result.value === true) {
                        abp.message.success(app.localize("UploadSuccess"));
                        abp.event.trigger("app.CreateOrUpdateModalSaved");
                        _modalManager.close();
                    } else {
                        abp.message.error(response.result.value, app.localize("DataImportFailure"));
                    }
                   
                } else {
                    abp.ui.clearBusy();
                    abp.message.error(response);
                    _modalManager.close();
                }
            }
        });
    }
})(jQuery);