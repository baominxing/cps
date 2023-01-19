//# sourceURL=dynamicWorkOrderLoginModalScript.js
(function () {
    app.modals.WorkOrderLoginModal = function () {
        var _modalManager,
            _args,
            $form,
            $machineSelect = $("#machineSelect"),
            $btnLogin = $("#btnWorkOrderLogin"),
            service = abp.services.app.loginReport,
            commonService = abp.services.app.commonLookup;

        var machineReportModal = new app.ModalManager({
            viewUrl: abp.appPath + "LoginReport/MachineReportModal",
            scriptUrl: abp.appPath + "view-resources/Views/Orders/LoginReport/_MachineReportModal.js",
            modalClass: "MachineReportModal"
        });

        this.init = function (modalManager, args) {
            _modalManager = modalManager;
            _args = args;
            $form = _modalManager.getModal().find("form[name='LoginForm']");

            commonService.getDeviceGroupAndMachineWithPermissions().done(function (response) {
                var machines = _.chain(response.machines)
                    .filter(
                        function (item) {
                            return _.contains(response.grantedGroupIds, item.deviceGroupId);
                        })
                    .map(function (item) {
                        return {
                            id: item.id,
                            text: item.name
                        };
                    });

                $machineSelect.select2({
                    data: machines._wrapped,
                    multiple: true,
                    placeholder: app.localize("PleaseSelect"),
                    language: {
                        noResults: function () {
                            return app.localize("PleaseMaintainTheEquipment");
                        }
                    }
                });
            });
        }

        this.shown = function () {
            var tasktableObject = {
                $table: $("#workOrderTaskTable"),
                datatables: null,
                load: function () {
                    tasktableObject.datatables = tasktableObject.$table.WIMIDataTable({
                        "ajax": {
                            url: abp.appAPIPath + "loginReport/ListWorkOrderTasks",
                            data: { id: _args.workOrderId }
                        },
                        serverSide: true,
                        retrieve: true,
                        responsive: false,
                        ordering: false,
                        scrollX: true,
                        columns: [
                            {
                                "defaultContent": "",
                                "title": app.localize("EquipmentReporting"),
                                "orderable": false,
                                "width": "50px",
                                "className": "action",
                                "createdCell": function (td, cellData, rowData, row, col) {
                                    var $td = $(td);
                                    $('<button type="button" class="btn btn-default btn-xs"><i class="fa fa-edit"></i></button> ')
                                        .appendTo($td)
                                        .click(function () {
                                            tasktableObject.machineReport(rowData);
                                        });
                                }
                            },
                            {
                                "data": "machineName",
                                "title": app.localize("MachineName")
                            },
                            {
                                "orderable": false,
                                "data": "putVolume",
                                "title": app.localize("PutVolume")
                            },
                            {
                                "orderable": false,
                                "data": "aimVolume",
                                "title": app.localize("TargetQuantity")
                            },
                            {
                                "data": "outputCount",
                                "title": app.localize("OutPutCount")
                            },
                            {
                                "data": "qualifiedCount",
                                "title": app.localize("QualifiedNumber")
                            },
                            {
                                "data": "defectiveCount",
                                "title": app.localize("DefectiveNumber")
                            }, {
                                "data": "startTime",
                                "title": app.localize("StartTime"),
                                "render": function (data) {
                                    return wimi.btl.dateTimeFormat(data);
                                }
                            },
                            {
                                "data": "endTime",
                                "title": app.localize("EndTime"),
                                "render": function (data) {
                                    return wimi.btl.dateTimeFormat(data);
                                }
                            },
                            {
                                "data": "userName",
                                "title": app.localize("Operator")
                            }
                        ]
                    });
                },
                reload: function () {
                    this.datatables.ajax.reload(null);
                },
                machineReport: function (rowData) {
                    machineReportModal.open({ workOrderTaskId: rowData.id }, function (result) {
                        tasktableObject.reload();
                    });
                }
            }

            //工单登录
            $btnLogin.click(function () {
                var selectedValue = $machineSelect.select2('data');
                if (selectedValue.length === 0) {
                    abp.message.error(app.localize("PleaseSelectEquipment")+"！");
                    return false;
                }

                var param = {
                    id: _args.workOrderId,
                    machineIdList: _.pluck(selectedValue, 'id')
                };
                service.login(param).done(function () {
                    $machineSelect.select2().val('').trigger("change");
                    tasktableObject.reload();
                    abp.notify.info(app.localize("SavedSuccessfully"));
                }).fail(function () {
                    abp.notify.info(app.localize("LoginFailed")+"！");
                });
            });

            tasktableObject.load();
        }
    }
})();
