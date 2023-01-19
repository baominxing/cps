(function () {
    $(function () {
        var service = abp.services.app.machineReport,
            commonService = abp.services.app.commonLookup;

        var feedbackModal = new app.ModalManager({
            viewUrl: abp.appPath + "MachineReport/FeedbackDefectiveReason",
            scriptUrl: abp.appPath + "view-resources/Views/Orders/MachineReport/_FeedbackDefectiveReason.js",
            modalClass: "FeedbackDefectiveReason"
        });

        var charts = {
            $piechartline: $("#piechartline"),
            piechartObject: null,
            $histogramchartline: $("#histogramchartline"),
            histogramchartObject: null,
            piechartlineOption: {
                tooltip: {
                    trigger: 'item',
                    formatter: "{a} <br/>{b} : {c} ({d}%)"
                },
                toolbox: {
                    show: true,
                    orient: 'vertical',
                    feature: {
                        dataView: { show: false, readOnly: false },
                        saveAsImage: { show: true }
                    }
                },
                legend: {
                    orient: 'horizontal',
                    left: 'center',
                    data: []
                },
                series: [
                    {
                        name: app.localize("DefectiveReasons"),
                        type: 'pie',
                        radius: '60%',
                        center: ['50%', '65%'],
                        data: [],
                        itemStyle: {
                            emphasis: {
                                shadowBlur: 10,
                                shadowOffsetX: 0,
                                shadowColor: 'rgba(0, 0, 0, 0.5)'
                            }
                        }
                    }
                ]
            },
            histogramchartlineOption: {
                tooltip: {
                    trigger: 'axis'
                },
                legend: {
                    data: []
                },
                toolbox: {
                    show: true,
                    orient: 'vertical',
                    feature: {
                        dataView: { show: false, readOnly: false },
                        magicType: { show: true, type: ['line', 'bar'] },
                        restore: { show: true },
                        saveAsImage: { show: true }
                    }
                },
                dataZoom: [
                    {
                        show: true,
                        start: 0,
                        end: 100
                    }
                ],
                calculable: true,
                grid: [{ buttom: "5%", containLabel: true }],
                xAxis: [
                    {
                        type: 'category',
                        data: [],
                        axisLabel: {
                            interval: 0,
                            formatter: function (value) {
                                return value.split("").join("\n");
                            }
                        }    
                    }
                ],
                yAxis: [
                    {
                        type: 'value'
                    }
                ],
                series: []
            },
            init: function (rowData) {
                if (rowData) {
                    var param = {
                        MachineId: rowData.machineId,
                        ShiftSolutionItemId: rowData.shiftSolutionItemId,
                        Date: $("#daterange-btn").val(),
                        ProductId: rowData.productId
                    };

                    service.listMachineDefective(param)
                        .done(function (response) {
                            charts.disposeChart();
                            charts.setChartHeight(response);
                            charts.initPiechart(response);
                            charts.initHistogramchart(response);
                        });
                }
            },
            initPiechart: function (data) {
                var option = this.piechartlineOption;
                option.legend.data = _.pluck(_.pluck(data, 'defectiveReason'), 'name');
                option.series[0].data = _.map(data,
                    function (item) {
                        return {
                            value: item.count,
                            name: item.defectiveReason.name
                        };
                    });

                this.piechartObject = echarts.init(charts.$piechartline.get(0));
                this.piechartObject.setOption(option);
            },
            initHistogramchart: function (data) {
                var option = this.histogramchartlineOption;
                option.xAxis[0].data = _.pluck(_.pluck(data, 'defectiveReason'), 'name');
                option.series = {
                    name: app.localize("NumberOfBad"),
                    type: 'bar',
                    data: _.pluck(data, 'count')
                };

                this.histogramchartObject = echarts.init(charts.$histogramchartline.get(0));
                this.histogramchartObject.setOption($.WIMI.echartOptionBuilder(option));
            },
            disposeChart: function () {
                if (this.histogramchartObject) {
                    echarts.dispose(charts.$histogramchartline.get(0));
                }
                if (this.piechartObject) {
                    echarts.dispose(charts.$piechartline.get(0));
                }
            },
            setChartHeight: function (data) {
                if (data && data.length !== 0) {
                    this.$histogramchartline.css("height", "458px");
                    this.$piechartline.css("height", "458px");
                } else {
                    this.$histogramchartline.css("height", "0");
                    this.$piechartline.css("height", "0");
                }
            }
        };

        var table = {
            datatables: null,
            $table: $("#hourlyYieldAnalysisTable"),
            load: function (callback) {
                if (this.datatables) {
                    table.datatables.destroy();
                    table.$table.empty();
                }

                table.bindingrowClickEvent();
                abp.ui.setBusy();
                service.listHourlyYieldAnalysis(table.getParams()).done(function (response) {
                    table.$table.WIMIDataTable({
                        data: response,
                        serverSide: false,
                        responsive: false,
                        scrollX: true,
                        ordering: false,
                        columns: table.getColumns(),
                        drawCallback: function (settings) {
                            table.datatables = this.api();
                            const row = this.api().row();
                            if (row.length > 0) {
                                $(row.node()).trigger('click');
                            } else {
                                charts.disposeChart();
                                charts.setChartHeight();
                            }
                        },
                        initComplete: function () {
                            if (callback) {
                                callback();
                            }
                        }
                    });
                }).always(function () {
                    abp.ui.clearBusy();
                });
            },
            selectedRow: function (data) {
                var row = table.datatables.row(':eq(' + data + ')');
                if (row.length > 0) {
                    $(row.node()).trigger('click');
                }
            },
            getColumns: function () {
                return [
                    {
                        "data": "machineName",
                        "title": app.localize("MachineName")
                    },
                    {
                        "data": "productName",
                        "title": app.localize("ProductName")
                    },
                    {
                        "data": "shiftName",
                        "title": app.localize("Shift")
                    },
                    {
                        "data": "yield",
                        "title": app.localize("Yield")
                    },
                    {
                        "data": "qualifiedCount",
                        "title": app.localize("GoodNumber")
                    },
                    {
                        "data": "unqualifiedCount",
                        "title": app.localize("DefectiveNumber")
                    },
                    {
                        "data": "badRate",
                        "title": app.localize("DefectiveRate"),
                        "render": function (data) {
                            return (data * 100.0).toFixed(2) + " %";
                        }
                    },
                    {
                        defaultContent: "",
                        title: app.localize("BadFeedback"),
                        width: "60px",
                        createdCell: function (td, cellData, rowData, row, col) {
                            const $td = $(td);
                            //if (moment().isAfter(rowData.creationTime, 'day')) {
                            //    $('<small class="text-muted">当天有效</small>').appendTo($td);
                            //    return false;
                            //}
                            $('<button class="btn btn-default btn-xs" >反馈</button> ')
                                .appendTo($td)
                                .click(function () {
                                    feedbackModal.open({ row: rowData },
                                        function () {
                                            charts.init(rowData);  
                                            table.load(function () {
                                                table.selectedRow(row);
                                            });
                                        });
                                });
                        }
                    }
                ];
            },
            getParams: function () {
                return {
                    Date: $("#daterange-btn").val(),
                    MacineId: $("#machineId").val()
                }
            },
            bindingrowClickEvent: function (rowData) {
                table.$table.on('click',
                    'tr',
                    function () {
                        if ($(this).hasClass("selected")) {
                            return;
                        } else {
                            table.datatables.$("tr.selected").removeClass("selected").css("background-color", "");
                            $(this).addClass("selected").css("background-color", "#a8e4ff");
                            var row = table.datatables.row($(this)).data();
                            charts.init(row);
                        }
                    });
            }
        };

        var page = {
            $machineSelect: $("#machineId"),
            $daterange: $("#daterange-btn"),
            init: function () {
                this.initMachines();
                this.initDateTimePicker();

                table.load();
                this.bindingQueryEvent();
                this.bindingKeypressEvent();
            },
            initMachines: function () {
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

                    var machinesData = [
                        {
                            id: "0",
                            text: app.localize("All")
                        }
                    ];
                    //_.each(_.groupBy(machines._wrapped, "id"),
                    //    function (item) {
                    //        machinesData.push(item[0]);
                    //    });
                    for (var n = 0; n < machines._wrapped.length; n++) {
                        machinesData.push(machines._wrapped[n]);
                    }
                    page.$machineSelect.select2({
                        data: machinesData,
                        multiple: false,
                        placeholder: app.localize("PleaseSelect"),
                        minimumResultsForSearch: -1,
                        language: {
                            noResults: function () {
                                return app.localize("PleaseMaintainTheEquipment");
                            }
                        }
                    }).val("null").trigger('change');
                });
            },
            initDateTimePicker: function () {
                service.getShiftDayTimeRange(table.getParams(), { async: false }).done(function (response) {
                    var startTime = moment();
                    if (response) {
                        startTime = moment(response.shiftDay);
                        if (moment().isBefore(response.startTime)) {
                            startTime = startTime.subtract(1, 'days').format();
                        }
                    }

                    page.$daterange.WIMIDaterangepicker({
                        singleDatePicker: true,
                        startDate: startTime,
                        endDate: startTime
                    });
                });
            },
            bindingQueryEvent: function () {
                $("#btnQuery").click(function () {
                    table.load();
                    charts.disposeChart();
                });
            },
            bindingKeypressEvent: function () {
                $(".content").find("input").keypress(function (e) {
                    if (e.which === 13) {
                        table.load();
                        charts.disposeChart();
                    }
                });
            }
        };


        page.init();
    });
})(jQuery);