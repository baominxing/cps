(function ($, abp) {
    app.modals.LinkedAccountsModal = function () {

        var _modalManager;

        var _$linkedAccountsTable = $("#LinkedAccountsTable");

        var _userLinkService = abp.services.app.userLink;

        this.init = function (modalManager) {
            _modalManager = modalManager;
        };

        var _linkNewAccountModal = new app.ModalManager({
            viewUrl: abp.appPath + "Profile/LinkAccountModal",
            scriptUrl: abp.appPath + "view-resources/Views/App/Profile/_LinkAccountModal.js",
            modalClass: "LinkAccountModal"
        });

        $("#LinkNewAccountButton").click(function () {

            _linkNewAccountModal.open({}, function () {
                getLinkedUsers();
            });

        });

        var table = _$linkedAccountsTable.WIMIDataTable({
            "ajax": {
                url: abp.appAPIPath + "userLink/GetLinkedUsers",
                data: function (d) {
                    //d.id = deviceTree.selectedOu.id;
                }
            },
            columns: [
                {
                    "defaultContent": "",
                    "title": app.localize("Action"),
                    "orderable": false,
                    "width": "60px",
                    "className": "text-center not-mobile",
                    "createdCell": function (td, cellData, rowData, row, col) {
                        $('<button class="btn btn-xs btn-primary blue"><i class="icon-login"></i>' + app.localize("LogIn") + "</button>")
                            .appendTo($(td))
                            .click(function () {
                                switchToUser(rowData);
                            });



                    }
                },
                { "data": "username", "title": app.localize("UId") },
                {
                    "defaultContent": "",
                    "title": app.localize("Remove"),
                    "orderable": false,
                    "width": "60px",
                    "className": "text-center not-mobile",
                    "createdCell": function (td, cellData, rowData, row, col) {

                        $('<button class="btn btn-default  btn-xs" title="' + app.localize("LogIn") + '"><i class="fa fa-trash"></i></button>')
                            .appendTo($(td))
                            .click(function () {

                                deleteLinkedUser(rowData);
                            });
                    }
                }

            ]
        });

        function switchToUser(linkedUser) {

            var targetUrl = window.location.pathname + window.location.hash;

            abp.ajax({
                url: abp.appPath + "Account/SwitchToLinkedAccount",
                data: JSON.stringify({
                    targetUserId: linkedUser.id,
                    targetTenantId: linkedUser.tenantId,
                    targetUrl: targetUrl
                })
            });
        }

        function deleteLinkedUser(linkedUser) {
            abp.message.confirm(
                abp.utils.formatString(app.localize("AssociatedUser{0}WillBeDeleted"), linkedUser.username),
                function (isConfirmed) {
                    if (isConfirmed) {
                        _userLinkService.unlinkUser({
                            userId: linkedUser.id,
                            tenantId: linkedUser.tenantId
                        }).done(function () {
                            getLinkedUsers();
                            abp.notify.success(app.localize("SuccessfullyUnlinked"));
                        });
                    }
                }
            );
        }

        function getLinkedUsers() {

            table.ajax.reload(null);


        }

        // getLinkedUsers();
    };
})(jQuery, abp);