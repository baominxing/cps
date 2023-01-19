(function ($) {
    app.modals.ShowCompareMachineYieldModal = function() {
        var _modalManger,
            _args,
            _chartObject,
            yieldService = abp.services.app.yield;
        var chartDom = document.getElementById("chart-line");

        var chartLineOption = {
            legend: {
                show: true,
                data: [app.localize("UtilizationRatio")]
            },
            tooltip: {
                trigger: 'axis'
            },
            grid: {
                left: '0px',
                right: '0px',
                bottom: '15px',
                containLabel: true
            },
            toolbox: {
                show: true,
                feature: {
                    dataView: { show: false, readOnly: false },
                    magicType: { show: true, type: ['line', 'bar'] },
                    restore: { show: true },
                    saveAsImage: { show: true }
                }
            },
            calculable: true,
            dataZoom: {
                show: true,
                realtime: true,
                height: 10,
                bottom: "0px",
                start: 0,
                end: 100
            },
            xAxis: {
                type: "category",
                axisLabel: { interval: 0 },
                boundaryGap: true,
                data: []
            },
            yAxis: {
                type: "value",
                name: app.localize("Unit") + "(%)"
            },
            series: []
        };


        this.init = function(modalManger, args) {
            _modalManger = modalManger;
            _args = args;
        }

        //页面加载完，加载数据，图表
        this.shown = function() {
            var param = {
                MachineIdList: _args.param.machineIdList,
                StartTime: _args.param.summaryDate,
                EndTime: _args.param.summaryDate,
                SummaryDate: _args.param.summaryDate
            };

            //echart 图
            yieldService.getMachineUtilizationRate(param)
                .done(function(response) {
                    _chartObject = echarts.getInstanceByDom(chartDom);
                    if (_chartObject) {
                        echarts.dispose(chartDom);
                    }

                    _chartObject = echarts.init(chartDom);
                    var b = _.pluck(response, 'utilizationRate');
                    var seriesData = {
                        name: app.localize("UtilizationRatio"),
                        type: "bar",
                        markLine: {
                            data: [
                                { type: "average", name: app.localize("AverageValue") }
                            ]
                        },
                        label: {
                            normal: {
                                show: true,
                                position: 'top'
                            }
                        },
                        data: b
                    };

                    chartLineOption.series = seriesData;
                    chartLineOption.xAxis.data = _.pluck(response, 'machineName');
                    _chartObject.setOption($.WIMI.echartOptionBuilder(chartLineOption));
                });


            //状态甘特图
            yieldService.getMachineStatesGanttChart(param)
                .done(function(response) {
                    var html = [];
                    _.each(_.pluck(response, "machineId"),
                        function(key) {
                            html.push('<div class="machine-gantt" data-key=' + key + '></div>');
                        });

                    $("#tab_20-20").append(html.join(''));
                    var option = {
                        timelineSelector: ".gantt-timeline",
                        ganttChartSelector: ".machine-gantt",
                        dataset: _.pluck(response, "chartDataList"),
                        SummaryDate: _args.param.summaryDate
                    };
                    _args.param.callback(option);
                });
        };
    };
})(jQuery);