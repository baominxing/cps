//# sourceURL=dynamic_ShowAlarmsDetailModal.js
(function($) {
    app.modals.ShowAlarmsDetailModal = function() {

        var alarmsService = abp.services.app.alarms;
        var _modalManager,_args;

        this.init = function(modalManager, args) {
            _modalManager = modalManager;
            _args = args;
        };


        this.shown = function() {
            alarmsService.getAlarmDetailDataForModal(_args.parameters).done(function(result) {

                $(".machineName").text(app.localize("MachineName") + ":" + result[0].machineName);
                $(".alarmMessage").text(app.localize("AlarmContent") + ":" + result[0].alarmMessage);

                _modalManager.getModal().find("#AlarmsTable").WIMIDataTable({
                    serverSide: false,
                    data: result,
                    order: [],
                    columns: [
                        {
                            title: app.localize("DateOfProduction"),
                            orderable: false,
                            data: function(item) {
                                return moment(item.startTime).format("YYYY-MM-DD");
                            }
                        },
                        {
                            title: app.localize("TimeOfProduction"),
                            orderable: false,
                            data: function(item) {
                                return moment(item.startTime).format("HH:mm:ss");
                            }
                        }
                    ]
                });
            });
        };
    };
})(jQuery);