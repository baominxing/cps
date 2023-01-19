(function () {
    $(function () {
        var stateTable = $('#stateInfoTables'),
            createButton = $('#CreateNewStateInfo'),
            basicDataService = abp.services.app.basicData,
            stateDataTables = null;

        var permissions = {
            manage: abp.auth.hasPermission("Pages.BasicData.StateInfo")
        };

        var createOrUpdateModal = new app.ModalManager({
            viewUrl: abp.appPath + "StateInfo/CreateOrUpdateModal",
            scriptUrl: abp.appPath + "view-resources/Views/BasicData/StateInfo/_CreateOrUpdateModal.js",
            modalClass: "CreateOrUpdateModal"
        });

        stateDataTables = stateTable.WIMIDataTable({
            order: [],
            "searching": true,
            "ajax": {
                "url": abp.appPath + "api/services/app/basicData/getStateInfoList",
                "type": "POST"
            },
            'columns': [
                {
                    "defaultContent": "",
                    "title": app.localize("Actions"),
                    "orderable": false,
                    "width": "80px",
                    "className": "text-center",
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
                                clickEvent: function () { deleteStateInfo(rowData); },
                                isShow: permissions.manage && !rowData.isStatic
                            }
                        ]);
                    }
                },
                {
                    "orderable": false,
                    "data": "code",
                    "title": app.localize("Code")
                },
                {
                    "orderable": false,
                    "data": "displayName",
                    "title": app.localize("Name")
                },
                {
                    "data": "type",
                    "title": app.localize("Type"),
                    "render": function (data) {
                        if (!data) {
                            return '<span class="label label-primary">' + app.localize("State") + '</span>';
                        } else {
                            return '<span class="label label-primary">' + app.localize("Reason") + '</span>';
                        }
                    }
                },
                {
                    "data": "isPlaned",
                    "title": app.localize("WhetherPlannedORNot"),
                    "render": function (data) {
                        if (!data) {
                            return '<span class="label label-default">' + app.localize("No") + "</span>";
                        } else {
                            return '<span class="label label-success">' + app.localize("Yes") + "</span>";
                        }
                    }
                },
                {
                    "orderable": false,
                    "data": "hexcode",
                    "title": app.localize("BackgroundColor"),
                    'render': function (data) {
                        return '<span class="info-box-icon bg-aqua" style="height:18px;background-color:' + data + ' !important""></span> ';
                    }
                }
            ]

        });

        createButton.on('click', function () {
            createOrUpdateModal.open();
        });

        abp.event.on("app.CreateOrUpdateModalSaved", function () {
            stateDataTables.ajax.reload(null);
        });

        //删除
        function deleteStateInfo(rowData) {
            abp.message.confirm(app.localize(app.localize("DeleteOrNot") + "[" + rowData.displayName + "]"),
                function (isConfirmed) {
                    if (isConfirmed) {
                        basicDataService.deleteStateInfo({ id: rowData.id }).done(function () {
                            abp.notify.success(app.localize("SuccessfullyDeleted"));
                            stateDataTables.ajax.reload(null);
                        });
                    }
                });
        }
    });

})();