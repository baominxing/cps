(function () {
    $(function () {

        var pageFirstLoad = true;

        var staffPerformance = abp.services.app.staffPerformance;

        var filterModal = new app.ModalManager({
            viewUrl: abp.appPath + "StaffPerformance/FilterModal",
            scriptUrl: abp.appPath + "view-resources/Views/StaffPerformance/_FilterModal.js",
            modalClass: "FilterModal"
        });

        var page = {

            parameters: null,

            //加载ScrollTab
            loadScrollTab: function (parameters, queryType) {

                if (parameters.machineIdList.length === 0) {
                    abp.message.warn(app.localize("PleaseSelectEquipment") + "！");
                    return false;
                }

                if (parameters.userIdList === null) {
                    abp.message.warn(app.localize("PleaseSelectEmployees") + "！");
                    return false;
                }

                staffPerformance.getScrollTab(parameters).done(function (result) {
                    if (result != null && result.length > 0) {

                        var tabData = [];
                        var source = $("#tab-template").html();
                        var rendered = Handlebars.compile(source);

                        if (queryType === "0") {
                            for (var i = 0; i < result.length; i++) {

                                tabData.push({
                                    id: result[i].userId,
                                    name: result[i].userName
                                });
                            }


                            $("#scrolltab").html(rendered({ data: tabData }));
                            $("#scrolltab").scrollTabs({
                                click_callback: function () {
                                    var userId = $(this).attr("id");
                                    parameters.userIdList = [userId];
                                    page.loadStaffPerformanceECharts(parameters, queryType);
                                }
                            });

                            parameters.userIdList = [tabData[0].id];

                        } else {
                            for (var j = 0; j < result.length; j++) {

                                var tabItem = result[j];
                                tabData.push({
                                    id: tabItem.machineId,
                                    name: tabItem.machineName
                                });
                            }

                            $("#scrolltab2").html(rendered({ data: tabData }));
                            $("#scrolltab2").scrollTabs({
                                click_callback: function () {
                                    var machineId = $(this).attr("id");
                                    parameters.machineIdList = [machineId];
                                    page.loadStaffPerformanceECharts(parameters, queryType);
                                }
                            });
                            parameters.machineIdList = [tabData[0].id];
                        }

                        page.loadStaffPerformanceECharts(parameters, queryType);
                    }
                });
            },

            //加载设备ECharts
            loadStaffPerformanceECharts: function (parameters, queryType) {
                staffPerformance.getStaffPerformance(parameters).done(function (result) {
                    var colorDatas = ["#d43a36", "#4cae4c", "#f2a332", "#c4c4c4", "#1d89cf"];
                    var legendDatas = [app.localize("Stop"), app.localize("Run"), app.localize("Free"), app.localize("Offline"), app.localize("Debug")];
                    var xAxisDatas = [];
                    var chartDom;

                    if (queryType === "0") {
                        chartDom = document.getElementById("chart-bar-staff");
                    } else {
                        chartDom = document.getElementById("chart-bar-machine");
                    }

                    var chartInstance = echarts.getInstanceByDom(chartDom);
                    if (chartInstance) {
                        echarts.dispose(chartDom);
                    }
                    chartInstance = echarts.init(chartDom);

                    var recordCount = 1;
                    if (result.stateRates.length !== 0) {
                        recordCount = result.stateRates.length;
                    };

                    var chartOption = page.getOption(recordCount, colorDatas, legendDatas, xAxisDatas);

                    //填充状态数据
                    var xData;
                    var i;
                    if (result.stateRates.length > 0) {

                        for (i = 0; i < result.stateRates.length; i++) {
                            if (queryType === "0") {
                                xData = result.stateRates[i].machineName + "\r\n" + result.stateRates[i].summaryDate;
                                if (jQuery.inArray(xData, xAxisDatas) === -1) {
                                    xAxisDatas.push(xData);
                                }
                            } else {
                                xData = result.stateRates[i].userName + "\r\n" + result.stateRates[i].summaryDate;
                                if (jQuery.inArray(xData, xAxisDatas) === -1) {
                                    xAxisDatas.push(xData);
                                }
                            }


                            chartOption.series[0].data.push(result.stateRates[i].stopDurationRate.toFixed(2));
                            chartOption.series[1].data.push(result.stateRates[i].runDurationRate.toFixed(2));
                            chartOption.series[2].data.push(result.stateRates[i].freeDurationRate.toFixed(2));
                            chartOption.series[4].data.push(result.stateRates[i].debugDurationRate.toFixed(2));
                            chartOption.series[3].data.push((100 -
                                result.stateRates[i].stopDurationRate.toFixed(2) -
                                result.stateRates[i].runDurationRate.toFixed(2) -
                                result.stateRates[i].freeDurationRate.toFixed(2) -
                                result.stateRates[i].debugDurationRate.toFixed(2)).toFixed(2));
                        }
                    }

                    //填充原因数据
                    if (result.reasonRates.length > 0) {
                        for (i = 0; i < result.reasonRates.length; i++) {

                            var lData = result.reasonRates[i].reasonName;
                            var hexcode = result.reasonRates[i].hexcode;
                            if (jQuery.inArray(lData, legendDatas) === -1 && hexcode != null) {
                                legendDatas.push(lData);
                                colorDatas.push(hexcode);
                            }

                            (function (n) {
                                if ($.grep(chartOption.series, function (v) { return v.name === result.reasonRates[n].reasonName; }).length === 0) {
                                    var serieData = {
                                        name: result.reasonRates[n].reasonName,
                                        type: 'bar',
                                        stack: app.localize("Reason"),
                                        data: [(result.reasonRates[n].reasonRate).toFixed(2)]
                                    };
                                    chartOption.series.push(serieData);
                                }
                                else {
                                    for (var j = 0; j < chartOption.series.length; j++) {
                                        if (chartOption.series[j].name === result.reasonRates[n].reasonName) {
                                            chartOption.series[j].data.push((result.reasonRates[n].reasonRate).toFixed(2));
                                            break;
                                        }
                                    }
                                }
                            })(i);
                        }
                    }

                    chartInstance.setOption(chartOption);
                });
            },

            //获取ECharts的Option
            getOption: function (recordCount, colorDatas, legendDatas, xAxisDatas) {
                return {
                    color: colorDatas,
                    legend: {
                        data: legendDatas
                    },
                    tooltip: {
                        trigger: "item",
                        formatter: function (params) {
                            return params.seriesName + ":" + params.data + "%";
                        }
                    },
                    grid: {
                        height: 300,
                        left: "3%",
                        right: "4%",
                        bottom: "10%",
                        containLabel: true
                    },
                    dataZoom: [
                        {
                            show: true,
                            realtime: true,
                            start: 0,
                            end: (app.consts.maximumNumberofQueries.staffPerformanceRecordCount / recordCount) * 100,
                            height: 15,
                            bottom: "10px"
                        }
                    ],
                    xAxis: [
                        {
                            type: "category",
                            boundaryGap: true,
                            axisLabel: {
                                interval: 0
                            },
                            data: xAxisDatas
                        }
                    ],
                    yAxis: [
                        {
                            name: app.localize("Unit") + "%",
                            type: "value",
                            axisLabel: {
                                show: true,
                                interval: "auto",
                                formatter: "{value}"
                            },
                            max: 100
                        }
                    ],
                    series: [
                        {
                            name: app.localize("Stop"),
                            type: 'bar',
                            stack: app.localize("State"),
                            data: []
                        }, {
                            name: app.localize("Run"),
                            type: 'bar',
                            stack: app.localize("State"),
                            data: []
                        }, {
                            name: app.localize("Free"),
                            type: 'bar',
                            stack: app.localize("State"),
                            data: []
                        }, {
                            name: app.localize("Offline"),
                            type: 'bar',
                            stack: app.localize("State"),
                            data: []
                        }, {
                            name: app.localize("Debug"),
                            type: 'bar',
                            stack: app.localize("State"),
                            data: []
                        }
                    ]
                };
            },

            loadStaffResult: function () {
                var parameters = {
                    startTime: page.parameters.startTime,
                    endTime: page.parameters.endTime,
                    statisticalWay: page.parameters.statisticalWay,
                    machineIdList: page.parameters.machineIdList,
                    userIdList: page.parameters.userIdList,
                    queryType: "0",
                    shiftSolutionId: page.parameters.shiftSolutionId
                };

                page.loadScrollTab(parameters, "0");
            },

            loadMachineResult: function () {
                var parameters = {
                    startTime: page.parameters.startTime,
                    endTime: page.parameters.endTime,
                    statisticalWay: page.parameters.statisticalWay,
                    machineIdList: page.parameters.machineIdList,
                    userIdList: page.parameters.userIdList,
                    queryType: "1",
                    shiftSolutionId: page.parameters.shiftSolutionId
                };
                page.loadScrollTab(parameters, "1");
            },

            setSearchParametersAndLoadEchart: function (parameters) {
                page.parameters = parameters;

                var $searchFilter = $("#searchFilter");
                var $searchFilter2 = $("#searchFilter2");
                var showString = parameters.startTime + "~" + parameters.endTime + " " + parameters.statisticalWayText + " " + parameters.shiftSolutionName;
                $searchFilter.text(showString);
                $searchFilter2.text(showString);

                page.loadStaffResult();

                page.loadMachineResult();
            },

            init: function () {

                var parameters = {
                    startTime: moment().format('YYYY-MM-DD'),
                    endTime: moment().format('YYYY-MM-DD'),
                    statisticalWay: "ByDay",
                    statisticalWayText: app.localize("ByDay"),
                    machineIdList: [0],
                    userIdList: [0],
                    queryType: "0",
                    shiftSolutionId: 0,
                    shiftSolutionName: ""
                };

                page.setSearchParametersAndLoadEchart(parameters);

                //pageFirstLoad = false;

                //点击查询按钮
                $("#btnQuery").click(function () {
                    filterModal.open({}, page.setSearchParametersAndLoadEchart);
                });
            }
        };

        page.init();
    });
})();