(function () {
    $(function () {
        var $StandardTimeTable = $("#StandardTimeTable"),
            $createNewButton = $("#Create"),
            standardTimeService = abp.services.app.standardTime,
            standardTimeDataTable = null;

        var permissions = {
            manage: abp.auth.hasPermission("Pages.Order.StandardTime.Manage")
        };

        var createOrUpdateModal = new app.ModalManager({
            viewUrl: abp.appPath + "StandardTime/CreateOrUpdateModal",
            scriptUrl: abp.appPath + "view-resources/Views/Orders/StandardTime/_CreateOrUpdateModal.js",
            modalClass: "CreateOrUpdateModal"
        });

        standardTimeDataTable = $StandardTimeTable.WIMIDataTable({
            "searching": true,
            "ajax": {
                "url": abp.appAPIPath + "standardTime/getStandardTime",
                "type": "POST"
            },
            "columns": [
                {
                    "defaultContent": "",
                    "title": app.localize("Actions"),
                    "orderable": false,
                    "width": "80px",
                    "className": "action",
                    "createdCell": function (td, cellData, rowData, row, col) {
                        $(td)
                            .buildActionButtons([
                                {
                                    title: app.localize("Editor"),
                                    clickEvent: function () {
                                        createOrUpdateModal.open({ Id: rowData.id });
                                    },
                                    isShow: permissions.manage
                                },
                                {
                                    title: app.localize("Delete"),
                                    clickEvent: function () { deletestandardTimeInfo(rowData) },
                                    isShow: permissions.manage
                                }
                            ]);
                    }
                },
                {
                    "data": "productCode",
                    "title": app.localize("ProductCode")
                },
                {
                    "data": "productName",
                    "title": app.localize("ProductName")
                },
                {
                    "data": "processCode",
                    "title": app.localize("ProcessCode")
                },
                {
                    "data": "processName",
                    "title": app.localize("ProcessName")
                },
                {
                    "data": "standardCostTime",
                    "title": app.localize("StandardTime") + "（" + app.localize("Second") + "）"
                },
                {
                    "data": "cycleRate",
                    "title": app.localize("Magnification")
                },
                {
                    "data": "memo",
                    "title": app.localize("Note")
                }

            ]
        });

        $createNewButton.on("click",
            function () {
                createOrUpdateModal.open();
            });
        abp.event.on("app.CreateOrUpdateModalSaved", function () {
            standardTimeDataTable.ajax.reload(null);
        });

        function deletestandardTimeInfo(rowData) {
            abp.message.confirm(app.localize("WhetherToDeleteThisData"),
                function (isConfirmed) {
                    if (isConfirmed) {
                        standardTimeService.deleteStandardTime({ id: rowData.id })
                            .done(function () {
                                abp.notify.success(app.localize("SuccessfullyDeleted"));
                                standardTimeDataTable.ajax.reload(null);
                            });
                    }
                });
        }
    });
})();