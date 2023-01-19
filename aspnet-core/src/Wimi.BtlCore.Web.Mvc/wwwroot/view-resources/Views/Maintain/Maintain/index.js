(function () {
    $(function () {
        var maintainAppService = abp.services.app.maintain;
        var syncDataTable = $("#syncDataTable");
        var deleteAllBtn = $("#allDelete");
        var page = {

            init: function () {

                page.table.load();
                deleteAllBtn.click(function () {
                    deleteAll();
                });
            },
            table: {
                tableForm: $("#syncDataTable"),
                dataTable: null,
                load: function () {

                    page.table.dataTable = page.table.tableForm.WIMIDataTable({
                        "ajax": {
                            url: abp.appAPIPath + "maintain/getDyncDataList",
                            data: { Id: null }
                        },
                        //"scrollX": true,
                        //"responsive": false,
                        "order": [],
                        "columns": [
                            {
                                data: "jobName",
                                title: app.localize("JobName"),
                                orderable: true
                            },
                            {
                                data: "creationTime",
                                title: app.localize("TimeOfProduction"),
                                orderable: true,
                                render: function (data) {
                                    return wimi.btl.dateTimeFormat(data);
                                }
                            },
                            {
                                data: null,
                                title: app.localize("Duration"),
                                orderable: true,
                                render: function (data, type, row) {
                                   return  moment().diff(moment(row.creationTime),'minutes');
                                }
                            },
                            {
                                title: app.localize("Action"),
                                data: null,
                                width: "10%",
                                className: "text-center",
                                orderable: false,
                                render: function () {
                                    return "";
                                },
                                createdCell: function (td, cellData, rowData, row, col) {

                                    $('<button class="btn btn-default btn-xs"><i class="fa fa-delete">' + app.localize("Delete") + '</i></button>')
                                        .appendTo($(td))
                                        .click(function () {
                                            deleteById(rowData.id);
                                        });
                                }
                            }
                        ]
                    });
                }
            }
          
        };
        var deleteById = function (id) {
            maintainAppService.deleteSyncData({ Id: id }).done(function () {
                abp.notify.success(app.localize("SuccessfullyDeleted"));
                page.table.dataTable.ajax.reload(null);
            });
        }
        var deleteAll = function () {
            maintainAppService.deleteAllSyncData().done(function () {
                abp.notify.success(app.localize("DeleteAllSuccess"));
                page.table.dataTable.ajax.reload();
            });
        }

        page.init();
       
    });
})();