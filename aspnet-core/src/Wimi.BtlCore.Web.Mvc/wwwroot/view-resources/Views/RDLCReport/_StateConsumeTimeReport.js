//# sourceURL=StateConsumereport.js
(function () {
    $(function () {
        var daterangepickerOption = {
            singleDatePicker: true,
            showWeekNumbers: false,
            autoApply: true,
            autoUpdateInput: true
        };
        $(".startTime").WIMIDaterangepicker(daterangepickerOption);
        $(".endTime").WIMIDaterangepicker($.extend({ "endTimeOnly": true }, daterangepickerOption));
        function loadData() {
            $.ajax({
                type: "post",
                url: "UpdateViewComponent",
                async: false,
                data: { "StartTime": $(".startTime").val(), "EndTime": $(".endTime").val(), "ReportName": "StateConsumeTimeReport" },
                success: function (result) {
                    debugger
                    $("#reportarea").html(result)
                }
            });
        }
        //function loadData() {
        //    $.ajax({
        //        type: "post",
        //        url: "updateViewComponent",
        //        async: false,
        //        data: { "StartTime": $(".startTime").val(), "EndTime": $(".endTime").val(), "ReportName": "StateConsumeTimeReport" },
        //        success: function (result) {
        //            debugger
        //            $("#reportarea").html(result)
        //        }
        //    });
        //}
        $("#search").click(function () {
            loadData();
        });
    });
})();