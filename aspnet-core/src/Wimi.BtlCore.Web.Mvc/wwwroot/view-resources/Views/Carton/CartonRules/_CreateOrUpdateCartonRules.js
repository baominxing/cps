//# sourceURL=dynamic_CreateOrUpdateCraft.js
(function () {
    app.modals.CreateOrUpdateRule = function () {
        var _modalManager;
        var service = abp.services.app.cartonRule;
        var $createOrUpdateForm = null;
        this.init = function (modalManager) {
            _modalManager = modalManager;
            $createOrUpdateForm = _modalManager.getModal().find("form[name=createOrUpdateForm]");
        };

        this.shown = function () {
            $("#IsActive").select2({
                multiple: false,
                minimumResultsForSearch: -1,
                language: {
                    noResults: function () {
                        return app.localize("NoMatchingData");
                    }
                }
            });
        }

        this.save = function () {
            if (!$createOrUpdateForm.valid()) {
                return;
            }

            var parameters = $createOrUpdateForm.serializeFormToObject();

            if ($("#IsActive").val() == "Yes") {
                parameters.IsActive = true;
            } else {
                parameters.IsActive = false;
            }

            _modalManager.setBusy(true);
            service.createOrUpdateCartonRule(parameters).done(function (result) {
                abp.notify.info(app.localize("SavedSuccessfully"));
                _modalManager.setResult(result);
                _modalManager.close();
            }).always(function () {
                _modalManager.setBusy(false);
            }); 
        };
    };
})();