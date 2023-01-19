(function () {
    $(function () {

        var $startDatepicker = $("#daterange-startDate");
        var $endDatepicker = $("#daterange-endDate");

        var showProcessParamModal = new app.ModalManager({
            viewUrl: abp.appPath + "Traceability/ProcessParameters",
            scriptUrl: abp.appPath + "view-resources/Views/Traceability/Query/_ProcessParameters.js",
            modalClass: "processParametersModal",
            modalSize: "modal-lg"
        });

        var showExtensionDataModal = new app.ModalManager({
            viewUrl: abp.appPath + "Traceability/ExtensionData",
            scriptUrl: abp.appPath + "view-resources/Views/Traceability/Query/_ExtensionData.js",
            modalClass: "extensionDataModal"
        });

        var service = abp.services.app.trace;

        //var queryDefaultStartDate = moment().format('YYYY-MM-DD');
        //var queryDefaultEndDate = moment().format('YYYY-MM-DD');

        var daterangepickerOption = {
            "singleDatePicker": false,
            "timePicker": true,
            "drops": "down",
            "timePicker24Hour": true,
            "autoApply": true,
            maxDate: moment().add(360, 'years')
        };


        var vm = new Vue({
            el: '#traceCatalogQuery',
            computed: {
            },
            created: function () {
                var _this = this;

                _this.getGroup();
                //_this.startTime = queryDefaultStartDate;
                //_this.endTime = queryDefaultEndDate;
            },
            mounted: function () {
                let _this = this;
                _this.initdaterangepicker();
                _this.getTraceList();
                _this.getShift();


                //绑定表格每行的click事件
                $("#traceTable").on("click",
                    "tr",
                    function () {
                        let $currentRow = $(this);
                        let tr = _this.traceTable.row($currentRow).node();
                        $currentRow = $(tr);
                        if ($currentRow.hasClass("selected")) {
                            return;
                        } else {
                            _this.traceTable.$("tr.selected").removeClass("selected");
                            $currentRow.addClass("selected");
                            let currentRow = _this.traceTable.row($currentRow).data();
                            _this.currId = currentRow.partNo;
                            _this.getTraceRecord(currentRow.partNo, currentRow.archivedTable);
                        }
                    });
            },
            methods: {
                getGroup() {
                    service.listDeviceGroups()
                        .done(function (response) {
                            $("#DeviceGroup").on("change",
                                function () {
                                    service.listDeviceGroupMachines({ Id: $("#DeviceGroup").val() * 1 })
                                        .done(function (response) {
                                            $("#Machine").empty();
                                            var data = _.map(response,
                                                function (item) {
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
                                function (item) {
                                    return { id: item.value, text: item.name };
                                });

                            $("#DeviceGroup").select2({
                                data: data,
                                multiple: false,
                                minimumResultsForSearch: -1,
                                //placeholder: "请选择",
                                language: {
                                    noResults: function () {
                                        return app.localize("NoMatchingData");
                                    }
                                }
                            }).val(0).trigger('change');

                            service.listDeviceGroupMachines({ Id: $("#DeviceGroup").val() })
                                .done(function (response) {
                                    $("#Machine").empty();
                                    var data = _.map(response,
                                        function (item) {
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
                                    });
                                });

                        });
                },
                getShift() {
                    $("#Shift").select2({
                        multiple: false,
                        minimumResultsForSearch: -1,
                        placeholder: app.localize("PleaseChoose"),
                        language: {
                            noResults: function () {
                                return app.localize("NoMatchingData");
                            }
                        }
                    });
                },
                getTraceList() {
                    let _this = this;
                    _this.traceTable = $('#traceTable').WIMIDataTable({
                        "scrollX": true,
                        "responsive": false,
                        "initComplete": function () {
                            let trObj = _this.traceTable.row(0).node();
                            if (trObj) {
                                let trData = _this.traceTable.row(0).data();
                                $(trObj).addClass('selected');
                                _this.currId = trData.partNo;
                                _this.getTraceRecord(trData.partNo, trData.archivedTable);
                            }
                        },
                        "fnCreatedRow": function () {
                            let trObj = _this.traceTable.row(0).node();
                            if (trObj) {
                                let trData = _this.traceTable.row(0).data();
                                $(trObj).addClass('selected');
                                _this.currId = trData.partNo;
                                //_this.getTraceRecord(trData.partNo, trData.archivedTable);
                            }
                        },
                        "stateSave": false,
                        order: [],
                        "ajax": {
                            "url": abp.appAPIPath + "trace/ListTraceCatalog",
                            "type": "POST",
                            "data": function (d) {
                                console.log($("#daterange-startDate").data("daterangepicker"));
                                let queryParameters = {
                                    "DeviceGroupId": $("#DeviceGroup").val(),
                                    "PartNo": $("#PartNo").val(),
                                    "StartFirstTime": $("#daterange-startDate").data("daterangepicker").startDate.format("YYYY-MM-DD HH:mm:ss"),
                                    "StartLastTime": $("#daterange-startDate").data("daterangepicker").endDate.format("YYYY-MM-DD HH:mm:ss"),
                                    "EndFirstTime": $("#daterange-endDate").data("daterangepicker").startDate.format("YYYY-MM-DD HH:mm:ss"),
                                    "EndLastTime": $("#daterange-endDate").data("daterangepicker").endDate.format("YYYY-MM-DD HH:mm:ss"),
                                    "StartDate": $("#daterange-startDate").val(),
                                    "EndDate": $("#daterange-endDate").val(),
                                    "MachineId": $("#Machine").val() ? $("#Machine").val() : [],
                                    "ShiftSolutionItemId": $("#Shift").val(),
                                    "StationCode": $("#StationCode").val(),
                                    "ngPartCatlogId": ngPartCatlogId ? ngPartCatlogId * 1 : 0,
                                    //order: []
                                };
                                $.extend(d, queryParameters);
                            }
                        },
                        "columns": [
                            {
                                "data": 'partNo',
                                "title": app.localize("WorkpieceNumber")
                            },
                            {
                                "data": "isOffline",
                                "orderable": false,
                                "title": app.localize("State"),

                                "render": function (data) {
                                    if (data) {
                                        return app.localize("HasBeenOffline");
                                    } else {
                                        return app.localize("NotOffline");
                                    }
                                }
                            },
                            {
                                "data": "qualified",
                                "orderable": false,

                                "title": app.localize("IsQualified"),
                                "render": function (data) {
                                    if (data === null) {
                                        return app.localize("Unknown");
                                    }

                                    if (data) {
                                        return app.localize("Qualified");
                                    } else {
                                        return app.localize("Unqualified");
                                    }
                                }
                            }
                        ]
                    });
                },
                initdaterangepicker: function () {
                    $("#daterange-startDate").WIMIDaterangepicker(daterangepickerOption);
                    $("#daterange-endDate").WIMIDaterangepicker(daterangepickerOption);

                    $("#daterange-startDate").on('cancel.daterangepicker',
                        function (ev, picker) {
                            $("#daterange-startDate").val('');
                        });
                    $("#daterange-endDate").on('cancel.daterangepicker', function (ev, picker) {
                        $("#daterange-endDate").val('');
                    });
                    $("#daterange-endDate").val("");
                },
                searchFn() {
                    let _this = this;
                    ngPartCatlogId = 0;
                    _this.getTraceRecord(0, "");
                    _this.traceTable.ajax.reload(_this.getFirst);
                },
                exportFn() {
                    var _this = this;
                    parameters = {
                        "DeviceGroupId": $("#DeviceGroup").val(),
                        "PartNo": $("#PartNo").val(),
                        "StartFirstTime": $("#daterange-startDate").data("daterangepicker").startDate.format("YYYY-MM-DD HH:mm:ss"),
                        "StartLastTime": $("#daterange-startDate").data("daterangepicker").endDate.format("YYYY-MM-DD HH:mm:ss"),

                        "EndFirstTime": $("#daterange-endDate").data("daterangepicker").startDate.format("YYYY-MM-DD HH:mm:ss"),
                        "EndLastTime": $("#daterange-endDate").data("daterangepicker").endDate.format("YYYY-MM-DD HH:mm:ss"),

                        "StartDate": $("#daterange-startDate").val(),
                        "EndDate": $("#daterange-endDate").val(),
                        "MachineId": $("#Machine").val() ? $("#Machine").val() : [],
                        "ShiftSolutionItemId": $("#Shift").val(),
                        "StationCode": $("#StationCode").val(),
                        "ngPartCatlogId": ngPartCatlogId ? ngPartCatlogId * 1 : 0,
                        order: []
                    };

                    if (parameters.StartDate === "") {
                        abp.message.confirm(app.localize("QueryDataIsTooLarge"),
                            function (isConfirmed) {
                                if (isConfirmed) {
                                    _this.export();
                                }
                            });
                    } else {
                        _this.export();
                    }
                },
                export() {
                    abp.ui.setBusy();
                    service.export(parameters).done(function (result) {
                        app.downloadTempFile(result);
                    }).always(function () {
                        abp.ui.clearBusy();
                    });
                },
                getFirst() {
                    let _this = this;
                    let trObj = _this.traceTable.row(0).node();
                    if (trObj) {
                        let trData = _this.traceTable.row(0).data();
                        _this.traceTable.$("tr.selected").removeClass("selected");
                        $(trObj).addClass('selected');
                        _this.currId = trData.partNo;
                        _this.getTraceRecord(trData.partNo, trData.archivedTable);
                    }
                },
                getTraceRecord(id, archivedTable) {
                    let _this = this;
                    let pid = id || _this.currId;
                    if (id === 0) {
                        pid = '';
                    }

                    service.listTraceRecordByPartNo({
                        PartNo: pid,
                        StartFirstTime: $("#daterange-startDate").data("daterangepicker").startDate.format("YYYY-MM-DD HH:mm:ss"),
                        StartLastTime: $("#daterange-startDate").data("daterangepicker").endDate.format("YYYY-MM-DD HH:mm:ss"),
                        EndFirstTime: $("#daterange-endDate").data("daterangepicker").startDate.format("YYYY-MM-DD HH:mm:ss"),
                        EndLastTime: $("#daterange-endDate").data("daterangepicker").endDate.format("YYYY-MM-DD HH:mm:ss"),
                        ArchivedTable: archivedTable
                    }).then(function (res) {
                        _this.partDetails = res.partDetails;
                        _this.traceRecords = res.traceRecords;

                    });

                },
                openModal(row) {

                    //_showProcessParamModal.open({});
                    if (row.flowDataSource === "mongo") {

                        showProcessParamModal.open({
                            partNo: row.partNo,
                            machineId: row.machineId,
                            machineName: row.machineName,
                            operationTimeBegin: row.entryTime,
                            operationTimeEnd: row.leftTime,
                            stepNo: row.stepNo
                        });
                    }

                    if (row.flowDataSource === "extensionData") {
                        showExtensionDataModal.open({
                            partNo: row.partNo,
                            machineName: row.machineName,
                            recordId: row.id
                        });
                    }
                }
            },
            filters: {
                formatData: function (time) {

                    if (time === null) {
                        return;
                    }

                    return moment(time).format("YYYY-MM-DD HH:mm:ss");
                },
                escapeFlowState: function (flowStateEnumValue) {

                    switch (flowStateEnumValue) {
                        case 0:
                            return app.localize("Plan-InProgress");
                        case 1:
                            return app.localize("Plan-Complete");
                        default:
                            return app.localize("Unknown");
                    }
                },
                escapeFlowTag: function (flowTagEnumValue) {

                    switch (flowTagEnumValue) {
                        case 0:
                            return "";
                        case 1:
                            return app.localize("Qualified");
                        case 2:
                            return app.localize("Unqualified");
                        default:
                            return app.localize("Unknown");
                    }
                }
            },
            data: {
                traceTable: null,
                groupId: '',
                partNo: '', //工件编码
                //startTime: '', //开始时间
                //endTime: '', //结束时间
                traceList: [],
                currId: '',
                "partDetails": {
                    "partNo": "",
                    "tags": [
                        ""
                    ],
                    "shiftName": "",
                    "onlineTime": null,
                    "offlineTime": null,
                    "qulified": true,
                    "isReworkPart": true
                },
                "traceRecords": [
                    {
                        "partNo": "",
                        "flowCode": "",
                        "flowDisplayName": "",
                        "machineName": "",
                        "machineCode": "",
                        "station": "",
                        "entryTime": null,
                        "leftTime": null,
                        "flowState": ""
                    }
                ]

            }
        });
    });
})(jQuery);
