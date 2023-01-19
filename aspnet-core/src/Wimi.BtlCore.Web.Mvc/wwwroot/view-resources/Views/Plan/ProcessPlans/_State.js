//# sourceURL=dynamic_CreateOrUpdatePlan.js
(function () {
    app.modals.State = function () {

        var _modalManager;
        var processPlanService = abp.services.app.processPlan;
        var $createOrUpdateForm = null;


        this.init = function (modalManager) {
            _modalManager = modalManager;

            $createOrUpdateForm = _modalManager.getModal().find("form[name=createOrUpdateForm]");
            if ($(".status").val() === "Complete") {
                $createOrUpdateForm.find("input").prop("disabled", "disabled");
                $createOrUpdateForm.find("select").prop("disabled", "disabled");
                _modalManager.getModal().find(".save-button").hide();
            }
        };


        this.save = function () {
            if (!$createOrUpdateForm.valid()) {
                return;
            }


            var parameters = {
                status: $createOrUpdateForm.find(".status").val()
            };

            if ($createOrUpdateForm.data("isEdit")) {
                parameters.id = $createOrUpdateForm.find("#Id").val();
            }

            _modalManager.setBusy(true);
            processPlanService.updateProcessPlanState(parameters).done(function (result) {
                abp.notify.info(app.localize("SavedSuccessfully"));
                _modalManager.setResult(result);
                _modalManager.close();
            }).always(function () {
                _modalManager.setBusy(false);
            }).fail(function () {
                status: $createOrUpdateForm.find(".status").val("New");
            });
        };
    };
})();