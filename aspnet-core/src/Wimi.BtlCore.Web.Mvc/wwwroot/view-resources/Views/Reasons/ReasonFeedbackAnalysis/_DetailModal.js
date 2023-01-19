//# sourceURL=dynamicDetailModal.js
(function () {
    app.modals.DetailModal = function () {
        var service = abp.services.app.reasonFeedbackAnalysis;
        var _modalManager;
        var summaryDate = null;
        var stateCode = null;
        var devicegroupId = null;
        var details = null;

        this.init = function (modalManager) {
            _modalManager = modalManager;

            var args = _modalManager.getArgs();
            summaryDate = args.SummaryDate;
            var msg = abp.utils.formatString(app.localize("{0}FeedbackDetails"), wimi.btl.dateFormat(summaryDate));
            _modalManager.getModal().find(".modal-title span").text(msg);
        };

        this.shown = function () {

            var args = _modalManager.getArgs();
            summaryDate = args.SummaryDate;
            stateCode = args.StateCode;
            devicegroupId = args.DeviceGroupId;

            var param = {
                SummaryDate: summaryDate,
                StateCode: stateCode,
                DeviceGroupId: devicegroupId
            };

            service.getDetail(param).done(function (response) {
                table.init(response);
            });
        };

        var table = {
            $detailtable: $("#detailTable"),
            datatables: null,

            init: function (data) {
                if (this.datatables) {
                    table.datatables.destroy();
                    table.$detailtable.empty();
                }

                var value = data;
                this.datatables = table.$detailtable.WIMIDataTable({
                    serverSide: false,
                    responsive: false,
                    data: value,
                    columns: [
                        {
                            orderable: false,
                            data: "machineName",
                            title: app.localize("Machines")
                        },
                        {
                            orderable: false,
                            data: "feedBackReason",
                            title: app.localize("Reason")
                        },
                        {
                            orderable: false,
                            data: "times",
                            title: app.localize("Number")
                        },
                        {
                            orderable: false,
                            data: "duration",
                            title: app.localize("Durations")
                        }
                    ]
                });
            }
        }
    }

})(jQuery);