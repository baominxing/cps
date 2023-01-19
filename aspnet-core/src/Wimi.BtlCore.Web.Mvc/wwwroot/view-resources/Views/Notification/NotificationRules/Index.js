(function () {
    $(function () {
        var TriggerWithShift = 2;

        var notificationAppService = abp.services.app.notification;

        var permissions = {
            manage: abp.auth.hasPermission("Pages.Notification.Rules.Manage")
        };

        var createOrUpdateModalForRuleType = new app.ModalManager({
            viewUrl: abp.appPath + "NotificationRules/CreateOrUpdateModalForRulesType",//controller path
            scriptUrl: abp.appPath + "view-resources/Views/Notification/NotificationRules/_CreateOrUpdateModalForRulesType.js",//referenced js path
            modalClass: "CreateOrUpdateModalForRulesType"//model object name
        });

        var createOrUpdateModalForRule = new app.ModalManager({
            viewUrl: abp.appPath + "NotificationRules/CreateOrUpdateModalForRules",//controller path
            scriptUrl: abp.appPath + "view-resources/Views/Notification/NotificationRules/_CreateOrUpdateModalForRules.js",//referenced js path
            modalClass: "CreateOrUpdateModalForRules"//model object name
        });

        var notificationRules;

        var notificationRulesTree = {
            tree: $("#notificationRulesTree"),

            btnCreateNotificationRulesType: $('#btnCreateNotificationRulesType'),

            emptyInfo: $("#notificationRulesTreeEmptyInfo"),

            show: function () {
                notificationRulesTree.emptyInfo.hide();
                notificationRulesTree.tree.show();
            },

            hide: function () {
                notificationRulesTree.emptyInfo.show();
                notificationRulesTree.tree.hide();
            },

            unitCount: 0,

            setUnitCount: function (unitCount) {
                notificationRulesTree.unitCount = unitCount;
                if (unitCount) {
                    notificationRulesTree.show();
                } else {
                    notificationRulesTree.hide();
                }
            },

            refreshUnitCount: function () {
                notificationRulesTree.setUnitCount(notificationRulesTree.tree.jstree("get_json").length);
            },

            selectedOu: {
                id: null,
                name: null,
                code: null,
                triggerType: null,

                set: function (ouInTree) {
                    if (!ouInTree) {
                        notificationRulesTree.selectedOu.id = null;
                        notificationRulesTree.selectedOu.name = null;
                        notificationRulesTree.selectedOu.code = null;
                    } else {
                        notificationRulesTree.selectedOu.id = ouInTree.id;
                        notificationRulesTree.selectedOu.name = ouInTree.original.displayName;
                        notificationRulesTree.selectedOu.code = ouInTree.original.code;
                    }

                    notificationRules.load();
                }
            },

            contextMenu: function (node) {

                var items = {
                    editUnit: {
                        label: app.localize("Edit"),
                        _disabled: !permissions.manage,
                        action: function (data) {
                            var instance = $.jstree.reference(data.reference);

                            createOrUpdateModalForRuleType.open({
                                id: node.id,
                                isEditMode: true
                            }, function (updatedOu) {
                                node.original.displayName = updatedOu.name;
                                instance.rename_node(node, notificationRulesTree.generateTextOnTree(updatedOu));
                            });
                        }
                    },

                    addMember: {
                        label: app.localize("NewMessageRules"),
                        _disabled: !permissions.manage,
                        action: function () {
                            notificationRules.add();
                        }
                    },

                    delete: {
                        label: app.localize("Delete"),
                        _disabled: !permissions.manage,
                        action: function (data) {
                            var instance = $.jstree.reference(data.reference);

                            abp.message.confirm(
                                app.localize("DeleteNotificationTypesConfirmTip"),
                                function (isConfirmed) {
                                    if (isConfirmed) {
                                        notificationAppService.deleteNotificationRule({
                                            id: node.id
                                        }).done(function () {
                                            abp.notify.success(app.localize("SuccessfullyDeleted"));
                                            instance.delete_node(node);
                                            notificationRulesTree.refreshUnitCount();
                                            notificationRulesTree.reload();
                                            notificationRulesTree.tree.jstree(true).select_node('ul > li:first');
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

            addUnit: function (id, isEditMode) {
                var instance = $.jstree.reference(notificationRulesTree.tree);

                createOrUpdateModalForRuleType.open({
                    id: id,
                    isEditMode: isEditMode
                }, function (newOu) {
                    instance.create_node("#",
                        {
                            id: newOu.id,
                            parent: "#",
                            code: newOu.triggerType,
                            displayName: newOu.displayName,
                            memberCount: 0,
                            text: notificationRulesTree.generateTextOnTree(newOu),
                            state: {
                                opened: true
                            }
                        });

                    notificationRulesTree.refreshUnitCount();
                    notificationRulesTree.reload();
                });
            },

            generateTextOnTree: function (ou) {
                return '<span class="ou-text ou-text-has-members" data-ou-id="' + ou.id + '">' + ou.name + '(' + ou.memberCount + ') [' + ou.deviceGroupNames + ']<i class="fa fa-caret-down text-muted"></i></span>';
            },

            incrementMemberCount: function (ouId, incrementAmount) {
                var treeNode = notificationRulesTree.tree.jstree("get_node", ouId);
                treeNode.original.memberCount = treeNode.original.memberCount + incrementAmount;
                notificationRulesTree.tree.jstree("rename_node", treeNode, notificationRulesTree.generateTextOnTree(treeNode.original));
            },

            getTreeDataFromServer: function (callback) {
                notificationAppService.listNotificationRule().done(function (result) {
                    var treeData = _.map(result, function (item) {
                        return {
                            id: item.id,
                            parent: "#",
                            code: item.triggerType,
                            displayName: item.name,
                            memberCount: item.memberCount,
                            text: notificationRulesTree.generateTextOnTree(item),
                            state: {
                                opened: true
                            }
                        };
                    });

                    callback(treeData);
                });
            },

            init: function () {
                notificationRulesTree.getTreeDataFromServer(function (treeData) {
                    notificationRulesTree.setUnitCount(treeData.length);

                    notificationRulesTree.tree
                        .on("changed.jstree", function (e, data) {
                            if (data.selected.length !== 1) {
                                notificationRulesTree.selectedOu.set(null);
                            } else {
                                var selectedNode = data.instance.get_node(data.selected[0]);
                                notificationRulesTree.selectedOu.set(selectedNode);
                            }
                        })
                        .on("ready.jstree", function () {
                            notificationRulesTree.tree.jstree(true).select_node('ul > li:first');
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
                                    "icon": "fa fa-bookmark tree-item-icon-color icon-lg"
                                },
                                "file": {
                                    "icon": "fa fa-bookmark tree-item-icon-color icon-lg"
                                }
                            },
                            contextmenu: {
                                items: notificationRulesTree.contextMenu
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


                    notificationRulesTree.tree.on("click", ".ou-text .fa-caret-down", function (e) {
                        e.preventDefault();
                        var ouId = $(this).closest(".ou-text").attr("data-ou-id");
                        setTimeout(function () {
                            notificationRulesTree.tree.jstree("show_contextmenu", ouId);
                        }, 100);
                    });

                    notificationRulesTree.btnCreateNotificationRulesType.click(function (e) {
                        e.preventDefault();
                        notificationRulesTree.addUnit(null, false);
                    });
                });
            },

            reload: function () {
                notificationRulesTree.getTreeDataFromServer(function (treeData) {
                    notificationRulesTree.setUnitCount(treeData.length);
                    notificationRulesTree.tree.jstree(true).settings.core.data = treeData;
                    notificationRulesTree.tree.jstree("refresh");
                });
            }
        };

        notificationRules = {
            table: $("#table"),
            dataTable: null,
            emptyInfo: $("#notificationRulesDetailEmptyInfo"),
            btnCreateNotificationRulesDetail: $('#btnCreateNotificationRulesDetail'),
            selectedNotificationRulesDetailRightTitle: $("#SelectedNotificationRulesDetailRightTitle"),
            showTable: function () {
                notificationRules.emptyInfo.hide();
                this.trydestroytable();
                notificationRules.table.show();
                notificationRules.selectedNotificationRulesDetailRightTitle.text("[" + notificationRulesTree.selectedOu.name + "]").show();
            },
            hideTable: function () {
                notificationRules.selectedNotificationRulesDetailRightTitle.hide();
                this.trydestroytable();
                notificationRules.emptyInfo.show();
            },
            trydestroytable: function () {
                if (notificationRules.dataTable) {
                    notificationRules.dataTable.destroy();
                    notificationRules.dataTable = null;
                    notificationRules.table.empty();
                }
            },
            add: function () {
                var notificationRuleId = notificationRulesTree.selectedOu.id;
                if (!notificationRuleId) {
                    abp.message.error(app.localize("PleaseSelectMessageType"));
                    return;
                }

                createOrUpdateModalForRule.open({ id: null, notificationRuleId: notificationRuleId }, function () {
                    notificationRulesTree.reload();
                });
            },

            edit: function (rowdata) {
                var notificationRuleId = notificationRulesTree.selectedOu.id;
                if (!notificationRuleId) {
                    return;
                }

                createOrUpdateModalForRule.open({ id: rowdata.id, notificationRuleId: notificationRuleId }, function () {
                    notificationRulesTree.reload();
                });
            },

            remove: function (rowdata) {
                abp.message.confirm(

                    app.localize("DeleteMessageRulesConfirm{0}", rowdata.triggerCondition),
                    function (isConfirmed) {
                        if (isConfirmed) {
                            notificationAppService.deleteNotificationRuleDetail({
                                id: rowdata.id
                            }).done(function () {
                                abp.notify.success(app.localize("SuccessfullyRemoved"));
                                notificationRulesTree.reload();
                            });
                        }
                    }
                );
            },

            init: function () {

                notificationRules.btnCreateNotificationRulesDetail.click(function (e) {
                    e.preventDefault();
                    notificationRules.add();
                });

                notificationRules.hideTable();
            },

            load: function () {
                if (!notificationRulesTree.selectedOu.id) {
                    notificationRules.hideTable();
                    return;
                }
                notificationRules.showTable();

                var cols = [
                    {
                        title: app.localize("Action"),
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
                                        notificationRules.edit(rowData);
                                    },
                                    isShow: permissions.manage
                                },
                                {
                                    title: app.localize("Delete"),
                                    clickEvent: function () {
                                        notificationRules.remove(rowData);
                                    },
                                    isShow: permissions.manage,
                                    className: "btn-danger"
                                }
                            ]);
                        }
                    },
                    {
                        data: "order",
                        title: app.localize("SortSeq")
                    }
                ];

                if (notificationRulesTree.selectedOu.code === TriggerWithShift) {
                    cols.push({
                        data: "shiftInfoName",
                        title: app.localize("TriggerCondition"),
                        render: function (data) {
                            return data;
                        }
                    });
                } else {
                    cols.push({
                        data: "triggerCondition",
                        title: app.localize("TriggerCondition"),
                        render: function (data) {
                            return data;
                        }
                    });
                }

                cols.push({
                    data: "isEnabled",
                    title: app.localize("IsEnable"),
                    render: function (data) {
                        if (data) {
                            return '<span class="label label-success">' + app.localize("Yes") + '</span>';
                        } else {
                            return '<span class="label label-primary">' + app.localize("No") + '</span>';
                        }
                    }
                });

                cols.push({
                    data: "noticeUserNames",
                    title: app.localize("NotificationOfficer")
                });
                notificationAppService.listNotificationRuleDetail({ id: notificationRulesTree.selectedOu.id }).done(function (result) {

                    for (var i = 0; i < result.length; i++) {
                        result[i].order = i + 1;
                    }

                    notificationRules.dataTable = notificationRules.table.WIMIDataTable({
                        serverSide: false,
                        retrieve: true,
                        responsive: false,
                        order: [],
                        data: result,
                        columns: cols
                    });
                });
            }
        };

        notificationRulesTree.init();

        notificationRules.init();
    });
})();