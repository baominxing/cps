(function () {
    $(function () {
        var checkDataTable = $("#checkDataTable"),
            selectedId,
            actionButtons,
            $checkDataTables = null;

        var importDataModal = new app.ModalManager({
            viewUrl: abp.appPath + "ImportData/ImportDataModal",
            scriptUrl: abp.appPath + "view-resources/Views/BasicData/ImportData/_ImportDataModal.js",
            modalClass: "ImportDataModal"
        });

        var importTypeTree = {
            $tree: $("#importTypeTree"),
            init: function () {
                importTypeTree.$tree.jstree({
                    "core": {
                        expand_selected_onload: false
                    },
                    plugins: ["types", "wholerow", "changed"]
                }),
                    importTypeTree.$tree.on("changed.jstree", function (e, data) {
                        if (!data.node) {
                            return;
                        }
                        selectedId = importTypeTree.getSelectedNode()[0].id;
                        actionButtons.importData([]);
                    }),
                    importTypeTree.$tree.on('ready.jstree', function () {
                        importTypeTree.$tree.jstree().select_node('ul > li:first');
                    });
            },
            getSelectedNode: function () {
                return importTypeTree.$tree.jstree("get_selected", true);
            }
        };

        actionButtons = {
            init: function () {
                $("#downloadTemplate").click(function () {
                    $(this).attr('href', abp.appPath + "Download/Template/" + selectedId + ".xlsx");
                });

                $("#checkData").click(function () {
                    importDataModal.open({ Type: selectedId }, function (data) {
                        var dataSet = [];
                        switch (selectedId) {
                            case "Users":
                                dataSet = data.result.value.usersList;
                                break;
                            case "Machines":
                                dataSet = data.result.value.machinesList;
                                break;
                            default:
                                dataSet = data.result.value.gatherParamsList;
                        }
                        actionButtons.importData(dataSet);
                    });
                });
            },
            importData: function (dataSet) {
                if ($checkDataTables != null) {
                    $checkDataTables.destroy();
                    $checkDataTables = null;
                    checkDataTable.empty();
                }

                $checkDataTables = checkDataTable.WIMIDataTable({
                    searching: true,
                    serverSide: false,
                    responsive: false,
                    order: [0, 'asc'],
                    scrollX: true,
                    data: dataSet,
                    columns: actionButtons.formatColumn(selectedId)
                });
            },
            formatColumn: function (type) {
                if (type === "Users") {
                    return this.getUsersColumns();
                } else if (type === "Machines") {
                    return this.getMachineColumns();
                } else {
                    return this.getGatherParamsColumns();
                }
            },
            getMachineColumns: function () {
                return [
                    {
                        title: app.localize("SerialNumber"),
                        data: 'seq'
                    },
                    {
                        title: app.localize("MachineCode"),
                        data: 'code'
                    },
                    {
                        title: app.localize("MachineName"),
                        data: 'name'
                    },
                    {
                        title: app.localize("MachineType"),
                        data: 'type'
                    }, {
                        title: app.localize("MachineDesc"),
                        data: 'description'
                    },
                    {
                        title: app.localize("IsEnable"),
                        data: 'isActive',
                        render: function (data) {
                            if (data) {
                                return '<span class="label label-success">' + app.localize("Yes") + '</span>';
                            } else {
                                return '<span class="label label-success">' + app.localize("No") + '</span>';
                            }
                        }
                    },
                    {
                        title: app.localize("ImportResults"),
                        data: 'errorMessage',
                        render: function (data) {
                            if (data.indexOf('Success') === -1) {
                                return '<span class="text-red">' + data + '</span>';
                            } else {
                                return '<span class="text-light-blue">' + app.localize("ImportSuccess") + '</span>';
                            }
                        }
                    }
                ];
            },
            getUsersColumns: function () {
                return [
                    {
                        title: app.localize("SerialNumber"),
                        data: 'seq'
                    },
                    {
                        title: app.localize("Name"),
                        data: 'name'
                    },
                    {
                        title: app.localize("UId"),
                        data: 'userName'
                    }, {
                        title: app.localize("Password"),
                        data: 'password'
                    },
                    {
                        title: app.localize("RoleName"),
                        data: 'rolesName'
                    },
                    {
                        title: app.localize("WeChatId"),
                        data: 'weChatId'
                    },
                    {
                        title: app.localize("IsEnable"),
                        data: 'isActive',
                        render: function (data) {
                            if (data) {
                                return '<span class="label label-success">' + app.localize("Yes") + '</span>';
                            } else {
                                return '<span class="label label-success">' + app.localize("No") + '</span>';
                            }
                        }
                    },
                    {
                        title: app.localize("UpdatePswdNextLogin"),
                        data: 'shouldChangePasswordOnNextLogin',
                        render: function (data) {
                            if (data) {
                                return '<span class="label label-success">' + app.localize("Yes") + '</span>';
                            } else {
                                return '<span class="label label-success">' + app.localize("No") + '</span>';
                            }
                        }
                    },
                    {
                        title: app.localize("ImportResults"),
                        data: 'errorMessage',
                        render: function (data) {
                            if (data.indexOf('Success') === -1) {
                                return '<span class="text-red">' + data + '</span>';
                            } else {
                                return '<span class="text-light-blue">' + app.localize("ImportSuccess") + '</span>';
                            }
                        }
                    }
                ];
            },

            getGatherParamsColumns: function () {
                return [
                    {
                        title: app.localize("SerialNumber"),
                        data: 'seq'
                    },
                    {
                        orderable: false,
                        title: app.localize("MachineId"),
                        data: 'machineId'
                    },
                    {
                        orderable: false,
                        title: app.localize("GatherParamsName"),
                        data: 'code'
                    },
                    {
                        orderable: false,
                        title: app.localize("GatherParamsDescription"),
                        data: 'description'
                    },
                    {
                        orderable: false,
                        title: app.localize("DeviceAddress"),
                        data: 'deviceAddress'
                    },
                    {
                        orderable: false,
                        title: app.localize("DataType"),
                        data: 'dataTypeString'
                    },
                    {
                        orderable: false,
                        title: app.localize("DataAccess"),
                        data: 'accessString'
                    },
                    {
                        orderable: false,
                        title: app.localize("ValueFactor"),
                        data: 'valueFactor'
                    },
                    {
                        orderable: false,
                        title: app.localize("DefaultValue"),
                        data: 'defaultValue'
                    },
                    {
                        title: app.localize("ImportResults"),
                        data: 'errorMessage',
                        render: function (data) {
                            if (data.indexOf('Success') === -1) {
                                return '<span class="text-red">' + data + '</span>';
                            } else {
                                return '<span class="text-light-blue">' + app.localize("ImportSuccess") + '</span>';
                            }
                        }
                    }
                ];
            }
        };

        importTypeTree.init();
        actionButtons.init();
    });
})();