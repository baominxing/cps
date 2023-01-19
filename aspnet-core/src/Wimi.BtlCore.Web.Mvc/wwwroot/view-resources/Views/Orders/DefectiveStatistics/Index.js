(function () {
    $(function () {
        var services = abp.services.app.defectiveStatistics;

        var queryObject = {
            $btnQuery: $("#btnQuery"),
            $daterange: $("#daterange-btn"),
            $productSelect: $("#Product"),
            init: function(callback) {
                this.initProduct(callback);
                this.initDatetimeRange();
            },
            initProduct: function(callback) {
                services.findProductList().done(function(response) {
                    var d = _.map(response,
                        function(item) {
                            return { id: item.id, text: item.name }
                        });

                    d.unshift({ id: 0, text: app.localize("All") });
                    queryObject.$productSelect.select2({
                        data: d
                    });

                    if (callback) {
                        callback();
                    }
                });
            },
            initDatetimeRange: function() {
                this.$daterange.WIMIDaterangepicker({
                    startDate: moment().subtract(6, "days"),
                    endDate: moment()
                });
            }
        };



        var chartbyMachine = {
            chartObject: null,
            option: {
                tooltip: {
                    trigger: 'axis'
                },
                legend: {
                    data: []
                },
                toolbox: {
                    show: true,
                    orient: 'vertical',
                    feature: {
                        dataView: { show: false, readOnly: false },
                        magicType: { show: true, type: ['line', 'bar'] },
                        restore: { show: true },
                        saveAsImage: { show: true }
                    }
                },
                dataZoom: [
                   {
                       show: true,
                       start: 0,
                       end: 100
                   }
                ],
                calculable: true,
                xAxis: [
                    {
                        type: 'category',
                        data: []
                    }
                ],
                yAxis: [
                    {
                        type: 'value'
                    }
                ],
                series: []
            },
            getechartsObject: function () {
                var chartDom = document.getElementById('chartlinebyMachine');
                this.chartObject = echarts.getInstanceByDom(chartDom);
                if (this.chartObject) {
                    echarts.dispose(chartDom);
                }
                this.chartObject = echarts.init(chartDom);
                return this.chartObject;
            },
            load: function (response) {
                var d = _.groupBy(response, "defectiveReasonName");
                var seriesData = [],
                    xAxisdata = [];
                _.each(d,
                    function (item, key) {
                        var yAxisData = [];
                        _.each(_.groupBy(item, "machineCode"),
                           function (k, name) {
                               if (!_.contains(xAxisdata, name)) {
                                   xAxisdata.push(name);
                               }
                               var count = _.reduce(_.pluck(k, 'count'), function (memo, num) { return memo + num; }, 0);
                               yAxisData.push(count);
                           });


                        seriesData.push({
                            name: key,
                            type: 'bar',
                            data: yAxisData,
                            stack: app.localize("Total")
                        });
                    });

                var option = chartbyMachine.option;
                option.legend.data = _.uniq(_.pluck(response, "defectiveReasonName"));
                option.xAxis[0].data =xAxisdata;

                //在最后一条绘制总数
                var lastSeriesData = _.last(seriesData);
                if (lastSeriesData) {
                    lastSeriesData.label = {
                        normal: {
                            show: true,
                            position: 'top',
                            textStyle: {
                                color: '#666'
                            },
                            formatter: function (params) {
                                var data = _.groupBy(response, "machineCode")[params.name];
                                var totalCount = 0;
                                if (data) {
                                    totalCount = _.reduce(_.pluck(data, 'count'), function (memo, num) { return memo + num; }, 0);
                                }
                                return app.localize("Total")+": " + totalCount;
                            }
                        }
                    };
                }
                option.series = seriesData;
                this.getechartsObject().setOption($.WIMI.echartOptionBuilder(option));
            }
        };


        var chartbyUser = {
            chartObject: null,
            option: {
                tooltip: {
                    trigger: 'axis'
                },
                legend: {
                    data: []
                },
                toolbox: {
                    show: true,
                    orient: 'vertical',
                    feature: {
                        dataView: { show: false, readOnly: false },
                        magicType: { show: true, type: ['line', 'bar'] },
                        restore: { show: true },
                        saveAsImage: { show: true }
                    }
                },
                dataZoom: [
                    {
                        show: true,
                        start: 0,
                        end: 100
                    }
                ],
                calculable: true,
                xAxis: [
                    {
                        type: 'category',
                        data: []
                    }
                ],
                yAxis: [
                    {
                        type: 'value'
                    }
                ],
                series: []
            },
            getechartsObject: function () {
                var chartDom = document.getElementById('chartlinebyUser');
                this.chartObject = echarts.getInstanceByDom(chartDom);
                if (this.chartObject) {
                    echarts.dispose(chartDom);
                }
                this.chartObject = echarts.init(chartDom);
                return this.chartObject;
            },
            load: function (response) {
                var d = _.groupBy(response, "defectiveReasonName");
                var seriesData = [];
                var xAxisdata = [];
                _.each(d,
                    function (item, key) {
                        var yAxisData = [];
                        _.each(_.groupBy(item, "userName"),
                            function (k, name) {
                                if (!_.contains(xAxisdata, name)) {
                                    xAxisdata.push(name);
                                }
                                var count = _.reduce(_.pluck(k, 'count'), function (memo, num) { return memo + num; }, 0);
                                yAxisData.push(count);
                            });


                        seriesData.push({
                            name: key,
                            type: 'bar',
                            data: yAxisData,
                            stack: app.localize("Total")
                        });

                    });

                //在最后一条绘制总数
                var lastSeriesData = _.last(seriesData);
                if (lastSeriesData) {
                    lastSeriesData.label = {
                        normal: {
                            show: true,
                            position: 'top',
                            textStyle: {
                                color: '#666'
                            },
                            formatter: function (params) {
                                var totalCount = 0;
                                var data = _.groupBy(response, "userName")[params.name];
                                if (data) {
                                    totalCount = _.reduce(_.pluck(data, 'count'),
                                        function (memo, num) { return memo + num; },
                                        0);
                                }
                                return app.localize("Total")+": " + totalCount;
                            }
                        }
                    };
                }

                var option = chartbyUser.option;
                option.legend.data = _.uniq(_.pluck(response, "defectiveReasonName"));
                option.xAxis[0].data =xAxisdata;
                option.series = seriesData;
                this.getechartsObject().setOption($.WIMI.echartOptionBuilder(option));
            }
        };


        var tableObject = {
            $table: $("#defectiveStatisticstable"),
            datatables: null,
            init: function () {
                queryObject.init(tableObject.load);
            },
            load: function () {
                var selectedNode = queryObject.$productSelect.select2('data');
                var param = {
                    productId: selectedNode.length === 0 ? null : selectedNode[0].id,
                    startTime: queryObject.$daterange.data("daterangepicker").startDate.format("YYYY-MM-DD"),
                    endTime: queryObject.$daterange.data("daterangepicker").endDate.format("YYYY-MM-DD")
                };
                services.defectiveStatisticsList(param).done(function (response) {
                    if (tableObject.datatables != null) {
                        tableObject.datatables.destroy();
                        tableObject.$table.empty();
                    }
                    tableObject.datatables = tableObject.$table.WIMIDataTable({
                        serverSide: false,
                        retrieve: true,
                        responsive: false,
                        scrollX: true,
                        ordering: false,
                        data: _.filter(response, function (item) { return item.count > 0 }),
                        columns: tableObject.getCloumns()
                    });

                    chartbyMachine.load(response);
                    chartbyUser.load(response);
                });
            },
            getCloumns: function () {
                return [
                    {
                        "data": "productCode",
                        "title": app.localize("ProductCode")
                    },
                    {
                        "data": "productName",
                        "title": app.localize("ProductName")
                    },
                    {
                        "data": "productionPlanCode",
                        "title": app.localize("ProductionPlanCode")
                    },
                    {
                        "data": "workOrderCode",
                        "title": app.localize("WorkOrderCode")
                    },
                    {
                        "data": "machineCode",
                        "title": app.localize("MachineCode")
                    },
                    {
                        "data": "machineName",
                        "title": app.localize("MachineName")
                    },
                    {
                        "data": "defectiveReasonCode",
                        "title": app.localize("ReasonCode")
                    },
                    {
                        "data": "defectiveReasonName",
                        "title": app.localize("ReasonName")
                    }, {
                        "data": "count",
                        "title": app.localize("DefectiveNumber")
                    },
                    {
                        "data": "userName",
                        "title": app.localize("Operator")
                    },
                    {
                        "data": "creationTime",
                        "title": app.localize("OperationTime"),
                        "render":function(data) {
                            return wimi.btl.dateTimeFormat(data);
                        }
                    }
                ];
            }
        };


        tableObject.init();

        queryObject.$btnQuery.click(function () {
            tableObject.load();
        });

    });
})();