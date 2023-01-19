//# sourceURL=dynamic_RelateMachines.js

(function () {

    app.modals.RelateMachines = function () {
        var planId;
        var _modalManager;
        var planAppService = abp.services.app.plan;
        var $selectableMachineTable = $('#SelectableMachineTable');
        var $selectedMachineTable = $('#SelectedMachineTable');

        var $selectableMachineDataTable, $selectedMachineDataTable;

        var relateMachines = {
            load: function (planId) {

                if ($selectableMachineDataTable != null) {
                    $selectableMachineDataTable.destroy();
                    $selectableMachineDataTable = null;
                }

                if ($selectedMachineDataTable != null) {
                    $selectedMachineDataTable.destroy();
                    $selectedMachineDataTable = null;
                }

                planAppService.listRelateMachines(planId).done(function (dataSet) {

                    $selectableMachineDataTable = $selectableMachineTable.WIMIDataTable({
                        serverSide: false,
                        "scrollX": true,
                        "responsive": false,
                        data: dataSet.unSelectedMachines,
                        columns: [
                            {
                                "defaultContent": "",
                                "title": app.localize("Actions"),
                                "orderable": false,
                                "width": "50px",
                                "className": "text-center not-mobile",
                                "createdCell": function (td, cellData, rowData, row, col) {
                                    var $selectButton = $("<button>" + app.localize("Select") + "</button>");
                                    $selectButton.on("click", function () {
                                        relateMachines.select(rowData);
                                    });

                                    $(td).append($selectButton);
                                }
                            },
                            { "data": "machineCode", "title": app.localize("MachineCode") },
                            { "data": "machineName", "title": app.localize("MachineName") }
                        ]
                    });

                    $selectedMachineDataTable = $selectedMachineTable.WIMIDataTable({
                        serverSide: false,
                        "scrollX": true,
                        "responsive": false,
                        data: dataSet.selectedMachines,
                        columns: [
                            {
                                "defaultContent": "",
                                "title": app.localize("Actions"),
                                "orderable": false,
                                "width": "50px",
                                "className": "text-center not-mobile",
                                "createdCell": function (td, cellData, rowData, row, col) {
                                    var $selectButton = $("<button>" + app.localize("Remove") + "</button>");
                                    $selectButton.on("click", function () {
                                        relateMachines.remove(rowData);
                                    });

                                    $(td).append($selectButton);
                                }
                            },
                            { "data": "machineCode", "title": app.localize("MachineCode") },
                            { "data": "machineName", "title": app.localize("MachineName") }
                        ]
                    });
                });
            },
            select: function (rowData) {

                planAppService
                    .addMachineIntoPlan({ planId: planId, machineId: rowData.id })
                    .done(function (result) {
                        abp.notify.info(app.localize("SavedSuccessfully"));
                        relateMachines.load(planId);
                    }).always(function () {
                        _modalManager.setBusy(false);
                    });
            },
            remove: function (rowData) {
                planAppService
                    .deleteMachineFromPlan({ planId: planId, machineId: rowData.id })
                    .done(function (result) {
                        abp.notify.info(app.localize("SavedSuccessfully"));
                        relateMachines.load(planId);
                    }).always(function () {
                        _modalManager.setBusy(false);
                    });
            }
        }

        this.shown = function () {
            planId = $("#plan-id").val();
            relateMachines.load(planId);
        };

        this.init = function (modalManager) {
            _modalManager = modalManager;
        };
    }
})();