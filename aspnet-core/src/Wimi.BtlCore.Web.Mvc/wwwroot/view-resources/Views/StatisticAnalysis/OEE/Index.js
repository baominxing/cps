(function () {
    $(function() {
        var $machineTree = new MachinesTree(),
            service = abp.services.app.oeeAnalysis;
        var commonService = abp.services.app.commonLookup;

        var seriesData = []; //所有折线
        var legendData = []; //所有图例

        var chart = {
            machineOEETemplate: $("#machineOEEtemplate").html(),
            detailDailyItemtemplate: $("#detailDailyItemtemplate").html(),
            chartObject: null,
            chartDom: document.getElementById("chart-line"),
            tendencyChartObject: null,
            tendencyChartDom: document.getElementById("detailDailyOee"),
            option: {
                tooltip: {
                    trigger: "axis"
                },
                legend: {
                    type: 'scroll',
                    data: [],
                    //animation: false,
                    scrollDataIndex: 1,
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
                    start: 0,
                    end: 100
                },
                xAxis: {
                    type: "category",
                    axisLabel: { interval: 0 },
                    boundaryGap: false,
                    data: []
                },
                yAxis: {
                    type: "value",
                    name: app.localize("Unit")+"(%)"
                },
                series: []
            },
            tendencyChartOption: {
                tooltip: {
                    trigger: "axis"
                },
                legend: {
                    show:false,
                    data: []
                },
                toolbox: {
                    orient: "vertical",
                    x: "right",
                    y: "center"
                },
                dataZoom: {
                    show: true,
                    realtime: true,
                    height: 20,
                    start: 0,
                    end: 100
                },
                xAxis: {
                    type: "category",
                    axisLabel: { interval: 0 },
                    boundaryGap: false,
                    data: []
                },
                yAxis: {
                    type: "value",
                    name: app.localize("Unit")+"(%)"
                },
                series: []
            },
            init: function(data, param) {
                this.disposeChart();
                var chartOption = this.option;
                chartOption.legend.data = _.unique(_.pluck(data.machineOee, 'machineName'));
                legendData = chartOption.legend.data;
                chartOption.xAxis.data = _.unique(_.pluck(data.shiftDayRanges, 'value'));
                chartOption.dataZoom.start = this.getPercentOfChartDataZoom(data);
                var oee = _.groupBy(data.machineOee, "machineName");
                chartOption.series = _.map(chartOption.legend.data,
                    function(item) {
                        return {
                            name: item,
                            type: 'line',
                            smooth: true,
                            data: _.pluck(oee[item], 'value')
                        };
                    });

                seriesData = chartOption.series;
                this.chartObject = echarts.init(chart.chartDom);
                //设置分页显示
                setPaging();
                //this.chartObject.setOption($.WIMI.echartOptionBuilder(chartOption));
                this.bindingChartClickEvent(data.shiftDayRanges, param);

                // 默认加载一笔记录到详细信息
                if (chartOption.series.length > 0) {
                    var serie = _.first(chartOption.series);
                    var point = {
                        seriesName: serie.name,
                        name: _.last(data.shiftDayRanges).value,
                        value: _.last(serie.data)
                    };

                    chart.initDetailOee(data.shiftDayRanges, param, point);
                }
            },
            initTendencyChart: function (data, param) {
                if (this.tendencyChartObject) {
                    this.tendencyChartObject.clear();
                    echarts.dispose(chart.tendencyChartDom);
                }
                var chartObject = this.tendencyChartOption;
                chartObject.xAxis.data = _.unique(_.pluck(data.shiftDayRanges, 'value'));
                chartObject.title = { text: param.machines.name + app.localize("TheTrendOfOEE") };
                chartObject.legend.data = _.unique(_.pluck(data.machineOee, 'machineName'));
                chartObject.dataZoom.start = this.getPercentOfChartDataZoom(data);

                var oee = _.groupBy(data.machineOee, "machineName");
                chartObject.series = _.map(chartObject.legend.data,
                    function (item) {
                        return {
                            name: item,
                            type: 'line',
                            smooth: true,
                            data: _.pluck(oee[item], 'value')
                        };
                    });

                this.tendencyChartObject = echarts.init(chart.tendencyChartDom);
                this.tendencyChartObject.setOption($.WIMI.echartOptionBuilder(chartObject));
            },
            disposeChart: function() {
                if (this.chartObject) {
                    echarts.dispose(chart.chartDom);
                }
            },
            getRender: function (template, data) {
                return Handlebars.compile(template);
            },
            getPercentOfChartDataZoom: function (data) {
                var percentOfChartDataZoom = 0;
                if (data.shiftDayRanges.length >= 40) {
                    percentOfChartDataZoom = 75;
                } else if (data.shiftDayRanges.length >= 20 && data.shiftDayRanges.length < 40) {
                    percentOfChartDataZoom = 40;
                }

                return percentOfChartDataZoom;
            },
            bindingChartClickEvent: function(shiftDayRanges, queryParam) {
                this.chartObject.on("click",
                    function(params) {
                        if (params.seriesType != null) {
                            chart.initDetailOee(shiftDayRanges, queryParam, params);
                        }
                    });
            },
            initDetailOee: function (shiftDayRanges, queryParam, clickparams) {
                var machine = _.find(queryParam.machines, { "name": clickparams.seriesName });
                var datelist = _.where(shiftDayRanges, { value: clickparams.name });
                if (datelist.length === 0) {
                    return false;
                }

                var startNode = _.first(datelist);
                var param = {
                    ShiftDay: startNode.value,
                    StartTime: startNode.name,
                    EndTime: _.last(datelist).name,
                    MachineIdList: [machine.id],
                    MachineId: machine.id,
                    machines:machine,
                    StatisticalWays:queryParam.StatisticalWays
                };

                service.getMachineOEEDetail(param).done(function (response) {
                    response.oee = clickparams.value;
                    var html = chart.getRender(chart.machineOEETemplate)(response);
                    $("#detailOee").empty().append(html);
                    $("#detailTitle").removeClass('hidden');
                    $("#shiftday").text(machine.name + " [" + param.ShiftDay + "]" + app.localize("TheOEEOfDetail"));
                });

                this.initDetailDailyItem(param);
            },
            initDetailDailyItem: function (param) {
                if (param.StatisticalWays === 'ByDay' || param.StatisticalWays === 'ByShift') {
                    $("#detailDailyOee").css("height", "auto");
                    service.getDetailDailyItem(param).done(function(response) {
                        chart.drawthTimeLine(param.StartTime, "#time-line");
                        var html = chart.getRender(chart.detailDailyItemtemplate)(response);
                        $("#detailDailyOee").empty().append(html);
                        $("div.td-gantt-chart").data('key', param.MachineId);
                        chart.drawStateGanttChart(response.ganttChart, "div.td-gantt-chart", false);
                    });
                } else {
                    $("#detailDailyOee").css("height", "250px");
                    service.listOeeDetailTendencyChart(param).done(function (response) {
                        // 补全数据
                        var data = [];
                        _.each(response.shiftDayRanges,
                            function(item) {
                                var d = _.find(response.machineOee, { machineId: param.machines.id, shiftDay: item.value });
                                var point = {
                                    machineId: param.machines.id,
                                    machineName: param.machines.name,
                                    shiftDay: item.value,
                                    value: d ? d.value : 0
                                };
                                if (_.where(data, point).length === 0) {
                                    data.push(point);
                                }
                            });


                        chart.initTendencyChart({
                                machineOee: data,
                                shiftDayRanges: response.shiftDayRanges
                            },
                            param);

                    }).fail(function () {
                        abp.ui.clearBusy();
                    });
                }
                
            },
            drawStateGanttChart: function(dataset, selector, showTitle) {
                $(selector).empty().StateGanttChart({ data: dataset, showTitle: showTitle });

                $(window)
                    .resize(function() {
                        $(selector).empty().StateGanttChart({ data: dataset, showTitle: showTitle });
                    });
            },
            drawthTimeLine: function(summaryDate, selector) {
                var $timeLine = $(selector);
                $timeLine.empty();
                var xScaleWidth = $timeLine.width();
                var xScaleHeight = '20px';

                var xScale = d3.time.scale()
                    .domain([moment(summaryDate + ' 00:00:01'), moment(summaryDate + ' 23:59:00')])
                    .range([0, xScaleWidth])
                    .clamp(true);

                var svg = d3.select(selector).append('svg')
                    .attr('width', xScaleWidth + 'px')
                    .attr('height', xScaleHeight)
                    .append('g');

                svg.append('g').attr('id', 'g_axis');

                var xAxis = d3.svg.axis()
                    .scale(xScale);

                svg.select('#g_axis').append('g')
                    .attr('class', 'axis')
                    .call(xAxis.orient('bottom'));
            }
        }

        var table = {
            $table: $("#EfficiencyTrendsTable"),
            datatables: null,
            init: function(data,param) {
                if (this.datatables) {
                    table.datatables.destroy();
                    table.$table.empty();
                }

                var columns = [];
                columns.push({ data: 'shiftDay', title: app.localize("StatisticalDate") });

                _.each(param.machines,
                    function(item) {
                        var key = {
                            data: item.id,
                            title: item.name
                        };
                        if (_.where(columns, key).length === 0) {
                            columns.push(key);
                        }
                    });

                var oee = _.groupBy(data.machineOee, "shiftDay");
                var values = _.map(_.unique(_.pluck(data.shiftDayRanges, 'value')),
                    function(item) {
                        var rowData = _.pluck(oee[item], 'machineId');
                        rowData.unshift("shiftDay");
                        var rowValue = _.pluck(oee[item], 'value');
                        rowValue.unshift(item);

                        return _.object(rowData, rowValue);
                    });


                this.datatables = table.$table.WIMIDataTable({
                    serverSide: false,
                    ordering: false,
                    scrollX: true,
                    order:[],
                    responsive: false,
                    data: values,
                    columns: columns
                });
            }
        };

        var page = {
            $datepicker: $("#daterange-btn"),
            init: function() {
                this.initDatepicker();
                this.initTree();                            
            },
            load: function () {
                this.clearDOM();
                chart.disposeChart();
                var param = this.getParamter();
                abp.ui.setBusy();
                service.listMachineOEEChart(param).done(function(response) {
                    // 补全数据
                    var data = [];
                        _.each(response.shiftDayRanges,
                            function(item) {
                                _.each(param.machines,
                                    function(key) {
                                        const d = _.find(response.machineOee, { machineId: key.id, shiftDay: item.value });
                                        const point = {
                                            machineId: key.id,
                                            machineName: key.name,
                                            shiftDay: item.value,
                                            value: d ? d.value : 0
                                        };
                                        if (_.where(data, point).length === 0) {
                                            data.push(point);
                                        }
                                    });
                            });

                        var result = {
                            machineOee: data,
                            shiftDayRanges: response.shiftDayRanges
                        };

                        table.init(result, param);
                        chart.init(result, param);
                        abp.ui.clearBusy();
                    }).fail(function() {
                        abp.ui.clearBusy();
                    });
            },
            getParamter: function() {
                var machines = [];

                _.each($machineTree.getSelectedNode(),
                    function(item) {
                        if (item.data.hasOwnProperty('machineid')) {
                            machines.push({ id: item.data.machineid, name: $.trim(item.text) });
                        } else {
                            return true;
                        }
                    });
                 
                var startTime = page.$datepicker.data("daterangepicker").startDate.format("YYYY-MM-DD");
                var endTime = page.$datepicker.data("daterangepicker").endDate.format("YYYY-MM-DD");
                var statistical = $("#statistical-way").val();

                return {
                    StartTime: startTime,
                    EndTime: endTime,
                    MachineIdList: _.unique(_.pluck(machines,'id')),
                    StatisticalWays: statistical,
                    QueryType: $machineTree.getQueryType(),
                    machines: machines//_.sortBy(machines, 'id')
                };
            },
            initDatepicker: function() {
                this.$datepicker.WIMIDaterangepicker({
                    startDate: moment().subtract(6, "days"),
                    endDate: moment()
                });
            },
            initTree: function() {
                $machineTree.init($("div.machines-tree"), true);
                $machineTree.selectMachineTree();
                $machineTree.openFirstMachineTreeNode();

                page.load();
                page.bindingQueryEvent();

                $("#statistical-way").select2({
                    multiple: false,
                    minimumResultsForSearch: -1,
                    language: {
                        noResults: function () {
                            return app.localize("NoMatchingData");
                        }
                    }
                });
            },
            bindingQueryEvent: function() {
                $("#btnQuery").click(function() {
                    page.load();
                });
            },
            clearDOM: function () {
                $("#shiftday").text('');
                $("#detailOee").empty();
                $("#time-line").empty();
                $("#detailDailyOee").empty();
                $("#detailTitle").addClass('hidden');
                $(".td-gantt-chart").empty();
            }
        }

        var showCount = 10; //一页显示数
        var index = 0; //当前页数据起始位置

        function setPaging() {
            index = 0;

            if (legendData.length <= showCount) {
                chart.option.legend.data = legendData;
                chart.option.series = seriesData;
                chart.chartObject.setOption($.WIMI.echartOptionBuilder(chart.option));
                $("#legend_page").hide();
            }
            else {

                $("#legend_page").show();

                updateChart();
            }

        }

        function updateChart() {
            chart.option.series = [];
            chart.option.legend.data = [];

            if (index == legendData.length) {
                index = 0;
            }

            for (var n = 0; n < showCount; n++) {
                if (index + n < legendData.length) {
                    chart.option.legend.data.push(legendData[index + n]);
                    chart.option.series.push(seriesData.find(item => item.name === legendData[index + n]));
                }
            }
            var totalPages = parseInt(legendData.length / showCount) + (legendData.length % showCount > 0 ? 1 : 0);
            $("#page_text").html(parseInt(index / showCount) + 1 + "/" + totalPages);

            index += chart.option.series.length;

            chart.chartObject.setOption($.WIMI.echartOptionBuilder(chart.option), {
                notMerge: true,
                lazyUpdate: false,
                silent: false
            });
        }

        //前一页
        $("#prePage").on("click",
            function () {

                if (index == legendData.length) {
                    index -= showCount + legendData.length % showCount;
                }
                else {
                    index -= showCount * 2;
                }
                index = index < 0 ? 0 : index;

                updateChart();
            });

        //后一页
        $("#nextPage").on("click",
            function () {
                updateChart();
            });

        $(".dataPane1").on("click",
            function () {
                if (legendData.length > showCount) {
                    $("#legend_page").show();
                }
            });

        $(".dataPane2").on("click",
            function () {
                $("#legend_page").hide();
            });

        page.init();
    });
})(jQuery)