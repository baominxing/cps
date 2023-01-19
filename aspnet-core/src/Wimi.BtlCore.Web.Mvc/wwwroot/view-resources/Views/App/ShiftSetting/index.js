(function () {
    $(function () {
        var _shiftAppService = abp.services.app.shift;

        var _createShiftSolutionModal = new app.ModalManager({
            viewUrl: abp.appPath + "ShiftSetting/CreateShiftSolutionModal",
            scriptUrl: abp.appPath + "view-resources/Views/App/ShiftSetting/_CreateShiftSolutionModal.js",
            modalClass: "CreateShiftSolutionModal"
        });

        var _createShiftInfoModal = new app.ModalManager({
            viewUrl: abp.appPath + "ShiftSetting/CreateShiftInfoModal",
            scriptUrl: abp.appPath + "view-resources/Views/App/ShiftSetting/_CreateShiftInfoModal.js",
            modalClass: "CreateShiftInfoModal",
            modalSize: "modal-lg"
        });

        var _permissions = {
            shiftSettingManage: abp.auth.hasPermission("Pages.BasicData.ShiftSetting.Manage")
        };
        var shiftInfo;
        var shiftSolutionTree = {
            $tree: $("#ShiftSolutionTree"),
            $emptyInfo: $("#ShiftSolutionTreeEmptyInfo"),
            $addShiftSolution: $("#AddShiftSolution"),

            show: function () {
                shiftSolutionTree.$emptyInfo.hide();
                shiftSolutionTree.$tree.show();
            },

            hide: function () {
                shiftSolutionTree.$emptyInfo.show();
                shiftSolutionTree.$tree.hide();
            },

            unitCount: 0,

            setUnitCount: function (unitCount) {
                shiftSolutionTree.unitCount = unitCount;
                if (unitCount) {
                    shiftSolutionTree.show();
                } else {
                    shiftSolutionTree.hide();
                }
            },

            refreshUnitCount: function () {
                shiftSolutionTree.setUnitCount(shiftSolutionTree.$tree.jstree("get_json").length);
            },

            selectedOu: {
                id: null,
                name: null,

                set: function (ouInTree) {
                    if (!ouInTree) {
                        shiftSolutionTree.selectedOu.id = null;
                        shiftSolutionTree.selectedOu.name = null;
                    } else {
                        shiftSolutionTree.selectedOu.id = ouInTree.id;
                        shiftSolutionTree.selectedOu.name = ouInTree.original.name;
                    }

                    shiftInfo.load();
                }
            },

            contextMenu: function (node) {

                var items = {
                    editUnit: {
                        label: app.localize("Edit"),
                        _disabled: !_permissions.shiftSettingManage,
                        action: function (data) {
                            var instance = $.jstree.reference(data.reference);

                            _createShiftSolutionModal.open({
                                id: node.id
                            }, function (updatedOu) {
                                node.original.name = updatedOu.name;
                                instance.rename_node(node, shiftSolutionTree.generateTextOnTree(updatedOu));
                                shiftSolutionTree.reload();
                            });
                        }
                    },

                    'delete': {
                        label: app.localize("Delete"),
                        _disabled: !_permissions.shiftSettingManage,
                        action: function (data) {
                            var instance = $.jstree.reference(data.reference);

                            abp.message.confirm(
                                abp.utils.formatString(app.localize("AreYouSureYouWantToDeleteShiftPlan{0}") + "?", node.original.name),
                                function (isConfirmed) {
                                    if (isConfirmed) {
                                        _shiftAppService.deleteShiftSolution({
                                            id: node.id
                                        }).done(function () {
                                            abp.notify.success(app.localize("SuccessfullyDeleted"));
                                            instance.delete_node(node);
                                            shiftSolutionTree.refreshUnitCount();
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

            generateTextOnTree: function (ou) {
                var itemClass = ou.memberCount > 0 ? " ou-text-has-shiftInfo" : " ou-text-no-shiftInfo";
                return '<span class="ou-text' + itemClass + '" data-ou-id="' + ou.id + '">' + ou.name + ' (<span class="ou-text-member-count">' + ou.memberCount + '</span>) <i class="fa fa-caret-down text-muted"></i></span>';
            },

            incrementMemberCount: function (ouId, incrementAmount) {
                var treeNode = shiftSolutionTree.$tree.jstree("get_node", ouId);
                treeNode.original.memberCount = treeNode.original.memberCount + incrementAmount;
                shiftSolutionTree.$tree.jstree("rename_node", treeNode, shiftSolutionTree.generateTextOnTree(treeNode.original));
            },

            getTreeDataFromServer: function (callback) {
                _shiftAppService.getShiftSolutions({}).done(function (result) {
                    var treeData = _.map(result.items, function (item) {
                        return {
                            id: item.id,
                            parent: item.parentId ? item.parentId : "#",
                            name: item.name,
                            memberCount: item.memberCount,
                            text: shiftSolutionTree.generateTextOnTree(item),
                            state: {
                                opened: true
                            }
                        };
                    });

                    callback(treeData);
                });
            },


            addShiftSolution: function () {
                if (!_permissions.manageshiftInfo) {
                    _createShiftSolutionModal.open({
                        id: null
                    }, function (newOu) {
                        shiftSolutionTree.reload();
                    });
                }
            },

            init: function () {

                shiftSolutionTree.getTreeDataFromServer(function (treeData) {
                    shiftSolutionTree.setUnitCount(treeData.length);

                    shiftSolutionTree.$tree
                        .on("changed.jstree", function (e, data) {
                            if (data.selected.length !== 1) {
                                shiftSolutionTree.selectedOu.set(null);
                            } else {
                                var selectedNode = data.instance.get_node(data.selected[0]);
                                shiftSolutionTree.selectedOu.set(selectedNode);
                            }
                        })
                        .on("ready.jstree", function () {
                            shiftSolutionTree.$tree.jstree(true).select_node('ul > li:first');
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
                                items: shiftSolutionTree.contextMenu
                            },
                            plugins: [
                                "types",
                                "contextmenu",
                                "wholerow",
                                "dnd"
                            ]
                        });

                    shiftSolutionTree.$tree.on("click", ".ou-text .fa-caret-down", function (e) {
                        e.preventDefault();

                        var ouId = $(this).closest(".ou-text").attr("data-ou-id");
                        setTimeout(function () {
                            shiftSolutionTree.$tree.jstree("show_contextmenu", ouId);
                        }, 100);
                    });
                });

                shiftSolutionTree.$addShiftSolution.click(function (e) {
                    e.preventDefault();
                    shiftSolutionTree.addShiftSolution();
                });


            },


            reload: function () {
                shiftSolutionTree.getTreeDataFromServer(function (treeData) {
                    shiftSolutionTree.setUnitCount(treeData.length);
                    shiftSolutionTree.$tree.jstree(true).settings.core.data = treeData;
                    shiftSolutionTree.$tree.jstree("refresh");
                });
            }
        };
        shiftInfo = {
            $table: $("#ShiftInfoTable"),
            $datatable: null,
            $addShiftInfo: $("#AddShiftInfo"),
            $editShiftInfo: $("#EditShiftInfo"),

            showTable: function () {
                shiftInfo.$table.show();
            },

            hideTable: function () {
                shiftInfo.$table.hide();
            },

            load: function () {
                if (!shiftSolutionTree.selectedOu.id) {
                    shiftInfo.hideTable();
                    return;
                }

                shiftInfo.showTable();

                if (!$.fn.DataTable.isDataTable("#ShiftInfoTable")) {

                    shiftInfo.$datatable = shiftInfo.$table.WIMIDataTable({
                        "ajax": {
                            url: abp.appAPIPath + "shift/getShiftInfos",
                            data: function (d) {
                                d.id = shiftSolutionTree.selectedOu.id;
                            }
                        },
                        "searching": false,
                        "order": [],
                        "columns": [
                            {
                                data: "name",
                                title: app.localize("ShiftName"),
                                orderable: false,
                                width: "10%"
                            },
                            {

                                data: "startTime",
                                title: app.localize("StartTime"),
                                orderable: false,
                                width: "10%",
                                render: function (data) {
                                    return data.substring(11, 16);
                                }
                            },
                            {

                                title: app.localize("EndTime"),
                                searchable: false,
                                orderable: false,
                                data: "endTime",
                                width: "10%",
                                render: function (data) {
                                    return data.substring(11, 16);
                                }
                            },
                            {

                                data: function (row, type, val, meta) {
                                    var endTime = moment(row.endTime);
                                    var startTime = moment(row.startTime);
                                    if (endTime > startTime) {
                                        var diffMinute = endTime.diff(startTime, 'minute');
                                        return Math.round(diffMinute / 60 * 100) / 100;
                                    }
                                    else {
                                        var timeHourMajor = 23 + endTime.hour() - startTime.hour();
                                        var timeHourLittle = Math.round((endTime.minute() - startTime.minute() + 60) / 60 * 100) / 100;
                                        return timeHourMajor + timeHourLittle;
                                    }
                                },
                                title: app.localize("LengthOfShift"),
                                width: "10%",
                                orderable: false
                            },
                            {
                                data: "duration",
                                title: app.localize("WorkHours"),
                                width: "10%",
                                orderable: false
                            },
                            {
                                data: "isNextDay",
                                title: app.localize("WhetherAcrossDay"),
                                width: "10%",
                                orderable: false,
                                "render": function (data) {
                                    if (data) {
                                        return '<span class="label label-success">' + app.localize("Yes") + '</span>';
                                    } else {
                                        return '<span class="label label-primary">' + app.localize("No") + '</span>';
                                    }
                                }
                            },
                            {

                                title: app.localize("CreationTime"),
                                searchable: false,
                                orderable: false,
                                data: "creationTime",
                                width: "20%",
                                render: function (data) {
                                    return data.substring(0, 19).replace("T", " ");
                                }
                            }
                        ]
                    });
                } else {

                    shiftInfo.$datatable.ajax.reload(null);
                }
            },

            addShiftInfo: function () {
                if (shiftSolutionTree.selectedOu.id == null) {
                    abp.message.warn(app.localize("PleaseSelectTheCorrespondingShiftSchedule"));
                    return;
                }

                if (!_permissions.manageshiftInfo) {
                    _createShiftInfoModal.open({
                        id: shiftSolutionTree.selectedOu.id
                    }, function (newOu) {
                    });
                }
            },

            editShiftInfo: function () {

                if (shiftSolutionTree.selectedOu.id == null) {
                    abp.message.warn(app.localize("PleaseSelectTheCorrespondingShiftSchedule"));
                    return;
                }

                if (!_permissions.manageshiftInfo) {
                    _createShiftInfoModal.open({
                        id: shiftSolutionTree.selectedOu.id
                    }, function (newOu) {
                        shiftSolutionTree.reload();
                    });
                }
            },

            deleteShiftInfo: function (rowData) {
                //从数据库中删除这笔数据
                var param =
                    { Id: rowData.id }
                _shiftAppService.deleteShiftInfo(param).done(function () {
                    abp.notify.success(app.localize("SuccessfullyDeleted"));
                    shiftSolutionTree.reload();
                });
            },

            init: function () {

                shiftInfo.$addShiftInfo.click(function (e) {
                    e.preventDefault();
                    shiftInfo.addShiftInfo();
                });

                shiftInfo.$editShiftInfo.click(function (e) {
                    e.preventDefault();
                    shiftInfo.editShiftInfo();
                });
            }
        };
        shiftSolutionTree.init();
        shiftInfo.init();

    });
})();