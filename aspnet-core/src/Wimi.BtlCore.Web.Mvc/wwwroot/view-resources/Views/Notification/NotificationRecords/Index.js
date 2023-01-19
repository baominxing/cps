(function () {
    $(function () {
        var notificationAppService = abp.services.app.notification;

        var page = {
            timeRange: $("#reservationtime"),
            btnQuery: $("#btnQuery"),
            datatable: null,
            table: $("#table"),

            getQueryParameters: function () {
                const queryParameters = {};

                const self = this;

                queryParameters.startTime = self.timeRange.data("daterangepicker").startDate.format("YYYY-MM-DD HH:mm");

                queryParameters.endTime = self.timeRange.data("daterangepicker").endDate.format("YYYY-MM-DD HH:mm");

                return queryParameters;
            },

            init: function () {

                this.timeRange.WIMIDaterangepicker();

                page.btnQuery.on("click", function (e) {
                    e.preventDefault();

                    if (page.datatable != null) {
                        page.datatable.destroy();
                        page.datatable = null;
                        page.table.empty();
                    }

                    page.load();
                });

                page.load();
            },

            load: function () {

                var parameters = page.getQueryParameters();

                notificationAppService.listNotificationRecord(parameters).done(function (result) {

                    page.datatable = page.table.WIMIDataTable({
                        serverSide: false,
                        retrieve: true,
                        responsive: false,
                        order: [],
                        data: result,
                        columns: [
                            {
                                data: "notificationType",
                                title: app.localize("NotificationMode"),
                                width: "10%",
                                render: function (data) {
                                    return app.localize(data);
                                }
                            },
                            {
                                data: "messageType",
                                title: app.localize("MessageType"),
                                width: "10%",
                                render: function (data) {
                                    return app.localize(data);
                                }
                            },
                            {
                                data: "status",
                                title: app.localize("State"),
                                width: "10%",
                                render: function (data) {
                                    return app.localize(data);
                                }
                            },
                            {
                                data: "creationTime",
                                title: app.localize("NotificationTime"),
                                width: "10%",
                                render: function (data) {
                                    return moment(data).format("YYYY-MM-DD HH:mm:ss");
                                }
                            },
                            {
                                data: "messageContent",
                                title: app.localize("MessageContent"),
                                width: "60%",
                                render: function (data) {
                                    var tableWidth = $(".box-body")[1].clientWidth;
                                    var charLength = Math.round(tableWidth * 0.6 / 14);
                                    var dotdotdot = data.length <= charLength ? "" : "...";
                                    var html = '<span>' + data.substring(0, charLength) + '' + dotdotdot + '</span>';
                                    return html;
                                }
                            }
                        ]
                    });
                });
            }
        }

        page.init();
    });
})();