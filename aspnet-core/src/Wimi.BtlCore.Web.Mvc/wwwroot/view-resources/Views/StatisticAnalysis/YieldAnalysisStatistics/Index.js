(function () {
    $(function () {
        var selectBtn = $("#SelectQueryConditions"),
            compareBtn = $("#CompareMachineYiledInfo"),
            $dateTabRender = Handlebars.compile($("#tab-date-template").html()),
            yieldService = abp.services.app.yield;

        //查询条件Modal
        var showSelectQueryConditionsModal = new app.ModalManager({
            viewUrl: abp.appPath + "YieldAnalysisStatistics/SelectQueryConditionsModal",
            scriptUrl: abp.appPath + "view-resources/Views/StatisticAnalysis/YieldAnalysisStatistics/_SelectQueryConditionsModal.js",
            modalClass: "ShowSelectQueryConditionsModal"
        });

        //设备比较Modal
        var showCompareMachineYieldModal = new app.ModalManager({
            viewUrl: abp.appPath + "YieldAnalysisStatistics/CompareMachineYieldModal",
            scriptUrl: abp.appPath + "view-resources/Views/StatisticAnalysis/YieldAnalysisStatistics/_CompareMachineYieldModal.js",
            modalClass: "ShowCompareMachineYieldModal",
            modalSize: "modal-lg"
        });

        //设备详情Modal
        var showDetailYieldModal = new app.ModalManager({
            viewUrl: abp.appPath + "YieldAnalysisStatistics/ShowDetailYieldModal",
            scriptUrl: abp.appPath + "view-resources/Views/StatisticAnalysis/YieldAnalysisStatistics/_ShowDetailYieldModal.js",
            modalClass: "ShowDetailYieldModal",
            modalSize: "modal-lg"
        });

        Handlebars.registerHelper('addTableClass',
            function (type) {
                switch (type) {
                    case "Less":
                        return new Handlebars.SafeString('<span class="description-percentage text-red pull-right"><i class="fa fa-long-arrow-down"></i></span>');
                    case "Equal":
                        return new Handlebars.SafeString('<span class="description-percentage text-yellow pull-right"><i class="fa fa-long-arrow-left"></i></span>');
                    case "More":
                        return new Handlebars.SafeString('<span class="description-percentage text-green pull-right"><i class="fa fa-long-arrow-up"></i></span>');
                    default:
                        return null;
                }
            });


        //表格
        var yieldTableObject = {
            getRender: function (template) {
                return Handlebars.compile(template);
            },
            //加载表格数据
            loadData: function (summaryDate, machineIdList) {
                abp.ui.setBusy();
                var parm = {
                    StartTime: summaryDate,
                    EndTime: summaryDate,
                    SummaryDate: summaryDate,
                    machineIdList: machineIdList
                };
                yieldService.getMachineYieldAnalysis(parm)
                    .done(function (response) {
                        //渲染加载table
                        var render = yieldTableObject.getRender($("#custom-table-template").html());
                        $("#customTable").empty().append(render(response));

                        yieldTableObject.bindPopover(summaryDate);
                        yieldTableObject.showStateGanttChart(summaryDate, machineIdList);

                    }).always(function () {
                        abp.ui.clearBusy();
                    });

            },
            //显示产量浮层
            showYieldTips: function (param) {
                var html;
                yieldService.getMachineAvgProgramDurationAndYield(param, { async: false })
                    .done(function (response) {
                        var render = yieldTableObject.getRender($("#td-yield-popover-template").html());
                        html = render(response);
                    });
                return html;
            },
            //显示设备利用率浮层
            showUtilizationTips: function (param) {
                var html, objs = [];
                yieldService.getMachineUtilizationRate4Popover(param, { async: false })
                    .done(function (response) {
                        var render = yieldTableObject.getRender($("#td-utilizationRate-popover-template").html());
                        _.each(response, function (key) {
                            objs.push({ Data: key.utilizationRate });
                        });
                        html = render(objs);
                    });
                return html;
            },
            //绑定鼠标hover时，弹出事件
            bindPopover: function (summaryDate) {
                $("td.td-popover").popover({
                    html: true,
                    placement: "auto",
                    container: 'body',
                    trigger: "hover",
                    content: function () {
                        var param = {
                            machineId: $(this).closest("tr").find('input[type="checkbox"]').val(),
                            summaryDate: summaryDate
                        }
                        if ($(this).data("type") === "yield") {
                            return yieldTableObject.showYieldTips(param);
                        } else {
                            return yieldTableObject.showUtilizationTips(param);
                        }
                    }
                });
            },
            //显示表格中的甘特图
            showStateGanttChart: function (summaryDate, machineIdList) {
                this.drawthTimeLine(summaryDate, "#time-line");

                yieldService.getMachineStatesGanttChart({
                    StartTime: summaryDate,
                    EndTime: summaryDate,
                    summaryDate: summaryDate,
                    machineIdList: machineIdList
                })
                    .done(function (response) {
                        var dataset = _.pluck(response, "chartDataList");
                        yieldTableObject.drawStateGanttChart(dataset, "div.td-gantt-chart", false);
                        abp.ui.clearBusy();
                    });


            },
            //绘制头部th 的时间轴
            drawthTimeLine: function (summaryDate, selector) {


                yieldService.getStartTimeOfGanttChart({ summaryDate: summaryDate })
                    .done(function (response) {
                        var $timeLine = $(selector);
                        var xScaleWidth = $timeLine.width();
                        var xScaleHeight = '20px';

                        var xScale = d3.time.scale()
                            .domain([moment(response.startTime), moment(response.endTime)])
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
                        abp.ui.clearBusy();
                    });
            },
            drawStateGanttChart: function (dataset, selector, showTitle) {
                $(selector).StateGanttChart({ data: dataset, showTitle: showTitle });

                $(window)
                    .resize(function () {
                        $(selector).empty().StateGanttChart({ data: dataset, showTitle: showTitle });
                    });
            },
            callBackfunction: function (option) {
                if (option) {
                    yieldTableObject.drawthTimeLine(option.SummaryDate, option.timelineSelector);
                    yieldTableObject.drawStateGanttChart(option.dataset, option.ganttChartSelector, true);
                }
            }
        }

        //日期选择tab
        var scrollTabObject = {
            $scrollTab: null,
            init: function (dateList, machineIdList) {
                if (this.$scrollTab !== null) {
                    this.$scrollTab.destroy();
                }

                this.$scrollTab = $("#tabsDate")
                    .html($dateTabRender({ dates: dateList }))
                    .scrollTabs({
                        click_callback: function () {
                            var $this = $(this);
                            scrollTabObject.loadData($this.text(), machineIdList);
                        }
                    });
                this.loadData($.trim($("#tabsDate .tab_selected").text()), machineIdList);
            },
            loadData: function (summaryDate, machineIdList) {
                yieldTableObject.loadData(summaryDate, machineIdList);
            }
        }

        //【按钮】选择查询条件
        selectBtn.click(function () {
            showSelectQueryConditionsModal.open({}, function (result) {
                result.statisticalWay = "ByDay";
                result.QueryType = 0;
                yieldService.getSummaryDate(result)
                    .done(function (response) {
                        scrollTabObject.init(_.pluck(response, "summaryDate"), result.MachineIdList);
                    });
            });
        });

        //【按钮】比较选择设备产量数据
        compareBtn.click(function () {
            if ($("input[type='checkbox']:checked").length <= 0) {
                layer.alert(app.localize("PleaseSelectAtLeastOneRecord")+"！");
                return false;
            }
            var param = {
                machineIdList: _.uniq(_.pluck($("input[type='checkbox']:checked"), "defaultValue")),
                summaryDate: $.trim($("#tabsDate .tab_selected").text()),
                callback: yieldTableObject.callBackfunction
            }
            showCompareMachineYieldModal.open({ param: param });

        });

        //【按钮】设备详情按钮
        $(document)
            .on("click",
            ".btn-search",
            function () {
                var machine = {
                    name: $.trim($(this).closest("tr").find('label').text()),
                    id: $(this).data("machineid"),
                    summaryDate: $.trim($("#tabsDate .tab_selected").text())
                };
                showDetailYieldModal.open({ machine: machine });
            });

        const parameters = {
            startTime: null,
            endTime: null
        };

        if (app.consts.fixedCalendar.enabled) {
            parameters.startTime = app.consts.fixedCalendar.startTime;
            parameters.endTime = app.consts.fixedCalendar.endTime;
        }

        //页面初始化时，自动加载数据
        yieldService.getFirstQueryParam(parameters)
            .done(function (response) {
                scrollTabObject.init(response.dataList, response.machineIdList);
            });
    });
})();
