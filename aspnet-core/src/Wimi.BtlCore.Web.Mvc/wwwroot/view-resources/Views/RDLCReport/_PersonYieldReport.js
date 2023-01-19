//# sourceURL=report.js
(function () {
    $(function () {
        var service = abp.services.app.rDLCReportService;

        var daterangepickerOption = {
            singleDatePicker: true,
            showWeekNumbers: false,
            maxDate: moment(),
            autoApply: true,
            autoUpdateInput: true
        };
        $(".startTime").WIMIDaterangepicker(daterangepickerOption);
        $(".endTime").WIMIDaterangepicker($.extend({ "endTimeOnly": true }, daterangepickerOption));

        function loadData() {

            $.ajax({
                type: "post",
                url: "updateViewComponent",
                async: false,
                data: { "StartTime": $(".startTime").val(), "EndTime": $(".endTime").val(), "ReportName": "PersonYield", "ShiftSolutionId": $("#ShiftSolution option:selected").val() },
                success: function (result) {
                    $("#reportarea").html(result)
                }
            });

        }

        $("#search").click(function () {
            loadData();
        });

        var masterLoad = true;

        $("#report").load(function () {

            if (masterLoad === true) {
                loadData();
                masterLoad = false;
            }
            $("#ProcessBarModal").modal('hide');
        });

        var page = {

            fillShiftSolutionList: function () {

                service.getShiftSolutionList().done(function (result) {
                    if (result !== null && result.length > 0) {

                        for (var i = 0; i < result.length; i++) {

                            var item = result[i];

                            var option = "<option value=" + item.shiftSolutionId + ">" + item.shiftSolutionName + "</option>";

                            $("#ShiftSolution").append(option);
                        }
                    }
                });
            },

            init: function () {
                page.fillShiftSolutionList();
            }
        };

        page.init();
    });
})();