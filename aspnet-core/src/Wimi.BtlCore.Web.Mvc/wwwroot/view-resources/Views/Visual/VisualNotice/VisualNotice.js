(function () {
    $(function () {
        var noticesTable = $("#noticesInfoTables"),
            createButton = $("#CreateNewNotice"),
            $noticesDataTabls = null,
            noticesService = abp.services.app.visual;

        var permissions = {
            manage: abp.auth.hasPermission("Pages.Visual.Notice.Manage")
        };

        var createOrEditModal = new app.ModalManager({
            viewUrl: abp.appPath + "VisualView/CreateOrEditModal",
            scriptUrl: abp.appPath + "view-resources/Views/Visual/VisualNotice/_CreateOrEditModal.js",
            modalClass: "CreateOrUpdateModal"
        });
        load();
        function load() {
                $noticesDataTabls = noticesTable.WIMIDataTable({
                    "searching": true,
                    "ajax": {
                        "url": abp.appAPIPath + "visual/getNoticesList",
                        "type": "POST"
                    },
                    'columns': [
                        {
                            "defaultContent": "",
                            "title": app.localize("Actions"),
                            "orderable": false,
                            "width": "10%",
                            "className": "text-center",
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
                                        clickEvent: function () { deleteNotices(rowData) },
                                        isShow: permissions.manage
                                    }
                                ]);
                            }
                        },
                        {
                            "data": "content",
                            "title": app.localize("NoticeContent"),
                            "width": "50%"
                        },
                        {
                            "data": "isActive",
                            "title": app.localize("IsEnable"),
                            "width": "8%",
                            "render": function (data, type, full, meta) {
                                return '<input class="switch" type="checkbox" ' + (data ? "checked" : "") +
                                    ' data-id="' + full.id + '"data-tag="priorshow" />';
                            }
                        },
                        {
                            "data": "workShopName",
                            "title": app.localize("RootLevelGroup(Workshop)"),
                            "width": "12%"
                        },
                        {
                            "data": "creatorUserName",
                            "title": app.localize("Publisher"),
                            "width": "10%"
                        },
                        {
                            "data": "creationTime",
                            "title": app.localize("ReleaseTime"),
                            "width": "15%",
                            "render": function (data) {
                                return moment(data).format("YYYY-MM-DD HH:mm:ss");
                            }
                        }
                    ],
                    "drawCallback": function (settings) {
                        $('.switch').bootstrapSwitch({ "size": "mini", "onColor": "success" });
                    }
                });
        //    });
        }
      

        createButton.on('click', function () {
            createOrEditModal.open();
        });


        abp.event.on("app.CreateOrUpdateModalSaved", function () {
            $noticesDataTabls.ajax.reload(null);
        });

        $(document)
            .on('switchChange.bootstrapSwitch',
                '.switch',
                function(event, state) {
                    var targetData = $(this).data();
                    noticesService
                        .updateNoticsActive({ id: targetData.id })
                        .done(function() {
                            $noticesDataTabls.ajax.reload(null);
                        });
                });

        //删除
        function deleteNotices(rowData) {
            abp.message.confirm(app.localize("WhetherToDeleteTheRecord")+"？",
                function(isConfirmed) {
                    if (isConfirmed) {
                        noticesService.deleteNotices({ id: rowData.id })
                            .done(function() {
                                abp.notify.success(app.localize("SuccessfullyDeleted"));
                                $noticesDataTabls.ajax.reload(null);
                            });
                    }
                });
        }
    });
})();