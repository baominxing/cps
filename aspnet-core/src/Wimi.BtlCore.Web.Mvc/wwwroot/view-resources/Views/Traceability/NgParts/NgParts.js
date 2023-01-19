(function () {
    $(function () {
        var service = abp.services.app.trace
        var datePickerStart = $("#daterange-start");
        var datePickerEnd = $("#daterange-end");

        var permissions = {
            manage: abp.auth.hasPermission("Pages.Traceability.NgParts.Manage"),
            trace: abp.auth.hasPermission("Pages.Traceability.CatalogQuery")
        };

        var showDefectsModal = new app.ModalManager({
            viewUrl: abp.appPath + "Traceability/ShowDefectsModal",
            scriptUrl: abp.appPath + "view-resources/Views/Traceability/NgParts/_ShowDefectsModal.js",
            modalClass: "ShowDefectsModal"
        });

        var collectionDefectsModal = new app.ModalManager({
            viewUrl: abp.appPath + "Traceability/CollectionDefectsModal",
            scriptUrl: abp.appPath + "view-resources/Views/Traceability/NgParts/_CollectionDefectsModal.js",
            modalClass: "CollectionDefectsModal"
        });

        var ngPartsResult = {
            $table: $("#NgPartsTable"),
            datatables: null,
            init: function () {
               
                if (this.datatables) {
                    ngPartsResult.datatables.destroy();
                    ngPartsResult.$table.empty();
                }
                this.datatables = this.$table.WIMIDataTable({
                    "ajax": {
                        url: abp.appAPIPath + "trace/listNgPartsRecord",
                        data: function (d) {
                            
                            $.extend(d, ngPartsResult.getParams());
                        }
                    },
                    serverSide: true,
                    retrieve: true,
                    responsive: false,
                    ordering: false,
                    order: [],
                    scrollCollapse: true,
                    scrollX: true,
                    columns: ngPartsResult.getColumns()
                });
            },

            getParams: function () {
                    return {
                        startTime: datePickerStart.val(),
                        endTime: datePickerEnd.val(),
                        partNo: $("#PartNo").val(),
                        DeviceGroupId: $("#DeviceGroup").val() === null ? 0 : $("#DeviceGroup").val(),
                        MachineId: $("#Machine").val() === null ? [] : $("#Machine").val(),
                        ShiftSolutionItemId: $("#Shift").val(),
                        StationCode: $("#StationCode").val()
                    };
                              
            },
            getColumns: function() {
                return [
                    {
                        "defaultContent": "",
                        "title": app.localize("NGCauseSupplementOfWorkpiece"),
                        "orderable": false,
                        "width":100,
                        "className": "action",
                        "createdCell": function(td, cellData, rowData, row, col) {
                            $(
                                '<button class="btn btn-default btn-xs"><i class="fa fa-pencil-square-o"></i></button>')
                                .appendTo($(td))
                                .click(function() {
                                    collectionDefectsModal.open({
                                        PartNo: rowData.partNo,
                                        DefectiveMachineId: rowData.machineId
                                    });
                                });

                        }
                    },
                    {
                        "data": "deviceGroupName",
                        "title": app.localize("DeviceGroup")
                    },
                    {
                        "data": "partNo",
                        "title": app.localize("WorkpieceQRCode")
                    },
                    {
                        "data": "onlineTime",
                        "title": app.localize("OnlineTime"),
                        "render": function(data) {
                            return wimi.btl.dateTimeFormat(data);
                        }
                    },
                    {
                        "data": "offlineTime",
                        "title": app.localize("OfflineTime"),
                        "render": function(data) {
                            return wimi.btl.dateTimeFormat(data);
                        }
                    },
                    {
                        "data": "machineName",
                        "title": app.localize("MachineName")
                    },
                    {
                        "data": "stationName",
                        "title": app.localize("StationName")
                    },
                    {
                        "data": "shiftName",
                        "title": app.localize("Shift"),
                        "render": function(data) {
                            if (data !== null) {
                                return data;
                            } else {
                                return "";
                            }
                        }
                    },
                    {
                        "data": "state",
                        "title": app.localize("State")
                    },
                    {
                        "defaultContent": "",
                        "title": app.localize("ProcessView"),
                        "orderable": false,
                        "className": "action",
                        "createdCell": function(td, cellData, rowData, row, col) {

                            $('<a href="javascript:void(0);" >' + app.localize("View") + '</a>')
                                .appendTo($(td))
                                .click(function() {
                                    window.open("/Traceability/Index?ngPartCatlogId=" + rowData.id);
                                });
                        }
                    },
                    {
                        "defaultContent": "",
                        "title": app.localize("WorkpieceNGCauseView"),
                        "orderable": false,
                        "className": "action",
                        "createdCell": function(td, cellData, rowData, row, col) {

                            $('<a href="javascript:void(0);" >' + app.localize("View") + '</a>')
                                .appendTo($(td))
                                .click(function() {
                                    showDefectsModal.open({
                                        PartNo: rowData.partNo,
                                        DefectiveMachineId: rowData.machineId
                                    });
                                });
                        }
                    }
                ];
            },
            reload: function() {
                ngPartsResult.datatables.ajax.reload();
            }
        };

        var page = {
            daterangepickerOption: {
                "singleDatePicker": true,
                "timePicker": true,
                "drops": "down",
                "timePicker24Hour": true,
                "autoApply": true,
                maxDate: moment().add(360, 'years')
            },
            init: function() {
                this.initdatepicker();
                this.initDeviceGroupAndMachine();
                this.initShift();
                this.bindingQueryEvent();
                this.bindingExportEvent();
                ngPartsResult.init();
            },
            initdatepicker: function() {
                datePickerStart.WIMIDaterangepicker(this.daterangepickerOption);
                datePickerEnd.WIMIDaterangepicker($.extend({ "endTimeOnly": true, startDate: moment().add(1, 'days').format('YYYY-MM-DD')}, this.daterangepickerOption));

                datePickerStart.on('cancel.daterangepicker', function(ev, picker) { datePickerStart.val(''); });
                datePickerEnd.on('cancel.daterangepicker', function(ev, picker) { datePickerEnd.val(''); });
            },
            bindingQueryEvent: function() {
                $("#btnQuery").click(function () {                   
                    ngPartsResult.reload();
                });
            },
            initDeviceGroupAndMachine: function() {
                service.listDeviceGroups()
                    .done(function(response) {
                        $("#DeviceGroup").on("change",
                            function() {
                                service.listDeviceGroupMachines({ Id: $("#DeviceGroup").val() })
                                    .done(function(response) {
                                        $("#Machine").empty();
                                        var data = _.map(response,
                                            function(item) {
                                                return { id: item.value, text: item.name };
                                            });

                                        $("#Machine").select2({
                                            data: data,
                                            multiple: true,
                                            minimumResultsForSearch: -1,
                                            placeholder: app.localize("PleaseChoose"),
                                            language: {
                                                noResults: function () {
                                                    return app.localize("NoMatchingData");
                                                }
                                            }
                                        }).val($("#Machine").attr("value")).trigger('change');
                                    });
                            });
                        var data = _.map(response,
                            function(item) {
                                return { id: item.value, text: item.name };
                            });

                        $("#DeviceGroup").select2({
                            data: data,
                            multiple: false,
                            minimumResultsForSearch: -1,
                            placeholder: app.localize("PleaseChoose"),
                            language: {
                                noResults: function() {
                                    return app.localize("NoMatchingData");
                                }
                            }
                        }).val(0).trigger('change');

                        service.listDeviceGroupMachines({ Id: $("#DeviceGroup").val() })
                            .done(function(response) {
                                $("#Machine").empty();
                                var data = _.map(response,
                                    function(item) {
                                        return { id: item.value, text: item.name };
                                    });

                                $("#Machine").select2({
                                    data: data,
                                    multiple: true,
                                    minimumResultsForSearch: -1,
                                    placeholder: app.localize("PleaseChoose"),
                                    language: {
                                        noResults: function() {
                                            return app.localize("NoMatchingData");
                                        }
                                    }
                                });
                            });

                    });
            },
            initShift: function() {           
                $("#Shift").select2({
                    multiple: false,
                    minimumResultsForSearch: -1,
                    placeholder: app.localize("PleaseChoose"),
                    language: {
                        noResults: function() {
                            return app.localize("NoMatchingData");
                        }
                    }
                });
            },
            bindingExportEvent: function () {
                $("#btnExport").click(function (e) {
                    e.preventDefault();
                    page.export();
                });
            },
            export: function () {
                var params = ngPartsResult.getParams();
                abp.ui.setBusy();
                service.exportNgParts(params).done(function (result) {
                    app.downloadTempFile(result);
                }).always(function () {
                    abp.ui.clearBusy();
                });
            }
        };

        page.init();


        abp.event.on("app.CollectionModalSaved", function () {
           
            ngPartsResult.reload();
        });
    });

})();