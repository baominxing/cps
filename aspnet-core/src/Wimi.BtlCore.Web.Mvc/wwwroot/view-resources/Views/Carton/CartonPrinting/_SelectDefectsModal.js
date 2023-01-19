(function () {
    app.modals.SelectDefectsModal = function () {
        var _modalManager,
            _args,
            service = abp.services.app.defectiveReasons;
        var rendered = Handlebars.compile($("#checkbox-template").html());

        var reasonIds = [];

        this.init = function (modalManager, args) {
            _modalManager = modalManager;
            _args = args;
        };

        this.shown = function () {

            var defectivePartTree = {
                $tree: $("#OrganizationUnitEditTree"),
                $emptyInfo: $("#OrganizationUnitTreeEmptyInfo"),
                show: function () {
                    defectivePartTree.$emptyInfo.hide();
                    defectivePartTree.$tree.show();

                },
                hide: function () {
                    defectivePartTree.$emptyInfo.show();
                    defectivePartTree.$tree.hide();

                },
                unitCount: 0,
                setUnitCount: function (unitCount) {
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
                            .on("ready.jstree", function () {
                                defectivePartTree.$tree.jstree(true).select_node('ul > li:first');
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
                        defectivePartTree.$tree.on("click", ".ou-text .fa-caret-down", function (e) {
                            e.preventDefault();

                            var ouId = $(this).closest(".ou-text").attr("data-ou-id");
                            setTimeout(function () {
                                defectivePartTree.$tree.jstree("show_contextmenu", ouId);
                            }, 100);
                        });

                    });
                }
            };

            var members = {
                load: function () {
                    service.listDefectiveReasonsByPartId({ Id: defectivePartTree.selectedOu.id }).done(function (response) {
                        _.each(response, function (item) {
                            if ($.inArray(item.id+"", reasonIds) >= 0) {
                                item.checked = true;
                            }
                        });

                        $("#defectiveReasons").html(rendered(response));

                        $('.minimal').on('ifChecked', function (event) {
                            reasonIds.push($(this).val());
                        });

                        $('.minimal').on('ifUnchecked', function (event) {
                            var value = $(this).val();
                            reasonIds = _.filter(reasonIds, function (item) { return item != value});
                        });

                        $('.minimal').iCheck({
                            checkboxClass: 'icheckbox_minimal-red'
                        });
                    });
                }
            }

            defectivePartTree.init();
        };

        this.save = function () {
            var result = reasonIds;
            _modalManager.setResult(result);
            _modalManager.close();
        }
    };
})();