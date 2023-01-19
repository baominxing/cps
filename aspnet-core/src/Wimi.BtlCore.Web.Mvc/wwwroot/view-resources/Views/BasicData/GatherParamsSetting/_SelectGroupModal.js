//# sourceURL=dynamic_SelectGroupModal.js
(function ($) {
    app.modals.SelectGroupModal = function() {
        var _basicDataService = abp.services.app.basicData;
        var _modalManager;
        var _$gatherParamsForm = null;
        var tree = new MachinesTree();
        var machineId = null;
        this.init = function(modalManager) {

            _modalManager = modalManager;

            _$gatherParamsForm = _modalManager.getModal().find("form[name=GatherParamForm]");
            _$gatherParamsForm.validate();
            var $machineTree = modalManager.getModal().find("div.machines-tree");
            $machineGroupTree = modalManager.getModal().find("div.machines-group-tree");
            machineId = modalManager.getModal().find("input[name=MachineId]").val();
            tree.init($machineTree, true);
            tree.initGroup($machineGroupTree);
            //tree.setSelectAll();
        };

        this.shown = function () {
            //var mcontent = _modalManager.getModal().find(".modal-content");
            //var li = _modalManager.getModal().find(".jstree-node");
            //li.click(function () {
            //    mcontent.removeAttr("style");
            //});
        }

        this.save = function () {
            var machineIdList = tree.getSelectedMachineIds();
            _modalManager.setBusy(true);
            _basicDataService.copyParameterToMachines({ "MachineIds": machineIdList, Id: machineId })
                .done(function(result) {
                    _modalManager.setResult(result);
                    _modalManager.close();
                  abp.notify.info(app.localize("SavedSuccessfully"));
                   
                }).always(function() {
                    _modalManager.setBusy(false);
                });
        };
    };
})()