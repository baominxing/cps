(function () {
    $(function () {

        var _$rolesTable = $("#RolesTable");
        var _$rolesDatatables;
        var _roleService = abp.services.app.role;

        var _$machineTree = new DeviceGroupPermissionTree();
        _$machineTree.initGroup($("div.machines-group-tree"));

        var _permissions = {
            create: abp.auth.hasPermission("Pages.Administration.Roles.Create"),
            edit: abp.auth.hasPermission("Pages.Administration.Roles.Edit"),
            'delete': abp.auth.hasPermission("Pages.Administration.Roles.Delete")
        };

        var _createOrEditModal = new app.ModalManager({
            viewUrl: abp.appPath + "Roles/CreateOrEditModal",
            scriptUrl: abp.appPath + "view-resources/Views/App/Roles/_CreateOrEditModal.js",
            modalClass: "CreateOrEditRoleModal"
        });

        function deleteRole(role) {
            abp.message.confirm(
                app.localize("RoleDeleteWarningMessage", role.displayName),
                function (isConfirmed) {
                    if (isConfirmed) {
                        _roleService.deleteRole({
                            id: role.id
                        }).done(function () {
                            reload();
                            getRoles();
                            abp.notify.success(app.localize("SuccessfullyDeleted"));
                        });
                    }
                }
            );
        };

        $("#CreateNewRoleButton").click(function () {
            _createOrEditModal.open();
        });

        function getRoles() {
            _roleService.getRoles({}).done(function (data) {
                _$rolesDatatables = _$rolesTable.WIMIDataTable({
                    "serverSide": false,
                    "data": data.items,
                    "destroy": true,
                    "searching": true,
                    "columns": [
                        {
                            "defaultContent": "",
                            "className": "text-center",
                            "title": app.localize("Actions"),
                            "orderable": false,
                            "createdCell": function (td, cellData, rowData, row, col) {

                                // [{title, clickEvent, isShow}]
                                $(td).buildActionButtons([
                                    {
                                        title: app.localize("Edit"),
                                        clickEvent: function () { _createOrEditModal.open({ id: rowData.id }) },
                                        isShow: _permissions.edit
                                    }, {
                                        title: app.localize("Delete"),
                                        clickEvent: function () { deleteRole(rowData) },
                                        isShow: !rowData.isStatic && _permissions.delete
                                    }
                                ]);
                            }
                        }, {
                            "data": "displayName",
                            "title": app.localize("RoleName"),
                            "render": function (data, type, full, meta) {
                                var $span = $("<span></span>");

                                $span.append(full.displayName + " &nbsp; ");

                                if (full.isStatic) {
                                    $span.append('<span class="label label-info" data-toggle="tooltip" data-placement="top">' + app.localize("Static") + "</span>&nbsp;");
                                }

                                if (full.isDefault) {
                                    $span.append('<span class="label label-default" data-toggle="tooltip" data-placement="top">' + app.localize("Default") + "</span>&nbsp;");
                                }


                                $span.find("[data-toggle=tooltip]").tooltip();

                                return $span.prop("outerHTML");
                            }
                        }, {
                            "data": "creationTime",
                            "title": app.localize("CreationTime"),
                            "render": function (data, type, full, meta) {
                                return wimi.btl.dateFormat(data);
                            }
                        }
                    ]
                });
            });
        }

        function reload() {
            if (_$rolesDatatables != null) {
                _$rolesDatatables.destroy();
                _$rolesDatatables = null;
                _$rolesTable.empty();
            }
        }

        abp.event.on("app.createOrEditRoleModalSaved", function () {
            reload();
            getRoles();
        });

        getRoles();
    });
})();
