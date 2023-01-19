//# sourceURL=dynamic_CreateOrUpdateModalForRulesType.js
(function () {
    app.modals.CreateOrUpdateModalForRulesType = function () {

        var _modalManager;
        var notificationAppService = abp.services.app.notification;
        var commonLookupAppService = abp.services.app.commonLookup;
        var createOrUpdateForm = null;
        var isEditMode = $("#IsEditMode").val();
        var messageType = 0, deviceGroupIds;
        var machineGroupTree = new MachinesTree();

        this.init = function (modalManager) {
            _modalManager = modalManager;
            createOrUpdateForm = _modalManager.getModal().find("form[name=CreateOrUpdateForm]");

            getDeviceGroupWithPermissions(messageType);
        };

        this.shown = function () {
            
            $("#MessageType").select2({
                multiple: false,
                minimumResultsForSearch: -1,
                language: {
                    noResults: function () {
                        return app.localize("NoMatchingData");
                    }
                }
            });
        }

        this.save = function () {
            if (!createOrUpdateForm.valid()) {
                return false;
            }

            if (machineGroupTree.getSelectedGroupIds().length <= 0) {
                abp.message.error(app.localize("ChooseDeviceGroup"));
                return false;
            }

            var parameters = {
                name: $(".name").val(),
                deviceGroupIds: machineGroupTree.getSelectedGroupIds().join(","),
                messageType: $(".messageType").val()
            }

            _modalManager.setBusy(true);

            if (isEditMode === "false") {
                notificationAppService.createNotificationRule(
                    parameters
                ).done(function (result) {
                    abp.notify.info(app.localize("SavedSuccessfully"));
                    _modalManager.setResult(result);
                    _modalManager.close();
                }).always(function () {
                    _modalManager.setBusy(false);
                });
            } else {
                parameters.id = $("#Id").val();
                notificationAppService.updateNotificationRule(
                    parameters
                ).done(function (result) {
                    abp.notify.info(app.localize("SavedSuccessfully"));
                    _modalManager.setResult(result);
                    _modalManager.close();
                }).always(function () {
                    _modalManager.setBusy(false);
                });
            }

        };

        // 重置 授权设备组的id
        var getGrantedGroups = function (groups, grantedGroupIds) {

            function isGranted(group) {
                return _.contains(grantedGroupIds, group.id);
            }

            function parentGroup(group) {
                return _.find(groups, function (item) {
                    return item.id === group.parentId;
                });
            }

            var grantedGroups = _.filter(groups, isGranted);

            function getParentId(child, parent) {

                if (child.parentId == null) {
                    return null;
                }

                var parentIsGranted = isGranted(parent);

                // 父授权
                if (parentIsGranted) {
                    return parent.id;
                }

                // 子授权
                var childIsGranted = isGranted(child);

                if (childIsGranted) {
                    // 父的父
                    var p = parentGroup(parent);
                    return getParentId(parent, p);
                }
                return null;// 子没有授权
            }

            _.each(grantedGroups, function (item) {
                if (item.parentId == null) {
                    return;
                }
                var parent = parentGroup(item);
                item.parentId = getParentId(item, parent);
            });

            return grantedGroups;

        };

        function getTree(source) {

            var treeData = _.map(source, function (item) {

                return {
                    id: item.id,
                    icon: 'fa fa-object-group',
                    parent: item.parentId == null ? '#' : item.parentId,
                    text: item.displayName,
                    state: {
                        selected: false,
                        opened: true
                    }, data: item
                }
            });

            return treeData;
        }

        function getDeviceGroupWithPermissions(messageType) {
            var id = $("#Id").val();
               var referencedDeviceGroupIds = [];

            commonLookupAppService.getDeviceGroupWithPermissions().done(function (result) {

                var grantedGroupIds = _.filter(result.grantedGroupIds, function (item) {
                    return !_.contains(referencedDeviceGroupIds, item);
                });

                if (grantedGroupIds.length <= 0) {

                    $('#DeviceGroupTree').remove();

                    $('.deviceGroupTreeContainer').append('<div id="DeviceGroupTree"></div>');

                    $(".save-button").attr("disabled", "true");

                } else {
                    var grantedGroups = getGrantedGroups(result.deviceGroups, grantedGroupIds);

                    _.each(grantedGroups, function (item) {
                        item.isGroup = true;
                    });

                    var treeData = getTree(grantedGroups);

                    if (machineGroupTree) {
                        machineGroupTree = null;
                        machineGroupTree = new MachinesTree();
                    }

                    machineGroupTree.initGroupWithJson($('#DeviceGroupTree'),
                        treeData, true,
                        function () {
                            if (isEditMode !== "true") {
                                machineGroupTree.setSelectGroupsTree();
                            }

                            if (isEditMode === "true") {

                                deviceGroupIds = $("#DeviceGroupIds").val().split(",");
                                _.each(deviceGroupIds, function (key) {
                                    machineGroupTree.selectGroupTreeNode(key * 1);
                                });
                            }
                        });
                        $(".save-button").removeAttr("disabled");
                    }

                    if (isEditMode === "true") {
                        messageType = $("#MessageType").val();
                        $(".messageType").val(messageType);
                        $(".messageType").attr("disabled", true);
                    }
                });
        };

        $(".messageType").on("change",
            function () {
                var messageType = $(".messageType").val();
                getDeviceGroupWithPermissions(messageType);
            });
    };
})();