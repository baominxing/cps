(function () {
    $(function () {

        var craftAppService = abp.services.app.craft;

        var craftTree = null,
            craftProcess = null,
            canRowreorder = true,
            canDelete = true;

        var permissions = {
            manage: abp.auth.hasPermission("Pages.Order.Craft.Manage")
        };

        var createOrUpdateCraft = new app.ModalManager({
            viewUrl: abp.appPath + "Craft/CreateOrUpdateCraft",
            scriptUrl: abp.appPath + "view-resources/Views/Orders/Craft/_CreateOrUpdateCraft.js",
            modalClass: "CreateOrUpdateCraft"
        });

        var processLookupModal = app.modals.LookupModal.create({
            title: app.localize("SelectionProcess"),
            serviceMethod: "commonLookup/findProcesses"
        });

        craftTree = {
            $tree: $("#craftTree"),
            $btnCreateCraft: $("#btnCreateCraft"),
            $emptyInfo: $("#craftTreeEmptyInfo"),

            show: function () {
                craftTree.$emptyInfo.hide();
                craftTree.$tree.show();
            },

            hide: function () {
                craftTree.$emptyInfo.show();
                craftTree.$tree.hide();
            },

            unitCount: 0,

            setUnitCount: function (unitCount) {
                craftTree.unitCount = unitCount;
                if (unitCount) {
                    craftTree.show();
                } else {
                    craftTree.hide();
                }
            },

            refreshUnitCount: function () {
                craftTree.setUnitCount(craftTree.$tree.jstree("get_json").length);
            },

            selectedOu: {
                id: null,
                code: null,
                name: null,

                set: function (ouInTree) {
                    if (!ouInTree) {
                        craftTree.selectedOu.id = null;
                        craftTree.selectedOu.code = null;
                        craftTree.selectedOu.name = null;
                    } else {
                        craftTree.selectedOu.id = ouInTree.id;
                        craftTree.selectedOu.code = ouInTree.original.code;
                        craftTree.selectedOu.name = ouInTree.original.name;

                        craftAppService.craftIsInProcess({ id: craftTree.selectedOu.id }).done(function (result) {
                            canDelete = canRowreorder = !result;
                            craftProcess.load();
                            if (result) {
                                if (craftProcess.$dataTable.rowReorder != null) {
                                    craftProcess.$dataTable.rowReorder.disable();
                                }
                                craftProcess.$btnSelectProcess.hide();
                            } else {
                                if (craftProcess.$dataTable.rowReorder != null) {
                                    craftProcess.$dataTable.rowReorder.enable();
                                }
                                craftProcess.$btnSelectProcess.show();
                            }
                        });
                    }
                }
            },

            contextMenu: function (node) {

                var items = {
                    edit: {
                        label: app.localize("Edit"),
                        _disabled: !permissions.manage,
                        action: function () {
                            createOrUpdateCraft.open({
                                id: node.id
                            }, function () {
                                craftTree.refreshUnitCount();
                                craftTree.reload();
                            });
                        }
                    },

                    delete: {
                        label: app.localize("Delete"),
                        _disabled: !permissions.manage,
                        action: function (data) {
                            var instance = $.jstree.reference(data.reference);

                            abp.message.confirm(
                                abp.utils.formatString(app.localize("DeleteCraft"), node.original.name),
                                function (isConfirmed) {
                                    if (isConfirmed) {
                                        craftAppService.deleteCraft({
                                            id: node.id
                                        }).done(function () {
                                            abp.notify.success(app.localize("SuccessfullyDeleted"));
                                            instance.delete_node(node);
                                            craftTree.refreshUnitCount();
                                            craftTree.reload();
                                            if (craftProcess) {
                                                craftProcess.hideTable();
                                            }
                                        }).fail(function (err) {
                                            setTimeout(function () { abp.message.error(err.message); }, 500);
                                        });;
                                    }
                                }
                                );
                        }
                    }
                };
                return items;
            },

            addUnit: function () {
                createOrUpdateCraft.open({}, function () {
                    craftTree.refreshUnitCount();
                    craftTree.reload();
                });
            },

            generateTextOnTree: function (ou) {
                return '<span class="ou-text" data-ou-id="' + ou.id + '">' + ou.name + '</span>';
            },

            getTreeDataFromServer: function (callback) {
                craftAppService.getCrafts().done(function (result) {
                    var treeData = _.map(result, function (item) {
                        return {
                            id: item.id,
                            parent: "#",
                            code: item.code,
                            name: item.name,
                            text: craftTree.generateTextOnTree(item),
                            state: {
                                opened: true
                            }
                        };
                    });

                    callback(treeData);
                });
            },

            init: function () {
                craftTree.getTreeDataFromServer(function (treeData) {
                    craftTree.setUnitCount(treeData.length);

                    craftTree.$tree
                        .on("ready.jstree", function () {
                            craftTree.$tree.jstree(true).select_node("ul > li:first");
                        })
                        .on("changed.jstree", function (e, data) {
                            if (data.selected.length !== 1) {
                                craftTree.selectedOu.set(null);
                            } else {
                                var selectedNode = data.instance.get_node(data.selected[0]);
                                craftTree.selectedOu.set(selectedNode);
                            }
                        })
                        .jstree({
                            'core': {
                                data: treeData,
                                multiple: false
                            },
                            types: {
                                "default": {
                                    "icon": "fa fa-th tree-item-icon-color icon-lg"
                                },
                                "file": {
                                    "icon": "fa fa-th tree-item-icon-color icon-lg"
                                }
                            },
                            contextmenu: {
                                items: craftTree.contextMenu
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

                    craftTree.$btnCreateCraft.click(function (e) {
                        e.preventDefault();
                        craftTree.addUnit();
                    });

                    craftTree.$tree.on("click", ".ou-text .fa-caret-down", function (e) {
                        e.preventDefault();

                        var ouId = $(this).closest(".ou-text").attr("data-ou-id");
                        setTimeout(function () {
                            craftTree.$tree.jstree("show_contextmenu", ouId);
                        }, 100);
                    });
                });
            },

            reload: function () {
                craftTree.getTreeDataFromServer(function (treeData) {
                    craftTree.setUnitCount(treeData.length);
                    craftTree.$tree.jstree(true).settings.core.data = treeData;
                    craftTree.$tree.jstree("refresh");
                });
            }
        };

        craftProcess = {
            $table: $("#table"),
            $dataTable: null,
            $emptyInfo: $("#craftProcessEmptyInfo"),
            $btnSelectProcess: $('#btnSelectProcess'),

            showTable: function () {
                craftProcess.$emptyInfo.hide();
                craftProcess.$table.show();
            },

            hideTable: function () {
                craftProcess.$btnSelectProcess.hide();
                if (craftProcess.$dataTable != null) {
                    craftProcess.$dataTable.destroy();
                    craftProcess.$dataTable = null;
                    craftProcess.$table.empty();
                }
                craftProcess.$table.hide();
                craftProcess.$emptyInfo.show();
            },

            initializeDataTable: function () {
                var cols = [
                    {
                        title: app.localize("Actions"),
                        data: null,
                        width: "10%",
                        className: "text-center",
                        orderable: false,
                        render: function () {
                            return "";
                        },
                        createdCell: function (td, cellData, rowData, row, col) {
                            if (permissions.manage && canDelete) {
                                $('<button class="btn btn-danger btn-xs">' +app.localize("Delete") +'</button>')
                                    .appendTo($(td))
                                    .click(function () {
                                        craftProcess.remove(rowData);
                                    });
                            } else {
                                $("-").appendTo($(td));
                            }
                        }
                    },
                    {
                        data: null,
                        width: "10%",
                        orderable: false,
                        title: app.localize("OperationSequence"),
                        render: function () {
                            return "";
                        },
                        createdCell: function (td, cellData, rowData, row, col) {
                            $(td).css("cursor", "n-resize");
                            $(td).append("<span>" + rowData.processOrder + "</span>");
                        }
                    },
                    {
                        data: "processCode",
                        orderable: false,
                        title: app.localize("ProcessCode")
                    },
                    {
                        data: "processName",
                        orderable: false,
                        title: app.localize("ProcessName")
                    },
                    {
                        data: "isLastProcess",
                        orderable: false,
                        title: app.localize("FinalProcedure"),
                        render: function (data) {
                            if (data) {
                                return "<span class='label label-success'>" + app.localize("Yes") + "</span>";
                            } else {
                                return "<span class='label label-warning'>" + app.localize("No") + "</span>";
                            }
                        }
                    },
                    {
                        data: "id",
                        orderable: false,
                        visible: false,
                        title: "id"
                    }
                ];

                craftProcess.$dataTable = craftProcess.$table.WIMIDataTable({
                    serverSide: true,
                    responsive: false,
                    retrieve: true,
                    paging: true,
                    rowReorder: {
                        enabled: canRowreorder,
                        selector: "td:nth-child(2)",
                        update: false
                    },
                    order: [],
                    ajax: {
                        url: abp.appAPIPath + "craft/getCraftProcesses",
                        data: function (d) {
                            d.id = craftTree.selectedOu.id;
                        },
                        type: "POST"
                    },
                    columns: cols
                });

                craftProcess.$dataTable.on("row-reorder",
                    function (e, diff, edit) {
                        var parameters = [];
                        for (var i = 0, ien = diff.length; i < ien; i++) {
                            // Get the data array for this row
                            var rowData = craftProcess.$dataTable.row(diff[i].node).data();

                            parameters.push({
                                id: rowData.id,
                                craftId: rowData.craftId,
                                increment: diff[i].newPosition - diff[i].oldPosition
                            });
                        }

                        if (parameters.length > 0) {
                            craftAppService.updateCraftProcess(parameters)
                                .done(function () {
                                    abp.notify.success(app.localize("SuccessfulOperation"));
                                    craftProcess.$dataTable.ajax.reload(null);
                                });
                        }
                    });
            },

            load: function () {

                if (!craftTree.selectedOu.id) {
                    craftProcess.hideTable();
                    return;
                }

                craftProcess.showTable();

                if (craftProcess.$dataTable != null) {
                    craftProcess.$dataTable.ajax.reload(null);
                } else {
                    craftProcess.initializeDataTable();
                }
            },

            selectProcess: function () {

                processLookupModal.open({ craftId: craftTree.selectedOu.id }, function (selectedItems) {
                    var parameters = {
                        id: craftTree.selectedOu.id,
                        processIdList: _.pluck(selectedItems, "id")
                    }
                    craftAppService.createCraftProcess(parameters).done(function () {
                        craftProcess.load();
                    });
                });
            },

            remove: function (rowData) {
                var parameters = {
                    id: rowData.id,
                    craftId: rowData.craftId
                };

                craftAppService.deleteCraftProcess(parameters).done(function () {
                    craftProcess.$dataTable.ajax.reload(null);
                });
            },

            init: function () {

                craftProcess.$btnSelectProcess.click(function (e) {
                    e.preventDefault();
                    craftProcess.selectProcess();
                });

                craftProcess.hideTable();
            }
        };

        craftTree.init();
        craftProcess.init();
    });
})();