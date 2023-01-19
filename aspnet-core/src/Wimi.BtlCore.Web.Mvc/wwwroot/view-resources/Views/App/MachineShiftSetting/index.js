(function () {
    $(function () {
        var shiftAppService = abp.services.app.shift;
        var machineShiftTable = $("#MachineShiftTable");
        var machineShiftDataTable;
        var machineTree = new MachinesTree();

        var createDeviceShiftSolutionModal = new app.ModalManager({
            viewUrl: abp.appPath + "MachineShiftSetting/CreateDeviceShiftSolutionModal",
            scriptUrl: abp.appPath + "view-resources/Views/App/MachineShiftSetting/CreateDeviceShiftSolutionModal.js",
            modalClass: "CreateDeviceShiftSolutionModal"
        });

        var createDeviceGroupShiftSolutionModal = new app.ModalManager({
            viewUrl: abp.appPath + "MachineShiftSetting/CreateDeviceGroupShiftSolutionModal",
            scriptUrl: abp.appPath + "view-resources/Views/App/MachineShiftSetting/CreateDeviceGroupShiftSolutionModal.js",
            modalClass: "CreateDeviceGroupShiftSolutionModal"
        });

        var viewDeviceShiftSolutionHistoryModal = new app.ModalManager({
            viewUrl: abp.appPath + "MachineShiftSetting/ViewDeviceShiftSolutionHistoryModal",
            scriptUrl: abp.appPath + "view-resources/Views/App/MachineShiftSetting/ViewDeviceShiftSolutionHistoryModal.js",
            modalClass: "ViewDeviceShiftSolutionHistoryModal"
        });

        var shift = {
            getShiftSolutions: function () {
                shiftAppService.getShiftSolutions().done(function (data) {
                    for (var i = 0; i < data.items.length; i++) {
                        $("#ShiftSolutions")
                            .append($("<option></option>").val(data.items[i].id).html(data.items[i].name));
                    }

                    $("#ShiftSolutions").select2({
                        multiple: false,
                        minimumResultsForSearch: -1,
                        language: {
                            noResults: function () {
                                return app.localize("NoMatchingData");
                            }
                        }
                    });

                });
            },

            init: function () {
                shift.getShiftSolutions();
                this.bindingAssociateDeviceGroup();
                this.bindingBatchDeleteEvent();
                this.loadTable();
            },
            loadTable: function () {
                if (machineShiftDataTable) {
                    machineShiftDataTable.destroy();
                    machineShiftDataTable = null;
                    machineShiftTable.empty();
                }

                var queryType = machineTree.getQueryType();

                var param = getParamsList(queryType);

                shiftAppService.getMachineShiftSolutions(param)
                    .done(function (data) {
                        machineShiftDataTable = machineShiftTable.WIMIDataTable({
                            serverSide: false,
                            data: data,
                            order: [],
                            searching: true,
                            columns: [
                                {
                                    title: app.localize("Action"),
                                    serverSide: false,
                                    data: null,
                                    width: "15%",
                                    className: "text-center",
                                    orderable: false,
                                    render: function () {
                                        return "";
                                    },
                                    createdCell: function (td, cellData, rowData, row, col) {

                                        $('<button class="btn btn-danger btn-xs">' + app.localize("RemoveRelationShift") + '</button>')
                                            .appendTo($(td))
                                            .click(function () {
                                                if (rowData.shiftSolutionName === "NotAssociated") {
                                                    abp.message.warn(app.localize("CurrentMachineNotRelateAnyShift"));
                                                    return;
                                                }
                                                deleteDeviceShiftSolution(rowData.id, rowData.machineId);
                                            });


                                        $('<button class="btn btn-default btn-xs">' + app.localize("SettingShift") + '</button>')
                                            .appendTo($(td))
                                            .click(function () {
                                                associateDeviceShiftSolution(rowData.machineId);
                                            });

                                        $('<button class="btn btn-default btn-xs">' + app.localize("View") + '</button>')
                                            .appendTo($(td))
                                            .click(function () {
                                                viewMachineShiftSolutionHistory(rowData.machineId);
                                            });
                                    }
                                },
                                {
                                    title: app.localize("Machines"),
                                    orderable: false,
                                    data: "machineName"
                                },
                                {
                                    title: app.localize("BelongToGroup"),
                                    orderable: false,
                                    data: "machineGroupName"
                                },
                                {
                                    title: app.localize("CurrentShiftPlan"),
                                    orderable: false,
                                    data: "shiftSolutionName",
                                    render: function (data) {
                                        return abp.localization.localize(data);
                                    }
                                },
                                {
                                    title: app.localize("EntryIntoForceTime"),
                                    orderable: false,
                                    data: "startTime",
                                    render: function (data) {
                                        if (data == null) {
                                            return abp.localization.localize("NotAssociated");
                                        } else {
                                            return moment(data).format("YYYY-MM-DD");
                                        }
                                    }
                                },
                                {
                                    title: app.localize("FailureTime"),
                                    orderable: false,
                                    data: "endTime",
                                    render: function (data) {
                                        if (data == null) {
                                            return abp.localization.localize("NotAssociated");
                                        } else {
                                            return moment(data).format("YYYY-MM-DD");
                                        }
                                    }
                                },
                                {
                                    title: app.localize("CreationTime"),
                                    orderable: false,
                                    data: "creationTime",
                                    render: function (data) {
                                        if (data == null) {
                                            return abp.localization.localize("NotAssociated");
                                        } else {
                                            return moment(data).format("YYYY-MM-DD");
                                        }

                                    }
                                }
                            ]
                        });
                        machineShiftDataTable.draw(false);
                    });
            },
            bindingBatchDeleteEvent: function () {
                $('#batchesDelete').click(function () {
                    abp.message.confirm(app.localize("DeleteAllShiftAfterToday"),
                        function (isConfirmed) {
                            if (isConfirmed) {
                                shiftAppService
                                    .batchDeleteMachineShift({ machineIdList: machineTree.getSelectedMachineIds() })
                                    .done(
                                        function () {
                                            shift.loadTable();
                                            abp.notify.success(app.localize("SuccessfullyCleared"));
                                        });
                            }
                        });
                });
            },
            bindingAssociateDeviceGroup: function () {
                // 批量排班
                $("#AssociateDeviceGroup")
                    .on("click",
                        function () {
                            associateDeviceGroupShiftSolution();
                        });
            }
        };

        function getParamsList(queryType) {
            var machineIdList;
            if (queryType === "0") {
                machineIdList = machineTree.getSelectedMachineIds();
            } else {
                machineIdList = machineTree.getSelectedGroupIds();
            }

            var shiftSolutionId = $("#ShiftSolutions").val();

            return {
                shiftSolutionId: shiftSolutionId,
                ids: machineIdList.join(","),
                queryType: queryType
            };
        }

        //查询
        $("#btnQuery").on("click", function (e) {
            e.preventDefault();
            shift.loadTable();
        });

        function associateDeviceShiftSolution(id) {
            createDeviceShiftSolutionModal.open({ id: id }, function () {
                $("#btnQuery").click();
            });
        }

        function viewMachineShiftSolutionHistory(id) {
            viewDeviceShiftSolutionHistoryModal.open({ id: id }, function () {
                $("#btnQuery").click();
            });
        }

        function associateDeviceGroupShiftSolution() {
            createDeviceGroupShiftSolutionModal.open({}, function () {
                shift.loadTable();
            });
        }

        //删除
        function deleteDeviceShiftSolution(id, machineId) {
            shiftAppService.checkBeforeDeleteMachineShiftSolution({ id: id, machineId: machineId }).done(function (result) {
                if (result.length > 0) {
                    abp.message.confirm(result + ',' + app.localize("ContinueConfirm"),
                        function (isConfirmed) {
                            if (isConfirmed) {
                                shiftAppService.deleteMachineShiftSolution({ id: id, machineId: machineId }).done(function () {
                                    abp.notify.success(app.localize("SuccessfullyDeleted"));
                                    $("#btnQuery").click();
                                });
                            }
                        }
                    );
                }
                else {
                    abp.message.confirm(
                        app.localize("DeleteMachineShiftsConfirm"),
                        function (isConfirmed) {
                            if (isConfirmed) {
                                shiftAppService.deleteMachineShiftSolution({ id: id, machineId: machineId }).done(function () {
                                    abp.notify.success(app.localize("SuccessfullyDeleted"));
                                    $("#btnQuery").click();
                                });
                            }
                        }
                    );
                }
            });

        }
        machineTree.init($("div.machines-tree"), true);
        machineTree.initGroup($("div.machines-group-tree"));
        machineTree.setSelectAll();

        shift.init();
    });
})();