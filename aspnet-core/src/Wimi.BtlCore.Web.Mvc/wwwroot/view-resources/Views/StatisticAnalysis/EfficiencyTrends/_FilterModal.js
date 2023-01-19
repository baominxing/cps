//# sourceURL=dynamic_FilterModal.js
(function () {
    app.modals.FilterModal = function () {
        var EfficiencyTrendsService = abp.services.app.efficiencyTrends,
            _$modalManager,
            _$EfficiencyTrendsFilterForm,
            _$timeRange = $("#daterange-btn"),
            _$machineTree = new MachinesTree();

        var machines = []; //班次对应的设备
        var machineShiftSolutions = [];  //设备班次方案

        this.init = function (modalManager) {
            _$modalManager = modalManager;
            _$EfficiencyTrendsFilterForm = _$modalManager.getModal().find('form[name=EfficiencyTrendsFilterForm]');

            _$timeRange.WIMIDaterangepicker({
                startDate: moment().subtract(6, "days"),
                endDate: moment()
            });

            _$machineTree.init($("div.machines-tree"), true, machineTreeChangedCallBack);
            _$machineTree.initGroup($("div.machines-group-tree"));
            _$machineTree.setSelectAll();


            $("#checkbox-shift").select2({
                data: [],
                multiple: true,
                placeholder: app.localize("PleaseSelect"),
                language: {
                    noResults: function () {
                        return app.localize("NoMatchingData");
                    }
                }
            });

        };

        this.shown = function () {
            $("#statisticalWay").select2({
                multiple: false,
                minimumResultsForSearch: -1,
                language: {
                    noResults: function () {
                        return app.localize("NoMatchingData");
                    }
                }
            });
            $("#groupType").select2({
                multiple: false,
                minimumResultsForSearch: -1,
                language: {
                    noResults: function () {
                        return app.localize("NoMatchingData");
                    }
                }
            });
            
        }

        this.save = function () {
            var parameters = getSearchParameters(_$machineTree.getQueryType());

            if (parameters.statisticalWays === "ByShift" && $("#checkbox-shift").val() === null) {
                abp.notify.error("请至少选择一个班次");
                return;
            }

            _$modalManager.setResult(parameters);
            _$modalManager.close();
        };

        //获取查询参数, qt: queryType:设备/设备组
        function getSearchParameters(qt) {
            var startTime = null;
            var endTime = null;
            var machine = [];
            var machineIdList = [];
            var machineNameList = [];
            var groupType = $("#groupType").val();
            var statisticalWay = $("#statisticalWay").val();
            var machineShiftSolutionNameList = [];
            var statisticalName = "";
            var queryType = qt;

            if (queryType === "0") {
                _.each(_$machineTree.getSelectedNode(), function (item) {
                    if (item.data.hasOwnProperty('machineid')) {
                        machineNameList.push(item.data.machineid + "$" + $.trim(item.text));
                        machineIdList.push(item.data.machineid);
                        machine.push({
                            id: item.data.machineid.toString(),
                            name: $.trim(item.text)
                        });
                    } else {
                        return true;
                    }
                });
            } else {
                _.each(_$machineTree.getSelectedGroupNode(), function (item) { 
                    machineNameList.push(item.id + "$" + $.trim(item.text));
                    machineIdList.push(item.id);
                    machine.push({
                        id: item.id.toString(),
                        name: $.trim(item.text)
                    });
                });
            }

            if (statisticalWay === "ByDay") {
                statisticalName = app.localize("ByDay") + ":";
                startTime = _$timeRange.data("daterangepicker").startDate.format("YYYY-MM-DD");
                endTime = _$timeRange.data("daterangepicker").endDate.format("YYYY-MM-DD");
            } else if (statisticalWay === "ByWeek") {
                statisticalName = app.localize("ByWeek") + ":";
                startTime = _$timeRange.data("daterangepicker").startDate.format("YYYY-MM-DD");
                endTime = _$timeRange.data("daterangepicker").endDate.format("YYYY-MM-DD");
            }else if (statisticalWay === "ByMonth") {
                statisticalName = app.localize("ByMonth") + ":";
                startTime = _$timeRange.data("daterangepicker").startDate.format("YYYY-MM-DD");
                endTime = _$timeRange.data("daterangepicker").endDate.format("YYYY-MM-DD");
            } else if (statisticalWay === "ByYear") {
                statisticalName = app.localize("ByYear") + ":";
                startTime = _$timeRange.data("daterangepicker").startDate.format("YYYY-MM-DD");
                endTime = _$timeRange.data("daterangepicker").endDate.format("YYYY-MM-DD");
            } else if (statisticalWay === "ByShift") {
                statisticalName = app.localize("ByShift") + ":";
                startTime = _$timeRange.data("daterangepicker").startDate.format("YYYY-MM-DD");
                endTime = _$timeRange.data("daterangepicker").endDate.format("YYYY-MM-DD");
                var selectedDatas = $('#checkbox-shift').select2("data");
                machineShiftSolutionNameList = _.pluck(selectedDatas, "text");
            }

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
                machine:machine
            };

            return parameters;
        }


        //填充班次方案Selec2 qt: queryType:设备/设备组
        function setMachineShiftSolutions(qt) {
            $("#checkbox-shift").select2().empty();

            var parameters = getSearchParameters(qt);

            machineShiftSolutions = [];
            EfficiencyTrendsService.getMachineShiftSolutions(parameters).done(function (result) {
                if (result !== null) {
                    var data = [];
                    machines = result;
                    for (var i = 0; i < result.length; i++) {
                        (function (n) {
                            if ($.grep(machineShiftSolutions, function (v) { return v === result[n].machineShiftSolutionName; }).length === 0) {
                                machineShiftSolutions.push(result[n].machineShiftSolutionName);
                                data.push({ id: n, text: result[n].machineShiftSolutionName });
                            }
                        })(i);
                    }

                    $("#checkbox-shift").select2({
                        data: data
                    });

                    //默认选择全部
                    var ids = _.pluck(data, "id");
                    $("#checkbox-shift").select2().val(ids).trigger("change");
                }
            });
        }

        //设备树选择变换事件
        function machineTreeChangedCallBack() {
            var statisticalWay = $("#statisticalWay").val();
            if (statisticalWay === "ByShift") {
                setMachineShiftSolutions(_$machineTree.getQueryType());
            }
        }

        //日期改变事件
        $("#daterange-btn").on("change",
                function () {
                    var statisticalWay = $("#statisticalWay").val();
                    if (statisticalWay === "ByShift") {
                        setMachineShiftSolutions(_$machineTree.getQueryType());
                    }
                });

        //时间维度改变事件
        $("#statisticalWay").on("change",
                function () {
                    var statisticalWay = $("#statisticalWay").val();
                    if (statisticalWay === "ByShift") {
                        $(".input-shift").removeAttr("style");
                        setMachineShiftSolutions(_$machineTree.getQueryType());
                    } else {
                        $(".input-shift").css('display', 'none');
                    }
            });

        //设备/设备组切换事件
        $(".nav-tabs li").on("click",
            function () {
                var statisticalWay = $("#statisticalWay").val();
                if (statisticalWay === "ByShift") {
                    $(".input-shift").removeAttr("style");
                    var queryType = $(this).closest("a").context.innerText.trim() === "设备分组" ? "1" : "0";
                    setMachineShiftSolutions(queryType);
                } else {
                    $(".input-shift").css('display', 'none');
                }
            });

        //班次方案Select2 光标事件
        $(document).on("mouseover", ".select2-selection__choice", function () {
            $(this).css("cursor", "pointer");
            var machineShiftSolutionName = $(this).attr("title");
            var machineNameString = app.localize("MachineList") + ":\r\n";
            for (var i = 0; i < machines.length; i++) {
                if (machines[i].machineShiftSolutionName === machineShiftSolutionName) {
                    machineNameString += machines[i].machineName + ",\r\n";
                }
            }
            layer.tips(machineNameString, this);
        });

        $(document).on("mouseout", ".select2-selection__choice", function () {
            $(this).css("cursor", "default");
            layer.closeAll();
        });
    }
})(jQuery);