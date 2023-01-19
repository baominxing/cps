
var MachineUsedTimeRateStatisticsModule = (function ($) {
    return function () {
        var dashboardAppService = abp.services.app.dashboard;
        var chartDom = document.getElementById('chart_usedtimerate');
        var chartOption;
        var chartInstance = echarts.init(chartDom);

        function run() {
            dashboardAppService.getMachineUsedTimeRateForDashboard()
                .done(function (result) {
                    chartInstance = echarts.getInstanceByDom(chartDom);
                    chartOption = getChartOption(result);
                    if (chartInstance) {
                        echarts.dispose(chartDom);
                        chartInstance = echarts.init(chartDom);
                    }

                    var seriesData = chartOption.series;
                    var xAxisData = chartOption.xAxis.data;



                    for (var i = 0; i < result.currentShiftMachineUsedTimeRates.length; i++) {
                        var item = result.currentShiftMachineUsedTimeRates[i];
                        if ($.inArray(item.machineName, xAxisData) === -1) {
                            xAxisData.push(item.machineName);
                        }

                        seriesData[0].data.push(item.stopRate.toFixed(2));
                        seriesData[1].data.push(item.runRate.toFixed(2));
                        seriesData[2].data.push(item.freeRate.toFixed(2));
                        seriesData[3].data.push(item.offlineRate.toFixed(2));
                        seriesData[4].data.push(item.debugRate.toFixed(2));
                    }

                    chartOption.series = seriesData;
                    chartOption.xAxis.data = xAxisData;
                    chartInstance.setOption(chartOption);
                });
        }

        function getChartOption(result) {
            var percentOfChartDataZoom = (app.consts.maximumNumberofQueries.dashboardUsedTimeRateCount / result.currentShiftMachineUsedTimeRates.length) * 100;
            var dataStyle = {
                normal: {
                    label: {
                        show: true,
                        position: 'top',
                        formatter: '{c}'
                    }
                }
            };
            return {
                tooltip: {
                    trigger: 'axis'
                },
                toolbox: {
                    feature: {
                    }
                },
                dataZoom: {
                    show: true,
                    realtime: true,
                    height: 15,
                    start: 0,
                    end: percentOfChartDataZoom,
                    bottom: "10px"
                },
                color: ["#d43a36", "#4cae4c", "#f2a332", "#c4c4c4", "#1d89cf"],
                legend: {
                    data: [app.localize("Stop"), app.localize("Run"), app.localize("Free"), app.localize("Offline"), app.localize("Debug")]
                },
                xAxis: {
                    type: 'category',
                    data: [],
                    axisLabel: {
                        interval: 0
                    }
                },
                yAxis: {
                    type: 'value',
                    name: app.localize("Percentage") + '(%)',
                    min: 0,
                    max: 100
                },
                series: [
                    {
                        name: app.localize("Stop"),
                        type: 'bar',
                        itemStyle: dataStyle,
                        data: []
                    },
                    {
                        name: app.localize("Run"),
                        type: 'bar',
                        itemStyle: dataStyle,
                        data: []
                    },
                    {
                        name: app.localize("Free"),
                        type: 'bar',
                        itemStyle: dataStyle,
                        data: []
                    },
                    {
                        name: app.localize("Offline"),
                        type: 'bar',
                        itemStyle: dataStyle,
                        data: []
                    }, {
                        name: app.localize("Debug"),
                        type: 'bar',
                        itemStyle: dataStyle,
                        data: []
                    }
                ]
            };
        }

        return { run: run };
    };
})(jQuery);