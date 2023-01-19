//# sourceURL=dynamicctretreteScript.js

(function () {
    app.modals.CreateOrUpdateModal = function () {
        var standardTimeSevice = abp.services.app.standardTime,
            _modalManager,
            $standardTimeForm = null;

        this.init = function (modalManager) {
            _modalManager = modalManager;
            $standardTimeForm = _modalManager.getModal().find("form[name=StandardTimeForm]");
            $("#Product").select2();
            $("#Process").select2();

         
        }
       
        this.save = function () {
            if (!$standardTimeForm.valid()) {
                return;
            }
         
            var standardTime = $standardTimeForm.serializeFormToObject();
            if (standardTime.CycleRate % 1 !== 0) {
                abp.notify.error(app.localize("MagnificationIsNotIntegers"))
                return;
            }
            _modalManager.setBusy(true);
            standardTimeSevice.createOrUpdateStandardTime(standardTime)
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