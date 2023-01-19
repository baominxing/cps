(function () {
    $(function () {
        var alarmsService = abp.services.app.alarms;
        var pageFirstLoad = true;
        var percentOfDetailChartDataZoom = 0;
        var originalDetailBarChartHeight = $("#AlarmStatisticDetailChart").height();
        var statisticalWay = "ByDay";
        var _$showName = $("#showName");
        var _$showValue = $("#showValue");
        var lastQueryParams = null;
        var dom = document.getElementById("AlarmStatisticChart");
        var gridwidth = dom.clientWidth - 2 * 0.1 * dom.clientWidth;
        //加载鼠标点击柱状图的设备报警的详细信息
        var selectedParameters;
        var selectedMachine;
        //加载详细信息表格
        var $AlarmStatisticDetailTable = $("#AlarmStatisticDetailTable");
        var $AlarmStatisticDetailDataTable;
        var classIndex = 0;
        var shiftSolutionNames = null;
        var shiftSolutionIds = null;

        var filterModal = new app.ModalManager({
            viewUrl: abp.appPath + "AlarmStatistics/FilterModal",
            scriptUrl: abp.appPath + "view-resources/Views/StatisticAnalysis/AlarmStatistics/_FilterModal.js",
            modalClass: "FilterModal"
        });

        var alarmsDetailModal = new app.ModalManager({
            viewUrl: abp.appPath + "AlarmStatistics/ShowAlarmsDetailModal",
            scriptUrl: abp.appPath + "view-resources/Views/StatisticAnalysis/AlarmStatistics/_ShowAlarmsDetailModal.js",
            modalClass: "ShowAlarmsDetailModal"
        });

        var page = {
            exportParam: null,

            setSearchParametersAndLoadBarEChart: function (parameters) {
                lastQueryParams = parameters;
                page.exportParam = parameters;
                shiftSolutionNames = parameters.machineShiftSolutionNameList;
                shiftSolutionIds = parameters.machineShiftSolutionIdList;
                $("#AlarmStatisticDetailHeadMessage").text('');
                statisticalWay = parameters.statisticalWay;
                page.checkDataRecordCount(parameters);
            },

            checkDataRecordCount: function (parameters) {
                alarmsService.getAlarmChartDataCount(parameters).done(function (result) {
                    if (result !== null) {
                        if (result > app.consts.maximumNumberofQueries.machineAlarmRecordCount) {

                            abp.message.confirm(
                                app.localize("SearchDataTooMuch"),
                                function (isConfirmed) {
                                    if (isConfirmed) {
                                        page.setBarEChart(parameters);
                                    }
                                }
                            );
                        }
                        else {
                            page.setBarEChart(parameters);
                        }
                    }
                });
            },

            //加载柱状图
            setBarEChart: function (parameters) {
                if (parameters.statisticalWay === "ByShift") {
                    page.constructShiftTabs(parameters);
                }
                else {
                    console.log("parameters : ");
                    console.log(parameters);
                    alarmsService.getAlarmChartData(parameters).done(function (result) {
                        if (result != null) {
                            var filter = app.localize(parameters.statisticalWay) + ":" + parameters.startTime + "~" + parameters.endTime;
                            page.setSearchFilter(filter);
                            page.loadEChart(result, "AlarmStatisticChart");
                        }
                    });
                }
            },

            //设置EChartOption
            getChartOption: function (result) {
                return {
                    tooltip: {
                        trigger: "item",
                        formatter: function (params) {
                            if (params.seriesName != null) {
                                var machineGroupName = app.localize("MachineCode") + ":" + params.seriesName.split("|")[0] + "<br/>";
                                machineGroupName += app.localize("MachineName") + ":" + params.seriesName.split("|")[1] + "<br/>";
                                machineGroupName += app.localize("NameOfGroup") + ":" + params.seriesName.split("|")[2];

                                return machineGroupName;
                            }
                        }
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
                        end: 100,
                        bottom: "10px"
                    },
                    legend: {
                        data: []
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
                        name: app.localize("Number")
                    },
                    series: []
                };
            },


            //加载EChart图表
            loadEChart: function (result, elementId) {

                // 清理明细组件
                page.clearAlarmDetail();

                var isShowName = _$showName.is(":checked");
                var isShowValue = _$showValue.is(":checked");

                var chartDom = document.getElementById(elementId);
                var chartInstance = echarts.getInstanceByDom(chartDom);
                if (chartInstance) {
                    echarts.dispose(chartDom);
                }
                chartInstance = echarts.init(chartDom);

                var chartOption = page.getChartOption(result);

                var legendDatas = chartOption.legend.data;
                var xAxisDatas = chartOption.xAxis.data;
                var seriesDatas = chartOption.series;

                var dataStyle = {
                    normal: {
                        label: {
                            show: true,
                            position: 'top',
                            align: 'left',
                            verticalAlign: 'middle',
                            fontSize: 12,
                            color: '#000',
                            rotate: 90,
                            formatter: function (item) {
                                if (item.dataIndex === 0 && item.seriesIndex === 0 && pageFirstLoad) {
                                    var machine = item.seriesName;
                                    var summaryDate = item.name;
                                    page.loadDetailInfo(machine, summaryDate, statisticalWay);
                                    pageFirstLoad = false;
                                }
                                if (isShowName && isShowValue) {
                                    return item.seriesName.split('|')[1] + "\r\n" + item.data;
                                } else if (isShowName) {
                                    return item.seriesName.split('|')[1];
                                } else if (isShowValue) {
                                    return item.data;
                                } else {
                                    return "";
                                }
                            }
                        }
                    }
                };
                for (var i = 0; i < result.length; i++) {

                    var lData = result[i].machineId + "|" + result[i].machineName + "|" + result[i].machineGroupName;

                    if (jQuery.inArray(lData, legendDatas) === -1) {
                        //legendDatas.push(lData);
                    }
                    var xData = result[i].summaryDate;

                    if (jQuery.inArray(xData, xAxisDatas) === -1) {
                        xAxisDatas.push(xData);
                    }

                    (function (n, l) {
                        if ($.grep(seriesDatas, function (v) { return v.name + "," + v.stack === l + "," + result[n].machineId; }).length === 0) {
                            seriesDatas.push(
                                {
                                    name: l,
                                    type: "bar",
                                    barGap: '20%',
                                    barCategoryGap: '10%',
                                    stack: result[n].machineId,
                                    itemStyle: dataStyle,
                                    data: [result[n].alarmCount]
                                });
                        }
                        else {
                            for (var j = 0; j < seriesDatas.length; j++) {
                                if (seriesDatas[j].name + "," + seriesDatas[j].stack === l + "," + result[n].machineId) {
                                    seriesDatas[j].data.push(result[n].alarmCount);
                                    break;
                                }
                            }
                        }
                    })(i, lData);
                }

                chartOption.legend.data = legendDatas;
                chartOption.xAxis.data = xAxisDatas;
                chartOption.series = seriesDatas;
                
                chartOption.dataZoom.end = 100;         

                chartInstance.setOption($.WIMI.echartOptionBuilder(chartOption));

                chartInstance.on('click', function (params) {
                    if (params.componentType === 'series') {
                        var machine = params.seriesName;
                        var summaryDate = params.name;
                        page.loadDetailInfo(machine, summaryDate, statisticalWay);
                    }
                });


                if (!pageFirstLoad) {
                    console.log(selectedMachine);
                    console.log(selectedParameters);
                    if (selectedParameters == null) {
                        return;
                    }
                    page.loadDetailInfo(selectedMachine, selectedParameters.summaryDate, selectedParameters.statisticalWay);
                }
            },
            loadDetailInfo: function (machine, summaryDate, statisticalWay) {
                console.log(machine + "--" + summaryDate + "--" + statisticalWay);
                var $alarmStatisticDetailHead = $("#AlarmStatisticDetailHeadMessage");
                var showString = app.localize("SelectedEquipment") + ": " + machine.split("|")[1] + app.localize("StatisticalTime") + ": " + summaryDate;
                selectedMachine = machine;
                $alarmStatisticDetailHead.text(showString);

                var selectString = $("#selectString").val();
                var chartString = $("#chartString").val();

                var parameters = {
                    startTime: lastQueryParams.startTime,
                    endTime: lastQueryParams.endTime,
                    selectString: selectString,
                    machineIdList: [machine.split("|")[0]],
                    summaryDate: summaryDate,
                    statisticalWay: statisticalWay
                };

                selectedParameters = parameters;

                alarmsService.getAlarmDetailData(parameters).done(function (result) {
                    if (result != null) {

                        page.loadDetailTable(result);

                        if (chartString === "Pie") {
                            page.loadDetailPieChart(result);
                        } else {
                            percentOfDetailChartDataZoom = (10 / result.length) * 100;
                            page.loadDetailBarChart(result);
                        }
                    }
                });
            },

            //加载饼图数据
            loadDetailPieChart: function (result) {
                var option = {
                    tooltip: {
                        trigger: 'item',
                        formatter: "{a} <br/>{b} : {d}%"
                    },
                    legend: {
                        orient: 'vertical',
                        left: 'left',
                        data: []
                    },
                    series: [
                        {
                            name: app.localize("AlarmStatistics"),
                            type: 'pie',
                            radius: '80%',
                            center: ['50%', '50%'],
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
                };
                $("#AlarmStatisticDetailChart").height(originalDetailBarChartHeight);
                var chartDom = document.getElementById('AlarmStatisticDetailChart');

                var chartInstance = echarts.init(chartDom);
                var chartOption = option;
                if (chartInstance) {
                    echarts.dispose(chartDom);
                    chartInstance = echarts.init(chartDom);
                }

                var seriesDatas = chartOption.series[0].data;
                for (var i = 0; i < result.length; i++) {
                    var item = { value: result[i].alarmCount, name: app.localize("AlarmNumber") + ":" + result[i].alarmCode + "(" + result[i].alarmCount + app.localize("Number") + ")" };
                    seriesDatas.push(item);
                }

                chartOption.series[0].data = seriesDatas;
                chartInstance.setOption(chartOption);
            },

            //加载柱状图数据
            loadDetailBarChart: function (result) {
                var option = {
                    //tooltip: {
                    //    trigger: 'item',
                    //    formatter: "{a} <br/>{b} : {c} ({d}%)"
                    //},
                    legend: {
                        data: []
                    },
                    grid: {
                        top: '10%',
                        left: '3%',
                        right: '15%',
                        bottom: '16%',
                        containLabel: true
                    },
                    dataZoom: [
                        {
                            show: true,
                            xAxisIndex: 0,
                            start: 100,
                            end: 100 - percentOfDetailChartDataZoom,
                            height: '40%',
                            showDataShadow: false
                        }
                    ],
                    yAxis: {
                        type: 'value',
                        name: app.localize("Number"),
                        splitNumber: 1
                    },
                    xAxis: {
                        type: 'category',
                        name: app.localize("AlarmNumber"),
                        axisLabel: {
                            interval: 0
                        },
                        data: []
                    },
                    series: [
                        {
                            type: 'bar',
                            label: {
                                normal: {
                                    show: true,
                                    position: 'top'
                                }
                            },
                            data: []
                        }
                    ]
                };
                $("#AlarmStatisticDetailChart").height(originalDetailBarChartHeight);
                var chartDom = document.getElementById('AlarmStatisticDetailChart');
                var chartInstance = echarts.init(chartDom);
                var chartOption = option;
                if (chartInstance) {
                    echarts.dispose(chartDom);
                    chartInstance = echarts.init(chartDom);
                }

                var xAxisDatas = chartOption.xAxis.data;
                var seriesDatas = chartOption.series[0].data;
                result = result.reverse();
                for (var i = 0; i < result.length; i++) {
                    xAxisDatas.push(result[i].alarmCode);
                    seriesDatas.push(result[i].alarmCount);
                }

                chartOption.xAxis.data = xAxisDatas;
                chartOption.series[0].data = seriesDatas;
                chartInstance.setOption($.WIMI.echartOptionBuilder(chartOption));
            },

            clearAlarmDetail: function () {
                page.loadDetailPieChart([]);
                page.loadDetailBarChart([]);
                page.loadDetailTable([]);
            },

            loadDetailTable: function (result) {
                if ($AlarmStatisticDetailDataTable != null) {
                    $AlarmStatisticDetailDataTable.destroy();
                    $AlarmStatisticDetailDataTable = null;
                    $AlarmStatisticDetailTable.empty();
                }

                var totalAlarmCount = 0;

                for (var i = 0; i < result.length; i++) {
                    totalAlarmCount += parseInt(result[i].alarmCount);
                }

                $AlarmStatisticDetailDataTable = $AlarmStatisticDetailTable.WIMIDataTable({
                    serverSide: false,
                    scrollX: true,
                    scrollCollapse: true,
                    data: result,
                    ordering: false,
                    responsive: false,
                    columns: [
                        {
                            title: app.localize("Actions"),
                            width: "80px",
                            className: "text-center",
                            orderable: false,
                            render: function () {
                                return "";
                            },
                            data: null,
                            createdCell: function (td, cellData, rowData, row, col) {
                                classIndex++;
                                var classString = "btn" + classIndex;
                                $('<button class="btn btn-default btn-xs no-padding ' + classString + '">' + app.localize("Details") + '</button>')
                                    .appendTo($(td))
                                    .click(function () {
                                        var parameters = {
                                            startTime: lastQueryParams.startTime,
                                            endTime: lastQueryParams.endTime,
                                            statisticalWay: statisticalWay,
                                            machineIdList: [rowData.machineId],
                                            summaryDate: rowData.summaryDate,
                                            alarmCode: rowData.alarmCode,
                                            classString: classString
                                        }
                                        page.showAlarmDetailInfo(parameters);
                                    });
                            }
                        },
                        {
                            title: app.localize("AlarmNumber"),
                            width: "80px",
                            data: "alarmCode"
                        },
                        {
                            title: app.localize("NumberOfOccurrences"),
                            width: "80px",
                            data: "alarmCount"
                        },
                        {
                            title: app.localize("Proportion"),
                            width: "80px",
                            orderable: false,
                            data: function (item) {
                                return (item.alarmCount / totalAlarmCount * 100).toFixed(2) + "%";
                            }
                        },
                        {
                            title: app.localize("AlarmContent"),
                            data: "alarmMessage",

                        }

                    ]
                });
            },

            //显示报警详情
            showAlarmDetailInfo: function (parameters) {
                alarmsDetailModal.open({ parameters });
            },

            //设置查询参数显示
            setSearchFilter: function (filter) {
                $("#searchFilter").text(filter);
            },

            //构建以班次方案为分割的Tab
            constructShiftTabs: function (parameters) {
                $("#AlarmStatisticChart").empty();

                var tabData = [];
                for (var i = 0; i < parameters.machineShiftSolutionNameList.length; i++) {
                    var className = "";
                    if (i === 0) {
                        className = "active";
                    }

                    tabData.push({
                        className: className,
                        machineShiftSolutionId: parameters.machineShiftSolutionIdList[i],
                        machineShiftSolutionName: parameters.machineShiftSolutionNameList[i]
                    });
                }

                var source = $("#chart-template").html();
                var rendered = Handlebars.compile(source);
                $("#AlarmStatisticChart").html(rendered());
                var tabDateSource = $("#tab-date-template").html();
                var tabDateRender = Handlebars.compile(tabDateSource);

                $("#tabDate").html(tabDateRender({ dates: tabData }));
                $("#tabDate").scrollTabs({
                    click_callback: function () {
                        pageFirstLoad = true;
                        parameters.machineShiftSolutionIdList = [$(this).attr("value")];
                        parameters.machineShiftSolutionNameList = [$(this).text()];
                        alarmsService.getAlarmChartData(parameters).done(function (result) {
                            if (result !== null) {
                                var filter = parameters.statisticalName + parameters.startTime + "~" + parameters.endTime;
                                page.setSearchFilter(filter);
                                page.loadEChart(result, "AlarmStatisticShiftChart");
                            }
                        });
                    }
                });

                //默认加载一个tab的班次数据
                parameters.machineShiftSolutionIdList = [tabData[0].machineShiftSolutionId];
                parameters.machineShiftSolutionNameList = [tabData[0].machineShiftSolutionName];

                page.exportParam = parameters;
                alarmsService.getAlarmChartData(parameters).done(function (result) {
                    if (result !== null) {
                        var filter = parameters.statisticalName + parameters.startTime + "~" + parameters.endTime;
                        page.setSearchFilter(filter);
                        page.loadEChart(result, "AlarmStatisticShiftChart");
                    }
                });
            },

            changeEvent: function () {
                if (selectedParameters) {
                    var selectString = $("#selectString").val();
                    var chartString = $("#chartString").val();
                    selectedParameters.selectString = selectString;
                    selectedParameters.chartString = chartString;
                    alarmsService.getAlarmDetailData(selectedParameters).done(function (result) {
                        if (result != null) {
                            page.loadDetailTable(result);
                            if (chartString === "Pie") {
                                page.loadDetailPieChart(result);
                            } else {
                                page.loadDetailBarChart(result);
                            }
                        }
                    });
                }
            },
            exports: function () {
                var param = page.exportParam;

                if (page.exportParam == null) {
                    param = {
                        startTime: moment().format('YYYY-MM-DD'),
                        endTime: moment().format('YYYY-MM-DD'),
                        statisticalWay: "ByDay"
                    };
                }

                alarmsService.export(param).done(function (response) {
                    app.downloadTempFile(response);
                });
            },

            init: function () {

                var parameters = {
                    startTime: moment().format('YYYY-MM-DD'),//moment().format('YYYY-MM-DD'),
                    endTime: moment().format('YYYY-MM-DD'),
                    statisticalWay: "ByDay"
                };

                if (app.consts.fixedCalendar.enabled) {
                    parameters.startTime = app.consts.fixedCalendar.startTime;
                    parameters.endTime = app.consts.fixedCalendar.endTime;
                }
                lastQueryParams = parameters;
                alarmsService.getDefaultAlarmChartData(parameters).done(function (result) {
                    if (result != null) {
                        page.loadEChart(result, "AlarmStatisticChart");
                        var filter = app.localize("ByDay") + " : " + moment(parameters.startTime).format('YYYY-MM-DD') + "~" + moment(parameters.endTime).format('YYYY-MM-DD');
                        page.setSearchFilter(filter);
                        var machineIdList = [];
                        for (i = 0; i < result.length; i++) {
                            machineIdList.push(result[i].machineId);
                        }
                        lastQueryParams = {
                            startTime: moment().format('YYYY-MM-DD'),//moment().format('YYYY-MM-DD'),
                            endTime: moment().format('YYYY-MM-DD'),
                            statisticalWay: "ByDay",
                            machineIdList: machineIdList
                        };
                    }
                });



                //点击查询按钮
                $("#btnQuery").click(function () {
                    selectedParameters = null;
                    selectedMachine = null;
                    pageFirstLoad = true;
                    filterModal.open({}, page.setSearchParametersAndLoadBarEChart);
                });

                // 导出数据
                $("#btnExport").click(function () {
                    page.exports();
                });

                _$showName.on('click', function () {
                    pageFirstLoad = false;
                    lastQueryParams.machineShiftSolutionNameList = shiftSolutionNames;
                    lastQueryParams.machineShiftSolutionIdList = shiftSolutionIds;
                    page.setSearchParametersAndLoadBarEChart(lastQueryParams);
                });

                _$showValue.on('click', function () {
                    pageFirstLoad = false;
                    lastQueryParams.machineShiftSolutionNameList = shiftSolutionNames;
                    lastQueryParams.machineShiftSolutionIdList = shiftSolutionIds;
                    page.setSearchParametersAndLoadBarEChart(lastQueryParams);
                });

                //动态banding下拉框更改事件
                $(document).on("change", "#selectString", function () {
                    page.changeEvent();
                });

                $(document).on("change", "#chartString", function () {
                    page.changeEvent();
                });
            }
        };

        page.init();
    });
})();