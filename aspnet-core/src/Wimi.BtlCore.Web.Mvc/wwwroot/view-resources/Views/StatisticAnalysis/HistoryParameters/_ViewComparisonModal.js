(function () {
    app.modals.ViewComparisonModalModal = function () {
        var _modalManager,
            _args,
            service = abp.services.app.paramters,
            commonService = abp.services.app.commonLookup;

        var page = {
            $dom: $("#paramChartline").get(0),
            $machineSelect: $("#machineId"),
            $paramSelect: $("#paramterCode"),
            init: function () {
                this.initMachines(_args.MachineId);
                this.bindingQueryEvent();
            },
            initMachines: function (defaultMachineId) {
                commonService.getDeviceGroupAndMachineWithPermissions().done(function (response) {
                    var machines = _.chain(response.machines)
                        .filter(
                            function (item) {
                                return _.contains(response.grantedGroupIds, item.deviceGroupId);
                            })
                        .map(function (item) {
                            var machine = {
                                id: item.id,
                                text: item.name
                            };
                            return machine;
                        });

                    var machinesData = [];
                    _.each(_.groupBy(machines._wrapped, "id"),
                        function (item) {
                            machinesData.push(item[0]);
                        });

                    page.$machineSelect.select2({
                        data: machinesData,
                        multiple: false,
                        placeholder: "请选择设备",
                        language: {
                            noResults: function () {
                                return "请维护设备";
                            }
                        }
                    }).val(defaultMachineId).trigger('change');

                    page.initParamters(defaultMachineId);
                });
            },
            initParamters: function (defaultMachineId) {
                service.listNumberParamters({ id: defaultMachineId}).done(function (response) {
                    var param = _.map(response,
                        function (item) {
                            return {
                                id: item.value,
                                text: item.name
                            }
                        });

                    var defaultValue = _.first(param);
                    page.$paramSelect.select2({
                        data: param,
                        multiple: true,
                        placeholder: "请选择参数",
                        language: {
                            noResults: function () {
                                return "请维护参数";
                            }
                        }
                    }).val(defaultValue.id).trigger('change');
                });

            },
            load: function () {
                var param = {
                    MachineId: page.$machineSelect.val(),
                    ParamCodes: page.$paramSelect.val(),
                    StartTime: _args.startTime,
                    EndTime: _args.endTime
                };

                if (!param.MachineId) {
                    abp.message.error("请选择设备！");
                    return false;
                }

                if (!param.ParamCodes) {
                    abp.message.error("请至少选择一个参数！");
                    return false;
                }

                $("#paramChartline").css({height:400});

                abp.ui.setBusy(); 
                service.listParamComparisonChart(param).done(function (response) {
                    if (!response) {
                        return false;
                    }

                    var creationTimes = response.creationTimes;
                    var chartOption = page.option;
                   
                    chartOption.legend.data = _.pluck(response.items, 'name');
                    chartOption.xAxis.data = creationTimes;
                    chartOption.dataZoom[0].startValue = _.first(creationTimes);
                    var series = []; 

                    _.each(response.items,
                        function(key,index) {
                            const point = {
                                name: key.name,
                                type: 'line', 
                                smooth: true,
                                data: key.paramValues
                            };

                            series.push(point); 
                        });

                    chartOption.series = series; 

                    try {
                        var chartInstance = echarts.getInstanceByDom(page.$dom);
                        if (chartInstance) {
                            echarts.dispose(page.$dom);
                        }
                        chartInstance = echarts.init(page.$dom);
                        chartInstance.setOption(chartOption);
                        chartInstance.on('datazoom', function (params) {
                            $("#paramChartline").removeAttr("style");

                        });
                    } catch (e) {
                        abp.ui.clearBusy();
                    } 

                }).fail(function(data) {
                    abp.message.error(data.message);
                    _modalManager.close();
                    return false;

                }).always(function() {
                    abp.ui.clearBusy();
                    $("#paramChartline").removeAttr("style");
                });               
            },
            bindingQueryEvent: function () {
                $("#btnParamQuery").click(function (e) {
                    page.load();
                });
            },
            option: {
                tooltip: {
                    trigger: 'axis'
                },
                legend: {
                    data: []
                },
                grid: {
                    left: '3%',
                    right: '4%',
                    containLabel: true
                },
                dataZoom: [
                    { startValue: '' },
                    {
                        type: 'inside'
                    }],
                xAxis: {
                    type: 'category',
                    boundaryGap: false,
                    data: []
                },
                yAxis: {
                    type: 'value'
                },
                visualMap:[],
                series: []
            }
        };

        this.init = function (modalManager, args) {
            _modalManager = modalManager;
            _args = args;

            page.init();
        }
    }
})(jQuery);