(function () {
    $(function () {
        var alarmTable = $("#alarmInfoTables"),
            createButton = $('#CreateNewAlarmInfo'),
            $importButton = $("#ImportNewAlarmInfo"),
            $alarmDataTable = null,
            alarmService = abp.services.app.alarms;

        var permissions = {
            manage: abp.auth.hasPermission("Pages.BasicData.AlarmInfo")
        };

        var createOrEditModal = new app.ModalManager({
            viewUrl: abp.appPath + "AlarmInfo/CreateOrEditModal",
            scriptUrl: abp.appPath + "view-resources/Views/BasicData/AlarmInfo/_CreateOrEditModal.js",
            modalClass: "CreateOrUpdateModal"
        });

        var importModal = new app.ModalManager({
            viewUrl: abp.appPath + "AlarmInfo/ImportModal",
            scriptUrl: abp.appPath + "view-resources/Views/BasicData/AlarmInfo/_ImportModal.js",
            modalClass: "ImportModal"
        });
        var page = {
            init: function () {
                page.getAlarmList();
            },
          
            getAlarmList: function () {

                    $alarmDataTable = alarmTable.WIMIDataTable({
                        "searching": true,
                        "ajax": {
                            "url": abp.appPath + "api/services/app/alarms/getAlarmInfoList",
                            "type": "POST"
                        },
                        'columns': [
                            {
                                "defaultContent": "",
                                "title": app.localize("Actions"),
                                "orderable": false,
                                "width": "30px",
                                "className": "action",
                                "createdCell": function (td, cellData, rowData, row, col) {
                                    $(td).buildActionButtons([
                                        {
                                            title: app.localize("Editor"),
                                            clickEvent: function () {
                                                createOrEditModal.open({ Id: rowData.id });
                                            },
                                            isShow: permissions.manage
                                        },
                                        {
                                            title: app.localize("Delete"),
                                            clickEvent: function () { deleteAlarmInfo(rowData) },
                                            isShow: permissions.manage,
                                            className: "btn-danger"
                                        }
                                    ]);
                                }
                            },
                            {
                                "data": "machineCode",
                                "title": app.localize("MachineCode")
                            },
                            {
                                "data": "machineName",
                                "title": app.localize("MachineName")
                            },
                            {
                                "data": "code",
                                "title": app.localize("AlarmCode")
                            },
                            {
                                "data": "message",
                                "title": app.localize("AlarmContent")
                            },
                            {
                                "data": "creationTime",
                                "title": app.localize("CreationTime"),
                                "render": function (data) {
                                    return wimi.btl.dateTimeFormat(data);
                                }
                            }
                        ]
                    });
            }
        }
        createButton.on('click', function () {
            createOrEditModal.open();
        });

        $("#downloadTemplate").click(function () {
            $(this).attr('href', abp.appPath + "Download/Template/AlarmInfo.xlsx");
        });

        $importButton.on('click', function () {
            importModal.open();
        });

        abp.event.on("app.CreateOrUpdateModalSaved", function () {
            $alarmDataTable.ajax.reload(null)
        });

        //删除
        function deleteAlarmInfo(rowData) {
            abp.message.confirm(app.localize("DeleteDataEncoding{0}",rowData.code),
                function (isConfirmed) {
                    if (isConfirmed) {
                        alarmService.deleteAlarmInfo({ Id: rowData.id }).done(function () {
                            abp.notify.success(app.localize("SuccessfullyDeleted"));
                            $alarmDataTable.ajax.reload(null)
                        });
                    }
                });
        }
        page.init();
    });
})(jQuery);
