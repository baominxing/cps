(function () {
    $(function () {
        var maintainOrderAppService = abp.services.app.maintainOrder;
        var commonService = abp.services.app.commonLookup;

        var createOrUpdateModal = new app.ModalManager({
            viewUrl: abp.appPath + "MaintainOrder/CreateOrUpdateModal",
            scriptUrl: abp.appPath + "view-resources/Views/Maintain/MaintainOrder/_CreateOrUpdateModal.js",
            modalClass: "CreateOrUpdateModal"
        });

        var maintainOrder = {
            $table: $("#table"),
            machineContainer: $("#input-machine-code"),
            $dataTable: null,
            query: function () {
                if (!$.fn.DataTable.isDataTable("#table")) {

                    maintainOrder.$datatable = maintainOrder.$table.WIMIDataTable({
                        "ajax": {
                            url: abp.appAPIPath + "maintainOrder/listMaintainOrder",
                            data: function (d) {
                                var statusList = [];
                                $('input[name="checkboxStatus"]:checked').each(function () {
                                    statusList.push($(this).val());
                                });

                                if (statusList.length > 0) {
                                    d.statusList = statusList;
                                }

                                d.code = $("#input-order-code").val();
                                d.maintainPlanCode = $("#input-plan-code").val();
                                d.machineId = $("#input-machine-code").val() == null ? 0 : $("#input-machine-code").val();
                            }
                        },
                        "responsive": false,
                        "scrollX": true,
                        "columns": [
                            {
                                "defaultContent": "",
                                "title": app.localize("Actions"),
                                "orderable": false,
                                "className": "text-center",
                                "createdCell": function (td, cellData, rowData, row, col) {
                                    $(td).buildActionButtons([
                                        {
                                            title: rowData.status === "Done" ? app.localize("View") : app.localize("Maintain"),
                                            clickEvent: function () {
                                                maintainOrder.edit(rowData);
                                            },
                                            isShow: true
                                        }
                                    ]);
                                }
                            },
                            { "data": "code", "title": app.localize("WorkOrderNumber") },
                            { "data": "maintainPlanCode", "title": app.localize("PlanCode") },
                            { "data": "machineCode", "title": app.localize("MachineCode") },
                            { "data": "machineName", "title": app.localize("MachineName") },
                            {
                                "data": "status",
                                "title": app.localize("State"),
                                "orderable": false,
                                "render": function (data, type, row) {
                                    return abp.localization.localize("MaintainOrder-" + data);
                                }
                            },
                            {
                                "data": "scheduledDate",
                                "title": app.localize("PlanMaintainDate"),
                                "render": function (data, type, full, meta) {
                                    return wimi.btl.dateFormat(data);
                                }
                            },
                            {
                                "data": "startTime",
                                "title": app.localize("StartTime"),
                                "render": function (data, type, full, meta) {
                                    return wimi.btl.dateTimeFormat(data);
                                }
                            },
                            {
                                "data": "endTime",
                                "title": app.localize("EndTime"),
                                "render": function (data, type, full, meta) {
                                    return wimi.btl.dateTimeFormat(data);
                                }
                            },
                            { "data": "cost", "title": app.localize("MaintainDuration") },
                            { "data": "maintainUserName", "title": app.localize("MaintenancePersonnel") },
                        ]
                    });
                } else {

                    maintainOrder.$datatable.ajax.reload();
                }
            },
            add: function () {
                createOrUpdateModal.open({ id: null }, function () {
                    maintainOrder.query();
                });
            },
            edit: function (rowdata) {
                createOrUpdateModal.open({ id: rowdata.id }, function () {
                    maintainOrder.query();
                });
            },

            initSelect2Plugin: function () {
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

                    var machinesData = [{ id: 0, text: app.localize("All") }];
                    //_.each(_.groupBy(machines._wrapped, "id"),
                    //    function (item) {
                    //        machinesData.push(item[0]);
                    //    });
                    for (var n = 0; n < machines._wrapped.length; n++) {
                        machinesData.push(machines._wrapped[n]);
                    }
                    maintainOrder.machineContainer.select2({
                        data: machinesData,
                        multiple: false,
                        minimumResultsForSearch: -1,
                        language: {
                            noResults: function () {
                                return app.localize("PleaseMaintainTheEquipment");
                            }
                        }
                    }).val(0).trigger('change');
                });
            },
            init: function () {
                maintainOrder.initSelect2Plugin();
                maintainOrder.query();
            }
        }

        $("#btnQuery").click(function () {
            maintainOrder.query();
        });

        $("#btn-add-maintainOrder").click(function () {
            maintainOrder.add();
        });

        maintainOrder.init();
    });
})();