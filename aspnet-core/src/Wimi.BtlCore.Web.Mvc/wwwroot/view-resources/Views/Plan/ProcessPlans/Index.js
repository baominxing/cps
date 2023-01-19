(function () {
    $(function () {
        var processPlanAppService = abp.services.app.processPlan;
        var productAppService = abp.services.app.product;
        var _$datepicker = $("#daterange-btn");

        _$datepicker.WIMIDaterangepicker({
            startDate: moment(),
            endDate: moment().add(30, "days"),
            "maxDate": moment().add(10,"years")
        });

        var createOrUpdatePlanModal = new app.ModalManager({
            viewUrl: abp.appPath + "Plans/CreateOrUpdatePlan",
            scriptUrl: abp.appPath + "view-resources/Views/Plan/ProcessPlans/_CreateOrUpdatePlan.js",
            modalClass: "CreateOrUpdatePlan"
        });

        var relateMachinesModal = new app.ModalManager({
            viewUrl: abp.appPath + "Plans/relateMachines",
            scriptUrl: abp.appPath + "view-resources/Views/Plan/ProcessPlans/_relateMachines.js",
            modalClass: "RelateMachines",
            modalSize: "modal-lg"
        });

        var stateModal = new app.ModalManager({
            viewUrl: abp.appPath + "Plans/State",
            scriptUrl: abp.appPath + "view-resources/Views/Plan/ProcessPlans/_State.js",
            modalClass: "State"
        });

        var shiftModal = new app.ModalManager({
            viewUrl: abp.appPath + "Plans/ShiftAmount",
            scriptUrl: abp.appPath + "view-resources/Views/Plan/ProcessPlans/_ShiftAmount.js",
            modalClass: "Shift"
        });
        var initialStartTime;
        var initialEndTime;
        var plan = {
            $table: $("#table"),
            $dataTable: null,
            query: function() {
                if (!$.fn.DataTable.isDataTable("#table")) {
                    plan.$datatable = plan.$table.WIMIDataTable({
                        "scrollX": true,
                        "responsive": false,
                        "ajax": {
                            url: abp.appAPIPath + "processPlan/listPlan",
                            data: function(d) {
                                var statusList = [];
                                var obj = document.getElementsByName("checkboxStatus");
                                for (var k in obj) {
                                    if (obj[k].checked) {
                                        statusList.push(obj[k].value);
                                    }
                                }
                                if (statusList.length > 0) {
                                    d.queryStatus = statusList;
                                }

                                d.planName = $("#input-plan-name").val();
                                d.productName = $("#input-product-name").val();

                                var pickerDate = _$datepicker.data("daterangepicker");
                                d.dateTimeFrom = pickerDate.startDate.format("YYYY-MM-DD");
                                d.dateTimeEnd = pickerDate.endDate.format("YYYY-MM-DD");
                                d.initialStartTime = initialStartTime;
                                d.initialEndTime = initialEndTime;
                            }
                        },
                        "columns": [
                            {
                                "defaultContent": "",
                                "title": app.localize("Actions"),
                                "orderable": false,
                                "width": "100px",
                                "className": "text-center not-mobile",
                                "createdCell": function (td, cellData, rowData, row, col) {
                                    var title ;
                                    if (rowData.planStatus == 3 || rowData.planStatus == 4) {
                                        title = app.localize("View");
                                    } else {
                                        title = app.localize("Editor");
                                    };
                                    $('<button class="btn btn-default btn-xs">' + title + '</button>')
                                        .appendTo($(td))
                                        .click(function () {
                                            plan.edit(rowData);
                                        });
                                    if (rowData.planStatus == 0) {
                                        $('<button class="btn btn-danger btn-xs">' + app.localize("Delete") + '</button>')
                                            .appendTo($(td))
                                            .click(function () {
                                                plan.remove(rowData);
                                            });
                                    }
                                    $('<button class="btn btn-default btn-xs">' + app.localize("State") + '</button>')
                                        .appendTo($(td))
                                        .click(function () {
                                            plan.state(rowData);
                                        });
                                }
                            },
                            {
                                "data": "planName",
                                "width": "100px",
                                "title": app.localize("PlanName")
                            },
                            {
                                "data": "planStatus",
                                "title": "状态",
                                "width": "110px",
                                "render": function (data, type, row) {
                                    if (data == 0) {
                                        return '<span class="label label-warning">' +
                                            abp.localization.localize(app.localize("Plan-New")) +
                                            '</span>';
                                    } else if (data == 1) {
                                        return '<span class="label label-danger">' +
                                            abp.localization.localize(app.localize("Plan-Pause")) +
                                            '</span>';
                                    } else if (data == 2) {
                                        return '<span class="label label-primary">' +
                                            abp.localization.localize(app.localize("Plan-InProgress")) +
                                            '</span>';
                                    } else if (data == 4) {
                                        return '<span class="label label-success">' +
                                            abp.localization.localize(app.localize("Plan-AutoComplete")) +
                                            '</span>';
                                    } else {
                                        return '<span class="label label-success">' +
                                            abp.localization.localize(app.localize("Plan-Complete")) +
                                            '</span>';
                                    }
                                }
                            },
                            {
                                "data": "machineGroupName",
                                "width": "100px",
                                "title": app.localize("BelongDeviceGroup")
                            },
                            {
                                "data": "planAmount",
                                "width": "100px",
                                "title": app.localize("PlanProduction")
                            },
                            {
                                "data": "processAmount",
                                "width": "100px",
                                "title": app.localize("CompletedQuantity")
                            },
                            {
                                "data": "targetType",
                                "title": app.localize("DimensionOfTargetQuantity"),
                                "width": "100px",
                                "render": function (data, type, row) {
                                    if (data == 0) {
                                        return abp.localization.localize(app.localize("ByDay"));
                                    } else if (data == 1) {
                                        return abp.localization.localize(app.localize("ByWeek"));
                                    } else if (data == 2) {
                                        return abp.localization.localize(app.localize("ByMonth"));
                                    } else if (data == 3) {
                                        return abp.localization.localize(app.localize("ByYear"));
                                    } else if (data == 4) {
                                        return abp.localization.localize(app.localize("ByShift"));
                                    }
                                }
                            },
                            {
                                "defaultContent": "",
                                "width": "100px",
                                "title": app.localize("TargetQuantity"),
                                "createdCell": function (td, cellData, rowData, row, col) {
                                    if (rowData.targetAmount === 0) {
                                        $("<button name='shiftAmount'value='" + rowData.planId + "'class='btn btn-default btn-xs bu'>" + app.localize("View") + "</button>")
                                            .appendTo($(td))
                                            .click(function () {
                                                plan.look(rowData);
                                            });

                                    } else {
                                        $("<span>"+rowData.targetAmount+"</span>")
                                            .appendTo($(td));
                                    }
                                }
                            },
                            {
                                "data": "yieldSummaryType",
                                "title": app.localize("ProductionAlculationMethod"),
                                "width": "110px",
                                "render": function(data, type, row) {
                                    if (data == 0) {
                                        return abp.localization.localize(app.localize("TraceTheNumberOfWorkpiece"));
                                    } else if (data == 1) {
                                        return abp.localization.localize(app.localize("AccordingToTheEquipmentOutputCounter"));
                                    }
                                }
                            },
                            {
                                "data": "productName",
                                "width": "100px",
                                "title": app.localize("ProductName")
                            },
                            {
                                "data": "planStartTime",
                                "title": app.localize("PlanStartTime"),
                                "width": "100px",
                                "render": function(data, type, full, meta) {
                                    return wimi.btl.dateTimeNoSecondFormat(data);
                                }
                            },
                            {
                                "data": "planEndTime",
                                "title": app.localize("PlanEndTime"),
                                "width": "100px",
                                "render": function(data, type, full, meta) {
                                    return wimi.btl.dateTimeNoSecondFormat(data);
                                }
                            },
                            {
                                "data": "realStartTime",
                                "title": app.localize("ActualStartTime"),
                                "width": "100px",
                                "render": function(data, type, full, meta) {
                                    return wimi.btl.dateTimeNoSecondFormat(data);
                                }
                            },
                            {
                                "data": "realEndTime",
                                "title": app.localize("ActualEndTime"),
                                "width": "100px",
                                "render": function(data, type, full, meta) {
                                    return wimi.btl.dateTimeNoSecondFormat(data);
                                }
                            }
                        ]
                    });
                } else {

                    plan.$datatable.ajax.reload();
                }
            },
            add: function() {
                createOrUpdatePlanModal.open({ id: null },
                    function() {
                        plan.query();
                    });
            },
            edit: function (rowdata) {
                $('#table_wrapper').attr("class", "dataTables_wrapper form-inline dt-bootstrap no-footer has");
                createOrUpdatePlanModal.open({ id: rowdata.planId },
                    function() {
                        plan.query();
                    });
            },
            look: function (rowData) {
                shiftModal.open({ id: rowData.planId});
            },
            state: function(rowdata) {
                processPlanAppService.isInProcessing(rowdata.planId)
                    .done(function(result) {
                        if (result == null) {
                            stateModal.open({ id: rowdata.planId },
                                function() {
                                    plan.query();
                                });
                        } else {
                            abp.message.warn("【" + result + "】" + app.localize("BeingProcessed"));
                        }
                    });
            },
            remove: function(rowdata) {
                abp.message.confirm(
                    app.localize("Plan-to-be-deleted") + rowdata.planName,
                    function(isConfirmed) {
                        if (isConfirmed) {
                            processPlanAppService
                                .deleteProcessPlan(rowdata.planId)
                                .done(function (result) {
                                    abp.notify.info(app.localize("SuccessfullyRemoved"));
                                    plan.query();
                                });
                        }
                    }
                );
            },
            relateMachines: function(rowdata) {
                relateMachinesModal.open({ planId: rowdata.id },
                    function() {
                        plan.query();
                    });
            },
            init: function () {
                initialStartTime = $("#daterange-btn").data("daterangepicker").startDate.format("YYYY-MM-DD");
                initialEndTime = $("#daterange-btn").data("daterangepicker").endDate.format("YYYY-MM-DD");
                productAppService.listProducts()
                    .done(function(response) {
                        var data = _.map(response,
                            function(item) {
                                return { id: item.name, text: item.name };
                            });
                        $("#input-product-name").select2({
                            data: data,
                            multiple: false,
                            minimumResultsForSearch: -1,
                            //placeholder: "请选择",
                            language: {
                                noResults: function () {
                                    return app.localize("NoMatchingData");
                                }
                            }
                        });
                    });
                plan.query();
            }
        };
        $("#btnQuery").click(function () {
            plan.query();
        });

        $("#btn-add-plan").click(function () {
            plan.add();
        });

        plan.init();
    });
})();