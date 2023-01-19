(function () {
    $(function () {

        var yieldStatisticsSevice = abp.services.app.yield;
        var chartDom = document.getElementById("chart-bar");
        var percentOfChartDataZoom = 100;

        var showQueryModal = new app.ModalManager({
            viewUrl: abp.appPath + "YieldStatistics/ShowQueryModal",
            scriptUrl: abp.appPath + "view-resources/Views/StatisticAnalysis/YieldStatistics/_ShowQueryModal.js",
            modalClass: "ShowQueryModal"
        });

        var page = {
            $tabDate: $("#tabDate"),
            $datepicker: $("#daterange-btn"),
            $datatable: $("#timeStatisticsTable"),
            datatableObject: null,
            $showName: $("#showName"),
            $showValue: $("#showValue"),
            $query: $("#btnQuery"),
            params: null,
            chartByDateOption: {
                legend: {
                    type: 'scroll',
                    left: '50%',
                    right: '8%',
                    top: '0%'
                },
                tooltip: {
                    trigger: "item",
                    formatter: function (params) {
                        var machineName = params.seriesName.split(",")[0];
                        if (params.value) {
                            var capacity = params.value;
                            var res = app.localize("MachineName") + ":" + machineName + "<br/>";
                            res += page.params.statisticalName + app.localize("Yield") + ":" + capacity + "<br/>";
                        } else {
                            var res = app.localize("MachineName") + ":" + machineName + "<br/>";
                        }
                        return res;
                    }
                },
                grid: {
                    left: '3%',
                    right: '3%',
                    top: '10%',
                    containLabel: true
                },
                dataZoom: [
                    {
                        show: true,
                        xAxisIndex: 0,
                        height: '5%',
                        top: '95%',
                        showDataShadow: false,
                        start: 0,
                        end: percentOfChartDataZoom 
                    }
                ],
                yAxis: [
                    {
                        name: abp.localization.localize('Yield'),
                        type: "value",
                        axisLabel: {
                            show: true,
                            interval: "auto",
                            formatter: "{value}"
                        },
                        minInterval: 1
                    }
                ],
                xAxis: [
                    {
                        type: "category",
                        boundaryGap: true,
                        axisLabel: {
                            interval: 0
                        },
                        data: []
                    }
                ],
                series: []
            },
            init: function () {

                this.initdaterangepicker();
                this.initParamter();
                this.bindingQueryEvent();
                this.bindingShowNameEvent();
                this.bindingShowValueEvent();
                this.bindingExportEvent();

                this.loadTableAndChart();
            },
            initdaterangepicker: function () {
                this.$datepicker.WIMIDaterangepicker({
                    startDate: moment().subtract(6, "days"),
                    endDate: moment()
                });
            },
            initParamter: function () {
                this.params = {
                    startTime: moment().subtract(6, "days").format('YYYY-MM-DD'),
                    endTime: moment().format('YYYY-MM-DD'),
                    statisticalName: app.localize("ByDay") + ":",
                    statisticalWay: "ByDay",
                    MachineIdList: [],
                    ShiftSolutionIdList: []

                };
            },
            scrollTabRender: function () {
                var solutionNameTabRender = Handlebars.compile($("#tab-date-template").html());

                if (page.params.statisticalWay === "ByShift") {
                    $("#tabDate").removeClass('hidden');

                    const tabData = [];
                    _.each(page.params.machineShiftSolutionNameList,
                        function (key, index) {
                            tabData.push({
                                id: page.params.machineShiftSolutionIdList[index],
                                name: page.params.machineShiftSolutionNameList[index]
                            });
                        });

                    this.$tabDate.html(solutionNameTabRender({ dates: tabData }));
                    this.$tabDate.scrollTabs({
                        click_callback: function () {
                            // 重新绘图
                            // 请求数据
                            var selectedId = $(this).data("id");

                            page.params.ShiftSolutionIdList = [selectedId];
                            page.loadTableAndChart();
                        }
                    });
                } else {
                    $("#tabDate").addClass('hidden');
                }
            },
            loadTableAndChart: function () {

                abp.ui.setBusy();
                yieldStatisticsSevice.getMachineCapability(page.params).done(function (result) {
                    page.drawTableAndChart(result);
                }).always(function () {
                    abp.ui.clearBusy();
                });
            },
            drawTableAndChart: function (result) {

                page.drawChart(result);
                page.drawTable(result.tableDataList);
            },
            drawChart: function (result) {
                var isShowName = this.$showName.is(":checked");
                var isShowValue = this.$showValue.is(":checked");

                var legendsData = [];
                var xAxisData = [];
                var seriesData = [];

                var chartByDateOption = page.chartByDateOption;
                chartByDateOption.legend.data = legendsData;
                chartByDateOption.series = seriesData;
                chartByDateOption.xAxis[0].data = xAxisData;

                var chartInstance = echarts.getInstanceByDom(chartDom);
                if (chartInstance) {
                    echarts.dispose(chartDom);
                }
                chartInstance = echarts.init(chartDom);

                for (var i = 0; i < result.tableDataList.length; i++) {

                    var xData = result.tableDataList[i].summaryDate;
                    if (jQuery.inArray(xData, xAxisData) === -1) {
                        xAxisData.push(xData);
                    }
                }

                for (var i = 0; i < result.chartDataList.length; i++) {
                    legendsData.push(result.chartDataList[i].machineName);
                    seriesData.push(
                        {
                            name: result.chartDataList[i].machineName,
                            type: 'bar',
                            barGap: '20%',
                            barCategoryGap: '10%',
                            stack: result.chartDataList[i].machineName,
                            itemStyle: {
                                normal: {
                                    label: {
                                        textStyle: {
                                            color: 'black'
                                        },
                                        show: true,
                                        position: 'top',
                                        align: 'left',
                                        verticalAlign: 'middle',
                                        fontSize: 12,
                                        rotate: 90,
                                        formatter: function (item) {
                                            if (isShowName && isShowValue) {
                                                return item.seriesName.split(',')[0] + "\r\n" + item.data;
                                            } else if (isShowName) {
                                                return item.seriesName.split(',')[0];
                                            } else if (isShowValue) {
                                                return item.data;
                                            } else {
                                                return "";
                                            }
                                        }
                                    }
                                }
                            },
                            data: result.chartDataList[i].yields
                        });
                }

                chartByDateOption.dataZoom[0].end = 100;

                chartInstance.setOption($.WIMI.echartOptionBuilder(chartByDateOption));
            },
            drawTable: function (tableDataList) {
                if (page.datatableObject) {
                    page.datatableObject.destroy();
                    page.$datatable.empty();
                }

                page.datatableObject = page.$datatable.WIMIDataTable({
                    serverSide: false,
                    data: tableDataList,
                    columns: [
                        {
                            title: app.localize("StatisticalMethod"),
                            data: "summaryDate"
                        },
                        {
                            title: app.localize("MachineName"),
                            data: "machineName"
                        },
                        {
                            title: app.localize("Yield"),
                            data: "yield"
                        }
                    ]
                });
            },
            bindingQueryEvent: function () {
                $("#btnQuery").on("click",
                    function (e) {
                        e.preventDefault();
                        showQueryModal.open({},
                            function (param) {
                                page.params = param;
                                if (param.machineShiftSolutionIdList.length > 0) {
                                    page.params.ShiftSolutionIdList = [param.machineShiftSolutionIdList[0]];
                                }                                                

                                const msg = param.queryMethod === "0"
                                    ? app.localize("PleaseSelectEquipment")
                                    : app.localize("PleaseSelectEquipmentGroup");
                                if (param.machineIdList.length === 0) {
                                    abp.message.warn(msg);
                                    return false;
                                }


                                page.scrollTabRender();
                                page.loadTableAndChart();
                            });
                    });
            },
            bindingShowNameEvent: function () {
                this.$showName.on('click',
                    function () {
                        page.loadTableAndChart();
                    });
            },
            bindingShowValueEvent: function () {
                this.$showValue.on('click',
                    function () {
                        page.loadTableAndChart();
                    });
            },
            bindingExportEvent: function() {
                $("#btnExport").click(function() {

                    yieldStatisticsSevice.export(page.params).done(function (response) {
                        app.downloadTempFile(response);
                    });
                });
            }
        };


        page.init();
    });
})();