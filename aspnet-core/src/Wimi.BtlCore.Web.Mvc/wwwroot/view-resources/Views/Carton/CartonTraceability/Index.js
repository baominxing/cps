(function () {
    $(function () {

        var service = abp.services.app.cartonTraceability;
        var deviceGroupAppService = abp.services.app.deviceGroup;
        var table = null;

        var vm = new Vue({
            el: "#contentPanel",
            data: {
                carton: null,
                inPackTabActive: true,
                allProTabActive: false
            }
        });


        var tabs = {
            $parttable: $("#inPackTable"),
            partDatatables: null,
            $logtable: $("#allProTable"),
            logDatatables: null,
            init: function () {
                service.checkUndoPermission(tabs.getParams()).done(function (response) {
                    tabs.loadParttable(response);
                });
            },
            export: function () {
                service.exportTraceabilityRecords(table.getParams()).done(function (response) {
                    app.downloadTempFile(response);
                });
            },
            undoPacking: function (rowData) {
                abp.message.confirm(app.localize("DeleteDataEncoding{0}", rowData.partNo),
                    function (isConfirmed) {
                        if (isConfirmed) {
                            service.delete({ Id: rowData.id }).done(function () {
                                if (table) {
                                    table.reload();
                                }
                            });
                        }
                    });
            },
            getParams: function () {
                const param = {
                    Id: vm.carton !== null && typeof (vm.carton) != "undefined" ? vm.carton.id : 0,
                    CartonNo: vm.carton !== null && typeof (vm.carton) != "undefined" ? vm.carton.cartonNo : "",
                    DeviceGroupId: vm.carton !== null && typeof (vm.carton) != "undefined" ? vm.carton.deviceGroupId : 0
                };

                return param;
            },
            loadParttable: function (data) {

                if (this.partDatatables) {
                    tabs.partDatatables.destroy();
                    tabs.partDatatables.clear();
                }

                const param = tabs.getParams();

                service.listPartsInCarton(param).done(function (response) {
                    tabs.partDatatables = tabs.$parttable.WIMIDataTable({
                        data: response,
                        serverSide: false,
                        retrieve: true,
                        responsive: false,
                        ordering: false,
                        order: [],
                        scrollCollapse: true,
                        scrollX: true,
                        columns: [
                            {
                                "defaultContent": "",
                                "title": app.localize('Actions'),
                                "orderable": false,
                                "width": "30px",
                                "createdCell": function (td, cellData, rowData, row, col) {

                                    var $td = $(td);

                                    if (data) {
                                        $('<button class="btn btn-danger btn-xs">' + app.localize('UndoCarton')+'</button> ')
                                            .appendTo($td)
                                            .click(function () {
                                                tabs.undoPacking(rowData);
                                            });
                                    }

                                }
                            },
                            {
                                "data": "partNo",
                                "title": app.localize("WorkpieceCoding")
                            },
                            {
                                "data": "shiftDay",
                                "title": app.localize("PackingShiftDay"),
                                "render": function (data) {
                                    return wimi.btl.dateFormat(data);
                                }
                            },
                            {
                                "data": "shiftSolutionItemName",
                                "title": app.localize("Shift")
                            },
                            {
                                "data": "optionTime",
                                "title": app.localize("OperationTime"),
                                "render": function (data) {
                                    return wimi.btl.dateTimeFormat(data);
                                }
                            }
                        ],
                        drawCallback: function (settings) {
                            tabs.partDatatables = this.api();

                            if (!vm.carton && typeof (vm.carton) != "undefined") {
                                service.checkUndoPermission(tabs.getParams()).done(function (response) {
                                    var column = tabs.partDatatables.column('0');
                                    column.visible(response);
                                });
                            }
                        }
                    });

                });
            },
            loadLogtable: function () {

                if (this.logDatatables) {
                    tabs.logDatatables.destroy();
                    tabs.logDatatables.clear();
                }

                const param = tabs.getParams();

                service.listCartonRecords(param).done(function (response) {
                    tabs.logDatatables = tabs.$logtable.WIMIDataTable({
                        data: response,
                        serverSide: false,
                        retrieve: true,
                        responsive: false,
                        ordering: false,
                        order: [],
                        scrollCollapse: true,
                        scrollX: true,
                        columns: [
                            {
                                "data": "partNo",
                                "title": app.localize("WorkpieceCoding")
                            },
                            {
                                "data": "type",
                                "title": app.localize("OperationType"),
                                "render": function (data) {
                                    if (!data) {
                                        return '<span class="label label-primary">' + app.localize("Packing") + '</span>';
                                    } else {
                                        return '<span class="label label-danger">' + app.localize("UndoPacking") + '</span>';
                                    }
                                }
                            },
                            {
                                "data": "optionTime",
                                "title": app.localize("OperationTime"),
                                "render": function (data) {
                                    return wimi.btl.dateTimeFormat(data);
                                }
                            }
                        ]
                    });
                });
            }
        };


        table = {
            $table: $("#cartonTable"),
            datatables: null,
            init: function (callback) {
                if (this.datatables) {
                    table.datatables.destroy();
                    table.datatables.clear();
                }

                table.bindingrowClickEvent(callback);
                abp.ui.setBusy();

                this.datatables = this.$table.WIMIDataTable({
                    "ajax": {
                        url: abp.appAPIPath + "cartonTraceability/listTraceabilityRecords",
                        data: function (d) {
                            $.extend(d, table.getParams());
                        }
                    },
                    serverSide: true,
                    retrieve: true,
                    responsive: false,
                    scrollCollapse: true,
                    scrollX: true,
                    columns: table.getColumns(),
                    drawCallback: function(settings) {
                        table.datatables = this.api();
                        var rows = this.api().rows();

                        vm.carton = null;
                        if (rows.nodes().length === 0) {
                            $(this).find('tr:eq(0)').trigger('click');
                        } else {
                            $(rows.nodes()[0]).trigger('click');
                        }
                    }
                });

                abp.ui.clearBusy();
            },
            getColumns: function () {
                return [
                    {
                        "data": "cartonNo",
                        "title": app.localize("CartonNo")
                    },
                    {
                        "orderable": false,
                        "data": "deviceGroupName",
                        "title": app.localize("DeviceGroup")
                    },
                    {
                        "data": "maxPackingCount",
                        "title": app.localize("MaxPackingCount")  
                    },
                    {
                        "data": "realPackingCount",
                        "title": app.localize("RealPackingCount")
                    },
                    {
                        "data": "printLabelCount",
                        "title": app.localize("LabelPrintCount")
                    },
                    {
                        "data": "creationTime",
                        "title": app.localize("OperationTime"),
                        "render": function (data) {
                            return wimi.btl.dateTimeFormat(data);
                        }
                    }
                ];
            },
            getParams: function () {
                return {
                    CartonNo: $("#CartonNo").val(),
                    PartNo: $("#PartNo").val(),
                    DeviceGroupId: $("#DeviceGroup").val(),
                    StartTime: $("#daterange-start").val(),
                    EndTime: $("#daterange-end").val()
                };
            },
            reload: function () {
                this.datatables.ajax.reload();
            },
            bindingrowClickEvent: function (callback) {
                table.$table.on('click',
                    'tr',
                    function () {
                        if ($(this).hasClass("selected")) {
                            return;
                        } else {
                            table.datatables.$("tr.selected").removeClass("selected").css("background-color", "");
                            $(this).addClass("selected").css("background-color", "#a8e4ff");
                            var row = table.datatables.row($(this)).data();

                            // 绑定数据
                            vm.carton = row;

                            callback();

                            tabs.init();
                        }
                    });
            }
        };

        var page = {
            init: function () {
                this.initdeviceGroup();
                this.initdatetimepicker();
                this.bindingQueryEvent();
                this.bindingTabClickEvent();
                this.bindingExportEvent();
            },
            initdeviceGroup: function () {
                deviceGroupAppService.listFirstClassDeviceGroups()
                    .done(function (response) {
                        var data = _.map(response,
                            function (item) {
                                return { id: item.id, text: item.displayName };
                            });
                        data.unshift({ id: 0, text: app.localize("All") });

                        $("#DeviceGroup").select2({
                            data: data,
                            multiple: false,
                            minimumResultsForSearch: -1,
                            placeholder: app.localize("PleaseChoose"),
                            language: {
                                noResults: function () {
                                    return app.localize("NoMatchingData");
                                }
                            }
                        });

                        table.init(page.setPackTabActive);
                    });
            },
            initdatetimepicker: function () {
                var dateOption = {
                    singleDatePicker: true,
                    maxDate: moment().add(360, 'years'),
                    timePicker: true,
                    timePicker24Hour: true,
                    "autoApply": true,
                    locale: {
                        format: 'YYYY-MM-DD HH:mm:ss',
                        applyLabel: app.localize("Confirm"),
                        cancelLabel: app.localize("Clear"),
                        monthNames: ['一月', '二月', '三月', '四月', '五月', '六月',
                            '七月', '八月', '九月', '十月', '十一月', '十二月'],
                        daysOfWeek: ['日', '一', '二', '三', '四', '五', '六'],
                    },
                    showWeekNumbers: false,
                };

                $("#daterange-start").WIMIDaterangepicker(dateOption)
                    .on('cancel.daterangepicker', function (ev, picker) { $(this).val(''); });

                $("#daterange-end").WIMIDaterangepicker($.extend({
                    "endTimeOnly": true,
                    "startDate": moment().add(1,'days').format("YYYY-MM-DD")
                }, dateOption))
                    .on('cancel.daterangepicker', function (ev, picker) { $(this).val(''); });
            },
            bindingQueryEvent: function () {
                $("#searchBtn").click(function () {
                    table.init();
                });
            },
            bindingExportEvent: function () {
                $("#exportBtn").click(function () {
                    tabs.export();
                });
            },
            bindingTabClickEvent: function () {

                $("#inPackTab").click(function () {
                    page.setPackTabActive();
                    tabs.init();
                });

                $("#allProTab").click(function () {
                    page.setProTabActive();
                    tabs.loadLogtable();
                });
            },
            setPackTabActive: function () {
                vm.inPackTabActive = true;
                vm.allProTabActive = false;
            },
            setProTabActive: function () {
                vm.inPackTabActive = false;
                vm.allProTabActive = true;
            }
        };


        page.init();
    });
})();