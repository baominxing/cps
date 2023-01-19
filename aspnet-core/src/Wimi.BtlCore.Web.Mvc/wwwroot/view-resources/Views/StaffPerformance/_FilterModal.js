//# sourceURL=dynamic_FilterModal.js
(function () {
    app.modals.FilterModal = function () {
        var staffPerformance = abp.services.app.staffPerformance;
        var $datepicker = $("#daterangepicker");
        var $machineTree = new MachinesTree();

        this.init = function (modalManager) {
            _$modalManager = modalManager;
            _$searchform = _$modalManager.getModal().find('form[name=SearchForm]');

            $machineTree.init($("div.machines-tree"), true);
            $machineTree.initGroup($("div.machines-group-tree"));
            $machineTree.setSelectAll();

            $datepicker.WIMIDaterangepicker({
                startDate: moment().subtract(6, "days"),
                endDate: moment()
            });

            fillStaffDropdownList();

            $("#statisticalWay").on("change", function () {
                var statisticalWay = $(this).val();

                if (statisticalWay === "ByShift") {
                    $(".shiftsolution").show();

                    fillShiftSolutionList();
                } else {
                    $(".shiftsolution").hide();
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
        };

        this.save = function () {

            var parameters = getSearchParameters();
            _$modalManager.setResult(parameters);
            _$modalManager.close();
        };

        function fillStaffDropdownList() {
            staffPerformance.getStaffList().done(function (result) {
                if (result !== null && result.length > 0) {

                    var data = _.map(result, function (item) {
                        return { id: item.userId, text: item.userName };
                    });

                    data.unshift({ id: 0, text: app.localize("All") });

                    $("#staff").select2({
                        data: data,
                        multiple: true,
                        width: "100%", //设置下拉框的宽度
                        placeholder: app.localize("PleaseChoose"),
                        language: {
                            noResults: function () {
                                return app.localize("NoMatchingData");
                            }
                        }
                    });

                    //默认选择全部
                    $("#staff").select2('val', [0]);
                }
            });
        }

        function fillShiftSolutionList() {

            staffPerformance.getShiftSolutionList().done(function (result) {
                if (result !== null && result.length > 0) {

                    var data = _.map(result, function (item) {
                        return { id: item.shiftSolutionId, text: item.shiftSolutionName };
                    });

                    for (var i = 0; i < result.length; i++) {

                        var item = result[i];

                        var option = "<option value=" + item.shiftSolutionId + ">" + item.shiftSolutionName + "</option>";

                        $("#ShiftSolution").append(option);
                    }
                }
            });
        }

        function getSearchParameters() {
            var statisticalWay = $("#statisticalWay").val();
            var startTime = $datepicker.data("daterangepicker").startDate.format("YYYY-MM-DD");
            var endTime = $datepicker.data("daterangepicker").endDate.format("YYYY-MM-DD");
            var machineIdList = $machineTree.getSelectedMachineIds();
            var userIdList = $('#staff').val();

            var parameters = {
                startTime: startTime,
                endTime: endTime,
                statisticalWay: statisticalWay,
                statisticalWayText: $("#statisticalWay option:selected").text(),
                machineIdList: machineIdList,
                userIdList: userIdList,
                queryType: "0",
                shiftSolutionId: $("#ShiftSolution option:selected").val(),
                shiftSolutionName: $("#ShiftSolution option:selected").text()
            };
            return parameters;
        }
    };
})(jQuery);