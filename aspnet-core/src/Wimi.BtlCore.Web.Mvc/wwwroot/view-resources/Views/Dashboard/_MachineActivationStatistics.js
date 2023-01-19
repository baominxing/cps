
var MachinePerformanceStatisticsModule = (function ($) {
    return function () {
        var dashboardAppService = abp.services.app.dashboard;
        var chartDom = document.getElementById('chart_activation');
        var chartOption;
        var chartInstance = echarts.init(chartDom);

        function run() {
            dashboardAppService.getMachineActivationForDashboard()
                .done(function (result) {

                    chartInstance = echarts.getInstanceByDom(chartDom);
                    chartOption = getChartOption(result);
                    if (chartInstance) {
                        echarts.dispose(chartDom);
                        chartInstance = echarts.init(chartDom);
                    }

                    var seriesData = chartOption.series;
                    var xAxisData = chartOption.xAxis.data;

                    var i, item;
                    for (i = 0; i < result.previousShiftMachineActivations.length; i++) {
                        item = result.previousShiftMachineActivations[i];
                        if ($.inArray(item.machineName, xAxisData) === -1) {
                            xAxisData.push(item.machineName);
                        }

                        seriesData[0].data.push(item.activation);
                    }

                    for (i = 0; i < result.currentShiftMachineActivations.length; i++) {
                        item = result.currentShiftMachineActivations[i];
                        if ($.inArray(item.machineName, xAxisData) === -1) {
                            xAxisData.push(item.machineName);
                        }

                        seriesData[1].data.push(item.activation);
                    }
                    chartOption.series = seriesData;
                    chartOption.xAxis.data = xAxisData;
                    chartInstance.setOption(chartOption);
                });
        }


        function getAverage(dataList) {
            var result;
            if (dataList.length > 0) {
                var sum = 0;
                _.each(dataList, function (data) {
                    sum += data;
                });
                result = sum / dataList.length;
            }
            else {
                result = 0;
            }

            return result;
        }

        function getChartOption(result) {
            var percentOfChartDataZoom = (app.consts.maximumNumberofQueries.dashboardMachineActivationCount / result.currentShiftMachineActivations.length) * 100;
            var currentShiftAverage = getAverage(_.pluck(result.currentShiftMachineActivations, 'activation'));
            var previousShiftAverage = getAverage(_.pluck(result.previousShiftMachineActivations, 'activation'));

            var dataStyle = {
                normal: {
                    label: {
                        show: true,
                        position: 'top',
                        formatter: '{c}'
                    },
                    color: function (params) {
                        var colorList = ['rgb(29,137,207)', 'rgb(0,191,255)'];
                        if (params.seriesName == app.localize("LastShiftCropMomentum")) {
                            return colorList[0];
                        } else {
                            return colorList[1];
                        }
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
                legend: {
                    data: [app.localize("LastShiftCropMomentum"), app.localize("CurrentShiftCropMobility")]
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
                        name: app.localize("LastShiftCropMomentum"),
                        type: 'bar',
                        itemStyle: dataStyle,
                        data: [],
                        markLine: {
                            data: [
                                {
                                    yAxis: previousShiftAverage
                                }
                            ]
                        }
                    },
                    {
                        name: app.localize("CurrentShiftCropMobility"),
                        type: 'bar',
                        itemStyle: dataStyle,
                        data: [],
                        markLine: {
                            data: [
                                {
                                    yAxis: currentShiftAverage
                                }
                            ]

                        }
                    }
                ]
            };
        }

        return { run: run };
    };
})(jQuery);