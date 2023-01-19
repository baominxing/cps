(function () {
    $(function () {

        var service = abp.services.app.reasonFeedbackAnalysis;

        var detailModal = new app.ModalManager({
            viewUrl: abp.appPath + "ReasonFeedbackAnalysis/DetailModal",
            scriptUrl: abp.appPath + "view-resources/Views/Reasons/ReasonFeedbackAnalysis/_DetailModal.js",
            modalClass: "DetailModal"
        });

        var chart = {
            timeChartObject: null,
            timeChartDom: document.getElementById("analysisChartByTime"),
            durationChartObject: null,
            durationChartDom: document.getElementById("analysisChartByDuration"),

            timeOption : {
                tooltip: {
                    position: 'top'
                },
                animation: false,
                grid: {
                    height: '50%',
                    y: '10%',
                    top: 0,
                    right:0
                },
                xAxis: {
                    type: 'category',
                    data: [],
                    splitArea: {
                        show: true
                    }
                },
                yAxis: {
                    type: 'category',
                    data:  [],
                    splitArea: {
                        show: true
                    }
                },
                dataZoom:[{
                    show: true,
                    start: 0,
                    end: 100,
                    bottom: '40%',
                    height: '40',
                    type: 'slider',
               
                },
                    {
                    type: 'inside',
                    yAxisIndex: [0],
                    start: 0,
                    end: 100,
                   // zoomOnMouseWheel: 'shift'  //按住shift触发缩放
                   }],
                visualMap: {
                    min: 0,
                    max: 5,
                    calculable: true,
                    orient: 'horizontal',
                    left: 'center',
                    bottom: '35%'
                },
                series: {
                    name: app.localize("NumberOfFeedback"),
                    type: 'heatmap',
                    data: [],
                    label: {
                        normal: {
                            show: true
                        }
                    },
                    itemStyle: {
                        emphasis: {
                            shadowBlur: 10,
                            shadowColor: 'rgba(0, 0, 0, 0.5)'
                        }
                    }
                }
            },

            durationOption: {
                tooltip: {
                    position: 'top'
                },
                animation: false,
                grid: {
                    height: '50%',
                    y: '10%',
                    top: 0,
                    right: 0
                },
                xAxis: {
                    type: 'category',
                    data: [],
                    splitArea: {
                        show: true
                    }
                },
                yAxis: {
                    type: 'category',
                    data: [],
                    splitArea: {
                        show: true
                    }
                },
                dataZoom: {
                    show: true,
                    start: 0,
                    end: 100,
                    bottom: '40%',
                    height: '40'
                },
                visualMap: {
                    min: 0,
                    max: 2000,
                    calculable: true,
                    orient: 'horizontal',
                    left: 'center',
                    bottom: '35%'
                },
                series: {
                    name: app.localize("FeedbackTime"),
                    type: 'heatmap',
                    data: [],
                    label: {
                        normal: {
                            show: true
                        }
                    },
                    itemStyle: {
                        emphasis: {
                            shadowBlur: 10,
                            shadowColor: 'rgba(0, 0, 0, 0.5)'
                        }
                    }
                }
            },

            init: function (data) {
                this.disposeChart();
                var timeChartOption = this.timeOption;
                var durationChartOption = this.durationOption;

                timeChartOption.xAxis.data = data.summaryDateList;
                timeChartOption.yAxis.data = data.machineNameList;
                timeChartOption.series.data = data.chartDataByTime;

                durationChartOption.xAxis.data = data.summaryDateList;
                durationChartOption.yAxis.data = data.machineNameList;
                durationChartOption.series.data = data.chartDataByDuration;
              
                this.timeChartObject = echarts.init(chart.timeChartDom);
                this.timeChartObject.setOption($.WIMI.echartOptionBuilder(timeChartOption));
                this.durationChartObject = echarts.init(chart.durationChartDom);
                this.durationChartObject.setOption($.WIMI.echartOptionBuilder(durationChartOption));
            },

            disposeChart: function () {
                if (this.chartObject) {
                    echarts.dispose(chart.timeChartDom);
                    echarts.dispose(chart.durationChartDom);
                }
            }
        }
        
        var table = {
            $timetable: $("#analysisTableBYTime"),
            timedatatables: null,
            $durationtable: $("#analysisTableBYDuration"),
            durationdatatables: null,

            init: function (data) {
                if (this.timedatatables) {
                    table.timedatatables.destroy();
                    table.$timetable.empty();
                }

                if (this.durationdatatables) {
                    table.durationdatatables.destroy();
                    table.$durationtable.empty();
                }

                var value = data.data;

                this.timedatatables = table.$timetable.WIMIDataTable({
                    serverSide: false,
                    responsive: false,
                    data: value,
                    columns: [
                        {
                            data: "summaryDate",
                            title: app.localize("Date"),
                            render: function (data) { return wimi.btl.dateFormat(data); }
                        },
                        {
                            orderable: false,
                            data: "feedBackReason",
                            title: app.localize("Reason")
                        },
                        {
                            orderable: false,
                            data: "times",
                            title: app.localize("Number")
                        },
                        {
                            "defaultContent": "",
                            "title": app.localize("Actions"),
                            "orderable": false,
                            "width": "30px",
                            "className": "action",
                            "createdCell": function (td, cellData, rowData, row, col) {
                                $(td)
                                    .buildActionButtons([
                                        {
                                            title: app.localize("CheckDetails"),
                                            clickEvent: function () {
                                                var deviceGroupId = page.$deviceGroupSelect.val();
                                                detailModal.open({ SummaryDate: rowData.summaryDate, StateCode: rowData.stateCode, DeviceGroupId: deviceGroupId });
                                            },
                                            isShow: true
                                        }
                                        
                                    ]);
                            }
                        }
                    ]
                });

                this.durationdatatables = table.$durationtable.WIMIDataTable({
                    serverSide: false,
                    responsive:false,
                    data: value,
                    columns: [
                        {
                            data: "summaryDate",
                            title: app.localize("Date"),
                            render: function (data) { return wimi.btl.dateFormat(data); }
                        },
                        {
                            orderable: false,
                            data: "feedBackReason",
                            title: app.localize("Reason")
                        },
                        {
                            orderable: false,
                            data: "duration",
                            title: app.localize("Durations") + "("+app.localize("Minutes")+")"
                        },
                        {
                            "defaultContent": "",
                            "title": app.localize("Actions"),
                            "orderable": false,
                            "width": "30px",
                            "className": "action",
                            "createdCell": function (td, cellData, rowData, row, col) {
                                $(td)
                                    .buildActionButtons([
                                        {
                                            title: app.localize("CheckDetails"),
                                            clickEvent: function () {
                                                var deviceGroupId = page.$deviceGroupSelect.val();
                                                detailModal.open({ SummaryDate: rowData.summaryDate, StateCode: rowData.stateCode, DeviceGroupId: deviceGroupId });
                                            },
                                            isShow: true
                                        }

                                    ]);
                            }
                        }
                    ]
                });
            }
        }

        var page = {
            $datepicker: $("#daterange-btn"),
            $deviceGroupSelect: $("#deviceGroup"),
            init: function () {
                this.initDatepicker();
                this.initDeviceGroups();
                this.bindingQueryEvent();
            },
            initDatepicker: function () {
                this.$datepicker.WIMIDaterangepicker({
                    startDate: moment().subtract(6, "days"),
                    endDate: moment()
                });
            },
            initDeviceGroups: function () {
                abp.services.app.commonLookup.getDeviceGroupAndMachineWithPermissions().done(function (response) {
                    var deviceGroups = _.chain(response.deviceGroups)
                        .filter(
                            function (item) {
                                return _.contains(response.grantedGroupIds, item.id);
                            })
                        .map(function (item) {
                            var deviceGroup = {
                                id: item.id,
                                text: item.displayName,
                                seq: item.seq
                            };
                            return deviceGroup;
                        });

                    var deviceGroupsData = [
                    ];
                    _.each(_.groupBy(deviceGroups._wrapped, "id"),
                        function (item) {
                            deviceGroupsData.push(item[0]);
                        });

                    var defaultValue = deviceGroupsData.length > 1 ? _.first(deviceGroupsData) : _.last(deviceGroupsData);
                    page.$deviceGroupSelect.select2({
                        data: _.sortBy(deviceGroupsData, "seq"),
                        multiple: false,
                        serach: false,
                        placeholder: app.localize("PleaseSelect"),
                        minimumResultsForSearch: -1,
                        language: {
                            noResults: function () {
                                return app.localize("PleaseMaintainTheEquipmentGroup");
                            }
                        }
                    }).val(defaultValue.id).trigger('change');

                    page.load();
                });
            },
            load: function () {

                var param = this.getParamter();

                abp.ui.setBusy();
                service.getReasonFeedBackResult(param).done(function (response) {
                      
                    var machine = _.unique( _.pluck(response.chartData, 'machineName'));
                    var summaryDate = _.unique(_.pluck(response.chartData, 'summaryDate'));
                    var summary = [];
                    _.each(summaryDate,
                        function (item) {
                            var d = wimi.btl.dateFormat(item); 
                            summary.push(d);
                        });

                   var dataByTime = response.chartData.map(function (item) {
                       return [item.horizontalValue, item.verticalValue, item.times || '-'];
                    });

                    var dataByDuration = response.chartData.map(function (item) {
                        return [item.horizontalValue, item.verticalValue, item.duration || '-'];
                    });

                    var chartResult = {
                        machineNameList: machine,
                        summaryDateList: summary,
                        chartDataByTime: dataByTime,
                        chartDataByDuration:dataByDuration
                    };

                    var tableResult = {
                        data: response.tableData
                    };

                    table.init(tableResult);
                    chart.init(chartResult);
                    abp.ui.clearBusy();
                }).fail(function () {
                    abp.ui.clearBusy();
                });
            },
            getParamter: function () {
                var startTime = page.$datepicker.data("daterangepicker").startDate.format("YYYY-MM-DD");
                var endTime = page.$datepicker.data("daterangepicker").endDate.format("YYYY-MM-DD");
                var deviceGroupId = page.$deviceGroupSelect.val();
                return {
                    StartTime: startTime,
                    EndTime: endTime,
                    DeviceGroupId: deviceGroupId
                };
            },
            bindingQueryEvent: function () {
                $("#btnQuery").click(function () {
                    page.load();
                });
            },
            clearDOM: function () {
                $("#analysisChartByTime").empty();
                $("#analysisTableBYTime").empty();
                $("#analysisChartByDuration").empty();
                $("#analysisTableBYDuration").empty();
            }
        }

        page.init();
    });
})(jQuery)