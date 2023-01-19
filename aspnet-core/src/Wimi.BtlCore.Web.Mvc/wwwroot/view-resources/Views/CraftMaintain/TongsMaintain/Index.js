(function () {
    $(function () {
        var service = abp.services.app.tong;
        var permissions = { manage: abp.auth.hasPermission("Pages.CraftMaintain.TongsMaintain.Manage") };
        var page = null;

        var createOrUpdateModal = new app.ModalManager({
            viewUrl: abp.appPath + "TongsMaintain/CreateOrUpdateModal",
            scriptUrl: abp.appPath + "view-resources/Views/CraftMaintain/TongsMaintain/CreateOrUpdateModal.js",
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
                abp.message.confirm(app.localize("WhetherToDeleteTong"),
                    function (isConfirmed) {
                        if (isConfirmed) {
                            service.deleteTong({ id: rowData.id })
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
                    Code: $("#Code").val().trim(),
                    Name: $("#Name").val().trim(),
                    CapacityList: $("#Capacity").val(),
                    ProgramName: $("#ProgramName").val()
                };

                page.dataTable = page.$table.WIMIDataTable({
                    "scrollCollapse": true,
                    "scrollX": true,
                    "searching": false,
                    "ajax": {
                        "url": abp.appAPIPath + "tong/listTong",
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
                                            clickEvent: function () { page.remove(rowData); },
                                            isShow: permissions.manage
                                        }
                                    ]);
                            }
                        },
                        {
                            data: "code",
                            title: app.localize("TongCode")
                        },
                        {
                            data: "name",
                            title: app.localize("TongName")
                        },
                        {
                            data: "capacity",
                            title: app.localize("TongCapacity")
                        },
                        {
                            title: app.localize("ProgramName"),
                            "render": function (td, cellData, rowData, row, col) {
                                var str = '';
                                if (rowData.programA != null && rowData.programA != "") {
                                    str += ' <b> A:</b> <button type="button" class="btn btn-primary btn-xs">' + rowData.programA +'</button>';
                                }
                                if (rowData.programB != null && rowData.programB != "") {
                                    str += ' <b> B:</b> <button type="button" class="btn btn-secondary btn-xs">' + rowData.programB + '</button>';
                                }
                                if (rowData.programC != null && rowData.programC != "") {
                                    str += ' <b> C:</b> <button type="button" class="btn btn-success btn-xs">' + rowData.programC + '</button>';
                                }
                                if (rowData.programD != null && rowData.programD != "") {
                                    str += ' <b> D:</b> <button type="button" class="btn btn-danger btn-xs">' + rowData.programD + '</button>';
                                }
                                if (rowData.programE != null && rowData.programE != "") {
                                    str += ' <b> E:</b> <button type="button" class="btn btn-warning btn-xs">' + rowData.programE + '</button>';
                                }
                                if (rowData.programF != null && rowData.programF != "") {
                                    str += ' <b> F:</b> <button type="button" class="btn btn-info btn-xs">' + rowData.programF + '</button>';
                                }
                                return str;
                            }
                        },
                        {
                            data: "note",
                            title: app.localize("Note")
                        },
                        {
                            data: "creationTime",
                            title: app.localize("CreationTime"),
                            render: function (data) {
                                return wimi.btl.dateTimeFormat(data);
                            }
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
            },

            initCapacity: function () {
                $("#Capacity").empty();
                var data = [
                    { id: 1, text: 1 },
                    { id: 2, text: 2 },
                    { id: 3, text: 3 },
                    { id: 4, text: 4 },
                    { id: 5, text: 5 },
                    { id: 6, text: 6 },
                ];

                $("#Capacity").select2({
                    data: data,
                    multiple: true,
                    minimumResultsForSearch: -1,
                    placeholder: app.localize("PleaseChoose"),
                    language: {
                        noResults: function () {
                            return app.localize("");
                        }
                    }
                }).val(0).trigger('change');

                //var selectMachine;
                //if ($('#Id').val() != 0) {
                //    service.getFrockToolMachineId({ id: $('#Id').val() })
                //        .done(function (result) {
                //            selectMachine = result;
                //            $("#Capacity").select2().val(selectMachine).trigger('change');
                //        });
                //}
            },
        };
        page.initCapacity();
        page.init();

        abp.event.on("app.CreateOrUpdateModalSaved", function () {
           page.load();
        });
    });

})();