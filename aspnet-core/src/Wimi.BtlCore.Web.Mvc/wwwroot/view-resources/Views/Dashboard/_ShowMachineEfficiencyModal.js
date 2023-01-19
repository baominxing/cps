//# sourceURL=ShowTenantDetailModal.js

(function () {
    app.modals.ShowMachineEfficiencyModal = function () {
        var dashboardService = abp.services.app.dashboard;
        var _modalManager;
        var _args;
        var pageApp = {};

        this.shown = function () {

            var machineModule = {
                _getTargetClassName: function (stateInt) {
                    switch (stateInt) {
                        case 1:
                            //报警
                            return { boxClassName: "box-danger", padbgClassName: "bg-red" };
                        case 2:
                            //运行
                            return { boxClassName: "box-success", padbgClassName: "bg-green" };
                        case 3:
                            //空闲
                            return { boxClassName: "box-warning", padbgClassName: "bg-yellow" };
                        case "Debug":
                            //调试
                            return { boxClassName: "box-warning", padbgClassName: "bg-blue" };
                        default:
                            //离线
                            return { boxClassName: "box-default", padbgClassName: "bg-gray" };
                    }
                },
                statisticChartOption: {
                    tooltip: {
                        trigger: 'axis',
                        axisPointer: {
                            // 坐标轴指示器，坐标轴触发有效
                            type: 'shadow' // 默认为直线，可选为：'line' | 'shadow'
                        }
                    },
                    legend: {
                        top: "10px",
                        data: [app.localize("OperatingRate"), app.localize("FailureRate"), app.localize("IdleRate")]
                    },
                    toolbox: {
                        show: true,
                        feature: {
                            mark: { show: true },
                            dataView: { show: false, readOnly: false },
                            saveAsImage: { show: true }
                        }
                    },
                    calculable: true,
                    dataZoom: {
                        show: true,
                        realtime: true,
                        height: 10,
                        bottom: "10px",
                        start: 50,
                        end: 100
                    },
                    grid: {
                        left: '20px',
                        right: '30px',
                        bottom: '25px',
                        containLabel: true
                    },
                    xAxis: [
                        {
                            type: 'category',
                            axisLabel: { interval: 0 }, //强制显示所有标签
                            data: []
                        }
                    ],
                    yAxis: [
                        {
                            type: 'value',
                            name: app.localize("Percentage") + '(%)',
                            min: 0,
                            max: 100
                        }
                    ],
                    series: [
                        {
                            name: app.localize("OperatingRate"),
                            type: 'line',
                            smooth: true,
                            data: []
                        }, {
                            name: app.localize("FailureRate"),
                            type: 'line',
                            smooth: true,
                            data: []
                        }, {
                            name: app.localize("IdleRate"),
                            type: 'line',
                            smooth: true,
                            data: []
                        }]
                },
                loadMachineStatistic: function ($machineCnt) {

                    dashboardService
                        .getDashboardStatisticForChartInGivenDays({
                            dateFrom: moment().add(-7, 'days').format("YYYYMMDD"),
                            dateEnd: moment().add(-1, 'days').format("YYYYMMDD"),
                            machineId: $machineCnt.data('machine-id')
                        })
                        .done(function (data) {

                            var chartDom = $machineCnt.find(".statistic-detailchart").get(0);
                            var chartInstance = echarts.getInstanceByDom(chartDom);
                            if (!chartInstance) {
                                chartInstance = echarts.init(chartDom);
                            }

                            var seriesArray = machineModule.statisticChartOption.series;
                            for (var i = 0; i < seriesArray.length; i++) {
                                switch (seriesArray[i].name) {
                                    case app.localize("OperatingRate"):
                                        seriesArray[i].data = data.runRate;
                                        break;
                                    case app.localize("FailureRate"):
                                        seriesArray[i].data = data.stopRate;
                                        break;
                                    case app.localize("IdleRate"):
                                        seriesArray[i].data = data.freeRate;
                                        break;
                                    default:
                                        break;
                                }
                            }
                            machineModule.statisticChartOption.series = seriesArray;
                            machineModule.statisticChartOption.xAxis[0].data = data.summaryDate;
                            chartInstance.setOption(machineModule.statisticChartOption);
                        });
                },
                loadMachineRealTimePad: function ($machineCnt) {

                    var machineData = $machineCnt.data();
                    if (!machineData) {
                        return false;
                    }
                    var code = machineData.machineCode;
                    var id = machineData.machineId;
                    dashboardService
                        .getMachineRealTimePadData({
                            machineCode: code,
                            machineId: id
                        }).done(function (data) {
                            //get new classname by state
                            var newClasses = machineModule._getTargetClassName(data.machineStatus);

                            //remove box css
                            var oldBoxClassName = $machineCnt.attr('class').match('box-[A-Za-z]+');

                            if (oldBoxClassName === null) {

                                $machineCnt.addClass(newClasses.boxClassName);

                            } else if (oldBoxClassName[0] !== newClasses.boxClassName) {
                                $machineCnt.removeClass(oldBoxClassName[0]);
                                $machineCnt.addClass(newClasses.boxClassName);
                            }

                            //remove bg css
                            var $statisticPad = $machineCnt.find('.statistic-pad');
                            if ($statisticPad.attr('class')) {
                                var oldPadbgClassName = $statisticPad.attr('class').match('bg-[A-Za-z]+');
                                if (oldPadbgClassName === null) {
                                    $statisticPad.addClass(newClasses.padbgClassName);
                                } else if (oldPadbgClassName[0] !== newClasses.padbgClassName) {
                                    $statisticPad.removeClass(oldPadbgClassName[0]);
                                    $statisticPad.addClass(newClasses.padbgClassName);
                                }
                            }

                            $machineCnt.find('.yield').text(data.yield);
                            $machineCnt.find('.stop-rate').text(data.stopRate);
                            $machineCnt.find('.run-rate').text(data.runRate);
                            $machineCnt.find('.free-rate').text(data.freeRate);
                        });

                },
                getMachines: function () {
                    var $machines = $(".modal .machine-box");
                    if ($machines.length === 0) {
                        return null;
                    }
                    return $machines;
                },
                initChart: function () {
                    var machineCollection = machineModule.getMachines();
                    if (machineCollection !== null) {
                        $.each(machineCollection, function (index, item) {
                            var $machineCnt = $(item);
                            //get statistic data
                            machineModule.loadMachineStatistic($machineCnt);
                            //get realtime data
                            machineModule.loadMachineRealTimePad($machineCnt);
                        });
                    }

                    //定时器 刷新内容
                    clearInterval(pageApp.timeTicket);
                    pageApp.timeTicket = setInterval(function () {
                        if (machineCollection !== null) {
                            $.each(machineCollection, function (index, item) {
                                var $machineCnt = $(item);
                                machineModule.loadMachineRealTimePad($machineCnt);
                            });
                        }
                    }, 60000);
                }
            }

            machineModule.initChart();
            $('.btn-refresh').click(function () {
                machineModule
                    .loadMachineRealTimePad($(this).closest(".modal .machine-box"));
            });

            _modalManager.onClose(function () {
                clearInterval(pageApp.timeTicket);
            });
        }

        this.init = function (modalManager, args) {
            _modalManager = modalManager;
            _args = args.groupId;


        }
    };
})();