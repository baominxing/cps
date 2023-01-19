(function () {
    $(function () {
        var $machineTypeTable = $('#machineTypeTable'),
            $create = $('#Create'),
            basicDataService = abp.services.app.basicData,
            machineTypeDataTable = null;

        var permissions = {
            manage: abp.auth.hasPermission("Pages.BasicData.MachineType")
        };

        var createOrUpdateModal = new app.ModalManager({
            viewUrl: abp.appPath + "MachineType/CreateOrUpdateModal",
            scriptUrl: abp.appPath + "view-resources/Views/BasicData/MachineType/_CreateOrUpdateModal.js",
            modalClass: "CreateOrUpdateModal"
        });

        //var sss = app.localize("Actions");
        //console.log(sss);


        machineTypeDataTable = $machineTypeTable.WIMIDataTable({
            "searching": true,
            "ajax": {
                "url": abp.appPath + "api/services/app/basicData/getMachineTypeList",
                "type": "POST"
            },
            'columns': [
                {
                    "defaultContent": "",
                    "title": app.localize("Actions"),
                    "orderable": false,
                    "width": "80px",
                    "className": "action",
                    "createdCell": function (td, cellData, rowData, row, col) {
                        $(td).buildActionButtons([
                            {
                                title: app.localize("Editor"),
                                clickEvent: function () {
                                    createOrUpdateModal.open({ Id: rowData.id });
                                },
                                isShow: permissions.manage
                            },
                            {
                                title: app.localize("Delete"),
                                clickEvent: function () { deleteMachineTypeInfo(rowData) },
                                isShow: permissions.manage
                            }
                        ]);
                    }
                },
                {
                    "data": "name",
                    "title": app.localize("ObjectName")
                },
                {
                    "data": "desc",
                    "title": app.localize("ObjectDesc")
                }
            ]

        });

        $create.on('click', function () {
            createOrUpdateModal.open();
        });

        abp.event.on("app.CreateOrUpdateModalSaved", function () {
            machineTypeDataTable.ajax.reload(null);
        });

        //删除
        function deleteMachineTypeInfo(rowData) {
            abp.message.confirm(app.localize("DeleteMachineTypeConfirm{0}", rowData.name),
                function (isConfirmed) {
                    if (isConfirmed) {
                        basicDataService.deleteMachineType({ id: rowData.id }).done(function () {
                            abp.notify.success(app.localize("SuccessfullyDeleted"));
                            machineTypeDataTable.ajax.reload(null
                            );
                        });
                    }
                });
        }
    });

})();