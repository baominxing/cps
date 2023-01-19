(function ($) {
    app.modals.ShowDetailYieldModal = function () {
        var _modalManger,
            _args,
            _chartObject,
            basicDataService = abp.services.app.basicData,
            yieldService = abp.services.app.yield;


        var chartDom = document.getElementById("chart-line");

        var chartLineOption = {
            tooltip: {
                trigger: 'axis'
            },
            grid: {
                left: '5px',
                right: '40px',
                bottom: '5px',
                top: '0px',
                containLabel: true
            },
            toolbox: {
                show: false,
                feature: {
                    dataView: { show: false, readOnly: false },
                    magicType: { show: true, type: ['line', 'bar'] },
                    restore: { show: true },
                    saveAsImage: { show: true }
                }
            },
            calculable: true,
            yAxis: {
                type: "category",
                axisLabel: { interval: 0 },
                boundaryGap: true,
                data: []
            },
            xAxis: {
                type: "value",
                name: app.localize("Unit")+"(%)",
                data: []
            },
            series: []
        };

        this.init = function (modalManger, args) {
            _modalManger = modalManger;
            _args = args;

            //标识title
            var title = abp.utils.formatString("【{0}】{1}", _args.machine.name, _args.machine.summaryDate);
            _modalManger.getModal().find(".modal-title span").text(title);
        }

        //页面加载完，加载数据，图表
        this.shown = function () {
            var param = {
                MachineId: _args.machine.id,
                StartTime: _args.machine.summaryDate,
                EndTime: _args.machine.summaryDate,
                SummaryDate: _args.machine.summaryDate
            };
            //加载echart表格
            yieldService.getMachineStateRate(param)
                .done(function (response) {
                    _chartObject = echarts.getInstanceByDom(chartDom);
                    if (_chartObject) {
                        echarts.dispose(chartDom);
                    }

                    _chartObject = echarts.init(chartDom);
                    var b = _.pluck(response, 'rate');
                    var colors = getStateColor();

                    var seriesData = {
                        name: app.localize("StatusRatio"),
                        type: "bar",
                        itemStyle: {
                            normal: {
                                color: function (p) {
                                    var sateInfo = _.where(colors, { 'displayName':p.name })[0];
                                    if (sateInfo) {
                                        return sateInfo.hexcode;
                                    } else {
                                        return "red";
                                    }
                                }
                            }
                        },
                        markLine: {
                            data: [
                                { type: "average", name: app.localize("AverageValue") }
                            ]
                        },
                        label: {
                            normal: {
                                show: true,
                                position: 'right'
                            }
                        },
                        data: b
                    };

                    chartLineOption.series = seriesData;
                    chartLineOption.yAxis.data = _.map(_.pluck(response, 'code'), function (code) {
                        return app.localize(code)
                    });
                    _chartObject.setOption(chartLineOption);
                });

            //加载甘特图
            yieldService.getMachineDetailGanttCharat(param)
                .done(function (response) {
                    var dataset = _.pluck(response, "chartDataList");
                    var gantt = $("div.detail-gantt-chart");
                    gantt.attr("data-key", _.pluck(response, "machineId"));
                    gantt.StateGanttChart({ data: dataset, showTitle: false });
                    $(window)
                        .resize(function () {
                            gantt.empty().StateGanttChart({ data: dataset, showTitle: false });
                        });
                });
        }

        function getStateColor() {
            var colors = [];
            basicDataService.getStateInfoList({}, { async: false }).done(function (response) {
                colors = response.items;
            });
            return colors;
        }
    }
})(jQuery);