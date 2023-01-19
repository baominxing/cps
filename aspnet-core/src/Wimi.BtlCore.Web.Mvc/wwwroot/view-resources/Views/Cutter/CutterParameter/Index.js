(function () {
    $(function () {
        var $table = $('#table'),
            $btnCreate = $('#btnCreate'),
            cutterAppService = abp.services.app.cutter,
            $dataTable = null;

        var permissions = {
            manage: abp.auth.hasPermission("Pages.Cutter.CutterParameter.Manage")
        };

        var createOrUpdateModal = new app.ModalManager({
            viewUrl: abp.appPath + "CutterParameter/CreateOrUpdateModal",
            scriptUrl: abp.appPath + "view-resources/Views/Cutter/CutterParameter/_CreateOrUpdateModal.js",
            modalClass: "CreateOrUpdateModal"
        });

        $btnCreate.on('click', function () {
            createOrUpdateModal.open();
        });

        abp.event.on("app.CreateOrUpdateModalSaved", function () {
            loadDataTable();
        });

        //页面加载
        function pageLoad() {
            loadDataTable();
        }
        //加载loadDataTable表格
        function loadDataTable() {

            if ($dataTable != null) {
                $dataTable.destroy();
                $dataTable = null;
                $table.empty();
            }

            cutterAppService.getCutterParameterList().done(function (result) {
                $dataTable = $table.WIMIDataTable({
                    searching:true,
                    serverSide: false,
                    data: result,
                    columns: [
                        {
                            title: app.localize("Actions"),
                            data: null,
                            width: "10%",
                            className: "text-center",
                            orderable: false,
                            render: function () {
                                return "";
                            },
                            createdCell: function (td, cellData, rowData, row, col) {
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
                                        clickEvent: function () { deleteCutterParameter(rowData) },
                                        isShow: permissions.manage,
                                        className: "btn-danger"
                                    }
                                ]);
                            }
                        },
                        {
                            data: "code",
                            title: app.localize("ParameterCoding")
                        },
                        {
                            data: "name",
                            title: app.localize("ParameterName")
                        },
                        {
                            data: "creatorName",
                            title: app.localize("Creator")
                        },
                        {
                            data: "creationTime",
                            title: app.localize("CreationTime"),
                            render: function (data) {
                                return moment(data).format("YYYY-MM-DD HH:mm:ss");
                            }
                        },
                        {
                            data: "lastModifierName",
                            title: app.localize("Modifier"),
                            render: function (data) {
                                if (data == null) {
                                    return "";
                                }
                                return data;
                            }
                        },
                        {
                            data: "lastModificationTime",
                            title: app.localize("ModificationTime"),
                            render: function (data) {
                                if (data == null) {
                                    return "";
                                }
                                return moment(data).format("YYYY-MM-DD HH:mm:ss");
                            }
                        }
                    ]

                });
            });
        }

        //删除
        function deleteCutterParameter(rowData) {
            abp.message.confirm(app.localize("DeleteToolParameterConfiguration{0}",rowData.name),
                function (isConfirmed) {
                    if (isConfirmed) {
                        cutterAppService.deleteCutterParameter({ id: rowData.id }).done(function () {
                            abp.notify.success(app.localize("SuccessfullyDeleted"));
                            loadDataTable();
                        });
                    }
                });
        }

        pageLoad();
    });

})();