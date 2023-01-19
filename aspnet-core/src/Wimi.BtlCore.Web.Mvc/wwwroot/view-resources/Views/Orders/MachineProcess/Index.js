(function () {
    $(function () {
        var machineProcessService = abp.services.app.machineProcess;
        var commonService = abp.services.app.commonLookup;

        var table = {
            machineProcessTable: $("#MachineProcessTable"),
            machineProcessDataTable: null,
            load: function () {
                if (this.machineProcessDataTable) {
                    this.machineProcessDataTable.destory();
                    this.machineProcessTable.empty();
                }

                this.machineProcessDataTable = this.machineProcessTable.WIMIDataTable({
                    "ajax": {
                        url: abp.appAPIPath + "machineProcess/listMachineProcess",
                        data: function (d) {
                            return $.extend(d, table.getParamter());
                        }
                    },
                    "scrollX": true,
                    "responsive": false,
                    "order": [],
                    "columns": table.getTableColumn()
                });

            },
            getTableColumn: function () {
                return [
                    {
                        defaultContent: "",
                        title: app.localize("Actions"),
                        orderable: false,
                        width: "30px",
                        createdCell: function (td, cellData, rowData, row, col) {
                            if (rowData.endTime==null) {
                                if (abp.auth.hasPermission('Pages.Order.MachineProcess.Manage')) {
                                    $('<button class="btn btn-danger btn-xs">' +app.localize("Delete") +'</button>')
                                        .appendTo($(td))
                                        .click(function () {
                                            table.removeRow(rowData.id);
                                        });
                                }
                            }
                        }
                    },
                    {
                        data: "machineCode",
                        title: app.localize("MachineCode"),

                        orderable: false
                    },
                    {
                        data: "machineName",
                        title: app.localize("MachineName"),
                        orderable: false

                    },
                    {
                        title: app.localize("ProductCode"),
                        searchable: false,
                        orderable: false,
                        data: "productCode"

                    },
                    {
                        title: app.localize("ProductName"),
                        searchable: false,
                        orderable: false,
                        data: "productName"
                    },
                    {
                        title: app.localize("ProcessCode"),
                        searchable: false,
                        orderable: false,
                        data: "processCode"
                    },
                    {
                        data: "processName",
                        title: app.localize("ProcessName"),
                        orderable: false
                    },
                    {
                        data: "createUserName",
                        title: app.localize("Founder"),
                        orderable: false
                    },
                    {
                        data: "creationTime",
                        title: app.localize("CreationTime"),
                        orderable: true,
                        render: function (data) {
                            return wimi.btl.dateTimeFormat(data);
                        }
                    },
                    {
                        data: "changeProductUserName",
                        title: app.localize("ChangeProductionPerson"),
                        orderable: false
                    },
                    {
                        data: "endTime",
                        title: app.localize("EndTime"),
                        orderable: true,
                        render: function (data) {
                            return wimi.btl.dateTimeFormat(data);
                        }
                    }
                ];
            },
            getParamter: function () {
                var machineId = $("#machineContainer").val();
                var productId = $("#productContainer").val();
                return {
                    'MachineId': machineId == null ? "0" : machineId,
                    "ProductId": productId == null ? "0" : productId
                }
            },
            reload: function () {
                this.machineProcessDataTable.ajax.reload(null);
            },
            removeRow: function (id) {
                abp.message.confirm(app.localize("AreYouSureYouWantToDeleteIt")+"？", function (result) {
                    if (result) {
                        machineProcessService.deleteMachineProcess({ id: id }).done(function () {
                            abp.notify.info(app.localize("SuccessfullyDeleted"));
                            table.reload();
                        });
                    }
                });
               
            }
        }

        var page = {
            machineData:null,
            btnQuey: $("#btnQuery"),
            btnMachineChange: $("#btnMachineChange"),
            machineContainer: $("#machineContainer"),
            productContainer: $("#productContainer"),
            changeProductModal:null,
            init: function () {
                this.bindClickEvent();
                this.initProductSelect2();
                this.initMachinesSelect2();
                this.initModal();
                table.load();
            },
            bindClickEvent: function () {
                this.btnQuey.click(function () {
                    table.reload();
                });
                this.btnMachineChange.click(function() {
                    page.changeProductModal.open({},function() {
                        table.reload();
                    });
                });
            },
            initProductSelect2: function () {
                machineProcessService.listProductType().done(function(response) {
                    page.machineData = response;
                    response.unshift({ name: 0, value: app.localize("All") });
                    var productData = _.map(response,
                        function (item) {
                            return {
                                id: item.name,
                                text: item.value
                            }
                        });
                    page.productContainer.select2({
                        data: productData,
                        multiple: false,
                        language: {
                            noResults: function () {
                                return app.localize("PleaseMaintainProduct");
                            }
                        }
                    }).val(0).trigger('change');
                });
            },
            initMachinesSelect2: function () {
                commonService.getDeviceGroupAndMachineWithPermissions().done(function (response) {
                    var machines = _.chain(response.machines)
                        .filter(
                            function (item) {
                                return _.contains(response.grantedGroupIds, item.deviceGroupId);
                            })
                        .map(function (item) {
                            var machine = { id: item.id ,text: item.name};
                            return machine;
                        });

                    var machinesData = [{ id: 0, text: app.localize("All") }];
                    for (var n = 0; n < machines._wrapped.length; n++) {
                        machinesData.push(machines._wrapped[n]);
                    }
                    //_.each(_.groupBy(machines._wrapped, "id"),     //_.groupBy(machines._wrapped, "id")
                    //    function (item) {
                    //        machinesData.push(item[0]);
                    //    });
                    page.machineContainer.select2({
                        data: machinesData,
                        multiple: false,
                        language: {
                            noResults: function () {
                                return app.localize("PleaseMaintainTheEquipment");
                            }
                        }
                    }).val(0).trigger('change');
                
                });
               
            },
            initModal: function() {
               page.changeProductModal = new app.ModalManager({
                    viewUrl: abp.appPath + "MachineProcess/ChangeProductModal",
                   scriptUrl: abp.appPath + "view-resources/Views/Orders/MachineProcess/_ChangeProductModal.js",
                    modalClass: "ChangeProductModal"    
                });
            }

        }
        page.init();
    });
})();