(function () {
    app.modals.BatchSwitchModal = function() {
        var _modalManager,
            _args,
            $form,
            service = abp.services.app.machineGatherParam;

        this.init = function(modalManager, args) {
            _modalManager = modalManager;
            _args = args;

            $form = _modalManager.getModal().find('form[name=BatchSwitchModalForm]');
            $(".drop-downlist").select2({ minimumResultsForSearch: -1 });
            $(".select2-container").css("width", "100%");
        };

        this.save = function() {
            if (!$form.valid()) {
                return false;
            }
            var param = {
                Type: $("#Type").val(),
                ParamIds: _args.Id,
                Value: $("#Result").val()
            };

            _modalManager.setBusy(true);
            service.batchSwitchs(param).done(function(response) {
                _modalManager.close();
                _modalManager.setResult(response);
                abp.notify.info(app.localize("SavedSuccessfully"));
            }).always(function() {
                _modalManager.setBusy(false);
            });
        };
    };
}());