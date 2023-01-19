//# sourceURL=report.js
(function () {
    $(function () {
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
                data: { "StartTime": $(".startTime").val(), "EndTime": $(".endTime").val(), "ReportName": "PersonPerformance"  },
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
            if (masterLoad == true) {
                loadData();
                masterLoad = false;
            }
            $("#ProcessBarModal").modal('hide');
        });
    });
   
})();