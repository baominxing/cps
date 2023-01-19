(function () {
    $(function () {
        var deviceGroupService = abp.services.app.deviceGroup;
        var service = abp.services.app.qualifiedStatistics;
        var chartDom = document.getElementById("echarts-dom");

        var page = {
            $deviceGroup: $("#DeviceGroup"),
            $way: $("#statistical-way"),
            $datepicker: $("#daterange-btn"),
            $tables: $("#qualificationRateTable"),
            datatables: null,
            $tabDate: $("#tabDate"),
            chartOption: {
                legend: {
                    data: [] //legendsData
                },
                tooltip: {
                    trigger: 'axis'
                },
                grid: {
                    left: '2%',
                    right: '2%',
                    bottom: '2%',
                    top: '12%',
                    containLabel: true
                },
                dataZoom: {
                    show: true,
                    xAxisIndex: 0,
                    height: '5%',
                    top: '95%',
                    showDataShadow: false
                },
                toolbox: {
                    feature: {
                        saveAsImage: {}
                    }
                },
                xAxis: {
                    type: 'category',
                    boundaryGap: false,
                    data: [],

                    axisLabel: {
                        interval: 0,
                        rotate: 45, //倾斜度 -90 至 90 默认为0
                        margin: 8
                    }
                },
                yAxis: {
                    type: 'value',
                    axisLabel: {
                        show: true,
                        interval: 'auto',
                        formatter: '{value} %'
                    }
                },
                series: []
            },
            init: function () {
                this.initDatetime();
                this.initDeviceGroup();
                this.initStatisticalWay();
                this.bindingQueryEvent();
                this.bindingExportEvent();
                this.bindingSelect2Event();

            },
            initDeviceGroup: function () {
                deviceGroupService.listRootDevices()
                    .done(function (response) {
                        var data = _.map(response,
                            function (item) {
                                return { id: item.value, text: item.name };
                            });

                        page.$deviceGroup.select2({
                            data: data,
                            multiple: false,
                            minimumResultsForSearch: -1,
                            //placeholder: "请选择",
                            language: {
                                noResults: function () {
                                    return app.localize("NoMatchingData");
                                }
                            }
                        }).val($("#DeviceGroup").attr("value")).trigger('change');

                    });

                page.$deviceGroup
                    .on("change",
                        function () {
                            page.$tabDate.addClass('hidden');
                            page.$way.val("ByDay").trigger('change');
                        });
            },
            initDatetime: function () {
                this.$datepicker.WIMIDaterangepicker({
                    startDate: moment().subtract(6, "days"),
                    endDate: moment()
                });
            },
            initStatisticalWay: function () {
                this.$way.select2({ minimumResultsForSearch: -1 });
            },
            getParamter: function () {

                const statisticalWay = this.$way.val();
                var deviceGroupId = page.$deviceGroup.val();
                return {
                    startTime: page.$datepicker.data("daterangepicker").startDate.format("YYYY-MM-DD"),
                    endTime: page.$datepicker.data("daterangepicker").endDate.format("YYYY-MM-DD"),
                    deviceGroupIdList: deviceGroupId === "" ? [] : [deviceGroupId],
                    statisticalWay: statisticalWay,
                    ShiftSolutionIdList: []
                };
            },
            getColumns: function (data) {

                var colums = _.map(_.pluck(data.items, 'summaryDate'),
                    function (item) {
                        return {
                            title: item,
                            render: function (d) {
                                return d;
                            }
                        };
                    });
                colums.unshift({ "title": data.displayName }); //data.displayName

                return colums;
            },
            drawTable: function (data) {

                var tableRender = Handlebars.compile($("#table-template").html());
                $("#tableContent").html(tableRender({ data: data }));

                _.each(data,
                    function (key, index) {

                        const qualifiedOfflineCount = _.pluck(key.items, 'qualifiedOfflineCount');
                        qualifiedOfflineCount.unshift(app.localize("QualifiedOfflineNumber"));

                        const onlineCount = _.pluck(key.items, 'onlineCount');
                        onlineCount.unshift(app.localize("OnlineCount"));

                        const ngCount = _.pluck(key.items, 'ngCount');
                        ngCount.unshift(app.localize("NgCount"));

                        const processingCount = _.pluck(key.items, 'processingCount');
                        processingCount.unshift(app.localize("NumberOfProcessing"));

                        const qualifiedTableRate = _.pluck(key.items, 'qualifiedTableRate');
                        qualifiedTableRate.unshift(app.localize("PassRate"));

                        page.datatables = $("#qualificationRateTable" + index).WIMIDataTable({
                            serverSide: false,
                            responsive: false,
                            paging: false,
                            info: false,
                            scrollX: true,
                            ordering: false,
                            data: [onlineCount, qualifiedOfflineCount, ngCount, processingCount, qualifiedTableRate],
                            columns: page.getColumns(key),
                            order: []
                        });
                    });
            },
            drawCharts: function (data) {
                var chartInstance = echarts.getInstanceByDom(chartDom);
                if (chartInstance) {
                    echarts.dispose(chartDom);
                }
                chartInstance = echarts.init(chartDom);

                var chartByDateOption = this.chartOption;

                var legend = [];
                var series = [];
                var xAxis = [];

                xAxis = data.sumarryDateList;

                _.each(data.tableData,
                    function (key, index) {
                        legend.push(key.displayName);

                        var seriesData = [];
                        _.each(xAxis, function (xais) {
                            if (_.contains(_.pluck(key.items, 'summaryDate'), xais)) {

                                var ss = _.find(key.items, function (a) { return a.summaryDate === xais }).qualifiedEchartRate * 100;
                                var value = Math.round(parseFloat(ss) * 100) / 100
                                seriesData.push(value);
                            } else {
                                seriesData.push(0);
                            }
                        });
                        series.push({
                            name: key.displayName,
                            type: 'line',
                            smooth: true,
                            data: seriesData
                        });
                    });


                chartByDateOption.legend.data = legend;
                chartByDateOption.xAxis.data = xAxis;
                chartByDateOption.series = series;

                chartInstance.setOption($.WIMI.echartOptionBuilder(chartByDateOption, chartInstance));
            },
            scrollTabRender: function (data) {
                var solutionNameTabRender = Handlebars.compile($("#tab-date-template").html());
                this.$tabDate.removeClass('hidden');

                this.$tabDate.html(solutionNameTabRender({ dates: data }));
                this.$tabDate.scrollTabs({
                    click_callback: function () {
                        // 重新绘图
                        // 请求数据
                        var selectedId = $(this).data("id");

                        page.loadTableAndChart(selectedId);
                    }
                });
            },
            getShiftSolution: function () {
                service.listDeviceGroupSolution(this.getParamter()).done(function (result) {
                    if (page.$way.val() === "ByShift") {
                        page.scrollTabRender(result);

                        page.loadTableAndChart(result[0] ? result[0].machineShiftSolutionId : null);

                    } else {
                        this.$tabDate.addClass('hidden');
                    }
                });
            },
            loadTableAndChart: function (tabid) {

                abp.ui.setBusy();
                var param = page.getParamter();
                if (tabid) {
                    param.ShiftSolutionIdList = [tabid];
                }

                service.listQualificationInfo(param).done(function (result) {
                    page.drawCharts(result);
                    page.drawTable(result.tableData);

                }).always(function () {
                    abp.ui.clearBusy();
                });
            },

            bindingQueryEvent: function () {
                $("#btnQuery").on("click",
                    function (e) {
                        e.preventDefault();
                        var statisticalWay = $("#statistical-way").val();
                        if (statisticalWay === "ByShift") {
                            page.getShiftSolution();
                        } else {
                            page.$tabDate.addClass('hidden');
                            page.loadTableAndChart();
                        }
                    });
            },
            bindingExportEvent: function () {
                $("#btnExport").on("click",
                    function () {
                        var paramter = page.getParamter();
                        paramter.ShiftSolutionIdList = page.getSelected() ? [page.getSelected()] : [];

                        service.export(paramter).done(function (result) {
                            app.downloadTempFile(result);
                        });
                    });
            },
            bindingSelect2Event: function () {
                $("#statistical-way")
                    .on("change",
                        function () {
                            var statisticalWay = $("#statistical-way").val();
                            if (statisticalWay === "ByShift") {
                                page.getShiftSolution();
                            } else {
                                page.$tabDate.addClass('hidden');
                                page.loadTableAndChart();
                            }
                        });
            },
            getSelected: function () {
                var selected;
                _.each($("[data-id]"),
                    function (key) {
                        if ($(key).hasClass('tab_selected')) {
                            selected = $(key).data('id');
                        }
                    });

                return selected;
            }
        };
        page.init();
    });
})();
