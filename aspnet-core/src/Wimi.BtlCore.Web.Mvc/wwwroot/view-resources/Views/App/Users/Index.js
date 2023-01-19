(function() {
    $(function() {

        var _$usersTable = $("#UsersTable");
        var _$usersDatatables = null;
        var _userService = abp.services.app.user;

        var _permissions = {
            create: abp.auth.hasPermission("Pages.Administration.Users.Create"),
            edit: abp.auth.hasPermission("Pages.Administration.Users.Edit"),
            changePermissions: abp.auth.hasPermission("Pages.Administration.Users.ChangePermissions"),
            impersonation: abp.auth.hasPermission("Pages.Administration.Users.Impersonation"),
            'delete': abp.auth.hasPermission("Pages.Administration.Users.Delete"),
            resetPassword: abp.auth.hasPermission("Pages.Administration.Users.ResetPassword"),
        };

        var _createOrEditModal = new app.ModalManager({
            viewUrl: abp.appPath + "Users/CreateOrEditModal",
            scriptUrl: abp.appPath + "view-resources/Views/App/Users/_CreateOrEditModal.js",
            modalClass: "CreateOrEditUserModal"
        });

        var _userPermissionsModal = new app.ModalManager({
            viewUrl: abp.appPath + "Users/PermissionsModal",
            scriptUrl: abp.appPath + "view-resources/Views/App/Users/_PermissionsModal.js",
            modalClass: "UserPermissionsModal"
        });

        var _resetPasswordModal = new app.ModalManager({
            viewUrl: abp.appPath + "Users/ResetPasswordModal",
            scriptUrl: abp.appPath + "view-resources/Views/App/Users/_ResetPasswordModal.js",
            modalClass: "ResetPasswordModal"
        });

        

        _$usersDatatables = _$usersTable.WIMIDataTable({
            "searching": true,
            "ajax": {
                "url": abp.appAPIPath + "user/getUsers"
            },
            "columns": [
                {
                    "defaultContent": "",
                    "title": app.localize("Actions"),
                    "width": "120px",
                    "orderable": false,
                    "createdCell": function(td, cellData, rowData, row, col) {

                        // [{title, clickEvent, permission}]
                        $(td).buildActionButtons([
                            {
                                title: app.localize("ResetPassword"),
                                clickEvent: function () { _resetPasswordModal.open({ id: rowData.id }) },
                                isShow: _permissions.edit
                            },{
                                title: app.localize("Edit"),
                                clickEvent: function() { _createOrEditModal.open({ id: rowData.id }) },
                                isShow: _permissions.edit
                            }, {
                                title: app.localize("Permissions"),
                                clickEvent: function() { _userPermissionsModal.open({ id: rowData.id }) },
                                isShow: _permissions.changePermissions
                            }, {
                                title: app.localize("Delete"),
                                clickEvent: function() { deleteUser(rowData) },
                                isShow: _permissions.delete
                            }, 
                        ]);
                    }
                },
                {
                    "data": "userName",
                    "title": app.localize("UserName")
                },
                {
                    "data": "name",
                    "title": app.localize("Name"),
                    "className": "not-mobile"
                },
                {
                    "data": "roles",
                    "orderable": false,
                    "title": app.localize("RoleName"),
                    "render": function(data, type, full, meta) {
                        var roleNames = "";

                        for (var j = 0; j < data.length; j++) {
                            if (roleNames.length) {
                                roleNames = roleNames + ", ";
                            }

                            roleNames = roleNames + data[j].roleName;
                        };

                        return roleNames;
                    }
                },
                {
                    "data": "emailAddress",
                    "className": "not-mobile",
                    "title": app.localize("EmailAddress")
                },
                {
                    "data": "weChatId",
                    "className": "not-mobile",
                    "title": app.localize("WeChatId")
                },
                {
                    "data": "isActive",
                    "className": "not-mobile",
                    "orderable": false,
                    "title": app.localize("Active"),
                    "render": function(data, type, full, meta) {
                        if (data) {
                            return '<span class="label label-success">' + app.localize("Yes") + "</span>";
                        } else {
                            return '<span class="label label-default">' + app.localize("No") + "</span>";
                        }
                    }
                },
                {
                    "data": "creationTime",
                    "className": "not-mobile",
                    "title": app.localize("CreationTime"),
                    "render": function (data, type, full, meta) {
                        return wimi.btl.dateFormat(data);
                    }
                }
            ]
        });

        function getUsers() {
            _$usersDatatables.ajax.reload(null);
        }

        function deleteUser(user) {
            if (user.userName === app.consts.userManagement.defaultAdminUserName) {
                abp.message.warn(app.localize("{0}UserCannotBeDeleted", app.consts.userManagement.defaultAdminUserName));
                return;
            }

            abp.message.confirm(
                app.localize("UserDeleteWarningMessage", user.userName),
                function(isConfirmed) {
                    if (isConfirmed) {
                        _userService.deleteUser({
                            id: user.id
                        }).done(function() {
                            getUsers(true);
                            abp.notify.success(app.localize("SuccessfullyDeleted"));
                        });
                    }
                }
            );
        }

        $("#CreateNewUserButton").click(function() {
            _createOrEditModal.open();
        });

        $("#ExportUsersToExcelButton").click(function() {
            _userService
                .getUsersToExcel({})
                .done(function(result) {
                    app.downloadTempFile(result);
                });
        });

        $("#GetUsersButton").click(function(e) {
            e.preventDefault();
            getUsers();
        });

        abp.event.on("app.createOrEditUserModalSaved", function() {
            getUsers(true);
        });

        $("#UsersTableFilter").focus();
    });
})();