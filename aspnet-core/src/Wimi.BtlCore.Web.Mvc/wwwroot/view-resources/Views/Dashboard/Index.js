
(function (abp) {


})(abp);



(function () {
    $(function () {

        var dashboardService = abp.services.app.dashboard;
        var stateSummaryModule = new StateStatisticModule();

        var machinePerformanceStatisticsModule = new MachinePerformanceStatisticsModule();
        var machineUsedTimeRateStatisticsModule = new MachineUsedTimeRateStatisticsModule();

        var detailModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Dashboard/ShowMachineEfficiencyModal',
            scriptUrl: abp.appPath + 'view-resources/Views/Dashboard/_ShowMachineEfficiencyModal.js',
            modalClass: 'ShowMachineEfficiencyModal',
            modalSize: 'modal-lg'
        });

        var shiftModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Dashboard/MachineShiftWarning',
            scriptUrl: abp.appPath + 'view-resources/Views/Dashboard/_MachineShiftWarning.js',
            modalClass: 'MachineShiftWarningModal'
        });


        var pageApp = {};
        var machineModule = {
            _getTargetClassName: function (stateCode) {
                switch (stateCode) {
                    case "Stop":
                        //停机
                        return { boxClassName: "box-danger", padbgClassName: "bg-red" };
                    case "Run":
                        //运行
                        return { boxClassName: "box-success", padbgClassName: "bg-green" };
                    case "Free":
                        //空闲
                        return { boxClassName: "box-warning", padbgClassName: "bg-yellow" };
                    case "Debug":
                        //调试
                        return { boxClassName: "box-warning", padbgClassName: "bg-blue" };
                    default:
                        //"Offline"
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
                    bottom: "10px"
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
                        data: []
                    }, {
                        name: app.localize("FailureRate"),
                        type: 'line',
                        data: []
                    }, {
                        name: app.localize("IdleRate"),
                        type: 'line',
                        data: []
                    }
                ]
            },
            loadMachineStatistic: function ($machineCnt) {

                dashboardService
                    .getDashboardStatisticForChartInGivenDaysByGroupId({
                        dateFrom: moment().add(-7, 'days').format("YYYYMMDD"),
                        dateEnd: moment().format("YYYYMMDD"),
                        groupId: $machineCnt.data('group-id')
                    })
                    .done(function (data) {

                        var chartDom = $machineCnt.find(".statistic-chart").get(0);
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
            getMachines: function () {
                var $machines = $(".machine-box");
                if ($machines.length === 0) {
                    return null;
                }
                return $machines;
            },

            init: function () {
                //var machineCollection = machineModule.getMachines();
                //if (machineCollection !== null) {
                //    $.each(machineCollection, function (index, item) {
                //        var $machineCnt = $(item);
                //        //get statistic data
                //       //   machineModule.loadMachineStatistic($machineCnt);
                //    });
                //}

                stateSummaryModule.run();
                machinePerformanceStatisticsModule.run();
                machineUsedTimeRateStatisticsModule.run();

                //定时器 刷新内容
                clearInterval(pageApp.timeTicket);
                pageApp.timeTicket = setInterval(function () {

                    stateSummaryModule.run();

                    //if (machineCollection !== null) {
                    //    $.each(machineCollection, function (index, item) {
                    //        var $machineCnt = $(item);
                    //        machineModule.loadMachineStatistic($machineCnt);
                    //    });
                    //}
                },
                    60000);
            }
        };

        machineModule.init();

        $('.btn-refresh').click(function () {
            var group = $(this).closest(".machine-box").data();
            detailModal.open({ groupId: group.groupId });
        });

        // 设备排班到期提醒
        $('.content-header').on('click',
            '.shift-warning',
            function (e) {
                e.preventDefault();
                shiftModal.open();
            });

        // 未排班设备
        $('.content-header').on('click',
            '.shift-scheduling',
            function (e) {
                e.preventDefault();
                abp.message.error($(e.currentTarget).data('shift'), app.localize("FollowingMachineWithoutShift"));
            });



        var shortcurMenuAppService = abp.services.app.shortcutMenu;

        var _shortcutMenuModal = new app.ModalManager({
            viewUrl: abp.appPath + "Dashboard/ShortcutMenuModal",
            scriptUrl: abp.appPath + "view-resources/Views/Dashboard/_ShortcutMenuModal.js",
            modalClass: "ShortcutMenuModal"
        });

        var vmData = {
            bindMenus: []
        };

        function getSystemMenus(menuItems) {
            var menus = [];

            _.each(menuItems, function (item) {

                if (item.items.length === 0) {
                    menus.push(item);
                } else {
                    var cmenus = getSystemMenus(item.items);
                    menus = _.union(menus, cmenus);
                }
            });

            return menus;
        }

        function buildBindMenus(bindMenus) {
            var menus = [];

            var systemMenus = getSystemMenus(abp.nav.menus.Mpa.items);

            _.each(bindMenus, function (bindItem) {

                var existMenu = _.find(systemMenus, function (item) {
                    return item.name === bindItem.name;
                });

                if (existMenu != null) {
                    existMenu.url = abp.appPath + existMenu.url;
                    existMenu.showRemove = false;
                    menus.push(existMenu);
                }

            });
            return menus;
        }

        vmData.init = function () {
            shortcurMenuAppService.listBindMenu()
                .done(function (result) {
                    vmData.bindMenus = buildBindMenus(result);
                });
        };

        // ReSharper disable once UnusedLocals
        var vm = new Vue({
            el: '#app-shortcutMenu',
            data: vmData,
            created: function () {
                vmData.init();
            },
            methods: {
                openMenuModal: function () {
                    _shortcutMenuModal.open();
                },
                clearAll: function () {

                    var msg = abp.utils.formatString(app.localize("RemoveShotcutMenusConfirm"));
                    abp.message.confirm(msg, function (isConfirmed) {

                        if (isConfirmed) {

                            shortcurMenuAppService.removeAll()
                                .done(function (result) {
                                    vmData.init();
                                    abp.notify.success(app.localize("RemoveShotcutMenusSuccess"));
                                });

                        }

                    });

                },
                removeMenu: function (menu) {

                    var msg = app.localize("RemoveOneShotcutMenuConfirm{0}", menu.displayName);
                    abp.message.confirm(msg, function (isConfirmed) {

                        if (isConfirmed) {
                            shortcurMenuAppService.unBindMenu(menu)
                                .done(function () {
                                    vmData.init();
                                    abp.notify.success(app.localize("RemoveOneShotcutMenuSuccess"));
                                });

                        }

                    });

                },
                restoreDefaults: function () {
                    var msg = abp.utils.formatString(app.localize("RestoreShortcutMenusConfirm"));
                    abp.message.confirm(msg, function (isConfirmed) {

                        if (isConfirmed) {

                            shortcurMenuAppService.restoreDefaults()
                                .done(function (result) {
                                    vmData.init();
                                    abp.notify.success(app.localize("RestoreShortcutMenusSuccess"));
                                });

                        }

                    });
                }
            }
        });

        abp.event.on("app.shortcutMenuModalSaved", function () {
            vmData.init();
        });
    });
})();



