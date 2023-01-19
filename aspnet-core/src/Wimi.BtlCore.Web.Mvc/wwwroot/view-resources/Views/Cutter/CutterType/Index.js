(function () {
    $(function () {

        var cutterAppService = abp.services.app.cutter;

        var permissions = {
            manage: abp.auth.hasPermission("Pages.Cutter.CutterType.Manage")
        };

        var _createOrUpdateModalForType = new app.ModalManager({
            viewUrl: abp.appPath + "CutterType/CreateOrUpdateModalForType",
            scriptUrl: abp.appPath + "view-resources/Views/Cutter/CutterType/_CreateOrUpdateModalForType.js",
            modalClass: "CreateOrUpdateModalForType"
        });

        var _createOrUpdateModalForModel = new app.ModalManager({
            viewUrl: abp.appPath + "CutterType/CreateOrUpdateModalForModel",
            scriptUrl: abp.appPath + "view-resources/Views/Cutter/CutterType/_CreateOrUpdateModalForModel.js",
            modalClass: "CreateOrUpdateModalForModel"
        });
        var cutterModels;
        var cutterTypeTree = {
            $tree: $("#cutterTypeTree"),
            $btnCreateRootCutterType: $('#btnCreateRootCutterType'),
            $emptyInfo: $("#cutterTypeTreeEmptyInfo"),

            show: function () {
                cutterTypeTree.$emptyInfo.hide();
                cutterTypeTree.$tree.show();
            },

            hide: function () {
                cutterTypeTree.$emptyInfo.show();
                cutterTypeTree.$tree.hide();
            },

            unitCount: 0,

            setUnitCount: function (unitCount) {
                cutterTypeTree.unitCount = unitCount;
                if (unitCount) {
                    cutterTypeTree.show();
                } else {
                    cutterTypeTree.hide();
                }
            },

            refreshUnitCount: function () {
                cutterTypeTree.setUnitCount(cutterTypeTree.$tree.jstree("get_json").length);
            },

            selectedOu: {
                id: null,
                name: null,
                code: null,

                set: function (ouInTree) {
                    if (!ouInTree) {
                        cutterTypeTree.selectedOu.id = null;
                        cutterTypeTree.selectedOu.name = null;
                        cutterTypeTree.selectedOu.code = null;
                    } else {
                        cutterTypeTree.selectedOu.id = ouInTree.id;
                        cutterTypeTree.selectedOu.name = ouInTree.original.displayName;
                        cutterTypeTree.selectedOu.code = ouInTree.original.code;
                    }

                    cutterModels.load();
                }
            },

            contextMenu: function (node) {

                var items = {
                    editUnit: {
                        label: app.localize("Edit"),
                        _disabled: !permissions.manage,
                        action: function (data) {
                            var instance = $.jstree.reference(data.reference);

                            _createOrUpdateModalForType.open({
                                id: node.id,
                                isCreateFromContextMenu: false
                            }, function (updatedOu) {
                                node.original.displayName = updatedOu.name;
                                instance.rename_node(node, cutterTypeTree.generateTextOnTree(updatedOu));
                            });
                        }
                    },

                    addSubUnit: {
                        label: app.localize("NewChildType"),
                        _disabled: !permissions.manage,
                        action: function () {
                            cutterTypeTree.addUnit(node.id, true);
                        }
                    },

                    addMember: {
                        label: app.localize("NewToolType"),
                        _disabled: !permissions.manage,
                        action: function () {
                            cutterModels.add();
                        }
                    },

                    delete: {
                        label: app.localize("Delete"),
                        _disabled: !permissions.manage,
                        action: function (data) {
                            var instance = $.jstree.reference(data.reference);

                            abp.message.confirm(
                                app.localize("DeleteCutterTypeConfirm{0}", node.original.displayName),
                                function (isConfirmed) {
                                    if (isConfirmed) {
                                        cutterAppService.deleteCutterType({
                                            id: node.id
                                        }).done(function () {
                                            abp.notify.success(app.localize("SuccessfullyDeleted"));
                                            instance.delete_node(node);
                                            cutterTypeTree.refreshUnitCount();
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

            addUnit: function (parentId, isCreateFromContextMenu) {
                var instance = $.jstree.reference(cutterTypeTree.$tree);

                _createOrUpdateModalForType.open({
                    id: parentId,
                    isCreateFromContextMenu: isCreateFromContextMenu
                }, function (newOu) {
                    instance.create_node(
                        parentId ? instance.get_node(parentId) : "#",
                        {
                            id: newOu.id,
                            parent: newOu.parentId ? newOu.parentId : "#",
                            code: newOu.code,
                            displayName: newOu.displayName,
                            memberCount: 0,
                            text: cutterTypeTree.generateTextOnTree(newOu),
                            state: {
                                opened: true
                            }
                        });

                    cutterTypeTree.refreshUnitCount();
                    cutterTypeTree.reload();
                });
            },

            generateTextOnTree: function (ou) {
                var itemClass = ou.isCutterNoPrefixCanEdit ? " ou-text-has-members" : " ou-text-no-members";
                return '<span class="ou-text ' + itemClass + '" data-ou-id="' + ou.id + '">' + ou.name + '[' + app.localize("ToolTypeCount") + ':' + ou.memberCount + ',' + app.localize("Prefix") + ':' + ou.cutterNoPrefix + '] <i class="fa fa-caret-down text-muted"></i></span>';
            },

            incrementMemberCount: function (ouId, incrementAmount) {
                var treeNode = cutterTypeTree.$tree.jstree("get_node", ouId);
                treeNode.original.memberCount = treeNode.original.memberCount + incrementAmount;
                cutterTypeTree.$tree.jstree("rename_node", treeNode, cutterTypeTree.generateTextOnTree(treeNode.original));
            },

            getTreeDataFromServer: function (callback) {
                cutterAppService.getCutterTypeList().done(function (result) {
                    var treeData = _.map(result, function (item) {
                        return {
                            id: item.id,
                            parent: item.pId ? item.pId : "#",
                            code: item.code,
                            displayName: item.name,
                            memberCount: item.memberCount,
                            text: cutterTypeTree.generateTextOnTree(item),
                            state: {
                                opened: true
                            }
                        };
                    });

                    callback(treeData);
                });
            },

            showHelp: function () {
                layer.open({
                    type: 4,
                    area: ['310px', '75px'],
                    shade: 0,
                    time: 2000,
                    content: [app.localize("CutterTypeTreeCodeTip"), '.fa-question-circle-o']
                });
            },

            init: function () {
                cutterTypeTree.getTreeDataFromServer(function (treeData) {
                    cutterTypeTree.setUnitCount(treeData.length);

                    cutterTypeTree.$tree
                        .on("changed.jstree", function (e, data) {
                            if (data.selected.length !== 1) {
                                cutterTypeTree.selectedOu.set(null);
                            } else {
                                var selectedNode = data.instance.get_node(data.selected[0]);
                                cutterTypeTree.selectedOu.set(selectedNode);
                            }
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
                                    "icon": "fa fa-cutlery tree-item-icon-color icon-lg"
                                },
                                "file": {
                                    "icon": "fa fa-cutlery tree-item-icon-color icon-lg"
                                }
                            },
                            contextmenu: {
                                items: cutterTypeTree.contextMenu
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

                    cutterTypeTree.$btnCreateRootCutterType.click(function (e) {
                        e.preventDefault();
                        cutterTypeTree.addUnit(null, false);
                    });

                    cutterTypeTree.$tree.on("click", ".ou-text .fa-caret-down", function (e) {
                        e.preventDefault();

                        var ouId = $(this).closest(".ou-text").attr("data-ou-id");
                        setTimeout(function () {
                            cutterTypeTree.$tree.jstree("show_contextmenu", ouId);
                        }, 100);
                    });

                    $(".fa-question-circle-o").click(function (e) {
                        e.preventDefault();
                        cutterTypeTree.showHelp();
                    });
                });
            },

            reload: function () {
                cutterTypeTree.getTreeDataFromServer(function (treeData) {
                    cutterTypeTree.setUnitCount(treeData.length);
                    cutterTypeTree.$tree.jstree(true).settings.core.data = treeData;
                    cutterTypeTree.$tree.jstree("refresh");
                });
            }
        };
        cutterModels = {
            $table: $("#table"),
            $datatable: null,
            $emptyInfo: $("#cutterModelEmptyInfo"),
            $btnCreateCutterModel: $('#btnCreateCutterModel'),
            $selectedCutterTypeRightTitle: $("#SelectedCutterTypeRightTitle"),

            showTable: function () {
                cutterModels.$emptyInfo.hide();
                cutterModels.$table.show();
                cutterModels.$btnCreateCutterModel.show();
                cutterModels.$selectedCutterTypeRightTitle.text("[" + cutterTypeTree.selectedOu.name + "]").show();
            },

            hideTable: function () {
                cutterModels.$selectedCutterTypeRightTitle.hide();
                cutterModels.$btnCreateCutterModel.hide();
                if (cutterModels.$datatable) {
                    cutterModels.$datatable.destroy();
                }
                cutterModels.$table.hide();
                cutterModels.$emptyInfo.show();
            },

            load: function () {
                if (!cutterTypeTree.selectedOu.id) {
                    cutterModels.hideTable();
                    return;
                }

                cutterModels.showTable();

                if (cutterModels.$dataTable != null) {
                    cutterModels.$dataTable.destroy();
                    cutterModels.$dataTable = null;
                    cutterModels.$table.empty();
                }

                cutterAppService.getCutterParameterList().done(function (result) {
                    //构造列名
                    var cols = cutterModels.getDynamicCols(result);

                    cutterAppService.getCutterModelList({ cutterTypeId: cutterTypeTree.selectedOu.id }).done(function (result) {
                        cutterModels.$dataTable = cutterModels.$table.WIMIDataTable({
                            serverSide: false,
                            scrollX: true,
                            scrollY: true,
                            scrollCollapse: true,
                            responsive: false,
                            retrieve: true,
                            data: result,
                            columns: cols
                        });
                      
                    });
                });
            },

            getDynamicCols: function (data) {
                var cols = [
                    {
                        title: app.localize("Actions"),
                        data: null,
                        width: "15%",
                        className: "text-center",
                        orderable: false,
                        render: function () {
                            return "";
                        },
                        createdCell: function (td, cellData, rowData, row, col) {
                            $(td).buildActionButtons([
                                {
                                    title: app.localize("Editor"),
                                    clickEvent: function () {
                                        cutterModels.edit(rowData);
                                    },
                                    isShow: permissions.manage
                                },
                                {
                                    title: app.localize("Delete"),
                                    clickEvent: function () {
                                        cutterModels.remove(rowData);
                                    },
                                    isShow: permissions.manage,
                                    className: "btn-danger"
                                }
                            ]);
                        }
                    },
                    {
                        data: "name",
                        title: app.localize("CutterType")
                    },
                    {
                        data: "cutterNoPrefix",
                        title: app.localize("ToolPrefixNumber")
                    },
                    {
                        data: "toolLifeCountingMethod",
                        title: app.localize("ToolLifeCountingMethod"),
                        render: function (data) {
                            return abp.localization.localize("Cutter-" + data);
                        }
                    },
                    {
                        data: "originalLife",
                        title: app.localize("OriginalLife")
                    },
                    {
                        data: "warningLife",
                        title: app.localize("WarningLife")
                    }
                ];

                //构造动态列
                for (var i = 0; i < data.length; i++) {
                    var parameter = {
                        data: data[i].code.toLowerCase(),
                        title: data[i].name
                    };

                    cols.push(parameter);
                }

                cols.push({
                    data: "creatorName",
                    title: app.localize("Creator")
                });
                cols.push({
                    data: "creationTime",
                    title: app.localize("CreationTime"),
                    render: function (data) {
                        return moment(data).format("YYYY-MM-DD HH:mm:ss");
                    }
                });
                cols.push({
                    data: "lastModifierName",
                    title: app.localize("Modifier"),
                    render: function (data) {
                        if (data == null) {
                            return "";
                        }
                        return data;
                    }
                });
                cols.push({
                    data: "lastModificationTime",
                    title: app.localize("ModificationTime"),
                    render: function (data) {
                        if (data == null) {
                            return "";
                        }
                        return moment(data).format("YYYY-MM-DD HH:mm:ss");
                    }
                });

                return cols;
            },

            add: function () {
                var cutterTypeId = cutterTypeTree.selectedOu.id;
                if (!cutterTypeId) {
                    return;
                }

                _createOrUpdateModalForModel.open({ id: null, cutterTypeId: cutterTypeId }, function () {
                    cutterTypeTree.reload();
                    cutterModels.load();
                });
            },

            edit: function (rowdata) {
                var cutterTypeId = cutterTypeTree.selectedOu.id;
                if (!cutterTypeId) {
                    return;
                }

                _createOrUpdateModalForModel.open({ id: rowdata.id, cutterTypeId: rowdata.cutterTypeId }, function () {
                    cutterTypeTree.reload();
                    cutterModels.load();
                });
            },

            remove: function (rowdata) {
                abp.message.confirm(
                    app.localize("DeleteToolTypeConfirm{0}", rowdata.name),
                    function (isConfirmed) {
                        if (isConfirmed) {
                            cutterAppService.deleteCutterModel({
                                id: rowdata.id
                            }).done(function () {
                                abp.notify.success(app.localize("SuccessfullyRemoved"));
                                cutterTypeTree.reload();
                                cutterModels.load();
                            });
                        }
                    }
                );
            },

            init: function () {

                cutterModels.$btnCreateCutterModel.click(function (e) {
                    e.preventDefault();
                    cutterModels.add();
                });

                cutterModels.hideTable();


            }
        };
        cutterModels.init();
        cutterTypeTree.init();
    });
})();