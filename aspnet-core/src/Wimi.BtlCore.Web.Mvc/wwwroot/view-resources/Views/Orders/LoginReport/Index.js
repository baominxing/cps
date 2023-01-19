(function () {
    $(function () {
        var $btnQuery = $("#btnQuery"),
            templateRender = Handlebars.compile($("#workOrder-template").html()),
            service = abp.services.app.loginReport;

        var permissions = {
            manage: abp.auth.hasPermission("Pages.Order.LoginReport.Manage")
        };

        var loginModal = new app.ModalManager({
            viewUrl: abp.appPath + "LoginReport/WorkOrderLoginModal",
            scriptUrl: abp.appPath + "view-resources/Views/Orders/LoginReport/_WorkOrderLoginModal.js",
            modalClass: "WorkOrderLoginModal",
            modalSize: "modal-lg"
        });

        var reportModal = new app.ModalManager({
            viewUrl: abp.appPath + "LoginReport/WorkOrderReportModal",
            scriptUrl: abp.appPath + "view-resources/Views/Orders/LoginReport/_WorkOrderReportModal.js",
            modalClass: "WorkOrderReportModal"
        });

       
        $("#State").select2({
            multiple: false,
            minimumResultsForSearch: -1,
            language: {
                noResults: function () {
                    return app.localize("NoMatchingData");
                }
            }
        });


        var tableObject = {
            $table: $("#workOrderTable"),
            datatables: null,
            checkActionPermissions: function (rowData) {
                return permissions.manage && rowData.state !== 4;
            },
            load: function () {

                if (this.datatables) {
                    tableObject.datatables.destroy();
                    tableObject.$table.empty();
                }

                tableObject.datatables = tableObject.$table.WIMIDataTable({
                    "ajax": {
                        url: abp.appAPIPath + "loginReport/listWorkOrders",
                        data: { ProductionPlanCode: $.trim($("#inputCode").val()), State: $("#State").val() * 1 }
                    },
                    serverSide: true,
                    retrieve: true,
                    responsive: false,
                    ordering: true,
                    scrollCollapse: true,
                    order: [],
                    scrollX: true,
                    columns: [
                        {
                            "defaultContent": "",
                            "title": app.localize("Actions"),
                            "orderable": false,
                            "width": "30px",
                            "className": "action",
                            "createdCell": function (td, cellData, rowData, row, col) {
                                //state 关闭的工单不能做任何操作
                                $(td).buildActionButtons([
                                    {
                                        title: app.localize("LoginWith") + "/" + app.localize("EquipmentReporting"),
                                        clickEvent: function () {
                                            tableObject.login(rowData);
                                        },
                                        isShow: tableObject.checkActionPermissions(rowData)
                                    },
                                    {
                                        title: app.localize("JobReport"),
                                        clickEvent: function () { tableObject.workOrderReport(rowData) },
                                        isShow: tableObject.checkActionPermissions(rowData)
                                    },
                                    {
                                        title: app.localize("CloseWorkOrder"),
                                        clickEvent: function () { tableObject.close(rowData) },
                                        isShow: tableObject.checkActionPermissions(rowData),
                                        className: "btn-danger"
                                    }
                                ]);
                            }
                        },
                        {
                            "data": "productionPlanCode",
                            "title": app.localize("ProductionPlanCode")
                        },
                        {
                            "data": "stateName",
                            "title": app.localize("WorkOrderState"),
                            "render": function (data) {
                                switch (data) {
                                    case "Producing": return '<span class="label label-success">' + app.localize(data) + '</span>';
                                    case "NotStart": return '<span class="label label-warning">' + app.localize(data) + '</span>';
                                    case "Closed": return '<span class="label label-danger">' + app.localize(data) + '</span>';
                                    default: return '<span class="label label-primary">' + app.localize(data) + '</span>';
                                }
                            }
                        },
                        {
                            "data": "code",
                            "title": app.localize("WorkOrderCode")
                        },
                        {
                            "orderable": false,
                            "data": "isLastProcessOrder",
                            "title": app.localize("WhetherTheFinalWorkOrder"),
                            "render": function (data) {
                                if (data) {
                                    return '<span class="label label-success">' + app.localize("Yes") + '</span>';
                                } else {
                                    return '<span class="label label-primary">' + app.localize("No") + '</span>';
                             }
                        }
                        },
                        {
                            "orderable": false,
                            "data": "orderCode",
                            "title": app.localize("OrderCode")
                        },
                        {
                            "data": "productCode",
                            "title": app.localize("ProductCode")
                        },
                        {
                            "orderable": false,
                            "data": "productName",
                            "title": app.localize("ProductName")
                        },
                        {
                            "data": "craftCode",
                            "title": app.localize("CraftCode")
                        },
                        {
                            "orderable": false,
                            "data": "craftName",
                            "title": app.localize("CraftName")
                        },
                        {
                            "data": "processCode",
                            "title": app.localize("ProcessCode")
                        },
                        {
                            "orderable": false,
                            "data": "processName",
                            "title": app.localize("ProcessName")
                        },
                        {
                            "orderable": false,
                            "data": "putVolume",
                            "title": app.localize("PutVolume")
                        },
                        {
                            "orderable": false,
                            "data": "aimVolume",
                            "title": app.localize("TargetQuantity")
                        },
                        {
                            "orderable": false,
                            "data": "outputCount",
                            "title": app.localize("OutPutCount")
                        },
                        {
                            "orderable": false,
                            "data": "qualifiedCount",
                            "title": app.localize("QualifiedNumber")
                        },
                        {
                            "orderable": false,
                            "data": "defectiveCount",
                            "title": app.localize("DefectiveNumber")
                        }, {
                            "data": "standardTime",
                            "title": app.localize("StandardTime")
                        },
                        {
                            "data": "circulationRate",
                            "title": app.localize("CirculationRatio")
                        },
                        {
                            "data": "completionRate",
                            "title": app.localize("Completion"),
                            "render": function (data) {
                                return '<div class="progress"><div class="progress-bar" role="progressbar" aria-valuenow="' + data + '" aria-valuemin="0" aria-valuemax="100" style="width:100%;">' + data + '%</div></div>';
                            }
                        }
                    ]
                });
            },
            keyTableSupport: function () {
                var keys = new $.fn.dataTable.KeyTable(tableObject.datatables);
                keys.fnSetPosition(2, 0);

                //监听键盘操作事件
                /* Focus handler for all cells in the column n */
                keys.event.focus(2, null, function (node, x, y) {
                    $("td.focus").popover({
                        html: true,
                        title: app.localize("WorkOrderDetails"),
                        placement: "bottom",
                        container: 'body',
                        trigger: "focus",
                        content: function () {
                            return tableObject.showWorkOrderInfo(node);
                        }
                    });

                    $("td.focus").focusin();
                    var settings = {
                        output: "css",
                        bgColor: "#D2EAC2",
                        color: "#000000",
                        barWidth: 1,
                        barHeight: 70
                    };
                    $(".order-barcode").html("").show().barcode(node.textContent, "code128", settings);
                });

                keys.event.blur(2, null, function (node, x, y) {
                    $("td").popover('hide');
                });
            },
            reload: function () {
                this.datatables.ajax.reload(null);
            },
            login: function (rowData) {
                loginModal.open({ workOrderId: rowData.id });
            },
            workOrderReport: function (rowData) {
                reportModal.open({ workOrderId: rowData.id }, function (response) {
                    tableObject.reload();
                });
            },
            close: function (data) {

                var alartMessage = data.isLastProcessOrder ? abp.utils.formatString(app.localize("AllWorkOrders{0}WillBeClosed"), data.productionPlanCode) : app.localize("OnceClosed");
                
                var msg = abp.utils.formatString(app.localize("TargetQuantityOfThisWorkOrder"), data.aimVolume, data.outputCount, data.qualifiedCount, data.defectiveCount, alartMessage);
                abp.message.confirm(msg, app.localize("PleaseConfirm"), function (isConfirm) {
                    if (isConfirm) {
                        service.close({ id: data.id }).done(function () {
                            tableObject.reload();
                        });
                    }
                });
            },
            showWorkOrderInfo: function (data) {
                var d = tableObject.datatables.rows(data).data()[0];
                return templateRender(d);
            }
        };

        $btnQuery.click(function () {
            tableObject.load();
        });

        loginModal.onClose(function () {
            tableObject.reload();
        });

        tableObject.load();
    });
})();