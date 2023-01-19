(function () {
    $(function () {

        var EfficiencyTrendsService = abp.services.app.efficiencyTrends;

        var filterModal = new app.ModalManager({
            viewUrl: abp.appPath + "EfficiencyTrends/FilterModal",
            scriptUrl: abp.appPath + "view-resources/Views/StatisticAnalysis/EfficiencyTrends/_FilterModal.js",
            modalClass: "FilterModal"
        });

        var exportparam = {};

        var machine = []; //选择的设备或设备组
        var _$EfficiencyTrendsTable = $("#EfficiencyTrendsTable");
        var _$EfficiencyTrendsDataTables = null;
        var percentOfChartDataZoom = 0;
        var mediaTag = $.WIMI.getMediaTag();

        //根据查询条件绘制页面 parameters:查询条件;
        function setSearchParametersAndLoadBarEChart(parameters) {
            exportparam = parameters;
            setTitle(parameters.groupType === "2" ? "稼动率" : "运行率");
            var filter = parameters.statisticalName + parameters.startTime + "~" + parameters.endTime;
            setSearchFilter(filter);
            if (parameters.statisticalWays === "ByShift") {
                constructShiftTabs(parameters);
            } else {
                loadData(parameters, "EfficiencyTrendStatisticChart");
            }
        }

        //弹出查询框
        $("#btnQuery").click(function () {
            filterModal.open({}, setSearchParametersAndLoadBarEChart);
        });

        //获取图表数据  parameters:查询条件;  elementId:Ehcart容器ID;
        function loadData(parameters, elementId) {

            var queryType = parameters.queryType;
            machine = parameters.machine;

            if (_$EfficiencyTrendsDataTables !== null) {
                _$EfficiencyTrendsDataTables.destroy();
                _$EfficiencyTrendsDataTables = null;
                _$EfficiencyTrendsTable.empty();
            }

            var param = {
                startTime: parameters.startTime,
                endTime: parameters.endTime,
                statisticalWays: parameters.statisticalWays,
                statisticalName: parameters.statisticalName,
                machineId: parameters.machineId,
                machineName: parameters.machineName,
                machineShiftSolutionNameList: parameters.machineShiftSolutionNameList,
                groupType: parameters.groupType,
                queryType: parameters.queryType,
                machine: parameters.machine
            };
            EfficiencyTrendsService.getEfficiencyTrendasDataTablesColumns(param)
                .done(function (result) {
                    if (result) {
                        if (param.statisticalWays === "ByShift") {
                            var rList = _.filter(result, function (a) { return a.data !== "dimensions"; });
                            param.machineId = _.pluck(rList, 'data');
                            machine = param.machine = _.map(rList,
                                function (item) {
                                    return { id: item.data, name: item.title };
                                });
                        }
                        if (param.groupType === "2") {
                            EfficiencyTrendsService.getMachineActivation(param).done(function (data) {
                                var resultArray = [];
                                _.each(data.items, function (item) {
                                    resultArray.push(item.rateData);
                                });

                                drawChart(resultArray, elementId);
                                _$EfficiencyTrendsDataTables = _$EfficiencyTrendsTable.WIMIDataTable({
                                    serverSide: false,
                                    "scrollX": true,
                                    "responsive": false,
                                    order: [],
                                    data: resultArray,
                                    columns: result
                                });
                            });
                        }
                        else {
                            EfficiencyTrendsService.getEfficiencyTrendasList(param).done(function (data) {
                                var resultArray = [];
                                _.each(data.items, function (item) {
                                    resultArray.push(item.rateData);
                                });

                                drawChart(resultArray, elementId);
                                _$EfficiencyTrendsDataTables = _$EfficiencyTrendsTable.WIMIDataTable({
                                    serverSide: false,
                                    "scrollX": true,
                                    "responsive": false,
                                    order: [],
                                    data: resultArray,
                                    columns: result
                                });
                            });
                        }
                    }
                });
        }

        //绘制图表(被loadData调用)  data:图表数据;  elementId:Ehcart容器ID;
        function drawChart(data, elementId) {
            var chartDom = document.getElementById(elementId);
            percentOfChartDataZoom = 0;

            if (mediaTag === "lg" || mediaTag === "md") {
                _$EfficiencyTrendsTable.parent().removeClass("table-responsive");
            } else {
                _$EfficiencyTrendsTable.parent().addClass("table-responsive");
            }

            var chartInstance = echarts.getInstanceByDom(chartDom);
            if (chartInstance) {
                echarts.dispose(chartDom);
            }
            chartInstance = echarts.init(chartDom);

            var chartLineOption = {
                tooltip: {
                    trigger: "axis"
                },
                legend: {
                    data: []
                },
                grid: {
                    left: "3%",
                    right: "4%",
                    bottom: "10%",
                    containLabel: true
                },
                toolbox: {
                    orient: "vertical",
                    x: "right",
                    y: "center",
                    feature: {
                        saveAsImage: {},
                        restore: {}
                    }
                },
                dataZoom: {
                    show: true,
                    realtime: true,
                    height: 20,
                    start: percentOfChartDataZoom,
                    end: 100
                },
                xAxis: {
                    type: "category",
                    //axisLabel: { interval: 0 },
                    boundaryGap: false,
                    data: []
                },
                yAxis: {
                    type: "value",
                    name: app.localize("Unit") + "(%)"
                },
                series: []
            };

            var xAxisData = [];
            var seriesData = [];
            _.each(data, function (item) {
                xAxisData.push(item.dimensions);
                var list = _.omit(item, "dimensions");
                var count = 0;
                _.each(list, function (value, key) {
                    var b = seriesData.length >= _.size(list) ? seriesData[count].data : [];
                    b.push(value);
                    if (seriesData.length >= _.size(list)) {
                        seriesData[count].data = b;
                    } else {
                        var node = {
                            name: _.where(machine, { id: key })[0].name,
                            type: "line",
                            smooth: true,
                            data: b
                        };
                        seriesData.push(node);
                    }
                    count++;
                });
            });


            chartLineOption.legend.data = _.pluck(machine, "name");
            chartLineOption.series = seriesData;
            chartLineOption.xAxis.data = xAxisData;

            chartInstance.setOption($.WIMI.echartOptionBuilder(chartLineOption, chartInstance));
        }

        //构建以班次方案为分割的Tab  parameters:查询条件;
        function constructShiftTabs(parameters) {
            $("#EfficiencyTrendStatisticChart").empty();

            var tabData = [];
            for (var i = 0; i < parameters.machineShiftSolutionNameList.length; i++) {
                var className = "";
                if (i === 0) {
                    className = "active";
                }

                tabData.push({
                    className: className,
                    machineShiftSolutionName: parameters.machineShiftSolutionNameList[i]
                });
            }

            var source = $("#chart-template").html();
            var rendered = Handlebars.compile(source);
            $("#EfficiencyTrendStatisticChart").html(rendered());
            var tabDateSource = $("#tab-date-template").html();
            var tabDateRender = Handlebars.compile(tabDateSource);

            $("#tabDate").html(tabDateRender({ dates: tabData }));
            $("#tabDate").scrollTabs({
                click_callback: function () {
                    parameters.machineShiftSolutionNameList = [$(this).text()];
                    loadData(parameters, "EfficiencyTrendStatisticShiftChart");
                }
            });

            //默认加载一个tab的班次数据
            parameters.machineShiftSolutionNameList = [tabData[0].machineShiftSolutionName];
            loadData(parameters, "EfficiencyTrendStatisticShiftChart");
        }

        //设置查询参数显示
        function setSearchFilter(filter) {
            $("#searchFilter").text(filter);
        }

        //设置标题显示
        function setTitle(filter) {
            $("#titleFilter").text(filter);
        }

        //页面加载的时候默认显示一周内所有设备的稼动率 按天
        function pageLoad() {
            var startTime = moment().subtract(6, "days").format('YYYY-MM-DD');//moment().format('YYYY-MM-DD'),
            var endTime = moment().format('YYYY-MM-DD');
            var machine = [];
            var machineIdList = [];
            var machineNameList = [];
            var groupType = "2";
            var statisticalWay = "ByDay";
            var machineShiftSolutionNameList = [];
            var statisticalName = app.localize("ByDay") + ":";
            var queryType = "0";

            EfficiencyTrendsService.getDefaultMachines().done(function (result) {
                if (result !== null) {
                    _.each(result, function (item) {
                        machineNameList.push(item.value + "$" + $.trim(item.name));
                        machineIdList.push(item.value);
                        machine.push({
                            id: item.value + "",
                            name: $.trim(item.name)
                        });
                    });

                    var parameters = {
                        startTime: startTime,
                        endTime: endTime,
                        statisticalWays: statisticalWay,
                        statisticalName: statisticalName,
                        machineId: _.unique(machineIdList),
                        machineName: _.unique(machineNameList).join(","),
                        machineShiftSolutionNameList: machineShiftSolutionNameList,
                        groupType: groupType,
                        queryType: queryType,
                        machine: machine
                    };

                    exportparam = parameters;

                    setSearchParametersAndLoadBarEChart(parameters);
                }
            });
        };

        //页面加载
        pageLoad();


        $("#btnExport").on("click",
            function () {
                var paramter = exportparam;

                EfficiencyTrendsService.export(paramter).done(function (result) {
                    app.downloadTempFile(result);
                });
            });

    });
})();