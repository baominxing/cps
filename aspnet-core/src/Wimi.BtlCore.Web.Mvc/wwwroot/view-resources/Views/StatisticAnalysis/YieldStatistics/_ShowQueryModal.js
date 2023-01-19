(function () {
    app.modals.ShowQueryModal = function () {
        var _$machineTree = new MachinesTree(),
            _$modalManager,
            alarmsService = abp.services.app.alarms,
            _$timeRange = $("#reservationtime");
        var commonService = abp.services.app.commonLookup;
        this.init = function (modalManager) {
            _$modalManager = modalManager;

            _$timeRange.WIMIDaterangepicker({
                startDate: moment().subtract(6, "days"),
                endDate: moment()
            });

            commonService.defaultSelectedMachineCount().done(function (count) {
                _$machineTree.init($("div.machines-tree"), true, machineTreeChangedCallBack, count);
                _$machineTree.initGroup($("div.machines-group-tree"));
                _$machineTree.setSelectAll();
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
        }

        function getSearchParameters() {

            var machineIdList = _$machineTree.getSelectedMachineIds();
            var statisticalWay = $("#statisticalWay").val();
            var machineShiftSolutionNameList = [];
            var machineShiftSolutionIdList = [];
            var statisticalName = "";

            if (statisticalWay === "ByDay") {
                statisticalName = app.localize("ByDay") + ":";

            } else if (statisticalWay === "ByMonth") {
                statisticalName = app.localize("ByMonth") + ":";

            } else if (statisticalWay === "ByShift") {
                statisticalName = app.localize("ByShift") + ":";

                var shiftSelected = $('#checkbox-shift').select2("data");
                machineShiftSolutionNameList = _.pluck(shiftSelected, "text");
                machineShiftSolutionIdList = _.pluck(shiftSelected, "id");
            };

            const parameters = {
                queryMethod: _$machineTree.getQueryType(),
                startTime: _$timeRange.data("daterangepicker").startDate.format("YYYY-MM-DD"),
                endTime: _$timeRange.data("daterangepicker").endDate.format("YYYY-MM-DD"),
                statisticalWay: statisticalWay,
                statisticalName: statisticalName,
                machineIdList: machineIdList,
                deviceGroupIdList: _$machineTree.getSelectedGroupIds(),
                machineShiftSolutionIdList: machineShiftSolutionIdList,
                machineShiftSolutionNameList: machineShiftSolutionNameList
            };

            return parameters;
        }

        var machineShiftSolutions = [];
        var machines = [];

        function setMachineShiftSolutions() {
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
            $("#checkbox-shift").select2().empty();
            machineShiftSolutions = [];
            var parameters = getSearchParameters();
            alarmsService.getMachineShiftSolutions(parameters).done(function (result) {
                if (result !== null) {

                    var data = [];
                    machines = result;
                    for (var i = 0; i < result.length; i++) {

                        (function (n) {
                            if ($.grep(machineShiftSolutions, function (v) { return v === result[n].machineShiftSolutionName; }).length === 0) {
                                machineShiftSolutions.push(result[n].machineShiftSolutionName);
                                data.push({ id: result[n].machineShiftSolutionId, text: result[n].machineShiftSolutionName });
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

        function machineTreeChangedCallBack() {
            var statisticalWay = $("#statisticalWay").val();
            if (statisticalWay === "ByShift") {
                setMachineShiftSolutions();
            }
        }

        $("#statisticalWay")
            .on("change",
                function () {
                    var statisticalWay = $("#statisticalWay").val();
                    if (statisticalWay === "ByShift") {
                        $(".input-shift").removeAttr("style");
                        setMachineShiftSolutions();
                    } else {
                        $(".input-shift").css('display', 'none');
                    }
                });

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


        this.save = function () {
            var parameters = getSearchParameters();

            if (parameters.statisticalWay === "ByShift" && $("#checkbox-shift").val() === null) {
                abp.notify.error("请至少选择一个班次");
                return;
            }

            _$modalManager.setResult(parameters);
            _$modalManager.close();
        };

    };
})(jQuery);