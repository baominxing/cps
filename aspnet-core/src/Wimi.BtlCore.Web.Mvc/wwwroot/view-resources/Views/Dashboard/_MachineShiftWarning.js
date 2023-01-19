(function () {
    app.modals.MachineShiftWarningModal = function () {
        var service = abp.services.app.machineShiftEffectiveInterval;
        var $table = $("#shiftWarning");
        this.shown = function () {

            service.listShiftEffectiveIntervals().done(function (response) {
                $table.WIMIDataTable({
                    serverSide: false,
                    orderable: false,
                    data: response,
                    columns: [
                        {
                            orderable: false,
                            data: 'machineName',
                            title: app.localize("MachineName")
                        },
                        {
                            orderable: false,
                            data: 'shiftSolutionName',
                            title: app.localize("PagesBasicDataShiftSetting")
                        },
                        {
                            orderable: false,
                            data: 'startTime',
                            title: app.localize("StartTime"),
                            render: function (data) {
                                return wimi.btl.dateFormat(data);
                            }
                        },
                        {
                            orderable: false,
                            data: 'endTime',
                            title: app.localize("EndTime"),
                            render: function (data) {
                                return wimi.btl.dateFormat(data);
                            }
                        }, {
                            orderable: false,
                            data: 'expiryDay',
                            title: app.localize("DaysRemaining")
                        }
                    ]
                });
            });
        };
    };
})();