(function () {
    $(function () {

        var _service = abp.services.app.dmpMachine;

        var _permissions = {
            manageMembers: abp.auth.hasPermission("Pages.Administration.DmpMachineSetting.Manage")
        };

        var _userLookupModal = app.modals.LookupModal.create({
            title: app.localize("PleaseSelectEquipment"),
            serviceMethod: "commonLookup/findMachinesToDmp"
        });

        var members;
        var dmpTree = {
            $tree: $("#OrganizationUnitEditTree"),

            $emptyInfo: $("#OrganizationUnitTreeEmptyInfo"),

            show: function () {
                dmpTree.$emptyInfo.hide();
                dmpTree.$tree.show();
            },

            hide: function () {
                dmpTree.$emptyInfo.show();
                dmpTree.$tree.hide();
            },

            unitCount: 0,

            setUnitCount: function (unitCount) {
                dmpTree.unitCount = unitCount;
                if (unitCount) {
                    dmpTree.show();
                } else {
                    dmpTree.hide();
                }
            },

            refreshUnitCount: function () {
                dmpTree.setUnitCount(dmpTree.$tree.jstree("get_json").length);
            },

            selectedOu: {
                id: null,
                displayName: null,
                code: null,

                set: function (ouInTree) {
                    if (!ouInTree) {
                        dmpTree.selectedOu.id = null;
                        dmpTree.selectedOu.displayName = null;
                        dmpTree.selectedOu.code = null;
                    } else {
                        dmpTree.selectedOu.id = ouInTree.id;
                        dmpTree.selectedOu.displayName = ouInTree.original.displayName;
                        dmpTree.selectedOu.code = ouInTree.original.code;
                    }

                    members.load();
                }
            },

            contextMenu: function (node) {

                var items = {       
                    addMember: {
                        label: app.localize("MachineSetting_Create"),
                        _disabled: !_permissions.manageMembers,
                        action: function () {
                            members.openAddModal();
                        }
                    },
                };
                return items;
            },

            generateTextOnTree: function (ou) {
                var itemClass = ou.memberCount > 0 ? " ou-text-has-members" : " ou-text-no-members";
                return '<span class="ou-text' + itemClass + '" data-ou-id="' + ou.id + '">' + ou.displayName + ' (<span class="ou-text-member-count">' + ou.memberCount + '</span>) <i class="fa fa-caret-down text-muted"></i></span>';
            },

            incrementMemberCount: function (ouId, incrementAmount) {
                var treeNode = dmpTree.$tree.jstree("get_node", ouId);
                treeNode.original.memberCount = treeNode.original.memberCount + incrementAmount;
                dmpTree.$tree.jstree("rename_node", treeNode, dmpTree.generateTextOnTree(treeNode.original));
            },

            getTreeDataFromServer: function (callback) {
                _service.getDmps({}).done(function (result) {
                    var treeData = _.map(result.items, function (item) {
                        return {
                            id: item.id,
                            parent:"#",
                            code: item.code,
                            displayName: item.displayName,
                            memberCount: item.memberCount,
                            text: dmpTree.generateTextOnTree(item),
                            state: {
                                opened: true
                            }
                        };
                    });

                    callback(treeData);
                });
            },

            init: function () {
                dmpTree.getTreeDataFromServer(function (treeData) {
                    dmpTree.setUnitCount(treeData.length);

                    dmpTree.$tree
                        .on("changed.jstree", function (e, data) {
                            if (data.selected.length !== 1) {
                                dmpTree.selectedOu.set(null);
                            } else {
                                var selectedNode = data.instance.get_node(data.selected[0]);
                                dmpTree.selectedOu.set(selectedNode);
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
                                    "icon": "fa fa-folder tree-item-icon-color icon-lg"
                                },
                                "file": {
                                    "icon": "fa fa-file tree-item-icon-color icon-lg"
                                }
                            },
                            contextmenu: {
                                items: dmpTree.contextMenu
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

                    dmpTree.$tree.on("click", ".ou-text .fa-caret-down", function (e) {
                        e.preventDefault();

                        var ouId = $(this).closest(".ou-text").attr("data-ou-id");
                        setTimeout(function () {
                            dmpTree.$tree.jstree("show_contextmenu", ouId);
                        }, 100);
                    });
                });
            },

            reload: function () {
                dmpTree.getTreeDataFromServer(function (treeData) {
                    dmpTree.setUnitCount(treeData.length);
                    dmpTree.$tree.jstree(true).settings.core.data = treeData;
                    dmpTree.$tree.jstree("refresh");
                });
            }
        };
        members = {
            $table: $("#OuMembersTable"),
            $datatable: null,
            $emptyInfo: $("#OuMembersEmptyInfo"),
            $addUserToOuButton: $("#AddUserToOuButton"),
            $batchDeleteButton: $("#BatchDeleteButton"),
            $selectedOuRightTitle: $("#SelectedOuRightTitle"),

            showTable: function () {
                members.$emptyInfo.hide();
                members.$table.show();
                members.$addUserToOuButton.show();
                members.$batchDeleteButton.show();
                members.$selectedOuRightTitle.text(": " + dmpTree.selectedOu.displayName).show();
            },

            hideTable: function () {
                members.$selectedOuRightTitle.hide();
                members.$addUserToOuButton.hide();
                members.$batchDeleteButton.hide();
                if (members.$datatable) {
                    members.$datatable.destroy();
                }
                members.$table.hide();
                members.$emptyInfo.show();
            },

            load: function () {
                if (!dmpTree.selectedOu.id) {
                    members.hideTable();
                    return;
                }

                members.showTable();

                if (!$.fn.DataTable.isDataTable("#OuMembersTable")) {

                    members.$datatable = members.$table.WIMIDataTable({
                        "ajax": {
                            url: abp.appPath + "api/services/app/dmpMachine/getDmpMachines",
                            data: function (d) {
                                d.id = dmpTree.selectedOu.id;
                            }
                        },
                        columnDefs: [{
                            'targets': 0,
                            'searchable': false,
                            'orderable': false,
                            'className': 'dt-body-center',
                            'render': function () {
                                return '<input type="checkbox" class="multiselect"  >';
                            }
                        }],
                        "columns": [
                            {
                                "orderable": false,
                                "title": "<input  type='checkbox' id='select_all'/>",
                                "width": "30px"
                            },
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
                var dmpId = dmpTree.selectedOu.id;
                if (!dmpId) {
                    return;
                }

                _service.addMachineListToDmp({
                    dmpId: dmpId,
                    machineIdList: _.pluck(machineList, "value")
                }).done(function (data) {
                    if (data === null) {
                        abp.notify.success(app.localize("SuccessfullyAdded"));
                    } else {
                        abp.message.error(data);
                    }
                    dmpTree.reload();
                    members.load();
                });
            },

            remove: function (machine) {
                var dmpId = dmpTree.selectedOu.id;
                if (!dmpId) {
                    return;
                }

                abp.message.confirm(
                    app.localize("RemoveMachineOutOfDmp{0}{1}", machine.name, dmpTree.selectedOu.displayName),
                    function (isConfirmed) {
                        if (isConfirmed) {
                            _service.removeMachineFromDmp({
                                dmpId: dmpId,
                                machineId: machine.id
                            }).done(function () {
                                abp.notify.success(app.localize("SuccessfullyRemoved"));
                                dmpTree.incrementMemberCount(dmpId, -1);
                                members.load();
                            });
                        }
                    }
                );
            },

            openAddModal: function () {
                var ouId = dmpTree.selectedOu.id;
                if (!ouId) {
                    return;
                }

                _userLookupModal.open({ dmpId: ouId }, function (selectedItems) {
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

        $(document).on('click', '#select_all', function () {
            var rows = members.$datatable.rows({ 'search': 'applied' }).nodes();
            $('input[type="checkbox"]', rows).prop('checked', this.checked);
        })

        $('#OuMembersTable tbody').on('change',
            'input[type="checkbox"]',
            function () {
                if (!this.checked) {
                    var el = $("#select_all").get(0);
                    if (el && el.checked && ('indeterminate' in el)) {
                        el.indeterminate = true;
                    }
                }
            });

        $("#BatchDeleteButton").click(function (e) {
            var selected = [];

            var ouId = dmpTree.selectedOu.id;
            if (!ouId) {
                return;
            }

            _.each(members.$table.find("input.multiselect[type='checkbox']:checked"),
                function (key) {
                    var rowData = members.$datatable.rows($(key).closest('tr')).data()[0];
                    if (rowData) {
                        selected.push(rowData.id);
                    }
                });

            if (selected.length > 0) {
                _service.batchRemoveMachineFromDmp({ DmpId: ouId, MachineIds: selected}).done(function () {
                    abp.notify.success(app.localize("SuccessfullyRemoved"));
                    dmpTree.incrementMemberCount(ouId, selected.length * (-1));
                    members.load();
                    $('#select_all').prop('checked', false);
                })
            } else {
                abp.message.error(app.localize("PleaseSelectAtLeastOneRecord"));
            }
        });

        members.init();
        dmpTree.init();

    });
})();