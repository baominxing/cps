//# sourceURL=report.js
(function () {
    $(function () {

        var daterangepickerOption = {
            singleDatePicker: true,
            showWeekNumbers: false,
            timePickerIncrement: 1,
            timePicker24Hour: false,
            autoApply: true,
            autoUpdateInput: true,

        };
        $(".startTime").WIMIDaterangepicker(daterangepickerOption);
        $(".endTime").WIMIDaterangepicker($.extend({ "endTimeOnly": true }, daterangepickerOption));
        var isLoad = false;

        function Query() {
            $.ajax({
                type: "post",
                url: "updateViewComponent",
                async: false,
                data: { "StartTime": $(".startTime").val(), "EndTime": $(".endTime").val(), "ReportName": "Plan" },
                success: function (result) {
                    $("#reportarea").html(result)
                }
            });

        }

        $("#search").click(function () {
            Query();
        });

        $("#report").load(function () {
            if(isLoad == false)
            {
                Query();
                isLoad = true;
            }
            $("#ProcessBarModal").modal('hide');
        });
    });
})();