(function () {
    $(function () {
        var maintainPlanAppService = abp.services.app.maintainPlan;
        var commonService = abp.services.app.commonLookup;

        var createOrUpdateModal = new app.ModalManager({
            viewUrl: abp.appPath + "MaintainPlan/CreateOrUpdateModal",
            scriptUrl: abp.appPath + "view-resources/Views/Maintain/MaintainPlan/_CreateOrUpdateModal.js",
            modalClass: "CreateOrUpdateModal"
        });

        var maintainPlan = {
            $table: $("#table"),
            machineContainer: $("#input-machine-code"),
            $dataTable: null,
            query: function () {
                if (!$.fn.DataTable.isDataTable("#table")) {

                    maintainPlan.$datatable = maintainPlan.$table.WIMIDataTable({
                        "ajax": {
                            url: abp.appAPIPath + "maintainPlan/listMaintainPlan",
                            data: function (d) {
                                var statusList = [];
                                $('input[name="checkboxStatus"]:checked').each(function () {
                                    statusList.push($(this).val());
                                });

                                if (statusList.length > 0) {
                                    d.statusList = statusList;
                                }

                                d.code = $("#input-plan-code").val();
                                d.name = $("#input-plan-name").val();
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
                                "width": "10%",
                                "className": "text-center",
                                "createdCell": function (td, cellData, rowData, row, col) {
                                    $(td).buildActionButtons([
                                        {
                                            title: rowData.status === "Done" ? app.localize("View") : app.localize("Editor"),
                                            clickEvent: function () {
                                                maintainPlan.edit(rowData);
                                            },
                                            isShow: true
                                        },
                                        {
                                            title: app.localize("Delete"),
                                            clickEvent: function () {
                                                maintainPlan.remove(rowData);
                                            },
                                            isShow: rowData.status === "New",
                                            className: "btn-danger"
                                        }
                                    ]);
                                }
                            },
                            { "data": "code", "title": app.localize("PlanCode") },
                            { "data": "name", "title": app.localize("PlanName") },
                            { "data": "machineCode", "title": app.localize("MachineCode") },
                            { "data": "machineName", "title": app.localize("MachineName") },
                            {
                                "data": "status", "title": app.localize("State"), "render": function (data, type, row) {
                                    return abp.localization.localize("MaintainPlan-" + data);
                                }
                            },
                            {
                                "data": "startDate",
                                "title": app.localize("StartTime"),
                                "render": function (data, type, full, meta) {
                                    return wimi.btl.dateFormat(data);
                                }
                            },
                            {
                                "data": "endDate",
                                "title": app.localize("EndTime"),
                                "render": function (data, type, full, meta) {
                                    return wimi.btl.dateFormat(data);
                                }
                            },
                            { "data": "intervalDate", "title": app.localize("IntervalTime") },
                            { "data": "personInChargeName", "title": app.localize("ResponsiblePerson") }
                        ]
                    });
                } else {

                    maintainPlan.$datatable.ajax.reload();
                }
            },
            add: function () {
                createOrUpdateModal.open({ id: null }, function () {
                    maintainPlan.query();
                });
            },
            edit: function (rowdata) {
                createOrUpdateModal.open({ id: rowdata.id }, function () {
                    maintainPlan.query();
                });
            },
            remove: function (rowdata) {
                abp.message.confirm(
                    app.localize("WillDeletePlan{0}", rowdata.name),
                    function (isConfirmed) {
                        if (isConfirmed) {
                            maintainPlanAppService
                                .delete({ id: rowdata.id })
                                .done(function (result) {
                                    abp.notify.info(app.localize("SuccessfullyRemoved"));
                                    maintainPlan.query();
                                });
                        }
                    }
                );
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
                    maintainPlan.machineContainer.select2({
                        data: machinesData,
                        multiple: false,
                        language: {
                            noResults: function () {
                                return app.localize("PleaseMaintainTheEquipment");
                            }
                        }
                    }).val(0).trigger('change');
                });
            },
            init: function () {
                maintainPlan.initSelect2Plugin();
                maintainPlan.query();
            }
        }

        $("#btnQuery").click(function () {
            maintainPlan.query();
        });

        $("#btn-add-maintainPlan").click(function () {
            maintainPlan.add();
        });

        maintainPlan.init();
    });
})();