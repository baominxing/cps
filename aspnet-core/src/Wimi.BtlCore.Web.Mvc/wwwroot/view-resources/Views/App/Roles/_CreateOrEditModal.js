(function () {
    app.modals.CreateOrEditRoleModal = function () {

        var _modalManager;
        var _roleService = abp.services.app.role;
        var _$roleInformationForm = null;
        var _permissionsTree;
        var _deviceGroupPermissionTree;

        this.init = function (modalManager) {
            _modalManager = modalManager;

            _permissionsTree = new PermissionsTree();
            _permissionsTree.init(_modalManager.getModal().find(".permission-tree"));

            _deviceGroupPermissionTree = new DeviceGroupPermissionTree();
            _deviceGroupPermissionTree.initGroup(_modalManager.getModal().find(".machines-group-tree"));
            _deviceGroupPermissionTree.openAll();

            _$roleInformationForm = _modalManager.getModal().find("form[name=RoleInformationsForm]");
        };

        this.save = function () {
            if (!_$roleInformationForm.valid()) {
                return;
            }

            var role = _$roleInformationForm.serializeFormToObject();

            _modalManager.setBusy(true);
            _roleService.createOrUpdateRole({
                role: role,
                grantedPermissionNames: _permissionsTree.getSelectedPermissionNames(),
                grantedDeviceGroupPermissions: _deviceGroupPermissionTree.getSelectedPermissionNames()
            }).done(function () {
                abp.notify.info(app.localize("SavedSuccessfully"));
                _modalManager.close();
                abp.event.trigger("app.createOrEditRoleModalSaved");
            }).always(function () {
                _modalManager.setBusy(false);
            });
        };
    };
})();
