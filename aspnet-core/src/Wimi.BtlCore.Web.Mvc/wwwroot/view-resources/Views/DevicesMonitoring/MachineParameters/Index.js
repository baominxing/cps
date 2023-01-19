(function () {
    $(function () {
        var $machineTree = $("div.machines-tree");
        var intervalid;
        var paramterService = abp.services.app.paramters;
        var chartLineDom = document.getElementById('chart-line');
        var blockPanelsource = $("#block-panel").html();
        var blockPanelrendered = Handlebars.compile(blockPanelsource);
        var alarmPanelsource = $("#alarm-panel").html();
        var alarmPanelrendered = Handlebars.compile(alarmPanelsource);

        var gauges = [];

        var tree = new MachinesTree();
        tree.init($machineTree, false, treeNodeChangedcallBackFunction);
        tree.setMachineTreeOpenAll();
        tree.selectFirst();


        function treeNodeChangedcallBackFunction(d) {
            if (d && d.node && d.node.data.hasOwnProperty("machineid")) {
                if (intervalid !== null) {
                    clearTimeout(intervalid);
                }
                gauges.length = 0;
                $(".gauge").find(".chart-gauge").remove();
                refreshData(d.node.data.machineid, true);
                intervalid = setInterval(function () {
                    refreshData(d.node.data.machineid, false);
                }, 5000);
            }
        }

        var chartLineOption;
        var chartInstance = echarts.init(chartLineDom);

        function refreshData(machineId, isInit) {
            paramterService.getParamtersList({ machineId: machineId })
                .done(function (result) {

                    if (result) {
                        populateHeaderInfo(machineId);
                        $("#blockMessage").empty();

                        if (isInit) {
                            chartInstance = echarts.getInstanceByDom(chartLineDom);
                            chartLineOption = getChartLineOption();
                            if (chartInstance) {
                                echarts.dispose(chartLineDom);
                                chartInstance = echarts.init(chartLineDom);
                            }
                        }

                        if (result.blockChartParamtersList && result.blockChartParamtersList.length > 0) {
                            $("#blockMessage").html(blockPanelrendered(result.blockChartParamtersList));
                        }

                        var seriesData = chartLineOption.series;
                        var legendData = chartLineOption.legend.data;
                        var xAxisData = chartLineOption.xAxis.data;

                        //if (isInit) {
                        //paramterService.getLastNRecords({ machineId: machineId }).done(function (result2) {

                        //    $.each(result.lineChartParamtersList, function (index, item) {
                        //        if ($.inArray(item.name, legendData) === -1) {
                        //            legendData.push(item.name);
                        //        }
                        //    });

                        //    for (var i = 0; i < result2.length; i++) {

                        //        xAxisData.push(result2[i].creationTime);

                        //        for (var j = 0; j < result2[i].lineChartParamtersList.length; j++) {

                        //            (function (n, m) {

                        //                var parameterName = result2[n].lineChartParamtersList[m].name;
                        //                var parameterValue = result2[n].lineChartParamtersList[m].value;

                        //                if (_.contains(legendData, parameterName) === false) {

                        //                    if (parameterValue === "--") {
                        //                        //abp.notify.error("参数" + parameterName + "的值" + parameterValue + "不是有效值");
                        //                        abp.log.error(app.localize("ParameterValueNotValid{0}{1}", parameterName, parameterValue));
                        //                    }
                        //                    else {
                        //                        if (_.where(seriesData, { "name": parameterName }).length === 0) {
                        //                            var node = {
                        //                                name: parameterName,
                        //                                type: 'line',
                        //                                data: [parameterValue]
                        //                            }
                        //                            seriesData.push(node);
                        //                        }
                        //                        else {
                        //                            for (var k = 0; k < seriesData.length; k++) {
                        //                                if (seriesData[k].name === parameterName) {
                        //                                    seriesData[k].data.push(parameterValue);
                        //                                    break;
                        //                                }
                        //                            }
                        //                        }
                        //                    }
                        //                }
                        //            })(i, j);

                        //        }
                        //    }

                        //    chartInstance.setOption($.WIMI.echartOptionBuilder(chartLineOption));
                        //});
                        //}
                        //else {
                        if (result.lineChartParamtersList && result.lineChartParamtersList.length > 0) {

                            var axisData = moment().format("HH:mm:ss");

                            $.each(result.lineChartParamtersList, function (index, item) {


                                if (_.contains(legendData, item.name) === false) {
                                    legendData.push(item.name);
                                }

                                if (_.where(seriesData, { "name": item.name }).length === 0) {

                                    if (item.value !== "--") {
                                        var node = {
                                            name: item.name,
                                            type: 'line',
                                            smooth: true,
                                            data: [item.value]
                                        }
                                        seriesData.push(node);
                                    }
                                }
                                else {
                                    for (var k = 0; k < seriesData.length; k++) {
                                        if (seriesData[k].name === item.name) {
                                            if (seriesData[k].data.length > 10) {
                                                seriesData[k].data.shift();
                                            }

                                            if (item.value !== "--") {
                                                seriesData[k].data.push(item.value);
                                            }
                                            else {
                                                abp.notify.error(app.localize("ParameterValueNotValid{0}{1}", item.name, item.value));
                                            }
                                            break;
                                        }
                                    }
                                }
                            });


                            chartLineOption.legend.data = legendData;
                            chartLineOption.series = seriesData;
                            if (chartLineOption.xAxis.data.length > 10) {
                                chartLineOption.xAxis.data.shift();
                            }

                            xAxisData.push(axisData);

                            if (isInit) {
                                xAxisData.push("");
                            }
                            chartLineOption.xAxis.data = xAxisData;
                            chartInstance.setOption($.WIMI.echartOptionBuilder(chartLineOption));
                            if (isInit) {
                                chartLineOption.xAxis.data.splice(1, 1);
                            }
                        }
                        //}

                        if (result.gaugeParamtersList && result.gaugeParamtersList.length > 0) {
                            var node;
                            var i;
                            if (isInit) {
                                for (i = 0; i < result.gaugeParamtersList.length; i++) {
                                    $(".gauge").append('<div class="col-md-3 col-sm-12 chart-gauge" id="chart-gauge_' + i + '" style="height: 140px"></div>');
                                    var parameterName = result.gaugeParamtersList[i].name;
                                    var chartGaugeDom = document.getElementById('chart-gauge_' + i + '');
                                    var chartGaugeOption = getChartGaugeOption();
                                    var chartGaugeInstance = echarts.init(chartGaugeDom);
                                    var parameterValue = result.gaugeParamtersList[i].value;

                                    node = {
                                        name: '',
                                        type: 'gauge',
                                        z: 3,
                                        splitNumber: 2,
                                        min: result.gaugeParamtersList[i].min,
                                        max: result.gaugeParamtersList[i].max,
                                        startAngle: 195,
                                        endAngle: -15,
                                        center: ['50%', '50%'],    // 默认全局居中
                                        radius: '100%',
                                        axisLine: {            // 坐标轴线
                                            lineStyle: {       // 属性lineStyle控制线条样式
                                                width: 5
                                            }
                                        },
                                        axisTick: {            // 坐标轴小标记
                                            length: 10,        // 属性length控制线长
                                            lineStyle: {       // 属性lineStyle控制线条样式
                                                color: 'auto'
                                            }
                                        },
                                        splitLine: {           // 分隔线
                                            length: 15,         // 属性length控制线长
                                            lineStyle: {       // 属性lineStyle（详见lineStyle）控制线条样式
                                                color: 'auto'
                                            }
                                        },
                                        title: {
                                            textStyle: {       // 其余属性默认使用全局文本样式，详见TEXTSTYLE
                                                fontWeight: 'bolder',
                                                fontSize: 5,
                                                fontStyle: 'italic'
                                            }
                                        },
                                        pointer: {
                                            width: 2
                                        },
                                        detail: {
                                            textStyle: {       // 其余属性默认使用全局文本样式，详见TEXTSTYLE
                                                fontWeight: 'bolder',
                                                fontSize: 10
                                            }
                                        },
                                        data: [{ value: parameterValue, name: parameterName }]
                                    };
                                    chartGaugeOption.series.push(node);
                                    chartGaugeInstance.setOption(chartGaugeOption);
                                    gauges.push({
                                        parameterName: parameterName,
                                        chartGaugeDom: chartGaugeDom,
                                        chartGaugeOption: chartGaugeOption,
                                        chartGaugeInstance: chartGaugeInstance
                                    });
                                }
                            }
                            else {
                                for (i = 0; i < result.gaugeParamtersList.length; i++) {

                                    parameterName = result.gaugeParamtersList[i].name;
                                    parameterValue = result.gaugeParamtersList[i].value;

                                    for (var j = 0; j < gauges.length; j++) {
                                        if (gauges[j].parameterName === parameterName) {

                                            node = {
                                                name: '',
                                                type: 'gauge',
                                                z: 3,
                                                splitNumber: 2,
                                                min: result.gaugeParamtersList[i].min,
                                                max: result.gaugeParamtersList[i].max,
                                                startAngle: 195,
                                                endAngle: -15,
                                                center: ['50%', '50%'],    // 默认全局居中
                                                radius: '100%',
                                                axisLine: {            // 坐标轴线
                                                    lineStyle: {       // 属性lineStyle控制线条样式
                                                        width: 5
                                                    }
                                                },
                                                axisTick: {            // 坐标轴小标记
                                                    length: 10,        // 属性length控制线长
                                                    lineStyle: {       // 属性lineStyle控制线条样式
                                                        color: 'auto'
                                                    }
                                                },
                                                splitLine: {           // 分隔线
                                                    length: 15,         // 属性length控制线长
                                                    lineStyle: {       // 属性lineStyle（详见lineStyle）控制线条样式
                                                        color: 'auto'
                                                    }
                                                },
                                                title: {
                                                    textStyle: {       // 其余属性默认使用全局文本样式，详见TEXTSTYLE
                                                        fontWeight: 'bolder',
                                                        fontSize: 5,
                                                        fontStyle: 'italic'
                                                    }
                                                },
                                                pointer: {
                                                    width: 2
                                                },
                                                detail: {
                                                    textStyle: {       // 其余属性默认使用全局文本样式，详见TEXTSTYLE
                                                        fontWeight: 'bolder',
                                                        fontSize: 10
                                                    }
                                                },
                                                data: [{ value: parameterValue, name: parameterName }]
                                            };
                                            gauges[j].chartGaugeOption.series.length = 0;
                                            gauges[j].chartGaugeOption.series.push(node);
                                            gauges[j].chartGaugeInstance.setOption(gauges[j].chartGaugeOption);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                });
        }

        function getChartLineOption() {
            return {
                tooltip: {
                    trigger: 'axis',
                    confine: true
                },
                legend: {
                    data: []
                },
                grid: {
                    left: '3%',
                    right: '4%',
                    bottom: '10%',
                    containLabel: true
                },
                toolbox: {
                    orient: 'vertical',
                    x: 'right',
                    y: 'center',
                    feature: {
                        saveAsImage: {}
                    }
                },
                xAxis: {
                    type: 'category',
                    boundaryGap: false,
                    axisLabel: {
                        interval: 0
                    },
                    data: []
                },
                yAxis: {
                    type: 'value'
                },
                series: []
            };
        }

        function getChartGaugeOption() {
            return {
                toolbox: {
                    show: true,
                    feature: {
                        saveAsImage: { show: false }
                    }
                },
                series: []
            };
        }

        function populateHeaderInfo(mcode) {
            paramterService.getMachineStatusDetail({ machineId: mcode })
                .done(function (result) {
                    $(".machinename").html(result.machine.name);
                    if (result.statusInfo !== null) {

                        $(".machinestate").css("background-color", result.statusInfo.hexcode);
                        $(".machinestate").html(result.statusInfo.displayName + "," + app.localize("ContinueMinutes{0}", result.mongoMachineInfo.statusDuration));
                    } else {
                        $(".machinestate").html("");
                    }

                    $(".machinealarm").find("h6").remove();

                    if (result.imagePath.length > 0) {
                        $(".img-circle").attr("src", result.imagePath);
                    } else {
                        $(".img-circle").attr("src", "/Content/Images/CNC1-128x128.png");
                    }

                    if (result.mongoMachineInfo.alarmItems.length > 0) {
                        $(".machinealarm").html(alarmPanelrendered(result.mongoMachineInfo.alarmItems));
                    }
                });
        }
    });
})();