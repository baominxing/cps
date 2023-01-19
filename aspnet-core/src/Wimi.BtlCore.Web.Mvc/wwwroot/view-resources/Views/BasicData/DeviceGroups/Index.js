(function () {
    $(function () {

        var _deviceGroupService = abp.services.app.deviceGroup;

        var _permissions = {
            manageDeviceTree: abp.auth.hasPermission("Pages.BasicData.DeviceGroups.ManageDeviceTree"),
            manageMembers: abp.auth.hasPermission("Pages.BasicData.DeviceGroups.ManageMembers")
        };

        var _createModal = new app.ModalManager({
            viewUrl: abp.appPath + "DeviceGroups/CreateModal",
            scriptUrl: abp.appPath + "view-resources/Views/BasicData/DeviceGroups/_CreateModal.js",
            modalClass: "CreateDeviceGroupModal"
        });

        var _editModal = new app.ModalManager({
            viewUrl: abp.appPath + "DeviceGroups/EditModal",
            scriptUrl: abp.appPath + "view-resources/Views/BasicData/DeviceGroups/_EditModal.js",
            modalClass: "EditDeviceGroupsModal"
        });

        var _userLookupModal = app.modals.LookupModal.create({
            title: app.localize("PleaseSelectEquipment"),
            serviceMethod: "commonLookup/findMachines"
        });
        var members;
        var deviceTree = {
            $tree: $("#OrganizationUnitEditTree"),

            $emptyInfo: $("#OrganizationUnitTreeEmptyInfo"),

            show: function () {
                deviceTree.$emptyInfo.hide();
                deviceTree.$tree.show();
            },

            hide: function () {
                deviceTree.$emptyInfo.show();
                deviceTree.$tree.hide();
            },

            unitCount: 0,

            setUnitCount: function (unitCount) {
                deviceTree.unitCount = unitCount;
                if (unitCount) {
                    deviceTree.show();
                } else {
                    deviceTree.hide();
                }
            },

            refreshUnitCount: function () {
                deviceTree.setUnitCount(deviceTree.$tree.jstree("get_json").length);
            },

            selectedOu: {
                id: null,
                displayName: null,
                code: null,

                set: function (ouInTree) {
                    if (!ouInTree) {
                        deviceTree.selectedOu.id = null;
                        deviceTree.selectedOu.displayName = null;
                        deviceTree.selectedOu.code = null;
                    } else {
                        deviceTree.selectedOu.id = ouInTree.id;
                        deviceTree.selectedOu.displayName = ouInTree.original.displayName;
                        deviceTree.selectedOu.code = ouInTree.original.code;
                    }

                    members.load();
                }
            },

            contextMenu: function (node) {

                var items = {
                    editUnit: {
                        label: app.localize("Edit"),
                        _disabled: !_permissions.manageDeviceTree,
                        action: function (data) {

                            _editModal.open({
                                id: node.id
                            }, function (updatedOu) {
                                deviceTree.reload();
                            });
                        }
                    },

                    addSubUnit: {
                        label: app.localize("NewSubordinateGroup"),
                        _disabled: !_permissions.manageDeviceTree,
                        action: function () {
                            deviceTree.addUnit(node.id);
                        }
                    },

                    addMember: {
                        label: app.localize("MachineSetting_Create"),
                        _disabled: !_permissions.manageMembers,
                        action: function () {
                            members.openAddModal();
                        }
                    },

                    'delete': {
                        label: app.localize("Delete"),
                        _disabled: !_permissions.manageDeviceTree,
                        action: function (data) {
                            var instance = $.jstree.reference(data.reference);

                            abp.message.confirm(
                                app.localize("DeleteDeviceGroupConfirmTip{0}", node.original.displayName),
                                function (isConfirmed) {
                                    if (isConfirmed) {
                                        _deviceGroupService.deleteDeviceGroup({
                                            id: node.id
                                        }).done(function () {
                                            abp.notify.success(app.localize("SuccessfullyDeleted"));
                                            instance.delete_node(node);
                                            deviceTree.refreshUnitCount();
                                        }).fail(function (err) {
                                            setTimeout(function () { abp.message.error(err.message); }, 500);
                                        });
                                    }
                                }
                            );
                        }
                    }
                };
                return items;
            },

            addUnit: function (parentId) {
                var instance = $.jstree.reference(deviceTree.$tree);

                _createModal.open({
                    parentId: parentId
                }, function (newOu) {
                    instance.create_node(
                        parentId ? instance.get_node(parentId) : "#",
                        {
                            id: newOu.id,
                            parent: newOu.parentId ? newOu.parentId : "#",
                            code: newOu.code,
                            displayName: newOu.displayName,
                            memberCount: 0,
                            text: deviceTree.generateTextOnTree(newOu),
                            state: {
                                opened: true
                            }
                        });

                    deviceTree.refreshUnitCount();
                });
            },

            generateTextOnTree: function (ou) {
                var itemClass = ou.memberCount > 0 ? " ou-text-has-members" : " ou-text-no-members";
                return '<span class="ou-text' + itemClass + '" data-ou-id="' + ou.id + '">' + ou.displayName + ' (<span class="ou-text-member-count">' + ou.memberCount + '</span>) <i class="fa fa-caret-down text-muted"></i></span>';
            },

            incrementMemberCount: function (ouId, incrementAmount) {
                var treeNode = deviceTree.$tree.jstree("get_node", ouId);
                treeNode.original.memberCount = treeNode.original.memberCount + incrementAmount;
                deviceTree.$tree.jstree("rename_node", treeNode, deviceTree.generateTextOnTree(treeNode.original));
            },

            getTreeDataFromServer: function (callback) {
                _deviceGroupService.getDeviceGroups({}).done(function (result) {
                    var treeData = _.map(result.items, function (item) {
                        return {
                            id: item.id,
                            parent: item.parentId ? item.parentId : "#",
                            code: item.code,
                            displayName: item.displayName,
                            memberCount: item.memberCount,
                            text: deviceTree.generateTextOnTree(item),
                            state: {
                                opened: true
                            }
                        };
                    });

                    callback(treeData);
                });
            },

            init: function () {
                deviceTree.getTreeDataFromServer(function (treeData) {
                    deviceTree.setUnitCount(treeData.length);

                    deviceTree.$tree
                        .on("changed.jstree", function (e, data) {
                            if (data.selected.length !== 1) {
                                deviceTree.selectedOu.set(null);
                            } else {
                                var selectedNode = data.instance.get_node(data.selected[0]);
                                deviceTree.selectedOu.set(selectedNode);
                            }
                        })
                        .on("move_node.jstree", function (e, data) {

                            var parentNodeName = (!data.parent || data.parent === "#")
                                ? app.localize("Root")
                                : deviceTree.$tree.jstree("get_node", data.parent).original.displayName;

                            abp.message.confirm(
                                app.localize("OrganizationUnitMoveConfirmMessage", data.node.original.displayName, parentNodeName),
                                function (isConfirmed) {
                                    if (isConfirmed) {
                                        _deviceGroupService.moveDeviceGroup({
                                            id: data.node.id,
                                            newParentId: data.parent
                                        }).done(function () {
                                            abp.notify.success(app.localize("SuccessfullyMoved"));
                                            deviceTree.reload();
                                        }).fail(function (err) {
                                            deviceTree.$tree.jstree("refresh"); //rollback
                                            setTimeout(function () { abp.message.error(err.message); }, 500);
                                        });
                                    } else {
                                        deviceTree.$tree.jstree("refresh"); //rollback
                                    }
                                }
                            );
                        })
                        .jstree({
                            'core': {
                                data: treeData,
                                multiple: false,
                                check_callback: function (operation, node, node_parent, node_position, more) {
                                    return true;
                                }
                            },
                            types: {
                                "default": {
                                    "icon": "fa fa-folder tree-item-icon-color icon-lg"
                                },
                                "file": {
                                    "icon": "fa fa-file tree-item-icon-color icon-lg"
                                }
                            },
                            contextmenu: {
                                items: deviceTree.contextMenu
                            },
                            sort: function (node1, node2) {
                                if (this.get_node(node2).original.seq <= this.get_node(node1).original.seq) {
                                    return -1;
                                }

                                return 1;
                            },
                            plugins: [
                                "types",
                                "contextmenu",
                                "wholerow",
                                "sort",
                                "dnd"
                            ]
                        });

                    $("#AddRootUnitButton").click(function (e) {
                        e.preventDefault();
                        deviceTree.addUnit(null);
                    });

                    deviceTree.$tree.on("click", ".ou-text .fa-caret-down", function (e) {
                        e.preventDefault();

                        var ouId = $(this).closest(".ou-text").attr("data-ou-id");
                        setTimeout(function () {
                            deviceTree.$tree.jstree("show_contextmenu", ouId);
                        }, 100);
                    });
                });
            },

            reload: function () {
                deviceTree.getTreeDataFromServer(function (treeData) {
                    deviceTree.setUnitCount(treeData.length);
                    deviceTree.$tree.jstree(true).settings.core.data = treeData;
                    deviceTree.$tree.jstree("refresh");
                });
            }
        };
        members = {
            $table: $("#OuMembersTable"),
            $datatable: null,
            $emptyInfo: $("#OuMembersEmptyInfo"),
            $addUserToOuButton: $("#AddUserToOuButton"),
            $selectedOuRightTitle: $("#SelectedOuRightTitle"),

            showTable: function () {
                members.$emptyInfo.hide();
                members.$table.show();
                members.$addUserToOuButton.show();
                members.$selectedOuRightTitle.text(": " + deviceTree.selectedOu.displayName).show();
            },

            hideTable: function () {
                members.$selectedOuRightTitle.hide();
                members.$addUserToOuButton.hide();
                if (members.$datatable) {
                    members.$datatable.destroy();
                }
                members.$table.hide();
                members.$emptyInfo.show();
            },

            load: function () {
                if (!deviceTree.selectedOu.id) {
                    members.hideTable();
                    return;
                }

                members.showTable();

                if (!$.fn.DataTable.isDataTable("#OuMembersTable")) {

                    members.$datatable = members.$table.WIMIDataTable({
                        "ajax": {
                            url: abp.appPath + "api/services/app/deviceGroup/getDeviceGroupMachines",
                            data: function (d) {
                                d.id = deviceTree.selectedOu.id;
                            }
                        },
                        "columns": [
                            {
                                "defaultContent": "",
                                "title": app.localize("Actions"),
                                "orderable": false,
                                "width": "30px",
                                "className": "text-center not-mobile",
                                "createdCell": function (td, cellData, rowData, row, col) {

                                    if (_permissions.manageMembers) {

                                        $('<button class="btn btn-danger btn-xs">' + app.localize("Delete") + '</button>')
                                            .appendTo($(td))
                                            .click(function () {
                                                members.remove(rowData);
                                            });
                                    }
                                }
                            },
                            { "data": "code", "title": app.localize("MachineCode") },
                            { "data": "name", "title": app.localize("MachineName") },
                            { "data": "desc", "title": app.localize("MachineDesc") },
                            {
                                "data": "addedTime",
                                "title": app.localize("AddedTime"),
                                "render": function (data, type, full, meta) {
                                    return wimi.btl.dateFormat(data);
                                }
                            }
                        ]
                    });
                } else {

                    members.$datatable.ajax.reload(null);
                }
            },

            add: function (machineList) {
                var deviceGroupId = deviceTree.selectedOu.id;
                if (!deviceGroupId) {
                    return;
                }

                _deviceGroupService.addMachineListToDeviceGroup({
                    deviceGroupId: deviceGroupId,
                    machineIdList: _.pluck(machineList, "value")
                }).done(function (data) {
                    if (data === null) {
                        abp.notify.success(app.localize("SuccessfullyAdded"));
                    } else {
                        abp.message.error(data);
                    }
                    deviceTree.reload();
                    members.load();
                });
            },

            remove: function (machine) {
                var deviceGroupId = deviceTree.selectedOu.id;
                if (!deviceGroupId) {
                    return;
                }

                abp.message.confirm(
                    app.localize("RemoveMachineOutOfGroup{0}{1}", machine.name, deviceTree.selectedOu.displayName),
                    function (isConfirmed) {
                        if (isConfirmed) {
                            _deviceGroupService.removeMachineFromDeviceGroup({
                                deviceGroupId: deviceGroupId,
                                machineId: machine.id
                            }).done(function () {
                                abp.notify.success(app.localize("SuccessfullyRemoved"));
                                deviceTree.incrementMemberCount(deviceGroupId, -1);
                                members.load();
                            });
                        }
                    }
                );
            },

            openAddModal: function () {
                var ouId = deviceTree.selectedOu.id;
                if (!ouId) {
                    return;
                }

                _userLookupModal.open({ deviceGroupId: ouId }, function (selectedItems) {
                    members.add(selectedItems);
                });
            },

            init: function () {

                $("#AddUserToOuButton").click(function (e) {
                    e.preventDefault();
                    members.openAddModal();
                });

                members.hideTable();
            }
        };
        members.init();
        deviceTree.init();

    });
})();