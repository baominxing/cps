(function (wimi, echarts) {
    wimi.btl.staffPerformance = wimi.btl.staffPerformance || {};

    wimi.btl.staffPerformance.ProductionChart = function (containerId) {

        var _chart = echarts.init(document.getElementById(containerId));

        function generateLegendDatas(dataSource, legendField) {
            if (dataSource.length === 0) {
                return [];
            }
            var field = legendField;
            if (legendField == null) {
                field = "machineName";
            }

            var fields = _.map(_.uniq(dataSource, false, field), function (item) {
                return item[field];
            });

            return _.sortBy(fields);

        }

        function generateXAxisDatas(dataSource) {
            if (dataSource.length === 0) {
                return [];
            }
            var groupByDate = _.groupBy(dataSource, function (item) {
                return item.groupBy;
            });
            var xAxisData = _.allKeys(groupByDate);
            return xAxisData;
        }

        function generateSeries(dataSource, legendDatas, xAxisDatas, legendField) {
            var isShowName = $("#showName").is(":checked");
            var isShowValue = $("#showValue").is(":checked");
            var field = legendField;

            if (legendField == null) {
                field = "machineName";
            }

            var series = [];

            if (legendDatas.length === 0 || xAxisDatas.length === 0) {
                return series;
            }

            _.each(legendDatas, function (legend) {
                var datas = [];
                _.each(xAxisDatas, function (xaxis) {

                    var sourceDatas = _.filter(dataSource, function (source) {
                        return source[field] === legend
                            && source.groupBy === xaxis;
                    });

                    if (_.size(sourceDatas) > 0) {

                        var sum = _.reduce(sourceDatas, function (memo, item) {

                            return memo + item.sumYield;
                        }, 0);

                        datas.push(sum);

                    } else {
                        datas.push(null);
                    }

                });
                series.push({
                    name: legend,
                    type: 'bar',
                    data: datas,
                    itemStyle: {
                        normal: {
                            label: {
                                textStyle: {
                                    color: 'black'
                                },
                                show: true,
                                position: 'top',
                                align: 'left',
                                verticalAlign: 'middle',
                                fontSize: 12,
                                rotate: 90,
                                formatter: function (item) {

                                    if (isShowName && isShowValue) {
                                        return item.seriesName.split(',')[0] + "\r\n" + item.data;
                                    } else if (isShowName) {
                                        return item.seriesName.split(',')[0];
                                    } else if (isShowValue) {
                                        return item.data;
                                    } else {
                                        return "";
                                    }
                                }
                            }
                        }
                    }
                });
            });
            return series;
        }
       
        function buildChartOption(legendDatas, xAxisDatas, series) {

            return {
                tooltip: {
                    trigger: 'axis'
                },
                legend: {
                    data: legendDatas,
                    left: '50%',
                    right: '8%'
                },
                grid: {
                    left: '2%',
                    right: '8%',
                    bottom: '10%',
                    top: '20%',
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
                dataZoom: {
                    type: 'slider',
                    show: true,
                    height: 20,
                    start: 0,
                    end: 80
                },
                xAxis: {
                    type: 'category',
                    data: xAxisDatas
                },
                yAxis: {
                    type: 'value',
                    nameLocation: 'end',
                    name: app.localize("Unit") + ' (' + app.localize("Piece") + ')'
                }, series: series
            };
        }

        this.clear = function () {
            _chart.clear();
        };

        this.buildChart = function (dataSource, legendName) {

            var machineNames = generateLegendDatas(dataSource, legendName);
            var xAxisData = generateXAxisDatas(dataSource);
            var series = generateSeries(dataSource,
                machineNames, xAxisData, legendName);
            _chart.clear();
            var chartOption = buildChartOption(machineNames, xAxisData, series);

            console.log(chartOption);
            _chart.setOption($.WIMI.echartOptionBuilder(chartOption));
        };

    };

    wimi.btl.staffPerformance.ScrollTabs = function (
        $tabsContainer, idprefix, clickCallBack) {

        var _dataSource = [];
        var _$container = $tabsContainer;
        var _$tab = _$container.scrollTabs({
            click_callback: function () {

                var $this = $(this);

                var val = $(this).attr('rel');

                if ($this.hasClass('tabsDate_current_tab')) {
                    return;
                } else {
                    $('.tabsDate_current_tab').removeClass('tabsDate_current_tab');
                    $this.addClass('tabsDate_current_tab');
                }
                if (clickCallBack != null) {
                    clickCallBack(val);
                }
            }
        });

        var _idprefix = idprefix || "";

        this.selectFirstTab = function () {
            if (_dataSource.length == null) {
                throw new Error(app.localize("TheInitFunctionIsNotExecuted"));
            }

            if (_dataSource.length === 0) {
                throw new Error(app.localize("NoData"));
            }

            var firstTabId = '#' + _dataSource[0];
            $(firstTabId).click();
        };

        this.clearTabs = function () {
            _$tab.clearTabs();
        };
      
        this.init = function (dataSource, selectedId) {

            _dataSource = dataSource;

            _$tab.clearTabs();

            _.each(_dataSource, function (item, index) {
                var html = abp.utils.formatString('<span id="{3}{1}" rel="{1}" class="{2}">{0}</span>', item.name, item['value'], item['value'] === selectedId ? "tab_selected" : "", _idprefix);

                _$tab.addTab(html);
            });
          
        };
    };

})(wimi, echarts);

(function ($, abp, moment, _, wimi) {

    $(function () {
        var commonLookupAppService = abp.services.app.commonLookup;
        var staffPerformanceYieldService = abp.services.app.staffPerformanceYield;

        var _$groupTree = new MachinesTree();
        var option = ["types", "wholerow", "changed"];
        _$groupTree.setPluginsOption(option);
        var _$userScrollTab;
        var _$machineScrollTab;
        var _$userChart;
        var _$machineChart;

        var filterModal = new app.ModalManager({
            viewUrl: abp.appPath + "StaffPerformance/FilterModal2",
            scriptUrl: abp.appPath + "view-resources/Views/StaffPerformance/_FilterModal2.js",
            modalClass: "FilterModal2"
        });

        // 重置 授权设备组的id
        var getGrantedGroups = function (groups, grantedGroupIds) {

            function isGranted(group) {
                return _.contains(grantedGroupIds, group.id);
            }

            function parentGroup(group) {
                return _.find(groups, function (item) {
                    return item.id === group.parentId;
                });
            }

            var grantedGroups = _.filter(groups, isGranted);

            function getParentId(child, parent) {

                if (child.parentId == null) {
                    return null;
                }

                var parentIsGranted = isGranted(parent);

                // 父授权
                if (parentIsGranted) {
                    return parent.id;
                }

                // 子授权
                var childIsGranted = isGranted(child);

                if (childIsGranted) {
                    // 父的父
                    var p = parentGroup(parent);
                    return getParentId(parent, p);
                }
                return null;// 子没有授权
            }

            _.each(grantedGroups, function (item) {
                if (item.parentId == null) {
                    return;
                }
                var parent = parentGroup(item);
                item.parentId = getParentId(item, parent);
            });

            return grantedGroups;

        };

        var vmData = {
            userNames: [],
            users: [],
            machines: [],
            userSelectedId: 0,
            machineSelectedId: 0,
            tabName: 'user',
            type: "day",
            searchWithUserOrMachine: function (queryType) {
                var searchMachinIds = page.parameters.machineIdList;
                if (searchMachinIds.length === 0) {

                    emptyScrollTabsAndCharts();
                    return;
                }

                var start = page.parameters.startTime;
                var end = page.parameters.endTime;

                staffPerformanceYieldService.productionChart({
                    MachineIds: searchMachinIds,
                    DateRange: {
                        Type: getDayRangeType(),
                        StartDate: start,
                        EndDate: end
                    },
                    ByUserOrMachine: queryType,
                    UserId: vmData.userSelectedId,
                    MachineId: vmData.machineSelectedId,
                    ShiftSolutionName: page.parameters.shiftSolutionName
                }).done(function (result) {

                    _.each(result.userMachinShiftYields, function (item) {

                        var dt = moment(item.shiftDate);
                        var startDateStr = dt.format("YYYY-MM-DD") + '';

                        item.startDateStr = startDateStr + '';

                        var week = dt.week()<=9 ? "0" + dt.week() : dt.week();
                        item.weekStr = dt.year() + '-' + week + app.localize("Week");

                        item.monthStr = dt.format("YYYY-MM") + '';
                        item.quarterStr = dt.year() + dt.quarter() + app.localize("Season");
                        item.yearStr = dt.year() + '';

                        item.staffShift = startDateStr + " " + item.staffShiftName;
                        item.machineShift = startDateStr + " " + item.machineShiftName;
                        var type = page.parameters.statisticalWay;

                        switch (type) {

                            case 'day':
                                item.groupBy = item.startDateStr;
                                break;
                            case 'week':
                                item.groupBy = item.weekStr;
                                break;
                            case 'month':
                                item.groupBy = item.monthStr;
                                break;
                            case 'quarter':
                                item.groupBy = item.quarterStr;
                                break;
                            case 'year':
                                item.groupBy = item.yearStr;
                                break;
                            default:
                                item.groupBy = item.startDateStr;
                        }

                    });

                    if (queryType === "user") {
                        userChart(result.userMachinShiftYields);
                    } else if (queryType === "machine") {
                        machineChart(result.userMachinShiftYields);
                    }
                });

            },
            search: function (queryType) {

                var searchMachinIds = page.parameters.machineIdList;
                if (searchMachinIds.length === 0) {

                    emptyScrollTabsAndCharts();

                    return;
                }

                $.when(
                    staffPerformanceYieldService.allUsers(),
                    staffPerformanceYieldService.allMachines({
                        machineIds: searchMachinIds
                    }))
                    .done(function (users, machines) {

                        machineScollTab(machines[0]);
                        userScollTab(users[0]);

                        vmData.searchWithUserOrMachine(queryType);
                    });

            },
            $dateRange: null,
            changeTabName: function (name) {
                var self = this;
                vmData.tabName = name;

                self.userSelectedId = self.users[0].value;
                self.machineSelectedId = self.machines[0].value;
                self.searchWithUserOrMachine();

            },
            init: function () {
                var self = this;

                self.initDateRange();

                $.when(commonLookupAppService.getDeviceGroupAndMachineWithPermissions())
                    .done(function (result) {

                        var grantedGroupIds = result.grantedGroupIds;

                        var machines = _.filter(result.machines, function (item) {
                            return _.contains(grantedGroupIds, item.deviceGroupId);
                        });

                        self.deviceGroups = result.deviceGroups;

                        var grantedGroups = getGrantedGroups(result.deviceGroups, grantedGroupIds);

                        var groupsAndMachines = _.union(grantedGroups, machines);

                        _.each(grantedGroups, function (item) {
                            item.isGroup = true;
                        });
                        _.each(result.machines, function (item) {
                            item.isMachine = true;
                        });

                        var treeData = getTree(groupsAndMachines);

                        _$groupTree.initGroupWithJson($('#deviceGroupTree'),
                            treeData, true,
                            function () {
                                _$groupTree.setSelectGroupsTree();
                                vmData.search();
                            });
                    });
            },
            initUserNames: function () {

            },
            initDateRange: function () {
                var self = this;

                self.$dateRange = $("#daterange-btn");
                self.$dateRange.WIMIDaterangepicker({
                    startDate: moment(),
                    endDate: moment()
                });
            }
        };

        var vm = new Vue({
            el: "#app-yield",
            data: vmData,
            mounted: function () {
                $("#showName").on('click', function () {
                    vmData.searchWithUserOrMachine();
                });
                $("#showValue").on('click', function () {
                    vmData.searchWithUserOrMachine();
                });
                vmData.init();
            }
        });

        function buildUserScrollTab(clickCallback) {

            if (_$userScrollTab != null) {
                return _$userScrollTab;
            }
            var tab = new wimi.btl.staffPerformance.ScrollTabs(
                $('#userTabs'), "u", clickCallback);
            return tab;
        }

        function buildMachineScrollTab(clickCallback) {
            if (_$machineScrollTab != null) {
                return _$machineScrollTab;
            }
            var tab = new wimi.btl.staffPerformance.ScrollTabs(
                $('#machineTabs'), "m", clickCallback);
            return tab;
        }

        function buildUserChart() {
            if (_$userChart != null) {
                return _$userChart;
            }
            return new wimi.btl.staffPerformance.ProductionChart("chartByUser");
        }

        function buildMachineChart() {
            if (_$machineChart != null) {
                return _$machineChart;
            }
            return new wimi.btl.staffPerformance.ProductionChart("chartByMachine");
        }
        $("#showName").on('click',
            function () {
                vmData.searchWithUserOrMachine("user");
                vmData.searchWithUserOrMachine("machine");
            });
        $("#showValue").on('click',
            function () {
                vmData.searchWithUserOrMachine("user");
                vmData.searchWithUserOrMachine("machine");
            });
        function getDayRangeType() {

            var type = page.parameters.statisticalWay;

            switch (type) {

                case 'day':
                    return 2;
                case 'week':
                    return 3;
                case 'month':
                    return 4;
                case 'quarter':
                    return 5;
                case 'year':
                    return 6;
                case 'shift':
                    return 1;

                default:
                    return 0;
            }
        }

        function userScollTab(result) {

            _$userScrollTab = buildUserScrollTab(function (userId) {

                vmData.userSelectedId = userId * 1;

                vmData.searchWithUserOrMachine("user");
            });

            if (result.length === 0) {
                _$userScrollTab.clearTabs();
                return;
            }

            const isExist = _.contains(result, vmData.userSelectedId);

            if (!isExist) {
                vmData.userSelectedId = null;
            }

            if (vmData.userSelectedId == null) {
                vmData.userSelectedId = result[0].value;
            }
            vmData.users = result;
            _$userScrollTab.init(result, vmData.userSelectedId);

        }

        function machineScollTab(result) {

            _$machineScrollTab = buildMachineScrollTab(function (machineId) {
                vmData.machineSelectedId = machineId * 1;

                vmData.searchWithUserOrMachine("machine");
            });

            if (result.length === 0) {
                _$machineScrollTab.clearTabs();
                return;
            }


            vmData.machines = result;

            const isExist = _.contains(result, vmData.machineSelectedId);

            if (!isExist) {
                vmData.machineSelectedId = 0;
            }

            if (vmData.machineSelectedId == null || vmData.machineSelectedId === 0) {
                vmData.machineSelectedId = result[0].value;
            }

            _$machineScrollTab.init(result, vmData.machineSelectedId);

        }

        function emptyScrollTabsAndCharts() {
            if (_$userChart !== undefined) {
                _$userChart.clear();
            }

            if (_$machineChart !== undefined) {
                _$machineChart.clear();
            }

            machineScollTab([]);
            userScollTab([]);
        }

        function getTree(source) {

            function isGroup(data) {
                return data.deviceGroupId == null;
            }

            var treeData = _.map(source, function (item) {

                if (isGroup(item)) {

                    return {
                        id: 'g' + item.id,
                        icon: 'fa fa-object-group',
                        parent: item.parentId == null ? '#' : 'g' + item.parentId,
                        text: item.displayName,
                        state: {
                            selected: true,
                            opened: true
                        }, data: item
                    }

                }

                return {
                    id: 'm' + item.deviceGroupId + '_' + item.id,
                    icon: 'fa fa-circle-o',
                    parent: 'g' + item.deviceGroupId,
                    text: item.name,
                    state: {
                        selected: true,
                        opened: true
                    }, data: item
                }

            });

            return treeData;

        }

        function userChart(userDatas) {
            var type = page.parameters.statisticalWay;

            _$userChart = buildUserChart();

            // 如果是班次，则需要重新指定 group by
            if (type === "shift") {
                _.each(userDatas, function (item) {
                    item.groupBy = item.staffShift;
                });
            }
            var shortDatas = _.sortBy(userDatas, "groupBy");
            _$userChart.buildChart(shortDatas, "machineName");

        }

        function machineChart(datas) {
            var type = page.parameters.statisticalWay;
            //var shortDatas = _.sortBy(datas, "machineStartDate");
            _$machineChart = buildMachineChart();
            // 如果是班次，则需要重新指定 group by
            if (type === "shift") {
                _.each(datas, function (item) {
                    item.groupBy = item.machineShift;
                });
            }
            var shortDatas = _.sortBy(datas, "groupBy");
            _$machineChart.buildChart(shortDatas, "userName");

        }

        var page = {
            parameters: null,
            setSearchParametersAndLoadEchart: function (parameters) {
                page.parameters = parameters;

                var $searchFilter = $("#searchFilter");
                var $searchFilter2 = $("#searchFilter2");
                var showString = parameters.startTime + "~" + parameters.endTime + " " + parameters.statisticalWayText + " " + parameters.shiftSolutionName;
                $searchFilter.text(showString);
                $searchFilter2.text(showString);

                vmData.search("user");

                vmData.search("machine");
            },

            init: function () {
                var parameters = {
                    startTime: moment().format('YYYY-MM-DD'),
                    endTime: moment().format('YYYY-MM-DD'),
                    statisticalWay: "day",
                    statisticalWayText: app.localize("ByDay"),
                    machineIdList: [0],
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

})(jQuery, abp, moment, _, wimi);