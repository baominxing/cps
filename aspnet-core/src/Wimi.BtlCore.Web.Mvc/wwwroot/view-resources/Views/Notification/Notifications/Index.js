(function () {
    $(function () {

        var _$notificationsTable = $("#NotificationsTable");
        var $notificationsDataTables = null;

        var _$targetValueFilterSelectionCombobox = $("#TargetValueFilterSelectionCombobox");
        _$targetValueFilterSelectionCombobox.selectpicker();


        var _appUserNotificationHelper = new app.UserNotificationHelper();

        $notificationsDataTables = _$notificationsTable.WIMIDataTable({
            "processing": true,
            "serverSide": false,
            "order": [[2, "desc"]],
            "ajax": {
                "url": abp.appPath + "notifications/getUserNotifications",
                "type": "POST",
                data: function (d) {
                    var state = _$targetValueFilterSelectionCombobox.val();
                    d.state = state === "ALL" ? null : state;
                },
                dataFilter: function (data) {
                    var json = $.parseJSON(data);
                    json.result.data = json.result.items;
                    return JSON.stringify(json.result);
                }
            },

            "columns": [
                {
                    "defaultContent": "",
                    "title": app.localize("Actions"),
                    "orderable": false,
                    "createdCell": function (td, cellData, rowData, row, col) {
                        var $td = $(td);

                        var $button = $('<button class="btn btn-xs btn-default"></button>')
                            .click(function (e) {
                                e.preventDefault();
                                setNotificationAsRead(rowData, function () {
                                    $button.find("i")
                                        .removeClass("fa-envelope")
                                        .addClass("fa-envelope-open");
                                    $button.attr("disabled", "disabled");

                                });
                            }).appendTo($td);

                        var $i = $('<i class="fa" >').appendTo($button);

                        var notificationState = _appUserNotificationHelper.format(rowData).state;

                        if (notificationState === "READ") {
                            $button.attr("disabled", "disabled");
                            $i.addClass("fa-envelope-open");
                        }

                        if (notificationState === "UNREAD") {
                            $i.addClass("fa-envelope");
                        }
                    }
                }, {
                    "data": "notification",
                    "orderable": false,
                    "title": app.localize("Notifications"),
                    "render": function (data, type, full, meta) {
                        var formattedRecord = _appUserNotificationHelper.format(full);
                        var rowClass = getRowClass(formattedRecord);

                        if (formattedRecord.url) {
                            return '<a href="' + formattedRecord.url + '" class="' + rowClass + '">' + formattedRecord.text + "</a>";
                        } else {
                            return '<span class="' + rowClass + '">' + formattedRecord.text + "</span>";
                        }
                    }
                }, {
                    "data": "creationTime",
                    "orderable": false,
                    "title": app.localize("CreationTime"),
                    "render": function (data, type, full, meta) {
                        var formattedRecord = _appUserNotificationHelper.format(full);
                        var rowClass = getRowClass(formattedRecord);
                        return '<span class="' + rowClass + '">' + formattedRecord.timeAgo + "</span> &nbsp;";
                    }
                }
            ]
        });

        function getRowClass(formattedRecord) {
            return formattedRecord.state === "READ" ? "notification-read" : "";
        }

        function getNotifications() {
            $notificationsDataTables.ajax.reload(null);
        }

        function setNotificationAsRead(userNotification, callback) {
            _appUserNotificationHelper.setAsRead(userNotification.id, function () {
                if (callback) {
                    callback();
                }
            });
        }

        function setAllNotificationsAsRead() {
            _appUserNotificationHelper.setAllAsRead(function () {
                getNotifications();
            });
        };

        function openNotificationSettingsModal() {
            _appUserNotificationHelper.openSettingsModal();
        };

        _$targetValueFilterSelectionCombobox.change(function () {
            getNotifications();
        });

        $("#RefreshNotificationTableButton").click(function (e) {
            e.preventDefault();
            getNotifications();
        });

        $("#btnOpenNotificationSettingsModal").click(function (e) {
            openNotificationSettingsModal();
        });

        $("#btnSetAllNotificationsAsRead").click(function (e) {
            e.preventDefault();
            setAllNotificationsAsRead();
        });

    });
})();