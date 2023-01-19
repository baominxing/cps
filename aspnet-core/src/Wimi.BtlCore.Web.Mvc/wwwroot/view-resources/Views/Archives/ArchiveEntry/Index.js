(function () {
    $(function () {
        var service = abp.services.app.archiveEntry;
        var permissions = { manage: abp.auth.hasPermission("Pages.Archives.ArchiveEntry.Manage") };
        var page = null;

        var createOrUpdateModal = new app.ModalManager({
            viewUrl: abp.appPath + "ArchiveEntry/CreateOrUpdateModal",
            scriptUrl: abp.appPath + "Views/Archives/ArchiveEntry/CreateOrUpdateModal.js",
            modalClass: "CreateOrUpdateModal"
        });

        page = {
            $create: $("#create"),
            $search: $("#search"),
            searchParameters: [],
            $table: $("#table"),
            dataTable: null,

            create: function () {
                createOrUpdateModal.open();
            },

            remove: function (rowData) {
                abp.message.confirm(app.localize("WhetherToDeleteArchiveEntry"),
                    function (isConfirmed) {
                        if (isConfirmed) {
                            service.delete({ Id: rowData.id })
                                .done(function () {
                                    abp.notify.success(app.localize("SuccessfullyDeleted"));
                                    page.dataTable.ajax.reload(null);
                                });
                        }
                    });
            },

            search: function () {
                page.load();
            },

            load: function () {
                if (page.dataTable) {
                    page.dataTable.destroy();
                    page.$table.empty();
                }

                page.searchParameters = {
                    TargetTable: $("#TargetTable").val(),
                    ArchivedTable: $("#ArchivedTable").val(),
                };

                page.dataTable = page.$table.WIMIDataTable({
                    "scrollCollapse": true,
                    "scrollX": true,
                    "searching": false,
                    "ajax": {
                        "url": abp.appAPIPath + "archiveentry/listArchiveEntry",
                        "type": "POST",
                        "data": page.searchParameters
                    },
                    "columns": [
                        {
                            "data": null,
                            "title": app.localize("SerialNumber"),
                            "width": "30px",
                            "orderable": false,
                            "render": function (data, type, row, meta) {
                                return meta.row + 1 +
                                    meta.settings._iDisplayStart;
                            }
                        },
                        {
                            data: "targetTable",
                            title: "数据表格"
                        },
                        {
                            data: "archivedTable",
                            title: "归档表格"
                        },
                        {
                            data: "archiveColumn",
                            title: "归档列名"
                        },
                        {
                            data: "archiveValue",
                            title: "归档列值"
                        },
                        {
                            data: "archiveCount",
                            title: "分表此次归档数量"
                        },
                        {
                            data: "archiveTotalCount",
                            title: "分表归档总数量"
                        },
                        {
                            data: "archivedMessage",
                            title: "归档消息"
                        },
                    ]
                });

                $.fn.dataTable.tables({ visible: true, api: true }).columns.adjust();//表头错位
            },

            init: function () {

                page.$create.on("click", function () {
                    page.create();
                });

                page.$search.on("click", function () {
                    page.search();
                });

                page.load();
            }
        };

        page.init();

        abp.event.on("app.CreateOrUpdateModalSaved", function () {
            page.load();
        });
    });

})();