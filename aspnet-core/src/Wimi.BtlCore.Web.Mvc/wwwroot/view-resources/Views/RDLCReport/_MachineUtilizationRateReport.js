//# sourceURL=MachineUtilizationRateReport.js
(function () {
    $(function () {
        loadData();

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
                data: { "StartTime": $(".startTime").val(), "EndTime": $(".endTime").val(), "ReportName": "MachineUtilizationRate" },
                success: function (result) {
                    $("#reportarea").html(result)
                }
            });
        }
        $("#search").click(function () {
            var startTime = $(".startTime").val();
            var endTime = $(".endTime").val();
            if (moment(endTime).diff(moment(startTime), 'days') > 30) {
                abp.message.warn(app.localize("TheSelectedPeriodCannotBeLongerThan30Days"));
                return;
            }
            loadData();
        });
    });
})();