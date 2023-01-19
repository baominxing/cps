(function () {
    app.modals.CreateOrUpdateModal = function () {
        var basicDataService = abp.services.app.basicData,
            _modalManager,
            _$stateInfoForm = null;

        this.init = function (modalManager) {
            _modalManager = modalManager;
            _$stateInfoForm = _modalManager.getModal().find('form[name=StateInfoForm]');
            _$stateInfoForm.find('.colorpicker-element').colorpicker();

            var typeValue = document.getElementById("Type").getAttribute("typevalue");
            document.getElementById('Type').value = typeValue;

        }

        this.shown = function () {

            $("#Type").select2({
                multiple: false,
                minimumResultsForSearch: -1,
                language: {
                    noResults: function () {
                        return app.localize("NoMatchingData");
                    }
                }
            });

            $("#IsPlaned").select2({
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
            if (!_$stateInfoForm.valid()) {
                return;
            }
            var stateInfo = _$stateInfoForm.serializeFormToObject();

            _modalManager.setBusy(true);
            basicDataService.creatOrUpdateStateInfo(stateInfo)
                .done(function (result) {
                    abp.notify.info(app.localize("SavedSuccessfully"));
                    _modalManager.setResult(result);
                    abp.event.trigger("app.CreateOrUpdateModalSaved");
                    _modalManager.close();
                }).always(function () {
                    _modalManager.setBusy(false);
                });
        }
    }
})(jQuery);