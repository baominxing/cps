(function() {
    $(function () {
        var service = abp.services.app.defectiveReasons;

        var permissions = {
            manage: abp.auth.hasPermission("Pages.Order.DefectiveReasons.Manage")
        };

        var createOrUpdateModal = new app.ModalManager({
            viewUrl: abp.appPath + "DefectiveReasons/CreateOrUpdateModal",
            scriptUrl: abp.appPath + "view-resources/Views/Orders/DefectiveReasons/_CreateOrUpdateModal.js",
            modalClass: "CreateOrUpdateModal"
        });

        var createPartModal = new app.ModalManager({
            viewUrl: abp.appPath + "DefectiveReasons/CreateDefectivePartModal",
            scriptUrl: abp.appPath + "view-resources/Views/Orders/DefectiveReasons/_CreatePartModal.js",
            modalClass: "CreateDefectivePartModal"
        });

        var editPartModal = new app.ModalManager({
            viewUrl: abp.appPath + "DefectiveReasons/EditDefectivePartModal",
            scriptUrl: abp.appPath + "view-resources/Views/Orders/DefectiveReasons/_EditPartModal.js",
            modalClass: "EditDefectivePartModal"
        });

        var members;
        var defectivePartTree = {
            $tree: $("#OrganizationUnitEditTree"),

            $emptyInfo: $("#OrganizationUnitTreeEmptyInfo"),

          

            show: function() {
                defectivePartTree.$emptyInfo.hide();
                defectivePartTree.$tree.show();
              
            },

            hide: function() {
                defectivePartTree.$emptyInfo.show();
                defectivePartTree.$tree.hide();
              
            },

            unitCount: 0,

            setUnitCount: function(unitCount) {
                defectivePartTree.unitCount = unitCount;
                if (unitCount) {
                    defectivePartTree.show();
                } else {
                    defectivePartTree.hide();
                }
            },

            refreshUnitCount: function () {
                defectivePartTree.setUnitCount(defectivePartTree.$tree.jstree("get_json").length);
            },

            selectedOu: {
                id: null,
                name: null,
                code: null,

                set: function (ouInTree) {
                    if (!ouInTree) {
                        defectivePartTree.selectedOu.id = null;
                        defectivePartTree.selectedOu.name = null;
                        defectivePartTree.selectedOu.code = null;
                    } else {
                        defectivePartTree.selectedOu.id = ouInTree.id;
                        defectivePartTree.selectedOu.name = ouInTree.original.name;
                        defectivePartTree.selectedOu.code = ouInTree.original.code;
                    }

                    members.load();
                }
            },
            contextMenu: function (node) {

                var items = {
                    editUnit: {
                        label: app.localize("Edit"),
                        _disabled: !permissions.manage,
                        action: function(data) {
                            var instance = $.jstree.reference(data.reference);
                            editPartModal.open({
                                id: node.id
                        },
                                function(updatedOu) {
                                    node.original.name = updatedOu.name;
                                    instance.rename_node(node, defectivePartTree.generateTextOnTree(updatedOu));
                                });
                        }
                    },

                    addSubUnit: {
                        label: app.localize("NewSubgroups"),
                        _disabled: !permissions.manage,
                        action: function() {
                            defectivePartTree.addUnit(node.id);
                        }
                    },

                    addMember: {
                        label: app.localize("NewDefectiveParts"),
                        _disabled: !permissions.manage,
                        action: function() {
                            members.openAddModal();
                        }
                    },

                    'delete': {
                        label: app.localize("Delete"),
                        _disabled: !permissions.manage,
                        action: function(data) {
                            var instance = $.jstree.reference(data.reference);

                            abp.message.confirm(
                                abp.utils.formatString(app.localize("RemoveBadParts"), node.original.name),
                                function(isConfirmed) {
                                    if (isConfirmed) {
                                        service.deleteDefectivePart({
                                            id: node.id
                                        }).done(function() {
                                            abp.notify.success(app.localize("SuccessfullyDeleted"));
                                            instance.delete_node(node);
                                            defectivePartTree.refreshUnitCount();
                                        }).fail(function(err) {
                                            setTimeout(function() { abp.message.error(err.message); }, 500);
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
                var instance = $.jstree.reference(defectivePartTree.$tree);

                createPartModal.open({
                    parentId: parentId
                }, function (newOu) {
                    instance.create_node(
                        parentId ? instance.get_node(parentId) : "#",
                        {
                            id: newOu.id,
                            parent: newOu.parentId ? newOu.parentId : "#",
                            code: newOu.code,
                            name: newOu.name,
                            memberCount: 0,
                            text: defectivePartTree.generateTextOnTree(newOu),
                            state: {
                                opened: true
                            }
                        });

                    defectivePartTree.refreshUnitCount();
                });
            },

            generateTextOnTree: function (ou) {
                var itemClass = ou.memberCount > 0 ? " ou-text-has-members" : " ou-text-no-members";
                return '<span class="ou-text' + itemClass + '" data-ou-id="' + ou.id + '">' + ou.name + ' (<span class="ou-text-member-count">' + ou.memberCount + '</span>) <i class="fa fa-caret-down text-muted"></i></span>';
            },

            incrementMemberCount: function (ouId, incrementAmount) {
                var treeNode = defectivePartTree.$tree.jstree("get_node", ouId);
                treeNode.original.memberCount = treeNode.original.memberCount + incrementAmount;
                defectivePartTree.$tree.jstree("rename_node", treeNode, defectivePartTree.generateTextOnTree(treeNode.original));
            },

            getTreeDataFromServer: function (callback) {
                service.listDefectivePart({}).done(function (result) {
                    var treeData = _.map(result.items, function (item) {
                        return {
                            id: item.id,
                            parent: item.parentId ? item.parentId : "#",
                            code: item.code,
                            name: item.name,
                            memberCount: item.memberCount,
                            text: defectivePartTree.generateTextOnTree(item),
                            state: {
                                opened: true
                            }
                        };
                    });

                    callback(treeData);
                });
            },

            init: function () {
                defectivePartTree.getTreeDataFromServer(function (treeData) {
                    defectivePartTree.setUnitCount(treeData.length);

                    defectivePartTree.$tree
                        .on("changed.jstree", function (e, data) {
                            if (data.selected.length !== 1) {
                                defectivePartTree.selectedOu.set(null);
                            } else {
                                var selectedNode = data.instance.get_node(data.selected[0]);
                                defectivePartTree.selectedOu.set(selectedNode);
                            }
                        })
                        .on("move_node.jstree", function (e, data) {

                            var parentNodeName = (!data.parent || data.parent === "#")
                                ? app.localize("Root")
                                : defectivePartTree.$tree.jstree("get_node", data.parent).original.name;

                            abp.message.confirm(
                                app.localize("OrganizationUnitMoveConfirmMessage", data.node.original.name, parentNodeName),
                                function (isConfirmed) {
                                    if (isConfirmed) {
                                        service.moveDefectivePart({
                                            id: data.node.id,
                                            newParentId: data.parent
                                        }).done(function () {
                                            abp.notify.success(app.localize("SuccessfullyMoved"));
                                            defectivePartTree.reload();
                                        }).fail(function (err) {
                                            defectivePartTree.$tree.jstree("refresh"); //rollback
                                            setTimeout(function () { abp.message.error(err.message); }, 500);
                                        });
                                    } else {
                                        defectivePartTree.$tree.jstree("refresh"); //rollback
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
                                items: defectivePartTree.contextMenu
                            },
                            sort: function (node1, node2) {
                                if (this.get_node(node2).original.name < this.get_node(node1).original.name) {
                                    return 1;
                                }

                                return -1;
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
                        defectivePartTree.addUnit(null);
                    });

                    defectivePartTree.$tree.on("click", ".ou-text .fa-caret-down", function (e) {
                        e.preventDefault();

                        var ouId = $(this).closest(".ou-text").attr("data-ou-id");
                        setTimeout(function () {
                            defectivePartTree.$tree.jstree("show_contextmenu", ouId);
                        }, 100);
                    });
                });
            },
            reload: function () {
                defectivePartTree.getTreeDataFromServer(function (treeData) {
                    defectivePartTree.setUnitCount(treeData.length);
                    defectivePartTree.$tree.jstree(true).settings.core.data = treeData;
                    defectivePartTree.$tree.jstree("refresh");
                });
            }
        };
        members = {
            $table: $("#DefectiveReasonsTable"),
                $datatable: null,
                $emptyInfo: $("#OuMembersEmptyInfo"),
                $createNewReason: $("#CreateNewReason"),
                $selectedOuRightTitle: $("#SelectedOuRightTitle"),

                showTable: function () {
                    members.$emptyInfo.hide();
                    members.$table.show();
                    members.$createNewReason.show();
                    members.$selectedOuRightTitle.text(": " + defectivePartTree.selectedOu.name).show();
                },

                hideTable: function () {
                    members.$selectedOuRightTitle.hide();
                    members.$createNewReason.hide();
                    if (members.$datatable) {
                        members.$datatable.destroy();
                    }
                    members.$table.hide();
                    members.$emptyInfo.show();
                },

                load: function () {
                    if (!defectivePartTree.selectedOu.id) {
                        members.hideTable();
                        return;
                    }

                    members.showTable();
                    if (!$.fn.DataTable.isDataTable("#DefectiveReasonsTable")) {

                    members.$datatable = members.$table.WIMIDataTable({
                        "ajax": {
                            url: abp.appAPIPath + "defectiveReasons/listDefectiveReasons",
                            data: function (d) {
                                d.partId = defectivePartTree.selectedOu.id;
                            }
                        },
                        "columns": [
                            {
                                "defaultContent": "",
                                "title": app.localize("Actions"),
                                "orderable": false,
                                "width": "15%",
                                "className": "text-center not-mobile",
                                "createdCell": function (td, cellData, rowData, row, col) {
                                        $(td)
                                            .buildActionButtons([
                                                {
                                                    title: app.localize("Editor"),
                                                    clickEvent: function () {
                                                        var partId = defectivePartTree.selectedOu.id;
                                                        var partCode = defectivePartTree.selectedOu.code;
                                                        createOrUpdateModal.open({ id: rowData.id, partId: partId, partCode: partCode }, function () {
                                                            members.add();
                                                        });
                                                    },
                                                    isShow: permissions.manage
                                                },
                                                {
                                                    title: app.localize("Delete"),
                                                    clickEvent: function () { members.remove(rowData); },
                                                    isShow: permissions.manage
                                                }
                                            ]); 
                                }
                            },
                            {
                                "data": "code",
                                "title": app.localize("ReasonCode")
                            },
                            {
                                "data": "name",
                                "title": app.localize("ReasonName")
                            },
                            {
                                "data": "memo",
                                "title": app.localize("Note")
                            },
                            {
                                "data": "creationTime",
                                "title": app.localize("CreationTime"),
                                "render": function (data, type, full, meta) {
                                    return wimi.btl.dateTimeFormat(data);
                                }
                            },
                            {
                                "data": "createUserName",
                                "title": app.localize("Founder")
                            }
                           
                        ]
                    });
                } else {

                    members.$datatable.ajax.reload(null);
                }
            },
            add: function () {
                    defectivePartTree.reload();
                    members.load();
               
            },
                remove: function (reason) {
                    var partId = defectivePartTree.selectedOu.id;
                    if (!partId) {
                        return;
                    }

                    abp.message.confirm(
                        abp.utils.formatString(app.localize("WantToDelete"), reason.name),
                        function (isConfirmed) {
                            if (isConfirmed) {
                                service.deleteDefectiveReason({
                                    partId: partId,
                                    id: reason.id
                                }).done(function () {
                                    abp.notify.success(app.localize("SuccessfullyRemoved"));
                                    defectivePartTree.incrementMemberCount(partId, -1);
                                    members.load();
                                });
                            }
                        }
                    );
                },

                openAddModal: function () {
                    var partId = defectivePartTree.selectedOu.id;
                    var partCode = defectivePartTree.selectedOu.code;
                    if (!partId) {
                        return;
                    }
                    createOrUpdateModal.open({ partId: partId, partCode: partCode }, function() {
                        members.add();
                    });
            },
            init: function () {

                $("#CreateNewReason").click(function (e) {
                    e.preventDefault();
                    members.openAddModal();
                });

                members.hideTable();
            }
        };
        members.init();
        defectivePartTree.init();

    });
})();