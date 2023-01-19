(function () {
    $(function () {
        var $processTable = $("#ProcessTable"),
            $createNewButton = $("#CreateNewProcess"),
            processService = abp.services.app.process,
            processDataTable = null;

        var permissions = {
            manage: abp.auth.hasPermission("Pages.Order.Process.Manage")
        };

        var createOrUpdateModal = new app.ModalManager({
            viewUrl: abp.appPath + "Process/CreateOrUpdateModal",
            scriptUrl: abp.appPath + "view-resources/Views/Orders/Process/_CreateOrUpdateModal.js",
            modalClass: "CreateOrUpdateModal"
        });

        console.log(abp);

        processDataTable = $processTable.WIMIDataTable({
            "searching": true,
            "ajax": {
                "url": abp.appAPIPath + "process/getProcess",
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
                                    clickEvent: function () { deleteProcessInfo(rowData) },
                                    isShow: permissions.manage
                                }
                            ]);
                    }
                },
                {
                    "data": "code",
                    "title": app.localize("ProcessCode")
                },
                {
                    "data": "name",
                    "title": app.localize("ProcessName")
                },
                {
                    "data": "memo",
                    "title": app.localize("Describe")
                }
            ]
        });

        $createNewButton.on("click",
            function () {
                createOrUpdateModal.open();
            });
        abp.event.on("app.CreateOrUpdateModalSaved", function () {
            processDataTable.ajax.reload(null);
        });

        function deleteProcessInfo(rowData) {
            abp.message.confirm(app.localize("WhetherToDeleteTheProcess")+"[" + rowData.name + "]",
                function (isConfirmed) {
                    if (isConfirmed) {
                        processService.deleteProcess({ id: rowData.id })
                            .done(function () {
                                abp.notify.success(app.localize("SuccessfullyDeleted"));
                                processDataTable.ajax.reload(null);
                            });
                    }
                });
        }
    });

})();