//# sourceURL=dynamic_ChangerProductModal.js
(function () {
    var changeProductOption= function(machineProcessService,_modalManager,obj) {
        machineProcessService
            .changeMachineProduct(obj)
            .done(function () {
                abp.notify.info(app.localize("SavedSuccessfully"));
                _modalManager.setResult();
                _modalManager.close();
            }).always(function () {
                _modalManager.setBusy(false);
            });
    }
    app.modals.ChangeProductModal = function () {
        var _modalManager;
        var machineContainer;
        var productContainer;
        var orderContainer;
        var changeProductForm;
        var machineProcessService;
        var commonService;
        this.init = function (modalManager, args) {
            _modalManager = modalManager;
            machineProcessService = abp.services.app.machineProcess;
            commonService = abp.services.app.commonLookup;
            changeProductForm = _modalManager.getModal().find("form[name=ChangeProductForm]");
            machineContainer = _modalManager.getModal().find("#machineContainer");
            productContainer = _modalManager.getModal().find("#productContainer");
            orderContainer = _modalManager.getModal().find("#orderContainer");
            //加载设备
            commonService.getDeviceGroupAndMachineWithPermissions().done(function (response) {
                var machines = _.chain(response.machines)
                    .filter(
                        function (item) {
                            return _.contains(response.grantedGroupIds, item.deviceGroupId);
                        })
                    .map(function (item) {
                        var machine = { id: item.id, text: item.name };
                        return machine;
                    });

                var machinesData = [];
                _.each(_.groupBy(machines._wrapped, "id"),
                    function (item) {
                        machinesData.push(item[0]);
                    });
                machineContainer.select2({
                    data: machinesData,
                    multiple: false,
                    language: {
                        noResults: function () {
                            return app.localize("PleaseMaintainTheEquipment");
                        }
                    }
                });
            });
            //加载产品
            machineProcessService.listProductType().done(function (response) {
                var productData = _.map(response,
                    function (item) {
                        return {
                            id: item.name,
                            text: item.value
                        }
                    });

                productContainer.select2({
                    data: productData,
                    multiple: false,
                    language: {
                        noResults: function () {
                            return app.localize("PleaseMaintainProduct");
                        }
                    }
                }).on("select2:select", function (e) {
                    //加载工序
                    machineProcessService.listProcessType({ Id: e.params.data.id }).done(function (returnData) {
                        orderContainer.empty();
                        var processData = _.map(returnData,
                            function (item) {
                                return {
                                    id: item.name,
                                    text: item.value
                                }
                            });
                        orderContainer.select2({
                            data: processData,
                            multiple: false,
                            language: {
                                noResults: function () {
                                    return app.localize("PleaseMaintainProcess");
                                }
                            }
                        });
                    });
                }); 
                //var first = response[0];
                //if (first)
                //    first = first.name;
                var id = response.length > 0 ? response[0].name : 0;
                machineProcessService.listProcessType({ Id: id }).done(function (result) {
                    var processData = _.map(result,
                        function (item) {
                            return {
                                id: item.name,
                                text: item.value
                            }
                        });
                    orderContainer.select2({
                        data: processData,
                        multiple: false,
                        language: {
                            noResults: function () {
                                return app.localize("PleaseMaintainProcess");
                            }
                        }
                    });
                });
            });
        };
        this.save = function () {

            changeProductForm.validate({
                rules: {
                    MachineId: { required: true},
                    ProductId: { required: true},
                    ProcessId: { required: true }
                },
                messages: {
                    MachineId: { required: app.localize("PleaseSelectEquipment") },
                    ProductId: { required: app.localize("ChooseProduct") },
                    ProcessId: { required: app.localize("PleaseSelectProcess") }
                },
                errorPlacement: function (error, element) {        
                    error.appendTo(element.next());                            
                }
            });

            if (!changeProductForm.valid()) {
                return false;
            }

            var obj = changeProductForm.serializeFormToObject();
           
            machineProcessService.checkMachineRecord(obj).done(function (response) {
                //true表示可以换产，false：设备正在加工需要提示是否换产
                if (response === "OK") {
                     _modalManager.setBusy(true);
                    changeProductOption(machineProcessService, _modalManager,obj);
                   
                }
                else {
                    if (response === "No") {
                        abp.message.error(app.localize("DoNotNeedToChangeProduction"));
                        return;
                    }
                    abp.message.confirm(app.localize("WhetherToChangeProduction"),
                        function(result) {
                            if (result === true) {
                                changeProductOption(machineProcessService, _modalManager,obj);
                            }
                            else {
                                return;
                            }
                        });
                }

            });
        };

    };
})();